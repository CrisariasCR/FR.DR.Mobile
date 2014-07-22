using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
//using System.Windows.Forms;

using Softland.ERP.FR.Mobile.Cls.Utilidad;
using Softland.ERP.FR.Mobile.Cls.FRArticulo;
using Softland.ERP.FR.Mobile.Cls.Documentos.FRPedido;
using Softland.ERP.FR.Mobile.Cls.FRCliente;

namespace Softland.ERP.FR.Mobile.Cls.Documentos
{
    public class GestorBonificacionDescuento
    {
#pragma warning disable 169
#pragma warning disable 414
#pragma warning disable 649

        #region Variables

        /// <summary>
        /// Detalle del pedido al que se le cambia la bonificacion
        /// </summary>
        private DetallePedido detalle;

        /// <summary>
        /// Unidad de empaque del detalle
        /// </summary>
        private decimal unidadEmpaque;

        /// <summary>
        /// El maximo que se puede bonificar
        /// </summary>

        private decimal cantidadMaximaBonificar;

        /// <summary>
        /// Indice de la lista
        /// </summary>
        private int indiceBonifica;

        private int indiceDescuento;

        private bool bonifica;

        private bool descuento;

        private ClienteCia cliente;

        private decimal cantidadMaximaDescuento;

        private decimal unidadAlmacen;

        private decimal unidadDetalle;

        private decimal valorDesc;

        private bool load = true;

        private Pedido pedidoActual;

        private Pedidos pedidos;

        #endregion

        #region Propiedades

        

        #endregion Propiedades

        #region Constructor

        public GestorBonificacionDescuento()
        { }

        /*public GestorBonificacionDescuento(Pedidos gestorPedidos)
        {
            pedidos = gestorPedidos;
        }*/

        public GestorBonificacionDescuento(Pedido pedido)
        {
            pedidoActual = pedido;
        }

        #endregion

        #region Metodos de Clase

        private void buscarPedido()
        {
            pedidoActual = pedidos.Buscar(detalle.Articulo.Compania);
        }

        private void cambiarBonificacion()
        {
            //decimal total = (Convert.ToDecimal(this.ltbCantidadBonificar.Text) * this.unidadEmpaque) + Convert.ToDecimal(this.ltbCantidadBonificarDetalle.Text);
            decimal total = (this.unidadAlmacen * this.unidadEmpaque) + this.unidadDetalle;
    
            /*if (total <= this.cantidadMaximaBonificar)
            {*/
            try
            {
                pedidoActual.Detalles.CambiaCantidadBonificar(this.indiceBonifica, this.unidadAlmacen, this.unidadDetalle);
            }
            catch (Exception ex)
            {
                string error = ex.Message;
                //MessageBox.Show(ex.Message.ToString(), "Error al cambiar cantidad bonificada", MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
            }
           /* }
            else
            {
                MessageBox.Show("La cantiad no puede ser mayor a la calculada. La cantidad almacén máxima permitida es: " + this.detalle.UnidadBonificadaAlmacen.ToString("0.00") + " y La cantidad detalle máxima permitida es: " + this.detalle.UnidadBonificadaDetalle.ToString("0.00"), "Cantidad Invalida", MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
            }*/
        }

        private void cambiarDescuento()
        {
            /*if (this.valorDesc >= 0 && this.valorDesc <= this.cantidadMaximaDescuento)
            {*/
            try
            {
                decimal descuentogeneral = pedidoActual.PorcDescGeneral;
                pedidoActual.RestarMontosLinea(detalle);
                detalle = pedidoActual.Detalles.CambiarDescuento(this.indiceDescuento, this.valorDesc, this.cliente, descuentogeneral);
                pedidoActual.AgregarMontosLinea(detalle);
                pedidoActual.ReCalcularDescuentosGenerales();
            }
            catch (Exception ex)
            {
                string error = ex.Message;
                //MessageBox.Show(ex.Message.ToString(), "Error al cambiar valor del descuento", MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
            }
            /*}
            else
            {
                MessageBox.Show("El valor del descuento no puede ser mayor al básico. El cual es: " + this.cantidadMaximaDescuento.ToString("0.00") + ". O menor que 0", "Cantidad Invalida", MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
            }*/
        }

        #endregion Metodos de Clase

        #region Metodos Publicos

