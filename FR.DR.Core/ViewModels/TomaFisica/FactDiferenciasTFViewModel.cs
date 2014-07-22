using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FR.Core.Model;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.Windows.Input;
using Cirrious.MvvmCross.Commands;
using System.Reflection;
using System.IO;
using Softland.ERP.FR.Mobile.Cls.Reporte;
using Softland.ERP.FR.Mobile.Cls;
using Softland.ERP.FR.Mobile.Cls.Utilidad;
using Softland.ERP.FR.Mobile.Cls.AccesoDatos;
using Softland.ERP.FR.Mobile.Cls.Seguridad;
using System.Windows.Forms;
using Softland.ERP.FR.Mobile.Cls.Sincronizar;
using Softland.ERP.FR.Mobile.Cls.Configuracion;
using Softland.ERP.FR.Mobile.Cls.FRArticulo;
using Softland.ERP.FR.Mobile.Cls.Corporativo;
using Softland.ERP.FR.Mobile.Cls.FRCliente;
using Softland.ERP.FR.Mobile.Cls.Documentos;
using Softland.ERP.FR.Mobile.Cls.Documentos.FRPedido;
using Softland.ERP.FR.Mobile.Cls.Documentos.FRInventario;

using System.Data.SQLite;

namespace Softland.ERP.FR.Mobile.ViewModels
{
    public class FactDiferenciasTFViewModel : DialogViewModel<bool>
    {
#pragma warning disable 414

        public FactDiferenciasTFViewModel(string messageId, string pCompania, string pBodega)
            : base(messageId)
        {       

            BodegaCamion = pBodega;
            CompaniaRuta = pCompania.ToUpper();
            HandHeld = Ruta.NombreDispositivo();
            Localizacion = "ND";            

            //Se inicializa la instancia de tomaFisica
            tomaFisica = new TomaFisicaInventario(CompaniaRuta, HandHeld, BodegaCamion, Localizacion, DateTime.Now.Date);

            //Se cargan las diferencias
            CargarArtConDiferencias();

            //txtCantNoFacturar.Enabled = false;
            //txtArticulo.Enabled = false;
        }

        #region Binding
        
        private string nombreCliente;
        public string NombreCliente
        {
            get { return nombreCliente; }
            set
            {
                if (value != nombreCliente)
                {
                    nombreCliente = value;
                    RaisePropertyChanged("NombreCliente");
                }
            }
        }        

        private decimal cantFacturar;
        public decimal CantFacturar
        {
            set { cantFacturar = value; txtCantFacturar_TextChanged(); RaisePropertyChanged("CantFacturar"); }
            get { return cantFacturar; }
        }

        private decimal cantNoFacturar;
        public decimal CantNoFacturar
        {
            set { cantNoFacturar = value; RaisePropertyChanged("CantNoFacturar"); }
            get { return cantNoFacturar; }
        }

        private string articuloDescripcion;
        public string ArticuloDescripcion
        {
            get { return articuloDescripcion; }
            set { articuloDescripcion = value; RaisePropertyChanged("ArticuloDescripcion");}
        }

        private string textoBusqueda;
        public string TextoBusqueda
        {
            get { return textoBusqueda; }
            set { textoBusqueda = value; RaisePropertyChanged("TextoBusqueda"); }
        }

        //public IObservableCollection<TomaFisicaInventario> Items;

        private IObservableCollection<TomaFisicaInventario> itemsFTF;
        public IObservableCollection<TomaFisicaInventario> ItemsFTF
        {
            get { return itemsFTF; }
            set { itemsFTF = value; RaisePropertyChanged("ItemsFTF"); }
        }

        private TomaFisicaInventario itemActualFTF;
        public TomaFisicaInventario ItemActualFTF
        {
            get { return itemActualFTF; }
            set { itemActualFTF = value; lstArticulosDiferencias_SelectedIndexChanged(); RaisePropertyChanged("ItemActualFTF"); }
        } 


        #endregion

        #region Comandos y Acciones

        public ICommand ComandoAgregar
        {
            get { return new MvxRelayCommand(Agregar); }
        }

