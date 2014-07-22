using System;
using System.Net;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Collections.Generic;
using System.Linq;

using Cirrious.MvvmCross.Commands;
using FR.Core.Model;
using Softland.ERP.FR.Mobile;
using Softland.ERP.FR.Mobile.Cls.FRArticulo;
using Softland.ERP.FR.Mobile.UI;
using Softland.ERP.FR.Mobile.Cls;
using Softland.ERP.FR.Mobile.Cls.Utilidad;
using Softland.ERP.FR.Mobile.Cls.FRCliente;
using Softland.ERP.FR.Mobile.Cls.Documentos.FRInventario;

namespace Softland.ERP.FR.Mobile.ViewModels
{
    public class TomaInventarioViewModel : BaseViewModel
    {
        /// <summary>
        /// Variable utilizada para evitar doble procesamiento en calculo de unidades.
        /// </summary>
        private bool inicial = true;
        private bool cargando = false;
        public bool Cargando { get { return cargando; } }
        public bool EsBarras = false;
        public static IObservableCollection<Articulo> listaTemporal = null;
        public static CriterioArticulo criterioTemporal = CriterioArticulo.Descripcion;
        public static bool esConsulta=false;

        #region Propiedades

        public IObservableCollection<string> Header { get { return new SimpleObservableCollection<string>() { "Hola" }; } }

        #region ListaInventarios
        private Articulo itemActual;
        public Articulo ItemActual
        {
            get { return itemActual; }
            set
            {
                itemActual = value; RaisePropertyChanged("ItemActual"); BuscarArticulo();
            }
        }

        public string NameCliente
        {
            get
            {
                return " Cod." + GlobalUI.ClienteActual.Codigo + "-" + GlobalUI.ClienteActual.Nombre;
            }
        }

        private IObservableCollection<Articulo> items;
        public IObservableCollection<Articulo> Items
        {
            get { return items; }
            set { items = value; RaisePropertyChanged("Items"); }
        }
        #endregion ListaInventarios


        #region Companías y CompaniaActual
        private string companiaActual;
        public string CompaniaActual
        {
            get { return companiaActual; }
            set
            {
                    companiaActual = value;
                    RaisePropertyChanged("CompaniaActual");
                
            }
        }

        public IObservableCollection<string> Companias { get ;set ;}
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
        public string TextoBusquedati
        {
            get { return textoBusqueda; }
            set
            {
                if (value != textoBusqueda)
                {
                    textoBusqueda = value;
                    RaisePropertyChanged("TextoBusqueda");
                    LecturaCodigoBarra(value);
                }
            }
        }

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

        private string lbunidadAlmacen;
        public string lbUnidadAlmacen
        {
            get { return lbunidadAlmacen; }
            set
            {
                if (value != lbunidadAlmacen)
                {
                    lbunidadAlmacen = value;
                    RaisePropertyChanged("lbUnidadAlmacen");
                }
            }
        }

        private string lbunidadDetalle;
        public string lbUnidadDetalle
        {
            get { return lbunidadDetalle; }
            set
            {
                if (value != lbunidadDetalle)
                {
                    lbunidadDetalle = value;
                    RaisePropertyChanged("lbUnidadDetalle");
                }
            }
        }

        private decimal unidadAlmacen;
        public decimal UnidadAlmacen
        {
            get { return unidadAlmacen; }
            set
            {
                if (value != unidadAlmacen)
                {
                    unidadAlmacen = value;
                    RaisePropertyChanged("UnidadAlmacen");
                }
            }
        }

        private decimal unidadDetalle;
        public decimal UnidadDetalle
        {
            get { return unidadDetalle; }
            set
            {
                if (value != unidadDetalle)
                {
                    unidadDetalle = value;
                    RaisePropertyChanged("UnidadDetalle");
                }
            }
        }
        #endregion Propiedades

        public TomaInventarioViewModel() 
        {            
            CargaInicial();
        }

        #region CargaInicial

