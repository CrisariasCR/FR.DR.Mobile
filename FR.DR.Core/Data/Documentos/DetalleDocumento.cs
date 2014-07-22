using EMF.Printing;
using Softland.ERP.FR.Mobile.Cls.FRArticulo;
using System.Collections.Generic;
using System;

namespace Softland.ERP.FR.Mobile.Cls.Documentos
{
    /// <summary>
    /// Detalle Base de una linea
    /// </summary>
    public class DetalleDocumento : IPrintable
    {
        public DetalleDocumento()
        {
            this.Articulo = new Articulo();
        }

        #region Variables y Propiedades de instancia

        private int numeroLinea = 0;
        /// <summary>
        /// Numero de la linea de un documento
        /// </summary>
        public int NumeroLinea
        {
            get { return numeroLinea; }
            set { numeroLinea = value; }
        }
        
        private Articulo articulo;
        /// <summary>
        /// Articulo asociado a la linea
        /// </summary>
        public Articulo Articulo
        {
          get { return articulo; }
          set { articulo = value; }
        }

        #region Unidades

        /// <summary>
        /// Total en Unidades de Almacen de la línea
        /// </summary>
        public decimal TotalAlmacen
        {
            get
            {
                return UnidadesAlmacen
                    + (UnidadesDetalle / Articulo.UnidadEmpaque);
            }
        }

        private decimal unidadesAlmacen;
        /// <summary>
        /// Cantidad en unidades de almacen del articulo.
        /// </summary>
        public decimal UnidadesAlmacen
        {
            get { return unidadesAlmacen; }
            set { unidadesAlmacen = value; }
        }

        private decimal unidadesDetalle;
        /// <summary>
        /// Cantidad en unidades de detalle del articulo.
        /// </summary>
        public decimal UnidadesDetalle
        {
            get { return unidadesDetalle; }
            set { unidadesDetalle = value; }
        }

        #endregion

        #endregion


        #region Logica

        /// <summary>
        /// Cargar un detalle sugerido al pedido
        /// </summary>
        /// <param name="facturado">Indica si el pedido es facturado</param>
        /// <param name="config">Configuracion de la venta</param>
        /// <returns>agrega exitosamente</returns>
        public bool CargarDetalleAlPedido(bool facturado, ConfigDocCia config, string bodega)
        {

            //validar que el articulo incluido en las lineas del 
            //sugerido exista dentro del catalogo de articulos de la ruta
            if (!Articulo.ExisteEnCatalogo())
                return false;

            try
            {
                if (facturado)
                {
                    try
                    {
                        Articulo.Cargar(new FiltroArticulo[]
                                                {FiltroArticulo.NivelPrecio,
                                                 FiltroArticulo.Existencia}, config.Nivel.Nivel);
                        
                        Articulo.CargarExistencia(bodega);
                        if (!Articulo.Bodega.SuficienteExistencia(unidadesAlmacen))
                        {
                            this.unidadesAlmacen = Articulo.Bodega.Existencia;
                        }
                    }
                    catch (Exception ex) 
                    {
                        //No se encuentra existencias para el articulo
                        if (ex.Message.Equals("Artículo no se encontró"))
                            return false;
                    }
                }
                else
                    Articulo.Cargar(new FiltroArticulo[] { FiltroArticulo.NivelPrecio }, config.Nivel.Nivel);

                Articulo.CargarPrecio(config.Nivel);

                if (Articulo.Bodega != null && articulo.Bodega.Existencia == decimal.MinValue)
                    Articulo.Bodega = new Bodega(Articulo, bodega);
            }
            catch (Exception)
            {
                throw new Exception("Error cargando información del artículo sugerido.");
            }
            return true;
        }

        #endregion

        #region IPrintable Members

        public virtual string GetObjectName() 
        {
            return "DETALLE_DOCUMENTO";
        }

        public virtual object GetField(string name)
        {
            switch (name)
            {
                case "CODIGO": return this.Articulo.Codigo;
                case "ARTICULO": return this.Articulo.Codigo;
                    
                case "DESCRIPCION": return this.Articulo.Descripcion;

                case "ALMACEN":
                    if (this.UnidadesAlmacen > Decimal.Zero)
                        return this.UnidadesAlmacen;
                    else
                        return 0;

                case "DETALLE":
                    if (this.UnidadesDetalle > Decimal.Zero)
                        return this.UnidadesDetalle;
                    else
                        return 0;

                default: return string.Empty;

            }
        }
        #endregion
    }
}