        public ICommand ComandoAceptar
        {
            get { return new MvxRelayCommand(Aceptar); }
        }

        #endregion

        #region mobile

        #region Variables Clase

        Articulo articuloDatos = null;
        TomaFisicaInventario tomaFisica = null;
        Cliente clienteImpresion;
        Pedido pedidoImpresion;

        public IObservableCollection<string> Header { get { return new SimpleObservableCollection<string>() { "Header" }; } }

        string compania = string.Empty;
        string bodega = string.Empty;
        string handHeld = string.Empty;
        string localizacion = string.Empty;
        int indiceSel = 0;
        decimal diferenciaLinea = decimal.Zero;

        public decimal DiferenciaLinea
        {
            get { return diferenciaLinea; }
            set { diferenciaLinea = value; }
        }

        public int IndiceSeleccionado
        {
            get { return indiceSel; }
            set { indiceSel = value; }
        }

        public string Localizacion
        {
            get { return localizacion; }
            set { localizacion = value; }
        }
        public string HandHeld
        {
            get { return handHeld; }
            set { handHeld = value; }
        }
        public string CompaniaRuta
        {
            get { return compania; }
            set { compania = value; }
        }
        public string BodegaCamion
        {
            get { return bodega; }
            set { bodega = value; }
        }

        #endregion

        public void Aceptar() 
        {         
            if (ActualizarCantArticulosBD() && GenerarFacturaDiferencia())
            {
                //if (Impresora.SugerirImprimir)
                if (Impresora.SugerirImprimir)
                {
                    if(clienteImpresion!=null)
                    {
                        this.mostrarMensaje(Mensaje.Accion.Imprimir, "el detalle de las facturas realizadas", res =>
                        {
                            if (res.Equals(DialogResult.Yes) || res.Equals(DialogResult.OK))
                            {
                                bool result=ImprimirDocumento(clienteImpresion);
                                if (result)
                                {
                                    this.mostrarMensaje(Mensaje.Accion.Imprimir, "el detalle de toma fisica de inventario realizada", res2 =>
                                    {
                                        if (res2.Equals(DialogResult.Yes) || res.Equals(DialogResult.OK))
                                        {
                                            ImprimirDocumentoInventario();
                                            ReturnResult(true);
                                        }
                                        else
                                        {
                                            ReturnResult(true);
                                        }
                                    }
                                    );
                                }
                                else
                                {
                                    ReturnResult(true);
                                }
                            }
                            else
                            {
                                this.mostrarMensaje(Mensaje.Accion.Imprimir, "el detalle de toma fisica de inventario realizada", res3 =>
                                {
                                    if (res3.Equals(DialogResult.Yes) || res.Equals(DialogResult.OK))
                                    {
                                        ImprimirDocumentoInventario();
                                        ReturnResult(true);
                                    }
                                    else
                                    {
                                        ReturnResult(true);
                                    }
                                }
                                 );
                            }                                
                        }
                        );
                    }
                    else
                    {

                        this.mostrarMensaje(Mensaje.Accion.Imprimir, "el detalle de toma fisica de inventario realizada", res =>
                        {
                            if (res.Equals(DialogResult.Yes) || res.Equals(DialogResult.OK))
                            {
                                ImprimirDocumentoInventario();
                                ReturnResult(true);
                            }
                            else
                            {
                                ReturnResult(true);
                            }
                        }
                        );
                    }                   

                }
                else
                {
                    ReturnResult(true);
                }
                
            }
            else
            {
                this.mostrarAlerta("Problemas al tratar de realizar la facturación de las diferencias.");
            }
             
        }

