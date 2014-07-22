using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Softland.ERP.FR.Mobile.Cls.Documentos.FRPedido;
using Softland.ERP.FR.Mobile.Cls.FRCliente;
using FR.Core.Model;
using Softland.ERP.FR.Mobile.Cls.FRArticulo;
using Cirrious.MvvmCross.Commands;
using System.Windows.Input;
using Softland.ERP.FR.Mobile.Cls;
using System.Windows.Forms;
using Softland.ERP.FR.Mobile.Cls.Utilidad;
using Softland.ERP.FR.Mobile.UI;

namespace Softland.ERP.FR.Mobile.ViewModels
{
    public class AsignaLotesViewModel : ListViewModel
    {
        public AsignaLotesViewModel(string bodega = "")
        {
            RaisePropertyChanged("DetalleLinea");
            this.bodega = bodega;
            CargarPantalla();
        }

        #region Propiedades

        public static DetallePedido DetalleLinea { get; set; }
        private string bodega;
        public bool EsBarras = false;

        public string NameCliente
        {
            get
            {
                return " Cod." + GlobalUI.ClienteActual.Codigo + "-" + GlobalUI.ClienteActual.Nombre;
            }
        }

        private IObservableCollection<string> companias;
        public IObservableCollection<string> Companias
        {
            get { return companias; }
            set { companias = value; RaisePropertyChanged("Companias"); }
        }

        private string companiaActual;
        public string CompaniaActual
        {
            get { return companiaActual; }
            set { companiaActual = value; RaisePropertyChanged("CompaniaActual"); }
        }

        private decimal asignadasAlmacen;
        public decimal AsignadasAlmacen
        {
            get { return asignadasAlmacen; }
            set { asignadasAlmacen = value; CalculaSubTotal(); RaisePropertyChanged("AsignadasAlmacen"); }
        }

        private decimal asignadasDetalle;
        public decimal AsignadasDetalle
        {
            get { return asignadasDetalle; }
            set { asignadasDetalle = value; CalculaSubTotal(); RaisePropertyChanged("AsignadasDetalle"); }
        }

        private string unidadesAlmacen;
        public string UnidadesAlmacen
        {
            get { return unidadesAlmacen; }
            set { unidadesAlmacen = value; RaisePropertyChanged("UnidadesAlmacen"); }
        }

        private string unidadesDetalle;
        public string UnidadesDetalle
        {
            get { return unidadesDetalle; }
            set { unidadesDetalle = value; RaisePropertyChanged("UnidadesDetalle"); }
        }

        private IObservableCollection<string> criterios;
        public IObservableCollection<string> Criterios
        {
            get { return criterios; }
            set { criterios = value; RaisePropertyChanged("Criterios"); }
        }

        private string criterioActual;
        public string CriterioActual
        {
            get { return criterioActual; }
            set { criterioActual = value; textoBusqueda = ""; RaisePropertyChanged("CriterioActual"); }
        }

        private IObservableCollection<Lotes> lotes;
        public IObservableCollection<Lotes> LotesArticulo
        {
            get { return lotes; }
            set { lotes = value; RaisePropertyChanged("LotesArticulo"); }
        }

        private Lotes loteSeleccionado;
        public Lotes LoteSeleccionado
        {
            get { return loteSeleccionado; }
            set { loteSeleccionado = value; CargarDetalleLote(); RaisePropertyChanged("LoteSeleccionado"); }
        }

        private decimal UnidadAsigAlm = decimal.Zero;
        private decimal UnidadAsigDet = decimal.Zero;

        private string textoBusqueda = string.Empty;
        public virtual string TextoBusqueda
        {
            get { return textoBusqueda; }
            set { if (value != textoBusqueda) { textoBusqueda = value; RaisePropertyChanged("TextoBusqueda"); CambioTextoBusqueda(); } }
        }

        #endregion

        #region Logica

        private void CargarPantalla()
        {
            Companias = new SimpleObservableCollection<string>() { DetalleLinea.Articulo.Compania };
            CompaniaActual = Companias[0];

            AsignadasDetalle = AsignadasAlmacen = 0;

            //this.ltbCodigo.Text = detalleLinea.Articulo.Codigo;
            //this.ltbDescripcion.Text = detalleLinea.Articulo.Descripcion;
            UnidadesAlmacen = DetalleLinea.UnidadesAlmacen + "/ (" + DetalleLinea.UnidadesAlmacen + ")";
            UnidadesDetalle = DetalleLinea.UnidadesDetalle + "/ (" + DetalleLinea.UnidadesDetalle + ")";

            CargarLotesArticulo();

            Criterios = new SimpleObservableCollection<string>() { "Manual", "Código Barras" };
            CriterioActual = Criterios[0];
        }

