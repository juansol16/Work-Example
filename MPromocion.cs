using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using RetailOne;
using RetailOne.Datos;

namespace ReportesWeb.Modelo
{
    public abstract class MPromocion : Entidad
    {

        #region Bitácora

        // AUTOR:               Juan Sol
        // FECHA:               10.06.19
        // COMENTARIOS:         Creación de la clase

        // AUTOR:               Juan Sol
        // FECHA:               09.07.19
        // COMENTARIOS:         Agregada propiedad de Existe y de Nombre de Tipo

        #endregion

        #region Atributos

        [JsonProperty("Existe")]
        public bool Existe { get; set; }

        [JsonProperty("U_SO1_PROMOCION")]
        public int U_SO1_PROMOCION { get; set; }

        [JsonProperty("U_SO1_JERARQUIA")]
        public int U_SO1_JERARQUIA { get; set; }

        [JsonProperty("U_SO1_TIPO")]
        public string U_SO1_TIPO { get; set; }

        [JsonProperty("NombreTipo")]
        public string NombreTipo { get; set; }

        [JsonProperty("U_SO1_FECHADESDE")]
        public string U_SO1_FECHADESDE { get; set; }

        [JsonProperty("U_SO1_FECHAHASTA")]
        public string U_SO1_FECHAHASTA { get; set; }

        [JsonProperty("U_SO1_DIARIO")]
        public string U_SO1_DIARIO { get; set; }

        [JsonProperty("U_SO1_DIACOMPLETO")]
        public string U_SO1_DIACOMPLETO { get; set; }

        [JsonProperty("U_SO1_DIAHORAINI")]
        public string U_SO1_DIAHORAINI { get; set; }

        [JsonProperty("U_SO1_DIAHORAFIN")]
        public string U_SO1_DIAHORAFIN { get; set; }

        [JsonProperty("U_SO1_LUNES")]
        public string U_SO1_LUNES { get; set; }

        [JsonProperty("U_SO1_LUNCOMPLETO")]
        public string U_SO1_LUNCOMPLETO { get; set; }

        [JsonProperty("U_SO1_LUNHORAINI")]
        public string U_SO1_LUNHORAINI { get; set; }

        [JsonProperty("U_SO1_LUNHORAFIN")]
        public string U_SO1_LUNHORAFIN { get; set; }

        [JsonProperty("U_SO1_MARTES")]
        public string U_SO1_MARTES { get; set; }

        [JsonProperty("U_SO1_MARCOMPLETO")]
        public string U_SO1_MARCOMPLETO { get; set; }

        [JsonProperty("U_SO1_MARHORAINI")]
        public string U_SO1_MARHORAINI { get; set; }

        [JsonProperty("U_SO1_MARHORAFIN")]
        public string U_SO1_MARHORAFIN { get; set; }

        [JsonProperty("U_SO1_MIERCOLES")]
        public string U_SO1_MIERCOLES { get; set; }

        [JsonProperty("U_SO1_MIECOMPLETO")]
        public string U_SO1_MIECOMPLETO { get; set; }

        [JsonProperty("U_SO1_MIEHORAINI")]
        public string U_SO1_MIEHORAINI { get; set; }

        [JsonProperty("U_SO1_MIEHORAFIN")]
        public string U_SO1_MIEHORAFIN { get; set; }

        [JsonProperty("U_SO1_JUEVES")]
        public string U_SO1_JUEVES { get; set; }

        [JsonProperty("U_SO1_JUECOMPLETO")]
        public string U_SO1_JUECOMPLETO { get; set; }

        [JsonProperty("U_SO1_JUEHORAINI")]
        public string U_SO1_JUEHORAINI { get; set; }

        [JsonProperty("U_SO1_JUEHORAFIN")]
        public string U_SO1_JUEHORAFIN { get; set; }

        [JsonProperty("U_SO1_VIERNES")]
        public string U_SO1_VIERNES { get; set; }

        [JsonProperty("U_SO1_VIECOMPLETO")]
        public string U_SO1_VIECOMPLETO { get; set; }

        [JsonProperty("U_SO1_VIEHORAINI")]
        public string U_SO1_VIEHORAINI { get; set; }

        [JsonProperty("U_SO1_VIEHORAFIN")]
        public string U_SO1_VIEHORAFIN { get; set; }

