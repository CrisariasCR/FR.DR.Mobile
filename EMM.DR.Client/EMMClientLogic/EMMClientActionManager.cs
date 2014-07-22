using System;
using System.Net;
//using System.Windows.Forms;
using System.Data;
using System.Data.SQLiteBase;
using System.IO;
using System.Threading;
using System.Xml.Serialization;
using System.Text;
//using ICSharpCode.SharpZipLib.Zip;
//using EMMClient.Forms;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using ICSharpCode.SharpZipLib.GZip;
using System.Reflection;

using EMMService = EMM.DR.Client.EMMService;

namespace EMMClient.EMMClientLogic
{
	/// <summary>
	/// Clase utilizada para encapsular la logica de la sincronizacion
	/// </summary>
	class EMMClientActionManager
	{	
		#region Constantes

		/// <summary>
		/// A duplicate value cannot be inserted into a unique index.
		/// </summary>
		public const int SSCE_M_KEYDUPLICATE  =  25016;

		#endregion

		#region Variables de instancia

		/// <summary>
		/// Indica el estado actual de la ejecucion del conducto
		/// </summary>
		private ConduitState estado;

        /// <summary>
        /// Indica si la sincronización es desatendida
        /// </summary>
        private bool desatendido;

        public bool Desatendido
        {
            get { return desatendido; }
            set { desatendido = value; }
        }

		/// <summary>
		/// Conexion al servidor remoto
		/// </summary>
		private EMMService.EMMWS emmServer;
		/// <summary>
		/// Puntero al formulario utilizado para mostrar el progreso de la sincronizacion
		/// </summary>
        #pragma warning disable 649
		private IStatusForm statusForm;
		/// <summary>
		/// Base de datos sobre la cual se esta operando
		/// </summary>
		private string currentDatabase;
		/// <summary>
		/// Conexion actual a la base de datos
		/// </summary>
		private SQLiteConnection currentConnection;
		/// <summary>
		/// Informacion de la instacia cliente
		/// </summary>
		private EMMService.EMMClientInfo clientInfo;
		/// <summary>
		/// Dominio de conexion
		/// </summary>
		private string domain;
		/// <summary>
		/// Nombre del servidor a sincronizar
		/// </summary>
		private string serverName;
		/// <summary>
		/// Usuario
		/// </summary>
		private string userName;
		/// <summary>
		/// Password
		/// </summary>
		private string password;
		/// <summary>
		/// Indica el mensaje a desplegar en el form de status
		/// </summary>
		private string currentMessage;
		/// <summary>
		/// Indica el progreso que debe desplegarse en el form de status
		/// </summary>
		private int currentProgress;
		/// <summary>
		/// Bitacora de errores
		/// </summary>
		private string errorLog = "";
		/// <summary>
		/// Indica que el error ocurrió en el servidor
		/// </summary>
		private bool serverError = false;	

		/// <summary>
		/// Flag que indica que la ejecucion del conducto debe ser cancelada por peticion de usuario
		/// </summary>
		private bool userCancel = false;

		/// <summary>
		/// Flag que indica que la ejecucion del conducto debe ser detenida por error
		/// </summary>
		private bool errorStop = false;

		/// <summary>
		/// Indica si se debe utilizar utilizar la bitacora de mensajes.
		/// </summary>
		private bool usarBitacora = false;

		/// <summary>
		/// Instancia de la bitacora.
		/// </summary>
		private Bitacora iBitacora = null;
        
        /// <summary>
        /// Lista de archivos por actualizar. 
        /// </summary>
        private EMMFile[] lista = null;

        /// <summary>
        /// Path donde se ejecuta FR.
        /// </summary>
        private string frPath = null;
        
        /// <summary>
        /// Determina si se debe compactar la BD despúes de realizar una modificación.
        /// </summary>
        private bool compactar = true;

        private string nombreBaseDatos = string.Empty;

        private string titleLabelText = "Sincronizando ...";

		#endregion

		#region Propiedades de la instancia

		/// <summary>
		/// Obtiene el estado actual de la ejecucion del conducto.
		/// </summary>
		public ConduitState CurrentState
		{
			get
			{
				return this.estado;
			}
		}

		public bool ServerError
		{
			get
			{
				return this.serverError;
			}
		}

		public string CurrentMessage
		{
			get
			{
				return this.currentMessage;
			}
		}

		public int CurrentProgress
		{
			get
			{
				return this.currentProgress;
			}
		}

		public string ErrorLog
		{
			get
			{
				return this.errorLog;
			}//Caso 34610 26/01/2009
            set
            {
                this.errorLog = value; 
            }
		}
	
        //public IStatusForm getStatusForm()
        //{
        //    if (this.statusForm == null)
        //        this.statusForm = new StatusForm();
        //    return this.statusForm;			
        //}

		public bool UsarBitacora
		{
			get
			{
				return this.usarBitacora;
			}
			set
			{
				if (value)
				{
					this.usarBitacora = true;
					this.InicializarBitacora();
				}
				else
				{
					this.usarBitacora = false;
					
					if (this.iBitacora != null)
					{
						this.iBitacora.Dispose();
						this.iBitacora = null;
					}
				}
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
                return this.compactar;
            }
            set
            {
                this.compactar = value;
            }
        }


        public string NombreBaseDatosPDA
        {
            get
            {
                return this.nombreBaseDatos;
            }
            set
            {
                this.nombreBaseDatos = value;
            }
        }

        public string TitleLabelText
        {
            get
            {
                return titleLabelText;
            }
            set
            {
                titleLabelText = value;
            }
        }
		#endregion 

		#region Constructor

		/// <summary>
		/// Contructor de la clase
		/// </summary>
		/// <param name="statusForm">Puntero al formulario utilizado para mostrar el progreso de la sincronizacion</param>
		public EMMClientActionManager(string domain,string serverName,string userName, string password,string company,string conduit,string digitalSignature,bool silent,
            SQLiteConnection cnx)
		{
			clientInfo =  new EMMService.EMMClientInfo();

			this.clientInfo.company = company;
			this.clientInfo.conduit = conduit;
			this.clientInfo.digitalSignature = digitalSignature;
            if (serverName.ToUpper().Equals("LOCAL") || serverName.ToUpper().Equals("LOCALHOST"))
            {
                this.serverName = "10.0.2.2";//Asi ve android el localhost
            }
            else
            {
                this.serverName = serverName;
            }
            //this.serverName = "10.0.2.2";//Asi ve android el localhost
			this.userName = userName;
			this.password = password;
			this.domain = domain;
            this.currentProgress = 0;
            this.currentConnection = cnx;

            //if (silent)			
            //    this.statusForm = new DummyStatusForm();			
            //else
            //    this.statusForm = new StatusForm();

            this.InicializarServicio();
		}

        /// <summary>
        /// Contructor de la clase
        /// </summary>
        /// <param name="statusForm">Puntero al formulario utilizado para mostrar el progreso de la sincronizacion</param>
        public EMMClientActionManager(string domain, string serverName, string userName, string password, string company, string conduit, string digitalSignature, bool silent, string path, SQLiteConnection cnx)
        {
            clientInfo = new EMMService.EMMClientInfo();

            this.clientInfo.company = company;
            this.clientInfo.conduit = conduit;
            this.clientInfo.digitalSignature = digitalSignature;
            if (serverName.ToUpper().Equals("LOCAL") || serverName.ToUpper().Equals("LOCALHOST"))
            {
                this.serverName = "10.0.2.2";//Asi ve android el localhost
            }
            else
            {
                this.serverName = serverName;
            }
            //this.serverName = "10.0.2.2";//Asi ve android el localhost
            this.userName = userName;
            this.password = password;
            this.domain = domain;
            this.currentProgress = 0;
            this.currentConnection = cnx;
            //if (silent)
            //    this.statusForm = new DummyStatusForm();
            //else
            //    this.statusForm = new StatusForm();

            this.FRPath = path;

            this.InicializarServicio();
        }
		#endregion 

		#region Metodos de instancia

		public void setParameters(string [] parameters)
		{
			this.clientInfo.parameters = parameters;
		}

		/// <summary>
		/// Realiza el login con el servidor de sinconizacion. 
		/// Obtiene el id de la sesion de sincronizacion
		/// </summary>
		/// <param name="userrname">Nombre de usuario</param>
		/// <param name="password">Password del usuario</param>
		/// <param name="server">Nombre del servidor</param>
		/// <param name="domain">Dominio del servidor</param>
		/// <returns>Retorna verdadero si se pudo realizar el login en el servidor remoto</returns>
        public bool Login(string NombreDispositivo,bool LicenciaValidada)
		{		

			//Indica si el resultado del login fue exitoso
			bool result = false;

			this.errorLog = "";			

			//Respuesta del servidor
			EMMService.EMMToken response;

            //TODO
			//this.statusForm.UpdateInterface(ImageStatus.Connecting,titleLabelText, "Contactando al servidor y autenticando ...",0);

			//Obtiene el nombre de la PDA
            clientInfo.deviceName = NombreDispositivo;

			//Borra todos los mensajes de error antiguos
			this.currentMessage="Iniciando ...";

			//Crea una instancia al servidor remoto
			emmServer = new EMMService.EMMWS();
            emmServer.Timeout = 600000;
			emmServer.Url = "http://"+ this.serverName + "/EMMService/EMMWS.asmx";
            
            //http://localhost/Prueba3/EMMWS.asmx
            //emmServer.Url = "http://abolanos/Prueba3/EMMWS.asmx";

			//Crea las credenciales de conexion del cliente
			if (domain==null||domain=="")
				emmServer.Credentials = new NetworkCredential(this.userName, this.password);			
			else
				emmServer.Credentials = new NetworkCredential(this.userName, this.password, this.domain);			
		
			try
			{	
				response = emmServer.StartSynchronization(clientInfo);	

				if (response!=null)
				{
					if (response.type == EMMService.EMMTokenType.restoreStateRequest)
					{
                        Thread.Sleep(6000);
                        //TODO
						//this.statusForm.UpdateInterface(ImageStatus.SyncServer,titleLabelText,"Iniciando ...",0);						
                        
                        //LJR 15/01/2009 
                        //Caso 34610
                        //Forzar el reinicio completo de una sincronizacion no finalizada correctamente

						//if (this.statusForm.ShowDialog("La última sincronización no fue finalizada. Desea continuarla?")==DialogResult.No)						
						//{
						//this.statusForm.UpdateInterface(ImageStatus.SyncServer,titleLabelText,"Reiniciando ...",0);						
						emmServer.ClearState(clientInfo.company,clientInfo.deviceName,clientInfo.conduit);                        	
						response = emmServer.StartSynchronization(clientInfo);						
						//}
						//else return true;
					}

					if (response.type == EMMService.EMMTokenType.response)					
						result =  true;

					if (response.type == EMMService.EMMTokenType.error)
					{
						this.statusForm.ShowErrorServer(response.result);
						this.errorLog += "\n" + response.result;
						result =  false;
					}

					if (response.type == EMMService.EMMTokenType.invalidLicense)
					{
                        //TODO
						//this.statusForm.UpdateInterface(ImageStatus.AuthenticationError,"Error de sincronización","Licencia inválida.",0);
						this.errorLog += "\n" + "Licencia inválida.";
						result =  false;
					}				
				}
				else
				{
                    //TODO
					//this.statusForm.ShowErrorServer("Ocurrió un error en el servidor al iniciar la sincronización");				
					result =  false;
				}
			}
			catch (Exception e)
			{
                //TODO
				//this.statusForm.UpdateInterface(ImageStatus.ConnectionError,"Error de sincronización",e.Message,0);
				this.errorLog += "\n" + e.Message;
				result =  false;
			}		
			
			return result;
		}


