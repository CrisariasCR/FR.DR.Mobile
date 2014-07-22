using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Softland.ERP.FR.Mobile.Cls.FRArticulo;
using Softland.ERP.FR.Mobile.Cls.Corporativo;
using Softland.ERP.FR.Mobile.Cls.FRCliente;
using EMF.Printing;
using System.Collections;
using Softland.ERP.FR.Mobile.Cls.Reporte;

namespace Softland.ERP.FR.Mobile.Cls.Documentos.FRConsignacion
{
    /// <summary>
    /// Representa la Gestion de las ventas de consignacion para un cliente Multi cia
    /// </summary>
    public class VentasConsignacion : Gestor, IPrintable
    {
        #region Constructores
        public VentasConsignacion()
        { }
        /// <summary>
        /// Crea una nueva lista de ventas en consignación que se pretenden imprimir.
        /// Utilizado cuando se van a imprimir solamente los encabezados de las ventas en consignación
        /// realizadas a un cliente determinado.
        /// </summary>
        /// <param name="gestionados">Corresponde a las ventas en consignación que se desean imprimir.</param>
        /// <param name="cliente">Cliente al cual pertenecen las ventas en consignación.</param>
        public VentasConsignacion(List<VentaConsignacion> gestionados, Cliente cliente)
        {
            this.gestionados = gestionados;
            this.Cliente = cliente;
        }
        #endregion

        #region Lista de Consignaciones

        private List<VentaConsignacion> gestionados = new List<VentaConsignacion>();
        /// <summary>
        /// Contiene las ventas por compañía que se estan gestionando.
        /// </summary>
        public List<VentaConsignacion> Gestionados
        {
            get { return gestionados; }
            set { gestionados = value; }
        }

        #endregion
        
        #region Logica Negocios

        /// <summary>
        /// Existen articulos
        /// </summary>
        /// <returns>existencia de articulos en la gestion</returns>
        public bool ExistenArticulosGestionados()
        {
            foreach (VentaConsignacion venta in gestionados)
            {
                if (!venta.Detalles.Vacio())
                    return true;
            }
            return false;
        }
        /// <summary>
        /// Guarda en la base de datos la información de las ventas en consignación  gestionadas.
        /// </summary>
        public void Guardar()
        {
            foreach (VentaConsignacion ventaConsignacion in gestionados)
                try
                {
                    ventaConsignacion.Guardar(true);
                }
                catch (Exception ex)
                {
                    throw new Exception("Error guardando venta en consignación de la compañía '" + ventaConsignacion.Compania + "'. " + ex.Message);
                }
        }

        /// <summary>
        /// Asigna el descuento 1  y el descuento 2 a cada venta en consignación de cada compañía.
        /// </summary>
        /// <param name="nombreDescuento">Número del descuento a modificar.</param>
        /// <param name="porcentajeDesc">Porcentaje definido para el descuento a modificar.</param>
        /// <returns>Retorna el monto de descuento generado por la suma de ambos descuentos.</returns>
        public decimal DefinirDescuento(EDescuento nombreDescuento, decimal porcentajeDesc)
        {
            decimal montoPorcentajeDescTotal = 0;

            foreach (VentaConsignacion ventaConsignacion in gestionados)
                if (nombreDescuento == EDescuento.DESC1)
                {
                    ventaConsignacion.PorcentajeDescuento1 = porcentajeDesc;
                    montoPorcentajeDescTotal += ventaConsignacion.MontoDescuento1;
                }
                else
                {
                    ventaConsignacion.PorcentajeDescuento2 = porcentajeDesc;
                    montoPorcentajeDescTotal += ventaConsignacion.MontoDescuento2;
                }

            return montoPorcentajeDescTotal;
        }
        /// <summary>
        /// Obtiene las compañías de las ventas en consignación que están siendo gestionadas.
        /// </summary>
        /// <returns>Retorna las compañía de las ventas en consignación.</returns>
        public List<Compania> ObtenerCompanias()
        {
            List<Compania> companias = new List<Compania>();
            companias.Clear();
            foreach (VentaConsignacion venta in gestionados)
            {
                //if (pedido.Detalles.Count > 0)
                companias.Add(venta.Configuracion.Compania);
            }
            return companias;
        }

        /// <summary>
        /// Elimina el detalle de la venta en consignación gestionada.
        /// </summary>
        /// <param name="articulo">Artículo a quitar de la venta en consignación gestionada.</param>
        public void EliminarDetalle(Articulo articulo)
        {
            this.Gestionar(articulo,string.Empty, 0, 0, 0, 0,new Precio(),false);
        }