        [JsonProperty("U_SO1_SABADO")]
        public string U_SO1_SABADO { get; set; }

        [JsonProperty("U_SO1_SABCOMPLETO")]
        public string U_SO1_SABCOMPLETO { get; set; }

        [JsonProperty("U_SO1_SABHORAINI")]
        public string U_SO1_SABHORAINI { get; set; }

        [JsonProperty("U_SO1_SABHORAFIN")]
        public string U_SO1_SABHORAFIN { get; set; }

        [JsonProperty("U_SO1_DOMINGO")]
        public string U_SO1_DOMINGO { get; set; }

        [JsonProperty("U_SO1_DOMCOMPLETO")]
        public string U_SO1_DOMCOMPLETO { get; set; }

        [JsonProperty("U_SO1_DOMHORAINI")]
        public string U_SO1_DOMHORAINI { get; set; }

        [JsonProperty("U_SO1_DOMHORAFIN")]
        public string U_SO1_DOMHORAFIN { get; set; }

        [JsonProperty("U_SO1_ARTICULOENLACE")]
        public string U_SO1_ARTICULOENLACE { get; set; }

        [JsonProperty("U_SO1_FILTRARARTART")]
        public string U_SO1_FILTRARARTART { get; set; }

        [JsonProperty("U_SO1_FILTRARARTGRU")]
        public string U_SO1_FILTRARARTGRU { get; set; }

        [JsonProperty("U_SO1_FILTRARARTPRO")]
        public string U_SO1_FILTRARARTPRO { get; set; }

        [JsonProperty("U_SO1_FILTRARARTPROE")]
        public string U_SO1_FILTRARARTPROE { get; set; }

        [JsonProperty("U_SO1_FILTRARARTPROC")]
        public string U_SO1_FILTRARARTPROC { get; set; }

        [JsonProperty("U_SO1_FILTRARARTCOE")]
        public string U_SO1_FILTRARARTCOE { get; set; }

        [JsonProperty("U_SO1_FILTRARARTCOEC")]
        public string U_SO1_FILTRARARTCOEC { get; set; }

        [JsonProperty("U_SO1_FILTRARARTCOEV")]
        public string U_SO1_FILTRARARTCOEV { get; set; }

        [JsonProperty("U_SO1_FILTRARARTPROV")]
        public string U_SO1_FILTRARARTPROV { get; set; }

        [JsonProperty("U_SO1_FILTRARARTFAB")]
        public string U_SO1_FILTRARARTFAB { get; set; }

        [JsonProperty("U_SO1_FILTRARARTEXC")]
        public string U_SO1_FILTRARARTEXC { get; set; }

        [JsonProperty("U_SO1_FILTRARARTUNE")]
        public string U_SO1_FILTRARARTUNE { get; set; }

        [JsonProperty("U_SO1_FILTRARPROMSUC")]
        public string U_SO1_FILTRARPROMSUC { get; set; }

        [JsonProperty("U_SO1_FILTRARPROMALI")]
        public string U_SO1_FILTRARPROMALI { get; set; }

        [JsonProperty("U_SO1_FILTRARPROMCLI")]
        public string U_SO1_FILTRARPROMCLI { get; set; }

        [JsonProperty("U_SO1_FILTRARCLICLI")]
        public string U_SO1_FILTRARCLICLI { get; set; }

        [JsonProperty("U_SO1_FILTRARCLIENLS")]
        public string U_SO1_FILTRARCLIENLS { get; set; }

        [JsonProperty("U_SO1_FILTRARCLIGRU")]
        public string U_SO1_FILTRARCLIGRU { get; set; }

        [JsonProperty("U_SO1_FILTRARCLIPRO")]
        public string U_SO1_FILTRARCLIPRO { get; set; }

        [JsonProperty("U_SO1_FILTRARCLIPROE")]
        public string U_SO1_FILTRARCLIPROE { get; set; }

        [JsonProperty("U_SO1_FILTRARCLIPROC")]
        public string U_SO1_FILTRARCLIPROC { get; set; }

        [JsonProperty("U_SO1_FILTRARCLICOE")]
        public string U_SO1_FILTRARCLICOE { get; set; }

        [JsonProperty("U_SO1_FILTRARCLICOEC")]
        public string U_SO1_FILTRARCLICOEC { get; set; }

