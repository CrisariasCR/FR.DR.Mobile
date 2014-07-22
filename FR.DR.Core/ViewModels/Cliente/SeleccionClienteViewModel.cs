using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
//using Android.Content;
using Android.OS;
//using Android.Runtime;
using Android.Views;
using Android.Widget;

using System.Windows.Input;
using Cirrious.MvvmCross.Commands;
using FR.DR.Core.Helper;
using FR.Core.Model;
using Softland.ERP.FR.Mobile.UI;
using Softland.ERP.FR.Mobile.Cls.Corporativo;
using Softland.ERP.FR.Mobile.Cls.FRCliente;
using Softland.ERP.FR.Mobile.Cls.Utilidad;
using Softland.ERP.FR.Mobile.Cls.FRCliente.FRVisita;
using Softland.ERP.FR.Mobile.Cls;
using Softland.ERP.FR.Mobile.Cls.FRArticulo;
using System.Windows.Forms;

namespace Softland.ERP.FR.Mobile.ViewModels
{
    public class SeleccionClienteViewModel : ListViewModel
    {
#pragma warning disable 414
#pragma warning disable 649

        public SeleccionClienteViewModel()
        {
            CargaInicial();
        }

        private void CargaInicial()
        {
            ventanaInactiva = false;
            Clientes = new SimpleObservableCollection<Cliente>();
            ItemsComboDia = new SimpleObservableCollection<DiaSemana>()
            {
                DiaSemana.Todos, DiaSemana.Lunes, DiaSemana.Martes, DiaSemana.Miercoles, DiaSemana.Jueves,
                DiaSemana.Viernes, DiaSemana.Sabado, DiaSemana.Domingo
            };
            DiaActual = (DiaSemana)((int)DateTime.Now.DayOfWeek);

            ItemsComboEstadoVisita = new SimpleObservableCollection<EstadoVisita>()
            {
                EstadoVisita.Todos, EstadoVisita.NoVisitados, EstadoVisita.Visitados
            };
            EstadoVisitaActual = EstadoVisita.Todos;

            CriteriosBusqueda = new SimpleObservableCollection<CriterioCliente>() { CriterioCliente.Codigo, CriterioCliente.Nombre };
            CriterioSeleccionado = CriterioCliente.Codigo;

            OrdenAlfabetico = false;

            #region Carga de Datos

            GlobalUI.Rutas = Cls.Corporativo.Ruta.ObtenerRutas();
            Rutas = new SimpleObservableCollection<Ruta>(GlobalUI.Rutas);
            if (Rutas.Count > 0)
            {
                RutaActual = Rutas[0];
            }
            #endregion
        }

        #region Propiedades

        private bool ultimaBusquedaXCodigo = false;
        private bool ultimaBusquedaXCodigoBarras = false;
        public static bool ventanaInactiva = false;     

        public IObservableCollection<DiaSemana> ItemsComboDia { get; set; }
        private DiaSemana diaActual;
        public DiaSemana DiaActual
        {
            get { return diaActual; }
            set { diaActual = value; CargaClientes(); }
        }

        public IObservableCollection<EstadoVisita> ItemsComboEstadoVisita { get; set; }
        private EstadoVisita estadoVisitaActual;
        public EstadoVisita EstadoVisitaActual
        {
            get { return estadoVisitaActual; }
            set { estadoVisitaActual = value; CargaClientes(); }
        }

        public IObservableCollection<CriterioCliente> CriteriosBusqueda { get; set; }
        public CriterioCliente CriterioSeleccionado { get; set; }

        public IObservableCollection<Ruta> Rutas { get; set; }
        private Ruta rutaActual;
        public Ruta RutaActual
        {
            get { return rutaActual; }
            set
            {
                rutaActual = value;
                GlobalUI.RutaActual = value;
                CargaClientes();
            }
        }

        private IObservableCollection<Cliente> clientes;
        public IObservableCollection<Cliente> Clientes
        {
            get { return this.clientes; }
            set { this.clientes = value; RaisePropertyChanged("Clientes"); }
        }

        private bool ordenAlfabetico;
        public bool OrdenAlfabetico
        {
            get { return ordenAlfabetico; }
            set { ordenAlfabetico = value; CambiarOrdenAlfabetico(); }
        }

