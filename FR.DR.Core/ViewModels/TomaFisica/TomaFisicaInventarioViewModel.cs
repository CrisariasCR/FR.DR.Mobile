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

namespace Softland.ERP.FR.Mobile.ViewModels
{
    public class TomaFisicaInventarioViewModel : DialogViewModel<bool>
    {
#pragma warning disable 169

        public TomaFisicaInventarioViewModel(string messageId, string pCompania, string pBodega)
            : base(messageId)
        {            
            

            Compania = pCompania;
            BodegaCamion = pBodega;
            HandHeld = Ruta.NombreDispositivo();
            Localizacion = "ND";

            //Se inicializa la instancia de boletas
            tomaFisica = new TomaFisicaInventario(Compania, HandHeld, BodegaCamion, Localizacion, DateTime.Now.Date);
            existenciaBodega = new Bodega(BodegaCamion);
            lstTomaFisica = new List<TomaFisicaInventario>();

            //Se definen valores iniciales            
            AcumDetalle = 0;
            AcumAlmacen = 0;
            DispAlmacen = 0;
            DispDetalle = 0;            

            //Se ocultan los datos de lote
            //lblLote.Visible = false;
            //cmbLote.Visible = false;
            LotesVisible = false;

            articuloDatos = new Articulo();
            
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
        TomaFisicaInventario tomaFisica = null;
        Articulo articuloDatos = null;
        Bodega existenciaBodega = null;
        string articulo = string.Empty;
        string compania = string.Empty;
        string bodega = string.Empty;
        string handHeld = string.Empty;
        string localizacion = string.Empty;
        bool existeBoletaInv = false;
        List<TomaFisicaInventario> lstTomaFisica = null;
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

        private decimal acumAlmacen;
        public decimal AcumAlmacen
        {
            set { acumAlmacen = value; RaisePropertyChanged("AcumAlmacen"); }
            get { return acumAlmacen; }
        }

        private decimal acumDetalle;
        public decimal AcumDetalle
        {
            set { acumDetalle = value; RaisePropertyChanged("AcumDetalle"); }
            get { return acumDetalle; }
        }

        private decimal dispAlmacen;
        public decimal DispAlmacen
        {
            set { dispAlmacen = value; RaisePropertyChanged("DispAlmacen"); }
            get { return dispAlmacen; }
        }

        private decimal dispDetalle;
        public decimal DispDetalle
        {
            set { dispDetalle = value; RaisePropertyChanged("DispDetalle"); }
            get { return dispDetalle; }
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

        private string textoBusqueda;
        public string TextoBusqueda
        {
            get { return textoBusqueda; }
            set { textoBusqueda = value; RaisePropertyChanged("TextoBusqueda"); }
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

        private List<string> lotesCombo = new List<string>();

        #endregion

        #endregion

        #region mobile

        

        #region Métodos

        private void Cancelar() 
        {
            this.mostrarMensaje(Mensaje.Accion.Decision, " cancelar el proceso de toma física. Perdiendo los datos ingresados", result =>
            {
                if (result == DialogResult.OK || result == DialogResult.Yes)
                {
                    lstTomaFisica.Clear();
                    ReturnResult(false);
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

            this.mostrarMensaje(Mensaje.Accion.Decision, " registrar la toma física ingresada", result =>
            {
                if (result == DialogResult.OK || result == DialogResult.Yes)
                {
                    if (lstTomaFisica.Count > 0)
                    {
                        if (InsertarBoletas())
                        {
                            //Se valida si se facturan las diferencias para mostrar la pantalla
                            if (FRdConfig.FacturarDiferencias)
                            {
                                Dictionary<string, object> parametros = new Dictionary<string, object>();
                                parametros.Add("pCompania", Compania);
                                parametros.Add("pBodega", BodegaCamion);
                                this.RequestDialogNavigate<FactDiferenciasTFViewModel, bool>(parametros,
                         res =>
                         {

                             //Se recarga el inventario en la pantalla
                             ReturnResult(res);
                         });
                                //this.RequestNavigate<FactDiferenciasTFViewModel>(parametros);
                                //this.DoClose();
                                
                            }
                            else
                            {
                                if (!tomaFisica.AplicarTomaFisica())
                                {
                                    this.mostrarAlerta("Problemas aplicando boletas de inventario físico. ");
                                }
                                else
                                {
                                    this.ReturnResult(true);
                                }
                            }
                            
                        }
                    }
                    else
                    {
                        this.mostrarAlerta("No existen datos para generar la toma física.");
                    }
                }
            });
        }

        public void BorrarBoleta() 
        {
            this.mostrarMensaje(Mensaje.Accion.Alerta, "Está seguro de eliminar la boleta de inventario físico?", result =>
            {
                if (result == DialogResult.OK || result == DialogResult.Yes)
                {
                    if (ExisteBoletaInv)
                    {
                        lstTomaFisica.RemoveAt(indiceBoleta);
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
            if (Texto.EndsWith(FRmConfig.CaracterDeRetorno))
            {
                Texto = Texto.Substring(0, Texto.Length - 1).ToUpper();
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
                    if (string.IsNullOrEmpty(DispAlmacen.ToString()) && string.IsNullOrEmpty(DispDetalle.ToString()))
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
                    if (!string.IsNullOrEmpty(DispAlmacen.ToString()) && !string.IsNullOrEmpty(DispDetalle.ToString()))
                    {
                        cantidad = DispAlmacen + DispDetalle;

                        if (cantidad <= 0)
                        {
                            this.mostrarAlerta("La cantidad a ingresar debe ser mayor a cero.");
                            datosValidos = false;
                        }
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(DispAlmacen.ToString()))
                        {
                            this.mostrarAlerta("Debe indicar una cantidad almacen.");
                            //txtDisponibleAlmacen.Focus();
                            datosValidos = false;
                        }

                        if (string.IsNullOrEmpty(DispDetalle.ToString()))
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
            Registrar();
        }

        protected bool Registrar()
        {
            decimal cantidad = decimal.Zero;
            bool procesoExitoso = true;
            TomaFisicaInventario datos = new TomaFisicaInventario();
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
                        //Se toma el lote para la boleta
                        if (articuloDatos.UsaLotes)
                        {
                            lote = LoteActual;
                        }
                        else
                        {
                            lote = "ND";
                        }

                        if (lstTomaFisica.Count > 0)
                        {
                            ExisteBoletaInv = false;

                            //Se recorren los datos
                            foreach (TomaFisicaInventario item in lstTomaFisica)
                            {
                                if (item.Articulo == articuloDatos.Codigo && item.Lote == lote && item.Localizacion == Localizacion)
                                {
                                    item.Cantidad = cantidad;
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
                            datos.Cantidad = cantidad;
                            datos.Localizacion = Localizacion;

                            //Se agrega la nueva linea al ListView
                            lstTomaFisica.Add(datos);
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
            //Bodega existenciaBodega = null;

            try
            {
                //Se inicializa la instacia de Bodega
                existenciaBodega = new Bodega(BodegaCamion);

                //Se recorren los datos del ListView
                foreach (TomaFisicaInventario item in lstTomaFisica)
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
                        tomaFisica.Articulo = item.Articulo;
                        tomaFisica.Lote = item.Lote;
                        tomaFisica.Cantidad = item.Cantidad;
                        tomaFisica.Localizacion = item.Localizacion;
                        tomaFisica.CantidadFacturar = decimal.Zero;
                        tomaFisica.CantidadDiferencia = tomaFisica.Cantidad - existenciaBodega.Existencia;
                    }

                    //Se registra la boleta en la base de datos
                    if (procesoExitoso && tomaFisica.CantidadDiferencia != 0)
                    {
                        procesoExitoso = tomaFisica.RegistrarNuevaBoleta();
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
                procesoExitoso = false;
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
                tomaFisica.Articulo = articulo;
                tomaFisica.BodegaCamion = BodegaCamion;
                tomaFisica.Localizacion = Localizacion;
                tomaFisica.Lote = lote;

                //Se valida si existe alguna boleta para el artículo
                if (procesoExitoso)
                {
                    procesoExitoso = tomaFisica.ExisteBoleta(ref existe);
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
                                        procesoExitoso = tomaFisica.CargarBoleta();
                                    }

                                    if (result == DialogResult.Yes)
                                    {
                                        cantBoleta = tomaFisica.Cantidad;
                                    }

                                    //Se elimina la boleta
                                    if (procesoExitoso)
                                    {
                                        procesoExitoso = tomaFisica.EliminarBoleta();
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
            decimal cantidadBoleta = decimal.Zero;
            decimal cantDetalle = decimal.Zero;
            decimal cantAlmacen = decimal.Zero;

            try
            {
                //Se cargan los datos del artículo
                if (procesoExitoso)
                {
                    procesoExitoso = articuloDatos.CargarDatosArticulo(TextoBusqueda, Compania);
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
                    if (lstTomaFisica.Count > 0)
                    {
                        int cont = 0;
                        foreach (TomaFisicaInventario item in lstTomaFisica)
                        {
                            if (item.Articulo == TextoBusqueda && item.Lote == loteArt)
                            {
                                cantidadBoleta = Convert.ToDecimal(item.Cantidad);
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

                    //Si existe una boleta para el artículo se cargan sus valores
                    if (procesoExitoso && ExisteBoletaInv)
                    {
                        //procesoExitoso = tomaFisica.CargarBoleta();

                        //Si se tuvo exito se cargan los valores de cantidad en la pantalla
                        if (procesoExitoso)
                        {
                            //Se obtienen las cantidades
                            procesoExitoso = ConvertirCantidadBoletas(cantidadBoleta, ref cantDetalle, ref cantAlmacen);

                            //Se deben realizar las conversiones de cantidades detalle y almacen
                            AcumDetalle = cantDetalle;
                            AcumAlmacen = cantAlmacen;
                        }
                    }
                }
                else
                {
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
            TextoBusqueda = string.Empty;
            ArticuloDescripcion = string.Empty;
            AcumAlmacen = 0;
            AcumDetalle = 0;
            DispAlmacen = 0;
            DispDetalle = 0;
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
            decimal cantDetalle = decimal.Zero;
            decimal cantAlmacen = decimal.Zero;
            decimal factorEmpaque = decimal.Zero;

            try
            {
                //Se obtienen los datos del artículo
                factorEmpaque = Articulo.ObtenerFactorEmpaque(TextoBusqueda, Compania);

                //Se verifican las cantidades almacen en la pantalla
                if (!string.IsNullOrEmpty(AcumAlmacen.ToString()) && !string.IsNullOrEmpty(DispAlmacen.ToString()))
                {
                    cantAlmacen = AcumAlmacen + DispAlmacen;
                }

                //Se verifican las cantidades detalle en la pantalla
                if (!string.IsNullOrEmpty(AcumDetalle.ToString()) && !string.IsNullOrEmpty(DispDetalle.ToString()))
                {
                    cantDetalle = ((AcumDetalle) + (DispDetalle)) / factorEmpaque;
                }

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