        [JsonProperty("U_SO1_FILTRARCLICOEV")]
        public string U_SO1_FILTRARCLICOEV { get; set; }

        [JsonProperty("U_SO1_FILTRARPROMMEM")]
        public string U_SO1_FILTRARPROMMEM { get; set; }

        [JsonProperty("U_SO1_FILTRARPROMLIS")]
        public string U_SO1_FILTRARPROMLIS { get; set; }

        [JsonProperty("U_SO1_FILTRARPROMFOP")]
        public string U_SO1_FILTRARPROMFOP { get; set; }

        [JsonProperty("U_SO1_ACUMPROMORM")]
        public string U_SO1_ACUMPROMORM { get; set; }

        [JsonProperty("U_SO1_ACUMPROMOPUNTO")]
        public string U_SO1_ACUMPROMOPUNTO { get; set; }

        [JsonProperty("U_SO1_ACUMPROMOOTRAS")]
        public string U_SO1_ACUMPROMOOTRAS { get; set; }

        [JsonProperty("U_SO1_ACTLIMPIEZAVEN")]
        public string U_SO1_ACTLIMPIEZAVEN { get; set; }

        [JsonProperty("U_SO1_LIMITEPIEZAVEN")]
        public string U_SO1_LIMITEPIEZAVEN { get; set; }

        [JsonProperty("U_SO1_ACTLIMPIEZAPRO")]
        public string U_SO1_ACTLIMPIEZAPRO { get; set; }

        [JsonProperty("U_SO1_LIMITEPIEZAPRO")]
        public string U_SO1_LIMITEPIEZAPRO { get; set; }

        [JsonProperty("U_SO1_COMPORTAESCAUT")]
        public string U_SO1_COMPORTAESCAUT { get; set; }

        [JsonProperty("U_SO1_ACTLIMVENTAPRO")]
        public string U_SO1_ACTLIMVENTAPRO { get; set; }

        [JsonProperty("U_SO1_LIMITEVENTAPRO")]
        public string U_SO1_LIMITEVENTAPRO { get; set; }

        [JsonProperty("U_SO1_ACTVENTACREDIT")]
        public string U_SO1_ACTVENTACREDIT { get; set; }

        [JsonProperty("U_SO1_FILTRARARTCON")]
        public string U_SO1_FILTRARARTCON { get; set; }

        [JsonProperty("U_SO1_FILTRARARTCONV")]
        public string U_SO1_FILTRARARTCONV { get; set; }

        /*public enum TipoPromocion
        {
            Alianza,
            Sucursal,
            Membresia,
            Precio,
            FormasPago,
            GrupoClientes,
            GrupoArticulos,
            PropiedadesClientes,
            PropiedadesArticulos
        }*/

        /// <summary>
        //Lista de Sucursales
        [JsonProperty("Sucursales")]
        private Dictionary<string, MPromoSucursal> _sucursales = new Dictionary<string, MPromoSucursal>();
        [JsonIgnore]
        public IEnumerable<MPromoSucursal> Sucursales
        {
            get
            {
                return _sucursales.Values;
            }
        }

        /// <summary>
        //Lista de Alianzas
        [JsonProperty("Alianzas")]
        private Dictionary<string, MPromoAlianza> _alianzas = new Dictionary<string, MPromoAlianza>();
        [JsonIgnore]
        public IEnumerable<MPromoAlianza> Alianzas
        {
            get
            {
                return _alianzas.Values;
            }
        }

        /// <summary>
        /// 
        //Lista de Membresias
        [JsonProperty("Membresias")]
        private Dictionary<string, MPromoMembresia> _membresias = new Dictionary<string, MPromoMembresia>();
        [JsonIgnore]
        public IEnumerable<MPromoMembresia> Membresias
        {
            get
            {
                return _membresias.Values;
            }
        }

        /// <summary>
        /// 
        //Lista de Precios
        [JsonProperty("Precios")]
        private Dictionary<string, MPromoPrecios> _precios = new Dictionary<string, MPromoPrecios>();
        [JsonIgnore]
        public IEnumerable<MPromoPrecios> Precios
        {
            get
            {
                return _precios.Values;
            }
        }

