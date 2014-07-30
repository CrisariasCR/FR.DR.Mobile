using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SQLiteBase;
using Softland.ERP.FR.Mobile.Cls.AccesoDatos;
using Softland.ERP.FR.Mobile.Cls.FRCliente;
using Softland.ERP.FR.Mobile.Cls.Corporativo;
using Softland.ERP.FR.Mobile.Cls.Documentos.FRPedido;

using Cirrious.MvvmCross.ViewModels;

namespace Softland.ERP.FR.Mobile.Cls.FRArticulo
{

    public abstract class Model : MvxNotifyPropertyChanged
    {
    }

    /// <summary>
    /// Clase que representa un articulo
    /// </summary>
    public class Articulo : MvxNotifyPropertyChanged
    {

        #region Constructores

        public Articulo() { }

        /// <summary>
        /// Constructor de un articulo con codigo y compania
        /// </summary>
        /// <param name="codigo">codigo del articulo</param>
        /// <param name="compania">compania a asociar</param>
        public Articulo(string codigo, string compania)
        {
            this.codigo = codigo;
            this.Compania = compania;

        }

        public Object Clone()
        {
            Articulo clon = new Articulo(this.codigo, this.Compania);
            clon.codigoBarras = this.codigoBarras;
            clon.clase = this.clase;
            clon.costoDolar = this.CostoDolar;
            clon.costoLocal = this.costoLocal;
            clon.descripcion = descripcion;
            clon.familia = familia;
            clon.grupoArticulo = grupoArticulo;
            clon.impuesto = this.impuesto;
            clon.tipoEmpaqueAlmacen = this.tipoEmpaqueAlmacen;
            clon.tipoEmpaqueDetalle = this.tipoEmpaqueDetalle;
            clon.unidadEmpaque = this.unidadEmpaque;
            clon.usaLotes = this.usaLotes;
            clon.precio = (Precio)this.precio.Clone();
            clon.bodega = (Bodega)this.Bodega.Clone();
            return clon;
        }

        #endregion
  
        #region Constantes para Acceso Datos

        public const string CAMPOS_ARTICULO = ",A.DES_ART,A.COD_FAM,A.COD_CLS,A.COD_BAR,A.GRP_ART," +
                                             @" A.UND_EMP,A.EMP_ALM,A.EMP_DET,A.USA_LOTES,I.IMPUESTO1,I.IMPUESTO2," +
                                             @" A.COSTO_LOCAL,A.COSTO_DOLAR,A.FACTOR_PRECIO ";
        //public const string INNER_JOIN =
        //        " INNER JOIN " + Table.ERPADMIN_ARTICULO + " A ON ( D.COD_ART = A.COD_ART AND D.COD_CIA = A.COD_CIA) " +
        //        " INNER JOIN " + Table.ERPADMIN_IMPUESTO + " I ON ( A.COD_CIA = I.COD_CIA AND A.IMPUESTO = I.IMPUESTO) ";

        #endregion

        #region Variables y Propiedades de instancia

        #region Datos Generales

        private string compania = string.Empty;
        /// <summary>
        /// Código de Compañía del artículo.
        /// </summary>
        public string Compania
        {
            get { return compania.ToUpper(); }
            set { compania = value.ToUpper(); RaisePropertyChanged("Compania"); }
        }
        
        #endregion

        #region Códigos

        private string codigo = string.Empty;
        /// <summary>
        /// Código del artículo.
        /// </summary>
        public string Codigo
        {
            get { return codigo; }
            set { codigo = value; RaisePropertyChanged("Codigo"); }
        }

        private string codigoEnvase = string.Empty;
        /// <summary>
        /// Código del artículo.
        /// </summary>
        public string CodigoEnvase
        {
            get { return codigoEnvase; }
            set { codigoEnvase = value; RaisePropertyChanged("CodigoEnvase"); }
        }

        private Articulo articuloEnvase;
        /// <summary>
        /// Código del artículo.
        /// </summary>
        public Articulo ArticuloEnvase
        {
            get { return articuloEnvase; }
            set { articuloEnvase = value; }
        }

        private string codigoBarras = string.Empty;
        /// <summary>
        /// Código de barras del artículo.
        /// </summary>
        public string CodigoBarras
        {
            get { return codigoBarras; }
            set { codigoBarras = value; RaisePropertyChanged("CodigoBarras"); }
        }

        #endregion

        #region Descripción/Clasificación

        private string descripcion =string.Empty;
        /// <summary>
        /// Descripción del artículo.
        /// </summary>
        public string Descripcion
        {
            get { return descripcion; }
            set { descripcion = value; RaisePropertyChanged("Descripcion"); }
        }

        private string familia = string.Empty;
        /// <summary>
        /// Código de Familia (Primera clasificación del artículo).
        /// </summary>
        public string Familia
        {
            get { return familia; }
            set { familia = value; RaisePropertyChanged("Familia"); }
        }

        // MejorasGrupoPelon600R6 - KF //
        private string familiaDesc = string.Empty;
        /// <summary>
        /// Descripcion de Familia (Primera clasificación del artículo).
        /// </summary>
        public string FamiliaDesc
        {
            get { return familiaDesc; }
            set { familiaDesc = value; RaisePropertyChanged("FamiliaDesc"); }
        }
         
        private string clase = string.Empty;
        /// <summary>
        ///  Código de Clase (Segunda clasificación del artículo).
        /// </summary>
        public string Clase
        {
            get { return clase; }
            set { clase = value; RaisePropertyChanged("Clase"); }
        }

        private string grupoArticulo = string.Empty;
        /// <summary>
        /// Grupo al que pertenece el artículo.
        /// </summary>
        public string GrupoArticulo
        {
            get { return grupoArticulo; }
            set { grupoArticulo = value; RaisePropertyChanged("GrupoArticulo"); }
        }

        private bool usaLotes = false;
        /// <summary>
        /// Indica si el artículo utiliza lotes.
        /// </summary>
        public bool UsaLotes
        {
            get { return usaLotes; }
            set { usaLotes = value; RaisePropertyChanged("UsaLotes"); }
        }

        private TipoArticulo typeArticulo;
        /// <summary>
        /// Indica si el artículo es contenedor.
        /// </summary>
        public TipoArticulo TypeArticulo
        {
            get { return typeArticulo; }
            set { typeArticulo = value; RaisePropertyChanged("TypeArticulo"); }
        }

        #endregion

        #region Unidades Empaque

        private decimal unidadEmpaque = decimal.Zero;
        /// <summary>
        /// Factor de unidad de empaque.
        /// </summary>
        public decimal UnidadEmpaque
        {
            get { return unidadEmpaque; }
            set { unidadEmpaque = value; RaisePropertyChanged("UnidadEmpaque"); }
        }

        private string tipoEmpaqueAlmacen = string.Empty;
        /// <summary>
        /// Tipo/Descripción de empaque de las unidades de almacén.
        /// </summary>
        public string TipoEmpaqueAlmacen
        {
            get { return tipoEmpaqueAlmacen; }
            set { tipoEmpaqueAlmacen = value; RaisePropertyChanged("TipoEmpaqueAlmacen"); }
        }

        private string tipoEmpaqueDetalle = string.Empty;
        /// <summary>
        /// Tipo/Descripción de empaque de las unidades de detalle.
        /// </summary>
        public string TipoEmpaqueDetalle
        {
            get { return tipoEmpaqueDetalle; }
            set { tipoEmpaqueDetalle = value; RaisePropertyChanged("TipoEmpaqueDetalle"); }
        }

        private List<Lotes> lotesAsociados = new List<Lotes>();
        /// <summary>
        /// Tipo/Descripción de empaque de las unidades de detalle.
        /// </summary>
        public List<Lotes> LotesAsociados
        {
            get { return lotesAsociados; }
            set { lotesAsociados = value; RaisePropertyChanged("LotesAsociados"); }
        }

        #endregion

        #region Costos

        private decimal costoLocal = decimal.Zero;
        /// <summary>
        /// Costo del artículo en moneda local.
        /// </summary>
        public decimal CostoLocal
        {
            get { return Math.Round(costoLocal,2); }
            set { costoLocal = Math.Round(value,2); RaisePropertyChanged("CostoLocal"); }
        }

        private decimal costoDolar = decimal.Zero;
        /// <summary>
        /// Costo del artículo en moneda dolar.
        /// </summary>
        public decimal CostoDolar
        {
            get { return Math.Round(costoDolar,2); }
            set { costoDolar = Math.Round(value,2); RaisePropertyChanged("CostoDolar"); }
        }

        #endregion

        #region Impuestos

        private Impuesto impuesto = new Impuesto();
        /// <summary>
        /// Conjunto de Impuestos Ventas/Consumo.
        /// </summary>
        public Impuesto Impuesto
        {
            get { return impuesto; }
            set { impuesto = value; RaisePropertyChanged("Impuesto"); }
        }

        #endregion

        #region Listas de Precio

        private Precio precio = new Precio();
        /// <summary>
        /// Conjunto de Precios Detalle/Almacen.
        /// </summary>
        public Precio Precio
        {
          get { return precio; }
            set { precio = value; RaisePropertyChanged("Precio"); }
        }

        public decimal PrecioMaximo
        {
            get { return precio.Maximo; }
            set { precio.Maximo = value; RaisePropertyChanged("PrecioMaximo"); }
        }

        public decimal PrecioMinimo
        {
            get { return precio.Minimo; }
            set { precio.Minimo = value; RaisePropertyChanged("PrecioMinimo"); }
        }
        #endregion 

        #region Bodega

        private Bodega bodega = new Bodega();
        /// <summary>
        /// Contiene datos de la bodega desde donde el artículo fue vendido.
        /// </summary>
        public Bodega Bodega
        {
            get { return bodega; }
            set { bodega = value; RaisePropertyChanged("Bodega"); }
        }

        #endregion

        #region Descuentos

        private List<Descuento> descuentosEscalonados = new List<Descuento>();
        /// <summary>
        /// Contiene todos los descuentos escalonados asociados al artículo.
        /// </summary>
        public List<Descuento> DescuentosEscalonados
        {
          get { return descuentosEscalonados; }
            set { descuentosEscalonados = value; RaisePropertyChanged("DescuentosEscalonados"); }
        }

