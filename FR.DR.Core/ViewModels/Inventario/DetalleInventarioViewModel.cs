using System;
//using System.Net;
using System.Collections.Generic;
using System.Linq;
//using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using Cirrious.MvvmCross.Commands;

using FR.Core.Model;
using Softland.ERP.FR.Mobile.Cls; // FrmConfig
using Softland.ERP.FR.Mobile.Cls.Documentos.FRInventario;
using Softland.ERP.FR.Mobile.Cls.FRArticulo;
using Softland.ERP.FR.Mobile.Cls.FRCliente;
using Softland.ERP.FR.Mobile.UI; //GlobalUI

namespace Softland.ERP.FR.Mobile.ViewModels
{
    public class DetalleInventarioViewModel : DialogViewModel<bool> //BaseViewModel
    {

        #region Propiedades

        public IObservableCollection<string> Header { get { return new SimpleObservableCollection<string>() { "Header" }; } }

        #region DetalleInventario DetalleInventario Actual
        public DetalleInventario ItemActual
        {
            get;
            set;
        }
        public IObservableCollection<DetalleInventario> Items { get; set; }
        #endregion DetalleInventario

        private bool esInventario;

        public string NameCliente
        {
            get
            {
                return " Cod." + GlobalUI.ClienteActual.Codigo + "-" + GlobalUI.ClienteActual.Nombre;
            }
        }


        #region Companías y CompaniaActual
        private string companiaActual;
        public string CompaniaActual
        {
            get { return companiaActual; }
            set
            {
                if (value != companiaActual)
                {
                    companiaActual = value;
                    RaisePropertyChanged("CompaniaActual");
                }
            }
        }

        public IObservableCollection<string> Companias { get; set; }
        #endregion Companías y CompaniaActual

        #region Criterios y CriterioActual

        private CriterioArticulo criterioActual;
        public CriterioArticulo CriterioActual
        {
            get { return criterioActual; }
            set
            {
                if (value != criterioActual)
                {
                    criterioActual = value;
                    RaisePropertyChanged("CriterioActual");
                }
            }
        }

        /// <summary>
        /// datasource de los criterios de filtro, deben conincidir con los del enumerado CriterioArticulo
        /// </summary>
        public IObservableCollection<CriterioArticulo> Criterios { get; set; }

        #endregion Criterios y CriterioActual

        private string textoBusqueda;
        public string TextoBusquedadi
        {
            get { return textoBusqueda; }
            set
            {
                if (value != textoBusqueda)
                {
                    textoBusqueda = value;
                    RaisePropertyChanged("TextoBusqueda");
                    BusquedaCodigoBarras(value);
                }
            }
        }

        public bool EsBarras = false;

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

        // hay que hacer binding de estas propiedades

        private decimal totalArticulos;
        public decimal TotalArticulos
        {
            get { return totalArticulos; }
            set
            {
                if (value != totalArticulos)
                {
                    totalArticulos = value;
                    RaisePropertyChanged("TotalArticulos");
                }
            }
        }

        private Inventarios Inventarios;
        #endregion Propiedades

        public DetalleInventarioViewModel(string messageId,string inventarios)
            : base(messageId) 
        {
            esInventario = inventarios.Equals("S");
            if (inventarios.Equals("N"))
            {
                this.Inventarios = new Inventarios();
                this.Inventarios.Gestionados.Add(ConsultaInventarioViewModel.ItemActual);
            }
            else
            {
                this.Inventarios = Gestor.Inventario; // proviene de TomaInventarioModel 
            }

            CargaInicial();
        }

        #region CargaInicial

        public void CargaInicial()
        {
            NombreCliente = " Código: " + GlobalUI.ClienteActual.Codigo + "\n" +
                            " Cliente: " + GlobalUI.ClienteActual.Nombre;

            TextoBusquedadi = string.Empty;

            Companias = new SimpleObservableCollection<string>(Util.CargarCiasInventario(this.Inventarios.Gestionados));
            if (Companias.Count > 0)
            {
                CompaniaActual = Companias[0];
            }

            Criterios = new SimpleObservableCollection<CriterioArticulo>()
                    { CriterioArticulo.Codigo,
                      CriterioArticulo.Barras,
                      CriterioArticulo.Descripcion,
                      CriterioArticulo.Familia,
                      CriterioArticulo.Clase
                    };
            CriterioActual = CriterioArticulo.Codigo;
        }