        private string textoBusqueda;
        public string TextoBusqueda
        {
            get { return textoBusqueda; }
            set
            {
                if (value != textoBusqueda && !ventanaInactiva)
                {
                    
                    textoBusqueda = value;
                    RaisePropertyChanged("TextoBusqueda");
                }
            }
        }

        private bool gpsEnabled=false;
        public bool GpsEnabled
        {
            get { return gpsEnabled; }
            set { gpsEnabled = value; RaisePropertyChanged("GpsEnabled"); }
        }

        private Cliente clienteSeleccionado;
        public Cliente ClienteSeleccionado
        {
            get { return clienteSeleccionado; }
            set { clienteSeleccionado = value; SeleccionarCliente(); RaisePropertyChanged("ClienteSeleccionado"); }
        }

        private bool RefrescarPress = false;
        #endregion

        #region Comandos

        public ICommand ComandoRefrescar
        {
            get { RefrescarPress = true; return new MvxRelayCommand(CargaGeneralClientes); }
        }

        public ICommand ComandoConsultar
        {
            get { return new MvxRelayCommand(ConsultarCliente); }
        }

        public ICommand ComandoGPS
        {
            get { return new MvxRelayCommand(IrGPS); }
        }

        public ICommand ComandoIniciar
        {
            get { return new MvxRelayCommand(Iniciar); }
        }

        public ICommand ComandoVisitar
        {
            get { return new MvxRelayCommand(Visitar); }
        }

        #endregion

        #region Acciones

        public void CargaGeneralClientes()
        {
            //string textoEscaneado = exlblCliente.Text;
            //this.comboDia.SelectedIndex = 0;
            //this.cboEstadoVisitas.SelectedIndex = 0;
            //if (this.cboRuta.Enabled)
            //    this.cboRuta.SelectedIndex = 0;
            if (string.IsNullOrEmpty(TextoBusqueda) && RefrescarPress) 
            {
                return;
            }
            RefrescarPress = false;
            ultimaBusquedaXCodigo = true;
            //exlblCliente.Text = textoEscaneado;

            //this.lsvClientes.Items.Clear();// borra todo los items del listview
            Clientes.Clear();

            try
            {

                //llama al metodo que carga los clientes para la ruta especifica
                Clientes = new SimpleObservableCollection<Cliente>(Cliente.CargarClientes(new CriterioBusquedaCliente(CriterioSeleccionado,
                        TextoBusqueda, OrdenAlfabetico, false)));
            }
            catch (Exception ex)
            {
                this.mostrarAlerta("Error cargando los clientes. " + ex.Message);
            }
            this.CargarListaClientes(true);
        }

        public void ConsultarCliente()
        {
            if (ClienteSeleccionado != null)
            {
                //GC.Collect();
                this.RequestNavigate<OpcionesClienteViewModel>();
            }
        }

        private void Visitar()
        {
            if (this.Validacion())
            {
                GlobalUI.VisitaActual = new Visita();
                if (GlobalUI.VisitaActual.ValidarVisita(GlobalUI.ClienteActual.Codigo))
                {
                    this.mostrarMensaje(Mensaje.Accion.Decision, "registrar otra visita para el cliente", result =>
                    {
                        if (result != DialogResult.No)
                            MostrarVisita();
                    });
                }
                else
                    MostrarVisita();
            }            
        }

        private void MostrarVisita()
        {
            //GC.Collect();
                // Se inicia el proceso para obtener la ubicación
                if (!FRmConfig.NoDetenerGPS)
                {
                    //Ubicacion.Iniciar(Ubicacion.TIEMPO_ESPERA_OMISION);
                }
                GlobalUI.VisitaActual = new Visita();
                GlobalUI.VisitaActual.Cliente.Codigo = GlobalUI.ClienteActual.Codigo;
                GlobalUI.VisitaActual.Cliente.Nombre = GlobalUI.ClienteActual.Nombre;
                GlobalUI.VisitaActual.Cliente.Zona = GlobalUI.ClienteActual.Zona;
                GlobalUI.VisitaActual.Tipo = this.tipoVisita;

                this.RequestDialogNavigate<VisitaViewModel, bool>(null, resultado =>
                {
                    if (!FRmConfig.NoDetenerGPS)
                    {
                        //Ubicacion.Parar();
                    }
                    //Caso 30523 LDS 07/11/2007
                    //Llama a la función que carga los clientes para esta ruta según los filtros.
                    this.CargaClientes();
                });
            
        }

