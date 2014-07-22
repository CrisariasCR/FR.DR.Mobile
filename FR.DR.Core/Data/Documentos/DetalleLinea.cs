using Softland.ERP.FR.Mobile.Cls.FRArticulo;
using Softland.ERP.FR.Mobile.Cls.FRCliente;
using EMF.Printing;
using System;
using FR.DR.Core.Data.Documentos.Retenciones;
using System.Text;
using Softland.ERP.FR.Mobile.Cls;
using Softland.ERP.FR.Mobile.Cls.AccesoDatos;
using System.Data.SQLiteBase;

namespace Softland.ERP.FR.Mobile.Cls.Documentos
{
    /// <summary>
    /// Detalle de una linea base de documento con precios, impuestos y descuentos
    /// </summary>
    public abstract class DetalleLinea : DetalleDocumento,IPrintable
    {
        #region Retenciones

        fclsRetencion retencionAsociada;
        /// <summary>
        /// Retención asociada a la línea.
        /// </summary>
        public fclsRetencion iRetencionAsociada
        {
            get { return retencionAsociada; }
            set { retencionAsociada = value; }
        }

        decimal montoRetencion;
        /// <summary>
        /// Monto Retención asociado a la línea.
        /// </summary>
        public decimal inMontoRetencion
        {
            get { return montoRetencion; }
            set { montoRetencion = value; }
        }



        #endregion Retenciones

        #region Precios

        private decimal montoTotal = 0;
        /// <summary>
        /// Monto total Bruto de la línea (sin impuestos ni descuentos).
        /// </summary>
        public decimal MontoTotal
        {
            get { return montoTotal; }
            set { montoTotal = value; }
        }

        /// <summary>
        /// Monto neto de la linea
        /// </summary>
        public decimal MontoNeto
        {
            get { return montoTotal + impuesto.MontoTotal - montoDescuento;  }
        }

        private Precio precio;
        /// <summary>
        /// Contiene precio almacen y detalle de la línea
        /// </summary>
        public Precio Precio
        {
            get { return precio; }
            set { precio = value; }
        }

        #endregion

        #region Impuestos

        private Impuesto impuesto = new Impuesto();
        /// <summary>
        /// Contiene impuestos de la línea
        /// </summary>
        public Impuesto Impuesto
        {
            get { return impuesto; }
            set { impuesto = value; }
        }
        #endregion

        #region Descuento

        private decimal montoDescuento = 0;
        /// <summary>
        /// Monto de descuento de la linea.
        /// </summary>
        public decimal MontoDescuento
        {
            get { return montoDescuento; }
            set { montoDescuento = value; }
        }

        private decimal porcentajeDescuento = 0;
        /// <summary>
        /// Porcentaje de descuento de la linea.
        /// </summary>
        public decimal PorcentajeDescuento
        {
            get { return porcentajeDescuento; }
            set { porcentajeDescuento = value; }
        }

        private Descuento descuento;
        /// <summary>
        /// Contiene detalle de Descuento de la línea.
        /// </summary>
        public Descuento Descuento
        {
            get { return descuento; }
            set { descuento = value; }
        }

        #endregion

        #region Constructor

        public DetalleLinea()
        { }
        #endregion

		#region Metodos de instancia

        /// <summary>
        /// Recalcula el monto de la línea y el monto del descuento.
        /// </summary>
        /// <param name="cliente">Datos del cliente sobre el que se realiza el calculo</param>
        /// <param name="descuento">Indica si carga el descuento de linea</param>
        public void CalcularMontos(ClienteCia cliente, bool descuento)
        {
            //Calculamos el monto de la linea
            decimal montoAlmacen = this.UnidadesAlmacen * this.Articulo.Precio.Maximo;
            decimal montoDetalle = this.UnidadesDetalle * this.Articulo.Precio.Minimo;
            this.MontoTotal = montoAlmacen + montoDetalle;

            if(descuento)
                this.Descuento = Descuento.ObtenerDescuento(cliente, Articulo, UnidadesAlmacen);

            //Recalculamos el monto de descuento de la línea.
            CalcularMontoDescuento();

            if (descuento)
            {
                this.Articulo.CargarImpuesto();
                //Calculamos el impuesto de venta.
                this.Impuesto.ImpuestoVentaLinea(this.MontoTotal, this.MontoDescuento, cliente.Descuento / 100, cliente.ExoneracionImp1);

                //Calculamos el impuesto de consumo
                this.Impuesto.ImpuestoConsumoLinea(this.MontoTotal, this.MontoDescuento, cliente.Descuento / 100, cliente.ExoneracionImp2);
            }
        }

        /// <summary>
        /// Recalcula el monto de la línea y el monto del descuento.
        /// </summary>
        /// <param name="cliente">Datos del cliente sobre el que se realiza el calculo</param>
        public void CalcularMontos(ClienteCia cliente)
        {            
            this.CalcularMontos(cliente, true);
        }

        /// <summary>
        /// Recalcula el monto de la línea y el monto del descuento.
        /// </summary>
        public void CalcularMontos()
        {
            this.CalcularMontos(null, false);
        }