		public void executeConduitThread()
		{
			int progress = 0;

			EMMService.EMMAction[] actions;
		
			try
			{
                if(this.NombreBaseDatosPDA != string.Empty)
                    emmServer.SetDataBaseNamePDA(this.NombreBaseDatosPDA);
	
				actions = emmServer.GetActions(this.clientInfo.company,this.clientInfo.deviceName,this.clientInfo.conduit);

				if (actions == null)
				{
					this.serverError = true;
					throw new Exception("El servidor no retornó acciones.");
				}

				foreach(EMMService.EMMAction action in actions)
				{
					progress++;

					this.EscribirBitacora("Progreso de sincronizacion: " + progress);

					if (actions.Length > 0)
						this.currentProgress = ( progress * 100 ) / (actions.Length + 1);

					switch(action.type)
					{
						case EMMService.EMMActionType.createTable:
							this.EscribirBitacora("Inicia acción tipo CreateTable.");
							createTable(action);
							break;

						case EMMService.EMMActionType.executeStatementPDA:
							this.EscribirBitacora("Inicia acción tipo executeStatementPDA.");
                            if (progress == 72)
                                this.EscribirBitacora("Acción tipo recibir tabla del servidor.");
							nextAction();
                            action.statement.statement = CambiarDATEDIFF(action.statement.statement);
							executeStatement(action.statement);
							break;

						case EMMService.EMMActionType.receiveTable:
							this.EscribirBitacora("Acción tipo enviar tabla al servidor.");
							sendTable(action);
							break;

						case EMMService.EMMActionType.sendTable:
							this.EscribirBitacora("Acción tipo recibir tabla del servidor.");
							receiveTable();
							break;

						case EMMService.EMMActionType.synchronizeTable:
							this.EscribirBitacora("Acción tipo sincronizar tabla.");
							synchronizeTable(action);
							break;
						
						default:
							this.EscribirBitacora("Acción de tipo desconozida.");
							nextAction();
							break;
					}

					//Ocurrio un error o el usuario cancelo la ejecucion
					if (this.errorStop || this.userCancel)
						break;
				}

				if (!this.errorStop && !this.userCancel)
					emmServer.EndSynchronization(this.clientInfo.company,this.clientInfo.deviceName,this.clientInfo.conduit);

                if(!Desatendido)
				    closeDatabase();
			}
			catch(Exception e)
			{
				this.errorLog += "\n" + e.Message;
				this.errorStop = true;
			}

			if (this.errorStop) //Ocurrio un error
				this.estado = ConduitState.Aborted;
			else
				if (this.userCancel)  //El usuario cancelo la ejecucion del conducto
				this.estado = ConduitState.Canceled;
			else
				this.estado = ConduitState.Finished;
			
		}

		/// <summary>
		/// Informa que la ejecucion del conducto se debe detener.
		/// </summary>
		public void Cancel()
		{
			this.userCancel = true;
		}

        //Reemplaza la funcion DATEDIFF por su equivalente en SQLLite
        public string CambiarDATEDIFF(string statement) 
        {
            if (statement.Contains("DATEDIFF"))
            {
                string result = statement;
                int Index = statement.IndexOf("DATEDIFF");
                string parte1 = statement.Substring(0, Index);
                string parte2 = statement.Substring(Index);
                Index = parte2.IndexOf("())");
                string parte3 = parte2.Substring(0, Index + 3);
                string parte4 = parte2.Substring(Index + 3);
                parte3 = (parte3.Split(Convert.ToChar(",")))[1].ToString();
                parte3 = "(julianday(date('now','localtime')) - julianday(" + parte3 + "))";            //
                statement = parte1 + parte3 + parte4;
                return statement;
            }
            else 
            {
                return statement;
            }
        }

		/// <summary>
		/// Valida que un token devuelto por el servidor de sincronizacion este válido.
		/// Válido significa no nulo y con un EMMTokenType diferente de EMMTokenType.Error
		/// </summary>
		/// <param name="tokenError"></param>
		/// <returns></returns>
		private bool verificarToken(EMMService.EMMToken token)
		{
			if (token == null || token.type == EMMService.EMMTokenType.error)
			{
				this.serverError = true;
				if (token != null && token.result != "")
					errorLog += "\n" + token.result;
				else
					errorLog = "Ocurrió un error en el servidor.";
				this.errorStop = true;

				return false;
			}

			return true;
		}

		private void nextAction()
		{
			//Cuando hay que detenerse no se puede ejecutar nada mas.
			if (errorStop) return;
			
			EMMService.EMMToken token =  new EMMService.EMMToken();
			
			token.type = EMMService.EMMTokenType.response;			

			try
			{
				token = emmServer.ExecuteAction(this.clientInfo.company,this.clientInfo.deviceName,this.clientInfo.conduit,token);
			}
			catch(Exception e)
			{
				this.serverError = true;
				errorLog += "\n" + e.Message;
				this.errorStop = true;
			}

			if (!errorStop)
				verificarToken(token);
		}

		/// <summary>
		/// Obtiene una tabla del servidor y la escribe en el dispositivo
		/// </summary>
		/// <param name="statusForm"></param>
		private void receiveTable()
		{
			//Cuando hay que detenerse no se puede ejecutar nada mas.
			if (errorStop) return;

			EMMService.EMMToken token = null;

			currentMessage = "Recibiendo datos ...";

			try
			{
				token = emmServer.ExecuteAction(this.clientInfo.company,this.clientInfo.deviceName,this.clientInfo.conduit,null);
			}
			catch(Exception exc)
			{
				this.serverError = true;
				errorLog += "\n" + exc.Message;
				this.errorStop = true;
			}

			if (!errorStop && verificarToken(token))
			{
				currentMessage = "Almacenando datos ...";

				this.EscribirBitacora("Se va a crear la estructura de la tabla con drop first.");

				CreateTableStructure(token.table,true);
				
				if (token.table.data.Length > 0)
				{
					this.EscribirBitacora("La tabla tiene datos(" + token.table.data.Length + ")... se procede a registrar los datos.");
					InflateTable(token.table);
				}
			}
		}

		/// <summary>
		/// Ejecuta un conducto en el servidor
		/// </summary>
		public void executeConduit()
		{			
			//this.executing = true;
			this.estado = ConduitState.Executing;
			Thread t = new Thread(new ThreadStart(executeConduitThread));
			t.Start();
		}

		/// <summary>
		/// Retorna los campos que son llave de una tabla
		/// </summary>
		/// <param name="tablename"></param>
		/// <returns></returns>
		public System.Data.DataColumn[] GetKeys(EMMService.SQLCETable table)
		{
			//Conexion a la base de datos
			SQLiteConnection connection;

			//Adaptador de la base de datos
			SQLiteDataAdapter adapter;

			try
			{
				//Schema de la tabla
				DataSet schema = new DataSet();

				//Crea una conexion a la base de datos
				connection = openDatabase(table.database);

				//Crea un nuevo data adapter
				adapter = new SQLiteDataAdapter("SELECT * FROM " + table.name,connection);
			
				//Lee el schema de la tabla
				adapter.FillSchema(schema,SchemaType.Source,table.name);

				//Retorna el schema de la tabla
				return schema.Tables[table.name].PrimaryKey;
			}
			catch(Exception exc)
			{
				errorLog += "\n" + exc.Message;
				this.errorStop = true;
				return null;
			}
		}