        /// <summary>
        /// 
        //Lista de Formas de Pago
        [JsonProperty("FormasPago")]
        private Dictionary<string, MPromoFPago> _formaspago = new Dictionary<string, MPromoFPago>();
        [JsonIgnore]
        public IEnumerable<MPromoFPago> FormasPago
        {
            get
            {
                return _formaspago.Values;
            }
        }

        /// <summary>
        /// 
        //Lista de Formas de Pago
        [JsonProperty("GrupoClientes")]
        private Dictionary<string, MPromoGrupo> _grupoclientes = new Dictionary<string, MPromoGrupo>();
        [JsonIgnore]
        public IEnumerable<MPromoGrupo> GrupoClientes
        {
            get
            {
                return _grupoclientes.Values;
            }
        }

        /// <summary>
        /// 
        //Lista de Formas de Pago
        [JsonProperty("GrupoArticulos")]
        private Dictionary<string, MPromoGrupo> _grupoarticulos = new Dictionary<string, MPromoGrupo>();
        [JsonIgnore]
        public IEnumerable<MPromoGrupo> GrupoArticulos
        {
            get
            {
                return _grupoarticulos.Values;
            }
        }

        /// <summary>
        /// 
        //Lista de Formas de Pago
        [JsonProperty("PropiedadesClientes")]
        private Dictionary<string, MPromoPropiedad> _propiedadesclientes = new Dictionary<string, MPromoPropiedad>();
        [JsonIgnore]
        public IEnumerable<MPromoPropiedad> PropiedadesClientes
        {
            get
            {
                return _propiedadesclientes.Values;
            }
        }

        /// <summary>
        /// 
        //Lista de Formas de Pago
        [JsonProperty("PropiedadesArticulos")]
        private Dictionary<string, MPromoPropiedad> _propiedadesarticulos = new Dictionary<string, MPromoPropiedad>();
        [JsonIgnore]
        public IEnumerable<MPromoPropiedad> PropiedadesArticulos
        {
            get
            {
                return _propiedadesarticulos.Values;
            }
        }

        /// <summary>
        /// 
        //Lista de Formas de Pago
        [JsonProperty("MedidasArticulos")]
        private Dictionary<string, MPromoArticuloMedida> _medidasarticulos = new Dictionary<string, MPromoArticuloMedida>();
        [JsonIgnore]
        public IEnumerable<MPromoArticuloMedida> MedidasArticulos
        {
            get
            {
                return _medidasarticulos.Values;
            }
        }

        /// <summary>
        /// 
        //Lista de Formas de Pago
        [JsonProperty("ArticuloFabricante")]
        private Dictionary<string, MPromoArticuloFabricante> _articulofabricante = new Dictionary<string, MPromoArticuloFabricante>();
        [JsonIgnore]
        public IEnumerable<MPromoArticuloFabricante> ArticuloFabricante
        {
            get
            {
                return _articulofabricante.Values;
            }
        }

        /// <summary>
        /// 
        //Lista de Proveedores
        [JsonProperty("ArticuloProveedor")]
        protected Dictionary<string, MPromoArticuloProveedor> _articuloproveedor = new Dictionary<string, MPromoArticuloProveedor>();
        [JsonIgnore]
        public IEnumerable<MPromoArticuloProveedor> ArticuloProveedor
        {
            get
            {
                return _articuloproveedor.Values;
            }
        }

        /// <summary>
        /// 
        //Lista de Formas de Pago
        [JsonProperty("Articulos")]
        protected Dictionary<string, MPromoArticulo> _articulos = new Dictionary<string, MPromoArticulo>();
        [JsonIgnore]
        public IEnumerable<MPromoArticulo> Articulos
        {
            get
            {
                return _articulos.Values;
            }
        }

        /// <summary>
        /// 
        //Lista de Formas de Pago
        [JsonProperty("Excepciones")]
        protected Dictionary<string, MPromoArticulo> _excepciones = new Dictionary<string, MPromoArticulo>();
        [JsonIgnore]
        public IEnumerable<MPromoArticulo> Excepciones
        {
            get
            {
                return _excepciones.Values;
            }
        }

        /// <summary>
        /// 
        //Lista de Formas de Pago
        [JsonProperty("Clientes")]
        protected Dictionary<string, MPromoCliente> _clientes = new Dictionary<string, MPromoCliente>();
        [JsonIgnore]
        public IEnumerable<MPromoCliente> Clientes
        {
            get
            {
                return _clientes.Values;
            }
        }

