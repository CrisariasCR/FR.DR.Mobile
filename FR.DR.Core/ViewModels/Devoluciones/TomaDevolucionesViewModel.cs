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
using Softland.ERP.FR.Mobile.Cls.Documentos.FRDevolucion;
using Softland.ERP.FR.Mobile.Cls;
using Softland.ERP.FR.Mobile.Cls.Reporte;
using FR.Core.Model;
using Softland.ERP.FR.Mobile.Cls.FRArticulo;
using Softland.ERP.FR.Mobile.UI;
using Softland.ERP.FR.Mobile.Cls.Utilidad;
using Softland.ERP.FR.Mobile.Cls.Documentos;
using System.Windows.Input;
using Cirrious.MvvmCross.Commands;
using System.Windows.Forms;
using Softland.ERP.FR.Mobile.Cls.Corporativo;
using Softland.ERP.FR.Mobile.Cls.AccesoDatos;

namespace Softland.ERP.FR.Mobile.ViewModels
{
    public class TomaDevolucionesViewModel : ListaArticulosViewModel
    {
#pragma warning disable 472
        public TomaDevolucionesViewModel(string tipo,string pais,string divA,string divB,  bool modificando = false)
            : base()
        {
            this.TipoDevolucion = tipo;
            stipoDevolucion = tipo;
            this.EstaModificando = modificando;           

            if (modificando)
            {
                Gestor.Devoluciones = new Devoluciones();
                Gestor.Devoluciones.Gestionados.Add(DevolucionActual);
            }

            //Llena las variables de ubicación
            this.Pais = pais;
            sPais = pais;
            this.DivisionGeograficaA = divA;
            sDiv1 = divA;
            this.DivisionGeograficaB = divB;
            sDiv2 = divB;

            CargaInicial();
        }

        #region Propiedades

        private bool inicial = true;
        private bool EligiendoPago = false;
        private string TipoDevolucion = string.Empty;
        private bool EstaModificando = false;
        private bool cargando = false;
        public bool EsBarras = false;
        private bool verResoluciones = false;
        private string Pais;
        private string DivisionGeograficaA;
        private string DivisionGeograficaB;
        public static IObservableCollection<Articulo> listaTemporal = null;
        public static CriterioArticulo criterioTemporal = CriterioArticulo.Descripcion;
        public static bool esConsulta = false;
        public static string stipoDevolucion = string.Empty;
        public static string sPais = string.Empty;
        public static string sDiv1 = string.Empty;
        public static string sDiv2 = string.Empty;

        public string NameCliente
        {
            get
            {
                return " Cod." + GlobalUI.ClienteActual.Codigo + "-" + GlobalUI.ClienteActual.Nombre;
            }
        }

        public static Devolucion DevolucionActual;

        private bool CreditoChecked { get; set; }

        private IObservableCollection<string> companias;
        public IObservableCollection<string> Companias
        {
            get { return companias; }
            set { companias = value; RaisePropertyChanged("Companias"); }
        }

        private string companiaSeleccionada;
        public string CompaniaSeleccionada
        {
            get { return companiaSeleccionada; }
            set { companiaSeleccionada = value; CambioEnCia(); RaisePropertyChanged("CompaniaSeleccionada"); }
        }

        public IObservableCollection<Estado> EstadosArticulo { get; set; }
        private Estado estadoSeleccionado;
        public Estado EstadoSeleccionado
        {
            get { return estadoSeleccionado; }
            set { estadoSeleccionado = value; CambioEstado(); RaisePropertyChanged("EstadoSeleccionado"); }
        }

        private IObservableCollection<Articulo> articulos;
        public IObservableCollection<Articulo> Articulos
        {
            get { return articulos; }
            set { articulos = value; RaisePropertyChanged("Articulos"); }
        }

        private Articulo articuloSeleccionado;
        public Articulo ArticuloSeleccionado
        {
            get { return articuloSeleccionado; }
            set { articuloSeleccionado = value; SeleccionarArticulo();  RaisePropertyChanged("ArticuloSeleccionado"); }
        }