        private List<Bonificacion> bonificaciones = new List<Bonificacion>();
        /// <summary>
        /// Contiene las bonificaciones asociadas al artículo.
        /// </summary>
        public List<Bonificacion> Bonificaciones
        {
            get { return bonificaciones; }
            set { bonificaciones = value; RaisePropertyChanged("Bonificaciones"); }
        }

        #endregion

        #region Factor Venta
        private decimal factorVenta;

        /// <summary>
        /// Factor de venta. Se utiliza cuando el cliente utiliza multipos de venta.
        /// </summary>
        public decimal FactorVenta
        {
            get
            {
                return factorVenta;
            }
            set
            {
                factorVenta = value; RaisePropertyChanged("FactorVenta");
            }
        }
        #endregion

        #region Cantidades

        private decimal cantidadAlmacen;
        private decimal cantidadDetalle;

        public decimal CantidadAlmacen
        {
            get { return cantidadAlmacen; }
            set { cantidadAlmacen = value; RaisePropertyChanged("CantidadAlmacen"); }
        }
        public decimal CantidadDetalle
        {
            get { return cantidadDetalle; }
            set { cantidadDetalle = value; RaisePropertyChanged("CantidadDetalle"); }
        }        

        #endregion

        #region Reporte Liquidacion Inventario

        private decimal cantInicial;
        public decimal CantInicial 
        {
            get { return cantInicial;}
        }
        private decimal cantInicialDet;
        public decimal CantInicialDet
        {
            get { return cantInicialDet; }
        }

        private decimal cantDisponible;
        public decimal CantDisponible
        {
            get { return cantDisponible; }
        }
        private decimal cantDisponibleDet;
        public decimal CantDisponibleDet
        {
            get { return cantDisponibleDet; }
        }

        private decimal cantVacios;
        public decimal CantVacios 
        {
            get { return cantVacios; }
        }
        private decimal cantSalidas;
        public decimal CantSalidas
        {
            get { return cantSalidas; }
        }
        private decimal cantEntradas;
        public decimal CantEntradas
        {
            get { return cantEntradas; }
        }
        private decimal cantVaciosDet;
        public decimal CantVaciosDet
        {
            get { return cantVacios; }
        }

        private decimal cantVendidas;
        public decimal CantVendidas 
        {
            get { return cantVendidas; }
        }
        private decimal cantVendidasDet;
        public decimal CantVendidasDet
        {
            get { return cantVendidas; }
        }

        private decimal cantDevueltos;
        public decimal CantDevueltos
        {
            get { return cantDevueltos; }
        }
        private decimal cantDevueltosDet;
        public decimal CantDevueltosDet
        {
            get { return cantDevueltosDet; }
        }

        private decimal cantGarantia;
        public decimal CantGarantia
        {
            get { return cantGarantia; }
        }

        private decimal cantGarantiaDet;
        public decimal CantGarantiaDet
        {
            get { return cantGarantiaDet; }
        }

        #endregion
        /// <summary>
        /// Se usa para saber si el checkbox asociado está o no seleccionado
        /// </summary>
        public bool Seleccionado { get; set; }

        #endregion        

        #region Logica

        /// <summary>
        /// realizar carga del articulo por filtro de nivel de precio e ignorando el grupo de articulo
        /// </summary>
        /// <param name="c">filtro</param>
        /// <param name="nivel">nombre del nivel de precio</param>
        public void Cargar(FiltroArticulo[] c, string nivel)
        {
            
            //Cargar(c, string.Empty,false,nivel);
            Cargar(c, string.Empty, this.esArtEnvase(), nivel);
        }

        public bool esArtEnvase() 
        {
            SQLiteDataReader reader = null;
            bool result = false;
            string sentencia = "select TIPO from " + Table.ERPADMIN_ARTICULO + " WHERE COD_ART='{0}'";

            try
            {
                reader = GestorDatos.EjecutarConsulta(string.Format( sentencia,codigo));

                if (reader.Read())
                {
                    if (!reader.IsDBNull(0))
                        result = reader.GetString(0).ToString().Equals(((char)TipoArticulo.ENVASE).ToString());
                }
                else
                {                    
                    result = false;
                }
            }
            catch (Exception ex)
            {
                result = false;
                //throw new Exception("Error cargando el tipo del artículo '" + codigo + "' en la compañía '" + compania + "'. " + ex.Message);
            }
            finally
            {
                if (reader != null)
                    reader.Close();

            }
            return result;
        }
        
        /// <summary>
        /// realizar carga del articulo por filtro ignorando el grupo de articulo y el nivel de precio
        /// </summary>
        public void Cargar()
        {
            //Cargar(new FiltroArticulo[]{}, string.Empty,false,string.Empty);
            Cargar(new FiltroArticulo[] { }, string.Empty, this.esArtEnvase(), string.Empty);
        }

        /// <summary>
        /// realizar carga del articulo por filtro ignorando el grupo de articulo y el nivel de precio
        /// </summary>
        public void CargarEnvase()
        {
            Cargar(new FiltroArticulo[] { }, string.Empty,true,string.Empty);
        }
        
        /// <summary>
        /// Cargar el precio del articulo
        /// </summary>
        /// <param name="nivel">nivel de precio asociado</param>
        public void CargarPrecio(NivelPrecio nivel)
        {
            precio.CargarDatosPrecio(codigo, nivel, unidadEmpaque);
        }
        
        /// <summary>
        /// cargar el precio por el historico de factura
        /// </summary>
        /// <param name="factura">codigo de la factura asociada al articulo</param>
        public void CargarPrecioHistorico(string factura)
        {
            precio.CargarDatosPrecioHistorico(codigo,this.Compania, factura, unidadEmpaque);
        }
        
        /// <summary>
        /// Cargar el margen de utilidad del articulo
        /// </summary>
        /// <param name="nivel">nivel de precio del articulo</param>
        /// <returns>Margen de utilidad</returns>
        public decimal CargarMargenUtilidad(NivelPrecio nivel)
        {
            precio.CargarMargenUtilidad(codigo, nivel);
            return precio.MargenUtilidad;
        }
        
        /// <summary>
        /// Cargar Impuesto del articulo
        /// </summary>
        public void CargarImpuesto()
        {
            Impuesto.CargarDatosImpuesto(this.Compania);
        }
        
        /// <summary>
        /// Cargar descuentos del articulo
        /// </summary>
        /// <param name="cliente">Cliente en compania para obtener datos especificos de descuento</param>
        /// <param name="totalAlmacen">Rango de unidades de almacen</param>
        /// <returns>descuento del cliente para el articulo</returns>
        public Descuento CargarDescuento(ClienteCia cliente, decimal totalAlmacen)
        {
            return Descuento.ObtenerDescuento(cliente,this,totalAlmacen);
        }
      
        /// <summary>
        /// Cargar Descuentos para ver en consulta, sin importar el rango
        /// </summary>
        /// <param name="cliente">Cliente en compania para obtener datos especificos de descuento</param>
        /// <returns>descuentos del cliente para el articulo</returns>
        public void CargarDescuentos(ClienteCia cliente)
        {
            if (this.DescuentosEscalonados.Count == 0)         
                DescuentosEscalonados = Descuento.ObtenerDescuentos(cliente, this, decimal.MinValue);
        }
        
        /// <summary>
        /// Cargar bonificaciones del cliente para el articulo
        /// </summary>
        /// <param name="cliente"></param>
        public void CargarBonificaciones(ClienteCia cliente)
        {
            if(this.Bonificaciones.Count == 0)
                Bonificaciones = Bonificacion.ObtenerBonificaciones(cliente,this.Codigo,decimal.MinValue);

        }
        
        /// <summary>
        /// Actualizar la cantidad en bodega para el articulo
        /// </summary>
        /// <param name="cantidad">cantidad a actualizar</param>
        public void ActualizarBodega(decimal cantidad)
        {
            Bodega.ActualizarExistencia(compania, codigo, cantidad);
        }
        
        /// <summary>
        /// Cargar existencia del articulo en la bodega indicada
        /// </summary>
        /// <param name="bodega">bodega a obtener existencias</param>
        public void CargarExistencia(string bodega)
        {
            Bodega.Codigo = bodega;
            if (this.usaLotes)
            {
                Bodega.CargarExistenciaLote(this.Compania, this.codigo);
            }
            else
            {
                Bodega.CargarExistencia(this.Compania, this.codigo);
            }
        }

        #region Caso CR2-11258-7LWP JEV
        
        public void CargarExistenciaPedido(string bodega)
        {
            //Se asigna el códgio de la bodega
            Bodega.Codigo = bodega;
            
            //Se realiza la carga de las existencias
            Bodega.CargarExistencia(this.Compania, this.codigo);
            
        }
        
        #endregion Caso CR2-11258-7LWP JEV

        /// <summary>
        /// Generar sentencia de actualizacion a ejecutar en el cargador para actualizar existencias en la bodega
        /// </summary>
        /// <returns></returns>
        public static string SentenciaActualizacionServidor()
        {
            List<Articulo> articulosVendidos = ObtenerArticulosVendidos();
            StringBuilder sentencia = new StringBuilder();

            foreach (Articulo a in articulosVendidos)
            {
                a.Bodega.CargarExistencia(a.Compania, a.Codigo);
                sentencia.Append(a.Bodega.SentenciaActualizacion(a.Compania, a.Codigo));
                sentencia.Append("#|#");
            }
            return sentencia.ToString();
        }
        