        /// <summary>
        /// Verifica si el articulo tiene asociado una retencion y calcula el monto de retencion si hay. Requiere que la instancia este cargada para conocer el codigo del articulo y lo montos.
        /// </summary>
        /* Separacion=ND*/
        public bool CalcularRetencion(string psPaisCli, string psDivGeo1, string psDivGeo2, string psRegimen,string sPaisInstalado,string pCompania)
        {
            fclsRetencion fciRetencion;
            SQLiteDataReader reader = null;
            StringBuilder sSqlCmd=new StringBuilder();
            //SalDecimal nInd;
            bool lbOk=true;
            string lsCodigoRetencion=string.Empty;
            string lsDescripcionRetencion=string.Empty;
            decimal lnMontoRetencion=0;
            decimal lnBaseRetencion=0;
            decimal lnMonto;

            // Inicializamos la instancia para indicar que no tiene retencion asociada.
            if (iRetencionAsociada==null)
                iRetencionAsociada = new fclsRetencion(pCompania);
            iRetencionAsociada.sRetencion = null;
            iRetencionAsociada.nMonto = 0;
            inMontoRetencion = 0;
            fciRetencion = new fclsRetencion(pCompania);
            // MRL Mejoras Colombia Entrega 2, se agrega el manejo de retenciones por artículo -->
            /*
                            // Set sSqlCmd = " SELECT ret.codigo_retencion, ret.descripcion FROM " || ConcatCompany( 'retenciones ret, ' ) || ConcatCompany( 'articulo art ' ) || " WHERE art.articulo = :sArticulo AND ret.estado = '" || CC_RET_ACTIVA || "' AND ret.ret_por_articulo = '" || FA_SI || "' AND art.clasificacion_1 = ret.clasificacion_art INTO :lsCodigoRetencion, :lsDescripcionRetencion"
            */
            // MRL Proyecto Colombia R4 --> Se cambia codigo_retencion por retencion_venta
            sSqlCmd.AppendLine(string.Format("SELECT ret.codigo_retencion, ret.descripcion FROM {0} ret,{1} art ", Table.ERPADMIN_RETENCIONES,Table.ERPADMIN_ARTICULO));
            sSqlCmd.AppendLine(string.Format(" WHERE art.cod_art = '{0}' AND ret.estado = '{1}'", Articulo.Codigo, fclsRetencion.CC_RET_ACTIVA));
            sSqlCmd.AppendLine(" AND art.retencion_venta = ret.codigo_retencion");
            //sSqlCmd = (((((" SELECT ret.codigo_retencion, ret.descripcion FROM " + ConcatCompany("retenciones ret, ")) + ConcatCompany("articulo art ")) +
            //                " WHERE art.articulo = :sArticulo AND ret.estado = '") +
            //                CC_RET_ACTIVA) +
            //                "' AND art.retencion_venta = ret.codigo_retencion INTO :lsCodigoRetencion, :lsDescripcionRetencion");
            // MRL Proyecto Colombia R4 <--
            // MRL Mejoras Colombia Entrega 2 <--
            try
            {
                //lbOk = SqlPrepareAndExecute(hsqlPrmQuery, sSqlCmd, this, local);
                reader = GestorDatos.EjecutarConsulta(sSqlCmd.ToString());
                // NOTA: Por requerimiento de colombia, los articulos pueden tener 0 o 1 retencion asociada.
                if (lbOk)
                {
                    if (reader.Read())
                    {
                        lsCodigoRetencion = reader.GetString(0);
                        lsDescripcionRetencion = reader.GetString(1);
                        
                        fciRetencion.ReadOnKey(lsCodigoRetencion);
                        
                        lnMonto = ((MontoTotal + Impuesto.MontoImpuesto1) + Impuesto.MontoImpuesto2);
                        // MRL Proyecto Colombia R4 -->
                        // Se envía la ciudad asociada al documento y el régimen del cliente
                        
                        fciRetencion.SetDatosCalcularMonto(0, psPaisCli, psDivGeo1, psDivGeo2, psRegimen);
                        
                        // MRL Proyecto Colombia R4 <--
                        // CO4-01749-V3BC lgonzalez ini --->
                        /*
                                        // Set lnMontoRetencion = fciRetencion.CalcularMonto ( lnMonto, nPrecioTotal, nDescuentoLineaTotal, nImpuesto1Total, nImpuesto2Total, 0, 0, 1, lnBaseRetencion )
                        */
                        // Se pasa por parametro el descuento general ya que el descuento de la linea ya la posee la variable nPrecioTotal
                        lnMontoRetencion = fciRetencion.CalcularMonto(lnMonto, MontoTotal, MontoDescuento, Impuesto.MontoImpuesto1, Impuesto.MontoImpuesto2, 0, 0, 1,ref lnBaseRetencion,sPaisInstalado);
                        
                        // CO4-01749-V3BC lgonzalez fin <---
                        if ((lnMontoRetencion > 0))
                        {
                            iRetencionAsociada.sCodigoRetencion = lsCodigoRetencion;
                            iRetencionAsociada.sRetencion = lsDescripcionRetencion;
                            iRetencionAsociada.nMonto = lnMontoRetencion;
                            iRetencionAsociada.nBaseGravada = lnBaseRetencion;
                            iRetencionAsociada.sAutoretenedora = fciRetencion.sEsAutoretenedor;
                            if ((fciRetencion.sTipo == fclsRetencion.CP_RET_REGISTRO))
                            {
                                iRetencionAsociada.sEstado = fclsRetencion.CC_RET_APLICADA;
                            }
                            else
                            {
                                iRetencionAsociada.sEstado = fclsRetencion.CC_RET_PENDIENTE;
                            }
                            inMontoRetencion = lnMontoRetencion;
                        }
                    }
                }                
            }
            catch (Exception ex)
            {
                throw ex;
                //fciReporteErrores.AgregueError(0, "fcDocumento.ObtenerRetenciones", "Error verificando las retenciones asociadas al artículo.");
                //if (!(bDebugging))
                //{
                //    return false;
                //}                
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                    reader = null;
                }
            }
            //fciRetencion.ReleInstance();
            return lbOk;
        }

