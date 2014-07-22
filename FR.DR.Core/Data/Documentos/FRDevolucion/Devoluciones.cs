using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using System.Text;

using EMF.Printing;
using Softland.ERP.FR.Mobile.Cls.FRCliente;
using Softland.ERP.FR.Mobile.Cls.FRArticulo;
using Softland.ERP.FR.Mobile.Cls.Cobro;
using Softland.ERP.FR.Mobile.Cls.Reporte;
using Softland.ERP.FR.Mobile.ViewModels;

namespace Softland.ERP.FR.Mobile.Cls.Documentos.FRDevolucion
{
    /// <summary>
    /// Representa la Gestion de devoluciones para un cliente Multi cia
    /// </summary>
    public class Devoluciones : IPrintable
    {
        #region Variables Configuracion Config.xml

        //Caso: 28086 LDS 02/05/2007
        /// <summary>
        /// Permite habilitar el item de devoluciones en el menú Procesos del sistema.
        /// </summary>
        /// <remarks>
        /// El valor por defecto es SI, habilita el item Devolución.
        /// NO, deshabilita el item Devolución.
        /// </remarks>
        public static bool HabilitarDevoluciones;

        public static bool DevolucionAutomatica;

        public static bool TiposDevoluciones;

        public static List<string> tiposDevolucion;

        //Facturas de contado y recibos en FR - KFC
        public static string tipoPagoDevolucion;
        

        #endregion

        #region Constructor

        public Devoluciones()
        { 
        }
        /// <summary>
        /// Conjunto de devoluciones para un cliente
        /// </summary>
        /// <param name="gestionados">devoluciones en gestion</param>
        /// <param name="cliente">cliente asociado</param>
        public Devoluciones(List<Devolucion> gestionados, ClienteBase cliente)
        {
            this.gestionados = gestionados;
            this.cliente = cliente;
        }
        #endregion

        #region Variables de la clase

        private List<Devolucion> gestionados = new List<Devolucion>();
        /// <summary>
        /// Devoluciones asociadas a la gestion
        /// </summary>
        public List<Devolucion> Gestionados
        {
            get { return gestionados; }
            set { gestionados = value; }
        }

        private ClienteBase cliente;
        /// <summary>
        /// Cliente asociado a la gestion
        /// </summary>
        public ClienteBase Cliente
        {
            get { return cliente; }
            set { cliente = value; }
        }

        public List<string> ListaTiposDevoluciones
        {
            get { return tiposDevolucion; }
        }
        

        #endregion

        #region Metodos de la logica del negocio