        /// <summary>
        /// Se verifica la existencia de saldo para los detalles en alguna de las boletas de venta en consignación.
        /// </summary>
        /// <returns>Retorna verdadero en caso de existir saldo para alguno de los detalles de alguna de las boletas de venta en consignación.</returns>
        public bool ExisteSaldo()
        {
            foreach (VentaConsignacion boleta in Gestionados)
                foreach (DetalleVenta detalle in boleta.Detalles.Lista)
                    if (detalle.UnidadesSaldo())
                        return true;

            return false;
        }
        /// <summary>
        /// Valida que la cantidad a consignar sea mayor al saldo.
        /// Esto es importante cuando el detalle gestionado proviene del desglose de una boleta de venta en consignación, por lo que
        /// los detalles que poseen saldo en la boleta de venta en consignación sugerida debe mantener la cantidad a consignar mayor o igual al saldo,
        /// ya que esa cantidad no fue desglosada.
        /// </summary>
        /// <param name="articuloAgregado">Artículo que se asociará al detalle de la venta en consignación.</param>
        /// <param name="cantidadAlmacenConsignar">Cantidad en unidad de almacén que se desea consignar del detalle.</param>
        /// <param name="cantidadDetalleConsignar">Cantidad en unidad de detalle que se desea consignar del detalle.</param>
        /// <param name="saldoAlmacen">Corresponde al saldo del detalle en unidades de almacén que no fue desglosado.</param>
        /// <param name="saldoDetalle">Corresponde al saldo del detalle en unidades de detalle que no fue desglosado.</param>
        /// <returns>Retorna falso en caso de que el detalle haya sido gestionado en una venta en consignación sugerida y la cantidad a consignar sea menor al saldo.</returns>
        public bool ValidarCantidadConsignar(Articulo articulo, decimal cantidadAlmacen,
            decimal cantidadDetalle,ref decimal saldoAlmacen,ref decimal saldoDetalle)
        {
            VentaConsignacion ventaConsignacion = this.Buscar(articulo.Compania);

            if (ventaConsignacion != null)
            {
                DetalleVenta detalle = ventaConsignacion.Detalles.Buscar(articulo.Codigo);
                if (detalle!= null)
                {
                    saldoAlmacen = detalle.SaldoDesgloseAlmacen;
                    saldoDetalle = detalle.SaldoDesgloseDetalle;

                    decimal totalAlmacenSaldo = saldoAlmacen + (saldoDetalle / articulo.UnidadEmpaque);
                    decimal totalAlmacenConsignar = cantidadAlmacen + (cantidadDetalle / articulo.UnidadEmpaque);
                    //La cantidad a consignar no debe ser menor al saldo del detalle ya que el detalle proviene del desglose de una boleta de venta en consignación.
                    if (totalAlmacenConsignar >= totalAlmacenSaldo)
                        return true;
                }
                else
                {
                    //No hay detalles gestionados para la venta en consignación de la compañía.
                    return true;
                }
            }
            else
            {
                //No hay ventas gestionadas.
                return true;
            }

            return false;
        }
        #endregion

        #region Gestion 

        /// <summary>
        /// Busca una venta en consignacion dentro de las gestionadas utilizando el código de la compañía.
        /// </summary>
        /// <param name="codigoCompania">Código de compañía de la venta.</param>
        /// <returns>En caso de haber una venta gestionada para la compañía retorna un objeto de lo contrario retorna null.</returns>
        public VentaConsignacion Buscar(string compania)
        {
            foreach (VentaConsignacion venta in gestionados)
            {
                if (venta.Compania == compania)
                    return venta;
            }
            return null;
        }

        /// <summary>
        /// Busca una línea dentro de los detalles de las ventas en consignación gestionadas.
        /// </summary>
        /// <param name="articulo">Artículo a buscar.</param>
        /// <returns>
        /// Si el detalle es encontrado se retorna el detalle de la venta en consignación de lo contrario
        /// se retorna 'null'.
        /// </returns>
        public DetalleVenta BuscarDetalle(Articulo articulo)
        {
            foreach (VentaConsignacion ventaConsignacion in gestionados)
                foreach (DetalleVenta detalle in ventaConsignacion.Detalles.Lista)
                    if (detalle.Articulo.Codigo == articulo.Codigo && detalle.Articulo.Compania == articulo.Compania)
                        return detalle;

            return null;
        }

        /// <summary>
        /// Borra una venta en consignación en gestión utilizando el código de la compañía.
        /// </summary>
        /// <param name="compania">Código de la compañía de la venta en consignación.</param>
        public void Borrar(string compania)
        {
            int pos = -1;
            int cont = 0;

            foreach (VentaConsignacion ventaConsignacion in gestionados)
            {
                if (ventaConsignacion.Compania.Equals(compania))
                {
                    pos = cont;
                    break;
                }

                cont++;
            }

            if (pos > -1)
                gestionados.RemoveAt(pos);
        }