        public void CambiarBonificacionDescuento(DetallePedido detalle, int indiceBonif, int indiceDesc, bool bonif, bool desc, string clien, decimal descuento, decimal cantAlm, decimal cantDet)
        {
            try
            {
                this.detalle = detalle;
                this.indiceBonifica = indiceBonif;
                this.indiceDescuento = indiceDesc;
                this.bonifica = bonif;
                this.descuento = desc;
                List<ClienteCia> clientes = ClienteCia.ObtenerClientes(clien, false);
                this.cliente = (ClienteCia)(clientes[0]);

                //this.buscarPedido();

                if (this.bonifica)
                {
                    this.unidadAlmacen = cantAlm;
                    this.unidadDetalle = cantDet;
                    this.unidadEmpaque = detalle.LineaBonificada.Articulo.UnidadEmpaque;
                    this.cambiarBonificacion();
                }
                if (this.descuento)
                {
                    this.valorDesc = descuento;
                    this.cambiarDescuento();
                }
            }
            catch (Exception ex)
            {
                string error = ex.Message;
                //MessageBox.Show(ex.Message.ToString(), "Error al cambiar valores de descuento o bonificacion", MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
            }
        }

        public static decimal DescuentoMaximo(Softland.ERP.FR.Mobile.Cls.FRCliente.ClienteCia clien, Articulo articulo, string cantAlmacen, string cantDetalle)
        {
            try
            {
                decimal totalAlmacen = decimal.Zero;

                List<ClienteCia> clientes = ClienteCia.ObtenerClientes(clien.Codigo, false);

                totalAlmacen = GestorUtilitario.TotalAlmacen(cantAlmacen, cantDetalle, Convert.ToInt32(articulo.UnidadEmpaque));
                ((ClienteCia)(clientes[0])).NivelPrecio = clien.NivelPrecio;
                Descuento desc = articulo.CargarDescuento((ClienteCia)(clientes[0]), totalAlmacen);

                if (desc != null)
                    return desc.Monto;
                else
                    return 0;
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message.ToString(), "Error cargando descuento maximo", MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
                string error = ex.Message;
                return decimal.Zero;
            }
        }

        /// <summary>
        /// Obtiene informacion de bonificacion
        /// </summary>
        /// <param name="clien">Cliente del pedido</param>
        /// <param name="articulo">Articulo procesar</param>
        /// <param name="totalAlmacen">Cantidad en unidades de almacen</param>
        /// <param name="artBonificacion">Articulo bonificado</param>
        /// <param name="cantBonAlm">Cantidad Almacen Bonificada</param>
        /// <param name="cantBonDet">Cantidad Detalle Bonificada</param>
        public static void Bonificacion(ClienteCia clien, Articulo articulo, string cantAlmacen, string cantDetalle, ref Articulo artBonificacion, ref decimal cantBonAlm, ref decimal cantBonDet, ref decimal totalAlmacenBonif)
        {
            try
            {
                decimal totalAlmacen = decimal.Zero;
                cantBonAlm = decimal.Zero;
                cantBonDet = decimal.Zero;
                totalAlmacenBonif = decimal.Zero;
                artBonificacion = null;
                
                //LDA R0-02009-S00S
                List<ClienteCia> clientes = ClienteCia.ObtenerClientes(clien.Codigo, false);
                ((ClienteCia)(clientes[0])).NivelPrecio = clien.NivelPrecio;

                totalAlmacen = GestorUtilitario.TotalAlmacen(cantAlmacen, cantDetalle, Convert.ToInt32(articulo.UnidadEmpaque));
                
                //Bonificacion.ObtenerBonificaciones(cliente, Articulo.Codigo, UnidadesAlmacen);
                List<Bonificacion> listaBonif = Softland.ERP.FR.Mobile.Cls.FRArticulo.Bonificacion.ObtenerBonificaciones(
                                                 (ClienteCia)(clientes[0]), articulo.Codigo, totalAlmacen);

                if (listaBonif.Count > 0)
                {
                    Bonificacion bonificacion = (Bonificacion)listaBonif[0];

                    artBonificacion = bonificacion.ArticuloBonificado;

                    //Verificamos si existe un factor de conversión.
                    if (bonificacion.Factor != 0)
                        cantBonAlm = Decimal.Truncate(totalAlmacen /**/ / bonificacion.Factor) * bonificacion.Cantidad;
                    else
                        cantBonAlm = bonificacion.Cantidad;

                    totalAlmacenBonif = cantBonAlm;

                    GestorUtilitario.CalcularCantidadBonificada(cantBonAlm, bonificacion.ArticuloBonificado.UnidadEmpaque, ref cantBonAlm, ref cantBonDet);
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message.ToString(), "Error cargando bonificación", MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
                string error = ex.Message;
                cantBonAlm = decimal.Zero;
                cantBonDet = decimal.Zero;
            }

        }

        #endregion Metodos Publicos        
    }
}