        private void CargarLotesArticulo()
        {
            AsignadasDetalle = AsignadasAlmacen = 0;

            if (DetalleLinea.Articulo.LotesAsociados.Count == 0)
                DetalleLinea.Articulo.CargarLotesArticulo(this.bodega);

            LotesArticulo = new SimpleObservableCollection<Lotes>(DetalleLinea.Articulo.LotesAsociados);

            var lista = LotesArticulo.Where(x => x.CantidadAlmacen > 0 || x.CantidadDetalle > 0);

            AsignadasAlmacen = lista.Sum(x => x.CantidadAlmacen);
            AsignadasDetalle = lista.Sum(x => x.CantidadDetalle);
            foreach (Lotes lote in lista)
            {
                ActualizarDetalle(true, LoteSeleccionado.CantidadAlmacen, LoteSeleccionado.CantidadDetalle);
            }

            RaisePropertyChanged("LotesArticulo");
        }

        private void ActualizarDetalle(bool agregar, decimal cantAlm, decimal cantDet)
        {
            decimal cantidadAlmacen = DetalleLinea.UnidadesAlmacen;
            decimal cantidadDetalle = DetalleLinea.UnidadesDetalle;

            cantidadAlmacen = DetalleLinea.UnidadesAlmacen - AsignadasAlmacen;
            cantidadDetalle = DetalleLinea.UnidadesDetalle - AsignadasDetalle;

            UnidadesAlmacen = DetalleLinea.UnidadesAlmacen.ToString() + "/ (" + cantidadAlmacen.ToString() + ")";
            UnidadesDetalle = DetalleLinea.UnidadesDetalle.ToString() + "/ (" + cantidadDetalle.ToString() + ")";

            if (agregar && cantidadAlmacen == decimal.Zero && cantidadDetalle == decimal.Zero)
                DetalleLinea.DesglosoLote = true;
            else
                DetalleLinea.DesglosoLote = false;
        }

        private void BuscarListaLotes()
        {
            bool encontro = false;
            
            if (TextoBusqueda.EndsWith(FRmConfig.CaracterDeRetorno))
                TextoBusqueda = TextoBusqueda.Remove(TextoBusqueda.Length - 1, 1);

            foreach (Lotes loteActual in LotesArticulo)
            {
                if (CriterioActual == Criterios[1])
                {
                    if (loteActual.Lote.ToUpper().Contains(TextoBusqueda.ToUpper())) //== criterio)
                    {
                        decimal unidad = decimal.Zero;
                        //this.lstLotes.EnsureVisible(item.Index);
                        //item.Selected = true;
                        encontro = true;

                        if (AsignadasAlmacen == 0)
                        {
                            unidad = AsignadasAlmacen;
                            unidad++;
                            AsignadasAlmacen = unidad;
                        }
                        break;
                    }
                }
                else
                {
                    if (loteActual.Lote.ToUpper().Contains(TextoBusqueda.ToUpper()))
                    {
                        //this.lstLotes.EnsureVisible(item.Index);
                        //item.Selected = true;
                        encontro = true;
                        break;
                    }
                }
            }

            LotesArticulo = new SimpleObservableCollection<Lotes>(LotesArticulo.Where(x=> x.Lote.ToUpper().Contains(TextoBusqueda.ToUpper())).ToList());

            if (!encontro)
            {
                this.mostrarAlerta("El lote no se encuentra.");
            }
        }

        private void CargarDetalleLote()
        {
            if (LoteSeleccionado != null)
            {
                int index = DetalleLinea.buscarLote(LoteSeleccionado);

                if (index != -1)
                {
                    AsignadasAlmacen = DetalleLinea.LotesLinea[index].CantidadAlmacen;
                    AsignadasDetalle = DetalleLinea.LotesLinea[index].CantidadDetalle;
                }
                else
                {
                    AsignadasAlmacen = AsignadasDetalle = 0;
                }
            }
        }