		/// <summary>
		/// Expande los registros del la tabla en el cliente y actualiza los registros de la tabla
		/// </summary>
		/// <param name="table">Tabla compresa</param>
		private void InflateUpdateTable(EMMService.SQLCETable table)
		{
			//Cuando hay que detenerse no se puede ejecutar nada mas.
			if (errorStop) return;
			
			//Cantidad de bytes disponibles para leer del stream comprimido
			int available;			
			//Tamaño del registro que se esta leyendo
			int recordsize;			
			//Bloque utilizado para almacenar los datos descompresos
			byte[] buffer=new byte[65536];

			//Conexion a la base de datos
			SQLiteConnection connection = null;
			
			//Comando del SQL utilizado para la inserción
			SQLiteCommand insertCommand = null;

			//Comando del SQL utilizado para la inserción
			SQLiteCommand updateCommand = null;

			//Transacción de SQL utilizada para generar la insercion de registros
			//SQLiteTransaction transaction=null;
			
			//Se crea un string de tamaño inicial de 2048 caracteres utilizado para almacenar la información
			//de la sentencia insert
			System.Text.StringBuilder recorddata = new System.Text.StringBuilder(2048);

			//Parte inicial de la sentencia de actualizacion
			string updateSQL;
			//Parte final de la sentencia de actualizacion
			string updateSQLEnding;
			
			//Parte inicial de la sentencia de actualizacion
			string insertSQL;
			//Parte final de la sentencia de actualizacion
			string insertSQLEnding;

			//Crea un stream en memoria del cual se pueden leer los datos comprimidos
			System.IO.MemoryStream zippeddata = new MemoryStream(table.data);

			//Crea un stream para extraer datos descomprimidos del stream con datos comprimidos
			ICSharpCode.SharpZipLib.Zip.Compression.Streams.InflaterInputStream records = new ICSharpCode.SharpZipLib.Zip.Compression.Streams.InflaterInputStream(zippeddata);

			//Indica si el registro debe ser actualizado o insertado
			bool doInsert = false;

			//Valor serializado a insertar
			string serializedValue;

			//Indice auxiliar
			int indexField;

			try 
			{			

				//Crea una conexion a la base de datos
				connection = openDatabase(table.database);
				
				//Crea un nuevo comando
				insertCommand = connection.CreateCommand();
				//Crea un nuevo comando
				updateCommand = connection.CreateCommand();

				//Crea una transaccion
				connection.BeginTransaction();

                // mvega: no hace falta, pues la conexion es la que tiene la transaccion
                // y ambos comandos se crearon a partir de la misma conexion
                ////Liga el comando con la transaccion
                //insertCommand.Transaction = transaction;

                ////Liga el comando con la transaccion
                //updateCommand.Transaction = transaction;


				/* CONSTRUCCION DE LA SENTENCIA DE ACTUALIZACION */

				//Construye la parte inicial de la sentencia de insercion				
				updateSQL = "UPDATE " + table.name+ " SET ";
				//Construye la parte final de la sentencia de insercion			
				updateSQLEnding = "";

                SQLiteParameterList updateParameters = new SQLiteParameterList();

				for (int index=0;index<table.fields.Length;index++)			
				{
					//updateParameters.Add(table.fields[index].name,GetSQLDBType(table.fields[index].type)); //,table.fields[index].size);
                    updateParameters.Add(new SQLiteParameter(table.fields[index].name, GetSQLDBType(table.fields[index].type), null));

					if (index<table.fields.Length-1)
                        updateSQL += table.fields[index].name + " = @" + table.fields[index].name + ", ";
					else
                        updateSQL += table.fields[index].name + " = @" + table.fields[index].name;

					if (table.fields[index].primarykey)
					{
						if (updateSQLEnding!="")
							updateSQLEnding += ",";

						updateSQLEnding += table.fields[index].name+ " = @"+table.fields[index].name;
					}
				}

				//Crea los parametros para las llaves
                for (int index = 0; index < table.fields.Length; index++)
                {
                    if (table.fields[index].primarykey)
                    {
                        //updateParameters.Add("IDX" + table.fields[index].name, GetSQLDBType(table.fields[index].type)); //, table.fields[index].size);
                        updateParameters.Add(new SQLiteParameter("IDX" + table.fields[index].name, GetSQLDBType(table.fields[index].type), null));
                    }
                }
			
				//Termina la construccion del comando de insercion
				updateCommand.CommandText = updateSQL + " WHERE " + updateSQLEnding;	


				/* CONSTRUCCION DE LA SENTENCIA DE INSERCION */

				//Construye la parte inicial de la sentencia de insercion				
				insertSQL = "INSERT INTO " + table.name+ " ( ";
				//Construye la parte final de la sentencia de insercion			
				insertSQLEnding = "";

                SQLiteParameterList insertParameters = new SQLiteParameterList();
				for (int index=0;index<table.fields.Length;index++)			
				{
                    //insertParameters.Add(table.fields[index].name, GetSQLDBType(table.fields[index].type), table.fields[index].size);
                    insertParameters.Add(new SQLiteParameter(table.fields[index].name, GetSQLDBType(table.fields[index].type), null));

					if (index<table.fields.Length-1)
					{						
						insertSQL += table.fields[index].name + ",";
                        insertSQLEnding += "@" + table.fields[index].name + ",";
					}
					else
					{
                        insertSQLEnding += "@" + table.fields[index].name;
						insertSQL += table.fields[index].name;			
					}
				}

				//Verifica si la tabla tiene el campo 'Created'
				if (table.createdField)
				{
					insertSQL += ", CREATED";
					insertSQLEnding += ",0 ";
				}

				//Verifica si la tabla tiene el campo 'Updated'
				if (table.updatedField)
				{
					insertSQL += ", UPDATED";
					insertSQLEnding += ",0 ";
				}
				
				//Finaliza la parte inicial de la sentencia de insercion
				insertSQL +=  " ) VALUES (";	
				//Cierra la sentencia de insercion			
				insertSQLEnding += ")";

				//Asigna el texto del comando
				insertCommand.CommandText = insertSQL + insertSQLEnding;

				//Obtiene la cantidad de bytes disponibles para leer
				available = ReadBytes(buffer,records,2);			

				//Obtiene el tamaño del primer registro de la tabla
				recordsize = GetInteger(buffer);

				//Prepara los comandos
                //insertCommand.Prepare(insertParameters);
                //updateCommand.Prepare(updateParameters);


				//Mientras existan datos por leer
				while (available!=0)
				{
					indexField = -1;

					//Extrae los valores de las columnas
					while (indexField<table.fields.Length)
					{
						//Lee del stream compreso el registro
						available = ReadBytes(buffer,records,recordsize);

						if (available!=0)
						{
							//Obtiene el dato serializado
							serializedValue = ASCIIEncoding.UTF8.GetString(buffer,0,available);

							if (indexField!=-1)
							{
								if (doInsert)
                                    insertParameters.Parametros[table.fields[indexField].name].Value = ParseType(serializedValue,
										table.fields[indexField].type);
								else
								{
									updateParameters.Parametros[table.fields[indexField].name].Value = ParseType(serializedValue,
										table.fields[indexField].type);

									if (table.fields[indexField].primarykey)
										updateParameters.Parametros["IDX" + table.fields[indexField].name].Value = 
											updateParameters.Parametros[table.fields[indexField].name].Value;
								}
							}
							else							
								//Deermina si la operacion es un INSERT o un UPDATE
								doInsert = (bool)ParseType(	serializedValue,EMMService.SQLCEType.db_bit);							
							
							available = ReadBytes(buffer,records,2);
							recordsize = GetInteger(buffer);																
						}
						else						
							available=0;	
					
						indexField++;
					}

					try		
					{
                        //Prepara los comandos
                        insertCommand.Prepare(insertParameters);
                        updateCommand.Prepare(updateParameters);
						//Verifica si el comando es una insercion o una actualizacion
						if (doInsert)						
						{
                            
							//Ejecuta la sentencia
							insertCommand.ExecuteNonQuery();	
						}
						else
						{
							//Ejecuta la sentencia
							updateCommand.ExecuteNonQuery();	
						}
					}
					catch (SQLiteException e) //En caso de ocurrir una excepcion la captura
					{											
						//Los errores de llave duplicada son ignorados
						if (e.NativeError!=SSCE_M_KEYDUPLICATE)
							throw new Exception("Error durante la inserción de registros. " + e.Message);
					}								

				}	
				
				//Aplica la transaccion a la base de datos
                connection.Commit();
			}
			catch(Exception e)//En caso de ocurrir una excepcion desconocida
			{
				//Guarda el error en la bitacora
				this.errorLog += "\n" + e.Message;

				if (insertCommand!=null)
					insertCommand.Dispose();

				if (updateCommand!=null)
					updateCommand.Dispose();


                if (connection != null && connection.IsInTransaction)
				{
                    connection.Rollback();
                    connection.Dispose();
				}

				this.errorStop = true;
			}

			//Cierra los streams con la informacion de la tabla
			records.Close();
			zippeddata.Close();
		}