        /// <summary>
        /// Calcula el porcentaje de descuento aplicado a la linea.
        /// </summary>
        /// <returns></returns>
        public decimal CalcularPorcentajeDescuento()
        {
            //decimal porcentajeDescuento = 0;

            if (this.Descuento != null)
            {
                if (this.Descuento.Tipo == TipoDescuento.Porcentual)
                    porcentajeDescuento = this.Descuento.Monto;
                else
                {
                    try
                    {
                        porcentajeDescuento = (this.Descuento.Monto * 100) / this.MontoTotal;
                    }
                    catch (Exception) { porcentajeDescuento = 0; }
                }
            }
            return porcentajeDescuento;
        }

        /// <summary>
        /// Calcula el monto de descuento aplicado a la linea.
        /// </summary>
        /// <returns></returns>
        public decimal CalcularMontoDescuento()
        {
            if (this.Descuento != null)
            {
                if (this.Descuento.Tipo == TipoDescuento.Porcentual)
                    //Recalculamos el monto de descuento de la línea.
                    montoDescuento = montoTotal * Descuento.Monto / 100;
                else
                    montoDescuento = Descuento.Monto;
            }
            return montoDescuento;
        }

        /// <summary>
        /// Metodo que calcula el monto de la linea
        /// Es importante recalcar que por linea no se incluye ningun tipo de
        /// impuestos ni descuentos.
        /// </summary>
        public void CalcularMontoLinea()
        {
            //Caso 28086 LDS 04/05/2007
            //Se cambia porque se puede cambiar el precio durante la toma del pedido o factura.	
		    //TODO: Redondear decimal en el cálculo de los montos segun el parámetro de FA
            decimal montoAlmacen = UnidadesAlmacen * Precio.Maximo;
            decimal montoDetalle = UnidadesDetalle * Precio.Minimo;
            this.MontoTotal = montoAlmacen + montoDetalle;
        }
        /// <summary>
        /// Aumentar la existencia del articulo asociado a la linea
        /// </summary>
        public void AumentarExistencia()
        {
            Articulo.ActualizarBodega(TotalAlmacen);
        }

        /// <summary>
        /// Aumentar la existencia del articulo envase asociado a la linea
        /// </summary>
        public void AumentarExistenciaEnvase(string pBodega)
        {
            Articulo.CargarArticuloEnvase();
            if (Articulo.ArticuloEnvase != null)
            {
                Articulo.ArticuloEnvase.Bodega = new Bodega(Articulo.ArticuloEnvase, pBodega);
                Articulo.ArticuloEnvase.ActualizarBodega(TotalAlmacen);
            }
        }

        /// <summary>
        /// Disminuir la existencia del articulo asociado a la linea
        /// </summary>
        public void DisminuirExistencia()
        {
            Articulo.ActualizarBodega(TotalAlmacen * -1);
        }

        /// <summary>
        /// Disminuir la existencia del articulo asociado a la linea
        /// </summary>
        public void DisminuirExistenciaEnvase(string pBodega)
        {
            Articulo.CargarArticuloEnvase();
            if (Articulo.ArticuloEnvase != null)
            {
                Articulo.ArticuloEnvase.Bodega = new Bodega(Articulo.ArticuloEnvase, pBodega);
                Articulo.ArticuloEnvase.ActualizarBodega(TotalAlmacen * -1);
            }
        }


        #endregion

        #region IPrintable Members

        public override string GetObjectName()
        {
            return "DETALLE_LINEA";
        }

        public override object GetField(string name)
        {
            switch (name)
            {
                //Caso: 32679 ABC 20/05/2008 Carga constante del Reporte de Inventario
                case "PRECIO_UNITARIO": return this.Precio.Minimo;
                case "PRECIO_ALMACEN": return this.Precio.Maximo;
                case "MONTO": return this.montoTotal;

                //Caso 28803 LDS 14/09/2007
                case "DESCUENTO_MONTO": return this.MontoDescuento;
                case "DESCUENTO_PORCENTAJE": return this.CalcularPorcentajeDescuento();
                default:
                    return base.GetField(name);
            }
        }
        #endregion
    }
}