        public override string TextoBusqueda
        {
            get
            {
                return textoBusqueda;
            }
            set
            {
                textoBusqueda = value; if (!this.cargando) this.CambioTexto();
                RaisePropertyChanged("TextoBusqueda");
            }
        }

        private decimal cantidadDetalle;
        public decimal CantidadDetalle
        {
            get { return cantidadDetalle; }
            set { cantidadDetalle = value; CalculaUnidades(); RaisePropertyChanged("CantidadDetalle"); }
        }

        private decimal cantidadAlmacen;
        public decimal CantidadAlmacen
        {
            get { return cantidadAlmacen; }
            set { cantidadAlmacen = value; RaisePropertyChanged("CantidadAlmacen"); }
        }

        private string notas;
        public string Notas
        {
            get { return notas; }
            set { notas = value; RaisePropertyChanged("Notas"); }
        }

        private string lote;
        public string Lote
        {
            get { return lote; }
            set { lote = value; RaisePropertyChanged("Lote"); }
        }

        private bool loteEnabled;
        public bool LoteEnabled
        {
            get { return loteEnabled; }
            set { loteEnabled = value; RaisePropertyChanged("LoteEnabled"); }
        }

        private bool ingresandoDatosPrincipal = true;
        public bool IngresandoDatosPrincipal
        {
            get { return ingresandoDatosPrincipal; }
            set { ingresandoDatosPrincipal = value; RaisePropertyChanged("IngresandoDatosPrincipal"); }
        }

        private bool ingresandoDatos = true;
        public bool IngresandoDatos
        {
            get { return ingresandoDatos; }
            set { ingresandoDatos = value; RaisePropertyChanged("IngresandoDatos"); }
        }

        private bool ingresandoNota = true;
        public bool IngresandoNota
        {
            get { return ingresandoNota; }
            set { ingresandoNota = value; RaisePropertyChanged("IngresandoNota"); }
        }

        public bool PagoEfectivo { get; set; }
        
        #endregion

        #region Metodos Logica Negocio

        protected bool ObtenerCantidad(ref decimal cantidad)
        {
            //Se definen las variables del proceso
            bool procesoExitoso = true;
            decimal cantBoleta = decimal.Zero;
            decimal cantDetalle = CantidadAlmacen;
            decimal cantAlmacen = CantidadDetalle;
            decimal factorEmpaque = decimal.Zero;

            try
            {
                //Se obtienen los datos del artículo
                factorEmpaque = Articulo.ObtenerFactorEmpaque(ArticuloSeleccionado.Codigo, CompaniaSeleccionada);


                cantDetalle = (cantDetalle) / factorEmpaque;


                //Se suman ambas cantidades
                cantBoleta = cantDetalle + cantAlmacen;
            }
            catch (Exception ex)
            {
                this.mostrarMensaje(Mensaje.Accion.Alerta, "Problemas obteniendo las cantidades de la pantalla." + ex.Message);
                procesoExitoso = false;
            }

            //Se asigna la cantidad al parámetro de retorno
            cantidad = cantBoleta;

            return procesoExitoso;
        }

        private void AgregarComando() 
        {
            this.Agregar();
            if (verResoluciones)
            {
                this.ValidarConsecResoluciones();
            }
        }

