using System;
//using System.Windows.Forms;
using Softland.ERP.FR.Mobile.Cls.Corporativo;
using System.Collections.Generic;
using Softland.ERP.FR.Mobile.UI;
using Softland.ERP.FR.Mobile.Cls.FRArticulo;
using Softland.ERP.FR.Mobile.Cls.Documentos;
using Softland.ERP.FR.Mobile.Cls.Documentos.FRInventario;
using Softland.ERP.FR.Mobile.Cls.Documentos.FRDevolucion;
using Softland.ERP.FR.Mobile.Cls.Documentos.FRPedido;
using Softland.ERP.FR.Mobile.Cls;
using Softland.ERP.FR.Mobile.Cls.Documentos.FRConsignacion;
using Softland.ERP.FR.Mobile.ViewModels;
//using System.Drawing;
using EMF.Win32;

namespace Softland.ERP.FR.Mobile
{
    public class Util
    {
        #region Carga de Companias

        public static List<Cls.FRCliente.ClienteCia> CargarCiasClienteActual()
        {
            return GlobalUI.ClienteActual.ClienteCompania;
        }

        public static List<string> CargarCiasInventario(List<Inventario> documentos)
        {
            List<string> lista = new List<string>();
            foreach (Inventario inv in documentos)
            {
                lista.Add(inv.Compania);
            }
            //OrdenarCombo(lista);
            return lista;
        }

        public static List<string> CargarCias()
        {
            //ABC 22/10/2008
            //Caso: 33706 Reflejo Multicompañia FR 5.0 a FR 6.0
            //De la coleccion de parametros del sistema se obtienen los codigos de cias.            
            List<string> lista = new List<string>();

            foreach (Softland.ERP.FR.Mobile.Cls.ParametroSistema elParametro in Softland.ERP.FR.Mobile.Cls.ParametroSistema.Consecutivos)
            {
                lista.Add(elParametro.Compania);                
            }
            return lista;
        }

        public static List<string> CargarCiasConsignacion(List<VentaConsignacion> documentos, EstadoConsignacion estado)
        {
            List<string> lista = new List<string>();
            foreach (VentaConsignacion venta in documentos)
            {
                if (venta.Estado == estado || estado == EstadoConsignacion.NoDefinido)
                    lista.Add(venta.Compania);
            }
            return lista;
        }

        /// <summary>
        /// Funcion que carga en el combo las compañias en consignacion que el cliente tiene asociadas 
        /// </summary>
        public static List<string> CargarCiasClienteConsignacion()
        {
            List<string> lista = new List<string>();
            //Llenamos el combo de companias con las companias asociadas al cliente
            foreach (Cls.FRCliente.ClienteCia clt in GlobalUI.ClienteActual.ClienteCompania)
            {
                if (clt.Bodega != FRdConfig.NoDefinido)
                    lista.Add(clt.Compania);
            }
            return lista;
        }

        /// <summary>
        /// Funcion que carga en el combo las compañias en consignacion que el cliente tiene asociadas 
        /// </summary>
        public static List<string> CargarCiasClienteActualConsignacion()
        {
            List<string> lista = new List<string>();
            //Llenamos el combo de companias con las companias asociadas al cliente
            foreach (Cls.FRCliente.ClienteCia clt in GlobalUI.ClienteActual.ClienteCompania)
            {
                    lista.Add(clt.Compania);
            }
            return lista;
        }

        /// <summary>
        /// Carga el combo de companias, para los detalles
        /// </summary>
        private static void OrdenarCombo(List<string> listaCompania)
        {
            //ABC 22/10/2008 Caso: 33706 Reflejo Multicompañia FR 5.0 a FR 6.0
            if (listaCompania.Count > 2)
            {
                try
                {
                    listaCompania.Remove("TODAS");
                }
                catch (Exception)
                { }
                //listaCompania.SelectedIndex = 0;
            }
            //else if (listaCompania.Items.Count == 2)
            //{
            //    listaCompania.SelectedIndex = 1;
            //    listaCompania.Enabled = false;
            //}
            //else if (listaCompania.Items.Count == 1)
            //{
            //    listaCompania.SelectedIndex = 0;
            //    listaCompania.Enabled = false;
            //}
        }
        #endregion

