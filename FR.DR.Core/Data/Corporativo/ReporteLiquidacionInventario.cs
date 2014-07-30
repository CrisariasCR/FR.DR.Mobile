using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using EMF.Printing;
using Softland.ERP.FR.Mobile.Cls.Reporte;

namespace Softland.ERP.FR.Mobile.Cls.Corporativo
{
    public class ReporteLiquidacionInventario : IPrintable 
    {

        private JornadaRuta jornada;

        #region Constantes 
        
        public const string ARTICULO = "ARTICULO";
        public const string INVENTARIOINICIAL = "INVENTARIOINICIAL";
        public const string CANTDISPONIBLE = "CANTDISPONIBLE";
        public const string FECHA = "FECHA";
        public const string BODEGA = "BODEGA";
        public const string DEVOLUCIONES = "DEVOLUCIONES";
        public const string DEVOLUCIONESDET = "DEVOLUCIONESDET";
        public const string UNDVENDIDAS = "UNDVENDIDAS";
        public const string UNDVENDIDASDET = "UNDVENDIDASDET";
        public const string DEPGARANTIAS = "DEPGARANTIAS";
        public const string DEPGARANTIASDET = "DEPGARANTIASDET";
        public const string VACIOS = "VACIOS";
        public const string ENTRADAS = "ENTRADAS";
        public const string SALIDAS = "SALIDAS";

        public const string SINCRONIZADO = "SINCRONIZADO";

        public const string JORNADA_RUTAS_SERVIDOR = "ERPADMIN.JORNADA_RUTAS";

        /// <summary>
        /// Constante para separar las sentencias de un conjunto de sentencias a ejecutar.
        /// </summary>
        private const string SEPARADOR = "#|#";

        #endregion

        #region Propiedades

        private string bodega;
        public string Bodega
        {
            get { return bodega; }
            set { bodega = value; }
        }

        private string articulo;
        public string Articulo
        {
            get { return articulo; }
            set { articulo = value; }
        }

        private decimal? inicial;
        public decimal? Inicial
        {
            get { return inicial; }
            set { inicial = value; }
        }

        private decimal? disponible;
        public decimal? Disponible
        {
            get { return disponible; }
            set { disponible = value; }
        }

        private decimal? vendidas;
        public decimal? Vendidas
        {
            get { return vendidas; }
            set { vendidas = value; }
        }

        private decimal? vendidasDet;
        public decimal? VendidasDet
        {
            get { return vendidasDet; }
            set { vendidasDet = value; }
        }

        private decimal? devoluciones;
        public decimal? Devoluciones
        {
            get { return devoluciones; }
            set { devoluciones = value; }
        }

        private decimal? devolucionesDet;
        public decimal? DevolucionesDet
        {
            get { return devolucionesDet; }
            set { devolucionesDet = value; }
        }

        private decimal? garantias;
        public decimal? Garantias
        {
            get { return garantias; }
            set { garantias = value; }
        }
        private decimal? garantiasDet;
        public decimal? GarantiasDet
        {
            get { return garantiasDet; }
            set { garantiasDet = value; }
        }

        private decimal? vacias;
        public decimal? Vacias
        {
            get { return vacias; }
            set { vacias = value; }
        }

        private decimal? entradas;
        public decimal? Entradas
        {
            get { return entradas; }
            set { entradas = value; }
        }

        private decimal? salidas;
        public decimal? Salidas
        {
            get { return salidas; }
            set { salidas = value; }
        }

        #endregion

        #region Constructores

        public ReporteLiquidacionInventario(decimal pInicial, decimal pDisponible, decimal pgarantia, decimal pgarantiaDet, decimal pdevueltos, decimal pdevueltosDet, decimal pvacios, decimal pVendidos, decimal pVendidosDet,decimal pEntradas,decimal pSalidas,string pBodega, string pArticulo)
        {
            Inicial = pInicial;
            Disponible=pDisponible;
            Devoluciones = pdevueltos;
            DevolucionesDet = pdevueltosDet;
            Garantias = pgarantia;
            GarantiasDet = pgarantiaDet;
            Vacias = pvacios;
            Vendidas = pVendidos;
            VendidasDet = pVendidosDet;
            Entradas = pEntradas;
            Salidas = pSalidas;
            Bodega = pBodega;
            Articulo = pArticulo;
        }

        /// <summary>
        /// Constructor definir clase de impresion Jornada
        /// </summary>
        /// <param name="ruta">Ruta a imprimir reporte</param>
        public ReporteLiquidacionInventario(JornadaRuta jornadaRuta)
        {
            jornada = jornadaRuta;
        }

        #endregion Constructores

        #region Metodos de Clase

        public void Imprime()
        {
            Report reporteLiqInventario = new Report(ReportHelper.CrearRutaReporte(Rdl.ReporteLiquidacionInventario), Impresora.ObtenerDriver());

            reporteLiqInventario.AddObject(this);
            //do
            {
                reporteLiqInventario.Print();
                if (reporteLiqInventario.ErrorLog != string.Empty)
                {
                    // TODO: hay que tener un ViewModel o View para poder desplegar mensajes
                    //Mensaje.mostrarAlerta("Ocurrió un error durante la impresión del reporte: " + reporteJornada.ErrorLog);
                }

            } 
            // TODO: hay que tener un ViewModel o View para poder desplegar mensajes
            //while (Mensaje.mostrarMensaje(Mensaje.Accion.Decision, "imprimir de nuevo") == System.Windows.Forms.DialogResult.Yes);
        }

        #endregion Metodos de Clase

        #region IPrintable Members

        public object GetField(string name)
        {
            switch (name)
            {
                case BODEGA: return Bodega;
                case ARTICULO: return Articulo;
                case FECHA: return DateTime.Now;                

                case INVENTARIOINICIAL: return Inicial ?? decimal.Zero;
                case CANTDISPONIBLE: return Disponible ?? decimal.Zero;
                case DEVOLUCIONES: return Devoluciones ?? decimal.Zero;
                case DEVOLUCIONESDET: return DevolucionesDet ?? decimal.Zero;
                case UNDVENDIDAS: return Vendidas ?? decimal.Zero;
                case UNDVENDIDASDET: return vendidasDet ?? decimal.Zero;
                case DEPGARANTIAS: return Garantias ?? decimal.Zero;
                case DEPGARANTIASDET: return GarantiasDet ?? decimal.Zero;
                case VACIOS: return Vacias ?? decimal.Zero;
                case ENTRADAS: return Entradas ?? decimal.Zero;
                case SALIDAS: return Salidas ?? decimal.Zero;       

                default: return null;
            }
        }

        public string GetObjectName()
        {
            return "LIQUIDACIONES";
        }

        #endregion IPrintable Members
    }
}