        private void Agregar()
        {
            /* se valida que haya seleccionado un estado, que los cantidades no esten vacias 
             * ni que sean 0 y que haya una compannia seleccionada, si todo es correcto se 
             * invoca al metodo que se encarga de realizar el ingreso de la devolucion y su detalle]
             * en las tablas temporales.
             * **/

            if (ArticuloSeleccionado == null)
            {
                this.mostrarAlerta("Debe seleccionar un articulo de devolución.");
                return;
            }

            if (EstadoSeleccionado == null )
            {
                this.mostrarAlerta("Debe seleccionar el estado de la devolución.");
                return;
            }

            if (CantidadAlmacen == 0 && CantidadDetalle == 0)
            {
                this.mostrarAlerta("Ambas cantidades no pueden ser cero.");
                return;
            }

            decimal cantValidar = 0;
            ObtenerCantidad(ref cantValidar);
            ArticuloSeleccionado.CargarArticuloEnvase();
            if (ArticuloSeleccionado.ArticuloEnvase != null && !string.IsNullOrEmpty(ArticuloSeleccionado.ArticuloEnvase.Codigo) && Devolucion.CantidadExedida(ArticuloSeleccionado.ArticuloEnvase.Codigo, GlobalUI.RutaActual.Bodega, cantValidar))
            {
                this.mostrarAlerta("No puede devolver esa cantidad ya que exede el numero de articulos envases disponibles");
                return;
            }

            //Se procede a agregar el detalle del Inventario.
            try
            {
                verResoluciones = Gestor.Devoluciones.Gestionados.Count == 0;
                Gestor.Devoluciones.Gestionar(ArticuloSeleccionado,
                    GlobalUI.ClienteActual.ObtenerClienteCia(companiaSeleccionada),
                    GlobalUI.RutaActual.Codigo, GlobalUI.RutaActual.Bodega,
                    EstadoSeleccionado, Notas, Lote, CantidadDetalle, CantidadAlmacen, string.Empty, string.Empty);
                Gestor.Devoluciones.Gestionados[0].TipoDevolucion = this.TipoDevolucion;
            }
            catch (Exception exc)
            {
                this.mostrarAlerta(exc.Message);
            }
        }

        private void CargaInicial()
        {
            if (Devoluciones.tipoPagoDevolucion == FRmConfig.TipoPagoEfectivo)
            {
                CreditoChecked = false;
                PagoEfectivo = true;
            }

            if (Devoluciones.tipoPagoDevolucion == FRmConfig.TipoPagoCredito)
            {
                CreditoChecked = true;
            }

            this.CargaCiasCombo();
            EstadosArticulo = new SimpleObservableCollection<Estado>(new List<Estado>() { Estado.Bueno, Estado.Malo });
            EstadoSeleccionado = EstadosArticulo[0];

            this.CriterioActual = Cls.FRArticulo.CriterioArticulo.Descripcion;

            if (esConsulta)
            {
                this.Articulos = listaTemporal;
                ArticuloSeleccionado = Articulos[0];
                CriterioActual = criterioTemporal;
                esConsulta = false;
            }
        }

        private void CargaCiasCombo()
        {            
            if (EstaModificando)
            {
                //Solamente se carga la compania de la devolucion que se esta modificando
                //y se carga la informacion del cliente respecto a esta compania.
                Devolucion dev = Gestor.Devoluciones.Gestionados[0];
                Companias = new SimpleObservableCollection<string>() { dev.Compania };
                CompaniaSeleccionada = Companias[0];
            }
            else
            {
                //Se cargan todas las compannias del cliente al combo
                var lista = Util.CargarCiasClienteActual().Select(x=>x.Compania).ToList();
                Companias = new SimpleObservableCollection<string>(lista);
                CompaniaSeleccionada = Companias.Count > 0 ? Companias[0] : null;
            }
        }

        private void CambioTexto()
        {            
            if (TextoBusqueda != string.Empty && CriterioActual == CriterioArticulo.Barras)
            {
                //if (TextoBusqueda.EndsWith(FRmConfig.CaracterDeRetorno))
                //    TextoBusqueda = TextoBusqueda.Substring(0, TextoBusqueda.Length - 1);

                if (CompaniaSeleccionada == null)
                {
                    this.mostrarMensaje(Mensaje.Accion.SeleccionNula, "una compañia");
                    return;
                }
                this.cargando = true;
                //Cambiamos el criterio de busqueda a Codigo de Barras
                CriterioActual = CriterioArticulo.Barras;
                this.RealizarBusqueda();
                if (Articulos.Count > 0)
                {
                    ArticuloSeleccionado = Articulos[0];
                    //Caso 35220 LJR 27/03/2009, Estandarizar codigos de barras, incremento de cantidades automatico
                    CantidadDetalle++;
                    this.Agregar();
                    EsBarras = true;
                }
                this.cargando = false;
                if (verResoluciones)
                {
                    this.ValidarConsecResoluciones();
                }
            }
        }