        /// <summary>
        /// Verificar si existen articulos en la gestion
        /// </summary>
        /// <returns>existen articulos</returns>
        public bool ExistenArticulosGestionados()
        {
            foreach (Devolucion dev in gestionados)
            {
                if (!dev.Detalles.Vacio())
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Guarda en la base de datos cada una de las devoluciones en gestión.
        /// </summary>
        public void Guardar()
        {
            foreach (Devolucion devolucion in gestionados)
            {
                try
                {
                    if (devolucion.Configuracion.Compania.UtilizaMinimoExento)
                        devolucion.actualizarMontosPercepcion();                    

                    devolucion.Guardar(true);
                }
                catch (Exception ex)
                {
                    throw new Exception("Error guardando devolución de compañía '" + devolucion.Compania + "'." + ex.Message);
                }
            }
        }

        public void ActualizarDevolucion()
        {
            if (gestionados[0].Configuracion.Compania.UtilizaMinimoExento)
                gestionados[0].actualizarMontosPercepcion();

            Gestionados[0].Actualizar(true);            
        }

        /// <summary>
        /// Buscar una devolucion para una compania
        /// </summary>
        /// <param name="compania">compania asociada</param>
        /// <returns>devolucion</returns>
        public Devolucion Buscar(string compania)
        {
            foreach (Devolucion devolucion in gestionados)
            {
                if (devolucion.Compania.Equals(compania))
                    return devolucion;
            }
            return null;
        }

        /// <summary>
        /// Borra una devolucion en gestión utilizando el codigo de la compania.
        /// </summary>
        /// <param name="compania">Codigo de compania del pedido</param>
        public void Borrar(string compania)
        {
            int pos = -1;
            int cont = 0;

            foreach (Devolucion devolucion in gestionados)
            {
                if (devolucion.Compania.Equals(compania))
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
        /// Se realiza la gestión de la devolución a generar por el desglose de la boleta de venta en consignación.
        /// </summary>
        /// <param name="consignacion">Numero de consignacion a asociar.</param>        
        /// <param name="articulo">Corresponde al artículo que se agregará como detalle de la devolución.</param>
        /// <param name="cliente">cliente a asociar.</param>
        /// <param name="cantidadAlmacen">Corresponde a la cantidad en unidad de almacén con la cual se generará el detalle de la devolución.</param>
        /// <param name="cantidadDetalle">Corresponde a la cantidad en unidad de detalle con la cual se generará el detalle de la devolución.</param>
        /// <param name="estado">Indica el estado del detalle a devolver en el desglose de la boleta de venta en consignación.</param>
        /// <param name="lote">Indica el lote del detalle a devolver en el desglose de la boleta de venta en consignación.</param>
        /// <param name="observaciones">Indica las observaciones asignadas al detalle a devolver en el desglose de la boleta de venta en consignación.</param>
        /// <param name="bodega">Indica la bodega que se asignará a la devolución a generar.</param>
        /// <param name="bodegaConsigna">Indica la bodega de donde proviene la devolución a generar.</param>
        /// <param name="zona">Zona que se le asigna a la devolucion.</param>        
        public void GestionarConsignado(
            string consignacion,
            Articulo articulo,
            ClienteCia cliente,
            decimal cantidadAlmacen,
            decimal cantidadDetalle,
            Estado estado,
            string lote,
            string observaciones,
            string bodega, string bodegaConsigna, string zona)
        {
            Devolucion devolucion = Buscar(articulo.Compania);
            //El inventario no existe	
            if (devolucion == null)
            {
                devolucion = new Devolucion(articulo, cliente, zona, bodega);
                
                //Se indica que la devolución está siendo generada por el desglose de una boleta de venta
                devolucion.EsConsignacion = true;
                devolucion.NumRef = consignacion;
                devolucion.BodegaConsigna = bodegaConsigna;
                //Agregar el detalle
                devolucion.Gestionar(cliente, articulo, cantidadDetalle, cantidadAlmacen, estado, observaciones, lote);
                gestionados.Add(devolucion);
            }
            //La devolucion existe
            else
            {
                devolucion.Gestionar(cliente, articulo, cantidadDetalle, cantidadAlmacen, estado, observaciones, lote);
            }
        }

        /// <summary>
        /// Realizar la gestion de una devolucion
        /// </summary>
        /// <param name="articulo">articulo a gestionar</param>
        /// <param name="cliente">cliente asociado a la devolucion</param>
        /// <param name="zona">zona asociada</param>
        /// <param name="bodega">bodega asociada</param>
        /// <param name="estado">estado del articulo a devolver</param>
        /// <param name="observaciones">observaciones del detalle a devolver</param>
        /// <param name="lote">lote del articulo a devolver</param>
        /// <param name="cantidadDetalle">cantidad en detalle a devolver</param>
        /// <param name="cantidadAlmacen">cantidad en almacen a devolver</param>   
        public void Gestionar(Articulo articulo, ClienteCia cliente, string zona, string bodega, Estado estado, string observaciones,string lote, decimal cantidadDetalle, decimal cantidadAlmacen, string referencia, string ncfRef)
        {
            Devolucion devolucion = Buscar(articulo.Compania);
            //El inventario no existe	
            if (devolucion == null)
            {
                //Si la cantidad es 0 ignore
                if (cantidadDetalle == 0 && cantidadAlmacen == 0)
                    return;

                //se crea una devolucion nuevo
                devolucion = new Devolucion(articulo,cliente,zona,bodega);
                devolucion.NumRef = referencia;
                devolucion.NCFRef = ncfRef;

                //Agregar el detalle
                devolucion.Gestionar(cliente, articulo, cantidadDetalle, cantidadAlmacen, estado, observaciones, lote);
                gestionados.Add(devolucion);                

            }
            //La devolucion existe
            else
            {
                //Se elimina el detalle
                if (cantidadAlmacen == 0 && cantidadDetalle == 0)
                {
                    devolucion.EliminarDetalle(articulo.Codigo,estado);
                    //Si No quedan detalles
                    if (devolucion.Detalles.Vacio())
                        Borrar(devolucion.Compania);
                }
                //Se actualiza las cantidades
                else
                    devolucion.Gestionar(cliente, articulo, cantidadDetalle, cantidadAlmacen, estado, observaciones, lote);
            }
        }

        public static void cargarTipos(string tipo)
        {
            if (tiposDevolucion == null || tiposDevolucion.Count == 0)
            { tiposDevolucion = new List<string>(); }

            tiposDevolucion.Add(tipo);
        }

        #endregion

        #region Facturas de contado y recibos en FR - KFC

        public static void GenerarNotaCredito(Devolucion devolucion, int diasCredito)
        {
            NotaCredito notaCredito;

            //Se obtiene el tipo de moneda
            if (devolucion.Moneda == "L") // provisional la L
            {
                notaCredito = new NotaCredito(devolucion.Numero, TipoMoneda.LOCAL, devolucion.Compania, devolucion.Zona, devolucion.Cliente);

                notaCredito.MontoDocLocal  = devolucion.MontoNeto;
                notaCredito.MontoDocDolar  =  notaCredito.MontoDocLocal / devolucion.Configuracion.Compania.TipoCambio;                
            }
            else
            {
                notaCredito = new NotaCredito(devolucion.Numero, TipoMoneda.DOLAR, devolucion.Compania, devolucion.Zona, devolucion.Cliente);

                notaCredito.MontoDocDolar  = devolucion.MontoNeto;
                notaCredito.MontoDocLocal  =  notaCredito.MontoDocDolar * devolucion.Configuracion.Compania.TipoCambio;
            }

            /*if (Devoluciones.tipoPagoDevolucion == "E")
                notaCredito.FechaVencimiento = notaCredito.FechaRealizacion;*/

            //if (Devoluciones.tipoPagoDevolucion == "C") creo que solo pasa por aqui si es credito asi que el if esta de mas
            notaCredito.FechaVencimiento = notaCredito.FechaRealizacion.AddDays(diasCredito); // se asigna el numero de dias de condicion de pago del cliente

            // aqui implementar que si el cliente es de contado la nota sea
            notaCredito.CondicionPago = devolucion.Configuracion.Compania.CondicionPagoDevoluciones;


            notaCredito.TipoCambio = devolucion.Configuracion.Compania.TipoCambio;

            try
            {
                //notaCredito.Guardar();
                notaCredito.GuardarPendienteCobro(notaCredito);
            }
            catch (Exception ex)
            {
                throw new Exception("Error guardando Nota de Crédito. " + ex.Message);
            }
        }

        #endregion

        

        #region Impresion de Devoluciones

        #region Impresion de resumen de devolucion
        /// <summary>
        /// Imprime la informacion que se encuentra en la pantalla 
        /// se debe ir indicando las posiciones de todos los 
        /// componentes de la colilla como las pocisiones de la información que se desplegara.
        /// </summary>
        public void ImprimeResumenDevolucion(BaseViewModel viewModel)
        {
            Report reporteDev = new Report(ReportHelper.CrearRutaReporte(Rdl.ResumenDevoluciones), Impresora.ObtenerDriver());

            reporteDev.AddObject(this);
            ImprimeResumenDevolucion(viewModel, reporteDev);
        }

        private void ImprimeResumenDevolucion(BaseViewModel viewModel, Report reporteDev)
        {
            reporteDev.Print();
            if (reporteDev.ErrorLog != string.Empty)
            {
                viewModel.mostrarAlerta("Ocurrió un error durante la impresión: " + reporteDev.ErrorLog);
            }

            viewModel.mostrarMensaje(Mensaje.Accion.Decision, "imprimir de nuevo",
                result =>
                {
                    if (result == System.Windows.Forms.DialogResult.Yes)
                    {
                        ImprimeResumenDevolucion(viewModel, reporteDev);
                    }
                });
        }            

        #endregion

        #region Impresión de Detalle de la Devolución

        //Caso 25452 LDS 29/10/2007
        /// <summary>
        /// Imprime el detalle de la devoluciones.
        /// <param name="cantidadCopias">
        /// Indica la cantidad de copias que se requiere imprimir de las devoluciones.
        /// </param>		
        /// <param name="criterio">criterio de ordenamiento de los detalles</param>          
        /// </summary>
        public void ImprimeDetalleDevolucion(int cantidadCopias, DetalleSort.Ordenador criterio)
        {
            Report reporteDev;
            //Imprime el original
            bool imprimirOriginal = false;
            string imprimirTodo = string.Empty;

            foreach (Devolucion devolucion in gestionados)
            {
                // LJR 20/02/09 Caso 34848 Ordenar articulos de la devolucion
                if (criterio != DetalleSort.Ordenador.Ninguno)
                    devolucion.Detalles.Lista.Sort(new DetalleSort(criterio));
                if (devolucion.LeyendaOriginal && !devolucion.Impreso)
                    imprimirOriginal = true;
            }
            //Contiene las devoluciones que ya han sido impresas
            List<Devolucion> devolucionesImpresas = new List<Devolucion>();
            devolucionesImpresas.Clear();

            if (imprimirOriginal)
            {
                foreach (Devolucion devolucion in gestionados)
                    if (devolucion.Impreso)
                        devolucionesImpresas.Add(devolucion);

                

                reporteDev = new Report(ReportHelper.CrearRutaReporte(Rdl.DetalleDevolucion), Impresora.ObtenerDriver());

                reporteDev.AddObject(this);

                //reporteDev.Print();
                reporteDev.PrintAll(ref imprimirTodo);
                imprimirTodo += "\n\n";

                if (reporteDev.ErrorLog != "")
                    throw new Exception("Ocurrió un error durante la impresión de la devolución: " + reporteDev.ErrorLog);

                reporteDev = null;

                foreach (Devolucion devolucion in gestionados)
                    if (!devolucion.Impreso && devolucion.LeyendaOriginal)
                    {
                        devolucion.Impreso = true;
                        devolucion.LeyendaOriginal = false;
                        devolucion.DBActualizarImpresion();
                    }

                for (int indice = devolucionesImpresas.Count; indice > 0; indice--)
                    gestionados.Remove(devolucionesImpresas[indice - 1]);
            }

            //Imprime las copias con la leyenda de copia
            if (cantidadCopias > 0)
            {
                foreach (Devolucion devolucion in devolucionesImpresas)
                    gestionados.Add(devolucion);

                reporteDev = new Report(ReportHelper.CrearRutaReporte(Rdl.DetalleDevolucion), Impresora.ObtenerDriver());

                reporteDev.AddObject(this);

                for (int i = 1; i <= (cantidadCopias); i++)
                {
                    //reporteDev.Print();
                    reporteDev.PrintAll(ref imprimirTodo);
                    imprimirTodo += "\n\n";

                    if (reporteDev.ErrorLog != string.Empty)
                        throw new Exception("Ocurrió un error durante la impresión de la devolución: " + reporteDev.ErrorLog);
                }
                //Imprime todo a la vez
                reporteDev.PrintText(imprimirTodo);
                reporteDev = null;
            }
        }
        #endregion

        #endregion

        #region IPrintable Members

        public string GetObjectName()
        {
            return "DEVOLUCIONES";
        }

        public object GetField(string name)
        {
            if (name == "LISTA_DEVOLUCIONES")
                return new ArrayList(gestionados);
            else
                return Cliente.GetField(name);
        }

        #endregion
    }
}