        //[JsonIgnore]
        //public List<MPostTransaccion> Transacciones { get; set; }

        /// <summary>
        /// 
        //Lista de Formas de Pago
        public MPromoAxB PromocionAxB { get; set; }

        public MPromDescEmp PromocionDescEmp { get; set; }

        public MPromDescEsc PromocionDescEsc { get; set; }

        public MPromDescImp PromocionDescImp { get; set; }

        public MPromDescPor PromocionDescPor { get; set; }

        public MPromDescVol PromocionDescVol { get; set; }

        public MPromoKitVenta PromocionKitVenta { get; set; }

        public MPromKitRegalo PromocionKitRegalo { get; set; }

        public MPromPolVenta PromocionPoliticaVenta { get; set; }

        public MPromoPrecioUnico PromocionPrecioUnico { get; set; }

        public MPromValeDescuento PromocionValeDescuento { get; set; }

        public MPromPoliticaRedencion PromocionPoliticaRedencion { get; set; }

        public MPromRegaloMonto PromocionRegaloMonto { get; set; }

        public MPromDescAle PromocionDescAle { get; set; }

        public MCupon PromocionCupon { get; set; }

        public static Origen OrigenDatosSecundario
        {
            get
            {
                if (RetailOne.Ejecucion.Sistema.Ambientes.Contains("LICE"))
                {
                    return RetailOne.Ejecucion.Sistema.Ambientes["LICE"].Origen;
                }
                throw new Exception("No se ha configurado el origen de datos de Licenciamiento");
            }
        } 

        #endregion

        #region Métodos
        //Método para agregar las sucursales
        public void Agregar(MPromoArticulo mArticulo, string tipo)
        {
            if (mArticulo == null || string.IsNullOrEmpty(mArticulo.U_SO1_ARTICULO))
                return;

            if (tipo == "G")
            {
                _articulos[mArticulo.U_SO1_ARTICULO] = mArticulo;
            }
            else
            {
                _excepciones[mArticulo.U_SO1_ARTICULO] = mArticulo;
            }
        }
        //Método para agregar los articulos con un IENumerable
        public void Agregar(IEnumerable<MPromoArticulo> articulos, string tipo, bool borrarPrevios = false)
        {
            if (articulos == null || articulos.Count() == 0)
                return;

            if (borrarPrevios)
            {
                if (tipo == "G")
                {
                    _articulos.Clear();
                }
                else
                {
                    _excepciones.Clear();
                }
            }

            foreach (MPromoArticulo articulo in articulos)
            {
                Agregar(articulo, tipo);
            }
        }
        //Método para eliminar articulos
        public void Eliminar(MPromoArticulo mArticulo, string tipo)
        {
            if (mArticulo == null || string.IsNullOrEmpty(mArticulo.U_SO1_ARTICULO))
                return;

            if (tipo == "G")
            {
                _articulos.Remove(mArticulo.U_SO1_ARTICULO);
            }
            else
            {
                _excepciones.Remove(mArticulo.U_SO1_ARTICULO);
            }
        }

        
        //Método para agregar las sucursales
        public void Agregar(MPromoCliente mCliente)
        {
            if (mCliente == null || string.IsNullOrEmpty(mCliente.U_SO1_CLIENTE))
                return;

            _clientes[mCliente.U_SO1_CLIENTE] = mCliente;
        }
        //Método para agregar los clientess con un IENumerable
        public void Agregar(IEnumerable<MPromoCliente> clientes, bool borrarPrevios = false)
        {
            if (clientes == null || clientes.Count() == 0)
                return;

            if (borrarPrevios)
                _clientes.Clear();

            foreach (MPromoCliente cliente in clientes)
            {
                Agregar(cliente);
            }
        }
        //Método para eliminar clientes
        public void Eliminar(MPromoCliente mCliente)
        {
            _clientes.Remove(mCliente.U_SO1_CLIENTE);
        }

