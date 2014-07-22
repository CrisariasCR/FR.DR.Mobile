using System;
//using System.Net;
using System.Windows.Forms;
using System.Linq;
using System.Windows.Input;
using System.Collections.Generic;

using Cirrious.MvvmCross.Commands;
using Cirrious.MvvmCross.ViewModels;
using Softland.ERP.FR.Mobile.Cls.Reporte;
using EMMClient;
using EMMClient.EMMClientLogic;
using Softland.ERP.FR.Mobile.Cls.Sincronizar;
using FR.Core.Model;
using Softland.ERP.FR.Mobile.UI;
using Softland.ERP.FR.Mobile.Cls;
using Softland.ERP.FR.Mobile.Cls.FRCliente;
using Softland.ERP.FR.Mobile.Cls.Utilidad;
using Softland.ERP.FR.Mobile.Cls.AccesoDatos;

namespace Softland.ERP.FR.Mobile.ViewModels
{
    public class ActualizarCustomViewModel : ListViewModel 
    {
        #region Propiedades

        public static Android.Content.Context contexto;

        private string appPath = string.Empty;

        public bool NohayArchivos = false;

        /// <summary>
        /// tipo de archivo con el que va a realizar la Actualización
        /// </summary>
        public static TipoArchivo TipoArchivo;

        private EMMFile itemActual;
        public EMMFile ItemActual
        {
            get { return itemActual; }
            set
            {
                if (value != itemActual)
                {
                    itemActual = value;
                    RaisePropertyChanged("ItemActual");
                }
            }
        }

        #region Items
        private IObservableCollection<EMMFile> items;
        public IObservableCollection<EMMFile> Items { get { return items; } set { items = value;} }

        private List<EMMFile> ItemsSeleccionados
        {
                get
                {
                    try
                    {
                        return new List<EMMFile>(this.Items.Where<EMMFile>(x => x.Selected));
                    }
                    catch
                    {
                        return new List<EMMFile>();
                    }
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
                for (int i = 0; i < Items.Count; i++)
                {
                    if (Items[i].Selected)
                        result.Add(i);
                }
                return result;
            }
        }

        #endregion Items

        private EMMClientManager clientManager;
        #endregion Propiedades

        public ActualizarCustomViewModel() 
        {
            CargaInicial();
        }

        #region CargaInicial

        public void CargaInicial()
        {
            //Verifica que el contexto no sea nulo sino lo asigna
            if (ActualizarCustomViewModel.contexto == null)
            {
                ActualizarCustomViewModel.contexto = Softland.ERP.FR.Mobile.App.getCurrentActivity().ApplicationContext;
 
            }
            NohayArchivos = false;
            switch (ActualizarCustomViewModel.TipoArchivo)
            {
                case TipoArchivo.Core:
                    appPath=FRmConfig.FullAppPath;
                    break;
                case TipoArchivo.Reporte:
                    appPath = ReportHelper.DevolverRutaReportes();
                    break;
                case TipoArchivo.Otro:
                    appPath = FRmConfig.FullAppPath;
                    break;
            }
            this.InicializarClientManager(FRmConfig.NombreHandHeld,contexto);
            EMMFile[] lista = this.clientManager.GetFileList(((char)ActualizarCustomViewModel.TipoArchivo).ToString());
            if (lista != null && lista.Length > 0)
            {
                this.LlenarTreeView(lista);
            }
            else
            {
                NohayArchivos = true;
            }
        }

        public void MostrarMensajeSinArchivos() 
        {
            this.mostrarMensaje(Mensaje.Accion.Informacion, "No hay archivos por actualizar para este dispositivo",
            result => { this.DoClose(); });
        }

        /// <summary>
        /// Llena el TreeView con los registros de la lista de archivos.
        /// Se parte del hecho que los archivos de un mismo paquete son consecutivos.
        /// </summary>
        private void LlenarTreeView(EMMFile[] lista)
        {
            List<EMMFile> listamtemporal = new List<EMMFile>();

            if (lista.Length > 0)
            {
                // Encabezado del archivo (paquete)
                string version = lista[0].Version.ToString();
                string id = lista[0].ID;
                string tipo = lista[0].Type;

                foreach (EMMFile file in lista)
                {
                    listamtemporal.Add(file);
                }
            }
            IEnumerable<EMMFile> custQuery = from item in listamtemporal orderby item.ID select item;
            listamtemporal = new List<EMMFile>();
            foreach (EMMFile file in custQuery)
            {
                listamtemporal.Add(file);
            }
            Items = new SimpleObservableCollection<EMMFile>(listamtemporal);
            RaisePropertyChanged("Items");
        }