        /// <summary>
        /// Reemplaza la venta en consignación de la compañía
        /// </summary>
        /// <param name="compania">Compañia de la venta en consignación</param>
        /// <param name="nueva">Venta en consignación por remplazar</param>
        public void modificar(string compania, VentaConsignacion nueva)
        {
            int pos = -1;
            int cont = 0;
            foreach (VentaConsignacion venta in gestionados)
            {
                if (venta.Compania.Equals(compania))
                {
                    pos = cont;
                    break;
                }
                cont++;
            }
            if (pos > -1)
                gestionados[pos] = nueva;
        }


        #endregion

        #region Gestion de los articulos

        /// <summary>
        /// Obtiene los montos total de la venta en consignacion para las ventas en una lista.
        /// Verifica si el cliente se excedio en credito en  alguna de las companias.
        /// </summary>
        /// <returns>Indicacion de si el cliente esta excedido en credido para alguna de las companias.</returns>
        public bool SacarMontosTotales()
        {
            LimpiarValores();

            bool creditoExcedido = false;

            foreach (VentaConsignacion venta in gestionados)
            {
                venta.CalcularImpuestos();
                TotalImpuesto1 += venta.Impuesto.MontoImpuesto1;
                TotalImpuesto2 += venta.Impuesto.MontoImpuesto2;
                //TotalDescuento += venta.MontoDescuento;
                TotalBruto += venta.MontoBruto;
                TotalDescuento1 += venta.MontoDescuento1;
                TotalDescuento2 += venta.MontoDescuento2;
                PorcDesc1 = venta.PorcentajeDescuento1;
                PorcDesc2 = venta.PorcentajeDescuento2;

                //ABC Caso 34622 20-01-2009
                if (venta.Configuracion.ClienteCia.LimiteCredito > 0 && venta.MontoNeto > venta.Configuracion.ClienteCia.LimiteCredito)
                {
                    //Se excedió el crédito para la compañía específica.
                    creditoExcedido = true;
                }
            }
            return creditoExcedido;
        }

        		/// <summary>
		/// Permite realizar la gestión del detalle de la venta en consignación.
		/// </summary>
		/// <param name="articuloAgregado">Artículo que va ser gestionado.</param>
		/// <param name="cantidadAlmacenConsignar">Cantidad en almacén que se desea consignar del artículo a gestionar.</param>
		/// <param name="cantidadDetalleConsignar">Cantidad en detalle que se desea consignar del artículo a gestionar.</param>
		/// <param name="cantidadAlmacenSaldo">Saldo en unidad de almacén del detalle de la boleta de venta en consignación que ha sido desglosado.</param>
		/// <param name="cantidadDetalleSaldo">Saldo en unidad de detalle del detalle de la boleta de venta en consignación que ha sido desglosado.</param>
		/// <param name="precioMax">Precio de la unidad de almacén del artículo.</param>
		/// <param name="precioMin">Precio de la unidad de detalle del artículo.</param>
		public void Gestionar( 
			Articulo articulo,
            string zona,
			decimal cantidadAlmacenConsignar,
			decimal cantidadDetalleConsignar,
			decimal cantidadAlmacenSaldo,
			decimal cantidadDetalleSaldo,
			Precio precio,
            bool validarCantidadLineas)
		{
			VentaConsignacion ventaConsignacion = Buscar(articulo.Compania);

			//Se crea una nueva venta en consignación.
			if (ventaConsignacion == null)
			{       
				if (cantidadAlmacenConsignar > 0 || cantidadDetalleConsignar > 0)
				{
                    ventaConsignacion = new VentaConsignacion(ObtenerConfiguracionVenta(articulo.Compania),articulo.Bodega.Codigo,zona);
					ventaConsignacion.AgregarDetalle(articulo,cantidadAlmacenConsignar,cantidadDetalleConsignar,cantidadAlmacenSaldo,cantidadDetalleSaldo,precio,validarCantidadLineas);
					gestionados.Add(ventaConsignacion);
				}
			}
			else
			{   
				//La venta en consignación ya esta creada.
				//Se procede a modificar el detalle y el encabezado.
				if (cantidadAlmacenConsignar == 0 && cantidadDetalleConsignar == 0)
				{
					ventaConsignacion.EliminarDetalle(articulo.Codigo);

					if (ventaConsignacion.Detalles.Lista.Count == 0)
						Borrar(ventaConsignacion.Compania);
				}
				else
				{
                    ventaConsignacion.AgregarDetalle(articulo, cantidadAlmacenConsignar, cantidadDetalleConsignar, cantidadAlmacenSaldo, cantidadDetalleSaldo, precio, validarCantidadLineas);
				}
                //LAS. CR1-04113-78LL
                // Debido a que se mantienen los saldos no se crea una nueva 
                // consignación en memoria, por lo que tiene el estado de impremida en
                // verdadero.
                ventaConsignacion.Impreso = false;
			}

			SacarMontosTotales();
		}

     
        #endregion

