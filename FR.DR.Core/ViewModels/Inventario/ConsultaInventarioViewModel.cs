using System;
//using System.Net;
//using System.Windows;
using System.Windows.Input;
using System.Collections.Generic;
using Cirrious.MvvmCross.Commands;

using FR.Core.Model;
//using Softland.ERP.FR.Mobile;
using Softland.ERP.FR.Mobile.UI;
using Softland.ERP.FR.Mobile.Cls;
using Softland.ERP.FR.Mobile.Cls.Documentos.FRInventario;
//using Softland.ERP.FR.Mobile.Cls.FRArticulo;
using Softland.ERP.FR.Mobile.Cls.FRCliente;
//using Softland.ERP.FR.Mobile.Cls.Utilidad;

namespace Softland.ERP.FR.Mobile.ViewModels
{
    public class ConsultaInventarioViewModel : BaseViewModel
    {
        public static bool SeleccionCliente;

        #region Propiedades

        public IObservableCollection<string> Header { get { return new SimpleObservableCollection<string>() { "Header" }; } }
        
        public string NameCliente
        {
            get
            {
                return " Cod." + GlobalUI.ClienteActual.Codigo + "-" + GlobalUI.ClienteActual.Nombre;
            }
        }

        #region Items e ItemActual
        public static Inventario ItemActual { get; set;}

        public Inventario inventarioActual;
        public Inventario InventarioActual
        {
            get { return inventarioActual; }
            set
            {
                if (value != inventarioActual)
                {
                    inventarioActual = value; ItemActual = inventarioActual;
                    RaisePropertyChanged("InventarioActual");
                }
            }
        }

        public IObservableCollection<Inventario> Items { get; set; }
        #endregion Items e ItemActual

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
        #endregion Propiedades

        public ConsultaInventarioViewModel() 
        {
            CargaInicial();
        }

        #region CargaInicial

        public void CargaInicial()
        {

            NombreCliente = " Código: " + GlobalUI.ClienteActual.Codigo + "\n" +
                            " Cliente: " + GlobalUI.ClienteActual.Nombre;

            FRmConfig.EnConsulta = true;

            try
            {
                List<Inventario> lista = Inventario.ObtenerInventarios(GlobalUI.ClienteActual.Codigo);
                this.Items = new SimpleObservableCollection<Inventario>(lista);
                RaisePropertyChanged("Items");
            }
            catch (Exception ex)
            {
                this.mostrarAlerta("Error cargando inventarios del cliente. " + ex.Message);
            }


            // el binding al list view es de 3 columnas fijas:
            // Compania, Numero y FechaRealizacion 
            // este ultimo con un convertidor a hilera que haga lo mismo que GestorUtilitario.ObtenerFechaString
        }

        #endregion CargaInicial

        #region Comandos

        public ICommand ComandoConsultarDetalle
        {
            get { return new MvxRelayCommand<Inventario>(ConsultarDetalle); }
        }

        #endregion Comandos

        #region Acciones

        public void ConsultarDetalle(Inventario inventarioSeleccionado)
        {
            inventarioSeleccionado = ItemActual;
            if (inventarioSeleccionado != null)
            {
                if (inventarioSeleccionado.Detalles.Vacio())
                {
                    try
                    {
                        inventarioSeleccionado.ObtenerDetalles(/*GlobalUI.RutaActual.GrupoArticulo*/);
                    }
                    catch (Exception ex)
                    {
                        this.mostrarAlerta("Error cargando detalles del inventario. " + ex.Message);
                    }
                }

                if (inventarioSeleccionado.Detalles.Lista.Count > 0)
                {
                    //Dictionary<string, string> pars = new Dictionary<string, string>() { { "from", "ConsultaInventario" } };
                    //this.RequestNavigate<DetalleInventarioViewModel>(pars);

                    //this.RequestNavigate<DetalleInventarioViewModel>();
                    Dictionary<string, object> parametros = new Dictionary<string, object>();
                    parametros.Add("inventarios", "N");
                    parametros.Add("messageid", "messageid");

                    this.RequestDialogNavigate<DetalleInventarioViewModel, bool>(parametros, result =>
                        {
                            //Preparamos la lista de inventarios en gestion
                            Gestor.Inventario.Gestionados.Clear();
                            Gestor.Inventario.Gestionados.Add(inventarioSeleccionado);

                            //Debemos abrir el de cantidades por si el usuario desea modificar el inventario consultado
                            this.DoClose();
                            this.RequestNavigate<TomaInventarioViewModel>();
                        }
                        ); 

                }
            }

        }

        public override void DoClose()
        {
            base.DoClose();
            if (!SeleccionCliente)
            {
                Dictionary<string, object> Parametros = new Dictionary<string, object>()
                                {
                                    {"habilitarPedidos", true}
                                };
                this.RequestNavigate<MenuClienteViewModel>(Parametros);
            }
            else
            {
                //this.RequestNavigate<OpcionesClienteViewModel>(); 
            }
        }

        #endregion Accioness
    }
}