        /// <summary>
        /// Inicializa la instancia de ClientManager
        /// </summary>
        private void InicializarClientManager(string host,Android.Content.Context contexto)
        {
            if (this.clientManager == null)
            {
                this.clientManager = new EMMClientManager(
                    GestorSincronizar.ObtenerFirmaDispositivo(contexto)
                    , GestorDatos.NombreUsuario
                    , GestorDatos.ContrasenaUsuario
                    , GestorDatos.ServidorWS
                    , GestorDatos.Dominio
                    , GestorDatos.Owner
                    , GestorDatos.cnx
                    , "FRa"
                    , false
                    ,this.appPath);
                this.clientManager.Parameters.Add(host);
                this.clientManager.NombreDispositivo = host;
            }
        }

        /// <summary>
        /// Cambia el aspecto de la interfaz deacuerdo al tipo de archivo.
        /// </summary>
        /// <param name="tipo"></param>
        public string ObtenerTitulo()
        {
            string titulo;
            //Cambia el aspecto de la pantalla de acuerdo a tipo de archivo que se va a actualizar.
            switch (ActualizarCustomViewModel.TipoArchivo)
            {
                case TipoArchivo.Core:
                    //this.ptbIcono.Image = Properties.Resources.system_software_update;
                    titulo = "Core";
                    break;
                case TipoArchivo.Reporte:
                    //this.ptbIcono.Image = Properties.Resources.reports;
                    titulo = "Reportes";
                    break;
                case TipoArchivo.Otro:
                    //this.ptbIcono.Image = Properties.Resources.document_save;
                    titulo = "Otros";
                    break;
                default:
                    titulo = "No Definido";
                    break;
            }
            return titulo;
        }

        #endregion CargaInicial

        #region Comandos y Acciones

        public ICommand ComandoAceptar
        {
            get { return new MvxRelayCommand(Aceptar); }
        }
        private void Aceptar()
        {
            if (this.HayNodosSeleccionados())
            {
                this.Actualizar(this.ObtenerLista());
                this.DoClose();
            }
            else
            {
                this.mostrarMensaje(Mensaje.Accion.Alerta, "No se ha seleccionado ningún archivo para actualizar.");
            }
        }

        #endregion Comandos y Acciones
        /// <summary>
        /// Obtiene la lista de archivos por actualizar.
        /// </summary>
        /// <returns></returns>
        private EMMFile[] ObtenerLista()
        {            
            return ItemsSeleccionados.ToArray();
        }

        /// <summary>
        /// Verifica que exista al menos un nodo seleccionado.
        /// </summary>
        /// <returns></returns>
        private bool HayNodosSeleccionados()
        {
            return ItemsSeleccionados.Count > 0;
        }

        /// <summary>
        /// Llama a las funciones de actualización según el tipo de archivo.
        /// </summary>
        /// <param name="lista"></param>
        private void Actualizar(EMMFile[] lista)
        {
            switch (ActualizarCustomViewModel.TipoArchivo)
            {
                case TipoArchivo.Core:
                    this.clientManager.ActualizarCore(lista);
                    break;
                case TipoArchivo.Reporte:
                    this.clientManager.ActualizarReportes(lista);
                    break;
                case TipoArchivo.Otro:
                    this.clientManager.DescargarArchivos(lista);
                    break;
            }
        }

        public  bool Regresar()
        {
            this.ShowMessage("Cancelar Actualización", "Desea cancelar la actualización", 
                MessageBoxButtons.YesNo, MessageBoxIcon.Question, 
                result =>   
                {
                    if (result == DialogResult.Yes)
                    {
                        this.DoClose();
                    }
                });
            return false;
        }

    }

    public enum TipoArchivo
    {
        Core = 'P',
        Reporte = 'R',
        Otro = 'O',
    }
   
}
