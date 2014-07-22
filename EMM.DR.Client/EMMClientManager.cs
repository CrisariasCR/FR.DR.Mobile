using System.Collections;
using System;
using System.Data.SQLiteBase;
using System.Threading;
using EMMClient.EMMClientLogic;
using System.IO;

namespace EMMClient
{
	/// <summary>
	/// Clase que encapsula el cliente de sincronizacion
	/// </summary>
	public class EMMClientManager
	{
		#region Variables de instancia

		/// <summary>
		/// Logica de sincronizacion
		/// </summary>
		EMMClientLogic.EMMClientActionManager clientActionManager;
		/// <summary>
		/// Parametros del conducto
		/// </summary>
		ArrayList parameters;

        /// <summary>
        /// Indica si el formulario se debe cerrar al finalizr el proceso,
        /// sin la inturrupción del usuario.
        /// </summary>
        private bool cerrarAlFinaliar = false;

        public string NombreDispositivo 
        {
            get { return clientActionManager.NombreDispositivo; }
            set { clientActionManager.NombreDispositivo = value; }
        }

		#endregion

		#region Propiedades de la instancia

		/// <summary>
		/// Retorna un string con los errores ocurridos durante la sincronizacion
		/// </summary>
		public string ErrorLog
		{
			get
			{
				return clientActionManager.ErrorLog;
			}
		}

		/// <summary>
		/// Parametros del conducto a sincronizar
		/// </summary>
		public ArrayList Parameters
		{
			get
			{
				return this.parameters;
			}
		}

		public bool UsarBitacora
		{
			get
			{
				return this.clientActionManager.UsarBitacora;
			}
			set
			{
				this.clientActionManager.UsarBitacora = value;
			}
		}

        /// <summary>
        /// Determina si debe compactar la BD después de realizar las transacciones.
        /// Por omisión es True
        /// </summary>
        public bool Compactar
        {
            get
            {
                return this.clientActionManager.Compactar;
            }
            set
            {
                this.clientActionManager.Compactar = value;
            }
        }

        public string NombreBaseDatosPDA
        {
            get
            {
                return this.clientActionManager.NombreBaseDatosPDA;
            }
            set
            {
                this.clientActionManager.NombreBaseDatosPDA = value;
            }
        }

        /// <summary>
        /// Indica si la ventana se debe cerrar al finalizar.
        /// </summary>
        public bool CerrarAlFinalizar
        {
            get
            {
                return cerrarAlFinaliar;
            }
            set
            {
                cerrarAlFinaliar = value;
            }
        }

        /// <summary>
        /// Valor de la etiqueta de la ventan de sincronización.
        /// </summary>
        public string TitleLabelText
        {
            get
            {
                return clientActionManager.TitleLabelText;
            }
            set
            {
                clientActionManager.TitleLabelText = value;
            }
        }

		#endregion

		#region Constructor

		/// <summary>
		/// Constructor de la clase
		/// </summary>
        public EMMClientManager(string digitalSignature, string userName, string password, string server, string domain, string company, 
            SQLiteConnection cnx, string conduit, bool silent)
		{
			this.clientActionManager = new EMMClient.EMMClientLogic.EMMClientActionManager(domain,server,userName,password,company,conduit,digitalSignature,silent, cnx);
			//Almacena los settings de conexion
			this.parameters =  new ArrayList();

		}

        /// <summary>
        /// Constructor de la clase
        /// </summary>
        public EMMClientManager(string digitalSignature, string userName, string password, string server, string domain, string company,
            SQLiteConnection cnx, string conduit, bool silent,bool desatendido)
        {
            this.clientActionManager = new EMMClient.EMMClientLogic.EMMClientActionManager(domain, server, userName, password, company, conduit, digitalSignature, silent, cnx);
            this.clientActionManager.Desatendido = desatendido;
            //Almacena los settings de conexion
            this.parameters = new ArrayList();

        }

        public EMMClientManager(string digitalSignature, string userName, string password, string server, string domain, string company,SQLiteConnection cnx, string conduit, bool silent , string path)
        {
            this.clientActionManager = new EMMClient.EMMClientLogic.EMMClientActionManager(domain, server, userName, password, company, conduit, digitalSignature, silent, path,cnx);
            //Almacena los settings de conexion
            this.parameters = new ArrayList();

        }

