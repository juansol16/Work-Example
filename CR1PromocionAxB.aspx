<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="CR1PromocionAxB.aspx.cs" Inherits="SO1_PR_ReportesWeb_V020000.CR1PromocionAxB" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <style type="text/css">
        .tabs 
        {
            position:inherit !important;
        }
        .so1-btn
        {
            margin: 10px;
        }
        .so1-tab2
        {
            border-radius: 6px !important;
            margin: 0 10px 0 0 !important;
            border: solid 1px #415F9D !important; /*position: relative;*/
            text-transform: none !important;
            position:none !import;
        }
        .so1-tab3
        {
            border-radius: 6px;
            border: 2px solid #415F9D;
            border-top: 2px solid white !important; /*position: relative;*/
            text-transform: none !important;
            position:none !import;
        }
        .so1-tab :not(.active)
        {
            /*color: #8108a7 !important;*/
            background: #0087b7;
            background: -webkit-linear-gradient(45deg, #0087b7 100%, #a53188 100%);
            background: linear-gradient(45deg, #0087b7 100%, #a53188 100%); /*color: White !important;
        font-weight: bold;*/
        }
        .so1-tab2 > a.active
        {
            color: white !important;
            font-weight: bold;
        }
        .so1-tab2 > a:not(.active)
        {
            color: #8108a7 !important;
        }
        .so1-tab2 .active
        {
            /*color: #8108a7 !important;*/
            background: #0087b7;
            background: -webkit-linear-gradient(45deg, #0087b7 0%, #a53188 100%);
            background: linear-gradient(45deg, #0087b7 0%, #a53188 100%); /*color: White !important; font-weight: bold;*/
        }
        .so1-tab2 > a:hover.active
        {
            background-color: #5a87dd !important;
            background: #5a87dd !important;
        }
        .tabs .indicator
        {
            /*background-color: #8108a7 !important;*/
            background-color: white !important;
        }
        .so1-btnseleccion
        {
            border: 1px solid #95398c;
            color: #95398c;
            padding-left: 5px;
            padding-right: 5px;
            font-size: 12px;
            margin: 10px;
            text-transform: none !important;
        }
        .picker__date-display { background-color: #0883B5; }
        .picker__close { color: #A33188 !important;}
        .picker__clear { color: #A33188 !important;}
        .clockpicker-tick:hover { background: #04a8ea !important;color:White; }
        .clockpicker-canvas line { stroke: #04a8ea !important; }
        .clockpicker-canvas-bearing { fill: #04a8ea !important; }
        .clockpicker-canvas-bg { fill: #04a8ea !important; }
        [type="radio"]:checked + label:after, [type="radio"].with-gap:checked + label:after {
            background-color: #0087B7;
        }
        [type="radio"]:checked + label:after, [type="radio"].with-gap:checked + label:before, [type="radio"].with-gap:checked + label:after {
            border: 2px solid #0087B7;
        }
    </style>
    <div id="breadcrumbs-wrapper">
        <div class="container">
            <div class="row">
                <div class="col s10 m6 l6">
                    <h5 class="breadcrumbs-title" data-translate="cr1axb1">
                        Reportes Web</h5>
                    <ol class="breadcrumbs">
                        <li><a href="Inicio.aspx" data-translate="cr1axb2">Inicio</a> </li>
                        <li><a href="#" data-translate="cr1axb3">Promociones</a> </li>
                        <li class="active" data-translate="cr1axb4">Promoción AxB</li>
                    </ol>
                </div>
            </div>
        </div>
    </div>
    <div class="container">
        <!-- Inline Form -->
        <div class="row">
            <ul class="collapsible collapsible-accordion card-panel" data-collapsible="accordion">
                <li>
                    <div class="collapsible-header active" id="cabeceraFiltros">
                        <i class="material-icons right-aligned" id="iconoCabecera">remove</i>
                        <label id="lblCabeceraFiltro" style="font-size: 1.64rem;">
                        </label>
                    </div>
                    <div class="collapsible-body">
                        <div class="row">
                            <div class="input-field col s6 l3 m3">
                                <div>
                                    <input id="txtCodigo" type="text" autocomplete="off"/>
                                </div>
                                <label id="lblCodigo" for="txtCodigo" class="active" data-translate="cr1axb5">
                                    Código</label>
                            </div>
                            <div class="input-field col s6 l3 m3">
                                <div>
                                    <input id="txtNombre" type="text" autocomplete="off"/>
                                </div>
                                <label id="lblNombre" for="txtNombre" class="active" data-translate="cr1axb6">
                                    Nombre</label>
                            </div>
                            <div class="input-field col s5 l2 m2">
                                <div>
                                    <input id="txtFechaInicio" type="text" class="flatpickr flatpickr-input" />
                                </div>
                                <label id="lblFechaInicio" for="txtFechaInicio" class="active" data-translate="cr1axb7">
                                    Fecha inicio</label>
                            </div>
                            <div class="input-field col s5 l2 m2">
                                <div>
                                    <input id="txtFechaFin" type="text" class="flatpickr flatpickr-input" />
                                </div>
                                <label id="lblFechaFin" for="txtFechaFin" class="active" data-translate="cr1axb8">
                                    Fecha fin</label>
                                    
                            </div>
                            <div class="input-field col s2 l1 m1">
                                <a id="btn_LimpiarFechas" class="btn red waves-effect waves-light left"
                                    style="width:40px; padding:0;"><i class='material-icons'>clear</i></a>
                               </div>

                            
                        </div>
                        <!--<div class="row">
                            <div class="input-field col s10 m10 l0">
                            </div>
                            <div class="input-field col s2 m2 l2">
                                <a id="btnBuscar" class="waves-effect waves-light btn blue right small-only-so1 so1-btn"
                                    style="margin-right: 1em;"><span data-translate="cr1axb9" class="hide-on-small-only" id="lblBuscar">
                                        Buscar </span><i class="material-icons show-on-small hide-on-med-and-up">search</i>
                                    <i class="material-icons right hide-on-small-only">search</i> </a>
                            </div>
                        </div> -->
                        <!-- Etiquetas para las traducciones del componente de reloj -->
                        <label id="traduccionOK" hidden for="txtFechaFin" class="active" data-translate="">OK</label>
                        <label id="traduccionLimpiar" hidden for="txtFechaFin" class="active" data-translate="">Limpiar</label>
                        <label id="traduccionCancelar" hidden for="txtFechaFin" class="active" data-translate="">Cancelar</label>
                    </div>
                </li>
            </ul>
        </div>
    </div>
    <div class="container">
        <div class="row">
            <ul class="collapsible collapsible-accordion card-panel" data-collapsible="accordion">
                <a id="btnCrear" class="waves-effect waves-light btn green right small-only-so1 so1-btn"
                    style="display: none; margin-right: 1em"><span data-translate="cr1axb10" class="hide-on-small-only"
                        id="lblGuardar">Guardar </span><i class="material-icons show-on-small hide-on-med-and-up">
                            save</i> <i class="material-icons right hide-on-small-only">save</i>
                </a><a id="btnCancelar" class="waves-effect waves-light btn orange right small-only-so1 so1-btn"
                    style="display: none; margin-right: 1em"><span data-translate="cr1axb11" class="hide-on-small-only"
                        id="lblCancelar">Regresar </span><i class="material-icons show-on-small hide-on-med-and-up">
                            cancel</i> <i class="material-icons right hide-on-small-only">cancel</i>
                </a>
                <br />
                <li>
                    <div class="collapsible-header active" id="cabeceraPromociones">
                        <i class="material-icons right-aligned" id="iconoPromociones">remove</i>
                        <label id="lblCabeceraPromociones" style="font-size: 1.64rem;">
                        </label>
                    </div>
                    <div class="collapsible-body">
                        <div class="container">
                            <div class="row" style="padding-bottom:1em">
                                <div id="tabsPrincipales" class="col s12">
                                    <ul class="tabs">
                                        <li class="tab col so1-tab2" style="width: 11%"><a class="active waves-effect waves-light so1-tab"
                                            href="#promocion" data-translate="cr1axb12">Promoción</a></li>
                                        <li class="tab col so1-tab2" style="width: 11%"><a class="waves-effect waves-light so1-tab"
                                            href="#articulos" data-translate="cr1axb13">Artículos</a></li>
                                        <li class="tab col so1-tab2" style="width: 11%"><a class="waves-effect waves-light so1-tab"
                                            href="#sucursales" data-translate="cr1axb14">Sucursales</a></li>
                                        <li class="tab col so1-tab2" style="width: 11%"><a class="waves-effect waves-light so1-tab"
                                            href="#alianzas" data-translate="cr1axb15">Alianzas</a></li>
                                        <li class="tab col so1-tab2" style="width: 11%"><a class="waves-effect waves-light so1-tab"
                                            href="#clientes" data-translate="cr1axb16">Clientes</a></li>
                                        <li class="tab col so1-tab2" style="width: 11%"><a class="waves-effect waves-light so1-tab"
                                            href="#membresias" data-translate="cr1axb17">Membresías</a></li>
                                        <li class="tab col so1-tab2" style="width: 11%"><a class="waves-effect waves-light so1-tab"
                                            href="#precios" data-translate="cr1axb18">Lista de precios</a></li>
                                        <li class="tab col so1-tab2"><a class="waves-effect waves-light so1-tab" href="#pago" data-translate="cr1axb19">
                                            Formas de pago</a></li>
                                    </ul>
                                </div>
                            </div>
                        </div>
                        <div class="container" id="promocion" style="margin-bottom: 1em">
                            <div class="card-panel">
                                <h6 data-translate="cr1axb12">
                                    Promoción</h6>
                                <div class="row">
                                    <div class="input-field col s4">
                                        <div>
                                            <input id="txtPzsTotales" type="text" autocomplete="off"/>
                                        </div>
                                        <label id="lblPzsTotales" for="txtPzsTotales" class="active" data-translate="cr1axb20">
                                            Piezas totales</label>
                                    </div>
                                    <div class="input-field col s4">
                                        <div>
                                            <input id="txtPzsPagar" type="text" autocomplete="off"/>
                                        </div>
                                        <label id="lblPzsPagar" for="txtPzsPagar" class="active" data-translate="cr1axb21">
                                            Piezas a pagar</label>
                                    </div>
                                    <div class="col s4">
                                        <input type="checkbox" class="filled-in" id="chkAgregarRegalo" />
                                        <label id="lblAgregarRegalo" for="chkAgregarRegalo" data-translate="cr1axb22">
                                            Agregar regalos con precio 1</label>
                                    </div>
                                </div>
                            </div>
                            <div class="card-panel">
                                <div class="row">
                                    <div class="col s6">
                                        <h6 data-translate="cr1axb23">
                                            Vigencia</h6>
                                        <div class="row">
                                            <div class="col s3">
                                            </div>
                                            <div class="col s3">
                                                <span data-translate="cr1axb24">Todo el dia</span>
                                            </div>
                                            <div class="col s3">
                                                <span data-translate="cr1axb25">Desde</span>
                                            </div>
                                            <div class="col s3">
                                                <span data-translate="cr1axb26">Hasta</span>
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="checkbox-field col s3">
                                                <input type="checkbox" class="filled-in" id="chkDiario" />
                                                <label id="lblDiario" for="chkDiario" data-translate="cr1axb27">
                                                    Diario</label>
                                            </div>
                                            <div class="checkbox-field col s3">
                                                <input type="checkbox" class="filled-in" id="chkDiarioTodo" />
                                                <label id="lblDiarioTodo" for="chkDiarioTodo" style="margin-left: 1rem;">
                                                </label>
                                            </div>
                                            <div class="col s3">
                                                <input type="text" id="DesdeDiario" class="timepicker" />
                                            </div>
                                            <div class="col s3">
                                                <input id="HastaDiario" type="text" class="timepicker" />
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="checkbox-field col s3">
                                                <input type="checkbox" class="filled-in" id="chkLunes" />
                                                <label id="lblLunes" for="chkLunes" data-translate="cr1axb28">
                                                    Lunes</label>
                                            </div>
                                            <div class="checkbox-field col s3">
                                                <input type="checkbox" class="filled-in" id="chkLunesTodo" />
                                                <label id="Label1" for="chkLunesTodo" style="margin-left: 1rem;">
                                                </label>
                                            </div>
                                            <div class="col s3">
                                                <input id="DesdeLunes" type="text" class="timepicker" />
                                            </div>
                                            <div class="col s3">
                                                <input id="HastaLunes" type="text" class="timepicker" />
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="checkbox-field col s3">
                                                <input type="checkbox" class="filled-in" id="chkMartes" />
                                                <label id="lblMartes" for="chkMartes" data-translate="cr1axb29">
                                                    Martes</label>
                                            </div>
                                            <div class="checkbox-field col s3">
                                                <input type="checkbox" class="filled-in" id="chkMartesTodo" />
                                                <label id="Label2" for="chkMartesTodo" style="margin-left: 1rem;">
                                                </label>
                                            </div>
                                            <div class="col s3">
                                                <input id="DesdeMartes" type="text" class="timepicker" />
                                            </div>
                                            <div class="col s3">
                                                <input id="HastaMartes" type="text" class="timepicker" />
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="checkbox-field col s3">
                                                <input type="checkbox" class="filled-in" id="chkMiercoles" />
                                                <label id="lblMiercoles" for="chkMiercoles" data-translate="cr1axb30">
                                                    Miércoles</label>
                                            </div>
                                            <div class="checkbox-field col s3">
                                                <input type="checkbox" class="filled-in" id="chkMiercolesTodo" />
                                                <label id="Label3" for="chkMiercolesTodo" style="margin-left: 1rem;">
                                                </label>
                                            </div>
                                            <div class="col s3">
                                                <input id="DesdeMiercoles" type="text" class="timepicker" />
                                            </div>
                                            <div class="col s3">
                                                <input id="HastaMiercoles" type="text" class="timepicker" />
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="checkbox-field col s3">
                                                <input type="checkbox" class="filled-in" id="chkJueves" />
                                                <label id="lblJueves" for="chkJueves" data-translate="cr1axb31">
                                                    Jueves</label>
                                            </div>
                                            <div class="checkbox-field col s3">
                                                <input type="checkbox" class="filled-in" id="chkJuevesTodo" />
                                                <label id="Label4" for="chkJuevesTodo" style="margin-left: 1rem;">
                                                </label>
                                            </div>
                                            <div class="col s3">
                                                <input id="DesdeJueves" type="text" class="timepicker" />
                                            </div>
                                            <div class="col s3">
                                                <input id="HastaJueves" type="text" class="timepicker" />
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="checkbox-field col s3">
                                                <input type="checkbox" class="filled-in" id="chkViernes" />
                                                <label id="lblViernes" for="chkViernes" data-translate="cr1axb32">
                                                    Viernes</label>
                                            </div>
                                            <div class="checkbox-field col s3">
                                                <input type="checkbox" class="filled-in" id="chkViernesTodo" />
                                                <label id="Label5" for="chkViernesTodo" style="margin-left: 1rem;">
                                                </label>
                                            </div>
                                            <div class="col s3">
                                                <input id="DesdeViernes" type="text" class="timepicker" />
                                            </div>
                                            <div class="col s3">
                                                <input id="HastaViernes" type="text" class="timepicker" />
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="checkbox-field col s3">
                                                <input type="checkbox" class="filled-in" id="chkSabado" />
                                                <label id="lblSabado" for="chkSabado" data-translate="cr1axb33">
                                                    Sábado</label>
                                            </div>
                                            <div class="checkbox-field col s3">
                                                <input type="checkbox" class="filled-in" id="chkSabadoTodo" />
                                                <label id="Label6" for="chkSabadoTodo" style="margin-left: 1rem;">
                                                </label>
                                            </div>
                                            <div class="col s3">
                                                <input id="DesdeSabado" type="text" class="timepicker" />
                                            </div>
                                            <div class="col s3">
                                                <input id="HastaSabado" type="text" class="timepicker" />
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="checkbox-field col s3">
                                                <input type="checkbox" class="filled-in" id="chkDomingo" />
                                                <label id="lblDomingo" for="chkDomingo" data-translate="cr1axb34">
                                                    Domingo</label>
                                            </div>
                                            <div class="checkbox-field col s3">
                                                <input type="checkbox" class="filled-in" id="chkDomingoTodo" />
                                                <label id="Label7" for="chkDomingoTodo" style="margin-left: 1rem;">
                                                </label>
                                            </div>
                                            <div class="col s3">
                                                <input id="DesdeDomingo" type="text" class="timepicker" />
                                            </div>
                                            <div class="col s3">
                                                <input id="HastaDomingo" type="text" class="timepicker" />
                                            </div>
                                        </div>
                                    </div>
                                    <div class="col s6">
                                        <h6>
                                            General</h6>
                                        <div class="row">
                                            <div class="checkbox-field col s6">
                                                <input type="checkbox" class="filled-in" id="chkAcumMonto" />
                                                <label id="Label8" for="chkAcumMonto" data-translate="cr1axb35">
                                                    Acumulable con regalo por monto</label>
                                            </div>
                                            <div class="col s4">
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="checkbox-field col s6">
                                                <input type="checkbox" class="filled-in" id="chkAcumPuntos" />
                                                <label id="Label9" for="chkAcumPuntos" data-translate="cr1axb36">
                                                    Acumulable con promoción de puntos</label>
                                            </div>
                                            <div class="col s4">
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="checkbox-field col s6">
                                                <input type="checkbox" class="filled-in" id="chkAcumProm" />
                                                <label id="Label10" for="chkAcumProm" data-translate="cr1axb37">
                                                    Acumulable con otras promociones (PU,PV,DE,DV)</label>
                                            </div>
                                            <div class="col s4">
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="checkbox-field col s6">
                                                <input type="checkbox" class="filled-in" id="chkMantProm" />
                                                <label id="Label11" for="chkMantProm" data-translate="cr1axb38">
                                                    Mantener descuento ajeno a promoción</label>
                                            </div>
                                            <div class="col s4">
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="checkbox-field col s6">
                                                <input type="checkbox" class="filled-in" id="chkLimVenta" />
                                                <label id="lblLimVenta" for="chkLimVenta" data-translate="cr1axb39">
                                                    Límite de aplicaciones por venta</label>
                                            </div>
                                            <div class="col s4">
                                                <input id="txtLimVenta" type="text" autocomplete="off"/>
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="checkbox-field col s6">
                                                <input type="checkbox" class="filled-in" id="chkPzsLimProm" />
                                                <label id="lblPzsLimProm" for="chkPzsLimProm" data-translate="cr1axb40">
                                                    Límite de piezas por promoción (Sucursal)</label>
                                            </div>
                                            <div class="col s4">
                                                <input id="txtPzsLimProm" type="text" autocomplete="off"/>
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="checkbox-field col s6">
                                                <input type="checkbox" class="filled-in" id="chkVenLimProm" />
                                                <label id="Label14" for="chkVenLimProm" data-translate="cr1axb41">
                                                    Límite de ventas por promoción (Sucursal)</label>
                                            </div>
                                            <div class="col s4">
                                                <input id="txtVenLimProm" type="text" autocomplete="off"/>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="container" id="articulos" class="col s12">
                            <div class="card-panel">
                                <div class="row">
                                    <div class="row">
                                        <div class="col s10">
                                            <h5 data-translate="cr1axb42">
                                                Artículos</h5>
                                        </div>
                                        <div class="col s2">
                                            <input name="group1" type="radio" id="chkY" />
                                            <label id="lblY" for="chkY" style="margin-right: 10px;" data-translate="cr1axb43">
                                                Y</label>
                                            <input name="group1" type="radio" id="chkOBien" />
                                            <label id="lblOBien" for="chkOBien" data-translate="cr1axb44">
                                                O bien</label>
                                        </div>
                                    </div>
                                    <div class="col s12" style="padding-bottom:1em">
                                        <ul class="tabs">
                                            <li class="tab col so1-tab2"><a class="active waves-effect waves-light so1-tab" href="#articulos2" data-translate="cr1axb42">
                                                Artículos</a></li>
                                            <li class="tab col so1-tab2"><a class="waves-effect waves-light so1-tab" href="#grupos" data-translate="cr1axb45">
                                                Grupos</a></li>
                                            <li class="tab col so1-tab2"><a class="waves-effect waves-light so1-tab" href="#propiedades" data-translate="cr1axb46">
                                                Propiedades</a></li>
                                            <li class="tab col so1-tab2"><a class="waves-effect waves-light so1-tab" href="#otros" data-translate="cr1axb47">
                                                Otros</a></li>
                                            <li class="tab col so1-tab2"><a class="waves-effect waves-light so1-tab" href="#excepciones" data-translate="cr1axb48">
                                                Excepciones</a></li>
                                            <li class="tab col so1-tab2"><a class="waves-effect waves-light so1-tab" href="#unidadesmedida" data-translate="cr1axb49">
                                                Unidades de medida de inventario </a></li>
                                        </ul>
                                    </div>
                                    <div id="articulos2" class="col s12">
                                        <div class="row" style="margin-top: 1em">
                                            <div class="col s10">
                                                <h6  data-translate="cr1axb50">
                                                    Lista de artículos</h6>
                                            </div>
                                            <div class="col s2">
                                                <input type="checkbox" class="filled-in" id="chkActivarArticulo" />
                                                <label id="lblActivarArticulo" for="chkActivarArticulo"  data-translate="cr1axb51">
                                                    Activar</label>
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="col s2">
                                                <label id="lbltxtArticuloCodigo" for="txtArticuloCodigo" data-translate="cr1axb5">
                                                    Código</label>
                                                <div>
                                                    <input id="txtArticuloCodigo" type="text" />
                                                </div>
                                            </div>
                                            <div class="col s3">
                                                <label id="lbltxtArticuloNombre" for="txtArticuloNombre" data-translate="cr1axb6">
                                                    Nombre</label>
                                                <div>
                                                    <input id="txtArticuloNombre" type="text" />
                                                </div>
                                            </div>
                                            <div class="s3">
                                                <a id="btn_agregar_articulo" class="waves-effect waves-light btn green left small-only-so1 so1-btn"
                                                    style="margin-right: 1em"><span data-translate="cr1axb52" class="hide-on-small-only">Agregar
                                                        artículo</span><i class="material-icons show-on-small hide-on-med-and-up">
                                                            add</i> <i class="material-icons right hide-on-small-only">add</i> </a>
  
                                            </div>
                                            <div class="file-field col s4">
                                                <div id="btn_importar_articulo" class="waves-effect waves-light btn blue left small-only-so1 so1-btn"
                                                    style="height: 36px; line-height: 36px;"><span data-translate="cr1axb53" class="hide-on-small-only" id="lblImportarArticulo">
                                                        Importar artículos</span> <i class="material-icons show-on-small hide-on-med-and-up">
                                                            archive</i> <i class="material-icons right hide-on-small-only">archive</i>
                                                            <input type = "file" id="inputExcel" accept="application/vnd.openxmlformats-officedocument.spreadsheetml.sheet, application/vnd.ms-excel"/>
                                                </div>
                                                <div class = "file-path-wrapper">
                                                    <input id="WrapperRutaArchivo" class = "file-path validate" type = "text" data-translate="" placeholder = "Subir Archívo" style="display: none;" />
                                                </div> 
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div id="externalPagerArticulos" class="external-pager">
                                            </div>
                                            <div id="jsArticulos">
                                            </div>
                                        </div>
                                    </div>
                                    <div id="grupos" class="col s12">
                                        <div class="row" style="margin-top: 1em">
                                            <div class="col s6">
                                                <h5 data-translate="cr1axb45">
                                                    Grupos</h5>
                                            </div>
                                            <div class="col s4">
                                                <input type="checkbox" class="filled-in" id="chkActivarGrupo" />
                                                <label id="lblActivarGrupo" for="chkActivarGrupo">
                                                    Activar</label>
                                                <a id="btnSeleccionarGruposArticulos" class="waves-effect waves-light btn white right so1-btnseleccion">
                                                    <span data-translate="cr1axb54">Seleccionar todo</span> </a><a id="btnDeseleccionarGruposArticulos"
                                                        class="waves-effect waves-light btn white right so1-btnseleccion"><span data-translate="cr1axb55">
                                                            Deseleccionar todo</span> </a>
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div id="externalPagerGrupos" class="external-pager">
                                            </div>
                                            <div id="jsArticulosGrupos">
                                            </div>
                                        </div>
                                    </div>
                                    <div id="propiedades" class="col s12">
                                        <div class="row" style="margin-top: 1em">
                                            <div class="col s3">
                                                <h6 data-translate="cr1axb56">
                                                    Propiedades de artículo</h6>
                                            </div>
                                            <div class="col s1">
                                                <input type="checkbox" class="filled-in" id="chkActivarPropiedades" />
                                                <label id="lblActivarPropiedades" for="chkActivarPropiedades" data-translate="cr1axb51">
                                                    Activar</label>
                                            </div>
                                            <div class="col s3">
                                                <input name="propiedadesArticulo" type="radio" id="relacionYArticulo" />
                                                <label id="Label12" for="relacionYArticulo" style="margin-right: 10px;" data-translate="cr1axb43">
                                                    Y</label>
                                                <input name="propiedadesArticulo" type="radio" id="relacionOArticulo"/>
                                                <label id="Label13" for="relacionOArticulo" data-translate="cr1axb44">
                                                    O Bien</label>
                                            </div>
                                            <div class="col s2">
                                                <input type="checkbox" class="filled-in" id="chkCEPropiedadesArticulo" />
                                                <label id="Label15" for="chkCEPropiedadesArticulo" data-translate="cr1axb57">
                                                    Coincidencia exacta</label>
                                            </div>
                                        </div>
                                        <div class="row" style="margin-top: 1em">
                                            <div class="col s8">
                                            </div>
                                            <div class="col s4">
                                                <a id="btnSeleccionarPropiedadesArticulos" class="waves-effect waves-light btn white right so1-btnseleccion">
                                                    <span data-translate="cr1axb54">Seleccionar todo</span> </a><a id="btnDeseleccionarPropiedadesArticulos"
                                                        class="waves-effect waves-light btn white right so1-btnseleccion"><span data-translate="cr1axb55">
                                                            Deseleccionar todo</span> </a>
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div id="externalPagerPropiedades" class="external-pager">
                                            </div>
                                            <div id="jsArticulosPropiedades">
                                            </div>
                                        </div>
                                    </div>
                                    <div id="otros" class="col s12">
                                        <div class="row" style="margin-top: 1em">
                                            <div class="col s4">
                                                <h6 data-translate="cr1axb58">
                                                    Condición especial</h6>
                                            </div>
                                            <div class="col s2">
                                                <input type="checkbox" class="filled-in" id="chkActivarCondicionEspecial" />
                                                <label id="lblActivarCondicionEspecial" for="chkActivarCondicionEspecial" data-translate="cr1axb51">
                                                    Activar</label>
                                            </div>
                                            <div class="col s4">
                                                <h6 data-translate="cr1axb59">
                                                    Consulta especial</h6>
                                            </div>
                                            <div class="col s2">
                                                <input type="checkbox" class="filled-in" id="chkActivarConsultaEspecial" />
                                                <label id="lblActivarConsultaEspecial" for="chkActivarConsultaEspecial" data-translate="cr1axb51">
                                                    Activar</label>
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="col s2">
                                                <label id="lblCampo" for="txtCampo" class="active" data-translate="cr1axb60">
                                                    Campo(OITM)</label>
                                                <div>
                                                    <input id="txtCampo" type="text" />
                                                </div>
                                            </div>
                                            <div class="col s4">
                                                <label id="lblValor" for="txtValor" class="active" data-translate="cr1axb61">
                                                    Valor</label>
                                                <div>
                                                    <input id="txtValor" type="text" />
                                                </div>
                                            </div>
                                            <div class="col s6">
                                                <label id="lblConsulta" for="txtConsulta" class="active" data-translate="cr1axb62">
                                                    Consulta</label>
                                                <div>
                                                    <input id="txtConsulta" type="text" />
                                                </div>
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="col s4">
                                                <h6 data-translate="cr1axb63">
                                                    Proveedor</h6>
                                            </div>
                                            <div class="col s2">
                                                <input type="checkbox" class="filled-in" id="chkActivarProveedor" />
                                                <label id="lblActivarProveedor" for="chkActivarProveedor" data-translate="cr1axb51">
                                                    Activar</label>
                                            </div>
                                            <div class="col s4">
                                                <h6 data-translate="cr1axb64">
                                                    Fabricante</h6>
                                            </div>
                                            <div class="col s2">
                                                <input type="checkbox" class="filled-in" id="chkActivarFabricante" />
                                                <label id="lblActivarFabricante" for="chkActivarFabricante" data-translate="cr1axb51">
                                                    Activar</label>
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="col s2">
                                                <label id="lblCodigoProveedor" for="txtCodigoProveedor" class="active" data-translate="cr1axb5">
                                                    Código</label>
                                                <div>
                                                    <input id="txtCodigoProveedor" type="text" />
                                                </div>
                                            </div>
                                            <div class="col s3">
                                                <label id="lblNombreProveedor" for="txtNombreProveedor" class="active" data-translate="cr1axb6">
                                                    Nombre</label>
                                                <div>
                                                    <input id="txtNombreProveedor" type="text" />
                                                </div>
                                            </div>
                                            <div class="col s1">
                                                <a id="btn_agregar_proveedor" class="waves-effect waves-light btn white right" style="border: 1px solid #95398c;
                                                    color: #95398c; padding-left: 5px; padding-right: 5px; font-size: 12px; margin: 10px;
                                                    text-transform: initial;"><span data-translate="cr1axb65" class="hide-on-small-only">AGREGAR</span>
                                                </a>
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="col s6">
                                                <div id="externalPagerProveedores" class="external-pager">
                                                </div>
                                                <div id="jsProveedores">
                                                </div>
                                            </div>
                                            <div class="col s6">
                                                <div id="externalPagerFabricantes" class="external-pager">
                                                </div>
                                                <div id="jsFabricantes">
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <div id="excepciones" class="col s12">
                                        <div class="row" style="margin-top: 1em">
                                            <div class="col s10">
                                                <h6 data-translate="cr1axb48">
                                                    Excepciones</h6>
                                            </div>
                                            <div class="col s2">
                                                <input type="checkbox" class="filled-in" id="chkActivarExcepciones" />
                                                <label id="lblActivarExcepciones" for="chkActivarExcepciones" data-translate="cr1axb51">
                                                    Activar</label>
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="col s2">
                                                <label id="Label23" for="txtExcepArtCodigo" class="active" data-translate="cr1axb5">
                                                    Código</label>
                                                <div>
                                                    <input id="txtExcepArtCodigo" type="text" />
                                                </div>
                                            </div>
                                            <div class="col s3">
                                                <label id="Label24" for="txtExcepArtNombre" class="active" data-translate="cr1axb66">
                                                    Nombre del artículo</label>
                                                <div>
                                                    <input id="txtExcepArtNombre" type="text" />
                                                </div>
                                            </div>
                                            <div class="col s7">
                                                <a id="btn_agregar_excepcion" class="waves-effect waves-light btn green left small-only-so1 so1-btn"
                                                    style="margin-right: 1em"><span data-translate="cr1axb67" class="hide-on-small-only" id="Span3">
                                                        Agregar artículo</span> </a>
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div id="externalPagerExcepciones" class="external-pager">
                                            </div>
                                            <div id="jsExcepciones">
                                            </div>
                                        </div>
                                    </div>
                                    <div id="unidadesmedida" class="col s12">
                                        <div class="row" style="margin-top: 1em">
                                            <div class="col s6">
                                                <h6 data-translate="cr1axb68">
                                                    Unidades de Medida</h6>
                                            </div>
                                            <div class="col s2">
                                                <input type="checkbox" class="filled-in" id="chkActivarUnidades" />
                                                <label id="lblActivarUnidades" for="chkActivarUnidades" data-translate="cr1axb51">
                                                    Activar</label>
                                            </div>
                                            <div class="col s4">
                                                <a id="btnSeleccionarUnidadMArticulos" class="waves-effect waves-light btn white right so1-btnseleccion">
                                                    <span data-translate="cr1axb54">Seleccionar todo</span> </a><a id="btnDeseleccionarUnidadMArticulos"
                                                        class="waves-effect waves-light btn white right so1-btnseleccion"><span data-translate="cr1axb55">
                                                            Deseleccionar todo</span> </a>
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div id="externalPagerUnidades" class="external-pager">
                                            </div>
                                            <div id="jsArticulosUnidades">
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="container" id="sucursales" class="col s12">
                            <div class="card-panel">
                                <div class="row">
                                    <div class="col s6">
                                        <h5 data-translate="cr1axb14">
                                            Sucursales</h5>
                                    </div>
                                    <div class="col s4">
                                        <input type="checkbox" class="filled-in" id="chkFiltrarSucursales" />
                                        <label id="lblFiltrarSucursales" for="chkFiltrarSucursales" data-translate="cr1axb69">
                                            Filtrar por sucursal</label>
                                    </div>
                                    <div class="col s4">
                                        <a id="btnSeleccionarSucursales" class="waves-effect waves-light btn white right so1-btnseleccion">
                                            <span data-translate="cr1axb54">Seleccionar todo</span> </a><a id="btnDeseleccionarSucursales"
                                                class="waves-effect waves-light btn white right so1-btnseleccion"><span data-translate="cr1axb55">
                                                    Deseleccionar todo</span> </a>
                                    </div>
                                </div>
                                <div class="row">
                                    <div id="externalPagerSucursales" class="external-pager">
                                    </div>
                                    <div id="jsSucursales">
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="container" id="alianzas" class="col s12">
                            <div class="card-panel">
                                <div class="row">
                                    <div class="col s6">
                                        <h5  data-translate="cr1axb15">
                                            Alianzas</h5>
                                    </div>
                                    <div class="col s2">
                                    <input type="checkbox" class="filled-in" id="chkFiltrarAlianzas" />
                                        <label id="lblFiltrarAlianzas" for="chkFiltrarAlianzas"  data-translate="cr1axb70">
                                            Filtrar por alianzas comerciales</label>
                                    </div>
                                    <div class="col s4">
                                        <a id="btnSeleccionarAlianza" class="waves-effect waves-light btn white right so1-btnseleccion">
                                            <span data-translate="cr1axb54">Seleccionar todo</span> </a><a id="btnDeseleccionarAlianza"
                                                class="waves-effect waves-light btn white right so1-btnseleccion"><span data-translate="cr1axb55">
                                                    Deseleccionar todo</span> </a>
                                    </div>
                                </div>
                                <div class="row">
                                    <div id="externalPagerAlianzas" class="external-pager">
                                    </div>
                                    <div id="jsAlianzas">
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="container" id="clientes" class="col s12">
                            <div class="card-panel">
                                <div class="row">
                                    <div class="row">
                                        <div class="col s8">
                                            <h5  data-translate="cr1axb16">
                                                Clientes</h5>
                                        </div>
                                        <div class="col s2">
                                        <input type="checkbox" class="filled-in" id="chkClientes" />
                                                <label id="Label18" for="chkClientes"  data-translate="cr1axb51">
                                                    Activar</label>
                                        </div>
                                        <div class="col s2">
                                            <input name="CondicionCliente" type="radio" id="CondicionClienteY" />
                                            <label id="lblCondicionClienteY" for="CondicionClienteY" style="margin-right: 10px;"  data-translate="cr1axb43">
                                                Y</label>
                                            <input name="CondicionCliente" type="radio" id="CondicionClienteO" />
                                            <label id="lblCondicionClienteO" for="CondicionClienteO"  data-translate="cr1axb44">
                                                O Bien</label>
                                        </div>
                                    </div>
                                    <div class="col s12" style="padding-bottom:1em">
                                        <ul class="tabs">
                                            <li id="btnTabClientes" class="tab col so1-tab2"><a href="#clientes2"  data-translate="cr1axb16">Clientes</a></li>
                                            <li id="btnTabGrupo" class="tab col so1-tab2"><a href="#grupos2"  data-translate="cr1axb45">Grupos</a></li>
                                            <li id="btnTabPropiedad" class="tab col so1-tab2"><a href="#propiedades2"  data-translate="cr1axb46">Propiedades</a></li>
                                        </ul>
                                    </div>
                                    <div id="clientes2" class="col s12">
                                        <div class="row" style="margin-top: 1em">
                                            <div class="col s5">
                                                <h6  data-translate="cr1axb71">
                                                    Lista de clientes</h6>
                                            </div>
                                            <div class="col s2">
                                                <input type="checkbox" class="filled-in" id="chkActivarClientes" />
                                                <label id="lblActivarClientes" for="chkActivarClientes"  data-translate="cr1axb51">
                                                    Activar</label>
                                            </div>
                                            <div class="col s3">
                                                <h6  data-translate="cr1axb58">
                                                    Condición especial</h6>
                                            </div>
                                            <div class="col s2">
                                                <input type="checkbox" class="filled-in" id="chkActivarClientesCE" />
                                                <label id="lblActivarClientesCE" for="chkActivarClientesCE"  data-translate="cr1axb51">
                                                    Activar</label>
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="col s2">
                                                <label id="lblClienteCodigo" for="txtClienteCodigo" class="active" data-translate="cr1axb5">
                                                    Código</label>
                                                <div>
                                                    <input id="txtClienteCodigo" type="text" />
                                                </div>
                                            </div>
                                            <div class="col s3">
                                                <label id="lblClienteNombre" for="txtClienteNombre" class="active" data-translate="cr1axb6">
                                                    Nombre</label>
                                                <div>
                                                    <input id="txtClienteNombre" type="text" />
                                                </div>
                                            </div>
                                            <div class="col s2">
                                                <a id="btn_agregar_cliente" class="waves-effect waves-light btn green left small-only-so1 so1-btn"
                                                    style="margin-right: 1em"><span data-translate="cr1axb72" class="hide-on-small-only" id="Span4">
                                                        Agregar cliente</span> </a>
                                            </div>
                                            <div class="col s2">
                                                <label id="lblClienteCampo" for="txtClienteCampo" class="active" data-translate="cr1axb73">
                                                    Campo(OCRD)</label>
                                                <div>
                                                    <input id="txtClienteCampo" type="text" />
                                                </div>
                                            </div>
                                            <div class="col s3">
                                                <label id="lblClienteValor" for="txtClienteValor" class="active" data-translate="cr1axb61">
                                                    Valor</label>
                                                <div>
                                                    <input id="txtClienteValor" type="text" />
                                                </div>
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div id="externalPagerClientes" class="external-pager">
                                            </div>
                                            <div id="jsClientes">
                                            </div>
                                        </div>
                                    </div>
                                    <div id="grupos2" class="col s12">
                                        <div class="row" style="margin-top: 1em">
                                            <div class="col s6">
                                                <h6 data-translate="cr1axb74">
                                                    Grupos de clientes</h6>
                                            </div>
                                            <div class="col s2">
                                                <input type="checkbox" class="filled-in" id="chkActivarGrupoClientes" />
                                                <label id="Label30" for="chkActivarGrupoClientes" data-translate="cr1axb51">
                                                    Activar</label>
                                            </div>
                                            <div class="col s4">
                                                <a id="btnSeleccionarGrupoCliente" class="waves-effect waves-light btn white right so1-btnseleccion">
                                                    <span data-translate="cr1axb54">Seleccionar todo</span> </a><a id="btnDeseleccionarGrupoCliente"
                                                        class="waves-effect waves-light btn white right so1-btnseleccion"><span data-translate="cr1axb55">
                                                            Deseleccionar todo</span> </a>
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div id="externalPagerGruposClientes" class="external-pager">
                                            </div>
                                            <div id="jsGruposClientes">
                                            </div>
                                        </div>
                                    </div>
                                    <div id="propiedades2" class="col s12">
                                        <div class="row" style="margin-top: 1em">
                                            <div class="col s3">
                                                <h6 data-translate="cr1axb75">
                                                    Propiedades de cliente</h6>
                                            </div>
                                            <div class="col s1 m1 l1">
                                                <input type="checkbox" class="filled-in" id="chkActivarPropiedadesClientes" />
                                                <label id="Label31" for="chkActivarPropiedadesClientes" data-translate="cr1axb51">
                                                    Activar</label>
                                            </div>
                                            <div class="col s2">
                                                <input name="PropiedadCliente" type="radio" id="PropiedadClienteY" />
                                                <label id="lblPropiedadClienteY" for="PropiedadClienteY" style="margin-right: 10px;" data-translate="cr1axb43">
                                                    Y</label>
                                                <input name="PropiedadCliente" type="radio" id="PropiedadClienteO" />
                                                <label id="lblPropiedadClienteO" for="PropiedadClienteO" data-translate="cr1axb44">
                                                    O Bien</label>
                                            </div>
                                            <div class="col s2 m2 l2">
                                                <input type="checkbox" class="filled-in" id="chkClienteCE" />
                                                <label id="lblClienteCE" for="chkClienteCE" data-translate="cr1axb76">
                                                    Coincidencia exacta</label>
                                            </div>
                                        
                                        
                                            <div class="col s4">
                                                <a id="btnSeleccionarPropCliente" class="waves-effect waves-light btn white right so1-btnseleccion">
                                                    <span data-translate="cr1axb54">Seleccionar todo</span> </a><a id="btnDeseleccionarPropCliente"
                                                        class="waves-effect waves-light btn white right so1-btnseleccion"><span data-translate="cr1axb55">
                                                            Deseleccionar todo</span> </a>
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div id="externalPagerPropiedadesClientes" class="external-pager">
                                            </div>
                                            <div id="jsPropiedadesClientes">
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="container" id="membresias" class="col s12">
                            <div class="card-panel">
                                <div class="row">
                                    <div class="col s6">
                                        <h5 data-translate="cr1axb17">
                                            Membresías</h5>
                                    </div>
                                    <div class="col s6">
                                        <input type="checkbox" class="filled-in" id="chkFiltrarMembresias" />
                                        <label id="lblFiltrarMembresias" for="chkFiltrarMembresias" data-translate="cr1axb77">
                                            Filtrar por tipo de membresía</label>
                                        <a id="btnSeleccionarMembresias" class="waves-effect waves-light btn white right so1-btnseleccion">
                                            <span data-translate="cr1axb54">Seleccionar todo</span> </a><a id="btnDeseleccionarMembresias"
                                                class="waves-effect waves-light btn white right so1-btnseleccion"><span data-translate="cr1axb55">
                                                    Deseleccionar todo</span> </a>
                                    </div>
                                </div>
                                <div class="row">
                                    <div id="externalPagerMembresias" class="external-pager">
                                    </div>
                                    <div id="jsMembresias">
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="container" id="precios" class="col s12">
                            <div class="card-panel">
                                <div class="row">
                                    <div class="col s6">
                                        <h5  data-translate="cr1axb18">
                                            Lista de precios</h5>
                                    </div>
                                    <div class="col s6">
                                        <input type="checkbox" class="filled-in" id="chkFiltrarPrecios" />
                                        <label id="lblFiltrarPrecios" for="chkFiltrarPrecios"  data-translate="cr1axb78">
                                            Filtrar por lista de precios</label>
                                        <a id="btnSeleccionarPrecios" class="waves-effect waves-light btn white right so1-btnseleccion">
                                            <span data-translate="cr1axb54">Seleccionar todo</span> </a><a id="btnDeseleccionarPrecios"
                                                class="waves-effect waves-light btn white right so1-btnseleccion"><span data-translate="cr1axb55">
                                                    Deseleccionar todo</span> </a>
                                    </div>
                                </div>
                                <div class="row">
                                    <div id="externalPagerPrecios" class="external-pager">
                                    </div>
                                    <div id="jsPrecios">
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="container" id="pago" class="col s12">
                            <div class="card-panel">
                                <div class="row">
                                    <div class="col s2">
                                        <h5  data-translate="cr1axb79">
                                            Forma de pago</h5>
                                    </div>
                                    <div class="col s3">
                                        <input type="checkbox" class="filled-in" id="chkFiltrarFormaPago" />
                                        <label id="lblFiltrarFormaPago" for="chkFiltrarFormaPago"  data-translate="cr1axb80">
                                            Filtrar por forma de pago</label>
                                    </div>
                                    <div class="col s3">
                                        <input type="checkbox" class="filled-in" id="chkFiltrarVentaCredito" />
                                        <label id="lblFiltrarVentaCredito" for="chkFiltrarVentaCredito"  data-translate="cr1axb81">
                                            Aplicar en venta a crédito</label>
                                    </div>
                                    <div class="col s4">
                                        <a id="btnSeleccionarPagos" class="waves-effect waves-light btn white right so1-btnseleccion">
                                            <span data-translate="cr1axb54">Seleccionar todo</span> </a><a id="btnDeseleccionarPagos"
                                                class="waves-effect waves-light btn white right so1-btnseleccion"><span data-translate="cr1axb55">
                                                    Deseleccionar todo</span> </a>
                                    </div>
                                </div>
                                <div class="row">
                                    <div id="externalPagerFormasPago" class="external-pager">
                                    </div>
                                    <div id="jsFormasPago">
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </li>
            </ul>
        </div>
    </div>
    <!-- START MODAL Confirmacion -->
    <div id="mdlConfirmacion" class="modal">
        <div class="modal-content">
            <div class="row">
                <div class="text-field col s12 m12 l12">
                    <span id="hConfirmacion" data-translate="cr1axb154" style="font-size: 1.64rem;">¿Quiere añadir o reemplazar los articulos existentes?</span>
                </div>
            </div>
            <div class="row">
                <div class="input-field col s12 m12 l12">
                    <a id="btnAñadir" class="waves-effect waves-light right btn green">
                        <span data-translate="cr1axb155">Añadir </span>
                    </a>
                    <a id="btnReemplazar" class="waves-effect waves-light right btn blue" style="margin-right: 10px;">
                        <span data-translate="cr1axb156">Reemplazar </span>
                    </a>
                </div>
             </div>
        </div>
    </div>
    <div id="mdlConfirmarEliminar" class="modal" style="width:50%">
        <div class="modal-content">
            <div class="row">
                <div class="text-field col s12 m12 l12">
                    <span data-translate="cr1axb162" style="font-size: 1.3rem;">¿Desea confirmar la eliminación?</span>
                </div>
            </div>
            <div class="row">
                <div class="col s6 m6 l6">
                    <input type="checkbox" class="filled-in" id="chkNoPreguntar" />
                    <label id="lblNoPreguntar" for="chkNoPreguntar"  data-translate="cr1axb163">
                                            No preguntar de nuevo</label>
                </div>
                <div class="input-field col s6 m6 l6">
                    <a id="btn_CancelarEliminar" class="waves-effect waves-light right btn red">
                        <span data-translate="cr1axb164">Cancelar</span>
                    </a>
                    <a id="btn_ConfirmarEliminar" class="waves-effect waves-light right btn green" style="margin-right: 10px;">
                        <span data-translate="cr1axb165">Eliminar</span>
                    </a>
                </div>
             </div>
        </div>
    </div>
    <!-- END MODAL Confirmacion -->
    <div id="divLoader" class="preloaded">
        <div id="preloader">
            <div id="loading">
            </div>
            <div class="loader-section section-left">
            </div>
            <div class="loader-section section-right">
            </div>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="PageScripts" runat="server">
    <script src="js/plugins/flatpickr/rangePlugin.js" type="text/javascript"></script>
    <link href="js/plugins/flatpickr/flatpickr.css" rel="stylesheet" type="text/css" />
    <script src="js/plugins/flatpickr/flatpickr.js" type="text/javascript"></script>
    <script src="js/plugins/flatpickr/ar.js" type="text/javascript"></script>
    <script src="js/plugins/flatpickr/es.js" type="text/javascript"></script>
    <link href="js/plugins/jsgrid-1.5.3/css/jsgrid.css" rel="Stylesheet" type="text/css" />
    <link href="js/plugins/jsgrid-1.5.3/css/theme.css" rel="Stylesheet" type="text/css" />
    <script src="js/plugins/jsgrid-1.5.3/demos/db.js" type="text/javascript"></script>
    <script src="js/plugins/jsgrid-1.5.3/src/jsgrid.core.js" type="text/javascript"></script>
    <script src="js/plugins/jsgrid-1.5.3/src/jsgrid.load-indicator.js" type="text/javascript"></script>
    <script src="js/plugins/jsgrid-1.5.3/src/jsgrid.load-strategies.js" type="text/javascript"></script>
    <script src="js/plugins/jsgrid-1.5.3/src/jsgrid.sort-strategies.js" type="text/javascript"></script>
    <script src="js/plugins/jsgrid-1.5.3/src/jsgrid.field.js" type="text/javascript"></script>
    <script src="js/plugins/jsgrid-1.5.3/src/fields/jsgrid.field.text.js" type="text/javascript"></script>
    <script src="js/plugins/jsgrid-1.5.3/src/fields/jsgrid.field.number.js" type="text/javascript"></script>
    <script src="js/plugins/jsgrid-1.5.3/src/fields/jsgrid.field.select.js" type="text/javascript"></script>
    <script src="js/plugins/jsgrid-1.5.3/src/fields/jsgrid.field.checkbox.js" type="text/javascript"></script>
    <script src="js/plugins/jsgrid-1.5.3/src/fields/jsgrid.field.control.js" type="text/javascript"></script>
    <script src="js/plugins/formatoNumerico/formato.js"></script>
    <script src="js/materialize-plugin/time_picker/time.js"></script>
    <script type="text/javascript" src="js/forms/CR1PromocionAxB.js"></script>

</asp:Content>