        /// <summary>
        /// Busqueda de articulos a partir de una coleccion dada
        /// </summary>
        /// <param name="articulos">coleccion inicial</param>
        /// <param name="criterio">criterio de busqueda</param>
        /// <param name="valor">valor a buscar</param>
        /// <param name="exacto">si es una busqueda exacta</param>
        /// <returns>lista de articulos que cumplen con el criterio de busqueda</returns>
        public static List<Articulo> BusquedaDesconectada(List<Articulo> articulos, CriterioArticulo criterio, string valor, bool exacto)
        {
            List<Articulo> nuevaColeccion = new List<Articulo>();

            string propiedad = string.Empty;

            foreach (Articulo articulo in articulos)
            {
                switch (criterio)
                {
                    case CriterioArticulo.Codigo:
                        propiedad = articulo.Codigo;
                        break;
                    case CriterioArticulo.Barras:
                        propiedad = articulo.CodigoBarras;
                        break;
                    case CriterioArticulo.Descripcion:
                        propiedad = articulo.Descripcion;
                        break;
                    case CriterioArticulo.Familia:
                        propiedad = articulo.Familia;
                        break;
                    case CriterioArticulo.Clase:
                        propiedad = articulo.Clase;
                        break;
                }

                if (propiedad.ToUpper() == valor.ToUpper())
                {
                    //Los valores son exactamente iguales
                    nuevaColeccion.Add(articulo);
                }
                else if (!exacto)
                {
                    //La propiedad contiene el valor buscado
                    if (propiedad.ToUpper().IndexOf(valor.ToUpper(), 0, propiedad.Length) != -1)
                        nuevaColeccion.Add(articulo);
                }
            }

            return nuevaColeccion;
        }
        