        private void Agregar()
        {
            decimal cantFacturar = decimal.Zero;
            decimal cantNoFacturar = decimal.Zero;

            try
            {
                cantFacturar = this.CantFacturar;
                cantNoFacturar = this.CantNoFacturar;

                if (DiferenciaLinea <= (cantFacturar + cantNoFacturar))
                {
                    ItemActualFTF.CantidadFacturar = cantFacturar;
                    ItemActualFTF.CantidadNoFacturar = cantNoFacturar;
                    this.ItemsFTF = new SimpleObservableCollection<TomaFisicaInventario>(this.ItemsFTF.ToList<TomaFisicaInventario>());                    
                }
                else
                {
                    this.mostrarAlerta(String.Format("La cantidad facturar más la no facturada({0}) no puede ser mayor a la diferencia({1})", Convert.ToString((cantNoFacturar + cantFacturar)), DiferenciaLinea));
                }
            }
            catch (Exception ex)
            {
                this.mostrarAlerta("Problemas al tratar de modificar las cantidades para la fila seleccionada. " + ex.Message);
            }
        }

        public void txtCantFacturar_TextChanged()
        {
            decimal cantFact = decimal.Zero;
            try
            {
                //Se obtienen las cantidades de la pantalla
                cantFact = Convert.ToDecimal(CantFacturar);

                //Se obtiene la diferencia
                RestarCantidades(DiferenciaLinea, cantFact);

                //Se asigna la cantidad que no se factura en el campo correspondiente
                //CantNoFacturar = cantNoFact;
            }
            catch (Exception ex)
            {
                this.mostrarAlerta("No se pudo realizar la resta de cantidades." + ex.Message);
            }
        }


        #region Eventos de controles

        private void lstArticulosDiferencias_SelectedIndexChanged()
        {
            try
            {
                if (ItemActualFTF != null)
                {
                    //Se obtienen las cantidades de la fila seleccionada
                    TextoBusqueda = ItemActualFTF.Articulo;
                    ArticuloDescripcion = ItemActualFTF.DescArticulo;
                    DiferenciaLinea = ItemActualFTF.CantidadDiferencia;
                    CantFacturar = ItemActualFTF.CantidadFacturar;
                    CantNoFacturar = ItemActualFTF.CantidadNoFacturar;
                }
            }
            catch (Exception ex)
            {
                this.mostrarAlerta("Problemas obteniendo datos de línea seleccionada." + ex.Message);
            }
        }

        

        #endregion Eventos de controles

        #region Otras Funciones

        public bool CargarArtConDiferencias()
        {
            List<TomaFisicaInventario> ItemsArt=new List<TomaFisicaInventario>();
            bool cargaExitosa = true;
            //Bodega bodegaArt = new Bodega(bodega);

            try
            {
                tomaFisica.CargarArticulosBoleta(ref ItemsArt);
                ItemsFTF = new SimpleObservableCollection<TomaFisicaInventario>(ItemsArt); 
                
            }
            catch (Exception ex)
            {
                this.mostrarAlerta("Problemas cargando las diferencias de la toma física." + ex.Message);
                cargaExitosa = false;
            }

            return cargaExitosa;
        }

        public bool RestarCantidades(decimal diferencia, decimal cantFact)
        {
            bool procesoExitoso = true;

            try
            {
                //Se cálcula la cantidad que no se va a facturar
                this.CantNoFacturar = diferencia - cantFact;      
            }
            catch (Exception ex)
            {
                this.mostrarAlerta("Problemas al realizar el cálculo de cantidades. " + ex.Message);
                procesoExitoso = false;
            }

            return procesoExitoso;
        }