        //Método para agregar las sucursales
        public void Agregar(MPromoSucursal mSucursal)
        {
         
            if (mSucursal == null || string.IsNullOrEmpty(mSucursal.U_SO1_SUCURSAL))
                return;

            _sucursales[mSucursal.U_SO1_SUCURSAL] = mSucursal;
        }
        //Método para agregar las sucursales con un IENumerable
        public void Agregar(IEnumerable<MPromoSucursal> sucursales, bool borrarPrevios = false)
        {
            if (sucursales == null || sucursales.Count() == 0)
                return;

            if (borrarPrevios)
                _sucursales.Clear();

            foreach (MPromoSucursal sucursal in sucursales)
            {
                Agregar(sucursal);
            }
        }
        
        //Método para agregar las alianzas
        public void Agregar(MPromoAlianza mAlianza)
        {
            if (mAlianza == null || string.IsNullOrEmpty(mAlianza.U_SO1_ALIANZA))
                return;

            _alianzas[mAlianza.U_SO1_ALIANZA] = mAlianza;
        }
        //Método para agregar las alianzas con un IENumerable
        public void Agregar(IEnumerable<MPromoAlianza> alianzas, bool borrarPrevios = false)
        {
            if (alianzas == null || alianzas.Count() == 0)
                return;

            if (borrarPrevios)
                _alianzas.Clear();

            foreach (MPromoAlianza alianza in alianzas)
            {
                Agregar(alianza);
            }
        }

        //Método para agregar las membresias
        public void Agregar(MPromoMembresia mMembresia)
        {
            if (mMembresia == null || string.IsNullOrEmpty(mMembresia.U_SO1_TIPOMEMBRESIA))
                return;

            _membresias[mMembresia.U_SO1_TIPOMEMBRESIA] = mMembresia;
        }
        //Método para agregar las membresias con un IENumerable
        public void Agregar(IEnumerable<MPromoMembresia> membresias, bool borrarPrevios = false)
        {
            if (membresias == null || membresias.Count() == 0)
                return;

            if (borrarPrevios)
                _membresias.Clear();

            foreach (MPromoMembresia membresia in membresias)
            {
                Agregar(membresia);
            }
        }

        //Método para agregar las membresias
        public void Agregar(MPromoPrecios mPrecios)
        {
            if (mPrecios == null || string.IsNullOrEmpty(mPrecios.U_SO1_LISTAPRECIO))
                return;

            _precios[mPrecios.U_SO1_LISTAPRECIO] = mPrecios;
        }
        //Método para agregar los precios con un IENumerable
        public void Agregar(IEnumerable<MPromoPrecios> precios, bool borrarPrevios = false)
        {
            if (precios == null || precios.Count() == 0)
                return;

            if (borrarPrevios)
                _precios.Clear();

            foreach (MPromoPrecios precio in precios)
            {
                Agregar(precio);
            }
        }

        //Método para agregar las formas de pago
        public void Agregar(MPromoFPago mFormasPago)
        {
            if (mFormasPago == null || string.IsNullOrEmpty(mFormasPago.U_SO1_FORMAPAGO))
                return;

            _formaspago[mFormasPago.U_SO1_FORMAPAGO] = mFormasPago;
        }
        //Método para agregar las formas de pago con un IENumerable
        public void Agregar(IEnumerable<MPromoFPago> formasPago, bool borrarPrevios = false)
        {
            if (formasPago == null || formasPago.Count() == 0)
                return;

            if (borrarPrevios)
                _formaspago.Clear();

            foreach (MPromoFPago formaPago in formasPago)
            {
                Agregar(formaPago);
            }
        }

        //Método para agregar las formas de pago
        public void Agregar(MPromoArticuloFabricante mArticuloFabricante)
        {
            if (mArticuloFabricante == null || string.IsNullOrEmpty(mArticuloFabricante.U_SO1_FABRICANTE))
                return;

            _articulofabricante[mArticuloFabricante.U_SO1_FABRICANTE] = mArticuloFabricante;
        }
        //Método para agregar los fabricantes con un IENumerable
        public void Agregar(IEnumerable<MPromoArticuloFabricante> fabricantes, bool borrarPrevios = false)
        {
            if (fabricantes == null || fabricantes.Count() == 0)
                return;

            if (borrarPrevios)
                _articulofabricante.Clear();

            foreach (MPromoArticuloFabricante fabricante in fabricantes)
            {
                Agregar(fabricante);
            }
        }

