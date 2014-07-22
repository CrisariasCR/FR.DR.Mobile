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
using Softland.ERP.FR.Mobile.Cls.Utilidad;
using Softland.ERP.FR.Mobile.Cls;

using System.Windows.Input;
using Cirrious.MvvmCross.Commands;
using FR.Core.Model;
using Softland.ERP.FR.Mobile.Cls.FRCliente;
using Softland.ERP.FR.Mobile.UI;
using System.Windows.Forms;
using Softland.ERP.FR.Mobile.Cls.Corporativo;
using Softland.ERP.FR.Mobile.Cls.AccesoDatos;
using Softland.ERP.FR.Mobile.Cls.FRArticulo;
using FR.DR.Core.Data.Trasiego;

namespace Softland.ERP.FR.Mobile.ViewModels
{
    public class TomaTrasiegoViewModel : DialogViewModel<bool>
    {
#pragma warning disable 169

        public TomaTrasiegoViewModel(string messageId,string pCompania, string pBodega)
            : base(messageId)
        {            
            

            Compania = pCompania;
            BodegaCamion = pBodega;
            HandHeld = Ruta.NombreDispositivo();
            Localizacion = "ND";

            //Se inicializa la instancia de boletas
            tomaTrasiego = new Trasiego(Compania, HandHeld, BodegaCamion, Localizacion, DateTime.Now.Date);
            existenciaBodega = new Bodega(BodegaCamion);
            lstTomaTrasiego = new List<Trasiego>();

            //Se definen valores iniciales            
            CantAlmacen = 0;
            CantDetalle = 0;

            //Se ocultan los datos de lote
            //lblLote.Visible = false;
            //cmbLote.Visible = false;
            LotesVisible = false;            

            articuloDatos = new Articulo();

            tiposCombo.Add(TipoTrasiego.Entrada);
            tiposCombo.Add(TipoTrasiego.Salida);
            Tipos = new SimpleObservableCollection<TipoTrasiego>(tiposCombo);

            clasesCombo.Add(ClaseTrasiego.Llenos);
            clasesCombo.Add(ClaseTrasiego.Vacios);
            Clases = new SimpleObservableCollection<ClaseTrasiego>(clasesCombo);
            ClasesVisible = FRdConfig.UsaEnvases;

            CantAlmEnabled = false;
            cantDetEnabled = false;
            
        }

        #region Comandos

        public ICommand ComandoAgregar
        {
            get { return new MvxRelayCommand(RegistrarCom); }
        }

        public ICommand ComandoRemover
        {
            get { return new MvxRelayCommand(BorrarBoleta); }
        }

        public ICommand ComandoCancelar
        {
            get { return new MvxRelayCommand(Cancelar); }
        }

        public ICommand ComandoAceptar
        {
            get { return new MvxRelayCommand(Aceptar); }
        }

        #endregion

        #region Propiedades        
        
        #region VariablesClase

        //
        public bool DispAlmacenFocus;
        //
        Trasiego tomaTrasiego = null;
        Articulo articuloDatos = null;
        Bodega existenciaBodega = null;
        string articulo = string.Empty;
        string compania = string.Empty;
        string bodega = string.Empty;
        string handHeld = string.Empty;
        string localizacion = string.Empty;
        bool existeBoletaInv = false;
        List<Trasiego> lstTomaTrasiego = null;
        int indiceBoleta;
        int indiceSeleccionado;