        public void CargaInicial()
        {
            if (FRmConfig.EnConsulta)
            {
                Companias = new SimpleObservableCollection<string>(Util.CargarCiasInventario(Gestor.Inventario.Gestionados));
            }
            else
            {
                Companias = new SimpleObservableCollection<string>(Util.CargarCiasClienteActual().ConvertAll(x => x.Compania));
            }
            if (Companias.Count > 0)
            {
                CompaniaActual = Companias[0];
            }


            //se configuran las tablas para el inventario y sus detalles
            Criterios = new SimpleObservableCollection<CriterioArticulo>()
                    { CriterioArticulo.Codigo,
                      CriterioArticulo.Barras,
                      CriterioArticulo.Descripcion,
                      CriterioArticulo.Familia,
                      CriterioArticulo.Clase
                    };
            CriterioActual = CriterioArticulo.Descripcion;

            if (esConsulta)
            {
                this.Items = listaTemporal;
                ItemActual = Items[0];
                CriterioActual = criterioTemporal;
                esConsulta = false;
            }

        }

        #endregion CargaInicial

        /// <summary>
        /// Funcion que hace la conversion de unidades de detalle a unidades de almacen tomando
        /// en cuenta las unidades de empaque.
        /// </summary>
        public void CalculaUnidades()
        {
            // hay seleccionados
            if (ItemActual !=null)
            {
                if (inicial)
                {
                    inicial = false;
                    decimal cantDetalle;
                    decimal cantAlmacen;

                    GestorUtilitario.CalculaUnidades(this.UnidadAlmacen,
                                                    this.UnidadDetalle,
                                                    Convert.ToInt32(this.ItemActual.UnidadEmpaque),
                                                    out cantAlmacen,
                                                    out cantDetalle);

                    this.UnidadDetalle = cantDetalle;
                    this.UnidadAlmacen = cantAlmacen;
                    inicial = true;
                }
            }
        }

        /// <summary>
        /// retorma la coleccion de Items Seleccionados
        /// </summary>
        public List<Articulo> SelectedItems
        {
            get
            {
                return new List<Articulo>(this.Items.Where<Articulo>(x => x.Seleccionado));
            }
        }

        /// <summary>
        /// retorma la coleccion de Indices seleccionados
        /// </summary>
        public List<int> SelectedIndex
        {
            get
            {
                List<int> result = new List<int>();
                for(int i = 0; i < Items.Count; i++)
                {
                    if (Items[i].Seleccionado) 
                        result.Add(i);
                }
                return result;
            }
        }

        /// <summary>
        /// Metodo se encarga de llamar a la funcion que busca en la base de datos los datos
        /// del articulo segun el codigo ingresado. 
        /// </summary>
        public void BuscarArticulo()
        {
            if (this.ItemActual== null)
            {
                //No se ha seleccionado un artículo
                return;
            }

            if (this.CompaniaActual == null)
            {
                this.mostrarMensaje(Mensaje.Accion.SeleccionNula, "una compañía");
                return;
            }

            //Obtenemos el articulo de la lista de articulos
            //this.elArticulo = this.ListaArticulos[this.lstArticulos.SelectedIndices[0]];

            DetalleInventario detalle = Gestor.Inventario.BuscarDetalleEnInventario(this.ItemActual);

            if (detalle == null)
                detalle = new DetalleInventario();

            if (!string.IsNullOrEmpty(this.ItemActual.TipoEmpaqueAlmacen))
            {
                this.lbUnidadAlmacen = this.ItemActual.TipoEmpaqueAlmacen + ":";
            }
            else
            {
                this.lbUnidadAlmacen = "Und. Almacen:";
            }
            if (!string.IsNullOrEmpty(this.ItemActual.TipoEmpaqueDetalle))
            {
                this.lbUnidadDetalle = this.ItemActual.TipoEmpaqueDetalle + ":";
            }
            else 
            {
                this.lbUnidadDetalle = "Und. Detalle:";
            }
            this.UnidadDetalle = detalle.UnidadesDetalle;
            this.UnidadAlmacen = detalle.UnidadesAlmacen;
        }