		#endregion

		#region Metodos de instancia

		/// <summary>
		/// Inicia la sincronizacion
		/// </summary>
		/// <param name="company">Nombre de la compañia</param>
		/// <param name="conduit">Nombre del conducto</param>
		/// <param name="parameters">Parametros de la sincronizacion</param>
		public void Synchronize(string nombreDispositivo,ref bool LicenciaValidada)
		{
			bool blink = false;
			bool canceled = false;
			string[] parametersString = new string[parameters.Count];

			int index = 0;
			foreach(string param in this.Parameters)	
				parametersString[index++] = param;
			
			this.clientActionManager.setParameters(parametersString);
            // TODO
			//this.clientActionManager.getStatusForm().ShowWindow();

            if (this.clientActionManager.Login(nombreDispositivo, LicenciaValidada))
            {
                LicenciaValidada = true;

                this.clientActionManager.executeConduit();

                while (this.clientActionManager.CurrentState == ConduitState.Executing)
                {
                    Thread.Sleep(1000);

                    if (canceled)
                    {
                        // TODO
                        //this.clientActionManager.getStatusForm().UpdateInterface(currentImage,
                        //	"Cancelando ...", "",
                        //	this.clientActionManager.CurrentProgress);

                        //Cancelamos la ejecucion del conducto
                        this.clientActionManager.Cancel();
                    }
                    else
                    {
                        // TODO
                        //this.clientActionManager.getStatusForm().UpdateInterface(currentImage,
                        //    TitleLabelText,
                        //    this.clientActionManager.CurrentMessage,
                        //    this.clientActionManager.CurrentProgress);

                        //Verificamos si el usuario no ha cancelado desde la interfaz
                        // TODO
                        //this.clientActionManager.getStatusForm().FocusWindow();
                        // TODO
                        //canceled = this.clientActionManager.getStatusForm().Canceled();
                    }

                    blink = !blink;
                }

                switch (this.clientActionManager.CurrentState)
                {
                    case ConduitState.Aborted:
                        //if (this.clientActionManager.ServerError)
                        // TODO
                        //this.clientActionManager.getStatusForm().ShowErrorServer(clientActionManager.ErrorLog);
                        //else
                        // TODO
                        //this.clientActionManager.getStatusForm().ShowErrorClient(clientActionManager.ErrorLog);
                        break;
                    case ConduitState.Canceled:
                        // TODO
                        //this.clientActionManager.getStatusForm().UpdateInterface(
                        //	ImageStatus.Complete,"Atención","Sincronización cancelada",100);
                        //Caso 34622 enviar al FRm mensaje cancelado en el ErrorLog 26/01/2009
                        this.clientActionManager.ErrorLog = "usercancel";
                        break;
                    case ConduitState.Finished:
                        // TODO
                        //this.clientActionManager.getStatusForm().UpdateInterface(
                        //ImageStatus.Complete,
                        //"Atención",
                        //"Sincronización completa",100);
                        break;
                }
            }
            else
            {
                LicenciaValidada = false;
            }

			//Debemos esperar a que el usuario cierre el formulario
			while(!canceled && !cerrarAlFinaliar)
			{
				Thread.Sleep(1000);
                break;
				//Verificamos si el usuario no ha cancelado desde la interfaz
				// TODO
                //this.clientActionManager.getStatusForm().FocusWindow();
				// TODO
                //canceled = this.clientActionManager.getStatusForm().Canceled();
			}

			//Cerramos la interfaz con el usuario
			// TODO
            //this.clientActionManager.getStatusForm().CloseWindow();
		}

		#endregion