        #endregion

        #region Metodos Logica de Negocio

        public void SeleccionarCliente()
        {

            if (ClienteSeleccionado !=null)
            {

                //LJR Asignar la ruta, si la busqueda se realizo en forma "Agil"
                if (RutaActual == null)
                    GlobalUI.RutaActual = Ruta.ObtenerRuta(GlobalUI.Rutas, ClienteSeleccionado.Zona);

                try
                {
                    GlobalUI.ClienteActual = ClienteSeleccionado;// le da a la variable globa de cliente seleccionado
                }
                catch (Exception ex)
                {
                    this.mostrarAlerta("Error en la selección del cliente. " + ex.Message);
                }

                if (FRmConfig.SeleccionXCodBarras)
                {
                    //Valida si el cliente fue seleccionado
                    //por escaneo o por seleccion manual

                    //if (this.exlblCliente.Text.Equals(client.Codigo) && ultimaBusquedaXCodigoBarras)
                    //    this.tipoVisita = TipoVisita.Real;
                    //else
                    //    this.tipoVisita = TipoVisita.Telefonica;

                    ////Limpiamos el campo para busqueda de clientes por codigo de barras
                    //this.exlblCliente.Text = string.Empty;
                }

                //Si No se ha cargado informacion de las companias para el cliente
                try
                {
                    GlobalUI.ClienteActual.ObtenerClientesCia();

                }
                catch (Exception ex)
                {
                    this.mostrarAlerta("Error cargando datos del cliente en compañías. " + ex.Message);
                }


                // (Des)Habilita las opciones para guardar la posición del cliente.
                HabilitarOpcionesGPS(GlobalUI.ClienteActual);
            }

        }

        private void CargarListaClientes(bool busquedaAgil)
        {
            //Caso 34430 Cargar el listview de manera distinta si es busqueda agil
            //if (GlobalUI.Rutas.Count > 1 && busquedaAgil && !this.lsvClientes.Columns.Contains(this.colRuta))
            //    this.lsvClientes.Columns.Add(this.colRuta);
            //else if (!busquedaAgil && this.lsvClientes.Columns.Contains(this.colRuta))
            //    this.lsvClientes.Columns.Remove(this.colRuta);

            //foreach (Cliente cliente in clientesCargados)
            //{
            //    ListViewItem row;// = new ListViewItem(colums);

            //    if (busquedaAgil)
            //        row = new ListViewItem(new string[] { cliente.Nombre, cliente.Codigo, cliente.Zona, string.Empty });
            //    else
            //        row = new ListViewItem(new string[] { cliente.Nombre, cliente.Codigo });
            //    this.lsvClientes.Items.Add(row);
            //}
        }

        public void Regresar() 
        {
            RequestNavigate<MenuPrincipalViewModel>();
            this.DoClose();
        }

        private void CargaClientes()
        {
            ultimaBusquedaXCodigo = ultimaBusquedaXCodigoBarras = false;
            TextoBusqueda = "";

            Clientes.Clear();

            //Verifica que la ruta del combo no sea la posicion cero (-Ruta-)
            //y que hay un dia seleccionado
            if (RutaActual != null)
            {
                try
                {                    
                    //Caso 26278 LDS 16/10/2007 llama al metodo que carga los clientes para la ruta especifica
                    Clientes = new SimpleObservableCollection<Cliente>(Cliente.CargarClientes(
                        new CriterioBusquedaCliente(
                        RutaActual.Codigo,
                        GestorUtilitario.Dia(DiaActual.Description()),
                        EstadoVisitaActual.Description(),
                        OrdenAlfabetico)));
                }
                catch (Exception ex)
                {
                    string error = ex.Message;
                    //Mensaje.mostrarAlerta("Error cargando los clientes. " + ex.Message);
                }
                this.CargarListaClientes(false);
            }
        }