        /// <summary>
        /// Metodo utilizando cuando el filtro de búsqueda es modificado.
        /// </summary>
        public void LecturaCodigoBarra(string textoConsulta)
        {
            if (textoConsulta != string.Empty && CriterioActual == CriterioArticulo.Barras)
            {
                this.cargando = true;
                //Cambiamos el criterio de busqueda a Codigo de Barras
                this.CriterioActual= CriterioArticulo.Barras;
                //if (TextoBusquedati.EndsWith(FRmConfig.CaracterDeRetorno))
                //    textoConsulta = textoConsulta.Substring(0, textoConsulta.Length - 1);
                this.TextoBusquedati = textoConsulta;

                this.RealizarBusqueda();

                // Items.Seleccionados > 0
                if (Items.Count > 0)
                {
                    if (this.ItemActual != null)
                    {
                        //this.ItemActual.Items[0].Focused = true;
                        //this.ItemActual.Items[0].Selected = true;
                        //Caso 35220 LJR 27/03/2009, Estandarizar codigos de barras, incremento de cantidades automatico

                        //Debemos incrementar la cantidad ingresada e inmediatamente agregarlo al inventario
                        //Se incrementa la cantidad en detalle pues el codigo de barras cargado a Frm es el 
                        //de ventas = unidades de detalle.
                        this.UnidadDetalle++;

                        this.CalculaUnidades();

                        this.AgregarDetalle();
                        EsBarras = true;
                    }
                }
                else
                {
                    this.UnidadAlmacen = 0;
                    this.UnidadDetalle = 0;
                    this.ItemActual = null;
                }
                //this.TextoBusqueda = string.Empty;
                this.cargando = false;
                
            }
            
            
        }

        #region Comandos

        public ICommand ComandoConsultarDetalle
        {
            get { return new MvxRelayCommand(ConsultarDetalles); }
        }

        public ICommand ComandoAgregar
        {
            get { return new MvxRelayCommand(AgregarDetalle); }
        }

        public ICommand ComandoRetirarDetalle
        {
            get { return new MvxRelayCommand(RetirarDetalle); }
        }

        public ICommand ComandoCancelarToma
        {
            get { return new MvxRelayCommand(CancelarToma); }
        }
        
        public ICommand ComandoAceptarToma
        {
            get { return new MvxRelayCommand(GestionarInventario); }
        }

        public ICommand ComandoRefrescar
        {
            get { return new MvxRelayCommand(RealizarBusqueda); }
        }

        #endregion Comandos

        #region Acciones

        ///<summary>
        /// Metodo que llama al form que despliega el detalle de los articulo ya
        /// incluidos en el inventario
        /// </summary>
        private void ConsultarDetalles()
        {
            if (Gestor.Inventario.ExistenArticulosGestionados())
            {
                Dictionary<string, object> parametros = new Dictionary<string, object>();
                parametros.Add("inventarios", "S");
                parametros.Add("messageid", "messageid");
                listaTemporal = Items;
                criterioTemporal = CriterioActual;
                esConsulta = true;
                this.DoClose();                
                this.RequestNavigate<DetalleInventarioViewModel>(parametros);
            }
            else
                this.mostrarMensaje(Mensaje.Accion.Informacion, "No hay detalles incluídos");
        }

        /// <summary>
        /// Metodo que se encarga  llamar a la funcion que realiza la gestion]
        /// de inventario y detalles.
        /// </summary>
        private void AgregarDetalle()
        {
            // Items.Seleccionados > 0
            if (ItemActual==null)
            {
                this.mostrarAlerta("Debe seleccionar un articulo para inventario.");
                return;
            }

            if (this.UnidadAlmacen == 0 && this.UnidadDetalle == 0)
            {
                this.mostrarAlerta("Ambas cantidades no pueden ser cero.");
                return;
            }

            //Se procede a agregar el detalle del Inventario.
            try
            {
                Gestor.Inventario.Gestionar(this.ItemActual,
                    GlobalUI.ClienteActual.Codigo, GlobalUI.RutaActual.Codigo,
                    UnidadDetalle, UnidadAlmacen);

            }
            catch (Exception ex)
            {
                this.mostrarAlerta("No se pudo agregar el detalle del inventario. " + ex.Message);
            }
        }

