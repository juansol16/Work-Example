using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using RetailOne.Datos;
using RetailOne.Utilidades;
using ReportesWeb.Modelo;
using ReportesWeb.Utilidades;
using System.Collections;
using SAPbobsCOM;
using RetailOne.Ejecucion;
using ReportesWeb.Seguridad;

namespace ReportesWeb.Adaptador
{
    public class Promocion : MPromocion
    {

        #region Constructor

        public Promocion()
            : base()
        {

        }

        #endregion

        private Promocion promocionOriginal;

        public void AgregarPromocion(Promocion promocion)
        {
            promocionOriginal = promocion;
        }

        static private readonly object token = new object();

        #region Métodos

        /// <summary>
        /// Método para listar promociones por tipo
        /// </summary>
        /// <returns>Regresa un IEnumerable de promociones</returns>
        public static IEnumerable<Promocion> Listar(string tipo)
        {
            List<Promocion> promociones = new List<Promocion>();
            using (Conexion conn = new Conexion())
            {
                conn.CargarConsulta(ConsultasPromocion.Listar);
                conn.AgregarLiteral("CONDICION", tipo == "TO" ? "" : "where U_SO1_TIPO = '" + tipo + "'");
                //conn.AgregarParametro("pTipo", tipo == "TO" ? "" : tipo);

                DataTable dtPromociones = conn.EjecutarTabla();
                foreach (DataRow fila in dtPromociones.Rows)
                {
                    Promocion promocion = new Promocion();
                    promocion.Codigo = fila.Valor<string>("Code");
                    promocion.Nombre = fila.Valor<string>("Name");
                    promocion.U_SO1_PROMOCION = fila.Valor<int>("U_SO1_PROMOCION");
                    promocion.U_SO1_JERARQUIA = fila.Valor<int>("U_SO1_JERARQUIA");
                    promocion.U_SO1_TIPO = fila.Valor<string>("U_SO1_TIPO");
                    promociones.Add(promocion);
                }
            }
            return promociones;
        }

        public static Promocion Consultar(Instancia instancia, string codigo, string nombre)
        {
            Promocion oPromocion = new Promocion();
            //Promocion oPromocion2 = new Promocion();
            oPromocion.Buscar(codigo, nombre);
            //oPromocion2 = oPromocion;
            return oPromocion;
        }

        public static Promocion Importar(String[] arArticulo)
        {
            Promocion oPromocion = new Promocion();
            oPromocion.ListarArticulos(arArticulo);
            return oPromocion;
        }

        public static Promocion ImportarConPrecio(String[] arArticulo, string ListaPrecio1, string ListaPrecio2)
        {
            Promocion oPromocion = new Promocion();
            oPromocion.ListarArticulosPrecios(arArticulo, ListaPrecio1, ListaPrecio2);
            return oPromocion;
        }

        /// <summary>
        /// Método para listar promociones por multiples filtros
        /// </summary>
        /// <returns>Regresa un IEnumerable de promociones</returns>
        public static IEnumerable<Promocion> Listar(String fechaInicio, String fechaFin, String[] tipo, String[] sucursal, /*String[] cliente, String[] grupoArticulo,*/ String[] formaPago, String vigencia, String promocionPermitida)
        {
            List<Promocion> promociones = new List<Promocion>();
            string lTipo = String.Empty;
            string lSucursal = String.Empty;
            //string lCliente = String.Empty;
            //string lGrupoArticulo = String.Empty;
            string lFormaPago = String.Empty;

            string sTipo = null;
            string sSucursal = null;
            //string sCliente = null;
            //string sGrupoArticulo = null;
            string sFormaPago = null;
            string caducada = null;
            string vigente = null;
            using (Conexion conn = new Conexion())
            {
                conn.CargarConsulta(ConsultasPromocion.ListarFiltrados);
                if (tipo.Length == 0)
                {
                    lTipo = "NULL";
                }
                else
                {
                    lTipo = String.Join(",", tipo.Select(s => "'" + s + "'"));
                    sTipo = "Tipo";
                }
                if (!String.IsNullOrEmpty(fechaInicio) && !String.IsNullOrEmpty(fechaFin))
                {
                    fechaInicio = DateTime.Parse(fechaInicio).ToString("yyyy-MM-dd");
                    fechaFin = DateTime.Parse(fechaFin).ToString("yyyy-MM-dd");
                }
                else
                {
                    fechaInicio = null;
                    fechaFin = null;
                }
                if (sucursal.Length != 0)
                {
                    lSucursal = String.Join(",", sucursal.Select(s => "'" + s + "'"));
                    sSucursal = "Sucursal";
                }
                else
                {
                    lSucursal = "NULL";
                }
                //if (cliente.Length != 0)
                //{
                //    lCliente = String.Join(",", cliente.Select(s => "'" + s + "'"));
                //    sCliente = "Cliente";
                //}
                //else
                //{
                //    lCliente = "NULL";
                //}
                //if (grupoArticulo.Length != 0)
                //{
                //    lGrupoArticulo = String.Join(",", grupoArticulo.Select(s => "'" + s + "'"));
                //    sGrupoArticulo = "GrupoArticulo";
                //}
                //else
                //{
                //    lGrupoArticulo = "NULL";
                //}
                if (formaPago.Length != 0)
                {
                    lFormaPago = String.Join(",", formaPago.Select(s => "'" + s + "'"));
                    sFormaPago = "FormaPago";
                }
                else
                {
                    lFormaPago = "NULL";
                }
                if (!String.IsNullOrEmpty(vigencia) && vigencia != "T")
                {
                    if (vigencia == "C")
                    {
                        caducada = DateTime.Now.ToString("yyyyMMdd");
                    }
                    else if (vigencia == "V")
                    {
                        vigente = DateTime.Now.ToString("yyyyMMdd");
                    }

                }
                //conn.AgregarLiteral("CONDICION", tipo == "TO" ? "" : "where U_SO1_TIPO = '" + tipo + "'");
                conn.AgregarParametro("pTipo", sTipo);
                conn.AgregarLiteral("lTipo", lTipo);
                conn.AgregarParametro("pFechaInicio", fechaInicio);
                conn.AgregarParametro("pFechaInicio", fechaInicio);
                conn.AgregarParametro("pFechaFin", fechaFin);

                conn.AgregarParametro("pFechaFin", fechaFin);
                conn.AgregarParametro("pFechaInicio", fechaInicio);
                conn.AgregarParametro("pFechaFin", fechaFin);

                conn.AgregarParametro("pVigente", vigente);
                conn.AgregarParametro("pVigente", vigente);
                conn.AgregarParametro("pVigente", vigente);

                conn.AgregarParametro("pCaducada", caducada);
                conn.AgregarParametro("pCaducada", caducada);
                conn.AgregarParametro("pCaducada", caducada);

                conn.AgregarParametro("pSucursal", sSucursal);
                conn.AgregarLiteral("lSucursal", lSucursal);

                conn.AgregarParametro("pCliente", null);
                conn.AgregarLiteral("lCliente", "NULL");

                conn.AgregarParametro("pGrupoArticulo", null);
                conn.AgregarLiteral("lGrupoArticulo", "NULL");

                conn.AgregarParametro("pFormaPago", sFormaPago);
                conn.AgregarLiteral("lFormaPago", lFormaPago);

                conn.AgregarLiteral("lPromociones", promocionPermitida);


                //conn.AgregarParametro("pTipo", sTipo);
                //conn.AgregarParametro("pSucursal", sSucursal);
                //conn.AgregarParametro("pCliente", null);
                //conn.AgregarParametro("pGrupoArticulo", null);
                //conn.AgregarParametro("pFormaPago", sFormaPago);
                //conn.AgregarParametro("pVigente", vigente);
                //conn.AgregarParametro("pCaducada", caducada);


                //conn.AgregarLiteral("lSucursal", lSucursal);
                //conn.AgregarLiteral("lCliente", "NULL");
                //conn.AgregarLiteral("lGrupoArticulo", "NULL");
                //conn.AgregarLiteral("lFormaPago", lFormaPago);


                string consulta = conn.TextoComandoCompleto;
                //foreach (System.Data.SqlClient.SqlParameter parametro in conn.Parametros)
                //{
                //    consulta = consulta.Replace(parametro.ParameterName, parametro.Value.ToString());
                //}
                ReportesWeb.Seguridad.RegistroEvento.GuardarError(consulta, null);


                DataTable dtPromociones = conn.EjecutarTabla();
                foreach (DataRow fila in dtPromociones.Rows)
                {
                    Promocion promocion = new Promocion();
                    promocion.Codigo = fila.Valor<string>("Code");
                    promocion.Nombre = fila.Valor<string>("Name");
                    promocion.U_SO1_FECHADESDE = fila.Valor<DateTime>("U_SO1_FECHADESDE").ToShortDateString();
                    promocion.U_SO1_FECHAHASTA = fila.Valor<DateTime>("U_SO1_FECHAHASTA").ToShortDateString();
                    promocion.U_SO1_PROMOCION = fila.Valor<int>("U_SO1_PROMOCION");
                    promocion.U_SO1_JERARQUIA = fila.Valor<int>("U_SO1_JERARQUIA");
                    promocion.U_SO1_TIPO = fila.Valor<string>("TIPO");
                    promociones.Add(promocion);
                }
            }
            return promociones;
        }

        public override bool ListarArticulos(String[] arArticulo)
        {
            try
            {
                string articulos = String.Join(",", arArticulo.Select(s => "'" + s + "'"));

                using (Conexion conn = new Conexion())
                {
                    conn.LimpiarParametros();
                    conn.CargarConsulta(ConsultasPromocion.ConsultarArticulos);
                    conn.AgregarLiteral("lArticulos", articulos);
                    DataTable dtArticuloPromo = conn.EjecutarTabla();
                    foreach (DataRow fila in dtArticuloPromo.Rows)
                    {
                        Modelo.MPromoArticulo articulo = new Modelo.MPromoArticulo();
                        articulo.U_SO1_ARTICULO = fila.Valor<string>("Codigo");
                        articulo.Nombre = fila.Valor<string>("Nombre");
                        Agregar(articulo, "G");
                    }
                }
                return true;
            }
            catch (Exception oError)
            {
                Log.GuardarError("Error", oError);
                return false;
            }
        }

        public override bool ListarArticulosPrecios(String[] arArticulo, string ListaPrecio1, string ListaPrecio2)
        {
            try
            {
                string articulos = String.Join(",", arArticulo.Select(s => "'" + s + "'"));

                using (Conexion conn = new Conexion())
                {
                    conn.LimpiarParametros();
                    conn.CargarConsulta(ConsultasPromocion.ConsultarArticulosPrecios);
                    conn.AgregarParametro("pListaPrecio1", ListaPrecio1);
                    conn.AgregarParametro("pListaPrecio2", ListaPrecio2);
                    conn.AgregarLiteral("lArticulos", articulos);
                    DataTable dtArticuloPromo = conn.EjecutarTabla();
                    foreach (DataRow fila in dtArticuloPromo.Rows)
                    {
                        Modelo.MPromoArticulo articulo = new Modelo.MPromoArticulo();
                        articulo.U_SO1_ARTICULO = fila.Valor<string>("Codigo");
                        articulo.Nombre = fila.Valor<string>("Nombre");
                        articulo.Precio1 = fila.Valor<string>("Precio");
                        articulo.Precio2 = fila.Valor<string>("Precio2");
                        Agregar(articulo, "G");
                    }
                }
                return true;
            }
            catch (Exception oError)
            {
                Log.GuardarError("Error", oError);
                return false;
            }
        }

        public static bool ModificarJerarquias(List<Promocion> listPromociones)
        {

            #region Bitácora

            // AUTOR:               Juan Sol
            // FECHA:               11.06.2019
            // COMENTARIOS:         Adaptación del método

            // AUTOR:               Juan Sol
            // FECHA:               28.11.2019
            // COMENTARIOS:         Se cambia de DIAPI a SQL/HANA

            #endregion

            using (Conexion conn = new Conexion())
            {
                try
                {

                    conn.IniciarTransaccion();

                    for (int i = 0; i < listPromociones.Count(); i++)
                    {
                        conn.LimpiarParametros();
                        conn.CargarConsulta(ConsultasPromocion.ActualizarJerarquia);
                        conn.AgregarParametro("pJerarquia", listPromociones[i].U_SO1_JERARQUIA);
                        conn.AgregarParametro("pCodigo", listPromociones[i].Codigo);
                        conn.EjecutarComando();
                    }

                    conn.ConfirmarTransaccion();
                }
                catch (Exception oError)
                {
                    conn.RestaurarTransaccion();
                    return false;
                }
                Promocion promocion = new Promocion();
                return promocion.RegistrarCambios("U");
            }
        }

        public override bool Registrar()
        {

            #region Bitácora

            // AUTOR:               Juan Sol
            // FECHA:               09.07.2019
            // COMENTARIOS:         Creación del método

            // AUTOR:               Juan Sol
            // FECHA:               19.11.2019
            // COMENTARIOS:         Se reacomodó el Using y Try-Catch para incluir el Lock

            #endregion

            using (Conexion conn = new Conexion())
            {
                try
                {
                    ValidarPropiedades();
                    // Funciona como un monitor, sólo se ejecuta uno a la vez, los demás usuarios se ponen en cola
                    lock (token)
                    {

                        //Aqui llamamos a las claves al objeto
                        ReportesWeb.Utilidades.CodigoControl codigosControl = new ReportesWeb.Utilidades.CodigoControl(TablaCodigoControl.PROMOSUCURSAL,
                            TablaCodigoControl.PROMOALICOMER,
                            TablaCodigoControl.PROMOTIPOMEMB,
                            TablaCodigoControl.PROMOLISTPREC,
                            TablaCodigoControl.PROMOFORMAPAG,
                            TablaCodigoControl.PROMODESESCPR,
                            TablaCodigoControl.PROMOARTIUNIE,
                            TablaCodigoControl.PROMODESVOLES,
                            TablaCodigoControl.PROMOKITVEN,
                            TablaCodigoControl.PROMOARTICULO,
                            TablaCodigoControl.PROMOPOLVENLI,
                            TablaCodigoControl.PROMCATHORAR,
                            TablaCodigoControl.CATPORPDESCA,
                            TablaCodigoControl.PROMOAMBITO,
                            TablaCodigoControl.PROMOARTIPROV,
                            TablaCodigoControl.PROMOARTIGRUP,
                            TablaCodigoControl.PROMOARTIPROP,
                            TablaCodigoControl.PROMOARTIFABR,
                            TablaCodigoControl.PROMOCLIENTE,
                            TablaCodigoControl.PROMOCLIEGRUP,
                            TablaCodigoControl.PROMOCLIEPROP);

                        //ValidarPropiedades();
                        conn.IniciarTransaccion();

                        if (!CreaPromocion(conn))
                            return false;

                        switch (U_SO1_TIPO)
                        {
                            case "AB":
                                ValidarAxB();
                                ModificarAxB(conn);
                                break;
                            case "DE":
                                ValidarDescEmp();
                                ModificarDescEmpleado(conn);
                                break;
                            case "DS":
                                ModificarDescEscala(conn, codigosControl);
                                break;
                            case "DI":
                                ModificarDescImporte(conn);
                                break;
                            case "DX":
                                ModificarDescPorcentaje(conn);
                                break;
                            case "DV":
                                ModificarDescVolumen(conn, codigosControl);
                                break;
                            case "KV":
                                ValidarKV();
                                ModificarKitVenta(conn, codigosControl);
                                break;
                            case "KR":
                                ModificarKitRegalo(conn, codigosControl);
                                break;
                            case "PU":
                                ValidarPrecioUnico();
                                ModificarPrecioUnico(conn);
                                break;
                            case "PV":
                                ModificarPoliticaVenta(conn, codigosControl);
                                break;
                            case "RM":
                                ModificarRegaloMonto(conn, codigosControl);
                                break;
                            case "VD":
                                validarValeDescuento();
                                ModificarValeDescuento(conn);
                                break;
                            case "DA":
                                ModificarDescuentoAleatorio(conn, codigosControl);
                                break;
                            case "PR":
                                validarPoliticaRedencion();
                                ModificarPoliticaRedencion(conn);
                                break;
                            default:
                                break;
                        }
                        /*
                        //if (U_SO1_FILTRARARTART == "Y")
                        //    ActualizarIdentificadores(conn);
                        */
                        if (U_SO1_FILTRARPROMSUC == "Y")
                            ActualizarSucursales(conn, codigosControl);

                        if (U_SO1_FILTRARPROMALI == "Y")
                            ActualizarAlianzas(conn, codigosControl);

                        if (U_SO1_FILTRARPROMMEM == "Y")
                            ActualizarMembresias(conn, codigosControl);

                        if (U_SO1_FILTRARPROMLIS == "Y")
                            ActualizarPrecios(conn, codigosControl);

                        if (U_SO1_FILTRARPROMFOP == "Y")
                            ActualizarFPago(conn, codigosControl);

                        /* Tablas de Clientes */

                        if (U_SO1_FILTRARCLIGRU == "Y")
                            ActualizarClienteGrupo(conn, codigosControl);

                        if (U_SO1_FILTRARCLIPRO == "Y")
                            ActualizarClientePropiedad(conn, codigosControl);

                        if (U_SO1_FILTRARCLICLI == "Y")
                            ActualizarClientes(conn, codigosControl);

                        /* Tablas de Articulo */

                        if (U_SO1_FILTRARARTART == "Y")
                            ActualizarTablaArticulos(conn, codigosControl);

                        if (U_SO1_FILTRARARTEXC == "Y")
                            ActualizarExcepciones(conn, codigosControl);

                        if (U_SO1_FILTRARARTPROV == "Y")
                            ActualizarProveedores(conn, codigosControl);

                        if (U_SO1_FILTRARARTGRU == "Y")
                            ActualizarArticuloGrupo(conn, codigosControl);

                        if (U_SO1_FILTRARARTPRO == "Y")
                            ActualizarArticuloPropiedad(conn, codigosControl);

                        if (U_SO1_FILTRARARTFAB == "Y")
                            ActualizarArticuloFabricante(conn, codigosControl);

                        if (U_SO1_FILTRARARTUNE == "Y")
                            ActualizarArticuloUnidadM(conn, codigosControl);

                        codigosControl.Actualizar(conn);
                        conn.ConfirmarTransaccion();
                        Log.GuardarError("Terminando transacción", null);

                    }

                }
                catch (Exception oError)
                {
                    conn.RestaurarTransaccion();
                    throw new Exception("Error al crear promoción", oError);
                }
            }
            //return true;
            return RegistrarCambios("A");
        }

        public override bool Eliminar()
        {

            #region Bitácora

            // AUTOR:               Mario Lopez
            // FECHA:               05.07.2019
            // COMENTARIOS:         Creacion del método

            // AUTOR:               Juan Sol
            // FECHA:               10.07.2019
            // COMENTARIOS:         Se eliminan validaciones al eliminar tablas

            // AUTOR:               Juan Sol
            // FECHA:               31.07.2019
            // COMENTARIOS:         Se cambia el DIAPI a Querys de SQL

            #endregion

            using (Conexion conn = new Conexion())
            {
                
                    Log.GuardarError("Transacción iniciada", null);

                    conn.IniciarTransaccion();

                    EliminarArticulos(conn);
                    EliminarSucursales(conn);
                    EliminarClientes(conn);
                    EliminarAlianzas(conn);
                    EliminarMembresias(conn);
                    EliminarPrecios(conn);
                    EliminarFPago(conn);
                    EliminarArticuloFabricante(conn);
                    EliminarArticuloMedida(conn);
                    EliminarArticuloProveedor(conn);
                    EliminarGrupoArticulo(conn);
                    EliminarPropiedadArticulo(conn);
                    EliminarGrupoCliente(conn);
                    EliminarPropiedadCliente(conn);
                    //EliminarIdentificadores(conn);

                    switch (U_SO1_TIPO)
                    {
                        case "AB":  // Descuento A x B
                            Log.GuardarError("AB", null);
                            EliminarPromo(conn, "@SO1_01PROMOAXB");
                            break;
                        case "DE":  // Descuento por empleado
                            Log.GuardarError("DE", null);
                            EliminarPromo(conn, "@SO1_01PROMODESEMP");
                            break;
                        case "DS":  // Descuento por escala
                            Log.GuardarError("DS", null);
                            EliminarPromo(conn, "@SO1_01PROMODESESC");
                            EliminarPromo(conn, "@SO1_01PROMODESESCPR");
                            break;
                        case "DI":  // Descuento por importe
                            Log.GuardarError("DI", null);
                            EliminarPromo(conn, "@SO1_01PROMODESIMP");
                            break;
                        case "DX":  // Descuento por porcentaje
                            Log.GuardarError("DX", null);
                            EliminarPromo(conn, "@SO1_01PROMODESPOR");
                            break;
                        case "DV":  // Descuento por volumen
                            Log.GuardarError("DV", null);
                            EliminarPromo(conn, "@SO1_01PROMODESVOL");
                            EliminarPromo(conn, "@SO1_01PROMODESVOLES");
                            break;
                        case "DY":  // 
                            //ValidarDE();
                            //EliminarDE(oDIAPI);
                            break;
                        case "EE":
                            //ValidarDE();
                            //EliminarDE(oDIAPI);
                            break;
                        case "KR":  // Kit de regalo
                            Log.GuardarError("KR", null);
                            EliminarPromo(conn, "@SO1_01PROMOKITREG");
                            break;
                        case "KV":  // Kit de venta
                            Log.GuardarError("KV", null);
                            EliminarPromo(conn, "@SO1_01PROMOKITVEN"); // Eliminamos la promo de la tabla de Kit de Venta
                            EliminarPromo(conn, "@SO1_01PROMOAMBITO"); // Eliminamos los ámbitos 
                            break;
                        case "PU":  // Precio único
                            //Log.GuardarError("PU", null);
                            EliminarPromo(conn, "@SO1_01PROMOPREUNI");
                            break;
                        case "PV":  // Politica de venta
                            Log.GuardarError("PV", null);
                            EliminarPromo(conn, "@SO1_01PROMOPOLVEN");
                            EliminarPromo(conn, "@SO1_01PROMOPOLVENLI");
                            break;
                        case "RM":  // Regalo por monto
                            Log.GuardarError("RM", null);
                            EliminarPromo(conn, "@SO1_01PROMOREGMON");
                            EliminarPromo(conn, "@SO1_01PROMOARTICULO");
                            break;
                        case "DA":  // Descuento aleatorio
                            Log.GuardarError("DA", null);
                            EliminarPromo(conn, "@SO1_01PROMODESALE");
                            break;
                        case "VD":
                            Log.GuardarError("VD", null);
                            EliminarPromo(conn, "@SO1_01PROMOVALDES");
                            break;
                        case "PR":  // Politica redención
                            Log.GuardarError("PR", null);
                            EliminarPromo(conn, "@SO1_01POLITICAREDEN");
                            EliminarPoliticaRedencion(conn);
                            break;
                        default:
                            Log.GuardarError("Default", null);
                            break;                                                    
                    }

                    EliminarPromo(conn, "@SO1_01PROMOCION");

                    conn.ConfirmarTransaccion();
                    Log.GuardarError("Terminando transacción", null);
                
            }
            //return true;
            return RegistrarCambios("D");
        }

        public override bool Modificar()
        {

            #region Bitácora

            // AUTOR:               Juan Sol
            // FECHA:               03.07.2019
            // COMENTARIOS:         Adaptación del método

            // AUTOR:               Juan Sol
            // FECHA:               19.11.2019
            // COMENTARIOS:         Se reacomodó el Using y Try-Catch para incluir el Lock

            #endregion

            using (Conexion conn = new Conexion())
            {
                try
                {
                    
                    ValidarPropiedades();

                    // Funciona como un monitor, sólo se ejecuta uno a la vez
                    lock (token)
                    {

                        //Aqui llamamos a las claves al objeto
                        ReportesWeb.Utilidades.CodigoControl codigosControl = new ReportesWeb.Utilidades.CodigoControl(TablaCodigoControl.PROMOSUCURSAL,
                            TablaCodigoControl.PROMOALICOMER,
                            TablaCodigoControl.PROMOTIPOMEMB,
                            TablaCodigoControl.PROMOLISTPREC,
                            TablaCodigoControl.PROMOFORMAPAG,
                            TablaCodigoControl.PROMODESESCPR,
                            TablaCodigoControl.PROMOARTIUNIE,
                            TablaCodigoControl.PROMODESVOLES,
                            TablaCodigoControl.PROMOKITVEN,
                            TablaCodigoControl.PROMOARTICULO,
                            TablaCodigoControl.PROMOPOLVENLI,
                            TablaCodigoControl.PROMCATHORAR,
                            TablaCodigoControl.CATPORPDESCA,
                            TablaCodigoControl.PROMOAMBITO,
                            TablaCodigoControl.PROMOARTIPROV,
                            TablaCodigoControl.PROMOARTIGRUP,
                            TablaCodigoControl.PROMOARTIPROP,
                            TablaCodigoControl.PROMOARTIFABR,
                            TablaCodigoControl.PROMOCLIENTE,
                            TablaCodigoControl.PROMOCLIEGRUP,
                            TablaCodigoControl.PROMOCLIEPROP);

                        conn.IniciarTransaccion();
                        ModificarPromo(conn);

                        /* Tablas de Tabs generales */
                        /* JD 1
                            if (U_SO1_FILTRARARTART == "N")
                                ActualizarIdentificadores(conn);
                        */

                        if (U_SO1_FILTRARPROMSUC == "Y" || (U_SO1_FILTRARPROMSUC == "N" && Existe))
                            ActualizarSucursales(conn, codigosControl);

                        if (U_SO1_FILTRARPROMALI == "Y" || (U_SO1_FILTRARPROMALI == "N" && Existe))
                            ActualizarAlianzas(conn, codigosControl);

                        if (U_SO1_FILTRARPROMMEM == "Y" || (U_SO1_FILTRARPROMMEM == "N" && Existe))
                            ActualizarMembresias(conn, codigosControl);

                        if (U_SO1_FILTRARPROMLIS == "Y" || (U_SO1_FILTRARPROMLIS == "N" && Existe))
                            ActualizarPrecios(conn, codigosControl);

                        if (U_SO1_FILTRARPROMFOP == "Y" || (U_SO1_FILTRARPROMFOP == "N" && Existe))
                            ActualizarFPago(conn, codigosControl);

                        /* Tablas de Clientes */

                        if (U_SO1_FILTRARCLIGRU == "Y" || (U_SO1_FILTRARCLIGRU == "N" && Existe))
                            ActualizarClienteGrupo(conn, codigosControl);

                        if (U_SO1_FILTRARCLIPRO == "Y" || (U_SO1_FILTRARCLIPRO == "N" && Existe))
                            ActualizarClientePropiedad(conn, codigosControl);

                        if (U_SO1_FILTRARCLICLI == "Y" || (U_SO1_FILTRARCLICLI == "N" && Existe))
                            ActualizarClientes(conn, codigosControl);

                        /* Tablas de Articulo */

                        if (U_SO1_FILTRARARTART == "Y" || (U_SO1_FILTRARARTART == "N" && Existe))
                            ActualizarTablaArticulos(conn, codigosControl);

                        if (U_SO1_FILTRARARTEXC == "Y" || (U_SO1_FILTRARARTEXC == "N" && Existe))
                            ActualizarExcepciones(conn, codigosControl);

                        if (U_SO1_FILTRARARTPROV == "Y" || (U_SO1_FILTRARARTPROV == "N" && Existe))
                            ActualizarProveedores(conn, codigosControl);

                        if (U_SO1_FILTRARARTGRU == "Y" || (U_SO1_FILTRARARTGRU == "N" && Existe))
                            ActualizarArticuloGrupo(conn, codigosControl);

                        if (U_SO1_FILTRARARTPRO == "Y" || (U_SO1_FILTRARARTPRO == "N" && Existe))
                            ActualizarArticuloPropiedad(conn, codigosControl);



                        if (U_SO1_FILTRARARTFAB == "Y" || (U_SO1_FILTRARARTFAB == "N" && Existe))
                            ActualizarArticuloFabricante(conn, codigosControl);

                        if (U_SO1_FILTRARARTUNE == "Y" || (U_SO1_FILTRARARTUNE == "N" && Existe))
                            ActualizarArticuloUnidadM(conn, codigosControl);

                        switch (U_SO1_TIPO)
                        {
                            case "AB":
                                ValidarAxB();
                                ModificarAxB(conn);
                                break;
                            case "DE":
                                ValidarDescEmp();
                                ModificarDescEmpleado(conn);
                                break;
                            case "DS":
                                ModificarDescEscala(conn, codigosControl);
                                break;
                            case "DI":
                                ModificarDescImporte(conn);
                                break;
                            case "DX":
                                ModificarDescPorcentaje(conn);
                                break;
                            case "DV":
                                ModificarDescVolumen(conn, codigosControl);
                                break;
                            case "KV":
                                ValidarKV();
                                //ActualizarIdentificadores(conn);
                                ModificarKitVenta(conn, codigosControl);
                                break;
                            case "KR":
                                ModificarKitRegalo(conn, codigosControl);
                                break;
                            case "PV":
                                ModificarPoliticaVenta(conn, codigosControl);
                                break;
                            case "RM":
                                ModificarRegaloMonto(conn, codigosControl);
                                break;
                            case "DA":
                                ModificarDescuentoAleatorio(conn, codigosControl);
                                break;
                            case "CS":
                                ModificarCupon(conn, codigosControl);
                                break;
                            case "PU":
                                ValidarPrecioUnico();
                                ModificarPrecioUnico(conn);
                                break;
                            case "VD":
                                validarValeDescuento();
                                ModificarValeDescuento(conn);
                                break;
                            case "PR":
                                validarPoliticaRedencion();
                                ModificarPoliticaRedencion(conn);
                                break;
                            default:
                                break;
                        }
                        codigosControl.Actualizar(conn);
                        conn.ConfirmarTransaccion();
                        Log.GuardarError("Terminando transacción", null);

                    }
                }
                catch (Exception oError)
                {
                    RegistroEvento.GuardarError("Error al modificar promoción", oError);
                    conn.RestaurarTransaccion();
                    throw new Exception("Error al modificar promoción", oError);
                }
            }
            
            //return true;
            return RegistrarCambios("U");
        }

        public override bool Consultar(string codigo)
        {
            
                if (string.IsNullOrEmpty(codigo))
                    return false;
                this.Existe = false;

                using (Conexion conn = new Conexion())
                {
                    conn.CargarConsulta(ConsultasPromocion.Consultar);
                    conn.AgregarParametro("pCodigo", codigo);
                    conn.AgregarParametro("pNombre", null);

                    conn.EjecutarLector();
                    if (conn.LeerFila())
                    {
                        Materializar(conn, this);
                        this.Existe = true;
                    }
                    else
                    {
                        this.Existe = false;
                    }
                    conn.CerrarLector();

                    //Cargamos las tablas hijas
                    CargarSucursales(conn, this);
                    CargarAlianzas(conn, this);
                    CargarMembresias(conn, this);
                    CargarPrecios(conn, this);
                    CargarFormasPago(conn, this);
                    CargarGruposCliente(conn, this);
                    CargarPropiedadesCliente(conn, this);

                    CargarGruposArticulo(conn, this);
                    CargarPropiedadesArticulo(conn, this);
                    CargarUnidadesMedidaArticulo(conn, this);
                    CargarClientes(conn, this);
                    CargarArticulos(conn, this);
                    CargarFabricantes(conn, this);
                    CargarProveedores(conn, this);
                    CargarExcepciones(conn, this);

                switch (U_SO1_TIPO)
                {
                    case "AB":
                        CargarPromocionAxB(conn, this);
                        break;
                    case "DE":
                        CargarPromocionDescuentoEmpleado(conn, this);
                        break;
                    case "DS":
                        CargarPromocionDescuentoEscala(conn, this);
                        break;
                    case "DI":
                        CargarPromocionDescuentoImporte(conn, this);
                        break;
                    case "DX":
                        CargarPromocionDescuentoPorcentaje(conn, this);
                        break;
                    case "DV":
                        CargarPromocionDescuentoVolumen(conn, this);
                        break;
                    case "KV":
                        CargarPromocionKitVenta(conn, this);
                        CargarIdentificadores(conn, this);
                        //CargarArticulosIdentificadores(conn, this.PromocionIdentificador);
                        break;
                    case "KR":
                        CargarPromocionKitRegalo(conn, this);
                        break;
                    case "PU":
                        CargarPromocionPrecioUnico(conn, this);
                        break;
                    case "PV":
                        CargarPromocionPoliticaVenta(conn, this);
                        break;
                    case "RM":
                        CargarPromocionRegaloMonto(conn, this);
                        break;
                    case "DA":
                        CargarPromocionDescuentoAleatorio(conn, this);
                        break;
                    case "CS":
                        CargarPromocionCupon(conn, this);
                        break;
                    case "VD":
                        CargarPromocionValeDescuento(conn, this);
                        break;
                    case "PR":
                        CargarPromocionPoliticaRedencion(conn, this);
                        break;
                    default:
                        break;
                }
                }
                return true;
           
        }

        public override bool Buscar(string codigo, string nombre)
        {
            if (!string.IsNullOrEmpty(codigo))
            {
                return Consultar(codigo);
            }
            else if (!string.IsNullOrEmpty(nombre))
            {
                this.Existe = false;
                using (Conexion conn = new Conexion())
                {
                    conn.CargarConsulta(ConsultasPromocion.Consultar);
                    conn.AgregarParametro("pCodigo", null);
                    conn.AgregarParametro("pNombre", nombre);
                    conn.EjecutarLector();

                    if (conn.LeerFila())
                    {

                        Materializar(conn, this);
                        this.Existe = true;
                    }
                    else
                    {
                        this.Existe = false;
                    }
                    conn.CerrarLector();

                    //Cargamos las tablas hijas
                    CargarSucursales(conn, this);
                    CargarAlianzas(conn, this);
                    CargarMembresias(conn, this);
                    CargarPrecios(conn, this);
                    CargarFormasPago(conn, this);
                    CargarGruposCliente(conn, this);
                    CargarPropiedadesCliente(conn, this);

                    CargarGruposArticulo(conn, this);
                    CargarPropiedadesArticulo(conn, this);
                    CargarUnidadesMedidaArticulo(conn, this);
                    CargarClientes(conn, this);
                    CargarArticulos(conn, this);
                    CargarFabricantes(conn, this);
                    CargarProveedores(conn, this);
                    CargarExcepciones(conn, this);

                    switch (U_SO1_TIPO)
                    {
                        case "AB":
                            CargarPromocionAxB(conn, this);
                            break;
                        case "DE":
                            CargarPromocionDescuentoEmpleado(conn, this);
                            break;
                        case "DS":
                            CargarPromocionDescuentoEscala(conn, this);
                            break;
                        case "DI":
                            CargarPromocionDescuentoImporte(conn, this);
                            break;
                        case "DX":
                            CargarPromocionDescuentoPorcentaje(conn, this);
                            break;
                        case "DV":
                            CargarPromocionDescuentoVolumen(conn, this);
                            break;
                        case "KV":
                            CargarPromocionKitVenta(conn, this);
                            CargarIdentificadores(conn, this);
                            break;
                        case "KR":
                            CargarPromocionKitRegalo(conn, this);
                            break;
                        case "PU":
                            CargarPromocionPrecioUnico(conn, this);
                            break;
                        case "PV":
                            CargarPromocionPoliticaVenta(conn, this);
                            break;
                        case "RM":
                            CargarPromocionRegaloMonto(conn, this);
                            break;
                        case "DA":
                            CargarPromocionDescuentoAleatorio(conn, this);
                            break;
                        case "CS":
                            CargarPromocionCupon(conn, this);
                            break;
                        case "VD":
                            CargarPromocionValeDescuento(conn, this);
                            break;
                        case "PR":
                            CargarPromocionPoliticaRedencion(conn, this);
                            break;
                        default:
                            break;
                    }
                }
            }
            return false;
        }

        protected static Promocion Materializar(Conexion conn, Promocion promocion)
        {
            promocion.Codigo = conn.DatoLector<string>("Code");
            promocion.Nombre = conn.DatoLector<string>("Name");
            promocion.U_SO1_PROMOCION = conn.DatoLector<int>("U_SO1_PROMOCION");
            //La Jerarquia no es necesaria                       
            promocion.U_SO1_TIPO = conn.DatoLector<string>("U_SO1_TIPO");
            promocion.U_SO1_FECHADESDE = !string.IsNullOrEmpty(conn.DatoLector<string>("U_SO1_FECHADESDE")) ? conn.DatoLector<string>("U_SO1_FECHADESDE").Substring(0, 10) : "";
            promocion.U_SO1_FECHAHASTA = !string.IsNullOrEmpty(conn.DatoLector<string>("U_SO1_FECHAHASTA")) ? conn.DatoLector<string>("U_SO1_FECHAHASTA").Substring(0, 10) : "";
            promocion.U_SO1_DIARIO = conn.DatoLector<string>("U_SO1_DIARIO");
            promocion.U_SO1_DIACOMPLETO = conn.DatoLector<string>("U_SO1_DIACOMPLETO");
            promocion.U_SO1_DIAHORAINI = conn.DatoLector<string>("U_SO1_DIAHORAINI");
            promocion.U_SO1_DIAHORAFIN = conn.DatoLector<string>("U_SO1_DIAHORAFIN");
            promocion.U_SO1_LUNES = conn.DatoLector<string>("U_SO1_LUNES");
            promocion.U_SO1_LUNCOMPLETO = conn.DatoLector<string>("U_SO1_LUNCOMPLETO");
            promocion.U_SO1_LUNHORAINI = conn.DatoLector<string>("U_SO1_LUNHORAINI");
            promocion.U_SO1_LUNHORAFIN = conn.DatoLector<string>("U_SO1_LUNHORAFIN");
            promocion.U_SO1_MARTES = conn.DatoLector<string>("U_SO1_MARTES");
            promocion.U_SO1_MARCOMPLETO = conn.DatoLector<string>("U_SO1_MARCOMPLETO");
            promocion.U_SO1_MARHORAINI = conn.DatoLector<string>("U_SO1_MARHORAINI");
            promocion.U_SO1_MARHORAFIN = conn.DatoLector<string>("U_SO1_MARHORAFIN");
            promocion.U_SO1_MIERCOLES = conn.DatoLector<string>("U_SO1_MIERCOLES");
            promocion.U_SO1_MIECOMPLETO = conn.DatoLector<string>("U_SO1_MIECOMPLETO");
            promocion.U_SO1_MIEHORAINI = conn.DatoLector<string>("U_SO1_MIEHORAINI");
            promocion.U_SO1_MIEHORAFIN = conn.DatoLector<string>("U_SO1_MIEHORAFIN");
            promocion.U_SO1_JUEVES = conn.DatoLector<string>("U_SO1_JUEVES");
            promocion.U_SO1_JUECOMPLETO = conn.DatoLector<string>("U_SO1_JUECOMPLETO");
            promocion.U_SO1_JUEHORAINI = conn.DatoLector<string>("U_SO1_JUEHORAINI");
            promocion.U_SO1_JUEHORAFIN = conn.DatoLector<string>("U_SO1_JUEHORAFIN");
            promocion.U_SO1_VIERNES = conn.DatoLector<string>("U_SO1_VIERNES");
            promocion.U_SO1_VIECOMPLETO = conn.DatoLector<string>("U_SO1_VIECOMPLETO");
            promocion.U_SO1_VIEHORAINI = conn.DatoLector<string>("U_SO1_VIEHORAINI");
            promocion.U_SO1_VIEHORAFIN = conn.DatoLector<string>("U_SO1_VIEHORAFIN");
            promocion.U_SO1_SABADO = conn.DatoLector<string>("U_SO1_SABADO");
            promocion.U_SO1_SABCOMPLETO = conn.DatoLector<string>("U_SO1_SABCOMPLETO");
            promocion.U_SO1_SABHORAINI = conn.DatoLector<string>("U_SO1_SABHORAINI");
            promocion.U_SO1_SABHORAFIN = conn.DatoLector<string>("U_SO1_SABHORAFIN");
            promocion.U_SO1_DOMINGO = conn.DatoLector<string>("U_SO1_DOMINGO");
            promocion.U_SO1_DOMCOMPLETO = conn.DatoLector<string>("U_SO1_DOMCOMPLETO");
            promocion.U_SO1_DOMHORAINI = conn.DatoLector<string>("U_SO1_DOMHORAINI");
            promocion.U_SO1_DOMHORAFIN = conn.DatoLector<string>("U_SO1_DOMHORAFIN");
            promocion.U_SO1_ARTICULOENLACE = conn.DatoLector<string>("U_SO1_ARTICULOENLACE");
            promocion.U_SO1_FILTRARARTART = conn.DatoLector<string>("U_SO1_FILTRARARTART");
            promocion.U_SO1_FILTRARARTGRU = conn.DatoLector<string>("U_SO1_FILTRARARTGRU");
            promocion.U_SO1_FILTRARARTPRO = conn.DatoLector<string>("U_SO1_FILTRARARTPRO");
            promocion.U_SO1_FILTRARARTPROE = conn.DatoLector<string>("U_SO1_FILTRARARTPROE");
            promocion.U_SO1_FILTRARARTPROC = conn.DatoLector<string>("U_SO1_FILTRARARTPROC");
            promocion.U_SO1_FILTRARARTCOE = conn.DatoLector<string>("U_SO1_FILTRARARTCOE");
            promocion.U_SO1_FILTRARARTCOEC = conn.DatoLector<string>("U_SO1_FILTRARARTCOEC");
            promocion.U_SO1_FILTRARARTCOEV = conn.DatoLector<string>("U_SO1_FILTRARARTCOEV");
            promocion.U_SO1_FILTRARARTPROV = conn.DatoLector<string>("U_SO1_FILTRARARTPROV");
            promocion.U_SO1_FILTRARARTFAB = conn.DatoLector<string>("U_SO1_FILTRARARTFAB");
            promocion.U_SO1_FILTRARARTEXC = conn.DatoLector<string>("U_SO1_FILTRARARTEXC");
            promocion.U_SO1_FILTRARARTUNE = conn.DatoLector<string>("U_SO1_FILTRARARTUNE");
            promocion.U_SO1_FILTRARPROMSUC = conn.DatoLector<string>("U_SO1_FILTRARPROMSUC");
            promocion.U_SO1_FILTRARPROMALI = conn.DatoLector<string>("U_SO1_FILTRARPROMALI");
            promocion.U_SO1_FILTRARPROMCLI = conn.DatoLector<string>("U_SO1_FILTRARPROMCLI");
            promocion.U_SO1_FILTRARCLICLI = conn.DatoLector<string>("U_SO1_FILTRARCLICLI");
            promocion.U_SO1_FILTRARCLIENLS = conn.DatoLector<string>("U_SO1_FILTRARCLIENLS");
            promocion.U_SO1_FILTRARCLIGRU = conn.DatoLector<string>("U_SO1_FILTRARCLIGRU");
            promocion.U_SO1_FILTRARCLIPRO = conn.DatoLector<string>("U_SO1_FILTRARCLIPRO");
            promocion.U_SO1_FILTRARCLIPROE = conn.DatoLector<string>("U_SO1_FILTRARCLIPROE");
            promocion.U_SO1_FILTRARCLIPROC = conn.DatoLector<string>("U_SO1_FILTRARCLIPROC");
            promocion.U_SO1_FILTRARCLICOE = conn.DatoLector<string>("U_SO1_FILTRARCLICOE");
            promocion.U_SO1_FILTRARCLICOEC = conn.DatoLector<string>("U_SO1_FILTRARCLICOEC");
            promocion.U_SO1_FILTRARCLICOEV = conn.DatoLector<string>("U_SO1_FILTRARCLICOEV");
            promocion.U_SO1_FILTRARPROMMEM = conn.DatoLector<string>("U_SO1_FILTRARPROMMEM");
            promocion.U_SO1_FILTRARPROMLIS = conn.DatoLector<string>("U_SO1_FILTRARPROMLIS");
            promocion.U_SO1_FILTRARPROMFOP = conn.DatoLector<string>("U_SO1_FILTRARPROMFOP");
            promocion.U_SO1_ACUMPROMORM = conn.DatoLector<string>("U_SO1_ACUMPROMORM");
            promocion.U_SO1_ACUMPROMOPUNTO = conn.DatoLector<string>("U_SO1_ACUMPROMOPUNTO");
            promocion.U_SO1_ACUMPROMOOTRAS = conn.DatoLector<string>("U_SO1_ACUMPROMOOTRAS");
            promocion.U_SO1_ACTLIMPIEZAVEN = conn.DatoLector<string>("U_SO1_ACTLIMPIEZAVEN");
            promocion.U_SO1_LIMITEPIEZAVEN = conn.DatoLector<string>("U_SO1_LIMITEPIEZAVEN");
            promocion.U_SO1_ACTLIMPIEZAPRO = conn.DatoLector<string>("U_SO1_ACTLIMPIEZAPRO");
            promocion.U_SO1_LIMITEPIEZAPRO = conn.DatoLector<string>("U_SO1_LIMITEPIEZAPRO");
            promocion.U_SO1_COMPORTAESCAUT = conn.DatoLector<string>("U_SO1_COMPORTAESCAUT");
            promocion.U_SO1_ACTLIMVENTAPRO = conn.DatoLector<string>("U_SO1_ACTLIMVENTAPRO");
            promocion.U_SO1_LIMITEVENTAPRO = conn.DatoLector<string>("U_SO1_LIMITEVENTAPRO");
            promocion.U_SO1_ACTVENTACREDIT = conn.DatoLector<string>("U_SO1_ACTVENTACREDIT");
            promocion.U_SO1_FILTRARARTCON = conn.DatoLector<string>("U_SO1_FILTRARARTCON");
            promocion.U_SO1_FILTRARARTCONV = conn.DatoLector<string>("U_SO1_FILTRARARTCONV");

            return promocion;
        }

        #region CreaPromocion

        public bool CreaPromocion(Conexion conn)
        {
            #region Bitácora

            // AUTOR:               Juan Sol
            // FECHA:               22.07.2019
            // COMENTARIOS:         Se sustituye el DIAPI a SQL

            #endregion

            int UltimaPromocion = 0;

            try
            {
                conn.LimpiarParametros();
                conn.CargarConsulta(ConsultasPromocion.UltimoCodigoControl);
                conn.EjecutarLector();
                if (conn.LeerFila())
                {
                    UltimaPromocion = conn.DatoLector<int>("Promo") + 1;
                }
                conn.CerrarLector();
                conn.LimpiarParametros();
                conn.AgregarParametro("pCodigo", Codigo);
                conn.CargarConsulta(ConsultasPromocion.VerificaCodigoUnico);
                conn.EjecutarLector();
                bool codigoRepetido = false;
                if (conn.LeerFila())
                {
                    codigoRepetido = true;
                }
                conn.CerrarLector();
                if (codigoRepetido)
                {
                    Log.GuardarError("Está repetido el código: " + Codigo, null);
                    throw new Exception("prom1"); // El código de la promoción ya se encuentra en uso
                }
                conn.LimpiarParametros();
                conn.AgregarParametro("pNombre", Nombre);
                conn.CargarConsulta(ConsultasPromocion.VerificaNombreUnico);
                conn.EjecutarLector();
                bool nombreRepetido = false;
                if (conn.LeerFila())
                {
                    nombreRepetido = true;
                }
                conn.CerrarLector();
                if (nombreRepetido)
                {
                    Log.GuardarError("Está repetido el nombre: " + Nombre, null);
                    throw new Exception("prom2"); // El nombre de la promoción ya se encuentra en uso
                }

                /* Asignamos los valores de promocion y jerarquia */

                U_SO1_PROMOCION = UltimaPromocion;
                U_SO1_JERARQUIA = UltimaPromocion;

                int horaIni = 0;
                int horaFin = 0;
                int horaLunIni = 0;
                int horaLunFin = 0;
                int horaMarIni = 0;
                int horaMarFin = 0;
                int horaMieIni = 0;
                int horaMieFin = 0;
                int horaJueIni = 0;
                int horaJueFin = 0;
                int horaVieIni = 0;
                int horaVieFin = 0;
                int horaSabIni = 0;
                int horaSabFin = 0;
                int horaDomIni = 0;
                int horaDomFin = 0;

                Int32.TryParse(U_SO1_DIAHORAINI, out horaIni);
                Int32.TryParse(U_SO1_DIAHORAINI, out horaFin);
                Int32.TryParse(U_SO1_LUNHORAINI, out horaLunIni);
                Int32.TryParse(U_SO1_LUNHORAFIN, out horaLunFin);
                Int32.TryParse(U_SO1_MARHORAINI, out horaMarIni);
                Int32.TryParse(U_SO1_MARHORAFIN, out horaMarFin);
                Int32.TryParse(U_SO1_MIEHORAINI, out horaMieIni);
                Int32.TryParse(U_SO1_MIEHORAFIN, out horaMieFin);
                Int32.TryParse(U_SO1_JUEHORAINI, out horaJueIni);
                Int32.TryParse(U_SO1_JUEHORAFIN, out horaJueFin);
                Int32.TryParse(U_SO1_VIEHORAINI, out horaVieIni);
                Int32.TryParse(U_SO1_VIEHORAFIN, out horaVieFin);
                Int32.TryParse(U_SO1_SABHORAINI, out horaSabIni);
                Int32.TryParse(U_SO1_SABHORAFIN, out horaSabFin);
                Int32.TryParse(U_SO1_DOMHORAINI, out horaDomIni);
                Int32.TryParse(U_SO1_DOMHORAFIN, out horaDomFin);

                conn.LimpiarParametros();
                conn.CargarConsulta(ConsultasPromocion.Registrar);

                //Datos Únicos
                conn.AgregarParametro("pCodigo", Codigo);
                conn.AgregarParametro("pNombre", Nombre);
                conn.AgregarParametro("pPromo", U_SO1_PROMOCION);
                conn.AgregarParametro("pJerarquia", U_SO1_JERARQUIA);
                conn.AgregarParametro("pTipo", U_SO1_TIPO);

                if (U_SO1_TIPO != "PR")
                {
                    DateTime FechaDesde = Convert.ToDateTime(U_SO1_FECHADESDE);
                    DateTime FechaHasta = Convert.ToDateTime(U_SO1_FECHAHASTA);
                    //Fechas
                    conn.AgregarParametro("pFechaDesde", FechaDesde);
                    conn.AgregarParametro("pFechaHasta", FechaHasta);
                }
                else
                {
                    conn.AgregarParametro("pFechaDesde", string.IsNullOrEmpty(U_SO1_FECHADESDE) ? 0 : 0);
                    conn.AgregarParametro("pFechaHasta", string.IsNullOrEmpty(U_SO1_FECHAHASTA) ? 0 : 0);
                }

                //Diario
                conn.AgregarParametro("pDiario", U_SO1_DIARIO);
                    conn.AgregarParametro("pDiaCompleto", U_SO1_DIACOMPLETO);
                    conn.AgregarParametro("pDiaHoraIni", string.IsNullOrEmpty(U_SO1_DIAHORAINI) ? 0 : horaIni);
                    conn.AgregarParametro("pDiaHoraFin", string.IsNullOrEmpty(U_SO1_DIAHORAFIN) ? 0 : horaFin);
                    //Lunes
                    conn.AgregarParametro("pLunes", U_SO1_LUNES);
                    conn.AgregarParametro("pLunesCompleto", U_SO1_LUNCOMPLETO);
                    conn.AgregarParametro("pLunHoraIni", string.IsNullOrEmpty(U_SO1_LUNHORAINI) ? 0 : horaLunIni);
                    conn.AgregarParametro("pLunHoraFin", string.IsNullOrEmpty(U_SO1_LUNHORAFIN) ? 0 : horaLunFin);
                    //Martes
                    conn.AgregarParametro("pMartes", U_SO1_MARTES);
                    conn.AgregarParametro("pMartesCompleto", U_SO1_MARCOMPLETO);
                    conn.AgregarParametro("pMarHoraIni", string.IsNullOrEmpty(U_SO1_MARHORAINI) ? 0 : horaMarIni);
                    conn.AgregarParametro("pMarHoraFin", string.IsNullOrEmpty(U_SO1_MARHORAFIN) ? 0 : horaMarFin);
                    //Miércoles
                    conn.AgregarParametro("pMiercoles", U_SO1_MIERCOLES);
                    conn.AgregarParametro("pMieCompleto", U_SO1_MIECOMPLETO);
                    conn.AgregarParametro("pMieHoraIni", string.IsNullOrEmpty(U_SO1_MIEHORAINI) ? 0 : horaMieIni);
                    conn.AgregarParametro("pMieHoraFin", string.IsNullOrEmpty(U_SO1_MIEHORAFIN) ? 0 : horaMieFin);
                    //Jueves
                    conn.AgregarParametro("pJueves", U_SO1_JUEVES);
                    conn.AgregarParametro("pJueCompleto", U_SO1_JUECOMPLETO);
                    conn.AgregarParametro("pJueHoraIni", string.IsNullOrEmpty(U_SO1_JUEHORAINI) ? 0 : horaJueIni);
                    conn.AgregarParametro("pJueHoraFin", string.IsNullOrEmpty(U_SO1_JUEHORAFIN) ? 0 : horaJueFin);
                    //Viernes
                    conn.AgregarParametro("pViernes", U_SO1_VIERNES);
                    conn.AgregarParametro("pVieCompleto", U_SO1_VIECOMPLETO);
                    conn.AgregarParametro("pVieHoraIni", string.IsNullOrEmpty(U_SO1_VIEHORAINI) ? 0 : horaVieIni);
                    conn.AgregarParametro("pVieHoraFin", string.IsNullOrEmpty(U_SO1_VIEHORAFIN) ? 0 : horaVieFin);
                    //Sábado
                    conn.AgregarParametro("pSabado", U_SO1_SABADO);
                    conn.AgregarParametro("pSabCompleto", U_SO1_SABCOMPLETO);
                    conn.AgregarParametro("pSabHoraIni", string.IsNullOrEmpty(U_SO1_SABHORAINI) ? 0 : horaSabIni);
                    conn.AgregarParametro("pSabHoraFin", string.IsNullOrEmpty(U_SO1_SABHORAFIN) ? 0 : horaSabFin);
                    //Domingo
                    conn.AgregarParametro("pDomingo", U_SO1_DOMINGO);
                    conn.AgregarParametro("pDomCompleto", U_SO1_DOMCOMPLETO);
                    conn.AgregarParametro("pDomHoraIni", string.IsNullOrEmpty(U_SO1_DOMHORAINI) ? 0 : horaDomIni);
                    conn.AgregarParametro("pDomHoraFin", string.IsNullOrEmpty(U_SO1_DOMHORAFIN) ? 0 : horaDomFin);
                    //Checks de Articulo
                    conn.AgregarParametro("pArticuloEnlace", U_SO1_ARTICULOENLACE);
                    conn.AgregarParametro("pFiltrarArtArt", U_SO1_FILTRARARTART);
                    conn.AgregarParametro("pFiltrarArtGru", U_SO1_FILTRARARTGRU);
                    conn.AgregarParametro("pFiltrarArtPro", U_SO1_FILTRARARTPRO);
                    conn.AgregarParametro("pFiltrarArtProe", U_SO1_FILTRARARTPROE);
                    conn.AgregarParametro("pFiltrarArtProc", U_SO1_FILTRARARTPROC);
                    conn.AgregarParametro("pFiltrarArtCoe", U_SO1_FILTRARARTCOE);
                    conn.AgregarParametro("pFiltrarArtCoec", U_SO1_FILTRARARTCOEC);
                    conn.AgregarParametro("pFiltrarArtCoev", U_SO1_FILTRARARTCOEV);
                    conn.AgregarParametro("pFiltrarArtProv", U_SO1_FILTRARARTPROV);
                    conn.AgregarParametro("pFiltrarArtFab", U_SO1_FILTRARARTFAB);
                    conn.AgregarParametro("pFiltrarArtExc", U_SO1_FILTRARARTEXC);
                    conn.AgregarParametro("pFiltrarArtUne", U_SO1_FILTRARARTUNE);
                    //Checks de Promocion
                    conn.AgregarParametro("pFiltrarPromSuc", U_SO1_FILTRARPROMSUC);
                    conn.AgregarParametro("pFiltrarPromAli", U_SO1_FILTRARPROMALI);
                    conn.AgregarParametro("pFiltrarPromCli", U_SO1_FILTRARPROMCLI);
                    //Checks de Cliente
                    conn.AgregarParametro("pFiltrarCliCli", U_SO1_FILTRARCLICLI);
                    conn.AgregarParametro("pFiltrarCliEnls", U_SO1_FILTRARCLIENLS);
                    conn.AgregarParametro("pFiltrarCliGru", U_SO1_FILTRARCLIGRU);
                    conn.AgregarParametro("pFiltrarCliPro", U_SO1_FILTRARCLIPRO);
                    conn.AgregarParametro("pFiltrarCliProe", U_SO1_FILTRARCLIPROE);
                    conn.AgregarParametro("pFiltrarCliProc", U_SO1_FILTRARCLIPROC);
                    conn.AgregarParametro("pFiltrarCliCoe", U_SO1_FILTRARCLICOE);
                    conn.AgregarParametro("pFiltrarCliCoec", U_SO1_FILTRARCLICOEC);
                    conn.AgregarParametro("pFiltrarCliCoev", U_SO1_FILTRARCLICOEV);
                    //Checks de Promocion
                    conn.AgregarParametro("pFiltrarPromMem", U_SO1_FILTRARPROMMEM);
                    conn.AgregarParametro("pFiltrarPromLis", U_SO1_FILTRARPROMLIS);
                    conn.AgregarParametro("pFiltrarPromFop", U_SO1_FILTRARPROMFOP);
                    conn.AgregarParametro("pAcumProMorm", U_SO1_ACUMPROMORM);
                    conn.AgregarParametro("pAcumPromoPunto", U_SO1_ACUMPROMOPUNTO);
                    conn.AgregarParametro("pAcumPromoOtras", U_SO1_ACUMPROMOOTRAS);
                    conn.AgregarParametro("pActLimpiezaVen", U_SO1_ACTLIMPIEZAVEN);

                    // Convertimos el limite de piezas en double
                    double LimitePiezaVen = 0;
                    double LimitePiezaPro = 0;
                    double LimiteVentaPro = 0;
                    LimitePiezaVen = string.IsNullOrEmpty(U_SO1_LIMITEPIEZAVEN)?0:Convert.ToDouble(U_SO1_LIMITEPIEZAVEN);
                    LimitePiezaPro = string.IsNullOrEmpty(U_SO1_LIMITEPIEZAPRO) ? 0 : Convert.ToDouble(U_SO1_LIMITEPIEZAPRO);
                    LimiteVentaPro = string.IsNullOrEmpty(U_SO1_LIMITEVENTAPRO) ? 0 : Convert.ToDouble(U_SO1_LIMITEVENTAPRO);
                    
                    conn.AgregarParametro("pLimitePiezaVen", LimitePiezaVen);
                    conn.AgregarParametro("pActLimpiezaPro", U_SO1_ACTLIMPIEZAPRO);
                    conn.AgregarParametro("pLimitePiezaPro", LimitePiezaPro);
                    conn.AgregarParametro("pComportaEscaut", U_SO1_COMPORTAESCAUT);
                    conn.AgregarParametro("pActLimVentaPro", U_SO1_ACTLIMVENTAPRO);
                    conn.AgregarParametro("pLimiteVentaPro", LimiteVentaPro);
                    conn.AgregarParametro("pActVentaCredit", U_SO1_ACTVENTACREDIT);
                    conn.AgregarParametro("pFiltrarArtCon", U_SO1_FILTRARARTCON);
                    conn.AgregarParametro("pFiltrarArtConv", U_SO1_FILTRARARTCONV);
                //}
                //else {
                //    conn.AgregarParametro("pFechaDesde", string.IsNullOrEmpty(U_SO1_FECHADESDE) ? 0 : 0);
                //    conn.AgregarParametro("pFechaHasta", string.IsNullOrEmpty(U_SO1_FECHAHASTA) ? 0 : 0);                   
                //    //Diario
                //    conn.AgregarParametro("pDiario", U_SO1_DIARIO);
                //    conn.AgregarParametro("pDiaCompleto", U_SO1_DIACOMPLETO);
                //    conn.AgregarParametro("pDiaHoraIni", string.IsNullOrEmpty(U_SO1_DIAHORAINI) ? 0 : horaIni);
                //    conn.AgregarParametro("pDiaHoraFin", string.IsNullOrEmpty(U_SO1_DIAHORAFIN) ? 0 : horaFin);
                //    //Lunes
                //    conn.AgregarParametro("pLunes", U_SO1_LUNES);
                //    conn.AgregarParametro("pLunesCompleto", U_SO1_LUNCOMPLETO);
                //    conn.AgregarParametro("pLunHoraIni", string.IsNullOrEmpty(U_SO1_LUNHORAINI) ? 0 : horaLunIni);
                //    conn.AgregarParametro("pLunHoraFin", string.IsNullOrEmpty(U_SO1_LUNHORAFIN) ? 0 : horaLunFin);
                //    //Martes
                //    conn.AgregarParametro("pMartes", U_SO1_MARTES);
                //    conn.AgregarParametro("pMartesCompleto", U_SO1_MARCOMPLETO);
                //    conn.AgregarParametro("pMarHoraIni", string.IsNullOrEmpty(U_SO1_MARHORAINI) ? 0 : horaMarIni);
                //    conn.AgregarParametro("pMarHoraFin", string.IsNullOrEmpty(U_SO1_MARHORAFIN) ? 0 : horaMarFin);
                //    //Miércoles
                //    conn.AgregarParametro("pMiercoles", U_SO1_MIERCOLES);
                //    conn.AgregarParametro("pMieCompleto", U_SO1_MIECOMPLETO);
                //    conn.AgregarParametro("pMieHoraIni", string.IsNullOrEmpty(U_SO1_MIEHORAINI) ? 0 : horaMieIni);
                //    conn.AgregarParametro("pMieHoraFin", string.IsNullOrEmpty(U_SO1_MIEHORAFIN) ? 0 : horaMieFin);
                //    //Jueves
                //    conn.AgregarParametro("pJueves", U_SO1_JUEVES);
                //    conn.AgregarParametro("pJueCompleto", U_SO1_JUECOMPLETO);
                //    conn.AgregarParametro("pJueHoraIni", string.IsNullOrEmpty(U_SO1_JUEHORAINI) ? 0 : horaJueIni);
                //    conn.AgregarParametro("pJueHoraFin", string.IsNullOrEmpty(U_SO1_JUEHORAFIN) ? 0 : horaJueFin);
                //    //Viernes
                //    conn.AgregarParametro("pViernes", U_SO1_VIERNES);
                //    conn.AgregarParametro("pVieCompleto", U_SO1_VIECOMPLETO);
                //    conn.AgregarParametro("pVieHoraIni", string.IsNullOrEmpty(U_SO1_VIEHORAINI) ? 0 : horaVieIni);
                //    conn.AgregarParametro("pVieHoraFin", string.IsNullOrEmpty(U_SO1_VIEHORAFIN) ? 0 : horaVieFin);
                //    //Sábado
                //    conn.AgregarParametro("pSabado", U_SO1_SABADO);
                //    conn.AgregarParametro("pSabCompleto", U_SO1_SABCOMPLETO);
                //    conn.AgregarParametro("pSabHoraIni", string.IsNullOrEmpty(U_SO1_SABHORAINI) ? 0 : horaSabIni);
                //    conn.AgregarParametro("pSabHoraFin", string.IsNullOrEmpty(U_SO1_SABHORAFIN) ? 0 : horaSabFin);
                //    //Domingo
                //    conn.AgregarParametro("pDomingo", U_SO1_DOMINGO);
                //    conn.AgregarParametro("pDomCompleto", U_SO1_DOMCOMPLETO);
                //    conn.AgregarParametro("pDomHoraIni", string.IsNullOrEmpty(U_SO1_DOMHORAINI) ? 0 : horaDomIni);
                //    conn.AgregarParametro("pDomHoraFin", string.IsNullOrEmpty(U_SO1_DOMHORAFIN) ? 0 : horaDomFin);
                //    //Checks de Articulo
                //    conn.AgregarParametro("pArticuloEnlace", U_SO1_ARTICULOENLACE);
                //    conn.AgregarParametro("pFiltrarArtArt", U_SO1_FILTRARARTART);
                //    conn.AgregarParametro("pFiltrarArtGru", U_SO1_FILTRARARTGRU);
                //    conn.AgregarParametro("pFiltrarArtPro", U_SO1_FILTRARARTPRO);
                //    conn.AgregarParametro("pFiltrarArtProe", U_SO1_FILTRARARTPROE);
                //    conn.AgregarParametro("pFiltrarArtProc", U_SO1_FILTRARARTPROC);
                //    conn.AgregarParametro("pFiltrarArtCoe", U_SO1_FILTRARARTCOE);
                //    conn.AgregarParametro("pFiltrarArtCoec", U_SO1_FILTRARARTCOEC);
                //    conn.AgregarParametro("pFiltrarArtCoev", U_SO1_FILTRARARTCOEV);
                //    conn.AgregarParametro("pFiltrarArtProv", U_SO1_FILTRARARTPROV);
                //    conn.AgregarParametro("pFiltrarArtFab", U_SO1_FILTRARARTFAB);
                //    conn.AgregarParametro("pFiltrarArtExc", U_SO1_FILTRARARTEXC);
                //    conn.AgregarParametro("pFiltrarArtUne", U_SO1_FILTRARARTUNE);
                //    //Checks de Promocion
                //    conn.AgregarParametro("pFiltrarPromSuc", U_SO1_FILTRARPROMSUC);
                //    conn.AgregarParametro("pFiltrarPromAli", U_SO1_FILTRARPROMALI);
                //    conn.AgregarParametro("pFiltrarPromCli", U_SO1_FILTRARPROMCLI);
                //    //Checks de Cliente
                //    conn.AgregarParametro("pFiltrarCliCli", U_SO1_FILTRARCLICLI);
                //    conn.AgregarParametro("pFiltrarCliEnls", U_SO1_FILTRARCLIENLS);
                //    conn.AgregarParametro("pFiltrarCliGru", U_SO1_FILTRARCLIGRU);
                //    conn.AgregarParametro("pFiltrarCliPro", U_SO1_FILTRARCLIPRO);
                //    conn.AgregarParametro("pFiltrarCliProe", U_SO1_FILTRARCLIPROE);
                //    conn.AgregarParametro("pFiltrarCliProc", U_SO1_FILTRARCLIPROC);
                //    conn.AgregarParametro("pFiltrarCliCoe", U_SO1_FILTRARCLICOE);
                //    conn.AgregarParametro("pFiltrarCliCoec", U_SO1_FILTRARCLICOEC);
                //    conn.AgregarParametro("pFiltrarCliCoev", U_SO1_FILTRARCLICOEV);
                //    //Checks de Promocion
                //    conn.AgregarParametro("pFiltrarPromMem", U_SO1_FILTRARPROMMEM);
                //    conn.AgregarParametro("pFiltrarPromLis", U_SO1_FILTRARPROMLIS);
                //    conn.AgregarParametro("pFiltrarPromFop", U_SO1_FILTRARPROMFOP);
                //    conn.AgregarParametro("pAcumProMorm", U_SO1_ACUMPROMORM);
                //    conn.AgregarParametro("pAcumPromoPunto", U_SO1_ACUMPROMOPUNTO);
                //    conn.AgregarParametro("pAcumPromoOtras", U_SO1_ACUMPROMOOTRAS);
                //    conn.AgregarParametro("pActLimpiezaVen", U_SO1_ACTLIMPIEZAVEN);
                //    conn.AgregarParametro("pLimitePiezaVen", U_SO1_LIMITEPIEZAVEN);
                //    conn.AgregarParametro("pActLimpiezaPro", U_SO1_ACTLIMPIEZAPRO);
                //    conn.AgregarParametro("pLimitePiezaPro", U_SO1_LIMITEPIEZAPRO);
                //    conn.AgregarParametro("pComportaEscaut", U_SO1_COMPORTAESCAUT);
                //    conn.AgregarParametro("pActLimVentaPro", U_SO1_ACTLIMPIEZAPRO);
                //    conn.AgregarParametro("pLimiteVentaPro", U_SO1_LIMITEVENTAPRO);
                //    conn.AgregarParametro("pActVentaCredit", U_SO1_ACTVENTACREDIT);
                //    conn.AgregarParametro("pFiltrarArtCon", U_SO1_FILTRARARTCON);
                //    conn.AgregarParametro("pFiltrarArtConv", U_SO1_FILTRARARTCONV);
                //}
                //string consulta = conn.TextoComando;
                //Log.GuardarError(consulta, null);

                conn.EjecutarComando();
                conn.LimpiarParametros();
                conn.CargarConsulta(ConsultasPromocion.ActualizarCodigoControl);
                conn.AgregarParametro("pCodigo", U_SO1_PROMOCION); //Se guarda el número de la actual promoción para ser tomado para la creación de la siguiente promoción.
                conn.EjecutarComando();

                Log.GuardarError("Actualización exitosa", null);
                return true;

            }
            catch (Exception oError)
            {
                //Log.GuardarError("Descripción de error: ", oError);
                throw new Exception("ErrorModificarPromo", oError);
            }
        }

        #endregion

        #region GuardarLicenciamiento

        public bool RegistrarCambios(string Tipo)
        {
            string nombreDB = Sistema.Ambientes.Predeterminado.Origen.NombreBD;
            //[@SO1_DT_01SBODEMOMX]
            string nombreTabla = "@SO1_DT_01" + nombreDB;
            Log.GuardarError("Accedemos a la conexión de la base de datos de Licenciamiento", null);
            Log.GuardarError("Tabla :" + nombreTabla, null);
            using (Conexion conn = new Conexion(OrigenDatosSecundario))
            //using (Conexion conn = new Conexion())
            {
                Log.GuardarError("Conexión realizada con éxito", null);
                conn.LimpiarParametros();
                conn.CargarConsulta(ConsultasPromocion.PostTransaccion);
                conn.AgregarLiteral("TABLA", nombreTabla);
                if(OrigenDatosSecundario.Proveedor == Proveedor.Hana)
                {
                    conn.AgregarLiteral("POST", "IDT" + nombreDB);
                }
                conn.AgregarParametro("pTipoObjeto", "-3\t@SO1_01PROMOCION");
                conn.AgregarParametro("pTipoTransaccion", Tipo);
                conn.AgregarParametro("pNumColumnas", 1);
                conn.AgregarParametro("pListaColumnas", "Code");
                conn.AgregarParametro("pListaValores", Codigo);
                conn.EjecutarComando();
                Log.GuardarError("Cambios guardados con éxito", null);
            }
            return true;
        }

        #endregion

        #region Eliminacion Generica

        public bool EliminarPromo(Conexion conn, string nombreTabla)
        {
            #region Bitácora

            // AUTOR:               Mario Lopez
            // FECHA:               05.07.2019
            // COMENTARIOS:         Creacion del método

            #endregion

            Log.GuardarError("numero: " + U_SO1_PROMOCION, null);
            Log.GuardarError("Asignamos la tabla", null);

            conn.LimpiarParametros();
            conn.CargarConsulta(ConsultasPromocion.EliminarPromo);
            conn.AgregarLiteral("TABLA", nombreTabla);
            conn.AgregarParametro("pNumPromocion", U_SO1_PROMOCION);
            conn.EjecutarComando();

            Log.GuardarError("Eliminacion exitosa", null);
            return true;
        }

        public void EliminarPoliticaRedencion(Conexion conn)
        {
            #region Bitácora

            // AUTOR:               Juan Sol
            // FECHA:               29.11.2019
            // COMENTARIOS:         Creacion del método

            #endregion

            conn.LimpiarParametros();
            conn.CargarConsulta(ConsultasPromocion.EliminarPoliticaRedencion);
            conn.AgregarParametro("pCadena", Codigo);
            conn.EjecutarComando();

            Log.GuardarError("Eliminacion exitosa", null);

            return;
        }

        public void EliminarIdentificadores(Conexion conn)
        {
            Log.GuardarError("Asignamos la tabla", null);
            conn.LimpiarParametros();
            conn.CargarConsulta(ConsultasPromocion.EliminarIdentificadores);
            conn.AgregarLiteral("TABLA", "@SO1_01PROMOAMBITO");
            conn.AgregarParametro("pCodigo", PromocionKitVenta.Code);
            conn.EjecutarComando();

            Log.GuardarError("Eliminación exitosa", null);
            return;
        }

        public void EliminarArticulos(Conexion conn)
        {
            #region Bitácora

            // AUTOR:               Mario Lopez
            // FECHA:               05.07.2019
            // COMENTARIOS:         Creacion del método

            #endregion

            Log.GuardarError("Asignamos la tabla", null);

            conn.LimpiarParametros();
            conn.CargarConsulta(ConsultasPromocion.EliminarPromo);
            conn.AgregarLiteral("TABLA", "@SO1_01PROMOARTICULO");
            conn.AgregarParametro("pNumPromocion", U_SO1_PROMOCION);
            conn.EjecutarComando();

            Log.GuardarError("Eliminacion exitosa", null);
            return;
        }

        public void EliminarSucursales(Conexion conn)
        {
            #region Bitácora

            // AUTOR:               Mario Lopez
            // FECHA:               05.07.2019
            // COMENTARIOS:         Creacion del método

            #endregion

            Log.GuardarError("Asignamos la tabla", null);

            conn.LimpiarParametros();
            conn.CargarConsulta(ConsultasPromocion.EliminarPromo);
            conn.AgregarLiteral("TABLA", "@SO1_01PROMOSUCURSAL");
            conn.AgregarParametro("pNumPromocion", U_SO1_PROMOCION);
            conn.EjecutarComando();

            Log.GuardarError("Eliminacion exitosa", null);

            return;
        }

        public void EliminarClientes(Conexion conn)
        {
            #region Bitácora

            // AUTOR:               Mario Lopez
            // FECHA:               05.07.2019
            // COMENTARIOS:         Creacion del método

            #endregion


            Log.GuardarError("Asignamos la tabla", null);

            conn.LimpiarParametros();
            conn.CargarConsulta(ConsultasPromocion.EliminarPromo);
            conn.AgregarLiteral("TABLA", "@SO1_01PROMOCLIENTE");
            conn.AgregarParametro("pNumPromocion", U_SO1_PROMOCION);
            conn.EjecutarComando();

            Log.GuardarError("Eliminacion exitosa", null);

            return;
        }

        public void EliminarAlianzas(Conexion conn)
        {
            #region Bitácora

            // AUTOR:               Mario Lopez
            // FECHA:               05.07.2019
            // COMENTARIOS:         Creacion del método

            #endregion


            Log.GuardarError("Asignamos la tabla", null);

            conn.LimpiarParametros();
            conn.CargarConsulta(ConsultasPromocion.EliminarPromo);
            conn.AgregarLiteral("TABLA", "@SO1_01PROMOALICOMER");
            conn.AgregarParametro("pNumPromocion", U_SO1_PROMOCION);
            conn.EjecutarComando();

            Log.GuardarError("Eliminacion exitosa", null);

            return;
        }

        public void EliminarMembresias(Conexion conn)
        {
            #region Bitácora

            // AUTOR:               Mario Lopez
            // FECHA:               05.07.2019
            // COMENTARIOS:         Creacion del método

            #endregion


            Log.GuardarError("Asignamos la tabla", null);

            conn.LimpiarParametros();
            conn.CargarConsulta(ConsultasPromocion.EliminarPromo);
            conn.AgregarLiteral("TABLA", "@SO1_01PROMOTIPOMEMB");
            conn.AgregarParametro("pNumPromocion", U_SO1_PROMOCION);
            conn.EjecutarComando();

            Log.GuardarError("Eliminacion exitosa", null);

            return;
        }

        public void EliminarPrecios(Conexion conn)
        {
            #region Bitácora

            // AUTOR:               Mario Lopez
            // FECHA:               05.07.2019
            // COMENTARIOS:         Creacion del método

            #endregion


            Log.GuardarError("Asignamos la tabla", null);

            conn.LimpiarParametros();
            conn.CargarConsulta(ConsultasPromocion.EliminarPromo);
            conn.AgregarLiteral("TABLA", "@SO1_01PROMOLISTPREC");
            conn.AgregarParametro("pNumPromocion", U_SO1_PROMOCION);
            conn.EjecutarComando();

            Log.GuardarError("Eliminacion exitosa", null);

            return;
        }

        public void EliminarFPago(Conexion conn)
        {
            #region Bitácora

            // AUTOR:               Mario Lopez
            // FECHA:               05.07.2019
            // COMENTARIOS:         Creacion del método

            #endregion


            Log.GuardarError("Asignamos la tabla", null);

            conn.LimpiarParametros();
            conn.CargarConsulta(ConsultasPromocion.EliminarPromo);
            conn.AgregarLiteral("TABLA", "@SO1_01PROMOFORMAPAG");
            conn.AgregarParametro("pNumPromocion", U_SO1_PROMOCION);
            conn.EjecutarComando();

            Log.GuardarError("Eliminacion exitosa", null);

            return;
        }

        public void EliminarArticuloFabricante(Conexion conn)
        {
            #region Bitácora

            // AUTOR:               Mario Lopez
            // FECHA:               05.07.2019
            // COMENTARIOS:         Creacion del método

            #endregion


            Log.GuardarError("Asignamos la tabla", null);

            conn.LimpiarParametros();
            conn.CargarConsulta(ConsultasPromocion.EliminarPromo);
            conn.AgregarLiteral("TABLA", "@SO1_01PROMOARTIFABR");
            conn.AgregarParametro("pNumPromocion", U_SO1_PROMOCION);
            conn.EjecutarComando();

            Log.GuardarError("Eliminacion exitosa", null);

            return;
        }

        public void EliminarArticuloMedida(Conexion conn)
        {
            #region Bitácora

            // AUTOR:               Mario Lopez
            // FECHA:               05.07.2019
            // COMENTARIOS:         Creacion del método

            #endregion


            Log.GuardarError("Asignamos la tabla", null);

            conn.LimpiarParametros();
            conn.CargarConsulta(ConsultasPromocion.EliminarPromo);
            conn.AgregarLiteral("TABLA", "@SO1_01PROMOARTIUNIE");
            conn.AgregarParametro("pNumPromocion", U_SO1_PROMOCION);
            conn.EjecutarComando();

            Log.GuardarError("Eliminacion exitosa", null);

            return;
        }

        public void EliminarArticuloProveedor(Conexion conn)
        {
            #region Bitácora

            // AUTOR:               Mario Lopez
            // FECHA:               05.07.2019
            // COMENTARIOS:         Creacion del método

            #endregion


            Log.GuardarError("Asignamos la tabla", null);

            conn.LimpiarParametros();
            conn.CargarConsulta(ConsultasPromocion.EliminarPromo);
            conn.AgregarLiteral("TABLA", "@SO1_01PROMOARTIPROV");
            conn.AgregarParametro("pNumPromocion", U_SO1_PROMOCION);
            conn.EjecutarComando();

            Log.GuardarError("Eliminacion exitosa", null);

            return;
        }

        public void EliminarGrupoArticulo(Conexion conn)
        {
            #region Bitácora

            // AUTOR:               Mario Lopez
            // FECHA:               05.07.2019
            // COMENTARIOS:         Creacion del método

            #endregion


            Log.GuardarError("Asignamos la tabla", null);

            conn.LimpiarParametros();
            conn.CargarConsulta(ConsultasPromocion.EliminarPromo);
            conn.AgregarLiteral("TABLA", "@SO1_01PROMOARTIGRUP");
            conn.AgregarParametro("pNumPromocion", U_SO1_PROMOCION);
            conn.EjecutarComando();

            Log.GuardarError("Eliminacion exitosa", null);

            return;
        }

        public void EliminarPropiedadArticulo(Conexion conn)
        {
            #region Bitácora

            // AUTOR:               Mario Lopez
            // FECHA:               05.07.2019
            // COMENTARIOS:         Creacion del método

            #endregion


            Log.GuardarError("Asignamos la tabla", null);

            conn.LimpiarParametros();
            conn.CargarConsulta(ConsultasPromocion.EliminarPromo);
            conn.AgregarLiteral("TABLA", "@SO1_01PROMOARTIPROP");
            conn.AgregarParametro("pNumPromocion", U_SO1_PROMOCION);
            conn.EjecutarComando();

            Log.GuardarError("Eliminacion exitosa", null);

            return;
        }

        public void EliminarGrupoCliente(Conexion conn)
        {
            #region Bitácora

            // AUTOR:               Mario Lopez
            // FECHA:               05.07.2019
            // COMENTARIOS:         Creacion del método

            #endregion


            Log.GuardarError("Asignamos la tabla", null);

            conn.LimpiarParametros();
            conn.CargarConsulta(ConsultasPromocion.EliminarPromo);
            conn.AgregarLiteral("TABLA", "@SO1_01PROMOCLIEGRUP");
            conn.AgregarParametro("pNumPromocion", U_SO1_PROMOCION);
            conn.EjecutarComando();

            Log.GuardarError("Eliminacion exitosa", null);

            return;
        }

        public void EliminarPropiedadCliente(Conexion conn)
        {
            #region Bitácora

            // AUTOR:               Mario Lopez
            // FECHA:               05.07.2019
            // COMENTARIOS:         Creacion del método

            #endregion


            Log.GuardarError("Asignamos la tabla", null);

            conn.LimpiarParametros();
            conn.CargarConsulta(ConsultasPromocion.EliminarPromo);
            conn.AgregarLiteral("TABLA", "@SO1_01PROMOCLIEPROP");
            conn.AgregarParametro("pNumPromocion", U_SO1_PROMOCION);
            conn.EjecutarComando();

            Log.GuardarError("Eliminacion exitosa", null);

            return;
        }

        #endregion

        #region Eliminacion Promociones

        /*public void EliminarAxB(Conexion conn)
        {
            #region Bitácora

            // AUTOR:               Mario Lopez
            // FECHA:               05.07.2019
            // COMENTARIOS:         Creacion del método

            #endregion

            Log.Registrar("Asignamos la tabla", null);
           
            conn.LimpiarParametros();
            conn.CargarConsulta(ConsultasPromocion.EliminarRegistro);
            conn.AgregarLiteral("TABLA", "@SO1_01PROMOAXB");
            conn.AgregarParametro("pNumPromocion", U_SO1_PROMOCION);
            conn.EjecutarComando();

            Log.Registrar("Eliminacion exitosa", null);
            return;
        }*/

        #endregion

        #region Modificacion Generica

        public bool ModificarPromo(Conexion conn)
        {
            #region Bitácora

            // AUTOR:               Juan Sol
            // FECHA:               22.07.2019
            // COMENTARIOS:         Se sustituye el DIAPI a SQL

            #endregion

            try
            {
                Log.GuardarError(Codigo, null);
                Log.GuardarError("Asignamos la tabla", null);

                int horaIni = 0;
                int horaFin = 0;
                int horaLunIni = 0;
                int horaLunFin = 0;
                int horaMarIni = 0;
                int horaMarFin = 0;
                int horaMieIni = 0;
                int horaMieFin = 0;
                int horaJueIni = 0;
                int horaJueFin = 0;
                int horaVieIni = 0;
                int horaVieFin = 0;
                int horaSabIni = 0;
                int horaSabFin = 0;
                int horaDomIni = 0;
                int horaDomFin = 0;
                Int32.TryParse(U_SO1_DIAHORAINI, out horaIni);
                Int32.TryParse(U_SO1_DIAHORAFIN, out horaFin);
                Int32.TryParse(U_SO1_LUNHORAINI, out horaLunIni);
                Int32.TryParse(U_SO1_LUNHORAFIN, out horaLunFin);
                Int32.TryParse(U_SO1_MARHORAINI, out horaMarIni);
                Int32.TryParse(U_SO1_MARHORAFIN, out horaMarFin);
                Int32.TryParse(U_SO1_MIEHORAINI, out horaMieIni);
                Int32.TryParse(U_SO1_MIEHORAFIN, out horaMieFin);
                Int32.TryParse(U_SO1_JUEHORAINI, out horaJueIni);
                Int32.TryParse(U_SO1_JUEHORAFIN, out horaJueFin);
                Int32.TryParse(U_SO1_VIEHORAINI, out horaVieIni);
                Int32.TryParse(U_SO1_VIEHORAFIN, out horaVieFin);
                Int32.TryParse(U_SO1_SABHORAINI, out horaSabIni);
                Int32.TryParse(U_SO1_SABHORAFIN, out horaSabFin);
                Int32.TryParse(U_SO1_DOMHORAINI, out horaDomIni);
                Int32.TryParse(U_SO1_DOMHORAFIN, out horaDomFin);

                Log.GuardarError("Empezamos a guardar los valores", null);
                conn.LimpiarParametros();
                conn.CargarConsulta(ConsultasPromocion.VerificaNombre);
                conn.AgregarParametro("pNombre", Nombre);
                conn.AgregarParametro("pCodigo", Codigo);
                conn.EjecutarLector();
                bool nombreRepetido = false;
                if (conn.LeerFila())
                {
                    nombreRepetido = true;
                }
                conn.CerrarLector();
                if(nombreRepetido)
                {
                    Log.GuardarError("Está repetido el nombre: " + Nombre, null);
                    throw new Exception("prom2"); // El nombre de la promoción ya se encuentra en uso
                }
                conn.LimpiarParametros();
                conn.CargarConsulta(ConsultasPromocion.Modificar);
                //if (U_SO1_TIPO != "PR")
                //{
                    conn.AgregarParametro("pNombre", Nombre);
                    //Fechas
                    if (U_SO1_TIPO != "PR")
                    {
                        DateTime FechaDesde = Convert.ToDateTime(U_SO1_FECHADESDE);
                        DateTime FechaHasta = Convert.ToDateTime(U_SO1_FECHAHASTA);
                        //Fechas
                        conn.AgregarParametro("pFechaDesde", FechaDesde);
                        conn.AgregarParametro("pFechaHasta", FechaHasta);
                    }
                    else
                    {
                        conn.AgregarParametro("pFechaDesde", string.IsNullOrEmpty(U_SO1_FECHADESDE) ? 0 : 0);
                        conn.AgregarParametro("pFechaHasta", string.IsNullOrEmpty(U_SO1_FECHAHASTA) ? 0 : 0);
                    }
                    //Diario
                    conn.AgregarParametro("pDiario", U_SO1_DIARIO);
                    conn.AgregarParametro("pDiaCompleto", U_SO1_DIACOMPLETO);
                    conn.AgregarParametro("pDiaHoraIni", string.IsNullOrEmpty(U_SO1_DIAHORAINI) ? 0 : horaIni);
                    conn.AgregarParametro("pDiaHoraFin", string.IsNullOrEmpty(U_SO1_DIAHORAFIN) ? 0 : horaFin);
                    //Lunes
                    conn.AgregarParametro("pLunes", U_SO1_LUNES);
                    conn.AgregarParametro("pLunesCompleto", U_SO1_LUNCOMPLETO);
                    conn.AgregarParametro("pLunHoraIni", string.IsNullOrEmpty(U_SO1_LUNHORAINI) ? 0 : horaLunIni);
                    conn.AgregarParametro("pLunHoraFin", string.IsNullOrEmpty(U_SO1_LUNHORAFIN) ? 0 : horaLunFin);
                    //Martes
                    conn.AgregarParametro("pMartes", U_SO1_MARTES);
                    conn.AgregarParametro("pMartesCompleto", U_SO1_MARCOMPLETO);
                    conn.AgregarParametro("pMarHoraIni", string.IsNullOrEmpty(U_SO1_MARHORAINI) ? 0 : horaMarIni);
                    conn.AgregarParametro("pMarHoraFin", string.IsNullOrEmpty(U_SO1_MARHORAFIN) ? 0 : horaMarFin);
                    //Miércoles
                    conn.AgregarParametro("pMiercoles", U_SO1_MIERCOLES);
                    conn.AgregarParametro("pMieCompleto", U_SO1_MIECOMPLETO);
                    conn.AgregarParametro("pMieHoraIni", string.IsNullOrEmpty(U_SO1_MIEHORAINI) ? 0 : horaMieIni);
                    conn.AgregarParametro("pMieHoraFin", string.IsNullOrEmpty(U_SO1_MIEHORAFIN) ? 0 : horaMieFin);
                    //Jueves
                    conn.AgregarParametro("pJueves", U_SO1_JUEVES);
                    conn.AgregarParametro("pJueCompleto", U_SO1_JUECOMPLETO);
                    conn.AgregarParametro("pJueHoraIni", string.IsNullOrEmpty(U_SO1_JUEHORAINI) ? 0 : horaJueIni);
                    conn.AgregarParametro("pJueHoraFin", string.IsNullOrEmpty(U_SO1_JUEHORAFIN) ? 0 : horaJueFin);
                    //Viernes
                    conn.AgregarParametro("pViernes", U_SO1_VIERNES);
                    conn.AgregarParametro("pVieCompleto", U_SO1_VIECOMPLETO);
                    conn.AgregarParametro("pVieHoraIni", string.IsNullOrEmpty(U_SO1_VIEHORAINI) ? 0 : horaVieIni);
                    conn.AgregarParametro("pVieHoraFin", string.IsNullOrEmpty(U_SO1_VIEHORAFIN) ? 0 : horaVieFin);
                    //Sábado
                    conn.AgregarParametro("pSabado", U_SO1_SABADO);
                    conn.AgregarParametro("pSabCompleto", U_SO1_SABCOMPLETO);
                    conn.AgregarParametro("pSabHoraIni", string.IsNullOrEmpty(U_SO1_SABHORAINI) ? 0 : horaSabIni);
                    conn.AgregarParametro("pSabHoraFin", string.IsNullOrEmpty(U_SO1_SABHORAFIN) ? 0 : horaSabFin);
                    //Domingo
                    conn.AgregarParametro("pDomingo", U_SO1_DOMINGO);
                    conn.AgregarParametro("pDomCompleto", U_SO1_DOMCOMPLETO);
                    conn.AgregarParametro("pDomHoraIni", string.IsNullOrEmpty(U_SO1_DOMHORAINI) ? 0 : horaDomIni);
                    conn.AgregarParametro("pDomHoraFin", string.IsNullOrEmpty(U_SO1_DOMHORAFIN) ? 0 : horaDomFin);
                    //Checks de Articulo
                    conn.AgregarParametro("pArticuloEnlace", U_SO1_ARTICULOENLACE);
                    conn.AgregarParametro("pFiltrarArtArt", U_SO1_FILTRARARTART);
                    conn.AgregarParametro("pFiltrarArtGru", U_SO1_FILTRARARTGRU);
                    conn.AgregarParametro("pFiltrarArtPro", U_SO1_FILTRARARTPRO);
                    conn.AgregarParametro("pFiltrarArtProe", U_SO1_FILTRARARTPROE);
                    conn.AgregarParametro("pFiltrarArtProc", U_SO1_FILTRARARTPROC);
                    conn.AgregarParametro("pFiltrarArtCoe", U_SO1_FILTRARARTCOE);
                    conn.AgregarParametro("pFiltrarArtCoec", U_SO1_FILTRARARTCOEC);
                    conn.AgregarParametro("pFiltrarArtCoev", U_SO1_FILTRARARTCOEV);
                    conn.AgregarParametro("pFiltrarArtProv", U_SO1_FILTRARARTPROV);
                    conn.AgregarParametro("pFiltrarArtFab", U_SO1_FILTRARARTFAB);
                    conn.AgregarParametro("pFiltrarArtExc", U_SO1_FILTRARARTEXC);
                    conn.AgregarParametro("pFiltrarArtUne", U_SO1_FILTRARARTUNE);
                    //Checks de Promocion
                    conn.AgregarParametro("pFiltrarPromSuc", U_SO1_FILTRARPROMSUC);
                    conn.AgregarParametro("pFiltrarPromAli", U_SO1_FILTRARPROMALI);
                    conn.AgregarParametro("pFiltrarPromCli", U_SO1_FILTRARPROMCLI);
                    //Checks de Cliente
                    conn.AgregarParametro("pFiltrarCliCli", U_SO1_FILTRARCLICLI);
                    conn.AgregarParametro("pFiltrarCliEnls", U_SO1_FILTRARCLIENLS);
                    conn.AgregarParametro("pFiltrarCliGru", U_SO1_FILTRARCLIGRU);
                    conn.AgregarParametro("pFiltrarCliPro", U_SO1_FILTRARCLIPRO);
                    conn.AgregarParametro("pFiltrarCliProe", U_SO1_FILTRARCLIPROE);
                    conn.AgregarParametro("pFiltrarCliProc", U_SO1_FILTRARCLIPROC);
                    conn.AgregarParametro("pFiltrarCliCoe", U_SO1_FILTRARCLICOE);
                    conn.AgregarParametro("pFiltrarCliCoec", U_SO1_FILTRARCLICOEC);
                    conn.AgregarParametro("pFiltrarCliCoev", U_SO1_FILTRARCLICOEV);
                    //Checks de Promocion
                    conn.AgregarParametro("pFiltrarPromMem", U_SO1_FILTRARPROMMEM);
                    conn.AgregarParametro("pFiltrarPromLis", U_SO1_FILTRARPROMLIS);
                    conn.AgregarParametro("pFiltrarPromFop", U_SO1_FILTRARPROMFOP);
                    conn.AgregarParametro("pAcumProMorm", U_SO1_ACUMPROMORM);
                    conn.AgregarParametro("pAcumPromoPunto", U_SO1_ACUMPROMOPUNTO);
                    conn.AgregarParametro("pAcumPromoOtras", U_SO1_ACUMPROMOOTRAS);
                    conn.AgregarParametro("pActLimpiezaVen", U_SO1_ACTLIMPIEZAVEN);
                    
                    double LimitePiezaVen = 0;
                    double LimitePiezaPro = 0;
                    double LimiteVentaPro = 0;
                    LimitePiezaVen = string.IsNullOrEmpty(U_SO1_LIMITEPIEZAVEN) ? 0 : Convert.ToDouble(U_SO1_LIMITEPIEZAVEN);
                    LimitePiezaPro = string.IsNullOrEmpty(U_SO1_LIMITEPIEZAPRO) ? 0 : Convert.ToDouble(U_SO1_LIMITEPIEZAPRO);
                    LimiteVentaPro = string.IsNullOrEmpty(U_SO1_LIMITEVENTAPRO) ? 0 : Convert.ToDouble(U_SO1_LIMITEVENTAPRO);
                    
                    //conn.AgregarParametro("pLimitePiezaVen", Convert.ToDouble(U_SO1_LIMITEPIEZAVEN));
                    conn.AgregarParametro("pLimitePiezaVen", LimitePiezaVen);
                    // conn.AgregarParametro("pActLimpiezaPro", Convert.ToDouble(U_SO1_LIMITEPIEZAPRO));
                    conn.AgregarParametro("pActLimpiezaPro",U_SO1_ACTLIMPIEZAPRO);
                   
                   // conn.AgregarParametro("pLimitePiezaPro", Convert.ToDouble(U_SO1_LIMITEPIEZAPRO));
                    conn.AgregarParametro("pLimitePiezaPro",LimitePiezaPro);
                   
                    conn.AgregarParametro("pComportaEscaut", U_SO1_COMPORTAESCAUT);
                    conn.AgregarParametro("pActLimVentaPro", U_SO1_ACTLIMVENTAPRO);
                    //conn.AgregarParametro("pLimiteVentaPro", Convert.ToDouble(U_SO1_LIMITEVENTAPRO));
                    conn.AgregarParametro("pLimiteVentaPro", LimiteVentaPro);
                    conn.AgregarParametro("pActVentaCredit", U_SO1_ACTVENTACREDIT);
                    conn.AgregarParametro("pFiltrarArtCon", U_SO1_FILTRARARTCON);
                    conn.AgregarParametro("pFiltrarArtConv", U_SO1_FILTRARARTCONV);
                //}
                //else {
                //    //Fechas
                //    conn.AgregarParametro("pFechaDesde", string.IsNullOrEmpty(U_SO1_FECHADESDE) ? 0 : 0);
                //    conn.AgregarParametro("pFechaHasta", string.IsNullOrEmpty(U_SO1_FECHAHASTA) ? 0 : 0);
                //    //Diario
                //    conn.AgregarParametro("pDiario", U_SO1_DIARIO);
                //    conn.AgregarParametro("pDiaCompleto", U_SO1_DIACOMPLETO);
                //    conn.AgregarParametro("pDiaHoraIni", string.IsNullOrEmpty(U_SO1_DIAHORAINI) ? 0 : horaIni);
                //    conn.AgregarParametro("pDiaHoraFin", string.IsNullOrEmpty(U_SO1_DIAHORAFIN) ? 0 : horaFin);
                //    //Lunes
                //    conn.AgregarParametro("pLunes", U_SO1_LUNES);
                //    conn.AgregarParametro("pLunesCompleto", U_SO1_LUNCOMPLETO);
                //    conn.AgregarParametro("pLunHoraIni", string.IsNullOrEmpty(U_SO1_LUNHORAINI) ? 0 : horaLunIni);
                //    conn.AgregarParametro("pLunHoraFin", string.IsNullOrEmpty(U_SO1_LUNHORAFIN) ? 0 : horaLunFin);
                //    //Martes
                //    conn.AgregarParametro("pMartes", U_SO1_MARTES);
                //    conn.AgregarParametro("pMartesCompleto", U_SO1_MARCOMPLETO);
                //    conn.AgregarParametro("pMarHoraIni", string.IsNullOrEmpty(U_SO1_MARHORAINI) ? 0 : horaMarIni);
                //    conn.AgregarParametro("pMarHoraFin", string.IsNullOrEmpty(U_SO1_MARHORAFIN) ? 0 : horaMarFin);
                //    //Miércoles
                //    conn.AgregarParametro("pMiercoles", U_SO1_MIERCOLES);
                //    conn.AgregarParametro("pMieCompleto", U_SO1_MIECOMPLETO);
                //    conn.AgregarParametro("pMieHoraIni", string.IsNullOrEmpty(U_SO1_MIEHORAINI) ? 0 : horaMieIni);
                //    conn.AgregarParametro("pMieHoraFin", string.IsNullOrEmpty(U_SO1_MIEHORAFIN) ? 0 : horaMieFin);
                //    //Jueves
                //    conn.AgregarParametro("pJueves", U_SO1_JUEVES);
                //    conn.AgregarParametro("pJueCompleto", U_SO1_JUECOMPLETO);
                //    conn.AgregarParametro("pJueHoraIni", string.IsNullOrEmpty(U_SO1_JUEHORAINI) ? 0 : horaJueIni);
                //    conn.AgregarParametro("pJueHoraFin", string.IsNullOrEmpty(U_SO1_JUEHORAFIN) ? 0 : horaJueFin);
                //    //Viernes
                //    conn.AgregarParametro("pViernes", U_SO1_VIERNES);
                //    conn.AgregarParametro("pVieCompleto", U_SO1_VIECOMPLETO);
                //    conn.AgregarParametro("pVieHoraIni", string.IsNullOrEmpty(U_SO1_VIEHORAINI) ? 0 : horaVieIni);
                //    conn.AgregarParametro("pVieHoraFin", string.IsNullOrEmpty(U_SO1_VIEHORAFIN) ? 0 : horaVieFin);
                //    //Sábado
                //    conn.AgregarParametro("pSabado", U_SO1_SABADO);
                //    conn.AgregarParametro("pSabCompleto", U_SO1_SABCOMPLETO);
                //    conn.AgregarParametro("pSabHoraIni", string.IsNullOrEmpty(U_SO1_SABHORAINI) ? 0 : horaSabIni);
                //    conn.AgregarParametro("pSabHoraFin", string.IsNullOrEmpty(U_SO1_SABHORAFIN) ? 0 : horaSabFin);
                //    //Domingo
                //    conn.AgregarParametro("pDomingo", U_SO1_DOMINGO);
                //    conn.AgregarParametro("pDomCompleto", U_SO1_DOMCOMPLETO);
                //    conn.AgregarParametro("pDomHoraIni", string.IsNullOrEmpty(U_SO1_DOMHORAINI) ? 0 : horaDomIni);
                //    conn.AgregarParametro("pDomHoraFin", string.IsNullOrEmpty(U_SO1_DOMHORAFIN) ? 0 : horaDomFin);
                //    //Checks de Articulo
                //    conn.AgregarParametro("pArticuloEnlace", U_SO1_ARTICULOENLACE);
                //    conn.AgregarParametro("pFiltrarArtArt", U_SO1_FILTRARARTART);
                //    conn.AgregarParametro("pFiltrarArtGru", U_SO1_FILTRARARTGRU);
                //    conn.AgregarParametro("pFiltrarArtPro", U_SO1_FILTRARARTPRO);
                //    conn.AgregarParametro("pFiltrarArtProe", U_SO1_FILTRARARTPROE);
                //    conn.AgregarParametro("pFiltrarArtProc", U_SO1_FILTRARARTPROC);
                //    conn.AgregarParametro("pFiltrarArtCoe", U_SO1_FILTRARARTCOE);
                //    conn.AgregarParametro("pFiltrarArtCoec", U_SO1_FILTRARARTCOEC);
                //    conn.AgregarParametro("pFiltrarArtCoev", U_SO1_FILTRARARTCOEV);
                //    conn.AgregarParametro("pFiltrarArtProv", U_SO1_FILTRARARTPROV);
                //    conn.AgregarParametro("pFiltrarArtFab", U_SO1_FILTRARARTFAB);
                //    conn.AgregarParametro("pFiltrarArtExc", U_SO1_FILTRARARTEXC);
                //    conn.AgregarParametro("pFiltrarArtUne", U_SO1_FILTRARARTUNE);
                //    //Checks de Promocion
                //    conn.AgregarParametro("pFiltrarPromSuc", U_SO1_FILTRARPROMSUC);
                //    conn.AgregarParametro("pFiltrarPromAli", U_SO1_FILTRARPROMALI);
                //    conn.AgregarParametro("pFiltrarPromCli", U_SO1_FILTRARPROMCLI);
                //    //Checks de Cliente
                //    conn.AgregarParametro("pFiltrarCliCli", U_SO1_FILTRARCLICLI);
                //    conn.AgregarParametro("pFiltrarCliEnls", U_SO1_FILTRARCLIENLS);
                //    conn.AgregarParametro("pFiltrarCliGru", U_SO1_FILTRARCLIGRU);
                //    conn.AgregarParametro("pFiltrarCliPro", U_SO1_FILTRARCLIPRO);
                //    conn.AgregarParametro("pFiltrarCliProe", U_SO1_FILTRARCLIPROE);
                //    conn.AgregarParametro("pFiltrarCliProc", U_SO1_FILTRARCLIPROC);
                //    conn.AgregarParametro("pFiltrarCliCoe", U_SO1_FILTRARCLICOE);
                //    conn.AgregarParametro("pFiltrarCliCoec", U_SO1_FILTRARCLICOEC);
                //    conn.AgregarParametro("pFiltrarCliCoev", U_SO1_FILTRARCLICOEV);
                //    //Checks de Promocion
                //    conn.AgregarParametro("pFiltrarPromMem", U_SO1_FILTRARPROMMEM);
                //    conn.AgregarParametro("pFiltrarPromLis", U_SO1_FILTRARPROMLIS);
                //    conn.AgregarParametro("pFiltrarPromFop", U_SO1_FILTRARPROMFOP);
                //    conn.AgregarParametro("pAcumProMorm", U_SO1_ACUMPROMORM);
                //    conn.AgregarParametro("pAcumPromoPunto", U_SO1_ACUMPROMOPUNTO);
                //    conn.AgregarParametro("pAcumPromoOtras", U_SO1_ACUMPROMOOTRAS);
                //    conn.AgregarParametro("pActLimpiezaVen", U_SO1_ACTLIMPIEZAVEN);
                //    conn.AgregarParametro("pLimitePiezaVen", U_SO1_LIMITEPIEZAVEN);
                //    conn.AgregarParametro("pActLimpiezaPro", U_SO1_ACTLIMPIEZAPRO);
                //    conn.AgregarParametro("pLimitePiezaPro", U_SO1_LIMITEPIEZAPRO);
                //    conn.AgregarParametro("pComportaEscaut", U_SO1_COMPORTAESCAUT);
                //    conn.AgregarParametro("pActLimVentaPro", U_SO1_ACTLIMPIEZAPRO);
                //    conn.AgregarParametro("pLimiteVentaPro", U_SO1_LIMITEVENTAPRO);
                //    conn.AgregarParametro("pActVentaCredit", U_SO1_ACTVENTACREDIT);
                //    conn.AgregarParametro("pFiltrarArtCon", U_SO1_FILTRARARTCON);
                //    conn.AgregarParametro("pFiltrarArtConv", U_SO1_FILTRARARTCONV);
                //}
                
                conn.AgregarParametro("pCodigo", Codigo);
                conn.EjecutarComando();

                Log.GuardarError("Actualización exitosa", null);

                return true;
            }
            catch (Exception oError)
            {
                //Log.GuardarError("Descripción de error: ", oError);
                throw new Exception("ErrorModificarPromo", oError);
            }
        }

        #region Métodos para actualizar o crear registros de tablas hijas

        public void ActualizarSucursales(Conexion conn, ReportesWeb.Utilidades.CodigoControl codigosControl)
        {
            #region Bitácora

            // AUTOR:               Juan Sol
            // FECHA:               18.07.2019
            // COMENTARIOS:         Se sustituye el DIAPI a SQL

            // AUTOR:               Juan Sol
            // FECHA:               19.11.2019
            // COMENTARIOS:         Se cambia la forma en el que se obtiene el nuevo código de control

            #endregion

            Log.GuardarError("Asignamos la tabla de sucursales", null);

            if (Sucursales != null)
            {
                try
                {
                    foreach (MPromoSucursal sucursal in Sucursales)
                    {
                        //MPostTransaccion transaccion = new MPostTransaccion();
                        //Checamos si el registro tiene Code
                        if (string.IsNullOrEmpty(sucursal.Code))
                        {
                            //Si no tiene codigo checamos si tiene 'Y' o 'N'
                            if (sucursal.U_SO1_ACTIVO == "Y")
                            {
                                //Como tiene 'Y' lo tenemos que crear, aquí registramos una nueva sucursal
                                conn.LimpiarParametros();
                                conn.CargarConsulta(ConsultasPromocion.RegistrarSucursal);
                                var nuevoCodigo = String.Empty;
                                nuevoCodigo = codigosControl.Proximo(TablaCodigoControl.PROMOSUCURSAL);
                                conn.AgregarParametro("pCode", nuevoCodigo);
                                conn.AgregarParametro("pName", nuevoCodigo);
                                conn.AgregarParametro("pPromo", U_SO1_PROMOCION);
                                conn.AgregarParametro("pPromoCadena", Codigo);
                                conn.AgregarParametro("pSucursal", sucursal.U_SO1_SUCURSAL);
                                conn.AgregarParametro("pActivo", sucursal.U_SO1_ACTIVO);
                                conn.EjecutarComando();
                            }
                        }
                        else //Si tiene codigo actualizamos el campo correspondiente 
                        {
                            conn.LimpiarParametros();
                            conn.CargarConsulta(ConsultasPromocion.ModificarSucursal);
                            conn.AgregarParametro("pActivo", sucursal.U_SO1_ACTIVO);
                            conn.AgregarParametro("pCode", sucursal.Code);
                            conn.EjecutarComando();
                        }
                    }
                    Log.GuardarError("Actualización exitosa", null);
                }

                catch (Exception oError)
                {
                    Log.GuardarError("Descripción de error: ", oError);
                    throw new Exception("prom3"); // Error al actualizar las sucursales asociadas a la promoción
                }
            }
            return;
        }

        public void ActualizarAlianzas(Conexion conn, ReportesWeb.Utilidades.CodigoControl codigosControl)
        {

            #region Bitácora

            // AUTOR:               Juan Sol
            // FECHA:               18.07.2019
            // COMENTARIOS:         Se sustituye el DIAPI a SQL

            // AUTOR:               Juan Sol
            // FECHA:               19.11.2019
            // COMENTARIOS:         Se cambia la forma en el que se obtiene el nuevo código de control

            #endregion

            Log.GuardarError("Asignamos la tabla de alianzas", null);

            if(Alianzas != null)
            {
                try
                {
                    foreach (MPromoAlianza alianza in Alianzas)
                    {
                        //Checamos si el registro tiene Code
                        if (string.IsNullOrEmpty(alianza.Code))
                        {
                            //Si no tiene codigo checamos si tiene 'Y' o 'N'
                            if (alianza.U_SO1_ACTIVO == "Y")
                            {
                                //Como tiene 'Y' lo tenemos que crear
                                conn.LimpiarParametros();
                                conn.CargarConsulta(ConsultasPromocion.RegistrarAlianza);
                                var nuevoCodigo = String.Empty;
                                nuevoCodigo = codigosControl.Proximo(TablaCodigoControl.PROMOALICOMER);
                                conn.AgregarParametro("pCode", nuevoCodigo);
                                conn.AgregarParametro("pName", nuevoCodigo);
                                conn.AgregarParametro("pPromo", U_SO1_PROMOCION);
                                conn.AgregarParametro("pPromoCadena", Codigo);
                                conn.AgregarParametro("pAlianza", alianza.U_SO1_ALIANZA);
                                conn.AgregarParametro("pActivo", alianza.U_SO1_ACTIVO);
                                conn.EjecutarComando();
                            }
                        }
                        else //Si tiene codigo actualizamos el campo correspondiente 
                        {
                            conn.LimpiarParametros();
                            conn.CargarConsulta(ConsultasPromocion.ModificarAlianza);
                            conn.AgregarParametro("pActivo", alianza.U_SO1_ACTIVO);
                            conn.AgregarParametro("pCode", alianza.Code);
                            conn.EjecutarComando();
                        }

                    }
                    Log.GuardarError("Actualización exitosa", null);
                }

                catch (Exception oError)
                {
                    Log.GuardarError("Descripción de error: ", oError);
                    throw new Exception("prom4"); // Error al actualizar las alianzas asociadas a la promoción
                }
            }
            return;
        }


        public void ActualizarMembresias(Conexion conn, ReportesWeb.Utilidades.CodigoControl codigosControl)
        {
            #region Bitácora

            // AUTOR:               Juan Sol
            // FECHA:               18.07.2019
            // COMENTARIOS:         Se sustituye el DIAPI a SQL

            // AUTOR:               Juan Sol
            // FECHA:               19.11.2019
            // COMENTARIOS:         Se cambia la forma en el que se obtiene el nuevo código de control

            #endregion

            Log.GuardarError("Asignamos la tabla de membresias", null);

            if (Membresias != null)
            {
                try
                {
                    foreach (MPromoMembresia membresia in Membresias)
                    {
                        //Checamos si el registro tiene Code
                        if (string.IsNullOrEmpty(membresia.Code))
                        {
                            //Si no tiene codigo checamos si tiene 'Y' o 'N'
                            if (membresia.U_SO1_ACTIVO == "Y")
                            {
                                //Como tiene 'Y' lo tenemos que crear
                                conn.LimpiarParametros();
                                conn.CargarConsulta(ConsultasPromocion.RegistrarMembresia);
                                var nuevoCodigo = String.Empty;
                                nuevoCodigo = codigosControl.Proximo(TablaCodigoControl.PROMOTIPOMEMB);
                                conn.AgregarParametro("pCode", nuevoCodigo);
                                conn.AgregarParametro("pName", nuevoCodigo);
                                conn.AgregarParametro("pPromo", U_SO1_PROMOCION);
                                conn.AgregarParametro("pPromoCadena", Codigo);
                                conn.AgregarParametro("pMembresia", membresia.U_SO1_TIPOMEMBRESIA);
                                conn.AgregarParametro("pActivo", membresia.U_SO1_ACTIVO);
                                conn.EjecutarComando();
                            }
                        }
                        else //Si tiene codigo actualizamos el campo correspondiente 
                        {
                            //Se busca el código de la promoción
                            conn.LimpiarParametros();
                            conn.CargarConsulta(ConsultasPromocion.ModificarMembresia);
                            conn.AgregarParametro("pActivo", membresia.U_SO1_ACTIVO);
                            conn.AgregarParametro("pCode", membresia.Code);
                            conn.EjecutarComando();
                        }
                    }
                    Log.GuardarError("Actualización exitosa", null);
                }

                catch (Exception oError)
                {

                    Log.GuardarError("Descripción del error: ", oError);
                    throw new Exception("prom5"); // Error al actualizar las membresías asociadas a la promoción

                }
            }
            return;
        }

        public void ActualizarPrecios(Conexion conn, ReportesWeb.Utilidades.CodigoControl codigosControl)
        {
            #region Bitácora

            // AUTOR:               Juan Sol
            // FECHA:               18.07.2019
            // COMENTARIOS:         Se sustituye el DIAPI a SQL

            // AUTOR:               Juan Sol
            // FECHA:               19.11.2019
            // COMENTARIOS:         Se cambia la forma en el que se obtiene el nuevo código de control

            #endregion

            Log.GuardarError("Asignamos la tabla Precio", null);

            if (Precios != null)
            {
                try
                {
                    foreach (MPromoPrecios precio in Precios)
                    {
                        //Checamos si el registro tiene Code
                        if (string.IsNullOrEmpty(precio.Code))
                        {
                            //Si no tiene codigo checamos si tiene 'Y' o 'N'
                            if (precio.U_SO1_ACTIVO == "Y")
                            {
                                //Como tiene 'Y' lo tenemos que crear
                                conn.LimpiarParametros();
                                conn.CargarConsulta(ConsultasPromocion.RegistrarPrecio);
                                var nuevoCodigo = String.Empty;
                                nuevoCodigo = codigosControl.Proximo(TablaCodigoControl.PROMOLISTPREC);
                                conn.AgregarParametro("pCode", nuevoCodigo);
                                conn.AgregarParametro("pName", nuevoCodigo);
                                conn.AgregarParametro("pPromo", U_SO1_PROMOCION);
                                conn.AgregarParametro("pPromoCadena", Codigo);
                                conn.AgregarParametro("pPrecio", precio.U_SO1_LISTAPRECIO);
                                conn.AgregarParametro("pActivo", precio.U_SO1_ACTIVO);
                                conn.EjecutarComando();
                            }
                        }
                        else //Si tiene codigo actualizamos el campo correspondiente 
                        {
                            conn.LimpiarParametros();
                            conn.CargarConsulta(ConsultasPromocion.ModificarPrecio);
                            conn.AgregarParametro("pActivo", precio.U_SO1_ACTIVO);
                            conn.AgregarParametro("pCode", precio.Code);
                            conn.EjecutarComando();
                        }
                    }
                    Log.GuardarError("Actualización exitosa", null);
                }

                catch (Exception oError)
                {

                    Log.GuardarError("Descripción de error: ", oError);
                    throw new Exception("prom6"); // Error al actualizar las listas de precio asociadas a la promoción

                }
            }
            return;
        }

        public void ActualizarFPago(Conexion conn, ReportesWeb.Utilidades.CodigoControl codigosControl)
        {
            #region Bitácora

            // AUTOR:               Juan Sol
            // FECHA:               18.07.2019
            // COMENTARIOS:         Se sustituye el DIAPI a SQL

            // AUTOR:               Juan Sol
            // FECHA:               19.11.2019
            // COMENTARIOS:         Se cambia la forma en el que se obtiene el nuevo código de control

            #endregion

            Log.GuardarError("Asignamos la tabla de Forma de Pago", null);

            if(FormasPago != null)
            {
                try
                {
                    foreach (MPromoFPago Fpago in FormasPago)
                    {
                        //Checamos si el registro tiene Code
                        if (string.IsNullOrEmpty(Fpago.Code))
                        {
                            //Si no tiene codigo checamos si tiene 'Y' o 'N'
                            if (Fpago.U_SO1_ACTIVO == "Y")
                            {
                                //Como tiene 'Y' lo tenemos que crear
                                conn.LimpiarParametros();
                                conn.CargarConsulta(ConsultasPromocion.RegistrarFormaPago);

                                var nuevoCodigo = String.Empty;
                                nuevoCodigo = codigosControl.Proximo(TablaCodigoControl.PROMOFORMAPAG);
                                conn.AgregarParametro("pCode", nuevoCodigo);
                                conn.AgregarParametro("pName", nuevoCodigo);
                                conn.AgregarParametro("pPromo", U_SO1_PROMOCION);
                                conn.AgregarParametro("pPromoCadena", Codigo);
                                conn.AgregarParametro("pFPago", Fpago.U_SO1_FORMAPAGO);
                                conn.AgregarParametro("pActivo", Fpago.U_SO1_ACTIVO);
                                conn.AgregarParametro("pMinimo", Convert.ToDouble(Fpago.U_SO1_MINIMO));
                                conn.AgregarParametro("pTipo", (Fpago.U_SO1_TIPO == null ? "P" : Fpago.U_SO1_TIPO));
                                conn.EjecutarComando();

                            }
                        }
                        else //Si tiene codigo actualizamos el campo correspondiente 
                        {
                            conn.LimpiarParametros();
                            conn.CargarConsulta(ConsultasPromocion.ModificarFormaPago);
                            conn.AgregarParametro("pActivo", Fpago.U_SO1_ACTIVO);
                            conn.AgregarParametro("pMinimo", Convert.ToDouble(Fpago.U_SO1_MINIMO));
                            conn.AgregarParametro("pTipo", (Fpago.U_SO1_TIPO == null ? "P" : Fpago.U_SO1_TIPO));
                            conn.AgregarParametro("pCode", Fpago.Code);
                            conn.EjecutarComando();
                        }
                    }
                    Log.GuardarError("Actualización exitosa", null);
                }

                catch (Exception oError)
                {
                    Log.GuardarError("Descripción de error: ", oError);
                    throw new Exception("prom7"); // Error al actualizar las formas de pago asociadas a la promoción
                }
            }
            
            return;
        }

        #endregion

        #region Métodos ActualizarTablaIdentificadores

        public void ActualizarIdentificadores(Conexion conn)
        {

            Log.GuardarError("Asignamos la tabla", null);

            if(!string.IsNullOrEmpty(PromocionKitVenta.Code))
            {
                foreach (var PromocionKitV in PromocionKitVenta.Identificador)
                {
                    conn.LimpiarParametros();
                    conn.CargarConsulta(ConsultasPromocion.EliminarIdentificadores);
                    conn.AgregarLiteral("TABLA", "@SO1_01PROMOAMBITO");
                    conn.AgregarParametro("pIdentificador", PromocionKitV.U_SO1_CODIGOAMBITO);
                }
            }

            Log.GuardarError("Eliminación exitosa", null);
            return;
        
        }
        
        #endregion

        #region Métodos para actualizar las tablas ligadas al articulo

        public void ActualizarTablaArticulos(Conexion conn, ReportesWeb.Utilidades.CodigoControl codigosControl)
        {

            #region Bitácora

            // AUTOR:               Juan Sol
            // FECHA:               17.07.2019
            // COMENTARIOS:         Se sustituye el DIAPI a SQL

            // AUTOR:               Juan Sol
            // FECHA:               19.11.2019
            // COMENTARIOS:         Se cambia la forma en el que se obtiene el nuevo código de control

            #endregion

            try
            {
                if(U_SO1_TIPO != "KV")
                {
                    DataTable dtArticulos = null;
                    conn.LimpiarParametros();
                    conn.CargarConsulta(ConsultasPromocion.ConsultarCodigosArticulos);
                    conn.AgregarParametro("pPromocion", U_SO1_PROMOCION);
                    conn.AgregarParametro("pTipo", "G");
                    dtArticulos = conn.EjecutarTabla();

                    Log.GuardarError("Asignamos la tabla de Articulos", null);

                    foreach (DataRow fila in dtArticulos.Rows)
                    {
                        //Si no se encuentra en el diccionario significa que fué eliminado
                        if (!_articulos.ContainsKey(fila.Valor<string>("Articulo")))
                        {
                            //Se busca el código del registro
                            conn.LimpiarParametros();
                            conn.CargarConsulta(ConsultasPromocion.VerificarRegistro);
                            conn.AgregarLiteral("TABLA", "@SO1_01PROMOARTICULO");
                            conn.AgregarParametro("pCode", fila.Valor<string>("Codigo"));
                            conn.EjecutarLector();
                            if (!conn.LeerFila())
                            {
                                Log.GuardarError("El articulo: " + fila.Valor<string>("Articulo") + " no se encuentra en la base de datos", null);
                                throw new Exception("prom18"); // Error al eliminar un articulo ligado a la promoción
                            }
                            conn.CerrarLector();
                            conn.LimpiarParametros();
                            conn.CargarConsulta(ConsultasPromocion.EliminarRegistro);
                            conn.AgregarLiteral("TABLA", "@SO1_01PROMOARTICULO");
                            conn.AgregarParametro("pCode", fila.Valor<string>("Codigo"));
                            conn.EjecutarComando();
                        }
                        else
                        {
                            _articulos.Remove(fila.Valor<string>("Articulo"));
                        }
                    }

                    //En el diccionario sólo quedan los nuevos valores
                    foreach (KeyValuePair<string, MPromoArticulo> articulo in _articulos)
                    {
                        conn.LimpiarParametros();
                        string nuevoCodigo = String.Empty;
                        nuevoCodigo = codigosControl.Proximo(TablaCodigoControl.PROMOARTICULO);
                        Log.GuardarError("El codigo: " + nuevoCodigo, null);
                        if (string.IsNullOrEmpty(articulo.Value.Code))
                        {
                            conn.CargarConsulta(ConsultasPromocion.RegistrarArticulo);
                            conn.AgregarParametro("pCode", nuevoCodigo);
                            conn.AgregarParametro("pName", nuevoCodigo);
                            conn.AgregarParametro("pPromo", U_SO1_PROMOCION);
                            conn.AgregarParametro("pPromoCadena", Codigo);
                            conn.AgregarParametro("pTipo", "G");
                            conn.AgregarParametro("pArticulo", articulo.Value.U_SO1_ARTICULO);
                            conn.AgregarParametro("pImporte", articulo.Value.U_SO1_IMPORTE);
                            conn.AgregarParametro("pCodigoAmbito", articulo.Value.U_SO1_CODIGOAMBITO);
                        }
                        else
                        {
                            conn.CargarConsulta(ConsultasPromocion.ActualizarArticulo);
                            conn.AgregarParametro("pImporte", articulo.Value.U_SO1_IMPORTE);
                            conn.AgregarParametro("pCodigo", articulo.Value.Code);
                        }
                        conn.EjecutarComando();
                    }
                }                
            }

            catch (Exception oError)
            {
                Log.GuardarError("Descripción de error: ", oError);
                throw new Exception("prom8"); // Error al actualizar los articulos asociadas a la promoción
            }
        }

        public void ActualizarExcepciones(Conexion conn, ReportesWeb.Utilidades.CodigoControl codigosControl)
        {
            #region Bitácora

            // AUTOR:               Juan Sol
            // FECHA:               17.07.2019
            // COMENTARIOS:         Se sustituye el DIAPI a SQL

            // AUTOR:               Juan Sol
            // FECHA:               19.11.2019
            // COMENTARIOS:         Se cambia la forma en el que se obtiene el nuevo código de control

            #endregion

            try
            {
                if (U_SO1_TIPO != "KV")
                {

                    DataTable dtArticulos = null;
                    conn.LimpiarParametros();
                    conn.CargarConsulta(ConsultasPromocion.ConsultarCodigosArticulos);
                    conn.AgregarParametro("pPromocion", U_SO1_PROMOCION);
                    conn.AgregarParametro("pTipo", "E");
                    dtArticulos = conn.EjecutarTabla();

                    foreach (DataRow fila in dtArticulos.Rows)
                    {
                        //Si no se encuentra en el diccionario significa que fué eliminado
                        if (!_excepciones.ContainsKey(fila.Valor<string>("Articulo")))
                        {
                            //Se busca el código del registro
                            conn.LimpiarParametros();
                            conn.CargarConsulta(ConsultasPromocion.VerificarRegistro);
                            conn.AgregarLiteral("TABLA", "@SO1_01PROMOARTICULO");
                            conn.AgregarParametro("pCode", fila.Valor<string>("Codigo"));
                            conn.EjecutarLector();
                            if (!conn.LeerFila())
                            {
                                Log.GuardarError("El articulo: " + fila.Valor<string>("Articulo") + " no se encuentra en la base de datos", null);
                                throw new Exception("prom19"); // Error al eliminar una excepción ligada a la promoción
                            }
                            conn.CerrarLector();

                            conn.LimpiarParametros();
                            conn.CargarConsulta(ConsultasPromocion.EliminarRegistro);
                            conn.AgregarLiteral("TABLA", "@SO1_01PROMOARTICULO");
                            conn.AgregarParametro("pCode", fila.Valor<string>("Codigo"));
                            conn.EjecutarComando();
                        }
                        else
                        {
                            _excepciones.Remove(fila.Valor<string>("Articulo"));
                        }
                    }

                    //En el diccionario sólo quedan los nuevos valores
                    foreach (KeyValuePair<string, MPromoArticulo> articulo in _excepciones)
                    {
                        conn.LimpiarParametros();
                        conn.CargarConsulta(ConsultasPromocion.RegistrarArticulo);
                        string nuevoCodigo = String.Empty;
                        nuevoCodigo = codigosControl.Proximo(TablaCodigoControl.PROMOARTICULO);
                        conn.AgregarParametro("pCode", nuevoCodigo);
                        conn.AgregarParametro("pName", nuevoCodigo);
                        conn.AgregarParametro("pPromo", U_SO1_PROMOCION);
                        conn.AgregarParametro("pPromoCadena", Codigo);
                        conn.AgregarParametro("pTipo", "E");
                        conn.AgregarParametro("pArticulo", articulo.Value.U_SO1_ARTICULO);
                        conn.AgregarParametro("pImporte", articulo.Value.U_SO1_IMPORTE);
                        conn.AgregarParametro("pCodigoAmbito", articulo.Value.U_SO1_CODIGOAMBITO);
                        conn.EjecutarComando();
                    }
                    Log.GuardarError("Actualización exitosa", null);
                }
                
            }

            catch (Exception oError)
            {

                Log.GuardarError("Descripción de error: ", oError);
                throw new Exception("prom9"); // Error al actualizar las excepciones asociadas a la promoción
            }
        }

        public void ActualizarProveedores(Conexion conn, ReportesWeb.Utilidades.CodigoControl codigosControl)
        {
            #region Bitácora

            // AUTOR:               Juan Sol
            // FECHA:               17.07.2019
            // COMENTARIOS:         Se sustituye el DIAPI a SQL

            // AUTOR:               Juan Sol
            // FECHA:               19.11.2019
            // COMENTARIOS:         Se cambia la forma en el que se obtiene el nuevo código de control

            #endregion

            try
            {

                DataTable dtProveedores = null;
                conn.LimpiarParametros();
                conn.CargarConsulta(ConsultasPromocion.ConsultarCodigosProveedor);
                conn.AgregarParametro("pPromocion", U_SO1_PROMOCION);
                dtProveedores = conn.EjecutarTabla();

                Log.GuardarError("Asignamos la tabla de Proveedores", null);

                foreach (DataRow fila in dtProveedores.Rows)
                {
                    //Si no se encuentra en el diccionario significa que fué eliminado
                    if (!_articuloproveedor.ContainsKey(fila.Valor<string>("Proveedor")))
                    {
                        //Se busca el código del registro
                        conn.LimpiarParametros();
                        conn.CargarConsulta(ConsultasPromocion.VerificarRegistro);
                        conn.AgregarLiteral("TABLA", "@SO1_01PROMOARTIPROV");
                        conn.AgregarParametro("pCode", fila.Valor<string>("Codigo"));
                        conn.EjecutarLector();
                        if (!conn.LeerFila())
                        {
                            Log.GuardarError("El proveedor: " + fila.Valor<string>("Proveedor") + " no se encuentra en la base de datos", null);
                            throw new Exception("prom20"); // Error al eliminar una proveedor ligado a la promoción
                        }
                        conn.CerrarLector();
                        conn.LimpiarParametros();
                        conn.CargarConsulta(ConsultasPromocion.EliminarRegistro);
                        conn.AgregarLiteral("TABLA", "@SO1_01PROMOARTIPROV");
                        conn.AgregarParametro("pCode", fila.Valor<string>("Codigo"));
                        conn.EjecutarComando();
                    }
                    else
                    {
                        _articuloproveedor.Remove(fila.Valor<string>("Proveedor"));
                    }
                }

                //En el diccionario sólo quedan los nuevos valores
                foreach (KeyValuePair<string, MPromoArticuloProveedor> proveedor in _articuloproveedor)
                {
                    conn.LimpiarParametros();
                    conn.CargarConsulta(ConsultasPromocion.RegistrarProveedor);
                    string nuevoCodigo = String.Empty;
                    nuevoCodigo = codigosControl.Proximo(TablaCodigoControl.PROMOARTIPROV);
                    conn.AgregarParametro("pCode", nuevoCodigo);
                    conn.AgregarParametro("pName", nuevoCodigo);
                    conn.AgregarParametro("pPromo", U_SO1_PROMOCION);
                    conn.AgregarParametro("pPromoCadena", Codigo);
                    conn.AgregarParametro("pProveedor", proveedor.Value.U_SO1_PROVEEDOR);
                    conn.EjecutarComando();
                }
                Log.GuardarError("Actualización exitosa", null);
            }

            catch (Exception oError)
            {
                Log.GuardarError("Descripción de error: ", oError);
                throw new Exception("prom10"); // Error al actualizar los proveedores asociados a la promoción
            }
        }

        public void ActualizarArticuloGrupo(Conexion conn, ReportesWeb.Utilidades.CodigoControl codigosControl)
        {
            #region Bitácora

            // AUTOR:               Juan Sol
            // FECHA:               17.07.2019
            // COMENTARIOS:         Se sustituye el DIAPI a SQL

            // AUTOR:               Juan Sol
            // FECHA:               19.11.2019
            // COMENTARIOS:         Se cambia la forma en el que se obtiene el nuevo código de control

            #endregion

            try
            {

                Log.GuardarError("Asignamos la tabla de Articulo Grupo", null);

                foreach (MPromoGrupo grupo in GrupoArticulos)
                {
                    //Checamos si el registro tiene Code
                    if (string.IsNullOrEmpty(grupo.Code))
                    {
                        //Si no tiene codigo checamos si tiene 'Y' o 'N'
                        if (grupo.U_SO1_ACTIVO == "Y")
                        {
                            //Como tiene 'Y' lo tenemos que crear
                            conn.LimpiarParametros();
                            conn.CargarConsulta(ConsultasPromocion.RegistrarGrupo);
                            string nuevoCodigo = String.Empty;
                            nuevoCodigo = codigosControl.Proximo(TablaCodigoControl.PROMOARTIGRUP);
                            conn.AgregarLiteral("TABLA", "@SO1_01PROMOARTIGRUP");
                            conn.AgregarParametro("pCode", nuevoCodigo);
                            conn.AgregarParametro("pName", nuevoCodigo);
                            conn.AgregarParametro("pPromo", U_SO1_PROMOCION);
                            conn.AgregarParametro("pPromoCadena", Codigo);
                            conn.AgregarParametro("pGrupo", grupo.U_SO1_GRUPO);
                            conn.AgregarParametro("pActivo", grupo.U_SO1_ACTIVO);
                            conn.EjecutarComando();
                        }
                    }
                    else //Si tiene codigo actualizamos el campo correspondiente 
                    {
                        //Se busca el código de la promoción
                        conn.LimpiarParametros();
                        conn.CargarConsulta(ConsultasPromocion.ModificarGrupo);
                        conn.AgregarLiteral("TABLA", "@SO1_01PROMOARTIGRUP");
                        conn.AgregarParametro("pActivo", grupo.U_SO1_ACTIVO);
                        conn.AgregarParametro("pCode", grupo.Code);
                        conn.EjecutarComando();
                    }
                }
                Log.GuardarError("Actualización exitosa", null);
            }

            catch (Exception oError)
            {

                Log.GuardarError("Descripción de error: ", oError);
                throw new Exception("prom11"); // Error al actualizar los grupos de articulos asociados a la promoción

            }
            return;
        }

        public void ActualizarArticuloPropiedad(Conexion conn, ReportesWeb.Utilidades.CodigoControl codigosControl)
        {

            #region Bitácora

            // AUTOR:               Juan Sol
            // FECHA:               17.07.2019
            // COMENTARIOS:         Se sustituye el DIAPI a SQL

            // AUTOR:               Juan Sol
            // FECHA:               19.11.2019
            // COMENTARIOS:         Se cambia la forma en el que se obtiene el nuevo código de control

            #endregion

            try
            {
                Log.GuardarError("Asignamos la tabla de Propiedad Articulo", null);

                foreach (MPromoPropiedad propiedad in PropiedadesArticulos)
                {
                    //Checamos si el registro tiene Code
                    if (string.IsNullOrEmpty(propiedad.Code))
                    {
                        //Si no tiene codigo checamos si tiene 'Y' o 'N'
                        if (propiedad.U_SO1_ACTIVO == "Y")
                        {
                            //Como tiene 'Y' lo tenemos que crear
                            conn.LimpiarParametros();
                            conn.CargarConsulta(ConsultasPromocion.RegistrarPropiedad);
                            string nuevoCodigo = String.Empty;
                            nuevoCodigo = codigosControl.Proximo(TablaCodigoControl.PROMOARTIPROP);
                            conn.AgregarLiteral("TABLA", "@SO1_01PROMOARTIPROP");
                            conn.AgregarParametro("pCode", nuevoCodigo);
                            conn.AgregarParametro("pName", nuevoCodigo);
                            conn.AgregarParametro("pPromo", U_SO1_PROMOCION);
                            conn.AgregarParametro("pPromoCadena", Codigo);
                            conn.AgregarParametro("pPropiedad", propiedad.U_SO1_PROPIEDAD);
                            conn.AgregarParametro("pActivo", propiedad.U_SO1_ACTIVO);
                            conn.EjecutarComando();
                        }
                    }
                    else //Si tiene codigo actualizamos el campo correspondiente 
                    {
                        //Se busca el código de la promoción
                        conn.LimpiarParametros();
                        conn.CargarConsulta(ConsultasPromocion.ModificarPropiedad);
                        conn.AgregarLiteral("TABLA", "@SO1_01PROMOARTIPROP");
                        conn.AgregarParametro("pActivo", propiedad.U_SO1_ACTIVO);
                        conn.AgregarParametro("pCode", propiedad.Code);
                        conn.EjecutarComando();
                    }
                }
                Log.GuardarError("Actualización exitosa", null);
            }

            catch (Exception oError)
            {

                Log.GuardarError("Descripción de error: ", oError);
                throw new Exception("prom12"); // Error al actualizar las propiedades de los articulos asociados a la promoción

            }
            return;
        }

        public void ActualizarArticuloFabricante(Conexion conn, ReportesWeb.Utilidades.CodigoControl codigosControl)
        {
            #region Bitácora

            // AUTOR:               Juan Sol
            // FECHA:               17.07.2019
            // COMENTARIOS:         Se sustituye el DIAPI a SQL

            // AUTOR:               Juan Sol
            // FECHA:               19.11.2019
            // COMENTARIOS:         Se cambia la forma en el que se obtiene el nuevo código de control

            #endregion

            try
            {

                Log.GuardarError("Asignamos la tabla de Articulo fabricante", null);

                foreach (MPromoArticuloFabricante fabricante in ArticuloFabricante)
                {
                    //Checamos si el registro tiene Code
                    if (string.IsNullOrEmpty(fabricante.Code))
                    {
                        //Si no tiene codigo checamos si tiene 'Y' o 'N'
                        if (fabricante.U_SO1_ACTIVO == "Y")
                        {
                            //Como tiene 'Y' lo tenemos que crear
                            conn.LimpiarParametros();
                            conn.CargarConsulta(ConsultasPromocion.RegistrarFabricante);
                            string nuevoCodigo = String.Empty;
                            nuevoCodigo = codigosControl.Proximo(TablaCodigoControl.PROMOARTIFABR);
                            conn.AgregarParametro("pCode", nuevoCodigo);
                            conn.AgregarParametro("pName", nuevoCodigo);
                            conn.AgregarParametro("pPromo", U_SO1_PROMOCION);
                            conn.AgregarParametro("pPromoCadena", Codigo);
                            conn.AgregarParametro("pFabricante", fabricante.U_SO1_FABRICANTE);
                            conn.AgregarParametro("pActivo", fabricante.U_SO1_ACTIVO);
                            conn.EjecutarComando();
                        }
                    }
                    else //Si tiene codigo actualizamos el campo correspondiente 
                    {
                        conn.LimpiarParametros();
                        conn.CargarConsulta(ConsultasPromocion.ModificarFabricante);
                        conn.AgregarParametro("pActivo", fabricante.U_SO1_ACTIVO);
                        conn.AgregarParametro("pCode", fabricante.Code);
                        conn.EjecutarComando();
                    }
                }
                Log.GuardarError("Actualización exitosa", null);
            }

            catch (Exception oError)
            {
                Log.GuardarError("Descripción de error: ", oError);
                throw new Exception("prom13"); // Error al actualizar los fabricantes de los articulos asociados a la promoción
            }
            return;
        }

        public void ActualizarArticuloUnidadM(Conexion conn, ReportesWeb.Utilidades.CodigoControl codigosControl)
        {

            #region Bitácora

            // AUTOR:               Juan Sol
            // FECHA:               17.07.2019
            // COMENTARIOS:         Se sustituye el DIAPI a SQL

            // AUTOR:               Juan Sol
            // FECHA:               19.11.2019
            // COMENTARIOS:         Se cambia la forma en el que se obtiene el nuevo código de control

            #endregion

            try
            {

                Log.GuardarError("Asignamos la tabla de articulo unidad de medida", null);
                foreach (MPromoArticuloMedida medida in MedidasArticulos)
                {
                    //Checamos si el registro tiene Code
                    if (string.IsNullOrEmpty(medida.Code))
                    {
                        //Si no tiene codigo checamos si tiene 'Y' o 'N'
                        if (medida.U_SO1_ACTIVO == "Y")
                        {
                            //Como tiene 'Y' lo tenemos que crear
                            conn.LimpiarParametros();
                            conn.CargarConsulta(ConsultasPromocion.RegistrarUnidadMedida);
                            string nuevoCodigo = String.Empty;
                            nuevoCodigo = codigosControl.Proximo(TablaCodigoControl.PROMOARTIUNIE);
                            conn.AgregarParametro("pCode", nuevoCodigo);
                            conn.AgregarParametro("pName", nuevoCodigo);
                            conn.AgregarParametro("pPromo", U_SO1_PROMOCION);
                            conn.AgregarParametro("pPromoCadena", Codigo);
                            conn.AgregarParametro("pGrupo", medida.U_SO1_GRUPO);
                            conn.AgregarParametro("pUnidad", medida.U_SO1_UNIDAD);
                            conn.AgregarParametro("pActivo", medida.U_SO1_ACTIVO);
                            conn.EjecutarComando();
                        }
                    }
                    else //Si tiene codigo actualizamos el campo correspondiente 
                    {
                        //Se busca el código de la promoción
                        conn.LimpiarParametros();
                        conn.CargarConsulta(ConsultasPromocion.ModificarUnidadMedida);
                        conn.AgregarParametro("pGrupo", medida.U_SO1_GRUPO);
                        conn.AgregarParametro("pUnidad", medida.U_SO1_UNIDAD);
                        conn.AgregarParametro("pActivo", medida.U_SO1_ACTIVO);
                        conn.AgregarParametro("pCode", medida.Code);
                        conn.EjecutarComando();
                    }
                }
                Log.GuardarError("Actualización exitosa", null);
            }

            catch (Exception oError)
            {

                Log.GuardarError("Descripción de error: ", oError);
                throw new Exception("prom14"); // Error al actualizar las unidades de medida de los articulos asociados a la promoción

            }
            return;
        }

        #endregion

        #region Métodos para actualizar las tablas ligadas al cliente

        public void ActualizarClientes(Conexion conn, ReportesWeb.Utilidades.CodigoControl codigosControl)
        {
            #region Bitácora

            // AUTOR:               Juan Sol
            // FECHA:               18.07.2019
            // COMENTARIOS:         Se sustituye el DIAPI a SQL

            // AUTOR:               Juan Sol
            // FECHA:               19.11.2019
            // COMENTARIOS:         Se cambia la forma en el que se obtiene el nuevo código de control

            #endregion

            try
            {

                DataTable dtClientes = null;
                conn.LimpiarParametros();
                conn.CargarConsulta(ConsultasPromocion.ConsultarCodigoClientes);
                conn.AgregarParametro("pPromocion", U_SO1_PROMOCION);
                dtClientes = conn.EjecutarTabla();

                Log.GuardarError("Asignamos la tabla de Clientes", null);

                foreach (DataRow fila in dtClientes.Rows)
                {
                    //Si no se encuentra en el diccionario significa que fué eliminado
                    if (!_articulos.ContainsKey(fila.Valor<string>("CodigoCliente")))
                    {

                        conn.LimpiarParametros();
                        conn.CargarConsulta(ConsultasPromocion.VerificarRegistro);
                        conn.AgregarLiteral("TABLA", "@SO1_01PROMOCLIENTE");
                        conn.AgregarParametro("pCode", fila.Valor<string>("Codigo"));
                        conn.EjecutarLector();
                        if (!conn.LeerFila())
                        {
                            Log.GuardarError("El cliente: " + fila.Valor<string>("CodigoCliente") + " no se encuentra en la base de datos", null);
                            throw new Exception("prom21"); // Error al eliminar una cliente ligado a la promoción
                        }
                        conn.CerrarLector();
                        conn.LimpiarParametros();
                        conn.CargarConsulta(ConsultasPromocion.EliminarRegistro);
                        conn.AgregarLiteral("TABLA", "@SO1_01PROMOCLIENTE");
                        conn.AgregarParametro("pCode", fila.Valor<string>("Codigo"));
                        conn.EjecutarComando();
                    }
                    else
                    {
                        _clientes.Remove(fila.Valor<string>("CodigoCliente"));
                    }
                }

                //En el diccionario sólo quedan los nuevos valores
                foreach (KeyValuePair<string, MPromoCliente> cliente in _clientes)
                {
                    conn.LimpiarParametros();
                    conn.CargarConsulta(ConsultasPromocion.RegistrarCliente);
                    string nuevoCodigo = String.Empty;
                    nuevoCodigo = codigosControl.Proximo(TablaCodigoControl.PROMOCLIENTE);
                    conn.AgregarParametro("pCode", nuevoCodigo);
                    conn.AgregarParametro("pName", nuevoCodigo);
                    conn.AgregarParametro("pPromo", U_SO1_PROMOCION);
                    conn.AgregarParametro("pPromoCadena", Codigo);
                    conn.AgregarParametro("pCliente", cliente.Value.U_SO1_CLIENTE);
                    conn.EjecutarComando();
                }
            }
            catch (Exception oError)
            {
                Log.GuardarError("Descripción de error: ", oError);
                throw new Exception("prom15"); // Error al actualizar los clientes asociados a la promoción;
            }

            return;
        }

        public void ActualizarClienteGrupo(Conexion conn, ReportesWeb.Utilidades.CodigoControl codigosControl)
        {
            #region Bitácora

            // AUTOR:               Juan Sol
            // FECHA:               18.07.2019
            // COMENTARIOS:         Se sustituye el DIAPI a SQL

            // AUTOR:               Juan Sol
            // FECHA:               19.11.2019
            // COMENTARIOS:         Se cambia la forma en el que se obtiene el nuevo código de control

            #endregion

            Log.GuardarError("Asignamos la tabla de grupo cliente", null);

            try
            {
                foreach (MPromoGrupo grupo in GrupoClientes)
                {
                    //Checamos si el registro tiene Code
                    if (string.IsNullOrEmpty(grupo.Code))
                    {
                        //Si no tiene codigo checamos si tiene 'Y' o 'N'
                        if (grupo.U_SO1_ACTIVO == "Y")
                        {
                            //Como tiene 'Y' lo tenemos que crear
                            conn.LimpiarParametros();
                            conn.CargarConsulta(ConsultasPromocion.RegistrarGrupo);
                            string nuevoCodigo = String.Empty;
                            nuevoCodigo = codigosControl.Proximo(TablaCodigoControl.PROMOCLIEGRUP);
                            conn.AgregarLiteral("TABLA", "@SO1_01PROMOCLIEGRUP");
                            conn.AgregarParametro("pCode", nuevoCodigo);
                            conn.AgregarParametro("pName", nuevoCodigo);
                            conn.AgregarParametro("pPromo", U_SO1_PROMOCION);
                            conn.AgregarParametro("pPromoCadena", Codigo);
                            conn.AgregarParametro("pGrupo", grupo.U_SO1_GRUPO);
                            conn.AgregarParametro("pActivo", grupo.U_SO1_ACTIVO);
                            conn.EjecutarComando();
                        }
                    }
                    else //Si tiene codigo actualizamos el campo correspondiente 
                    {
                        //Se busca el código de la promoción
                        conn.LimpiarParametros();
                        conn.CargarConsulta(ConsultasPromocion.ModificarGrupo);
                        conn.AgregarLiteral("TABLA", "@SO1_01PROMOCLIEGRUP");
                        conn.AgregarParametro("pActivo", grupo.U_SO1_ACTIVO);
                        conn.AgregarParametro("pCode", grupo.Code);
                        conn.EjecutarComando();
                    }
                }
                Log.GuardarError("Actualización exitosa", null);
            }
            catch (Exception oError)
            {
                Log.GuardarError("Descripción de error: ", oError);
                throw new Exception("prom16"); // Error al actualizar los grupos de clientes asociados a la promoción;
            }
            return;
        }

        public void ActualizarClientePropiedad(Conexion conn, ReportesWeb.Utilidades.CodigoControl codigosControl)
        {
            #region Bitácora

            // AUTOR:               Juan Sol
            // FECHA:               18.07.2019
            // COMENTARIOS:         Se sustituye el DIAPI a SQL

            // AUTOR:               Juan Sol
            // FECHA:               19.11.2019
            // COMENTARIOS:         Se cambia la forma en el que se obtiene el nuevo código de control

            #endregion

            try
            {
                Log.GuardarError("Asignamos la tabla de propiedad cliente", null);

                foreach (MPromoPropiedad propiedad in PropiedadesClientes)
                {
                    //Checamos si el registro tiene Code
                    if (string.IsNullOrEmpty(propiedad.Code))
                    {
                        //Si no tiene codigo checamos si tiene 'Y' o 'N'
                        if (propiedad.U_SO1_ACTIVO == "Y")
                        {
                            //Como tiene 'Y' lo tenemos que crear
                            conn.LimpiarParametros();
                            conn.CargarConsulta(ConsultasPromocion.RegistrarPropiedad);
                            string nuevoCodigo = String.Empty;
                            nuevoCodigo = codigosControl.Proximo(TablaCodigoControl.PROMOCLIEPROP);
                            conn.AgregarLiteral("TABLA", "@SO1_01PROMOCLIEPROP");
                            conn.AgregarParametro("pCode", nuevoCodigo);
                            conn.AgregarParametro("pName", nuevoCodigo);
                            conn.AgregarParametro("pPromo", U_SO1_PROMOCION);
                            conn.AgregarParametro("pPromoCadena", Codigo);
                            conn.AgregarParametro("pPropiedad", propiedad.U_SO1_PROPIEDAD);
                            conn.AgregarParametro("pActivo", propiedad.U_SO1_ACTIVO);
                            conn.EjecutarComando();
                        }
                    }
                    else //Si tiene codigo actualizamos el campo correspondiente 
                    {
                        conn.LimpiarParametros();
                        conn.CargarConsulta(ConsultasPromocion.ModificarPropiedad);
                        conn.AgregarLiteral("TABLA", "@SO1_01PROMOCLIEPROP");
                        conn.AgregarParametro("pActivo", propiedad.U_SO1_ACTIVO);
                        conn.AgregarParametro("pCode", propiedad.Code);
                        conn.EjecutarComando();
                    }
                }
                Log.GuardarError("Actualización exitosa", null);
            }
            catch (Exception oError)
            {
                Log.GuardarError("Descripción de error: ", oError);
                throw new Exception("prom17"); // Error al actualizar las propiedades de los clientes asociados a la promoción;
            }
            return;
        }

        #endregion

        #endregion

        #region Modificacion Promociones

        public void ModificarAxB(Conexion conn)
        {
            try
            {
                Log.GuardarError("Asignamos la tabla de promocion AxB", null);
                if (!string.IsNullOrEmpty(PromocionAxB.Code))
                {
                    //Se busca el código de la promoción
                    conn.LimpiarParametros();
                    conn.CargarConsulta(ConsultasPromocion.ModificarPromocionAxB);
                    conn.AgregarParametro("pNombre",Nombre);
                    conn.AgregarParametro("pPiezasTotales", Convert.ToDouble(PromocionAxB.U_SO1_PIEZASTOTALES));
                    conn.AgregarParametro("pPiezasPagar", Convert.ToDouble(PromocionAxB.U_SO1_PIEZASAPAGAR));
                    conn.AgregarParametro("pRegaloPrecio", string.IsNullOrEmpty(PromocionAxB.U_SO1_REGALOPRECIO1) ? "N" : PromocionAxB.U_SO1_REGALOPRECIO1);
                    conn.AgregarParametro("pMantenerDesc", string.IsNullOrEmpty(PromocionAxB.U_SO1_MANTENERDESC) ? "N" : PromocionAxB.U_SO1_MANTENERDESC);
                    conn.AgregarParametro("pCode", PromocionAxB.Code);
                    conn.EjecutarComando();
                }
                else
                {
                    conn.LimpiarParametros();
                    conn.CargarConsulta(ConsultasPromocion.RegistrarPromocionAxB);
                    conn.AgregarParametro("pCodigo", Codigo);
                    conn.AgregarParametro("pNombre", Nombre);
                    conn.AgregarParametro("pPromo", U_SO1_PROMOCION);
                    conn.AgregarParametro("pPromoCadena", Codigo);
                    conn.AgregarParametro("pPiezasTotales", Convert.ToDouble(PromocionAxB.U_SO1_PIEZASTOTALES));
                    conn.AgregarParametro("pPiezasPagar", Convert.ToDouble(PromocionAxB.U_SO1_PIEZASAPAGAR));
                    conn.AgregarParametro("pRegaloPrecio", string.IsNullOrEmpty(PromocionAxB.U_SO1_REGALOPRECIO1) ? "N" : PromocionAxB.U_SO1_REGALOPRECIO1);
                    conn.AgregarParametro("pMantenerDesc", string.IsNullOrEmpty(PromocionAxB.U_SO1_MANTENERDESC) ? "N" : PromocionAxB.U_SO1_MANTENERDESC);
                    conn.EjecutarComando();
                    Log.GuardarError("Guardado en la tabla de AxB con éxito", null);
                }
            }
            catch (Exception oError)
            {
                Log.GuardarError("Descripción de error: ", oError);
                throw new Exception("PromocionAxB");
            }
            return;
        }

        public void ModificarDescEmpleado(Conexion conn)
        {
            try
            {

                Log.GuardarError("Asignamos la tabla de promocion Desc por Empleado", null);
                //Si el código está vacio es que no existe y tenemos que crearlo
                if (!string.IsNullOrEmpty(PromocionDescEmp.Code))
                {
                    //Se busca el código de la promoción
                    conn.LimpiarParametros();
                    conn.CargarConsulta(ConsultasPromocion.ModificarPromocionDescEmp);
                    conn.AgregarParametro("pNombre", Nombre);
                    conn.AgregarParametro("pListaPrecio", PromocionDescEmp.U_SO1_LISTAPRECIO);
                    conn.AgregarParametro("pCantidadMes", PromocionDescEmp.U_SO1_CANTIDADMES.ToString());
                    conn.AgregarParametro("pPorcentajeDesc", PromocionDescEmp.U_SO1_PORCENTAJEDESC.ToString());
                    conn.AgregarParametro("pCode", PromocionDescEmp.Code);
                    conn.EjecutarComando();
                }
                else
                {
                    conn.LimpiarParametros();
                    conn.CargarConsulta(ConsultasPromocion.RegistrarPromocionDescEmp);
                    conn.AgregarParametro("pCodigo", Codigo);
                    conn.AgregarParametro("pNombre", Nombre);
                    conn.AgregarParametro("pPromo", U_SO1_PROMOCION);
                    conn.AgregarParametro("pPromoCadena", Codigo);
                    conn.AgregarParametro("pListaPrecio", PromocionDescEmp.U_SO1_LISTAPRECIO);
                    conn.AgregarParametro("pCantidadMes", PromocionDescEmp.U_SO1_CANTIDADMES.ToString());
                    conn.AgregarParametro("pPorcentajeDesc", PromocionDescEmp.U_SO1_PORCENTAJEDESC.ToString());
                    conn.EjecutarComando();
                }
            }
            catch (Exception oError)
            {
                Log.GuardarError("Descripción de error: ", oError);
                throw new Exception("PromocionDescEmp");
            }
            return;
        }

        public void ModificarDescEscala(Conexion conn, ReportesWeb.Utilidades.CodigoControl codigosControl)
        {

            #region Bitácora

            // AUTOR:               Juan Sol
            // FECHA:               19.11.2019
            // COMENTARIOS:         Se cambia la forma en el que se obtiene el nuevo código de control

            #endregion

            try
            {

                Log.GuardarError("Asignamos la tabla de promocion Desc por Empleado", null);
                //Si el código está vacio es que no existe y tenemos que crearlo
                conn.LimpiarParametros();
                if (!string.IsNullOrEmpty(PromocionDescEsc.Code))
                {
                    conn.CargarConsulta(ConsultasPromocion.ModificarPromocionDescEsc);
                    conn.AgregarParametro("pNombre", Nombre);
                    conn.AgregarParametro("pImpuesto", PromocionDescEsc.U_SO1_CONIMPUESTO);
                    conn.AgregarParametro("pTipo", PromocionDescEsc.U_SO1_TIPO.ToString());
                    conn.AgregarParametro("pArticulosTodos", PromocionDescEsc.U_SO1_ARTICULOSTODOS.ToString());
                    conn.AgregarParametro("pCode", PromocionDescEsc.Code);
                    conn.EjecutarComando();
                }
                else
                {
                    conn.CargarConsulta(ConsultasPromocion.RegistrarPromocionDescEsc);
                    conn.AgregarParametro("pCodigo", Codigo);
                    conn.AgregarParametro("pNombre", Nombre);
                    conn.AgregarParametro("pPromo", U_SO1_PROMOCION);
                    conn.AgregarParametro("pPromoCadena", Codigo);
                    conn.AgregarParametro("pImpuesto", PromocionDescEsc.U_SO1_CONIMPUESTO);
                    conn.AgregarParametro("pTipo", PromocionDescEsc.U_SO1_TIPO.ToString());
                    conn.AgregarParametro("pArticulosTodos", PromocionDescEsc.U_SO1_ARTICULOSTODOS.ToString());
                    conn.EjecutarComando();
                }


                DataTable dtDescuentos = null;
                conn.LimpiarParametros();
                conn.CargarConsulta(ConsultasPromocion.ConsultarCodigosDescuentos);
                conn.AgregarLiteral("TABLA", "@SO1_01PROMODESESCPR");
                conn.AgregarParametro("pPromocion", U_SO1_PROMOCION);
                dtDescuentos = conn.EjecutarTabla();

                Log.GuardarError("Asignamos la tabla de Monto de descuento", null);

                foreach (DataRow fila in dtDescuentos.Rows)
                {
                    //Si no se encuentra en el diccionario significa que fué eliminado
                    //if (!PromocionDescEsc._descuentos.Contains(fila.Valor<string>("Articulo")))
                    bool existe = false;

                    for (int i = 0; i < PromocionDescEsc._descuentos.Count(); i++)
                    {
                        if (PromocionDescEsc._descuentos[i].Code == fila.Valor<string>("Codigo"))
                        {
                            //Actualizamos el registro con los nuevos valores
                            existe = true;
                            conn.LimpiarParametros();
                            conn.CargarConsulta(ConsultasPromocion.ModificarPromocionDescEscPr);
                            conn.AgregarParametro("pMonto", PromocionDescEsc._descuentos[i].U_SO1_MONTO);
                            conn.AgregarParametro("pPorcentajeDesc", PromocionDescEsc._descuentos[i].U_SO1_PORCENTAJEDESC);
                            conn.AgregarParametro("pCode", PromocionDescEsc._descuentos[i].Code);
                            conn.EjecutarComando();
                            PromocionDescEsc._descuentos.Remove(PromocionDescEsc._descuentos[i]);
                        }
                    }
                    if (!existe)
                    {
                        //Eliminamos el registro, ya que fué eliminado de la nueva lista
                        conn.LimpiarParametros();
                        conn.CargarConsulta(ConsultasPromocion.EliminarRegistro);
                        conn.AgregarLiteral("TABLA", "@SO1_01PROMODESESCPR");
                        conn.AgregarParametro("pCode", fila.Valor<string>("Codigo"));
                        conn.EjecutarComando();

                    }
                }

                // Recorremos la lista para para crear los nuevos registros

                for (int i = 0; i < PromocionDescEsc._descuentos.Count(); i++)
                {
                    string nuevoCodigo = String.Empty;
                    nuevoCodigo = codigosControl.Proximo(TablaCodigoControl.PROMOARTIUNIE);
                    conn.LimpiarParametros();
                    conn.CargarConsulta(ConsultasPromocion.RegistrarPromocionDescEscPr);
                    conn.AgregarParametro("pCodigo", nuevoCodigo);
                    conn.AgregarParametro("pNombre", nuevoCodigo);
                    conn.AgregarParametro("pPromo", U_SO1_PROMOCION);
                    conn.AgregarParametro("pPromoCadena", Codigo);
                    conn.AgregarParametro("pMonto", PromocionDescEsc._descuentos[i].U_SO1_MONTO);
                    conn.AgregarParametro("pPorcentajeDesc", PromocionDescEsc._descuentos[i].U_SO1_PORCENTAJEDESC);
                    conn.EjecutarComando();
                }

            }
            catch (Exception oError)
            {
                Log.GuardarError("Descripción de error: ", oError);
                throw new Exception("PromocionDescEmp");
            }
            return;

        }

        public void ModificarDescImporte(Conexion conn)
        {
            try
            {

                Log.GuardarError("Asignamos la tabla de promocion descuento por importe", null);
                //Si el código está vacio es que no existe y tenemos que crearlo
                if (!string.IsNullOrEmpty(PromocionDescImp.Code))
                {
                    //Se busca el código de la promoción
                    conn.LimpiarParametros();
                    conn.CargarConsulta(ConsultasPromocion.ModificarPromocionDescImp);
                    conn.AgregarParametro("pNombre", Nombre);
                    conn.AgregarParametro("pComportamiento", PromocionDescImp.U_SO1_COMPORTAMIENTO);
                    conn.AgregarParametro("pMoneda", PromocionDescImp.U_SO1_MONEDA);
                    conn.AgregarParametro("pMontoMinimo", PromocionDescImp.U_SO1_MONTOMINIMO.ToString());
                    conn.AgregarParametro("pImporteDesc", PromocionDescImp.U_SO1_IMPORTEDESC.ToString());
                    conn.AgregarParametro("pCode", PromocionDescImp.Code);
                    conn.EjecutarComando();
                }
                else
                {
                    conn.LimpiarParametros();
                    conn.CargarConsulta(ConsultasPromocion.RegistrarPromocionDescImp);
                    conn.AgregarParametro("pCodigo", Codigo);
                    conn.AgregarParametro("pNombre", Nombre);
                    conn.AgregarParametro("pPromo", U_SO1_PROMOCION);
                    conn.AgregarParametro("pPromoCadena", Codigo);
                    conn.AgregarParametro("pComportamiento", PromocionDescImp.U_SO1_COMPORTAMIENTO);
                    conn.AgregarParametro("pMoneda", PromocionDescImp.U_SO1_MONEDA);
                    conn.AgregarParametro("pMontoMinimo", PromocionDescImp.U_SO1_MONTOMINIMO.ToString());
                    conn.AgregarParametro("pImporteDesc", PromocionDescImp.U_SO1_IMPORTEDESC.ToString());
                    conn.EjecutarComando();
                }
            }
            catch (Exception oError)
            {
                Log.GuardarError("Descripción de error: ", oError);
                throw new Exception("PromocionDescEmp");
            }
            return;
        }

        public void ModificarDescPorcentaje(Conexion conn)
        {
            try
            {

                Log.GuardarError("Asignamos la tabla de promocion descuento por porcentaje", null);
                //Si el código está vacio es que no existe y tenemos que crearlo
                if (!string.IsNullOrEmpty(PromocionDescPor.Code))
                {
                    //Se busca el código de la promoción
                    conn.LimpiarParametros();
                    conn.CargarConsulta(ConsultasPromocion.ModificarPromocionDescPor);
                    conn.AgregarParametro("pNombre", Nombre);
                    conn.AgregarParametro("pDescuento1", PromocionDescPor.U_SO1_DESCUENTO1.ToString());
                    conn.AgregarParametro("pDescuento2", PromocionDescPor.U_SO1_DESCUENTO2.ToString());
                    conn.AgregarParametro("pMantenerDesc", PromocionDescPor.U_SO1_MANTENERDESC);
                    conn.AgregarParametro("pCode", PromocionDescPor.Code);
                    conn.EjecutarComando();
                }
                else
                {
                    conn.LimpiarParametros();
                    conn.CargarConsulta(ConsultasPromocion.RegistrarPromocionDescPor);
                    conn.AgregarParametro("pCodigo", Codigo);
                    conn.AgregarParametro("pNombre", Nombre);
                    conn.AgregarParametro("pPromo", U_SO1_PROMOCION);
                    conn.AgregarParametro("pPromoCadena", Codigo);
                    conn.AgregarParametro("pDescuento1", PromocionDescPor.U_SO1_DESCUENTO1.ToString());
                    conn.AgregarParametro("pDescuento2", PromocionDescPor.U_SO1_DESCUENTO2.ToString());
                    conn.AgregarParametro("pMantenerDesc", PromocionDescPor.U_SO1_MANTENERDESC);
                    conn.EjecutarComando();
                }
            }
            catch (Exception oError)
            {
                Log.GuardarError("Descripción de error: ", oError);
                throw new Exception("PromocionDescEmp");
            }
            return;
        }

        public void ModificarDescVolumen(Conexion conn, ReportesWeb.Utilidades.CodigoControl codigosControl)
        {

            #region Bitácora

            // AUTOR:               Juan Sol
            // FECHA:               19.11.2019
            // COMENTARIOS:         Se cambia la forma en el que se obtiene el nuevo código de control

            #endregion

            try
            {

                Log.GuardarError("Asignamos la tabla de promocion Desc por Empleado", null);
                //Si el código está vacio es que no existe y tenemos que crearlo
                conn.LimpiarParametros();
                if (!string.IsNullOrEmpty(PromocionDescVol.Code))
                {
                    conn.CargarConsulta(ConsultasPromocion.ModificarPromocionDescVol);
                    conn.AgregarParametro("pNombre", Nombre);
                    conn.AgregarParametro("pCantidad", PromocionDescVol.U_SO1_CANTIDAD);
                    conn.AgregarParametro("pPorcentajeDesc", PromocionDescVol.U_SO1_PORCENTAJEDESC.ToString());
                    conn.AgregarParametro("pMantenerDesc", PromocionDescVol.U_SO1_MANTENERDESC.ToString());
                    conn.AgregarParametro("pAcumulaArt", PromocionDescVol.U_SO1_ACUMULARART);
                    conn.AgregarParametro("pActDesIncre", PromocionDescVol.U_SO1_ACTDESCINCRE);
                    conn.AgregarParametro("pCode", PromocionDescVol.Code);
                    conn.EjecutarComando();
                }
                else
                {
                    conn.CargarConsulta(ConsultasPromocion.RegistrarPromocionDescVol);
                    conn.AgregarParametro("pCodigo", Codigo);
                    conn.AgregarParametro("pNombre", Nombre);
                    conn.AgregarParametro("pPromo", U_SO1_PROMOCION);
                    conn.AgregarParametro("pPromoCadena", Codigo);
                    conn.AgregarParametro("pCantidad", 0);
                    conn.AgregarParametro("pPorcentajeDesc", 0);
                    conn.AgregarParametro("pMantenerDesc", "N");
                    conn.AgregarParametro("pAcumulaArt", PromocionDescVol.U_SO1_ACUMULARART);
                    conn.AgregarParametro("pActDesIncre", PromocionDescVol.U_SO1_ACTDESCINCRE);
                    conn.EjecutarComando();
                }


                DataTable dtDescuentos = null;
                conn.LimpiarParametros();
                conn.CargarConsulta(ConsultasPromocion.ConsultarCodigosDescuentos);
                conn.AgregarLiteral("TABLA", "@SO1_01PROMODESVOLES");
                conn.AgregarParametro("pPromocion", U_SO1_PROMOCION);
                dtDescuentos = conn.EjecutarTabla();

                Log.GuardarError("Asignamos la tabla de Monto de descuento", null);

                foreach (DataRow fila in dtDescuentos.Rows)
                {
                    //Si no se encuentra en el diccionario significa que fué eliminado
                    //if (!PromocionDescEsc._descuentos.Contains(fila.Valor<string>("Articulo")))
                    bool existe = false;

                    for (int i = 0; i < PromocionDescVol._porcentajes.Count(); i++)
                    {
                        if (PromocionDescVol._porcentajes[i].Code == fila.Valor<string>("Codigo"))
                        {
                            //Actualizamos el registro con los nuevos valores
                            existe = true;
                            conn.LimpiarParametros();
                            conn.CargarConsulta(ConsultasPromocion.ModificarPromocionDescVolEs);
                            conn.AgregarParametro("pCantidad", PromocionDescVol._porcentajes[i].U_SO1_CANTIDAD);
                            conn.AgregarParametro("pPorcentajeDesc", PromocionDescVol._porcentajes[i].U_SO1_PORCENTAJEDESC);
                            conn.AgregarParametro("pMantenerDesc", PromocionDescVol._porcentajes[i].U_SO1_MANTENERDESC);
                            conn.AgregarParametro("pCode", PromocionDescVol._porcentajes[i].Code);
                            conn.EjecutarComando();
                            PromocionDescVol._porcentajes.Remove(PromocionDescVol._porcentajes[i]);
                        }
                    }
                    if (!existe)
                    {
                        //Eliminamos el registro, ya que fué eliminado de la nueva lista
                        conn.LimpiarParametros();
                        conn.CargarConsulta(ConsultasPromocion.EliminarRegistro);
                        conn.AgregarLiteral("TABLA", "@SO1_01PROMODESVOLES");
                        conn.AgregarParametro("pCode", fila.Valor<string>("Codigo"));
                        conn.EjecutarComando();

                    }
                }

                // Recorremos la lista para para crear los nuevos registros

                for (int i = 0; i < PromocionDescVol._porcentajes.Count(); i++)
                {
                    string nuevoCodigo = String.Empty;
                    nuevoCodigo = codigosControl.Proximo(TablaCodigoControl.PROMODESVOLES);
                    conn.LimpiarParametros();
                    conn.CargarConsulta(ConsultasPromocion.RegistrarPromocionDescVolEs);
                    conn.AgregarParametro("pCodigo", nuevoCodigo);
                    conn.AgregarParametro("pNombre", nuevoCodigo);
                    conn.AgregarParametro("pPromo", U_SO1_PROMOCION);
                    conn.AgregarParametro("pPromoCadena", Codigo);
                    conn.AgregarParametro("pCantidad", PromocionDescVol._porcentajes[i].U_SO1_CANTIDAD);
                    conn.AgregarParametro("pPorcentajeDesc", PromocionDescVol._porcentajes[i].U_SO1_PORCENTAJEDESC);
                    conn.AgregarParametro("pMantenerDesc", PromocionDescVol._porcentajes[i].U_SO1_MANTENERDESC);
                    conn.EjecutarComando();
                }

            }
            catch (Exception oError)
            {
                Log.GuardarError("Descripción de error: ", oError);
                throw new Exception("PromocionDescEmp");
            }
            return;

        }

        public void ModificarKitVenta(Conexion conn, ReportesWeb.Utilidades.CodigoControl codigosControl)
        {

            #region Bitácora

            // AUTOR:               Juan Sol
            // FECHA:               19.11.2019
            // COMENTARIOS:         Se cambia la forma en el que se obtiene el nuevo código de control

            #endregion

            try
            {

                Log.GuardarError("Asignamos la tabla de KitVenta", null);
                //Si el código está vacio es que no existe y tenemos que crearlo
                conn.LimpiarParametros();
                if (!string.IsNullOrEmpty(PromocionKitVenta.Code))
                {
                    // Actualizamos los datos del Kit de Venta
                    conn.CargarConsulta(ConsultasPromocion.ModificarPromocionKitVenta);
                    conn.AgregarParametro("pNombre", Nombre);
                    conn.AgregarParametro("pTipoRegalo", PromocionKitVenta.U_SO1_TIPOREGALO);
                    conn.AgregarParametro("pConImpuesto", PromocionKitVenta.U_SO1_CONIMPUESTO);
                    conn.AgregarParametro("pCode", PromocionKitVenta.Code);
                    conn.EjecutarComando();

                    // Traemos una tabla con los identificadores actuales
                    conn.LimpiarParametros();
                    conn.CargarConsulta(ConsultasPromocion.ConsultaIdentificador);
                    conn.AgregarParametro("pPromocion", U_SO1_PROMOCION);
                    DataTable dtIdentificadorPromo = conn.EjecutarTabla();

                    // Vamos a iterar el datatable con identificadores para remover los existente y los nuevos se quedarán para ser creados
                    foreach (DataRow fila in dtIdentificadorPromo.Rows)
                    {
                        var ambito = PromocionKitVenta._identificador.Find(r => r.Code == fila.Valor<string>("Codigo"));
                        if (ambito != null) //Como se encontró el ámbito debemos actualizar sus articulos
                        {
                            // Actualizamos los articulos asociados al ámbito
                            conn.LimpiarParametros();
                            conn.CargarConsulta(ConsultasPromocion.ConsultarArticulosxIdentificador); //Aqui va una nueva query para cargar los articulos de este identificador
                            conn.AgregarParametro("pPromocion", ambito.U_SO1_PROMOCION);
                            conn.AgregarParametro("pCodigoAmbito", ambito.U_SO1_CODIGOAMBITO);
                            DataTable dtArticuloPromo = conn.EjecutarTabla();
                            foreach (DataRow filaArticulo in dtArticuloPromo.Rows)
                            {
                                if (!ambito._articulos.ContainsKey(filaArticulo.Valor<string>("Articulo")))
                                {
                                    //Se busca el código del registro
                                    conn.LimpiarParametros();
                                    conn.CargarConsulta(ConsultasPromocion.EliminarRegistro);
                                    conn.AgregarLiteral("TABLA", "@SO1_01PROMOARTICULO");
                                    conn.AgregarParametro("pCode", filaArticulo.Valor<string>("Codigo"));
                                    conn.EjecutarComando();
                                    ambito._articulos.Remove(filaArticulo.Valor<string>("Articulo"));
                                }
                            }

                            //En el diccionario sólo quedan los nuevos valores
                            foreach (KeyValuePair<string, MPromoArticulo> articulo in ambito._articulos)
                            {
                                conn.LimpiarParametros();
                                if (string.IsNullOrEmpty(articulo.Value.Code))
                                {
                                    string nuevoCodigo = String.Empty;
                                    nuevoCodigo = codigosControl.Proximo(TablaCodigoControl.PROMOARTICULO);
                                    conn.CargarConsulta(ConsultasPromocion.RegistrarArticulo);
                                    conn.AgregarParametro("pCode", nuevoCodigo);
                                    conn.AgregarParametro("pName", nuevoCodigo);
                                    conn.AgregarParametro("pPromo", U_SO1_PROMOCION);
                                    conn.AgregarParametro("pPromoCadena", Codigo);
                                    conn.AgregarParametro("pTipo", "G");
                                    conn.AgregarParametro("pArticulo", articulo.Value.U_SO1_ARTICULO);
                                    conn.AgregarParametro("pImporte", articulo.Value.U_SO1_IMPORTE);
                                    conn.AgregarParametro("pCodigoAmbito", ambito.U_SO1_CODIGOAMBITO);
                                }
                                else
                                {
                                    conn.CargarConsulta(ConsultasPromocion.ActualizarArticulo);
                                    conn.AgregarParametro("pImporte", articulo.Value.U_SO1_IMPORTE);
                                    conn.AgregarParametro("pCodigo", articulo.Value.Code);
                                }
                                conn.EjecutarComando();
                            }
                            // Removemos el ámbito de la lista
                            PromocionKitVenta._identificador.Remove(ambito);
                        }
                        else // Como no se encontró el ámbito debemos eliminarlo de la DB y a sus articulos
                        {
                            // Cargamos la consulta para eliminar el ámbito
                            conn.LimpiarParametros();
                            conn.CargarConsulta(ConsultasPromocion.EliminarIdentificadores);
                            conn.AgregarLiteral("TABLA", "@SO1_01PROMOAMBITO");
                            conn.AgregarParametro("pCodigo", fila.Valor<string>("Promo"));
                            conn.AgregarParametro("pIdentificador", fila.Valor<string>("CodigoAmbito"));
                            conn.EjecutarComando();

                            // Cargamos la consulta para eliminar los articulos del ámbito que eliminamos
                            conn.LimpiarParametros();
                            conn.CargarConsulta(ConsultasPromocion.EliminarIdentificadores);
                            conn.AgregarLiteral("TABLA", "@SO1_01PROMOARTICULO");
                            conn.AgregarParametro("pCodigo", fila.Valor<string>("Promo"));
                            conn.AgregarParametro("pIdentificador", fila.Valor<string>("CodigoAmbito"));
                            conn.EjecutarComando();
                        }
                    }

                    foreach (var promoKit in PromocionKitVenta._identificador)
                    {
                        conn.LimpiarParametros();
                        // Si el código del Ámbito es nulo, sigifica que no existe y lo creamos
                        if (promoKit.Code == null)
                        {
                            conn.LimpiarParametros();
                            conn.CargarConsulta(ConsultasPromocion.tipoAmbito);
                            string sCodigoControlIdenA = String.Empty;
                            sCodigoControlIdenA = codigosControl.Proximo(TablaCodigoControl.PROMOKITVEN);

                            DataTable tipo = conn.EjecutarTabla();
                            promoKit.U_SO1_CODIGOAMBITO = tipo.Rows[0].Valor<int>("tipo") + 1;

                            // Registramos el identificador
                            conn.CargarConsulta(ConsultasPromocion.RegistrarIdentificador);
                            conn.AgregarParametro("pCode", sCodigoControlIdenA);
                            conn.AgregarParametro("pName", sCodigoControlIdenA);
                            conn.AgregarParametro("pPromocion", U_SO1_PROMOCION);
                            conn.AgregarParametro("pPromoCadena", Codigo);
                            conn.AgregarParametro("pCodigoAmbito", promoKit.U_SO1_CODIGOAMBITO);
                            conn.AgregarParametro("pNombreAmbito", promoKit.U_SO1_NOMBREAMBITO);
                            conn.AgregarParametro("pCantidad", promoKit.U_SO1_CANTIDAD);
                            conn.AgregarParametro("pImagen", promoKit.U_SO1_IMAGEN);
                            conn.AgregarParametro("pMantenerPromo", promoKit.U_SO1_MANTENERPROMO);
                            conn.AgregarParametro("pAmbitOblig", promoKit.U_SO1_AMBOBLIGATORIO);
                            conn.EjecutarComando();
                            if (promoKit != null)
                            {
                                // Registramos los articulos con el identificador
                                foreach (var j in promoKit.Articulos)
                                {
                                    conn.LimpiarParametros();
                                    conn.CargarConsulta(ConsultasPromocion.tipoAmbito);
                                    string nuevoCodigo = String.Empty;
                                    nuevoCodigo = codigosControl.Proximo(TablaCodigoControl.PROMOARTICULO);
                                    promoKit.U_SO1_CODIGOAMBITO = tipo.Rows[0].Valor<int>("tipo") + 1;
                                    conn.CargarConsulta(ConsultasPromocion.RegistrarArticuloIdentificador);
                                    conn.AgregarParametro("pCode", nuevoCodigo);
                                    conn.AgregarParametro("pName", nuevoCodigo);
                                    conn.AgregarParametro("pPromocion", U_SO1_PROMOCION);
                                    conn.AgregarParametro("pPromoCadena", Codigo);
                                    conn.AgregarParametro("pTipo", "G");
                                    conn.AgregarParametro("pArticulo", j.U_SO1_ARTICULO);
                                    conn.AgregarParametro("pImporte", j.U_SO1_IMPORTE);
                                    conn.AgregarParametro("pCodigoAmbito", promoKit.U_SO1_CODIGOAMBITO);
                                    conn.EjecutarComando();
                                }
                            }
                        }
                    }
                }
                else {
                    // Aqui creamos un nuevo kit de venta
                    conn.LimpiarParametros();
                    
                    conn.CargarConsulta(ConsultasPromocion.RegistrarKitVenta);
                    conn.AgregarParametro("pCode", Codigo);
                    conn.AgregarParametro("pName", Nombre);
                    conn.AgregarParametro("pPromocion", U_SO1_PROMOCION);
                    conn.AgregarParametro("pPromoCadena", Codigo);
                    conn.AgregarParametro("pTipoRegalo", PromocionKitVenta.U_SO1_TIPOREGALO);
                    conn.AgregarParametro("pConImpuesto", PromocionKitVenta.U_SO1_CONIMPUESTO);
                    conn.EjecutarComando();

                    foreach (var PromocionKitV in PromocionKitVenta.Identificador)
                    {
                        conn.LimpiarParametros();
                        conn.CargarConsulta(ConsultasPromocion.tipoAmbito);
                        string nuevoCodigo = String.Empty;
                        nuevoCodigo = codigosControl.Proximo(TablaCodigoControl.PROMOKITVEN);
                        DataTable tipo = conn.EjecutarTabla();
                        PromocionKitV.U_SO1_CODIGOAMBITO = tipo.Rows[0].Valor<int>("tipo") + 1;

                        //Registramos el Ámbito
                        conn.CargarConsulta(ConsultasPromocion.RegistrarIdentificador);
                        conn.AgregarParametro("pCode", nuevoCodigo);
                        conn.AgregarParametro("pName", nuevoCodigo);
                        conn.AgregarParametro("pPromocion", U_SO1_PROMOCION);
                        conn.AgregarParametro("pPromoCadena", Codigo);
                        conn.AgregarParametro("pCodigoAmbito", PromocionKitV.U_SO1_CODIGOAMBITO);
                        conn.AgregarParametro("pNombreAmbito", PromocionKitV.U_SO1_NOMBREAMBITO);
                        conn.AgregarParametro("pCantidad", PromocionKitV.U_SO1_CANTIDAD);
                        conn.AgregarParametro("pImagen", PromocionKitV.U_SO1_IMAGEN);
                        conn.AgregarParametro("pMantenerPromo", PromocionKitV.U_SO1_MANTENERPROMO);
                        conn.AgregarParametro("pAmbitOblig", PromocionKitV.U_SO1_AMBOBLIGATORIO);
                        conn.EjecutarComando();
                        if (PromocionKitV != null)
                        {
                            foreach (var j in PromocionKitV.Articulos)
                            {
                                conn.LimpiarParametros();

                                //Registramos Articulos en identificador
                                conn.CargarConsulta(ConsultasPromocion.tipoAmbito);
                                string nuevoCodigoArticulo = String.Empty;
                                nuevoCodigoArticulo = codigosControl.Proximo(TablaCodigoControl.PROMOARTICULO);
                                PromocionKitV.U_SO1_CODIGOAMBITO = tipo.Rows[0].Valor<int>("tipo") + 1;
                                conn.CargarConsulta(ConsultasPromocion.RegistrarArticuloIdentificador);
                                conn.AgregarParametro("pCode", nuevoCodigoArticulo);
                                conn.AgregarParametro("pName", nuevoCodigoArticulo);
                                conn.AgregarParametro("pPromocion", U_SO1_PROMOCION);
                                conn.AgregarParametro("pPromoCadena", Codigo);
                                conn.AgregarParametro("pTipo", "G");
                                conn.AgregarParametro("pArticulo", j.U_SO1_ARTICULO);
                                conn.AgregarParametro("pImporte", j.U_SO1_IMPORTE);
                                conn.AgregarParametro("pCodigoAmbito", PromocionKitV.U_SO1_CODIGOAMBITO);
                                conn.EjecutarComando();
                            }
                        }
                    }
                }
            }
            catch (Exception oError)
            {
                Log.GuardarError("Descripción de error: ", oError);
                throw new Exception("PromocionKitVenta");
            }
            return;
        }

        public void ModificarPoliticaVenta(Conexion conn, ReportesWeb.Utilidades.CodigoControl codigosControl)
        {

            #region Bitácora

            // AUTOR:               Juan Sol
            // FECHA:               19.11.2019
            // COMENTARIOS:         Se cambia la forma en el que se obtiene el nuevo código de control

            #endregion

            try
            {

                Log.GuardarError("Asignamos la tabla de promocion Politica de Venta", null);
                conn.LimpiarParametros();

                if (!string.IsNullOrEmpty(PromocionPoliticaVenta.Code))
                {
                    conn.CargarConsulta(ConsultasPromocion.ModificarPromocionPolVenta);
                    conn.AgregarParametro("pNombre", Nombre);
                    conn.AgregarParametro("pAcumulaArt", PromocionPoliticaVenta.U_SO1_ACUMULARART);
                    conn.AgregarParametro("pMantenerDesc", PromocionPoliticaVenta.U_SO1_MANTENERDESC);
                    conn.AgregarParametro("pCode", Codigo);
                    conn.EjecutarComando();
                }
                else
                {
                    conn.CargarConsulta(ConsultasPromocion.RegistrarPromocionPolVenta);
                    conn.AgregarParametro("pCodigo", Codigo);
                    conn.AgregarParametro("pNombre", Nombre);
                    conn.AgregarParametro("pPromo", U_SO1_PROMOCION);
                    conn.AgregarParametro("pPromoCadena", Codigo);
                    conn.AgregarParametro("pAcumulaArt", PromocionPoliticaVenta.U_SO1_ACUMULARART);
                    conn.AgregarParametro("pMantenerDesc", PromocionPoliticaVenta.U_SO1_MANTENERDESC);
                    conn.EjecutarComando();
                }

                DataTable dtListasPrecios = null;
                conn.LimpiarParametros();
                conn.CargarConsulta(ConsultasPromocion.ConsultarListaPreciosPromo2);
                conn.AgregarParametro("pPromocion", U_SO1_PROMOCION);
                dtListasPrecios = conn.EjecutarTabla();

                Log.GuardarError("Asignamos la tabla de Articulos", null);

                foreach (DataRow fila in dtListasPrecios.Rows)
                {
                    //Si no se encuentra en el diccionario significa que fué eliminado
                    //if (!PromocionKitRegalo._articulos.ContainsKey(fila.Valor<string>("Articulo")))
                    if (!PromocionPoliticaVenta._precios.ContainsKey(fila.Valor<string>("ListaPrecio")))
                    {
                        conn.LimpiarParametros();
                        conn.CargarConsulta(ConsultasPromocion.EliminarRegistro);
                        conn.AgregarLiteral("TABLA", "@SO1_01PROMOPOLVENLI");
                        conn.AgregarParametro("pCode", fila.Valor<string>("Codigo"));
                        conn.EjecutarComando();
                    }
                    else
                    {
                        PromocionPoliticaVenta._precios.Remove(fila.Valor<string>("ListaPrecio"));
                    }
                }

                //En el diccionario sólo quedan los nuevos valores
                foreach (KeyValuePair<string, MPromoPrecios> listaPrecio in PromocionPoliticaVenta._precios)
                {
                    conn.LimpiarParametros();
                    conn.CargarConsulta(ConsultasPromocion.RegistrarListaPrecio);
                    string nuevoCodigo = String.Empty;
                    nuevoCodigo = codigosControl.Proximo(TablaCodigoControl.PROMOPOLVENLI);
                    //String sCodigoControl = String.Empty;
                    //sCodigoControl = Adaptador.Utilidades.ObtenerCodigoValido("@SO1_01PROMOPOLVENLI");
                    conn.AgregarParametro("pCodigo", nuevoCodigo);
                    conn.AgregarParametro("pNombre", nuevoCodigo);
                    conn.AgregarParametro("pPromo", U_SO1_PROMOCION);
                    conn.AgregarParametro("pPromoCadena", Codigo);
                    conn.AgregarParametro("pCantidadIni", listaPrecio.Value.U_SO1_CANTIDADINI);
                    conn.AgregarParametro("pListaPrecio", listaPrecio.Value.U_SO1_LISTAPRECIO);
                    conn.EjecutarComando();
                }

            }
            catch (Exception oError)
            {

            }
        }

        public void ModificarRegaloMonto(Conexion conn, ReportesWeb.Utilidades.CodigoControl codigosControl)
        {

            #region Bitácora

            // AUTOR:               Juan Sol
            // FECHA:               19.11.2019
            // COMENTARIOS:         Se cambia la forma en el que se obtiene el nuevo código de control

            #endregion

            try
            {
                Log.GuardarError("Asignamos la tabla de promocion Politica de Venta", null);
                conn.LimpiarParametros();

                if (!string.IsNullOrEmpty(PromocionRegaloMonto.Code))
                {
                    conn.CargarConsulta(ConsultasPromocion.ModificarPromocionRegaloMonto);
                    conn.AgregarParametro("pNombre", Nombre);
                    conn.AgregarParametro("pAcumulaArt", PromocionRegaloMonto.U_SO1_AGREGARARTAUTO);
                    conn.AgregarParametro("pRegaloPrecio", PromocionRegaloMonto.U_SO1_REGALOPRECIO1);
                    conn.AgregarParametro("pxMonto", PromocionRegaloMonto.U_SO1_PORCADAXMONTO);
                    conn.AgregarParametro("pPorcentajePro", PromocionRegaloMonto.U_SO1_PORCENTAJEPRO);
                    conn.AgregarParametro("pMensaje", PromocionRegaloMonto.U_SO1_MENSAJEPROXIM);
                    conn.AgregarParametro("pMoneda", PromocionRegaloMonto.U_SO1_MONEDA);
                    conn.AgregarParametro("pMontoRegalo", PromocionRegaloMonto.U_SO1_MONTOREGALO);
                    conn.AgregarParametro("pCantidadRegalo", PromocionRegaloMonto.U_SO1_CANTIDADREGAL);
                    conn.AgregarParametro("pCode", PromocionRegaloMonto.Code);
                    conn.EjecutarComando();
                }
                else
                {
                    conn.CargarConsulta(ConsultasPromocion.RegistrarPromocionRegaloMonto);
                    conn.AgregarParametro("pCodigo", Codigo);
                    conn.AgregarParametro("pNombre", Nombre);
                    conn.AgregarParametro("pPromo", U_SO1_PROMOCION);
                    conn.AgregarParametro("pPromoCadena", Codigo);
                    conn.AgregarParametro("pAcumulaArt", PromocionRegaloMonto.U_SO1_AGREGARARTAUTO);
                    conn.AgregarParametro("pRegaloPrecio", PromocionRegaloMonto.U_SO1_REGALOPRECIO1);
                    conn.AgregarParametro("pxMonto", PromocionRegaloMonto.U_SO1_PORCADAXMONTO);
                    conn.AgregarParametro("pPorcentajePro", PromocionRegaloMonto.U_SO1_PORCENTAJEPRO);
                    conn.AgregarParametro("pMensaje", PromocionRegaloMonto.U_SO1_MENSAJEPROXIM);
                    conn.AgregarParametro("pMoneda", PromocionRegaloMonto.U_SO1_MONEDA);
                    conn.AgregarParametro("pMontoRegalo", PromocionRegaloMonto.U_SO1_MONTOREGALO);
                    conn.AgregarParametro("pCantidadRegalo", PromocionRegaloMonto.U_SO1_CANTIDADREGAL);
                    conn.EjecutarComando();
                }

                conn.LimpiarParametros();
                conn.CargarConsulta(ConsultasPromocion.ConsultarArticulosPromo);
                conn.AgregarParametro("pPromocion", U_SO1_PROMOCION);
                conn.AgregarParametro("pTipo", "R");
                DataTable dtArticuloPromo = conn.EjecutarTabla();
                foreach (DataRow fila in dtArticuloPromo.Rows)
                {
                    if (!PromocionRegaloMonto._articulos.ContainsKey(fila.Valor<string>("Articulo")))
                    {

                        conn.LimpiarParametros();
                        conn.CargarConsulta(ConsultasPromocion.EliminarRegistro);
                        conn.AgregarLiteral("TABLA", "@SO1_01PROMOARTICULO");
                        conn.AgregarParametro("pCode", fila.Valor<string>("Codigo"));
                        conn.EjecutarComando();
                    }
                    else
                    {
                        PromocionRegaloMonto._articulos.Remove(fila.Valor<string>("Articulo"));
                    }
                }

                //En el diccionario sólo quedan los nuevos valores
                foreach (KeyValuePair<string, MPromoArticulo> articulo in PromocionRegaloMonto._articulos)
                {
                    conn.LimpiarParametros();
                    conn.CargarConsulta(ConsultasPromocion.RegistrarArticulo);
                    string nuevoCodigo = String.Empty;
                    nuevoCodigo = codigosControl.Proximo(TablaCodigoControl.PROMOARTICULO);
                    //String sCodigoControl = String.Empty;
                    //sCodigoControl = Adaptador.Utilidades.ObtenerCodigoValido("@SO1_01PROMOARTICULO");
                    conn.AgregarParametro("pCode", nuevoCodigo);
                    conn.AgregarParametro("pName", nuevoCodigo);
                    conn.AgregarParametro("pPromo", U_SO1_PROMOCION);
                    conn.AgregarParametro("pPromoCadena", Codigo);
                    conn.AgregarParametro("pTipo", "R");
                    conn.AgregarParametro("pArticulo", articulo.Value.U_SO1_ARTICULO);
                    conn.AgregarParametro("pImporte", articulo.Value.U_SO1_IMPORTE);
                    conn.AgregarParametro("pCodigoAmbito", articulo.Value.U_SO1_CODIGOAMBITO);
                    conn.EjecutarComando();
                }

            }
            catch (Exception oError)
            {

            }
        }

        public void ModificarCupon(Conexion conn, ReportesWeb.Utilidades.CodigoControl codigosControl)
        {

            #region Bitácora

            // AUTOR:               Juan Sol
            // FECHA:               19.11.2019
            // COMENTARIOS:         Se cambia la forma en el que se obtiene el nuevo código de control

            #endregion

            try
            {
                Log.GuardarError("Asignamos la tabla de cupones", null);
                conn.LimpiarParametros();

                if (!string.IsNullOrEmpty(PromocionCupon.Code))
                {
                    conn.CargarConsulta(ConsultasPromocion.ModificarCupones);
                    conn.AgregarParametro("pTipo", PromocionCupon.U_SO1_TIPO);
                    conn.AgregarParametro("pMonto", PromocionCupon.U_SO1_MONTO);
                    conn.AgregarParametro("pCantidad", PromocionCupon.U_SO1_CANTIDAD);
                    conn.AgregarParametro("pNumeroCupones", PromocionCupon.U_SO1_NUMEROCUPONES);
                    conn.AgregarParametro("pActImporte", PromocionCupon.U_SO1_ACTIMPORTEIMP);
                    conn.AgregarParametro("pCompoTras", PromocionCupon.U_SO1_COMPOTRASPROM);
                    conn.AgregarParametro("pAplicaProm", PromocionCupon.U_SO1_APLICPROMIGUAL);
                    conn.AgregarParametro("pTipoComportamiento", PromocionCupon.U_SO1_TIPOCOMPORTAMI);
                    conn.AgregarParametro("pCode", PromocionCupon.Code);
                    conn.EjecutarComando();
                }
                else
                {
                    conn.CargarConsulta(ConsultasPromocion.RegistrarCupones);
                    conn.AgregarParametro("pCodigo", Codigo);
                    conn.AgregarParametro("pNombre", Nombre);
                    conn.AgregarParametro("pPromo", U_SO1_PROMOCION);
                    conn.AgregarParametro("pPromoCadena", Codigo);
                    conn.AgregarParametro("pTipo", PromocionCupon.U_SO1_TIPO);
                    conn.AgregarParametro("pMonto", PromocionCupon.U_SO1_MONTO);
                    conn.AgregarParametro("pCantidad", PromocionCupon.U_SO1_CANTIDAD);
                    conn.AgregarParametro("pNumeroCupones", PromocionCupon.U_SO1_NUMEROCUPONES);
                    conn.AgregarParametro("pActImporte", PromocionCupon.U_SO1_ACTIMPORTEIMP);
                    conn.AgregarParametro("pCompoTras", PromocionCupon.U_SO1_COMPOTRASPROM);
                    conn.AgregarParametro("pAplicaProm", PromocionCupon.U_SO1_APLICPROMIGUAL);
                    conn.AgregarParametro("pTipoComportamiento", PromocionCupon.U_SO1_TIPOCOMPORTAMI);
                    conn.EjecutarComando();
                }

                conn.LimpiarParametros();
                conn.CargarConsulta(ConsultasPromocion.ConsultarCatalogoHoras);
                conn.AgregarParametro("pPromocion", U_SO1_PROMOCION);
                DataTable dtCatalogo = conn.EjecutarTabla();
                foreach (DataRow fila in dtCatalogo.Rows)
                {
                    if (!PromocionCupon._catalogosHoras.ContainsKey(fila.Valor<string>("CodigoCatalogo")))
                    {

                        conn.LimpiarParametros();
                        conn.CargarConsulta(ConsultasPromocion.EliminarRegistro);
                        conn.AgregarLiteral("TABLA", "@SO1_01PROMCATHORAR");
                        conn.AgregarParametro("pCode", fila.Valor<string>("Codigo"));
                        conn.EjecutarComando();
                    }
                    else
                    {
                        PromocionCupon._catalogosHoras.Remove(fila.Valor<string>("CodigoCatalogo"));
                    }
                }

                //En el diccionario sólo quedan los nuevos valores
                foreach (KeyValuePair<string, MPromoCupon> horario in PromocionCupon._catalogosHoras)
                {
                    conn.LimpiarParametros();
                    conn.CargarConsulta(ConsultasPromocion.RegistrarCatalogo);
                    string nuevoCodigo = String.Empty;
                    nuevoCodigo = codigosControl.Proximo(TablaCodigoControl.PROMCATHORAR);
                   // conn.AgregarParametro("pCode", nuevoCodigo);
                    //conn.AgregarParametro("pName", nuevoCodigo);
                    conn.AgregarParametro("pCodigo", nuevoCodigo);
                    conn.AgregarParametro("pNombre", nuevoCodigo);
                    conn.AgregarParametro("pPromo", U_SO1_PROMOCION);
                    conn.AgregarParametro("pPromoCadena", Codigo);
                    conn.AgregarParametro("pCodigoHorario", horario.Value.U_SO1_CODIGCATEHORAR);
                    conn.EjecutarComando();
                }

            }
            catch (Exception oError)
            {
                Log.GuardarError("Descripción de error: ", oError);
                throw new Exception("PromocionCupon");
            }

        }

        public void ModificarKitRegalo(Conexion conn, ReportesWeb.Utilidades.CodigoControl codigosControl)
        {

            #region Bitácora

            // AUTOR:               Juan Sol
            // FECHA:               19.11.2019
            // COMENTARIOS:         Se cambia la forma en el que se obtiene el nuevo código de control

            #endregion

            try
            {

                Log.GuardarError("Asignamos la tabla de promocion Kit de Regalo", null);
                //Si el código está vacio es que no existe y tenemos que crearlo
                conn.LimpiarParametros();
                if (!string.IsNullOrEmpty(PromocionKitRegalo.Code))
                {
                    conn.CargarConsulta(ConsultasPromocion.ModificarPromocionKitReg);
                    conn.AgregarParametro("pNombre", Nombre);
                    conn.AgregarParametro("pCantidadCri", PromocionKitRegalo.U_SO1_CANTIDADCRI);
                    conn.AgregarParametro("pCantidadReg", PromocionKitRegalo.U_SO1_CANTIDADREG);
                    conn.AgregarParametro("pTipoRegalo", string.IsNullOrEmpty(PromocionKitRegalo.U_SO1_TIPOREGALO) ? "N" : PromocionKitRegalo.U_SO1_TIPOREGALO);
                    conn.AgregarParametro("pMonto", PromocionKitRegalo.U_SO1_MONTO);
                    conn.AgregarParametro("pRegaloPrecio", PromocionKitRegalo.U_SO1_REGALOPRECIO1);
                    conn.AgregarParametro("pAgregarArt", string.IsNullOrEmpty(PromocionKitRegalo.U_SO1_AGREGARARTAUTO) ? "N" : PromocionKitRegalo.U_SO1_AGREGARARTAUTO);
                    conn.AgregarParametro("pCode", PromocionKitRegalo.Code);
                    conn.EjecutarComando();
                }
                else
                {
                    conn.CargarConsulta(ConsultasPromocion.RegistrarPromocionKitReg);
                    conn.AgregarParametro("pCodigo", Codigo);
                    conn.AgregarParametro("pNombre", Nombre);
                    conn.AgregarParametro("pPromo", U_SO1_PROMOCION);
                    conn.AgregarParametro("pPromoCadena", Codigo);
                    conn.AgregarParametro("pCantidadCri", PromocionKitRegalo.U_SO1_CANTIDADCRI);
                    conn.AgregarParametro("pCantidadReg", PromocionKitRegalo.U_SO1_CANTIDADREG);
                    conn.AgregarParametro("pTipoRegalo", string.IsNullOrEmpty(PromocionKitRegalo.U_SO1_TIPOREGALO) ? "N" : PromocionKitRegalo.U_SO1_TIPOREGALO);
                    conn.AgregarParametro("pMonto", PromocionKitRegalo.U_SO1_MONTO);
                    conn.AgregarParametro("pRegaloPrecio", PromocionKitRegalo.U_SO1_REGALOPRECIO1);
                    conn.AgregarParametro("pAgregarArt", string.IsNullOrEmpty(PromocionKitRegalo.U_SO1_AGREGARARTAUTO) ? "N" : PromocionKitRegalo.U_SO1_AGREGARARTAUTO);
                    conn.EjecutarComando();
                }

                DataTable dtArticulos = null;
                conn.LimpiarParametros();
                conn.CargarConsulta(ConsultasPromocion.ConsultarCodigosArticulos);
                conn.AgregarParametro("pPromocion", U_SO1_PROMOCION);
                conn.AgregarParametro("pTipo", "R");
                dtArticulos = conn.EjecutarTabla();

                Log.GuardarError("Asignamos la tabla de Articulos", null);

                foreach (DataRow fila in dtArticulos.Rows)
                {
                    //Si no se encuentra en el diccionario significa que fué eliminado
                    //if (!PromocionKitRegalo._articulos.ContainsKey(fila.Valor<string>("Articulo")))
                    if (!PromocionKitRegalo._articulos.ContainsKey(fila.Valor<string>("Articulo")))
                    {
                        //Se busca el código del registro
                        conn.LimpiarParametros();
                        conn.CargarConsulta(ConsultasPromocion.VerificarRegistro);
                        conn.AgregarLiteral("TABLA", "@SO1_01PROMOARTICULO");
                        conn.AgregarParametro("pCode", fila.Valor<string>("Codigo"));
                        conn.EjecutarLector();
                        if (!conn.LeerFila())
                        {
                            Log.GuardarError("El articulo: " + fila.Valor<string>("Articulo") + " no se encuentra en la base de datos", null);
                            throw new Exception("Eliminar articulo");
                        }
                        conn.CerrarLector();
                        conn.LimpiarParametros();
                        conn.CargarConsulta(ConsultasPromocion.EliminarRegistro);
                        conn.AgregarLiteral("TABLA", "@SO1_01PROMOARTICULO");
                        conn.AgregarParametro("pCode", fila.Valor<string>("Codigo"));
                        conn.EjecutarComando();
                    }
                    else
                    {
                        PromocionKitRegalo._articulos.Remove(fila.Valor<string>("Articulo"));
                    }
                }

                //En el diccionario sólo quedan los nuevos valores
                foreach (KeyValuePair<string, MPromoArticulo> articulo in PromocionKitRegalo._articulos)
                {
                    conn.LimpiarParametros();
                    conn.CargarConsulta(ConsultasPromocion.RegistrarArticulo);
                    string nuevoCodigo = String.Empty;
                    nuevoCodigo = codigosControl.Proximo(TablaCodigoControl.PROMOARTICULO);
                    conn.AgregarParametro("pCode", nuevoCodigo);
                    conn.AgregarParametro("pName", nuevoCodigo);
                    conn.AgregarParametro("pPromo", U_SO1_PROMOCION);
                    conn.AgregarParametro("pPromoCadena", Codigo);
                    conn.AgregarParametro("pTipo", "R");
                    conn.AgregarParametro("pArticulo", articulo.Value.U_SO1_ARTICULO);
                    conn.AgregarParametro("pImporte", articulo.Value.U_SO1_IMPORTE);
                    conn.AgregarParametro("pCodigoAmbito", articulo.Value.U_SO1_CODIGOAMBITO);
                    conn.EjecutarComando();
                }
            }
            catch (Exception oError)
            {

            }
        }

        public void ModificarPrecioUnico(Conexion conn)
        {
            try
            {

                Log.GuardarError("Asignamos la tabla de promocion precio unico", null);
                //Si el código está vacio es que no existe y tenemos que crearlo
                if (!string.IsNullOrEmpty(PromocionPrecioUnico.Code))
                {
                    //Se busca el código de la promoción
                    conn.LimpiarParametros();
                    conn.CargarConsulta(ConsultasPromocion.ModificarPromocionPrecioUnico);
                    conn.AgregarParametro("pNombre", Nombre);
                    conn.AgregarParametro("pMoneda", PromocionPrecioUnico.U_SO1_MONEDA);
                    conn.AgregarParametro("pMonto", PromocionPrecioUnico.U_SO1_MONTO.ToString());
                    conn.AgregarParametro("pConImpuesto", PromocionPrecioUnico.U_SO1_CONIMPUESTO);
                    conn.AgregarParametro("pMantenerDesc", PromocionPrecioUnico.U_SO1_MANTENERDESC);
                    conn.AgregarParametro("pCode", PromocionPrecioUnico.Code);
                    conn.EjecutarComando();
                }
                else
                {
                    conn.LimpiarParametros();
                    conn.CargarConsulta(ConsultasPromocion.RegistrarPromocionPrecioUnico);
                    conn.AgregarParametro("pCodigo", Codigo);
                    conn.AgregarParametro("pNombre", Nombre);
                    conn.AgregarParametro("pPromo", U_SO1_PROMOCION);
                    conn.AgregarParametro("pPromoCadena", Codigo);
                    conn.AgregarParametro("pMoneda", PromocionPrecioUnico.U_SO1_MONEDA);
                    conn.AgregarParametro("pMonto", PromocionPrecioUnico.U_SO1_MONTO.ToString());
                    conn.AgregarParametro("pConImpuesto", PromocionPrecioUnico.U_SO1_CONIMPUESTO);
                    conn.AgregarParametro("pMantenerDesc", PromocionPrecioUnico.U_SO1_MANTENERDESC);
                    conn.EjecutarComando();
                }
            }
            catch (Exception oError)
            {
                Log.GuardarError("Descripción de error: ", oError);
                throw new Exception("PromocionAxB");
            }
            return;
        }

        public void ModificarValeDescuento(Conexion conn)
        {
            try
            {

                Log.GuardarError("Asignamos la tabla de promocion precio unico", null);
                //Si el código está vacio es que no existe y tenemos que crearlo
                if (!string.IsNullOrEmpty(PromocionValeDescuento.Code))
                {
                    //Se busca el código de la promoción
                    conn.LimpiarParametros();
                    conn.CargarConsulta(ConsultasPromocion.ModificarPromocionValeDescuento);
                    conn.AgregarParametro("pNombre", Nombre);
                    conn.AgregarParametro("pComportamiento", PromocionValeDescuento.U_SO1_COMPORTAMIENTO);
                    conn.AgregarParametro("pMoneda", PromocionValeDescuento.U_SO1_MONEDA);
                    conn.AgregarParametro("pMontoMinimo", PromocionValeDescuento.U_SO1_MONTOMINIMO);
                    conn.AgregarParametro("pTipoDescuento", PromocionValeDescuento.U_SO1_TIPODESCUENTO);
                    conn.AgregarParametro("pImporteDescuento", PromocionValeDescuento.U_SO1_IMPORTEDESC);
                    conn.AgregarParametro("pFechaVencimiento", Convert.ToDateTime(PromocionValeDescuento.U_SO1_FECHAVENCI));
                    conn.AgregarParametro("pDiasVencimiento", PromocionValeDescuento.U_SO1_DIASVENCI);
                    conn.AgregarParametro("pPoliticaReden", PromocionValeDescuento.U_SO1_POLITICAREDEN);
                    conn.AgregarParametro("pCode", PromocionValeDescuento.Code);
                    conn.EjecutarComando();
                }
                else
                {
                    conn.LimpiarParametros();
                    conn.CargarConsulta(ConsultasPromocion.RegistrarPromocionValeDescuento);
                    conn.AgregarParametro("pCodigo", Codigo);
                    conn.AgregarParametro("pNombre", Nombre);
                    conn.AgregarParametro("pPromo", U_SO1_PROMOCION);
                    conn.AgregarParametro("pPromoCadena", Codigo);
                    conn.AgregarParametro("pComportamiento", PromocionValeDescuento.U_SO1_COMPORTAMIENTO);
                    conn.AgregarParametro("pMoneda", PromocionValeDescuento.U_SO1_MONEDA);
                    conn.AgregarParametro("pMontoMinimo", PromocionValeDescuento.U_SO1_MONTOMINIMO);
                    conn.AgregarParametro("pTipoDescuento", PromocionValeDescuento.U_SO1_TIPODESCUENTO);
                    conn.AgregarParametro("pImporteDescuento", PromocionValeDescuento.U_SO1_IMPORTEDESC);
                    conn.AgregarParametro("pFechaVencimiento", Convert.ToDateTime(PromocionValeDescuento.U_SO1_FECHAVENCI));
                    conn.AgregarParametro("pDiasVencimiento", PromocionValeDescuento.U_SO1_DIASVENCI);
                    conn.AgregarParametro("pPoliticaReden", PromocionValeDescuento.U_SO1_POLITICAREDEN);
                    conn.EjecutarComando();
                }
            }
            catch (Exception oError)
            {
                Log.GuardarError("Descripción de error: ", oError);
                throw new Exception("PromocionAxB");
            }
            return;
        }

        public void ModificarPoliticaRedencion(Conexion conn)
        {
            try
            {
                Log.GuardarError("Asignamos la tabla de promocion politica de redencion", null);
                //Si el código está vacio es que no existe y tenemos que crearlo
                int horaIni = 0;
                int horaFin = 0;
                int horaLunIni = 0;
                int horaLunFin = 0;
                int horaMarIni = 0;
                int horaMarFin = 0;
                int horaMieIni = 0;
                int horaMieFin = 0;
                int horaJueIni = 0;
                int horaJueFin = 0;
                int horaVieIni = 0;
                int horaVieFin = 0;
                int horaSabIni = 0;
                int horaSabFin = 0;
                int horaDomIni = 0;
                int horaDomFin = 0;
                Int32.TryParse(U_SO1_DIAHORAINI, out horaIni);
                Int32.TryParse(U_SO1_DIAHORAFIN, out horaFin);
                Int32.TryParse(U_SO1_LUNHORAINI, out horaLunIni);
                Int32.TryParse(U_SO1_LUNHORAFIN, out horaLunFin);
                Int32.TryParse(U_SO1_MARHORAINI, out horaMarIni);
                Int32.TryParse(U_SO1_MARHORAFIN, out horaMarFin);
                Int32.TryParse(U_SO1_MIEHORAINI, out horaMieIni);
                Int32.TryParse(U_SO1_MIEHORAFIN, out horaMieFin);
                Int32.TryParse(U_SO1_JUEHORAINI, out horaJueIni);
                Int32.TryParse(U_SO1_JUEHORAFIN, out horaJueFin);
                Int32.TryParse(U_SO1_VIEHORAINI, out horaVieIni);
                Int32.TryParse(U_SO1_VIEHORAFIN, out horaVieFin);
                Int32.TryParse(U_SO1_SABHORAINI, out horaSabIni);
                Int32.TryParse(U_SO1_SABHORAFIN, out horaSabFin);
                Int32.TryParse(U_SO1_DOMHORAINI, out horaDomIni);
                Int32.TryParse(U_SO1_DOMHORAFIN, out horaDomFin);

                if (!string.IsNullOrEmpty(PromocionPoliticaRedencion.Code))
                {
                    //Se busca el código de la promoción
                    conn.LimpiarParametros();
                    conn.CargarConsulta(ConsultasPromocion.ModicarPoliticaRedencion);
                    conn.AgregarParametro("pNombre", Nombre);
                    conn.AgregarParametro("pTipo", PromocionPoliticaRedencion.U_SO1_TIPO);
                    conn.AgregarParametro("pDiario", PromocionPoliticaRedencion.U_SO1_DIARIO);
                    conn.AgregarParametro("pDiaCompleto", PromocionPoliticaRedencion.U_SO1_DIACOMPLETO);
                    conn.AgregarParametro("pDiaHoraIni", string.IsNullOrEmpty(PromocionPoliticaRedencion.U_SO1_DIAHORAINI) ? 0 : horaIni);
                    conn.AgregarParametro("pDiaHoraFin", string.IsNullOrEmpty(PromocionPoliticaRedencion.U_SO1_DIAHORAFIN) ? 0 : horaFin);
                    //Lunes
                    conn.AgregarParametro("pLunes", PromocionPoliticaRedencion.U_SO1_LUNES);
                    conn.AgregarParametro("pLunesCompleto", PromocionPoliticaRedencion.U_SO1_LUNCOMPLETO);
                    conn.AgregarParametro("pLunHoraIni", string.IsNullOrEmpty(PromocionPoliticaRedencion.U_SO1_LUNHORAINI) ? 0 : horaLunIni);
                    conn.AgregarParametro("pLunHoraFin", string.IsNullOrEmpty(PromocionPoliticaRedencion.U_SO1_LUNHORAFIN) ? 0 : horaLunFin);
                    //Martes
                    conn.AgregarParametro("pMartes", PromocionPoliticaRedencion.U_SO1_MARTES);
                    conn.AgregarParametro("pMartesCompleto", PromocionPoliticaRedencion.U_SO1_MARCOMPLETO);
                    conn.AgregarParametro("pMarHoraIni", string.IsNullOrEmpty(PromocionPoliticaRedencion.U_SO1_MARHORAINI) ? 0 : horaMarIni);
                    conn.AgregarParametro("pMarHoraFin", string.IsNullOrEmpty(PromocionPoliticaRedencion.U_SO1_MARHORAFIN) ? 0 : horaMarFin);
                    //Miércoles
                    conn.AgregarParametro("pMiercoles", PromocionPoliticaRedencion.U_SO1_MIERCOLES);
                    conn.AgregarParametro("pMieCompleto", PromocionPoliticaRedencion.U_SO1_MIECOMPLETO);
                    conn.AgregarParametro("pMieHoraIni", string.IsNullOrEmpty(PromocionPoliticaRedencion.U_SO1_MIEHORAINI) ? 0 : horaMieIni);
                    conn.AgregarParametro("pMieHoraFin", string.IsNullOrEmpty(PromocionPoliticaRedencion.U_SO1_MIEHORAFIN) ? 0 : horaMieFin);
                    //Jueves
                    conn.AgregarParametro("pJueves", PromocionPoliticaRedencion.U_SO1_JUEVES);
                    conn.AgregarParametro("pJueCompleto", PromocionPoliticaRedencion.U_SO1_JUECOMPLETO);
                    conn.AgregarParametro("pJueHoraIni", string.IsNullOrEmpty(PromocionPoliticaRedencion.U_SO1_JUEHORAINI) ? 0 : horaJueIni);
                    conn.AgregarParametro("pJueHoraFin", string.IsNullOrEmpty(PromocionPoliticaRedencion.U_SO1_JUEHORAFIN) ? 0 : horaJueFin);
                    //Viernes
                    conn.AgregarParametro("pViernes", PromocionPoliticaRedencion.U_SO1_VIERNES);
                    conn.AgregarParametro("pVieCompleto", PromocionPoliticaRedencion.U_SO1_VIECOMPLETO);
                    conn.AgregarParametro("pVieHoraIni", string.IsNullOrEmpty(PromocionPoliticaRedencion.U_SO1_VIEHORAINI) ? 0 : horaVieIni);
                    conn.AgregarParametro("pVieHoraFin", string.IsNullOrEmpty(PromocionPoliticaRedencion.U_SO1_VIEHORAFIN) ? 0 : horaVieFin);
                    //Sábado
                    conn.AgregarParametro("pSabado", PromocionPoliticaRedencion.U_SO1_SABADO);
                    conn.AgregarParametro("pSabCompleto", PromocionPoliticaRedencion.U_SO1_SABCOMPLETO);
                    conn.AgregarParametro("pSabHoraIni", string.IsNullOrEmpty(PromocionPoliticaRedencion.U_SO1_SABHORAINI) ? 0 : horaSabIni);
                    conn.AgregarParametro("pSabHoraFin", string.IsNullOrEmpty(PromocionPoliticaRedencion.U_SO1_SABHORAFIN) ? 0 : horaSabFin);
                    //Domingo
                    conn.AgregarParametro("pDomingo", PromocionPoliticaRedencion.U_SO1_DOMINGO);
                    conn.AgregarParametro("pDomCompleto", PromocionPoliticaRedencion.U_SO1_DOMCOMPLETO);
                    conn.AgregarParametro("pDomHoraIni", string.IsNullOrEmpty(PromocionPoliticaRedencion.U_SO1_DOMHORAINI) ? 0 : horaDomIni);
                    conn.AgregarParametro("pDomHoraFin", string.IsNullOrEmpty(PromocionPoliticaRedencion.U_SO1_DOMHORAFIN) ? 0 : horaDomFin);
                    //Checks de Articulo
                    conn.AgregarParametro("pArticuloEnlace", PromocionPoliticaRedencion.U_SO1_ARTICULOENLACE);
                    conn.AgregarParametro("pFiltrarArtArt", PromocionPoliticaRedencion.U_SO1_FILTRARARTART);
                    conn.AgregarParametro("pFiltrarArtGru", PromocionPoliticaRedencion.U_SO1_FILTRARARTGRU);
                    conn.AgregarParametro("pFiltrarArtPro", PromocionPoliticaRedencion.U_SO1_FILTRARARTPRO);
                    conn.AgregarParametro("pFiltrarArtProe", PromocionPoliticaRedencion.U_SO1_FILTRARARTPROE);
                    conn.AgregarParametro("pFiltrarArtProc", PromocionPoliticaRedencion.U_SO1_FILTRARARTPROC);
                    conn.AgregarParametro("pFiltrarArtCoe", PromocionPoliticaRedencion.U_SO1_FILTRARARTCOE);
                    conn.AgregarParametro("pFiltrarArtCoec", PromocionPoliticaRedencion.U_SO1_FILTRARARTCOEC);
                    conn.AgregarParametro("pFiltrarArtCoev", PromocionPoliticaRedencion.U_SO1_FILTRARARTCOEV);
                    conn.AgregarParametro("pFiltrarArtProv", PromocionPoliticaRedencion.U_SO1_FILTRARARTPROV);
                    conn.AgregarParametro("pFiltrarArtFab", PromocionPoliticaRedencion.U_SO1_FILTRARARTFAB);
                    conn.AgregarParametro("pFiltrarArtExc", PromocionPoliticaRedencion.U_SO1_FILTRARARTEXC);
                    conn.AgregarParametro("pFiltrarArtUne", PromocionPoliticaRedencion.U_SO1_FILTRARARTUNE);
                    //Checks de Promocion
                    conn.AgregarParametro("pFiltrarPromSuc", PromocionPoliticaRedencion.U_SO1_FILTRARPROMSUC);
                    conn.AgregarParametro("pFiltrarPromAli", PromocionPoliticaRedencion.U_SO1_FILTRARPROMALI);
                    conn.AgregarParametro("pFiltrarPromCli", PromocionPoliticaRedencion.U_SO1_FILTRARPROMCLI);
                    //Checks de Cliente
                    conn.AgregarParametro("pFiltrarCliCli", PromocionPoliticaRedencion.U_SO1_FILTRARCLICLI);
                    conn.AgregarParametro("pFiltrarCliEnls", PromocionPoliticaRedencion.U_SO1_FILTRARCLIENLS);
                    conn.AgregarParametro("pFiltrarCliGru", PromocionPoliticaRedencion.U_SO1_FILTRARCLIGRU);
                    conn.AgregarParametro("pFiltrarCliPro", PromocionPoliticaRedencion.U_SO1_FILTRARCLIPRO);
                    conn.AgregarParametro("pFiltrarCliProe", PromocionPoliticaRedencion.U_SO1_FILTRARCLIPROE);
                    conn.AgregarParametro("pFiltrarCliProc", PromocionPoliticaRedencion.U_SO1_FILTRARCLIPROC);
                    conn.AgregarParametro("pFiltrarCliCoe", PromocionPoliticaRedencion.U_SO1_FILTRARCLICOE);
                    conn.AgregarParametro("pFiltrarCliCoec", PromocionPoliticaRedencion.U_SO1_FILTRARCLICOEC);
                    conn.AgregarParametro("pFiltrarCliCoev", PromocionPoliticaRedencion.U_SO1_FILTRARCLICOEV);
                    //Checks de Promocion
                    conn.AgregarParametro("pFiltrarPromMem", PromocionPoliticaRedencion.U_SO1_FILTRARPROMMEM);
                    conn.AgregarParametro("pFiltrarPromLis", PromocionPoliticaRedencion.U_SO1_FILTRARPROMLIS);
                    conn.AgregarParametro("pFiltrarPromFop", PromocionPoliticaRedencion.U_SO1_FILTRARPROMFOP);
                    conn.AgregarParametro("pAcumPromoOtras", PromocionPoliticaRedencion.U_SO1_ACUMPROMOOTRAS);
                    conn.AgregarParametro("pCode", PromocionPoliticaRedencion.Code);
                    conn.EjecutarComando();
                }
                else
                {
                    conn.CargarConsulta(ConsultasPromocion.tipoPromocion);
                    DataTable tipo = conn.EjecutarTabla();
                    PromocionPoliticaRedencion.U_SO1_POLITICAREDEN = tipo.Rows[0].Valor<int>("tipo") + 1;

                    conn.LimpiarParametros();
                    conn.CargarConsulta(ConsultasPromocion.RegistrarPromocionPoliticaRedencion);
                    
                    conn.AgregarParametro("pCode", Codigo);
                    conn.AgregarParametro("pNombre", Nombre);                    
                    conn.AgregarParametro("pPolitRed", PromocionPoliticaRedencion.U_SO1_POLITICAREDEN);
                    conn.AgregarParametro("pTipo", PromocionPoliticaRedencion.U_SO1_TIPO);
                    conn.AgregarParametro("pDiario", PromocionPoliticaRedencion.U_SO1_DIARIO);
                    conn.AgregarParametro("pDiaCompleto", PromocionPoliticaRedencion.U_SO1_DIACOMPLETO);
                    conn.AgregarParametro("pDiaHoraIni", string.IsNullOrEmpty(PromocionPoliticaRedencion.U_SO1_DIAHORAINI) ? 0 : horaIni);
                    conn.AgregarParametro("pDiaHoraFin", string.IsNullOrEmpty(PromocionPoliticaRedencion.U_SO1_DIAHORAFIN) ? 0 : horaFin);
                    //Lunes
                    conn.AgregarParametro("pLunes", PromocionPoliticaRedencion.U_SO1_LUNES);
                    conn.AgregarParametro("pLunesCompleto", PromocionPoliticaRedencion.U_SO1_LUNCOMPLETO);
                    conn.AgregarParametro("pLunHoraIni", string.IsNullOrEmpty(PromocionPoliticaRedencion.U_SO1_LUNHORAINI) ? 0 : horaLunIni);
                    conn.AgregarParametro("pLunHoraFin", string.IsNullOrEmpty(PromocionPoliticaRedencion.U_SO1_LUNHORAFIN) ? 0 : horaLunFin);
                    //Martes
                    conn.AgregarParametro("pMartes", PromocionPoliticaRedencion.U_SO1_MARTES);
                    conn.AgregarParametro("pMartesCompleto", PromocionPoliticaRedencion.U_SO1_MARCOMPLETO);
                    conn.AgregarParametro("pMarHoraIni", string.IsNullOrEmpty(PromocionPoliticaRedencion.U_SO1_MARHORAINI) ? 0 : horaMarIni);
                    conn.AgregarParametro("pMarHoraFin", string.IsNullOrEmpty(PromocionPoliticaRedencion.U_SO1_MARHORAFIN) ? 0 : horaMarFin);
                    //Miércoles
                    conn.AgregarParametro("pMiercoles", PromocionPoliticaRedencion.U_SO1_MIERCOLES);
                    conn.AgregarParametro("pMieCompleto", PromocionPoliticaRedencion.U_SO1_MIECOMPLETO);
                    conn.AgregarParametro("pMieHoraIni", string.IsNullOrEmpty(PromocionPoliticaRedencion.U_SO1_MIEHORAINI) ? 0 : horaMieIni);
                    conn.AgregarParametro("pMieHoraFin", string.IsNullOrEmpty(PromocionPoliticaRedencion.U_SO1_MIEHORAFIN) ? 0 : horaMieFin);
                    //Jueves
                    conn.AgregarParametro("pJueves", PromocionPoliticaRedencion.U_SO1_JUEVES);
                    conn.AgregarParametro("pJueCompleto", PromocionPoliticaRedencion.U_SO1_JUECOMPLETO);
                    conn.AgregarParametro("pJueHoraIni", string.IsNullOrEmpty(PromocionPoliticaRedencion.U_SO1_JUEHORAINI) ? 0 : horaJueIni);
                    conn.AgregarParametro("pJueHoraFin", string.IsNullOrEmpty(PromocionPoliticaRedencion.U_SO1_JUEHORAFIN) ? 0 : horaJueFin);
                    //Viernes
                    conn.AgregarParametro("pViernes", PromocionPoliticaRedencion.U_SO1_VIERNES);
                    conn.AgregarParametro("pVieCompleto", PromocionPoliticaRedencion.U_SO1_VIECOMPLETO);
                    conn.AgregarParametro("pVieHoraIni", string.IsNullOrEmpty(PromocionPoliticaRedencion.U_SO1_VIEHORAINI) ? 0 : horaVieIni);
                    conn.AgregarParametro("pVieHoraFin", string.IsNullOrEmpty(PromocionPoliticaRedencion.U_SO1_VIEHORAFIN) ? 0 : horaVieFin);
                    //Sábado
                    conn.AgregarParametro("pSabado", PromocionPoliticaRedencion.U_SO1_SABADO);
                    conn.AgregarParametro("pSabCompleto", PromocionPoliticaRedencion.U_SO1_SABCOMPLETO);
                    conn.AgregarParametro("pSabHoraIni", string.IsNullOrEmpty(PromocionPoliticaRedencion.U_SO1_SABHORAINI) ? 0 : horaSabIni);
                    conn.AgregarParametro("pSabHoraFin", string.IsNullOrEmpty(PromocionPoliticaRedencion.U_SO1_SABHORAFIN) ? 0 : horaSabFin);
                    //Domingo
                    conn.AgregarParametro("pDomingo", PromocionPoliticaRedencion.U_SO1_DOMINGO);
                    conn.AgregarParametro("pDomCompleto", PromocionPoliticaRedencion.U_SO1_DOMCOMPLETO);
                    conn.AgregarParametro("pDomHoraIni", string.IsNullOrEmpty(PromocionPoliticaRedencion.U_SO1_DOMHORAINI) ? 0 : horaDomIni);
                    conn.AgregarParametro("pDomHoraFin", string.IsNullOrEmpty(PromocionPoliticaRedencion.U_SO1_DOMHORAFIN) ? 0 : horaDomFin);
                    //Checks de Articulo
                    conn.AgregarParametro("pArticuloEnlace", PromocionPoliticaRedencion.U_SO1_ARTICULOENLACE);
                    conn.AgregarParametro("pFiltrarArtArt", PromocionPoliticaRedencion.U_SO1_FILTRARARTART);
                    conn.AgregarParametro("pFiltrarArtGru", PromocionPoliticaRedencion.U_SO1_FILTRARARTGRU);
                    conn.AgregarParametro("pFiltrarArtPro", PromocionPoliticaRedencion.U_SO1_FILTRARARTPRO);
                    conn.AgregarParametro("pFiltrarArtProe", PromocionPoliticaRedencion.U_SO1_FILTRARARTPROE);
                    conn.AgregarParametro("pFiltrarArtProc", PromocionPoliticaRedencion.U_SO1_FILTRARARTPROC);
                    conn.AgregarParametro("pFiltrarArtCoe", PromocionPoliticaRedencion.U_SO1_FILTRARARTCOE);
                    conn.AgregarParametro("pFiltrarArtCoec", PromocionPoliticaRedencion.U_SO1_FILTRARARTCOEC);
                    conn.AgregarParametro("pFiltrarArtCoev", PromocionPoliticaRedencion.U_SO1_FILTRARARTCOEV);
                    conn.AgregarParametro("pFiltrarArtProv", PromocionPoliticaRedencion.U_SO1_FILTRARARTPROV);
                    conn.AgregarParametro("pFiltrarArtFab", PromocionPoliticaRedencion.U_SO1_FILTRARARTFAB);
                    conn.AgregarParametro("pFiltrarArtExc", PromocionPoliticaRedencion.U_SO1_FILTRARARTEXC);
                    conn.AgregarParametro("pFiltrarArtUne", PromocionPoliticaRedencion.U_SO1_FILTRARARTUNE);
                    //Checks de Promocion
                    conn.AgregarParametro("pFiltrarPromSuc", PromocionPoliticaRedencion.U_SO1_FILTRARPROMSUC);
                    conn.AgregarParametro("pFiltrarPromAli", PromocionPoliticaRedencion.U_SO1_FILTRARPROMALI);
                    conn.AgregarParametro("pFiltrarPromCli", PromocionPoliticaRedencion.U_SO1_FILTRARPROMCLI);
                    //Checks de Cliente
                    conn.AgregarParametro("pFiltrarCliCli", PromocionPoliticaRedencion.U_SO1_FILTRARCLICLI);
                    conn.AgregarParametro("pFiltrarCliEnls", PromocionPoliticaRedencion.U_SO1_FILTRARCLIENLS);
                    conn.AgregarParametro("pFiltrarCliGru", PromocionPoliticaRedencion.U_SO1_FILTRARCLIGRU);
                    conn.AgregarParametro("pFiltrarCliPro", PromocionPoliticaRedencion.U_SO1_FILTRARCLIPRO);
                    conn.AgregarParametro("pFiltrarCliProe", PromocionPoliticaRedencion.U_SO1_FILTRARCLIPROE);
                    conn.AgregarParametro("pFiltrarCliProc", PromocionPoliticaRedencion.U_SO1_FILTRARCLIPROC);
                    conn.AgregarParametro("pFiltrarCliCoe", PromocionPoliticaRedencion.U_SO1_FILTRARCLICOE);
                    conn.AgregarParametro("pFiltrarCliCoec", PromocionPoliticaRedencion.U_SO1_FILTRARCLICOEC);
                    conn.AgregarParametro("pFiltrarCliCoev", PromocionPoliticaRedencion.U_SO1_FILTRARCLICOEV);
                    //Checks de Promocion
                    conn.AgregarParametro("pFiltrarPromMem", PromocionPoliticaRedencion.U_SO1_FILTRARPROMMEM);
                    conn.AgregarParametro("pFiltrarPromLis", PromocionPoliticaRedencion.U_SO1_FILTRARPROMLIS);
                    conn.AgregarParametro("pFiltrarPromFop", PromocionPoliticaRedencion.U_SO1_FILTRARPROMFOP);                    
                    conn.AgregarParametro("pAcumPromoOtras", PromocionPoliticaRedencion.U_SO1_ACUMPROMOOTRAS);
                    conn.EjecutarComando();
                }
            }
            catch (Exception oError)
            {
                Log.GuardarError("Descripción de error: ", oError);
                throw new Exception("PromocionAxB");
            }
            return;
        }

        public void ModificarDescuentoAleatorio(Conexion conn, ReportesWeb.Utilidades.CodigoControl codigosControl)
        {

            #region Bitácora

            // AUTOR:               Juan Sol
            // FECHA:               19.11.2019
            // COMENTARIOS:         Se cambia la forma en el que se obtiene el nuevo código de control

            #endregion

            try
            {

                Log.GuardarError("Asignamos la tabla de descuento aleatorio", null);
                //Si el código está vacio es que no existe y tenemos que crearlo
                if (!string.IsNullOrEmpty(PromocionDescAle.Codigo))
                {
                    //Se busca el código de la promoción
                    conn.LimpiarParametros();
                    conn.CargarConsulta(ConsultasPromocion.ModificarPromocionDescuentoAleatorio);
                    conn.AgregarParametro("pActMontoMaximo", PromocionDescAle.U_SO1_ACTMONTOMAXIMO);
                    conn.AgregarParametro("pMontoMaxiPromo", PromocionDescAle.U_SO1_MONTOMAXIPROMO);
                    conn.AgregarParametro("pActMontoMaxDia", PromocionDescAle.U_SO1_ACTMONTOMAXDIA);
                    conn.AgregarParametro("pMontoMaxDia", PromocionDescAle.U_SO1_MONTOMAXPORDIA);
                    conn.AgregarParametro("pActImpVenDesde", PromocionDescAle.U_SO1_ACTIMPVENDESDE);
                    conn.AgregarParametro("pImportVenDesde", PromocionDescAle.U_SO1_IMPORTVENDESDE);
                    conn.AgregarParametro("pActImpVenHasta", PromocionDescAle.U_SO1_ACTIMPVENHASTA);
                    conn.AgregarParametro("pImportVenHasta", PromocionDescAle.U_SO1_IMPORTVENHASTA);
                    conn.AgregarParametro("pActImporteImp", PromocionDescAle.U_SO1_ACTIMPORTEIMP);
                    conn.AgregarParametro("pCompoTrasProm", PromocionDescAle.U_SO1_COMPOTRASPROM);
                    conn.AgregarParametro("pActFrecuenProm", PromocionDescAle.U_SO1_ACTFRECUENPROM);
                    conn.AgregarParametro("pMinFrecPromo", PromocionDescAle.U_SO1_MINFRECPROMO);
                    conn.AgregarParametro("pCode", PromocionDescAle.Codigo);
                    conn.EjecutarComando();
                }
                else
                {
                    conn.LimpiarParametros();
                    conn.CargarConsulta(ConsultasPromocion.RegistrarPromocionDescuentoAleatorio);
                    conn.AgregarParametro("pCodigo", Codigo);
                    conn.AgregarParametro("pNombre", Nombre);
                    conn.AgregarParametro("pPromo", U_SO1_PROMOCION);
                    conn.AgregarParametro("pPromoCadena", Codigo);
                    conn.AgregarParametro("pActMontoMaximo", PromocionDescAle.U_SO1_ACTMONTOMAXIMO);
                    conn.AgregarParametro("pMontoMaxiPromo", PromocionDescAle.U_SO1_MONTOMAXIPROMO);
                    conn.AgregarParametro("pActMontoMaxDia", PromocionDescAle.U_SO1_ACTMONTOMAXDIA);
                    conn.AgregarParametro("pMontoMaxDia", PromocionDescAle.U_SO1_MONTOMAXPORDIA);
                    conn.AgregarParametro("pActImpVenDesde", PromocionDescAle.U_SO1_ACTIMPVENDESDE);
                    conn.AgregarParametro("pImportVenDesde", PromocionDescAle.U_SO1_IMPORTVENDESDE);
                    conn.AgregarParametro("pActImpVenHasta", PromocionDescAle.U_SO1_ACTIMPVENHASTA);
                    conn.AgregarParametro("pImportVenHasta", PromocionDescAle.U_SO1_IMPORTVENHASTA);
                    conn.AgregarParametro("pActImporteImp", PromocionDescAle.U_SO1_ACTIMPORTEIMP);
                    conn.AgregarParametro("pCompoTrasProm", PromocionDescAle.U_SO1_COMPOTRASPROM);
                    conn.AgregarParametro("pActFrecuenProm", PromocionDescAle.U_SO1_ACTFRECUENPROM);
                    conn.AgregarParametro("pMinFrecPromo", PromocionDescAle.U_SO1_MINFRECPROMO);
                    conn.EjecutarComando();
                }

                #region Actualizar el catálogo de horas

                conn.LimpiarParametros();
                conn.CargarConsulta(ConsultasPromocion.ConsultarCatalogoHoras);
                conn.AgregarParametro("pPromocion", U_SO1_PROMOCION);
                DataTable dtCatalogo = conn.EjecutarTabla();
                foreach (DataRow fila in dtCatalogo.Rows)
                {
                    if (!PromocionDescAle._catalogosHoras.ContainsKey(fila.Valor<string>("CodigoCatalogo")))
                    {

                        conn.LimpiarParametros();
                        conn.CargarConsulta(ConsultasPromocion.EliminarRegistro);
                        conn.AgregarLiteral("TABLA", "@SO1_01PROMCATHORAR");
                        conn.AgregarParametro("pCode", fila.Valor<string>("Codigo"));
                        conn.EjecutarComando();
                    }
                    else
                    {
                        PromocionDescAle._catalogosHoras.Remove(fila.Valor<string>("CodigoCatalogo"));
                    }
                }

                //En el diccionario sólo quedan los nuevos valores
                foreach (KeyValuePair<string, MPromoCupon> horario in PromocionDescAle._catalogosHoras)
                {
                    conn.LimpiarParametros();
                    conn.CargarConsulta(ConsultasPromocion.RegistrarCatalogo);
                    string nuevoCodigo = String.Empty;
                    nuevoCodigo = codigosControl.Proximo(TablaCodigoControl.PROMCATHORAR);
                    conn.AgregarParametro("pCodigo", nuevoCodigo);
                    conn.AgregarParametro("pNombre", nuevoCodigo);
                    conn.AgregarParametro("pPromo", U_SO1_PROMOCION);
                    conn.AgregarParametro("pPromoCadena", Codigo);
                    conn.AgregarParametro("pCodigoHorario", horario.Value.U_SO1_CODIGCATEHORAR);
                    conn.EjecutarComando();
                }

                #endregion

                #region Actualizar los porcentajes

                DataTable dtDescuentos = null;
                conn.LimpiarParametros();
                conn.CargarConsulta(ConsultasPromocion.ConsultarCodigosCatDescuentos);
                conn.AgregarParametro("pPromocion", U_SO1_PROMOCION);
                dtDescuentos = conn.EjecutarTabla();

                Log.GuardarError("Asignamos la tabla de Monto de descuento", null);

                foreach (DataRow fila in dtDescuentos.Rows)
                {
                    //Si no se encuentra en el diccionario significa que fué eliminado
                    //if (!PromocionDescEsc._descuentos.Contains(fila.Valor<string>("Articulo")))
                    bool existe = false;

                    for (int i = 0; i < PromocionDescAle._porcentajes.Count(); i++)
                    {
                        if (PromocionDescAle._porcentajes[i].Code == fila.Valor<string>("Codigo"))
                        {
                            //Actualizamos el registro con los nuevos valores
                            existe = true;
                            conn.LimpiarParametros();
                            conn.CargarConsulta(ConsultasPromocion.ModificarCatalogoPorcentaje);
                            conn.AgregarParametro("pNumeroLinea", PromocionDescAle._porcentajes[i].U_SO1_NUMEROLINEA);
                            conn.AgregarParametro("pPorcentaje", PromocionDescAle._porcentajes[i].U_SO1_PORCENTAJE);
                            conn.AgregarParametro("pCode", PromocionDescAle._porcentajes[i].Code);
                            conn.EjecutarComando();
                            PromocionDescAle._porcentajes.Remove(PromocionDescAle._porcentajes[i]);
                        }
                    }
                    if (!existe)
                    {
                        //Eliminamos el registro, ya que fué eliminado de la nueva lista
                        conn.LimpiarParametros();
                        conn.CargarConsulta(ConsultasPromocion.EliminarRegistro);
                        conn.AgregarLiteral("TABLA", "@SO1_01CATPORPDESCA");
                        conn.AgregarParametro("pCode", fila.Valor<string>("Codigo"));
                        conn.EjecutarComando();

                    }
                }

                // Recorremos la lista para para crear los nuevos registros

                for (int i = 0; i < PromocionDescAle._porcentajes.Count(); i++)
                {
                    string nuevoCodigo = String.Empty;
                    nuevoCodigo = codigosControl.Proximo(TablaCodigoControl.CATPORPDESCA);
                    conn.LimpiarParametros();
                    conn.CargarConsulta(ConsultasPromocion.RegistrarCatalogoPorcentaje);
                    conn.AgregarParametro("pCodigo", nuevoCodigo);
                    conn.AgregarParametro("pNombre", nuevoCodigo);
                    conn.AgregarParametro("pPromo", U_SO1_PROMOCION);
                    conn.AgregarParametro("pPromoCadena", Codigo);
                    conn.AgregarParametro("pNumeroLinea", PromocionDescAle._porcentajes[i].U_SO1_NUMEROLINEA);
                    conn.AgregarParametro("pPorcentaje", PromocionDescAle._porcentajes[i].U_SO1_PORCENTAJE);
                    conn.EjecutarComando();
                }

                #endregion

            }
            catch (Exception oError)
            {
                Log.GuardarError("Descripción de error: ", oError);
                throw new Exception("PromocionAxB");
            }
            return;
        }

        #endregion

        #region Cargas Genericas

        public void CargarSucursales(Conexion conn, MPromocion promocion)
        {
            conn.LimpiarParametros();
            conn.CargarConsulta(ConsultasPromocion.ConsultarSucursales);
            conn.AgregarParametro("pPromocion", promocion.U_SO1_PROMOCION);
            DataTable dtSucursalPromo = conn.EjecutarTabla();
            foreach (DataRow fila in dtSucursalPromo.Rows)
            {
                Modelo.MPromoSucursal sucursal = new Modelo.MPromoSucursal();
                sucursal.Code = fila.Valor<string>("Codigo");
                sucursal.Name = fila.Valor<string>("Nombre");
                sucursal.U_SO1_PROMOCION = fila.Valor<string>("Promo");
                sucursal.U_SO1_PROMOCADENA = fila.Valor<string>("PromoCadena");
                sucursal.U_SO1_SUCURSAL = fila.Valor<string>("CodigoSucursal");
                sucursal.U_SO1_ACTIVO = fila.Valor<string>("Activo");
                sucursal.Nombre = fila.Valor<string>("NombreSucursal");
                sucursal.ListaPrecio1 = fila.Valor<string>("ListaPrecio1");
                sucursal.ListaPrecio2 = fila.Valor<string>("ListaPrecio2");
                Agregar(sucursal);
            }
        }

        public void CargarArticulos(Conexion conn, MPromocion promocion)
        {
            if (promocion.U_SO1_TIPO != "KV")
            {

                conn.LimpiarParametros();

                if (promocion.U_SO1_TIPO == "DX")
                {                    
                    conn.CargarConsulta(ConsultasPromocion.ConsultarSucursalesActivas);
                    conn.AgregarParametro("pPromocion", promocion.U_SO1_PROMOCION);
                    DataTable dtSucursalPromo = conn.EjecutarTabla();
                    conn.LimpiarParametros();
                    if (dtSucursalPromo.Rows.Count > 0)
                    {
                        int listaprecio1 = 0;
                        int listaprecio2 = 0;
                        string sListaprecio1 = (dtSucursalPromo.Rows[0]["ListaPrecio1"]).ToString();
                        string slistaprecio2 = (dtSucursalPromo.Rows[0]["ListaPrecio2"]).ToString();
                        Int32.TryParse(sListaprecio1, out listaprecio1);
                        Int32.TryParse(slistaprecio2, out listaprecio2);
                        conn.CargarConsulta(ConsultasPromocion.ConsultarArticulosLPPromo);
                        conn.AgregarParametro("pListaPrecio", listaprecio1);
                        conn.AgregarParametro("pListaPrecio2", listaprecio2);
                    }
                    else
                    {
                        conn.CargarConsulta(ConsultasPromocion.ConsultarArticulosPromo);
                    }
                }
                else
                {
                    conn.CargarConsulta(ConsultasPromocion.ConsultarArticulosPromo);
                }

                conn.AgregarParametro("pPromocion", promocion.U_SO1_PROMOCION);
                conn.AgregarParametro("pTipo", "G");
                DataTable dtArticuloPromo = conn.EjecutarTabla();
                foreach (DataRow fila in dtArticuloPromo.Rows)
                {
                    Modelo.MPromoArticulo articulo = new Modelo.MPromoArticulo();
                    articulo.Code = fila.Valor<string>("Codigo");
                    articulo.Name = fila.Valor<string>("Nombre");
                    articulo.U_SO1_PROMOCION = fila.Valor<int>("Promo");
                    articulo.U_SO1_PROMOCADENA = fila.Valor<string>("PromoCadena");
                    articulo.U_SO1_ARTICULO = fila.Valor<string>("Articulo");
                    articulo.U_SO1_TIPO = fila.Valor<string>("Tipo"); ;
                    articulo.U_SO1_IMPORTE = fila.Valor<decimal>("Importe");
                    articulo.U_SO1_CODIGOAMBITO = fila.Valor<string>("CodigoAmbito");
                    articulo.ListaPrecio1 = fila.Valor<string>("ListaPrecio");
                    articulo.Precio1 = fila.Valor<string>("Precio");
                    articulo.ListaPrecio2 = fila.Valor<string>("ListaPrecio2");
                    articulo.Precio2 = fila.Valor<string>("Precio2");
                    //articulo.PrecioAcuerdo = fila.Valor<decimal>("Importe");
                    articulo.Nombre = fila.Valor<string>("NombreArticulo");
                    Agregar(articulo, "G");
                }
            }
        }

        public void CargarExcepciones(Conexion conn, MPromocion promocion)
        {
            conn.LimpiarParametros();
            conn.CargarConsulta(ConsultasPromocion.ConsultarArticulosPromo);
            conn.AgregarParametro("pPromocion", promocion.U_SO1_PROMOCION);
            conn.AgregarParametro("pTipo", "E");
            DataTable dtArticuloPromo = conn.EjecutarTabla();
            foreach (DataRow fila in dtArticuloPromo.Rows)
            {
                Modelo.MPromoArticulo articulo = new Modelo.MPromoArticulo();
                articulo.Code = fila.Valor<string>("Codigo");
                articulo.Name = fila.Valor<string>("Nombre");
                articulo.U_SO1_PROMOCION = fila.Valor<int>("Promo");
                articulo.U_SO1_PROMOCADENA = fila.Valor<string>("PromoCadena");
                articulo.U_SO1_ARTICULO = fila.Valor<string>("Articulo");
                articulo.U_SO1_TIPO = fila.Valor<string>("Tipo"); ;
                articulo.U_SO1_IMPORTE = fila.Valor<decimal>("Importe");
                articulo.U_SO1_CODIGOAMBITO = fila.Valor<string>("CodigoAmbito");
                articulo.Nombre = fila.Valor<string>("NombreArticulo");
                Agregar(articulo, "E");
            }
        }

        public void CargarFabricantes(Conexion conn, MPromocion promocion)
        {
            conn.LimpiarParametros();
            conn.CargarConsulta(ConsultasPromocion.ConsultarFabricantes);
            conn.AgregarParametro("pPromocion", promocion.U_SO1_PROMOCION);
            DataTable dtFabricantePromo = conn.EjecutarTabla();
            foreach (DataRow fila in dtFabricantePromo.Rows)
            {
                Modelo.MPromoArticuloFabricante fabricante = new Modelo.MPromoArticuloFabricante();
                fabricante.Code = fila.Valor<string>("Codigo");
                fabricante.Name = fila.Valor<string>("Nombre");
                fabricante.U_SO1_PROMOCION = fila.Valor<int>("Promo");
                fabricante.U_SO1_PROMOCADENA = fila.Valor<string>("PromoCadena");
                fabricante.U_SO1_FABRICANTE = fila.Valor<string>("CodigoFabricante");
                fabricante.U_SO1_ACTIVO = fila.Valor<string>("Activo");
                fabricante.Nombre = fila.Valor<string>("NombreFabricante");
                Agregar(fabricante);
            }
        }

        public void CargarProveedores(Conexion conn, MPromocion promocion)
        {
            conn.LimpiarParametros();
            conn.CargarConsulta(ConsultasPromocion.ConsultarProveedor);
            conn.AgregarParametro("pPromocion", promocion.U_SO1_PROMOCION);
            DataTable dtProveedorPromo = conn.EjecutarTabla();
            foreach (DataRow fila in dtProveedorPromo.Rows)
            {
                Modelo.MPromoArticuloProveedor proveedor = new Modelo.MPromoArticuloProveedor();
                proveedor.Code = fila.Valor<string>("Codigo");
                proveedor.Name = fila.Valor<string>("Nombre");
                proveedor.U_SO1_PROMOCION = fila.Valor<int>("Promo");
                proveedor.U_SO1_PROMOCADENA = fila.Valor<string>("PromoCadena");
                proveedor.U_SO1_PROVEEDOR = fila.Valor<string>("Proveedor");
                proveedor.Nombre = fila.Valor<string>("NombreProveedor");
                Agregar(proveedor);
            }
        }

        public void CargarClientes(Conexion conn, MPromocion promocion)
        {
            conn.LimpiarParametros();
            conn.CargarConsulta(ConsultasPromocion.ConsultarClientes);
            conn.AgregarParametro("pPromocion", promocion.U_SO1_PROMOCION);
            DataTable dtClientePromo = conn.EjecutarTabla();
            foreach (DataRow fila in dtClientePromo.Rows)
            {
                Modelo.MPromoCliente cliente = new Modelo.MPromoCliente();
                cliente.Code = fila.Valor<string>("Codigo");
                cliente.Name = fila.Valor<string>("Nombre");
                cliente.U_SO1_PROMOCION = fila.Valor<int>("Promo");
                cliente.U_SO1_PROMOCADENA = fila.Valor<string>("PromoCadena");
                cliente.U_SO1_CLIENTE = fila.Valor<string>("CodigoCliente"); ;
                cliente.Nombre = fila.Valor<string>("NombreCliente");
                Agregar(cliente);
            }
        }

        public void CargarAlianzas(Conexion conn, MPromocion promocion)
        {
            conn.LimpiarParametros();
            conn.CargarConsulta(ConsultasPromocion.ConsultarAlianzas);
            conn.AgregarParametro("pPromocion", promocion.U_SO1_PROMOCION);
            DataTable dtAlianzaPromo = conn.EjecutarTabla();

            foreach (DataRow fila in dtAlianzaPromo.Rows)
            {
                Modelo.MPromoAlianza alianza = new Modelo.MPromoAlianza();
                alianza.Code = fila.Valor<string>("Codigo");
                alianza.Name = fila.Valor<string>("Nombre");
                alianza.U_SO1_PROMOCION = fila.Valor<string>("Promocion");
                alianza.U_SO1_PROMOCADENA = fila.Valor<string>("PromoCadena");
                alianza.U_SO1_ALIANZA = fila.Valor<string>("Alianza");
                alianza.U_SO1_NOMBRE = fila.Valor<string>("NombreAlianza");
                alianza.U_SO1_ACTIVO = fila.Valor<string>("Activo");
                Agregar(alianza);
            }
        }

        public void CargarMembresias(Conexion conn, MPromocion promocion)
        {
            conn.LimpiarParametros();
            conn.CargarConsulta(ConsultasPromocion.ConsultarMembresias);
            conn.AgregarParametro("pPromocion", promocion.U_SO1_PROMOCION);
            DataTable dtMembresiaPromo = conn.EjecutarTabla();

            foreach (DataRow fila in dtMembresiaPromo.Rows)
            {
                Modelo.MPromoMembresia membresia = new Modelo.MPromoMembresia();
                membresia.Code = fila.Valor<string>("Codigo");
                membresia.Name = fila.Valor<string>("Nombre");
                membresia.U_SO1_PROMOCION = fila.Valor<string>("Promo");
                membresia.U_SO1_PROMOCADENA = fila.Valor<string>("PromoCadena");
                membresia.U_SO1_TIPOMEMBRESIA = fila.Valor<string>("CodigoTarjeta");
                membresia.U_SO1_NOMBREMEMBRESIA = fila.Valor<string>("NombreTarjeta");
                membresia.U_SO1_ACTIVO = fila.Valor<string>("Activo");
                Agregar(membresia);
            }
        }

        public void CargarPrecios(Conexion conn, MPromocion promocion)
        {
            conn.LimpiarParametros();
            conn.CargarConsulta(ConsultasPromocion.ConsultarPrecios);
            conn.AgregarParametro("pPromocion", promocion.U_SO1_PROMOCION);
            DataTable dtPrecioPromo = conn.EjecutarTabla();

            foreach (DataRow fila in dtPrecioPromo.Rows)
            {
                Modelo.MPromoPrecios precio = new Modelo.MPromoPrecios();
                precio.Code = fila.Valor<string>("Codigo");
                precio.Name = fila.Valor<string>("Nombre");
                precio.U_SO1_PROMOCION = fila.Valor<string>("Promo");
                precio.U_SO1_PROMOCADENA = fila.Valor<string>("PromoCadena");
                precio.U_SO1_LISTAPRECIO = fila.Valor<string>("CodigoPrecio");
                precio.U_SO1_ACTIVO = fila.Valor<string>("Activo");
                precio.ListName = fila.Valor<string>("NombrePrecio");
                Agregar(precio);
            }
        }

        public void CargarFormasPago(Conexion conn, MPromocion promocion)
        {
            conn.LimpiarParametros();
            conn.CargarConsulta(ConsultasPromocion.ConsultarFormasPago);
            conn.AgregarParametro("pPromocion", promocion.U_SO1_PROMOCION);
            DataTable dtFormaPagoPromo = conn.EjecutarTabla();

            foreach (DataRow fila in dtFormaPagoPromo.Rows)
            {
                Modelo.MPromoFPago formaPago = new Modelo.MPromoFPago();
                formaPago.Code = fila.Valor<string>("Codigo");
                formaPago.Name = fila.Valor<string>("Nombre");
                formaPago.U_SO1_PROMOCION = fila.Valor<string>("Promo");
                formaPago.U_SO1_PROMOCADENA = fila.Valor<string>("PromoCadena");
                formaPago.U_SO1_FORMAPAGO = fila.Valor<string>("CodigoPago");
                formaPago.U_SO1_ACTIVO = fila.Valor<string>("Activo");
                formaPago.U_SO1_MINIMO = fila.Valor<decimal>("Minimo");
                formaPago.U_SO1_TIPO = fila.Valor<string>("Tipo");
                formaPago.Nombre = fila.Valor<string>("NombrePago");
                Agregar(formaPago);
            }
        }

        public void CargarGruposCliente(Conexion conn, MPromocion promocion)
        {
            conn.LimpiarParametros();
            conn.CargarConsulta(ConsultasPromocion.ConsultarGruposClientes);
            conn.AgregarParametro("pPromocion", promocion.U_SO1_PROMOCION);
            DataTable dtGrupoClientePromo = conn.EjecutarTabla();

            foreach (DataRow fila in dtGrupoClientePromo.Rows)
            {
                Modelo.MPromoGrupo grupo = new Modelo.MPromoGrupo();
                grupo.Code = fila.Valor<string>("Codigo");
                grupo.Name = fila.Valor<string>("Nombre");
                grupo.U_SO1_PROMOCION = fila.Valor<string>("Promo");
                grupo.U_SO1_PROMOCADENA = fila.Valor<string>("PromoCadena");
                grupo.U_SO1_GRUPO = fila.Valor<string>("CodigoGrupo");
                grupo.U_SO1_ACTIVO = fila.Valor<string>("Activo");
                grupo.Nombre = fila.Valor<string>("NombreGrupo");
                Agregar(grupo, "Cliente");
            }
        }

        public void CargarPropiedadesCliente(Conexion conn, MPromocion promocion)
        {
            conn.LimpiarParametros();
            conn.CargarConsulta(ConsultasPromocion.ConsultarPropiedadesClientes);
            conn.AgregarParametro("pPromocion", promocion.U_SO1_PROMOCION);
            DataTable dtPropiedadClientePromo = conn.EjecutarTabla();

            foreach (DataRow fila in dtPropiedadClientePromo.Rows)
            {
                Modelo.MPromoPropiedad propiedad = new Modelo.MPromoPropiedad();
                propiedad.Code = fila.Valor<string>("Codigo");
                propiedad.Name = fila.Valor<string>("Nombre");
                propiedad.U_SO1_PROMOCION = fila.Valor<string>("Promo");
                propiedad.U_SO1_PROMOCADENA = fila.Valor<string>("PromoCadena");
                propiedad.U_SO1_PROPIEDAD = fila.Valor<string>("CodigoPropiedad");
                propiedad.U_SO1_ACTIVO = fila.Valor<string>("Activo");
                propiedad.Nombre = fila.Valor<string>("NombrePropiedad");
                Agregar(propiedad, "Cliente");
            }
        }

        public void CargarGruposArticulo(Conexion conn, MPromocion promocion)
        {
            conn.LimpiarParametros();
            conn.CargarConsulta(ConsultasPromocion.ConsultarGruposArticulos);
            conn.AgregarParametro("pPromocion", promocion.U_SO1_PROMOCION);
            DataTable dtGrupoArticuloPromo = conn.EjecutarTabla();

            foreach (DataRow fila in dtGrupoArticuloPromo.Rows)
            {
                Modelo.MPromoGrupo grupo = new Modelo.MPromoGrupo();
                grupo.Code = fila.Valor<string>("Codigo");
                grupo.Name = fila.Valor<string>("Nombre");
                grupo.U_SO1_PROMOCION = fila.Valor<string>("Promo");
                grupo.U_SO1_PROMOCADENA = fila.Valor<string>("PromoCadena");
                grupo.U_SO1_GRUPO = fila.Valor<string>("CodigoGrupo");
                grupo.U_SO1_ACTIVO = fila.Valor<string>("Activo");
                grupo.Nombre = fila.Valor<string>("NombreGrupo");
                Agregar(grupo, "Articulo");
            }
        }

        public void CargarPropiedadesArticulo(Conexion conn, MPromocion promocion)
        {
            conn.LimpiarParametros();
            conn.CargarConsulta(ConsultasPromocion.ConsultarPropiedadesArticulos);
            conn.AgregarParametro("pPromocion", promocion.U_SO1_PROMOCION);
            DataTable dtPropiedadArticuloPromo = conn.EjecutarTabla();

            foreach (DataRow fila in dtPropiedadArticuloPromo.Rows)
            {
                Modelo.MPromoPropiedad propiedad = new Modelo.MPromoPropiedad();
                propiedad.Code = string.IsNullOrEmpty(fila.Valor<string>("Codigo")) ? string.Empty : fila.Valor<string>("Codigo");
                propiedad.Name = fila.Valor<string>("Nombre");
                propiedad.U_SO1_PROMOCION = fila.Valor<string>("Promo");
                propiedad.U_SO1_PROMOCADENA = fila.Valor<string>("PromoCadena");
                propiedad.U_SO1_PROPIEDAD = fila.Valor<string>("CodigoPropiedad");
                propiedad.U_SO1_ACTIVO = fila.Valor<string>("Activo");
                propiedad.Nombre = fila.Valor<string>("NombrePropiedad");
                Agregar(propiedad, "Articulo");
            }
        }

        public void CargarUnidadesMedidaArticulo(Conexion conn, MPromocion promocion)
        {
            conn.LimpiarParametros();
            conn.CargarConsulta(ConsultasPromocion.ConsultarMedidasArticulos);
            conn.AgregarParametro("pPromocion", promocion.U_SO1_PROMOCION);
            DataTable dtPropiedadArticuloPromo = conn.EjecutarTabla();

            foreach (DataRow fila in dtPropiedadArticuloPromo.Rows)
            {
                Modelo.MPromoArticuloMedida propiedad = new Modelo.MPromoArticuloMedida();
                propiedad.Code = string.IsNullOrEmpty(fila.Valor<string>("Codigo")) ? string.Empty : fila.Valor<string>("Codigo");
                propiedad.Name = fila.Valor<string>("Nombre");
                propiedad.U_SO1_PROMOCION = fila.Valor<string>("Promo");
                propiedad.U_SO1_PROMOCADENA = fila.Valor<string>("PromoCadena");
                propiedad.U_SO1_GRUPO = fila.Valor<string>("CodigoGrupo");
                propiedad.U_SO1_UNIDAD = fila.Valor<string>("CodigoUnidad");
                propiedad.U_SO1_ACTIVO = fila.Valor<string>("Activo");
                propiedad.NombreGrupo = fila.Valor<string>("NombreGrupo");
                propiedad.NombreUnidad = fila.Valor<string>("NombreUnidad");
                propiedad.Cantidad = fila.Valor<string>("Cantidad");
                Agregar(propiedad);
            }

        }

        public void CargarArticulosIdentificadores(Conexion conn, MPromoIdentificador identificador)
        {
            conn.LimpiarParametros();
            conn.CargarConsulta(ConsultasPromocion.ConsultarIdentificadorArticulo); //Aqui va una nueva query para cargar los articulos de este identificador
            conn.AgregarParametro("pTipo", identificador.U_SO1_CODIGOAMBITO);
            conn.AgregarParametro("pPromocion", identificador.U_SO1_PROMOCION);
            DataTable dtArticuloPromo = conn.EjecutarTabla();
            foreach (DataRow fila in dtArticuloPromo.Rows)
            {
                Modelo.MPromoArticulo articulo = new Modelo.MPromoArticulo();
                articulo.Code = fila.Valor<string>("Codigo");
                articulo.Name = fila.Valor<string>("Nombre");
                articulo.U_SO1_PROMOCION = fila.Valor<int>("Promo");
                articulo.U_SO1_PROMOCADENA = fila.Valor<string>("PromoCadena");
                articulo.U_SO1_ARTICULO = fila.Valor<string>("Articulo");
                articulo.U_SO1_TIPO = fila.Valor<string>("Tipo"); ;
                articulo.U_SO1_IMPORTE = fila.Valor<decimal>("Importe");
                articulo.U_SO1_CODIGOAMBITO = fila.Valor<string>("CodigoAmbito");
                articulo.Nombre = fila.Valor<string>("NombreArticulo");
                identificador.Agregar(articulo);
            }
        }

        public void CargarIdentificadores(Conexion conn, MPromocion promocion)
        {
            conn.LimpiarParametros();
            conn.CargarConsulta(ConsultasPromocion.ConsultaIdentificador);
            conn.AgregarParametro("pPromocion", promocion.U_SO1_PROMOCION);
            DataTable dtIdentificadorPromo = conn.EjecutarTabla();

            foreach (DataRow fila in dtIdentificadorPromo.Rows)
            {
                Modelo.MPromoIdentificador propiedad = new Modelo.MPromoIdentificador();
                propiedad.Code = string.IsNullOrEmpty(fila.Valor<string>("Codigo")) ? string.Empty : fila.Valor<string>("Codigo");
                propiedad.Name = fila.Valor<string>("Nombre");
                propiedad.U_SO1_PROMOCION = fila.Valor<string>("Promo");
                propiedad.U_SO1_PROMOCADENA = fila.Valor<string>("PromoCadena");
                propiedad.U_SO1_CODIGOAMBITO =fila.Valor<int>("CodigoAmbito");
                propiedad.U_SO1_NOMBREAMBITO = fila.Valor<string>("NombreAmbito");
                propiedad.U_SO1_CANTIDAD = fila.Valor<int>("Cantidad").ToString();
                propiedad.U_SO1_IMAGEN = fila.Valor<string>("Imagen");
                propiedad.U_SO1_MANTENERPROMO = fila.Valor<string>("MantenerPromo");
                propiedad.U_SO1_AMBOBLIGATORIO = fila.Valor<string>("AmbitObligatorio");
                CargarArticulosIdentificadores(conn, propiedad);
                PromocionKitVenta.Agregar(propiedad);
            }
        }

        #endregion

        #region Cargas Promociones

        public void CargarPromocionAxB(Conexion conn, MPromocion promocion)
        {
            conn.LimpiarParametros();
            conn.CargarConsulta(ConsultasPromocion.ConsultarPromocionAxB);
            conn.AgregarParametro("pPromocion", promocion.U_SO1_PROMOCION);
            conn.EjecutarLector();
            if (conn.LeerFila())
            {
                Modelo.MPromoAxB datos = new Modelo.MPromoAxB();
                datos.Code = conn.DatoLector<string>("Code");
                datos.Name = conn.DatoLector<string>("Name");
                datos.U_SO1_PROMOCION = conn.DatoLector<string>("U_SO1_PROMOCION");
                datos.U_SO1_PROMOCADENA = conn.DatoLector<string>("U_SO1_PROMOCADENA");
                datos.U_SO1_PIEZASTOTALES = conn.DatoLector<decimal>("U_SO1_PIEZASTOTALES");
                datos.U_SO1_PIEZASAPAGAR = conn.DatoLector<decimal>("U_SO1_PIEZASAPAGAR");
                datos.U_SO1_REGALOPRECIO1 = conn.DatoLector<string>("U_SO1_REGALOPRECIO1");
                datos.U_SO1_MANTENERDESC = conn.DatoLector<string>("U_SO1_MANTENERDESC");
                promocion.PromocionAxB = datos;
            }
            else
            {
                Modelo.MPromoAxB datos = new Modelo.MPromoAxB();
                promocion.PromocionAxB = datos;
            }
            conn.CerrarLector();
        }

        public void CargarPromocionDescuentoEmpleado(Conexion conn, MPromocion promocion)
        {
            conn.LimpiarParametros();
            conn.CargarConsulta(ConsultasPromocion.ConsultarPromocionDescEmp);
            conn.AgregarParametro("pPromocion", promocion.U_SO1_PROMOCION);
            conn.EjecutarLector();
            if (conn.LeerFila())
            {
                Modelo.MPromDescEmp datos = new Modelo.MPromDescEmp();
                datos.Code = conn.DatoLector<string>("Code");
                datos.Name = conn.DatoLector<string>("Name");
                datos.U_SO1_PROMOCION = conn.DatoLector<string>("U_SO1_PROMOCION");
                datos.U_SO1_PROMOCADENA = conn.DatoLector<string>("U_SO1_PROMOCADENA");
                datos.U_SO1_LISTAPRECIO = conn.DatoLector<decimal>("U_SO1_LISTAPRECIO");
                datos.U_SO1_CANTIDADMES = conn.DatoLector<decimal>("U_SO1_CANTIDADMES");
                datos.U_SO1_PORCENTAJEDESC = conn.DatoLector<decimal>("U_SO1_PORCENTAJEDESC");
                promocion.PromocionDescEmp = datos;
            }
            else
            {
                Modelo.MPromDescEmp datos = new Modelo.MPromDescEmp();
                promocion.PromocionDescEmp = datos;
            }
            conn.CerrarLector();
        }

        public void CargarPromocionDescuentoEscala(Conexion conn, MPromocion promocion)
        {
            conn.LimpiarParametros();
            conn.CargarConsulta(ConsultasPromocion.ConsultarPromocionDescEsc);
            conn.AgregarParametro("pPromocion", promocion.U_SO1_PROMOCION);
            conn.EjecutarLector();
            Modelo.MPromDescEsc datos = new Modelo.MPromDescEsc();
            if (conn.LeerFila())
            {
                datos.Code = conn.DatoLector<string>("Codigo");
                datos.Name = conn.DatoLector<string>("Nombre");
                datos.U_SO1_PROMOCION = conn.DatoLector<string>("Promo");
                datos.U_SO1_PROMOCADENA = conn.DatoLector<string>("PromoCadena");
                datos.U_SO1_CONIMPUESTO = conn.DatoLector<string>("Impuesto");
                datos.U_SO1_TIPO = conn.DatoLector<decimal>("Tipo");
                datos.U_SO1_ARTICULOSTODOS = conn.DatoLector<string>("ArticuloTodos");
                promocion.PromocionDescEsc = datos;
            }
            else
            {
                promocion.PromocionDescEsc = datos;
            }
            conn.CerrarLector();

            conn.LimpiarParametros();
            conn.CargarConsulta(ConsultasPromocion.ConsultarPromocionDescEscPr);
            conn.AgregarParametro("pPromocion", promocion.U_SO1_PROMOCION);
            DataTable dtPromoDescuento = conn.EjecutarTabla();
            foreach (DataRow fila in dtPromoDescuento.Rows)
            {
                Modelo.MPromDescMonto DescuentoMonto = new Modelo.MPromDescMonto();
                DescuentoMonto.Code = fila.Valor<string>("Codigo");
                DescuentoMonto.Name = fila.Valor<string>("Nombre");
                DescuentoMonto.U_SO1_PROMOCION = fila.Valor<string>("Promo");
                DescuentoMonto.U_SO1_PROMOCADENA = fila.Valor<string>("PromoCadena");
                DescuentoMonto.U_SO1_MONTO = fila.Valor<decimal>("Monto").ToString("N2");
                DescuentoMonto.U_SO1_PORCENTAJEDESC = fila.Valor<decimal>("Porcentaje").ToString("N2");
                datos.Agregar(DescuentoMonto);
            }
        }

        public void CargarPromocionDescuentoImporte(Conexion conn, MPromocion promocion)
        {
            conn.LimpiarParametros();
            conn.CargarConsulta(ConsultasPromocion.ConsultarPromocionDescImp);
            conn.AgregarParametro("pPromocion", promocion.U_SO1_PROMOCION);
            conn.EjecutarLector();
            if (conn.LeerFila())
            {
                Modelo.MPromDescImp datos = new Modelo.MPromDescImp();
                datos.Code = conn.DatoLector<string>("Codigo");
                datos.Name = conn.DatoLector<string>("Nombre");
                datos.U_SO1_PROMOCION = conn.DatoLector<string>("Promo");
                datos.U_SO1_PROMOCADENA = conn.DatoLector<string>("PromoCadena");
                datos.U_SO1_COMPORTAMIENTO = conn.DatoLector<string>("Comportamiento");
                datos.U_SO1_MONEDA = conn.DatoLector<string>("Moneda");
                datos.U_SO1_MONTOMINIMO = conn.DatoLector<decimal>("MontoMinimo");
                datos.U_SO1_IMPORTEDESC = conn.DatoLector<decimal>("ImporteDesc");
                promocion.PromocionDescImp = datos;
            }
            else
            {
                Modelo.MPromDescImp datos = new Modelo.MPromDescImp();
                promocion.PromocionDescImp = datos;
            }
            conn.CerrarLector();
        }

        public void CargarPromocionDescuentoPorcentaje(Conexion conn, MPromocion promocion)
        {
            conn.LimpiarParametros();
            conn.CargarConsulta(ConsultasPromocion.ConsultarPromocionDescPor);
            conn.AgregarParametro("pPromocion", promocion.U_SO1_PROMOCION);
            conn.EjecutarLector();
            if (conn.LeerFila())
            {
                Modelo.MPromDescPor datos = new Modelo.MPromDescPor();
                datos.Code = conn.DatoLector<string>("Codigo");
                datos.Name = conn.DatoLector<string>("Nombre");
                datos.U_SO1_PROMOCION = conn.DatoLector<string>("Promo");
                datos.U_SO1_PROMOCADENA = conn.DatoLector<string>("PromoCadena");
                datos.U_SO1_DESCUENTO1 = conn.DatoLector<decimal>("Descuento1");
                datos.U_SO1_DESCUENTO2 = conn.DatoLector<decimal>("Descuento2");
                datos.U_SO1_MANTENERDESC = conn.DatoLector<string>("MantenerDesc");
                promocion.PromocionDescPor = datos;
            }
            else
            {
                Modelo.MPromDescPor datos = new Modelo.MPromDescPor();
                promocion.PromocionDescPor = datos;
            }
            conn.CerrarLector();
        }

        public void CargarPromocionDescuentoVolumen(Conexion conn, MPromocion promocion)
        {
            conn.LimpiarParametros();
            conn.CargarConsulta(ConsultasPromocion.ConsultarPromocionDescVol);
            conn.AgregarParametro("pPromocion", promocion.U_SO1_PROMOCION);
            conn.EjecutarLector();
            Modelo.MPromDescVol datos = new Modelo.MPromDescVol();
            if (conn.LeerFila())
            {
                datos.Code = conn.DatoLector<string>("Codigo");
                datos.Name = conn.DatoLector<string>("Nombre");
                datos.U_SO1_PROMOCION = conn.DatoLector<string>("Promo");
                datos.U_SO1_PROMOCADENA = conn.DatoLector<string>("PromoCadena");
                datos.U_SO1_CANTIDAD = conn.DatoLector<decimal>("Cantidad");
                datos.U_SO1_PORCENTAJEDESC = conn.DatoLector<decimal>("PorcentajeDesc");
                datos.U_SO1_MANTENERDESC = conn.DatoLector<string>("MantenerDesc");
                datos.U_SO1_ACUMULARART = conn.DatoLector<string>("AcumulaArt");
                datos.U_SO1_ACTDESCINCRE = conn.DatoLector<string>("ActDescuentoIncre");
                promocion.PromocionDescVol = datos;
            }
            else
            {
                promocion.PromocionDescVol = datos;
            }
            conn.CerrarLector();

            conn.LimpiarParametros();
            conn.CargarConsulta(ConsultasPromocion.ConsultarPromocionDescVolEs);
            conn.AgregarParametro("pPromocion", promocion.U_SO1_PROMOCION);
            DataTable dtPromoDescuento = conn.EjecutarTabla();
            foreach (DataRow fila in dtPromoDescuento.Rows)
            {
                Modelo.MPromDescMonto DescuentoMonto = new Modelo.MPromDescMonto();
                DescuentoMonto.Code = fila.Valor<string>("Codigo");
                DescuentoMonto.Name = fila.Valor<string>("Nombre");
                DescuentoMonto.U_SO1_PROMOCION = fila.Valor<string>("Promo");
                DescuentoMonto.U_SO1_PROMOCADENA = fila.Valor<string>("PromoCadena");
                DescuentoMonto.U_SO1_CANTIDAD = fila.Valor<decimal>("Cantidad").ToString("N2");
                DescuentoMonto.U_SO1_PORCENTAJEDESC = fila.Valor<decimal>("PorcentajeDesc").ToString("N2");
                DescuentoMonto.U_SO1_MANTENERDESC = fila.Valor<string>("MantenerDesc");
                datos.Agregar(DescuentoMonto);
            }
        }

        public void CargarPromocionKitVenta(Conexion conn, MPromocion promocion)
        {
            conn.LimpiarParametros();
            conn.CargarConsulta(ConsultasPromocion.ConsultarKitVenta);
            conn.AgregarParametro("pPromocion", promocion.U_SO1_PROMOCION);
            conn.EjecutarLector();
            if (conn.LeerFila())
            {
                Modelo.MPromoKitVenta datos = new Modelo.MPromoKitVenta();
                datos.Code = conn.DatoLector<string>("Code");
                datos.Name = conn.DatoLector<string>("Name");
                datos.U_SO1_PROMOCION = conn.DatoLector<string>("U_SO1_PROMOCION");
                datos.U_SO1_PROMOCADENA = conn.DatoLector<string>("U_SO1_PROMOCADENA");
                datos.U_SO1_TIPOREGALO = conn.DatoLector<string>("U_SO1_TIPOREGALO");
                datos.U_SO1_CONIMPUESTO = conn.DatoLector<string>("U_SO1_CONIMPUESTO");
                promocion.PromocionKitVenta = datos;
            }
            else
            {
                Modelo.MPromoKitVenta datos = new Modelo.MPromoKitVenta();
                promocion.PromocionKitVenta = datos;
            }
            conn.CerrarLector();
        }

        public void CargarPromocionKitRegalo(Conexion conn, MPromocion promocion)
        {
            conn.LimpiarParametros();
            conn.CargarConsulta(ConsultasPromocion.ConsultarPromocionKitReg);
            conn.AgregarParametro("pPromocion", promocion.U_SO1_PROMOCION);
            conn.EjecutarLector();
            Modelo.MPromKitRegalo datos = new Modelo.MPromKitRegalo();
            if (conn.LeerFila())
            {
                datos.Code = conn.DatoLector<string>("Codigo");
                datos.Name = conn.DatoLector<string>("Nombre");
                datos.U_SO1_PROMOCION = conn.DatoLector<string>("Promo");
                datos.U_SO1_PROMOCADENA = conn.DatoLector<string>("PromoCadena");
                datos.U_SO1_CANTIDADREG = conn.DatoLector<decimal>("CantidadReg");
                datos.U_SO1_CANTIDADCRI = conn.DatoLector<decimal>("CantidadCri");
                datos.U_SO1_TIPOREGALO = conn.DatoLector<string>("TipoRegalo");
                datos.U_SO1_MONTO = conn.DatoLector<decimal>("Monto");
                datos.U_SO1_REGALOPRECIO1 = conn.DatoLector<string>("RegaloPrecio");
                datos.U_SO1_AGREGARARTAUTO = conn.DatoLector<string>("AgregarArt");
                promocion.PromocionKitRegalo = datos;
            }
            else
            {
                promocion.PromocionKitRegalo = datos;
            }
            conn.CerrarLector();

            conn.LimpiarParametros();
            conn.CargarConsulta(ConsultasPromocion.ConsultarArticulosPromo);
            conn.AgregarParametro("pPromocion", promocion.U_SO1_PROMOCION);
            conn.AgregarParametro("pTipo", "R");
            DataTable dtArticuloPromo = conn.EjecutarTabla();
            foreach (DataRow fila in dtArticuloPromo.Rows)
            {
                Modelo.MPromoArticulo articulo = new Modelo.MPromoArticulo();
                articulo.Code = fila.Valor<string>("Codigo");
                articulo.Name = fila.Valor<string>("Nombre");
                articulo.U_SO1_PROMOCION = fila.Valor<int>("Promo");
                articulo.U_SO1_PROMOCADENA = fila.Valor<string>("PromoCadena");
                articulo.U_SO1_ARTICULO = fila.Valor<string>("Articulo");
                articulo.U_SO1_TIPO = fila.Valor<string>("Tipo"); ;
                articulo.U_SO1_IMPORTE = fila.Valor<decimal>("Importe");
                articulo.U_SO1_CODIGOAMBITO = fila.Valor<string>("CodigoAmbito");
                articulo.Nombre = fila.Valor<string>("NombreArticulo");
                datos.Agregar(articulo);
            }
        }

        public void CargarPromocionPoliticaVenta(Conexion conn, MPromocion promocion)
        {
            conn.LimpiarParametros();
            conn.CargarConsulta(ConsultasPromocion.ConsultarPromocionPolVenta);
            conn.AgregarParametro("pPromocion", promocion.U_SO1_PROMOCION);
            conn.EjecutarLector();
            Modelo.MPromPolVenta datos = new Modelo.MPromPolVenta();
            if (conn.LeerFila())
            {
                datos.Code = conn.DatoLector<string>("Codigo");
                datos.Name = conn.DatoLector<string>("Nombre");
                datos.U_SO1_PROMOCION = conn.DatoLector<string>("Promo");
                datos.U_SO1_PROMOCADENA = conn.DatoLector<string>("PromoCadena");
                datos.U_SO1_ACUMULARART = conn.DatoLector<string>("AcumulaArt");
                datos.U_SO1_MANTENERDESC = conn.DatoLector<string>("MantenerDesc");
                promocion.PromocionPoliticaVenta = datos;
            }
            else
            {
                promocion.PromocionPoliticaVenta = datos;
            }
            conn.CerrarLector();

            conn.LimpiarParametros();
            conn.CargarConsulta(ConsultasPromocion.ConsultarListaPreciosPromo);
            conn.AgregarParametro("pPromocion", promocion.U_SO1_PROMOCION);
            DataTable dtLPrecioPromo = conn.EjecutarTabla();
            foreach (DataRow fila in dtLPrecioPromo.Rows)
            {
                Modelo.MPromoPrecios listaPrecio = new Modelo.MPromoPrecios();
                listaPrecio.Code = fila.Valor<string>("Codigo");
                listaPrecio.Name = fila.Valor<string>("Nombre");
                listaPrecio.U_SO1_PROMOCION = fila.Valor<string>("Promo");
                listaPrecio.U_SO1_PROMOCADENA = fila.Valor<string>("PromoCadena");
                listaPrecio.U_SO1_CANTIDADINI = fila.Valor<decimal>("CantidadIni");
                listaPrecio.U_SO1_LISTAPRECIO = fila.Valor<string>("ListaPrecio");
                listaPrecio.ListName = fila.Valor<string>("NombreLista");
                datos.Agregar(listaPrecio);
            }
        }

        public void CargarPromocionRegaloMonto(Conexion conn, MPromocion promocion)
        {
            conn.LimpiarParametros();
            conn.CargarConsulta(ConsultasPromocion.ConsultarPromocionRegaloMonto);
            conn.AgregarParametro("pPromocion", promocion.U_SO1_PROMOCION);
            conn.EjecutarLector();
            Modelo.MPromRegaloMonto datos = new Modelo.MPromRegaloMonto();
            if (conn.LeerFila())
            {
                datos.Code = conn.DatoLector<string>("Codigo");
                datos.Name = conn.DatoLector<string>("Nombre");
                datos.U_SO1_PROMOCION = conn.DatoLector<string>("Promo");
                datos.U_SO1_PROMOCADENA = conn.DatoLector<string>("PromoCadena");
                datos.U_SO1_AGREGARARTAUTO = conn.DatoLector<string>("AgregarArt");
                datos.U_SO1_REGALOPRECIO1 = conn.DatoLector<string>("RegaloPrecio");
                datos.U_SO1_PORCADAXMONTO = conn.DatoLector<string>("xMonto");
                datos.U_SO1_PORCENTAJEPRO = conn.DatoLector<decimal>("PorcentajePro");
                datos.U_SO1_MENSAJEPROXIM = conn.DatoLector<string>("Mensaje");
                datos.U_SO1_MONEDA = conn.DatoLector<string>("Moneda");
                datos.U_SO1_MONTOREGALO = conn.DatoLector<decimal>("MontoRegalo");
                datos.U_SO1_CANTIDADREGAL = conn.DatoLector<decimal>("CantidadRegalo");
                promocion.PromocionRegaloMonto = datos;
            }
            else
            {
                promocion.PromocionRegaloMonto = datos;
            }
            conn.CerrarLector();

            conn.LimpiarParametros();
            conn.CargarConsulta(ConsultasPromocion.ConsultarArticulosPromo);
            conn.AgregarParametro("pPromocion", promocion.U_SO1_PROMOCION);
            conn.AgregarParametro("pTipo", "R");
            DataTable dtArticuloPromo = conn.EjecutarTabla();
            foreach (DataRow fila in dtArticuloPromo.Rows)
            {
                Modelo.MPromoArticulo articulo = new Modelo.MPromoArticulo();
                articulo.Code = fila.Valor<string>("Codigo");
                articulo.Name = fila.Valor<string>("Nombre");
                articulo.U_SO1_PROMOCION = fila.Valor<int>("Promo");
                articulo.U_SO1_PROMOCADENA = fila.Valor<string>("PromoCadena");
                articulo.U_SO1_ARTICULO = fila.Valor<string>("Articulo");
                articulo.U_SO1_TIPO = fila.Valor<string>("Tipo"); ;
                articulo.U_SO1_IMPORTE = fila.Valor<decimal>("Importe");
                articulo.U_SO1_CODIGOAMBITO = fila.Valor<string>("CodigoAmbito");
                articulo.Nombre = fila.Valor<string>("NombreArticulo");
                promocion.PromocionRegaloMonto.Agregar(articulo);
            }
        }

        public void CargarPromocionDescuentoAleatorio(Conexion conn, MPromocion promocion)
        {
            try
            {
                conn.LimpiarParametros();
                conn.CargarConsulta(ConsultasPromocion.ConsultarPromocionDescuentoAleatorio);
                conn.AgregarParametro("pPromocion", promocion.U_SO1_PROMOCION);
                conn.EjecutarLector();
                Modelo.MPromDescAle datos = new Modelo.MPromDescAle();
                if (conn.LeerFila())
                {
                    datos.Codigo = conn.DatoLector<string>("Codigo");
                    datos.Nombre = conn.DatoLector<string>("Nombre");
                    datos.U_SO1_PROMOCION = conn.DatoLector<string>("Promo");
                    datos.U_SO1_PROMOCADENA = conn.DatoLector<string>("PromoCadena");
                    datos.U_SO1_ACTMONTOMAXIMO = conn.DatoLector<string>("ActMontoMaximo");
                    datos.U_SO1_MONTOMAXIPROMO = conn.DatoLector<decimal>("MontoMaxPromo");
                    datos.U_SO1_ACTMONTOMAXDIA = conn.DatoLector<string>("ActMontoMaxDia");
                    datos.U_SO1_MONTOMAXPORDIA = conn.DatoLector<decimal>("MontoMaxDia");
                    datos.U_SO1_ACTIMPVENDESDE = conn.DatoLector<string>("ActImpVende");
                    datos.U_SO1_IMPORTVENDESDE = conn.DatoLector<decimal>("ImporteVende");
                    datos.U_SO1_ACTIMPVENHASTA = conn.DatoLector<string>("ActImporteVendeHasta");
                    datos.U_SO1_IMPORTVENHASTA = conn.DatoLector<decimal>("ImporteVendeHasta");
                    datos.U_SO1_ACTIMPORTEIMP = conn.DatoLector<string>("ActImpoImp");
                    datos.U_SO1_COMPOTRASPROM = conn.DatoLector<string>("CompoTrasProm");
                    datos.U_SO1_ACTFRECUENPROM = conn.DatoLector<string>("ActFrecuencia");
                    datos.U_SO1_MINFRECPROMO = conn.DatoLector<string>("MinFrecuencia");
                    datos.U_SO1_FECHULTPROMAPL = conn.DatoLector<string>("FechaUltima");
                    datos.U_SO1_HORAULTPROMAPL = conn.DatoLector<string>("HoraUltima");
                    datos.U_SO1_FECHANTPROAPLI = conn.DatoLector<string>("FechaAnt");
                    datos.U_SO1_HORANTPROMAPLI = conn.DatoLector<string>("HoraAntPro");
                    promocion.PromocionDescAle = datos;
                }
                else
                {
                    promocion.PromocionDescAle = datos;
                }
                conn.CerrarLector();
                conn.LimpiarParametros();
                conn.CargarConsulta(ConsultasPromocion.ConsultarCatalogoHoras);
                conn.AgregarParametro("pPromocion", promocion.U_SO1_PROMOCION);
                DataTable dtCatalogo = conn.EjecutarTabla();
                foreach (DataRow fila in dtCatalogo.Rows)
                {
                    Modelo.MPromoCupon catHorario = new Modelo.MPromoCupon();
                    catHorario.Code = fila.Valor<string>("Codigo");
                    catHorario.Name = fila.Valor<string>("Nombre");
                    catHorario.U_SO1_CODIGPROMOCION = fila.Valor<string>("Promo");
                    catHorario.U_SO1_PROMOCIONCADEN = fila.Valor<string>("PromoCadena");
                    catHorario.U_SO1_CODIGCATEHORAR = fila.Valor<string>("CodigoCatalogo");
                    catHorario.NombreCatalogo = fila.Valor<string>("NombreHorario");
                    promocion.PromocionDescAle.Agregar(catHorario);
                }

                conn.LimpiarParametros();
                conn.CargarConsulta(ConsultasPromocion.ConsultarPorcentajes);
                conn.AgregarParametro("pPromocion", promocion.U_SO1_PROMOCION);
                DataTable dtPorcentajes = conn.EjecutarTabla();
                foreach (DataRow fila in dtPorcentajes.Rows)
                {
                    Modelo.MPromPorcentaje porcentaje = new Modelo.MPromPorcentaje();
                    porcentaje.Code = fila.Valor<string>("Codigo");
                    porcentaje.Name = fila.Valor<string>("Nombre");
                    porcentaje.U_SO1_CODIGPROMOCION = fila.Valor<string>("Promo");
                    porcentaje.U_SO1_PROMOCIONCADEN= fila.Valor<string>("PromoCadena");
                    porcentaje.U_SO1_NUMEROLINEA = fila.Valor<string>("NumeroLinea");
                    porcentaje.U_SO1_PORCENTAJE = fila.Valor<decimal>("Porcentaje");
                    promocion.PromocionDescAle.Agregar(porcentaje);
                }
            }
            catch (Exception oError)
            {
            }
        }

        public void CargarPromocionCupon(Conexion conn, MPromocion promocion)
        {
            try
            {

                conn.LimpiarParametros();
                conn.CargarConsulta(ConsultasPromocion.ConsultarPromocionCupon);
                conn.AgregarParametro("pPromocion", promocion.U_SO1_PROMOCION);
                conn.EjecutarLector();
                Modelo.MCupon datos = new Modelo.MCupon();
                if (conn.LeerFila())
                {
                    datos.Code = conn.DatoLector<string>("Codigo");
                    datos.Name = conn.DatoLector<string>("Nombre");
                    datos.U_SO1_CODIGPROMOCION = conn.DatoLector<string>("Promo");
                    datos.U_SO1_PROMOCADENA = conn.DatoLector<string>("PromoCadena");
                    datos.U_SO1_TIPO = conn.DatoLector<string>("Tipo");
                    datos.U_SO1_MONTO = conn.DatoLector<decimal>("Monto");
                    datos.U_SO1_CANTIDAD = conn.DatoLector<decimal>("Cantidad");
                    datos.U_SO1_NUMEROCUPONES = conn.DatoLector<string>("NumeroCupones");
                    datos.U_SO1_ACTIMPORTEIMP = conn.DatoLector<string>("ActImporte");
                    datos.U_SO1_COMPOTRASPROM = conn.DatoLector<string>("CompoTras");
                    datos.U_SO1_APLICPROMIGUAL = conn.DatoLector<string>("AplicaProm");
                    datos.U_SO1_TIPOCOMPORTAMI = conn.DatoLector<string>("TipoComportamiento");
                    promocion.PromocionCupon = datos;
                }
                else
                {
                    promocion.PromocionCupon = datos;
                }
                conn.CerrarLector();
                conn.LimpiarParametros();
                conn.CargarConsulta(ConsultasPromocion.ConsultarCatalogoHoras);
                conn.AgregarParametro("pPromocion", promocion.U_SO1_PROMOCION);
                DataTable dtCatalogo = conn.EjecutarTabla();
                foreach (DataRow fila in dtCatalogo.Rows)
                {
                    Modelo.MPromoCupon catHorario = new Modelo.MPromoCupon();
                    catHorario.Code = fila.Valor<string>("Codigo");
                    catHorario.Name = fila.Valor<string>("Nombre");
                    catHorario.U_SO1_CODIGPROMOCION = fila.Valor<string>("Promo");
                    catHorario.U_SO1_PROMOCIONCADEN = fila.Valor<string>("PromoCadena");
                    catHorario.U_SO1_CODIGCATEHORAR = fila.Valor<string>("CodigoCatalogo");
                    catHorario.NombreCatalogo = fila.Valor<string>("NombreHorario");
                    promocion.PromocionCupon.Agregar(catHorario);
                }
            }
            catch (Exception oError)
            {
            }
        }

        public void CargarPromocionPrecioUnico(Conexion conn, MPromocion promocion)
        {
            conn.LimpiarParametros();
            conn.CargarConsulta(ConsultasPromocion.ConsultarPromocionPrecioUnico);
            conn.AgregarParametro("pPromocion", promocion.U_SO1_PROMOCION);
            conn.EjecutarLector();
            if (conn.LeerFila())
            {
                Modelo.MPromoPrecioUnico datos = new Modelo.MPromoPrecioUnico();
                datos.Code = conn.DatoLector<string>("Code");
                datos.Name = conn.DatoLector<string>("Name");
                datos.U_SO1_PROMOCION = conn.DatoLector<string>("U_SO1_PROMOCION");
                datos.U_SO1_PROMOCADENA = conn.DatoLector<string>("U_SO1_PROMOCADENA");
                datos.U_SO1_MONEDA = conn.DatoLector<string>("U_SO1_MONEDA");
                datos.U_SO1_MONTO = conn.DatoLector<decimal>("U_SO1_MONTO");
                datos.U_SO1_CONIMPUESTO = conn.DatoLector<string>("U_SO1_CONIMPUESTO");
                datos.U_SO1_MANTENERDESC = conn.DatoLector<string>("U_SO1_MANTENERDESC");
                promocion.PromocionPrecioUnico = datos;
            }
            else
            {
                Modelo.MPromoPrecioUnico datos = new Modelo.MPromoPrecioUnico();
                promocion.PromocionPrecioUnico = datos;
            }
            conn.CerrarLector();
        }

        public void CargarPromocionValeDescuento(Conexion conn, MPromocion promocion)
        {
            conn.LimpiarParametros();
            conn.CargarConsulta(ConsultasPromocion.ConsultarPromocionValeDescuento);
            conn.AgregarParametro("pPromocion", promocion.U_SO1_PROMOCION);
            conn.EjecutarLector();
            if (conn.LeerFila())
            {
                Modelo.MPromValeDescuento datos = new Modelo.MPromValeDescuento();
                datos.Code = conn.DatoLector<string>("Code");
                datos.Name = conn.DatoLector<string>("Name");
                datos.U_SO1_PROMOCION = conn.DatoLector<string>("U_SO1_PROMOCION");
                datos.U_SO1_PROMOCADENA = conn.DatoLector<string>("U_SO1_PROMOCADENA");
                datos.U_SO1_COMPORTAMIENTO = conn.DatoLector<string>("U_SO1_COMPORTAMIENTO");
                datos.U_SO1_MONEDA = conn.DatoLector<string>("U_SO1_MONEDA");
                datos.U_SO1_MONTOMINIMO = conn.DatoLector<decimal>("U_SO1_MONTOMINIMO");
                datos.U_SO1_TIPODESCUENTO = conn.DatoLector<string>("U_SO1_TIPODESCUENTO");
                datos.U_SO1_IMPORTEDESC = conn.DatoLector<decimal>("U_SO1_IMPORTEDESC");
                datos.U_SO1_FECHAVENCI = conn.DatoLector<string>("U_SO1_FECHAVENCI") != "" ? conn.DatoLector<string>("U_SO1_FECHAVENCI").Substring(0, 10) : "";
                datos.U_SO1_DIASVENCI = conn.DatoLector<decimal>("U_SO1_DIASVENCI");
                datos.U_SO1_POLITICAREDEN = conn.DatoLector<string>("U_SO1_POLITICAREDEN");
                promocion.PromocionValeDescuento = datos;
            }
            else
            {
                Modelo.MPromValeDescuento datos = new Modelo.MPromValeDescuento();
                promocion.PromocionValeDescuento = datos;
            }
            conn.CerrarLector();
        }

        public void CargarPromocionPoliticaRedencion(Conexion conn, MPromocion promocion)
        {
            conn.LimpiarParametros();
            conn.CargarConsulta(ConsultasPromocion.ConsultarPromocionPoliticaRedencion);
            conn.AgregarParametro("pPromocion", promocion.Codigo);
            conn.EjecutarLector();
            if (conn.LeerFila())
            {
                Modelo.MPromPoliticaRedencion datos = new Modelo.MPromPoliticaRedencion();

                datos.Code = conn.DatoLector<string>("Code");
                datos.Name = conn.DatoLector<string>("Name");
                datos.U_SO1_POLITICAREDEN = conn.DatoLector<int>("U_SO1_POLITICAREDEN");
                //La Jerarquia no es necesaria
                datos.U_SO1_TIPO = conn.DatoLector<string>("U_SO1_TIPO");
                datos.U_SO1_DIARIO = conn.DatoLector<string>("U_SO1_DIARIO");
                datos.U_SO1_DIACOMPLETO = conn.DatoLector<string>("U_SO1_DIACOMPLETO");
                datos.U_SO1_DIAHORAINI = conn.DatoLector<string>("U_SO1_DIAHORAINI");
                datos.U_SO1_DIAHORAFIN = conn.DatoLector<string>("U_SO1_DIAHORAFIN");
                datos.U_SO1_LUNES = conn.DatoLector<string>("U_SO1_LUNES");
                datos.U_SO1_LUNCOMPLETO = conn.DatoLector<string>("U_SO1_LUNCOMPLETO");
                datos.U_SO1_LUNHORAINI = conn.DatoLector<string>("U_SO1_LUNHORAINI");
                datos.U_SO1_LUNHORAFIN = conn.DatoLector<string>("U_SO1_LUNHORAFIN");
                datos.U_SO1_MARTES = conn.DatoLector<string>("U_SO1_MARTES");
                datos.U_SO1_MARCOMPLETO = conn.DatoLector<string>("U_SO1_MARCOMPLETO");
                datos.U_SO1_MARHORAINI = conn.DatoLector<string>("U_SO1_MARHORAINI");
                datos.U_SO1_MARHORAFIN = conn.DatoLector<string>("U_SO1_MARHORAFIN");
                datos.U_SO1_MIERCOLES = conn.DatoLector<string>("U_SO1_MIERCOLES");
                datos.U_SO1_MIECOMPLETO = conn.DatoLector<string>("U_SO1_MIECOMPLETO");
                datos.U_SO1_MIEHORAINI = conn.DatoLector<string>("U_SO1_MIEHORAINI");
                datos.U_SO1_MIEHORAFIN = conn.DatoLector<string>("U_SO1_MIEHORAFIN");
                
                datos.U_SO1_JUEVES = conn.DatoLector<string>("U_SO1_JUEVES");
                datos.U_SO1_JUECOMPLETO = conn.DatoLector<string>("U_SO1_JUECOMPLETO");
                datos.U_SO1_JUEHORAINI = conn.DatoLector<string>("U_SO1_JUEHORAINI");
                datos.U_SO1_JUEHORAFIN = conn.DatoLector<string>("U_SO1_JUEHORAFIN");
                datos.U_SO1_VIERNES = conn.DatoLector<string>("U_SO1_VIERNES");
                datos.U_SO1_VIECOMPLETO = conn.DatoLector<string>("U_SO1_VIECOMPLETO");
                datos.U_SO1_VIEHORAINI = conn.DatoLector<string>("U_SO1_VIEHORAINI");
                datos.U_SO1_VIEHORAFIN = conn.DatoLector<string>("U_SO1_VIEHORAFIN");
                datos.U_SO1_SABADO = conn.DatoLector<string>("U_SO1_SABADO");
                datos.U_SO1_SABCOMPLETO = conn.DatoLector<string>("U_SO1_SABCOMPLETO");
                datos.U_SO1_SABHORAINI = conn.DatoLector<string>("U_SO1_SABHORAINI");
                datos.U_SO1_SABHORAFIN = conn.DatoLector<string>("U_SO1_SABHORAFIN");
                datos.U_SO1_DOMINGO = conn.DatoLector<string>("U_SO1_DOMINGO");
                datos.U_SO1_DOMCOMPLETO = conn.DatoLector<string>("U_SO1_DOMCOMPLETO");
                datos.U_SO1_DOMHORAINI = conn.DatoLector<string>("U_SO1_DOMHORAINI");
                datos.U_SO1_DOMHORAFIN = conn.DatoLector<string>("U_SO1_DOMHORAFIN");
                datos.U_SO1_ARTICULOENLACE = conn.DatoLector<string>("U_SO1_ARTICULOENLACE");
                datos.U_SO1_FILTRARARTART = conn.DatoLector<string>("U_SO1_FILTRARARTART");
                datos.U_SO1_FILTRARARTGRU = conn.DatoLector<string>("U_SO1_FILTRARARTGRU");
                datos.U_SO1_FILTRARARTPRO = conn.DatoLector<string>("U_SO1_FILTRARARTPRO");
                
                datos.U_SO1_FILTRARARTPROE = conn.DatoLector<string>("U_SO1_FILTRARARTPROE");
                datos.U_SO1_FILTRARARTPROC = conn.DatoLector<string>("U_SO1_FILTRARARTPROC");
                datos.U_SO1_FILTRARARTCOE = conn.DatoLector<string>("U_SO1_FILTRARARTCOE");
                datos.U_SO1_FILTRARARTCOEC = conn.DatoLector<string>("U_SO1_FILTRARARTCOEC");
                datos.U_SO1_FILTRARARTCOEV = conn.DatoLector<string>("U_SO1_FILTRARARTCOEV");
                datos.U_SO1_FILTRARARTPROV = conn.DatoLector<string>("U_SO1_FILTRARARTPROV");
                datos.U_SO1_FILTRARARTFAB = conn.DatoLector<string>("U_SO1_FILTRARARTFAB");
                datos.U_SO1_FILTRARARTEXC = conn.DatoLector<string>("U_SO1_FILTRARARTEXC");
                datos.U_SO1_FILTRARARTUNE = conn.DatoLector<string>("U_SO1_FILTRARARTUNE");
                datos.U_SO1_FILTRARPROMSUC = conn.DatoLector<string>("U_SO1_FILTRARPROMSUC");
                datos.U_SO1_FILTRARPROMALI = conn.DatoLector<string>("U_SO1_FILTRARPROMALI");
                datos.U_SO1_FILTRARPROMCLI = conn.DatoLector<string>("U_SO1_FILTRARPROMCLI");
                datos.U_SO1_FILTRARCLICLI = conn.DatoLector<string>("U_SO1_FILTRARCLICLI");
                datos.U_SO1_FILTRARCLIENLS = conn.DatoLector<string>("U_SO1_FILTRARCLIENLS");
                datos.U_SO1_FILTRARCLIGRU = conn.DatoLector<string>("U_SO1_FILTRARCLIGRU");
                datos.U_SO1_FILTRARCLIPRO = conn.DatoLector<string>("U_SO1_FILTRARCLIPRO");
                datos.U_SO1_FILTRARCLIPROE = conn.DatoLector<string>("U_SO1_FILTRARCLIPROE");
                datos.U_SO1_FILTRARCLIPROC = conn.DatoLector<string>("U_SO1_FILTRARCLIPROC");
                datos.U_SO1_FILTRARCLICOE = conn.DatoLector<string>("U_SO1_FILTRARCLICOE");
                datos.U_SO1_FILTRARCLICOEC = conn.DatoLector<string>("U_SO1_FILTRARCLICOEC");

                datos.U_SO1_FILTRARCLICOEV = conn.DatoLector<string>("U_SO1_FILTRARCLICOEV");
                datos.U_SO1_FILTRARPROMMEM = conn.DatoLector<string>("U_SO1_FILTRARPROMMEM");                
                datos.U_SO1_FILTRARPROMLIS = conn.DatoLector<string>("U_SO1_FILTRARPROMLIS");
                datos.U_SO1_FILTRARPROMFOP = conn.DatoLector<string>("U_SO1_FILTRARPROMFOP");
                datos.U_SO1_ACUMPROMOOTRAS = conn.DatoLector<string>("U_SO1_ACUMPROMOOTRAS");
                promocion.PromocionPoliticaRedencion = datos;
            }
            else
            {
                Modelo.MPromPoliticaRedencion datos = new Modelo.MPromPoliticaRedencion();
                promocion.PromocionPoliticaRedencion = datos;
            }
            conn.CerrarLector();
        }
        
        #endregion

        #region Validaciones

        public override void ValidarPropiedades()
        {

            //U_SO1_PROMOCION = string.IsNullOrEmpty(U_SO1_PROMOCION) ? string.Empty : U_SO1_PROMOCION;
            U_SO1_TIPO = string.IsNullOrEmpty(U_SO1_TIPO) ? string.Empty : U_SO1_TIPO;
            U_SO1_FECHADESDE = string.IsNullOrEmpty(U_SO1_FECHADESDE) ? string.Empty : U_SO1_FECHADESDE;
            U_SO1_FECHAHASTA = string.IsNullOrEmpty(U_SO1_FECHAHASTA) ? string.Empty : U_SO1_FECHAHASTA;
            U_SO1_DIARIO = string.IsNullOrEmpty(U_SO1_DIARIO) ? "N" : U_SO1_DIARIO;
            U_SO1_DIACOMPLETO = string.IsNullOrEmpty(U_SO1_DIACOMPLETO) ? "N" : U_SO1_DIACOMPLETO;
            U_SO1_DIAHORAINI = string.IsNullOrEmpty(U_SO1_DIAHORAINI) ? string.Empty : U_SO1_DIAHORAINI; //Este campo es INT
            U_SO1_DIAHORAFIN = string.IsNullOrEmpty(U_SO1_DIAHORAFIN) ? string.Empty : U_SO1_DIAHORAFIN; //Este campo es INT
            U_SO1_LUNES = string.IsNullOrEmpty(U_SO1_LUNES) ? "N" : U_SO1_LUNES;
            U_SO1_LUNCOMPLETO = string.IsNullOrEmpty(U_SO1_LUNCOMPLETO) ? "N" : U_SO1_LUNCOMPLETO;
            U_SO1_LUNHORAINI = string.IsNullOrEmpty(U_SO1_LUNHORAINI) ? string.Empty : U_SO1_LUNHORAINI; //Este campo es INT
            U_SO1_LUNHORAFIN = string.IsNullOrEmpty(U_SO1_LUNHORAFIN) ? string.Empty : U_SO1_LUNHORAFIN; //Este campo es INT
            U_SO1_MARTES = string.IsNullOrEmpty(U_SO1_MARTES) ? "N" : U_SO1_MARTES;
            U_SO1_MARCOMPLETO = string.IsNullOrEmpty(U_SO1_MARCOMPLETO) ? "N" : U_SO1_MARCOMPLETO;
            U_SO1_MARHORAINI = string.IsNullOrEmpty(U_SO1_MARHORAINI) ? string.Empty : U_SO1_MARHORAINI; //Este campo es INT
            U_SO1_MARHORAFIN = string.IsNullOrEmpty(U_SO1_MARHORAFIN) ? string.Empty : U_SO1_MARHORAFIN; //Este campo es INT
            U_SO1_MIERCOLES = string.IsNullOrEmpty(U_SO1_MIERCOLES) ? "N" : U_SO1_MIERCOLES;
            U_SO1_MIECOMPLETO = string.IsNullOrEmpty(U_SO1_MIECOMPLETO) ? "N" : U_SO1_MIECOMPLETO;
            U_SO1_MIEHORAINI = string.IsNullOrEmpty(U_SO1_MIEHORAINI) ? string.Empty : U_SO1_MIEHORAINI; //Este campo es INT
            U_SO1_MIEHORAFIN = string.IsNullOrEmpty(U_SO1_MIEHORAFIN) ? string.Empty : U_SO1_MIEHORAFIN; //Este campo es INT
            U_SO1_JUEVES = string.IsNullOrEmpty(U_SO1_JUEVES) ? "N" : U_SO1_JUEVES;
            U_SO1_JUECOMPLETO = string.IsNullOrEmpty(U_SO1_JUECOMPLETO) ? "N" : U_SO1_JUECOMPLETO;
            U_SO1_JUEHORAINI = string.IsNullOrEmpty(U_SO1_JUEHORAINI) ? string.Empty : U_SO1_JUEHORAINI; //Este campo es INT
            U_SO1_JUEHORAFIN = string.IsNullOrEmpty(U_SO1_JUEHORAFIN) ? string.Empty : U_SO1_JUEHORAFIN; //Este campo es INT
            U_SO1_VIERNES = string.IsNullOrEmpty(U_SO1_VIERNES) ? "N" : U_SO1_VIERNES;
            U_SO1_VIECOMPLETO = string.IsNullOrEmpty(U_SO1_VIECOMPLETO) ? "N" : U_SO1_VIECOMPLETO;
            U_SO1_VIEHORAINI = string.IsNullOrEmpty(U_SO1_VIEHORAINI) ? string.Empty : U_SO1_VIEHORAINI; //Este campo es INT
            U_SO1_VIEHORAFIN = string.IsNullOrEmpty(U_SO1_VIEHORAFIN) ? string.Empty : U_SO1_VIEHORAFIN; //Este campo es INT
            U_SO1_SABADO = string.IsNullOrEmpty(U_SO1_SABADO) ? "N" : U_SO1_SABADO;
            U_SO1_SABCOMPLETO = string.IsNullOrEmpty(U_SO1_SABCOMPLETO) ? "N" : U_SO1_SABCOMPLETO;
            U_SO1_SABHORAINI = string.IsNullOrEmpty(U_SO1_SABHORAINI) ? string.Empty : U_SO1_SABHORAINI; //Este campo es INT
            U_SO1_SABHORAFIN = string.IsNullOrEmpty(U_SO1_SABHORAFIN) ? string.Empty : U_SO1_SABHORAFIN; //Este campo es INT
            U_SO1_DOMINGO = string.IsNullOrEmpty(U_SO1_DOMINGO) ? "N" : U_SO1_DOMINGO;
            U_SO1_DOMCOMPLETO = string.IsNullOrEmpty(U_SO1_DOMCOMPLETO) ? "N" : U_SO1_DOMCOMPLETO;
            U_SO1_DOMHORAINI = string.IsNullOrEmpty(U_SO1_DOMHORAINI) ? string.Empty : U_SO1_DOMHORAINI;  //Este campo es INT
            U_SO1_DOMHORAFIN = string.IsNullOrEmpty(U_SO1_DOMHORAFIN) ? string.Empty : U_SO1_DOMHORAFIN;  //Este campo es INT
            U_SO1_ARTICULOENLACE = string.IsNullOrEmpty(U_SO1_ARTICULOENLACE) ? "O" : U_SO1_ARTICULOENLACE;
            U_SO1_FILTRARARTART = string.IsNullOrEmpty(U_SO1_FILTRARARTART) ? "N" : U_SO1_FILTRARARTART;
            U_SO1_FILTRARARTGRU = string.IsNullOrEmpty(U_SO1_FILTRARARTGRU) ? "N" : U_SO1_FILTRARARTGRU;
            U_SO1_FILTRARARTPRO = string.IsNullOrEmpty(U_SO1_FILTRARARTPRO) ? "N" : U_SO1_FILTRARARTPRO;
            U_SO1_FILTRARARTPROE = string.IsNullOrEmpty(U_SO1_FILTRARARTPROE) ? "O" : U_SO1_FILTRARARTPROE;
            U_SO1_FILTRARARTPROC = string.IsNullOrEmpty(U_SO1_FILTRARARTPROC) ? "N" : U_SO1_FILTRARARTPROC;
            U_SO1_FILTRARARTCOE = string.IsNullOrEmpty(U_SO1_FILTRARARTCOE) ? "N" : U_SO1_FILTRARARTCOE;
            U_SO1_FILTRARARTCOEC = string.IsNullOrEmpty(U_SO1_FILTRARARTCOEC) ? string.Empty : U_SO1_FILTRARARTCOEC;
            U_SO1_FILTRARARTCOEV = string.IsNullOrEmpty(U_SO1_FILTRARARTCOEV) ? string.Empty : U_SO1_FILTRARARTCOEV;
            U_SO1_FILTRARARTPROV = string.IsNullOrEmpty(U_SO1_FILTRARARTPROV) ? "N" : U_SO1_FILTRARARTPROV;
            U_SO1_FILTRARARTFAB = string.IsNullOrEmpty(U_SO1_FILTRARARTFAB) ? "N" : U_SO1_FILTRARARTFAB;
            U_SO1_FILTRARARTEXC = string.IsNullOrEmpty(U_SO1_FILTRARARTEXC) ? "N" : U_SO1_FILTRARARTEXC;
            U_SO1_FILTRARARTUNE = string.IsNullOrEmpty(U_SO1_FILTRARARTUNE) ? "N" : U_SO1_FILTRARARTUNE;
            U_SO1_FILTRARPROMSUC = string.IsNullOrEmpty(U_SO1_FILTRARPROMSUC) ? "N" : U_SO1_FILTRARPROMSUC;
            U_SO1_FILTRARPROMALI = string.IsNullOrEmpty(U_SO1_FILTRARPROMALI) ? "N" : U_SO1_FILTRARPROMALI;
            U_SO1_FILTRARPROMCLI = string.IsNullOrEmpty(U_SO1_FILTRARPROMCLI) ? "N" : U_SO1_FILTRARPROMCLI;
            U_SO1_FILTRARCLICLI = string.IsNullOrEmpty(U_SO1_FILTRARCLICLI) ? "N" : U_SO1_FILTRARCLICLI;
            U_SO1_FILTRARCLIENLS = string.IsNullOrEmpty(U_SO1_FILTRARCLIENLS) ? "O" : U_SO1_FILTRARCLIENLS;
            U_SO1_FILTRARCLIGRU = string.IsNullOrEmpty(U_SO1_FILTRARCLIGRU) ? "N" : U_SO1_FILTRARCLIGRU;
            U_SO1_FILTRARCLIPRO = string.IsNullOrEmpty(U_SO1_FILTRARCLIPRO) ? "N" : U_SO1_FILTRARCLIPRO;
            U_SO1_FILTRARCLIPROE = string.IsNullOrEmpty(U_SO1_FILTRARCLIPROE) ? "O" : U_SO1_FILTRARCLIPROE;
            U_SO1_FILTRARCLIPROC = string.IsNullOrEmpty(U_SO1_FILTRARCLIPROC) ? "N" : U_SO1_FILTRARCLIPROC;
            U_SO1_FILTRARCLICOE = string.IsNullOrEmpty(U_SO1_FILTRARCLICOE) ? "N" : U_SO1_FILTRARCLICOE;
            U_SO1_FILTRARCLICOEC = string.IsNullOrEmpty(U_SO1_FILTRARCLICOEC) ? string.Empty : U_SO1_FILTRARCLICOEC;
            U_SO1_FILTRARCLICOEV = string.IsNullOrEmpty(U_SO1_FILTRARCLICOEV) ? string.Empty : U_SO1_FILTRARCLICOEV;
            U_SO1_FILTRARPROMMEM = string.IsNullOrEmpty(U_SO1_FILTRARPROMMEM) ? "N" : U_SO1_FILTRARPROMMEM;
            U_SO1_FILTRARPROMLIS = string.IsNullOrEmpty(U_SO1_FILTRARPROMLIS) ? "N" : U_SO1_FILTRARPROMLIS;
            U_SO1_FILTRARPROMFOP = string.IsNullOrEmpty(U_SO1_FILTRARPROMFOP) ? "N" : U_SO1_FILTRARPROMFOP;
            U_SO1_ACUMPROMORM = string.IsNullOrEmpty(U_SO1_ACUMPROMORM) ? "N" : U_SO1_ACUMPROMORM;
            U_SO1_ACUMPROMOPUNTO = string.IsNullOrEmpty(U_SO1_ACUMPROMOPUNTO) ? "N" : U_SO1_ACUMPROMOPUNTO;
            U_SO1_ACUMPROMOOTRAS = string.IsNullOrEmpty(U_SO1_ACUMPROMOOTRAS) ? "N" : U_SO1_ACUMPROMOOTRAS;
            U_SO1_ACTLIMPIEZAVEN = string.IsNullOrEmpty(U_SO1_ACTLIMPIEZAVEN) ? "N" : U_SO1_ACTLIMPIEZAVEN;
            U_SO1_LIMITEPIEZAVEN = string.IsNullOrEmpty(U_SO1_LIMITEPIEZAVEN) ? string.Empty : U_SO1_LIMITEPIEZAVEN;
            U_SO1_ACTLIMPIEZAPRO = string.IsNullOrEmpty(U_SO1_ACTLIMPIEZAPRO) ? "N" : U_SO1_ACTLIMPIEZAPRO;
            U_SO1_LIMITEPIEZAPRO = string.IsNullOrEmpty(U_SO1_LIMITEPIEZAPRO) ? string.Empty : U_SO1_LIMITEPIEZAPRO;
            U_SO1_COMPORTAESCAUT = string.IsNullOrEmpty(U_SO1_COMPORTAESCAUT) ? "AU" : U_SO1_COMPORTAESCAUT;
            U_SO1_ACTLIMVENTAPRO = string.IsNullOrEmpty(U_SO1_ACTLIMVENTAPRO) ? "N" : U_SO1_ACTLIMVENTAPRO;
            U_SO1_LIMITEVENTAPRO = string.IsNullOrEmpty(U_SO1_LIMITEVENTAPRO) ? string.Empty : U_SO1_LIMITEVENTAPRO;
            U_SO1_ACTVENTACREDIT = string.IsNullOrEmpty(U_SO1_ACTVENTACREDIT) ? "N" : U_SO1_ACTVENTACREDIT;
            U_SO1_FILTRARARTCON = string.IsNullOrEmpty(U_SO1_FILTRARARTCON) ? "N" : U_SO1_FILTRARARTCON;
            U_SO1_FILTRARARTCONV = string.IsNullOrEmpty(U_SO1_FILTRARARTCONV) ? string.Empty : U_SO1_FILTRARARTCONV;
        }

        public void ValidarAxB()
        {
            PromocionAxB.Code = string.IsNullOrEmpty(PromocionAxB.Code) ? string.Empty : PromocionAxB.Code;
            PromocionAxB.Name = string.IsNullOrEmpty(PromocionAxB.Name) ? string.Empty : PromocionAxB.Name;
            PromocionAxB.U_SO1_PROMOCION = U_SO1_PROMOCION < 0 ? string.Empty : U_SO1_PROMOCION.ToString();
            PromocionAxB.U_SO1_PROMOCADENA = string.IsNullOrEmpty(PromocionAxB.U_SO1_PROMOCADENA) ? string.Empty : PromocionAxB.U_SO1_PROMOCADENA;
            PromocionAxB.U_SO1_PIEZASTOTALES = PromocionAxB.U_SO1_PIEZASTOTALES < 0 ? 0 : PromocionAxB.U_SO1_PIEZASTOTALES;
            PromocionAxB.U_SO1_PIEZASAPAGAR = PromocionAxB.U_SO1_PIEZASAPAGAR < 0 ? 0 : PromocionAxB.U_SO1_PIEZASAPAGAR;
            PromocionAxB.U_SO1_REGALOPRECIO1 = string.IsNullOrEmpty(PromocionAxB.U_SO1_REGALOPRECIO1) ? string.Empty : PromocionAxB.U_SO1_REGALOPRECIO1;
            PromocionAxB.U_SO1_MANTENERDESC = string.IsNullOrEmpty(PromocionAxB.U_SO1_MANTENERDESC) ? string.Empty : PromocionAxB.U_SO1_MANTENERDESC;
        }

        public void ValidarDescEmp()
        {
            PromocionDescEmp.Code = string.IsNullOrEmpty(PromocionDescEmp.Code) ? string.Empty : PromocionDescEmp.Code;
            PromocionDescEmp.Name = string.IsNullOrEmpty(PromocionDescEmp.Name) ? string.Empty : PromocionDescEmp.Name;
            PromocionDescEmp.U_SO1_PROMOCION = U_SO1_PROMOCION < 0 ? string.Empty : U_SO1_PROMOCION.ToString();
            PromocionDescEmp.U_SO1_PROMOCADENA = string.IsNullOrEmpty(PromocionDescEmp.U_SO1_PROMOCADENA) ? string.Empty : PromocionDescEmp.U_SO1_PROMOCADENA;
            PromocionDescEmp.U_SO1_LISTAPRECIO = PromocionDescEmp.U_SO1_LISTAPRECIO < 0 ? 0 : PromocionDescEmp.U_SO1_LISTAPRECIO;
            PromocionDescEmp.U_SO1_CANTIDADMES = PromocionDescEmp.U_SO1_CANTIDADMES < 0 ? 0 : PromocionDescEmp.U_SO1_CANTIDADMES;
            PromocionDescEmp.U_SO1_PORCENTAJEDESC = PromocionDescEmp.U_SO1_PORCENTAJEDESC < 0 ? 0 : PromocionDescEmp.U_SO1_PORCENTAJEDESC;
        }

        public void ValidarKV()
        {
            PromocionKitVenta.Code = string.IsNullOrEmpty(PromocionKitVenta.Code) ? string.Empty : PromocionKitVenta.Code;
            PromocionKitVenta.Name = string.IsNullOrEmpty(PromocionKitVenta.Name) ? string.Empty : PromocionKitVenta.Name;
            PromocionKitVenta.U_SO1_PROMOCION = U_SO1_PROMOCION < 0 ? string.Empty : U_SO1_PROMOCION.ToString();
            PromocionKitVenta.U_SO1_PROMOCADENA = string.IsNullOrEmpty(PromocionKitVenta.U_SO1_PROMOCADENA) ? string.Empty : PromocionKitVenta.U_SO1_PROMOCADENA;
            PromocionKitVenta.U_SO1_TIPOREGALO = string.IsNullOrEmpty(PromocionKitVenta.U_SO1_TIPOREGALO) ? string.Empty : PromocionKitVenta.U_SO1_TIPOREGALO;
            PromocionKitVenta.U_SO1_CONIMPUESTO = string.IsNullOrEmpty(PromocionKitVenta.U_SO1_CONIMPUESTO) ? string.Empty : PromocionKitVenta.U_SO1_CONIMPUESTO;
            //PromocionKitVenta.Identificador = PromocionKitVenta.Identificador;
            //PromocionKitVenta.Name = string.IsNullOrEmpty(PromocionKitVenta.Name) ? string.Empty : PromocionKitVenta.Name;
        }

        public void ValidarPrecioUnico()
        {
            PromocionPrecioUnico.Code = string.IsNullOrEmpty(PromocionPrecioUnico.Code) ? string.Empty : PromocionPrecioUnico.Code;
            PromocionPrecioUnico.Name = string.IsNullOrEmpty(PromocionPrecioUnico.Name) ? string.Empty : PromocionPrecioUnico.Name;
            PromocionPrecioUnico.U_SO1_PROMOCION = U_SO1_PROMOCION < 0 ? string.Empty : U_SO1_PROMOCION.ToString();
            PromocionPrecioUnico.U_SO1_PROMOCADENA = string.IsNullOrEmpty(PromocionPrecioUnico.U_SO1_PROMOCADENA) ? string.Empty : PromocionPrecioUnico.U_SO1_PROMOCADENA;
            PromocionPrecioUnico.U_SO1_MONEDA = string.IsNullOrEmpty(PromocionPrecioUnico.U_SO1_MONEDA) ? string.Empty : PromocionPrecioUnico.U_SO1_MONEDA;
            PromocionPrecioUnico.U_SO1_MONTO = PromocionPrecioUnico.U_SO1_MONTO < 0 ? 0 : PromocionPrecioUnico.U_SO1_MONTO;
            PromocionPrecioUnico.U_SO1_CONIMPUESTO = string.IsNullOrEmpty(PromocionPrecioUnico.U_SO1_CONIMPUESTO) ? string.Empty : PromocionPrecioUnico.U_SO1_CONIMPUESTO;
            PromocionPrecioUnico.U_SO1_MANTENERDESC = string.IsNullOrEmpty(PromocionPrecioUnico.U_SO1_MANTENERDESC) ? string.Empty : PromocionPrecioUnico.U_SO1_MANTENERDESC;
        }

        public void validarValeDescuento()
        {
            PromocionValeDescuento.Code = string.IsNullOrEmpty(PromocionValeDescuento.Code) ? string.Empty : PromocionValeDescuento.Code;
            PromocionValeDescuento.Name = string.IsNullOrEmpty(PromocionValeDescuento.Name) ? string.Empty : PromocionValeDescuento.Name;
            PromocionValeDescuento.U_SO1_PROMOCION = U_SO1_PROMOCION < 0 ? string.Empty : U_SO1_PROMOCION.ToString();
            PromocionValeDescuento.U_SO1_PROMOCADENA = string.IsNullOrEmpty(PromocionValeDescuento.U_SO1_PROMOCADENA) ? string.Empty : PromocionValeDescuento.U_SO1_PROMOCADENA;
            PromocionValeDescuento.U_SO1_COMPORTAMIENTO = string.IsNullOrEmpty(PromocionValeDescuento.U_SO1_COMPORTAMIENTO) ? string.Empty : PromocionValeDescuento.U_SO1_COMPORTAMIENTO;
            PromocionValeDescuento.U_SO1_MONEDA = string.IsNullOrEmpty(PromocionValeDescuento.U_SO1_MONEDA) ? string.Empty : PromocionValeDescuento.U_SO1_MONEDA;
            PromocionValeDescuento.U_SO1_MONTOMINIMO = PromocionValeDescuento.U_SO1_MONTOMINIMO < 0 ? 0 : PromocionValeDescuento.U_SO1_MONTOMINIMO;
            PromocionValeDescuento.U_SO1_TIPODESCUENTO = string.IsNullOrEmpty(PromocionValeDescuento.U_SO1_TIPODESCUENTO) ? string.Empty : PromocionValeDescuento.U_SO1_TIPODESCUENTO;
            PromocionValeDescuento.U_SO1_IMPORTEDESC = PromocionValeDescuento.U_SO1_IMPORTEDESC < 0 ? 0 : PromocionValeDescuento.U_SO1_IMPORTEDESC;
            PromocionValeDescuento.U_SO1_FECHAVENCI = string.IsNullOrEmpty(PromocionValeDescuento.U_SO1_FECHAVENCI) ? string.Empty : PromocionValeDescuento.U_SO1_FECHAVENCI;
            PromocionValeDescuento.U_SO1_DIASVENCI = PromocionValeDescuento.U_SO1_DIASVENCI < 0 ? 0 : PromocionValeDescuento.U_SO1_DIASVENCI;
            PromocionValeDescuento.U_SO1_POLITICAREDEN = string.IsNullOrEmpty(PromocionValeDescuento.U_SO1_POLITICAREDEN) ? string.Empty : PromocionValeDescuento.U_SO1_POLITICAREDEN;

        }

        public void validarPoliticaRedencion()
        {

            //U_SO1_PROMOCION = string.IsNullOrEmpty(U_SO1_PROMOCION) ? string.Empty : U_SO1_PROMOCION;
            PromocionPoliticaRedencion.Code = string.IsNullOrEmpty(PromocionPoliticaRedencion.Code) ? string.Empty : PromocionPoliticaRedencion.Code;
            PromocionPoliticaRedencion.Name = string.IsNullOrEmpty(PromocionPoliticaRedencion.Name) ? string.Empty : PromocionPoliticaRedencion.Name;
            PromocionPoliticaRedencion.U_SO1_POLITICAREDEN = PromocionPoliticaRedencion.U_SO1_POLITICAREDEN < 0 ? 0 : PromocionPoliticaRedencion.U_SO1_POLITICAREDEN;
            PromocionPoliticaRedencion.U_SO1_TIPO = string.IsNullOrEmpty(PromocionPoliticaRedencion.U_SO1_TIPO) ? string.Empty : PromocionPoliticaRedencion.U_SO1_TIPO;
            PromocionPoliticaRedencion.U_SO1_DIARIO = string.IsNullOrEmpty(PromocionPoliticaRedencion.U_SO1_DIARIO) ? "N" : PromocionPoliticaRedencion.U_SO1_DIARIO;
            PromocionPoliticaRedencion.U_SO1_DIACOMPLETO = string.IsNullOrEmpty(PromocionPoliticaRedencion.U_SO1_DIACOMPLETO) ? "N" : PromocionPoliticaRedencion.U_SO1_DIACOMPLETO;
            PromocionPoliticaRedencion.U_SO1_DIAHORAINI = string.IsNullOrEmpty(PromocionPoliticaRedencion.U_SO1_DIAHORAINI) ? string.Empty : PromocionPoliticaRedencion.U_SO1_DIAHORAINI; //Este campo es INT
            PromocionPoliticaRedencion.U_SO1_DIAHORAFIN = string.IsNullOrEmpty(PromocionPoliticaRedencion.U_SO1_DIAHORAFIN) ? string.Empty : PromocionPoliticaRedencion.U_SO1_DIAHORAFIN; //Este campo es INT
            PromocionPoliticaRedencion.U_SO1_LUNES = string.IsNullOrEmpty(PromocionPoliticaRedencion.U_SO1_LUNES) ? "N" : PromocionPoliticaRedencion.U_SO1_LUNES;
            PromocionPoliticaRedencion.U_SO1_LUNCOMPLETO = string.IsNullOrEmpty(PromocionPoliticaRedencion.U_SO1_LUNCOMPLETO) ? "N" : PromocionPoliticaRedencion.U_SO1_LUNCOMPLETO;
            PromocionPoliticaRedencion.U_SO1_LUNHORAINI = string.IsNullOrEmpty(PromocionPoliticaRedencion.U_SO1_LUNHORAINI) ? string.Empty : PromocionPoliticaRedencion.U_SO1_LUNHORAINI; //Este campo es INT
            PromocionPoliticaRedencion.U_SO1_LUNHORAFIN = string.IsNullOrEmpty(PromocionPoliticaRedencion.U_SO1_LUNHORAFIN) ? string.Empty : PromocionPoliticaRedencion.U_SO1_LUNHORAFIN; //Este campo es INT
            PromocionPoliticaRedencion.U_SO1_MARTES = string.IsNullOrEmpty(PromocionPoliticaRedencion.U_SO1_MARTES) ? "N" : PromocionPoliticaRedencion.U_SO1_MARTES;
            PromocionPoliticaRedencion.U_SO1_MARCOMPLETO = string.IsNullOrEmpty(PromocionPoliticaRedencion.U_SO1_MARCOMPLETO) ? "N" : PromocionPoliticaRedencion.U_SO1_MARCOMPLETO;
            PromocionPoliticaRedencion.U_SO1_MARHORAINI = string.IsNullOrEmpty(PromocionPoliticaRedencion.U_SO1_MARHORAINI) ? string.Empty : PromocionPoliticaRedencion.U_SO1_MARHORAINI; //Este campo es INT
            PromocionPoliticaRedencion.U_SO1_MARHORAFIN = string.IsNullOrEmpty(PromocionPoliticaRedencion.U_SO1_MARHORAFIN) ? string.Empty : PromocionPoliticaRedencion.U_SO1_MARHORAFIN; //Este campo es INT
            PromocionPoliticaRedencion.U_SO1_MIERCOLES = string.IsNullOrEmpty(PromocionPoliticaRedencion.U_SO1_MIERCOLES) ? "N" : PromocionPoliticaRedencion.U_SO1_MIERCOLES;
            PromocionPoliticaRedencion.U_SO1_MIECOMPLETO = string.IsNullOrEmpty(PromocionPoliticaRedencion.U_SO1_MIECOMPLETO) ? "N" : PromocionPoliticaRedencion.U_SO1_MIECOMPLETO;
            PromocionPoliticaRedencion.U_SO1_MIEHORAINI = string.IsNullOrEmpty(PromocionPoliticaRedencion.U_SO1_MIEHORAINI) ? string.Empty : PromocionPoliticaRedencion.U_SO1_MIEHORAINI; //Este campo es INT
            PromocionPoliticaRedencion.U_SO1_MIEHORAFIN = string.IsNullOrEmpty(PromocionPoliticaRedencion.U_SO1_MIEHORAFIN) ? string.Empty : PromocionPoliticaRedencion.U_SO1_MIEHORAFIN; //Este campo es INT
            PromocionPoliticaRedencion.U_SO1_JUEVES = string.IsNullOrEmpty(PromocionPoliticaRedencion.U_SO1_JUEVES) ? "N" : PromocionPoliticaRedencion.U_SO1_JUEVES;
            PromocionPoliticaRedencion.U_SO1_JUECOMPLETO = string.IsNullOrEmpty(PromocionPoliticaRedencion.U_SO1_JUECOMPLETO) ? "N" : PromocionPoliticaRedencion.U_SO1_JUECOMPLETO;
            PromocionPoliticaRedencion.U_SO1_JUEHORAINI = string.IsNullOrEmpty(PromocionPoliticaRedencion.U_SO1_JUEHORAINI) ? string.Empty : PromocionPoliticaRedencion.U_SO1_JUEHORAINI; //Este campo es INT
            PromocionPoliticaRedencion.U_SO1_JUEHORAFIN = string.IsNullOrEmpty(PromocionPoliticaRedencion.U_SO1_JUEHORAFIN) ? string.Empty : PromocionPoliticaRedencion.U_SO1_JUEHORAFIN; //Este campo es INT
            PromocionPoliticaRedencion.U_SO1_VIERNES = string.IsNullOrEmpty(PromocionPoliticaRedencion.U_SO1_VIERNES) ? "N" : PromocionPoliticaRedencion.U_SO1_VIERNES;
            PromocionPoliticaRedencion.U_SO1_VIECOMPLETO = string.IsNullOrEmpty(PromocionPoliticaRedencion.U_SO1_VIECOMPLETO) ? "N" : PromocionPoliticaRedencion.U_SO1_VIECOMPLETO;
            PromocionPoliticaRedencion.U_SO1_VIEHORAINI = string.IsNullOrEmpty(PromocionPoliticaRedencion.U_SO1_VIEHORAINI) ? string.Empty : PromocionPoliticaRedencion.U_SO1_VIEHORAINI; //Este campo es INT
            PromocionPoliticaRedencion.U_SO1_VIEHORAFIN = string.IsNullOrEmpty(PromocionPoliticaRedencion.U_SO1_VIEHORAFIN) ? string.Empty : PromocionPoliticaRedencion.U_SO1_VIEHORAFIN; //Este campo es INT
            PromocionPoliticaRedencion.U_SO1_SABADO = string.IsNullOrEmpty(PromocionPoliticaRedencion.U_SO1_SABADO) ? "N" : PromocionPoliticaRedencion.U_SO1_SABADO;
            PromocionPoliticaRedencion.U_SO1_SABCOMPLETO = string.IsNullOrEmpty(PromocionPoliticaRedencion.U_SO1_SABCOMPLETO) ? "N" : PromocionPoliticaRedencion.U_SO1_SABCOMPLETO;
            PromocionPoliticaRedencion.U_SO1_SABHORAINI = string.IsNullOrEmpty(PromocionPoliticaRedencion.U_SO1_SABHORAINI) ? string.Empty : PromocionPoliticaRedencion.U_SO1_SABHORAINI; //Este campo es INT
            PromocionPoliticaRedencion.U_SO1_SABHORAFIN = string.IsNullOrEmpty(PromocionPoliticaRedencion.U_SO1_SABHORAFIN) ? string.Empty : PromocionPoliticaRedencion.U_SO1_SABHORAFIN; //Este campo es INT
            PromocionPoliticaRedencion.U_SO1_DOMINGO = string.IsNullOrEmpty(PromocionPoliticaRedencion.U_SO1_DOMINGO) ? "N" : PromocionPoliticaRedencion.U_SO1_DOMINGO;
            PromocionPoliticaRedencion.U_SO1_DOMCOMPLETO = string.IsNullOrEmpty(PromocionPoliticaRedencion.U_SO1_DOMCOMPLETO) ? "N" : PromocionPoliticaRedencion.U_SO1_DOMCOMPLETO;
            PromocionPoliticaRedencion.U_SO1_DOMHORAINI = string.IsNullOrEmpty(PromocionPoliticaRedencion.U_SO1_DOMHORAINI) ? string.Empty : PromocionPoliticaRedencion.U_SO1_DOMHORAINI;  //Este campo es INT
            PromocionPoliticaRedencion.U_SO1_DOMHORAFIN = string.IsNullOrEmpty(PromocionPoliticaRedencion.U_SO1_DOMHORAFIN) ? string.Empty : PromocionPoliticaRedencion.U_SO1_DOMHORAFIN;  //Este campo es INT
            PromocionPoliticaRedencion.U_SO1_ARTICULOENLACE = string.IsNullOrEmpty(PromocionPoliticaRedencion.U_SO1_ARTICULOENLACE) ? "O" : PromocionPoliticaRedencion.U_SO1_ARTICULOENLACE;
            PromocionPoliticaRedencion.U_SO1_FILTRARARTART = string.IsNullOrEmpty(PromocionPoliticaRedencion.U_SO1_FILTRARARTART) ? "N" : PromocionPoliticaRedencion.U_SO1_FILTRARARTART;
            PromocionPoliticaRedencion.U_SO1_FILTRARARTGRU = string.IsNullOrEmpty(PromocionPoliticaRedencion.U_SO1_FILTRARARTGRU) ? "N" : PromocionPoliticaRedencion.U_SO1_FILTRARARTGRU;
            PromocionPoliticaRedencion.U_SO1_FILTRARARTPRO = string.IsNullOrEmpty(PromocionPoliticaRedencion.U_SO1_FILTRARARTPRO) ? "N" : PromocionPoliticaRedencion.U_SO1_FILTRARARTPRO;
            PromocionPoliticaRedencion.U_SO1_FILTRARARTPROE = string.IsNullOrEmpty(PromocionPoliticaRedencion.U_SO1_FILTRARARTPROE) ? "O" : PromocionPoliticaRedencion.U_SO1_FILTRARARTPROE;
            PromocionPoliticaRedencion.U_SO1_FILTRARARTPROC = string.IsNullOrEmpty(PromocionPoliticaRedencion.U_SO1_FILTRARARTPROC) ? "N" : PromocionPoliticaRedencion.U_SO1_FILTRARARTPROC;
            PromocionPoliticaRedencion.U_SO1_FILTRARARTCOE = string.IsNullOrEmpty(PromocionPoliticaRedencion.U_SO1_FILTRARARTCOE) ? "N" : PromocionPoliticaRedencion.U_SO1_FILTRARARTCOE;
            PromocionPoliticaRedencion.U_SO1_FILTRARARTCOEC = string.IsNullOrEmpty(PromocionPoliticaRedencion.U_SO1_FILTRARARTCOEC) ? string.Empty : PromocionPoliticaRedencion.U_SO1_FILTRARARTCOEC;
            PromocionPoliticaRedencion.U_SO1_FILTRARARTCOEV = string.IsNullOrEmpty(PromocionPoliticaRedencion.U_SO1_FILTRARARTCOEV) ? string.Empty : PromocionPoliticaRedencion.U_SO1_FILTRARARTCOEV;
            PromocionPoliticaRedencion.U_SO1_FILTRARARTPROV = string.IsNullOrEmpty(PromocionPoliticaRedencion.U_SO1_FILTRARARTPROV) ? "N" : PromocionPoliticaRedencion.U_SO1_FILTRARARTPROV;
            PromocionPoliticaRedencion.U_SO1_FILTRARARTFAB = string.IsNullOrEmpty(PromocionPoliticaRedencion.U_SO1_FILTRARARTFAB) ? "N" : PromocionPoliticaRedencion.U_SO1_FILTRARARTFAB;
            PromocionPoliticaRedencion.U_SO1_FILTRARARTEXC = string.IsNullOrEmpty(PromocionPoliticaRedencion.U_SO1_FILTRARARTEXC) ? "N" : PromocionPoliticaRedencion.U_SO1_FILTRARARTEXC;
            PromocionPoliticaRedencion.U_SO1_FILTRARARTUNE = string.IsNullOrEmpty(PromocionPoliticaRedencion.U_SO1_FILTRARARTUNE) ? "N" : PromocionPoliticaRedencion.U_SO1_FILTRARARTUNE;
            PromocionPoliticaRedencion.U_SO1_FILTRARPROMSUC = string.IsNullOrEmpty(PromocionPoliticaRedencion.U_SO1_FILTRARPROMSUC) ? "N" : PromocionPoliticaRedencion.U_SO1_FILTRARPROMSUC;
            PromocionPoliticaRedencion.U_SO1_FILTRARPROMALI = string.IsNullOrEmpty(PromocionPoliticaRedencion.U_SO1_FILTRARPROMALI) ? "N" : PromocionPoliticaRedencion.U_SO1_FILTRARPROMALI;
            PromocionPoliticaRedencion.U_SO1_FILTRARPROMCLI = string.IsNullOrEmpty(PromocionPoliticaRedencion.U_SO1_FILTRARPROMCLI) ? "N" : PromocionPoliticaRedencion.U_SO1_FILTRARPROMCLI;
            PromocionPoliticaRedencion.U_SO1_FILTRARCLICLI = string.IsNullOrEmpty(PromocionPoliticaRedencion.U_SO1_FILTRARCLICLI) ? "N" : PromocionPoliticaRedencion.U_SO1_FILTRARCLICLI;
            PromocionPoliticaRedencion.U_SO1_FILTRARCLIENLS = string.IsNullOrEmpty(PromocionPoliticaRedencion.U_SO1_FILTRARCLIENLS) ? "O" : PromocionPoliticaRedencion.U_SO1_FILTRARCLIENLS;
            PromocionPoliticaRedencion.U_SO1_FILTRARCLIGRU = string.IsNullOrEmpty(PromocionPoliticaRedencion.U_SO1_FILTRARCLIGRU) ? "N" : PromocionPoliticaRedencion.U_SO1_FILTRARCLIGRU;
            PromocionPoliticaRedencion.U_SO1_FILTRARCLIPRO = string.IsNullOrEmpty(PromocionPoliticaRedencion.U_SO1_FILTRARCLIPRO) ? "N" : PromocionPoliticaRedencion.U_SO1_FILTRARCLIPRO;
            PromocionPoliticaRedencion.U_SO1_FILTRARCLIPROE = string.IsNullOrEmpty(PromocionPoliticaRedencion.U_SO1_FILTRARCLIPROE) ? "O" : PromocionPoliticaRedencion.U_SO1_FILTRARCLIPROE;
            PromocionPoliticaRedencion.U_SO1_FILTRARCLIPROC = string.IsNullOrEmpty(PromocionPoliticaRedencion.U_SO1_FILTRARCLIPROC) ? "N" : PromocionPoliticaRedencion.U_SO1_FILTRARCLIPROC;
            PromocionPoliticaRedencion.U_SO1_FILTRARCLICOE = string.IsNullOrEmpty(PromocionPoliticaRedencion.U_SO1_FILTRARCLICOE) ? "N" : PromocionPoliticaRedencion.U_SO1_FILTRARCLICOE;
            PromocionPoliticaRedencion.U_SO1_FILTRARCLICOEC = string.IsNullOrEmpty(PromocionPoliticaRedencion.U_SO1_FILTRARCLICOEC) ? string.Empty : PromocionPoliticaRedencion.U_SO1_FILTRARCLICOEC;
            PromocionPoliticaRedencion.U_SO1_FILTRARCLICOEV = string.IsNullOrEmpty(PromocionPoliticaRedencion.U_SO1_FILTRARCLICOEV) ? string.Empty : PromocionPoliticaRedencion.U_SO1_FILTRARCLICOEV;
            PromocionPoliticaRedencion.U_SO1_FILTRARPROMMEM = string.IsNullOrEmpty(PromocionPoliticaRedencion.U_SO1_FILTRARPROMMEM) ? "N" : PromocionPoliticaRedencion.U_SO1_FILTRARPROMMEM;
            PromocionPoliticaRedencion.U_SO1_FILTRARPROMLIS = string.IsNullOrEmpty(PromocionPoliticaRedencion.U_SO1_FILTRARPROMLIS) ? "N" : PromocionPoliticaRedencion.U_SO1_FILTRARPROMLIS;
            PromocionPoliticaRedencion.U_SO1_FILTRARPROMFOP = string.IsNullOrEmpty(PromocionPoliticaRedencion.U_SO1_FILTRARPROMFOP) ? "N" : PromocionPoliticaRedencion.U_SO1_FILTRARPROMFOP;           
            PromocionPoliticaRedencion.U_SO1_ACUMPROMOOTRAS = string.IsNullOrEmpty(PromocionPoliticaRedencion.U_SO1_ACUMPROMOOTRAS) ? "N" : PromocionPoliticaRedencion.U_SO1_ACUMPROMOOTRAS;
        }

        #endregion

        #endregion

    }

    #region Consultas

    internal static class ConsultasPromocion
    {
        #region Consultas promocion

        internal static Consulta Listar = new Consulta(Proveedor.SQLServer,
             new ComandoConsulta(Proveedor.SQLServer, @"SELECT *
                                                     FROM [@SO1_01PROMOCION] ${CONDICION} Order By U_SO1_JERARQUIA "),
             new ComandoConsulta(Proveedor.Hana, @"SELECT *
                                                     FROM ""@SO1_01PROMOCION"" ${CONDICION} Order By ""U_SO1_JERARQUIA"" "));
        internal static Consulta ListarFiltrados = new Consulta(Proveedor.SQLServer,
         new ComandoConsulta(Proveedor.SQLServer, @"SELECT P.""Code"", P.""Name"", P.""U_SO1_PROMOCION"", 
                                                        P.""U_SO1_PROMOCION"", P.""U_SO1_JERARQUIA"",
                                                        CASE
                                                            WHEN LEFT(P.""Code"",4) = 'DPA-' THEN 'DPA'
                                                            ELSE P.""U_SO1_TIPO""
                                                        END ""TIPO"",
                                                        P.""U_SO1_FECHADESDE"", P.""U_SO1_FECHAHASTA""
                                                        FROM ""@SO1_01PROMOCION"" P 
                                                        LEFT JOIN ""@SO1_01PROMOSUCURSAL"" PS ON P.""U_SO1_PROMOCION"" = PS.""U_SO1_PROMOCION""
                                                        LEFT JOIN ""@SO1_01PROMOCLIENTE"" PC ON P.""U_SO1_PROMOCION"" = PC.""U_SO1_PROMOCION""
                                                        LEFT JOIN ""@SO1_01PROMOARTIGRUP"" PG ON P.""U_SO1_PROMOCION"" = PG.""U_SO1_PROMOCION""
                                                        LEFT JOIN ""@SO1_01PROMOFORMAPAG"" PFP ON P.""U_SO1_PROMOCION"" = PFP.""U_SO1_PROMOCION""
                                                        WHERE (@pTipo IS NULL OR P.""U_SO1_TIPO"" IN (${lTipo}) OR LEFT(P.""Code"",3) IN (${lTipo})) 
                                                        AND (@pFechaInicio IS NULL OR P.""U_SO1_FECHADESDE"" BETWEEN @pFechaInicio AND @pFechaFin)
                                                        AND (@pFechaFin IS NULL OR P.""U_SO1_FECHAHASTA"" BETWEEN @pFechaInicio AND @pFechaFin)
                                                        AND (@pVigente IS NULL OR (P.""U_SO1_FECHADESDE"" <= @pVigente AND P.""U_SO1_FECHAHASTA"" >= @pVigente))
                                                        AND (@pCaducada IS NULL OR (P.""U_SO1_FECHADESDE"" <= @pCaducada AND P.""U_SO1_FECHAHASTA"" < @pCaducada))
                                                        AND (@pSucursal  IS NULL OR PS.""U_SO1_SUCURSAL"" IN (${lSucursal}))
                                                        AND (@pCliente IS NULL OR PC.""U_SO1_CLIENTE"" IN (${lCliente}))
                                                        AND (@pGrupoArticulo IS NULL OR PG.""U_SO1_GRUPO"" IN (${lGrupoArticulo}))
                                                        AND (@pFormaPago IS NULL OR PFP.""U_SO1_FORMAPAGO"" IN (${lFormaPago}))
                                                        AND P.U_SO1_TIPO IN (${lPromociones}) OR LEFT(P.""Code"",3) IN (${lPromociones})
                                                        GROUP BY P.""Code"", P.""Name"", P.""U_SO1_PROMOCION"",
                                                        P.""U_SO1_PROMOCION"", P.""U_SO1_JERARQUIA"", P.""U_SO1_TIPO"",
                                                        P.""U_SO1_FECHADESDE"", P.""U_SO1_FECHAHASTA""
                                                        ORDER BY P.""U_SO1_JERARQUIA"" "),
         new ComandoConsulta(Proveedor.Hana, @"SELECT   P.""Code"", P.""Name"", P.""U_SO1_PROMOCION"", 
                                                        P.""U_SO1_PROMOCION"", P.""U_SO1_JERARQUIA"", 
                                                        CASE
                                                            WHEN LEFT(P.""Code"",4) = 'DPA-' THEN 'DPA'
                                                            ELSE P.""U_SO1_TIPO""
                                                        END ""TIPO"",
                                                        P.""U_SO1_FECHADESDE"", P.""U_SO1_FECHAHASTA""
                                                        FROM ""@SO1_01PROMOCION"" P 
                                                        LEFT JOIN ""@SO1_01PROMOSUCURSAL"" PS ON P.""U_SO1_PROMOCION"" = PS.""U_SO1_PROMOCION""
                                                        LEFT JOIN ""@SO1_01PROMOCLIENTE"" PC ON P.""U_SO1_PROMOCION"" = PC.""U_SO1_PROMOCION""
                                                        LEFT JOIN ""@SO1_01PROMOARTIGRUP"" PG ON P.""U_SO1_PROMOCION"" = PG.""U_SO1_PROMOCION""
                                                        LEFT JOIN ""@SO1_01PROMOFORMAPAG"" PFP ON P.""U_SO1_PROMOCION"" = PFP.""U_SO1_PROMOCION""
                                                        WHERE (@pTipo IS NULL OR P.""U_SO1_TIPO"" IN (${lTipo}) OR LEFT(P.""Code"",3) IN (${lTipo})) 
                                                        AND (@pFechaInicio IS NULL OR P.""U_SO1_FECHADESDE"" BETWEEN @pFechaInicio AND @pFechaFin)
                                                        AND (@pFechaFin IS NULL OR P.""U_SO1_FECHAHASTA"" BETWEEN @pFechaInicio AND @pFechaFin)
                                                        AND (@pVigente IS NULL OR (P.""U_SO1_FECHADESDE"" <= @pVigente AND P.""U_SO1_FECHAHASTA"" >= @pVigente))
                                                        AND (@pCaducada IS NULL OR (P.""U_SO1_FECHADESDE"" <= @pCaducada AND P.""U_SO1_FECHAHASTA"" < @pCaducada))                                                        
                                                        AND (@pSucursal  IS NULL OR PS.""U_SO1_SUCURSAL"" IN (${lSucursal}))
                                                        AND (@pCliente IS NULL OR PC.""U_SO1_CLIENTE"" IN (${lCliente}))
                                                        AND (@pGrupoArticulo IS NULL OR PG.""U_SO1_GRUPO"" IN (${lGrupoArticulo}))
                                                        AND (@pFormaPago IS NULL OR PFP.""U_SO1_FORMAPAGO"" IN (${lFormaPago}))
                                                        AND P.""U_SO1_TIPO"" IN (${lPromociones}) OR LEFT(P.""Code"",3) IN (${lPromociones})
                                                        GROUP BY P.""Code"", P.""Name"", P.""U_SO1_PROMOCION"",
                                                        P.""U_SO1_PROMOCION"", P.""U_SO1_JERARQUIA"", P.""U_SO1_TIPO"",
                                                        P.""U_SO1_FECHADESDE"", P.""U_SO1_FECHAHASTA""
                                                        ORDER BY P.""U_SO1_JERARQUIA"" "));

        internal static Consulta Consultar = new Consulta(Proveedor.SQLServer,
            new ComandoConsulta(Proveedor.SQLServer, @"SELECT * FROM [@SO1_01PROMOCION] WHERE Code = @pCodigo OR Name = @pNombre "),
            new ComandoConsulta(Proveedor.Hana, @"SELECT * FROM ""@SO1_01PROMOCION"" WHERE ""Code"" = @pCodigo OR ""Name"" = @pNombre "));

        internal static Consulta Registrar = new Consulta(Proveedor.SQLServer,
            new ComandoConsulta(Proveedor.SQLServer, @"INSERT INTO [@SO1_01PROMOCION] ([Code], [Name], [U_SO1_PROMOCION], [U_SO1_JERARQUIA], [U_SO1_TIPO],
                                                      [U_SO1_FECHADESDE], [U_SO1_FECHAHASTA],[U_SO1_DIARIO],
                                                      [U_SO1_DIACOMPLETO], [U_SO1_DIAHORAINI], [U_SO1_DIAHORAFIN], [U_SO1_LUNES], [U_SO1_LUNCOMPLETO],  
                                                      [U_SO1_LUNHORAINI], [U_SO1_LUNHORAFIN], [U_SO1_MARTES], [U_SO1_MARCOMPLETO], [U_SO1_MARHORAINI],
                                                      [U_SO1_MARHORAFIN], [U_SO1_MIERCOLES], [U_SO1_MIECOMPLETO], [U_SO1_MIEHORAINI], [U_SO1_MIEHORAFIN],
                                                      [U_SO1_JUEVES], [U_SO1_JUECOMPLETO], [U_SO1_JUEHORAINI], [U_SO1_JUEHORAFIN], [U_SO1_VIERNES],  
                                                      [U_SO1_VIECOMPLETO], [U_SO1_VIEHORAINI], [U_SO1_VIEHORAFIN], [U_SO1_SABADO], [U_SO1_SABCOMPLETO],
                                                      [U_SO1_SABHORAINI], [U_SO1_SABHORAFIN], [U_SO1_DOMINGO], [U_SO1_DOMCOMPLETO], [U_SO1_DOMHORAINI],
                                                      [U_SO1_DOMHORAFIN], [U_SO1_ARTICULOENLACE], [U_SO1_FILTRARARTART], [U_SO1_FILTRARARTGRU],
                                                      [U_SO1_FILTRARARTPRO], [U_SO1_FILTRARARTPROE], [U_SO1_FILTRARARTPROC], [U_SO1_FILTRARARTCOE], 
                                                      [U_SO1_FILTRARARTCOEC], [U_SO1_FILTRARARTCOEV], [U_SO1_FILTRARARTPROV], [U_SO1_FILTRARARTFAB], 
                                                      [U_SO1_FILTRARARTEXC], [U_SO1_FILTRARARTUNE],  [U_SO1_FILTRARPROMSUC], [U_SO1_FILTRARPROMALI],
                                                      [U_SO1_FILTRARPROMCLI], [U_SO1_FILTRARCLICLI], [U_SO1_FILTRARCLIENLS], [U_SO1_FILTRARCLIGRU], 
                                                      [U_SO1_FILTRARCLIPRO], [U_SO1_FILTRARCLIPROE], [U_SO1_FILTRARCLIPROC], [U_SO1_FILTRARCLICOE],
                                                      [U_SO1_FILTRARCLICOEC], [U_SO1_FILTRARCLICOEV], [U_SO1_FILTRARPROMMEM], [U_SO1_FILTRARPROMLIS],
                                                      [U_SO1_FILTRARPROMFOP], [U_SO1_ACUMPROMORM], [U_SO1_ACUMPROMOPUNTO], [U_SO1_ACUMPROMOOTRAS], 
                                                      [U_SO1_ACTLIMPIEZAVEN], [U_SO1_LIMITEPIEZAVEN], [U_SO1_ACTLIMPIEZAPRO], 
                                                      [U_SO1_LIMITEPIEZAPRO], [U_SO1_COMPORTAESCAUT], [U_SO1_ACTLIMVENTAPRO],
                                                      [U_SO1_LIMITEVENTAPRO], [U_SO1_ACTVENTACREDIT], [U_SO1_FILTRARARTCON], 
                                                      [U_SO1_FILTRARARTCONV]) VALUES (@pCodigo, @pNombre, @pPromo, @pJerarquia, @pTipo, @pFechaDesde, @pFechaHasta, @pDiario, @pDiaCompleto, @pDiaHoraIni, @pDiaHoraFin, @pLunes, @pLunesCompleto,
                                                       @pLunHoraIni, @pLunHoraFin, @pMartes, @pMartesCompleto, @pMarHoraIni, @pMarHoraFin, @pMiercoles, @pMieCompleto,  @pMieHoraIni,
                                                       @pMieHoraFin, @pJueves, @pJueCompleto, @pJueHoraIni, @pJueHoraFin, @pViernes, @pVieCompleto, @pVieHoraIni, @pVieHoraFin,
                                                       @pSabado, @pSabCompleto, @pSabHoraIni, @pSabHoraFin, @pDomingo, @pDomCompleto, @pDomHoraIni, @pDomHoraFin,  @pArticuloEnlace,
                                                       @pFiltrarArtArt, @pFiltrarArtGru, @pFiltrarArtPro, @pFiltrarArtProe,  @pFiltrarArtProc,  @pFiltrarArtCoe, @pFiltrarArtCoec,
                                                       @pFiltrarArtCoev, @pFiltrarArtProv, @pFiltrarArtFab, @pFiltrarArtExc, @pFiltrarArtUne, @pFiltrarPromSuc, @pFiltrarPromAli,
                                                       @pFiltrarPromCli, @pFiltrarCliCli,  @pFiltrarCliEnls, @pFiltrarCliGru, @pFiltrarCliPro, @pFiltrarCliProe, @pFiltrarCliProc,
                                                       @pFiltrarCliCoe, @pFiltrarCliCoec, @pFiltrarCliCoev, @pFiltrarPromMem, @pFiltrarPromLis, @pFiltrarPromFop, @pAcumProMorm, @pAcumPromoPunto,
                                                       @pAcumPromoOtras, @pActLimpiezaVen, @pLimitePiezaVen, @pActLimpiezaPro, @pLimitePiezaPro, @pComportaEscaut, @pActLimVentaPro,
                                                       @pLimiteVentaPro, @pActVentaCredit, @pFiltrarArtCon, @pFiltrarArtConv)"),
            new ComandoConsulta(Proveedor.Hana, @"INSERT INTO ""@SO1_01PROMOCION"" (""Code"", ""Name"", ""U_SO1_PROMOCION"", ""U_SO1_JERARQUIA"", ""U_SO1_TIPO"",
                                                      ""U_SO1_FECHADESDE"", ""U_SO1_FECHAHASTA"",""U_SO1_DIARIO"",
                                                      ""U_SO1_DIACOMPLETO"", ""U_SO1_DIAHORAINI"", ""U_SO1_DIAHORAFIN"", ""U_SO1_LUNES"", ""U_SO1_LUNCOMPLETO"",  
                                                      ""U_SO1_LUNHORAINI"", ""U_SO1_LUNHORAFIN"", ""U_SO1_MARTES"", ""U_SO1_MARCOMPLETO"", ""U_SO1_MARHORAINI"",
                                                      ""U_SO1_MARHORAFIN"", ""U_SO1_MIERCOLES"", ""U_SO1_MIECOMPLETO"", ""U_SO1_MIEHORAINI"", ""U_SO1_MIEHORAFIN"",
                                                      ""U_SO1_JUEVES"", ""U_SO1_JUECOMPLETO"", ""U_SO1_JUEHORAINI"", ""U_SO1_JUEHORAFIN"", ""U_SO1_VIERNES"",  
                                                      ""U_SO1_VIECOMPLETO"", ""U_SO1_VIEHORAINI"", ""U_SO1_VIEHORAFIN"", ""U_SO1_SABADO"", ""U_SO1_SABCOMPLETO"",
                                                      ""U_SO1_SABHORAINI"", ""U_SO1_SABHORAFIN"", ""U_SO1_DOMINGO"", ""U_SO1_DOMCOMPLETO"", ""U_SO1_DOMHORAINI"",
                                                      ""U_SO1_DOMHORAFIN"", ""U_SO1_ARTICULOENLACE"", ""U_SO1_FILTRARARTART"", ""U_SO1_FILTRARARTGRU"",
                                                      ""U_SO1_FILTRARARTPRO"", ""U_SO1_FILTRARARTPROE"", ""U_SO1_FILTRARARTPROC"", ""U_SO1_FILTRARARTCOE"", 
                                                      ""U_SO1_FILTRARARTCOEC"", ""U_SO1_FILTRARARTCOEV"", ""U_SO1_FILTRARARTPROV"", ""U_SO1_FILTRARARTFAB"", 
                                                      ""U_SO1_FILTRARARTEXC"", ""U_SO1_FILTRARARTUNE"",  ""U_SO1_FILTRARPROMSUC"", ""U_SO1_FILTRARPROMALI"",
                                                      ""U_SO1_FILTRARPROMCLI"", ""U_SO1_FILTRARCLICLI"", ""U_SO1_FILTRARCLIENLS"", ""U_SO1_FILTRARCLIGRU"", 
                                                      ""U_SO1_FILTRARCLIPRO"", ""U_SO1_FILTRARCLIPROE"", ""U_SO1_FILTRARCLIPROC"", ""U_SO1_FILTRARCLICOE"",
                                                      ""U_SO1_FILTRARCLICOEC"", ""U_SO1_FILTRARCLICOEV"", ""U_SO1_FILTRARPROMMEM"", ""U_SO1_FILTRARPROMLIS"",
                                                      ""U_SO1_FILTRARPROMFOP"", ""U_SO1_ACUMPROMORM"", ""U_SO1_ACUMPROMOPUNTO"", ""U_SO1_ACUMPROMOOTRAS"", 
                                                      ""U_SO1_ACTLIMPIEZAVEN"", ""U_SO1_LIMITEPIEZAVEN"", ""U_SO1_ACTLIMPIEZAPRO"", 
                                                      ""U_SO1_LIMITEPIEZAPRO"", ""U_SO1_COMPORTAESCAUT"", ""U_SO1_ACTLIMVENTAPRO"",
                                                      ""U_SO1_LIMITEVENTAPRO"", ""U_SO1_ACTVENTACREDIT"", ""U_SO1_FILTRARARTCON"", 
                                                      ""U_SO1_FILTRARARTCONV"") VALUES (@pCodigo, @pNombre, @pPromo, @pJerarquia, @pTipo, @pFechaDesde, @pFechaHasta, @pDiario, @pDiaCompleto, @pDiaHoraIni, @pDiaHoraFin, @pLunes, @pLunesCompleto,
                                                       @pLunHoraIni, @pLunHoraFin, @pMartes, @pMartesCompleto, @pMarHoraIni, @pMarHoraFin, @pMiercoles, @pMieCompleto,  @pMieHoraIni,
                                                       @pMieHoraFin, @pJueves, @pJueCompleto, @pJueHoraIni, @pJueHoraFin, @pViernes, @pVieCompleto, @pVieHoraIni, @pVieHoraFin,
                                                       @pSabado, @pSabCompleto, @pSabHoraIni, @pSabHoraFin, @pDomingo, @pDomCompleto, @pDomHoraIni, @pDomHoraFin,  @pArticuloEnlace,
                                                       @pFiltrarArtArt, @pFiltrarArtGru, @pFiltrarArtPro, @pFiltrarArtProe,  @pFiltrarArtProc,  @pFiltrarArtCoe, @pFiltrarArtCoec,
                                                       @pFiltrarArtCoev, @pFiltrarArtProv, @pFiltrarArtFab, @pFiltrarArtExc, @pFiltrarArtUne, @pFiltrarPromSuc, @pFiltrarPromAli,
                                                       @pFiltrarPromCli, @pFiltrarCliCli,  @pFiltrarCliEnls, @pFiltrarCliGru, @pFiltrarCliPro, @pFiltrarCliProe, @pFiltrarCliProc,
                                                       @pFiltrarCliCoe, @pFiltrarCliCoec, @pFiltrarCliCoev, @pFiltrarPromMem, @pFiltrarPromLis, @pFiltrarPromFop, @pAcumProMorm, @pAcumPromoPunto,
                                                       @pAcumPromoOtras, @pActLimpiezaVen, @pLimitePiezaVen, @pActLimpiezaPro, @pLimitePiezaPro, @pComportaEscaut, @pActLimVentaPro,
                                                       @pLimiteVentaPro, @pActVentaCredit, @pFiltrarArtCon, @pFiltrarArtConv) "));

        internal static Consulta Modificar = new Consulta(Proveedor.SQLServer,
         new ComandoConsulta(Proveedor.SQLServer, @"UPDATE [@SO1_01PROMOCION] SET [Name] = @pNombre, [U_SO1_FECHADESDE] = @pFechaDesde, [U_SO1_FECHAHASTA] = @pFechaHasta,
                                                      [U_SO1_DIARIO] = @pDiario, [U_SO1_DIACOMPLETO] = @pDiaCompleto, [U_SO1_DIAHORAINI] = @pDiaHoraIni,
                                                      [U_SO1_DIAHORAFIN] = @pDiaHoraFin, [U_SO1_LUNES] = @pLunes, [U_SO1_LUNCOMPLETO] = @pLunesCompleto,
                                                      [U_SO1_LUNHORAINI] = @pLunHoraIni, [U_SO1_LUNHORAFIN] = @pLunHoraFin, [U_SO1_MARTES] = @pMartes,
                                                      [U_SO1_MARCOMPLETO] = @pMartesCompleto, [U_SO1_MARHORAINI] = @pMarHoraIni, [U_SO1_MARHORAFIN] = @pMarHoraFin,
                                                      [U_SO1_MIERCOLES] = @pMiercoles, [U_SO1_MIECOMPLETO] = @pMieCompleto, [U_SO1_MIEHORAINI] = @pMieHoraIni,
                                                      [U_SO1_MIEHORAFIN] = @pMieHoraFin, [U_SO1_JUEVES] = @pJueves, [U_SO1_JUECOMPLETO] = @pJueCompleto,
                                                      [U_SO1_JUEHORAINI] = @pJueHoraIni, [U_SO1_JUEHORAFIN] = @pJueHoraFin, [U_SO1_VIERNES] = @pViernes,
                                                      [U_SO1_VIECOMPLETO] = @pVieCompleto, [U_SO1_VIEHORAINI] = @pVieHoraIni, [U_SO1_VIEHORAFIN] = @pVieHoraFin,
                                                      [U_SO1_SABADO] = @pSabado, [U_SO1_SABCOMPLETO] = @pSabCompleto, [U_SO1_SABHORAINI] = @pSabHoraIni,
                                                      [U_SO1_SABHORAFIN] = @pSabHoraFin, [U_SO1_DOMINGO] = @pDomingo, [U_SO1_DOMCOMPLETO] = @pDomCompleto,
                                                      [U_SO1_DOMHORAINI] = @pDomHoraIni, [U_SO1_DOMHORAFIN] = @pDomHoraFin, [U_SO1_ARTICULOENLACE] = @pArticuloEnlace,
                                                      [U_SO1_FILTRARARTART] = @pFiltrarArtArt, [U_SO1_FILTRARARTGRU] = @pFiltrarArtGru, [U_SO1_FILTRARARTPRO] = @pFiltrarArtPro,
                                                      [U_SO1_FILTRARARTPROE] = @pFiltrarArtProe, [U_SO1_FILTRARARTPROC] = @pFiltrarArtProc, [U_SO1_FILTRARARTCOE] = @pFiltrarArtCoe,
                                                      [U_SO1_FILTRARARTCOEC] = @pFiltrarArtCoec, [U_SO1_FILTRARARTCOEV] = @pFiltrarArtCoev, [U_SO1_FILTRARARTPROV] = @pFiltrarArtProv,
                                                      [U_SO1_FILTRARARTFAB] = @pFiltrarArtFab, [U_SO1_FILTRARARTEXC] = @pFiltrarArtExc, [U_SO1_FILTRARARTUNE] = @pFiltrarArtUne,
                                                      [U_SO1_FILTRARPROMSUC] = @pFiltrarPromSuc, [U_SO1_FILTRARPROMALI] = @pFiltrarPromAli, [U_SO1_FILTRARPROMCLI] = @pFiltrarPromCli,
                                                      [U_SO1_FILTRARCLICLI] = @pFiltrarCliCli, [U_SO1_FILTRARCLIENLS] = @pFiltrarCliEnls, [U_SO1_FILTRARCLIGRU] = @pFiltrarCliGru,
                                                      [U_SO1_FILTRARCLIPRO] = @pFiltrarCliPro, [U_SO1_FILTRARCLIPROE] = @pFiltrarCliProe, [U_SO1_FILTRARCLIPROC] = @pFiltrarCliProc,
                                                      [U_SO1_FILTRARCLICOE] = @pFiltrarCliCoe, [U_SO1_FILTRARCLICOEC] = @pFiltrarCliCoec, [U_SO1_FILTRARCLICOEV] = @pFiltrarCliCoev,
                                                      [U_SO1_FILTRARPROMMEM] = @pFiltrarPromMem, [U_SO1_FILTRARPROMLIS] = @pFiltrarPromLis, [U_SO1_FILTRARPROMFOP] = @pFiltrarPromFop,
                                                      [U_SO1_ACUMPROMORM] = @pAcumProMorm, [U_SO1_ACUMPROMOPUNTO] = @pAcumPromoPunto, [U_SO1_ACUMPROMOOTRAS] = @pAcumPromoOtras,
                                                      [U_SO1_ACTLIMPIEZAVEN] = @pActLimpiezaVen, [U_SO1_LIMITEPIEZAVEN] = @pLimitePiezaVen, [U_SO1_ACTLIMPIEZAPRO] = @pActLimpiezaPro,
                                                      [U_SO1_LIMITEPIEZAPRO] = @pLimitePiezaPro, [U_SO1_COMPORTAESCAUT] = @pComportaEscaut, [U_SO1_ACTLIMVENTAPRO] = @pActLimVentaPro,
                                                      [U_SO1_LIMITEVENTAPRO] = @pLimiteVentaPro, [U_SO1_ACTVENTACREDIT] = @pActVentaCredit, [U_SO1_FILTRARARTCON] = @pFiltrarArtCon,
                                                      [U_SO1_FILTRARARTCONV] = @pFiltrarArtConv 
                                                      WHERE [Code] = @pCodigo"),
         new ComandoConsulta(Proveedor.Hana, @"UPDATE ""@SO1_01PROMOCION"" SET ""Name"" = @pNombre, ""U_SO1_FECHADESDE"" = @pFechaDesde, ""U_SO1_FECHAHASTA"" = @pFechaHasta,
                                                 ""U_SO1_DIARIO"" = @pDiario, ""U_SO1_DIACOMPLETO"" = @pDiaCompleto, ""U_SO1_DIAHORAINI"" = @pDiaHoraIni,
                                                 ""U_SO1_DIAHORAFIN"" = @pDiaHoraFin, ""U_SO1_LUNES"" = @pLunes, ""U_SO1_LUNCOMPLETO"" = @pLunesCompleto,
                                                 ""U_SO1_LUNHORAINI"" = @pLunHoraIni, ""U_SO1_LUNHORAFIN"" = @pLunHoraFin, ""U_SO1_MARTES"" = @pMartes,
                                                 ""U_SO1_MARCOMPLETO"" = @pMartesCompleto, ""U_SO1_MARHORAINI"" = @pMarHoraIni, ""U_SO1_MARHORAFIN"" = @pMarHoraFin,
                                                 ""U_SO1_MIERCOLES"" = @pMiercoles, ""U_SO1_MIECOMPLETO"" = @pMieCompleto, ""U_SO1_MIEHORAINI"" = @pMieHoraIni,
                                                 ""U_SO1_MIEHORAFIN"" = @pMieHoraFin, ""U_SO1_JUEVES"" = @pJueves, ""U_SO1_JUECOMPLETO"" = @pJueCompleto,
                                                 ""U_SO1_JUEHORAINI"" = @pJueHoraIni, ""U_SO1_JUEHORAFIN"" = @pJueHoraFin, ""U_SO1_VIERNES"" = @pViernes,
                                                 ""U_SO1_VIECOMPLETO"" = @pVieCompleto, ""U_SO1_VIEHORAINI"" = @pVieHoraIni, ""U_SO1_VIEHORAFIN"" = @pVieHoraFin,
                                                 ""U_SO1_SABADO"" = @pSabado, ""U_SO1_SABCOMPLETO"" = @pSabCompleto, ""U_SO1_SABHORAINI"" = @pSabHoraIni,
                                                 ""U_SO1_SABHORAFIN"" = @pSabHoraFin, ""U_SO1_DOMINGO"" = @pDomingo, ""U_SO1_DOMCOMPLETO"" = @pDomCompleto,
                                                 ""U_SO1_DOMHORAINI"" = @pDomHoraIni, ""U_SO1_DOMHORAFIN"" = @pDomHoraFin, ""U_SO1_ARTICULOENLACE"" = @pArticuloEnlace,
                                                 ""U_SO1_FILTRARARTART"" = @pFiltrarArtArt, ""U_SO1_FILTRARARTGRU"" = @pFiltrarArtGru, ""U_SO1_FILTRARARTPRO"" = @pFiltrarArtPro,
                                                 ""U_SO1_FILTRARARTPROE"" = @pFiltrarArtProe, ""U_SO1_FILTRARARTPROC"" = @pFiltrarArtProc, ""U_SO1_FILTRARARTCOE"" = @pFiltrarArtCoe,
                                                 ""U_SO1_FILTRARARTCOEC"" = @pFiltrarArtCoec, ""U_SO1_FILTRARARTCOEV"" = @pFiltrarArtCoev, ""U_SO1_FILTRARARTPROV"" = @pFiltrarArtProv,
                                                 ""U_SO1_FILTRARARTFAB"" = @pFiltrarArtFab, ""U_SO1_FILTRARARTEXC"" = @pFiltrarArtExc, ""U_SO1_FILTRARARTUNE"" = @pFiltrarArtUne,
                                                 ""U_SO1_FILTRARPROMSUC"" = @pFiltrarPromSuc, ""U_SO1_FILTRARPROMALI"" = @pFiltrarPromAli, ""U_SO1_FILTRARPROMCLI"" = @pFiltrarPromCli,
                                                 ""U_SO1_FILTRARCLICLI"" = @pFiltrarCliCli, ""U_SO1_FILTRARCLIENLS"" = @pFiltrarCliEnls, ""U_SO1_FILTRARCLIGRU"" = @pFiltrarCliGru,
                                                 ""U_SO1_FILTRARCLIPRO"" = @pFiltrarCliPro, ""U_SO1_FILTRARCLIPROE"" = @pFiltrarCliProe, ""U_SO1_FILTRARCLIPROC"" = @pFiltrarCliProc,
                                                 ""U_SO1_FILTRARCLICOE"" = @pFiltrarCliCoe, ""U_SO1_FILTRARCLICOEC"" = @pFiltrarCliCoec, ""U_SO1_FILTRARCLICOEV"" = @pFiltrarCliCoev,
                                                 ""U_SO1_FILTRARPROMMEM"" = @pFiltrarPromMem, ""U_SO1_FILTRARPROMLIS"" = @pFiltrarPromLis, ""U_SO1_FILTRARPROMFOP"" = @pFiltrarPromFop,
                                                 ""U_SO1_ACUMPROMORM"" = @pAcumProMorm, ""U_SO1_ACUMPROMOPUNTO"" = @pAcumPromoPunto, ""U_SO1_ACUMPROMOOTRAS"" = @pAcumPromoOtras,
                                                 ""U_SO1_ACTLIMPIEZAVEN"" = @pActLimpiezaVen, ""U_SO1_LIMITEPIEZAVEN"" = @pLimitePiezaVen, ""U_SO1_ACTLIMPIEZAPRO"" = @pActLimpiezaPro,
                                                 ""U_SO1_LIMITEPIEZAPRO"" = @pLimitePiezaPro, ""U_SO1_COMPORTAESCAUT"" = @pComportaEscaut, ""U_SO1_ACTLIMVENTAPRO"" = @pActLimVentaPro,
                                                 ""U_SO1_LIMITEVENTAPRO"" = @pLimiteVentaPro, ""U_SO1_ACTVENTACREDIT"" = @pActVentaCredit, ""U_SO1_FILTRARARTCON"" = @pFiltrarArtCon,
                                                 ""U_SO1_FILTRARARTCONV"" = @pFiltrarArtConv 
                                                 WHERE ""Code"" = @pCodigo"));

        #endregion

        #region Consultas Sucursal

        internal static Consulta ConsultarSucursales = new Consulta(Proveedor.SQLServer,
        new ComandoConsulta(Proveedor.SQLServer, @"SELECT A.Code CodigoSucursal, A.Name NombreSucursal, B.Code Codigo, B.Name Nombre,
                                                     B.U_SO1_PROMOCADENA PromoCadena, B.U_SO1_PROMOCION Promo, B.U_SO1_ACTIVO Activo,
                                                     T2.ListNum ListaPrecio1, A.U_SO1_LISTPRECIOMINI ListaPrecio2
                                                     FROM [@SO1_01SUCURSAL]A
                                                     INNER JOIN [OCRD] T2 ON A.U_SO1_CLIENTEMOS = T2.CardCode
                                                     LEFT JOIN [@SO1_01PROMOSUCURSAL]B ON (A.Code = B.U_SO1_SUCURSAL
                                                     AND B.U_SO1_PROMOCION = @pPromocion)"),
        new ComandoConsulta(Proveedor.Hana, @"SELECT A.""Code"" ""CodigoSucursal"", A.""Name"" ""NombreSucursal"", B.""Code"" ""Codigo"", 
                                                B.""Name"" ""Nombre"", B.""U_SO1_PROMOCADENA"" ""PromoCadena"", B.""U_SO1_PROMOCION"" ""Promo"", 
                                                B.""U_SO1_ACTIVO"" ""Activo"", B.""U_SO1_ACTIVO"" ""Activo"",
                                                T2.""ListNum"" ""ListaPrecio1"", A.""U_SO1_LISTPRECIOMINI"" ""ListaPrecio2""
                                                FROM ""@SO1_01SUCURSAL""A
                                                INNER JOIN ""OCRD"" T2 ON A.""U_SO1_CLIENTEMOS"" = T2.""CardCode""
                                                LEFT JOIN ""@SO1_01PROMOSUCURSAL""B ON (A.""Code"" = B.""U_SO1_SUCURSAL""
                                                AND B.""U_SO1_PROMOCION"" = @pPromocion)"));

        internal static Consulta RegistrarSucursal = new Consulta(Proveedor.SQLServer,
            new ComandoConsulta(Proveedor.SQLServer, @"INSERT INTO [@SO1_01PROMOSUCURSAL] ([Code], [Name], [U_SO1_PROMOCION], [U_SO1_PROMOCADENA],
                                                         [U_SO1_SUCURSAL],[U_SO1_ACTIVO]) 
                                                         VALUES (@pCode, @pName, @pPromo, @pPromoCadena, @pSucursal, @pActivo)"),
            new ComandoConsulta(Proveedor.Hana, @"INSERT INTO [@SO1_01PROMOSUCURSAL] ([Code], [Name], [U_SO1_PROMOCION], [U_SO1_PROMOCADENA],
                                                    [U_SO1_SUCURSAL],[U_SO1_ACTIVO]) 
                                                    VALUES (@pCode, @pName, @pPromo, @pPromoCadena, @pSucursal, @pActivo)"));

        internal static Consulta ModificarSucursal = new Consulta(Proveedor.SQLServer,
        new ComandoConsulta(Proveedor.SQLServer, @"UPDATE [@SO1_01PROMOSUCURSAL] SET  [U_SO1_ACTIVO] = @pActivo WHERE [Code] = @pCode "),
        new ComandoConsulta(Proveedor.Hana, @"UPDATE [@SO1_01PROMOSUCURSAL] SET  [U_SO1_ACTIVO] = @pActivo WHERE [Code] = @pCode "));

        internal static Consulta ConsultarSucursalesActivas = new Consulta(Proveedor.SQLServer,
            new ComandoConsulta(Proveedor.SQLServer, @"SELECT T2.ListNum ListaPrecio1, A.U_SO1_LISTPRECIOMINI ListaPrecio2
                                                  FROM [@SO1_01PROMOSUCURSAL] PROM
                                                  INNER JOIN [@SO1_01SUCURSAL] A ON A.Code = PROM.U_SO1_SUCURSAL
                                                  INNER JOIN [OCRD] T2 ON A.U_SO1_CLIENTEMOS = T2.CardCode
                                                  WHERE PROM.U_SO1_PROMOCION = @pPromocion"),
            new ComandoConsulta(Proveedor.Hana, 
                                              @"SELECT T2.""ListNum"" ""ListaPrecio1"", A.""U_SO1_LISTPRECIOMINI"" ""ListaPrecio2""
                                              FROM ""@SO1_01PROMOSUCURSAL"" PROM
                                              INNER JOIN ""@SO1_01SUCURSAL"" A ON A.""Code"" = PROM.""U_SO1_SUCURSAL""
                                              INNER JOIN ""OCRD"" T2 ON A.""U_SO1_CLIENTEMOS"" = T2.""CardCode""
                                              WHERE PROM.""U_SO1_PROMOCION"" = @pPromocion"));

        #endregion

        #region Consultas Alianzas

        internal static Consulta ConsultarAlianzas = new Consulta(Proveedor.SQLServer,
        new ComandoConsulta(Proveedor.SQLServer, @"SELECT A.U_SO1_NOMBRE NombreAlianza, B.Code Codigo, B.Name Nombre, B.U_SO1_PROMOCION Promocion, B.U_SO1_PROMOCADENA PromoCadena, 
                                                     A.Code Alianza, B.U_SO1_ACTIVO Activo
                                                     FROM [@SO1_01ALIANZACOMER] A
                                                     LEFT JOIN [@SO1_01PROMOALICOMER] B ON (A.Code = B.U_SO1_ALIANZA 
                                                     AND B.U_SO1_PROMOCION = @pPromocion) "),
        new ComandoConsulta(Proveedor.Hana, @"SELECT A.""U_SO1_NOMBRE"" ""NombreAlianza"", B.""Code"" ""Codigo"", B.""Name"" ""Nombre"", B.""U_SO1_PROMOCION"" ""Promocion"",
                                                B.""U_SO1_PROMOCADENA"" ""PromoCadena"", A.""Code"" ""Alianza"", B.""U_SO1_ACTIVO"" ""Activo""
                                                FROM ""@SO1_01ALIANZACOMER"" A
                                                LEFT JOIN ""@SO1_01PROMOALICOMER"" B ON (A.""Code"" = B.""U_SO1_ALIANZA"" 
                                                AND B.""U_SO1_PROMOCION"" = @pPromocion) "));

        internal static Consulta RegistrarAlianza = new Consulta(Proveedor.SQLServer,
            new ComandoConsulta(Proveedor.SQLServer, @"INSERT INTO [@SO1_01PROMOALICOMER] ([Code], [Name], [U_SO1_PROMOCION], [U_SO1_PROMOCADENA],
                                                         [U_SO1_ALIANZA],[U_SO1_ACTIVO]) 
                                                         VALUES (@pCode, @pName, @pPromo, @pPromoCadena, @pAlianza, @pActivo)"),
            new ComandoConsulta(Proveedor.Hana, @"INSERT INTO ""@SO1_01PROMOALICOMER"" (""Code"", ""Name"", ""U_SO1_PROMOCION"", ""U_SO1_PROMOCADENA"",
                                                ""U_SO1_ALIANZA"", ""U_SO1_ACTIVO"") 
                                                VALUES (@pCode, @pName, @pPromo, @pPromoCadena, @pAlianza, @pActivo)"));

        internal static Consulta ModificarAlianza = new Consulta(Proveedor.SQLServer,
        new ComandoConsulta(Proveedor.SQLServer, @"UPDATE [@SO1_01PROMOALICOMER] SET  [U_SO1_ACTIVO] = @pActivo WHERE [Code] = @pCode "),
        new ComandoConsulta(Proveedor.Hana, @"UPDATE ""@SO1_01PROMOALICOMER"" SET  ""U_SO1_ACTIVO"" = @pActivo WHERE ""Code"" = @pCode "));

        #endregion

        #region Consultas Membresias

        internal static Consulta ConsultarMembresias = new Consulta(Proveedor.SQLServer,
        new ComandoConsulta(Proveedor.SQLServer, @"SELECT Promo.Code Codigo, Promo.Name Nombre, Promo.U_SO1_PROMOCION Promo, Promo.U_SO1_PROMOCADENA PromoCadena, Tarjeta.Code CodigoTarjeta, 
                                                     Promo.U_SO1_ACTIVO Activo, Tarjeta.Name NombreTarjeta
                                                     FROM [@SO1_01PROMOTIPOMEMB] Promo
                                                     RIGHT JOIN [@SO1_01TARJETAPUNTOS] Tarjeta ON
                                                     (Promo.U_SO1_TIPOMEMBRESIA = Tarjeta.Code
                                                     AND Promo.U_SO1_PROMOCION = @pPromocion)"),
        new ComandoConsulta(Proveedor.Hana, @"SELECT Promo.""Code"" ""Codigo"", Promo.""Name"" ""Nombre"", Promo.""U_SO1_PROMOCION"" ""Promo"", Promo.""U_SO1_PROMOCADENA"" ""PromoCadena"", 
                                                Tarjeta.""Code"" ""CodigoTarjeta"", Promo.""U_SO1_ACTIVO"" ""Activo"", Tarjeta.""Name"" ""NombreTarjeta""
                                                FROM ""@SO1_01PROMOTIPOMEMB"" Promo
                                                RIGHT JOIN ""@SO1_01TARJETAPUNTOS"" Tarjeta ON
                                                (Promo.""U_SO1_TIPOMEMBRESIA"" = Tarjeta.""Code""
                                                AND Promo.""U_SO1_PROMOCION"" = @pPromocion)"));

        internal static Consulta RegistrarMembresia = new Consulta(Proveedor.SQLServer,
           new ComandoConsulta(Proveedor.SQLServer, @"INSERT INTO [@SO1_01PROMOTIPOMEMB] ([Code], [Name], [U_SO1_PROMOCION], [U_SO1_PROMOCADENA],
                                                    [U_SO1_TIPOMEMBRESIA],[U_SO1_ACTIVO]) 
                                                    VALUES (@pCode, @pName, @pPromo, @pPromoCadena, @pMembresia, @pActivo)"),
           new ComandoConsulta(Proveedor.Hana, @"INSERT INTO ""@SO1_01PROMOTIPOMEMB"" (""Code"", ""Name"", ""U_SO1_PROMOCION"", ""U_SO1_PROMOCADENA"",
                                               ""U_SO1_TIPOMEMBRESIA"", ""U_SO1_ACTIVO"") 
                                               VALUES (@pCode, @pName, @pPromo, @pPromoCadena, @pMembresia, @pActivo)"));

        internal static Consulta ModificarMembresia = new Consulta(Proveedor.SQLServer,
        new ComandoConsulta(Proveedor.SQLServer, @"UPDATE [@SO1_01PROMOTIPOMEMB] SET  [U_SO1_ACTIVO] = @pActivo WHERE [Code] = @pCode "),
        new ComandoConsulta(Proveedor.Hana, @"UPDATE ""@SO1_01PROMOTIPOMEMB"" SET  ""U_SO1_ACTIVO"" = @pActivo WHERE ""Code"" = @pCode "));


        #endregion

        #region Consultas Precios

        internal static Consulta ConsultarPrecios = new Consulta(Proveedor.SQLServer,
        new ComandoConsulta(Proveedor.SQLServer, @"SELECT Promo.Code Codigo, Promo.Name Nombre, Promo.U_SO1_PROMOCION Promo, Promo.U_SO1_PROMOCADENA PromoCadena, Precio.ListNum CodigoPrecio, 
                                                     Promo.U_SO1_ACTIVO Activo, Precio.ListName NombrePrecio
                                                     FROM [@SO1_01PROMOLISTPREC] Promo
                                                     RIGHT JOIN [OPLN] Precio ON
                                                     (Promo.U_SO1_LISTAPRECIO = Precio.ListNum
                                                     AND Promo.U_SO1_PROMOCION = @pPromocion) "),
        new ComandoConsulta(Proveedor.Hana, @"SELECT Promo.""Code"" ""Codigo"", Promo.""Name"" ""Nombre"", Promo.""U_SO1_PROMOCION"" ""Promo"", Promo.""U_SO1_PROMOCADENA"" ""PromoCadena"", 
                                                Precio.""ListNum"" ""CodigoPrecio"", Promo.""U_SO1_ACTIVO"" ""Activo"", Precio.""ListName"" ""NombrePrecio""
                                                FROM ""@SO1_01PROMOLISTPREC"" Promo
                                                RIGHT JOIN ""OPLN"" Precio ON
                                                (Promo.""U_SO1_LISTAPRECIO"" = Precio.""ListNum""
                                                AND Promo.""U_SO1_PROMOCION"" = @pPromocion) "));

        internal static Consulta RegistrarPrecio = new Consulta(Proveedor.SQLServer,
           new ComandoConsulta(Proveedor.SQLServer, @"INSERT INTO [@SO1_01PROMOLISTPREC] ([Code], [Name], [U_SO1_PROMOCION], [U_SO1_PROMOCADENA],
                                                         [U_SO1_LISTAPRECIO],[U_SO1_ACTIVO]) 
                                                         VALUES (@pCode, @pName, @pPromo, @pPromoCadena, @pPrecio, @pActivo)"),
           new ComandoConsulta(Proveedor.Hana, @"INSERT INTO ""@SO1_01PROMOLISTPREC"" (""Code"", ""Name"", ""U_SO1_PROMOCION"", ""U_SO1_PROMOCADENA"",
                                               ""U_SO1_LISTAPRECIO"", ""U_SO1_ACTIVO"") 
                                               VALUES (@pCode, @pName, @pPromo, @pPromoCadena, @pPrecio, @pActivo)"));

        internal static Consulta ModificarPrecio = new Consulta(Proveedor.SQLServer,
        new ComandoConsulta(Proveedor.SQLServer, @"UPDATE [@SO1_01PROMOLISTPREC] SET  [U_SO1_ACTIVO] = @pActivo WHERE [Code] = @pCode "),
        new ComandoConsulta(Proveedor.Hana, @"UPDATE ""@SO1_01PROMOLISTPREC"" SET  ""U_SO1_ACTIVO"" = @pActivo WHERE ""Code"" = @pCode "));


        #endregion

        #region Consultas Forma de Pago

        internal static Consulta ConsultarFormasPago = new Consulta(Proveedor.SQLServer,
        new ComandoConsulta(Proveedor.SQLServer, @"SELECT Promo.Code Codigo, Promo.Name Nombre, Promo.U_SO1_PROMOCION Promo, Promo.U_SO1_PROMOCADENA PromoCadena,
                                                     Forma.Code CodigoPago, Promo.U_SO1_MINIMO Minimo, Promo.U_SO1_TIPO Tipo, Promo.U_SO1_ACTIVO Activo, Forma.Name NombrePago
                                                     FROM [@SO1_01PROMOFORMAPAG] Promo
                                                     RIGHT JOIN [@SO1_01FORMAPAGO] Forma ON
                                                     (Promo.U_SO1_FORMAPAGO = Forma.Code
                                                     AND Promo.U_SO1_PROMOCION =  @pPromocion)"),
        new ComandoConsulta(Proveedor.Hana, @"SELECT Promo.""Code"" ""Codigo"", Promo.""Name"" ""Nombre"", Promo.""U_SO1_PROMOCION"" ""Promo"", Promo.""U_SO1_PROMOCADENA"" ""PromoCadena"",
                                                     Forma.""Code"" ""CodigoPago"", Promo.""U_SO1_MINIMO"" ""Minimo"", Promo.""U_SO1_TIPO"" ""Tipo"", Promo.""U_SO1_ACTIVO"" ""Activo"", Forma.""Name"" ""NombrePago""
                                                     FROM ""@SO1_01PROMOFORMAPAG"" Promo
                                                     RIGHT JOIN ""@SO1_01FORMAPAGO"" Forma ON
                                                     (Promo.""U_SO1_FORMAPAGO"" = Forma.""Code""
                                                     AND Promo.""U_SO1_PROMOCION"" =  @pPromocion) "));

        internal static Consulta RegistrarFormaPago = new Consulta(Proveedor.SQLServer,
          new ComandoConsulta(Proveedor.SQLServer, @"INSERT INTO [@SO1_01PROMOFORMAPAG] ([Code], [Name], [U_SO1_PROMOCION], [U_SO1_PROMOCADENA],
                                                         [U_SO1_FORMAPAGO], [U_SO1_ACTIVO], [U_SO1_MINIMO], [U_SO1_TIPO]) 
                                                         VALUES (@pCode, @pName, @pPromo, @pPromoCadena, @pFPago, @pActivo, @pMinimo, @pTipo)"),
          new ComandoConsulta(Proveedor.Hana, @"INSERT INTO ""@SO1_01PROMOFORMAPAG"" (""Code"", ""Name"", ""U_SO1_PROMOCION"", ""U_SO1_PROMOCADENA"",
                                              ""U_SO1_FORMAPAGO"", ""U_SO1_ACTIVO"", ""U_SO1_MINIMO"", ""U_SO1_TIPO"") 
                                              VALUES (@pCode, @pName, @pPromo, @pPromoCadena, @pFPago, @pActivo, @pMinimo, @pTipo)"));

        internal static Consulta ModificarFormaPago = new Consulta(Proveedor.SQLServer,
        new ComandoConsulta(Proveedor.SQLServer, @"UPDATE [@SO1_01PROMOFORMAPAG] SET  [U_SO1_ACTIVO] = @pActivo, [U_SO1_MINIMO] = @pMinimo, [U_SO1_TIPO] = @pTipo
                                                 WHERE [Code] = @pCode "),
        new ComandoConsulta(Proveedor.Hana, @"UPDATE ""@SO1_01PROMOFORMAPAG"" SET  ""U_SO1_ACTIVO"" = @pActivo, ""U_SO1_MINIMO"" = @pMinimo, ""U_SO1_TIPO"" = @pTipo
                                            WHERE ""Code"" = @pCode "));



        #endregion

        #region Consultas Clientes

        internal static Consulta ConsultarClientes = new Consulta(Proveedor.SQLServer,
        new ComandoConsulta(Proveedor.SQLServer, @"SELECT A.Code Codigo, A.Name Nombre, A.U_SO1_PROMOCION Promo, A.U_SO1_PROMOCADENA PromoCadena, 
                                                     A.U_SO1_CLIENTE CodigoCliente, B.CardName NombreCliente 
                                                     FROM [@SO1_01PROMOCLIENTE] A
                                                     LEFT JOIN [OCRD] B ON (A.U_SO1_CLIENTE = B.CardCode)
                                                     WHERE U_SO1_PROMOCION = @pPromocion"),
        new ComandoConsulta(Proveedor.Hana, @"SELECT A.""Code"" ""Codigo"", A.""Name"" ""Nombre"", A.""U_SO1_PROMOCION"" ""Promo"", A.""U_SO1_PROMOCADENA"" ""PromoCadena"", 
                                                     A.""U_SO1_CLIENTE"" ""CodigoCliente"", B.""CardName"" ""NombreCliente"" 
                                                     FROM ""@SO1_01PROMOCLIENTE"" A
                                                     LEFT JOIN ""OCRD"" B ON (A.""U_SO1_CLIENTE"" = B.""CardCode"")
                                                     WHERE ""U_SO1_PROMOCION"" = @pPromocion"));

        internal static Consulta ConsultarGruposClientes = new Consulta(Proveedor.SQLServer,
        new ComandoConsulta(Proveedor.SQLServer, @"SELECT Promo.Code Codigo, Promo.Name Nombre, Promo.U_SO1_PROMOCION Promo, Promo.U_SO1_PROMOCADENA PromoCadena, Grupo.GroupCode CodigoGrupo, 
                                                     Promo.U_SO1_ACTIVO Activo, Grupo.GroupName NombreGrupo
                                                     FROM [@SO1_01PROMOCLIEGRUP] Promo
                                                     RIGHT JOIN [OCRG] Grupo ON
                                                     (Promo.U_SO1_GRUPO = Grupo.GroupCode
                                                     AND Promo.U_SO1_PROMOCION = @pPromocion)"),
        new ComandoConsulta(Proveedor.Hana, @"SELECT Promo.""Code"" ""Codigo"", Promo.""Name"" ""Nombre"", Promo.""U_SO1_PROMOCION"" ""Promo"", Promo.""U_SO1_PROMOCADENA"" ""PromoCadena"", 
                                                Grupo.""GroupCode"" ""CodigoGrupo"", Promo.""U_SO1_ACTIVO"" ""Activo"", Grupo.""GroupName"" ""NombreGrupo""
                                                FROM ""@SO1_01PROMOCLIEGRUP"" Promo
                                                RIGHT JOIN ""OCRG"" Grupo ON
                                                (Promo.""U_SO1_GRUPO"" = Grupo.""GroupCode""
                                                AND Promo.""U_SO1_PROMOCION"" = @pPromocion)"));

        internal static Consulta ConsultarPropiedadesClientes = new Consulta(Proveedor.SQLServer,
        new ComandoConsulta(Proveedor.SQLServer, @"SELECT Promo.Code Codigo, Promo.Name Nombre, Promo.U_SO1_PROMOCION Promo, Promo.U_SO1_PROMOCADENA PromoCadena, Propiedad.GroupCode CodigoPropiedad, 
                                                     Promo.U_SO1_ACTIVO Activo, Propiedad.GroupName NombrePropiedad
                                                     FROM [@SO1_01PROMOCLIEPROP] Promo
                                                     RIGHT JOIN [OCQG] Propiedad ON
                                                     (Promo.U_SO1_PROPIEDAD = Propiedad.GroupCode
                                                     AND Promo.U_SO1_PROMOCION = @pPromocion) "),
        new ComandoConsulta(Proveedor.Hana, @"SELECT Promo.""Code"" ""Codigo"", Promo.""Name"" ""Nombre"", Promo.""U_SO1_PROMOCION"" ""Promo"", Promo.""U_SO1_PROMOCADENA"" ""PromoCadena"", 
                                                Propiedad.""GroupCode"" ""CodigoPropiedad"", Promo.""U_SO1_ACTIVO"" ""Activo"", Propiedad.""GroupName"" ""NombrePropiedad""
                                                FROM ""@SO1_01PROMOCLIEPROP"" Promo
                                                RIGHT JOIN ""OCQG"" Propiedad ON
                                                (Promo.""U_SO1_PROPIEDAD"" = Propiedad.""GroupCode""
                                                AND Promo.""U_SO1_PROMOCION"" = @pPromocion)"));

        internal static Consulta RegistrarCliente = new Consulta(Proveedor.SQLServer,
            new ComandoConsulta(Proveedor.SQLServer, @"INSERT INTO [@SO1_01PROMOCLIENTE] ([Code], [Name], [U_SO1_PROMOCION], [U_SO1_PROMOCADENA],
                                                     [U_SO1_CLIENTE]) 
                                                     VALUES (@pCode, @pName, @pPromo, @pPromoCadena, @pCliente)"),
            new ComandoConsulta(Proveedor.Hana, @"INSERT INTO ""@SO1_01PROMOCLIENTE"" (""Code"", ""Name"", ""U_SO1_PROMOCION"", ""U_SO1_PROMOCADENA"",
                                                ""U_SO1_CLIENTE"") 
                                                VALUES (@pCode, @pName, @pPromo, @pPromoCadena, @pCliente)"));
        #endregion

        #region Consultas Articulos

        internal static Consulta ConsultarCodigosProveedor = new Consulta(Proveedor.SQLServer,
        new ComandoConsulta(Proveedor.SQLServer, @"SELECT  A.Code Codigo, A.U_SO1_PROVEEDOR Proveedor
                                                     FROM [@SO1_01PROMOARTIPROV] A
                                                     WHERE A.U_SO1_PROMOCION =  @pPromocion "),
        new ComandoConsulta(Proveedor.Hana, @"SELECT  A.""Code"" ""Codigo"", A.""U_SO1_PROVEEDOR"" ""Proveedor""
                                                FROM ""@SO1_01PROMOARTIPROV"" A
                                                WHERE A.""U_SO1_PROMOCION"" =  @pPromocion"));

        internal static Consulta ConsultarGruposArticulos = new Consulta(Proveedor.SQLServer,
        new ComandoConsulta(Proveedor.SQLServer, @"SELECT Promo.Code Codigo, Promo.Name Nombre, Promo.U_SO1_PROMOCION Promo, Promo.U_SO1_PROMOCADENA PromoCadena, Grupo.ItmsGrpCod CodigoGrupo, 
                                                     Promo.U_SO1_ACTIVO Activo, Grupo.ItmsGrpNam NombreGrupo
                                                     FROM [@SO1_01PROMOARTIGRUP] Promo
                                                     RIGHT JOIN [OITB] Grupo ON
                                                     (Promo.U_SO1_GRUPO = Grupo.ItmsGrpCod
                                                     AND Promo.U_SO1_PROMOCION = @pPromocion)"),
        new ComandoConsulta(Proveedor.Hana, @"SELECT Promo.""Code"" ""Codigo"", Promo.""Name"" ""Nombre"", Promo.""U_SO1_PROMOCION"" ""Promo"", Promo.""U_SO1_PROMOCADENA"" ""PromoCadena"", Grupo.""ItmsGrpCod"" ""CodigoGrupo"", 
                                                Promo.""U_SO1_ACTIVO"" ""Activo"", Grupo.""ItmsGrpNam"" ""NombreGrupo""
                                                FROM ""@SO1_01PROMOARTIGRUP"" Promo
                                                RIGHT JOIN ""OITB"" Grupo ON
                                                (Promo.""U_SO1_GRUPO"" = Grupo.""ItmsGrpCod""
                                                AND Promo.""U_SO1_PROMOCION"" = @pPromocion)"));

        internal static Consulta ConsultarPropiedadesArticulos = new Consulta(Proveedor.SQLServer,
        new ComandoConsulta(Proveedor.SQLServer, @"SELECT Promo.Code Codigo, Promo.Name Nombre, Promo.U_SO1_PROMOCION Promo, Promo.U_SO1_PROMOCADENA PromoCadena, Propiedad.ItmsTypCod CodigoPropiedad, 
                                                     Promo.U_SO1_ACTIVO Activo, Propiedad.ItmsGrpNam NombrePropiedad
                                                     FROM [@SO1_01PROMOARTIPROP] Promo
                                                     RIGHT JOIN [OITG] Propiedad ON
                                                     (Promo.U_SO1_PROPIEDAD = Propiedad.ItmsTypCod
                                                     AND Promo.U_SO1_PROMOCION = @pPromocion)"),
        new ComandoConsulta(Proveedor.Hana, @"SELECT Promo.""Code"" ""Codigo"", Promo.""Name"" ""Nombre"", Promo.""U_SO1_PROMOCION"" ""Promo"", Promo.""U_SO1_PROMOCADENA"" ""PromoCadena"", 
                                                Propiedad.""ItmsTypCod"" ""CodigoPropiedad"", Promo.""U_SO1_ACTIVO"" ""Activo"", Propiedad.""ItmsGrpNam"" ""NombrePropiedad""
                                                FROM ""@SO1_01PROMOARTIPROP"" Promo
                                                RIGHT JOIN ""OITG"" Propiedad ON
                                                (Promo.""U_SO1_PROPIEDAD"" = Propiedad.""ItmsTypCod""
                                                AND Promo.""U_SO1_PROMOCION"" = @pPromocion)"));

        internal static Consulta ConsultarArticulosPromo = new Consulta(Proveedor.SQLServer,
        new ComandoConsulta(Proveedor.SQLServer, @"SELECT A.Code Codigo, A.Name Nombre, A.U_SO1_PROMOCION Promo, A.U_SO1_PROMOCADENA PromoCadena, 
                                                     A.U_SO1_TIPO Tipo, A.U_SO1_ARTICULO Articulo, A.U_SO1_IMPORTE Importe, A.U_SO1_CODIGOAMBITO CodigoAmbito, 
                                                     B.ItemName NombreArticulo, '' ListaPrecio, '' Precio, '' ListaPrecio2, '' Precio2 
                                                     FROM [@SO1_01PROMOARTICULO] A
                                                     LEFT JOIN [OITM] B ON (A.U_SO1_ARTICULO = B.ItemCode)  
                                                     WHERE A.U_SO1_PROMOCION = @pPromocion AND A.U_SO1_TIPO = @pTipo "),
        new ComandoConsulta(Proveedor.Hana, @"SELECT A.""Code"" ""Codigo"", A.""Name"" ""Nombre"", A.""U_SO1_PROMOCION"" ""Promo"", A.""U_SO1_PROMOCADENA"" ""PromoCadena"", 
                                                A.""U_SO1_TIPO"" ""Tipo"", A.""U_SO1_ARTICULO"" ""Articulo"", A.""U_SO1_IMPORTE"" ""Importe"", A.""U_SO1_CODIGOAMBITO"" ""CodigoAmbito"", 
                                                B.""ItemName"" ""NombreArticulo"", '' ""ListaPrecio"", '' ""Precio"", '' ""ListaPrecio2"", '' ""Precio2"" 
                                                FROM ""@SO1_01PROMOARTICULO"" A
                                                LEFT JOIN ""OITM"" B ON (A.""U_SO1_ARTICULO"" = B.""ItemCode"")  
                                                WHERE A.""U_SO1_PROMOCION"" = @pPromocion AND A.""U_SO1_TIPO"" = @pTipo "));

        internal static Consulta ConsultarArticulosLPPromo = new Consulta(Proveedor.SQLServer,
        new ComandoConsulta(Proveedor.SQLServer, @"SELECT A.Code Codigo, A.Name Nombre, A.U_SO1_PROMOCION Promo, A.U_SO1_PROMOCADENA PromoCadena, 
                                                 A.U_SO1_TIPO Tipo, A.U_SO1_ARTICULO Articulo, A.U_SO1_IMPORTE Importe, A.U_SO1_CODIGOAMBITO CodigoAmbito, 
                                                 B.ItemName NombreArticulo, IT.PriceList ListaPrecio, IT.Price Precio, IT2.PriceList ListaPrecio2, IT2.Price Precio2 
                                                 FROM [@SO1_01PROMOARTICULO] A
                                                 LEFT JOIN [OITM] B ON (A.U_SO1_ARTICULO = B.ItemCode)
                                                 INNER JOIN [ITM1] IT ON (IT.ItemCode = A.U_SO1_ARTICULO AND IT.PriceList = @pListaPrecio)
                                                 INNER JOIN [ITM1] IT2 ON (IT2.ItemCode = A.U_SO1_ARTICULO AND IT2.PriceList = @pListaPrecio2)  
                                                 WHERE A.U_SO1_PROMOCION = @pPromocion AND A.U_SO1_TIPO = @pTipo "),
        new ComandoConsulta(Proveedor.Hana, @"SELECT A.""Code"" ""Codigo"", A.""Name"" ""Nombre"", A.""U_SO1_PROMOCION"" ""Promo"", A.""U_SO1_PROMOCADENA"" ""PromoCadena"", 
                                            A.""U_SO1_TIPO"" ""Tipo"", A.""U_SO1_ARTICULO"" ""Articulo"", A.""U_SO1_IMPORTE"" ""Importe"", A.""U_SO1_CODIGOAMBITO"" ""CodigoAmbito"", 
                                            B.""ItemName"" ""NombreArticulo"", IT.""PriceList"" ""ListaPrecio"", IT.""Price"" ""Precio"", IT2.""PriceList"" ""ListaPrecio2"",
                                            IT2.""Price"" ""Precio2"" 
                                            FROM ""@SO1_01PROMOARTICULO"" A
                                            LEFT JOIN ""OITM"" B ON (A.""U_SO1_ARTICULO"" = B.""ItemCode"")
                                            INNER JOIN ""ITM1"" IT ON (IT.""ItemCode"" = A.""U_SO1_ARTICULO"" AND IT.""PriceList"" = @pListaPrecio1)
                                            INNER JOIN ""ITM1"" IT2 ON (IT2.""ItemCode"" = A.""U_SO1_ARTICULO"" AND IT2.""PriceList"" = @pListaPrecio2)  
                                            WHERE A.""U_SO1_PROMOCION"" = @pPromocion AND A.""U_SO1_TIPO"" = @pTipo "));

        internal static Consulta ConsultarArticulos = new Consulta(Proveedor.SQLServer,
            new ComandoConsulta(Proveedor.SQLServer, @"SELECT B.ItemCode Codigo, B.ItemName Nombre
                                                     FROM [OITM]B
                                                     WHERE B.ItemCode IN (${lArticulos}) "),
            new ComandoConsulta(Proveedor.Hana, @"SELECT B.""ItemCode"" ""Codigo"", B.""ItemName"" ""Nombre""
                                                FROM ""OITM"" B 
                                                WHERE B.""ItemCode"" IN (${lArticulos}) "));

        internal static Consulta ConsultarArticulosPrecios = new Consulta(Proveedor.SQLServer,
            new ComandoConsulta(Proveedor.SQLServer, @"SELECT B.ItemCode Codigo, B.ItemName Nombre, IT.Price Precio, IT2.Price Precio2 
                                                     FROM [OITM]B 
                                                     INNER JOIN [ITM1] IT ON (IT.ItemCode = B.ItemCode AND IT.PriceList = @pListaPrecio1)
                                                     INNER JOIN [ITM1] IT2 ON (IT2.ItemCode = B.ItemCode AND IT2.PriceList = @pListaPrecio2) 
                                                     WHERE B.ItemCode IN (${lArticulos}) "),
            new ComandoConsulta(Proveedor.Hana, @"SELECT B.""ItemCode"" ""Codigo"", B.""ItemName"" ""Nombre"", IT.""Price"" ""Precio"", IT2.""Price"" ""Precio2""
                                                FROM ""OITM"" B 
                                                INNER JOIN ""ITM1"" IT ON (IT.""ItemCode"" = B.""ItemCode"" AND IT.""PriceList"" = @pListaPrecio1)
                                                INNER JOIN ""ITM1"" IT2 ON (IT2.""ItemCode"" = B.""ItemCode"" AND IT2.""PriceList"" = @pListaPrecio2) 
                                                WHERE B.""ItemCode"" IN (${lArticulos}) "));

        internal static Consulta RegistrarArticulo = new Consulta(Proveedor.SQLServer,
          new ComandoConsulta(Proveedor.SQLServer, @"INSERT INTO [@SO1_01PROMOARTICULO] ([Code], [Name], [U_SO1_PROMOCION], [U_SO1_PROMOCADENA],
                                                         [U_SO1_TIPO], [U_SO1_ARTICULO], [U_SO1_IMPORTE], [U_SO1_CODIGOAMBITO]) 
                                                         VALUES (@pCode, @pName, @pPromo, @pPromoCadena, @pTipo, @pArticulo, @pImporte, @pCodigoAmbito)"),
          new ComandoConsulta(Proveedor.Hana, @"INSERT INTO ""@SO1_01PROMOARTICULO"" (""Code"", ""Name"", ""U_SO1_PROMOCION"", ""U_SO1_PROMOCADENA"",
                                                    ""U_SO1_TIPO"", ""U_SO1_ARTICULO"", ""U_SO1_IMPORTE"", ""U_SO1_CODIGOAMBITO"") 
                                                    VALUES (@pCode, @pName, @pPromo, @pPromoCadena,  @pTipo, @pArticulo, @pImporte, @pCodigoAmbito)"));

        internal static Consulta ActualizarArticulo = new Consulta(Proveedor.SQLServer,
            new ComandoConsulta(Proveedor.SQLServer, @"UPDATE [@SO1_01PROMOARTICULO] SET [U_SO1_IMPORTE] = @pImporte
                                                         WHERE [Code] = @pCodigo "),
            new ComandoConsulta(Proveedor.Hana, @"UPDATE ""@SO1_01PROMOARTICULO"" SET ""U_SO1_IMPORTE"" = @pImporte
                                                         WHERE ""Code"" = @pCodigo "));

        internal static Consulta ConsultarCodigosArticulos = new Consulta(Proveedor.SQLServer,
        new ComandoConsulta(Proveedor.SQLServer, @"SELECT A.Code Codigo, U_SO1_ARTICULO Articulo
                                                     FROM [@SO1_01PROMOARTICULO] A
                                                     WHERE A.U_SO1_PROMOCION = @pPromocion AND A.U_SO1_TIPO = @pTipo "),
        new ComandoConsulta(Proveedor.Hana, @"SELECT A.""Code"" ""Codigo"", A.""U_SO1_ARTICULO"" ""Articulo""
                                                     FROM ""@SO1_01PROMOARTICULO"" A
                                                     WHERE A.""U_SO1_PROMOCION"" = @pPromocion AND A.""U_SO1_TIPO"" = @pTipo "));


        #endregion

        #region Consultas Proveedores

        internal static Consulta ConsultarProveedor = new Consulta(Proveedor.SQLServer,
        new ComandoConsulta(Proveedor.SQLServer, @"SELECT  A.Code Codigo, A.Name Nombre, A.U_SO1_PROMOCION Promo, A.U_SO1_PROMOCADENA PromoCadena, B.CardCode Proveedor, B.CardName NombreProveedor 
                                                     FROM [@SO1_01PROMOARTIPROV] A
                                                     LEFT JOIN [OCRD] B ON (A.U_SO1_PROVEEDOR = B.CardCode)  
                                                     WHERE A.U_SO1_PROMOCION =  @pPromocion "),
        new ComandoConsulta(Proveedor.Hana, @"SELECT  A.""Code"" ""Codigo"", A.""Name"" ""Nombre"", A.""U_SO1_PROMOCION"" ""Promo"", 
                                                A.""U_SO1_PROMOCADENA"" ""PromoCadena"", B.""CardCode"" ""Proveedor"", B.""CardName"" ""NombreProveedor"" 
                                                FROM ""@SO1_01PROMOARTIPROV"" A
                                                LEFT JOIN ""OCRD"" B ON (A.""U_SO1_PROVEEDOR"" = B.""CardCode"" ) 
                                                WHERE A.""U_SO1_PROMOCION"" =  @pPromocion "));

        internal static Consulta RegistrarProveedor = new Consulta(Proveedor.SQLServer,
          new ComandoConsulta(Proveedor.SQLServer, @"INSERT INTO [@SO1_01PROMOARTIPROV] ([Code], [Name], [U_SO1_PROMOCION], [U_SO1_PROMOCADENA],
                                                         [U_SO1_PROVEEDOR]) 
                                                         VALUES (@pCode, @pName, @pPromo, @pPromoCadena, @pProveedor)"),
          new ComandoConsulta(Proveedor.Hana, @"INSERT INTO ""@SO1_01PROMOARTIPROV"" (""Code"", ""Name"", ""U_SO1_PROMOCION"", ""U_SO1_PROMOCADENA"",
                                                    ""U_SO1_PROVEEDOR"") 
                                                    VALUES (@pCode, @pName, @pPromo, @pPromoCadena, @pProveedor)"));

        #endregion

        #region Consultas Fabricante

        internal static Consulta ConsultarFabricantes = new Consulta(Proveedor.SQLServer,
        new ComandoConsulta(Proveedor.SQLServer, @"SELECT  A.FirmCode CodigoFabricante, A.FirmName NombreFabricante, B.Code Codigo, 
                                                     B.Name Nombre, B.U_SO1_PROMOCION Promo, B.U_SO1_PROMOCADENA PromoCadena, B.U_SO1_ACTIVO Activo 
                                                     FROM [OMRC] A
                                                     LEFT JOIN [@SO1_01PROMOARTIFABR] B ON (B.U_SO1_FABRICANTE = A.FirmCode  
                                                     AND B.U_SO1_PROMOCION = @pPromocion)"),
        new ComandoConsulta(Proveedor.Hana, @"SELECT  A.""FirmCode"" ""CodigoFabricante"", A.""FirmName"" ""NombreFabricante"", B.""Code"" ""Codigo"", 
                                                B.""Name"" ""Nombre"", B.""U_SO1_PROMOCION"" ""Promo"", B.""U_SO1_PROMOCADENA"" ""PromoCadena"", B.""U_SO1_ACTIVO"" ""Activo"" 
                                                FROM ""OMRC"" A
                                                LEFT JOIN ""@SO1_01PROMOARTIFABR"" B ON (B.""U_SO1_FABRICANTE"" = A.""FirmCode""  
                                                AND B.""U_SO1_PROMOCION"" = @pPromocion)"));

        internal static Consulta RegistrarFabricante = new Consulta(Proveedor.SQLServer,
          new ComandoConsulta(Proveedor.SQLServer, @"INSERT INTO [@SO1_01PROMOARTIFABR] ([Code], [Name], [U_SO1_PROMOCION], [U_SO1_PROMOCADENA],
                                                   [U_SO1_FABRICANTE], [U_SO1_ACTIVO]) 
                                                   VALUES (@pCode, @pName, @pPromo, @pPromoCadena, @pFabricante, @pActivo)"),
          new ComandoConsulta(Proveedor.Hana, @"INSERT INTO ""@SO1_01PROMOARTIFABR"" (""Code"", ""Name"", ""U_SO1_PROMOCION"", ""U_SO1_PROMOCADENA"",
                                              ""U_SO1_FABRICANTE"", ""U_SO1_ACTIVO"") 
                                              VALUES (@pCode, @pName, @pPromo, @pPromoCadena, @pFabricante, @pActivo)"));

        internal static Consulta ModificarFabricante = new Consulta(Proveedor.SQLServer,
        new ComandoConsulta(Proveedor.SQLServer, @"UPDATE [@SO1_01PROMOARTIFABR] SET  [U_SO1_ACTIVO] = @pActivo
                                                 WHERE [Code] = @pCode "),
        new ComandoConsulta(Proveedor.Hana, @"UPDATE ""@SO1_01PROMOARTIFABR"" SET  ""U_SO1_ACTIVO"" = @pActivo
                                            WHERE ""Code"" = @pCode "));

        #endregion

        #region Consultas UnidadMedida

        internal static Consulta ConsultarMedidasArticulos = new Consulta(Proveedor.SQLServer,
        new ComandoConsulta(Proveedor.SQLServer, @"SELECT B.UgpEntry CodigoGrupo, B.UomEntry CodigoUnidad,  A.UgpName NombreGrupo, C.UomName NombreUnidad, B.BaseQty Cantidad,
                                                     D.Code Codigo,D.Name Nombre,D.U_SO1_ACTIVO Activo, D.U_SO1_PROMOCADENA PromoCadena, D.U_SO1_PROMOCION Promo
                                                     FROM [OUGP] A
                                                     JOIN [UGP1] B ON (A.UgpEntry = B.UgpEntry)
                                                     JOIN [OUOM] C ON (B.UomEntry = C.UomEntry)
                                                     LEFT JOIN [@SO1_01PROMOARTIUNIE] D ON (D.U_SO1_PROMOCION = @pPromocion
                                                     AND D.U_SO1_GRUPO = A.UgpEntry 
	                                                 AND D.U_SO1_UNIDAD = B.UomEntry) 
                                                     ORDER BY A.UgpEntry "),
        new ComandoConsulta(Proveedor.Hana, @"SELECT B.""UgpEntry"" ""CodigoGrupo"", B.""UomEntry"" ""CodigoUnidad"",  A.""UgpName"" ""NombreGrupo"", 
                                                C.""UomName"" ""NombreUnidad"", B.""BaseQty"" ""Cantidad"", D.""Code"" ""Codigo"", D.""Name"" ""Nombre"",D.""U_SO1_ACTIVO"" ""Activo"", 
                                                D.""U_SO1_PROMOCADENA"" ""PromoCadena"", D.""U_SO1_PROMOCION"" ""Promo""
                                                FROM ""OUGP"" A
                                                JOIN ""UGP1"" B ON (A.""UgpEntry"" = B.""UgpEntry"")
                                                JOIN ""OUOM"" C ON (B.""UomEntry"" = C.""UomEntry"")
                                                LEFT JOIN ""@SO1_01PROMOARTIUNIE"" D ON (D.""U_SO1_PROMOCION"" = @pPromocion
                                                AND D.""U_SO1_GRUPO"" = A.""UgpEntry"" 
	                                            AND D.""U_SO1_UNIDAD"" = B.""UomEntry"") 
                                                ORDER BY A.""UgpEntry"" "));

        internal static Consulta RegistrarUnidadMedida = new Consulta(Proveedor.SQLServer,
          new ComandoConsulta(Proveedor.SQLServer, @"INSERT INTO [@SO1_01PROMOARTIUNIE] ([Code], [Name], [U_SO1_PROMOCION], [U_SO1_PROMOCADENA],
                                                         [U_SO1_GRUPO], [U_SO1_UNIDAD], [U_SO1_ACTIVO]) 
                                                         VALUES (@pCode, @pName, @pPromo, @pPromoCadena, @pGrupo, @pUnidad, @pActivo)"),
          new ComandoConsulta(Proveedor.Hana, @"INSERT INTO ""@SO1_01PROMOARTIUNIE"" (""Code"", ""Name"", ""U_SO1_PROMOCION"", ""U_SO1_PROMOCADENA"",
                                              ""U_SO1_GRUPO"", ""U_SO1_UNIDAD"", ""U_SO1_ACTIVO"") 
                                              VALUES (@pCode, @pName, @pPromo, @pPromoCadena, @pGrupo, @pUnidad, @pActivo)"));

        internal static Consulta ModificarUnidadMedida
            = new Consulta(Proveedor.SQLServer,
        new ComandoConsulta(Proveedor.SQLServer, @"UPDATE [@SO1_01PROMOARTIUNIE] SET [U_SO1_GRUPO] = @pGrupo, [U_SO1_UNIDAD] = @pUnidad, [U_SO1_ACTIVO] = @pActivo
                                                 WHERE [Code] = @pCode "),
        new ComandoConsulta(Proveedor.Hana, @"UPDATE ""@SO1_01PROMOARTIUNIE"" SET ""U_SO1_GRUPO"" = @pGrupo, ""U_SO1_UNIDAD"" = @pUnidad, ""U_SO1_ACTIVO"" = @pActivo
                                            WHERE ""Code""= @pCode "));

        #endregion

        #region Consultas Promociones

        #region AxB

        internal static Consulta ConsultarPromocionAxB = new Consulta(Proveedor.SQLServer,
        new ComandoConsulta(Proveedor.SQLServer, @"SELECT Code, Name, U_SO1_PROMOCION, U_SO1_PROMOCADENA, U_SO1_PIEZASTOTALES, 
                                                     U_SO1_PIEZASAPAGAR, U_SO1_REGALOPRECIO1, U_SO1_MANTENERDESC
                                                     FROM [@SO1_01PROMOAXB] 
                                                     WHERE U_SO1_PROMOCION = @pPromocion"),
        new ComandoConsulta(Proveedor.Hana, @"SELECT ""Code"", ""Name"", ""U_SO1_PROMOCION"", ""U_SO1_PROMOCADENA"", ""U_SO1_PIEZASTOTALES"", 
                                                ""U_SO1_PIEZASAPAGAR"", ""U_SO1_REGALOPRECIO1"", ""U_SO1_MANTENERDESC""
                                                FROM ""@SO1_01PROMOAXB"" 
                                                WHERE ""U_SO1_PROMOCION"" = @pPromocion"));

        internal static Consulta RegistrarPromocionAxB = new Consulta(Proveedor.SQLServer,
          new ComandoConsulta(Proveedor.SQLServer, @"INSERT INTO [@SO1_01PROMOAXB] ([Code], [Name], [U_SO1_PROMOCION], [U_SO1_PROMOCADENA], [U_SO1_PIEZASTOTALES],
                                                       [U_SO1_PIEZASAPAGAR], [U_SO1_REGALOPRECIO1], [U_SO1_MANTENERDESC]) 
                                                       VALUES (@pCodigo, @pNombre, @pPromo, @pPromoCadena, @pPiezasTotales, @pPiezasPagar, @pRegaloPrecio, @pMantenerDesc)"),
          new ComandoConsulta(Proveedor.Hana, @"INSERT INTO ""@SO1_01PROMOAXB"" (""Code"", ""Name"", ""U_SO1_PROMOCION"", ""U_SO1_PROMOCADENA"", ""U_SO1_PIEZASTOTALES"",
                                                  ""U_SO1_PIEZASAPAGAR"", ""U_SO1_REGALOPRECIO1"", ""U_SO1_MANTENERDESC"") 
                                                  VALUES (@pCodigo, @pNombre, @pPromo, @pPromoCadena, @pPiezasTotales, @pPiezasPagar, @pRegaloPrecio, @pMantenerDesc)"));

        internal static Consulta ModificarPromocionAxB = new Consulta(Proveedor.SQLServer,
            new ComandoConsulta(Proveedor.SQLServer, @"UPDATE [@SO1_01PROMOAXB] SET [Name] = @pNombre, [U_SO1_PIEZASTOTALES] = @pPiezasTotales, [U_SO1_PIEZASAPAGAR] = @pPiezasPagar,
                                                         [U_SO1_REGALOPRECIO1] = @pRegaloPrecio, [U_SO1_MANTENERDESC] = @pMantenerDesc
                                                         WHERE [Code] = @pCode "),
            new ComandoConsulta(Proveedor.Hana, @"UPDATE ""@SO1_01PROMOAXB"" SET ""Name"" = @pNombre, ""U_SO1_PIEZASTOTALES"" = @pPiezasTotales, ""U_SO1_PIEZASAPAGAR"" = @pPiezasPagar,
                                                    ""U_SO1_REGALOPRECIO1"" = @pRegaloPrecio, ""U_SO1_MANTENERDESC"" = @pMantenerDesc
                                                    WHERE ""Code"" = @pCode "));

        #endregion

        #region Descuento Empleado

        internal static Consulta ConsultarPromocionDescEmp = new Consulta(Proveedor.SQLServer,
        new ComandoConsulta(Proveedor.SQLServer, @"SELECT Code, Name, U_SO1_PROMOCION, U_SO1_PROMOCADENA, U_SO1_LISTAPRECIO, 
                                                     U_SO1_CANTIDADMES, U_SO1_PORCENTAJEDESC
                                                     FROM [@SO1_01PROMODESEMP] 
                                                     WHERE U_SO1_PROMOCION = @pPromocion"),
        new ComandoConsulta(Proveedor.Hana, @"SELECT ""Code"", ""Name"", ""U_SO1_PROMOCION"", ""U_SO1_PROMOCADENA"", ""U_SO1_LISTAPRECIO"", 
                                                ""U_SO1_CANTIDADMES"", ""U_SO1_PORCENTAJEDESC""
                                                FROM ""@SO1_01PROMODESEMP"" 
                                                WHERE ""U_SO1_PROMOCION"" = @pPromocion"));

        internal static Consulta RegistrarPromocionDescEmp = new Consulta(Proveedor.SQLServer,
          new ComandoConsulta(Proveedor.SQLServer, @"INSERT INTO [@SO1_01PROMODESEMP] ([Code], [Name], [U_SO1_PROMOCION], [U_SO1_PROMOCADENA], [U_SO1_LISTAPRECIO],
                                                   [U_SO1_CANTIDADMES], [U_SO1_PORCENTAJEDESC]) 
                                                   VALUES (@pCodigo, @pNombre, @pPromo, @pPromoCadena, @pListaPrecio, @pCantidadMes, @pPorcentajeDesc)"),
          new ComandoConsulta(Proveedor.Hana, @"INSERT INTO ""@SO1_01PROMODESEMP"" (""Code"", ""Name"", ""U_SO1_PROMOCION"", ""U_SO1_PROMOCADENA"", ""U_SO1_LISTAPRECIO"",
                                              ""U_SO1_CANTIDADMES"", ""U_SO1_PORCENTAJEDESC"") 
                                              VALUES (@pCodigo, @pNombre, @pPromo, @pPromoCadena, @pListaPrecio, @pCantidadMes, @pPorcentajeDesc)"));

        internal static Consulta ModificarPromocionDescEmp = new Consulta(Proveedor.SQLServer,
            new ComandoConsulta(Proveedor.SQLServer, @"UPDATE [@SO1_01PROMODESEMP] SET [Name] = @pNombre, [U_SO1_LISTAPRECIO] = @pListaPrecio, [U_SO1_CANTIDADMES] = @pCantidadMes,
                                                         [U_SO1_PORCENTAJEDESC] = @pPorcentajeDesc
                                                         WHERE [Code] = @pCode "),
            new ComandoConsulta(Proveedor.Hana, @"UPDATE ""@SO1_01PROMODESEMP"" SET ""Name"" = @pNombre, ""U_SO1_LISTAPRECIO"" = @pListaPrecio, ""U_SO1_CANTIDADMES"" = @pCantidadMes,
                                                ""U_SO1_PORCENTAJEDESC"" = @pPorcentajeDesc
                                                WHERE ""Code"" = @pCode "));

        #endregion

        #region Descuento Escala

        internal static Consulta ConsultarPromocionDescEsc = new Consulta(Proveedor.SQLServer,
        new ComandoConsulta(Proveedor.SQLServer, @"SELECT Code Codigo, Name Nombre, U_SO1_PROMOCION Promo, U_SO1_PROMOCADENA PromoCadena, U_SO1_CONIMPUESTO Impuesto, 
                                                     U_SO1_TIPO Tipo, U_SO1_ARTICULOSTODOS ArticuloTodos
                                                     FROM [@SO1_01PROMODESESC] 
                                                     WHERE U_SO1_PROMOCION = @pPromocion"),
        new ComandoConsulta(Proveedor.Hana, @"SELECT ""Code"" ""Codigo"", ""Name"" ""Nombre"", ""U_SO1_PROMOCION"" ""Promo"", ""U_SO1_PROMOCADENA"" ""PromoCadena"", ""U_SO1_CONIMPUESTO"" ""Impuesto"", 
                                            ""U_SO1_TIPO"" ""Tipo"", ""U_SO1_ARTICULOSTODOS"" ""ArticuloTodos""
                                            FROM ""@SO1_01PROMODESESC"" 
                                            WHERE ""U_SO1_PROMOCION"" = @pPromocion"));

        internal static Consulta RegistrarPromocionDescEsc = new Consulta(Proveedor.SQLServer,
          new ComandoConsulta(Proveedor.SQLServer, @"INSERT INTO [@SO1_01PROMODESESC] ([Code], [Name], [U_SO1_PROMOCION], [U_SO1_PROMOCADENA], [U_SO1_CONIMPUESTO],
                                                       [U_SO1_TIPO], [U_SO1_ARTICULOSTODOS]) 
                                                       VALUES (@pCodigo, @pNombre, @pPromo, @pPromoCadena, @pImpuesto, @pTipo, @pArticulosTodos)"),
          new ComandoConsulta(Proveedor.Hana, @"INSERT INTO ""@SO1_01PROMODESESC"" (""Code"", ""Name"", ""U_SO1_PROMOCION"", ""U_SO1_PROMOCADENA"", ""U_SO1_CONIMPUESTO"",
                                                  ""U_SO1_TIPO"", ""U_SO1_ARTICULOSTODOS"") 
                                                  VALUES (@pCodigo, @pNombre, @pPromo, @pPromoCadena, @pImpuesto, @pTipo, @pArticulosTodos)"));

        internal static Consulta ModificarPromocionDescEsc = new Consulta(Proveedor.SQLServer,
            new ComandoConsulta(Proveedor.SQLServer, @"UPDATE [@SO1_01PROMODESESC] SET  [Name] = @pNombre,[U_SO1_CONIMPUESTO] = @pImpuesto, [U_SO1_TIPO] = @pTipo,
                                                         [U_SO1_ARTICULOSTODOS] = @pArticulosTodos
                                                         WHERE [Code] = @pCode "),
            new ComandoConsulta(Proveedor.Hana, @"UPDATE ""@SO1_01PROMODESESC"" SET ""Name"" = @pNombre, ""U_SO1_CONIMPUESTO"" = @pImpuesto, ""U_SO1_TIPO"" = @pTipo,
                                                         ""U_SO1_ARTICULOSTODOS"" = @pArticulosTodos
                                                         WHERE ""Code"" = @pCode "));

        internal static Consulta ConsultarPromocionDescEscPr = new Consulta(Proveedor.SQLServer,
        new ComandoConsulta(Proveedor.SQLServer, @"SELECT Code Codigo, Name Nombre, U_SO1_PROMOCION Promo, U_SO1_PROMOCADENA PromoCadena, U_SO1_MONTO Monto, 
                                                     U_SO1_PORCENTAJEDESC Porcentaje
                                                     FROM [@SO1_01PROMODESESCPR] 
                                                     WHERE U_SO1_PROMOCION = @pPromocion"),
        new ComandoConsulta(Proveedor.Hana, @"SELECT ""Code"" ""Codigo"", ""Name"" ""Nombre"", ""U_SO1_PROMOCION"" ""Promo"", ""U_SO1_PROMOCADENA"" ""PromoCadena"", ""U_SO1_MONTO"" ""Monto"", 
                                                ""U_SO1_PORCENTAJEDESC"" ""Porcentaje""
                                                FROM ""@SO1_01PROMODESESCPR"" 
                                                WHERE ""U_SO1_PROMOCION"" = @pPromocion"));

        internal static Consulta RegistrarPromocionDescEscPr = new Consulta(Proveedor.SQLServer,
          new ComandoConsulta(Proveedor.SQLServer, @"INSERT INTO [@SO1_01PROMODESESCPR] ([Code], [Name], [U_SO1_PROMOCION], [U_SO1_PROMOCADENA], [U_SO1_MONTO],
                                                   [U_SO1_PORCENTAJEDESC]) 
                                                   VALUES (@pCodigo, @pNombre, @pPromo, @pPromoCadena, @pMonto, @pPorcentajeDesc)"),
          new ComandoConsulta(Proveedor.Hana, @"INSERT INTO ""@SO1_01PROMODESESCPR"" (""Code"", ""Name"", ""U_SO1_PROMOCION"", ""U_SO1_PROMOCADENA"", ""U_SO1_MONTO"",
                                              ""U_SO1_PORCENTAJEDESC"") 
                                              VALUES (@pCodigo, @pNombre, @pPromo, @pPromoCadena, @pMonto, @pPorcentajeDesc)"));

        internal static Consulta ModificarPromocionDescEscPr = new Consulta(Proveedor.SQLServer,
            new ComandoConsulta(Proveedor.SQLServer, @"UPDATE [@SO1_01PROMODESESCPR] SET [U_SO1_MONTO] = @pMonto, [U_SO1_PORCENTAJEDESC] = @pPorcentajeDesc
                                                     WHERE [Code] = @pCode "),
            new ComandoConsulta(Proveedor.Hana, @"UPDATE ""@SO1_01PROMODESESCPR"" SET ""U_SO1_MONTO"" = @pMonto, ""U_SO1_PORCENTAJEDESC"" = @pPorcentajeDesc
                                                WHERE ""Code"" = @pCode "));


        #endregion

        #region Descuento Importe

        internal static Consulta ConsultarPromocionDescImp = new Consulta(Proveedor.SQLServer,
        new ComandoConsulta(Proveedor.SQLServer, @"SELECT Code Codigo, Name Nombre, U_SO1_PROMOCION Promo, U_SO1_PROMOCADENA PromoCadena, U_SO1_COMPORTAMIENTO Comportamiento, 
                                                 U_SO1_MONEDA Moneda, U_SO1_MONTOMINIMO MontoMinimo, U_SO1_IMPORTEDESC ImporteDesc
                                                 FROM [@SO1_01PROMODESIMP] 
                                                 WHERE U_SO1_PROMOCION = @pPromocion"),
        new ComandoConsulta(Proveedor.Hana, @"SELECT ""Code"" ""Codigo"", ""Name"" ""Nombre"", ""U_SO1_PROMOCION"" ""Promo"", ""U_SO1_PROMOCADENA"" ""PromoCadena"", ""U_SO1_COMPORTAMIENTO"" ""Comportamiento"", 
                                            ""U_SO1_MONEDA"" ""Moneda"", ""U_SO1_MONTOMINIMO"" ""MontoMinimo"", ""U_SO1_IMPORTEDESC"" ""ImporteDesc""
                                            FROM ""@SO1_01PROMODESIMP"" 
                                            WHERE ""U_SO1_PROMOCION"" = @pPromocion"));

        internal static Consulta RegistrarPromocionDescImp = new Consulta(Proveedor.SQLServer,
          new ComandoConsulta(Proveedor.SQLServer, @"INSERT INTO [@SO1_01PROMODESIMP] ([Code], [Name], [U_SO1_PROMOCION], [U_SO1_PROMOCADENA], [U_SO1_COMPORTAMIENTO],
                                                   [U_SO1_MONEDA], [U_SO1_MONTOMINIMO], [U_SO1_IMPORTEDESC]) 
                                                   VALUES (@pCodigo, @pNombre, @pPromo, @pPromoCadena, @pComportamiento, @pMoneda, @pMontoMinimo, @pImporteDesc)"),
          new ComandoConsulta(Proveedor.Hana, @"INSERT INTO ""@SO1_01PROMODESIMP"" (""Code"", ""Name"", ""U_SO1_PROMOCION"", ""U_SO1_PROMOCADENA"", ""U_SO1_COMPORTAMIENTO"",
                                              ""U_SO1_MONEDA"", ""U_SO1_MONTOMINIMO"", ""U_SO1_IMPORTEDESC"") 
                                              VALUES (@pCodigo, @pNombre, @pPromo, @pPromoCadena, @pComportamiento, @pMoneda, @pMontoMinimo, @pImporteDesc)"));

        internal static Consulta ModificarPromocionDescImp = new Consulta(Proveedor.SQLServer,
            new ComandoConsulta(Proveedor.SQLServer, @"UPDATE [@SO1_01PROMODESIMP] SET [Name] = @pNombre, [U_SO1_COMPORTAMIENTO] = @pComportamiento, [U_SO1_MONEDA] = @pMoneda,
                                                     [U_SO1_MONTOMINIMO] = @pMontoMinimo, [U_SO1_IMPORTEDESC] = @pImporteDesc
                                                     WHERE [Code] = @pCode "),
            new ComandoConsulta(Proveedor.Hana, @"UPDATE ""@SO1_01PROMODESIMP"" SET ""Name"" = @pNombre, ""U_SO1_COMPORTAMIENTO"" = @pComportamiento, ""U_SO1_MONEDA"" = @pMoneda,
                                                ""U_SO1_MONTOMINIMO"" = @pMontoMinimo, ""U_SO1_IMPORTEDESC"" = @pImporteDesc
                                                WHERE ""Code"" = @pCode "));

        #endregion

        #region Descuento Porcentaje

        internal static Consulta ConsultarPromocionDescPor = new Consulta(Proveedor.SQLServer,
        new ComandoConsulta(Proveedor.SQLServer, @"SELECT Code Codigo, Name Nombre, U_SO1_PROMOCION Promo, U_SO1_PROMOCADENA PromoCadena, U_SO1_DESCUENTO1 Descuento1, 
                                                     U_SO1_DESCUENTO2 Descuento2, U_SO1_MANTENERDESC MantenerDesc
                                                     FROM [@SO1_01PROMODESPOR] 
                                                     WHERE U_SO1_PROMOCION = @pPromocion"),
        new ComandoConsulta(Proveedor.Hana, @"SELECT ""Code"" ""Codigo"", ""Name"" ""Nombre"", ""U_SO1_PROMOCION"" ""Promo"", ""U_SO1_PROMOCADENA"" ""PromoCadena"", ""U_SO1_DESCUENTO1"" ""Descuento1"", 
                                            ""U_SO1_DESCUENTO2"" ""Descuento2"", ""U_SO1_MANTENERDESC"" ""MantenerDesc""
                                            FROM ""@SO1_01PROMODESPOR"" 
                                            WHERE ""U_SO1_PROMOCION"" = @pPromocion"));

        internal static Consulta RegistrarPromocionDescPor = new Consulta(Proveedor.SQLServer,
          new ComandoConsulta(Proveedor.SQLServer, @"INSERT INTO [@SO1_01PROMODESPOR] ([Code], [Name], [U_SO1_PROMOCION], [U_SO1_PROMOCADENA], [U_SO1_DESCUENTO1],
                                                   [U_SO1_DESCUENTO2], [U_SO1_MANTENERDESC]) 
                                                   VALUES (@pCodigo, @pNombre, @pPromo, @pPromoCadena, @pDescuento1, @pDescuento2, @pMantenerDesc)"),
          new ComandoConsulta(Proveedor.Hana, @"INSERT INTO ""@SO1_01PROMODESPOR"" (""Code"", ""Name"", ""U_SO1_PROMOCION"", ""U_SO1_PROMOCADENA"", ""U_SO1_DESCUENTO1"",
                                              ""U_SO1_DESCUENTO2"", ""U_SO1_MANTENERDESC"") 
                                              VALUES (@pCodigo, @pNombre, @pPromo, @pPromoCadena, @pDescuento1, @pDescuento2, @pMantenerDesc)"));

        internal static Consulta ModificarPromocionDescPor = new Consulta(Proveedor.SQLServer,
            new ComandoConsulta(Proveedor.SQLServer, @"UPDATE [@SO1_01PROMODESPOR] SET [Name]=@pNombre, [U_SO1_DESCUENTO1] = @pDescuento1, [U_SO1_DESCUENTO2] = @pDescuento2,
                                                     [U_SO1_MANTENERDESC] = @pMantenerDesc
                                                     WHERE [Code] = @pCode "),
            new ComandoConsulta(Proveedor.Hana, @"UPDATE ""@SO1_01PROMODESPOR"" SET ""Name""=@pNombre, ""U_SO1_DESCUENTO1"" = @pDescuento1, ""U_SO1_DESCUENTO2"" = @pDescuento2,
                                                ""U_SO1_MANTENERDESC"" = @pMantenerDesc
                                                WHERE ""Code"" = @pCode "));

        #endregion

        #region Descuento Volumen

        internal static Consulta ConsultarPromocionDescVol = new Consulta(Proveedor.SQLServer,
        new ComandoConsulta(Proveedor.SQLServer, @"SELECT Code Codigo, Name Nombre, U_SO1_PROMOCION Promo, U_SO1_PROMOCADENA PromoCadena, U_SO1_CANTIDAD Cantidad, 
                                                     U_SO1_PORCENTAJEDESC PorcentajeDesc, U_SO1_MANTENERDESC MantenerDesc, U_SO1_ACUMULARART AcumulaArt, U_SO1_ACTDESCINCRE ActDescuentoIncre
                                                     FROM [@SO1_01PROMODESVOL] 
                                                     WHERE U_SO1_PROMOCION = @pPromocion"),
        new ComandoConsulta(Proveedor.Hana, @"SELECT ""Code"" ""Codigo"", ""Name"" ""Nombre"", ""U_SO1_PROMOCION"" ""Promo"", ""U_SO1_PROMOCADENA"" ""PromoCadena"", ""U_SO1_CANTIDAD"" ""Cantidad"", 
                                            ""U_SO1_PORCENTAJEDESC"" ""PorcentajeDesc"", ""U_SO1_MANTENERDESC"" ""MantenerDesc"", ""U_SO1_ACUMULARART"" ""AcumulaArt"", ""U_SO1_ACTDESCINCRE"" ""ActDescuentoIncre""
                                            FROM ""@SO1_01PROMODESVOL"" 
                                            WHERE ""U_SO1_PROMOCION"" = @pPromocion"));

        internal static Consulta RegistrarPromocionDescVol = new Consulta(Proveedor.SQLServer,
          new ComandoConsulta(Proveedor.SQLServer, @"INSERT INTO [@SO1_01PROMODESVOL] ([Code], [Name], [U_SO1_PROMOCION], [U_SO1_PROMOCADENA], [U_SO1_CANTIDAD],
                                                   [U_SO1_PORCENTAJEDESC], [U_SO1_MANTENERDESC], [U_SO1_ACUMULARART], [U_SO1_ACTDESCINCRE]) 
                                                   VALUES (@pCodigo, @pNombre, @pPromo, @pPromoCadena, @pCantidad, @pPorcentajeDesc, @pMantenerDesc, @pAcumulaArt, @pActDesIncre)"),
          new ComandoConsulta(Proveedor.Hana, @"INSERT INTO ""@SO1_01PROMODESVOL"" (""Code"", ""Name"", ""U_SO1_PROMOCION"", ""U_SO1_PROMOCADENA"", ""U_SO1_CANTIDAD"",
                                              ""U_SO1_PORCENTAJEDESC"", ""U_SO1_MANTENERDESC"", ""U_SO1_ACUMULARART"", ""U_SO1_ACTDESCINCRE"") 
                                              VALUES (@pCodigo, @pNombre, @pPromo, @pPromoCadena, @pCantidad, @pPorcentajeDesc, @pMantenerDesc, @pAcumulaArt, @pActDesIncre)"));

        internal static Consulta ModificarPromocionDescVol = new Consulta(Proveedor.SQLServer,
            new ComandoConsulta(Proveedor.SQLServer, @"UPDATE [@SO1_01PROMODESVOL] SET [Name] = @pNombre, [U_SO1_CANTIDAD] = @pCantidad, [U_SO1_PORCENTAJEDESC] = @pPorcentajeDesc,
                                                     [U_SO1_MANTENERDESC] = @pMantenerDesc, [U_SO1_ACUMULARART] = @pAcumulaArt, [U_SO1_ACTDESCINCRE] = @pActDesIncre
                                                     WHERE [Code] = @pCode "),
            new ComandoConsulta(Proveedor.Hana, @"UPDATE ""@SO1_01PROMODESVOL"" SET ""Name"" = @pNombre, ""U_SO1_CANTIDAD"" = @pCantidad, ""U_SO1_PORCENTAJEDESC"" = @pPorcentajeDesc,
                                                ""U_SO1_MANTENERDESC"" = @pMantenerDesc, ""U_SO1_ACUMULARART"" = @pAcumulaArt, ""U_SO1_ACTDESCINCRE"" = @pActDesIncre
                                                WHERE ""Code"" = @pCode "));

        internal static Consulta ConsultarPromocionDescVolEs = new Consulta(Proveedor.SQLServer,
        new ComandoConsulta(Proveedor.SQLServer, @"SELECT Code Codigo, Name Nombre, U_SO1_PROMOCION Promo, U_SO1_PROMOCADENA PromoCadena, U_SO1_CANTIDAD Cantidad, 
                                                     U_SO1_PORCENTAJEDESC PorcentajeDesc, U_SO1_MANTENERDESC MantenerDesc
                                                     FROM [@SO1_01PROMODESVOLES] 
                                                     WHERE U_SO1_PROMOCION = @pPromocion"),
        new ComandoConsulta(Proveedor.Hana, @"SELECT ""Code"" ""Codigo"", ""Name"" ""Nombre"", ""U_SO1_PROMOCION"" ""Promo"", ""U_SO1_PROMOCADENA"" ""PromoCadena"", ""U_SO1_CANTIDAD"" ""Cantidad"", 
                                            ""U_SO1_PORCENTAJEDESC"" ""PorcentajeDesc"", ""U_SO1_MANTENERDESC"" ""MantenerDesc""
                                            FROM ""@SO1_01PROMODESVOLES"" 
                                            WHERE ""U_SO1_PROMOCION"" = @pPromocion"));

        internal static Consulta RegistrarPromocionDescVolEs = new Consulta(Proveedor.SQLServer,
          new ComandoConsulta(Proveedor.SQLServer, @"INSERT INTO [@SO1_01PROMODESVOLES] ([Code], [Name], [U_SO1_PROMOCION], [U_SO1_PROMOCADENA], [U_SO1_CANTIDAD],
                                                   [U_SO1_PORCENTAJEDESC], [U_SO1_MANTENERDESC]) 
                                                   VALUES (@pCodigo, @pNombre, @pPromo, @pPromoCadena, @pCantidad, @pPorcentajeDesc, @pMantenerDesc)"),
          new ComandoConsulta(Proveedor.Hana, @"INSERT INTO ""@SO1_01PROMODESVOLES"" (""Code"", ""Name"", ""U_SO1_PROMOCION"", ""U_SO1_PROMOCADENA"", ""U_SO1_CANTIDAD"",
                                              ""U_SO1_PORCENTAJEDESC"", ""U_SO1_MANTENERDESC"") 
                                              VALUES (@pCodigo, @pNombre, @pPromo, @pPromoCadena, @pCantidad, @pPorcentajeDesc, @pMantenerDesc)"));

        internal static Consulta ModificarPromocionDescVolEs = new Consulta(Proveedor.SQLServer,
            new ComandoConsulta(Proveedor.SQLServer, @"UPDATE [@SO1_01PROMODESVOLES] SET [U_SO1_CANTIDAD] = @pCantidad, [U_SO1_PORCENTAJEDESC] = @pPorcentajeDesc,
                                                     [U_SO1_MANTENERDESC] = @pMantenerDesc
                                                     WHERE [Code] = @pCode "),
            new ComandoConsulta(Proveedor.Hana, @"UPDATE ""@SO1_01PROMODESVOLES"" SET ""U_SO1_CANTIDAD"" = @pCantidad, ""U_SO1_PORCENTAJEDESC"" = @pPorcentajeDesc,
                                                ""U_SO1_MANTENERDESC"" = @pMantenerDesc
                                                WHERE ""Code"" = @pCode "));

        internal static Consulta ConsultarCodigosDescuentos = new Consulta(Proveedor.SQLServer,
            new ComandoConsulta(Proveedor.SQLServer, @"SELECT Code Codigo FROM [${TABLA}] WHERE U_SO1_PROMOCION = @pPromocion"),
            new ComandoConsulta(Proveedor.Hana, @"SELECT ""Code"" ""Codigo"" FROM ""${TABLA}"" WHERE ""U_SO1_PROMOCION"" = @pPromocion"));

        #endregion

        #region Kit Regalo

        internal static Consulta ConsultarPromocionKitReg = new Consulta(Proveedor.SQLServer,
            new ComandoConsulta(Proveedor.SQLServer, @"SELECT Code Codigo, Name Nombre, U_SO1_PROMOCION Promo, U_SO1_PROMOCADENA PromoCadena, 
                                                         U_SO1_CANTIDADREG CantidadReg, U_SO1_CANTIDADCRI CantidadCri, U_SO1_TIPOREGALO TipoRegalo, U_SO1_MONTO Monto, 
                                                         U_SO1_REGALOPRECIO1 RegaloPrecio, U_SO1_AGREGARARTAUTO AgregarArt
                                                     FROM [@SO1_01PROMOKITREG] 
                                                     WHERE U_SO1_PROMOCION = @pPromocion"),
            new ComandoConsulta(Proveedor.Hana, @"SELECT ""Code"" ""Codigo"", ""Name"" ""Nombre"", ""U_SO1_PROMOCION"" ""Promo"", ""U_SO1_PROMOCADENA"" ""PromoCadena"", 
                                                    ""U_SO1_CANTIDADREG"" ""CantidadReg"", ""U_SO1_CANTIDADCRI"" ""CantidadCri"", ""U_SO1_TIPOREGALO"" ""TipoRegalo"", ""U_SO1_MONTO"" ""Monto"", 
                                                    ""U_SO1_REGALOPRECIO1"" ""RegaloPrecio"", ""U_SO1_AGREGARARTAUTO"" ""AgregarArt""
                                                    FROM ""@SO1_01PROMOKITREG"" 
                                                    WHERE ""U_SO1_PROMOCION"" = @pPromocion"));

        internal static Consulta ModificarPromocionKitReg = new Consulta(Proveedor.SQLServer,
            new ComandoConsulta(Proveedor.SQLServer, @"UPDATE [@SO1_01PROMOKITREG] SET [Name] = @pNombre, [U_SO1_CANTIDADCRI] = @pCantidadCri, [U_SO1_CANTIDADREG] = @pCantidadReg,
                                                     [U_SO1_TIPOREGALO] = @pTipoRegalo, [U_SO1_MONTO] = @pMonto, [U_SO1_REGALOPRECIO1] = @pRegaloPrecio,
                                                     [U_SO1_AGREGARARTAUTO] = @pAgregarArt
                                                     WHERE [Code] = @pCode"),
            new ComandoConsulta(Proveedor.Hana, @"UPDATE ""@SO1_01PROMOKITREG"" SET ""Name"" = @pNombre, ""U_SO1_CANTIDADCRI"" = @pCantidadCri, ""U_SO1_CANTIDADREG"" = @pCantidadReg,
                                                ""U_SO1_TIPOREGALO"" = @pTipoRegalo, ""U_SO1_MONTO"" = @pMonto, ""U_SO1_REGALOPRECIO1"" = @pRegaloPrecio,
                                                ""U_SO1_AGREGARARTAUTO"" = @pAgregarArt
                                                WHERE ""Code"" = @pCode"));

        internal static Consulta RegistrarPromocionKitReg = new Consulta(Proveedor.SQLServer,
          new ComandoConsulta(Proveedor.SQLServer, @"INSERT INTO [@SO1_01PROMOKITREG] ([Code], [Name], [U_SO1_PROMOCION], [U_SO1_PROMOCADENA], [U_SO1_CANTIDADCRI],
                                                   [U_SO1_CANTIDADREG], [U_SO1_TIPOREGALO], [U_SO1_MONTO], [U_SO1_REGALOPRECIO1], [U_SO1_AGREGARARTAUTO]) 
                                                   VALUES (@pCodigo, @pNombre, @pPromo, @pPromoCadena, @pCantidadCri, @pCantidadReg, @pTipoRegalo, @pMonto,
                                                   @pRegaloPrecio, @pAgregarArt)"),
          new ComandoConsulta(Proveedor.Hana, @"INSERT INTO ""@SO1_01PROMOKITREG"" (""Code"", ""Name"", ""U_SO1_PROMOCION"", ""U_SO1_PROMOCADENA"", ""U_SO1_CANTIDADCRI"",
                                              ""U_SO1_CANTIDADREG"", ""U_SO1_TIPOREGALO"", ""U_SO1_MONTO"", ""U_SO1_REGALOPRECIO1"", ""U_SO1_AGREGARARTAUTO"") 
                                              VALUES (@pCodigo, @pNombre, @pPromo, @pPromoCadena, @pCantidadCri, @pCantidadReg, @pTipoRegalo, @pMonto,
                                              @pRegaloPrecio, @pAgregarArt)"));

        #endregion

        #region Kit de venta

        internal static Consulta ConsultarKitVenta = new Consulta(Proveedor.SQLServer,
            new ComandoConsulta(Proveedor.SQLServer, @"SELECT Code, Name, U_SO1_PROMOCION, U_SO1_PROMOCADENA, U_SO1_TIPOREGALO, U_SO1_CONIMPUESTO FROM [@SO1_01PROMOKITVEN] WHERE U_SO1_PROMOCION = @pPromocion"),
            new ComandoConsulta(Proveedor.Hana, @"SELECT ""Code"", ""Name"", ""U_SO1_PROMOCION"", ""U_SO1_PROMOCADENA"", ""U_SO1_TIPOREGALO"", ""U_SO1_CONIMPUESTO"" FROM ""@SO1_01PROMOKITVEN"" WHERE ""U_SO1_PROMOCION"" = @pPromocion"));

        internal static Consulta RegistrarKitVenta = new Consulta(Proveedor.SQLServer,
            new ComandoConsulta(Proveedor.SQLServer, @"INSERT INTO [@SO1_01PROMOKITVEN] (Code,Name,U_SO1_PROMOCION,U_SO1_PROMOCADENA,U_SO1_TIPOREGALO,U_SO1_CONIMPUESTO)
                                                     VALUES(@pCode, @pName, @pPromocion, @pPromoCadena, @pTipoRegalo, @pConImpuesto)"),
            new ComandoConsulta(Proveedor.Hana, @"INSERT INTO ""@SO1_01PROMOKITVEN"" (""Code"", ""Name"", ""U_SO1_PROMOCION"", ""U_SO1_PROMOCADENA"", ""U_SO1_TIPOREGALO"", ""U_SO1_CONIMPUESTO"")
                                                VALUES(@pCode, @pName, @pPromocion, @pPromoCadena, @pTipoRegalo, @pConImpuesto)"));

        internal static Consulta ModificarPromocionKitVenta = new Consulta(Proveedor.SQLServer,
               new ComandoConsulta(Proveedor.SQLServer, @"UPDATE [@SO1_01PROMOKITVEN] SET [Name] = @pNombre, [U_SO1_TIPOREGALO] = @pTipoRegalo, [U_SO1_CONIMPUESTO] = @pConImpuesto
                                                          WHERE [Code] = @pCode"),
               new ComandoConsulta(Proveedor.Hana, @"UPDATE ""@SO1_01PROMOKITVEN"" SET ""Name"" = @pNombre, ""U_SO1_TIPOREGALO"" = @pTipoRegalo, ""U_SO1_CONIMPUESTO"" = @pCImpuesto 
                                                     WHERE ""Code"" = @pCode"));
        internal static Consulta tipoAmbito = new Consulta(Proveedor.SQLServer,
        new ComandoConsulta(Proveedor.SQLServer, @"SELECT MAX(U_SO1_CODIGOAMBITO) tipo FROM [@SO1_01PROMOAMBITO]"),
        new ComandoConsulta(Proveedor.Hana, @"SELECT MAX(""U_SO1_CODIGOAMBITO"") ""tipo"" FROM ""@SO1_01PROMOAMBITO"""));

        internal static Consulta RegistrarIdentificador = new Consulta(Proveedor.SQLServer,
            new ComandoConsulta(Proveedor.SQLServer, @"INSERT INTO [@SO1_01PROMOAMBITO] (Code, Name, U_SO1_PROMOCION, U_SO1_PROMOCADENA, U_SO1_CODIGOAMBITO, 
                                                     U_SO1_NOMBREAMBITO, U_SO1_CANTIDAD, U_SO1_IMAGEN, U_SO1_MANTENERPROMO, U_SO1_AMBOBLIGATORIO) 
                                                     VALUES (@pCode, @pName, @pPromocion, @pPromoCadena, @pCodigoAmbito, @pNombreAmbito, @pCantidad, 
                                                     @pImagen, @pMantenerPromo, @pAmbitOblig)"),
            new ComandoConsulta(Proveedor.Hana, @"INSERT INTO ""@SO1_01PROMOAMBITO"" (""Code"", ""Name"", ""U_SO1_PROMOCION"", ""U_SO1_PROMOCADENA"", ""U_SO1_CODIGOAMBITO"", 
                                                ""U_SO1_NOMBREAMBITO"", ""U_SO1_CANTIDAD"", ""U_SO1_IMAGEN"", ""U_SO1_MANTENERPROMO"", ""U_SO1_AMBOBLIGATORIO"") 
                                                VALUES (@pCode, @pName, @pPromocion, @pPromoCadena, @pCodigoAmbito, @pNombreAmbito, @pCantidad, 
                                                @pImagen, @pMantenerPromo, @pAmbitObligatorio)"));

        internal static Consulta RegistrarArticuloIdentificador = new Consulta(Proveedor.SQLServer,
            new ComandoConsulta(Proveedor.SQLServer, @"INSERT INTO [@SO1_01PROMOARTICULO] (Code, Name, U_SO1_PROMOCION, U_SO1_PROMOCADENA, U_SO1_TIPO, U_SO1_ARTICULO, U_SO1_IMPORTE, U_SO1_CODIGOAMBITO)
                                                     VALUES (@pCode, @pName, @pPromocion, @pPromoCadena, @pTipo, @pArticulo, @pImporte, @pCodigoAmbito)"),
            new ComandoConsulta(Proveedor.Hana, @"INSERT INTO ""@SO1_01PROMOARTICULO"" (""Code"", ""Name"", ""U_SO1_PROMOCION"", ""U_SO1_PROMOCADENA"", ""U_SO1_TIPO"", ""U_SO1_ARTICULO"", ""U_SO1_IMPORTE"", ""U_SO1_CODIGOAMBITO"")
                                                VALUES (@pCode, @pName, @pPromocion, @pPromoCadena, @pTipo, @pArticulo, @pImporte, @pCodigoAmbito)"));

        internal static Consulta ModificarIdentificador = new Consulta(Proveedor.SQLServer,
            new ComandoConsulta(Proveedor.SQLServer, @"UPDATE [@SO1_01PROMOAMBITO] SET U_SO1_NOMBREAMBITO = @pNombreAmbito, U_SO1_CANTIDAD = @pCantidad, 
                                                     U_SO1_MANTENERPROMO = @pMantenerPromo, U_SO1_AMBOBLIGATORIO = @pAmbitOblig WHERE [Code] = @pCode"),
            new ComandoConsulta(Proveedor.Hana, @"UPDATE ""@SO1_01PROMOAMBITO"" SET ""U_SO1_NOMBREAMBITO"" = @pNombreAmbito, ""U_SO1_CANTIDAD"" = @pCantidad, 
                                                ""U_SO1_MANTENERPROMO"" = @pMantenerPromo, ""U_SO1_AMBOBLIGATORIO"" = @pAmbitOblig"));

        internal static Consulta ConsultaIdentificador = new Consulta(Proveedor.SQLServer,
                new ComandoConsulta(Proveedor.SQLServer, @"SELECT Code Codigo, Name Nombre, U_SO1_PROMOCION Promo, U_SO1_PROMOCADENA PromoCadena, U_SO1_CODIGOAMBITO CodigoAmbito, U_SO1_NOMBREAMBITO NombreAmbito, U_SO1_CANTIDAD Cantidad, U_SO1_IMAGEN Imagen, U_SO1_MANTENERPROMO MantenerPromo, U_SO1_AMBOBLIGATORIO AmbitObligatorio
                                                            from [@SO1_01PROMOAMBITO] where U_SO1_PROMOCION = @pPromocion"),
                new ComandoConsulta(Proveedor.Hana, @"SELECT ""Code"" ""Codigo"", ""Name"" ""Nombre"", ""U_SO1_PROMOCION"" ""Promo"", ""U_SO1_PROMOCADENA"" ""PromoCadena"", ""U_SO1_CODIGOAMBITO"" ""CodigoAmbito"", ""U_SO1_NOMBREAMBITO"" ""NombreAmbito"", ""U_SO1_CANTIDAD"" ""Cantidad"", ""U_SO1_IMAGEN"" ""Imagen"", ""U_SO1_MANTENERPROMO"" ""MantenerPromo"", ""U_SO1_AMBOBLIGATORIO"" ""AmbitObligatorio""
                                                    FROM ""@SO1_01PROMOAMBITO"" WHERE ""U_SO1_PROMOCION"" = @pPromocion"));

        internal static Consulta ConsultarIdentificadorArticulo = new Consulta(Proveedor.SQLServer,
                new ComandoConsulta(Proveedor.SQLServer, @"SELECT A.Code Codigo, A.Name Nombre, A.U_SO1_PROMOCION Promo, A.U_SO1_PROMOCADENA PromoCadena,
                                                         A.U_SO1_TIPO Tipo, A.U_SO1_ARTICULO Articulo, A.U_SO1_IMPORTE Importe, A.U_SO1_CODIGOAMBITO CodigoAmbito,B.ItemName NombreArticulo 
                                                         FROM [@SO1_01PROMOARTICULO] A LEFT JOIN [OITM] B ON (A.U_SO1_ARTICULO = B.ItemCode)  
                                                         WHERE A.U_SO1_CODIGOAMBITO = @pTipo and A.U_SO1_PROMOCION = @pPromocion"),
                new ComandoConsulta(Proveedor.Hana, @"SELECT A.""Code"" ""Codigo"", A.""Name"" ""Nombre"", A.""U_SO1_PROMOCION"" ""Promo"", A.""U_SO1_PROMOCADENA"" ""PromoCadena"",
                                                    A.""U_SO1_TIPO"" ""Tipo"", A.""U_SO1_ARTICULO"" ""Articulo"", A.""U_SO1_IMPORTE"" ""Importe"", A.""U_SO1_CODIGOAMBITO"" ""CodigoAmbito"", B.""ItemName"" ""NombreArticulo"" 
                                                    FROM ""@SO1_01PROMOARTICULO"" A LEFT JOIN ""OITM"" B ON (A.""U_SO1_ARTICULO"" = B.""ItemCode"")  
                                                    WHERE A.""U_SO1_CODIGOAMBITO"" = @pTipo and A.""U_SO1_PROMOCION"" = @pPromocion"));

        internal static Consulta ConsultaArticulo = new Consulta(Proveedor.SQLServer,
            new ComandoConsulta(Proveedor.SQLServer, @"SELECT * FROM [@SO1_01PROMOARTICULO] where Code = @pCode"),
            new ComandoConsulta(Proveedor.Hana, @"SELECT * FROM ""@SO1_01PROMOARTICULO"" WHERE ""Code"" = @pCode"));

        internal static Consulta EliminarIdentificadores = new Consulta(Proveedor.SQLServer,
            new ComandoConsulta(Proveedor.SQLServer, @"DELETE FROM ""${TABLA}"" WHERE U_SO1_PROMOCION = @pCodigo AND U_SO1_CODIGOAMBITO = @pIdentificador"),
            new ComandoConsulta(Proveedor.Hana, @"DELETE FROM ""${TABLA}"" WHERE ""U_SO1_PROMOCION"" = @pCodigo AND ""U_SO1_CODIGOAMBITO"" = @pIdentificador"));


        internal static Consulta ConsultarIdentificadores = new Consulta(Proveedor.SQLServer,
            new ComandoConsulta(Proveedor.SQLServer, @"SELECT * FROM [@SO1_01PROMOAMBITO] WHERE U_SO1_PROMOCION = @pPromo AND U_SO1_CODIGOAMBITO = @pCodigoAMbito"),
            new ComandoConsulta(Proveedor.Hana, @"SELECT * FROM ""@SO1_01PROMOAMBITO"" WHERE ""U_SO1_PROMOCION"" = @pPromo AND ""U_SO1_CODIGOAMBITO"" = @pCodigoAMbito"));

        #endregion

        #region Politica Venta

        internal static Consulta ConsultarPromocionPolVenta = new Consulta(Proveedor.SQLServer,
           new ComandoConsulta(Proveedor.SQLServer, @"SELECT Code Codigo, Name Nombre, U_SO1_PROMOCION Promo, U_SO1_PROMOCADENA PromoCadena, 
                                                    U_SO1_ACUMULARART AcumulaArt, U_SO1_MANTENERDESC MantenerDesc
                                                    FROM [@SO1_01PROMOPOLVEN] 
                                                    WHERE U_SO1_PROMOCION = @pPromocion"),
           new ComandoConsulta(Proveedor.Hana, @"SELECT ""Code"" ""Codigo"", ""Name"" ""Nombre"", ""U_SO1_PROMOCION"" ""Promo"", ""U_SO1_PROMOCADENA"" ""PromoCadena"", 
                                               ""U_SO1_ACUMULARART"" ""AcumulaArt"", ""U_SO1_MANTENERDESC"" ""MantenerDesc""
                                               FROM ""@SO1_01PROMOPOLVEN"" 
                                               WHERE ""U_SO1_PROMOCION"" = @pPromocion"));

        internal static Consulta ConsultarListaPreciosPromo = new Consulta(Proveedor.SQLServer,
           new ComandoConsulta(Proveedor.SQLServer, @"SELECT PROMO.Code Codigo, PROMO.Name Nombre, PROMO.U_SO1_PROMOCION Promo, PROMO.U_SO1_PROMOCADENA PromoCadena, 
                                                    PROMO.U_SO1_CANTIDADINI CantidadIni, PROMO.U_SO1_LISTAPRECIO ListaPrecio, LISTA.ListName NombreLista
                                                    FROM [@SO1_01PROMOPOLVENLI]PROMO
                                                    JOIN [OPLN]LISTA ON (PROMO.U_SO1_LISTAPRECIO = LISTA.ListNum)
                                                    WHERE PROMO.U_SO1_PROMOCION = @pPromocion"),
           new ComandoConsulta(Proveedor.Hana, @"SELECT PROMO.""Code"" ""Codigo"", PROMO.""Name"" ""Nombre"", PROMO.""U_SO1_PROMOCION"" ""Promo"", PROMO.""U_SO1_PROMOCADENA"" ""PromoCadena"", 
                                               PROMO.""U_SO1_CANTIDADINI"" ""CantidadIni"", PROMO.""U_SO1_LISTAPRECIO"" ""ListaPrecio"", LISTA.""ListName"" ""NombreLista""
                                               FROM ""@SO1_01PROMOPOLVENLI""PROMO
                                               JOIN ""OPLN""LISTA ON (PROMO.""U_SO1_LISTAPRECIO"" = LISTA.""ListNum"") 
                                               WHERE PROMO.""U_SO1_PROMOCION"" = @pPromocion"));

        internal static Consulta RegistrarListaPrecio = new Consulta(Proveedor.SQLServer,
               new ComandoConsulta(Proveedor.SQLServer, @"INSERT INTO [@SO1_01PROMOPOLVENLI] ([Code], [Name], [U_SO1_PROMOCION], [U_SO1_PROMOCADENA], [U_SO1_CANTIDADINI], 
                                                       [U_SO1_LISTAPRECIO]) 
                                                       VALUES (@pCodigo, @pNombre, @pPromo, @pPromoCadena, @pCantidadIni, @pListaPrecio)"),
               new ComandoConsulta(Proveedor.Hana, @"INSERT INTO ""@SO1_01PROMOPOLVENLI"" (""Code"", ""Name"", ""U_SO1_PROMOCION"", ""U_SO1_PROMOCADENA"", ""U_SO1_CANTIDADINI"",
                                                   ""U_SO1_LISTAPRECIO"") 
                                                   VALUES (@pCodigo, @pNombre, @pPromo, @pPromoCadena, @pCantidadIni, @pListaPrecio)"));

        internal static Consulta ModificarPromocionPolVenta = new Consulta(Proveedor.SQLServer,
            new ComandoConsulta(Proveedor.SQLServer, @"UPDATE [@SO1_01PROMOPOLVEN] SET [Name] = @pNombre, [U_SO1_ACUMULARART] = @pAcumulaArt, [U_SO1_MANTENERDESC] = @pMantenerDesc
                                                         WHERE [Code] = @pCode"),
            new ComandoConsulta(Proveedor.Hana, @"UPDATE ""@SO1_01PROMOPOLVEN"" SET ""Name"" = @pNombre, ""U_SO1_ACUMULARART"" = @pAcumulaArt, ""U_SO1_MANTENERDESC"" = @pMantenerDesc
                                                    WHERE ""Code"" = @pCode"));

        internal static Consulta RegistrarPromocionPolVenta = new Consulta(Proveedor.SQLServer,
          new ComandoConsulta(Proveedor.SQLServer, @"INSERT INTO [@SO1_01PROMOPOLVEN] ([Code], [Name], [U_SO1_PROMOCION], [U_SO1_PROMOCADENA], [U_SO1_ACUMULARART],
                                                       [U_SO1_MANTENERDESC]) 
                                                       VALUES (@pCodigo, @pNombre, @pPromo, @pPromoCadena, @pAcumulaArt, @pMantenerDesc)"),
          new ComandoConsulta(Proveedor.Hana, @"INSERT INTO ""@SO1_01PROMOPOLVEN"" (""Code"", ""Name"", ""U_SO1_PROMOCION"", ""U_SO1_PROMOCADENA"", ""U_SO1_ACUMULARART"",
                                                  ""U_SO1_MANTENERDESC"") 
                                                  VALUES (@pCodigo, @pNombre, @pPromo, @pPromoCadena, @pAcumulaArt, @pMantenerDesc)"));

        internal static Consulta ConsultarListaPreciosPromo2 = new Consulta(Proveedor.SQLServer,
              new ComandoConsulta(Proveedor.SQLServer, @"SELECT Code Codigo, U_SO1_LISTAPRECIO ListaPrecio
                                                        FROM [@SO1_01PROMOPOLVENLI]
                                                        WHERE U_SO1_PROMOCION = @pPromocion"),
              new ComandoConsulta(Proveedor.Hana, @"SELECT ""Code"" ""Codigo"", ""U_SO1_LISTAPRECIO"" ""ListaPrecio""
                                                  FROM ""@SO1_01PROMOPOLVENLI""
                                                  WHERE ""U_SO1_PROMOCION"" = @pPromocion"));
        #endregion

        #region Regalo Monto

        internal static Consulta ConsultarPromocionRegaloMonto = new Consulta(Proveedor.SQLServer,
           new ComandoConsulta(Proveedor.SQLServer, @"SELECT Code Codigo, Name Nombre, U_SO1_PROMOCION Promo, U_SO1_PROMOCADENA PromoCadena, 
                                                    U_SO1_AGREGARARTAUTO AgregarArt, U_SO1_REGALOPRECIO1 RegaloPrecio, U_SO1_PORCADAXMONTO xMonto,
                                                    U_SO1_PORCENTAJEPRO PorcentajePro, U_SO1_MENSAJEPROXIM Mensaje, U_SO1_MONEDA Moneda,
                                                    U_SO1_MONTOREGALO MontoRegalo, U_SO1_CANTIDADREGAL CantidadRegalo
                                                    FROM [@SO1_01PROMOREGMON] 
                                                    WHERE U_SO1_PROMOCION = @pPromocion"),
           new ComandoConsulta(Proveedor.Hana, @"SELECT ""Code"" ""Codigo"", ""Name"" ""Nombre"", ""U_SO1_PROMOCION"" ""Promo"", ""U_SO1_PROMOCADENA"" ""PromoCadena"", 
                                               ""U_SO1_AGREGARARTAUTO"" ""AgregarArt"", ""U_SO1_REGALOPRECIO1"" ""RegaloPrecio"", ""U_SO1_PORCADAXMONTO"" ""xMonto"",
                                               ""U_SO1_PORCENTAJEPRO"" ""PorcentajePro"", ""U_SO1_MENSAJEPROXIM"" ""Mensaje"", ""U_SO1_MONEDA"" ""Moneda"",
                                               ""U_SO1_MONTOREGALO"" ""MontoRegalo"", ""U_SO1_CANTIDADREGAL"" ""CantidadRegalo""
                                               FROM ""@SO1_01PROMOREGMON"" 
                                               WHERE ""U_SO1_PROMOCION"" = @pPromocion"));

        internal static Consulta ModificarPromocionRegaloMonto = new Consulta(Proveedor.SQLServer,
           new ComandoConsulta(Proveedor.SQLServer, @"UPDATE [@SO1_01PROMOREGMON] SET [Name] = @pNombre, [U_SO1_AGREGARARTAUTO] = @pAcumulaArt, [U_SO1_REGALOPRECIO1] = @pRegaloPrecio,
                                                    [U_SO1_PORCADAXMONTO] = @pxMonto, [U_SO1_PORCENTAJEPRO] = @pPorcentajePro, [U_SO1_MENSAJEPROXIM] = @pMensaje,
                                                    [U_SO1_MONEDA] = @pMoneda, [U_SO1_MONTOREGALO] = @pMontoRegalo, [U_SO1_CANTIDADREGAL] = @pCantidadRegalo
                                                    WHERE [Code] = @pCode "),
           new ComandoConsulta(Proveedor.Hana, @"UPDATE ""@SO1_01PROMOREGMON"" SET ""Name"" = @pNombre, ""U_SO1_AGREGARARTAUTO"" = @pAcumulaArt, ""U_SO1_REGALOPRECIO1"" = @pRegaloPrecio,
                                               ""U_SO1_PORCADAXMONTO"" = @pxMonto, ""U_SO1_PORCENTAJEPRO"" = @pPorcentajePro, ""U_SO1_MENSAJEPROXIM"" = @pMensaje,
                                               ""U_SO1_MONEDA"" = @pMoneda, ""U_SO1_MONTOREGALO"" = @pMontoRegalo, ""U_SO1_CANTIDADREGAL"" = @pCantidadRegalo
                                               WHERE ""Code"" = @pCode "));

        internal static Consulta RegistrarPromocionRegaloMonto = new Consulta(Proveedor.SQLServer,
           new ComandoConsulta(Proveedor.SQLServer, @"INSERT INTO [@SO1_01PROMOREGMON] ([Code], [Name], [U_SO1_PROMOCION], [U_SO1_PROMOCADENA], [U_SO1_AGREGARARTAUTO],
                                                    [U_SO1_REGALOPRECIO1], [U_SO1_PORCADAXMONTO], [U_SO1_PORCENTAJEPRO], [U_SO1_MENSAJEPROXIM], [U_SO1_MONEDA], 
                                                    [U_SO1_MONTOREGALO], [U_SO1_CANTIDADREGAL]) 
                                                    VALUES (@pCodigo, @pNombre, @pPromo, @pPromoCadena, @pAcumulaArt, @pRegaloPrecio, @pxMonto, @pPorcentajePro,
                                                    @pMensaje, @pMoneda, @pMontoRegalo, @pCantidadRegalo) "),
           new ComandoConsulta(Proveedor.Hana, @"INSERT INTO ""@SO1_01PROMOREGMON"" (""Code"", ""Name"", ""U_SO1_PROMOCION"", ""U_SO1_PROMOCADENA"", ""U_SO1_AGREGARARTAUTO"",
                                               ""U_SO1_REGALOPRECIO1"", ""U_SO1_PORCADAXMONTO"", ""U_SO1_PORCENTAJEPRO"", ""U_SO1_MENSAJEPROXIM"", ""U_SO1_MONEDA"", 
                                               ""U_SO1_MONTOREGALO"", ""U_SO1_CANTIDADREGAL"") 
                                               VALUES (@pCodigo, @pNombre, @pPromo, @pPromoCadena, @pAcumulaArt, @pRegaloPrecio, @pxMonto, @pPorcentajePro,
                                               @pMensaje, @pMoneda, @pMontoRegalo, @pCantidadRegalo) "));


        #endregion

        #region Descuento Aleatorio

        internal static Consulta ConsultarPromocionDescuentoAleatorio = new Consulta(Proveedor.SQLServer,
                new ComandoConsulta(Proveedor.SQLServer, @"SELECT Code Codigo, Name Nombre, U_SO1_PROMOCION Promo, U_SO1_PROMOCADENA PromoCadena,
                                                     U_SO1_ACTMONTOMAXIMO ActMontoMaximo, U_SO1_MONTOMAXIPROMO MontoMaxPromo, U_SO1_ACTMONTOMAXDIA ActMontoMaxDia,
                                                     U_SO1_MONTOMAXPORDIA MontoMaxDia, U_SO1_ACTIMPVENDESDE ActImpVende, U_SO1_IMPORTVENDESDE ImporteVende,
                                                     U_SO1_ACTIMPVENHASTA ActImporteVendeHasta, U_SO1_IMPORTVENHASTA ImporteVendeHasta, U_SO1_ACTIMPORTEIMP ActImpoImp,
                                                     U_SO1_COMPOTRASPROM CompoTrasProm, U_SO1_ACTFRECUENPROM ActFrecuencia, U_SO1_MINFRECPROMO MinFrecuencia,
                                                     U_SO1_FECHULTPROMAPL FechaUltima, U_SO1_HORAULTPROMAPL HoraUltima, U_SO1_FECHANTPROAPLI FechaAnt, 
                                                     U_SO1_HORANTPROMAPLI HoraAntPro
                                                     FROM [@SO1_01PROMODESALE] 
                                                     WHERE U_SO1_PROMOCION = @pPromocion"),
                new ComandoConsulta(Proveedor.Hana, @"SELECT ""Code"" ""Codigo"", ""Name"" ""Nombre"", ""U_SO1_PROMOCION"" ""Promo"", ""U_SO1_PROMOCADENA"" ""PromoCadena"",
                                                ""U_SO1_ACTMONTOMAXIMO"" ""ActMontoMaximo"", ""U_SO1_MONTOMAXIPROMO"" ""MontoMaxPromo"", ""U_SO1_ACTMONTOMAXDIA"" ""ActMontoMaxDia"",
                                                ""U_SO1_MONTOMAXPORDIA"" ""MontoMaxDia"", ""U_SO1_ACTIMPVENDESDE"" ""ActImpVende"", ""U_SO1_IMPORTVENDESDE"" ""ImporteVende"",
                                                ""U_SO1_ACTIMPVENHASTA"" ""ActImporteVendeHasta"", ""U_SO1_IMPORTVENHASTA"" ""ImporteVendeHasta"", ""U_SO1_ACTIMPORTEIMP"" ""ActImpoImp"",
                                                ""U_SO1_COMPOTRASPROM"" ""CompoTrasProm"", ""U_SO1_ACTFRECUENPROM"" ""ActFrecuencia"", ""U_SO1_MINFRECPROMO"" ""MinFrecuencia"",
                                                ""U_SO1_FECHULTPROMAPL"" ""FechaUltima"", ""U_SO1_HORAULTPROMAPL"" ""HoraUltima"", ""U_SO1_FECHANTPROAPLI"" ""FechaAnt"", 
                                                ""U_SO1_HORANTPROMAPLI"" ""HoraAntPro""
                                                FROM ""@SO1_01PROMODESALE"" 
                                                WHERE ""U_SO1_PROMOCION"" = @pPromocion "));

        internal static Consulta ConsultarPorcentajes = new Consulta(Proveedor.SQLServer,
                new ComandoConsulta(Proveedor.SQLServer, @"SELECT Code Codigo, Name Nombre, U_SO1_CODIGPROMOCION Promo, U_SO1_PROMOCIONCADEN PromoCadena,
                                                         U_SO1_NUMEROLINEA NumeroLinea, U_SO1_PORCENTAJE Porcentaje
                                                         FROM [@SO1_01CATPORPDESCA] 
                                                         WHERE U_SO1_CODIGPROMOCION = @pPromocion"),
                new ComandoConsulta(Proveedor.Hana, @"SELECT ""Code"" ""Codigo"", ""Name"" ""Nombre"", ""U_SO1_CODIGPROMOCION"" ""Promo"", ""U_SO1_PROMOCIONCADEN"" ""PromoCadena"",
                                                    ""U_SO1_NUMEROLINEA"" ""NumeroLinea"", ""U_SO1_PORCENTAJE"" ""Porcentaje""
                                                    FROM ""@SO1_01CATPORPDESCA"" 
                                                    WHERE ""U_SO1_CODIGPROMOCION"" = @pPromocion"));

        internal static Consulta ModificarPromocionDescuentoAleatorio = new Consulta(Proveedor.SQLServer,
                new ComandoConsulta(Proveedor.SQLServer, @"UPDATE [@SO1_01PROMODESALE] SET [U_SO1_ACTMONTOMAXIMO] = @pActMontoMaximo, [U_SO1_MONTOMAXIPROMO] = @pMontoMaxiPromo, 
                                                         [U_SO1_ACTMONTOMAXDIA] = @pActMontoMaxDia, [U_SO1_MONTOMAXPORDIA] = @pMontoMaxDia, [U_SO1_ACTIMPVENDESDE] = @pActImpVenDesde,
                                                         [U_SO1_IMPORTVENDESDE] = @pImportVenDesde, [U_SO1_ACTIMPVENHASTA] = @pActImpVenHasta, [U_SO1_IMPORTVENHASTA] = @pImportVenHasta,
                                                         [U_SO1_ACTIMPORTEIMP] = @pActImporteImp, [U_SO1_COMPOTRASPROM] = @pCompoTrasProm, [U_SO1_ACTFRECUENPROM] = @pActFrecuenProm,
                                                         [U_SO1_MINFRECPROMO] = @pMinFrecPromo
                                                         WHERE [Code] = @pCode"),
                new ComandoConsulta(Proveedor.Hana, @"UPDATE ""@SO1_01PROMODESALE"" SET ""U_SO1_ACTMONTOMAXIMO"" = @pActMontoMaximo, ""U_SO1_MONTOMAXIPROMO"" = @pMontoMaxiPromo, 
                                                    ""U_SO1_ACTMONTOMAXDIA"" = @pActMontoMaxDia, ""U_SO1_MONTOMAXPORDIA"" = @pMontoMaxDia, ""U_SO1_ACTIMPVENDESDE"" = @pActImpVenDesde,
                                                    ""U_SO1_IMPORTVENDESDE"" = @pImportVenDesde, ""U_SO1_ACTIMPVENHASTA"" = @pActImpVenHasta, ""U_SO1_IMPORTVENHASTA"" = @pImportVenHasta,
                                                    ""U_SO1_ACTIMPORTEIMP"" = @pActImporteImp, ""U_SO1_COMPOTRASPROM"" = @pCompoTrasProm, ""U_SO1_ACTFRECUENPROM"" = @pActFrecuenProm,
                                                    ""U_SO1_MINFRECPROMO"" = @pMinFrecPromo
                                                    WHERE ""Code"" = @pCode"));

        internal static Consulta RegistrarPromocionDescuentoAleatorio = new Consulta(Proveedor.SQLServer,
            new ComandoConsulta(Proveedor.SQLServer, @"INSERT INTO [@SO1_01PROMODESALE] ([Code], [Name], [U_SO1_PROMOCION], [U_SO1_PROMOCADENA], [U_SO1_ACTMONTOMAXIMO],
                                                     [U_SO1_MONTOMAXIPROMO], [U_SO1_ACTMONTOMAXDIA], [U_SO1_MONTOMAXPORDIA], [U_SO1_ACTIMPVENDESDE], [U_SO1_IMPORTVENDESDE],
                                                     [U_SO1_ACTIMPVENHASTA], [U_SO1_IMPORTVENHASTA], [U_SO1_ACTIMPORTEIMP], [U_SO1_COMPOTRASPROM], [U_SO1_ACTFRECUENPROM],
                                                     [U_SO1_MINFRECPROMO]) 
                                                     VALUES (@pCodigo, @pNombre, @pPromo, @pPromoCadena, @pActMontoMaximo, @pMontoMaxiPromo, @pActMontoMaxDia, @pMontoMaxDia,
                                                     @pActImpVenDesde, @pImportVenDesde, @pActImpVenHasta, @pImportVenHasta, @pActImporteImp, @pCompoTrasProm, @pActFrecuenProm,
                                                     @pMinFrecPromo )"),
            new ComandoConsulta(Proveedor.Hana, @"INSERT INTO ""@SO1_01PROMODESALE"" (""Code"", ""Name"", ""U_SO1_PROMOCION"", ""U_SO1_PROMOCADENA"", ""U_SO1_ACTMONTOMAXIMO"",
                                                ""U_SO1_MONTOMAXIPROMO"", ""U_SO1_ACTMONTOMAXDIA"", ""U_SO1_MONTOMAXPORDIA"", ""U_SO1_ACTIMPVENDESDE"", ""U_SO1_IMPORTVENDESDE"",
                                                ""U_SO1_ACTIMPVENHASTA"", ""U_SO1_IMPORTVENHASTA"", ""U_SO1_ACTIMPORTEIMP"", ""U_SO1_COMPOTRASPROM"", ""U_SO1_ACTFRECUENPROM"",
                                                ""U_SO1_MINFRECPROMO"") 
                                                VALUES (@pCodigo, @pNombre, @pPromo, @pPromoCadena, @pActMontoMaximo, @pMontoMaxiPromo, @pActMontoMaxDia, @pMontoMaxDia,
                                                @pActImpVenDesde, @pImportVenDesde, @pActImpVenHasta, @pImportVenHasta, @pActImporteImp, @pCompoTrasProm, @pActFrecuenProm,
                                                @pMinFrecPromo )"));

        internal static Consulta ModificarCatalogoPorcentaje = new Consulta(Proveedor.SQLServer,
            new ComandoConsulta(Proveedor.SQLServer, @"UPDATE [@SO1_01CATPORPDESCA] SET [U_SO1_NUMEROLINEA] = @pNumeroLinea, [U_SO1_PORCENTAJE] = @pPorcentaje 
                                                     WHERE [Code] = @pCode "),
            new ComandoConsulta(Proveedor.Hana, @"UPDATE ""@SO1_01CATPORPDESCA"" SET ""U_SO1_NUMEROLINEA"" = @pNumeroLinea, ""U_SO1_PORCENTAJE"" = @pPorcentaje 
                                                     WHERE ""Code"" = @pCode "));

        internal static Consulta RegistrarCatalogoPorcentaje = new Consulta(Proveedor.SQLServer,
            new ComandoConsulta(Proveedor.SQLServer, @"INSERT INTO [@SO1_01CATPORPDESCA] ([Code], [Name], [U_SO1_CODIGPROMOCION], [U_SO1_PROMOCIONCADEN],
                                                     [U_SO1_NUMEROLINEA], [U_SO1_PORCENTAJE])
                                                     VALUES (@pCodigo, @pNombre, @pPromo, @pPromoCadena, @pNumeroLinea, @pPorcentaje)"),
            new ComandoConsulta(Proveedor.Hana, @"INSERT INTO ""@SO1_01CATPORPDESCA"" (""Code"", ""Name"", ""U_SO1_CODIGPROMOCION"", ""U_SO1_PROMOCIONCADEN"",
                                                ""U_SO1_NUMEROLINEA"", ""U_SO1_PORCENTAJE"")
                                                VALUES (@pCodigo, @pNombre, @pPromo, @pPromoCadena, @pNumeroLinea, @pPorcentaje)"));

        internal static Consulta ConsultarCodigosCatDescuentos = new Consulta(Proveedor.SQLServer,
            new ComandoConsulta(Proveedor.SQLServer, @"SELECT Code Codigo FROM [@SO1_01CATPORPDESCA] WHERE U_SO1_CODIGPROMOCION = @pPromocion"),
            new ComandoConsulta(Proveedor.Hana, @"SELECT ""Code"" ""Codigo"" FROM ""@SO1_01CATPORPDESCA"" WHERE ""U_SO1_CODIGPROMOCION"" = @pPromocion"));

        #endregion

        #region Cupones

        internal static Consulta ConsultarPromocionCupon = new Consulta(Proveedor.SQLServer,
        new ComandoConsulta(Proveedor.SQLServer, @"SELECT Code Codigo, Name Nombre, U_SO1_CODIGPROMOCION Promo, U_SO1_PROMOCADENA PromoCadena,
                                                     U_SO1_TIPO Tipo, U_SO1_MONTO Monto, U_SO1_CANTIDAD Cantidad, U_SO1_NUMEROCUPONES NumeroCupones, 
                                                     U_SO1_ACTIMPORTEIMP ActImporte, U_SO1_COMPOTRASPROM CompoTras, U_SO1_APLICPROMIGUAL AplicaProm, 
                                                     U_SO1_TIPOCOMPORTAMI TipoComportamiento
                                                     FROM [@SO1_01CUPONSORTEO] 
                                                     WHERE U_SO1_CODIGPROMOCION = @pPromocion"),
        new ComandoConsulta(Proveedor.Hana, @"SELECT ""Code"" ""Codigo"", ""Name"" ""Nombre"", ""U_SO1_CODIGPROMOCION"" ""Promo"", ""U_SO1_PROMOCADENA"" ""PromoCadena"",
                                                ""U_SO1_TIPO"" ""Tipo"", ""U_SO1_MONTO"" ""Monto"", ""U_SO1_CANTIDAD"" ""Cantidad"", ""U_SO1_NUMEROCUPONES"" ""NumeroCupones"", 
                                                ""U_SO1_ACTIMPORTEIMP"" ""ActImporte"", ""U_SO1_COMPOTRASPROM"" ""CompoTras"", ""U_SO1_APLICPROMIGUAL"" ""AplicaProm"", 
                                                ""U_SO1_TIPOCOMPORTAMI"" ""TipoComportamiento""
                                                FROM ""@SO1_01CUPONSORTEO"" 
                                                WHERE ""U_SO1_CODIGPROMOCION"" = @pPromocion"));

        internal static Consulta RegistrarCupones = new Consulta(Proveedor.SQLServer,
        new ComandoConsulta(Proveedor.SQLServer, @"INSERT INTO [@SO1_01CUPONSORTEO] ([Code], [Name], [U_SO1_CODIGPROMOCION], [U_SO1_PROMOCADENA], [U_SO1_TIPO], 
                                                     [U_SO1_MONTO], [U_SO1_CANTIDAD], [U_SO1_NUMEROCUPONES], [U_SO1_ACTIMPORTEIMP], [U_SO1_COMPOTRASPROM], [U_SO1_APLICPROMIGUAL], 
                                                     [U_SO1_TIPOCOMPORTAMI]) 
                                                     VALUES (@pCodigo, @pNombre, @pPromo, @pPromoCadena, @pTipo, @pMonto, @pCantidad, @pNumeroCupones, @pActImporte, @pCompoTras, 
                                                     @pAplicaProm, @pTipoComportamiento)"),
        new ComandoConsulta(Proveedor.Hana, @"INSERT INTO ""@SO1_01CUPONSORTEO"" (""Code"", ""Name"", ""U_SO1_CODIGPROMOCION"", ""U_SO1_PROMOCADENA"", ""U_SO1_TIPO"", 
                                                ""U_SO1_MONTO"", ""U_SO1_CANTIDAD"", ""U_SO1_NUMEROCUPONES"", ""U_SO1_ACTIMPORTEIMP"", ""U_SO1_COMPOTRASPROM"", ""U_SO1_APLICPROMIGUAL"", 
                                                ""U_SO1_TIPOCOMPORTAMI"") 
                                                VALUES (@pCodigo, @pNombre, @pPromo, @pPromoCadena, @pTipo, @pMonto, @pCantidad, @pNumeroCupones, @pActImporte, @pCompoTras, 
                                                @pAplicaProm, @pTipoComportamiento)"));

        internal static Consulta ModificarCupones = new Consulta(Proveedor.SQLServer,
        new ComandoConsulta(Proveedor.SQLServer, @"UPDATE [@SO1_01CUPONSORTEO] SET [U_SO1_TIPO] = @pTipo, [U_SO1_MONTO] = @pMonto, [U_SO1_CANTIDAD] = @pCantidad, 
                                                     [U_SO1_NUMEROCUPONES] = @pNumeroCupones, [U_SO1_ACTIMPORTEIMP] = @pActImporte, [U_SO1_COMPOTRASPROM] = @pCompoTras, 
                                                     [U_SO1_APLICPROMIGUAL] = @pAplicaProm, [U_SO1_TIPOCOMPORTAMI] = @pTipoComportamiento
                                                     WHERE [Code] = @pCode "),
        new ComandoConsulta(Proveedor.Hana, @"UPDATE ""@SO1_01CUPONSORTEO"" SET ""U_SO1_TIPO"" = @pTipo, ""U_SO1_MONTO"" = @pMonto, ""U_SO1_CANTIDAD"" = @pCantidad, 
                                                ""U_SO1_NUMEROCUPONES"" = @pNumeroCupones, ""U_SO1_ACTIMPORTEIMP"" = @pActImporte, ""U_SO1_COMPOTRASPROM"" = @pCompoTras, 
                                                ""U_SO1_APLICPROMIGUAL"" = @pAplicaProm, ""U_SO1_TIPOCOMPORTAMI"" = @pTipoComportamiento
                                                WHERE ""Code"" = @pCode"));

        #endregion

        #region Precio Único

        internal static Consulta ConsultarPromocionPrecioUnico = new Consulta(Proveedor.SQLServer,
            new ComandoConsulta(Proveedor.SQLServer, @"SELECT Code, Name, U_SO1_PROMOCION, U_SO1_PROMOCADENA, U_SO1_MONEDA, 
                                                     U_SO1_MONTO, U_SO1_CONIMPUESTO, U_SO1_MANTENERDESC
                                                     FROM [@SO1_01PROMOPREUNI] 
                                                     WHERE U_SO1_PROMOCION = @pPromocion"),
            new ComandoConsulta(Proveedor.Hana, @"SELECT ""Code"", ""Name"", ""U_SO1_PROMOCION"", ""U_SO1_PROMOCADENA"", ""U_SO1_MONEDA"", 
                                                ""U_SO1_MONTO"", ""U_SO1_CONIMPUESTO"", ""U_SO1_MANTENERDESC""
                                                FROM ""@SO1_01PROMOPREUNI"" 
                                                WHERE ""U_SO1_PROMOCION"" = @pPromocion"));

        internal static Consulta ModificarPromocionPrecioUnico = new Consulta(Proveedor.SQLServer,
            new ComandoConsulta(Proveedor.SQLServer, @"UPDATE [@SO1_01PROMOPREUNI] SET [Name] = @pNombre, [U_SO1_MONEDA] = @pMoneda, [U_SO1_MONTO] = @pMonto,
                                                     [U_SO1_CONIMPUESTO] = @pConImpuesto, [U_SO1_MANTENERDESC] = @pMantenerDesc
                                                     WHERE [Code] = @pCode"),
            new ComandoConsulta(Proveedor.Hana, @"UPDATE ""@SO1_01PROMOPREUNI"" SET ""Name"" = @pNombre,""U_SO1_MONEDA"" = @pMoneda, ""U_SO1_MONTO"" = @pMonto,
                                                ""U_SO1_CONIMPUESTO"" = @pConImpuesto, ""U_SO1_MANTENERDESC"" = @pMantenerDesc
                                                WHERE ""Code"" = @pCode"));

        internal static Consulta RegistrarPromocionPrecioUnico = new Consulta(Proveedor.SQLServer,
            new ComandoConsulta(Proveedor.SQLServer, @"INSERT INTO [@SO1_01PROMOPREUNI] ([Code], [Name], [U_SO1_PROMOCION], [U_SO1_PROMOCADENA], [U_SO1_MONEDA],
                                                     [U_SO1_MONTO], [U_SO1_CONIMPUESTO], [U_SO1_MANTENERDESC]) 
                                                     VALUES (@pCodigo, @pNombre, @pPromo, @pPromoCadena, @pMoneda, @pMonto, @pConImpuesto, @pMantenerDesc)"),
            new ComandoConsulta(Proveedor.Hana, @"INSERT INTO ""@SO1_01PROMOPREUNI"" (""Code"", ""Name"", ""U_SO1_PROMOCION"", ""U_SO1_PROMOCADENA"", ""U_SO1_MONEDA"",
                                                ""U_SO1_MONTO"", ""U_SO1_CONIMPUESTO"", ""U_SO1_MANTENERDESC"") 
                                                VALUES (@pCodigo, @pNombre, @pPromo, @pPromoCadena, @pMoneda, @pMonto, @pConImpuesto, @pMantenerDesc)"));
        #endregion

        #region Vale Descuento

        internal static Consulta ModificarPromocionValeDescuento = new Consulta(Proveedor.SQLServer,
            new ComandoConsulta(Proveedor.SQLServer, @"update [@SO1_01PROMOVALDES] 
                                                        set [Name] = @pNombre, [U_SO1_COMPORTAMIENTO] = @pComportamiento, [U_SO1_MONEDA] = @pMoneda, 
                                                        [U_SO1_MONTOMINIMO] = @pMontoMinimo, [U_SO1_TIPODESCUENTO] = @pTipoDescuento, 
                                                        [U_SO1_IMPORTEDESC] = @pImporteDescuento, [U_SO1_FECHAVENCI] = @pFechaVencimiento, 
                                                        [U_SO1_DIASVENCI] = @pDiasVencimiento, [U_SO1_POLITICAREDEN] = @pPoliticaReden
                                                        where [Code] = @pCode"),
            new ComandoConsulta(Proveedor.Hana, @"UPDATE ""@SO1_01PROMOVALDES"" 
                                                SET ""Name"" = @pNombre, ""U_SO1_COMPORTAMIENTO"" = @pComportamiento, ""U_SO1_MONEDA"" = @pMoneda, 
                                                ""U_SO1_MONTOMINIMO"" = @pMontoMinimo, ""U_SO1_TIPODESCUENTO"" = @pTipoDescuento, 
                                                ""U_SO1_IMPORTEDESC"" = @pImporteDescuento, ""U_SO1_FECHAVENCI"" = @pFechaVencimiento, 
                                                ""U_SO1_DIASVENCI"" = @pDiasVencimiento, ""U_SO1_POLITICAREDEN"" = @pPoliticaReden
                                                WHERE ""Code"" = @pCode"));

        internal static Consulta ConsultarPromocionValeDescuento = new Consulta(Proveedor.SQLServer,
            new ComandoConsulta(Proveedor.SQLServer, @"select Code, Name, U_SO1_PROMOCION, U_SO1_PROMOCADENA, 
                                                        U_SO1_COMPORTAMIENTO, U_SO1_MONEDA, U_SO1_MONTOMINIMO,
                                                        U_SO1_TIPODESCUENTO, U_SO1_IMPORTEDESC, U_SO1_FECHAVENCI,
                                                        U_SO1_DIASVENCI, U_SO1_POLITICAREDEN
                                                        from [@SO1_01PROMOVALDES] where U_SO1_PROMOCION = @pPromocion"),
            new ComandoConsulta(Proveedor.Hana, @"SELECT ""Code"", ""Name"", ""U_SO1_PROMOCION"", ""U_SO1_PROMOCADENA"", 
                                                ""U_SO1_COMPORTAMIENTO"", ""U_SO1_MONEDA"", ""U_SO1_MONTOMINIMO"",
                                                ""U_SO1_TIPODESCUENTO"", ""U_SO1_IMPORTEDESC"", ""U_SO1_FECHAVENCI"",
                                                ""U_SO1_DIASVENCI"", ""U_SO1_POLITICAREDEN""
                                                FROM ""@SO1_01PROMOVALDES"" WHERE ""U_SO1_PROMOCION"" = @pPromocion"));

        internal static Consulta RegistrarPromocionValeDescuento = new Consulta(Proveedor.SQLServer,
            new ComandoConsulta(Proveedor.SQLServer, @"INSERT INTO [@SO1_01PROMOVALDES] (Code, Name, U_SO1_PROMOCION, U_SO1_PROMOCADENA, 
                                                        U_SO1_COMPORTAMIENTO, U_SO1_MONEDA, U_SO1_MONTOMINIMO,
                                                        U_SO1_TIPODESCUENTO, U_SO1_IMPORTEDESC, U_SO1_FECHAVENCI,
                                                        U_SO1_DIASVENCI, U_SO1_POLITICAREDEN)
                                                        VALUES (@pCodigo, @pNombre, @pPromo, @pPromoCadena, @pComportamiento, @pMoneda, @pMontoMinimo, 
                                                        @pTipoDescuento, @pImporteDescuento, @pFechaVencimiento, @pDiasVencimiento, @pPoliticaReden)"),
            new ComandoConsulta(Proveedor.Hana, @"INSERT INTO ""@SO1_01PROMOVALDES"" (""Code"", ""Name"", ""U_SO1_PROMOCION"", ""U_SO1_PROMOCADENA"", 
                                                ""U_SO1_COMPORTAMIENTO"", ""U_SO1_MONEDA"", ""U_SO1_MONTOMINIMO"",
                                                ""U_SO1_TIPODESCUENTO"", ""U_SO1_IMPORTEDESC"", ""U_SO1_FECHAVENCI"",
                                                ""U_SO1_DIASVENCI"", ""U_SO1_POLITICAREDEN"")
                                                VALUES (@pCodigo, @pNombre, @pPromo, @pPromoCadena, @pComportamiento, @pMoneda, @pMontoMinimo, 
                                                @pTipoDescuento, @pImporteDescuento, @pFechaVencimiento, @pDiasVencimiento, @pPoliticaReden)"));

        #endregion

        #region Politica Redención

        internal static Consulta ConsultarPromocionPoliticaRedencion = new Consulta(Proveedor.SQLServer,
                new ComandoConsulta(Proveedor.SQLServer, @"SELECT Code, Name, U_SO1_POLITICAREDEN, U_SO1_TIPO, 
                                                    U_SO1_DIARIO, U_SO1_DIACOMPLETO, U_SO1_DIAHORAINI, U_SO1_DIAHORAFIN, 
                                                    U_SO1_LUNES, U_SO1_LUNCOMPLETO, U_SO1_LUNHORAINI, U_SO1_LUNHORAFIN,
                                                    U_SO1_MARTES, U_SO1_MARCOMPLETO, U_SO1_MARHORAINI, U_SO1_MARHORAFIN,
                                                    U_SO1_MIERCOLES, U_SO1_MIECOMPLETO, U_SO1_MIEHORAINI, U_SO1_MIEHORAFIN,
                                                    U_SO1_JUEVES, U_SO1_JUECOMPLETO, U_SO1_JUEHORAINI, U_SO1_JUEHORAFIN,
                                                    U_SO1_VIERNES, U_SO1_VIECOMPLETO, U_SO1_VIEHORAINI, U_SO1_VIEHORAFIN,
                                                    U_SO1_SABADO, U_SO1_SABCOMPLETO, U_SO1_SABHORAINI, U_SO1_SABHORAFIN,
                                                    U_SO1_DOMINGO, U_SO1_DOMCOMPLETO, U_SO1_DOMHORAINI, U_SO1_DOMHORAFIN,
                                                    U_SO1_ARTICULOENLACE, U_SO1_FILTRARARTART, U_SO1_FILTRARARTGRU, U_SO1_FILTRARARTPRO,
                                                    U_SO1_FILTRARARTPROE, U_SO1_FILTRARARTPROC, U_SO1_FILTRARARTCOE, U_SO1_FILTRARARTCOEC, U_SO1_FILTRARARTCOEV,
                                                    U_SO1_FILTRARARTPROV, U_SO1_FILTRARARTFAB, U_SO1_FILTRARARTEXC, U_SO1_FILTRARARTUNE, 
                                                    U_SO1_FILTRARPROMSUC, U_SO1_FILTRARPROMALI, U_SO1_FILTRARPROMCLI, U_SO1_FILTRARCLICLI,
                                                    U_SO1_FILTRARCLIENLS, U_SO1_FILTRARCLICOEC, U_SO1_FILTRARCLIGRU, U_SO1_FILTRARCLIPRO,
                                                    U_SO1_FILTRARCLIPROE, U_SO1_FILTRARCLIPROC, U_SO1_FILTRARCLICOE, U_SO1_FILTRARCLICOEV,
                                                    U_SO1_FILTRARPROMMEM, U_SO1_FILTRARPROMLIS, U_SO1_FILTRARPROMFOP, U_SO1_ACUMPROMOOTRAS
                                                    FROM [@SO1_01POLITICAREDEN] WHERE Code = @pPromocion"),
        new ComandoConsulta(Proveedor.Hana, @"SELECT ""Code"", ""Name"", ""U_SO1_POLITICAREDEN"", ""U_SO1_TIPO"", 
                                            ""U_SO1_DIARIO"", ""U_SO1_DIACOMPLETO"", ""U_SO1_DIAHORAINI"", ""U_SO1_DIAHORAFIN"", 
                                            ""U_SO1_LUNES"", ""U_SO1_LUNCOMPLETO"", ""U_SO1_LUNHORAINI"", ""U_SO1_LUNHORAFIN"",
                                            ""U_SO1_MARTES"", ""U_SO1_MARCOMPLETO"", ""U_SO1_MARHORAINI"", ""U_SO1_MARHORAFIN"",
                                            ""U_SO1_MIERCOLES"", ""U_SO1_MIECOMPLETO"", ""U_SO1_MIEHORAINI"", ""U_SO1_MIEHORAFIN"",
                                            ""U_SO1_JUEVES"", ""U_SO1_JUECOMPLETO"", ""U_SO1_JUEHORAINI"", ""U_SO1_JUEHORAFIN"",
                                            ""U_SO1_VIERNES"", ""U_SO1_VIECOMPLETO"", ""U_SO1_VIEHORAINI"", ""U_SO1_VIEHORAFIN"",
                                            ""U_SO1_SABADO"", ""U_SO1_SABCOMPLETO"", ""U_SO1_SABHORAINI"", ""U_SO1_SABHORAFIN"",
                                            ""U_SO1_DOMINGO"", ""U_SO1_DOMCOMPLETO"", ""U_SO1_DOMHORAINI"", ""U_SO1_DOMHORAFIN"",
                                            ""U_SO1_ARTICULOENLACE"", ""U_SO1_FILTRARARTART"", ""U_SO1_FILTRARARTGRU"", ""U_SO1_FILTRARARTPRO"",
                                            ""U_SO1_FILTRARARTPROE"", ""U_SO1_FILTRARARTPROC"", ""U_SO1_FILTRARARTCOE"", ""U_SO1_FILTRARARTCOEC"", ""U_SO1_FILTRARARTCOEV"",
                                            ""U_SO1_FILTRARARTPROV"", ""U_SO1_FILTRARARTFAB"", ""U_SO1_FILTRARARTEXC"", ""U_SO1_FILTRARARTUNE"", 
                                            ""U_SO1_FILTRARPROMSUC"", ""U_SO1_FILTRARPROMALI"", ""U_SO1_FILTRARPROMCLI"", ""U_SO1_FILTRARCLICLI"",
                                            ""U_SO1_FILTRARCLIENLS"", ""U_SO1_FILTRARCLICOEC"", ""U_SO1_FILTRARCLIGRU"", ""U_SO1_FILTRARCLIPRO"",
                                            ""U_SO1_FILTRARCLIPROE"", ""U_SO1_FILTRARCLIPROC"", ""U_SO1_FILTRARCLICOE"", ""U_SO1_FILTRARCLICOEV"",
                                            ""U_SO1_FILTRARPROMMEM"", ""U_SO1_FILTRARPROMLIS"", ""U_SO1_FILTRARPROMFOP"", ""U_SO1_ACUMPROMOOTRAS""
                                            FROM ""@SO1_01POLITICAREDEN"" WHERE ""Code"" = @pPromocion"));

        internal static Consulta RegistrarPromocionPoliticaRedencion = new Consulta(Proveedor.SQLServer,
                  new ComandoConsulta(Proveedor.SQLServer, @"INSERT INTO [@SO1_01POLITICAREDEN] (Code, Name, U_SO1_POLITICAREDEN, U_SO1_TIPO, 
                                                            U_SO1_DIARIO, U_SO1_DIACOMPLETO, U_SO1_DIAHORAINI, U_SO1_DIAHORAFIN, 
                                                            U_SO1_LUNES, U_SO1_LUNCOMPLETO, U_SO1_LUNHORAINI, U_SO1_LUNHORAFIN,
                                                            U_SO1_MARTES, U_SO1_MARCOMPLETO, U_SO1_MARHORAINI, U_SO1_MARHORAFIN,
                                                            U_SO1_MIERCOLES, U_SO1_MIECOMPLETO, U_SO1_MIEHORAINI, U_SO1_MIEHORAFIN,
                                                            U_SO1_JUEVES, U_SO1_JUECOMPLETO, U_SO1_JUEHORAINI, U_SO1_JUEHORAFIN,
                                                            U_SO1_VIERNES, U_SO1_VIECOMPLETO, U_SO1_VIEHORAINI, U_SO1_VIEHORAFIN,
                                                            U_SO1_SABADO, U_SO1_SABCOMPLETO, U_SO1_SABHORAINI, U_SO1_SABHORAFIN,
                                                            U_SO1_DOMINGO, U_SO1_DOMCOMPLETO, U_SO1_DOMHORAINI, U_SO1_DOMHORAFIN,
                                                            U_SO1_ARTICULOENLACE, U_SO1_FILTRARARTART, U_SO1_FILTRARARTGRU, 
                                                            U_SO1_FILTRARARTPRO, U_SO1_FILTRARARTPROE, U_SO1_FILTRARARTPROC, 
                                                            U_SO1_FILTRARARTCOE, U_SO1_FILTRARARTCOEC, U_SO1_FILTRARARTCOEV,
                                                            U_SO1_FILTRARARTPROV, U_SO1_FILTRARARTFAB, U_SO1_FILTRARARTEXC, 
                                                            U_SO1_FILTRARARTUNE, U_SO1_FILTRARPROMSUC, U_SO1_FILTRARPROMALI, 
                                                            U_SO1_FILTRARPROMCLI, U_SO1_FILTRARCLICLI, U_SO1_FILTRARCLIENLS, 
                                                            U_SO1_FILTRARCLIGRU, U_SO1_FILTRARCLIPRO,U_SO1_FILTRARCLIPROE, 
                                                            U_SO1_FILTRARCLIPROC, U_SO1_FILTRARCLICOE, U_SO1_FILTRARCLICOEC,
                                                            U_SO1_FILTRARCLICOEV, U_SO1_FILTRARPROMMEM, U_SO1_FILTRARPROMLIS, 
                                                            U_SO1_FILTRARPROMFOP, U_SO1_ACUMPROMOOTRAS)
                                                            VALUES (@pCode, @pNombre, @pPolitRed, @pTipo, @pDiario, 
                                                            @pDiaCompleto, @pDiaHoraIni, @pDiaHoraFin, @pLunes, @pLunesCompleto,
                                                            @pLunHoraIni, @pLunHoraFin, @pMartes, @pMartesCompleto, @pMarHoraIni, 
                                                            @pMarHoraFin, @pMiercoles, @pMieCompleto,  @pMieHoraIni,
                                                            @pMieHoraFin, @pJueves, @pJueCompleto, @pJueHoraIni, @pJueHoraFin, 
                                                            @pViernes, @pVieCompleto, @pVieHoraIni, @pVieHoraFin,
                                                            @pSabado, @pSabCompleto, @pSabHoraIni, @pSabHoraFin, @pDomingo, 
                                                            @pDomCompleto, @pDomHoraIni, @pDomHoraFin,@pArticuloEnlace,
                                                            @pFiltrarArtArt, @pFiltrarArtGru, @pFiltrarArtPro, @pFiltrarArtProe,  
                                                            @pFiltrarArtProc,  @pFiltrarArtCoe, @pFiltrarArtCoec,
                                                            @pFiltrarArtCoev, @pFiltrarArtProv, @pFiltrarArtFab, @pFiltrarArtExc, 
                                                            @pFiltrarArtUne, @pFiltrarPromSuc, @pFiltrarPromAli,
                                                            @pFiltrarPromCli, @pFiltrarCliCli,  @pFiltrarCliEnls, 
                                                            @pFiltrarCliGru, @pFiltrarCliPro, @pFiltrarCliProe, @pFiltrarCliProc,
                                                            @pFiltrarCliCoe,  @pFiltrarCliCoec, @pFiltrarCliCoev, @pFiltrarPromMem, 
                                                            @pFiltrarPromLis, 
                                                            @pFiltrarPromFop,
                                                            @pAcumPromoOtras)"),
                  new ComandoConsulta(Proveedor.Hana, @"INSERT INTO ""@SO1_01POLITICAREDEN"" (""Code"", ""Name"", ""U_SO1_POLITICAREDEN"", ""U_SO1_TIPO"", 
                                                      ""U_SO1_DIARIO"", ""U_SO1_DIACOMPLETO"", ""U_SO1_DIAHORAINI"", ""U_SO1_DIAHORAFIN"", 
                                                      ""U_SO1_LUNES"", ""U_SO1_LUNCOMPLETO"", ""U_SO1_LUNHORAINI"", ""U_SO1_LUNHORAFIN"",
                                                      ""U_SO1_MARTES"", ""U_SO1_MARCOMPLETO"", ""U_SO1_MARHORAINI"", ""U_SO1_MARHORAFIN"",
                                                      ""U_SO1_MIERCOLES"", ""U_SO1_MIECOMPLETO"", ""U_SO1_MIEHORAINI"", ""U_SO1_MIEHORAFIN"",
                                                      ""U_SO1_JUEVES"", ""U_SO1_JUECOMPLETO"", ""U_SO1_JUEHORAINI"", ""U_SO1_JUEHORAFIN"",
                                                      ""U_SO1_VIERNES"", ""U_SO1_VIECOMPLETO"", ""U_SO1_VIEHORAINI"", ""U_SO1_VIEHORAFIN"",
                                                      ""U_SO1_SABADO"", ""U_SO1_SABCOMPLETO"", ""U_SO1_SABHORAINI"", ""U_SO1_SABHORAFIN"",
                                                      ""U_SO1_DOMINGO"", ""U_SO1_DOMCOMPLETO"", ""U_SO1_DOMHORAINI"", ""U_SO1_DOMHORAFIN"",
                                                      ""U_SO1_ARTICULOENLACE"", ""U_SO1_FILTRARARTART"", ""U_SO1_FILTRARARTGRU"", ""U_SO1_FILTRARARTPRO"",
                                                      ""U_SO1_FILTRARARTPROE"", ""U_SO1_FILTRARARTPROC"", ""U_SO1_FILTRARARTCOE"", ""U_SO1_FILTRARARTCOEC"", ""U_SO1_FILTRARARTCOEV"",
                                                      ""U_SO1_FILTRARARTPROV"", ""U_SO1_FILTRARARTFAB"", ""U_SO1_FILTRARARTEXC"", ""U_SO1_FILTRARARTUNE"", 
                                                      ""U_SO1_FILTRARPROMSUC"", ""U_SO1_FILTRARPROMALI"", ""U_SO1_FILTRARPROMCLI"", ""U_SO1_FILTRARCLICLI"",
                                                      ""U_SO1_FILTRARCLIENLS"", ""U_SO1_FILTRARCLICOEC"", ""U_SO1_FILTRARCLIGRU"", ""U_SO1_FILTRARCLIPRO"",
                                                      ""U_SO1_FILTRARCLIPROE"", ""U_SO1_FILTRARCLIPROC"", ""U_SO1_FILTRARCLICOE"", ""U_SO1_FILTRARCLICOEV"",
                                                      ""U_SO1_FILTRARPROMMEM"", ""U_SO1_FILTRARPROMLIS"", ""U_SO1_FILTRARPROMFOP"", ""U_SO1_ACUMPROMOOTRAS"") 
                                                      VALUES (@pCodigo, @pNombre, @pPolitRed, @pTipo, @pDiario, @pDiaCompleto, @pDiaHoraIni, @pDiaHoraFin, @pLunes, @pLunesCompleto,
                                                      @pLunHoraIni, @pLunHoraFin, @pMartes, @pMartesCompleto, @pMarHoraIni, @pMarHoraFin, @pMiercoles, @pMieCompleto,  @pMieHoraIni,
                                                      @pMieHoraFin, @pJueves, @pJueCompleto, @pJueHoraIni, @pJueHoraFin, @pViernes, @pVieCompleto, @pVieHoraIni, @pVieHoraFin,
                                                      @pSabado, @pSabCompleto, @pSabHoraIni, @pSabHoraFin, @pDomingo, @pDomCompleto, @pDomHoraIni, @pDomHoraFin,@pArticuloEnlace,
                                                      @pFiltrarArtArt, @pFiltrarArtGru, @pFiltrarArtPro, @pFiltrarArtProe,  @pFiltrarArtProc,  @pFiltrarArtCoe, @pFiltrarArtCoec,
                                                      @pFiltrarArtCoev, @pFiltrarArtProv, @pFiltrarArtFab, @pFiltrarArtExc, @pFiltrarArtUne, @pFiltrarPromSuc, @pFiltrarPromAli,
                                                      @pFiltrarPromCli, @pFiltrarCliCli,  @pFiltrarCliEnls, @pFiltrarCliCoec, @pFiltrarCliGru, @pFiltrarCliPro, @pFiltrarCliProe, @pFiltrarCliProc,
                                                      @pFiltrarCliCoe,  @pFiltrarCliCoev, @pFiltrarPromMem, @pFiltrarPromLis, @pFiltrarPromFop,
                                                      @pAcumPromoOtras)"));

        internal static Consulta ModicarPoliticaRedencion = new Consulta(Proveedor.SQLServer,
               new ComandoConsulta(Proveedor.SQLServer, @"UPDATE [@SO1_01POLITICAREDEN] SET [Name] = @pNombre, [U_SO1_TIPO] = @pTipo, [U_SO1_DIARIO] = @pDiario, [U_SO1_DIACOMPLETO] =     @pDiaCompleto,[U_SO1_DIAHORAINI] = @pDiaHoraIni,
                                        [U_SO1_DIAHORAFIN] = @pDiaHoraFin, [U_SO1_LUNES] = @pLunes, [U_SO1_LUNCOMPLETO] = @pLunesCompleto,
                                        [U_SO1_LUNHORAINI] = @pLunHoraIni, [U_SO1_LUNHORAFIN] = @pLunHoraFin, [U_SO1_MARTES] = @pMartes,
                                        [U_SO1_MARCOMPLETO] = @pMartesCompleto, [U_SO1_MARHORAINI] = @pMarHoraIni, [U_SO1_MARHORAFIN] = @pMarHoraFin,
                                        [U_SO1_MIERCOLES] = @pMiercoles, [U_SO1_MIECOMPLETO] = @pMieCompleto, [U_SO1_MIEHORAINI] = @pMieHoraIni,
                                        [U_SO1_MIEHORAFIN] = @pMieHoraFin, [U_SO1_JUEVES] = @pJueves, [U_SO1_JUECOMPLETO] = @pJueCompleto,
                                        [U_SO1_JUEHORAINI] = @pJueHoraIni, [U_SO1_JUEHORAFIN] = @pJueHoraFin, [U_SO1_VIERNES] = @pViernes,
                                        [U_SO1_VIECOMPLETO] = @pVieCompleto, [U_SO1_VIEHORAINI] = @pVieHoraIni, [U_SO1_VIEHORAFIN] = @pVieHoraFin,
                                        [U_SO1_SABADO] = @pSabado, [U_SO1_SABCOMPLETO] = @pSabCompleto, [U_SO1_SABHORAINI] = @pSabHoraIni,
                                        [U_SO1_SABHORAFIN] = @pSabHoraFin, [U_SO1_DOMINGO] = @pDomingo, [U_SO1_DOMCOMPLETO] = @pDomCompleto,
                                        [U_SO1_DOMHORAINI] = @pDomHoraIni, [U_SO1_DOMHORAFIN] = @pDomHoraFin, [U_SO1_ARTICULOENLACE] = @pArticuloEnlace,
                                        [U_SO1_FILTRARARTART] = @pFiltrarArtArt, [U_SO1_FILTRARARTGRU] = @pFiltrarArtGru, [U_SO1_FILTRARARTPRO] = @pFiltrarArtPro,
                                        [U_SO1_FILTRARARTPROE] = @pFiltrarArtProe, [U_SO1_FILTRARARTPROC] = @pFiltrarArtProc, [U_SO1_FILTRARARTCOE] =           @pFiltrarArtCoe,
                                        [U_SO1_FILTRARARTCOEC] = @pFiltrarArtCoec, [U_SO1_FILTRARARTCOEV] = @pFiltrarArtCoev, [U_SO1_FILTRARARTPROV] = @pFiltrarArtProv,
                                        [U_SO1_FILTRARARTFAB] = @pFiltrarArtFab, [U_SO1_FILTRARARTEXC] = @pFiltrarArtExc, [U_SO1_FILTRARARTUNE] = @pFiltrarArtUne,
                                        [U_SO1_FILTRARPROMSUC] = @pFiltrarPromSuc, [U_SO1_FILTRARPROMALI] = @pFiltrarPromAli, [U_SO1_FILTRARPROMCLI] = @pFiltrarPromCli,
                                        [U_SO1_FILTRARCLICLI] = @pFiltrarCliCli, [U_SO1_FILTRARCLIENLS] = @pFiltrarCliEnls, [U_SO1_FILTRARCLIGRU] = @pFiltrarCliGru,
                                        [U_SO1_FILTRARCLIPRO] = @pFiltrarCliPro, [U_SO1_FILTRARCLIPROE] = @pFiltrarCliProe, [U_SO1_FILTRARCLIPROC] = @pFiltrarCliProc,
                                        [U_SO1_FILTRARCLICOE] = @pFiltrarCliCoe, [U_SO1_FILTRARCLICOEC] = @pFiltrarCliCoec, [U_SO1_FILTRARCLICOEV] = @pFiltrarCliCoev,
                                        [U_SO1_FILTRARPROMMEM] = @pFiltrarPromMem, [U_SO1_FILTRARPROMLIS] = @pFiltrarPromLis, [U_SO1_FILTRARPROMFOP] = @pFiltrarPromFop, [U_SO1_ACUMPROMOOTRAS] = @pAcumPromoOtras                                                     
                                        WHERE [Code] = @pCode"),
                 new ComandoConsulta(Proveedor.Hana, @"UPDATE ""@SO1_01POLITICAREDEN"" SET ""Name"" = @pNombre, ""U_SO1_TIPO"" = @pTipo, ""U_SO1_DIARIO"" = @pDiario, ""U_SO1_DIACOMPLETO"" = @pDiaCompleto, ""U_SO1_DIAHORAINI"" = @pDiaHoraIni,
                                        ""U_SO1_DIAHORAFIN"" = @pDiaHoraFin, ""U_SO1_LUNES"" = @pLunes, ""U_SO1_LUNCOMPLETO"" = @pLunesCompleto,
                                        ""U_SO1_LUNHORAINI"" = @pLunHoraIni, ""U_SO1_LUNHORAFIN"" = @pLunHoraFin, ""U_SO1_MARTES"" = @pMartes,
                                        ""U_SO1_MARCOMPLETO"" = @pMartesCompleto, ""U_SO1_MARHORAINI"" = @pMarHoraIni, ""U_SO1_MARHORAFIN"" = @pMarHoraFin,
                                        ""U_SO1_MIERCOLES"" = @pMiercoles, ""U_SO1_MIECOMPLETO"" = @pMieCompleto, ""U_SO1_MIEHORAINI"" = @pMieHoraIni,
                                        ""U_SO1_MIEHORAFIN"" = @pMieHoraFin, ""U_SO1_JUEVES"" = @pJueves, ""U_SO1_JUECOMPLETO"" = @pJueCompleto,
                                        ""U_SO1_JUEHORAINI"" = @pJueHoraIni, ""U_SO1_JUEHORAFIN"" = @pJueHoraFin, ""U_SO1_VIERNES"" = @pViernes,
                                        ""U_SO1_VIECOMPLETO"" = @pVieCompleto, ""U_SO1_VIEHORAINI"" = @pVieHoraIni, ""U_SO1_VIEHORAFIN"" = @pVieHoraFin,
                                        ""U_SO1_SABADO"" = @pSabado, ""U_SO1_SABCOMPLETO"" = @pSabCompleto, ""U_SO1_SABHORAINI"" = @pSabHoraIni,
                                        ""U_SO1_SABHORAFIN"" = @pSabHoraFin, ""U_SO1_DOMINGO"" = @pDomingo, ""U_SO1_DOMCOMPLETO"" = @pDomCompleto,
                                        ""U_SO1_DOMHORAINI"" = @pDomHoraIni, ""U_SO1_DOMHORAFIN"" = @pDomHoraFin, ""U_SO1_ARTICULOENLACE"" = @pArticuloEnlace,
                                        ""U_SO1_FILTRARARTART"" = @pFiltrarArtArt, ""U_SO1_FILTRARARTGRU"" = @pFiltrarArtGru, ""U_SO1_FILTRARARTPRO"" = @pFiltrarArtPro,
                                        ""U_SO1_FILTRARARTPROE"" = @pFiltrarArtProe, ""U_SO1_FILTRARARTPROC"" = @pFiltrarArtProc, ""U_SO1_FILTRARARTCOE"" = @pFiltrarArtCoe,
                                        ""U_SO1_FILTRARARTCOEC"" = @pFiltrarArtCoec, ""U_SO1_FILTRARARTCOEV"" = @pFiltrarArtCoev, ""U_SO1_FILTRARARTPROV"" = @pFiltrarArtProv,
                                        ""U_SO1_FILTRARARTFAB"" = @pFiltrarArtFab, ""U_SO1_FILTRARARTEXC"" = @pFiltrarArtExc, ""U_SO1_FILTRARARTUNE"" = @pFiltrarArtUne,
                                        ""U_SO1_FILTRARPROMSUC"" = @pFiltrarPromSuc, ""U_SO1_FILTRARPROMALI"" = @pFiltrarPromAli, ""U_SO1_FILTRARPROMCLI"" = @pFiltrarPromCli,
                                        ""U_SO1_FILTRARCLICLI"" = @pFiltrarCliCli, ""U_SO1_FILTRARCLIENLS"" = @pFiltrarCliEnls, ""U_SO1_FILTRARCLIGRU"" = @pFiltrarCliGru,
                                        ""U_SO1_FILTRARCLIPRO"" = @pFiltrarCliPro, ""U_SO1_FILTRARCLIPROE"" = @pFiltrarCliProe, ""U_SO1_FILTRARCLIPROC"" = @pFiltrarCliProc,
                                        ""U_SO1_FILTRARCLICOE"" = @pFiltrarCliCoe, ""U_SO1_FILTRARCLICOEC"" = @pFiltrarCliCoec, ""U_SO1_FILTRARCLICOEV"" = @pFiltrarCliCoev,
                                        ""U_SO1_FILTRARPROMMEM"" = @pFiltrarPromMem, ""U_SO1_FILTRARPROMLIS"" = @pFiltrarPromLis, ""U_SO1_FILTRARPROMFOP"" = @pFiltrarPromFop, ""U_SO1_ACUMPROMOOTRAS"" = @pAcumPromoOtras                                                     
                                        WHERE ""Code"" = @pCodigo"));

        internal static Consulta tipoPromocion = new Consulta(Proveedor.SQLServer,
         new ComandoConsulta(Proveedor.SQLServer, @"SELECT MAX(U_SO1_POLITICAREDEN) tipo FROM [@SO1_01POLITICAREDEN]"),
         new ComandoConsulta(Proveedor.Hana, @"SELECT MAX(""U_SO1_POLITICAREDEN"") ""tipo"" FROM ""@SO1_01POLITICAREDEN"" "));

        #endregion

        #endregion

        #region Consultas comunes

        internal static Consulta RegistrarGrupo = new Consulta(Proveedor.SQLServer,
          new ComandoConsulta(Proveedor.SQLServer, @"INSERT INTO [${TABLA}] ([Code], [Name], [U_SO1_PROMOCION], [U_SO1_PROMOCADENA],
                                                   [U_SO1_GRUPO], [U_SO1_ACTIVO]) 
                                                   VALUES (@pCode, @pName, @pPromo, @pPromoCadena, @pGrupo, @pActivo)"),
          new ComandoConsulta(Proveedor.Hana, @"INSERT INTO ""${TABLA}"" (""Code"", ""Name"", ""U_SO1_PROMOCION"", ""U_SO1_PROMOCADENA"",
                                              ""U_SO1_GRUPO"", ""U_SO1_ACTIVO"") 
                                              VALUES (@pCode, @pName, @pPromo, @pPromoCadena, @pGrupo, @pActivo)"));

        internal static Consulta ModificarGrupo = new Consulta(Proveedor.SQLServer,
            new ComandoConsulta(Proveedor.SQLServer, @"UPDATE [${TABLA}] SET [U_SO1_ACTIVO] = @pActivo
                                                     WHERE [Code] = @pCode "),
            new ComandoConsulta(Proveedor.Hana, @"UPDATE ""${TABLA}"" SET ""U_SO1_ACTIVO"" = @pActivo
                                                WHERE ""Code"" = @pCode "));

        internal static Consulta RegistrarPropiedad = new Consulta(Proveedor.SQLServer,
          new ComandoConsulta(Proveedor.SQLServer, @"INSERT INTO [${TABLA}] ([Code], [Name], [U_SO1_PROMOCION], [U_SO1_PROMOCADENA],
                                                   [U_SO1_PROPIEDAD], [U_SO1_ACTIVO]) 
                                                   VALUES (@pCode, @pName, @pPromo, @pPromoCadena, @pPropiedad, @pActivo)"),
          new ComandoConsulta(Proveedor.Hana, @"INSERT INTO ""${TABLA}"" (""Code"", ""Name"", ""U_SO1_PROMOCION"", ""U_SO1_PROMOCADENA"",
                                              ""U_SO1_PROPIEDAD"", ""U_SO1_ACTIVO"") 
                                              VALUES (@pCode, @pName, @pPromo, @pPromoCadena, @pPropiedad, @pActivo)"));

        internal static Consulta ModificarPropiedad = new Consulta(Proveedor.SQLServer,
            new ComandoConsulta(Proveedor.SQLServer, @"UPDATE [${TABLA}] SET [U_SO1_ACTIVO] = @pActivo
                                                     WHERE [Code] = @pCode "),
            new ComandoConsulta(Proveedor.Hana, @"UPDATE ""${TABLA}"" SET ""U_SO1_ACTIVO"" = @pActivo
                                                WHERE ""Code"" = @pCode "));

        internal static Consulta VerificarRegistro = new Consulta(Proveedor.SQLServer,
            new ComandoConsulta(Proveedor.SQLServer, @"SELECT Code FROM [${TABLA}] WHERE [Code] = @pCode"),
            new ComandoConsulta(Proveedor.Hana, @"SELECT ""Code"" FROM ""${TABLA}"" WHERE ""Code"" = @pCode"));

        internal static Consulta EliminarPromo = new Consulta(Proveedor.SQLServer,
            new ComandoConsulta(Proveedor.SQLServer, @"DELETE FROM ""${TABLA}"" WHERE ""U_SO1_PROMOCION"" = @pNumPromocion"),
            new ComandoConsulta(Proveedor.Hana, @"DELETE FROM ""${TABLA}"" WHERE ""U_SO1_PROMOCION"" = @pNumPromocion"));

        internal static Consulta EliminarPoliticaRedencion = new Consulta(Proveedor.SQLServer,
            new ComandoConsulta(Proveedor.SQLServer, @"DELETE FROM ""@SO1_01POLITICAREDEN"" WHERE ""Code"" = @pCadena"),
            new ComandoConsulta(Proveedor.Hana, @"DELETE FROM ""@SO1_01POLITICAREDEN"" WHERE ""Code"" = @pCadena"));

        internal static Consulta EliminarRegistro = new Consulta(Proveedor.SQLServer,
            new ComandoConsulta(Proveedor.SQLServer, @"DELETE FROM ""${TABLA}"" WHERE ""Code"" = @pCode"),
            new ComandoConsulta(Proveedor.Hana, @"DELETE FROM ""${TABLA}"" WHERE ""Code"" = @pCode"));

        #endregion

        internal static Consulta ConsultarArticulosxIdentificador = new Consulta(Proveedor.SQLServer,
            new ComandoConsulta(Proveedor.SQLServer, @"SELECT A.Code Codigo, A.U_SO1_ARTICULO Articulo
                                                     FROM [@SO1_01PROMOARTICULO] A
                                                     WHERE A.U_SO1_PROMOCION = @pPromocion AND A.U_SO1_CODIGOAMBITO = @pCodigoAmbito "),
            new ComandoConsulta(Proveedor.Hana, @"SELECT A.""Code"" ""Codigo"", A.""U_SO1_ARTICULO"" ""Articulo""
                                                     FROM ""@SO1_01PROMOARTICULO"" A
                                                     WHERE A.""U_SO1_PROMOCION"" = @pPromocion AND A.""U_SO1_CODIGOAMBITO"" = @pCodigoAmbito "));

        internal static Consulta ConsultarCatalogoHoras = new Consulta(Proveedor.SQLServer,
            new ComandoConsulta(Proveedor.SQLServer, @"SELECT A.Code Codigo, A.Name Nombre, A.U_SO1_CODIGPROMOCION Promo,
                                                     A.U_SO1_PROMOCIONCADEN PromoCadena, A.U_SO1_CODIGCATEHORAR CodigoCatalogo, Horario.Name NombreHorario
                                                     FROM [@SO1_01PROMCATHORAR] A
                                                     LEFT JOIN [@SO1_01CATHORARIO] Horario 
                                                     ON (A.U_SO1_CODIGCATEHORAR = Horario.U_SO1_CODIGOHORARIO)
                                                     WHERE A.U_SO1_CODIGPROMOCION = @pPromocion"),
            new ComandoConsulta(Proveedor.Hana, @"SELECT A.""Code"" ""Codigo"", A.""Name"" ""Nombre"", A.""U_SO1_CODIGPROMOCION"" ""Promo"",
                                                A.""U_SO1_PROMOCIONCADEN"" ""PromoCadena"", A.""U_SO1_CODIGCATEHORAR"" ""CodigoCatalogo"", Horario.""Name"" ""NombreHorario""
                                                FROM ""@SO1_01PROMCATHORAR"" A
                                                LEFT JOIN ""@SO1_01CATHORARIO"" Horario 
                                                ON (A.""U_SO1_CODIGCATEHORAR"" = Horario.""U_SO1_CODIGOHORARIO"")
                                                WHERE A.""U_SO1_CODIGPROMOCION"" = @pPromocion"));

        internal static Consulta RegistrarCatalogo = new Consulta(Proveedor.SQLServer,
            new ComandoConsulta(Proveedor.SQLServer, @"INSERT INTO [@SO1_01PROMCATHORAR] ([Code], [Name], [U_SO1_CODIGPROMOCION], [U_SO1_PROMOCIONCADEN],
                                                       [U_SO1_CODIGCATEHORAR]) 
                                                       VALUES (@pCodigo, @pNombre, @pPromo, @pPromoCadena, @pCodigoHorario)"),
            new ComandoConsulta(Proveedor.Hana, @"INSERT INTO ""@SO1_01PROMCATHORAR"" (""Code"", ""Name"", ""U_SO1_CODIGPROMOCION"", ""U_SO1_PROMOCIONCADEN"",
                                                ""U_SO1_CODIGCATEHORAR"") 
                                                VALUES (@pCodigo, @pNombre, @pPromo, @pPromoCadena, @pCodigoHorario)"));

        internal static Consulta ConsultarCodigoClientes = new Consulta(Proveedor.SQLServer,
            new ComandoConsulta(Proveedor.SQLServer, @"SELECT A.Code Codigo, A.U_SO1_CLIENTE CodigoCliente
                                                     FROM [@SO1_01PROMOCLIENTE] A
                                                     WHERE A.U_SO1_PROMOCION = @pPromocion"),
            new ComandoConsulta(Proveedor.Hana, @"SELECT A.""Code"" ""Codigo"", A.""U_SO1_CLIENTE"" ""CodigoCliente""
                                                FROM ""@SO1_01PROMOCLIENTE"" A
                                                WHERE A.""U_SO1_PROMOCION"" = @pPromocion"));

        internal static Consulta ListarAlianzas = new Consulta(Proveedor.SQLServer,
            new ComandoConsulta(Proveedor.SQLServer, @"SELECT *
                                                     FROM [@SO1_01ALIANZACOMER] "),
            new ComandoConsulta(Proveedor.Hana, @"SELECT *
                                                     FROM ""@SO1_01ALIANZACOMER"" "));

        internal static Consulta UltimoCodigoControl = new Consulta(Proveedor.SQLServer,
            new ComandoConsulta(Proveedor.SQLServer, @"SELECT U_SO1_PROMOCION Promo FROM [@SO1_01CODIGOCONTROL] WHERE Code = 1"),
            new ComandoConsulta(Proveedor.Hana, @"SELECT ""U_SO1_PROMOCION"" ""Promo"" FROM ""@SO1_01CODIGOCONTROL"" WHERE ""Code"" = 1 "));

        internal static Consulta ActualizarCodigoControl = new Consulta(Proveedor.SQLServer,
            new ComandoConsulta(Proveedor.SQLServer, @"UPDATE [@SO1_01CODIGOCONTROL] SET  [U_SO1_PROMOCION] = @pCodigo WHERE [Code] = 1"),
            new ComandoConsulta(Proveedor.Hana, @"UPDATE ""@SO1_01CODIGOCONTROL"" SET  ""U_SO1_PROMOCION"" = @pCodigo WHERE ""Code"" = 1 "));

        internal static Consulta ActualizarJerarquia = new Consulta(Proveedor.SQLServer,
            new ComandoConsulta(Proveedor.SQLServer, @"UPDATE [@SO1_01PROMOCION] SET  [U_SO1_JERARQUIA] = @pJerarquia WHERE [Code] = @pCodigo"),
            new ComandoConsulta(Proveedor.Hana, @"UPDATE ""@SO1_01PROMOCION"" SET  ""U_SO1_JERARQUIA"" = @pJerarquia WHERE ""Code"" = @pCodigo "));

        internal static Consulta UltimaPromocion = new Consulta(Proveedor.SQLServer,
            new ComandoConsulta(Proveedor.SQLServer, @"SELECT  TOP 1  U_SO1_PROMOCION Promo FROM [@SO1_01PROMOCION] order by U_SO1_PROMOCION DESC"),
            new ComandoConsulta(Proveedor.Hana, @"SELECT TOP 1  ""U_SO1_PROMOCION"" ""Promo"" FROM ""@SO1_01PROMOCION"" order by ""U_SO1_PROMOCION"" DESC "));

        internal static Consulta UltimaJerarquia = new Consulta(Proveedor.SQLServer,
            new ComandoConsulta(Proveedor.SQLServer, @"SELECT  TOP 1  U_SO1_JERARQUIA Jerarquia FROM [@SO1_01PROMOCION] order by U_SO1_JERARQUIA DESC "),
            new ComandoConsulta(Proveedor.Hana, @"SELECT  TOP 1  ""U_SO1_JERARQUIA"" ""Jerarquia"" FROM ""@SO1_01PROMOCION"" order by ""U_SO1_JERARQUIA"" DESC "));

        internal static Consulta PostTransaccion = new Consulta(Proveedor.SQLServer,
            new ComandoConsulta(Proveedor.SQLServer, @"INSERT INTO [${TABLA}] ([SO1_TIPOOBJETO], [SO1_TIPOTRANSACCION], [SO1_NUMCOLUMNAS], [SO1_LISTACOLUMNAS],
                                                     [SO1_LISTAVALORES]) 
                                                     VALUES (@pTipoObjeto, @pTipoTransaccion, @pNumColumnas, @pListaColumnas, @pListaValores)"),
            new ComandoConsulta(Proveedor.Hana, @"INSERT INTO ""${TABLA}"" (""SO1_ID"", ""SO1_TIPOOBJETO"", ""SO1_TIPOTRANSACCION"", ""SO1_NUMCOLUMNAS"", ""SO1_LISTACOLUMNAS"",
                                                  ""SO1_LISTAVALORES"") 
                                                  VALUES (""SO1_BD_LICENCIAMIENTOR1"".""${POST}"".NEXTVAL,@pTipoObjeto, @pTipoTransaccion, @pNumColumnas, @pListaColumnas, @pListaValores)"));
        
        internal static Consulta VerificaCodigoUnico = new Consulta(Proveedor.SQLServer,
            new ComandoConsulta(Proveedor.SQLServer, @"SELECT *
                                                     FROM [@SO1_01PROMOCION] WHERE Code = @pCodigo "),
            new ComandoConsulta(Proveedor.Hana, @"SELECT *
                                                     FROM ""@SO1_01PROMOCION""  WHERE ""Code"" = @pCodigo "));

        internal static Consulta VerificaNombreUnico = new Consulta(Proveedor.SQLServer,
            new ComandoConsulta(Proveedor.SQLServer, @"SELECT *
                                                     FROM [@SO1_01PROMOCION] WHERE Name = @pNombre "),
            new ComandoConsulta(Proveedor.Hana, @"SELECT *
                                                     FROM ""@SO1_01PROMOCION"" WHERE ""Name"" = @pNombre "));

        internal static Consulta VerificaNombre = new Consulta(Proveedor.SQLServer,
            new ComandoConsulta(Proveedor.SQLServer, @"SELECT Name
                                                     FROM [@SO1_01PROMOCION] WHERE Name = @pNombre AND Code != @pCodigo"),
            new ComandoConsulta(Proveedor.Hana, @"SELECT ""Name""
                                                     FROM ""@SO1_01PROMOCION"" WHERE ""Name"" = @pNombre AND ""Code"" != @pCodigo "));
    }
    
    #endregion

}