        private void CambiarOrdenAlfabetico()
        {
            if (ultimaBusquedaXCodigo)
                this.CargaGeneralClientes();
            else //Llama a la función que carga los clientes para esta ruta
                this.CargaClientes();
        }

        TipoVisita tipoVisita;
        public void Iniciar()
        {
            if (this.Validacion())
            {
                this.ValidarDescuentos();
            }
        }

        public void IrGPS()
        {
            RequestNavigate<UbicacionClienteViewModel>();
        }

        private void IniciarMenuCliente(bool habilitarPedidos)
        {
            //GC.Collect();
            GlobalUI.VisitaActual = new Visita();
            GlobalUI.VisitaActual.Cliente.Codigo = GlobalUI.ClienteActual.Codigo;
            GlobalUI.VisitaActual.Cliente.Nombre = GlobalUI.ClienteActual.Nombre;
            GlobalUI.VisitaActual.Cliente.Zona = GlobalUI.ClienteActual.Zona;
            GlobalUI.VisitaActual.Tipo = this.tipoVisita;
            MenuClienteViewModel.habilitarPed = habilitarPedidos;
            if (FRmConfig.GuardarUbicacionVisita && !FRmConfig.NoDetenerGPS)
            {
                //Ubicacion.Iniciar(Ubicacion.TIEMPO_ESPERA_OMISION);
            }
            this.DoClose();
            Dictionary<string, object> Parametros = new Dictionary<string, object>()
                {
                    {"habilitarPedidos", habilitarPedidos}
                };
            this.RequestNavigate<MenuClienteViewModel>(Parametros);
        }

        private bool ValidarDescuentos()
        {
            DateTime fechaFinDescuento = DateTime.Now;

            try
            {
                fechaFinDescuento = Ruta.ObtenerFechaFin();
            }
            catch (Exception ex)
            {
                this.mostrarAlerta("Error consultando fecha fin de descuentos. " + ex.Message);
            }

            if (fechaFinDescuento.Date < DateTime.Now.Date)
            {
                string mensaje = "La información de descuentos se encuentra vencida.";

                if (Descuento.PermitirInfoDescuentosVencida)
                {
                    this.mostrarMensaje(Mensaje.Accion.Informacion, mensaje, result =>
                    {
                        IniciarMenuCliente(true);
                    });
                    return true;
                }
                else
                {
                    mensaje += " No podrá realizar pedidos hasta realizar una carga de datos.";

                    this.mostrarAlerta(mensaje, result =>
                    {
                        IniciarMenuCliente(false);
                    });
                    return false;
                }
            }
            else
            {
                IniciarMenuCliente(true);
                return true;
            }
        }

        private bool Validacion()
        {
            if (this.ClienteSeleccionado == null)
            {
                this.mostrarMensaje(Mensaje.Accion.SeleccionNula, "un cliente");
                return false;
            }
            return true;
        }

        public void OnResume()
        {
            ventanaInactiva = false;
            RaisePropertyChanged("TextoBusqueda");

        }

        private void HabilitarOpcionesGPS(Cliente cliente)
        {
            bool tieneUbicacion = ClienteUbicacion.TieneUbicacion(cliente.Zona, cliente.Codigo);
            bool mostrar = (FRmConfig.GuardarUbicacionClientes && !tieneUbicacion)
                || (tieneUbicacion && FRmConfig.ActualizarUbicacionClientes);
            GpsEnabled=mostrar;
        }

        #endregion

    }

    public enum DiaSemana
    {
        [Description("Todos")]
        Todos = 0,
        [Description("Lunes")]
        Lunes = 1,
        [Description("Martes")]
        Martes = 2,
        [Description("Miércoles")]
        Miercoles = 3,
        [Description("Jueves")]
        Jueves = 4,
        [Description("Viernes")]
        Viernes = 5,
        [Description("Sábado")]
        Sabado = 6,
        [Description("Domingo")]
        Domingo = 7,
    }

    public enum EstadoVisita
    {
        [Description("Todos")]
        Todos = 0,
        [Description("No Visitados")]
        NoVisitados = 1,
        [Description("Visitados")]
        Visitados = 3
    }
}