        protected bool ActualizarCantArticulosBD()
        {
            bool procesoExitoso = true;

            try
            {
                //Se actualizan las cantidades de las boletas en la base de datos
                if (procesoExitoso)
                {
                    foreach (TomaFisicaInventario item in ItemsFTF)
                    {
                        //Se valida que el item este seleccionado para facturar
                        if (item.Selected)
                        {
                            if (procesoExitoso)
                            {
                                tomaFisica.Articulo = item.Articulo;;
                                tomaFisica.Lote = item.Lote;

                                //Se carga la boleta del artículo en memoria
                                procesoExitoso = tomaFisica.CargarBoleta();
                            }

                            if (procesoExitoso)
                            {
                                //Se asignan los datos a la clase
                                tomaFisica.CantidadFacturar = item.CantidadFacturar;
                                tomaFisica.CantidadDiferencia = Convert.ToDecimal(item.CantidadNoFacturar * -1);

                                //Se actualizan los datos
                                procesoExitoso = tomaFisica.ActualizarCantBoleta();

                                //Se cambia el estado de la boleta a aplicada
                                if (tomaFisica.CantidadDiferencia == 0)
                                {
                                        procesoExitoso = tomaFisica.ActualizarEstadoBoleta(tomaFisica.Consecutivo, TomaFisicaInventario.BOLETA_APLICADA);                                    
                                }
                            }
                        }
                    }

                    //Se aplica la toma física
                    if (procesoExitoso)
                    {
                        procesoExitoso = tomaFisica.AplicarTomaFisica();
                    }
                }
            }
            catch (Exception ex)
            {
                this.mostrarAlerta("Problemas actualizando boletas de inventario. " + ex.Message);
                procesoExitoso = false;
            }

            return procesoExitoso;
        }

        protected bool GenerarFacturaDiferencia()
        {
            bool procesoExitoso = true;
            List<TomaFisicaInventario> lstArtFacturar = new List<TomaFisicaInventario>();            

            decimal unidadesAlm = decimal.Zero;
            decimal unidadesDet = decimal.Zero;
            //int unidadEmpaque = 0;

            Articulo articulo = new Articulo();
            Cliente cliente = new Cliente();
            ClienteCia clienteCompania = new ClienteCia();
            ConfigDocCia config = null;

            Pais pais = new Pais();
            ClaseDoc clasedoc = ClaseDoc.Normal;

            CondicionPago condicion = new CondicionPago();

            Compania cia = null;
            
            try
            {
                procesoExitoso = tomaFisica.FacturarDiferencias(ref lstArtFacturar);

                if (procesoExitoso && lstArtFacturar.Count > 0)
                {
                    if (!string.IsNullOrEmpty(FRmConfig.ClienteRutero))
                    {
                        cliente.Codigo = FRmConfig.ClienteRutero;
                        cliente.Cargar();

                        string codigoRuta = Ruta.ObtenerRutas()[0].Codigo;

                        cliente.ObtenerClientesCia();
                        clienteCompania = cliente.ObtenerClienteCia(compania);

                        pais.Cargar(clienteCompania.Compania);
                        pais.ObtenerPais(clienteCompania.Compania, cliente.Codigo);

                        condicion.ObtenerCondicionPago(clienteCompania.Compania, cliente.Codigo);

                        cia = new Compania(clienteCompania.Compania);
                        cia.Cargar();
                        cia.CargarConfiguracionImpuestos();

                        config = new ConfigDocCia(pais, clasedoc, condicion, clienteCompania.NivelPrecio, cia, clienteCompania);

                        Gestor.Pedidos.CargarConfiguracionVenta(clienteCompania.Compania, config);

                        Pedidos.FacturarPedido = true;

                        foreach (TomaFisicaInventario item in lstArtFacturar)
                        {
                            string artCompania = item.Compania;
                            string codArticulo = item.Articulo;
                            string bodega = item.BodegaCamion;
                            string localizacion = item.Localizacion;
                            string lote = item.Lote;
                            decimal cantidad = item.CantidadFacturar;
                            string boleta = item.Consecutivo.ToString();

                            articulo = new Articulo(codArticulo, artCompania);

                            articulo.CargarDatosArticulo(codArticulo, artCompania);
                            articulo.UnidadEmpaque = Articulo.ObtenerFactorEmpaque(codArticulo, artCompania);
                            articulo.CargarExistencia(bodega);

                            articulo.CargarPrecio(clienteCompania.NivelPrecio);

                            GestorUtilitario.CalculaUnidades(cantidad, 0, Convert.ToInt32(articulo.UnidadEmpaque), out unidadesAlm, out unidadesDet);

                            Gestor.Pedidos.GestionarTomaFisica(articulo, codigoRuta, new Precio(articulo.Precio.Maximo, articulo.Precio.Minimo), unidadesAlm, unidadesDet, false, string.Empty);

                            tomaFisica.ActualizarEstadoBoleta(Convert.ToInt32(boleta), TomaFisicaInventario.BOLETA_FACTURADA);
                        }
                        Gestor.Pedidos.Gestionados[0].Notas = "Diferencias de inventarios en la PDA: " + this.HandHeld;
                        Gestor.Pedidos.CargarConsecutivos();
                        //Gestor.Pedidos.GuardarPedidos();

                        //if (Impresora.SugerirImprimir)
                        clienteImpresion = null;

                            if (Impresora.SugerirImprimir)
                            {
                                clienteImpresion = cliente;                               
                                
                            }

                        pedidoImpresion = Gestor.Pedidos.Gestionados[0];
                        Gestor.Pedidos.GuardarPedidosFactDirerencias();
                        Gestor.Pedidos.LimpiarValores();
                        Gestor.Pedidos.Gestionados.Clear();

                    }
                    else
                    {
                        this.mostrarAlerta("El proceso de facturación no puede llevarse a cabo ya que el cliente rutero no esta asociado.");
                        procesoExitoso = false;
                    }
                }
                else 
                {
                    this.mostrarAlerta("Error al tratar de facturar las diferencias encontradas. ");
                }
            }
            catch (Exception ex)
            {
                this.mostrarAlerta("Problemas a la hora de Generar Factura por Diferencias. " + ex.Message);
                procesoExitoso = false;
            }

            return procesoExitoso;
        }

