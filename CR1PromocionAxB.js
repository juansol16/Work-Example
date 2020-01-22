/**
*   FORMULARIO:      CR1PromocionAxB.aspx
*   AUTOR:           Juan Sol
*   FECHA:           18.06.2019
*   COMENTARIOS:     Creación del archivo.

*   AUTOR:           Mario Lopez
*   FECHA:           09.07.2019
*   COMENTARIOS:     Agregado el eliminar, el crear y los collapse

    AUTOR:           Diego Hernandez
*   FECHA:           22.11.2019
*   COMENTARIOS:     Agregado de validaciones
**/

(function ($) {
    'use strict';

    $.getScript("../js/plugins/lolibox/js/lobibox.js"); //se incluye el js para la notificacion tooltip.
    $.getScript("../vendors/jszip/jszip.js"); //se incluye el js para la comprension del archivo excel
    $.getScript("../vendors/xlsx/xlsx.js"); //se incluye la lectura del archivo xml
    $.getScript("../vendors/xlsx/xlsx.full.min.js"); //se incluye la lectura del archivo xml


    var oFileIn = document.getElementById('inputExcel').value = null; //variable que almanecera el archivo excel que cargue el usuario
    var oJson; //Objeto json que contiene la lista importada del excel para traducir
    var oFilasExcel; //Objeto con las Filas del excel
    var sFilename; //Variable que almacena el nombre del archivo excel a importar
    var wrapperRutaArchivo = document.getElementById("WrapperRutaArchivo"); //Wrapper de la ruta del archivo excel que selecciona el usuario
    var sFileExt;
    var bCargado = false;

    $(function () {

       InicializamosReloj();

        $(".modal").modal({
            opacity: .5, // Opacity of modal background
            inDuration: 400, // Transition in duration
            outDuration: 300
        });
        cargarAutocomplete();
        if(!$('#cabeceraPromociones').hasClass('active')){
            $('#cabeceraPromociones').click();
        }

        // Obtenemos los valores de las variables de sesión
        if(sessionStorage.getItem('PromocionAxB') != null && sessionStorage.getItem('PromocionAxB') != '')
        {
            var vParametros = "codigo:'" + sessionStorage.getItem('PromocionAxB') + "', nombre:''";
            Consultar(vParametros);
            sessionStorage.setItem("PromocionAxB", null);
        }else{
            var vParametros = "codigo:'@@@@@@', nombre:''";
            Consultar(vParametros);
        }

        //Aquí asignamos los eventos a las entradas
        var Codigo = document.getElementById('txtCodigo');
        Codigo.addEventListener('keypress', function (event) {
            if (event.keyCode == 13) {
                $("#txtNombre").val("");
                //$("#lblNombre").addClass("active");
                var vParametros = "codigo:'" + $("#txtCodigo").val() + "', nombre:''";
                Consultar(vParametros);
            }
        });

        var txtPzsTotales = document.getElementById('txtPzsTotales');
        txtPzsTotales.addEventListener('focusout', function () {
            var numero=0;
            if(convertirNumero(txtPzsTotales.value)>0)
            {
                numero = formatoNumero(txtPzsTotales.value,"Cantidad");
                txtPzsTotales.value = numero ? numero : formatoNumero(0,"Cantidad");
            }
            else
            {
                txtPzsTotales.value = numero ? numero : formatoNumero(0,"Cantidad");
            }
        });

        var txtPzsPagar = document.getElementById('txtPzsPagar');
        txtPzsPagar.addEventListener('focusout', function () {
            var numero=0;
            if(convertirNumero(txtPzsPagar.value)>0)
            {
                numero = formatoNumero(txtPzsPagar.value,"Cantidad");
                txtPzsPagar.value = numero ? numero : formatoNumero(0,"Cantidad");

            }
            else
            {
                txtPzsPagar.value = numero ? numero : formatoNumero(0,"Cantidad");
            }
        });

        var txtLimVenta = document.getElementById('txtLimVenta');
        txtLimVenta.addEventListener('focusout', function () {
            var numero=0;
            if(Number.isInteger(convertirNumero(txtLimVenta.value)) &&  convertirNumero(txtLimVenta.value)>0)
            {
                numero = formatoNumero(txtLimVenta.value,"Cantidad");
                txtLimVenta.value = numero ? numero : formatoNumero(0,"Cantidad");

            }
            else
            {
                txtLimVenta.value = numero ? numero : formatoNumero(0,"Cantidad");
            }
        });

        var txtPzsLimProm = document.getElementById('txtPzsLimProm');
        txtPzsLimProm.addEventListener('focusout', function () {
            var numero=0;
            if(Number.isInteger(convertirNumero(txtPzsLimProm.value)) && convertirNumero(txtPzsLimProm.value)>0)
            {
                numero = formatoNumero(txtPzsLimProm.value,"Cantidad");
                txtPzsLimProm.value = numero ? numero : formatoNumero(0,"Cantidad");
            }
            else
            {
                txtPzsLimProm.value = numero ? numero : formatoNumero(0,"Cantidad");
            } 
        });

        var txtVenLimProm = document.getElementById('txtVenLimProm');
        txtVenLimProm.addEventListener('focusout', function () {
            var numero=0;
            if(Number.isInteger(convertirNumero(txtVenLimProm.value)) && convertirNumero(txtVenLimProm.value)>0)
            {
                numero = formatoNumero(txtVenLimProm.value,"Cantidad");
                txtVenLimProm.value = numero ? numero : formatoNumero(0,"Cantidad");
            }
            else
            {
                txtVenLimProm.value = numero ? numero : formatoNumero(0,"Cantidad");
            }
        });

        //

        oFileIn = document.getElementById('inputExcel');
        if (oFileIn.addEventListener) {
            oFileIn.addEventListener('change', filePicked, false); 
        }

        /*var Nombre = document.getElementById('txtNombre');
        Codigo.addEventListener('keypress', function (event) {
            if (event.keyCode == 13) {
                $("#txtCodigo").val("");
                //$("#lblCodigo").addClass("active");
                var vParametros = "codigo:'', nombre:'" + $("#txtNombre").val() + "'";
                Consultar(vParametros);
            }
        });*/

    });

    /* ------Variables Globales-----*/
    var listaArticuloCodigos = [];
    var listaArticuloNombres = [];
    var listaArticulos = [];

    var listaClientesCodigos = [];
    var listaClientesNombres = [];
    var listaClientes = [];

    var listaProveedoresCodigos = [];
    var listaProveedoresNombres = [];
    var listaProveedores = [];

   

    let ObjetoPromocion = {};
    let PromoOriginal = {};
    let itemBorrar = {};
    let noPreguntar = false;
    let tipoItem = '';
    /*-----------------------*/

    /*---------Botones--------*/
    $("#btnCrear").click(function () {
       
        if(LeerDatos())
        {
            //ArmarDictionary();
            //ActualizarPromocion(ObjetoPromocion,PromoOriginal);
            ActualizarPromocion(ObjetoPromocion);
        }    
        loadTranslated(sessionStorage.getItem("lang"));
    });

    $("#btnBuscar").click(function () {
        var vParametros = "codigo:'" + $("#txtCodigo").val() + "', nombre:''";
        Consultar(vParametros);
    });

    $("#btnCancelar").click(function () {
        window.location = '/CR1PromocionesRegistradas.aspx';
    });

    $("#btn_LimpiarFechas").click(function () {
		$("#txtFechaInicio").val("");
        $("#txtFechaFin").val("");
       loadFechas(sessionStorage.getItem("lang"));
    });

    $("#btn_agregar_cliente").click(function () {
        loadingActive(true);

        var bValido = true;

        if ($("#txtClienteCodigo").val() == null || !($("#txtClienteCodigo").val()) || $("#txtClienteNombre").val() == null || !($("#txtClienteNombre").val())) {
            Notificacion('info', '<span data-translate="cr1axb132">Seleccione el codigo o nombre del cliente.');
        }
        else{ 
            if(checaRelacionCliente() && ChecaClienteCargado($("#txtClienteCodigo").val())){
                $("#jsClientes").jsGrid("insertItem",  { U_SO1_CLIENTE: $("#txtClienteCodigo").val(), Nombre: $("#txtClienteNombre").val(), Seleccionado: "<input type='checkbox' class='filled-in' id='test' />" }).done(function() {
                    $("#txtClienteCodigo").val("");
                    $("#txtClienteNombre").val("");
                });
            }
        }
        loadTranslated(sessionStorage.getItem("lang"));
        loadingActive(false);

    });

    $("#btn_agregar_articulo").click(function () {
        loadingActive(true);

        var bValido = true;

        if ($("#txtArticuloCodigo").val() == null || !($("#txtArticuloCodigo").val()) || $("#txtArticuloNombre").val() == null || !($("#txtArticuloNombre").val())) {
            Notificacion('info', '<span data-translate="cr1axb133">Seleccione el codigo o nombre del articulo.');
        }
        else{ 
            if(checaRelacionArticulo() && checaArticuloCargado($("#txtArticuloCodigo").val())){
                $("#jsArticulos").jsGrid("insertItem",  { U_SO1_ARTICULO: $("#txtArticuloCodigo").val(), Nombre: $("#txtArticuloNombre").val(), Seleccionado: "<input type='checkbox' />" }).done(function() {
                    $("#txtArticuloCodigo").val("");
                    $("#txtArticuloNombre").val("");
                });
            }
        }
        loadTranslated(sessionStorage.getItem("lang"));
        loadingActive(false);

    });

    $("#btn_agregar_proveedor").click(function () {
        loadingActive(true);

        var bValido = true;

        if ($("#txtCodigoProveedor").val() == null || !($("#txtCodigoProveedor").val()) || $("#txtNombreProveedor").val() == null || !($("#txtNombreProveedor").val())) {
            Notificacion('info', '<span data-translate="cr1axb134">Seleccione el codigo o nombre del proveedor.');
        }
        else{ 
            if(checaRelacionProveedor() && checaProveedorCargado($("#txtCodigoProveedor").val())){
                $("#jsProveedores").jsGrid("insertItem",  { U_SO1_PROVEEDOR: $("#txtCodigoProveedor").val(), Nombre: $("#txtNombreProveedor").val() }).done(function() {
                    $("#txtCodigoProveedor").val("");
                    $("#txtNombreProveedor").val("");
                });
            }
          
        }
        loadTranslated(sessionStorage.getItem("lang"));
        loadingActive(false);

    });

    $("#btn_agregar_excepcion").click(function () {
        loadingActive(true);

        var bValido = true;

        if ($("#txtExcepArtCodigo").val() == null || !($("#txtExcepArtCodigo").val()) || $("#txtExcepArtNombre").val() == null || !($("#txtExcepArtNombre").val())) {
            Notificacion('info', '<span data-translate="cr1axb133">Seleccione el codigo o nombre del articulo.');
        }
        else{ 
            if(checaRelacionExcepcion() && checaExcepcionCargada($("#txtExcepArtCodigo").val())){
                $("#jsExcepciones").jsGrid("insertItem",  { U_SO1_ARTICULO: $("#txtExcepArtCodigo").val(), Nombre: $("#txtExcepArtNombre").val(), Seleccionado: "<input type='checkbox' />" }).done(function() {
                    $("#txtExcepArtCodigo").val("");
                    $("#txtExcepArtNombre").val("");
                });
            }
        }
        loadTranslated(sessionStorage.getItem("lang"));
        loadingActive(false);
    });

    $('#cabeceraFiltros').click(function () {

            var sTexto = $('#iconoCabecera').html();

            if (sTexto !== 'add') {
                $('#iconoCabecera').html('add');
            }
            else {
                $('#iconoCabecera').html('remove');
            }
    });

    $('#cabeceraPromociones').click(function () {

            var sTexto = $('#iconoPromociones').html();

            if (sTexto !== 'add') {
                $('#iconoPromociones').html('add');
            }
            else {
                $('#iconoPromociones').html('remove');
            }
    });

    $("#btnAñadir").click(function () {
        $("#mdlConfirmacion").modal('close');
        Importar(oJson, true);    
    });

    $("#btnReemplazar").click(function () {
        $("#mdlConfirmacion").modal('close');
        Importar(oJson, false);
        
    });

    $("#btn_CancelarEliminar").click(function () {
        itemBorrar = {};
        $("#mdlConfirmarEliminar").modal('close');
    });

    $("#btn_ConfirmarEliminar").click(function () {
        switch(tipoItem){
            case "Articulo":
                $("#jsArticulos").jsGrid("deleteItem", itemBorrar);
            break;
            case "Excepcion":
                $("#jsExcepciones").jsGrid("deleteItem", itemBorrar);
            break;
            case "Proveedor":
                $("#jsProveedores").jsGrid("deleteItem", itemBorrar);
            break;
            case "Cliente":
                $("#jsClientes").jsGrid("deleteItem", itemBorrar);
            break;
            default:
            break;
        }
        tipoItem = '';
        $("#mdlConfirmarEliminar").modal('close');
    });

    $('#chkNoPreguntar').change(function() {
        noPreguntar = $('#chkNoPreguntar').prop('checked');
    });

    /*-------------------------*/

    function LeerDatos()
    {
        /* Codigo, nombre y fechas */
        if($("#txtCodigo").val() == "")
        {
            $("#txtCodigo").focus();
            Notificacion('info', '<span data-translate="cr1axb82">Ingrese un codigo de promoción.</span>');
            return false;
        }
        if($("#txtNombre").val() == "")
        {
            $("#txtNombre").focus();
            Notificacion('info', '<span data-translate="cr1axb83">Ingrese el nombre de la promoción </span>');
            return false;
        }
        ObjetoPromocion.Codigo = $("#txtCodigo").val();
        ObjetoPromocion.Nombre = $("#txtNombre").val();

        if($("#txtFechaInicio").val() == "" || $("#txtFechaFin").val() == "")
        {
            $("#txtFechaInicio").focus();
            Notificacion('info', '<span data-translate="cr1axb84">Ingrese un rango de fechas.</span>');
            return false;
        }
        ObjetoPromocion.U_SO1_FECHADESDE = $("#txtFechaInicio").val();
        ObjetoPromocion.U_SO1_FECHAHASTA = $("#txtFechaFin").val();

        /* Datos de Promocion */
        if(convertirNumero($("#txtPzsTotales").val()) <= 0  || $("#txtPzsTotales").val() == "")
        {
            $("#txtPzsTotales").focus();
            Notificacion('info', '<span data-translate="cr1axb85">Ingrese piezas totales mayores a cero</span>');
            return false;
        }

        if(convertirNumero($("#txtPzsPagar").val()) <= 0  || $("#txtPzsPagar").val() == "")
        {
            $("#txtPzsPagar").focus();
            Notificacion('info', '<span data-translate="cr1axb86">Ingrese piezas a pagar mayores a cero</span>');
            return false;
        }
        
        if(convertirNumero($("#txtPzsTotales").val()) < convertirNumero($("#txtPzsPagar").val()))
        {
            $("#txtPzsPagar").focus();
            Notificacion('info', '<span data-translate="cr1axb87">Las piezas a pagar deben ser menor a las piezas totales</span>');
            return false;
        }

        ObjetoPromocion.PromocionAxB.U_SO1_PIEZASTOTALES = convertirNumero($("#txtPzsTotales").val());
        ObjetoPromocion.PromocionAxB.U_SO1_PIEZASAPAGAR = convertirNumero($("#txtPzsPagar").val());
        ObjetoPromocion.PromocionAxB.U_SO1_REGALOPRECIO1 = $('#chkAgregarRegalo').is(":checked") ? "Y" : "N";
        ObjetoPromocion.PromocionAxB.U_SO1_MANTENERDESC = $('#chkMantProm').is(":checked") ? "Y" : "N";

        /* Checks de Vigencia */
        //Check todos los días
        if(!$('#chkDiario').is(":checked") && !$('#chkLunes').is(":checked") && !$('#chkMartes').is(":checked") && !$('#chkMiercoles').is(":checked") && !$('#chkJueves').is(":checked") && !$('#chkViernes').is(":checked") && !$('#chkSabado').is(":checked") && !$('#chkDomingo').is(":checked"))
        {
            Notificacion('info', '<span data-translate="cr1axb166">Ingrese al menos un día para hacer válida la promoción</span>');
            return false;
        }

        //Todos los días
        ObjetoPromocion.U_SO1_DIARIO = $('#chkDiario').is(":checked") ? "Y" : "N";
        ObjetoPromocion.U_SO1_DIACOMPLETO = $('#chkDiarioTodo').is(":checked") ? "Y" : "N";
        //check diario
        if($("#DesdeDiario").val() == "00:00" || $("#HastaDiario").val() == "00:00" )
        {
            if($('#chkDiario').is(":checked")) 
            {
                if(!$('#chkDiarioTodo').is(":checked")) 
                {
                    Notificacion('info', '<span data-translate="cr1axb88">Ingrese un rango de horas</span>');
                    return false;
                }
            }
        }
        ObjetoPromocion.U_SO1_DIAHORAINI = obtieneHora($("#DesdeDiario").val());
        ObjetoPromocion.U_SO1_DIAHORAFIN = obtieneHora($("#HastaDiario").val());

        //Lunes
        ObjetoPromocion.U_SO1_LUNES = $('#chkLunes').is(":checked") ? "Y" : "N";
        ObjetoPromocion.U_SO1_LUNCOMPLETO = $('#chkLunesTodo').is(":checked") ? "Y" : "N";

        if($("#DesdeLunes").val() == "00:00" || $("#HastaLunes").val() == "00:00" )
        {
            if($('#chkLunes').is(":checked")) 
            {
                if(!$('#chkLunesTodo').is(":checked")) 
                {
                    Notificacion('info', '<span data-translate="cr1axb88">Ingrese un rango de horas</span>');
                    return false;
                }
            }
        }

        ObjetoPromocion.U_SO1_LUNHORAINI = obtieneHora($("#DesdeLunes").val());
        ObjetoPromocion.U_SO1_LUNHORAFIN = obtieneHora($("#HastaLunes").val());

        //Martes
        ObjetoPromocion.U_SO1_MARTES = $('#chkMartes').is(":checked") ? "Y" : "N";
        ObjetoPromocion.U_SO1_MARCOMPLETO = $('#chkMartesTodo').is(":checked") ? "Y" : "N";
        if($("#DesdeMartes").val() == "00:00" || $("#HastaMartes").val() == "00:00" )
        {
            if($('#chkMartes').is(":checked")) 
            {
                if(!$('#chkMartesTodo').is(":checked"))
                {
                    Notificacion('info', '<span data-translate="cr1axb88">Ingrese un rango de horas</span>');
                    return false;
                }
            }
        }

        ObjetoPromocion.U_SO1_MARHORAINI = obtieneHora($("#DesdeMartes").val());
        ObjetoPromocion.U_SO1_MARHORAFIN = obtieneHora($("#HastaMartes").val());

        //Miercoles
        ObjetoPromocion.U_SO1_MIERCOLES = $('#chkMiercoles').is(":checked") ? "Y" : "N";
        ObjetoPromocion.U_SO1_MIECOMPLETO = $('#chkMiercolesTodo').is(":checked") ? "Y" : "N";
        if ($("#DesdeMiercoles").val() == "00:00" || $("#HastaMiercoles").val() == "00:00" )
        {
            if($('#chkMiercoles').is(":checked"))
            {
                if (!$('#chkMiercolesTodo').is(":checked")) 
                {
                    Notificacion('info', '<span data-translate="cr1axb88">Ingrese un rango de horas</span>');
                    return false;
                }
            }
        }

        ObjetoPromocion.U_SO1_MIEHORAINI = obtieneHora($("#DesdeMiercoles").val());
        ObjetoPromocion.U_SO1_MIEHORAFIN = obtieneHora($("#HastaMiercoles").val());

        //Jueves
        ObjetoPromocion.U_SO1_JUEVES = $('#chkJueves').is(":checked") ? "Y" : "N";
        ObjetoPromocion.U_SO1_JUECOMPLETO = $('#chkJuevesTodo').is(":checked") ? "Y" : "N";
        if ( $("#DesdeJueves").val() == "00:00" || $("#HastaJueves").val() == "00:00" )
        {
            if($('#chkJueves').is(":checked"))
            {
                if (!$('#chkJuevesTodo').is(":checked"))
                {
                    Notificacion('info', '<span data-translate="cr1axb88">Ingrese un rango de horas</span>');
                    return false;
                }
            }
        }

        ObjetoPromocion.U_SO1_JUEHORAINI = obtieneHora($("#DesdeJueves").val());
        ObjetoPromocion.U_SO1_JUEHORAFIN = obtieneHora($("#HastaJueves").val());

        //Viernes
        ObjetoPromocion.U_SO1_VIERNES = $('#chkViernes').is(":checked") ? "Y" : "N";
        ObjetoPromocion.U_SO1_VIECOMPLETO = $('#chkViernesTodo').is(":checked") ? "Y" : "N";
        if ($("#DesdeViernes").val() == "00:00" || $("#HastaViernes").val() == "00:00" )
        {
            if ($('#chkViernes').is(":checked")) 
            {
                if (!$('#chkViernesTodo').is(":checked"))
                 {
                    Notificacion('info', '<span data-translate="cr1axb88">Ingrese un rango de horas</span>');
                    return false;
                }
            }
        }

        ObjetoPromocion.U_SO1_VIEHORAINI = obtieneHora($("#DesdeViernes").val());
        ObjetoPromocion.U_SO1_VIEHORAFIN = obtieneHora($("#HastaViernes").val());

        //Sábado
        ObjetoPromocion.U_SO1_SABADO = $('#chkSabado').is(":checked") ? "Y" : "N";
        ObjetoPromocion.U_SO1_SABCOMPLETO = $('#chkSabadoTodo').is(":checked") ? "Y" : "N";
        if ($("#DesdeSabado").val() == "00:00" || $("#HastaSabado").val() == "00:00") 
        {
            if ($('#chkSabado').is(":checked"))
             {
                if (!$('#chkSabadoTodo').is(":checked"))
                {

                    Notificacion('info', '<span data-translate="cr1axb88">Ingrese un rango de horas</span>');
                    return false;
                }
            }
        }

        ObjetoPromocion.U_SO1_SABHORAINI = obtieneHora($("#DesdeSabado").val());
        ObjetoPromocion.U_SO1_SABHORAFIN = obtieneHora($("#HastaSabado").val());

        //Domingo
        ObjetoPromocion.U_SO1_DOMINGO = $('#chkDomingo').is(":checked") ? "Y" : "N";
        ObjetoPromocion.U_SO1_DOMCOMPLETO = $('#chkDomingoTodo').is(":checked") ? "Y" : "N";

        if ($("#DesdeDomingo").val() == "00:00" || $("#HastaDomingo").val() == "00:00") 
        {
            if ($('#chkDomingo').is(":checked"))
             {
                if (!$('#chkDomingoTodo').is(":checked"))
                 {
                    Notificacion('info', '<span data-translate="cr1axb88">Ingrese un rango de horas</span>');
                    return false;
                }
            }
        }

        ObjetoPromocion.U_SO1_DOMHORAINI = obtieneHora($("#DesdeDomingo").val());
        ObjetoPromocion.U_SO1_DOMHORAFIN = obtieneHora($("#HastaDomingo").val());

        /* Checks de General */

        ObjetoPromocion.U_SO1_ACUMPROMORM = $('#chkAcumMonto').is(":checked") ? "Y" : "N";
        ObjetoPromocion.U_SO1_ACUMPROMOPUNTO = $('#chkAcumPuntos').is(":checked") ? "Y" : "N";
        ObjetoPromocion.U_SO1_ACUMPROMOOTRAS = $('#chkAcumProm').is(":checked") ? "Y" : "N";
        ObjetoPromocion.U_SO1_ACTLIMPIEZAVEN = $('#chkLimVenta').is(":checked") ? "Y" : "N";
        if(convertirNumero($("#txtLimVenta").val()) <= 0  || $("#txtLimVenta").val() == "")
        {
            if( $('#chkLimVenta').is(":checked"))
            {
                $("#txtLimVenta").focus();
                Notificacion('info', '<span data-translate="cr1axb89">Ingrese un limite de aplicaciones</span>');
                return false;
            }
        }
        ObjetoPromocion.U_SO1_LIMITEPIEZAVEN = convertirNumero($("#txtLimVenta").val());
        ObjetoPromocion.U_SO1_ACTLIMPIEZAPRO = $('#chkPzsLimProm').is(":checked") ? "Y" : "N";
        if(convertirNumero($("#txtPzsLimProm").val()) <= 0  || $("#txtPzsLimProm").val() == "")
        {
            if( $('#chkPzsLimProm').is(":checked"))
            {   
                $("#txtPzsLimProm").focus();
                Notificacion('info', '<span data-translate="cr1axb90">Ingrese un limite de piezas</span>');
                return false;
            }
        }
        ObjetoPromocion.U_SO1_LIMITEPIEZAPRO = convertirNumero($("#txtPzsLimProm").val());
        ObjetoPromocion.U_SO1_ACTLIMVENTAPRO = $('#chkVenLimProm').is(":checked") ? "Y" : "N";
        if(convertirNumero($("#txtVenLimProm").val()) <= 0  || $("#txtVenLimProm").val() == "")
        {
            if( $('#chkVenLimProm').is(":checked"))
            {   
                $("#txtVenLimProm").focus();
                Notificacion('info', '<span data-translate="cr1axb91">Ingrese un limite de ventas</span>');
                return false;
            }
        }
        ObjetoPromocion.U_SO1_LIMITEVENTAPRO = convertirNumero($("#txtVenLimProm").val());

        /* ----------------------------------------------------------------------------------------------------------------- */

        /* Check de los Articulos */
        ObjetoPromocion.U_SO1_ARTICULOENLACE = $('#chkY').is(":checked") ? "Y" : "O";
        ObjetoPromocion.U_SO1_FILTRARARTART = $('#chkActivarArticulo').is(":checked") ? "Y" : "N";
        /* Articulo Grupo */
        ObjetoPromocion.U_SO1_FILTRARARTGRU = $('#chkActivarGrupo').is(":checked") ? "Y" : "N";
        /* Articulo Propiedades */
        ObjetoPromocion.U_SO1_FILTRARARTPRO = $('#chkActivarPropiedades').is(":checked") ? "Y" : "N";
        ObjetoPromocion.U_SO1_FILTRARARTPROC = $('#chkCEPropiedadesArticulo').is(":checked") ? "Y" : "N";
        ObjetoPromocion.U_SO1_FILTRARARTPROE = $('#relacionYArticulo').is(":checked") ? "Y" : "O";

        /* Articulo Otros */
        ObjetoPromocion.U_SO1_FILTRARARTCOE = $('#chkActivarCondicionEspecial').is(":checked") ? "Y" : "N";
        ObjetoPromocion.U_SO1_FILTRARARTCOEC = $("#txtCampo").val();
        ObjetoPromocion.U_SO1_FILTRARARTCOEV = $("#txtValor").val();
        if(($("#txtCampo").val() == "" || $("#txtValor").val()=="" ) && $('#chkActivarCondicionEspecial').is(":checked"))
        {
            $("#txtCampo").focus();
            Notificacion('info', '<span data-translate="cr1axb92">Ingrese una condición especial</span>');
            return false;
        }

        ObjetoPromocion.U_SO1_FILTRARARTCON = $('#chkActivarConsultaEspecial').is(":checked") ? "Y" : "N";
        ObjetoPromocion.U_SO1_FILTRARARTCONV = $("#txtConsulta").val();
        if($("#txtConsulta").val() == "" && $('#chkActivarConsultaEspecial').is(":checked"))
        {
            $("#txtConsulta").focus();
            Notificacion('info', '<span data-translate="cr1axb93">Ingrese una consulta especial</span>');
            return false;
        }
        ObjetoPromocion.U_SO1_FILTRARARTPROV = $('#chkActivarProveedor').is(":checked") ? "Y" : "N";
        ObjetoPromocion.U_SO1_FILTRARARTFAB = $('#chkActivarFabricante').is(":checked") ? "Y" : "N";
        /* Articulo excepciones */
        ObjetoPromocion.U_SO1_FILTRARARTEXC = $('#chkActivarExcepciones').is(":checked") ? "Y" : "N";
        /* Articulo Unidades medida */
        ObjetoPromocion.U_SO1_FILTRARARTUNE = $('#chkActivarUnidades').is(":checked") ? "Y" : "N";

        /* Check de los clientes */
        ObjetoPromocion.U_SO1_FILTRARPROMCLI = $('#chkClientes').is(":checked") ? "Y" : "N";
        ObjetoPromocion.U_SO1_FILTRARCLIENLS = $('#CondicionClienteY').is(":checked") ? "Y" : "O";
        ObjetoPromocion.U_SO1_FILTRARCLICLI = $('#chkActivarClientes').is(":checked") ? "Y" : "N";
        ObjetoPromocion.U_SO1_FILTRARCLICOE = $('#chkActivarClientesCE').is(":checked") ? "Y" : "N";
       
        if($('#chkActivarClientesCE').is(":checked") && $('#chkClientes').is(":checked") && 
        $('#chkActivarClientesCE').attr('disabled')!="disabled" && $("#txtClienteCampo").val()!="" && $("#txtClienteValor").val()!="")
        {
            ObjetoPromocion.U_SO1_FILTRARCLICOEC = $("#txtClienteCampo").val();
            ObjetoPromocion.U_SO1_FILTRARCLICOEV = $("#txtClienteValor").val();
        }
        else
        {
            ObjetoPromocion.U_SO1_FILTRARCLICOEC = "";
            ObjetoPromocion.U_SO1_FILTRARCLICOEV = "";
        }
        if(($("#txtClienteCampo").val() == "" || $("#txtClienteValor").val()=="") && $('#chkActivarClientesCE').prop("checked") && $('#chkActivarClientesCE').attr('disabled')!="disabled")
        {
            $("#txtClienteCampo").focus();
            Notificacion('info', '<span data-translate="cr1axb94">Ingrese una condición especial</span>');
            return false;
        }
        if($("#chkClientes").prop("checked")==false && $('#chkActivarClientesCE').attr('disabled')=="disabled" && $("#chkActivarClientesCE").prop("checked"))
        {
            $("#chkActivarClientesCE").prop("checked",false);
            ObjetoPromocion.U_SO1_FILTRARCLICOE = $('#chkActivarClientesCE').is(":checked") ? "Y" : "N";
        }

        ObjetoPromocion.U_SO1_FILTRARCLIGRU = $('#chkActivarGrupoClientes').is(":checked") ? "Y" : "N";
        ObjetoPromocion.U_SO1_FILTRARCLIPRO = $('#chkActivarPropiedadesClientes').is(":checked") ? "Y" : "N";
        ObjetoPromocion.U_SO1_FILTRARCLIPROE = $('#PropiedadClienteY').is(":checked") ? "Y" : "O";
        ObjetoPromocion.U_SO1_FILTRARCLIPROC = $('#chkClienteCE').is(":checked") ? "Y" : "N";

        /* Check de los Tabs */
        ObjetoPromocion.U_SO1_FILTRARPROMSUC = $('#chkFiltrarSucursales').is(":checked") ? "Y" : "N";
        ObjetoPromocion.U_SO1_FILTRARPROMALI = $('#chkFiltrarAlianzas').is(":checked") ? "Y" : "N";
        ObjetoPromocion.U_SO1_FILTRARPROMMEM = $('#chkFiltrarMembresias').is(":checked") ? "Y" : "N";
        ObjetoPromocion.U_SO1_FILTRARPROMLIS = $('#chkFiltrarPrecios').is(":checked") ? "Y" : "N";
        ObjetoPromocion.U_SO1_FILTRARPROMFOP = $('#chkFiltrarFormaPago').is(":checked") ? "Y" : "N";
        ObjetoPromocion.U_SO1_ACTVENTACREDIT = $('#chkFiltrarVentaCredito').is(":checked") ? "Y" : "N";
       
        var articulos = "";
        var articulosGrupos = "";
        var articulosPropiedades = "";
        var proveedores = "";
        var fabricantes = "";
        var excepciones = "";
        var medidas = "";
        var sucursales = "";
        var alianzas = "";
        var clientes = "";
        var clientesGrupos = "";
        var clientesPropiedades = "";
        var membresias = "";
        var precios = "";
        var formasPago = "";

        //Grid articulo
        if($("#chkActivarArticulo").prop("checked"))
        {
            articulos = $("#jsArticulos").jsGrid("option", "data");
        }
        else
        {
            articulos="";
        }
        //Grid grupo
        articulosGrupos = $("#jsArticulosGrupos").jsGrid("option", "data");
        //Grid propiedades
        articulosPropiedades = $("#jsArticulosPropiedades").jsGrid("option", "data");
        //Grid proveedores
        if($("#chkActivarProveedor").prop("checked"))
        {
            proveedores = $("#jsProveedores").jsGrid("option", "data");
        }
        else
        {
            proveedores="";
        }
        //Grid fabricantes
        fabricantes = $("#jsFabricantes").jsGrid("option", "data");
        //Grid exepciones
        if($("#chkActivarExcepciones").prop("checked"))
        {
            excepciones = $("#jsExcepciones").jsGrid("option", "data");
        }
        else
        {
            excepciones="";
        }
        //Grid unidades de medida
        medidas = $("#jsArticulosUnidades").jsGrid("option", "data");
        //Grid sucursales
        if($("#chkFiltrarSucursales").prop("checked"))
        {
            sucursales = $("#jsSucursales").jsGrid("option", "data");
        }
        //Grid alianzas
        alianzas = $("#jsAlianzas").jsGrid("option", "data");
        //Grid clientes
        if($("#chkClientes").prop("checked") && $('#chkActivarClientes').attr('disabled')!="disabled" && $("#chkActivarClientes").prop("checked"))
        {        
            clientes = $("#jsClientes").jsGrid("option", "data");
        }
        else
        {
            if(!$("#chkClientes").prop("checked") && $('#chkActivarClientes').attr('disabled')=="disabled" && $("#chkActivarClientes").prop("checked"))
            {
          
                clientes = "";
                $('#chkActivarClientes').prop("checked",false);
                ObjetoPromocion.U_SO1_FILTRARCLICLI = $('#chkActivarClientes').is(":checked") ? "Y" : "N";

            }
            
        }
        //Grid cliente grupo
        if($("#chkClientes").prop("checked") && $('#chkActivarGrupoClientes').attr('disabled')!="disabled" && $("#chkActivarGrupoClientes").prop("checked"))
        {          
            clientesGrupos = $("#jsGruposClientes").jsGrid("option", "data");
        }
        else
        {
            if($("#chkClientes").prop("checked")==false && $('#chkActivarGrupoClientes').attr('disabled')=="disabled" && $("#chkActivarGrupoClientes").prop("checked"))
            {
                $('#chkActivarGrupoClientes').prop("checked",false);
                ObjetoPromocion.U_SO1_FILTRARCLIGRU = $('#chkActivarGrupoClientes').is(":checked") ? "Y" : "N";
                Seleccion("N", "jsGruposClientes");
                clientesGrupos = $("#jsGruposClientes").jsGrid("option", "data");
            }
        }
        //Grid clientes propiedades
        if( $("#chkClientes").prop("checked") && $('#chkActivarPropiedadesClientes').attr('disabled')!="disabled" && $("#chkActivarPropiedadesClientes").prop("checked"))
        {
            clientesPropiedades = $("#jsPropiedadesClientes").jsGrid("option", "data");
        }
        else
        {
            if($("#chkClientes").prop("checked")==false && $('#chkActivarPropiedadesClientes').attr('disabled')=="disabled" && $("#chkActivarPropiedadesClientes").prop("checked"))
            {
                $('#chkActivarPropiedadesClientes').prop("checked",false);
                $('#chkClienteCE').prop("checked",false);
                ObjetoPromocion.U_SO1_FILTRARCLIPRO = $('#chkActivarPropiedadesClientes').is(":checked") ? "Y" : "N";
                ObjetoPromocion.U_SO1_FILTRARCLIPROC = $('#chkClienteCE').is(":checked") ? "Y" : "N";

                Seleccion("N", "jsPropiedadesClientes");
                clientesPropiedades = $("#jsPropiedadesClientes").jsGrid("option", "data");
            }
        }
        //Grid membresias
        if($("#chkFiltrarMembresias").prop("checked"))
        {
            membresias = $("#jsMembresias").jsGrid("option", "data");
        }
        //Grid precios
        if($("#chkFiltrarPrecios").prop("checked"))
        {
           
            precios = $("#jsPrecios").jsGrid("option", "data");
        }
        //Grid formas de pago
        if($("#chkFiltrarFormaPago").prop("checked"))
        {
           
            formasPago = $("#jsFormasPago").jsGrid("option", "data");
        }



        /* Convertimos los arreglos en Dictionaries */
        // Articulos
        var Articulos = {};
        var validaArticulo = false;
        for (var i=0;i<articulos.length;i++){
            Articulos[articulos[i].U_SO1_ARTICULO] = articulos[i];
            validaArticulo = true;
        }
        ObjetoPromocion.Articulos = Articulos;
        if($('#chkActivarArticulo').is(":checked") && !validaArticulo)
        {
            Notificacion('info', '<span data-translate="cr1axb95">Agregue al menos un articulo</span>');
            return false;
        }

        // Grupos de Articulos
        var GrupoArticulos = {};
        var validaGArticulo = false;
        for (var i=0;i<articulosGrupos.length;i++){
            GrupoArticulos[articulosGrupos[i].U_SO1_GRUPO] = articulosGrupos[i];
            if(articulosGrupos[i].U_SO1_ACTIVO == "Y")
                validaGArticulo = true;
        }
        if(!$('#chkActivarGrupo').is(":checked") && ObjetoPromocion.Existe==true)
        {
            var obj=ObjetoPromocion.GrupoArticulos;
            for (var i=0;i<obj.length;i++)
            {
                GrupoArticulos[obj[i].U_SO1_GRUPO] = obj[i];
                
            }
            
        }
        ObjetoPromocion.GrupoArticulos = GrupoArticulos;
        if($('#chkActivarGrupo').is(":checked") && !validaGArticulo)
        {
            Notificacion('info', '<span data-translate="cr1axb96">Seleccione al menos un grupo de articulo</span>');
            return false;
        }

        // Propiedades de Articulos
        var PropiedadesArticulos = {};
        var validaPArticulo = false;
        for (var i=0;i<articulosPropiedades.length;i++){
            PropiedadesArticulos[articulosPropiedades[i].U_SO1_PROPIEDAD] = articulosPropiedades[i];
            if(articulosPropiedades[i].U_SO1_ACTIVO == "Y")
                validaPArticulo = true;
        }
        if(!$('#chkActivarPropiedades').is(":checked") && ObjetoPromocion.Existe)
        {
            var obj=ObjetoPromocion.PropiedadesArticulos;
            for (var i=0;i<obj.length;i++)
            {
                PropiedadesArticulos[obj[i].U_SO1_PROPIEDAD] = obj[i];
                
            }  
        }
        ObjetoPromocion.PropiedadesArticulos = PropiedadesArticulos;
        if($('#chkActivarPropiedades').is(":checked") && !validaPArticulo)
        {
            Notificacion('info', '<span data-translate="cr1axb97">Seleccione al menos una propiedad de articulo</span>');
            return false;
        }

        // Proveedores de Articulos
        var ArticuloProveedor = {};
        var validaProArticulo = false;
        for (var i=0;i<proveedores.length;i++){
            ArticuloProveedor[proveedores[i].U_SO1_PROVEEDOR] = proveedores[i];
            validaProArticulo = true;
        }
        ObjetoPromocion.ArticuloProveedor = ArticuloProveedor;
        if($('#chkActivarProveedor').is(":checked") && !validaProArticulo)
        {
            Notificacion('info', '<span data-translate="cr1axb98">Agregue al menos un Proveedor</span>');
            return false;
        }

        // Fabricantes de Articulos
        var ArticuloFabricante = {};
        var validaFAArticulo = false;
        for (var i=0;i<fabricantes.length;i++){
            ArticuloFabricante[fabricantes[i].U_SO1_FABRICANTE] = fabricantes[i];
            if(fabricantes[i].U_SO1_ACTIVO == "Y")
                validaFAArticulo = true;
        }
        if(!$('#chkActivarFabricante').is(":checked") && ObjetoPromocion.Existe==true)
        {
            var obj=ObjetoPromocion.ArticuloFabricante;
            for (var i=0;i<obj.length;i++)
            {
                ArticuloFabricante[obj[i].U_SO1_FABRICANTE] = obj[i];
                
            }
        }
        ObjetoPromocion.ArticuloFabricante = ArticuloFabricante;
        if($('#chkActivarFabricante').is(":checked") && !validaFAArticulo)
        {
            Notificacion('info', '<span data-translate="cr1axb99">Seleccione al menos un fabricante</span>');
            return false;
        }

        // Excepciones
        var Excepciones = {};
        var validaExArticulo = false;
        for (var i=0;i<excepciones.length;i++){
            Excepciones[excepciones[i].U_SO1_ARTICULO] = excepciones[i];
            validaExArticulo = true;
        }
        ObjetoPromocion.Excepciones = Excepciones;
        if($('#chkActivarExcepciones').is(":checked") && !validaExArticulo)
        {
            Notificacion('info', '<span data-translate="cr1axb100">Agregue al menos una excepción</span>');
            return false;
        }

        // Medidas de Articulos
        var MedidasArticulos = {};
        var validaMedArticulo = false;
        for (var i=0;i<medidas.length;i++){
            MedidasArticulos[medidas[i].U_SO1_UNIDAD] = medidas[i];
            if(medidas[i].U_SO1_ACTIVO == "Y")
                validaMedArticulo = true;
        }
        if(!$('#chkActivarUnidades').is(":checked") && ObjetoPromocion.Existe==true)
        {
            var obj=ObjetoPromocion.MedidasArticulos;
            for (var i=0;i<obj.length;i++)
            {
                MedidasArticulos[obj[i].U_SO1_UNIDAD] = obj[i];
                
            }
            
        }
        ObjetoPromocion.MedidasArticulos = MedidasArticulos;
        if($('#chkActivarUnidades').is(":checked") && !validaMedArticulo)
        {
            Notificacion('info', '<span data-translate="cr1axb101">Agregue al menos una medida de articulo</span>');
            return false;
        }

         //Sucursales
        var Sucursales = {};
        var validaSucursal = false;
        for (var i=0;i<sucursales.length;i++){
            Sucursales[sucursales[i].U_SO1_SUCURSAL] = sucursales[i];
            if(sucursales[i].U_SO1_ACTIVO == "Y")
                validaSucursal = true;
        }
        if(!$('#chkFiltrarSucursales').is(":checked") && ObjetoPromocion.Existe==true)
        {
            var obj=ObjetoPromocion.Sucursales;
            for (var i=0;i<obj.length;i++)
            {
                Sucursales[obj[i].U_SO1_SUCURSAL] = obj[i];   
            }
        }
        ObjetoPromocion.Sucursales = Sucursales;
        if($('#chkFiltrarSucursales').is(":checked") && !validaSucursal)
        {
            Notificacion('info', '<span data-translate="cr1axb102">Seleccione al menos una sucursal</span>');
            return false;
        }

        // Alianzas
        var Alianzas = {};
        var validaAlianza = false;
        for (var i=0;i<alianzas.length;i++){
            Alianzas[alianzas[i].U_SO1_ALIANZA] = alianzas[i];
            if(alianzas[i].U_SO1_ACTIVO == "Y")
                validaAlianza = true;
        }
        if(!$('#chkFiltrarAlianzas').is(":checked") && ObjetoPromocion.Existe==true)
        {
            var obj=ObjetoPromocion.Alianzas;
            for (var i=0;i<obj.length;i++)
            {
                Alianzas[obj[i].U_SO1_ALIANZA] = obj[i];   
            }
        }
        ObjetoPromocion.Alianzas = Alianzas;
        if($('#chkFiltrarAlianzas').is(":checked") && !validaAlianza)
        {
            Notificacion('info', '<span data-translate="cr1axb103">Seleccione al menos una alianza</span>');
            return false;
        }

        // Clientes
        var Clientes = {};
        var validaClientes = false;
        for (var i=0;i<clientes.length;i++){
            Clientes[clientes[i].U_SO1_CLIENTE] = clientes[i];
            validaClientes = true;
        }
        ObjetoPromocion.Clientes = Clientes;
        if($('#chkActivarClientes').attr('disabled')==false && $('#chkActivarClientes').is(":checked") && !validaClientes)
        {
            Notificacion('info', '<span data-translate="cr1axb104">Agregue un cliente</span>');
            return false;
        }

        // Grupos de clientes
        var GrupoClientes = {};
        var validaGrupoClientes = false;
        for (var i=0;i<clientesGrupos.length;i++){
            GrupoClientes[clientesGrupos[i].U_SO1_GRUPO] = clientesGrupos[i];
            if(clientesGrupos[i].U_SO1_ACTIVO == "Y")
                validaGrupoClientes = true;
        }
        if(!$('#chkActivarGrupoClientes').is(":checked") && ObjetoPromocion.Existe==true)
        {
            var obj=ObjetoPromocion.GrupoClientes;
            for (var i=0;i<obj.length;i++)
            {
                GrupoClientes[obj[i].U_SO1_GRUPO] = obj[i];   
            }
        }
        ObjetoPromocion.GrupoClientes = GrupoClientes;
        if($('#chkActivarGrupoClientes').attr('disabled')==false && $('#chkActivarGrupoClientes').is(":checked") && !validaGrupoClientes)
        {
            Notificacion('info', '<span data-translate="cr1axb105">Seleccione al menos un grupo de clientes</span>');
            return false;
        }

        // Propiedades de Clientes
        var PropiedadesClientes = {};
        var validaProClientes = false;
        for (var i=0;i<clientesPropiedades.length;i++){
            PropiedadesClientes[clientesPropiedades[i].U_SO1_PROPIEDAD] = clientesPropiedades[i];
            if(clientesPropiedades[i].U_SO1_ACTIVO == "Y")
                validaProClientes = true;
        }
        if(!$('#chkActivarPropiedadesClientes').is(":checked") && ObjetoPromocion.Existe==true)
        {
            var obj=ObjetoPromocion.PropiedadesClientes;
            for (var i=0;i<obj.length;i++)
            {
                PropiedadesClientes[obj[i].U_SO1_PROPIEDAD] = obj[i];   
            }
        }
        ObjetoPromocion.PropiedadesClientes = PropiedadesClientes;
        if($('#chkActivarPropiedadesClientes').attr('disabled')==false && $('#chkActivarPropiedadesClientes').is(":checked") && !validaProClientes)
        {
            Notificacion('info', '<span data-translate="cr1axb106">Seleccione al menos una propiedad de cliente</span>');
            return false;
        }

        // Membresias
        var Membresias = {};
        var validaMembresia = false;
        for (var i=0;i<membresias.length;i++){
            Membresias[membresias[i].U_SO1_TIPOMEMBRESIA] = membresias[i];
            if(membresias[i].U_SO1_ACTIVO == "Y")
                validaMembresia = true;
        }
        if(!$('#chkFiltrarMembresias').is(":checked") && ObjetoPromocion.Existe==true)
        {
            var obj=ObjetoPromocion.Membresias;
            for (var i=0;i<obj.length;i++)
            {
                Membresias[obj[i].U_SO1_TIPOMEMBRESIA] = obj[i];   
            }
        }
        ObjetoPromocion.Membresias = Membresias;
        if($('#chkFiltrarMembresias').is(":checked") && !validaMembresia)
        {
            Notificacion('info', '<span data-translate="cr1axb107">Seleccione al menos una membresia</span>');
            return false;
        }

        // Lista de Precios
        var Precios = {};
        var validaPrecios = false;
        for (var i=0;i<precios.length;i++){
            Precios[precios[i].U_SO1_LISTAPRECIO] = precios[i];
            if(precios[i].U_SO1_ACTIVO == "Y")
                validaPrecios = true;
        }
        if(!$('#chkFiltrarPrecios').is(":checked") && ObjetoPromocion.Existe==true)
        {
            var obj=ObjetoPromocion.Precios;
            for (var i=0;i<obj.length;i++)
            {
                Precios[obj[i].U_SO1_LISTAPRECIO] = obj[i];   
            }
        }
        ObjetoPromocion.Precios = Precios;
         if($('#chkFiltrarPrecios').is(":checked") && !validaPrecios)
        {
            Notificacion('info', '<span data-translate="cr1axb108">Seleccione al menos un precio</span>');
            return false;
        }

        // Formas de Pago
        var FormasPago = {};
        var validaFormaPago = false;
        for (var i=0;i<formasPago.length;i++){

            formasPago[i].U_SO1_MINIMO=convertirNumero(formasPago[i].U_SO1_MINIMO);
            FormasPago[formasPago[i].U_SO1_FORMAPAGO] = formasPago[i];
            if(formasPago[i].U_SO1_ACTIVO == "Y")
                validaFormaPago = true;
            if(formasPago[i].U_SO1_ACTIVO == "Y" && (formasPago[i].U_SO1_MINIMO < 1 || !formasPago[i].U_SO1_TIPO))
            {
                Notificacion('info', '<span data-translate="">Ingrese un tipo y un minimo a las formas de pago</span>');
                return false;
            }
        }
        if(!$('#chkFiltrarFormaPago').is(":checked") && ObjetoPromocion.Existe==true)
        {
            var obj=ObjetoPromocion.FormasPago;
            for (var i=0;i<obj.length;i++)
            {
                FormasPago[obj[i].U_SO1_FORMAPAGO] = obj[i];   
            }
        }
        ObjetoPromocion.FormasPago = FormasPago;
         if($('#chkFiltrarFormaPago').is(":checked") && !validaFormaPago)
        {
            Notificacion('info', '<span data-translate="cr1axb109">Seleccione al menos una forma de pago</span>');
            return false;
        }

        //Si no tiene cargado un articulo, grupo u otro, etonces selecciona al menos uno
        if(!$('#chkActivarArticulo').is(":checked") && !$('#chkActivarGrupo').is(":checked") && !$('#chkActivarPropiedades').is(":checked") && !$('#chkActivarProveedor').is(":checked")
                && !$('#chkActivarFabricante').is(":checked") && !$('#chkActivarConsultaEspecial').is(":checked") && !$('#chkActivarCondicionEspecial').is(":checked"))
        {
            Notificacion('info', '<span data-translate="cr1axb110">Seleccione al menos parámetro que haga válida la promoción</span>');
            return false;
        }

        return true;
    }

    function ArmarDictionary(){
        /* Convertimos los arreglos en Dictionaries */
        // Articulos
        var _Articulos = {};
        var validaArticulo = false;
        for (var i=0;i<PromoOriginal.Articulos.length;i++){
            _Articulos[PromoOriginal.Articulos[i].U_SO1_ARTICULO] = PromoOriginal.Articulos[i];
        }
        PromoOriginal.Articulos = _Articulos;

        // Grupos de Articulos
        var _GrupoArticulos = {};
        for (var i=0;i<PromoOriginal.GrupoArticulos.length;i++){
            _GrupoArticulos[PromoOriginal.GrupoArticulos[i].U_SO1_GRUPO] = PromoOriginal.GrupoArticulos[i];
        }
        PromoOriginal.GrupoArticulos = _GrupoArticulos;

        // Propiedades de Articulos
        var _PropiedadesArticulos = {};
        for (var i=0;i<PromoOriginal.PropiedadesArticulos.length;i++){
            _PropiedadesArticulos[PromoOriginal.PropiedadesArticulos[i].U_SO1_PROPIEDAD] = PromoOriginal.PropiedadesArticulos[i];
        }
        PromoOriginal.PropiedadesArticulos = _PropiedadesArticulos;

        // Proveedores de Articulos
        var _ArticuloProveedor = {};
        for (var i=0;i<PromoOriginal.ArticuloProveedor.length;i++){
            _ArticuloProveedor[PromoOriginal.ArticuloProveedor[i].U_SO1_PROVEEDOR] = PromoOriginal.ArticuloProveedor[i];
        }
        PromoOriginal.ArticuloProveedor = _ArticuloProveedor;

        // Fabricantes de Articulos
        var _ArticuloFabricante = {};
        for (var i=0;i<PromoOriginal.ArticuloFabricante.length;i++){
            _ArticuloFabricante[PromoOriginal.ArticuloFabricante[i].U_SO1_FABRICANTE] = PromoOriginal.ArticuloFabricante[i];
        }
        PromoOriginal.ArticuloFabricante = _ArticuloFabricante;
        
        // Excepciones
        var _Excepciones = {};
        for (var i=0;i<PromoOriginal.Excepciones.length;i++){
            _Excepciones[PromoOriginal.Excepciones[i].U_SO1_ARTICULO] = PromoOriginal.Excepciones[i];
        }
        PromoOriginal.Excepciones = _Excepciones;
        
        // Medidas de Articulos
        var _MedidasArticulos = {};
        for (var i=0;i<PromoOriginal.MedidasArticulos.length;i++){
            _MedidasArticulos[PromoOriginal.MedidasArticulos[i].U_SO1_UNIDAD] = PromoOriginal.MedidasArticulos[i];
        }
        PromoOriginal.MedidasArticulos = _MedidasArticulos;
        
         //Sucursales
        var _Sucursales = {};
        for (var i=0;i<PromoOriginal.Sucursales.length;i++){
            _Sucursales[PromoOriginal.Sucursales[i].U_SO1_SUCURSAL] = PromoOriginal.Sucursales[i];
        }
        PromoOriginal.Sucursales = _Sucursales;
       
        // Alianzas
        var _Alianzas = {};
        for (var i=0;i<PromoOriginal.Alianzas.length;i++){
            _Alianzas[PromoOriginal.Alianzas[i].U_SO1_ALIANZA] = PromoOriginal.Alianzas[i];
        }
        PromoOriginal.Alianzas = _Alianzas;
        
        // Clientes
        var _Clientes = {};
        for (var i=0;i<PromoOriginal.Clientes.length;i++){
            _Clientes[PromoOriginal.Clientes[i].U_SO1_CLIENTE] = PromoOriginal.Clientes[i];
        }
        PromoOriginal.Clientes = _Clientes;
        
        // Grupos de clientes
        var _GrupoClientes = {};
        for (var i=0;i<PromoOriginal.GrupoClientes.length;i++){
            _GrupoClientes[PromoOriginal.GrupoClientes[i].U_SO1_GRUPO] = PromoOriginal.GrupoClientes[i];
        }
        PromoOriginal.GrupoClientes = _GrupoClientes;
        
        // Propiedades de Clientes
        var _PropiedadesClientes = {};
        for (var i=0;i<PromoOriginal.PropiedadesClientes.length;i++){
            _PropiedadesClientes[PromoOriginal.PropiedadesClientes[i].U_SO1_PROPIEDAD] = PromoOriginal.PropiedadesClientes[i];
        }
        PromoOriginal.PropiedadesClientes = _PropiedadesClientes;
        
        // Membresias
        var _Membresias = {};
        for (var i=0;i<PromoOriginal.Membresias.length;i++){
            _Membresias[PromoOriginal.Membresias[i].U_SO1_TIPOMEMBRESIA] = PromoOriginal.Membresias[i];
        }
        PromoOriginal.Membresias = _Membresias;
        
        // Lista de Precios
        var _Precios = {};
        for (var i=0;i<PromoOriginal.Precios.length;i++){
            _Precios[PromoOriginal.Precios[i].U_SO1_LISTAPRECIO] = PromoOriginal.Precios[i];
        }
        PromoOriginal.Precios = _Precios;
        
        // Formas de Pago
        var _FormasPago = {};
        for (var i=0;i<PromoOriginal.FormasPago.length;i++){
            _FormasPago[PromoOriginal.FormasPago[i].U_SO1_FORMAPAGO] = PromoOriginal.FormasPago[i];
        }
        PromoOriginal.FormasPago = _FormasPago;
    }

    function cargarAutocomplete() {
        $.ajax({
            type: "POST",
            url: "CR1PromocionAxB.aspx/CargarAutocomplete",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (result) {
                var response = result["d"];
                if (response[0] == "OK") {

                   listaClientes = response[1];
                   if (listaClientes.length > 0) {
                        var clientesCodigos = {};
                        var clientesNombres = {};
                        for (var i = 0; i < listaClientes.length; i++) {
                            clientesCodigos[listaClientes[i].Codigo] = null;
                            clientesNombres[listaClientes[i].Nombre] = null;
                            listaClientesCodigos.push(listaClientes[i].Codigo);
                            listaClientesNombres.push(listaClientes[i].Nombre);
                        }
                        $("#txtClienteCodigo").autocomplete({
                            limit: 20,
                            data: clientesCodigos,
                            onAutocomplete: function (val) {
                                var index = listaClientesCodigos.indexOf(val);
                                $("#txtClienteNombre").val(listaClientesNombres[index]);
                            }
                        });

                        $("#txtClienteNombre").autocomplete({
                            limit: 20,
                            data: clientesNombres,
                            onAutocomplete: function (val) {
                                var index = listaClientesNombres.indexOf(val);
                                $("#txtClienteCodigo").val(listaClientesCodigos[index]);
                            }
                        });
                    }

                    listaArticulos = response[2];
                    if (listaArticulos.length > 0) {
                        var articulosCodigos = {};
                        var articulosNombres = {};
                        for (var i = 0; i < listaArticulos.length; i++) {
                            articulosCodigos[listaArticulos[i].Codigo] = null;
                            articulosNombres[listaArticulos[i].Nombre] = null;
                            listaArticuloCodigos.push(listaArticulos[i].Codigo);
                            listaArticuloNombres.push(listaArticulos[i].Nombre);
                        }

                        $("#txtArticuloCodigo").autocomplete({
                            limit: 20,
                            data: articulosCodigos,
                            onAutocomplete: function (val) {
                                var index = listaArticuloCodigos.indexOf(val);
                                $("#txtArticuloNombre").val(listaArticuloNombres[index]);
                            }
                        });

                        $("#txtArticuloNombre").autocomplete({
                            limit: 20,
                            data: articulosNombres,
                            onAutocomplete: function (val) {
                                var index = listaArticuloNombres.indexOf(val);
                                $("#txtArticuloCodigo").val(listaArticuloCodigos[index]);
                            }
                        });

                         $("#txtExcepArtCodigo").autocomplete({
                            limit: 20,
                            data: articulosCodigos,
                            onAutocomplete: function (val) {
                                var index = listaArticuloCodigos.indexOf(val);
                                $("#txtExcepArtNombre").val(listaArticuloNombres[index]);
                            }
                        });

                        $("#txtExcepArtNombre").autocomplete({
                            limit: 20,
                            data: articulosNombres,
                            onAutocomplete: function (val) {
                                var index = listaArticuloNombres.indexOf(val);
                                $("#txtExcepArtCodigo").val(listaArticuloCodigos[index]);
                            }
                        });
                    }

                    listaProveedores = response[3];
                    if (listaProveedores.length > 0) {
                        var proveedoresCodigos = {};
                        var proveedoresNombres = {};
                        for (var i = 0; i < listaProveedores.length; i++) {
                            proveedoresCodigos[listaProveedores[i].Codigo] = null;
                            proveedoresNombres[listaProveedores[i].Nombre] = null;
                            listaProveedoresCodigos.push(listaProveedores[i].Codigo);
                            listaProveedoresNombres.push(listaProveedores[i].Nombre);
                        }
                        $("#txtCodigoProveedor").autocomplete({
                            limit: 20,
                            data: proveedoresCodigos,
                            onAutocomplete: function (val) {
                                var index = listaProveedoresCodigos.indexOf(val);
                                $("#txtNombreProveedor").val(listaProveedoresNombres[index]);
                            }
                        });

                        $("#txtNombreProveedor").autocomplete({
                            limit: 20,
                            data: proveedoresNombres,
                            onAutocomplete: function (val) {
                                var index = listaProveedoresNombres.indexOf(val);
                                $("#txtCodigoProveedor").val(listaProveedoresCodigos[index]);
                            }
                        });
                    }

                    var columnasCliente = response[4];
                    if (columnasCliente.length > 0) {
                        var columnasNombres = {};
                        for (var i = 0; i < columnasCliente.length; i++) {
                            columnasNombres[columnasCliente[i].Nombre] = null;
                        }
                        $("#txtClienteCampo").autocomplete({
                            limit: 20,
                            data: columnasNombres,
                            onAutocomplete: function (val) {
                               
                            }
                        });
                    }

                    var columnasArticulo = response[5];
                    if (columnasArticulo.length > 0) {
                        var columnasNombres = {};
                        for (var i = 0; i < columnasArticulo.length; i++) {
                            columnasNombres[columnasArticulo[i].Nombre] = null;
                        }
                        $("#txtCampo").autocomplete({
                            limit: 20,
                            data: columnasNombres,
                            onAutocomplete: function (val) {
                               
                            }
                        });
                    }
                }
            }
        });
    }

    function checaRelacionCliente(){
        var index = listaClientesCodigos.indexOf($("#txtClienteCodigo").val());
        //var index2 = listaClientesNombres.indexOf($("#txtClienteNombre").val());
        var Cliente = listaClientes[index];
        if(index != -1 && $("#txtClienteNombre").val() == Cliente.Nombre)
            return true
        Notificacion('info', '<span data-translate="cr1axb111">El código y nombre de cliente no se relacionan.</span>');
        return false                          
    }

    function checaRelacionArticulo(){
        var index = listaArticuloCodigos.indexOf($("#txtArticuloCodigo").val());
        //var index2 = listaArticuloNombres.indexOf($("#txtArticuloNombre").val());
        var Articulo = listaArticulos[index];
        if(index != -1 && $("#txtArticuloNombre").val() == Articulo.Nombre)
            return true
        Notificacion('info', '<span data-translate="cr1axb112">El código y nombre de articulo no se relacionan.</span>');
        return false   
    }

    function checaRelacionExcepcion(){
        var index = listaArticuloCodigos.indexOf($("#txtExcepArtCodigo").val());
        //var index2 = listaArticuloNombres.indexOf($("#txtExcepArtNombre").val());
        var Articulo = listaArticulos[index];
        if(index != -1 && $("#txtExcepArtNombre").val() == Articulo.Nombre)
            return true
        Notificacion('info', '<span data-translate="cr1axb112">El código y nombre de articulo no se relacionan.</span>');
        return false   
    }

    function checaRelacionProveedor(){
        var index = listaProveedoresCodigos.indexOf($("#txtCodigoProveedor").val());
        //var index2 = listaProveedoresNombres.indexOf($("#txtNombreProveedor").val());
        var Proveedor = listaProveedores[index];
        if(index != -1 && $("#txtNombreProveedor").val() == Proveedor.Nombre)
            return true
        Notificacion('info', '<span data-translate="cr1axb113">El código y nombre de proveedor no se relacionan.</span>');
        return false   
    }

    function ChecaClienteCargado(codigo){
        var clientes = $("#jsClientes").jsGrid("option", "data");
        for(var i=0;i<clientes.length;i++)
        {
            if(clientes[i].U_SO1_CLIENTE == codigo)
            {
                Notificacion('info', '<span data-translate="cr1axb114">El cliente ya ha sido cargado.</span>');
                return false;
            }
        }
        return true;
    }

    function checaArticuloCargado(codigo){
        var articulos = $("#jsArticulos").jsGrid("option", "data");
        for(var i=0;i<articulos.length;i++)
        {
            if(articulos[i].U_SO1_ARTICULO == codigo)
            {
                Notificacion('info', '<span data-translate="cr1axb115">El artículo ya ha sido cargado.</span>');
                return false;
            }
        }
        var excepciones = $("#jsExcepciones").jsGrid("option", "data");
        for(var i=0;i<excepciones.length;i++)
        {
            if(excepciones[i].U_SO1_ARTICULO == codigo)
            {
                Notificacion('info', '<span data-translate="cr1axb116">El articulo ya se encuentra en la excepciones.</span>');
                return false;
            }
        }
        return true;
    }

    function checaProveedorCargado(codigo){
        var proveedores = $("#jsProveedores").jsGrid("option", "data");
        for(var i=0;i<proveedores.length;i++)
        {
            if(proveedores[i].U_SO1_PROVEEDOR == codigo)
            {
                Notificacion('info', '<span data-translate="cr1axb117">El proveedor ya ha sido cargado.</span>');
                return false;
            }
        }
        return true;
    }

    function checaExcepcionCargada(codigo){
        var articulos = $("#jsArticulos").jsGrid("option", "data");
        for(var i=0;i<articulos.length;i++)
        {
            if(articulos[i].U_SO1_ARTICULO == codigo)
            {
                Notificacion('info', '<span data-translate="cr1axb118">Ya se encuentra cargado en los articulos.</span>');
                return false;
            }
        }
        var excepciones = $("#jsExcepciones").jsGrid("option", "data");
        for(var i=0;i<excepciones.length;i++)
        {
            if(excepciones[i].U_SO1_ARTICULO == codigo)
            {
                Notificacion('info', '<span data-translate="cr1axb119">La excepción ya ha sido cargada</span>');
                return false;
            }
        }
        return true;
    }

    function focusMethod(){
        document.getElementById("promocion").focus();
    }

    function Seleccion(seleccion, grid) {
        var items = $("#" + grid).jsGrid("option", "data"); //Optenemos
        for (var i = 0, max = items.length; i < max; i++) {
            var vPromocion = items[i];
            vPromocion.U_SO1_ACTIVO = seleccion;
            $("#" + grid).jsGrid("updateItem", vPromocion);
        }
        loadTranslated(sessionStorage.getItem("lang"));
    }
    function LimpiarValorMinimo(valor, grid) {
        var items = $("#" + grid).jsGrid("option", "data"); //Optenemos
        for (var i = 0, max = items.length; i < max; i++) {
            var vPromocion = items[i];
            vPromocion.U_SO1_MINIMO = formatoNumero(valor,"Cantidad");
            vPromocion.U_SO1_TIPO = "";
            $("#" + grid).jsGrid("updateItem", vPromocion);
        }
        loadTranslated(sessionStorage.getItem("lang"));
    }
    /*-------Consultar---------*/

    function cargarAutocompletePromociones(){
       $.ajax({
            type: "POST",
            url: "CR1PromocionAxB.aspx/CargarAutocompletePromociones",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (result) {
                var response = result["d"];
                if (response[0] == "OK") {
                    var datos = response[1];
                    if (datos.length > 0) {
                        var dataCodigos = {};
                        var dataNombres = {};
                        for (var i = 0; i < datos.length; i++) {
                            dataCodigos[datos[i].Codigo] = null;
                            dataNombres[datos[i].Nombre] = null;
                        }
                        /*$("#txtCodigo").autocomplete({
                            limit: 20,
                            data: dataCodigos,
                            onAutocomplete: function (val) {
                                loadingActive(true);
                                var vParametros = "codigo:'" + $("#txtCodigo").val() + "', nombre:''";
                                Consultar(vParametros);
                            }
                        });*/

                        $("#txtNombre").autocomplete({
                            limit: 20,
                            data: dataNombres,
                            onAutocomplete: function (val) {
                                loadingActive(true);
                                var vParametros = "codigo:'', nombre:'" + $("#txtNombre").val() + "'";
                                Consultar(vParametros);
                            }
                        });
                    }
                    else
                    {

                    }
                }
            },
            error: function (res, msg, code) {

                console.log(msg);
                loadingActive(false);
            }
        }); 
    } 

    function Consultar(vParametros) {
        loadingActive(true);
        $.ajax({
            type: "POST",
            url: "CR1PromocionAxB.aspx/Consultar",
            data: "{" + vParametros + "}",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (result) {
                var response = result["d"];
                if (response[0] == "OK") {
                    if(response[1].U_SO1_TIPO == "AB" || response[1].U_SO1_TIPO == "" || response[1].U_SO1_TIPO == null)
                    {
                        loadingActive(false);

                        PromoOriginal = Object.assign({},response[1]);
                        ObjetoPromocion = JSON.parse(JSON.stringify(response[1]));
                        //ObjetoPromocion = Object.assign({}, response[1]);
                        //ObjetoPromocion.Sucursales =  Object.assign({},PromoOriginal.Sucursales);
                        if(ObjetoPromocion.PromocionAxB != null)
                        {
                            //listarDescuentos(ObjetoPromocion.PromocionAxB);
                        }
                        else
                        {   
                            
                            var PromocionAxB = 
                            {
                                
                            };
                            ObjetoPromocion.PromocionAxB = PromocionAxB;
                            //listarDescuentos(ObjetoPromocion.PromocionDescEsc);   
                        }
                        /* Scripts para mostrar la información */
                        MostrarDatosPromocion(ObjetoPromocion);
                        listarSucursales(ObjetoPromocion.Sucursales);
                        listarArticulos(ObjetoPromocion.Articulos);
                        listarClientes(ObjetoPromocion.Clientes);
                        listarMembresias(ObjetoPromocion.Membresias)
                        listarAlianzas(ObjetoPromocion.Alianzas);
                        listarPrecios(ObjetoPromocion.Precios);
                        listarFormasPago(ObjetoPromocion.FormasPago);
                        listarGruposClientes(ObjetoPromocion.GrupoClientes);
                        listarPropiedadesClientes(ObjetoPromocion.PropiedadesClientes);
                        listarGruposArticulos(ObjetoPromocion.GrupoArticulos);
                        listarPropiedadesArticulos(ObjetoPromocion.PropiedadesArticulos);
                        listarMedidasArticulos(ObjetoPromocion.MedidasArticulos);
                        listarFabricantes(ObjetoPromocion.ArticuloFabricante);
                        listarExcepciones(ObjetoPromocion.Excepciones);
                        listarProveedores(ObjetoPromocion.ArticuloProveedor);

                        if(!$('#cabeceraPromociones').hasClass('active')){
                                $('#cabeceraPromociones').click();
                        }

                        $("#btnCrear").css("display", "block");
                        $("#btnCancelar").css("display", "block");

                        if(ObjetoPromocion.Existe){
                            cargarAutocompletePromociones();
                            $('#txtCodigo').prop('readonly', true);
                            $("#lblCabeceraFiltro").html("<span data-translate='cr1axb120'>Actualización</span>");
                            Notificacion('info', '<span data-translate="cr1axb121">Modo actualización</span>');
                            if($('#cabeceraFiltros').hasClass('active')){
                                    $('#cabeceraFiltros').click();
                            }
                            $('html, body').animate({
                                scrollTop: 390
                            }, 390);
                        }

                        else{
                            $('#txtCodigo').prop('readonly', false);
                            $("#lblCabeceraFiltro").html("<span data-translate='cr1axb122'>Creación</span>");
                            $("#chkDiario").prop('checked', true);
                            $("#chkDiarioTodo").prop('checked', true);
                            Notificacion('info', '<span data-translate="cr1axb123">Modo creación</span>');

                            if(!$('#cabeceraFiltros').hasClass('active')){
                                $('#cabeceraFiltros').click();
                            }
                            
                            
                        }
                        
                        /* Ejecutamos los scripts de validacion */

                        checksTodoelDia();
                        checksIndividual();
                        checksPromocion();
                        checksTablas();
                       
                    }
                    else{
                        Notificacion("warning", '<span data-translate="cr1axb161">La promoción encontrada no es de tipo: AxB</span>');
                    }
                }
                else {
                    if (response[0] == undefined) {
                        window.location = "Login.aspx";
                    }
                    else{
                        Notificacion("warning", '<span data-translate="' + response[1] + '"> Hubo un problema</span>');
                        loadingActive(false);
                    }
                }

                //Traducimos todo el portal.
                loadTranslated(sessionStorage.getItem("lang"));
                loadingActive(false);
            },
            error: function (res, msg, code) {

                console.log(msg);
                loadingActive(false);
            }
        });
    }

    function ActualizarPromocion(Promocion){
        loadingActive(true);
        var param = {};
        param["promocion"] = JSON.stringify(Promocion);
        //param["promocionOriginal"] = JSON.stringify(PromoOriginal);
        $.ajax({
            type: "POST",
            url: "CR1PromocionAxB.aspx/RealizarPromocion",
            data: JSON.stringify(param),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (result) {
                var response = result["d"];
                if (response[0] == "OK") {
                    Notificacion('success', '<span data-translate="cr1axb135">Informacion Actualizada</span>');
                    loadingActive(false);
                    if (response[1] == "SI") {
                        var vParametros = "codigo:'" + Promocion.Codigo + "', nombre:''";
                        Consultar(vParametros);
                    }
                    else{
                        var vParametros = "codigo:'@@@@@@', nombre:''";
                        Consultar(vParametros);
                        $("#txtCodigo").val("");
		                $("#txtNombre").val("");
                    }
                }
                else{
                    if (response[0] == undefined) {
                        window.location = "Login.aspx";
                    }
                    else{
                        Notificacion("warning", '<span data-translate="' + response[0] + '"> Hubo un problema</span>');
                        loadTranslated(sessionStorage.getItem("lang"));
                        loadingActive(false);
                    }
                }
            },
            error: function (result) {
                var response = result["d"];
                loadingActive(false);
                //console.log(res);
                //console.log(msg);
                //console.log(code);
            } //error
        });
        loadTranslated(sessionStorage.getItem("lang"));     
    }

    /*-------------------------*/

    /*-------- Eventos -------*/

    $( "#chkDiario" ).change(function() {
        checksPromocion();  
    });

    $( "#chkDiarioTodo" ).change(function() {
        if($('#chkDiarioTodo').prop('checked'))
        {
            $("#DesdeDiario").attr("disabled", this.value);
            $("#HastaDiario").attr("disabled", this.value);
            $("#DesdeDiario").val("00:00");
            $("#HastaDiario").val("00:00");
        }
        else
        {
            $("#DesdeDiario").attr("disabled", !this.value);
            $("#HastaDiario").attr("disabled", !this.value);
            
        }
    });

    $('#DesdeDiario').on('change', function() {
    if(obtieneHora(this.value)>obtieneHora($("#HastaDiario").val()))
        {
            var aux = $("#HastaDiario").val();
            $("#HastaDiario").val(this.value);
            $("#DesdeDiario").val(aux);
        }
    });

    $("#HastaDiario" ).change(function() {
        if(obtieneHora($("#DesdeDiario").val())>obtieneHora(this.value))
        {
            var aux = this.value;
            $("#HastaDiario").val($("#DesdeDiario").val());
            $("#DesdeDiario").val(aux);
        }
    });

    $( "#chkLunes").change(function() {
        if($('#chkLunes').prop('checked'))
        {
            $("#chkLunesTodo").attr("disabled", !this.value);
            $("#DesdeLunes").attr("disabled", this.value);
            $("#HastaLunes").attr("disabled", this.value);
            $("#chkLunesTodo").prop("checked", true);
        }
        else
        {        
            $("#chkLunesTodo").attr("disabled", this.value);
            $("#chkLunesTodo").prop("checked", false);
            $("#DesdeLunes").val("00:00");
            $("#HastaLunes").val("00:00");
            $("#DesdeLunes").attr("disabled", this.value);
            $("#HastaLunes").attr("disabled", this.value); 
        }
    });

    $('#DesdeLunes').on('change', function() {
    if(obtieneHora(this.value)>obtieneHora($("#HastaLunes").val()))
        {
            var aux = $("#HastaLunes").val();
            $("#HastaLunes").val(this.value);
            $("#DesdeLunes").val(aux);
        }
    });

    $("#HastaLunes" ).change(function() {
        if(obtieneHora($("#DesdeLunes").val())>obtieneHora(this.value))
        {
            var aux = this.value;
            $("#HastaLunes").val($("#DesdeLunes").val());
            $("#DesdeLunes").val(aux);
        }
    });

    $( "#chkMartes" ).change(function() {
    if($('#chkMartes').prop('checked'))
        {
            $("#chkMartesTodo").attr("disabled", !this.value);
            $("#DesdeMartes").attr("disabled", this.value);
            $("#HastaMartes").attr("disabled", this.value);
            $("#chkMartesTodo").prop("checked", true);
        }
        else
        {        
            $("#chkMartesTodo").attr("disabled", this.value);
            $("#chkMartesTodo").prop("checked", false);
            $("#DesdeMartes").val("00:00");
            $("#HastaMartes").val("00:00");
            $("#DesdeMartes").attr("disabled", this.value);
            $("#HastaMartes").attr("disabled", this.value); 
        }
    });

    $('#DesdeMartes').on('change', function() {
    if(obtieneHora(this.value)>obtieneHora($("#HastaMartes").val()))
        {
            var aux = $("#HastaMartes").val();
            $("#HastaMartes").val(this.value);
            $("#DesdeMartes").val(aux);
        }
    });

    $("#HastaMartes" ).change(function() {
        if(obtieneHora($("#DesdeMartes").val())>obtieneHora(this.value))
        {
            var aux = this.value;
            $("#HastaMartes").val($("#DesdeMartes").val());
            $("#DesdeMartes").val(aux);
        }
    });

    $( "#chkMiercoles" ).change(function() {
        if($('#chkMiercoles').prop('checked'))
        {
            $("#chkMiercolesTodo").attr("disabled", !this.value);
            $("#DesdeMiercoles").attr("disabled", this.value);
            $("#HastaMiercoles").attr("disabled", this.value);
            $("#chkMiercolesTodo").prop("checked", true);
        }
        else
        {        
            $("#chkMiercolesTodo").attr("disabled", this.value);
            $("#chkMiercolesTodo").prop("checked", false);
            $("#DesdeMiercoles").val("00:00");
            $("#HastaMiercoles").val("00:00");
            $("#DesdeMiercoles").attr("disabled", this.value);
            $("#HastaMiercoles").attr("disabled", this.value); 
        }
    });

    $('#DesdeMiercoles').on('change', function() {
    if(obtieneHora(this.value)>obtieneHora($("#HastaMiercoles").val()))
        {
            var aux = $("#HastaMiercoles").val();
            $("#HastaMiercoles").val(this.value);
            $("#DesdeMiercoles").val(aux);
        }
    });

    $("#HastaMiercoles" ).change(function() {
        if(obtieneHora($("#DesdeMiercoles").val())>obtieneHora(this.value))
        {
            var aux = this.value;
            $("#HastaMiercoles").val($("#DesdeMiercoles").val());
            $("#DesdeMiercoles").val(aux);
        }
    });

    $( "#chkJueves" ).change(function() {

        if($('#chkJueves').prop('checked'))
        {
            $("#chkJuevesTodo").attr("disabled", !this.value);
            $("#DesdeJueves").attr("disabled", this.value);
            $("#HastaJueves").attr("disabled", this.value);
            $("#chkJuevesTodo").prop("checked", true);
        }
        else
        {        
            $("#chkJuevesTodo").attr("disabled", this.value);
            $("#chkJuevesTodo").prop("checked", false);
            $("#DesdeJueves").val("00:00");
            $("#HastaJueves").val("00:00");
            $("#DesdeJueves").attr("disabled", this.value);
            $("#HastaJueves").attr("disabled", this.value); 
        }
    });

    $('#DesdeJueves').on('change', function() {
    if(obtieneHora(this.value)>obtieneHora($("#HastaJueves").val()))
        {
            var aux = $("#HastaJueves").val();
            $("#HastaJueves").val(this.value);
            $("#DesdeJueves").val(aux);
        }
    });

    $("#HastaJueves" ).change(function() {
        if(obtieneHora($("#DesdeJueves").val())>obtieneHora(this.value))
        {
            var aux = this.value;
            $("#HastaJueves").val($("#DesdeJueves").val());
            $("#DesdeJueves").val(aux);
        }
    });

    $( "#chkViernes" ).change(function() {

        if($('#chkViernes').prop('checked'))
        {
            $("#chkViernesTodo").attr("disabled", !this.value);
            $("#DesdeViernes").attr("disabled", this.value);
            $("#HastaViernes").attr("disabled", this.value);
            $("#chkViernesTodo").prop("checked", true);
        }
        else
        {        
            $("#chkViernesTodo").attr("disabled", this.value);
            $("#chkViernesTodo").prop("checked", false);
            $("#DesdeViernes").val("00:00");
            $("#HastaViernes").val("00:00");
            $("#DesdeViernes").attr("disabled", this.value);
            $("#HastaViernes").attr("disabled", this.value); 
        }
    });

    $('#DesdeViernes').on('change', function() {
    if(obtieneHora(this.value)>obtieneHora($("#HastaViernes").val()))
        {
            var aux = $("#HastaViernes").val();
            $("#HastaViernes").val(this.value);
            $("#DesdeViernes").val(aux);
        }
    });

    $("#HastaViernes" ).change(function() {
        if(obtieneHora($("#DesdeViernes").val())>obtieneHora(this.value))
        {
            var aux = this.value;
            $("#HastaViernes").val($("#DesdeViernes").val());
            $("#DesdeViernes").val(aux);
        }
    });

    $( "#chkSabado" ).change(function() {

        if($('#chkSabado').prop('checked'))
        {
            $("#chkSabadoTodo").attr("disabled", !this.value);
            $("#DesdeSabado").attr("disabled", this.value);
            $("#HastaSabado").attr("disabled", this.value);
            $("#chkSabadoTodo").prop("checked", true);
        }
        else
        {        
            $("#chkSabadoTodo").attr("disabled", this.value);
            $("#chkSabadoTodo").prop("checked", false);
            $("#DesdeSabado").val("00:00");
            $("#HastaSabado").val("00:00");
            $("#DesdeSabado").attr("disabled", this.value);
            $("#HastaSabado").attr("disabled", this.value); 
        }
    });

    $('#DesdeSabado').on('change', function() {
    if(obtieneHora(this.value)>obtieneHora($("#HastaSabado").val()))
        {
            var aux = $("#HastaSabado").val();
            $("#HastaSabado").val(this.value);
            $("#DesdeSabado").val(aux);
        }
    });

    $("#HastaSabado" ).change(function() {
        if(obtieneHora($("#DesdeSabado").val())>obtieneHora(this.value))
        {
            var aux = this.value;
            $("#HastaSabado").val($("#DesdeSabado").val());
            $("#DesdeSabado").val(aux);
        }
    });

    $( "#chkDomingo" ).change(function() {

        if($('#chkDomingo').prop('checked'))
        {
            $("#chkDomingoTodo").attr("disabled", !this.value);
            $("#DesdeDomingo").attr("disabled", this.value);
            $("#HastaDomingo").attr("disabled", this.value);
            $("#chkDomingoTodo").prop("checked", true);
        }
        else
        {        
            $("#chkDomingoTodo").attr("disabled", this.value);
            $("#chkDomingoTodo").prop("checked", false);
            $("#DesdeDomingo").val("00:00");
            $("#HastaDomingo").val("00:00");
            $("#DesdeDomingo").attr("disabled", this.value);
            $("#HastaDomingo").attr("disabled", this.value); 
        }
    });

    $('#DesdeDomingo').on('change', function() {
    if(obtieneHora(this.value)>obtieneHora($("#HastaDomingo").val()))
        {
            var aux = $("#HastaDomingo").val();
            $("#HastaDomingo").val(this.value);
            $("#DesdeDomingo").val(aux);
        }
    });

    $("#HastaDomingo" ).change(function() {
        if(obtieneHora($("#DesdeDomingo").val())>obtieneHora(this.value))
        {
            var aux = this.value;
            $("#HastaDomingo").val($("#DesdeDomingo").val());
            $("#DesdeDomingo").val(aux);
        }
    });

    $( "#chkLunesTodo" ).change(function() {
        if($('#chkLunesTodo').prop('checked'))
        {
            $("#DesdeLunes").attr("disabled", this.value);
            $("#HastaLunes").attr("disabled", this.value);
            $("#DesdeLunes").val("00:00");
            $("#HastaLunes").val("00:00");
        }
        else
        {
            $("#DesdeLunes").attr("disabled", !this.value);
            $("#HastaLunes").attr("disabled", !this.value);
            
        }
    });

    $( "#chkMartesTodo" ).change(function() {
        if($('#chkMartesTodo').prop('checked'))
        {
            $("#DesdeMartes").attr("disabled", this.value);
            $("#HastaMartes").attr("disabled", this.value);
            $("#DesdeMartes").val("00:00");
            $("#HastaMartes").val("00:00");
        }
        else
        {
            $("#DesdeMartes ").attr("disabled", !this.value);
            $("#HastaMartes").attr("disabled", !this.value);
            
        }
    });

    $( "#chkMiercolesTodo" ).change(function() {
        if($('#chkMiercolesTodo').prop('checked'))
        {
            $("#DesdeMiercoles").attr("disabled", this.value);
            $("#HastaMiercoles").attr("disabled", this.value);
            $("#DesdeMiercoles").val("00:00");
            $("#HastaMiercoles").val("00:00");
        }
        else
        {
            $("#DesdeMiercoles").attr("disabled", !this.value);
            $("#HastaMiercoles").attr("disabled", !this.value);
            
        }
    });

    $( "#chkJuevesTodo" ).change(function() {
        if($('#chkJuevesTodo').prop('checked'))
        {
            $("#DesdeJueves").attr("disabled", this.value);
            $("#HastaJueves").attr("disabled", this.value);
            $("#DesdeJueves").val("00:00");
            $("#HastaJueves").val("00:00");
        }
        else
        {
            $("#DesdeJueves").attr("disabled", !this.value);
            $("#HastaJueves").attr("disabled", !this.value);
            
        }
    });

    $( "#chkViernesTodo" ).change(function() {
    if($('#chkViernesTodo').prop('checked'))
        {
            $("#DesdeViernes").attr("disabled", this.value);
            $("#HastaViernes").attr("disabled", this.value);
            $("#DesdeViernes").val("00:00");
            $("#HastaViernes").val("00:00");
        }
        else
        {
            $("#DesdeViernes").attr("disabled", !this.value);
            $("#HastaViernes").attr("disabled", !this.value);
            
        }
    });

    $( "#chkSabadoTodo" ).change(function() {
    if($('#chkSabadoTodo').prop('checked'))
        {
            $("#DesdeSabado").attr("disabled", this.value);
            $("#HastaSabado").attr("disabled", this.value);
            $("#DesdeSabado").val("00:00");
            $("#HastaSabado").val("00:00");
        }
        else
        {
            $("#DesdeSabado").attr("disabled", !this.value);
            $("#HastaSabado").attr("disabled", !this.value);
            
        }
    });

    $( "#chkDomingoTodo" ).change(function() {
        if($('#chkDomingoTodo').prop('checked'))
        {
            $("#DesdeDomingo").attr("disabled", this.value);
            $("#HastaDomingo").attr("disabled", this.value);
            $("#DesdeDomingo").val("00:00");
            $("#HastaDomingo").val("00:00");
        }
        else
        {
            $("#DesdeDomingo").attr("disabled", !this.value);
            $("#HastaDomingo").attr("disabled", !this.value);
            
        }
    });

    $( "#chkLimVenta" ).change(function() {
        $("#txtLimVenta").attr("disabled",  !$('#chkLimVenta').prop('checked'));
        $("#txtLimVenta").val(formatear(0,"Cantidad"));
    });

    $( "#chkPzsLimProm" ).change(function() {
        $("#txtPzsLimProm").attr("disabled",  !$('#chkPzsLimProm').prop('checked'));
        $("#txtPzsLimProm").val(formatear(0,"Cantidad"));

    });

    $( "#chkVenLimProm" ).change(function() {
        $("#txtVenLimProm").attr("disabled",  !$('#chkVenLimProm').prop('checked'));
        $("#txtVenLimProm").val(formatear(0,"Cantidad"));

    });

    /* Tabs de Articulos */

    $( "#chkActivarArticulo" ).change(function() {
        $("#txtArticuloCodigo").attr("disabled",  !$('#chkActivarArticulo').prop('checked'));
        $("#txtArticuloNombre").attr("disabled",  !$('#chkActivarArticulo').prop('checked'));
        $("#btn_agregar_articulo").attr("disabled",  !$('#chkActivarArticulo').prop('checked'));
        $("#btn_importar_articulo").attr("disabled",  !$('#chkActivarArticulo').prop('checked'));
    });

    $( "#chkActivarGrupo" ).change(function() {
        $("#jsArticulosGrupos").jsGrid("fieldOption", "U_SO1_ACTIVO", "visible", $('#chkActivarGrupo').prop('checked'));
        $("#btnSeleccionarGruposArticulos").attr("disabled",  !$('#chkActivarGrupo').prop('checked'));
        $("#btnDeseleccionarGruposArticulos").attr("disabled",  !$('#chkActivarGrupo').prop('checked'));
        if(!$('#chkActivarGrupo').prop('checked'))
        {
            Seleccion("N", "jsArticulosGrupos");
        }
        loadTranslated(sessionStorage.getItem("lang"));
    });

    $("#btnSeleccionarGruposArticulos").click(function () {
        Seleccion("Y", "jsArticulosGrupos");
    });

    $("#btnDeseleccionarGruposArticulos").click(function () {
        Seleccion("N", "jsArticulosGrupos");
    });

    $( "#chkActivarPropiedades" ).change(function() {
        $("#jsArticulosPropiedades").jsGrid("fieldOption", "U_SO1_ACTIVO", "visible", $('#chkActivarPropiedades').prop('checked'));
        $("#btnSeleccionarPropiedadesArticulos").attr("disabled",  !$('#chkActivarPropiedades').prop('checked'));
        $("#btnDeseleccionarPropiedadesArticulos").attr("disabled",  !$('#chkActivarPropiedades').prop('checked'));
        $("#relacionYArticulo").attr("disabled",  !$('#chkActivarPropiedades').prop('checked'));
        $("#relacionOArticulo").attr("disabled",  !$('#chkActivarPropiedades').prop('checked'));
        $("#chkCEPropiedadesArticulo").attr("disabled",  !$('#chkActivarPropiedades').prop('checked'));
        if(!$('#chkActivarPropiedades').prop('checked'))
        {
            Seleccion("N", "jsArticulosPropiedades");
            $("#relacionYArticulo").prop('checked',true);
            $("#relacionOArticulo").prop('checked',false);
            $("#chkCEPropiedadesArticulo").prop('checked',false);
        }
        loadTranslated(sessionStorage.getItem("lang"));
    });

    $("#btnSeleccionarPropiedadesArticulos").click(function () {
        Seleccion("Y", "jsArticulosPropiedades");
    });

    $("#btnDeseleccionarPropiedadesArticulos").click(function () {
        Seleccion("N", "jsArticulosPropiedades");
    });

    $( "#chkActivarCondicionEspecial" ).change(function() {
        if(!$('#chkActivarCondicionEspecial').prop('checked'))
        {
            $("#txtCampo").val("");
            $("#txtValor").val("");
        }
        $("#txtCampo").attr("disabled",  !$('#chkActivarCondicionEspecial').prop('checked'));
        $("#txtValor").attr("disabled",  !$('#chkActivarCondicionEspecial').prop('checked'));
    });

    $( "#chkActivarConsultaEspecial" ).change(function() {
        if(!$('#chkActivarConsultaEspecial').prop('checked'))
        {
            $("#txtConsulta").val("");
        }
        $("#txtConsulta").attr("disabled",  !$('#chkActivarConsultaEspecial').prop('checked'));
        
    });

    $( "#chkActivarProveedor" ).change(function() {
        $("#txtCodigoProveedor").attr("disabled",  !$('#chkActivarProveedor').prop('checked'));
        $("#txtNombreProveedor").attr("disabled",  !$('#chkActivarProveedor').prop('checked'));
        $("#btn_agregar_proveedor").attr("disabled",  !$('#chkActivarProveedor').prop('checked'));
    });

    $( "#chkActivarFabricante" ).change(function() {
        if(!$('#chkActivarFabricante').prop('checked'))
        {
            Seleccion("N", "jsFabricantes");
        }
        $("#jsFabricantes").jsGrid("fieldOption", "U_SO1_ACTIVO", "visible", $('#chkActivarFabricante').prop('checked'));
        loadTranslated(sessionStorage.getItem("lang"));
    });

    $( "#chkActivarExcepciones" ).change(function() {
        $("#txtExcepArtCodigo").attr("disabled",  !$('#chkActivarExcepciones').prop('checked'));
        $("#txtExcepArtNombre").attr("disabled",  !$('#chkActivarExcepciones').prop('checked'));
        $("#btn_agregar_excepcion").attr("disabled",  !$('#chkActivarExcepciones').prop('checked'));
    });

    $( "#chkActivarUnidades" ).change(function() {
        if(!$('#chkActivarUnidades').prop('checked'))
        {
            Seleccion("N", "jsArticulosUnidades");
        }
        $("#btnSeleccionarUnidadMArticulos").attr("disabled",  !$('#chkActivarUnidades').prop('checked'));
        $("#btnDeseleccionarUnidadMArticulos").attr("disabled",  !$('#chkActivarUnidades').prop('checked'));
        $("#jsArticulosUnidades").jsGrid("fieldOption", "U_SO1_ACTIVO", "visible", $('#chkActivarUnidades').prop('checked'));
        loadTranslated(sessionStorage.getItem("lang"));
    });

    $("#btnSeleccionarUnidadMArticulos").click(function () {
        Seleccion("Y", "jsArticulosUnidades");
    });

    $("#btnDeseleccionarUnidadMArticulos").click(function () {
        Seleccion("N", "jsArticulosUnidades");
    });

    /* Tab de Sucursal */

    $( "#chkFiltrarSucursales" ).change(function() {
        if(!$('#chkFiltrarSucursales').prop('checked'))
        {
            Seleccion("N", "jsSucursales");
        }
        $("#jsSucursales").jsGrid("fieldOption", "U_SO1_ACTIVO", "visible", $('#chkFiltrarSucursales').prop('checked'));
        $("#btnSeleccionarSucursales").attr("disabled",  !$('#chkFiltrarSucursales').prop('checked'));
        $("#btnDeseleccionarSucursales").attr("disabled",  !$('#chkFiltrarSucursales').prop('checked'));
        loadTranslated(sessionStorage.getItem("lang"));
    });

    $("#btnSeleccionarSucursales").click(function () {
        Seleccion("Y", "jsSucursales");
    });

    $("#btnDeseleccionarSucursales").click(function () {
        Seleccion("N", "jsSucursales");
    });

    /* Tab de Alianza */

    $( "#chkFiltrarAlianzas" ).change(function() {
        if(!$('#chkFiltrarAlianzas').prop('checked'))
        {
            Seleccion("N", "jsAlianzas");
        }
        $("#jsAlianzas").jsGrid("fieldOption", "U_SO1_ACTIVO", "visible", $('#chkFiltrarAlianzas').prop('checked'));
        $("#btnSeleccionarAlianza").attr("disabled",  !$('#chkFiltrarAlianzas').prop('checked'));
        $("#btnDeseleccionarAlianza").attr("disabled",  !$('#chkFiltrarAlianzas').prop('checked'));
        loadTranslated(sessionStorage.getItem("lang"));
    });


    $("#btnSeleccionarAlianza").click(function () {
        Seleccion("Y", "jsAlianzas");
    });

    $("#btnDeseleccionarAlianza").click(function () {
        Seleccion("N", "jsAlianzas");
    });

    /* Tab de Clientes */
    $( "#chkClientes" ).change(function() {
        
        if($('#chkClientes').is(':checked'))
        {
            $('#btnTabClientes').removeClass("disabled");
            $('#btnTabGrupo').removeClass("disabled");
            $('#btnTabPropiedad').removeClass("disabled");
       }
       else
       {
            $('#btnTabClientes').addClass("disabled");
            $('#btnTabGrupo').addClass("disabled");
            $('#btnTabPropiedad').addClass("disabled");
       }

       if($('#chkClientes').is(':checked') && $('#chkActivarClientes').is(':checked'))
       {
            $('#txtClienteCodigo').attr('disabled',false);
            $('#txtClienteNombre').attr('disabled', false);
            $('#btn_agregar_cliente').attr('disabled', false);
       }
       else
       {
            $('#txtClienteCodigo').attr('disabled',true);
            $('#txtClienteNombre').attr('disabled', true);
            $('#btn_agregar_cliente').attr('disabled', true);
       }

       if($('#chkClientes').is(':checked') && $('#chkActivarClientesCE').is(':checked'))
       {
            $('#txtClienteCampo').attr('disabled',false);
            $('#txtClienteValor').attr('disabled', false);

       }
       else
       {
            $('#txtClienteCampo').attr('disabled',true);
            $('#txtClienteValor').attr('disabled', true);
       }

       if($('#chkClientes').is(':checked') && $('#chkActivarGrupoClientes').is(':checked'))
       {
            $('#btnSeleccionarGrupoCliente').attr('disabled',false);
            $('#btnDeseleccionarGrupoCliente').attr('disabled', false);

       }
       else
       {
            $('#btnSeleccionarGrupoCliente').attr('disabled',true);
            $('#btnDeseleccionarGrupoCliente').attr('disabled', true);
       }

       if($('#chkClientes').is(':checked') && $('#chkActivarPropiedadesClientes').is(':checked'))
       {
            $('#btnSeleccionarPropCliente').attr('disabled',false);
            $('#btnDeseleccionarPropCliente').attr('disabled', false);
            $('#chkClienteCE').attr('disabled', false);

       }
       else
       {
            $('#btnSeleccionarPropCliente').attr('disabled',true);
            $('#btnDeseleccionarPropCliente').attr('disabled', true);
            $('#chkClienteCE').attr('disabled', true);
       }

        $('#chkActivarClientes').attr('disabled', !$('#chkClientes').prop('checked'));
        
        $('#chkActivarClientesCE').attr('disabled', !$('#chkClientes').prop('checked'));
        
        $('#chkActivarGrupoClientes').attr('disabled', !$('#chkClientes').prop('checked'));
        $('#chkActivarPropiedadesClientes').attr('disabled', !$('#chkClientes').prop('checked'));
        $('#PropiedadClienteY').attr('disabled', !$('#chkClientes').prop('checked'));
        $('#PropiedadClienteO').attr('disabled',!$('#chkClientes').prop('checked'));
        
    });

    $( "#chkActivarClientes" ).change(function() {
        $("#txtClienteCodigo").attr("disabled",  !$('#chkActivarClientes').prop('checked'));
        $("#txtClienteNombre").attr("disabled",  !$('#chkActivarClientes').prop('checked'));
        $("#btn_agregar_cliente").attr("disabled",  !$('#chkActivarClientes').prop('checked'));
    });

    $( "#chkActivarClientesCE" ).change(function() {
        if(!$('#chkActivarClientesCE').prop('checked'))
        {
            $("#txtClienteCampo").val("");
            $("#txtClienteValor").val("");
        }
        $("#txtClienteCampo").attr("disabled",  !$('#chkActivarClientesCE').prop('checked'));
        $("#txtClienteValor").attr("disabled",  !$('#chkActivarClientesCE').prop('checked'));

    });

    $( "#chkActivarGrupoClientes" ).change(function() {
        if(!$('#chkActivarGrupoClientes').prop('checked'))
        {
            Seleccion("N", "jsGruposClientes");
        }
        $("#jsGruposClientes").jsGrid("fieldOption", "U_SO1_ACTIVO", "visible", $('#chkActivarGrupoClientes').prop('checked'));
        $("#btnSeleccionarGrupoCliente").attr("disabled",  !$('#chkActivarGrupoClientes').prop('checked'));
        $("#btnDeseleccionarGrupoCliente").attr("disabled",  !$('#chkActivarGrupoClientes').prop('checked'));
        loadTranslated(sessionStorage.getItem("lang"));
    });

    $("#btnSeleccionarGrupoCliente").click(function () {
        Seleccion("Y", "jsGruposClientes");
    });

    $("#btnDeseleccionarGrupoCliente").click(function () {
        Seleccion("N", "jsGruposClientes");
    });

    $( "#chkActivarPropiedadesClientes" ).change(function() {
        if(!$('#chkActivarPropiedadesClientes').prop('checked'))
        {
            Seleccion("N", "jsPropiedadesClientes");
            $('#chkClienteCE').prop('checked',false);
            $('#PropiedadClienteY').prop('checked',true);
            $('#PropiedadClienteO').prop('checked',false);
        }
        $("#jsPropiedadesClientes").jsGrid("fieldOption", "U_SO1_ACTIVO", "visible", $('#chkActivarPropiedadesClientes').prop('checked'));
        $("#btnSeleccionarPropCliente").attr("disabled",  !$('#chkActivarPropiedadesClientes').prop('checked'));
        $("#btnDeseleccionarPropCliente").attr("disabled",  !$('#chkActivarPropiedadesClientes').prop('checked'));
       // $("#PropiedadClienteY").attr("disabled",  !$('#chkActivarPropiedadesClientes').prop('checked'));
        //$("#PropiedadClienteO").attr("disabled",  !$('#chkActivarPropiedadesClientes').prop('checked'));
        $("#chkClienteCE").attr("disabled",  !$('#chkActivarPropiedadesClientes').prop('checked'));
        loadTranslated(sessionStorage.getItem("lang"));
    });

    $("#btnSeleccionarPropCliente").click(function () {
        Seleccion("Y", "jsPropiedadesClientes");
    });

    $("#btnDeseleccionarPropCliente").click(function () {
        Seleccion("N", "jsPropiedadesClientes");
    });

    /* Tabs de Membresias */

    $( "#chkFiltrarMembresias" ).change(function() {
        if(!$('#chkFiltrarMembresias').prop('checked'))
        {
            Seleccion("N", "jsMembresias");
        }
        $("#jsMembresias").jsGrid("fieldOption", "U_SO1_ACTIVO", "visible", $('#chkFiltrarMembresias').prop('checked'));
        $("#btnSeleccionarMembresias").attr("disabled",  !$('#chkFiltrarMembresias').prop('checked'));
        $("#btnDeseleccionarMembresias").attr("disabled",  !$('#chkFiltrarMembresias').prop('checked'));
        loadTranslated(sessionStorage.getItem("lang"));
    });

    $("#btnSeleccionarMembresias").click(function () {
        Seleccion("Y", "jsMembresias");
    });

    $("#btnDeseleccionarMembresias").click(function () {
        Seleccion("N", "jsMembresias");
    });

    /* Tabs de Lista de Precios */

    $( "#chkFiltrarPrecios" ).change(function() {
        if(!$('#chkFiltrarPrecios').prop('checked'))
        {
            Seleccion("N", "jsPrecios");
        }
        $("#jsPrecios").jsGrid("fieldOption", "U_SO1_ACTIVO", "visible", $('#chkFiltrarPrecios').prop('checked'));
        $("#btnSeleccionarPrecios").attr("disabled",  !$('#chkFiltrarPrecios').prop('checked'));
        $("#btnDeseleccionarPrecios").attr("disabled",  !$('#chkFiltrarPrecios').prop('checked'));
        loadTranslated(sessionStorage.getItem("lang"));
    });

    $("#btnSeleccionarPrecios").click(function () {
        Seleccion("Y", "jsPrecios");
    });

    $("#btnDeseleccionarPrecios").click(function () {
        Seleccion("N", "jsPrecios");
    });

    /* Tabs de Pago */

    $( "#chkFiltrarFormaPago" ).change(function() {
        if(!$('#chkFiltrarFormaPago').prop('checked'))
        {
            Seleccion("N", "jsFormasPago");
            LimpiarValorMinimo(0,"jsFormasPago");
            $('#chkFiltrarVentaCredito').prop('checked',false);
        }
        $("#jsFormasPago").jsGrid("fieldOption", "U_SO1_ACTIVO", "visible", $('#chkFiltrarFormaPago').prop('checked'));
        $("#chkFiltrarVentaCredito").attr("disabled",  !$('#chkFiltrarFormaPago').prop('checked'));
        $("#btnSeleccionarPagos").attr("disabled",  !$('#chkFiltrarFormaPago').prop('checked'));
        $("#btnDeseleccionarPagos").attr("disabled",  !$('#chkFiltrarFormaPago').prop('checked'));
        loadTranslated(sessionStorage.getItem("lang"));
    });    

    $("#btnSeleccionarPagos").click(function () {
        //Seleccion("Y", "jsFormasPago");
        SeleccionFormasPago("Y");
    });

    $("#btnDeseleccionarPagos").click(function () {
        //Seleccion("N", "jsFormasPago");
        SeleccionFormasPago("N");
    });

    /*-------------------------*/

    /*------Validaciones ------*/

    function checksTablas(){

        $("#txtLimVenta").attr("disabled",  !$('#chkLimVenta').prop('checked'));
        $("#txtPzsLimProm").attr("disabled",  !$('#chkPzsLimProm').prop('checked'));
        $("#txtVenLimProm").attr("disabled",  !$('#chkVenLimProm').prop('checked'));

        $("#txtArticuloCodigo").attr("disabled",  !$('#chkActivarArticulo').prop('checked'));
        $("#txtArticuloNombre").attr("disabled",  !$('#chkActivarArticulo').prop('checked'));
        $("#btn_agregar_articulo").attr("disabled",  !$('#chkActivarArticulo').prop('checked'));
        $("#btn_importar_articulo").attr("disabled",  !$('#chkActivarArticulo').prop('checked'));
        $("#jsArticulosGrupos").jsGrid("fieldOption", "U_SO1_ACTIVO", "visible", $('#chkActivarGrupo').prop('checked'));
        $("#btnSeleccionarGruposArticulos").attr("disabled",  !$('#chkActivarGrupo').prop('checked'));
        $("#btnDeseleccionarGruposArticulos").attr("disabled",  !$('#chkActivarGrupo').prop('checked'));

        $("#jsArticulosPropiedades").jsGrid("fieldOption", "U_SO1_ACTIVO", "visible", $('#chkActivarPropiedades').prop('checked'));
        $("#btnSeleccionarPropiedadesArticulos").attr("disabled",  !$('#chkActivarPropiedades').prop('checked'));
        $("#btnDeseleccionarPropiedadesArticulos").attr("disabled",  !$('#chkActivarPropiedades').prop('checked'));
        $("#relacionYArticulo").attr("disabled",  !$('#chkActivarPropiedades').prop('checked'));
        $("#relacionOArticulo").attr("disabled",  !$('#chkActivarPropiedades').prop('checked'));
        $("#chkCEPropiedadesArticulo").attr("disabled",  !$('#chkActivarPropiedades').prop('checked'));

        $("#txtCampo").attr("disabled",  !$('#chkActivarCondicionEspecial').prop('checked'));
        $("#txtValor").attr("disabled",  !$('#chkActivarCondicionEspecial').prop('checked'));
        $("#txtConsulta").attr("disabled",  !$('#chkActivarConsultaEspecial').prop('checked'));
        $("#txtCodigoProveedor").attr("disabled",  !$('#chkActivarProveedor').prop('checked'));
        $("#txtNombreProveedor").attr("disabled",  !$('#chkActivarProveedor').prop('checked'));
        $("#btn_agregar_proveedor").attr("disabled",  !$('#chkActivarProveedor').prop('checked'));
        $("#jsFabricantes").jsGrid("fieldOption", "U_SO1_ACTIVO", "visible", $('#chkActivarFabricante').prop('checked'));

        
        $("#txtExcepArtCodigo").attr("disabled",  !$('#chkActivarExcepciones').prop('checked'));
        $("#txtExcepArtNombre").attr("disabled",  !$('#chkActivarExcepciones').prop('checked'));
        $("#btn_agregar_excepcion").attr("disabled",  !$('#chkActivarExcepciones').prop('checked'));

        $("#jsArticulosUnidades").jsGrid("fieldOption", "U_SO1_ACTIVO", "visible", $('#chkActivarUnidades').prop('checked'));
        $("#btnSeleccionarUnidadMArticulos").attr("disabled",  !$('#chkActivarUnidades').prop('checked'));
        $("#btnDeseleccionarUnidadMArticulos").attr("disabled",  !$('#chkActivarUnidades').prop('checked'));


        $("#jsSucursales").jsGrid("fieldOption", "U_SO1_ACTIVO", "visible", $('#chkFiltrarSucursales').prop('checked'));
        $("#btnSeleccionarSucursales").attr("disabled",  !$('#chkFiltrarSucursales').prop('checked'));
        $("#btnDeseleccionarSucursales").attr("disabled",  !$('#chkFiltrarSucursales').prop('checked'));

        $("#jsAlianzas").jsGrid("fieldOption", "U_SO1_ACTIVO", "visible", $('#chkFiltrarAlianzas').prop('checked'));
        $("#btnSeleccionarAlianza").attr("disabled",  !$('#chkFiltrarAlianzas').prop('checked'));
        $("#btnDeseleccionarAlianza").attr("disabled",  !$('#chkFiltrarAlianzas').prop('checked'));

        $("#txtClienteCodigo").attr("disabled",  !$('#chkActivarClientes').prop('checked'));
        $("#txtClienteNombre").attr("disabled",  !$('#chkActivarClientes').prop('checked'));
        $("#btn_agregar_cliente").attr("disabled",  !$('#chkActivarClientes').prop('checked'));
        $("#txtClienteCampo").attr("disabled",  !$('#chkActivarClientesCE').prop('checked'));
        $("#txtClienteValor").attr("disabled",  !$('#chkActivarClientesCE').prop('checked'));
        $("#jsGruposClientes").jsGrid("fieldOption", "U_SO1_ACTIVO", "visible", $('#chkActivarGrupoClientes').prop('checked'));
        $("#btnSeleccionarGrupoCliente").attr("disabled",  !$('#chkActivarGrupoClientes').prop('checked'));
        $("#btnDeseleccionarGrupoCliente").attr("disabled",  !$('#chkActivarGrupoClientes').prop('checked'));
        $("#jsPropiedadesClientes").jsGrid("fieldOption", "U_SO1_ACTIVO", "visible", $('#chkActivarPropiedadesClientes').prop('checked'));
        $("#btnSeleccionarPropCliente").attr("disabled",  !$('#chkActivarPropiedadesClientes').prop('checked'));
        $("#btnDeseleccionarPropCliente").attr("disabled",  !$('#chkActivarPropiedadesClientes').prop('checked'));
        $("#ClienteY").attr("disabled",  !$('#chkActivarPropiedadesClientes').prop('checked'));
        $("#ClienteO").attr("disabled",  !$('#chkActivarPropiedadesClientes').prop('checked'));
        $("#chkClienteCE").attr("disabled",  !$('#chkActivarPropiedadesClientes').prop('checked'));


        $("#jsMembresias").jsGrid("fieldOption", "U_SO1_ACTIVO", "visible", $('#chkFiltrarMembresias').prop('checked'));
        $("#btnSeleccionarMembresias").attr("disabled",  !$('#chkActivarPropiedadesClientes').prop('checked'));
        $("#btnDeseleccionarMembresias").attr("disabled",  !$('#chkActivarPropiedadesClientes').prop('checked'));

        $("#jsPrecios").jsGrid("fieldOption", "U_SO1_ACTIVO", "visible", $('#chkFiltrarPrecios').prop('checked'));
        $("#btnSeleccionarPrecios").attr("disabled",  !$('#chkActivarPropiedadesClientes').prop('checked'));
        $("#btnDeseleccionarPrecios").attr("disabled",  !$('#chkActivarPropiedadesClientes').prop('checked'));

        $("#jsFormasPago").jsGrid("fieldOption", "U_SO1_ACTIVO", "visible", $('#chkFiltrarFormaPago').prop('checked'));
        $("#chkFiltrarVentaCredito").attr("disabled",  !$('#chkFiltrarFormaPago').prop('checked'));
        $("#btnSeleccionarPagos").attr("disabled",  !$('#chkFiltrarFormaPago').prop('checked'));
        $("#btnDeseleccionarPagos").attr("disabled",  !$('#chkFiltrarFormaPago').prop('checked'));
    }

    function checksPromocion(){
        ChecksDias ($('#chkDiario').prop('checked')); 
    }

    function checksIndividual(){

         $("#chkLunesTodo").attr("disabled",  $('#chkLunes').prop('checked'));
         $("#chkMartesTodo").attr("disabled",  $('#chkMartes').prop('checked'));
         $("#chkMiercolesTodo").attr("disabled", $('#chkMiercoles').prop('checked'));
         $("#chkJuevesTodo").attr("disabled",  $('#chkJueves').prop('checked'));
         $("#chkViernesTodo").attr("disabled",  $('#chkViernes').prop('checked'));
         $("#chkSabadoTodo").attr("disabled",  $('#chkSabado').prop('checked'));
         $("#chkDomingoTodo").attr("disabled", $('#chkDomingo').prop('checked'));
    }

    function checksTodoelDia(){

        //Diario
        if($("#DesdeDiario").val()=="00:00" &&  $("#HastaDiario").val()=="00:00")
        {
            $("#DesdeDiario").attr('disabled', true);
            $("#HastaDiario").attr('disabled', true);
        }
        else
        {
            $("#DesdeDiario").attr('disabled', false);
            $("#HastaDiario").attr('disabled', false);
        }
        //Lunes
        if ($("#DesdeLunes").val() == "00:00" && $("#HastaLunes").val() == "00:00") 
        {
            $("#DesdeLunes").attr('disabled', true);
            $("#HastaLunes").attr('disabled', true);
        }
        else 
        {
            $("#DesdeLunes").attr('disabled', false);
            $("#HastaLunes").attr('disabled', false);
        }
         //Martes
        if ($("#DesdeMartes").val() == "00:00" && $("#HastaMartes").val() == "00:00") 
        {
             $("#DesdeMartes").attr('disabled', true);
             $("#HastaMartes").attr('disabled', true);
        }
        else 
        {
             $("#DesdeMartes").attr('disabled', false);
             $("#HastaMartes").attr('disabled', false);
        }
        //Miercoles
        if ($("#DesdeMiercoles").val() == "00:00" && $("#HastaMiercoles").val() == "00:00") 
        {
            $("#DesdeMiercoles").attr('disabled', true);
            $("#HastaMiercoles").attr('disabled', true);
        }
        else 
        {
            $("#DesdeMiercoles").attr('disabled', false);
            $("#HastaMiercoles").attr('disabled', false);
        }
        //Jueves
        if ($("#DesdeJueves").val() == "00:00" && $("#HastaJueves").val() == "00:00") 
        {
            $("#DesdeJueves").attr('disabled', true);
            $("#HastaJueves").attr('disabled', true);
        }
        else 
        {
            $("#DesdeJueves").attr('disabled', false);
            $("#HastaJueves").attr('disabled', false);
        }
        //Viernes
        if ($("#DesdeViernes").val() == "00:00" && $("#HastaViernes").val() == "00:00") 
        {
            $("#DesdeViernes").attr('disabled', true);
            $("#HastaViernes").attr('disabled', true);
        }
        else 
        {
            $("#DesdeViernes").attr('disabled', false);
            $("#HastaViernes").attr('disabled', false);
        }
        //Sabado
         if ($("#DesdeSabado").val() == "00:00" && $("#HastaSabado").val() == "00:00") 
         {
             $("#DesdeSabado").attr('disabled', true);
             $("#HastaSabado").attr('disabled', true);
         }
         else 
         {
             $("#DesdeSabado").attr('disabled', false);
             $("#HastaSabado").attr('disabled', false);
         }
        //Domingo
        if($("#DesdeDomingo").val()=="00:00" &&  $("#HastaDomingo").val()=="00:00")
        {
            $("#DesdeDomingo").attr('disabled', true);
            $("#HastaDomingo").attr('disabled', true);
        }
        else
        {
            $("#DesdeDomingo").attr('disabled', false);
            $("#HastaDomingo").attr('disabled', false);
        }
        
        
    }

    function ChecksDias (valor){

        $("#chkLunes").attr("disabled", valor);
        $("#chkMartes").attr("disabled", valor);
        $("#chkMiercoles").attr("disabled", valor);
        $("#chkJueves").attr("disabled", valor);
        $("#chkViernes").attr("disabled", valor);
        $("#chkSabado").attr("disabled", valor);
        $("#chkDomingo").attr("disabled", valor);

        $("#chkDiarioTodo").attr("disabled", !valor);
        $("#chkDiarioTodo").prop("checked", valor);
        $("#DesdeDiario").attr("disabled",  valor);
        $("#HastaDiario").attr("disabled", valor);
        
        if(valor)
        {
            if( $("#DesdeDiario").val()!="00:00" &&  $("#HastaDiario").val()!="00:00")
            {
                $("#chkDiarioTodo").prop("checked", !valor);
            }
            //Acticar y desactivar los check de los dias de la semana
            $("#chkLunes").prop("checked", !valor);
            $("#chkMartes").prop("checked", !valor);
            $("#chkMiercoles").prop("checked", !valor);
            $("#chkJueves").prop("checked", !valor);
            $("#chkViernes").prop("checked", !valor);
            $("#chkSabado").prop("checked", !valor);
            $("#chkDomingo").prop("checked", !valor);

            $("#DesdeLunes").attr("disabled", valor);
            $("#HastaLunes").attr("disabled", valor);
            $("#chkLunesTodo").attr("disabled", valor);
            $('#chkLunesTodo').prop('checked',!valor); 
            $("#DesdeLunes").val("00:00");
            $("#HastaLunes").val("00:00");

            $("#DesdeMartes").attr("disabled", valor);
            $("#HastaMartes").attr("disabled", valor);
            $("#chkMartesTodo").attr("disabled", valor);
            $('#chkMartesTodo').prop('checked',!valor);
            $("#DesdeMartes").val("00:00");
            $("#HastaMartes").val("00:00");

            $("#DesdeMiercoles").attr("disabled", valor);
            $("#HastaMiercoles").attr("disabled", valor);
            $("#chkMiercolesTodo").attr("disabled", valor);
            $('#chkMiercolesTodo').prop('checked',!valor); 
            $("#DesdeMiercoles").val("00:00");
            $("#HastaMiercoles").val("00:00");

            $("#chkJuevesTodo").attr("disabled", valor);
            $("#DesdeJueves").attr("disabled", valor);
            $("#HastaJueves").attr("disabled", valor);
            $('#chkJuevesTodo').prop('checked',!valor);
            $("#DesdeJueves").val("00:00");
            $("#HastaJueves").val("00:00");

            $("#chkViernesTodo").attr("disabled", valor);
            $("#DesdeViernes").attr("disabled", valor);
            $("#HastaViernes").attr("disabled", valor);
            $('#chkViernesTodo').prop('checked',!valor); 
            $("#DesdeViernes").val("00:00");
            $("#HastaViernes").val("00:00");

            $("#chkSabadoTodo").attr("disabled", valor);
            $("#DesdeSabado").attr("disabled", valor);
            $("#HastaSabado").attr("disabled", valor);
            $('#chkSabadoTodo').prop('checked',!valor); 
            $("#DesdeSabado").val("00:00");
            $("#HastaSabado").val("00:00");
            
            $("#chkDomingoTodo").attr("disabled", valor);
            $("#DesdeDomingo").attr("disabled", valor);
            $("#HastaDomingo").attr("disabled", valor);
            $('#chkDomingoTodo').prop('checked',!valor); 
            $("#DesdeDomingo").val("00:00");
            $("#HastaDomingo").val("00:00");
        }
        else
        {
            $("#chkDiarioTodo").prop("checked", valor)
            $("#chkDiarioTodo").attr("disabled", !valor);
            $("#DesdeDiario").attr("disabled", !valor);
            $("#HastaDiario").attr("disabled", !valor);
            $("#DesdeDiario").val("00:00");
            $("#HastaDiario").val("00:00");
        }
        
        $("#chkLunesTodo").attr("disabled", valor?true:!$('#chkLunes').prop('checked'));
        $("#chkMartesTodo").attr("disabled", valor?true:!$('#chkMartes').prop('checked'));
        $("#chkMiercolesTodo").attr("disabled", valor?true:!$('#chkMiercoles').prop('checked'));
        $("#chkJuevesTodo").attr("disabled", valor?true:!$('#chkJueves').prop('checked'));
        $("#chkViernesTodo").attr("disabled", valor?true:!$('#chkViernes').prop('checked'));
        $("#chkSabadoTodo").attr("disabled", valor?true:!$('#chkSabado').prop('checked'));
        $("#chkDomingoTodo").attr("disabled", valor?true:!$('#chkDomingo').prop('checked'));
    }

    /* ------------------------*/

    /*------MostrarDatos-------*/

    function MostrarDatosPromocion(datos) {

        /* Codigo, nombre y fechas */
        if(datos.Existe)
        {
            $("#txtCodigo").val(datos.Codigo);
            $("#txtNombre").val(datos.Nombre);
        }
        else{
            if(datos.Codigo == "@@@@@@" || datos.Nombre == "@@@@@@"){
                $("#txtCodigo").val("");
                $("#txtNombre").val("");
            }
        }
        
        $("#txtFechaInicio").val(datos.U_SO1_FECHADESDE);
        $("#txtFechaFin").val(datos.U_SO1_FECHAHASTA);
        /* Checks de Vigencia */

        $('#chkDiario').attr('disabled', false);
        $('#chkDiario').prop('checked', datos.U_SO1_DIARIO == "Y" ? true : false);
        $('#chkDiarioTodo').prop('checked', datos.U_SO1_DIACOMPLETO == "Y" ? true : false);
        $("#DesdeDiario").val(creaHora(datos.U_SO1_DIAHORAINI));
        $("#HastaDiario").val(creaHora(datos.U_SO1_DIAHORAFIN));
        $('#chkDiarioTodo').attr('disabled', ($('#chkDiario').prop('checked') ? false : true));
       
        $('#chkLunes').attr('disabled', false );
        $('#chkLunes').prop('checked', datos.U_SO1_LUNES == "Y" ? true : false);
        $('#chkLunesTodo').prop('checked', datos.U_SO1_LUNCOMPLETO == "Y" ? true : false);
        $("#DesdeLunes").val(creaHora(datos.U_SO1_LUNHORAINI));
        $("#HastaLunes").val(creaHora(datos.U_SO1_LUNHORAFIN));
        $('#chkLunesTodo').attr('disabled', ($('#chkLunes').prop('checked') ? false : true));
       
        $('#chkMartes').attr('disabled', false );
        $('#chkMartes').prop('checked', datos.U_SO1_MARTES == "Y" ? true : false);
        $('#chkMartesTodo').prop('checked', datos.U_SO1_MARCOMPLETO == "Y" ? true : false);
        $("#DesdeMartes").val(creaHora(datos.U_SO1_MARHORAINI));
        $("#HastaMartes").val(creaHora(datos.U_SO1_MARHORAFIN));
        $('#chkMartesTodo').attr('disabled', ($('#chkMartes').prop('checked') ? false : true));
      
        $('#chkMiercoles').attr('disabled', false);
        $('#chkMiercoles').prop('checked', datos.U_SO1_MIERCOLES == "Y" ? true : false);
        $('#chkMiercolesTodo').prop('checked', datos.U_SO1_MIECOMPLETO == "Y" ? true : false);
        $("#DesdeMiercoles").val(creaHora(datos.U_SO1_MIEHORAINI));
        $("#HastaMiercoles").val(creaHora(datos.U_SO1_MIEHORAFIN));
        $('#chkMiercolesTodo').attr('disabled', ($('#chkMiercoles').prop('checked') ? true : false));
        
        $('#chkJueves').attr('disabled', false);
        $('#chkJueves').prop('checked', datos.U_SO1_JUEVES == "Y" ? true : false);
        $('#chkJuevesTodo').prop('checked', datos.U_SO1_JUECOMPLETO == "Y" ? true : false);
        $("#DesdeJueves").val(creaHora(datos.U_SO1_JUEHORAINI));
        $("#HastaJueves").val(creaHora(datos.U_SO1_JUEHORAFIN));
        $('#chkJuevesTodo').attr('disabled', ($('#chkJueves').prop('checked') ? false : true));
      
        $('#chkViernes').attr('disabled', false);
        $('#chkViernes').prop('checked', datos.U_SO1_VIERNES == "Y" ? true : false);
        $('#chkViernesTodo').prop('checked', datos.U_SO1_VIECOMPLETO == "Y" ? true : false);
        $("#DesdeViernes").val(creaHora(datos.U_SO1_VIEHORAINI));
        $("#HastaViernes").val(creaHora(datos.U_SO1_VIEHORAFIN));
        $('#chkViernesTodo').attr('disabled', ($('#chkViernes').prop('checked') ? false : true));
    
        $('#chkSabado').attr('disabled', false);
        $('#chkSabado').prop('checked', datos.U_SO1_SABADO == "Y" ? true : false);
        $('#chkSabadoTodo').prop('checked', datos.U_SO1_SABCOMPLETO == "Y" ? true : false);
        $("#DesdeSabado").val(creaHora(datos.U_SO1_SABHORAINI));
        $("#HastaSabado").val(creaHora(datos.U_SO1_SABHORAFIN));
        $('#chkSabadoTodo').attr('disabled', ($('#chkSabado').prop('checked') ? false : true));
       
        $('#chkDomingo').attr('disabled',  false );
        $('#chkDomingo').prop('checked', datos.U_SO1_DOMINGO == "Y" ? true : false);
        $('#chkDomingoTodo').prop('checked', datos.U_SO1_DOMCOMPLETO == "Y" ? true : false);
        $("#DesdeDomingo").val(creaHora(datos.U_SO1_DOMHORAINI));
        $("#HastaDomingo").val(creaHora(datos.U_SO1_DOMHORAFIN));
        $('#chkDomingoTodo').attr('disabled', ($('#chkDomingo').prop('checked') ? false : true));
      
        /* Checks de General */

        $('#chkAcumMonto').prop('checked', datos.U_SO1_ACUMPROMORM == "Y" ? true : false);
        $('#chkAcumPuntos').prop('checked', datos.U_SO1_ACUMPROMOPUNTO == "Y" ? true : false);
        $('#chkAcumProm').prop('checked', datos.U_SO1_ACUMPROMOOTRAS == "Y" ? true : false);
        $('#chkLimVenta').prop('checked', datos.U_SO1_ACTLIMPIEZAVEN == "Y" ? true : false);
        $('#chkPzsLimProm').prop('checked', datos.U_SO1_ACTLIMPIEZAPRO == "Y" ? true : false);
        $('#chkVenLimProm').prop('checked', datos.U_SO1_ACTLIMVENTAPRO == "Y" ? true : false);
        $("#txtLimVenta").val(formatear(datos.U_SO1_LIMITEPIEZAVEN,"Cantidad"));
        $("#txtPzsLimProm").val(formatear(datos.U_SO1_LIMITEPIEZAPRO,"Cantidad"));
        $("#txtVenLimProm").val(formatear(datos.U_SO1_LIMITEVENTAPRO,"Cantidad"));


        /* Datos de Promocion */
        if(datos.PromocionAxB != null)
        {
            $("#txtPzsPagar").val(formatear(datos.PromocionAxB.U_SO1_PIEZASAPAGAR,"Cantidad"));
            $('#chkAgregarRegalo').prop('checked', datos.PromocionAxB.U_SO1_REGALOPRECIO1 == "Y" ? true : false);
            $("#txtPzsTotales").val(formatear(datos.PromocionAxB.U_SO1_PIEZASTOTALES,"Cantidad"));
            $('#chkMantProm').prop('checked', datos.PromocionAxB.U_SO1_MANTENERDESC == "Y" ? true : false);
        }
            

        /* Check de los Articulos */
        $('#chkActivarArticulo').prop('checked', datos.U_SO1_ARTICULOENLACE == "Y" ? true : false);
        if(datos.U_SO1_ARTICULOENLACE == "O")
        {
            $('#chkOBien').prop('checked',  true);
        }
        else{
            $('#chkY').prop('checked',  true);
        }
        $('#chkActivarArticulo').prop('checked', datos.U_SO1_FILTRARARTART == "Y" ? true : false);
        //ObjetoPromocion.U_SO1_FILTRARARTART = $('#chkActivarArticulo').is(":checked") ? "Y" : "N";
        $('#chkActivarGrupo').prop('checked', datos.U_SO1_FILTRARARTGRU == "Y" ? true : false);
        $('#chkActivarPropiedades').prop('checked', datos.U_SO1_FILTRARARTPRO == "Y" ? true : false);
        $('#chkCEPropiedadesArticulo').prop('checked', datos.U_SO1_FILTRARARTPROC == "Y" ? true : false);
        if(datos.U_SO1_FILTRARARTPROE == "O")
        {
            $('#relacionOArticulo').prop('checked',  true);
        }
        else{
            $('#relacionYArticulo').prop('checked',  true);
        }
        
        $('#chkActivarCondicionEspecial').prop('checked', datos.U_SO1_FILTRARARTCOE == "Y" ? true : false);
        $("#txtCampo").val(datos.U_SO1_FILTRARARTCOEC);
        $("#txtValor").val(datos.U_SO1_FILTRARARTCOEV);

        $('#chkActivarConsultaEspecial').prop('checked', datos.U_SO1_FILTRARARTCON == "Y" ? true : false);
        $("#txtConsulta").val(datos.U_SO1_FILTRARARTCONV);

        $('#chkActivarProveedor').prop('checked', datos.U_SO1_FILTRARARTPROV == "Y" ? true : false);
        $('#chkActivarFabricante').prop('checked', datos.U_SO1_FILTRARARTFAB == "Y" ? true : false);
        
        $('#chkActivarExcepciones').prop('checked', datos.U_SO1_FILTRARARTEXC == "Y" ? true : false);
        $('#chkActivarUnidades').prop('checked', datos.U_SO1_FILTRARARTUNE == "Y" ? true : false);



        /* Checks de Sucursales */
        $('#chkFiltrarSucursales').prop('checked', datos.U_SO1_FILTRARPROMSUC == "Y" ? true : false);

        /* Checks de Alianzas */
        $('#chkFiltrarAlianzas').prop('checked', datos.U_SO1_FILTRARPROMALI == "Y" ? true : false);

        /* Checks de Clientes */
        $('#chkClientes').prop('checked', datos.U_SO1_FILTRARPROMCLI == "Y" ? true : false);
        
        /*Tabs clientes*/
        $('#btnTabClientes').addClass(datos.U_SO1_FILTRARPROMCLI == "Y" ? "enable" : "disabled");
        $('#btnTabGrupo').addClass(datos.U_SO1_FILTRARPROMCLI == "Y" ? "enable" : "disabled");
        $('#btnTabPropiedad').addClass(datos.U_SO1_FILTRARPROMCLI == "Y" ? "enable" : "disabled");

        $('#chkActivarClientes').attr('disabled', datos.U_SO1_FILTRARPROMCLI == "Y" ? false : true);
        $('#chkActivarClientesCE').attr('disabled', datos.U_SO1_FILTRARPROMCLI == "Y" ? false : true);
        $('#chkActivarGrupoClientes').attr('disabled', datos.U_SO1_FILTRARPROMCLI == "Y" ? false : true);
        $('#chkActivarPropiedadesClientes').attr('disabled', datos.U_SO1_FILTRARPROMCLI == "Y" ? false : true);
        $('#PropiedadClienteY').attr('disabled', datos.U_SO1_FILTRARPROMCLI == "Y" ? false : true);
        $('#PropiedadClienteO').attr('disabled', datos.U_SO1_FILTRARPROMCLI == "Y" ? false : true);



        if(datos.U_SO1_FILTRARCLIENLS == "O"){
            $('#CondicionClienteO').prop('checked', true);
        }
        else{
            $('#CondicionClienteY').prop('checked', true);
        }

        $('#chkActivarClientes').prop('checked', datos.U_SO1_FILTRARCLICLI == "Y" ? true : false);
        $('#btn_agregar_cliente').attr('disabled', !$('#chkActivarClientes').prop('checked'));
        $('#txtClienteCodigo').attr('disabled', !$('#chkActivarClientes').prop('checked'));
        $('#txtClienteNombre').attr('disabled', !$('#chkActivarClientes').prop('checked'));



        $('#chkActivarClientesCE').prop('checked', datos.U_SO1_FILTRARCLICOE == "Y" ? true : false);
        $("#txtClienteCampo").val(datos.U_SO1_FILTRARCLICOEC);
        $("#txtClienteValor").val(datos.U_SO1_FILTRARCLICOEV);
        $('#txtClienteCampo').attr('disabled', !$('#chkActivarClientesCE').prop('checked'));
        $('#txtClienteValor').attr('disabled', !$('#chkActivarClientesCE').prop('checked'));



        $('#chkActivarGrupoClientes').prop('checked', datos.U_SO1_FILTRARCLIGRU == "Y" ? true : false);
        $('#btnSeleccionarGrupoCliente').attr('disabled', !$('#chkActivarGrupoClientes').prop('checked'));
        $('#btnDeseleccionarGrupoCliente').attr('disabled', !$('#chkActivarGrupoClientes').prop('checked'));



        $('#chkActivarPropiedadesClientes').prop('checked', datos.U_SO1_FILTRARCLIPRO == "Y" ? true : false);
        $('#btnSeleccionarPropCliente').attr('disabled', !$('#chkActivarPropiedadesClientes').prop('checked'));
        $('#btnDeseleccionarPropCliente').attr('disabled', !$('#chkActivarPropiedadesClientes').prop('checked'));
        

        if(datos.U_SO1_FILTRARCLIPROE == "O"){
            $('#PropiedadClienteO').prop('checked', true);
        }
        else{
            $('#PropiedadClienteY').prop('checked', true);
        }

        $('#chkClienteCE').prop('checked', datos.U_SO1_FILTRARCLIPROC == "Y" ? true : false);

        /* Checks de Membresias */
        $('#chkFiltrarMembresias').prop('checked', datos.U_SO1_FILTRARPROMMEM == "Y" ? true : false);

        /* Checks de Filtrar Precios */
        $('#chkFiltrarPrecios').prop('checked', datos.U_SO1_FILTRARPROMLIS == "Y" ? true : false);

        /* Checks de Formas de Pago */
        $('#chkFiltrarFormaPago').prop('checked', datos.U_SO1_FILTRARPROMFOP == "Y" ? true : false);
        $('#chkFiltrarVentaCredito').prop('checked', datos.U_SO1_ACTVENTACREDIT == "Y" ? true : false);
    }

    function listarSucursales(sucursales) {
        $("#jsSucursales").jsGrid("destroy");
        $("#jsSucursales").jsGrid("loadData");
        $("#jsSucursales").jsGrid({
            height: "100%",
            width: "100%",
            sorting: true,
            editing: false,
            paging: (sessionStorage.getItem("ReportesPaginado").toUpperCase() == 'Y' ? true : false),
            autoload: true,
            pagerContainer: "#externalPagerSucursales",
            pagerFormat: "Paginas: {first} {prev} {pages} {next} {last} &nbsp;&nbsp; {pageIndex} de {pageCount} &nbsp;&nbsp; total de registros: {itemCount}",
            pagerFormat: "Paginas: {first} {prev} {pages} {next} {last} &nbsp;&nbsp; {pageIndex} de {pageCount} &nbsp;&nbsp; total de registros: {itemCount}",
            pagePrevText: "<span data-translate='cr1axb170'>Ant.</span>",
            pageNextText: "<span data-translate='cr1axb171'>Sig.</span>",
            pageFirstText: "<span data-translate='cr1axb172'>Primero</span>",
            pageLastText: "<span data-translate='cr1axb173'>Último</span>",
            noDataContent: "<span data-translate='cr1axb174'>No se encontraron resultados</span>",
            fields: [
                            { name: "U_SO1_SUCURSAL", title: "<span data-translate='cr1axb5'>Código</span>", type: "text", width: 50 },
                            { name: "Nombre", title: "<span data-translate='cr1axb6'>Nombre</span>", type: "text", width: 150 },
                            { name: "U_SO1_ACTIVO", title: "<span data-translate='cr1axb124'>Activo</span>", width: 65,
                                itemTemplate: function (value, item) {
                                    var $result = jsGrid.fields.control.prototype.itemTemplate.apply(this, arguments);
                                    var $customButton;
                                    if (item.U_SO1_ACTIVO == 'Y') {
                                        $customButton = $("<div class='switch' name='autorizaciones'><label><span data-translate='cr1axb126'>No</span><input type='checkbox' checked><span class='lever'></span><span data-translate='cr1axb125'>Si</span></label></div>")
                                              .click(function (e) {
                                                  var vAutorizacion = item;
                                                  vAutorizacion.U_SO1_ACTIVO = 'N';
                                                  $("#jsSucursales").jsGrid("updateItem", vAutorizacion);
                                                  e.stopPropagation();
                                                  loadTranslated(sessionStorage.getItem("lang"));
                                                  return false;
                                              });
                                    }
                                    else {
                                        $customButton = $("<div class='switch' name='autorizaciones'><label><span data-translate='cr1axb126'>No</span><input type='checkbox'><span class='lever'></span><span data-translate='cr1axb125'>Si</span></label></div>")
                                          .click(function (e) {
                                              var vAutorizacion = item;
                                              vAutorizacion.U_SO1_ACTIVO = 'Y';
                                              $("#jsSucursales").jsGrid("updateItem", vAutorizacion);
                                              e.stopPropagation();
                                              loadTranslated(sessionStorage.getItem("lang"));
                                              return false;
                                          });
                                    }
                                    return $result.add($customButton);
                                }
                            },
                            { type: "control", _createFilterSwitchButton: function () {
                                return this._createOnOffSwitchButton("filtering", this.searchModeButtonClass, false);
                            },
                                modeSwitchButton: true, editButton: false, deleteButton: false, width: 20
                            }
                        ],

            controller: cargarSucursales(sucursales)

        });
    }

    function cargarSucursales(sucursales) {
        var vController = {
            loadData: function (filter) {
                return $.grep(sucursales, function (autorizacion) {
                    return (!filter.U_SO1_SUCURSAL || autorizacion.U_SO1_SUCURSAL.indexOf(filter.U_SO1_SUCURSAL) > -1)
                    && (!filter.Nombre || autorizacion.Nombre.indexOf(filter.Nombre) > -1);
                });
            },

            insertItem: function (insertingSucursal) {
                this.push(insertingSucursal);
            }

        };
        return vController;
    }

    function listarArticulos(articulos) {
        $("#jsArticulos").jsGrid("destroy");
        $("#jsArticulos").jsGrid("loadData");
        $("#jsArticulos").jsGrid({
            height: "100%",
            width: "100%",
            sorting: true,
            editing: false,
            autoload: true,
            paging: (sessionStorage.getItem("ReportesPaginado").toUpperCase() == 'Y' ? true : false),
            pagerContainer: "#externalPagerArticulos",
            pagerFormat: "Paginas: {first} {prev} {pages} {next} {last} &nbsp;&nbsp; {pageIndex} de {pageCount} &nbsp;&nbsp; total de registros: {itemCount}",
            pagePrevText: "<span data-translate='cr1axb170'>Ant.</span>",
            pageNextText: "<span data-translate='cr1axb171'>Sig.</span>",
            pageFirstText: "<span data-translate='cr1axb172'>Primero</span>",
            pageLastText: "<span data-translate='cr1axb173'>Último</span>",
            noDataContent: "<span data-translate='cr1axb174'>No se encontraron resultados</span>",
            confirmDeleting: false,
            fields: [
                        { name: "U_SO1_ARTICULO", title: "<span data-translate='cr1axb5'>Código</span>", type: "text", width: 50 },
                        { name: "Nombre", title: "<span data-translate='cr1axb6'>Nombre</span>", type: "text", width: 150},
                        { name: "Eliminar", title: "<span data-translate=''></span>", width: 30,
                            itemTemplate: function (value, item) {
                                var $result = jsGrid.fields.control.prototype.itemTemplate.apply(this, arguments);
                                //var $customButton = $("<div class='button'><label>Eliminar</label></div>")
                                var $customButton = $("<a class='waves-effect red btn'><i class='material-icons center'>cancel</i></a>")
                                    .click(function (e) {
                                        if(!noPreguntar)
                                        {
                                            itemBorrar = item;
                                            tipoItem = "Articulo";
                                            $("#mdlConfirmarEliminar").modal('open');
                                        }
                                        else{
                                            $("#jsArticulos").jsGrid("deleteItem", item);
                                        }
                                        //$("#jsArticulos").jsGrid("deleteItem", item);
                                        //var vAutorizacion = item;
                                        //vAutorizacion.U_SO1_ACTIVO = 'N';
                                        //$("#jsFabricantes").jsGrid("updateItem", vAutorizacion);
                                        loadTranslated(sessionStorage.getItem("lang"));
                                        e.stopPropagation();
                                        return false;
                                    });
                                return $result.add($customButton);
                                }
                        },
                    ],

            controller: cargarArticulos(articulos)

        });
    }

    function cargarArticulos(articulos) {
        var vController = {
            loadData: function (filter) {
                return $.grep(articulos, function (autorizacion) {
                    return (!filter.U_SO1_ARTICULO || autorizacion.U_SO1_ARTICULO.indexOf(filter.U_SO1_ARTICULO) > -1)
                    && (!filter.Nombre || autorizacion.Nombre.indexOf(filter.Nombre) > -1);
                });
            },

            insertItem: function (item) {
               
            }

        };
        return vController;
    }
   
    function listarClientes(clientes) {
        $("#jsClientes").jsGrid("destroy");
        $("#jsClientes").jsGrid("loadData");
        $("#jsClientes").jsGrid({
            height: "100%",
            width: "100%",
            sorting: true,
            editing: false,
            paging: (sessionStorage.getItem("ReportesPaginado").toUpperCase() == 'Y' ? true : false),
            autoload: true,
            pagerContainer: "#externalPagerClientes",
            pagerFormat: "Paginas: {first} {prev} {pages} {next} {last} &nbsp;&nbsp; {pageIndex} de {pageCount} &nbsp;&nbsp; total de registros: {itemCount}",
            pagePrevText: "<span data-translate='cr1axb170'>Ant.</span>",
            pageNextText: "<span data-translate='cr1axb171'>Sig.</span>",
            pageFirstText: "<span data-translate='cr1axb172'>Primero</span>",
            pageLastText: "<span data-translate='cr1axb173'>Último</span>",
            noDataContent: "<span data-translate='cr1axb174'>No se encontraron resultados</span>",
            confirmDeleting: false,
            fields: [
                            { name: "U_SO1_CLIENTE", title: "<span data-translate='cr1axb5'>Código</span>", type: "text", width: 50 },
                            { name: "Nombre", title: "<span data-translate='cr1axb6'>Nombre</span>", type: "text", width: 150 },
                            { name: "Eliminar", title: "<span data-translate=''></span>", width: 30,
                                itemTemplate: function (value, item) {
                                    var $result = jsGrid.fields.control.prototype.itemTemplate.apply(this, arguments);
                                    var $customButton = $("<a class='waves-effect red btn'><i class='material-icons center'>cancel</i></a>")
                                        .click(function (e) {
                                            if(!noPreguntar)
                                            {
                                                itemBorrar = item;
                                                tipoItem = "Cliente";
                                                $("#mdlConfirmarEliminar").modal('open');
                                            }
                                            else{
                                                $("#jsClientes").jsGrid("deleteItem", item);
                                            }
                                            loadTranslated(sessionStorage.getItem("lang"));
                                            e.stopPropagation();
                                            return false;
                                        });
                                    return $result.add($customButton);
                                }
                            },
                        ],

            controller: cargarClientes(clientes)

        });
    }

    function cargarClientes(clientes) {
        var vController = {
            loadData: function (filter) {
                return $.grep(clientes, function (autorizacion) {
                    return (!filter.U_SO1_CLIENTE || autorizacion.U_SO1_CLIENTE.indexOf(filter.U_SO1_CLIENTE) > -1)
                    && (!filter.Nombre || autorizacion.Nombre.indexOf(filter.Nombre) > -1);
                });
            },

            insertItem: function (clientes) {
              
            }

        };
        return vController;
    }

    function listarExcepciones(excepciones) {
        $("#jsExcepciones").jsGrid("destroy");
        $("#jsExcepciones").jsGrid("loadData");
        $("#jsExcepciones").jsGrid({
            height: "100%",
            width: "100%",
            sorting: true,
            paging: (sessionStorage.getItem("ReportesPaginado").toUpperCase() == 'Y' ? true : false),
            editing: false,
            autoload: true,
            pagerContainer: "#externalPagerExcepciones",
            pagerFormat: "Paginas: {first} {prev} {pages} {next} {last} &nbsp;&nbsp; {pageIndex} de {pageCount} &nbsp;&nbsp; total de registros: {itemCount}",
            pagePrevText: "<span data-translate='cr1axb170'>Ant.</span>",
            pageNextText: "<span data-translate='cr1axb171'>Sig.</span>",
            pageFirstText: "<span data-translate='cr1axb172'>Primero</span>",
            pageLastText: "<span data-translate='cr1axb173'>Último</span>",
            noDataContent: "<span data-translate='cr1axb174'>No se encontraron resultados</span>",
            confirmDeleting: false,
            fields: [
                            { name: "U_SO1_ARTICULO", title: "<span data-translate='cr1axb5'>Código</span>", type: "text", width: 50 },
                            { name: "Nombre", title: "<span data-translate='cr1axb6'>Nombre</span>", type: "text", width: 150 },
                            { name: "Eliminar", title: "<span data-translate=''></span>", width: 30,
                                itemTemplate: function (value, item) {
                                    var $result = jsGrid.fields.control.prototype.itemTemplate.apply(this, arguments);
                                    var $customButton = $("<a class='waves-effect red btn'><i class='material-icons center'>cancel</i></a>")
                                        .click(function (e) {
                                            if(!noPreguntar)
                                            {
                                                itemBorrar = item;
                                                tipoItem = "Excepcion";
                                                $("#mdlConfirmarEliminar").modal('open');
                                            }
                                            else{
                                                $("#jsExcepciones").jsGrid("deleteItem", item);
                                            }
                                            loadTranslated(sessionStorage.getItem("lang"));
                                            e.stopPropagation();
                                            return false;
                                        });
                                    return $result.add($customButton);
                                }
                            },
                        ],

            controller: cargarExcepciones(excepciones)

        });
    }

    function cargarExcepciones(excepciones) {
        var vController = {
            loadData: function (filter) {
                return $.grep(excepciones, function (autorizacion) {
                    return (!filter.U_SO1_ARTICULO || autorizacion.U_SO1_ARTICULO.indexOf(filter.U_SO1_ARTICULO) > -1)
                    && (!filter.Nombre || autorizacion.Nombre.indexOf(filter.Nombre) > -1);
                });
            },

            insertItem: function (excepciones) {
             
            }

        };
        return vController;
    }

    function listarFabricantes(fabricantes) {
        $("#jsFabricantes").jsGrid("destroy");
        $("#jsFabricantes").jsGrid("loadData");
        $("#jsFabricantes").jsGrid({
            height: "100%",
            width: "100%",
            sorting: true,
            editing: false,
            paging: (sessionStorage.getItem("ReportesPaginado").toUpperCase() == 'Y' ? true : false),
            autoload: true,
            pagerContainer: "#externalPagerFabricantes",
            pagerFormat: "Paginas: {first} {prev} {pages} {next} {last} &nbsp;&nbsp; {pageIndex} de {pageCount} &nbsp;&nbsp; total de registros: {itemCount}",
            pagePrevText: "<span data-translate='cr1axb170'>Ant.</span>",
            pageNextText: "<span data-translate='cr1axb171'>Sig.</span>",
            pageFirstText: "<span data-translate='cr1axb172'>Primero</span>",
            pageLastText: "<span data-translate='cr1axb173'>Último</span>",
            noDataContent: "<span data-translate='cr1axb174'>No se encontraron resultados</span>",
            fields: [
                            { name: "U_SO1_FABRICANTE", title: "<span data-translate='cr1axb5'>Código</span>", type: "text", width: 30 },
                            { name: "Nombre", title: "<span data-translate='cr1axb6'>Nombre</span>", type: "text", width: 150 },
                            { name: "U_SO1_ACTIVO", title: "<span data-translate='cr1axb124'>Activo</span>", width: 70,
                                itemTemplate: function (value, item) {
                                    var $result = jsGrid.fields.control.prototype.itemTemplate.apply(this, arguments);
                                    var $customButton;
                                    if (item.U_SO1_ACTIVO == 'Y') {                                    
                                        $customButton = $("<div class='switch' name='autorizaciones'><label><span data-translate='cr1axb126'>No</span><input type='checkbox' checked><span class='lever'></span><span data-translate='cr1axb125'>Si</span></label></div>")
                                              .click(function (e) {
                                                  var vAutorizacion = item;
                                                  vAutorizacion.U_SO1_ACTIVO = 'N';
                                                  $("#jsFabricantes").jsGrid("updateItem", vAutorizacion);
                                                  loadTranslated(sessionStorage.getItem("lang"));
                                                  e.stopPropagation();
                                                  return false;
                                              });
                                    }
                                    else {
                                        $customButton = $("<div class='switch' name='autorizaciones'><label><span data-translate='cr1axb126'>No</span><input type='checkbox'><span class='lever'></span><span data-translate='cr1axb125'>Si</span></label></div>")
                                          .click(function (e) {
                                              var vAutorizacion = item;
                                              vAutorizacion.U_SO1_ACTIVO = 'Y';
                                              $("#jsFabricantes").jsGrid("updateItem", vAutorizacion);
                                              e.stopPropagation();
                                              loadTranslated(sessionStorage.getItem("lang"));
                                              return false;
                                          });
                                    }
                                    return $result.add($customButton);
                                }
                            },
                            { type: "control", _createFilterSwitchButton: function () {
                                return this._createOnOffSwitchButton("filtering", this.searchModeButtonClass, false);
                            },
                                modeSwitchButton: true, editButton: false, deleteButton: false, width: 20
                            }
                        ],

            controller: cargarFabricantes(fabricantes)

        });
    }

    function cargarFabricantes(fabricantes) {
        var vController = {
            loadData: function (filter) {
                return $.grep(fabricantes, function (autorizacion) {
                    return (!filter.U_SO1_FABRICANTE || autorizacion.U_SO1_FABRICANTE.indexOf(filter.U_SO1_FABRICANTE) > -1)
                    && (!filter.Nombre || autorizacion.Nombre.indexOf(filter.Nombre) > -1);
                });
            },

            insertItem: function (fabricantes) {
             
            }

        };
        return vController;
    }

    function listarAlianzas(alianzas) {
        $("#jsAlianzas").jsGrid("destroy");
        $("#jsAlianzas").jsGrid("loadData");
        $("#jsAlianzas").jsGrid({
            height: "100%",
            width: "100%",
            sorting: true,
            editing: false,
            autoload: true,
            paging: (sessionStorage.getItem("ReportesPaginado").toUpperCase() == 'Y' ? true : false),
            pagerContainer: "#externalPagerAlianzas",
            pagerFormat: "Paginas: {first} {prev} {pages} {next} {last} &nbsp;&nbsp; {pageIndex} de {pageCount} &nbsp;&nbsp; total de registros: {itemCount}",
            pagePrevText: "<span data-translate='cr1axb170'>Ant.</span>",
            pageNextText: "<span data-translate='cr1axb171'>Sig.</span>",
            pageFirstText: "<span data-translate='cr1axb172'>Primero</span>",
            pageLastText: "<span data-translate='cr1axb173'>Último</span>",
            noDataContent: "<span data-translate='cr1axb174'>No se encontraron resultados</span>",
            rowClick: function (datos) {
            
            },
            fields: [
                            { name: "U_SO1_ALIANZA", title: "<span data-translate='cr1axb5'>Código</span>", type: "text", width: 50 },
                            { name: "U_SO1_NOMBRE", title: "<span data-translate='cr1axb6'>Nombre</span>", type: "text", width: 150 },
                            { name: "U_SO1_ACTIVO", title: "<span data-translate='cr1axb124'>Activo</span>", width: 65,
                                itemTemplate: function (value, item) {
                                    var $result = jsGrid.fields.control.prototype.itemTemplate.apply(this, arguments);
                                    var $customButton;
                                    if (item.U_SO1_ACTIVO == 'Y') {
                                        $customButton = $("<div class='switch' name='autorizaciones'><label><span data-translate='cr1axb126'>No</span><input type='checkbox' checked><span class='lever'></span><span data-translate='cr1axb125'>Si</span></label></div>")
                                              .click(function (e) {
                                                  var vAutorizacion = item;
                                                  vAutorizacion.U_SO1_ACTIVO = 'N';
                                                  $("#jsAlianzas").jsGrid("updateItem", vAutorizacion);
                                                  e.stopPropagation();
                                                  loadTranslated(sessionStorage.getItem("lang"));
                                                  return false;
                                              });
                                    }
                                    else {
                                        $customButton = $("<div class='switch' name='autorizaciones'><label><span data-translate='cr1axb126'>No</span><input type='checkbox'><span class='lever'></span><span data-translate='cr1axb125'>Si</span></label></div>")
                                          .click(function (e) {
                                              var vAutorizacion = item;
                                              vAutorizacion.U_SO1_ACTIVO = 'Y';
                                              $("#jsAlianzas").jsGrid("updateItem", vAutorizacion);
                                              e.stopPropagation();
                                              loadTranslated(sessionStorage.getItem("lang"));
                                              return false;
                                          });
                                    }
                                    return $result.add($customButton);
                                }
                            },
                            { type: "control", _createFilterSwitchButton: function () {
                                return this._createOnOffSwitchButton("filtering", this.searchModeButtonClass, false);
                            },
                                modeSwitchButton: true, editButton: false, deleteButton: false, width: 20
                            }
                        ],

            controller: cargarAlianzas(alianzas)

        });
    }

    function cargarAlianzas(alianzas) {
        var vController = {
            loadData: function (filter) {
                return $.grep(alianzas, function (autorizacion) {
                    return (!filter.U_SO1_ALIANZA || autorizacion.U_SO1_ALIANZA.indexOf(filter.U_SO1_ALIANZA) > -1)
                    && (!filter.U_SO1_NOMBRE || autorizacion.U_SO1_NOMBRE.indexOf(filter.U_SO1_NOMBRE) > -1);
                });
            },

            insertItem: function (alianzas) {
                this.push(alianzas);
            }

        };
        return vController;
    }

    function listarMembresias(membresias) {
        $("#jsMembresias").jsGrid("destroy");
        $("#jsMembresias").jsGrid("loadData");
        $("#jsMembresias").jsGrid({
            height: "100%",
            width: "100%",
            sorting: true,
            editing: false,
            autoload: true,
            paging: (sessionStorage.getItem("ReportesPaginado").toUpperCase() == 'Y' ? true : false),
            pagerContainer: "#externalPagerMembresias",
            pagerFormat: "Paginas: {first} {prev} {pages} {next} {last} &nbsp;&nbsp; {pageIndex} de {pageCount} &nbsp;&nbsp; total de registros: {itemCount}",
            pagePrevText: "<span data-translate='cr1axb170'>Ant.</span>",
            pageNextText: "<span data-translate='cr1axb171'>Sig.</span>",
            pageFirstText: "<span data-translate='cr1axb172'>Primero</span>",
            pageLastText: "<span data-translate='cr1axb173'>Último</span>",
            noDataContent: "<span data-translate='cr1axb174'>No se encontraron resultados</span>",
            rowClick: function (datos) {
              
            },
            fields: [
                            { name: "U_SO1_TIPOMEMBRESIA", title: "<span data-translate='cr1axb5'>Código</span>", type: "text", width: 50 },
                            { name: "U_SO1_NOMBREMEMBRESIA", title: "<span data-translate='cr1axb6'>Nombre</span>", type: "text", width: 150 },
                           { name: "U_SO1_ACTIVO", title: "<span data-translate='cr1axb124'>Activo</span>", width: 65,
                               itemTemplate: function (value, item) {
                                   var $result = jsGrid.fields.control.prototype.itemTemplate.apply(this, arguments);
                                   var $customButton;
                                   if (item.U_SO1_ACTIVO == 'Y') {
                                       $customButton = $("<div class='switch' name='autorizaciones'><label><span data-translate='cr1axb126'>No</span><input type='checkbox' checked><span class='lever'></span><span data-translate='cr1axb125'>Si</span></label></div>")
                                              .click(function (e) {
                                                  var vAutorizacion = item;
                                                  vAutorizacion.U_SO1_ACTIVO = 'N';
                                                  $("#jsMembresias").jsGrid("updateItem", vAutorizacion);
                                                  e.stopPropagation();
                                                  loadTranslated(sessionStorage.getItem("lang"));
                                                  return false;
                                              });
                                      
                                   }
                                   else {
                                   
                                       $customButton = $("<div class='switch' name='autorizaciones'><label><span data-translate='cr1axb126'>No</span><input type='checkbox'><span class='lever'></span><span data-translate='cr1axb125'>Si</span></label></div>")
                                          .click(function (e) {
                                              var vAutorizacion = item;
                                              vAutorizacion.U_SO1_ACTIVO = 'Y';
                                              $("#jsMembresias").jsGrid("updateItem", vAutorizacion);
                                              e.stopPropagation();
                                              loadTranslated(sessionStorage.getItem("lang"));
                                              return false;
                                          });
                                      
                                   }
                                   return $result.add($customButton);
                               }
                           },
                            { type: "control", _createFilterSwitchButton: function () {
                                return this._createOnOffSwitchButton("filtering", this.searchModeButtonClass, false);
                            },
                                modeSwitchButton: true, editButton: false, deleteButton: false, width: 20
                            }
                        ],

            controller: cargarMembresias(membresias)

        });
    }

    function cargarMembresias(membresias) {
        var vController = {
            loadData: function (filter) {
                return $.grep(membresias, function (autorizacion) {
                    return (!filter.U_SO1_TIPOMEMBRESIA || autorizacion.U_SO1_TIPOMEMBRESIA.indexOf(filter.U_SO1_TIPOMEMBRESIA) > -1)
                    && (!filter.U_SO1_NOMBREMEMBRESIA || autorizacion.U_SO1_NOMBREMEMBRESIA.indexOf(filter.U_SO1_NOMBREMEMBRESIA) > -1);
                });
            },

            insertItem: function (insertinMembresias) {
                this.push(insertingMembresias);
            }

        };
        return vController;
    }

    function listarPrecios(precios) {
        $("#jsPrecios").jsGrid("destroy");
        $("#jsPrecios").jsGrid("loadData");
        $("#jsPrecios").jsGrid({
            height: "100%",
            width: "100%",
            sorting: true,
            editing: false,
            autoload: true,
            paging: (sessionStorage.getItem("ReportesPaginado").toUpperCase() == 'Y' ? true : false),
            pagerContainer: "#externalPagerPrecios",
            pagerFormat: "Paginas: {first} {prev} {pages} {next} {last} &nbsp;&nbsp; {pageIndex} de {pageCount} &nbsp;&nbsp; total de registros: {itemCount}",
            pagePrevText: "<span data-translate='cr1axb170'>Ant.</span>",
            pageNextText: "<span data-translate='cr1axb171'>Sig.</span>",
            pageFirstText: "<span data-translate='cr1axb172'>Primero</span>",
            pageLastText: "<span data-translate='cr1axb173'>Último</span>",
            noDataContent: "<span data-translate='cr1axb174'>No se encontraron resultados</span>",
            rowClick: function (datos) {
           
            },
            fields: [
                            { name: "U_SO1_LISTAPRECIO", title: "<span data-translate='cr1axb5'>Código</span>", type: "text", width: 50 },
                            { name: "ListName", title: "<span data-translate='cr1axb6'>Nombre</span>", type: "text", width: 150 },
                            { name: "U_SO1_ACTIVO", title: "<span data-translate='cr1axb124'>Activo</span>", width: 65,
                                itemTemplate: function (value, item) {
                                    var $result = jsGrid.fields.control.prototype.itemTemplate.apply(this, arguments);
                                    var $customButton;
                                    if (item.U_SO1_ACTIVO == 'Y') {
                                        $customButton = $("<div class='switch' name='autorizaciones'><label><span data-translate='cr1axb126'>No</span><input type='checkbox' checked><span class='lever'></span><span data-translate='cr1axb125'>Si</span></label></div>")
                                              .click(function (e) {
                                                  var vAutorizacion = item;
                                                  vAutorizacion.U_SO1_ACTIVO = 'N';
                                                  $("#jsPrecios").jsGrid("updateItem", vAutorizacion);
                                                  e.stopPropagation();
                                                  loadTranslated(sessionStorage.getItem("lang"));
                                                  return false;
                                              });
                                    }
                                    else {
                                        $customButton = $("<div class='switch' name='autorizaciones'><label><span data-translate='cr1axb126'>No</span><input type='checkbox'><span class='lever'></span><span data-translate='cr1axb125'>Si</span></label></div>")
                                          .click(function (e) {
                                              var vAutorizacion = item;
                                              vAutorizacion.U_SO1_ACTIVO = 'Y';
                                              $("#jsPrecios").jsGrid("updateItem", vAutorizacion);
                                              e.stopPropagation();
                                              loadTranslated(sessionStorage.getItem("lang"));
                                              return false;
                                          });
                                    }
                                    return $result.add($customButton);
                                }
                            },
                            { type: "control", _createFilterSwitchButton: function () {
                                return this._createOnOffSwitchButton("filtering", this.searchModeButtonClass, false);
                            },
                                modeSwitchButton: true, editButton: false, deleteButton: false, width: 20
                            }
                        ],

            controller: cargarPrecios(precios)

        });
    }

    function cargarPrecios(precios) {
        var vController = {
            loadData: function (filter) {
                return $.grep(precios, function (autorizacion) {
                    return (!filter.U_SO1_LISTAPRECIO || autorizacion.U_SO1_LISTAPRECIO.indexOf(filter.U_SO1_LISTAPRECIO) > -1)
                    && (!filter.ListName || autorizacion.ListName.indexOf(filter.ListName) > -1);
                });
            },

            insertItem: function (insertinMembresias) {
                this.push(insertingMembresias);
            }

        };
        return vController;
    }

    function listarFormasPago(formaspago) {
        $("#jsFormasPago").jsGrid("destroy");
        $("#jsFormasPago").jsGrid("loadData");
        $("#jsFormasPago").jsGrid({
            height: "100%",
            width: "100%",
            editing: true,
            autoload: true,
            paging: (sessionStorage.getItem("ReportesPaginado").toUpperCase() == 'Y' ? true : false),
            pagerContainer: "#externalPagerFormasPago",
            pagerFormat: "Paginas: {first} {prev} {pages} {next} {last} &nbsp;&nbsp; {pageIndex} de {pageCount} &nbsp;&nbsp; total de registros: {itemCount}",
            pagePrevText: "<span data-translate='cr1axb170'>Ant.</span>",
            pageNextText: "<span data-translate='cr1axb171'>Sig.</span>",
            pageFirstText: "<span data-translate='cr1axb172'>Primero</span>",
            pageLastText: "<span data-translate='cr1axb173'>Último</span>",
            noDataContent: "<span data-translate='cr1axb174'>No se encontraron resultados</span>",
            onItemUpdating:function(args) {
                
                    if(!ValidarFPago(args.item.U_SO1_MINIMO,args.item.U_SO1_TIPO)) {
                
                        args.cancel = true;
                    }
               
            },
            onItemUpdated:function(args) {
                var tipo = args.item.U_SO1_TIPO == "P" ? "Porcentaje" : "Importe";
                var numero = formatoNumero(args.item.U_SO1_MINIMO, tipo)
                args.item.U_SO1_MINIMO = numero ? numero : formatoNumero(0, tipo);
                $("#jsFormasPago").jsGrid("refresh");
            },
            fields: [
                            { name: "U_SO1_FORMAPAGO", title: "<span data-translate='cr1axb5'>Código</span>", readOnly:true, type: "text", width: 50 },
                            { name: "Nombre", title: "<span data-translate='cr1axb6'>Nombre</span>", readOnly:true, type: "text", width: 150 },
                            { name: "U_SO1_MINIMO", title: "<span data-translate='cr1preuni127'>Mínimo</span>", type: "text", width: 150 ,
                            
                            itemTemplate: function (value,item) {
                             
                            var tipo = item.U_SO1_TIPO == "P" ? "Porcentaje" : "Importe";
                            var numero = formatoNumero(item.U_SO1_MINIMO, tipo)
                            item.U_SO1_MINIMO = numero ? numero : formatoNumero(0, tipo);
                             //$("#jsFormasPago").jsGrid("updateItem", item);
                                return item.U_SO1_MINIMO;
                                }
                            },
                            { name: "U_SO1_TIPO", title: "<span data-translate='cr1axb128'>Tipo</span>", type: "select", items: [ { Name: "Porcentaje", Id: "P" },{ Name: "Importe", Id: "I" }], valueField: "Id", textField: "Name", width: 150 },
                            { name: "U_SO1_ACTIVO", title: "<span data-translate='cr1axb124'>Activo</span>", width: 65,
                                itemTemplate: function (value, item) {
                                    var $result = jsGrid.fields.control.prototype.itemTemplate.apply(this, arguments);
                                    var $customButton;
                                    if (item.U_SO1_ACTIVO == 'Y') {                
                                        $customButton = $("<div class='switch' name='autorizaciones'><label><span data-translate='cr1axb126'>No</span><input type='checkbox' checked><span class='lever'></span><span data-translate='cr1axb125'>Si</span></label></div>")
                                              .click(function (e) {
                                                  var vAutorizacion = item;
                                                  vAutorizacion.U_SO1_ACTIVO = 'N';
                                                  $("#jsFormasPago").jsGrid("updateItem", vAutorizacion);
                                                  e.stopPropagation();
                                                  loadTranslated(sessionStorage.getItem("lang"));
                                                  return false;
                                              });
                                    }
                                    else {
                                        $customButton = $("<div class='switch' name='autorizaciones'><label><span data-translate='cr1axb126'>No</span><input type='checkbox'><span class='lever'></span><span data-translate='cr1axb125'>Si</span></label></div>")
                                          .click(function (e) {
                                              var vAutorizacion = item;
                                              vAutorizacion.U_SO1_ACTIVO = 'Y';
                                              $("#jsFormasPago").jsGrid("updateItem", vAutorizacion);
                                              e.stopPropagation();
                                              loadTranslated(sessionStorage.getItem("lang"));
                                              return false;
                                          });
                                    }
                                    return $result.add($customButton);
                                }
                            },
                            { type: "control", _createFilterSwitchButton: function () {
                                return this._createOnOffSwitchButton("filtering", this.searchModeButtonClass, false);
                            },
                                modeSwitchButton: true, editButton: false, deleteButton: false, width: 20
                            }
                        ],

            controller: cargarFormasPago(formaspago)

        });
    }
    
    function SeleccionFormasPago(seleccion)
    {
        var items = $("#jsFormasPago").jsGrid("option", "data");
        for (var i = 0, max = items.length; i < max; i++) 
        {
            var vPromocion = items[i];
            vPromocion.U_SO1_MINIMO = seleccion == "Y" ? 1 : 0 ;
            vPromocion.U_SO1_ACTIVO = seleccion;
            vPromocion.U_SO1_TIPO = seleccion == "Y" ? "P" : "" ;
            $("#jsFormasPago").jsGrid("updateItem", vPromocion);
        }
        $("#jsFormasPago").jsGrid("refresh");
    }

    function ValidarFPago(minimo,tipo){
        if(validarNumero(minimo))
        {
            if(tipo == "P")
            {
                if(minimo<0 || minimo>100)
                {
                    Notificacion('info', '<span data-translate="cr1axb168">Ingrese un porcentaje entre 0 y 100');
                    return false;
                }
            }
            else
            {
                if(minimo<0)
                {
                    Notificacion('info', '<span data-translate="cr1axb169">Ingrese un monto mayor a cero');
                    return false;
                }
            }
            return true;
        }
        else
        {
            return false;
        } 
        return false; 
    }
    
    function cargarFormasPago(formaspago) {
        var vController = {
            loadData: function (filter) {
                return $.grep(formaspago, function (autorizacion) {
                    return (!filter.U_SO1_FORMAPAGO || autorizacion.U_SO1_FORMAPAGO.indexOf(filter.U_SO1_FORMAPAGO) > -1)
                    && (!filter.Nombre || autorizacion.Nombre.indexOf(filter.Nombre) > -1)
                    && (!filter.U_SO1_MINIMO || autorizacion.U_SO1_MINIMO.indexOf(filter.U_SO1_MINIMO) > -1)
                    && (!filter.U_SO1_TIPO || autorizacion.U_SO1_TIPO.indexOf(filter.U_SO1_TIPO) > -1);
                });
            },

            insertItem: function (insertinMembresias) {
                this.push(insertingMembresias);
            }

        };
        return vController;
    }

    function listarGruposClientes(grupos) {
        $("#jsGruposClientes").jsGrid("destroy");
        $("#jsGruposClientes").jsGrid("loadData");
        $("#jsGruposClientes").jsGrid({
            height: "100%",
            width: "100%",
            sorting: true,
            editing: false,
            autoload: true,
            paging: (sessionStorage.getItem("ReportesPaginado").toUpperCase() == 'Y' ? true : false),
            pagerContainer: "#externalPagerGruposClientes",
            pagerFormat: "Paginas: {first} {prev} {pages} {next} {last} &nbsp;&nbsp; {pageIndex} de {pageCount} &nbsp;&nbsp; total de registros: {itemCount}",
            pagePrevText: "<span data-translate='cr1axb170'>Ant.</span>",
            pageNextText: "<span data-translate='cr1axb171'>Sig.</span>",
            pageFirstText: "<span data-translate='cr1axb172'>Primero</span>",
            pageLastText: "<span data-translate='cr1axb173'>Último</span>",
            noDataContent: "<span data-translate='cr1axb174'>No se encontraron resultados</span>",
            fields: [
                            { name: "U_SO1_GRUPO", title: "<span data-translate='cr1axb5'>Código</span>", type: "text", width: 50 },
                            { name: "Nombre", title: "<span data-translate='cr1axb6'>Nombre</span>", type: "text", width: 150 },
                            { name: "U_SO1_ACTIVO", title: "<span data-translate='cr1axb124'>Activo</span>", width: 65,
                                itemTemplate: function (value, item) {
                                    var $result = jsGrid.fields.control.prototype.itemTemplate.apply(this, arguments);
                                    var $customButton;
                                    if (item.U_SO1_ACTIVO == 'Y') {
                                        $customButton = $("<div class='switch' name='autorizaciones'><label><span data-translate='cr1axb126'>No</span><input type='checkbox' checked><span class='lever'></span><span data-translate='cr1axb125'>Si</span></label></div>")
                                              .click(function (e) {
                                                  var vAutorizacion = item;
                                                  vAutorizacion.U_SO1_ACTIVO = 'N';
                                                  $("#jsGruposClientes").jsGrid("updateItem", vAutorizacion);
                                                  e.stopPropagation();
                                                  loadTranslated(sessionStorage.getItem("lang"));
                                                  return false;
                                              });
                                    }
                                    else {
                                        $customButton = $("<div class='switch' name='autorizaciones'><label><span data-translate='cr1axb126'>No</span><input type='checkbox'><span class='lever'></span><span data-translate='cr1axb125'>Si</span></label></div>")
                                          .click(function (e) {
                                              var vAutorizacion = item;
                                              vAutorizacion.U_SO1_ACTIVO = 'Y';
                                              $("#jsGruposClientes").jsGrid("updateItem", vAutorizacion);
                                              e.stopPropagation();
                                              loadTranslated(sessionStorage.getItem("lang"));
                                              return false;
                                          });
                                    }
                                    return $result.add($customButton);
                                }
                            },
                            { type: "control", _createFilterSwitchButton: function () {
                                return this._createOnOffSwitchButton("filtering", this.searchModeButtonClass, false);
                            },
                                modeSwitchButton: true, editButton: false, deleteButton: false, width: 20
                            }
                        ],

            controller: cargarGrupos(grupos)

        });
    }

    function listarGruposArticulos(grupos) {
        $("#jsArticulosGrupos").jsGrid("destroy");
        $("#jsArticulosGrupos").jsGrid("loadData");
        $("#jsArticulosGrupos").jsGrid({
            height: "100%",
            width: "100%",
            sorting: true,
            editing: false,
            paging: (sessionStorage.getItem("ReportesPaginado").toUpperCase() == 'Y' ? true : false),
            autoload: true,
            pagerContainer: "#externalPagerGrupos",
            pagerFormat: "Paginas: {first} {prev} {pages} {next} {last} &nbsp;&nbsp; {pageIndex} de {pageCount} &nbsp;&nbsp; total de registros: {itemCount}",
            pagePrevText: "<span data-translate='cr1axb170'>Ant.</span>",
            pageNextText: "<span data-translate='cr1axb171'>Sig.</span>",
            pageFirstText: "<span data-translate='cr1axb172'>Primero</span>",
            pageLastText: "<span data-translate='cr1axb173'>Último</span>",
            noDataContent: "<span data-translate='cr1axb174'>No se encontraron resultados</span>",
            fields: [
                            { name: "U_SO1_GRUPO", title: "<span data-translate='cr1axb5'>Código</span>", type: "text", width: 50 },
                            { name: "Nombre", title: "<span data-translate='cr1axb6'>Nombre</span>", type: "text", width: 150 },
                            { name: "U_SO1_ACTIVO", title: "<span data-translate='cr1axb124'>Activo</span>", width: 65,
                                itemTemplate: function (value, item) {
                                    var $result = jsGrid.fields.control.prototype.itemTemplate.apply(this, arguments);
                                    var $customButton;
                                    if (item.U_SO1_ACTIVO == 'Y') {
                                        $customButton = $("<div class='switch' name='autorizaciones'><label><span data-translate='cr1axb126'>No</span><input type='checkbox' checked><span class='lever'></span><span data-translate='cr1axb125'>Si</span></label></div>")
                                              .click(function (e) {
                                                  var vAutorizacion = item;
                                                  vAutorizacion.U_SO1_ACTIVO = 'N';
                                                  $("#jsArticulosGrupos").jsGrid("updateItem", vAutorizacion);
                                                  e.stopPropagation();
                                                  loadTranslated(sessionStorage.getItem("lang"));
                                                  return false;
                                              });
                                    }
                                    else {
                                        $customButton = $("<div class='switch' name='autorizaciones'><label><span data-translate='cr1axb126'>No</span><input type='checkbox'><span class='lever'></span><span data-translate='cr1axb125'>Si</span></label></div>")
                                          .click(function (e) {
                                              var vAutorizacion = item;
                                              vAutorizacion.U_SO1_ACTIVO = 'Y';
                                              $("#jsArticulosGrupos").jsGrid("updateItem", vAutorizacion);
                                              e.stopPropagation();
                                              loadTranslated(sessionStorage.getItem("lang"));
                                              return false;
                                          });
                                       }
                                    return $result.add($customButton);
                                }
                            },
                            { type: "control", _createFilterSwitchButton: function () {
                                return this._createOnOffSwitchButton("filtering", this.searchModeButtonClass, false);
                            },
                                modeSwitchButton: true, editButton: false, deleteButton: false, width: 20
                            }
                        ],

            controller: cargarGrupos(grupos)

        });
    }

    function cargarGrupos(grupos) {
        var vController = {
            loadData: function (filter) {
                return $.grep(grupos, function (autorizacion) {
                    return (!filter.U_SO1_GRUPO || autorizacion.U_SO1_GRUPO.indexOf(filter.U_SO1_GRUPO) > -1)
                    && (!filter.Nombre || autorizacion.Nombre.indexOf(filter.Nombre) > -1);
                });
            },

            insertItem: function (insertinMembresias) {
                this.push(insertingMembresias);
            }

        };
        return vController;
    }

    function listarPropiedadesClientes(propiedades) {
        $("#jsPropiedadesClientes").jsGrid("destroy");
        $("#jsPropiedadesClientes").jsGrid("loadData");
        $("#jsPropiedadesClientes").jsGrid({
            height: "100%",
            width: "100%",
            sorting: true,
            editing: false,
            autoload: true,
            paging: (sessionStorage.getItem("ReportesPaginado").toUpperCase() == 'Y' ? true : false),
            pagerContainer: "#externalPagerPropiedadesClientes",
            pagerFormat: "Paginas: {first} {prev} {pages} {next} {last} &nbsp;&nbsp; {pageIndex} de {pageCount} &nbsp;&nbsp; total de registros: {itemCount}",
            pagePrevText: "<span data-translate='cr1axb170'>Ant.</span>",
            pageNextText: "<span data-translate='cr1axb171'>Sig.</span>",
            pageFirstText: "<span data-translate='cr1axb172'>Primero</span>",
            pageLastText: "<span data-translate='cr1axb173'>Último</span>",
            noDataContent: "<span data-translate='cr1axb174'>No se encontraron resultados</span>",
            fields: [
                            { name: "U_SO1_PROPIEDAD", title: "<span data-translate='cr1axb5'>Código</span>", type: "text", width: 50 },
                            { name: "Nombre", title: "<span data-translate='cr1axb6'>Nombre</span>", type: "text", width: 150 },
                            { name: "U_SO1_ACTIVO", title: "<span data-translate='cr1axb124'>Activo</span>", width: 65,
                                itemTemplate: function (value, item) {
                                    var $result = jsGrid.fields.control.prototype.itemTemplate.apply(this, arguments);
                                    var $customButton;
                                    if (item.U_SO1_ACTIVO == 'Y') {
                                        $customButton = $("<div class='switch' name='autorizaciones'><label><span data-translate='cr1axb126'>No</span><input type='checkbox' checked><span class='lever'></span><span data-translate='cr1axb125'>Si</span></label></div>")
                                              .click(function (e) {
                                                  var vAutorizacion = item;
                                                  vAutorizacion.U_SO1_ACTIVO = 'N';
                                                  $("#jsPropiedadesClientes").jsGrid("updateItem", vAutorizacion);
                                                  e.stopPropagation();
                                                  loadTranslated(sessionStorage.getItem("lang"));
                                                  return false;
                                              });
                                      }
                                    else {
                                        $customButton = $("<div class='switch' name='autorizaciones'><label><span data-translate='cr1axb126'>No</span><input type='checkbox'><span class='lever'></span><span data-translate='cr1axb125'>Si</span></label></div>")
                                          .click(function (e) {
                                              var vAutorizacion = item;
                                              vAutorizacion.U_SO1_ACTIVO = 'Y';
                                              $("#jsPropiedadesClientes").jsGrid("updateItem", vAutorizacion);
                                              e.stopPropagation();
                                              loadTranslated(sessionStorage.getItem("lang"));
                                              return false;
                                          });
                                        }
                                    return $result.add($customButton);
                                }
                            },
                            { type: "control", _createFilterSwitchButton: function () {
                                return this._createOnOffSwitchButton("filtering", this.searchModeButtonClass, false);
                            },
                                modeSwitchButton: true, editButton: false, deleteButton: false, width: 20
                            }
                        ],

            controller: cargarPropiedades(propiedades)

        });
    }

    function listarPropiedadesArticulos(propiedades) {
        $("#jsArticulosPropiedades").jsGrid("destroy");
        $("#jsArticulosPropiedades").jsGrid("loadData");
        $("#jsArticulosPropiedades").jsGrid({
            height: "100%",
            width: "100%",
            sorting: true,
            editing: false,
            autoload: true,
            paging: (sessionStorage.getItem("ReportesPaginado").toUpperCase() == 'Y' ? true : false),
            pagerContainer: "#externalPagerPropiedades",
            pagerFormat: "Paginas: {first} {prev} {pages} {next} {last} &nbsp;&nbsp; {pageIndex} de {pageCount} &nbsp;&nbsp; total de registros: {itemCount}",
            pagePrevText: "<span data-translate='cr1axb170'>Ant.</span>",
            pageNextText: "<span data-translate='cr1axb171'>Sig.</span>",
            pageFirstText: "<span data-translate='cr1axb172'>Primero</span>",
            pageLastText: "<span data-translate='cr1axb173'>Último</span>",
            noDataContent: "<span data-translate='cr1axb174'>No se encontraron resultados</span>",
            fields: [
                            { name: "U_SO1_PROPIEDAD", title: "<span data-translate='cr1axb5'>Código</span>", type: "text", width: 50 },
                            { name: "Nombre", title: "<span data-translate='cr1axb6'>Nombre</span>", type: "text", width: 150 },
                            { name: "U_SO1_ACTIVO", title: "<span data-translate='cr1axb124'>Activo</span>", width: 65,
                                itemTemplate: function (value, item) {
                                    var $result = jsGrid.fields.control.prototype.itemTemplate.apply(this, arguments);
                                    var $customButton;
                                    if (item.U_SO1_ACTIVO == 'Y') {
                                        $customButton = $("<div class='switch' name='autorizaciones'><label><span data-translate='cr1axb126'>No</span><input type='checkbox' checked><span class='lever'></span><span data-translate='cr1axb125'>Si</span></label></div>")
                                              .click(function (e) {
                                                  var vAutorizacion = item;
                                                  vAutorizacion.U_SO1_ACTIVO = 'N';
                                                  $("#jsArticulosPropiedades").jsGrid("updateItem", vAutorizacion);
                                                  e.stopPropagation();
                                                  loadTranslated(sessionStorage.getItem("lang"));
                                                  return false;
                                              });
                                       }
                                    else {
                                        $customButton = $("<div class='switch' name='autorizaciones'><label><span data-translate='cr1axb126'>No</span><input type='checkbox'><span class='lever'></span><span data-translate='cr1axb125'>Si</span></label></div>")
                                          .click(function (e) {
                                              var vAutorizacion = item;
                                              vAutorizacion.U_SO1_ACTIVO = 'Y';
                                              $("#jsArticulosPropiedades").jsGrid("updateItem", vAutorizacion);
                                              e.stopPropagation();
                                              loadTranslated(sessionStorage.getItem("lang"));
                                              return false;
                                          });
                                      }
                                    return $result.add($customButton);
                                }
                            },
                            { type: "control", _createFilterSwitchButton: function () {
                                return this._createOnOffSwitchButton("filtering", this.searchModeButtonClass, false);
                            },
                                modeSwitchButton: true, editButton: false, deleteButton: false, width: 20
                            }
                        ],

            controller: cargarPropiedades(propiedades)

        });
    }

    function cargarPropiedades(propiedades) {
        var vController = {
            loadData: function (filter) {
                return $.grep(propiedades, function (autorizacion) {
                    return (!filter.U_SO1_PROPIEDAD || autorizacion.U_SO1_PROPIEDAD.indexOf(filter.U_SO1_PROPIEDAD) > -1)
                    && (!filter.Nombre || autorizacion.Nombre.indexOf(filter.Nombre) > -1);
                });
            },

            insertItem: function (insertinMembresias) {
                this.push(insertingMembresias);
            }

        };
        return vController;
    }

    function listarMedidasArticulos(medidas) {
        $("#jsArticulosUnidades").jsGrid("destroy");
        $("#jsArticulosUnidades").jsGrid("loadData");
        $("#jsArticulosUnidades").jsGrid({
            height: "100%",
            width: "100%",
            sorting: true,
            editing: false,
            autoload: true,
            paging: (sessionStorage.getItem("ReportesPaginado").toUpperCase() == 'Y' ? true : false),
            pagerContainer: "#externalPagerUnidades",
            pagerFormat: "Paginas: {first} {prev} {pages} {next} {last} &nbsp;&nbsp; {pageIndex} de {pageCount} &nbsp;&nbsp; total de registros: {itemCount}",
            pagePrevText: "<span data-translate='cr1axb170'>Ant.</span>",
            pageNextText: "<span data-translate='cr1axb171'>Sig.</span>",
            pageFirstText: "<span data-translate='cr1axb172'>Primero</span>",
            pageLastText: "<span data-translate='cr1axb173'>Último</span>",
            noDataContent: "<span data-translate='cr1axb174'>No se encontraron resultados</span>",
            fields: [
                            { name: "NombreGrupo", title: "<span data-translate='cr1axb129'>Grupo</span>", type: "text", width: 50 },
                            { name: "NombreUnidad", title: "<span data-translate='cr1axb130'>Unidad</span>", type: "text", width: 150 },
                            { name: "Cantidad", title: "<span data-translate='cr1axb131'>Cantidad</span>", type: "text", width: 50 },
                            { name: "U_SO1_ACTIVO", title: "<span data-translate='cr1axb124'>Activo</span>", width: 65,
                                itemTemplate: function (value, item) {
                                    var $result = jsGrid.fields.control.prototype.itemTemplate.apply(this, arguments);
                                    var $customButton;
                                    if (item.U_SO1_ACTIVO == 'Y') {
                                        $customButton = $("<div class='switch' name='autorizaciones'><label><span data-translate='cr1axb126'>No</span><input type='checkbox' checked><span class='lever'></span><span data-translate='cr1axb125'>Si</span></label></div>")
                                              .click(function (e) {
                                                  var vAutorizacion = item;
                                                  vAutorizacion.U_SO1_ACTIVO = 'N';
                                                  $("#jsArticulosUnidades").jsGrid("updateItem", vAutorizacion);
                                                  e.stopPropagation();
                                                  loadTranslated(sessionStorage.getItem("lang"));
                                                  return false;
                                              });
                                    }
                                    else {
                                        $customButton = $("<div class='switch' name='autorizaciones'><label><span data-translate='cr1axb126'>No</span><input type='checkbox'><span class='lever'></span><span data-translate='cr1axb125'>Si</span></label></div>")
                                          .click(function (e) {
                                              var vAutorizacion = item;
                                              vAutorizacion.U_SO1_ACTIVO = 'Y';
                                              $("#jsArticulosUnidades").jsGrid("updateItem", vAutorizacion);
                                              e.stopPropagation();
                                              loadTranslated(sessionStorage.getItem("lang"));
                                              return false;
                                          });
                                    }
                                    return $result.add($customButton);
                                }
                            },
                            { type: "control", _createFilterSwitchButton: function () {
                                return this._createOnOffSwitchButton("filtering", this.searchModeButtonClass, false);
                            },
                                modeSwitchButton: true, editButton: false, deleteButton: false, width: 20
                            }
                        ],

            controller: cargarUnidades(medidas)

        });
    }

    function cargarUnidades(medidas) {
        var vController = {
            loadData: function (filter) {
                return $.grep(medidas, function (autorizacion) {
                    return (!filter.NombreGrupo || autorizacion.NombreGrupo.indexOf(filter.NombreGrupo) > -1)
                    && (!filter.NombreUnidad || autorizacion.NombreUnidad.indexOf(filter.NombreUnidad) > -1)
                    && (!filter.Cantidad || autorizacion.Cantidad.indexOf(filter.Cantidad) > -1);
                });
            },

            insertItem: function (insertinMembresias) {
                this.push(insertingMembresias);
            }

        };
        return vController;
    }

    function listarProveedores(proveedores) {
        $("#jsProveedores").jsGrid("destroy");
        $("#jsProveedores").jsGrid("loadData");
        $("#jsProveedores").jsGrid({
            height: "100%",
            width: "100%",
            sorting: true,
            editing: false,
            autoload: true,
            paging: (sessionStorage.getItem("ReportesPaginado").toUpperCase() == 'Y' ? true : false),
            pagerContainer: "#externalPagerProveedores",
            pagerFormat: "Paginas: {first} {prev} {pages} {next} {last} &nbsp;&nbsp; {pageIndex} de {pageCount} &nbsp;&nbsp; total de registros: {itemCount}",
            pagePrevText: "<span data-translate='cr1axb170'>Ant.</span>",
            pageNextText: "<span data-translate='cr1axb171'>Sig.</span>",
            pageFirstText: "<span data-translate='cr1axb172'>Primero</span>",
            pageLastText: "<span data-translate='cr1axb173'>Último</span>",
            noDataContent: "<span data-translate='cr1axb174'>No se encontraron resultados</span>",
            confirmDeleting: false,
            fields: [
                            { name: "U_SO1_PROVEEDOR", title: "<span data-translate='cr1axb5'>Grupo</span>", type: "text", width: 30 },
                            { name: "Nombre", title: "<span data-translate='cr1axb6'>Activo</span>", width: 85},
                            { name: "Eliminar", title: "<span data-translate=''></span>", width: 30,
                                itemTemplate: function (value, item) {
                                    var $result = jsGrid.fields.control.prototype.itemTemplate.apply(this, arguments);
                                    var $customButton = $("<a class='waves-effect red btn'><i class='material-icons center'>cancel</i></a>")
                                        .click(function (e) {
                                            if(!noPreguntar)
                                            {
                                                itemBorrar = item;
                                                tipoItem = "Proveedor";
                                                $("#mdlConfirmarEliminar").modal('open');
                                            }
                                            else{
                                                $("#jsProveedores").jsGrid("deleteItem", item);
                                            }
                                            loadTranslated(sessionStorage.getItem("lang"));
                                            e.stopPropagation();
                                            return false;
                                        });
                                    return $result.add($customButton);
                                }
                            },
                        ],

            controller: cargarProveedores(proveedores)

        });
    }

    function cargarProveedores(proveedores) {
        var vController = {
            loadData: function (filter) {
                return $.grep(proveedores, function (autorizacion) {
                    return (!filter.U_SO1_PROVEEDOR || autorizacion.U_SO1_PROVEEDOR.indexOf(filter.U_SO1_PROVEEDOR) > -1)
                    && (!filter.Nombre || autorizacion.Nombre.indexOf(filter.Nombre) > -1);
                });
            },

            insertItem: function (insertinMembresias) {
            }

        };
        return vController;
    }

    /* ------------------------*/

    function filePicked(oEvent) {

        // Get The File From The Input
        var file = oEvent.target.files[0];
        if (file != null) {
            sFilename = file.name;
            sFileExt = sFilename.split('.').pop()
            if (sFileExt == "xls" || sFileExt == "xlsx") {
                // Create A File Reader HTML5
                var reader = new FileReader();
                // Ready The Event For When A File Gets Selected
                reader.onload = function (e) {
                    var excelData = [];
                    var data = e.target.result;
                    var workbook = XLSX.read(data, { type: 'binary' });
                    // Loop Over Each Sheet
                    workbook.SheetNames.forEach(function (sheetName) {
                        // Here is your object
                        // var sCSV = XLS.utils.make_csv(workbook.Sheets[sheetName]);
                        oFilasExcel = XLSX.utils.sheet_to_json(workbook.Sheets[sheetName], {header: "A"});
                        //oJson = JSON.stringify(oFilasExcel);
                        // $("#my_file_output").html(sCSV);
                        // console.log(json_object);
                        for (var i = 0; i < oFilasExcel.length; i++)
                        {
                            excelData.push(oFilasExcel[i]["A"]);
                        }
                        oJson = JSON.stringify(excelData);
                    })
                    $("#mdlConfirmacion").modal('open');
                };
                reader.onerror = function (ex) {
                    console.log(ex);
                };
                reader.readAsBinaryString(file);
            } else {
                Notificacion("info", "Archivo no soportado");
                resetArchivo();
            }
        }
    }

    function Importar(archivo, tipoImportacion) {
        if($("#jsArticulos").jsGrid("option", "data") == "" || $("#jsArticulos").jsGrid("option", "data") == null){
            tipoImportacion = false;
        }
        if (sFilename != null) {
            if (archivo != null) {
                    var param = {};
                    param["datos"] = archivo;
                    param["articulos"] = tipoImportacion == true ? JSON.stringify($("#jsArticulos").jsGrid("option", "data")) : null;
                    param["tipoImportacion"] = tipoImportacion;
                    loadingActive(true);
                    $.ajax({
                        type: "POST",
                        url: 'CR1PromocionAxB.aspx/ImportarArticulos',
                        data: JSON.stringify(param),
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        success: function (result) {
                            loadingActive(false);
                            var response = result["d"];
                            if (response[0] == "OK") {
                                listarArticulos(response[1]);

                                if(!response[2]){
                                    Notificacion('warning', '<span data-translate="cr1axb157">Algunos articulos no se agregaron. Mas informacion en Bitacora</span>');
                                }
                                
                                resetArchivo();
                            } else {
                                Notificacion('error', '<span data-translate="cr1axb158">Archivo No Importado.</span>');
                               
                                resetArchivo();
                            }
                            //Traducimos todo el portal.
                            loadTranslated(sessionStorage.getItem("lang"));
                        },
                        error: function (res, msg, code) {
                            // log the error to the console
                            console.log(msg);
                            loadingActive(false);
                            var vResponse = res.responseJSON;
                            if (vResponse != undefined) {
                                vResponse = res.responseJSON;
                                Notificacion('warning', vResponse['Message']);
                            }
                            else {
                                var rbody = /<body.*?>([\s\S]*)<\/body>/.exec(res.responseText)[1];
                                var rh2 = /<h2.*?>([\s\S]*)<\/h2>/.exec(rbody)[1];
                                var rError = /<i.*?>([\s\S]*)<\/i>/.exec(rh2)[1];
                                Notificacion('warning', rError);
                            }
                            //Traducimos todo el portal.
                            loadTranslated(sessionStorage.getItem("lang"));
                            resetArchivo();
                        }
                    });
            } else {
                Notificacion('info', '<span data-translate="cr1axb159">Archivo no soportado</span>');
                resetArchivo();
            }
        } else {
            Notificacion('info', '<span data-translate="cr1axb160">No se ha cargado ningun archivo</span>');
            resetArchivo();
        }
        loadTranslated(sessionStorage.getItem("lang"));
    }

    function resetArchivo() {
        wrapperRutaArchivo.value = null;
        oFileIn.value = null;
    }

    function loadingActive(valor) {
        if (valor) {
            $("#divLoader").removeClass("preloaded");
        }
        else {
            $("#divLoader").addClass("preloaded");
        }
    }
    function loadTranslated(sLang) {
        switch (sLang) {
            case "es":
                $("#btnEs").click();
                break;
            case "en":
                $("#btnEn").click();
                break;
            case "ar":
                $("#btnAr").click();
                break;
        }
    }

    function Notificacion(tipo, mensaje) {
        Lobibox.notify(tipo, {
            size: 'mini',
            msg: mensaje
        });
    };
})(jQuery);