        public void RealizarBusqueda()
        {
            if (!Util.ValidarDatos(CriterioActual, CompaniaSeleccionada, TextoBusqueda, this))
                return;

            try
            {
                if (Articulos != null)
                {
                    Articulos.Clear();
                }

                var criterio = new CriterioBusquedaArticulo(CriterioActual, TextoBusqueda, false);

                var resultado =
                    Articulo.ObtenerArticulos(
                        new FiltroArticulo[] { FiltroArticulo.GrupoArticulo, FiltroArticulo.NivelPrecio },
                        criterio, CompaniaSeleccionada,
                        GlobalUI.RutaActual.GrupoArticulo,
                        GlobalUI.ClienteActual.ObtenerClienteCia(CompaniaSeleccionada).NivelPrecio.Nivel);

                Articulos = new SimpleObservableCollection<Articulo>(resultado);

                if (Articulos.Count == 0 && CriterioActual!=CriterioArticulo.Barras)
                {
                    this.mostrarMensaje(Mensaje.Accion.BusquedaMala);
                }
            }
            catch (Exception ex)
            {
                this.mostrarAlerta("Error realizando búsqueda. " + ex.Message);
            }
        }

        private void CambioEnCia()
        {
            //Limpiamos la lista de articulos pues los articulos que hay
            //son de la compania anterior
            if(Articulos != null)
                Articulos.Clear();

            ////Solo cuando se haya seleccionado una compania y no se este en consulta
            ////se selecciona una nueva compania
            //if (this.cboCompania.SelectedIndex >= 0 && !this.EstaModificando)
            //{
            //    //ABC 22/10/2008
            //    //Caso: 33706 Reflejo Multicompañia FR 5.0 a FR 6.0
            //    this.companiaSeleccionada = this.cboCompania.SelectedItem.ToString();
            //}
        }

        private void CalculaUnidades()
        {
            if (ArticuloSeleccionado != null)
            {
                if (inicial)
                {
                    inicial = false;

                    decimal cantDetalle = 0;
                    decimal cantAlmacen = 0;

                    GestorUtilitario.CalculaUnidades(CantidadAlmacen,
                        CantidadDetalle,
                        Convert.ToInt32(ArticuloSeleccionado.UnidadEmpaque),
                        out cantAlmacen,
                        out cantDetalle);

                    CantidadDetalle = cantDetalle;
                    CantidadAlmacen = cantAlmacen;

                    inicial = true;
                }                
            }
        }

        private void SeleccionarArticulo()
        {
            if (ArticuloSeleccionado != null)
            {
                DetalleDevolucion detalle = new DetalleDevolucion();

                Devolucion devolucion = Gestor.Devoluciones.Buscar(companiaSeleccionada);

                if (devolucion != null)
                {
                    //Hay una devolucion gestionada para la compania seleccionada

                    DetallesDevolucion detalles = devolucion.Detalles.Buscar(CriterioArticulo.Codigo, ArticuloSeleccionado.Codigo, true, EstadoSeleccionado);

                    if (!detalles.Vacio())
                    {
                        detalle = detalles.Lista[0];
                        articuloSeleccionado = detalle.Articulo;
                    }
                }

                try
                {
                    articuloSeleccionado.CargarPrecio(GlobalUI.ClienteActual.ObtenerClienteCia(companiaSeleccionada).NivelPrecio);
                }
                catch (Exception ex)
                {
                    this.mostrarAlerta("Error cargando precios del artículo. " + ex.Message);
                }

                if (articuloSeleccionado.UsaLotes)
                    LoteEnabled = true;
                else
                    LoteEnabled = false;

                Notas = detalle.Observaciones;
                Lote = detalle.Lote;
                CantidadAlmacen = detalle.UnidadesAlmacen;
                CantidadDetalle = detalle.UnidadesDetalle;

                // Caso: 27702 LDS 19/03/2007
                // Cambio del tipo de variable en la variable Estado: de string al enumerador Articulo.Estado.
                if (detalle.Estado != Estado.NoDefinido)
                    estadoSeleccionado = detalle.Estado;
            }
        }

        private bool ValidaParaRealizarBusqueda()
        {
            /*Si el campo de articulo no esta vacio y se selecciona una compania
             * y el buleano que me indica si vengo de la busqueda es falso
             *  entonces se ejecuta una busqueda
             * */
            return !TextoBusqueda.Equals(string.Empty) &&
                CompaniaSeleccionada != null &&
                ArticuloSeleccionado != null;
        }