        #region Carga Rutas
        /// <summary>
        /// Llenar ComboBox de un Formulario con las rutas
        /// </summary>
        /// <param name="cboCompania"></param>
        //public static void CargarRutasCombo(ref ComboBox cboRuta)
        //{
        //    foreach (Softland.ERP.FR.Mobile.Cls.Corporativo.Ruta ruta in Softland.ERP.FR.Mobile.UI.GlobalUI.Rutas)
        //        cboRuta.Items.Add(ruta.Codigo);

        //    if (cboRuta.Items.Count == 1)
        //        cboRuta.Enabled = false;

        //    cboRuta.SelectedIndex = 0;
        //}

        public static List<string> CargarRutas()
        {
            List<string> lista = new List<string>();

            foreach (Softland.ERP.FR.Mobile.Cls.Corporativo.Ruta ruta in Softland.ERP.FR.Mobile.UI.GlobalUI.Rutas)
            {
                lista.Add(ruta.Codigo);
            }
            return lista;
        }
        
        #endregion

        #region Cargar Familias
        /// <summary>
        /// Llenar ComboBox de un Formulario con las rutas
        /// </summary>
        /// <param name="cboCompania"></param>
        /// <param name="codigosFamilia"></param>
        //public static void CargarFamilias(ref ComboBox cboFamilia, ref List<String> codigosFamilia)
        //{
        //    cboFamilia.Items.Clear();
        //    List<Articulo> familias = Articulo.CargarFamilias();            

        //    // MejorasGrupoPelon600R6 - KF //
        //    foreach (Articulo articulo in familias)
        //    {
        //        switch (Pedidos.DatoFamiliaMostrar)
        //        {
        //            case 0:
        //                cboFamilia.Items.Add(articulo.Familia);
        //                break;
        //            case 1:
        //                cboFamilia.Items.Add(articulo.FamiliaDesc);
        //                break;
        //            case 2:
        //                cboFamilia.Items.Add(String.Concat(articulo.Familia, "-", articulo.FamiliaDesc));
        //                break;
        //        }
        //        codigosFamilia.Add(articulo.Familia);
        //    }            

        //    if (cboFamilia.Items.Count == 1)
        //        cboFamilia.Enabled = false;

        //    cboFamilia.SelectedIndex = 0;
        //}

        
        #endregion

        #region Validar Busquedas

        public static bool ValidarDatos(CriterioArticulo criterio, string compania, string textoConsulta, BaseViewModel viewModel)
        {
            if (criterio == CriterioArticulo.Ninguno)
            {
                viewModel.mostrarMensaje(Mensaje.Accion.SeleccionNula, "un criterio de búsqueda");
                return false;
            }

            if (compania == null)
            {
                viewModel.mostrarMensaje(Mensaje.Accion.SeleccionNula, "una compañía");
                return false;
            }

            if (string.IsNullOrEmpty(textoConsulta))
            {
                viewModel.mostrarAlerta("Ingrese el texto a consultar.");
                return false;
            }
            return true;
        }

        #endregion

        #region Paneles Impresion

        public static void ImpresionCopias(bool imprimir, ref int cantidadCopias)
        {
			if (!imprimir)
			{
                cantidadCopias = 0;
				//txtCantidadCopias.Enabled = false;
			}
			else
			{
				cantidadCopias = Cls.Reporte.Impresora.CantidadCopias;
				//txtCantidadCopias.Enabled = true;
			}        
        }