		/// <summary>
		/// Expande los registros del la tabla en el cliente
		/// </summary>
		/// <param name="table">Tabla compresa</param>
		private void InflateTable(EMMService.SQLCETable table)
		{
			//Cuando hay que detenerse no se puede ejecutar nada mas.
			if (errorStop) return;
			
			//Cantidad de bytes disponibles para leer del stream comprimido
			int available;			
			//Tamaño del registro que se esta leyendo
			int recordsize;			
			//Bloque utilizado para almacenar los datos descompresos
			byte[] buffer=new byte[65536];

			//Conexion a la base de datos
			SQLiteConnection connection = null;
			//Comando del SQL utilizado para la inserción
			SQLiteCommand cmd=null;
			//Transacción de SQL utilizada para generar la insercion de registros
			//SQLiteTransaction transaction=null;
			
			//Se crea un string de tamaño inicial de 2048 caracteres utilizado para almacenar la información
			//de la sentencia insert
			System.Text.StringBuilder recorddata = new System.Text.StringBuilder(2048);

			//Parte inicial de la sentencia de insercion
			string insertcommand;

			//Parte final de la sentencia de insercion
			string insertcommandending;

			//Crea un stream en memoria del cual se pueden leer los datos comprimidos
			System.IO.MemoryStream zippeddata = new MemoryStream(table.data);

			//Crea un stream para extraer datos descomprimidos del stream con datos comprimidos
			ICSharpCode.SharpZipLib.Zip.Compression.Streams.InflaterInputStream records = new ICSharpCode.SharpZipLib.Zip.Compression.Streams.InflaterInputStream(zippeddata);

			try 
			{				

				//Crea una conexion a la base de datos
				connection = openDatabase(table.database);
				//Crea un nuevo comando
				cmd = connection.CreateCommand();
				//Crea una transaccion
				connection.BeginTransaction();
				//Liga el comando con la transaccion
				//cmd.Transaction=transaction;
				//Construye la parte inicial de la sentencia de insercion				
				insertcommand= "INSERT INTO " + table.name+ " ( ";
				//Construye la parte final de la sentencia de insercion			
				insertcommandending = "";

                SQLiteParameterList cmdParameters = new SQLiteParameterList();

				for (int index=0;index<table.fields.Length;index++)			
				{
					//cmdParameters.Add(table.fields[index].name,GetSQLDBType(table.fields[index].type)); //,table.fields[index].size);
                    cmdParameters.Add(new SQLiteParameter(table.fields[index].name,GetSQLDBType(table.fields[index].type),null));

					if (index<table.fields.Length-1)
					{						
						insertcommand +=  table.fields[index].name + ",";
						//insertcommandending += "?,";
                        insertcommandending += "@" + table.fields[index].name + ",";
					}
					else
					{
						//insertcommandending += "?";
                        insertcommandending += "@" + table.fields[index].name;
						insertcommand += table.fields[index].name;			
					}
				}

				//Verifica si la tabla tiene el campo 'Created'
				if (table.createdField)
				{
					insertcommand += ", CREATED";
					insertcommandending  += ",0 ";
				}

				//Verifica si la tabla tiene el campo 'Updated'
				if (table.updatedField)
				{
					insertcommand += ", UPDATED";
					insertcommandending += ",0 ";
				}

				//Finaliza la parte inicial de la sentencia de insercion
				insertcommand += " ) VALUES (";	
				//Cierra la sentencia de insercion			
				insertcommandending += ")";
				//Termina la construccion del comando de insercion
				cmd.CommandText = insertcommand + insertcommandending;

				this.EscribirBitacora("InflateTable: El texto de inserción de datos es " + cmd.CommandText);

				//Obtiene la cantidad de bytes disponibles para leer
				available = ReadBytes(buffer,records,2);

				this.EscribirBitacora("InflateTable: Available es " + available);

				//Obtiene el tamaño del primer registro de la tabla
				recordsize = GetInteger(buffer);

				this.EscribirBitacora("InflateTable: Recordsize es " + recordsize);

                //cmd.Prepare(cmdParameters);

				//Mientras existan datos por leer
				while (available!=0)
				{	
					for (int index=0;index<table.fields.Length;index++)
					{
						//Lee del stream compreso el registro
						available = ReadBytes(buffer,records,recordsize);

						if (available!=0)
						{
                            if (table.fields[index].type == EMMService.SQLCEType.db_binary ||
                                table.fields[index].type == EMMService.SQLCEType.db_varbinary ||
                                table.fields[index].type == EMMService.SQLCEType.db_image)
                            {
                                if (available == 1 && buffer[0] == 0) //Revisamos si el campo es NULL
                                    cmdParameters.Parametros["@" + table.fields[index].name].Value = DBNull.Value;
                                else
                                {
                                    byte[] valor = new byte[available];

                                    for (int i = 0; i < available; i++)
                                        valor[i] = buffer[i];

                                    cmdParameters.Parametros["@" + table.fields[index].name].Value = valor;
                                }
                            }
                            else
                            {
                                //cmdParameters[index].Value = ParseType(
                                //    ASCIIEncoding.UTF8.GetString(buffer, 0, available),
                                //    table.fields[index].type);
                                cmdParameters.Parametros["@" + table.fields[index].name].Value = ParseType(
                                    ASCIIEncoding.UTF8.GetString(buffer, 0, available),
                                    table.fields[index].type);
                            }

							available = ReadBytes(buffer,records,2);
							recordsize = GetInteger(buffer);																
						}
					}

					try		
					{
						//Ejecuta la consulta
                        cmd = connection.CreateCommand(cmd.CommandText, cmdParameters);
						cmd.ExecuteNonQuery();	
					}
					catch (SQLiteException e)//En caso de ocurrir una excepcion la captura
					{											
						//Los errores de llave duplicada son ignorados
						if (e.NativeError!=SSCE_M_KEYDUPLICATE)
						{
							this.EscribirBitacora("InflateTable: Excepcion al ejecutar query número " + e.NativeError);
							throw new Exception("Error durante la inserción de registros. " + e.Message);
						}
					}								

				}	
				
				//Aplica la transaccion a la base de datos
                connection.Commit();
			}
			catch(Exception e)//En caso de ocurrir una excepcion desconocida
			{
				//Guarda el error en la bitacora
				this.errorLog += "\n" + e.Message;

				if (cmd!=null)
					cmd.Dispose();

                if (connection != null)
				{
                    connection.Rollback();
                    connection.Dispose();
				}

				this.errorStop = true;
			}

			//Cierra los streams con la informacion de la tabla
			records.Close();
			zippeddata.Close();
		}

		object ParseType(string stringValue, EMMService.SQLCEType type)		
		{	
			
			object returnObject=null;
		
			if (stringValue!="\0")
			{			

				switch (type)
				{
					case EMMService.SQLCEType.db_int:
						returnObject = Int32.Parse(stringValue, CultureInfo.InvariantCulture);
						break;
					case EMMService.SQLCEType.db_bigint:					
						returnObject = Int64.Parse(stringValue, NumberStyles.Any , CultureInfo.InvariantCulture);
						break;
					case EMMService.SQLCEType.db_numeric:					
						returnObject = decimal.Parse(stringValue, CultureInfo.InvariantCulture);
						break;
					case EMMService.SQLCEType.db_smallint:					
						returnObject = Int16.Parse(stringValue, NumberStyles.Any , CultureInfo.InvariantCulture);
						break;
					case EMMService.SQLCEType.db_tinyint:					
						returnObject = byte.Parse(stringValue, NumberStyles.Any , CultureInfo.InvariantCulture);
						break;
					case EMMService.SQLCEType.db_bit:					
						returnObject = bool.Parse(stringValue);
						break;
					case EMMService.SQLCEType.db_real:					
						returnObject = Single.Parse(stringValue, CultureInfo.InvariantCulture);
						break;
					case EMMService.SQLCEType.db_float:					
						returnObject = double.Parse(stringValue, CultureInfo.InvariantCulture);
						break;
					case EMMService.SQLCEType.db_money:					
						returnObject = decimal.Parse(stringValue, CultureInfo.InvariantCulture);				
						break;
					case EMMService.SQLCEType.db_datetime:
						returnObject = DateTime.Parse(stringValue, CultureInfo.InvariantCulture);
						break;
					default:									
						returnObject = stringValue;		
						break;		
				}
			}
			else
			{
				returnObject = DBNull.Value; 
                //returnObject = null;
			}

			return returnObject;
		}

        /// <summary>
        /// Obtiene el valor de un campo del reader y lo retorna en forma
        /// de cadena, pero utilizando la cultura invariante.
        /// </summary>
        /// <param name="reader">Reader conectado a la base de datos</param>
        /// <param name="index">Indice del reader a consultar.</param>
        /// <returns>El valor convertido a string, pero con cultura invariante</returns>
        private byte[] ObtenerValorReader(SQLiteDataReader reader, int index)
        {
            EMMService.SQLCEType type = SqlHelper.GetSQLCEType(((SqlDbType)reader.GetDataTypeName(index)));

            string valor=string.Empty;
            try
            {
                if (!reader.IsDBNull(index))
                {
                    switch (type)
                    {
                        case EMMService.SQLCEType.db_int:
                            int entero = reader.GetInt32(index);
                            valor = entero.ToString(CultureInfo.InvariantCulture);
                            break;

                        case EMMService.SQLCEType.db_bigint:
                            long largo = reader.GetInt64(index);
                            valor = largo.ToString(CultureInfo.InvariantCulture);
                            break;

                        case EMMService.SQLCEType.db_numeric:
                            decimal dec = reader.GetDecimal(index);
                            valor = dec.ToString(CultureInfo.InvariantCulture);
                            break;

                        case EMMService.SQLCEType.db_smallint:
                            short corto = reader.GetInt16(index);
                            valor = corto.ToString(CultureInfo.InvariantCulture);
                            break;

                        case EMMService.SQLCEType.db_tinyint:
                            byte by = reader.GetByte(index);
                            valor = by.ToString(CultureInfo.InvariantCulture);
                            break;

                        case EMMService.SQLCEType.db_bit:
                            bool bol = reader.GetBoolean(index);
                            valor = bol.ToString(CultureInfo.InvariantCulture);
                            break;

                        case EMMService.SQLCEType.db_real:
                            float flotante = reader.GetFloat(index);
                            valor = flotante.ToString(CultureInfo.InvariantCulture);
                            break;

                        case EMMService.SQLCEType.db_float:
                            double doble = reader.GetDouble(index);
                            valor = doble.ToString(CultureInfo.InvariantCulture);
                            break;

                        case EMMService.SQLCEType.db_money:
                            decimal deci = reader.GetDecimal(index);
                            valor = deci.ToString(CultureInfo.InvariantCulture);
                            break;

                        case EMMService.SQLCEType.db_datetime:
                            DateTime fecha = reader.GetDateTime(index);
                            valor = fecha.ToString(CultureInfo.InvariantCulture);
                            break;

                        case EMMService.SQLCEType.db_nvarchar:
                        case EMMService.SQLCEType.db_nchar:
                        case EMMService.SQLCEType.db_ntext:
                            valor = reader.GetString(index);
                            break;

                        case EMMService.SQLCEType.db_uniqueidentifier:
                            valor = reader.GetGuid(index).ToString();
                            break;

                        case EMMService.SQLCEType.db_binary:
                        case EMMService.SQLCEType.db_varbinary:
                        case EMMService.SQLCEType.db_image:

                            //Caso especial
                            byte[] buffer = (byte[])reader.GetValue(index);
                            return buffer;

                        default:
                            valor = reader.GetValue(index).ToString();
                            break;
                    }
                }
                else
                    valor = string.Empty;
            }
            catch (Exception ex)
            {
                this.errorLog += "\n" + ex.Message;
                this.errorStop = true;
                return null;
            }
            return ASCIIEncoding.UTF8.GetBytes(valor);
        }

		/// <summary>
		/// Llena el buffer pasado con la cantidad de bytes solicitados
		/// </summary>
		/// <param name="buffer">Buffer utilizado para almacenar datos descompresos</param>
		/// <param name="stream">Stream de datos compresos</param>
		/// <param name="bytestoread">Cantidad de bytes que deben ser leidos del stream compreso </param>
		/// <returns>Cantidad de bytes leidos</returns>
		private int ReadBytes(byte[] buffer, ICSharpCode.SharpZipLib.Zip.Compression.Streams.InflaterInputStream stream,int bytestoread)
		{
			int offset = 0;
			int bytesLeidos = 0;

			while (offset < bytestoread)
			{
				bytesLeidos = stream.Read(buffer,offset,bytestoread-offset);

				if (bytesLeidos == 0) //Significa que no hay mas datos disponibles
					break;

				offset = offset + bytesLeidos;
			}

			return offset;			
		}

		/// <summary>
		/// Obtiene un entero almacenado en big endian en un vector de 2 bytes
		/// </summary>
		/// <param name="bytes">Vector de 2 bytes</param>
		/// <returns>El enterno decodificado</returns>
		private int GetInteger(byte[] bytes)
		{
			if (bytes.Length < 2)
				return 0;
			else
				//Convierte el entero encodificado
				return (((int)bytes[0])*256) + (int)bytes[1];
		}