        private void CambioEstado()
        {
            //Cuando se cambia el estado hay que validar si 
            //el articulo seleccionado ya fue agregado a la devolucion
            //con el estado seleccionado, para desplegar la cantidad ingresada
            if (this.ValidaParaRealizarBusqueda())
                this.SeleccionarArticulo();
        }

        private void Consulta()
        {
            if (Gestor.Devoluciones.ExistenArticulosGestionados())
            {
                Dictionary<string, object> Pars = new Dictionary<string, object>();
                Pars.Add("esConsulta", false);

                DetalleDevolucionesViewModel.Devoluciones = Gestor.Devoluciones;
                listaTemporal = Articulos;
                criterioTemporal = CriterioActual;
                esConsulta = true;
                this.DoClose();
                this.RequestNavigate<DetalleDevolucionesViewModel>(Pars);

                //frmDetalleDevolucion dev = new frmDetalleDevolucion(Gestor.Devoluciones);
                //dev.ShowDialog();
                //dev.Close();
            }
            else
            {
                this.mostrarMensaje(Mensaje.Accion.Informacion, "No hay detalles incluídos");
            }
        }

        private void Elimina()
        {
            //No se ha seleccionado un artículo
            if (ArticuloSeleccionado == null)
            {
                this.mostrarAlerta("Debe seleccionar un articulo de devolución.");
                return;
            }

            if (CompaniaSeleccionada == null)
            {
                this.mostrarMensaje(Mensaje.Accion.SeleccionNula, "una compañía");
                return;
            }

            if (EstadoSeleccionado == null)
            {
                this.mostrarMensaje(Mensaje.Accion.SeleccionNula, "un estado");
                return;
            }

            Devolucion devolucion = Gestor.Devoluciones.Buscar(this.companiaSeleccionada);

            //Buscamos si el articulo realmente ha sido ingresado como detalle de la devolucion
            DetallesDevolucion detalles = devolucion.Detalles.Buscar(CriterioArticulo.Codigo, ArticuloSeleccionado.Codigo, true, EstadoSeleccionado);

            if (detalles.Vacio())
            {
                this.mostrarAlerta("El artículo no a sido ingresado a la devolución, por esto no puede ser eliminado.");
                return;
            }

            this.mostrarMensaje(Mensaje.Accion.Retirar, "el artículo", result =>
                {
                    if (result.Equals(DialogResult.Yes))
                    {
                        try
                        {
                            devolucion.EliminarDetalle(ArticuloSeleccionado.Codigo, EstadoSeleccionado);
                            CantidadAlmacen = CantidadDetalle = 0;
                            Notas = "";
                        }
                        catch (Exception ex)
                        {
                            this.mostrarAlerta("Error al gestionar devolución. " + ex.Message);
                        }
                    }
                });
        }

        private void Cancela()
        {            
            if (IngresandoDatosPrincipal && !EligiendoPago)
            {
                this.mostrarMensaje(Mensaje.Accion.Cancelar, "la toma de las devoluciones", result =>
                {
                    if (result == DialogResult.Yes)
                    {
                        //Limpiamos la lista de devoluciones.
                        Gestor.Devoluciones = new Devoluciones();
                        DoClose();
                        Dictionary<string, object> par = new Dictionary<string, object>();
                        par.Add("habilitarPedidos", true);
                        RequestNavigate<MenuClienteViewModel>(par);
                    }
                });
            }
            else
            {
                if (!EligiendoPago)
                {
                    AceptaNotas();
                }
                else
                {
                    this.mostrarAlerta("Debe elegir un tipo de pago y continuar");
                }
            }
        }

        private void CancelarTomaResolucion(string motivo)
        {
            this.mostrarMensaje(Mensaje.Accion.Informacion, "El consecutivo de resolución de factura no es válido. " + motivo, res =>
                {
                    //Limpiamos la lista de devoluciones.
                    Gestor.Devoluciones = new Devoluciones();
                    DoClose();
                });
        }

        private void AceptaNotas() 
        {
            IngresandoDatosPrincipal = true;
            IngresandoNota = true;
        }
        private void MostrarNotas()
        {
            IngresandoDatosPrincipal = false;
            IngresandoNota = false;
        }