        public static int CantidadCopias(int cantidad)
        {
            try
            {
                if (cantidad < 0)
                    return 0;
                return cantidad;
            }
            catch (FormatException)
            {
                //Mensaje.mostrarAlerta("Error obteniendo la cantidad de copias. Formato inválido.");
            }
            return 0;
        }

        #endregion Paneles Impresion

        #region MejorasFRTostadoraElDorado600R6 JEV
        public static void aplicarConversiones(ref string almacenp, ref string detallep, double factorEmpaquep)
        {
            decimal cantAlmacen = Convert.ToDecimal(almacenp);
            decimal cantDetalle = Convert.ToDecimal(detallep);
            decimal cantEntAlmacen = 0;
            decimal cantDecAlmacen = 0;
            decimal cantEntDetalle = 0;

            //Se obtiene la parte entera de la cantidad
            cantEntAlmacen = decimal.Truncate(cantAlmacen);

            //Se obtiene la parte decimal de la cantidad
            cantDecAlmacen = decimal.Subtract(cantAlmacen, cantEntAlmacen);

            if (cantDecAlmacen > 0)
            {
                //Se convierte la parte decimal a Detalle y se agrega a la cantidad existente
                cantDetalle = decimal.Multiply(cantDecAlmacen, Convert.ToDecimal(factorEmpaquep));
            }

            //se asigna la parte entera de la cantidad a Almacen
            cantAlmacen = cantEntAlmacen;

            //se convierte la cantidad detalle a Almacen
            cantEntAlmacen = decimal.Divide(cantDetalle, Convert.ToDecimal(factorEmpaquep));

            //Se verifica si de la cantidad Detalle se pueden extraer unidades de Almacen
            if (cantEntAlmacen >= 1)
            {
                //Se toma la parte Entera
                cantEntDetalle = decimal.Truncate(cantEntAlmacen);

                //Se toma la parte Decimal y se pasa a cantidad Detalle
                cantDetalle = decimal.Multiply(decimal.Subtract(cantEntAlmacen, cantEntDetalle), Convert.ToDecimal(factorEmpaquep));

                //Se agrega la parte entera a la cantidad Almacen
                cantAlmacen += cantEntDetalle;
            }
            else
            {
                //Se convierte nuevamente a cantidad Detalle
                cantDetalle = decimal.Multiply(cantEntAlmacen, Convert.ToDecimal(factorEmpaquep));
            }


            //Se aplica el # de decimales especificado en los parametros del módulo.
            //cantDetalle = Math.Round(cantDetalle, Globales.numDecimales);

            almacenp = Convert.ToString(cantAlmacen);
            detallep = Convert.ToString(cantDetalle);
        }

        public static double valorAlmacen(double almacenp, double detallep, double factorEmpaquep)
        {
            double almacen = almacenp;

            almacen += detallep / factorEmpaquep;

            return almacen;

        }

        public static void desglosarValorAlmacen(ref double almacenp, ref double detallep, double factorEmpaquep)
        {
            decimal cantAlmacen = Convert.ToDecimal(almacenp);
            decimal cantEntAlmacen = 0;
            decimal cantDecAlmacen = 0;
            decimal cantDetalle = Convert.ToDecimal(detallep);

            try
            {
                cantEntAlmacen = decimal.Truncate(Convert.ToDecimal(almacenp));
                cantDecAlmacen = decimal.Subtract(cantAlmacen, cantEntAlmacen);

                if (cantDecAlmacen > 0)
                {
                    cantDetalle = decimal.Multiply(cantDecAlmacen, Convert.ToDecimal(factorEmpaquep));
                }

                cantAlmacen = cantEntAlmacen;

                almacenp = Convert.ToDouble(cantAlmacen);
                detallep = Convert.ToDouble(cantDetalle);
            }
            catch (Exception ex)
            {
                throw new Exception("Error desglosando valor almacén.\n" + ex.Message);
            }

        }
        #endregion MejorasFRTostadoraElDorado600R6 JEV
    }
}