        public int IndiceBoleta
        {
            get { return indiceBoleta; }
            set { indiceBoleta = value; }
        }
        public bool ExisteBoletaInv
        {
            get { return existeBoletaInv; }
            set { existeBoletaInv = value; }
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
        public string BodegaCamion
        {
            get { return bodega; }
            set { bodega = value; }
        }
        public string Compania
        {
            get { return compania; }
            set { compania = value; }
        }

        #endregion

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

        private decimal cantAlmacen;
        public decimal CantAlmacen
        {
            set { cantAlmacen = value; RaisePropertyChanged("CantAlmacen"); }
            get { return cantAlmacen; }
        }

        private decimal cantDetalle;
        public decimal CantDetalle
        {
            set { cantDetalle = value; RaisePropertyChanged("CantDetalle"); }
            get { return cantDetalle; }
        }

        private string articuloDescripcion;
        public string ArticuloDescripcion
        {
            get { return articuloDescripcion; }
            set { articuloDescripcion = value; RaisePropertyChanged("ArticuloDescripcion");}
        }

        private bool lotesVisible;
        public bool LotesVisible
        {
            get { return lotesVisible; }
            set { lotesVisible = value; RaisePropertyChanged("LotesVisible"); }
        }

        private bool cantAlmEnabled;
        public bool CantAlmEnabled
        {
            get { return cantAlmEnabled; }
            set { cantAlmEnabled = value; RaisePropertyChanged("CantAlmEnabled"); }
        }

        private bool cantDetEnabled;
        public bool CantDetEnabled
        {
            get { return cantDetEnabled; }
            set { cantDetEnabled = value; RaisePropertyChanged("CantDetEnabled"); }
        }

        private bool clasesVisible;
        public bool ClasesVisible
        {
            get { return clasesVisible; }
            set { clasesVisible = value; RaisePropertyChanged("ClasesVisible"); }
        }

        private string textoBusqueda;
        public string TextoBusqueda
        {
            get { return textoBusqueda; }
            set { textoBusqueda = value; RaisePropertyChanged("TextoBusqueda");}
        }

        
        private string loteActual;
        public string LoteActual
        {
            get { return loteActual; }
            set { loteActual = value; RaisePropertyChanged("LoteActual"); }
        }

     

        private IObservableCollection<string> lotes;
        public IObservableCollection<string> Lotes
        {
            get { return lotes; }
            set { lotes = value; RaisePropertyChanged("Lotes"); }
        }

        private IObservableCollection<TipoTrasiego> tipos;
        public IObservableCollection<TipoTrasiego> Tipos
        {
            get { return tipos; }
            set { tipos = value; RaisePropertyChanged("Tipos"); }
        }

        private TipoTrasiego tipoActual;
        public TipoTrasiego TipoActual
        {
            get { return tipoActual; }
            set { tipoActual = value; RaisePropertyChanged("TipoActual"); }
        }

        private IObservableCollection<ClaseTrasiego> clases;
        public IObservableCollection<ClaseTrasiego> Clases
        {
            get { return clases; }
            set { clases = value; RaisePropertyChanged("Clases"); }
        }

        private ClaseTrasiego claseActual;
        public ClaseTrasiego ClaseActual
        {
            get { return claseActual; }
            set { claseActual = value; RaisePropertyChanged("ClaseActual"); }
        }

        private List<string> lotesCombo = new List<string>();
        private List<TipoTrasiego> tiposCombo = new List<TipoTrasiego>();
        private List<ClaseTrasiego> clasesCombo = new List<ClaseTrasiego>();

        #endregion

        #endregion

        #region mobile

        

        #region Métodos

        private void Cancelar() 
        {
            this.mostrarMensaje(Mensaje.Accion.Decision, " cancelar el proceso de trasiego. Perdiendo los datos ingresados", result =>
            {
                if (result == DialogResult.OK || result == DialogResult.Yes)
                {
                    lstTomaTrasiego.Clear();
                    this.ReturnResult(true);
                }
            }
            );
        }

        public bool LoteSelectedItem() 
        {
            bool result = false;
            if (!string.IsNullOrEmpty(LoteActual))
            {
                if (ObtenerDatosArticulo(false))
                {
                    result = true;
                }

                //Se obtiene la localizacion asociada al lote
                existenciaBodega.ObtenerLocalizacionLote(Compania, TextoBusqueda, LoteActual, out localizacion);
            }
            return result;            
        }

        private void Aceptar() 
        {

            this.mostrarMensaje(Mensaje.Accion.Decision, " registrar la toma del trasiego ingresada", result =>
            {
                if (result == DialogResult.OK || result == DialogResult.Yes)
                {
                    if (lstTomaTrasiego.Count > 0)
                    {
                        if (InsertarBoletas())
                        {
                            ReporteInventarioViewModel.ActualizarAlResumir = true;
                            this.ReturnResult(true);
                        }
                    }
                    else
                    {
                        this.mostrarAlerta("No existen datos para generar el Traspaso.");
                    }
                }
            });
        }

        public void BorrarBoleta() 
        {
            this.mostrarMensaje(Mensaje.Accion.Alerta, "Está seguro de eliminar la boleta de Traspaso?", result =>
            {
                if (result == DialogResult.OK || result == DialogResult.Yes)
                {
                    if (ExisteBoletaInv)
                    {
                        lstTomaTrasiego.RemoveAt(indiceBoleta);
                        LimpiarCampos();
                    }
                    else
                    {
                        this.mostrarAlerta("No se tiene un registro ingresado para el artículo.");
                    }
                }
            }
           );
        }

        public bool txtDisponibleAlmacen_Validating(string Texto)
        {
            try
            {
                if (!string.IsNullOrEmpty(Texto) && Convert.ToDecimal(Texto) < 0)
                {
                    this.mostrarAlerta("La cantidad no puede ser menor a cero.");
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                string error = ex.Message;
                this.mostrarAlerta("Problemas al validar la cantidad disponible almacen.");
                return false;
            }
        }

        public bool txtDisponibleDetalle_Validating(string Texto)
        {
            try
            {
                if (!string.IsNullOrEmpty(Texto) && Convert.ToDecimal(Texto) < 0)
                {
                    this.mostrarAlerta("La cantidad no puede ser menor a cero.");
                    //txtDisponibleDetalle.Focus();
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                string error = ex.Message;
                this.mostrarAlerta("Problemas al validar la cantidad disponible detalle.");
                return false;
            }
        }

        public bool txtBusquedaTextChanged(string Texto) 
        {
            bool result = false;
			//TextoBusqueda = Texto;
            if (Texto.EndsWith(FRmConfig.CaracterDeRetorno))
            {
                TextoBusqueda = Texto.Substring(0, Texto.Length - 1).ToUpper();
            }

            if (!string.IsNullOrEmpty(Texto))
            {
                if (!articuloDatos.CargarDatosArticulo(Texto, Compania))
                {                    
                    result = false;
                }
                else 
                {
                    if (ObtenerDatosArticulo(true))
                    {
                        result = true;
                    }
                }
            }       
            return result;
        }

        protected bool ValidarDatos()
        {
            bool datosValidos = true;
            decimal cantidad = decimal.Zero;

            try
            {
                //Se valida el campo de artículo
                if (datosValidos)
                {
                    if (string.IsNullOrEmpty(TextoBusqueda))
                    {
                        this.mostrarMensaje(Mensaje.Accion.Alerta, "Debe indicar un código de artículo.");
                        datosValidos = false;
                    }
                }

                //Se validan los campos de cantidad
                if (datosValidos)
                {
                    if (string.IsNullOrEmpty(CantAlmacen.ToString()) && string.IsNullOrEmpty(CantDetalle.ToString()))
                    {
                        this.mostrarMensaje(Mensaje.Accion.Alerta, "Debe indicar una cantidad para registrar la boleta.");
                        datosValidos = false;
                    }
                }

                //Se valida el lote en caso de que el artículo utilice
                if (datosValidos && articuloDatos.UsaLotes)
                {
                    if (string.IsNullOrEmpty(LoteActual))
                    {
                        this.mostrarMensaje(Mensaje.Accion.Alerta, "Debe seleccionar un código de lote para registrar la boleta.");
                        datosValidos = false;
                    }
                }

                //Se validan las cantidades ingresadas
                if (datosValidos)
                {
                    if (!string.IsNullOrEmpty(CantAlmacen.ToString()) && !string.IsNullOrEmpty(CantDetalle.ToString()))
                    {
                        cantidad = CantAlmacen + CantDetalle;

                        if (cantidad <= 0)
                        {
                            this.mostrarAlerta("La cantidad a ingresar debe ser mayor a cero.");
                            datosValidos = false;
                        }
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(CantAlmacen.ToString()))
                        {
                            this.mostrarAlerta("Debe indicar una cantidad almacen.");
                            //txtDisponibleAlmacen.Focus();
                            datosValidos = false;
                        }

                        if (string.IsNullOrEmpty(CantDetalle.ToString()))
                        {
                            this.mostrarAlerta("Debe indicar una cantidad detalle.");
                            //txtDisponibleDetalle.Focus();
                            datosValidos = false;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                this.mostrarAlerta("Problemas al realizar la validación de los datos de la pantalla. " + ex.Message);
                datosValidos = false;
            }

            return datosValidos;
        }

        private void RegistrarCom()
        {
            if (!articuloDatos.CargarDatosArticulo(TextoBusqueda, Compania))
            {
                LimpiarCampos();
                this.mostrarAlerta("El Articulo no existe en la Base de Datos");
                return;
            }
            Registrar();
        }

        private bool VerificarCantidad(decimal lnCantidad) 
        {
            bool esSalida=TipoActual==TipoTrasiego.Salida;
            if (esSalida && Trasiego.CantidadExedida(articuloDatos.Codigo, BodegaCamion, lnCantidad))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        protected bool Registrar()
        {
            decimal cantidad = decimal.Zero;
            bool procesoExitoso = true;
            Trasiego datos = new Trasiego();
            string lote = string.Empty;

            try
            {
                //Se validan los datos de la ventana
                if (ValidarDatos())
                {
                    //Se obtienen las cantidades
                    if (procesoExitoso)
                    {
                        procesoExitoso = ObtenerCantidad(ref cantidad);
                    }

                    if (procesoExitoso)
                    {
                        if (this.VerificarCantidad(cantidad))
                        {
                            this.mostrarAlerta("No puede sacar más articulos de los que hay actualmente.");
                            return false;
                        }
                        //Se toma el lote para la boleta
                        if (articuloDatos.UsaLotes)
                        {
                            lote = LoteActual;
                        }
                        else
                        {
                            lote = "ND";
                        }

                        if (lstTomaTrasiego.Count > 0)
                        {
                            ExisteBoletaInv = false;

                            //Se recorren los datos
                            foreach (Trasiego item in lstTomaTrasiego)
                            {
                                if (item.Articulo == articuloDatos.Codigo && item.Lote == lote && item.Localizacion == Localizacion)
                                {
                                    item.CantDiferencia = cantidad;
                                    item.CantAlmacen = CantAlmacen;
                                    item.CantDetalle = CantDetalle;
                                    item.ClasTras = ClaseActual;
                                    item.TipoTras = TipoActual;
                                    ExisteBoletaInv = true;
                                    break;
                                }
                            }
                        }

                        //Se cargan los datos a la instancia para generar la nueva boleta
                        if (!ExisteBoletaInv)
                        {
                            datos.Articulo = articuloDatos.Codigo;
                            datos.Lote = lote;
                            datos.CantDiferencia = cantidad;
                            datos.CantAlmacen = CantAlmacen;
                            datos.CantDetalle = CantDetalle;
                            datos.TipoTras = TipoActual;
                            datos.ClasTras = ClaseActual;
                            datos.Localizacion = Localizacion;

                            //Se agrega la nueva linea al ListView
                            lstTomaTrasiego.Add(datos);
                        }

                        //Se limpian los campos de la pantalla
                        if (procesoExitoso)
                        {
                            LimpiarCampos();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                this.mostrarMensaje(Mensaje.Accion.Alerta, "Problemas al registrar la nueva boleta de inventario físico. " + ex.Message);
                procesoExitoso = false;
            }

            return procesoExitoso;
        }

        protected bool InsertarBoletas()
        {
            bool procesoExitoso = true;
            bool mostrarError = true;
            //Bodega existenciaBodega = null;

            try
            {
                //Se inicializa la instacia de Bodega
                existenciaBodega = new Bodega(BodegaCamion);

                //Se recorren los datos del ListView
                foreach (Trasiego item in lstTomaTrasiego)
                {
                    //Se cargan los datos del artículo
                    if (procesoExitoso)
                    {
                        procesoExitoso = articuloDatos.CargarDatosArticulo(item.Articulo, compania);
                    }

                    //Se obtiene la existencia actual del articulo en la bodega
                    if (procesoExitoso)
                    {
                        if (articuloDatos.UsaLotes)
                        {
                            existenciaBodega.ObtenerExistenciaLotes(Compania, item.Articulo, item.Lote);
                        }
                        else
                        {
                            existenciaBodega.CargarExistencia(Compania, item.Articulo);
                        }
                    }

                    //Se asignan los valores a la instancia de toma fisica
                    if (procesoExitoso)
                    {
                        tomaTrasiego.Articulo = item.Articulo;
                        tomaTrasiego.Lote = item.Lote;
                        tomaTrasiego.CantDiferencia = item.CantDiferencia;
                        tomaTrasiego.CantAlmacen = item.CantAlmacen;
                        tomaTrasiego.CantDetalle = item.CantDetalle;
                        tomaTrasiego.Localizacion = item.Localizacion;
                        tomaTrasiego.ClasTras = item.ClasTras;
                        tomaTrasiego.TipoTras = item.TipoTras;
                        //tomaTrasiego.CantidadFacturar = decimal.Zero;
                        //tomaTrasiego.CantidadDiferencia = tomaTrasiego.Cantidad - existenciaBodega.Existencia;
                    }

                    //Se registra la boleta en la base de datos
                    //if (procesoExitoso && tomaTrasiego.CantidadDiferencia != 0)
                    if (procesoExitoso)
                    {
                        procesoExitoso = tomaTrasiego.RegistrarNuevaBoleta();
                    }

                    if (procesoExitoso)
                    {
                        procesoExitoso=tomaTrasiego.ActualizarExistencias();
                    }

                    //Si ocurrio un error en el proceso se sale del recorrido del ListView
                    if (!procesoExitoso)
                    {
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                this.mostrarAlerta("Problemas al registrar las boletas ingresadas por el usuario. " + ex.Message);
                mostrarError = false;
                procesoExitoso = false;
            }
            if (!procesoExitoso&&mostrarError)
            {
                this.mostrarAlerta("Problemas al registrar las boletas ingresadas por el usuario. ");
            }

            return procesoExitoso;
        }

        

        /// <summary>
        /// Se encarga de validar si existe otra boleta
        /// para el mismo artículo, fecha, bodega, localización y lote
        /// en la base de datos, si encuentra una indica al usuario
        /// si desea eliminar la existente o bien sumar las cantidades
        /// </summary>
        /// <returns></returns>
        protected bool ObtenerBoletaArticulo(string articulo, string lote, out decimal cantidad, out bool tieneBoleta)
        {
            bool procesoExitoso = true;
            bool existe = false;
            decimal cantBoleta = decimal.Zero;
            try
            {
                //Se asginan las propiedades de la clase
                tomaTrasiego.Articulo = articulo;
                tomaTrasiego.Bodega = BodegaCamion;
                tomaTrasiego.Localizacion = Localizacion;
                tomaTrasiego.Lote = lote;

                //Se valida si existe alguna boleta para el artículo
                if (procesoExitoso)
                {
                    //procesoExitoso = tomaTrasiego.ExisteBoleta(ref existe);
                }

                //
                if (procesoExitoso)
                {
                    if (existe)
                    {
                        this.mostrarMensaje(Mensaje.Accion.Alerta, "Ya existe otra boleta registrada para el mismo artículo. ¿Desea cargar los datos de esta boleta?",
                            result =>
                            {
                                //Se valida el boton presionado por el usuario
                                if (result == DialogResult.OK || result == DialogResult.No)
                                {
                                    //Se cargan los datos de la boleta
                                    if (procesoExitoso)
                                    {
                                     //   procesoExitoso = tomaTrasiego.CargarBoleta();
                                    }

                                    if (result == DialogResult.Yes)
                                    {
                                     //   cantBoleta = tomaTrasiego.Cantidad;
                                    }

                                    //Se elimina la boleta
                                    if (procesoExitoso)
                                    {
                                      //  procesoExitoso = tomaTrasiego.EliminarBoleta();
                                    }
                                }
                            }
                        );
                    }
                }
            }
            catch (Exception ex)
            {
                this.mostrarAlerta(String.Format("Problemas obteniendo la boleta del artículo: '{0}'. {1}", articulo, ex.Message));
                procesoExitoso = false;
            }

            //Se asigna el parámetro de retorno
            cantidad = cantBoleta;
            tieneBoleta = existe;

            return procesoExitoso;
        }

        public bool ObtenerDatosArticulo(bool cargarLotes)
        {
            //articuloDatos = new Articulo();
            bool procesoExitoso = true;
            string loteArt = string.Empty;

            try
            {
                //Se cargan los datos del artículo
                if (procesoExitoso)
                {
                    if (FRdConfig.UsaEnvases)
                    {
                        procesoExitoso = articuloDatos.CargarDatosArticuloTrasiego(TextoBusqueda, Compania,ClaseActual.Equals(ClaseTrasiego.Llenos));
                    }
                    else
                    {
                        procesoExitoso = articuloDatos.CargarDatosArticulo(TextoBusqueda, Compania);
                    }
                }

                //Se cargan los lotes del artículo
                if (articuloDatos.UsaLotes)
                {
                    //Se muestran los datos de lote
                    LotesVisible = true;

                    if (cargarLotes)
                    {
                        //Se asigna el código de la compania a la instancia
                        articuloDatos.Compania = Compania;

                        //Se cargan los lotes asociados al artículo
                        articuloDatos.CargarLotesArticulo(BodegaCamion);

                        //Se limpia el combo de lotes
                        if (Lotes != null)
                        {
                            Lotes.Clear();
                            lotesCombo.Clear();
                        }                        

                        if (articuloDatos.LotesAsociados.Count > 0)
                        {

                            //Se recorren los lotes del artículo para cargarlos al combo
                            foreach (Lotes lote in articuloDatos.LotesAsociados)
                            {
                                lotesCombo.Add(lote.Lote);
                            }

                            Lotes = new SimpleObservableCollection<string>(lotesCombo);

                            //Se selecciona el primer lote del articulo
                            if (Lotes.Count > 0)
                            {
                                LoteActual = Lotes[0];
                                //Se carga la localizacion asociada al lote seleccionado
                                existenciaBodega.ObtenerLocalizacionLote(Compania, TextoBusqueda, LoteActual, out localizacion);
                            }
                        }
                        else
                        {
                            this.mostrarAlerta(String.Format("El artículo: '{0}', no posee lotes cargados.", TextoBusqueda));
                            procesoExitoso = false;
                            //txtArticulo.Focus();
                        }
                    }

                    //Se asigna el lote
                    loteArt = LoteActual;
                }
                else
                {
                    //Se ocultan los datos de lote
                    LotesVisible = false;
                    loteArt = "ND";
                }

                //Se busca el artículo en la lista
                if (procesoExitoso)
                {
                    if (lstTomaTrasiego.Count > 0)
                    {
                        int cont = 0;
                        foreach (Trasiego item in lstTomaTrasiego)
                        {
                            if (item.Articulo == TextoBusqueda && item.Lote == loteArt)
                            {
                                CantDetalle = item.CantDetalle;
                                CantAlmacen = item.CantAlmacen;
                                TipoActual = item.TipoTras;
                                if (FRdConfig.UsaEnvases)
                                {
                                    ClaseActual = item.ClasTras;
                                }
                                else
                                {
                                    ClaseActual = ClaseTrasiego.Llenos;
                                }
                                ExisteBoletaInv = true;
                                IndiceBoleta = cont;
                                break;
                            }
                            cont++;
                        }
                    }
                }

                //Se realiza la asignacion de datos del artículo en la pantalla
                if (procesoExitoso)
                {
                    //Se pone la descripción del artículo
                    ArticuloDescripcion = articuloDatos.Descripcion;
                    CantAlmEnabled = true;
                    cantDetEnabled = true;
                }
                else
                {
                    CantAlmEnabled = false;
                    cantDetEnabled = false;
                    this.LimpiarCampos();
                    this.mostrarAlerta("El artículo indicado no existe en la base de datos.");
                }
            }
            catch (Exception)
            {
                this.mostrarMensaje(Mensaje.Accion.Alerta, "Problemas al tratar de mostrar los datos del artículo seleccionado.");
                procesoExitoso = false;
            }

            return procesoExitoso;
        }

        protected void LimpiarCampos()
        {
            cantDetEnabled = false;
            CantAlmEnabled = false;
            TextoBusqueda = string.Empty;
            ArticuloDescripcion = string.Empty;
            CantAlmacen = 0;
            CantDetalle = 0;
            ExisteBoletaInv = false;
            if (Lotes != null)
            {
                Lotes.Clear();
            }            
            LotesVisible = false;
            //txtArticulo.Focus();
        }

        protected bool ObtenerCantidad(ref decimal cantidad)
        {
            //Se definen las variables del proceso
            bool procesoExitoso = true;
            decimal cantBoleta = decimal.Zero;
            decimal cantDetalle = CantDetalle;
            decimal cantAlmacen = CantAlmacen;
            decimal factorEmpaque = decimal.Zero;

            try
            {
                //Se obtienen los datos del artículo
                factorEmpaque = Articulo.ObtenerFactorEmpaque(TextoBusqueda, Compania);


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

        protected bool ConvertirCantidadBoletas(decimal cantidadBoleta, ref decimal cantidadDetalle, ref decimal cantidadAlmacen)
        {
            //Se definen las variables del proceso
            bool procesoExitoso = true;
            decimal cantDetalle = decimal.Zero;
            decimal cantAlmacen = decimal.Zero;
            decimal factorEmpaque = decimal.Zero;
            decimal diferencia = decimal.Zero;

            try
            {
                //Se obtienen los datos del artículo
                factorEmpaque = Articulo.ObtenerFactorEmpaque(TextoBusqueda, Compania);

                //Se revisa que no exista un recido en la division
                if (cantidadBoleta % 1 == 0)
                {
                    cantAlmacen = cantidadBoleta;
                    cantDetalle = 0;
                }
                else
                {
                    diferencia = cantidadBoleta % 1;
                    cantAlmacen = cantidadBoleta - diferencia;
                    if (diferencia < 1)
                    {
                        cantDetalle = diferencia * factorEmpaque;
                    }
                    else
                    {
                        cantDetalle = decimal.Truncate(diferencia);
                        cantDetalle = cantDetalle + ((diferencia - cantDetalle) * factorEmpaque);
                    }
                }
            }
            catch (Exception ex)
            {
                this.mostrarMensaje(Mensaje.Accion.Alerta, "Problemas obteniendo las cantidades de la pantalla." + ex.Message);
                procesoExitoso = false;
            }

            //Se asigna la cantidad al parámetro de retorno
            cantidadAlmacen = cantAlmacen;
            cantidadDetalle = cantDetalle;

            return procesoExitoso;
        }

        #endregion

        #endregion


    }
}