        private void AgregarModificarEliminarLote(bool agregar)
        {
            bool result = true;

            if (LoteSeleccionado != null)
            {
                if (this.UnidadAsigAlm > 0)
                    this.UnidadAsigAlm = this.UnidadAsigAlm - LoteSeleccionado.CantidadAlmacen;
                if (this.UnidadAsigDet > 0)
                    this.UnidadAsigDet = this.UnidadAsigDet - LoteSeleccionado.CantidadDetalle;

                if (agregar)
                {
                    if (AsignadasAlmacen > (DetalleLinea.UnidadesAlmacen - this.UnidadAsigAlm) ||
                        AsignadasDetalle > (DetalleLinea.UnidadesDetalle - this.UnidadAsigDet))
                    {
                        result = false;
                        this.mostrarAlerta("La cantidad no puede ser mayor a la indicada en el detalle.");
                    }

                    if (result)
                    {
                        LoteSeleccionado.CantidadAlmacen = AsignadasAlmacen;
                        LoteSeleccionado.CantidadDetalle = AsignadasDetalle;

                        this.UnidadAsigAlm = this.UnidadAsigAlm + LoteSeleccionado.CantidadAlmacen;
                        this.UnidadAsigDet = this.UnidadAsigDet + LoteSeleccionado.CantidadDetalle;

                        if (LoteSeleccionado.ExistenciaParcial >= 0)
                        {
                            try
                            {
                                DetalleLinea.AgregarLote(LoteSeleccionado);
                            }
                            catch (Exception ex)
                            {
                                result = false;
                                this.mostrarAlerta("Error Asignación de lotes: " + ex.ToString());
                            }
                        }
                        else
                        {
                            result = false;
                            this.mostrarAlerta("La cantidad disponible del lotes no satisface la cantidad asignada");
                        }
                    }

                }
                else
                {
                    LoteSeleccionado.CantidadAlmacen = 0;
                    LoteSeleccionado.CantidadDetalle = 0;

                    try
                    {
                        DetalleLinea.EliminarLote(LoteSeleccionado);
                    }
                    catch (Exception ex)
                    {
                        result = false;
                        this.mostrarAlerta("Error Asignación de lotes: " + ex.ToString());
                    }
                }

                if (result)
                {
                    //detalleLinea.Articulo.LotesAsociados[index] = lote;
                    ActualizarDetalle(agregar, AsignadasAlmacen, AsignadasDetalle);
                    CargarLotesArticulo();
                }

            }
            else
                this.mostrarAlerta("Debe de seleccionar un LoteSeleccionado.");
        }

        private void CalculaSubTotal()
        {
            decimal cantDetalle = 0;
            decimal cantAlmacen = 0;

            GestorUtilitario.CalculaUnidades(AsignadasAlmacen,
                AsignadasDetalle,
                Convert.ToInt32(DetalleLinea.Articulo.UnidadEmpaque),
                out cantAlmacen,
                out cantDetalle);

            asignadasDetalle = cantDetalle;
            asignadasAlmacen = cantAlmacen;
        }

        #endregion

        #region Comandos

        public ICommand ComandoRefrescar
        {
            get { return new MvxRelayCommand(Refrescar); }
        }

        public ICommand ComandoAceptar
        {
            get { return new MvxRelayCommand(Aceptar); }
        }

        public ICommand ComandoCancelar
        {
            get { return new MvxRelayCommand(Cancelar); }
        }

        public ICommand ComandoAgregar
        {
            get { return new MvxRelayCommand(Agregar); }
        }

        public ICommand ComandoRemover
        {
            get { return new MvxRelayCommand(Remover); }
        }

        #endregion

        #region Acciones

        public void Refrescar()
        {
            if (!string.IsNullOrEmpty(TextoBusqueda))
                BuscarListaLotes();
            else
            {
                this.mostrarAlerta("Ingrese un valor de busqueda.");
            }
        }

        private void CambioTextoBusqueda()
        {
            if (string.IsNullOrEmpty(TextoBusqueda))
            {
                LotesArticulo = new SimpleObservableCollection<Lotes>();
                //this.lstLotes.Items.Clear();
            }
            else if (TextoBusqueda != string.Empty && (CriterioActual == CriterioArticulo.Barras.ToString() || TextoBusqueda.EndsWith(FRmConfig.CaracterDeRetorno)))
            {
                BuscarListaLotes();
                TextoBusqueda = string.Empty;
                EsBarras = true;
            }
        }

        private void Cancelar()
        {
            this.mostrarMensaje(Mensaje.Accion.Decision, "salir de la asignación de lotes? Esto provocara que no se guarden los cambios hechos", result =>
            {
                if (result == DialogResult.Yes)
                {
                    DetalleLinea.Articulo.LotesAsociados.Clear();
                    DetalleLinea.LotesLinea.Clear();
                    DetalleLinea.DesglosoLote = false;
                    DoClose();
                }
            });
        }

        private void Agregar()
        {
            AgregarModificarEliminarLote(true);
        }

        private void Remover()
        {
            AgregarModificarEliminarLote(false);
        }

        private void Aceptar()
        {
            DoClose();
        }
        #endregion
    }
}