		/// <summary>
		/// Crea la estructura de la base de datos
		/// </summary>
		/// <param name="table">Nombre de la base de datos</param>
		private bool CreateDatabaseStructure(string Database)
		{
			//Cuando hay que detenerse no se puede ejecutar nada mas.
			if (errorStop) return false;
			
			try
			{

				//Si no esxiste la base de datos la crea
                if (!File.Exists(currentConnection.DatabasePath))
				{	
					//Invoca el motor de base de datos y crea la base de datos 
					//con el nombre especificado
                    SQLiteEngine.CreateDatabase(currentConnection.DatabasePath);
				}	
			}
			catch (SQLiteException e)
			{
				this.errorLog += "\n" + e.Message;
				this.errorStop = true;
				return false;
			}
			catch (Exception e)
			{
				this.errorLog += "\n" + e.Message;
				this.errorStop = true;
				return false;
			}

			return true;
			
		}

		/// <summary>
		/// Crea la estructura de la tabla en la base de datos
		/// </summary>
		/// <param name="table">Tabla con informacion de su estructura</param>
		private void CreateTableStructure(EMMService.SQLCETable table,bool dropfirst)
		{
			//Cuando hay que detenerse no se puede ejecutar nada mas.
			if (errorStop) return;
			
			//Indica si la tabla es indexada o no
			bool indexed=false;
			//Indica la cantidad de campos indexados
			int indexedfields=0;

			//Crea una conexion a la base de datos
			SQLiteConnection connection;			

			//Destruye la informacion anterior de la tabla
			try 
			{	
				if (dropfirst)
				{
					connection = openDatabase(table.database);
					SQLiteCommand cmd = connection.CreateCommand();
					cmd.CommandText = "DROP TABLE " + table.name;
					cmd.ExecuteNonQuery();
				}
			}
			catch
			{//Ignora el error en caso de que no pueda destruirla y cierra la conexion a la base de datos
			}

			try 
			{
				//Crea una nueva conexion a la base de datos
				connection = openDatabase(table.database);

				//Crea un nuevo comando para la insercion de registros
				SQLiteCommand cmd = connection.CreateCommand();

				//Crea la parte inicial del comando de creacion de la tabla
				cmd.CommandText = "CREATE TABLE " + table.name + " (";

				//Construye la sentencia de creacion de la tabla
				for(int index=0;index<table.fields.Length;index++)
				{
					string nombreTipo = GetSQLCETypeName((EMMService.SQLCEType)table.fields[index].type);

					switch(table.fields[index].type)
					{
						case EMMService.SQLCEType.db_nchar:
						case EMMService.SQLCEType.db_nvarchar:
                        case EMMService.SQLCEType.db_binary:
                        case EMMService.SQLCEType.db_varbinary:
							cmd.CommandText = cmd.CommandText  + table.fields[index].name + " " + nombreTipo + "(" + table.fields[index].size +")";
							break;
						case EMMService.SQLCEType.db_numeric:
							//JDDR - Por mientras le ponemos 8 en SCALE - 19-02-2007
							cmd.CommandText = cmd.CommandText  + table.fields[index].name + " " + nombreTipo + "(" + table.fields[index].precision + ",8)";
							break;
						default:
							cmd.CommandText = cmd.CommandText  + table.fields[index].name + " " + nombreTipo;
							break;
					}

					//Si el campo es llave primaria lo declara NOT NULL
					if (table.fields[index].primarykey)
					{
						indexed=true;
						cmd.CommandText = cmd.CommandText  + " NOT NULL ";
						indexedfields++;

						//Si el campo no permite nulos lo declara NOT NULL
					}
					else if (!table.fields[index].allownull)
						cmd.CommandText = cmd.CommandText  + " NOT NULL ";

					//Verificamos si hay un default para el campo
					string valordefault = table.fields[index].defaultvalue;
					if (valordefault != null && valordefault != "")
					{
						cmd.CommandText = cmd.CommandText  + " DEFAULT '" + valordefault + "'";
					}

					//Si no se ha llegado al final de los campos se adjunta una ','
					if (index<table.fields.Length-1)
						cmd.CommandText = cmd.CommandText  +",";
				}


				//Si la tabla tiene el campo created lo adjunta a la sentencia
				if (table.createdField)
					cmd.CommandText = cmd.CommandText + " ,CREATED BIT DEFAULT 1 ";

				//Si la tabla tiene el campo updated lo adjunta a la sentencia
				if (table.updatedField)
					cmd.CommandText = cmd.CommandText + " ,UPDATED BIT DEFAULT 0 ";

				//Termina la sentencia sql
				cmd.CommandText = cmd.CommandText  +");";

				//Ejecuta la sentencia de creacion
				cmd.ExecuteNonQuery();

				//En caso de que tenga indices la tabla
				if (indexed)
				{

					//Verifica la manera en la cual se debe crear el indice de la tabla
					if (table.indextype == EMMService.SQLCEIndexType.SplitIndex)
					{
						// El indice de la tabla es separado, cada campo maneja su propio indice							
						
						//Busca todos los campos indexados
						for(int index=0;index<table.fields.Length;index++)						
							//Si el campo es indexado
							if (table.fields[index].primarykey)
							{
								if (table.indexUnique)
									//Crea el comando de creacion de indice
									cmd.CommandText = "CREATE UNIQUE INDEX  idx_" + table.fields[index].name + " ON " +  table.name + "(\"" + table.fields[index].name + "\")";
								else
									//Crea el comando de creacion de indice
									cmd.CommandText = "CREATE INDEX  idx_" + table.fields[index].name + " ON " +  table.name + "(\"" + table.fields[index].name + "\")";


								//Altera la tabla y crea el indice
								cmd.ExecuteNonQuery();
							}
					}
					else
					{							
						//El indice de la tabla es unido (se juntan todos los campos llave primaria)

						if (table.indexUnique)
							//Crea la parte inicial del la sentencia de contruccion del indice
							cmd.CommandText = "CREATE UNIQUE INDEX  idx_" + table.name + " ON " +  table.name + "(";
						else
							//Crea la parte inicial del la sentencia de contruccion del indice
							cmd.CommandText = "CREATE INDEX  idx_" + table.name + " ON " +  table.name + "(";

						//Busca todos los campos indexados
						for(int index=0;index<table.fields.Length;index++)						
							//Si el campo es indexado
							if (table.fields[index].primarykey)
							{
								//Crea el comando de creacion de indice
								cmd.CommandText = cmd.CommandText +  "\"" + table.fields[index].name + "\"";
								//Reduce la cantidad de indices disponibles
								indexedfields--;
								//Si no es el ultimo campo adjunta una coma
								if (indexedfields!=0)
									cmd.CommandText =  cmd.CommandText  + "," ;
							}

						//Termina la sentencia
						cmd.CommandText = cmd.CommandText + ")";
						//Altera la tabla y crea el indice
						cmd.ExecuteNonQuery();

					}
				}

			}
			catch(Exception e) 
			{//En caso de ocurrir un error
				if (dropfirst)
				{
					this.serverError = false;
					this.errorLog += "\n" + e.Message;
					this.errorStop = true;
				}
			}	
		}

		/// <summary>
		/// Obtiene un string con un nombre de tipo valido en SQLCE a partir de un tipo enumerado
		/// </summary>
		/// <param name="Type">Tipo de la enumeracion SQLCEType</param>
		/// <returns>El nombre valido del tipo</returns>
		private string GetSQLCETypeName(EMMService.SQLCEType Type)
		{

			switch (Type)
			{
				case EMMService.SQLCEType.db_int:
					return "int";				
				case EMMService.SQLCEType.db_bigint:
					return "bigint";				
				case EMMService.SQLCEType.db_numeric:
					return "numeric";				
				case EMMService.SQLCEType.db_smallint:
					return "smallint";				
				case EMMService.SQLCEType.db_tinyint:
					return "tinyint";				
				case EMMService.SQLCEType.db_bit:
					return "bit";				
				case EMMService.SQLCEType.db_real:
					return "real";				
				case EMMService.SQLCEType.db_float:
					return "float";				
				case EMMService.SQLCEType.db_money:
					return "money";				
				case EMMService.SQLCEType.db_datetime:
					return "datetime";				
				case EMMService.SQLCEType.db_ntext:
					return "ntext";				
				case EMMService.SQLCEType.db_nchar:
					return "nchar";				
				case EMMService.SQLCEType.db_nvarchar:
					return "nvarchar";				
				case EMMService.SQLCEType.db_binary:
					return "binary";				
				case EMMService.SQLCEType.db_varbinary:
					return "varbinary";				
				case EMMService.SQLCEType.db_image:
					return "image";				
				case EMMService.SQLCEType.db_uniqueidentifier :
					return "uniqueidentifier";		
					//Caso especial para los campos de tipo identity
				case EMMService.SQLCEType.db_identity:
					return "int identity(1,1)";		
			}

			return "ntext";
		}

		/// <summary>
		/// Obtiene un string con un nombre de tipo valido en SQLCE a partir de un tipo enumerado
		/// </summary>
		/// <param name="Type">Tipo de la enumeracion SQLCEType</param>
		/// <returns>El nombre valido del tipo</returns>
		private SqlDbType GetSQLDBType(EMMService.SQLCEType Type)
		{

			switch (Type)
			{
				case EMMService.SQLCEType.db_int:
					return SqlDbType.Int;				
				case EMMService.SQLCEType.db_bigint:
					return SqlDbType.BigInt;				
				case EMMService.SQLCEType.db_numeric:
					return SqlDbType.Decimal;				
				case EMMService.SQLCEType.db_smallint:
					return SqlDbType.SmallInt;				
				case EMMService.SQLCEType.db_tinyint:
					return SqlDbType.TinyInt;				
				case EMMService.SQLCEType.db_bit:
					return SqlDbType.Bit;				
				case EMMService.SQLCEType.db_real:
					return SqlDbType.Real;				
				case EMMService.SQLCEType.db_float:
					return SqlDbType.Float;				
				case EMMService.SQLCEType.db_money:
					return SqlDbType.Money;				
				case EMMService.SQLCEType.db_datetime:
					return SqlDbType.DateTime;				
				case EMMService.SQLCEType.db_ntext:
					return SqlDbType.NText;				
				case EMMService.SQLCEType.db_nchar:
					return SqlDbType.NChar;				
				case EMMService.SQLCEType.db_nvarchar:
					return SqlDbType.NVarChar;				
				case EMMService.SQLCEType.db_binary :
					return SqlDbType.Binary;				
				case EMMService.SQLCEType.db_varbinary:
					return SqlDbType.VarBinary;				
				case EMMService.SQLCEType.db_image:
					return SqlDbType.Image;				
				case EMMService.SQLCEType.db_uniqueidentifier:
					return SqlDbType.UniqueIdentifier;		
			}

			return SqlDbType.NText;
		}