        #endregion CargaInicial

        #region Comandos

        public ICommand ComandoConsultaArticulo
        {
            get { return new MvxRelayCommand(gestionIbtPropArt); }
        }

        public ICommand ComandoRetirarDetalle
        {
            get { return new MvxRelayCommand(gestionRetirarDetalle); }
        }


        public ICommand ComandoRefrescar
        {
            get { return new MvxRelayCommand(Refrescar); }
        }

        #endregion Comandos

        #region Acciones

        //public override void DoClose()
        //{
        //    base.DoClose();
        //    ReturnResult(true);
        //}

        public void Regresar() 
        {
            //this.DoClose();
            if (esInventario)
            {
                this.DoClose();
                this.RequestNavigate<TomaInventarioViewModel>();
            }
            ReturnResult(true);
        }

        public void Refrescar()
        {
            try
            {
                this.EjecutarBusquedaDetalles();
            }
            catch (Exception exc)
            {
                this.mostrarAlerta("Error :" + exc.Message);
            }

            TextoBusquedadi = string.Empty;
        }

        public void EjecutarBusquedaDetalles()
        {
            DetallesInventario detalles;

            if (TextoBusquedadi != null && CompaniaActual != null)
            {
                Inventario inv = this.Inventarios.Buscar(this.CompaniaActual);

                if (inv != null)
                {
                    detalles = inv.Detalles.Buscar(this.criterioActual, this.TextoBusquedadi, false);
                }
                else
                {
                    detalles = new DetallesInventario();
                }

                if (detalles.Vacio()&&CriterioActual!=CriterioArticulo.Barras)
                {
                    this.mostrarMensaje(Mensaje.Accion.BusquedaMala);
                }

                //Refrescamos el listView y los totales
                this.Items = new SimpleObservableCollection<DetalleInventario>(detalles.Lista);
                TotalArticulos = detalles.Lista.Sum(p => p.UnidadesAlmacen);
                RaisePropertyChanged("Items");
            }
        }

        private void gestionRetirarDetalle()
        {
            if (this.ItemActual != null)
            {
                this.mostrarMensaje(Mensaje.Accion.Retirar, "el artículo: " + ItemActual.Articulo.Codigo, 
                    result => 
                {
                    if (result == DialogResult.Yes)
                    {
                        try
                        {
                            //Gestionamos el inventario con cantidades 0
                            Inventarios.Gestionar(ItemActual.Articulo, GlobalUI.ClienteActual.Codigo, GlobalUI.RutaActual.Codigo, 0, 0);                           
                            TotalArticulos -= ItemActual.UnidadesAlmacen;
                            Items.Remove(ItemActual);
                            RaisePropertyChanged("Items");
                        }
                        catch (Exception e)
                        {
                            this.mostrarAlerta("Error eliminando la línea. " + e.Message);
                        }
                    }
                }) ;
            }
            else
            {
                this.mostrarMensaje(Mensaje.Accion.SeleccionNula, " un artículo");
                return;
            }
        }

        /// <summary>
        /// Metodo que se encarga de llamar al form de propiedades de articulos, para
        /// ver las propiedades del articulo seleccionado.
        /// </summary>
        private void gestionIbtPropArt()
        {
            if (this.ItemActual == null)
            {
                this.mostrarMensaje(Mensaje.Accion.SeleccionNula, "un artículo");
            }
            else
            {
                ConsultaArticuloViewModel.Articulo = this.ItemActual.Articulo;
                this.RequestNavigate<ConsultaArticuloViewModel>();
            }
        }

        /// <summary>
        /// invocar en TextoBusqueda_TextChanged
        /// </summary>
        public void BusquedaCodigoBarras(string texto)
        {
            // si termina con caracter de retorno (enter)
            if (texto != string.Empty && CriterioActual == CriterioArticulo.Barras)
            {
                //if (texto.EndsWith(FRmConfig.CaracterDeRetorno))
                //    texto = texto.Substring(0, texto.Length - 1);
                TextoBusquedadi = texto;

                //cambiamos el criterio a codigo de barras
                CriterioActual = CriterioArticulo.Barras;

                // se refresca la consulta
                this.EjecutarBusquedaDetalles();
                //TextoBusqueda = string.Empty;
                if(Items.Count>0)
                    EsBarras = true;
            }
        }
        #endregion Accioness
    }
}