        ///<summary>
        /// Metodo que gestiona el retiro de una linea de detalle
        /// incluida en el inventario
        /// </summary>
        private void RetirarDetalle()
        {
            if (this.ItemActual==null)
            {
                //No se ha seleccionado un artículo
                this.mostrarAlerta("Debe seleccionar un articulo para retirar del inventario.");
                return;
            }

            if (this.CompaniaActual == null)
            {
                this.mostrarMensaje(Mensaje.Accion.SeleccionNula, "una compañía");
                return;
            }
            //Obtenemos el articulo de la lista de articulos
            //this.elArticulo = this.ListaArticulos[this.lstArticulos.SelectedIndices[0]]; ;
            DetalleInventario detalle = Gestor.Inventario.BuscarDetalleEnInventario(this.ItemActual);

            if (detalle == null)
            {
                this.mostrarAlerta("El artículo no ha sido ingresado al inventario, por esto no puede ser eliminado.");
                return;
            }

            this.mostrarMensaje(Mensaje.Accion.Retirar, "el artículo", result =>
                {

                    if (result == DialogResult.Yes)
                    {
                        try
                        {
                            Gestor.Inventario.Gestionar(this.ItemActual, GlobalUI.ClienteActual.Codigo, GlobalUI.RutaActual.Codigo, 0, 0);

                            this.UnidadAlmacen = 0;
                            this.UnidadDetalle = 0;

                            //dialog.Dismiss();
                        }
                        catch (Exception ex)
                        {
                            this.mostrarAlerta("Error al gestionar el inventario. " + ex.Message);
                        }
                    }
                });
        }

        ///<summary>
        /// Metodo gestiona  la cancelacion de la toma del inventario.
        /// </summary>
        private void CancelarToma()
        {
            this.mostrarMensaje(Mensaje.Accion.Cancelar, "la toma del inventario", result =>
                {
                    if (result == DialogResult.Yes)
                    {
                        Gestor.Inventario = new Inventarios();
                        this.DoClose();
                        Dictionary<string, object> par = new Dictionary<string, object>();
                        par.Add("habilitarPedidos", true);
                        RequestNavigate<MenuClienteViewModel>(par);
                    }
                });
        }

        /// <summary>
        /// Metodo que gestiona el inventario.
        /// </summary>
        private void GestionarInventario()
        {
            this.mostrarMensaje(Mensaje.Accion.Terminar, "la toma del inventario", result =>
                {

                    if (result == DialogResult.Yes)
                    {
                        try
                        {
                            if (FRmConfig.EnConsulta)
                                Gestor.Inventario.Actualizar();
                            else
                                Gestor.Inventario.Guardar();
                        }
                        catch (Exception ex)
                        {
                            this.mostrarAlerta("Error " + (FRmConfig.EnConsulta ? "actualizando" : "guardando") + " inventario. " + ex.Message);
                        }
                        //Limpiamos la lista de inventarios gestionados
                        Gestor.Inventario = new Inventarios();

                        //this.CloseCommand.Execute(null);
                        this.DoClose();
                        Dictionary<string, object> par = new Dictionary<string, object>();
                        par.Add("habilitarPedidos", true);
                        RequestNavigate<MenuClienteViewModel>(par);
                    }
                });
        }

        /// <summary>
        /// Funcion encargada de validar si la busqueda se debe hacer 
        /// a la base  de datos o al dataset en memoria tomando en cuenta el texto raiz
        /// de la busqueda y el texto que se digito para realizar la busqueda.
        /// </summary>
        public void RealizarBusqueda()
        {
            if (!Util.ValidarDatos(CriterioActual, CompaniaActual, TextoBusquedati, this))
                return;

            if (CriterioActual == CriterioArticulo.Ninguno)
            {
                this.mostrarAlerta("Opción de búsqueda inválida");
                return;
            }
            try
            {
                //this.lstArticulos.Items.Clear();
                //string compania = cmbCompania.SelectedItem.ToString();

                this.Items = new SimpleObservableCollection<Articulo>(
                    Articulo.ObtenerArticulos(
                        new FiltroArticulo[] { FiltroArticulo.GrupoArticulo, FiltroArticulo.NivelPrecio },
                        new CriterioBusquedaArticulo(CriterioActual, TextoBusquedati, false),
                        CompaniaActual,
                        GlobalUI.RutaActual.GrupoArticulo,
                        GlobalUI.ClienteActual.ObtenerClienteCia(CompaniaActual).NivelPrecio.Nivel)
                        );

                if (this.Items.Count == 0&&CriterioActual!=CriterioArticulo.Barras)
                    Mensaje.mostrarMensaje(Mensaje.Accion.BusquedaMala);

                RaisePropertyChanged("Items");

                if (Items.Count > 0)
                    ItemActual = Items[0];
            }
            catch (Exception ex)
            {
                this.mostrarAlerta("Error realizando búsqueda. " + ex.Message);
            }
        }

        #endregion Accioness

    }
}