		/// <summary>
		/// Obtiene un string con un nombre de tipo valido en SQLCE a partir de un tipo enumerado
		/// </summary>
		/// <param name="Type">Tipo de la enumeracion SQLCEType</param>
		/// <returns>El nombre valido del tipo</returns>
		private EMMService.SQLCEType GetSQLCEType(SqlDbType Type)
		{
			switch (Type)
			{
				case SqlDbType.Int:
					return EMMService.SQLCEType.db_int;
				case SqlDbType.BigInt:
					return EMMService.SQLCEType.db_bigint;
				case SqlDbType.Decimal:
					return EMMService.SQLCEType.db_numeric;
				case SqlDbType.SmallInt:
					return EMMService.SQLCEType.db_smallint;
				case SqlDbType.TinyInt:
					return EMMService.SQLCEType.db_tinyint;
				case SqlDbType.Bit:
					return EMMService.SQLCEType.db_bit;
				case SqlDbType.Real:
					return EMMService.SQLCEType.db_real;
				case SqlDbType.Float:
					return EMMService.SQLCEType.db_float;
				case SqlDbType.Money:
					return EMMService.SQLCEType.db_money;
				case SqlDbType.DateTime:
					return EMMService.SQLCEType.db_datetime;
				case SqlDbType.NText:
					return EMMService.SQLCEType.db_ntext;
				case SqlDbType.NChar:
					return EMMService.SQLCEType.db_nchar;
				case SqlDbType.NVarChar:
					return EMMService.SQLCEType.db_nvarchar;
				case SqlDbType.Binary:
					return EMMService.SQLCEType.db_binary;
				case SqlDbType.VarBinary:
					return EMMService.SQLCEType.db_varbinary;
				case SqlDbType.Image:
					return EMMService.SQLCEType.db_image;
				case SqlDbType.UniqueIdentifier:
					return EMMService.SQLCEType.db_uniqueidentifier;
			}

			return EMMService.SQLCEType.db_ntext;
		}

        //public EMMService.SQLCEType GetSQLCEType(string ColumnType)
        //{
        //    ColumnType = ColumnType.ToLower();

        //    if (ColumnType == "int")
        //        return EMMService.SQLCEType.db_int;

        //    if (ColumnType == "bigint")
        //        return EMMService.SQLCEType.db_bigint;

        //    if (ColumnType == "numeric")
        //        return EMMService.SQLCEType.db_numeric;

        //    if (ColumnType == "smallint")
        //        return EMMService.SQLCEType.db_smallint;

        //    if (ColumnType == "tinyint")
        //        return EMMService.SQLCEType.db_tinyint;

        //    if (ColumnType == "bit")
        //        return EMMService.SQLCEType.db_bit;

        //    if (ColumnType == "real")
        //        return EMMService.SQLCEType.db_real;

        //    if (ColumnType == "float")
        //        return EMMService.SQLCEType.db_float;

        //    if (ColumnType == "money")
        //        return EMMService.SQLCEType.db_money;

        //    if (ColumnType == "datetime")
        //        return EMMService.SQLCEType.db_datetime;

        //    if (ColumnType == "ntext")
        //        return EMMService.SQLCEType.db_ntext;

        //    if (ColumnType == "nchar")
        //        return EMMService.SQLCEType.db_nchar;

        //    if (ColumnType == "nvarchar")
        //        return EMMService.SQLCEType.db_nvarchar;

        //    if (ColumnType == "binary")
        //        return EMMService.SQLCEType.db_binary;

        //    if (ColumnType == "varbinary")
        //        return EMMService.SQLCEType.db_varbinary;

        //    if (ColumnType == "image")
        //        return EMMService.SQLCEType.db_image;

        //    if (ColumnType == "uniqueidentifier")
        //        return EMMService.SQLCEType.db_uniqueidentifier;

        //    return  EMMService.SQLCEType.db_nvarchar;
        //}

		/// <summary>
		/// Verifica la existencia de una tabla
		/// </summary>
		/// <param name="tablename"></param>
		/// <param name="database"></param>
		/// <returns></returns>
		public bool TableExists(string database,string tablename)
		{
			//Cuando hay que detenerse no se puede ejecutar nada mas.
			if (errorStop) return false;
			
			//Conexion a base de datos
			SQLiteConnection connection;			
			SQLiteDataAdapter dataadapter = new SQLiteDataAdapter();
			SQLiteCommand command;		
			SQLiteDataReader reader=null;
			int result;

            try
            {

                //Abre la conexion a base de datos
                connection = openDatabase(database);

                command = connection.CreateCommand();

                command.CommandText = "SELECT count(*) FROM MSysObjects WHERE NAME = '" + tablename + "'";
                reader = command.ExecuteDataReader();
                reader.Read();

                result = (int)reader.GetInt32(0);

                command.Dispose();
                reader.Close();

                return (result == 1);

            }
            catch (SQLiteException)
            {
                return false;
            }
            catch (Exception e)
            {
                errorLog += "\n" + e.Message;
                this.errorStop = true;
                return false;
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                    reader = null;
                }
            }
		}

		/// <summary>
		/// Sincroniza una tabla con el servidor
		/// </summary>
		/// <param name="action"></param>
		private void synchronizeTable(EMMService.EMMAction action)
		{
			//Cuando hay que detenerse no se puede ejecutar nada mas.
			if (errorStop) return;

			EMMService.EMMToken token =  new EMMService.EMMToken();
			token.type = EMMService.EMMTokenType.response;

			currentMessage = "Sincronizando datos en servidor ...";

			if (TableExists(action.statement.database,action.company + "_" + action.statement.table))			
			{
				token.table = DeflateTable(action.statement.database,action.company + "_" + action.statement.table,
					"SELECT * FROM " + action.company + "_" +action.statement.table + " WHERE UPDATED = 1 OR CREATED = 1");

				if (token.table == null)
					return; //Ocurrio un error al cargar la tabla				
			}
			else			
			{
				//Con esto le indicamos al servidor que nos envie todos los registros de la tabla
				token.table = null;
			}

			try
			{
				token = emmServer.ExecuteAction(this.clientInfo.company,this.clientInfo.deviceName,this.clientInfo.conduit,token);
			}
			catch(Exception e)
			{
				this.serverError = true;
				errorLog += "\n" + e.Message;
				this.errorStop = true;
				return;
			}

			if (verificarToken(token))
			{
				currentMessage = "Sincronizando datos en PDA ...";

				CreateTableStructure(token.table,false);	

				if (token.table.data.Length>0)									
					InflateUpdateTable(token.table);

				executeStatement(action.statement.database,"UPDATE " + action.company + "_" +action.statement.table + " SET UPDATED = 0 ,CREATED = 0 WHERE UPDATED = 1 OR CREATED = 1");
			}
			
		}

		/// <summary>
		/// Escribe una tabla en el servidor
		/// </summary>
		/// <param name="statusForm"></param>
		private void sendTable(EMMService.EMMAction action)
		{
			//Cuando hay que detenerse no se puede ejecutar nada mas.
			if (errorStop) return;
			
			EMMService.EMMToken token = new EMMService.EMMToken();
			token.type = EMMService.EMMTokenType.response;

			currentMessage = "Preparando datos ...";
			
			token.table = DeflateTable(action.statement);

			if (token.table == null)
			{
				return; //Ocurrio un error, volvemos
			}
			
			currentMessage = "Enviando datos ...";
			
			try
			{
				token = emmServer.ExecuteAction(this.clientInfo.company,this.clientInfo.deviceName,this.clientInfo.conduit,token);
			}
			catch(Exception e)
			{
				this.serverError = true;
				this.errorLog += "\n" + e.Message;
				this.errorStop = true;
			}

			verificarToken(token);
		}

		/// <summary>
		/// Comprime el resultado de una sentencia SQL ejecutada en el cliente (DataSet) y
		/// lo retorna como un arreglo de bytes compresos
		/// </summary>
		/// <param name="query">Sentencia utilizada para obtener los registros a comprimir</param>
		/// <returns>Dataset compreso</returns>
		private EMMService.SQLCETable DeflateTable(EMMService.SQLCEStatement sqlStatement)
		{
			//Cuando hay que detenerse no se puede ejecutar nada mas.
			if (errorStop) return null;
			
			return DeflateTable(sqlStatement.database,sqlStatement.table,sqlStatement.statement);
		}
		/// <summary>
		/// Comprime el resultado de una sentencia SQL ejecutada en el cliente (DataSet) y
		/// lo retorna como un arreglo de bytes compresos
		/// </summary>
		/// <param name="query">Sentencia utilizada para obtener los registros a comprimir</param>
		/// <returns>Dataset compreso</returns>
		private  EMMService.SQLCETable DeflateTable(string database,string tablename,string query)
		{
			//Cuando hay que detenerse no se puede ejecutar nada mas.
			if (errorStop) return null;

			//Conexion a base de datos
			SQLiteConnection connection;			
			SQLiteCommand command;	
			SQLiteDataReader reader=null;
			EMMService.SQLCETable table = new EMMService.SQLCETable();

			//Objetos para compresion de datos
			ICSharpCode.SharpZipLib.Zip.Compression.Streams.DeflaterOutputStream zipper;
			MemoryStream zippeddata = new MemoryStream();
			MemoryStream unzippeddata = new MemoryStream();			
			byte[] fieldBuffer;

            try
            {

                //Abre la conexion a base de datos
                connection = openDatabase(database);

                //Ejecuta la sentencia SQL y llena el dataset
                command = connection.CreateCommand();
                command.CommandText = query;

                reader = command.ExecuteDataReader();

                table.fields = new EMMService.SQLCEField[reader.FieldCount];

                table.name = tablename;

                for (int index = 0; index < reader.FieldCount; index++)
                {
                    table.fields[index] = new EMMService.SQLCEField();
                    table.fields[index].name = reader.GetName(index);
                    table.fields[index].type = SqlHelper.GetSQLCEType(((SqlDbType)reader.GetDataTypeName(index)));
                }

                while (reader.Read())
                    for (int index = 0; index < reader.FieldCount; index++)
                    {
                        fieldBuffer = ObtenerValorReader(reader, index);

                        if (fieldBuffer.Length != 0)
                        {
                            unzippeddata.Write(GetBytes(fieldBuffer.Length), 0, 2);
                            unzippeddata.Write(fieldBuffer, 0, fieldBuffer.Length);
                        }
                        else
                        {
                            unzippeddata.Write(GetBytes(0x01), 0, 2);
                            unzippeddata.Write(new byte[] { 0x00 }, 0, 1);
                        }
                    }

                unzippeddata.Flush();

                //Comprime el dataset serializado
                zipper = new ICSharpCode.SharpZipLib.Zip.Compression.Streams.DeflaterOutputStream(zippeddata);
                zipper.Write(unzippeddata.GetBuffer(), 0, unzippeddata.GetBuffer().Length);
                zipper.Close();

                table.data = zippeddata.GetBuffer();

                return table;

            }
            catch (SQLiteException e)//Si ocurrio una excepcion de SQLCE
            {
                if (reader != null)
                {
                    reader.Close();
                    reader = null;
                }
                this.errorLog += "\n" + e.Message;
                this.errorStop = true;
                return null;
            }
            catch (Exception e)//Si ocurrio una excepcion no contemplada
            {
                if (reader != null)
                {
                    reader.Close();
                    reader = null;
                }
                this.errorLog += "\n" + e.Message;
                this.errorStop = true;
                return null;
            }
            finally 
            {
                if (reader != null)
                {
                    reader.Close();
                    reader = null;
                }
                
            }
		}

		
		/// <summary>
		/// 
		/// </summary>
		private byte[] GetBytes(int Number)
		{
			byte fbyte;
			byte lbyte;
			fbyte =(byte)( Number / 256 );
			lbyte =(byte)( Number % 256 );

			return new byte[]{fbyte,lbyte};
		}