        private void Aceptar()
        {
            if (Gestor.Devoluciones.Gestionados.Count == 0)
            {
                this.mostrarAlerta("No hay devoluciones gestionadas");
                return;
            }

            this.mostrarMensaje(Mensaje.Accion.Terminar, "la toma de las devoluciones", result =>
            {
                if (result == DialogResult.Yes)
                {
                    bool devolucionGuardada = false;

                    if (this.EstaModificando)
                    {
                        //Se procede a actualizar la devolucion existente
                        try
                        {
                            Gestor.Devoluciones.ActualizarDevolucion();
                            devolucionGuardada = true;

                            /*Devolucion devolucion = Gestor.Devoluciones.Gestionados[0];					
                            devolucion.Actualizar(true);
                            devolucionGuardada = true;*/

                        }
                        catch (Exception ex)
                        {
                            this.mostrarAlerta("Error guardando devolución. " + ex.Message);
                        }
                    }
                    else
                    {
                        //Se procede a guardar la nueva devolucion
                        try
                        {
                            //Verifica si tiene resolucion para cambiar el numero del pedido
                            if (ResolucionUtilitario.usaResolucionFactura(Gestor.Devoluciones.Gestionados[0].Zona))
                            {
                                Gestor.Devoluciones.Gestionados[0].Numero = Gestor.Devoluciones.Gestionados[0].ConsecResolucion.ValorResolucion;
                            }
                            //Actualiza las ubicaciones del documento
                            Gestor.Devoluciones.Gestionados[0].Configuracion.Pais.Codigo = this.Pais;
                            Gestor.Devoluciones.Gestionados[0].Configuracion.Pais.DivisionGeografica1=this.DivisionGeograficaA;
                            Gestor.Devoluciones.Gestionados[0].Configuracion.Pais.DivisionGeografica2 = this.DivisionGeograficaB;
                            Gestor.Devoluciones.Guardar();
                            devolucionGuardada = true;

                            #region Facturas de contado y recibos en FR - KFC
                            DevolucionActual = Gestor.Devoluciones.Gestionados[0];

                            // Ahora se actualizará la jornada en el metodo GuardarTipoPago() - KFC
                            //MejorasGrupoPelon600R6 - KFC
                            //Actualizamos la tabla JORNADA_RUTAS 
                            //ActualizarJornada(GlobalUI.RutaActual.Codigo, Gestor.Devoluciones.Gestionados[0].MontoNeto);

                            #endregion

                        }
                        catch (Exception ex)
                        {
                            this.mostrarAlerta("Error guardando devolución. " + ex.Message);
                        }
                    }

                    //Caso 25452 LDS 30/10/2007
                    //Ya estando guardada la devolución se procede a validar la impresión del documento.
                    if (devolucionGuardada)
                    {

                        if (Devoluciones.tipoPagoDevolucion == FRmConfig.TipoPagoAmbos)
                        {
                            this.mostrarMensaje(Mensaje.Accion.Informacion, "Seleccione un tipo de pago para la devolución");
                            ElegirTipoPago();
                        }
                        else
                        {
                            GuardarTipoPago();
                            //this.Close();
                        }

                        #region Facturas de contado y recibos en FR - KFC

                        // Se comenta la pocrcion de codigo debido a que ahora sera contenido en el metodo MostrarPanelImpresion() - KFC

                        ////caso S2A35723 LDAV 10/02/2010
                        ////Verificar sugerir Imprimir
                        //if (Impresora.SugerirImprimir)
                        //{
                        //    DialogResult res = Mensaje.mostrarMensaje(Mensaje.Accion.Imprimir, "el detalle de las devoluciones");

                        //    if (res.Equals(DialogResult.Yes))
                        //        this.RelocalizarImpresion();
                        //    else
                        //    {
                        //        //Limpiamos la lista de devoluciones.
                        //        Gestor.Devoluciones = new Devoluciones();
                        //        //this.Close();
                        //    }
                        //}
                        ////caso S2A35723 LDAV 10/02/2010
                        ////Verificar sugerir Imprimir
                        //else
                        //{
                        //    //Limpiamos la lista de devoluciones.
                        //    Gestor.Devoluciones = new Devoluciones();
                        //    //this.Close();
                        //}

                        #endregion
                    }
                }
            });
        }