        //Método para agregar los proveedores
        public void Agregar(MPromoArticuloProveedor mArticuloProveedor)
        {
            if (mArticuloProveedor == null || string.IsNullOrEmpty(mArticuloProveedor.U_SO1_PROVEEDOR))
                return;

            _articuloproveedor[mArticuloProveedor.U_SO1_PROVEEDOR] = mArticuloProveedor;
        }
        //Método para agregar los proveedores con un IENumerable
        public void Agregar(IEnumerable<MPromoArticuloProveedor> proveedores, bool borrarPrevios = false)
        {
            if (proveedores == null || proveedores.Count() == 0)
                return;

            if (borrarPrevios)
                _articuloproveedor.Clear();

            foreach (MPromoArticuloProveedor proveedor in proveedores)
            {
                Agregar(proveedor);
            }
        }
        //Método para eliminar proveedores
        public void Eliminar(MPromoArticuloProveedor mArticuloProveedor)
        {
            
            _articuloproveedor.Remove(mArticuloProveedor.U_SO1_PROVEEDOR);
        }

        //Método para agregar las formas de pago
        public void Agregar(MPromoArticuloMedida mArticuloMedida)
        {
            if (mArticuloMedida == null || string.IsNullOrEmpty(mArticuloMedida.U_SO1_UNIDAD))
                return;

            _medidasarticulos[mArticuloMedida.U_SO1_UNIDAD] = mArticuloMedida;
        }
        //Método para agregar los proveedores con un IENumerable
        public void Agregar(IEnumerable<MPromoArticuloMedida> medidasArticulo, bool borrarPrevios = false)
        {
            if (medidasArticulo == null || medidasArticulo.Count() == 0)
                return;

            if (borrarPrevios)
                _medidasarticulos.Clear();

            foreach (MPromoArticuloMedida medidaArticulo in medidasArticulo)
            {
                Agregar(medidaArticulo);
            }
        }

        //Método para agregar las formas de pago
        public void Agregar(MPromoGrupo mGrupo, string tipo)
        {
            if (mGrupo == null || string.IsNullOrEmpty(mGrupo.U_SO1_GRUPO))
                return;

            if (tipo == "Cliente")
            {
                _grupoclientes[mGrupo.U_SO1_GRUPO] = mGrupo;
            }
            else
            {
                _grupoarticulos[mGrupo.U_SO1_GRUPO] = mGrupo;
            }
        }
        //Método para agregar los proveedores con un IENumerable
        public void Agregar(IEnumerable<MPromoGrupo> grupos, string tipo, bool borrarPrevios = false)
        {
            if (grupos == null || grupos.Count() == 0)
                return;

            if (borrarPrevios)
            {
                if (tipo == "Cliente")
                {
                    _grupoclientes.Clear();
                }
                else
                {
                    _grupoarticulos.Clear();
                }
            }

            foreach (MPromoGrupo grupo in grupos)
            {
                Agregar(grupo, tipo);
            }
        }

        //Método para agregar las formas de pago
        public void Agregar(MPromoPropiedad mPropiedad, string tipo)
        {
            if (mPropiedad == null || string.IsNullOrEmpty(mPropiedad.U_SO1_PROPIEDAD))
                return;

            if (tipo == "Cliente")
            {
                _propiedadesclientes[mPropiedad.U_SO1_PROPIEDAD] = mPropiedad; 
            }

            else
            {
                _propiedadesarticulos[mPropiedad.U_SO1_PROPIEDAD] = mPropiedad; 
            }
        }
        //Método para agregar los proveedores con un IENumerable
        public void Agregar(IEnumerable<MPromoPropiedad> propiedades, string tipo, bool borrarPrevios = false)
        {
            if (propiedades == null || propiedades.Count() == 0)
                return;

            if (borrarPrevios)
            {
                if (tipo == "Cliente")
                {
                    _propiedadesclientes.Clear();
                }
                else
                {
                    _propiedadesarticulos.Clear();
                }
            }

            foreach (MPromoPropiedad propiedad in propiedades)
            {
                Agregar(propiedad, tipo);
            }
        }

        public abstract bool Buscar(string Codigo, string Nombre);

        public abstract bool ListarArticulos(String[] arArticulo);

        public abstract bool ListarArticulosPrecios(String[] arArticulo, string ListaPrecio1, string ListaPrecio2);

        #endregion

    }
    
}