        private bool ImprimirDocumentoInventario()
        {
            bool resultado = true;
            try
            {
                DetallesInventario datos;
                ReporteInventario reporte = null;
                List<DetalleInventario> datos_busqueda;

                IObservableCollection<string> Comp;
                Comp = new SimpleObservableCollection<string>() { Compania.Obtener(CompaniaRuta).Codigo };
                //List<Compania> compania = new List<Compania>();
                //compania.Add(Compania.Obtener(CompaniaRuta));

                datos = Bodega.CargarInventarioTomaFisica(BodegaCamion, CompaniaRuta);
                datos_busqueda = datos.Lista;

                reporte = new ReporteInventario(this, BodegaCamion, Comp, datos.Lista);
                reporte.Imprime(true);
            }
            catch (FormatException)
            {
                resultado = false;
                this.mostrarAlerta("Error Formato inválido.");
            }
            catch (Exception ex)
            {
                resultado = false;
                this.mostrarAlerta(ex.Message);
            }

            return resultado;
        }

        private bool ImprimirDocumento(Cliente cliente)
        {
            bool resultado = true;            
            try
            {
                int cantidad = 1;

                if (Pedidos.FacturarPedido)
                {
                    if (cantidad >= 0)
                    {
                        Gestor.Pedidos.Gestionados.Add(pedidoImpresion);
                        foreach (Pedido pedido in Gestor.Pedidos.Gestionados)
                            pedido.LeyendaOriginal = true;

                        Pedidos pedidosImprimir = new Pedidos(Gestor.Pedidos.Gestionados, cliente);
                        pedidosImprimir.ImprimeDetalleFactura(cantidad, DetalleSort.Ordenador.Codigo);

                        foreach (Pedido pedido in Gestor.Pedidos.Gestionados)
                            pedido.Impreso = true;
                        Gestor.Pedidos.LimpiarValores();
                        Gestor.Pedidos.Gestionados.Clear();
                    }
                    else
                    {
                        this.mostrarMensaje(Mensaje.Accion.Informacion, "Solo se guardará la factura.");
                    }
                }                
            }
            catch (FormatException)
            {
                resultado = false;
                Gestor.Pedidos.LimpiarValores();
                Gestor.Pedidos.Gestionados.Clear();
                this.mostrarAlerta("Error obteniendo la cantidad de copias. Formato inválido.");
            }
            catch (Exception ex)
            {
                resultado = false;
                Gestor.Pedidos.LimpiarValores();
                Gestor.Pedidos.Gestionados.Clear();
                this.mostrarAlerta(ex.Message);
            }

            return resultado;
        }
        #endregion Otras Funciones

        #endregion


    }
}