        #region Impresión de Detalle del Pedido
        // LJR 20/02/09 Caso 34848 Se agrega el parametro de consignaciones, para no manipular los datos en el GestorVentasConsignacion
        /// <summary>
        /// Imprime las ventas en consignación con sus respectivos detalles.
        /// </summary>
        /// <param name="cantidadCopias">Indica la cantidad de copias que se requiere imprimir de las ventas en consignacion.</param>
        /// <param name="criterio">criterio de ordenamiento de los detalles</param>       
        public void ImprimeDetalles(int cantidadCopias, DetalleSort.Ordenador criterio)
        {
            Report reporteVentaConsignacion;
            string imprimirTodo = string.Empty;

            //Imprime el original.
            bool imprimirOriginal = false;

            foreach (VentaConsignacion ventaConsignacion in gestionados)
            {
                // LJR 20/02/09 Caso 34848 Ordenar articulos de la factura
                if (criterio != DetalleSort.Ordenador.Ninguno)
                    ventaConsignacion.Detalles.Lista.Sort(new DetalleSort(criterio));
                if (ventaConsignacion.LeyendaOriginal && !ventaConsignacion.Impreso)
                {
                    imprimirOriginal = true;
                    break;
                }
            }
            //Contiene las ventas en consignación que ya han sido impresas.
            ArrayList ventasConsignacionImpresas = new ArrayList();

            if (imprimirOriginal)
            {
                foreach (VentaConsignacion ventaConsignacion in this.gestionados)
                    if (ventaConsignacion.Impreso)
                        ventasConsignacionImpresas.Add(ventaConsignacion);
                

                reporteVentaConsignacion = new Report(ReportHelper.CrearRutaReporte( Rdl.VentaConsignacion), Impresora.ObtenerDriver());

                reporteVentaConsignacion.AddObject(this);

                //reporteVentaConsignacion.Print();
                reporteVentaConsignacion.PrintAll(ref imprimirTodo);
                imprimirTodo += "\n\n";

                if (reporteVentaConsignacion.ErrorLog != string.Empty)
                    throw new Exception("Ocurrió un error durante la impresión de la venta en consignación: " + reporteVentaConsignacion.ErrorLog);

                reporteVentaConsignacion = null;

                foreach (VentaConsignacion ventaConsignacion in this.gestionados)
                    if (!ventaConsignacion.Impreso && ventaConsignacion.LeyendaOriginal)
                    {
                        ventaConsignacion.Impreso = true;
                        ventaConsignacion.LeyendaOriginal = false;
                        ventaConsignacion.DBActualizarImpresion();
                    }

                for (int indice = ventasConsignacionImpresas.Count; indice > 0; indice--)
                    gestionados.Remove((VentaConsignacion)ventasConsignacionImpresas[indice - 1]);
            }

            //Imprime las copias con la leyenda de copia.
            if (cantidadCopias > 0)
            {
                foreach (VentaConsignacion ventaConsignacion in ventasConsignacionImpresas)
                    gestionados.Add(ventaConsignacion);

                reporteVentaConsignacion = new Report(ReportHelper.CrearRutaReporte( Rdl.VentaConsignacion), Impresora.ObtenerDriver());

                reporteVentaConsignacion.AddObject(this);

                for (int i = 1; i <= (cantidadCopias); i++)
                {
                    //reporteVentaConsignacion.Print();
                    reporteVentaConsignacion.PrintAll(ref imprimirTodo);
                    imprimirTodo += "\n\n";

                    if (reporteVentaConsignacion.ErrorLog != string.Empty)
                        throw new Exception("Ocurrió un error durante la impresión de la venta en consignación: " + reporteVentaConsignacion.ErrorLog);
                }
                //Imprime todo a la vez
                reporteVentaConsignacion.PrintText(imprimirTodo);
                reporteVentaConsignacion = null;
            }
        }
        #endregion

        #region IPrintable Members
        
        public override string GetObjectName()
        {
            return "VENTASCONSIGNACION";
        }
        public override object GetField(string name)
        {
            if (name == "LISTA_CONSIGNACIONES")
                return new ArrayList(gestionados);
            else
                return base.GetField(name);

        }
        #endregion
    }
}