        /// <summary>
        /// Indica si el articulo cumple dentro de un criterio de busqueda
        /// </summary>
        /// <param name="criterio">criterio de busqueda</param>
        /// <param name="valor">valor a buscar</param>
        /// <param name="exacto">si es una busqueda exacta</param>
        /// <returns>si el articulo cumple con el criterio de busqueda</returns>
        public bool BusquedaDesconectada(CriterioArticulo criterio, string valor, bool exacto)
        {
            valor = valor.ToUpper();
            string propiedad = ObtenerDato(criterio) ;

            if (propiedad.ToUpper() == valor)
                return true;
            else if (!exacto && propiedad.ToUpper().IndexOf(valor, 0, propiedad.Length) != -1)
                return true;

            //Si no encontro barras entonces buscar por codigo sencillo
            if (criterio == CriterioArticulo.Barras)
            {
                propiedad = ObtenerDato(CriterioArticulo.Codigo);
                if (propiedad.ToUpper() == valor)
                    return true;
                else if (!exacto && propiedad.ToUpper().IndexOf(valor, 0, propiedad.Length) != -1)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Busca dentro una cadena brindada la existencia de una a cadena buscar.
        /// </summary>
        /// <param name="cadenaOriginal">Cadena brindada sobre la cuál se va a realizar la búsqueda.</param>
        /// <param name="cadenaBusqueda">Cadena a buscar.</param>
        /// <returns>Retorna verdadero en caso de que se encuentra alguna coincidencia.</returns>
        public bool BuscarCadenaAproximada(CriterioArticulo criterio, string filtro)
        {
            string propiedad = ObtenerDato(criterio).Trim();
            filtro = filtro.Trim();

            if (propiedad.Length > 0 && filtro.Length > 0)
                if (propiedad.IndexOf(filtro, 0) >= 0)
                    return true;

            return false;
        }
        
        public string ObtenerDato(CriterioArticulo criterio)
        {
            switch (criterio)
            {
                case CriterioArticulo.Codigo:
                    return codigo;
                case CriterioArticulo.Barras:
                    return codigoBarras;
                case CriterioArticulo.Descripcion:
                    return descripcion;
                case CriterioArticulo.Familia:
                    return familia;
                case CriterioArticulo.Clase:
                   return clase;
                default:
                   return string.Empty;
            }        
        }
        
        #endregion 

        #region Acceso Datos

        /// <summary>
        /// Crear Sentencia SQL Basado en el Filtro (FROM) y/o Criterio de Busqueda (WHERE y ORDER BY)
        /// </summary>
        /// <param name="c">Filtro</param>
        /// <param name="cb">Criterio de busqueda</param>
        /// <returns></returns>
        private static string CrearSentencia(FiltroArticulo[] c, CriterioBusquedaArticulo cb,bool esEnvase)
        {
            string where = string.Empty;
            return CrearSentencia(c, cb,esEnvase, ref where);
        }

        /// <summary>
        /// Crear Sentencia SQL Basado en el Filtro (FROM) y/o Criterio de Busqueda (WHERE y ORDER BY)
        /// </summary>
        /// <param name="c">Filtro</param>
        /// <param name="cb">Criterio de busqueda</param>
        /// <returns></returns>
        private static string CrearSentenciaTomGarantia(FiltroArticulo[] c, CriterioBusquedaArticulo cb)
        {
            string where = string.Empty;
            return CrearSentenciaTomaGarantia(c, cb,ref where);
        }

        /// <summary>
        /// Crear Sentencia SQL Basado en el Filtro (FROM) y/o Criterio de Busqueda (WHERE y ORDER BY)
        /// </summary>
        /// <param name="c">Filtro</param>
        /// <param name="cb">Criterio de busqueda</param>
        /// <returns></returns>
        private static string CrearSentencia(FiltroArticulo[] c, CriterioBusquedaArticulo cb,bool esEnvase,ref string where)
        {
            List<FiltroArticulo> criterios = (new List<FiltroArticulo>(c.Length));
            criterios.AddRange(c);
            //LDA Mejora Cesar Iglesias, se agrega el campo A.ORDEN_ART
            string select =
                " SELECT A.COD_ART, A.DES_ART,A.COD_FAM,A.COD_CLS,A.COD_BAR,A.GRP_ART," +
                @" A.UND_EMP,A.EMP_ALM,A.EMP_DET,A.USA_LOTES,I.IMPUESTO1,I.IMPUESTO2," +
                @" A.COSTO_LOCAL,A.COSTO_DOLAR,A.FACTOR_PRECIO,A.ORDEN_ART, A.FACTOR_VENTA, A.ARTICULO_ENVASE,A.TIPO ";

            string from = " FROM " + Table.ERPADMIN_ARTICULO + " AS A INNER JOIN " + Table.ERPADMIN_IMPUESTO + " AS I " +
                          " ON UPPER(A.COD_CIA) = UPPER(I.COMPANIA) AND A.IMPUESTO = I.IMPUESTO ";

            string sentencia =
                "WHERE UPPER(A.COD_CIA) = @COMPANIA ";
            if (!esEnvase)
            {
                sentencia += " AND TIPO!=@TIPOENVASE ";
            }
            else
            {
                sentencia += " AND TIPO=@TIPOENVASE ";
            }

            if (criterios.Contains(FiltroArticulo.Existencia))
            {
                from += " INNER JOIN " + Table.ERPADMIN_ARTICULO_EXISTENCIA + " AS E " +
                          " ON UPPER(A.COD_CIA) = UPPER(E.COMPANIA) AND A.COD_ART = E.ARTICULO ";
                sentencia += " AND E.EXISTENCIA > 0 ";
            }
            if (criterios.Contains(FiltroArticulo.NivelPrecio))
            {
                from += "INNER JOIN " + Table.ERPADMIN_ARTICULO_PRECIO + " AS P " +
                        " ON UPPER(A.COD_CIA) = UPPER(P.COMPANIA) AND A.COD_ART = P.ARTICULO ";
                sentencia += " AND P.NIVEL_PRECIO = @NIVEL ";
            }            
            if (criterios.Contains(FiltroArticulo.GrupoArticulo))
                sentencia += " AND A.GRP_ART = @GRUPO ";

            //Si hay criterios de Busqueda
            if (cb != null)
            {
                switch (cb.Criterio)
                {
                    case CriterioArticulo.Codigo:
                        sentencia += " AND A.COD_ART " + (cb.Agil ? "=" : "LIKE") + " @ARTICULO ";
                        if(FRmConfig.OrdenarCriterio)
                           sentencia += " ORDER BY A.COD_ART ";
                        break;
                    case CriterioArticulo.Barras:
                        if(cb.Agil)
                            sentencia += " AND (A.COD_BAR = @BARRAS) ";
                        else
                            sentencia += " AND (A.COD_ART = @BARRAS OR A.COD_BAR = @BARRAS) ";
                        break;
                    case CriterioArticulo.Descripcion:
                        sentencia += " AND A.DES_ART " + (cb.Agil ? "=" : "LIKE") + " @DESCRIPCION ";
                        if(FRmConfig.OrdenarCriterio)
                           sentencia += " ORDER BY A.DES_ART ";                                     
                        break;
                    case CriterioArticulo.Familia:
                        sentencia += " AND A.COD_FAM " + (cb.Agil ? "=" : "LIKE") + " @FAMILIA ";
                        break;
                    case CriterioArticulo.Clase:
                        sentencia += " AND A.COD_CLS " + (cb.Agil ? "=" : "LIKE") + " @CLASE ";
                        break;
                }

                //LDA mejora Cesar Iglesias, se agrega order By ORDEN_ART a la sentencia
                if (FRmConfig.OrdenarIndiceArticulo)
                {
                    if (sentencia.Contains("ORDER BY"))
                        sentencia += ", ORDEN_ART";
                    else
                        sentencia += "ORDER BY ORDEN_ART";
                }
            }
            else //Se obtiene un unico articulo
            {
                sentencia += " AND A.COD_ART = @ARTICULO ";           
            }
            where = sentencia;
            return select + from + sentencia;
        }

        /// <summary>
        /// Crear Sentencia SQL Basado en el Filtro (FROM) y/o Criterio de Busqueda (WHERE y ORDER BY)
        /// </summary>
        /// <param name="c">Filtro</param>
        /// <param name="cb">Criterio de busqueda</param>
        /// <returns></returns>
        private static string CrearSentenciaTomaGarantia(FiltroArticulo[] c, CriterioBusquedaArticulo cb,ref string where)
        {
            List<FiltroArticulo> criterios = (new List<FiltroArticulo>(c.Length));
            criterios.AddRange(c);
            //LDA Mejora Cesar Iglesias, se agrega el campo A.ORDEN_ART
            string select =
                " SELECT A.COD_ART, A.DES_ART,A.COD_FAM,A.COD_CLS,A.COD_BAR,A.GRP_ART," +
                @" A.UND_EMP,A.EMP_ALM,A.EMP_DET,A.USA_LOTES,I.IMPUESTO1,I.IMPUESTO2," +
                @" A.COSTO_LOCAL,A.COSTO_DOLAR,A.FACTOR_PRECIO,A.ORDEN_ART, A.FACTOR_VENTA, A.ARTICULO_ENVASE,A.TIPO ";

            string from = " FROM " + Table.ERPADMIN_ARTICULO + " AS A INNER JOIN " + Table.ERPADMIN_IMPUESTO + " AS I " +
                          " ON UPPER(A.COD_CIA) = UPPER(I.COMPANIA) AND A.IMPUESTO = I.IMPUESTO ";

            string sentencia =
                "WHERE UPPER(A.COD_CIA) = @COMPANIA ";

            if (criterios.Contains(FiltroArticulo.Existencia))
            {
                from += " INNER JOIN " + Table.ERPADMIN_ARTICULO_EXISTENCIA + " AS E " +
                          " ON UPPER(A.COD_CIA) = UPPER(E.COMPANIA) AND A.COD_ART = E.ARTICULO ";
                sentencia += " AND E.EXISTENCIA > 0 ";
            }
            if (criterios.Contains(FiltroArticulo.NivelPrecio))
            {
                from += "INNER JOIN " + Table.ERPADMIN_ARTICULO_PRECIO + " AS P " +
                        " ON UPPER(A.COD_CIA) = UPPER(P.COMPANIA) AND A.COD_ART = P.ARTICULO ";
                sentencia += " AND P.NIVEL_PRECIO = @NIVEL ";
            }
            if (criterios.Contains(FiltroArticulo.GrupoArticulo))
                sentencia += " AND A.GRP_ART = @GRUPO ";

            //Si hay criterios de Busqueda
            if (cb != null)
            {
                switch (cb.Criterio)
                {
                    case CriterioArticulo.Codigo:
                        sentencia += " AND A.COD_ART " + (cb.Agil ? "=" : "LIKE") + " @ARTICULO ";
                        if (FRmConfig.OrdenarCriterio)
                            sentencia += " ORDER BY A.COD_ART ";
                        break;
                    case CriterioArticulo.Barras:
                        if (cb.Agil)
                            sentencia += " AND (A.COD_BAR = @BARRAS) ";
                        else
                            sentencia += " AND (A.COD_ART = @BARRAS OR A.COD_BAR = @BARRAS) ";
                        break;
                    case CriterioArticulo.Descripcion:
                        sentencia += " AND A.DES_ART " + (cb.Agil ? "=" : "LIKE") + " @DESCRIPCION ";
                        if (FRmConfig.OrdenarCriterio)
                            sentencia += " ORDER BY A.DES_ART ";
                        break;
                    case CriterioArticulo.Familia:
                        sentencia += " AND A.COD_FAM " + (cb.Agil ? "=" : "LIKE") + " @FAMILIA ";
                        break;
                    case CriterioArticulo.Clase:
                        sentencia += " AND A.COD_CLS " + (cb.Agil ? "=" : "LIKE") + " @CLASE ";
                        break;
                }

                //LDA mejora Cesar Iglesias, se agrega order By ORDEN_ART a la sentencia
                if (FRmConfig.OrdenarIndiceArticulo)
                {
                    if (sentencia.Contains("ORDER BY"))
                        sentencia += ", ORDEN_ART";
                    else
                        sentencia += "ORDER BY ORDEN_ART";
                }
            }
            else //Se obtiene un unico articulo
            {
                sentencia += " AND A.COD_ART = @ARTICULO ";
            }
            where = sentencia;
            return select + from + sentencia;
        }
        
        /// <summary>
        /// Crear parametros para consulta de un articulo en particular por codigo
        /// </summary>
        /// <param name="c">filtro</param>
        /// <param name="grupoArticulo">grupo de articulo</param>
        /// <param name="nivel">nivel de precio</param>
        /// <returns>Parametros creados</returns>        
        private SQLiteParameterList CrearParametros(FiltroArticulo[] c, string grupoArticulo,  string nivel)
        {
            List<FiltroArticulo> criterios = (new List<FiltroArticulo>(c.Length));
            criterios.AddRange(c);

            SQLiteParameterList parametros = new SQLiteParameterList();
            parametros.Add("@COMPANIA", compania);
            parametros.Add("@ARTICULO", codigo);
            parametros.Add("@TIPOENVASE", ((char)TipoArticulo.ENVASE).ToString());
            if (criterios.Contains(FiltroArticulo.NivelPrecio))
                parametros.Add("@NIVEL", nivel);
            if (criterios.Contains(FiltroArticulo.GrupoArticulo))
                parametros.Add("@GRUPO", grupoArticulo);

            return parametros;            
        }
        
        /// <summary>
        /// Crear Parametros para Busquedas Por Aproximacion
        /// </summary>
        /// <param name="c">filtro</param>       
        /// <param name="cb">criterio de busqueda</param>
        /// <param name="compania">compania a filtrar articulos</param>          
        /// <param name="grupoArticulo">grupo de articulo</param>        
        /// <param name="nivel">nivel de precio</param>
        /// <returns>Parametros creados</returns>  
        private static SQLiteParameterList CrearParametros(FiltroArticulo[] c, CriterioBusquedaArticulo cb, string compania, string grupoArticulo, string nivel)
        {
            List<FiltroArticulo> criterios = (new List<FiltroArticulo>(c.Length));
            criterios.AddRange(c);

            SQLiteParameterList parametros = new SQLiteParameterList();

            parametros.Add("@COMPANIA", compania);
            parametros.Add("@TIPOENVASE",((char)TipoArticulo.ENVASE).ToString());
            if (criterios.Contains(FiltroArticulo.NivelPrecio))
                parametros.Add("@NIVEL", nivel);
            if (criterios.Contains(FiltroArticulo.GrupoArticulo))
                parametros.Add("@GRUPO", grupoArticulo);

            //Criterios de Busqueda por aproximacion
            string parametroBusqueda = string.Empty;

            switch(cb.Criterio)
            {
                case CriterioArticulo.Barras : 
                    parametroBusqueda = "@BARRAS";
                    break;
                 case CriterioArticulo.Codigo : 
                    parametroBusqueda = "@ARTICULO";
                    break;                   
                 case CriterioArticulo.Familia : 
                    parametroBusqueda = "@FAMILIA";
                    break;      
                 case CriterioArticulo.Descripcion : 
                    parametroBusqueda = "@DESCRIPCION";
                    break;                 
                 case CriterioArticulo.Clase : 
                    parametroBusqueda = "@CLASE";
                    break;            
            }

            parametros.Add(parametroBusqueda, cb.Valor);
            return parametros;
        }

        public void Cargar(ref SQLiteDataReader reader, int offset)
        {
            this.descripcion = reader.GetString(offset);
            if (!reader.IsDBNull(offset+1))
                this.familia = reader.GetString(offset + 1);
            if (!reader.IsDBNull(offset + 2))
                this.clase = reader.GetString(offset + 2);
            if (!reader.IsDBNull(offset + 3))
                this.codigoBarras = reader.GetString(offset + 3);
            this.grupoArticulo = reader.GetString(offset + 4);
            this.unidadEmpaque = reader.GetDecimal(offset + 5);
            this.tipoEmpaqueAlmacen = reader.GetString(offset + 6);
            this.tipoEmpaqueDetalle = reader.GetString(offset + 7);
            this.usaLotes = reader.GetString(offset + 8).Equals("S");
            this.impuesto.Impuesto1 = reader.GetDecimal(offset + 9);
            this.impuesto.Impuesto2 = reader.GetDecimal(offset + 10);
            this.costoLocal = reader.GetDecimal(offset + 11);
            this.costoDolar = reader.GetDecimal(offset + 12);
            this.precio.PorcentajeRecargo = reader.GetDecimal(offset + 13);
        }

        /// <summary>
        /// Cargar un articulo particular
        /// </summary>
        /// <param name="c">filtro de busqueda</param>
        /// <param name="grupoArticulo">grupo de articulo</param>
        /// <param name="nivel">nivel de precio</param>
        public void Cargar(FiltroArticulo[] c, string grupoArticulo,bool esEnvase,string nivel)
        {
            SQLiteDataReader reader = null;
            string sentencia = CrearSentencia(c, null, esEnvase);
            SQLiteParameterList parametros = CrearParametros(c,grupoArticulo,nivel);
            bool existe = true;
            Compania cia = Corporativo.Compania.Obtener(compania);

            try
            {
                reader = GestorDatos.EjecutarConsulta(sentencia, parametros);

                if (reader.Read())
                {
                    this.codigo = reader.GetString(0);
                    this.descripcion = reader.GetString(1);

                    if (!reader.IsDBNull(2))
                        this.familia = reader.GetString(2);

                    if (!reader.IsDBNull(3))
                        this.clase = reader.GetString(3);

                    if (!reader.IsDBNull(4))
                        this.codigoBarras = reader.GetString(4);

                    this.grupoArticulo = reader.GetString(5);
                    this.unidadEmpaque = reader.GetDecimal(6);
                    this.tipoEmpaqueAlmacen = reader.GetString(7);
                    this.tipoEmpaqueDetalle = reader.GetString(8);
                    this.usaLotes = reader.GetString(9).Equals("S");
                    this.impuesto.Imp1AfectaDescto = cia.Impuesto1AfectaDescto;
                    this.impuesto.Impuesto1 = reader.GetDecimal(10);
                    this.impuesto.Impuesto2 = reader.GetDecimal(11);
                    this.costoLocal = reader.GetDecimal(12);
                    this.costoDolar = reader.GetDecimal(13);
                    this.precio.PorcentajeRecargo = reader.GetDecimal(14);
                    this.factorVenta = reader.GetDecimal(16);
                }
                else
                {
                    existe = false;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error cargando la información del artículo '" + codigo + "' en la compañía '" + compania + "'. " + ex.Message);
            }
            finally
            {
                if (reader != null)
                    reader.Close();
                if (!existe)
                    throw new Exception("Artículo " + this.codigo + " no se encontró");

            }
        }

        //public static Articulo ObtenerArticuloCodigo(string codigo, string compania)
        //{
        //    CriterioBusquedaArticulo cb = new CriterioBusquedaArticulo(CriterioArticulo.Codigo, codigo) { Agil = false };
        //    string sentencia = CrearSentencia(new FiltroArticulo[]{}, cb);
        //    SQLiteParameterList parametros = CrearParametros(new FiltroArticulo[] { }, cb, compania, null, null);
        //    SQLiteDataReader reader = null;
        //    Articulo a = null;
        //    try
        //    {
        //        Compania cia = Corporativo.Compania.Obtener(compania);
        //        reader = GestorDatos.EjecutarConsulta(sentencia, parametros);
        //        if (reader.Read())
        //        {
        //            a = new Articulo();
        //            a.codigo = reader.GetString(0);
        //            a.descripcion = reader.GetString(1);

        //            if (!reader.IsDBNull(2))
        //                a.familia = reader.GetString(2);

        //            if (!reader.IsDBNull(3))
        //                a.clase = reader.GetString(3);

        //            if (!reader.IsDBNull(4))
        //                a.codigoBarras = reader.GetString(4);

        //            a.grupoArticulo = reader.GetString(5);
        //            a.unidadEmpaque = reader.GetDecimal(6);
        //            a.tipoEmpaqueAlmacen = reader.GetString(7);
        //            a.tipoEmpaqueDetalle = reader.GetString(8);
        //            a.usaLotes = reader.GetString(9).Equals("S");
        //            a.impuesto.Impuesto1 = reader.GetDecimal(10);
        //            a.impuesto.Impuesto2 = reader.GetDecimal(11);
        //            a.impuesto.Imp1AfectaDescto = cia.Impuesto1AfectaDescto;
        //            a.costoLocal = reader.GetDecimal(12);
        //            a.costoDolar = reader.GetDecimal(13);
        //            a.precio.PorcentajeRecargo = reader.GetDecimal(14);
        //            a.factorVenta = reader.GetDecimal(16);
        //            a.Compania = compania;
        //        }
                
        //    }
        //    catch (Exception e) { }
        //    finally
        //    {
        //        if (reader != null)
        //            reader.Close();
        //    }
            
        //    return a;
        //}
        
        /// <summary>
        /// Obtener articulos
        /// </summary>   
        /// <param name="cb">criterio de busqueda</param>
        /// <param name="compania">compania a filtrar articulos</param>          
        /// <param name="grupoArticulo">grupo de articulo</param>        
        /// <param name="nivel">nivel de precio</param>
        /// <returns></returns>
        public static List<Articulo> ObtenerArticulos(CriterioBusquedaArticulo cb, string compania, string grupoArticulo, string nivel)
        {            
            return ObtenerArticulos(new FiltroArticulo[] { }, cb, compania, grupoArticulo, nivel);
        }
        
        /// <summary>
        /// Obtener articulos por aproximacion de criterio de busqueda
        /// </summary>
        /// <param name="filtro">Indicadores de filtros</param>/// 
        /// <param name="cb">criterio de busqueda</param>
        /// <param name="compania">compania a filtrar articulos</param>          
        /// <param name="grupoArticulo">grupo de articulo</param>        
        /// <param name="nivel">nivel de precio</param>
        /// <returns></returns>
        public static List<Articulo> ObtenerArticulos(FiltroArticulo[] c, CriterioBusquedaArticulo cb, string compania, string grupoArticulo, string nivel)
        {
            SQLiteDataReader reader = null;
            string sentencia = CrearSentencia(c, cb,false);
            SQLiteParameterList parametros = CrearParametros(c,cb,compania, grupoArticulo, nivel);
            List<Articulo> articulos = new List<Articulo>();
            articulos.Clear();

            try
            {
                Compania cia = Corporativo.Compania.Obtener(compania);
                reader = GestorDatos.EjecutarConsulta(sentencia, parametros);
                while (reader.Read())
                {
                    Articulo a = new Articulo();
                    a.codigo = reader.GetString(0);
                    a.descripcion = reader.GetString(1);

                    if (!reader.IsDBNull(2))
                        a.familia = reader.GetString(2);

                    if (!reader.IsDBNull(3))
                        a.clase = reader.GetString(3);

                    if (!reader.IsDBNull(4))
                        a.codigoBarras = reader.GetString(4);

                    a.grupoArticulo = reader.GetString(5);
                    a.unidadEmpaque = reader.GetDecimal(6);
                    a.tipoEmpaqueAlmacen = reader.GetString(7);
                    a.tipoEmpaqueDetalle = reader.GetString(8);
                    a.usaLotes = reader.GetString(9).Equals("S");
                    a.impuesto.Impuesto1 = reader.GetDecimal(10);
                    a.impuesto.Impuesto2 = reader.GetDecimal(11);
                    a.impuesto.Imp1AfectaDescto = cia.Impuesto1AfectaDescto;
                    a.costoLocal = reader.GetDecimal(12);
                    a.costoDolar = reader.GetDecimal(13);
                    a.precio.PorcentajeRecargo = reader.GetDecimal(14);
                    a.factorVenta = reader.GetDecimal(16);
                    a.CodigoEnvase = reader.GetString(17);
                    if (!string.IsNullOrEmpty(reader.GetString(18)))
                    {
                        a.TypeArticulo = a.DevolverTipoArticulo(reader.GetString(18));
                    }
                    a.Compania = compania;

                    articulos.Add(a);
                    //Agilizar consulta en codigo de barra
                    if (cb.Criterio == CriterioArticulo.Barras)
                        break;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error ejecutando consulta de artículo por criterio '" + cb.Criterio + "'. " + ex.Message);
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }
            return articulos;
        }

        /// <summary>
        /// Obtener articulos por aproximacion de criterio de busqueda
        /// </summary>
        /// <param name="filtro">Indicadores de filtros</param>/// 
        /// <param name="cb">criterio de busqueda</param>
        /// <param name="compania">compania a filtrar articulos</param>          
        /// <param name="grupoArticulo">grupo de articulo</param>        
        /// <param name="nivel">nivel de precio</param>
        /// <returns></returns>
        public static List<Articulo> ObtenerArticulosGarantia(FiltroArticulo[] c, CriterioBusquedaArticulo cb, string compania, string grupoArticulo, string nivel)
        {
            SQLiteDataReader reader = null;
            string sentencia = CrearSentenciaTomGarantia(c, cb);
            SQLiteParameterList parametros = CrearParametros(c, cb, compania, grupoArticulo, nivel);
            List<Articulo> articulos = new List<Articulo>();
            articulos.Clear();

            try
            {
                Compania cia = Corporativo.Compania.Obtener(compania);
                reader = GestorDatos.EjecutarConsulta(sentencia, parametros);
                while (reader.Read())
                {
                    Articulo a = new Articulo();
                    a.codigo = reader.GetString(0);
                    a.descripcion = reader.GetString(1);

                    if (!reader.IsDBNull(2))
                        a.familia = reader.GetString(2);

                    if (!reader.IsDBNull(3))
                        a.clase = reader.GetString(3);

                    if (!reader.IsDBNull(4))
                        a.codigoBarras = reader.GetString(4);

                    a.grupoArticulo = reader.GetString(5);
                    a.unidadEmpaque = reader.GetDecimal(6);
                    a.tipoEmpaqueAlmacen = reader.GetString(7);
                    a.tipoEmpaqueDetalle = reader.GetString(8);
                    a.usaLotes = reader.GetString(9).Equals("S");
                    a.impuesto.Impuesto1 = reader.GetDecimal(10);
                    a.impuesto.Impuesto2 = reader.GetDecimal(11);
                    a.impuesto.Imp1AfectaDescto = cia.Impuesto1AfectaDescto;
                    a.costoLocal = reader.GetDecimal(12);
                    a.costoDolar = reader.GetDecimal(13);
                    a.precio.PorcentajeRecargo = reader.GetDecimal(14);
                    a.factorVenta = reader.GetDecimal(16);
                    a.CodigoEnvase = reader.GetString(17);
                    if (!string.IsNullOrEmpty(reader.GetString(18)))
                    {
                        a.TypeArticulo = a.DevolverTipoArticulo(reader.GetString(18));
                    }
                    a.Compania = compania;

                    articulos.Add(a);
                    //Agilizar consulta en codigo de barra
                    if (cb.Criterio == CriterioArticulo.Barras)
                        break;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error ejecutando consulta de artículo por criterio '" + cb.Criterio + "'. " + ex.Message);
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }
            return articulos;
        }

        /// <summary>
        /// Obtiene todos los articulos en la cual la familia es nula o esta vacia        
        /// </summary>
        /// <param name="c"></param>
        /// <param name="cb"></param>
        /// <param name="compania"></param>
        /// <param name="grupoArticulo"></param>
        /// <param name="nivel"></param>
        /// <returns></returns>
        public static List<Articulo> ObtenerArticulosSinFamilia(FiltroArticulo[] c, CriterioBusquedaArticulo cb,string compania, string grupoArticulo, string nivel)
        {
            SQLiteDataReader reader = null;
            string where = string.Empty;
            string nuevowhere = string.Empty;
            string sentencia = CrearSentencia(c, cb,false,ref where);

            if (where.Contains("A.COD_FAM LIKE @FAMILIA"))
            {
                string pivot = "A.COD_FAM IS NULL OR" + where.Substring(5, where.Length - 57) + " AND A.COD_FAM = ''";
                nuevowhere = where.Replace("A.COD_FAM LIKE @FAMILIA", pivot);
            }
            else
            {
                string pivot = "A.COD_FAM IS NULL OR" + where.Substring(5, where.Length - 46) + "AND A.COD_FAM = ''";
                nuevowhere = where.Replace("A.COD_FAM LIKE @FAMILIA", pivot);
            }

            sentencia = sentencia.Replace(where, nuevowhere);
            //sentencia = sentencia.Replace(where, "(A.COD_FAM IS NULL OR COD_FAM = '')");

            SQLiteParameterList parametros = CrearParametros(c, cb, compania, grupoArticulo, nivel);
            List<Articulo> articulos = new List<Articulo>();
            articulos.Clear();

            try
            {
                Compania cia = Corporativo.Compania.Obtener(compania);

                reader = GestorDatos.EjecutarConsulta(sentencia, parametros);
                while (reader.Read())
                {
                    Articulo a = new Articulo();
                    a.codigo = reader.GetString(0);
                    a.descripcion = reader.GetString(1);

                    if (!reader.IsDBNull(2))
                        a.familia = reader.GetString(2);

                    if (!reader.IsDBNull(3))
                        a.clase = reader.GetString(3);

                    if (!reader.IsDBNull(4))
                        a.codigoBarras = reader.GetString(4);

                    a.grupoArticulo = reader.GetString(5);
                    a.unidadEmpaque = reader.GetDecimal(6);
                    a.tipoEmpaqueAlmacen = reader.GetString(7);
                    a.tipoEmpaqueDetalle = reader.GetString(8);
                    a.usaLotes = reader.GetString(9).Equals("S");
                    a.impuesto.Impuesto1 = reader.GetDecimal(10);
                    a.impuesto.Impuesto2 = reader.GetDecimal(11);
                    a.impuesto.Imp1AfectaDescto = cia.Impuesto1AfectaDescto;
                    a.costoLocal = reader.GetDecimal(12);
                    a.costoDolar = reader.GetDecimal(13);
                    a.precio.PorcentajeRecargo = reader.GetDecimal(14);
                    a.factorVenta = reader.GetDecimal(16);
                    a.CodigoEnvase = reader.GetString(17);
                    if (!string.IsNullOrEmpty(reader.GetString(18)))
                    {
                        a.TypeArticulo = a.DevolverTipoArticulo(reader.GetString(18));
                    }

                    a.Compania = compania;

                    articulos.Add(a);
                    //Agilizar consulta en codigo de barra
                    if (cb.Criterio == CriterioArticulo.Barras)
                        break;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error ejecutando consulta de artículo por criterio '" + cb.Criterio + "'. " + ex.Message);
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }
            return articulos;
        }

        /// <summary>
        /// Obtiene todos los articulos en la cual la familia es nula o esta vacia        
        /// </summary>
        /// <param name="c"></param>
        /// <param name="cb"></param>
        /// <param name="compania"></param>
        /// <param name="grupoArticulo"></param>
        /// <param name="nivel"></param>
        /// <returns></returns>
        public static List<Articulo> ObtenerArticulosSinFamiliaGarantia(FiltroArticulo[] c, CriterioBusquedaArticulo cb, string compania, string grupoArticulo, string nivel)
        {
            SQLiteDataReader reader = null;
            string where = string.Empty;
            string nuevowhere = string.Empty;
            string sentencia = CrearSentenciaTomaGarantia(c, cb,ref where);

            if (where.Contains("A.COD_FAM LIKE @FAMILIA"))
            {
                string pivot = "A.COD_FAM IS NULL OR" + where.Substring(5, where.Length - 57) + " AND A.COD_FAM = ''";
                nuevowhere = where.Replace("A.COD_FAM LIKE @FAMILIA", pivot);
            }
            else
            {
                string pivot = "A.COD_FAM IS NULL OR" + where.Substring(5, where.Length - 46) + "AND A.COD_FAM = ''";
                nuevowhere = where.Replace("A.COD_FAM LIKE @FAMILIA", pivot);
            }

            sentencia = sentencia.Replace(where, nuevowhere);
            //sentencia = sentencia.Replace(where, "(A.COD_FAM IS NULL OR COD_FAM = '')");

            SQLiteParameterList parametros = CrearParametros(c, cb, compania, grupoArticulo, nivel);
            List<Articulo> articulos = new List<Articulo>();
            articulos.Clear();

            try
            {
                Compania cia = Corporativo.Compania.Obtener(compania);

                reader = GestorDatos.EjecutarConsulta(sentencia, parametros);
                while (reader.Read())
                {
                    Articulo a = new Articulo();
                    a.codigo = reader.GetString(0);
                    a.descripcion = reader.GetString(1);

                    if (!reader.IsDBNull(2))
                        a.familia = reader.GetString(2);

                    if (!reader.IsDBNull(3))
                        a.clase = reader.GetString(3);

                    if (!reader.IsDBNull(4))
                        a.codigoBarras = reader.GetString(4);

                    a.grupoArticulo = reader.GetString(5);
                    a.unidadEmpaque = reader.GetDecimal(6);
                    a.tipoEmpaqueAlmacen = reader.GetString(7);
                    a.tipoEmpaqueDetalle = reader.GetString(8);
                    a.usaLotes = reader.GetString(9).Equals("S");
                    a.impuesto.Impuesto1 = reader.GetDecimal(10);
                    a.impuesto.Impuesto2 = reader.GetDecimal(11);
                    a.impuesto.Imp1AfectaDescto = cia.Impuesto1AfectaDescto;
                    a.costoLocal = reader.GetDecimal(12);
                    a.costoDolar = reader.GetDecimal(13);
                    a.precio.PorcentajeRecargo = reader.GetDecimal(14);
                    a.factorVenta = reader.GetDecimal(16);
                    a.CodigoEnvase = reader.GetString(17);
                    if (!string.IsNullOrEmpty(reader.GetString(18)))
                    {
                        a.TypeArticulo = a.DevolverTipoArticulo(reader.GetString(18));
                    }

                    a.Compania = compania;

                    articulos.Add(a);
                    //Agilizar consulta en codigo de barra
                    if (cb.Criterio == CriterioArticulo.Barras)
                        break;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error ejecutando consulta de artículo por criterio '" + cb.Criterio + "'. " + ex.Message);
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }
            return articulos;
        }
        
        /// <summary>
        /// Determinar si un articulo existe en el catalogo para aprobarlo en el sugerido
        /// </summary>
        /// <returns>existencia en catalogo</returns>
        public bool ExisteEnCatalogo()
        {
            string sentencia =
                " SELECT COUNT(COD_ART) FROM " + Table.ERPADMIN_ARTICULO + 
                " WHERE COD_ART = @ARTICULO " +
                " AND UPPER(COD_CIA) = @COMPANIA";

            SQLiteParameterList parametros = new SQLiteParameterList();
            parametros.Add("@ARTICULO",codigo);
            parametros.Add("@COMPANIA", compania.ToUpper());

            object valor; 

            try
            {
                valor = GestorDatos.EjecutarScalar(sentencia, parametros);
            }
            catch (Exception ex)
            {
                throw new Exception("Error validando existencia de artículo. " + ex.Message);
            }

            if (valor != DBNull.Value)
                if (Convert.ToInt32(valor) > 0)
                    return true;
            return false;        
        }

        /// <summary>
        /// Obtener el factor de empaque de un articulo, para restablecer el articulo en una consignacion
        /// </summary>
        /// <param name="articulo">codigo de articulo a buscar</param>
        /// <param name="compania">compania asociada</param>
        /// <returns>factor de empaque del articulo</returns>
        public static decimal ObtenerFactorEmpaque(string articulo, string compania)
        {
            string sentencia =
                " SELECT UND_EMP FROM " + Table.ERPADMIN_ARTICULO +
                " WHERE COD_ART = @ARTICULO " +
                " AND UPPER(COD_CIA) = @COMPANIA";

            SQLiteParameterList parametros = new SQLiteParameterList();
            parametros.Add("@ARTICULO",articulo);
            parametros.Add("@COMPANIA", compania.ToUpper());

            object valor;

            try
            {
                valor = GestorDatos.EjecutarScalar(sentencia, parametros);                
            }
            catch (Exception ex)
            {
                throw new Exception("Error obteniendo factor de empaque del artículo. " + ex.Message);
            }

            if (valor != DBNull.Value) //UNDONE Revisar los DBNull
                return Convert.ToDecimal(valor);
            else
                return 1;
        }
        
        /// <summary>
        /// Obtener una lista de articulos vendidos en ruta
        /// </summary>
        /// <returns></returns>
        private static List<Articulo> ObtenerArticulosVendidos()
        {
            List<Articulo> articulosVendidos = new List<Articulo>();
            articulosVendidos.Clear();

            SQLiteDataReader reader = null;

            //Sacamos los articulos vendidos en facturas
            string sentencia =
                " SELECT DISTINCT DP.COD_ART,DP.COD_CIA,EP.COD_BOD " +
                " FROM " + Table.ERPADMIN_alFAC_DET_PED  + " DP, " + Table.ERPADMIN_alFAC_ENC_PED + " EP " +
                " WHERE EP.TIP_DOC = @TIPO" +
                " AND EP.DOC_PRO IS NULL " +
                " AND EP.NUM_PED = DP.NUM_PED " +
                " AND EP.COD_CIA = DP.COD_CIA " +
                " AND EP.CONSIGNACION <> 'S'";
            
            SQLiteParameterList parametros = new SQLiteParameterList();
            parametros.Add("@TIPO",((char)TipoPedido.Factura).ToString());

            try
            {
                reader = GestorDatos.EjecutarConsulta(sentencia,parametros);

                while (reader.Read())
                {
                    Articulo a = new Articulo();
                    a.codigo = reader.GetString(0);
                    a.Compania = reader.GetString(1);
                    a.Bodega = new Bodega(reader.GetString(2));

                    articulosVendidos.Add(a);
                }

                return articulosVendidos;
            }
            catch (Exception ex)
            {
                throw new Exception("Error obteniendo los artículos vendidos. " + ex.Message);

            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }

            if (FRdConfig.UsaEnvases)
            {

                //Genera existencias de envases

                sentencia =
                    " SELECT DISTINCT DP.COD_ART,DP.COD_CIA,EP.COD_BOD " +
                    " FROM " + Table.ERPADMIN_alFAC_DET_GARANTIA + " DP, " + Table.ERPADMIN_alFAC_ENC_GARANTIA + " EP " +
                    " WHERE EP.DOC_PRO IS NULL " +
                    " AND EP.NUM_GAR = DP.NUM_GAR " +
                    " AND EP.COD_CIA = DP.COD_CIA ";
                try
                {
                    reader = GestorDatos.EjecutarConsulta(sentencia);

                    while (reader.Read())
                    {
                        Articulo a = new Articulo();
                        a.codigo = reader.GetString(0);
                        a.Compania = reader.GetString(1);
                        a.Bodega = new Bodega(reader.GetString(2));

                        articulosVendidos.Add(a);
                    }

                    return articulosVendidos;
                }
                catch (Exception ex)
                {
                    throw new Exception("Error obteniendo los artículos vendidos. " + ex.Message);

                }
                finally
                {
                    if (reader != null)
                        reader.Close();
                }
            }
            
        }

        /// <summary>
        /// Carga las familias
        /// </summary>
        /// <param name="famila"></param>
        /// <returns></returns>
        public static List<Articulo> CargarFamilias()
        {
            SQLiteDataReader reader = null;
            List<Articulo> articulosFamilias = new List<Articulo>();
            //string sentencia = " SELECT DISTINCT COD_FAM FROM " + Table.ERPADMIN_ARTICULO;

            // MejorasGrupoPelon600R6 - KF //
            string sentencia = "";

            if (Pedidos.DatoFamiliaMostrar.Equals(0))
            {
                sentencia = " SELECT DISTINCT COD_FAM FROM " + Table.ERPADMIN_ARTICULO;
            }
            else
            {
                sentencia = " SELECT DISTINCT cla.clasificacion,cla.descripcion FROM " + Table.ERPADMIN_ARTICULO +
                            " art, " + Table.ERPADMIN_CLASIFICACION_FR + " cla where art.COD_CIA = cla.compania and art.COD_FAM = cla.clasificacion";
            }
	
            try
            {
                reader = GestorDatos.EjecutarConsulta(sentencia);

                while (reader.Read())
                {
                    Articulo a = new Articulo();
                    if (reader.IsDBNull(0))
                        a.Familia = "SIN FAMILIA";
                    else
                    {
                        if (Pedidos.DatoFamiliaMostrar.Equals(0))
                        {
                            a.familia = reader.GetString(0);
                        }                        
                        else
                        {
                            a.familia = reader.GetString(0);
                            a.familiaDesc = reader.GetString(1);
                        }                            
                    }
                        
                    articulosFamilias.Add(a);

                    /*
                    Articulo a = new Articulo();
                    if (reader.IsDBNull(0))
                        a.Familia = "SIN FAMILIA";
                    else
                        a.familia = reader.GetString(0);
                    articulosFamilias.Add(a);
                     */
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error obteniendo las familias de los artículos. " + ex.Message);
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }
            return articulosFamilias;
        }

        public void CargarArticuloEnvase() 
        {
            try
            {
                string sql = string.Format("SELECT ARTICULO_ENVASE FROM " + Table.ERPADMIN_ARTICULO + " where COD_ART='{0}'", Codigo);
                CodigoEnvase = Convert.ToString( GestorDatos.EjecutarScalar(sql));
            }
            catch
            { 
                //Do Nothing no hay codigo envase
            }
            if (string.IsNullOrEmpty(CodigoEnvase))
            {
                return;
            }
            ArticuloEnvase = new Articulo(CodigoEnvase, Compania);
            bool existe=ArticuloEnvase.CargarDatosArticulo(CodigoEnvase,compania);
            if (!existe)
                ArticuloEnvase = null;
            if (ArticuloEnvase != null)
            {
                ArticuloEnvase.CargarEnvase();                
            }
        }

        /// <summary>
        /// Carga lotes asociado al articulo con existencias disponibles
        /// </summary>
        /// <returns></returns>
        public void CargarLotesArticulo(string bodega)
        {
            SQLiteDataReader reader = null;

            if (this.LotesAsociados != null)
                this.LotesAsociados.Clear();
            else
                this.LotesAsociados = new List<Lotes>();

            //Sacamos los articulos vendidos en facturas
            string sentencia =
                " SELECT LOCALIZACION, LOTE, FECHA_VENCIMIENTO, EXISTENCIA " +
                " FROM " + Table.ERPADMIN_ARTICULO_EXISTENCIA_LOTE +
                " WHERE COMPANIA = '" + compania + "' "+
                " AND BODEGA = '" + bodega + "' " +
                " AND ARTICULO = '" + codigo + "' " +
                " AND EXISTENCIA > 0 " +
                " ORDER BY FECHA_VENCIMIENTO, EXISTENCIA asc";

            try
            {
                reader = GestorDatos.EjecutarConsulta(sentencia);

                while (reader.Read())
                {
                    Lotes lote = new Lotes();

                    lote.Localizacion = reader.GetString(0);
                    lote.Lote = reader.GetString(1);
                    lote.FechaVencimiento = reader.GetDateTime(2);
                    lote.UnidadEmpaque = this.UnidadEmpaque;
                    lote.Existencia = reader.GetDecimal(3);                   

                    this.LotesAsociados.Add(lote);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error obteniendo los lotes asociados al artículo. " + ex.Message);
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }

        }

        #region MejorasFRTostadoraElDorado600R6 JEV
        
        public bool CargarDatosArticulo(string articulo, string compania)
        {
            bool procesoExitoso = true;
            string consulta = string.Empty;
            SQLiteDataReader reader = null;

            try
            {
                //Se crea la consulta

                consulta = String.Format("SELECT COD_ART, DES_ART, USA_LOTES FROM {0} WHERE (COD_ART = @ARTICULO OR COD_BAR = @ARTICULO ) AND UPPER(COD_CIA) = @COMPANIA", Table.ERPADMIN_ARTICULO);

                //Se crean los parámetros para la consulta
                SQLiteParameterList parametros = new SQLiteParameterList();
                parametros.Add("@ARTICULO", articulo);
                parametros.Add("@COMPANIA", compania.ToUpper());

                //Se ejecuta la consulta
                reader = GestorDatos.EjecutarConsulta(consulta, parametros);

                //Se obtienen los datos
                if (reader.Read())
                {
                    Codigo = reader.GetString(0);
                    Descripcion = reader.GetString(1);
                    UsaLotes = reader.GetString(2).Equals("S");
                }
                else
                {
                    //Mensaje.mostrarAlerta("El artículo no existe en la base de datos.");
                    procesoExitoso = false;
                }

            }
            catch (Exception ex)
            {
                string error = ex.Message;
                //throw new Exception("Error obteniendo los datos del artículo. " + ex.Message);
                procesoExitoso = false;
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                    reader = null;
                }
            }
            return procesoExitoso;
        }

        public bool CargarDatosArticuloTrasiego(string articulo, string compania,bool Lleno)
        {
            bool procesoExitoso = true;
            string consulta = string.Empty;
            SQLiteDataReader reader = null;

            try
            {
                //Se crea la consulta

                if (!Lleno)
                {
                    consulta = String.Format("SELECT COD_ART, DES_ART, USA_LOTES,ARTICULO_ENVASE,TIPO FROM {0} WHERE (COD_ART = @ARTICULO OR COD_BAR = @ARTICULO ) AND UPPER(COD_CIA) = @COMPANIA AND TIPO=@TIPO", Table.ERPADMIN_ARTICULO);
                }
                else
                {
                    consulta = String.Format("SELECT COD_ART, DES_ART, USA_LOTES,ARTICULO_ENVASE,TIPO FROM {0} WHERE (COD_ART = @ARTICULO OR COD_BAR = @ARTICULO ) AND UPPER(COD_CIA) = @COMPANIA AND TIPO!=@TIPO", Table.ERPADMIN_ARTICULO);
                }
                

                //Se crean los parámetros para la consulta
                SQLiteParameterList parametros = new SQLiteParameterList();
                parametros.Add("@ARTICULO", articulo);
                parametros.Add("@COMPANIA", compania.ToUpper());
                parametros.Add("@TIPO", ((char)TipoArticulo.ENVASE).ToString());

                //Se ejecuta la consulta
                reader = GestorDatos.EjecutarConsulta(consulta, parametros);

                //Se obtienen los datos
                if (reader.Read())
                {
                    Codigo = reader.GetString(0);
                    Descripcion = reader.GetString(1);
                    UsaLotes = reader.GetString(2).Equals("S");
                    CodigoEnvase = reader.GetString(3);
                    if (!string.IsNullOrEmpty(reader.GetString(4)))
                    {
                        TypeArticulo = DevolverTipoArticulo(reader.GetString(4));
                    }
                }
                else
                {
                    //Mensaje.mostrarAlerta("El artículo no existe en la base de datos.");
                    procesoExitoso = false;
                }

            }
            catch (Exception ex)
            {
                string error = ex.Message;
                //throw new Exception("Error obteniendo los datos del artículo. " + ex.Message);
                procesoExitoso = false;
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                    reader = null;
                }
            }
            return procesoExitoso;
        }

        private TipoArticulo DevolverTipoArticulo(string tipo)
        {
            switch (tipo)
            {
                case "N":return TipoArticulo.ENVASE;
                case "B":return TipoArticulo.SUBPRODUCTO;
                case "C":return TipoArticulo.COPRODUCTO;
                case "D":return TipoArticulo.DESECHO;
                case "E":return TipoArticulo.SEMIELABORADO;
                case "F":return TipoArticulo.FANTASMA;
                case "K":return TipoArticulo.KIT;
                case "L":return TipoArticulo.LABORAL;
                case "M":return TipoArticulo.MATERIA_PRIMA;
                case "O":return TipoArticulo.OTROS;
                case "P":return TipoArticulo.PROCESO;
                case "Q":return TipoArticulo.MATEMPAQUE;
                case "R":return TipoArticulo.REFACCION;
                case "S":return TipoArticulo.REPROCESO;
                case "T":return TipoArticulo.TERMINADO;
                case "U":return TipoArticulo.SUMINISTRO;
                case "V":return TipoArticulo.SERVICIO;
                default: return TipoArticulo.OTROS;
            } 
        }


        #endregion MejorasFRTostadoraElDorado600R6 JEV

        #region Reporte Liquidacion Inventario Gas Z

        public bool CargarReporteLiquidacion(string bodega)
        {
            bool result = true;
            SQLiteDataReader reader = null;

            //Se obtiene la existencia actual e inicial.
            string sentencia =
                " SELECT EXISTENCIA,INICIAL " +
                " FROM " + Table.ERPADMIN_ARTICULO_EXISTENCIA +
                " WHERE UPPER(COMPANIA) = '" + compania.ToUpper() + "' " +
                " AND BODEGA = '" + bodega + "' " +
                " AND ARTICULO = '" + codigo + "' ";
            try
            {
                reader = GestorDatos.EjecutarConsulta(sentencia);

                while (reader.Read())
                {
                    if (!string.IsNullOrEmpty(reader.GetString(0)))
                        this.cantDisponible = reader.GetDecimal(0);
                    if (!string.IsNullOrEmpty(reader.GetString(1)))
                        this.cantInicial = reader.GetDecimal(1);                   
                }
            }
            catch (Exception ex)
            {
                result = false;
                throw new Exception("Error obteniendo Reporte Liquidación asociado al artículo. " + ex.Message);
            }
            finally
            {
                if (reader != null)
                    reader.Close();
                reader = null;
            }

            //Se obtiene la cantidad vendida.
            sentencia =
                " SELECT SUM(CNT_MAX),SUM(CNT_MIN) " +
                " FROM " + Table.ERPADMIN_alFAC_DET_PED + " dp, " +
                Table.ERPADMIN_alFAC_ENC_PED + " ep " +
                " WHERE UPPER(ep.[COD_CIA]) = '" + compania.ToUpper() + "' " +
                " AND ep.TIP_DOC = 'F' " +
                " AND julianday(date(ep.[FEC_PED])) = julianday(date('now','localtime')) " +                
                " AND ep.[NUM_PED]=dp.[NUM_PED] AND dp.[COD_ART]='" + codigo + "'";
            try
            {
                reader = GestorDatos.EjecutarConsulta(sentencia);

                while (reader.Read())
                {
                    if (!string.IsNullOrEmpty(reader.GetString(0)))
                        this.cantVendidas = reader.GetDecimal(0);
                    if (!string.IsNullOrEmpty(reader.GetString(1)))
                        this.cantVendidasDet = reader.GetDecimal(1);
                }
            }
            catch (Exception ex)
            {
                result = false;
                throw new Exception("Error obteniendo Reporte Liquidación asociado al artículo. " + ex.Message);
            }
            finally
            {
                if (reader != null)
                    reader.Close();
                reader = null;
            }

            //Se obtiene la cantidad en garantía.
            sentencia =
                " SELECT SUM(CNT_MAX),SUM(CNT_MIN) " +
                " FROM " + Table.ERPADMIN_alFAC_DET_GARANTIA + " dp, " +
                Table.ERPADMIN_alFAC_ENC_GARANTIA + " ep " +
                " WHERE UPPER(ep.[COD_CIA]) = '" + compania.ToUpper() + "' " +
                " AND julianday(date(ep.[FEC_GAR])) = julianday(date('now','localtime')) " +
                " AND ep.[NUM_GAR]=dp.[NUM_GAR] AND dp.[COD_ART]='" + codigo + "'";
            try
            {
                reader = GestorDatos.EjecutarConsulta(sentencia);

                while (reader.Read())
                {
                    if (!string.IsNullOrEmpty(reader.GetString(0)))
                        this.cantGarantia = reader.GetDecimal(0);
                    if (!string.IsNullOrEmpty(reader.GetString(1)))
                        this.cantGarantiaDet = reader.GetDecimal(1);
                }
            }
            catch (Exception ex)
            {
                result = false;
                throw new Exception("Error obteniendo Reporte Liquidación asociado al artículo. " + ex.Message);
            }
            finally
            {
                if (reader != null)
                    reader.Close();
                reader = null;
            }

            //Se obtiene la cantidad Devuelta.
            sentencia =
                " SELECT SUM(CNT_MAX),SUM(CNT_MIN) " +
                " FROM " + Table.ERPADMIN_alFAC_DET_DEV + " dp, " +
                Table.ERPADMIN_alFAC_ENC_DEV + " ep " +
                " WHERE UPPER(ep.[COD_CIA]) = '" + compania.ToUpper() + "' " +
                " AND julianday(date(ep.[FEC_DEV])) = julianday(date('now','localtime')) " +
                " AND ep.[NUM_DEV]=dp.[NUM_DEV] AND dp.[COD_ART]='" + codigo + "'";
            try
            {
                reader = GestorDatos.EjecutarConsulta(sentencia);

                while (reader.Read())
                {
                    if (!string.IsNullOrEmpty(reader.GetString(0)))
                        this.cantDevueltos = reader.GetDecimal(0);
                    if (!string.IsNullOrEmpty(reader.GetString(1)))
                        this.cantDevueltosDet = reader.GetDecimal(1);
                }
            }
            catch (Exception ex)
            {
                result = false;
                throw new Exception("Error obteniendo Reporte Liquidación asociado al artículo. " + ex.Message);
            }
            finally
            {
                if (reader != null)
                    reader.Close();
                reader = null;
            }

            //Se obtienen los cilindros vacíos.

            sentencia =
                " SELECT EXISTENCIA,INICIAL " +
                " FROM " + Table.ERPADMIN_ARTICULO_EXISTENCIA +
                " WHERE UPPER(COMPANIA) = '" + compania.ToUpper() + "' " +
                " AND BODEGA = '" + bodega + "' " +
                " AND ARTICULO = '" + codigoEnvase + "' ";
            try
            {
                reader = GestorDatos.EjecutarConsulta(sentencia);

                while (reader.Read())
                {
                    if (!string.IsNullOrEmpty(reader.GetString(0)))
                        this.cantVacios = reader.GetDecimal(0);
                }
            }
            catch (Exception ex)
            {
                result = false;
                throw new Exception("Error obteniendo Reporte Liquidación asociado al artículo. " + ex.Message);
            }
            finally
            {
                if (reader != null)
                    reader.Close();
                reader = null;
            }
            //Se obtiene la cantidad Entrante.
            sentencia =
                " SELECT SUM(CANT_DIF) " +
                " FROM " + Table.ERPADMIN_TRASIEGO + " ts " +
                " WHERE UPPER(ts.[COMPANIA]) = '" + compania.ToUpper() + "' " +
                " AND julianday(date(ts.[FECHA])) = julianday(date('now','localtime')) " +
                " AND ts.[TIPO]='E' AND ts.[ARTICULO]='" + codigo + "'";
            try
            {
                reader = GestorDatos.EjecutarConsulta(sentencia);

                while (reader.Read())
                {
                    if (!string.IsNullOrEmpty(reader.GetString(0)))
                        this.cantEntradas = reader.GetDecimal(0);
                }
            }
            catch (Exception ex)
            {
                result = false;
                throw new Exception("Error obteniendo Reporte Liquidación asociado al artículo. " + ex.Message);
            }
            finally
            {
                if (reader != null)
                    reader.Close();
                reader = null;
            }
            //Se obtiene la cantidad Saliente.
            sentencia =
                " SELECT SUM(CANT_DIF) " +
                " FROM " + Table.ERPADMIN_TRASIEGO + " ts " +
                " WHERE UPPER(ts.[COMPANIA]) = '" + compania.ToUpper() + "' " +
                " AND julianday(date(ts.[FECHA])) = julianday(date('now','localtime')) " +
                " AND ts.[TIPO]='S' AND ts.[ARTICULO]='" + codigo + "'";
            try
            {
                reader = GestorDatos.EjecutarConsulta(sentencia);

                while (reader.Read())
                {
                    if (!string.IsNullOrEmpty(reader.GetString(0)))
                        this.cantSalidas = reader.GetDecimal(0);
                }
            }
            catch (Exception ex)
            {
                result = false;
                throw new Exception("Error obteniendo Reporte Liquidación asociado al artículo. " + ex.Message);
            }
            finally
            {
                if (reader != null)
                    reader.Close();
                reader = null;
            }
            return result;


        }

        #endregion

        #endregion

        #region BindData

        /// <summary>
        /// Ordenar itemes por criterio para el list view
        /// </summary>
        /// <param name="criterio">criterio de ordenacion</param>
        /// <returns>lista de itemes ordenados</returns>
        public string[] OrdenarItems(CriterioArticulo criterio)
        {
            string[] itemes = new string[3];
            switch (criterio)
            {
                case CriterioArticulo.Codigo:
                    itemes[0] = Codigo;
                    itemes[1] = Descripcion;
                    itemes[2] = CodigoBarras;
                    break;
                case CriterioArticulo.Barras:
                    itemes[0] = CodigoBarras;
                    itemes[1] = Descripcion;
                    itemes[2] = Codigo;
                    break;
                case CriterioArticulo.Descripcion:
                    itemes[0] = Descripcion;
                    itemes[1] = Codigo;
                    itemes[2] = CodigoBarras;
                    break;
                case CriterioArticulo.Clase:
                    itemes[0] = Clase;
                    itemes[1] = Descripcion;
                    itemes[2] = Codigo;
                    break;
                case CriterioArticulo.Familia:
                    itemes[0] = Familia;
                    itemes[1] = Descripcion;
                    itemes[2] = Codigo;
                    break;
                default:
                    itemes[0] = Codigo;
                    itemes[1] = Descripcion;
                    itemes[2] = CodigoBarras;
                    break;
            }
            return itemes;
        }
       
        #endregion

    }

    public enum TipoArticulo
    {
        ENVASE='N',
        SUBPRODUCTO='B',
        COPRODUCTO='C',
        DESECHO='D',
        SEMIELABORADO='E',
        FANTASMA='F',
        KIT='K',
        LABORAL='L',
        MATERIA_PRIMA='M',
        OTROS='O',
        PROCESO='P',
        MATEMPAQUE='Q',
        REFACCION ='R',
        REPROCESO='S',
        TERMINADO='T',
        SUMINISTRO='U',
        SERVICIO='V'
    }
}