		/// <summary>
		/// Abre la base de datos SQLCE indicada. Si la base de datos ya se encuentra abierta retorna 
		/// la conexion ya abierta. Tambien carga la informacion correspondiente a las tablas de sincronizacion.
		/// </summary>
		/// <param name="Database">Nombre de la base de datos</param>
		/// <returns>Conexion a la base de datos.</returns>
		SQLiteConnection openDatabase(string Database)
		{

			string connectionString = "Data Source = \\"+Database+".sdf; Password =;";

			//Verifica si existe una base de datos cargada en memoria
			if (this.currentDatabase != Database)
			{

				//Vefica si existe una conexion a base de datos en memoria
				if (this.currentConnection!=null)
				{					
					//Verifica si la conexion esta abierta
					if (this.currentConnection.isOpen)
					{
                        if (!this.Desatendido)
                        {
                            //Cierra la conexion a base de datos	
                            this.currentConnection.Close();
                        }
					}
				}

				//Verifica la existencia de la base de datos, sino la crea
				if (CreateDatabaseStructure(Database))
				{
					//Almacena la informacion de la base de datos
					this.currentDatabase = Database;
					//Abre y almacena la informacion de la conexion a la base de datos
                    //this.currentConnection = new SqlCeConnection(connectionString);
                    if (!this.Desatendido)
                    {
                        this.currentConnection.Open();
                    }
				}

			}			
			
			//Retorna la conexion a la base de datos
			return this.currentConnection;

		}

		/// <summary>
		/// Cierra la base de datos que se encuentra actualmente abierta y ejecuta los procesos relacionados con su cierre
		/// </summary>
		void closeDatabase()
		{	
			if (currentConnection!=null)
			{

				//Cierra la conexion con la base de datos
				this.currentConnection.Close();
				this.currentConnection.Dispose();

                if (this.Compactar)
                {
                    CompactDatabase(this.currentDatabase);
                }

				//Borra la informacion de las instancias de la base de datos
				this.currentDatabase = "";
				this.currentConnection = null;
			}

		}

        /// <summary>
        /// Compacta la base de datos
        /// </summary>
        /// <param name="table">Nombre de la base de datos</param>
        private void CompactDatabase(string Database)
        {
            string dbSource = "\\" + Database + ".sdf";
            string dbTarget = "\\" + Database + "_TMP.sdf";

            try
            {
                if (File.Exists(dbSource))
                {
                    currentMessage = "Compactando ...";

                    //Invoca el motor de base de datos y crea la base de datos 
                    //con el nombre especificado
                    SQLiteEngine.Compact(currentConnection);

                    //File.Delete(dbSource);
                    //File.Move(dbTarget, dbSource);
                }
            }
            catch (SQLiteException e)
            {
                //Debemos ignorar el error de que no se pudo compactar la base de datos
                if (e.NativeError != 28550)//El error 28550 es cuando no se pudo compactar
                {
                    this.errorLog += "\n" + e.Message;
                    this.errorStop = true;
                }

                //Borramos el archivo temporal para que no quite espacio en la pocket
                //File.Delete(dbTarget);
            }
            catch (Exception e)
            {
                this.errorLog += "\n" + e.Message;
                this.errorStop = true;

                //Borramos el archivo temporal para que no quite espacio en la pocket
                //File.Delete(dbTarget);
            }
        }


		/// <summary>
		/// Crea una tabla en el cliente
		/// </summary>
		/// <param name="statusForm"></param>
		private void createTable(EMMService.EMMAction action)
		{
			//Cuando hay que detenerse no se puede ejecutar nada mas.
			if (errorStop) return;

			
			EMMService.EMMToken token = null;

			currentMessage = "Recibiendo datos ...";

			try
			{
				token = emmServer.ExecuteAction(this.clientInfo.company,this.clientInfo.deviceName,this.clientInfo.conduit,null);
			}
			catch(Exception e)
			{
				this.errorLog += "\n" + e.Message;
				this.serverError = true;
				this.errorStop = true;
			}

			if (!errorStop && verificarToken(token))
			{
				try
				{
					currentMessage = "Procesando ...";

					//Crea la estructura de la tabla
					CreateTableStructure(token.table,true);
				}
				catch(Exception e)
				{
					this.errorLog += "\n" + e.Message;
					this.errorStop = true;
				}
			}
		}

		/// <summary>
		/// Ejecuta una sentencia SQL en el cliente
		/// </summary>
		/// <param name="sqlquery"></param>
		//private void ExecuteQuery(emmServer.QueryClass sqlquery )
		private void executeStatement(EMMService.SQLCEStatement sqlStatement)
		{
			//Cuando hay que detenerse no se puede ejecutar nada mas.
			if (errorStop) return;

			currentMessage = "Ejecutando sentencia ...";
			executeStatement(sqlStatement.database,sqlStatement.statement);
		}

		/// <summary>
		/// Ejecuta una sentencia SQL en el cliente
		/// </summary>
		/// <param name="sqlquery"></param>
		private void executeStatement(string database ,string sqlquery )
		{
			//Cuando hay que detenerse no se puede ejecutar nada mas.
			if (errorStop) return;

			//Crea una nueva conexion con la base de datos			
			SQLiteConnection connection;			

			try 
			{				
				//Abre la conexion a la base de datos
				connection = openDatabase(database);
				//Ejecuta la sentencia SQL
				SQLiteCommand cmd = connection.CreateCommand();
				cmd.CommandText = sqlquery;
				cmd.ExecuteNonQuery();
			}
			catch(SQLiteException e)
			{
				this.errorLog += "\n" + e.Message;
				this.errorStop = true;
			}
			catch(Exception e)
			{
				this.errorLog += "\n" + e.Message;
				this.errorStop = true;
			}
		}

		private void InicializarBitacora()
		{
			this.iBitacora = new Bitacora();

			this.iBitacora.Inicializar();
		}

		private void EscribirBitacora(string mensaje)
		{
			if (this.usarBitacora)
			{
				this.iBitacora.EscribirLinea(mensaje);
			}
		}

		#endregion

        #region FRa

        #region Constantes
        private const string REPORTES = "Reportes";
        private const string RESPALDO = "OLD";
        private const string OTROS = "Otros";

        #endregion

        #region Obtener Lista
        /// <summary>
        /// Basado en el tipo de archivo y el ID de la HH, obtiene la lista de archivos por ACTUALIZAR/DESCARGAR
        /// </summary>
        /// <param name="tipo"></param>
        /// <returns></returns>
        public EMMFile[] GetFileList(string tipo)
        {
            EMMFile[] lista = null;
            try
            {
                //Obtiene la lista de archivos asociada a la HH del tipo dado.
                string[] listaText = emmServer.GetFileList(this.clientInfo.company, this.NombreDispositivo, tipo);

                if (listaText != null && listaText.Length > 0)
                {
                    lista = new EMMFile[listaText.Length];

                    //Convierte el texto, en instancias de la clase EMMFile
                    int i = 0;
                    foreach (string file in listaText)
                    {
                        lista[i++] = new EMMFile(file);
                    }
                }

            }
            catch (Exception ex)
            {
                string error = ex.Message;
                lista = null;
            }
            return lista;
        }
        #endregion

        #region Actualizar
        /// <summary>
        /// Ejecuta el Thread de actualización.
        /// </summary>
        /// <param name="lista">Lista de archivos por actualizar.</param>
        public void EjecutarActualizar(EMMFile[] lista)
        {
            this.estado = ConduitState.Executing;
            this.lista = lista;
            Thread t = new Thread(new ThreadStart(ActualizarThread));
            t.Start();
        }
        /// <summary>
        /// Ejecuta los pasos para realizar la actualización. 
        /// En caso de fallar deja los archivos en el estado anterior.
        /// </summary>
        private void ActualizarThread()
        {
            string path = this.FRPath;
            string respaldo = System.IO.Path.Combine(path, RESPALDO);
            this.ProcesoActualizar(path, respaldo);
        }

        #endregion

        #region Actualizar Estado

        /// <summary>
        /// Actualiza el estado del archivo dado.
        /// </summary>
        /// <param name="file"></param>
        /// <returns>Cantidad de registros actualizados.</returns>
        public int ActualizarEstado(EMMFile file)
        {
            int resultado = 0;
            try
            {
                resultado = emmServer.ActualizarEstado(this.clientInfo.company, this.NombreDispositivo
                    , file.Version, file.ID, file.Type);

            }
            catch (Exception ex)
            {
                string error = ex.Message;
                resultado = -1;
            }
            return resultado;
        }
        #endregion

        #region Verificar Actualización

        /// <summary>
        /// Verifica si el dispositivo tiene actualizaciones pedientes.
        /// </summary>
        /// <param name="file"></param>
        /// <returns>Cantidad de registros actualizados.</returns>
        public bool VerificarActualizacion()
        {
            bool resultado = false;
            try
            {
                resultado = emmServer.VerificarActualizacion(this.clientInfo.company, this.NombreDispositivo);
            }
            catch (Exception ex)
            {
                string error = ex.Message;
                resultado = false;
            }
            return resultado;
        }
        #endregion

