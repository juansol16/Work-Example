using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using SO1_PR_ReportesWeb_V020000.BO;
using SO1_PR_ReportesWeb_V020000.DAO;
using SO1_PR_ReportesWeb_V020000.Clases;
using ReportesWeb.Modelo;
using System.Web.Services;
using ReportesWeb.Adaptador;
using ReportesWeb.Ejecucion;

namespace SO1_PR_ReportesWeb_V020000
{
    public partial class CR1PromocionAxB : System.Web.UI.Page
    {
        #region Campos
        SO1_CL_Login oLogin;
        #endregion

        #region Eventos

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (Session["NUsuario"] == null)
                {
                    Session.Abandon();
                    Response.Redirect("Login", false);
                }
                else
                {
                    oLogin = new SO1_CL_Login();
                    oLogin.Nombre = Session["NUsuario"].ToString();
                    oLogin.Usuario = Session["Usuario"].ToString();
                }
                
                if (!this.IsPostBack)
                {
                  
                }
            }
            catch (Exception oError)
            {
                ReportesWeb.Seguridad.RegistroEvento.GuardarError("Error: ", oError);
            }
        }

        protected override void OnInit(EventArgs e)
        {

            #region Bitácora

            // AUTOR:               Juan Sol
            // FECHA:               20.06.2019
            // COMENTARIOS:         Creación del evento

            #endregion

            if (!RetailOne.Ejecucion.Sistema.Inicializado)
            {
                RetailOne.Ejecucion.Sistema.Inicializacion("~/RetailOne.config");
            }
            if (!ReportesWeb.Ejecucion.Sistema.InicializadoWeb)
            {
                ReportesWeb.Ejecucion.Sistema.InicializacionWeb("~/ReportesWeb.config");
            }
            
            base.OnInit(e);
        }
        
        #endregion

        #region WebMethods

        [WebMethod]
        public static List<Object> cargarConfiguracion()
        {
            List<Object> listResponse = new List<Object>();

            try
            {
                string ImportesDec = Sistema.ConfiguracionSap["ImportesDecimales"];
                string PreciosDec = Sistema.ConfiguracionSap["PreciosDecimales"];
                string TasasDec = Sistema.ConfiguracionSap["TasasDecimales"];
                string CantidadDec = Sistema.ConfiguracionSap["CantidadesDecimales"];
                string PorcentajeDec = Sistema.ConfiguracionSap["PorcentajeDecimales"];
                string UnidadesDec = Sistema.ConfiguracionSap["UnidadesDecimales"];
                string ConsultaDec = Sistema.ConfiguracionSap["ConsultaDecimales"];

                listResponse.Add("OK");
                listResponse.Add(ImportesDec);
                listResponse.Add(PreciosDec);
                listResponse.Add(TasasDec);
                listResponse.Add(CantidadDec);
                listResponse.Add(PorcentajeDec);
                listResponse.Add(UnidadesDec);
                listResponse.Add(ConsultaDec);

            }
            catch (Exception oException)
            {
                listResponse.Add("ERROR");
            }
             
            return listResponse;
        }

        [WebMethod]
        public static List<Object> ListarTablas() 
        {
            IEnumerable<Grupo> grupos = Grupo.Listar();
            IEnumerable<FormaPago> formaspago = FormaPago.Listar();

            List<Object> listResponse = new List<Object>();

            if (grupos != null || formaspago != null)
            {
                listResponse.Add("OK");
                listResponse.Add(grupos);
                listResponse.Add(formaspago);
            }
            else
            {
                listResponse.Add("Error");
                listResponse.Add("");
            }

            return listResponse;
        }

        [WebMethod]
        public static List<Object> CargarAutocomplete()
        {
            List<Object> listResponse = new List<Object>();

            try
            {
                //IEnumerable<Promocion> promociones = Promocion.Listar("AB");
                IEnumerable<Cliente> clientes = Cliente.Listar();
                IEnumerable<Cliente> columnasCliente = Cliente.ListarColumnas();
                IEnumerable<Articulo> articulos = Articulo.Listar();
                IEnumerable<Articulo> columnasArticulo = Articulo.ListarColumnas();
                IEnumerable<Proveedorr> proveedores = Proveedorr.Listar();
                
                listResponse.Add("OK");
                //listResponse.Add(promociones);
                listResponse.Add(clientes);
                listResponse.Add(articulos);
                listResponse.Add(proveedores);
                listResponse.Add(columnasCliente);
                listResponse.Add(columnasArticulo);

            }
            catch (Exception oError)
            {
                listResponse.Add("Error");
                listResponse.Add("");
            }
            
            return listResponse;

        }

        [WebMethod]
        public static List<Object> CargarAutocompletePromociones()
        {
            IEnumerable<Promocion> promociones = Promocion.Listar("AB");

            List<Object> listResponse = new List<Object>();

            if (promociones != null)
            {
                listResponse.Add("OK");
                listResponse.Add(promociones);
            }
            else
            {
                listResponse.Add("Error");
                listResponse.Add("");
            }

            return listResponse;
        }

        [WebMethod]
        public static List<Object> Consultar(string codigo, string nombre) 
        {
            
            List<Object> listResponse = new List<Object>();
            if (ValidarSesion())
            {
                Promocion oPromocion = Promocion.Consultar(null, codigo, nombre);
                if (oPromocion != null)
                {
                    listResponse.Add("OK");
                    listResponse.Add(oPromocion);
                }
                else
                {
                    listResponse.Add("Error");
                    listResponse.Add("");
                }
            }

            return listResponse;
        }

        [WebMethod]
        public static List<Object> RealizarPromocion(string promocion)
        {
            List<Object> listResponse = new List<Object>();
            if (ValidarSesion())
            {
                Idioma oIdioma;
                oIdioma = new Idioma();
                try
                {
                    Promocion oPromocion = new Promocion();
                    oPromocion = Newtonsoft.Json.JsonConvert.DeserializeObject<Promocion>(promocion);
                    //Promocion oPromocionOriginal = new Promocion();
                    //oPromocionOriginal = Newtonsoft.Json.JsonConvert.DeserializeObject<Promocion>(promocionOriginal);
                    //oPromocion.AgregarPromocion(oPromocionOriginal);

                    if (oPromocion.Existe)
                    {
                        if (oPromocion.Modificar())
                        {
                            listResponse.Add("OK");
                            listResponse.Add("SI");
                        }
                        else
                        {
                            listResponse.Add("Error");
                            listResponse.Add("Error al guardar los cambios");
                        }

                    }
                    else
                    {
                        oPromocion.U_SO1_TIPO = "AB";
                        if (oPromocion.Registrar())
                        {
                            listResponse.Add("OK");
                            listResponse.Add("NO");
                        }
                        else
                        {
                            listResponse.Add("Error");
                            listResponse.Add("Error al crear la promoción");
                        }
                    }
                }
                catch (Exception oError)
                {
                    string sIdioma = oIdioma.ObtenerCodigoIdioma();
                    while (oError.InnerException != null) oError = oError.InnerException;
                    //listResponse.Add(oIdioma.RegresaTraduccionJSON(oError.Message, sIdioma).ToString());
                    listResponse.Add(oError.Message);
                }
            }
            return listResponse;
        }

        [WebMethod]
        public static List<Object> ImportarArticulos(string datos, string articulos, bool tipoImportacion)
        {
            string[] arArticulos;
            Int32 iIndex = 0;
            List<Object> listResponse;
            Boolean articulosAgregados = true;
            int numArticulosActuales = 0;
            string[] arArticulos2;
            try
            {

            listResponse = new List<Object>();

            arArticulos = Newtonsoft.Json.JsonConvert.DeserializeObject<string[]>(datos);

            List<MPromoArticulo> listArticulosActuales = null;
            if(articulos != null)
                listArticulosActuales = Newtonsoft.Json.JsonConvert.DeserializeObject<List<MPromoArticulo>>(articulos);
            int cont = arArticulos.Count();
            numArticulosActuales = listArticulosActuales != null ? listArticulosActuales.Count() : 0;
            arArticulos2 = new string[cont + numArticulosActuales];
            for (int i = 0; i < arArticulos.Count(); i++)
            {
                arArticulos2[i] = arArticulos[i];
            }
            if (tipoImportacion)
            {
                foreach (MPromoArticulo art in listArticulosActuales)
                {
                    arArticulos2[cont] = art.U_SO1_ARTICULO;
                    cont++;
                }
            }
            Promocion oPromocion = null;
            if(tipoImportacion)
            {
                oPromocion = Promocion.Importar(arArticulos2);
            }
            else
            {
                oPromocion = Promocion.Importar(arArticulos);
            }
            for(int i=0; i<arArticulos.Count();i++)
            {
                iIndex = oPromocion.Articulos.ToList().FindIndex(x => x.U_SO1_ARTICULO == arArticulos[i]);
                if (iIndex == -1)
                {
                    ReportesWeb.Seguridad.RegistroEvento.GuardarError("Promocion AXB: No se agrego el codigo: " + arArticulos[i], null);
                    articulosAgregados = false;
                }
            }

            if (oPromocion != null)
            {
                listResponse.Add("OK");
                listResponse.Add(oPromocion.Articulos);
                listResponse.Add(articulosAgregados);
            }
            else
            {
                listResponse.Add("Error");
                listResponse.Add("");
            }
            }
            catch (Exception oError)
            {
                listResponse = new List<Object>();
                ReportesWeb.Seguridad.RegistroEvento.GuardarError("Error: ", oError);
                listResponse.Add("Error");
                listResponse.Add(oError.Message);
            }
            return listResponse;
        }
        
        #endregion

        #region Metodos

        private static bool ValidarSesion()
        {
            if (HttpContext.Current.Session["NUsuario"] == null)
            {
                return false;
            }

            return true;
        }

        #endregion

    }
}