        #region Ejecutar Actualización
        /// <summary>
        /// Lanza la pantalla de actualización, y ejecuta la acciones de actualización
        /// </summary>
        private void Ejecutar(EMMFile[] lista)
        {
            bool blink = false;
            bool canceled = false;
            string[] parametersString = new string[parameters.Count];

            // TODO
            //this.clientActionManager.getStatusForm().ShowWindow();

            this.clientActionManager.EjecutarActualizar(lista);

            while (this.clientActionManager.CurrentState == ConduitState.Executing)
            {
                Thread.Sleep(1000);

                if (canceled)
                {
                    // TODO
                    //this.clientActionManager.getStatusForm().UpdateInterface(currentImage,
                    //    "Cancelando ...", "",
                    //    this.clientActionManager.CurrentProgress);

                    //Cancelamos la ejecucion del conducto
                    this.clientActionManager.Cancel();
                }
                else
                {
                    // TODO
                    //this.clientActionManager.getStatusForm().UpdateInterface(currentImage,
                    //    "Actualizando ...",
                    //    this.clientActionManager.CurrentMessage,
                    //    this.clientActionManager.CurrentProgress);

                    //Verificamos si el usuario no ha cancelado desde la interfaz
                    // TODO
                    //this.clientActionManager.getStatusForm().FocusWindow();
                    // TODO
                    //canceled = this.clientActionManager.getStatusForm().Canceled();
                }

                blink = !blink;
                
            }

            switch (this.clientActionManager.CurrentState)
            {
                case ConduitState.Aborted:
                    //if (this.clientActionManager.ServerError)
                        // TODO
                        //this.clientActionManager.getStatusForm().ShowErrorServer(clientActionManager.ErrorLog);
                    //else
                        // TODO
                        //this.clientActionManager.getStatusForm().ShowErrorClient(clientActionManager.ErrorLog);
                    break;
                case ConduitState.Canceled:
                    // TODO
                    //this.clientActionManager.getStatusForm().UpdateInterface(
                    //    ImageStatus.Complete, "Atención", "Actualización cancelada", 100);
                    //Caso 34622 enviar al FRm mensaje cancelado en el ErrorLog 26/01/2009
                    this.clientActionManager.ErrorLog = "usercancel";
                    break;
                case ConduitState.Finished:
                    // TODO
                    //this.clientActionManager.getStatusForm().UpdateInterface(
                        //ImageStatus.Complete,
                        //"Atención",
                        //"Actualización completa", 100);
                    break;
            }


            //Debemos esperar a que el usuario cierre el formulario
            while (!canceled)
            {
                Thread.Sleep(1000);
                break;
                //Verificamos si el usuario no ha cancelado desde la interfaz
                // TODO
                //this.clientActionManager.getStatusForm().FocusWindow();
                // TODO
                //canceled = this.clientActionManager.getStatusForm().Canceled();
            }

            //Cerramos la interfaz con el usuario
            // TODO
            //this.clientActionManager.getStatusForm().CloseWindow();
        }
        #endregion

        #region Metodos Interfaz Actualización
        /// <summary>
        /// Interfaz para ejecutar el proceso de actualización del CORE de acuerdo al tipo de archivo.
        /// </summary>
        /// <param name="lista">Lista de archivos por actualizar.</param>
        public void ActualizarCore(EMMFile[] lista)
        {
            this.Ejecutar(lista);
        }
        /// <summary>
        /// Interfaz para ejecutar el proceso de actualización de REPORTES de acuerdo al tipo de archivo.
        /// </summary>
        /// <param name="lista">Lista de reportes por actualizar.</param>
        public void ActualizarReportes(EMMFile[] lista)
        {
            this.Ejecutar(lista);
        }
        /// <summary>
        /// Interfaz para ejecutar el proceso de descarga de otros archivos.
        /// </summary>
        /// <param name="lista">Lista de archivos por descargar.</param>
        public void DescargarArchivos(EMMFile[] lista)
        {
            this.Ejecutar(lista);
        }
        /// <summary>
        /// Obtiene la lista de archivos de acuerdo al tipo.
        /// </summary>
        /// <param name="tipo"></param>
        /// <returns></returns>
        public EMMFile[] GetFileList(string tipo)
        {
            return this.clientActionManager.GetFileList(tipo);
        }

        /// <summary>
        /// Actualiza el estado para el archivo dado.
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public int ActualizarEstado(EMMFile file)
        {
            return this.clientActionManager.ActualizarEstado(file);
        }

        /// <summary>
        /// Verifica que el dispositivo tenga actualizaciones pendientes.
        /// </summary>
        /// <returns></returns>
        public bool VerificarActualizacion()
        {
            return this.clientActionManager.VerificarActualizacion();
        }
        #endregion
    }
}