        #region Util

        /// <summary>
        /// Guarda el flujo de bytes en lugar especificado.
        /// </summary>
        /// <param name="fs"></param>
        /// <param name="path"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private bool WriteBinarFile(byte[] fs, string path, string fileName)
        {
            bool resultado = false;
            this.currentMessage = "Escribiendo el archivo "+ fileName + " en " + path;
            try
            {
                Java.IO.File file = new Java.IO.File(path, fileName);
                if (file.Length() > 0)
                    file.Delete();
                file = new Java.IO.File(path, fileName);

                //convert array of bytes into file
                Java.IO.FileOutputStream fileOuputStream =
                          new Java.IO.FileOutputStream(file);
                fileOuputStream.Write(fs);
                fileOuputStream.Close();
                //
                //MemoryStream memoryStream = new MemoryStream(fs);
                //FileStream fileStream = new FileStream(path + fileName, FileMode.Create);
                //memoryStream.WriteTo(fileStream);
                //memoryStream.Close();
                //fileStream.Close();
                //fileStream = null;
                //memoryStream = null;
                resultado = true;
            }
            catch (Exception ex)
            {
                string error = ex.Message;
                resultado = false;
            }
            return resultado;
        }

        /// <summary>
        /// Descomprime un arreglo de bytes
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public byte[] Descomprimir(byte[] bytes)
        {
            byte[] resultado = null;
            MemoryStream ms;
            GZipInputStream gz;
            MemoryStream os;
            ms = new MemoryStream(bytes);
            gz = new GZipInputStream(ms);
            os = new MemoryStream(10000);
            int i = 0;
            while (gz.CanRead)
            {
                int b = gz.ReadByte();
                if (b == -1)
                {
                    break;
                }
                os.WriteByte((byte)b);
                i++;
            }
            gz.Close();
            ms.Close();
            os.Close();

            // Se asigna el Stream comprimido a la colección de Bytes
            resultado = os.ToArray();

            return resultado;
        }

        public void copyFile( Java.IO.File src, Java.IO.File dst)
        {
            //Java.IO.InputStream entrada = new Java.IO.FileInputStream(src);
            //Java.IO.OutputStream salida = new Java.IO.FileOutputStream(dst);

            //// Transfer bytes from in to out
            //byte[] buf = new byte[1024];
            //int len;
            //while ((len = entrada.Read(buf)) > 0) {
            //    salida.Write(buf, 0, len);
            //}
            //entrada.Close();
            //salida.Close();
            //
           Java.IO.FileInputStream fileInputStream=null; 
           byte[] bFile = new byte[(int) src.Length()];
           try
           {
               //convert file into array of bytes
               fileInputStream = new Java.IO.FileInputStream(src);
               fileInputStream.Read(bFile);
               fileInputStream.Close();

               //convert array of bytes into file
               Java.IO.FileOutputStream fileOuputStream =
               new Java.IO.FileOutputStream(dst);
               fileOuputStream.Write(bFile);
               fileOuputStream.Close();
           }
           catch (Exception e)
           {
               string error = e.Message;
               //e.printStackTrace();
           }
        }

        /// <summary>
        /// Respalda el archivo que se va a actualizar
        /// </summary>
        /// <param name="path"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private bool RespaldarArchivo(string pathFuente,string pathDestino, string fileName)
        {
            bool resultado = false;
            this.currentMessage = "Respaldando el archivo " + fileName;
            try
            {
                //
                Java.IO.File original = new Java.IO.File(pathFuente, fileName);
                Java.IO.File bakup = new Java.IO.File(pathDestino, fileName);
                if (original.Length() > 0)
                {
                    if (bakup.Length() > 0)
                        bakup.Delete();
                    bakup = new Java.IO.File(pathDestino, fileName);
                    this.copyFile(original, bakup);
                }                                        
                // Copia el archivo en la carpeta de respaldo.
                //File.Copy(pathFuente + fileName, pathDestino + fileName, true);
                resultado = true;
            }
            catch (FileNotFoundException fnfe)
            {
                string error = fnfe.Message;
                // Si el archivo no se encuentra, significa que es nuevo, por lo tanto no se debe respaldar.
                resultado = true;
            }
            catch (Exception e)
            {
                string error = e.Message;
                resultado = false;
            }
            return resultado;
        }

        /// <summary>
        /// Crea la carpeta donde se almacenaran los archivos por actualizar.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private bool CrearCarpeta(string path)
        {
            bool resultado = false;
            this.currentMessage = "Creando la carpeta de " + path;
            try
            {
                Java.IO.File Dir = new Java.IO.File(path);
                if (!Dir.Exists())
                    Dir.Mkdirs();
                resultado = true;
            }
            catch (Exception e)
            {
                string error = e.Message;
                resultado = false;
            }
            return resultado;
        }

        /// <summary>
        /// Elimina la carpeta de respaldo
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public bool EliminarCarpeta(string path)
        {
            bool resultado = false;

            try
            {
                Directory.Delete(path, true);
                resultado = true;
            }
            catch (Exception e)
            {
                string error = e.Message;
                resultado = false;
            }
            return resultado;

        }
        /// <summary>
        /// Reversa los cambios realizados por una actualización
        /// </summary>
        /// <param name="old">Ruta donde se encuentran los archivos de respaldo.</param>
        /// <param name="path">Ruta donde se copiaran los archivos.</param>
        private void Rollback(string old,string path,List<string> fileNames)
        {
            this.currentMessage = "Ocurrió un error, restaurando al estado anterior ...";
            try
            {
                //string[] files = Directory.GetFiles(path);
                //foreach (string file in files)
                //{
                //    File.Copy(System.IO.Path.Combine(old,fileName),System.IO.Path.Combine(path,fileName),true);
                //}
                foreach (string str in fileNames)
                {
                    File.Copy(System.IO.Path.Combine(old, str), System.IO.Path.Combine(path, str), true);
                }
                Directory.Delete(old, true);
            }
            catch (Exception e)
            {
                string error = e.Message;
            }

        }

        /// <summary>
        /// Obtiene el Path de FR, de acuerdo al Path de ejecución
        /// </summary>
        /// <returns></returns>
        private string ObtenerFRPath()
        {
            string fullAppName = Assembly.GetExecutingAssembly().GetName().CodeBase;

            DirectoryInfo info = new DirectoryInfo(fullAppName);

            string path = info.Parent.Parent.FullName;

            return path;
        }
        #endregion

        #region Common
        
        /// <summary>
        /// Verifica el estado del proceso principal.
        /// </summary>
        private void VerificarEstado()
        {
            if (this.errorStop) //Ocurrio un error
                this.estado = ConduitState.Aborted;
            else
                if (this.userCancel)  //El usuario cancelo la ejecucion del conducto
                    this.estado = ConduitState.Canceled;
                else
                    this.estado = ConduitState.Finished;
        }

        /// <summary>
        /// Inicializa la instancia del servicio web
        /// </summary>
        private void InicializarServicio(){
            emmServer = new EMMService.EMMWS();
            emmServer.Timeout = 600000;
            emmServer.Url = "http://" + this.serverName + "/EMMService/EMMWS.asmx";
            
        }
        
        /// <summary>
        /// Nombre identificador del dispositivo movil.
        /// </summary>
        public string NombreDispositivo;
        

        
        /// <summary>
        /// Ruta de la instalación de FRm
        /// </summary>
        private string FRPath
        {
            get
            {
                return this.frPath == null? this.ObtenerFRPath() : this.frPath;
            }
            set
            {
                this.frPath = value;
            }
        }

        /// <summary>
        /// Realiza el proceso de actualización/descargar.
        /// Respaldar archivo, descargar, eliminar archivos respaldo | Rollback.
        /// Ejecutar .exe en caso de ser actualización del core.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="respaldo"></param>
        /// <param name="tipo"></param>
        private void ProcesoActualizar(string path,string respaldo)
        {
            int progress = 0;
            bool resultado = false;
            List<string> ArchivosProcesados = new List<string>();
            try
            {
                if (this.lista == null)
                {
                    this.serverError = true;
                    throw new Exception("No fue posible obtener la lista de archivos");
                }

                this.CrearCarpeta(System.IO.Path.Combine(path, RESPALDO));
                //this.CrearCarpeta(System.IO.Path.Combine("/sdcard", OTROS));
                this.CrearCarpeta(System.IO.Path.Combine(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath, OTROS));
                this.CrearCarpeta(path);

                foreach (EMMFile file in this.lista)
                {
                    progress++;
                    ArchivosProcesados.Add(file.Name);
                    this.EscribirBitacora("Progreso de actualización: " + progress);

                    if (this.lista.Length > 0)
                        this.currentProgress = (progress * 100) / (this.lista.Length + 1);

                    //Respaldar el archivo que se va a obtener.
                    this.RespaldarArchivo(path, respaldo, file.Name);

                    // Obtiene el archivo del Web Service en un byte[] 
                    // y lo escribe en la carpeta de la HH.
                    this.currentMessage = "Bajando el archivo " + file.Name;
                    byte[] fs = emmServer.GetFile(this.clientInfo.company, file.Version, file.ID, file.Type, file.Name,file.VersionDetail);

                    string currentPath = path;
                    string nameTemp = string.Empty;

                    if (file.Type == "R")
                        currentPath = path;
                    if (file.Type == "O")
                        //currentPath = System.IO.Path.Combine("/sdcard", OTROS);
                        currentPath = System.IO.Path.Combine(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath, OTROS);
                    if(file.Type == "P")
                        currentPath = path;

                    

                    // Escribir el archivo fisicamente.
                    resultado = this.WriteBinarFile(this.Descomprimir(fs), currentPath, file.Name);

                    if(file.TypeDetail == "P" && Path.GetExtension(file.Name) == ".exe")
                        Launcher.Launch(currentPath + file.Name, "", true);

                    //No se pudo escrbir el archivo.
                    if (!resultado)
                    {
                        this.Rollback(respaldo, path, ArchivosProcesados);
                        break;
                    }
                    else
                    {
                        this.ActualizarEstado(file);
                    }
                }
                // Se elimina la carpeta de respaldo.
                this.EliminarCarpeta(respaldo);

            }
            catch (Exception e)
            {
                this.Rollback(respaldo, path, ArchivosProcesados);
                this.errorLog += "\n" + e.Message;
                this.errorStop = true;
            }

            this.VerificarEstado();
        }
        #endregion

        #endregion
    }
}