        private void ElegirTipoPago()
        {
            IngresandoDatos = false;
            IngresandoDatosPrincipal = false;
            EligiendoPago = true;

            //cboCriterioOrden.DataSource = DetalleSort.GetValues(new DetalleSort.Ordenador(), false);

            //this.cboCompania.Hide();
            //this.pnlTipoPago.Show();
            //this.pnlTipoPago.BringToFront();
            //this.lblTipoPagoDev.Visible = true;
            //this.rbtCredito.Checked = true;

            //this.picLogo.BringToFront();
        }

        public void GuardarTipoPago()
        {
            if (!PagoEfectivo)
            {
                try
                {
                    Devoluciones.GenerarNotaCredito(DevolucionActual, GlobalUI.ClienteActual.ObtenerClienteCia(companiaSeleccionada).DiasCredito);
                }
                catch (Exception ex)
                {
                    this.mostrarAlerta("Error generando Nota de Crédito. " + ex.Message);
                }
            }
            else
            {
                DevolucionActual.ActualizarTipoDevolucion(DevolucionActual.Numero, FRmConfig.TipoPagoEfectivo);
                //ActualizarJornada(GlobalUI.RutaActual.Codigo, DevolucionActual.MontoNeto);
                //ActualizarEfectivoJornada(GlobalUI.RutaActual.Codigo, DevolucionActual.MontoNeto);                   
            }
            EligiendoPago = false;
            ActualizarJornada(GlobalUI.RutaActual.Codigo, DevolucionActual.MontoNeto);
            MostrarPanelImpresion();
        }

        public override void DoClose()
        {
          
                base.DoClose();
                Dictionary<string, object> Parametros = new Dictionary<string, object>()
                                {
                                    {"habilitarPedidos", true}
                                };
                this.RequestNavigate<MenuClienteViewModel>(Parametros);
            
        }

        private void ActualizarJornada(string ruta, decimal monto)
        {
            TipoMoneda moneda = DevolucionActual.Configuracion.Nivel.Moneda;
            string colCantidad = "";
            string colMonto = "";

            if (!PagoEfectivo)
            {
                if (moneda == TipoMoneda.LOCAL)
                {
                    colCantidad = JornadaRuta.DEVOLUCIONES_LOCAL;
                    colMonto = JornadaRuta.MONTO_DEVOLUCIONES_LOCAL;
                }
                else
                {
                    colCantidad = JornadaRuta.DEVOLUCIONES_DOLAR;
                    colMonto = JornadaRuta.MONTO_DEVOLUCIONES_DOLAR;
                }
            }
            else
            {
                if (moneda == TipoMoneda.LOCAL)
                {
                    colCantidad = JornadaRuta.DEVOLUCIONES_EFC_LOCAL;
                    colMonto = JornadaRuta.MONTO_DEVOLUCION_EFC_LOCAL;
                }
                else
                {
                    colCantidad = JornadaRuta.DEVOLUCIONES_EFC_DOLAR;
                    colMonto = JornadaRuta.MONTO_DEVOLUCION_EFC_DOLAR;
                }
            }


            try
            {
                GestorDatos.BeginTransaction();

                JornadaRuta.ActualizarRegistro(ruta, colCantidad, 1);
                JornadaRuta.ActualizarRegistro(ruta, colMonto, monto);

                GestorDatos.CommitTransaction();
            }
            catch (Exception ex)
            {
                GestorDatos.RollbackTransaction();
                this.mostrarAlerta("Error al actualizar datos. " + ex.Message);
            }
        }

        private void MostrarPanelImpresion()
        {
            //caso S2A35723 LDAV 10/02/2010
            //Verificar sugerir Imprimir
            if (Impresora.SugerirImprimir)
            {
                this.mostrarMensaje(Mensaje.Accion.Imprimir, "el detalle de las devoluciones", res =>
                    {
                        if (res.Equals(DialogResult.Yes))
                        {
                            ImpresionViewModel.OriginalEn = true;
                            ImpresionViewModel.OnImprimir = ImprimirDocumento;
                            this.RequestNavigate<ImpresionViewModel>(new { tituloImpresion = "Impresión Detalle Devoluciones", mostrarCriterioOrden = true });
                        }
                        else
                        {
                            //Limpiamos la lista de devoluciones.
                            Gestor.Devoluciones = new Devoluciones();
                            this.DoClose();
                        }
                    });
                
            }
            //caso S2A35723 LDAV 10/02/2010
            //Verificar sugerir Imprimir
            else
            {
                //Limpiamos la lista de devoluciones.
                Gestor.Devoluciones = new Devoluciones();
                this.DoClose();
            }
        }

        /// <summary>
        /// Valida el consecutivo de resolución
        /// </summary>
        private void ValidarConsecResoluciones()
        {
            //Verifica lo del consecutivo de resoluciones
            if (Gestor.Devoluciones.Gestionados[0].ConsecResolucion != null)
            {
                string error = string.Empty;
                string advertencia = string.Empty;
                if (!Gestor.Devoluciones.Gestionados[0].ConsecResolucion.ValidarConsecutivoResolucion(ref error, ref advertencia))
                {
                    if (!string.IsNullOrEmpty(error))
                    {
                        this.CancelarTomaResolucion(error);
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(advertencia))
                        {
                            this.CancelarTomaResolucion(advertencia);
                        }
                    }

                }
                else
                {
                    if (!string.IsNullOrEmpty(advertencia))
                    {
                        this.mostrarMensaje(Mensaje.Accion.Informacion, advertencia);
                    }
                }
            }
        }

        //Caso 25452 LDS 30/10/2007
        /// <summary>
        /// Imprime los documentos que han sido gestionados.
        /// </summary>
        private void ImprimirDocumento(bool esOriginal, int cantidadCopias, DetalleSort.Ordenador ordernarPor, BaseViewModel viewModel)
        {

            try
            {
                int cantidad = 0;
                cantidad = Util.CantidadCopias(cantidadCopias);

                if (cantidad >= 0)
                {
                    if (esOriginal)
                        foreach (Devolucion devolucion in Gestor.Devoluciones.Gestionados)
                            if (!devolucion.Impreso)
                                devolucion.LeyendaOriginal = true;
                    Gestor.Devoluciones.Cliente = GlobalUI.ClienteActual;
                    //Caso 34848, cambio en los parametros
                    Gestor.Devoluciones.ImprimeDetalleDevolucion(cantidad, (DetalleSort.Ordenador)ordernarPor);
                }
                else
                {
                    this.mostrarMensaje(Mensaje.Accion.Informacion, "Solo se guardará la devolución.");
                }
            }
            catch (FormatException)
            {
                this.mostrarAlerta("Error obteniendo la cantidad de copias. Formato inválido.");
            }
            catch (Exception ex)
            {
                this.mostrarAlerta(ex.Message);
            }

            //Limpiamos la lista de devoluciones.
            Gestor.Devoluciones = new Devoluciones();
            this.DoClose();
        }

        #endregion

        #region Comandos

        public override ICommand ComandoRefrescar
        {
            get { return new MvxRelayCommand(RealizarBusqueda); }
        }

        public ICommand ComandoConsultar
        {
            get { return new MvxRelayCommand(Consulta); }
        }

        public ICommand ComandoAgregar
        {
            get { return new MvxRelayCommand(AgregarComando); }
        }

        public ICommand ComandoMostrarNotas
        {
            get { return new MvxRelayCommand(MostrarNotas); }
        }

        public ICommand ComandoAceptarNotas
        {
            get { return new MvxRelayCommand(AceptaNotas); }
        }

        public ICommand ComandoEliminar
        {
            get { return new MvxRelayCommand(Elimina); }
        }

        public ICommand ComandoCancelar
        {
            get { return new MvxRelayCommand(Cancela); }
        }

        public ICommand ComandoContinuar
        {
            get { return new MvxRelayCommand(Aceptar); }
        }

        public ICommand ComandoGuardar
        {
            get { return new MvxRelayCommand(GuardarTipoPago); }
        }

        #endregion
    }
}