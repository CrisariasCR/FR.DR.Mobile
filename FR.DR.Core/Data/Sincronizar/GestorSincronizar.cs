using System;
using EMMClient;
using Softland.ERP.FR.Mobile.Cls.AccesoDatos;
using System.Data.SQLiteBase;
using Softland.ERP.FR.Mobile.Cls.Corporativo;
using Softland.ERP.FR.Mobile.Cls.Documentos.FRPedido;
using Softland.ERP.FR.Mobile.Cls.Documentos.FRDevolucion;
using Softland.ERP.FR.Mobile.Cls.Documentos.FRInventario;
using Softland.ERP.FR.Mobile.Cls.Documentos.FRConsignacion;
using Softland.ERP.FR.Mobile.Cls.Cobro;
using Softland.ERP.FR.Mobile.Cls.FRCliente.FRVisita;
using System.IO;
using Softland.ERP.FR.Mobile.Cls.Utilidad;
using Softland.ERP.FR.Mobile.Cls.FRArticulo;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Xml;
using FR.DR.Core.Data.Trasiego;

//

namespace Softland.ERP.FR.Mobile.Cls.Sincronizar
{
	/// <summary>
	/// Gestor que se encarga de todo lo que respecta a sincronizaciones.
	/// </summary>
	public static class GestorSincronizar
	{
		#region Constantes

		/// <summary>
		/// Mensaje que se debe mostrar cuando se realiza una carga de datos y
		/// no hay informacion asociada al codigo de dispositivo.
		/// </summary>
		public const string INFONOASOCIADA = 
			"No se cargó información asociada a este dispositivo. " + 
			"Verificar que las rutas asociadas estén activas y cargadas.";

		#endregion

        static public Softland.ERP.FR.Mobile.ViewModels.BaseViewModel viewModel;

        public static bool sincroDesatendida = false;

        public static bool repetir=true;

		#region Metodos de la clase

		public static bool ValidaCargaCorrecta(string handHeld)
		{
		    string sentencia = 
				@" SELECT COUNT(*) "+
                @" FROM " + Table.ERPADMIN_RUTA_CFG +
                @" WHERE HANDHELD = @DISPOSITIVO ";

            SQLiteParameterList parametros = new SQLiteParameterList( new SQLiteParameter[] {new SQLiteParameter("@DISPOSITIVO", handHeld)});

            return GestorDatos.RetornaDatos(sentencia,parametros);

		}

		/// <summary>
		/// Funcion que valida si hay registros no sincronizados
		/// </summary>
		/// <returns>un boleano indicando si ya se sincronizaron todos los datos o no</returns>
		public static bool ValidaSincronizacion()
		{
			try
			{
				//Pedidos y facturas no sincronizadas.
                if (Pedido.HayPendientesCarga())
                    return false;
                if (Garantia.HayPendientesCarga())
                    return false;
				//Devoluciones no sincronizadas.
                if (Devolucion.HayPendientesCarga())
                    return false;
				//Inventarios no sincronizados.
                if (Inventario.HayPendientesCarga())
                    return false;
				//Ventas en consignación no sincronizadas.
                if (VentaConsignacion.HayPendientesCarga())
                    return false;
				//Recibos no sincronizados.
                if (Recibo.HayPendientesCarga())
                    return false;
				//Visitas no sincronizadas.
                if (Visita.HayPendientesCarga())
                    return false;
                #region MejorasFRTostadoraElDorado600R6 JEV
                if (TomaFisicaInventario.HayPendientesCarga())
                {
                    return false;
                }
                #endregion MejorasFRTostadoraElDorado600R6 JEV

                if (Trasiego.HayPendientesCarga())
                {
                    return false;
                }

            }
			catch(Exception ex)
			{
				throw new Exception("Error validando la sincronización. " + ex.Message);
			}

			return true;
		}

        /// <summary>
        /// retorna un xml con la firma del dispositivo
        /// Nombre del dispositivo
        /// Tipo de Sistema Operativo
        /// /// VersionAndroid
        /// MACAddress de red
        /// Serial Number Procesador
        /// etc.
        /// </summary>
        /// <returns></returns>
        public static string ObtenerFirmaDispositivo(Android.Content.Context Contexto)
        {
/*
            Context app = Setup.messageHandler._applicationContext;

            var id = Android.Provider.Settings.Secure.GetString(app.ContentResolver, Android.Provider.Settings.Secure.AndroidId);
            var brand = Android.OS.Build.Brand;
            var cpu = Android.OS.Build.CpuAbi;
            var cpu2 = Android.OS.Build.CpuAbi2;
            var device = Android.OS.Build.Device;
            var finger = Android.OS.Build.Fingerprint;

            var hard = Android.OS.Build.Hardware;
            var host = Android.OS.Build.Host;
            var bid = Android.OS.Build.Id;
            var manufacturer = Android.OS.Build.Manufacturer;
            var model = Android.OS.Build.Model;
            var product = Android.OS.Build.Product;
            var radio = Android.OS.Build.Radio;
            var version = Android.OS.Build.VERSION.Release;
            //var version = Android.OS.Build.Seria;
            */
            //Toast.MakeText(app, "Error mostrando Mensajes!", ToastLength.Long).Show();

            // requires READ_PHONE_STATE in Properties
            //TelephonyManager tm = (TelephonyManager)GetSystemService(TelephonyService);
            //var deviceId = tm.DeviceId;
            //var serialNumber = tm.SimSerialNumber;

            // requires ACCESS_WIFI_STATE in Properties
            //WifiManager wf = (WifiManager)GetSystemService(WifiService);
            //var macAddress = wf.ConnectionInfo.MacAddress;
            //if (!wf.IsWifiEnabled) macAddress = "DISABLED";
            //if (macAddress == "") macAddress = "UNKNOWN";

            //Log.Debug("INFO", "{0} {1} {2} {3} {4} {5} {6} {7}", id, device, model, manufacturer, brand, deviceId, serialNumber, macAddress);
            // Simulator: 9774d56d682e549c generic sdk unknown generic 000000000000000 89014103211118510720 DISABLED

            // ojo  cambiar por un xml con la firma del dispositivo, que se va a comparar con la licencia!
            // TODO            
            //return "NP0-6CRL-RWI";
            //"NP0-6CRL-RWI"

            //Codigo WindowsMobile            

            string firmaHilera = string.Empty;
            //var macAddress = string.Empty;
            //// requires ACCESS_WIFI_STATE in Properties
            //Android.Net.Wifi.WifiManager wf = (Android.Net.Wifi.WifiManager)Contexto.GetSystemService("WifiService");
            //if (wf != null)
            //{
            //    macAddress = wf.ConnectionInfo.MacAddress;
            //    if (!wf.IsWifiEnabled) macAddress = "DISABLED";
            //    if (macAddress == "") macAddress = "UNKNOWN";
            //}
            //else
            //{
            //    macAddress = "UNKNOWN";
            //}

            //

            //firmaHilera += "Name "+Android.Provider.Settings.System.Name + " \n" +
            //    "MACAdress " + macAddress + " \n" + 
            //    "AndroidID " + Android.Provider.Settings.Secure.GetString(Contexto.ContentResolver, Android.Provider.Settings.Secure.AndroidId) + " \n" +
            //    "Device " + Android.OS.Build.Device + " \n" +
            //    "Hardware " + Android.OS.Build.Hardware + " \n" +
            //    "Host " + Android.OS.Build.Host + " \n" +
            //    "Id " + Android.OS.Build.Id + " \n" +
            //    "Manufacturer " + Android.OS.Build.Manufacturer + " \n" +
            //    "Model " + Android.OS.Build.Model + " \n" +
            //    "Product " + Android.OS.Build.Product + " \n" +
            //    "Codename " + Android.OS.Build.VERSION.Codename + " \n" +
            //    "Release " + Android.OS.Build.VERSION.Release + " \n";

            //firmaHilera += "NOTHING";

            #region Version XMLWritter

            
            XmlWriterSettings settings=new XmlWriterSettings();
            settings.OmitXmlDeclaration=true;
            using (var sw = new StringWriter())
            {
                using (var xw = XmlWriter.Create(sw, settings))
                {
                    xw.WriteStartDocument();
                    xw.WriteStartElement("FirmaDispositivoMovil");

                    xw.WriteStartElement("Nombre");
                    xw.WriteString(Android.Provider.Settings.Secure.GetString(Contexto.ContentResolver, Android.Provider.Settings.Secure.AndroidId));
                    xw.WriteEndElement();

                    //xw.WriteStartElement("MacAdress");
                    //xw.WriteString(macAddress);
                    //xw.WriteEndElement();

                    xw.WriteStartElement("ServidorWS");
                    xw.WriteString(GestorDatos.ServidorWS);
                    xw.WriteEndElement();

                    xw.WriteStartElement("Dominio");
                    xw.WriteString(GestorDatos.Dominio);
                    xw.WriteEndElement();

                    xw.WriteStartElement("Propietario");
                    xw.WriteString(Android.OS.Build.Manufacturer);
                    xw.WriteEndElement();

                    xw.WriteStartElement("Model");
                    xw.WriteString(Android.OS.Build.Model);
                    xw.WriteEndElement();

                    xw.WriteStartElement("NombreSO");
                    xw.WriteString(Android.OS.Build.Id);
                    xw.WriteEndElement();

                    xw.WriteStartElement("VersionSO");
                    xw.WriteString(Android.OS.Build.VERSION.Release);
                    xw.WriteEndElement();

                    xw.WriteStartElement("Plataforma");
                    xw.WriteString("Android");
                    xw.WriteEndElement();

                    xw.WriteEndDocument();
                    xw.Close();

                }
                firmaHilera = sw.ToString();
            }
            #endregion

            //throw new Exception("TODO: Hay que calcular la firma del dispositivo");

            return firmaHilera;
        }
		/// <summary>
		/// Realiza la carga de datos.
		/// Supone que se esta desconectado de la base de datos y no deja conectado luego de la sincronizacion.
		/// </summary>
		/// <returns></returns>
		public static string CargaDatos(string host,Android.Content.Context contexto)
		{
			string errorLog = string.Empty;

			EMMClientManager Syn = null;
			try
			{
                Syn = new EMMClientManager(ObtenerFirmaDispositivo(contexto), 
					GestorDatos.NombreUsuario,
					GestorDatos.ContrasenaUsuario,
					GestorDatos.ServidorWS,
					GestorDatos.Dominio,
					GestorDatos.Owner,
                    GestorDatos.cnx,
					FRmConfig.EMMCarga,
					false);
                
			    bool temp=GestorDatos.LicenciaValida;
				//Syn.UsarBitacora = true;
				Syn.Parameters.Add(host);
                Syn.Synchronize(host,ref temp);
                GestorDatos.LicenciaValida = temp;
				errorLog = Syn.ErrorLog;
			}
			catch(Exception e)
			{
				if (Syn != null)
					errorLog = Syn.ErrorLog + " " + e.Message;
				else
					errorLog = e.Message;
			}
			return errorLog;
		}

		/// <summary>
		/// Realiza el envio de datos. 
		/// Supone que se esta desconectado de la base de datos y no deja conectado luego de la sincronizacion.
		/// </summary>
		/// <param name="sentenciaConsecutivos">
		/// Sentencia SQL que se envía al servidor para que actualice los consecutivos de documento
		/// para cada una de las rutas asignadas al dispositivo móvil.
		/// </param>
		/// <param name="sentenciaExistencias">
		/// Sentencia SQL que se envía al servidor para que actualice las existencias de los artículos
		/// que fueron vendidos en documentos tipo factura.
		/// </param>
		/// <param name="sentenciaActualizacionBoletas">
		/// Sentencia SQL que se envía al servidor para que elimine las boletas de venta en consignación
		/// que deben ser procesadas en caso de que se haya realizado un desglose y se haya generado una nueva boleta.
		/// </param>
        public static string Sincronizar(string sentenciaConsecutivos, string sentenciaExistencias, string sentenciaActualizacionBoletas, string sentenciaActualizacionJornadas, string sentenciaConsecutivosNCF, string sensentenciaConsecutivosResolucion, Android.Content.Context contexto)
		{
			string errorLog = string.Empty;

			EMMClientManager Syn = null;
	
			try
			{
				Syn = new EMMClientManager(ObtenerFirmaDispositivo(contexto),
					GestorDatos.NombreUsuario,
					GestorDatos.ContrasenaUsuario,
					GestorDatos.ServidorWS,
					GestorDatos.Dominio,
					GestorDatos.Owner,
                    GestorDatos.cnx,
                    FRmConfig.EMMDescarga,
					false);

				//LDS 21/02/2008 - Venta en consignación.
				//Se agrega la sentencia que se ejecuta en el servidor para actualizar las boletas de venta en consignación.
				//Elimina las boletas por ruta para luego sincronizar los cambios realizados por el desglose de la boleta.
				Syn.Parameters.Add(sentenciaConsecutivos);
				Syn.Parameters.Add(sentenciaExistencias);
				Syn.Parameters.Add(sentenciaActualizacionBoletas);
                // LAS. Se incluye la sentencia de actualización de jornadas.
                Syn.Parameters.Add(sentenciaActualizacionJornadas);
                //ABC Manejo NCF
                Syn.Parameters.Add(sentenciaConsecutivosNCF);
                //Manejo Resoluciones
                Syn.Parameters.Add(sensentenciaConsecutivosResolucion);

                bool temp = GestorDatos.LicenciaValida;
                Syn.Synchronize(Ruta.NombreDispositivo(),ref temp);
                GestorDatos.LicenciaValida = temp;

				errorLog = Syn.ErrorLog;
			}
			catch(Exception ex)
			{
				if (Syn != null)
					errorLog = Syn.ErrorLog + " " + ex.Message;
				else
					errorLog = ex.Message;
			}

			return errorLog;
		}

        /// <summary>
        /// Realiza el envio de datos. 
        /// Supone que se esta desconectado de la base de datos y no deja conectado luego de la sincronizacion.
        /// </summary>
        /// <param name="sentenciaConsecutivos">
        /// Sentencia SQL que se envía al servidor para que actualice los consecutivos de documento
        /// para cada una de las rutas asignadas al dispositivo móvil.
        /// </param>
        /// <param name="sentenciaExistencias">
        /// Sentencia SQL que se envía al servidor para que actualice las existencias de los artículos
        /// que fueron vendidos en documentos tipo factura.
        /// </param>
        /// <param name="sentenciaActualizacionBoletas">
        /// Sentencia SQL que se envía al servidor para que elimine las boletas de venta en consignación
        /// que deben ser procesadas en caso de que se haya realizado un desglose y se haya generado una nueva boleta.
        /// </param>
        public static string SincronizarDesatendido(string sentenciaConsecutivos, string sentenciaExistencias, string sentenciaActualizacionBoletas, string sentenciaActualizacionJornadas, string sentenciaConsecutivosNCF,string sensentenciaConsecutivosResolucion, Android.Content.Context contexto)
        {
            string errorLog = string.Empty;

            EMMClientManager Syn = null;

            try
            {
                Syn = new EMMClientManager(ObtenerFirmaDispositivo(contexto),
                    GestorDatos.NombreUsuario,
                    GestorDatos.ContrasenaUsuario,
                    GestorDatos.ServidorWS,
                    GestorDatos.Dominio,
                    GestorDatos.Owner,
                    GestorDatos.cnx,
                    FRmConfig.EMMDescarga,
                    false,true);

                

                

                //LDS 21/02/2008 - Venta en consignación.
                //Se agrega la sentencia que se ejecuta en el servidor para actualizar las boletas de venta en consignación.
                //Elimina las boletas por ruta para luego sincronizar los cambios realizados por el desglose de la boleta.
                Syn.Parameters.Add(sentenciaConsecutivos);
                Syn.Parameters.Add(sentenciaExistencias);
                Syn.Parameters.Add(sentenciaActualizacionBoletas);
                // LAS. Se incluye la sentencia de actualización de jornadas.
                Syn.Parameters.Add(sentenciaActualizacionJornadas);
                //ABC Manejo NCF
                Syn.Parameters.Add(sentenciaConsecutivosNCF);
                //Manejo Resoluciones
                Syn.Parameters.Add(sensentenciaConsecutivosResolucion);

                bool temp = GestorDatos.LicenciaValida;
                Syn.Synchronize(Ruta.NombreDispositivo(), ref temp);
                GestorDatos.LicenciaValida = temp;

                errorLog = Syn.ErrorLog;
            }
            catch (Exception ex)
            {
                if (Syn != null)
                    errorLog = Syn.ErrorLog + " " + ex.Message;
                else
                    errorLog = ex.Message;
            }

            return errorLog;
        }

        private static void BorrarBD() 
        {
            string folder = string.Empty;
#if DEBUG
            {
                if (!FRmConfig.GuardarenSD)
                {
                    folder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
                }
                else
                {
                    //folder = "/sdcard";
                    folder = Softland.ERP.FR.Mobile.App.ObtenerRutaSD();
                }
            }
#else
                        {
                            if (!FRmConfig.GuardarenSD)
                            {
                                folder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
                            }
                            else
                            {
                                //folder = "/sdcard";
                                folder=Softland.ERP.FR.Mobile.App.ObtenerRutaSD();
                            }
                        }
#endif

            Java.IO.File file = new Java.IO.File(folder, GestorDatos.BaseDatos + ".db");
            if (file.Length() != 0)
            {
                bool res = file.Delete();           
            }
        }

        public static bool CrearBaseDatos(Android.Content.Context contexto,string NombreDispositivo,ref string errorMSJ)
        {

            //viewModel.mostrarAlerta("Falta probar la creacion de la base de datos.");
            string host = NombreDispositivo;
            GestorDatos.LicenciaValida = false;
            bool infoCargada = true;
            GestorDatos.Desconectar();
            string errorLog = GestorSincronizar.EjecutarCrearTablas(contexto,NombreDispositivo);
            if (!string.IsNullOrEmpty(errorLog))
            {
                //Reintentar 1 vez
                errorLog = GestorSincronizar.EjecutarCrearTablas(contexto, NombreDispositivo);
            }
            if (string.IsNullOrEmpty(errorLog))
            {
                errorLog = CargaDatos(host, contexto);
            }
            if (!string.IsNullOrEmpty(errorLog))
            {
                //Reintentar 1 vez
                errorLog = CargaDatos(host, contexto);
            }

            //Caso 34610, no considerar un error si la sincronizacion fue cancelada por el usuario
            if (errorLog.Equals("usercancel"))
            {
                errorMSJ="Carga cancelada por el usuario.";
                infoCargada = false;
            }
            else if (errorLog != string.Empty)
            {
                errorMSJ="Se produjo un error en la carga de datos. " + errorLog;
                infoCargada = false;
            }

            try
            {
                GestorDatos.Conectar();
            }
            catch (Exception)
            {
                if (infoCargada)
                    errorMSJ="No se pudo conectar a la base de datos.";
                infoCargada = false;
            }
            try
            {
                if (infoCargada)
                    infoCargada = GestorSincronizar.ValidaCargaCorrecta(host);
            }
            catch (Exception ex)
            {
                errorMSJ="Error validando si hay información asociada a este dispositivo. " + ex.Message;
                infoCargada = false;
            }

            if (!infoCargada)
            {
                //viewModel.mostrarAlerta(GestorSincronizar.INFONOASOCIADA);

                try
                {
                    errorMSJ = "No existen Rutas asociadas a este dispositivo. " + errorLog;
                    GestorDatos.Desconectar();
                    //File.Delete(GestorDatos.BaseDatos + ".db");
                    BorrarBD();
                }
                catch
                {

                }

                return false;
            }
            else
                return true;

        }        

        public static bool CargaDatos(Android.Content.Context contexto)
        {
            bool resultado = false;
            try
            {
                if (GestorSincronizar.ValidaSincronizacion())
                {
                    Bitacora.Escribir("Se van a cargar datos");
                    Bitacora.Escribir("Consecutivos actuales: " + ParametroSistema.GenerarSentenciaActualizacion());

                    string host = Ruta.NombreDispositivo();

                    GestorDatos.Desconectar();

                    string errorLog = GestorSincronizar.CargaDatos(host,contexto);
                    if (!string.IsNullOrEmpty(errorLog))
                    {
                        //reintentar 1 vez
                        errorLog = GestorSincronizar.CargaDatos(host, contexto);
                    }
                    //Caso 34610, no considerar un error si la sincronizacion fue cancelada por el usuario
                    if (errorLog.Equals("usercancel"))
                    {
                       viewModel.mostrarAlerta("Sincronización cancelada por el usuario.");
                        Bitacora.Escribir("Sincronización cancelada por el usuario.");
                        resultado = false;
                    }
                    else if (errorLog != string.Empty)
                    {
                        viewModel.mostrarAlerta("Se produjo un error en la carga de datos. " + errorLog);
                        Bitacora.Escribir("Se produjo un error en la carga de datos. " + errorLog);
                        resultado = false;
                    }
                    else
                    {
                        resultado = true;
                    }
                     
                    try
                    {
                        //Sin importar el resultado de la carga, intentamos volver a conectar a la base de datos
                        GestorDatos.Conectar();
                    }
                    catch (Exception)
                    {
                       viewModel.mostrarAlerta("No se pudo reconectar a la base de datos.");
                    }

                    if (resultado)
                    {
                        //Validamos que se haya cargado informacion asociada al dispositivo
                        if (!GestorSincronizar.ValidaCargaCorrecta(host))
                        {
                            viewModel.mostrarAlerta(GestorSincronizar.INFONOASOCIADA);
                        }
                        else
                        {
                            if (errorLog == string.Empty)
                            {
                                //Mostramos este mensaje solo cuando todo fue exitoso
                                viewModel.mostrarMensaje(Mensaje.Accion.Informacion, "La carga finalizó con éxito.");
                                Bitacora.Escribir("La carga finalizó con éxito.");
                            }
                            resultado = true;
                        }
                    }
                }
                else
                {
                    viewModel.mostrarAlerta("No se puede realizar la carga porque hay datos que no han sido sincronizados.");
                    resultado = false;
                }
            }
            catch (Exception es)
            {
                viewModel.mostrarAlerta(es.Message);
                resultado = false;
            }
            return resultado;
        }

        public static bool CargaParametros(bool logBitacora)
        {
            try
            {
                ParametroSistema.CargarParametros();
                if (logBitacora)
                {
                    Bitacora.Escribir("Se van a leer los consecutivos luego de carga de datos");
                    Bitacora.Escribir("Consecutivos luego de carga: " + ParametroSistema.GenerarSentenciaActualizacion());
                }
            }
            catch (System.Exception ex)
            {
                viewModel.mostrarAlerta("Error al cargar los parametros del Sistema. " + ex.Message);
                return false;
            }

            try
            {
                FRdConfig.CargarGlobales();
            }
            catch (Exception ex)
            {
                viewModel.mostrarAlerta("Error al cargar los parámetros globales. " + ex.Message);
                return false;
            }

            return true;
        }

        public static bool CargaParametrosX(bool logBitacora)
        {
            try
            {
                ParametroSistema.CargarParametros();
                if (logBitacora)
                {
                    Bitacora.Escribir("Se van a leer los consecutivos luego de carga de datos");
                    Bitacora.Escribir("Consecutivos luego de carga: " + ParametroSistema.GenerarSentenciaActualizacion());
                }
            }
            catch (System.Exception ex)
            {
                throw new Exception("Error al cargar los parametros del Sistema. " + ex.Message);
            }

            try
            {
                FRdConfig.CargarGlobales();
            }
            catch (Exception ex)
            {
                throw new Exception("Error al cargar los parámetros globales. " + ex.Message);
            }

            return true;
        }

        public static bool DescargaDatos(List<Ruta> rutasActualizacion,Android.Content.Context contexto)
        {
            GestorDatos.LicenciaValida = false;
            string sentencia1 = string.Empty;
            string sentencia2 = string.Empty;
            string sentencia3 = string.Empty;
            string sentencia4 = string.Empty;
            string sentencia5 = string.Empty;
            string sentencia6 = string.Empty;
            bool resultado = false;
            //LJR 04/03/2009 Caso: 34986,34987,34988
            //Las siguientes sentencias 4 y 5 se ejecutan en los conductos del EMM ahora 

            try
            {
                sentencia1 = ParametroSistema.GenerarSentenciaActualizacion();
            }
            catch (Exception ex)
            {
                if(!sincroDesatendida)
                    viewModel.mostrarAlerta("Error creando sentencia de actualización de consecutivos. " + ex.Message);
                return false;
            }

            try
            {
                if (FRdConfig.UsaFacturacion) sentencia2 = Articulo.SentenciaActualizacionServidor();
            }
            catch (Exception ex)
            {
                if (!sincroDesatendida)
                    viewModel.mostrarAlerta("Error creando sentencia de actualización de existencias. " + ex.Message);
                return false;
            }

            //LDS 21/02/2008 - Venta en consignación.
            //Se obtiene la sentencia que permite actualizar las boletas de venta en consignación.
            try
            {
                //LJR 04/03/2009 Caso: 34986,34987,34988
                //Las siguientes sentencias 4 y 5 se ejecutan en los conductos del EMM ahora  
                BoletasConsignacion.GenerarSentenciaActualizacion(ref sentencia3, rutasActualizacion);
            }
            catch (Exception ex)
            {
                if (!sincroDesatendida)
                    viewModel.mostrarAlerta("Error creando sentencia de sincronización de boletas. " + ex.Message);
                return false;
            }

            // LAS.  Crear las sentencias para sincronizar los datos de las jornadas.
            try
            {
                sentencia4 = JornadaRuta.ObtenerSentenciaActualizacionJornada();
            }
            catch (Exception ex)
            {
                if (!sincroDesatendida)
                    viewModel.mostrarAlerta(String.Format("Error creando sentencia de sincronización de jornadas. ", ex.Message));
                return false;
            }

            try
            {
                sentencia5 = NCFUtilitario.ObtenerSentenciaActualizacionNCF();
            }
            catch (Exception ex)
            {
                if (!sincroDesatendida)
                    viewModel.mostrarAlerta(String.Format("Error creando sentencia de sincronización de consecutivos NCF. ", ex.Message));
                return false;
            }
            try
            {
                sentencia6 = ResolucionUtilitario.ObtenerSentenciaActualizacionResolucion();
            }
            catch (Exception ex)
            {
                viewModel.mostrarAlerta(String.Format("Error creando sentencia de sincronización de consecutivos NCF. ", ex.Message));
                return false;
            }
            


            //Se agrega bitacora para ver la sentencia de actualizacion de consecutivos.
            //Se agrega bitacora para ver la sentencia de actualizacion de existencias.
            //Se agrega bitacora para ver la sentencia de actualizacion de boletas.
            Bitacora.Escribir("Se van a descargar datos");
            Bitacora.Escribir("Consecutivos actuales: " + sentencia1);
            Bitacora.Escribir("Actualización de existencias: " + sentencia2);
            Bitacora.Escribir("Actualización de boletas de venta en consignación: " + sentencia3);
            Bitacora.Escribir("Actualización de jornadas laborales " + sentencia4);
            Bitacora.Escribir("Consecutivos NCF actuales y a Actualizar" + sentencia5);

            GestorDatos.Desconectar();

            string errorLog = GestorSincronizar.Sincronizar(sentencia1, sentencia2, sentencia3, sentencia4, sentencia5,sentencia6,contexto);

            if (!string.IsNullOrEmpty(errorLog))
            {
                //reintentar 1 vez
                errorLog = GestorSincronizar.Sincronizar(sentencia1, sentencia2, sentencia3, sentencia4, sentencia5,sentencia6,contexto);
            }

            //LJR Caso 34610, no considerar un error si la sincronizacion fue cancelada por el usuario
            if (errorLog.Equals("usercancel"))
            {
                if (!sincroDesatendida)
                    viewModel.mostrarAlerta("Sincronización cancelada por el usuario.");
                Bitacora.Escribir("Sincronización cancelada por el usuario.");
                resultado = false;
            }
            else if (errorLog != string.Empty)
            {
                if (!sincroDesatendida)
                    viewModel.mostrarAlerta("Se produjo un error en la sincronización de datos. " + errorLog);
                Bitacora.Escribir("Se produjo un error en la sincronización de datos. " + errorLog);
                resultado = false;
            }
            else
            {
                if (!sincroDesatendida)
                    viewModel.mostrarAlerta("La sincronización se realizó exitósamente");
                Bitacora.Escribir("La sincronización se realizó exitósamente");
                resultado = true;
            }
            try
            {
                GestorDatos.Conectar();
                // LAS. Actualizar los registros de jornada rutas como actualizados.
                if (resultado)
                {
                    JornadaRuta.EstablecerSincronizado();
                }
            }
            catch (Exception ex)
            {
                if (!sincroDesatendida)
                    viewModel.mostrarAlerta("No se pudo reconectar a la base de datos.");
                Bitacora.Escribir("No se pudo reconectar a la base de datos." + ex.Message);
                resultado = false;
            }
            return resultado;
        }

        public static bool DescargaDatosDesatendida(List<Ruta> rutasActualizacion, Android.Content.Context contexto)
        {
            GestorDatos.LicenciaValida = false;
            string sentencia1 = string.Empty;
            string sentencia2 = string.Empty;
            string sentencia3 = string.Empty;
            string sentencia4 = string.Empty;
            string sentencia5 = string.Empty;
            string sentencia6 = string.Empty;
            bool resultado = false;
            //LJR 04/03/2009 Caso: 34986,34987,34988
            //Las siguientes sentencias 4 y 5 se ejecutan en los conductos del EMM ahora 

            try
            {
                sentencia1 = ParametroSistema.GenerarSentenciaActualizacion();
            }
            catch (Exception ex)
            {
                if (!sincroDesatendida)
                    viewModel.mostrarAlerta("Error creando sentencia de actualización de consecutivos. " + ex.Message);
                return false;
            }

            try
            {
                if (FRdConfig.UsaFacturacion) sentencia2 = Articulo.SentenciaActualizacionServidor();
            }
            catch (Exception ex)
            {
                if (!sincroDesatendida)
                    viewModel.mostrarAlerta("Error creando sentencia de actualización de existencias. " + ex.Message);
                return false;
            }

            //LDS 21/02/2008 - Venta en consignación.
            //Se obtiene la sentencia que permite actualizar las boletas de venta en consignación.
            try
            {
                //LJR 04/03/2009 Caso: 34986,34987,34988
                //Las siguientes sentencias 4 y 5 se ejecutan en los conductos del EMM ahora  
                BoletasConsignacion.GenerarSentenciaActualizacion(ref sentencia3, rutasActualizacion);
            }
            catch (Exception ex)
            {
                if (!sincroDesatendida)
                    viewModel.mostrarAlerta("Error creando sentencia de sincronización de boletas. " + ex.Message);
                return false;
            }

            // LAS.  Crear las sentencias para sincronizar los datos de las jornadas.
            try
            {
                sentencia4 = JornadaRuta.ObtenerSentenciaActualizacionJornada();
            }
            catch (Exception ex)
            {
                if (!sincroDesatendida)
                    viewModel.mostrarAlerta(String.Format("Error creando sentencia de sincronización de jornadas. ", ex.Message));
                return false;
            }

            try
            {
                sentencia5 = NCFUtilitario.ObtenerSentenciaActualizacionNCF();
            }
            catch (Exception ex)
            {
                if (!sincroDesatendida)
                    viewModel.mostrarAlerta(String.Format("Error creando sentencia de sincronización de consecutivos NCF. ", ex.Message));
                return false;
            }
            try
            {
                sentencia6 = ResolucionUtilitario.ObtenerSentenciaActualizacionResolucion();
            }
            catch (Exception ex)
            {
                viewModel.mostrarAlerta(String.Format("Error creando sentencia de sincronización de consecutivos NCF. ", ex.Message));
                return false;
            }



            //Se agrega bitacora para ver la sentencia de actualizacion de consecutivos.
            //Se agrega bitacora para ver la sentencia de actualizacion de existencias.
            //Se agrega bitacora para ver la sentencia de actualizacion de boletas.
            Bitacora.Escribir("Se van a descargar datos");
            Bitacora.Escribir("Consecutivos actuales: " + sentencia1);
            Bitacora.Escribir("Actualización de existencias: " + sentencia2);
            Bitacora.Escribir("Actualización de boletas de venta en consignación: " + sentencia3);
            Bitacora.Escribir("Actualización de jornadas laborales " + sentencia4);
            Bitacora.Escribir("Consecutivos NCF actuales y a Actualizar" + sentencia5);

            //GestorDatos.Desconectar();

            string errorLog = GestorSincronizar.SincronizarDesatendido(sentencia1, sentencia2, sentencia3, sentencia4, sentencia5,sentencia6, contexto);

            //LJR Caso 34610, no considerar un error si la sincronizacion fue cancelada por el usuario
            if (errorLog.Equals("usercancel"))
            {
                if (!sincroDesatendida)
                    viewModel.mostrarAlerta("Sincronización cancelada por el usuario.");
                Bitacora.Escribir("Sincronización cancelada por el usuario.");
                resultado = false;
            }
            else if (errorLog != string.Empty)
            {
                if (!sincroDesatendida)
                    viewModel.mostrarAlerta("Se produjo un error en la sincronización de datos. " + errorLog);
                Bitacora.Escribir("Se produjo un error en la sincronización de datos. " + errorLog);
                resultado = false;
            }
            else
            {
                if (!sincroDesatendida)
                    viewModel.mostrarAlerta("La sincronización se realizó exitósamente");
                Bitacora.Escribir("La sincronización se realizó exitósamente");
                resultado = true;
            }
            try
            {
                //GestorDatos.Conectar();
                // LAS. Actualizar los registros de jornada rutas como actualizados.
                if (resultado)
                {
                    JornadaRuta.EstablecerSincronizado();
                }
            }
            catch (Exception ex)
            {
                if (!sincroDesatendida)
                    viewModel.mostrarAlerta("No se pudo reconectar a la base de datos.");
                Bitacora.Escribir("No se pudo reconectar a la base de datos." + ex.Message);
                resultado = false;
            }
            return resultado;
        }

        #region Verificar Actualización

        public static bool VerificarActualizacion(Android.Content.Context contexto)
        {
            bool resultado = false;;

			EMMClientManager clientManager = null;
	
			try
			{
				clientManager = new EMMClientManager(ObtenerFirmaDispositivo(contexto), 
					GestorDatos.NombreUsuario,
					GestorDatos.ContrasenaUsuario,
					GestorDatos.ServidorWS,
					GestorDatos.Dominio,
					GestorDatos.Owner,
                    GestorDatos.cnx,
                    FRmConfig.EMMVerificar,
					false);

                resultado = clientManager.VerificarActualizacion();
			}
			catch
			{
				resultado = false;
			}

			return resultado;
		}



        #endregion

        #region Purga de datos temporizada
        /// <summary>
        ///  Purga los documentos de la ruta dada y con fecha de creación
        /// anterior a la cantidad de días determinada. Se debe desconectar la base de datos
        /// antes de ejecutar este proceso.
        /// </summary>
        /// <param name="ruta"></param>
        /// <param name="cantidadDias"></param>
        /// <returns></returns>
        public static bool PurgarDatos(string ruta,int cantidadDias,bool manual,Android.Content.Context contexto,bool soloPurga)
        {
            GestorDatos.LicenciaValida = false;
            bool resultado = false;
            try
            {
                Bitacora.Escribir("Se van a pugar los documentos");

                GestorDatos.Desconectar();

                string errorLog = GestorSincronizar.EjecutarPurgaDatos(ruta, cantidadDias, manual, contexto);
                if (errorLog.Equals("usercancel"))
                {
                    if (!sincroDesatendida)
                        viewModel.mostrarAlerta("Purga cancelada por el usuario.");
                    Bitacora.Escribir("Purga cancelada por el usuario.");
                    resultado = false;
                }
                else if (errorLog != string.Empty)
                {
                    if (!sincroDesatendida)
                        viewModel.mostrarAlerta("Se produjo un error en la purga de datos. " + errorLog);
                    Bitacora.Escribir("Se produjo un error en la purga de datos. " + errorLog);
                    resultado = false;
                }
                else
                {             
                    if(soloPurga)
                        if (!sincroDesatendida)
                            viewModel.mostrarAlerta("La purga de datos se realizó exitosamente.");
                    Bitacora.Escribir("La purga de datos se realizó exitosamente.");
                    resultado = true;
                }
                try
                {
                    //Sin importar el resultado de la carga, intentamos volver a conectar a la base de datos
                    GestorDatos.Conectar();
                }
                catch (Exception)
                {
                    if (!sincroDesatendida)
                        viewModel.mostrarAlerta("No se pudo reconectar a la base de datos.");
                    Bitacora.Escribir("No se pudo reconectar a la base de datos.");
                    resultado = false;
                }
            }
            catch (Exception ex)
            {
                if (!sincroDesatendida)
                    viewModel.mostrarAlerta("Se produjo un error en la purga de datos.");
                Bitacora.Escribir("Se produjo un error en la purga de datos." + ex.Message);
            }
            return resultado;
        }
        /// <summary>
        ///  Purga los documentos de la ruta dada y con fecha de creación
        /// anterior a la cantidad de días determinada. Se debe desconectar la base de datos
        /// antes de ejecutar este proceso.
        /// </summary>
        /// <param name="ruta"></param>
        /// <param name="cantidadDias"></param>
        /// <returns></returns>
        public static bool PurgarDatosDesatendida(string ruta, int cantidadDias, bool manual, Android.Content.Context contexto, bool soloPurga)
        {
            GestorDatos.LicenciaValida = false;
            bool resultado = false;
            try
            {
                Bitacora.Escribir("Se van a pugar los documentos");

                //GestorDatos.Desconectar();

                string errorLog = GestorSincronizar.EjecutarPurgaDatosDesatendido(ruta, cantidadDias, manual, contexto);
                if (errorLog.Equals("usercancel"))
                {
                    if (!sincroDesatendida)
                        viewModel.mostrarAlerta("Purga cancelada por el usuario.");
                    Bitacora.Escribir("Purga cancelada por el usuario.");
                    resultado = false;
                }
                else if (errorLog != string.Empty)
                {
                    if (!sincroDesatendida)
                        viewModel.mostrarAlerta("Se produjo un error en la purga de datos. " + errorLog);
                    Bitacora.Escribir("Se produjo un error en la purga de datos. " + errorLog);
                    resultado = false;
                }
                else
                {
                    if (soloPurga)
                        if (!sincroDesatendida)
                            viewModel.mostrarAlerta("La purga de datos se realizó exitosamente.");
                    Bitacora.Escribir("La purga de datos se realizó exitosamente.");
                    resultado = true;
                }
                try
                {
                    //Sin importar el resultado de la carga, intentamos volver a conectar a la base de datos
                    //GestorDatos.Conectar();
                }
                catch (Exception)
                {
                    if (!sincroDesatendida)
                        viewModel.mostrarAlerta("No se pudo reconectar a la base de datos.");
                    Bitacora.Escribir("No se pudo reconectar a la base de datos.");
                    resultado = false;
                }
            }
            catch (Exception ex)
            {
                if (!sincroDesatendida)
                    viewModel.mostrarAlerta("Se produjo un error en la purga de datos.");
                Bitacora.Escribir("Se produjo un error en la purga de datos." + ex.Message);
            }
            return resultado;
        }
        /// <summary>
        /// Ejecuta el preceso de purga de datos
        /// </summary>
        /// <returns></returns>
        public static string EjecutarPurgaDatos(string ruta, int cantidadDias, bool manual,Android.Content.Context contexto)
        {
            string errorLog = string.Empty;

            EMMClientManager syn = null;
            try
            {
                syn = new EMMClientManager(ObtenerFirmaDispositivo(contexto), 
                    GestorDatos.NombreUsuario,
                    GestorDatos.ContrasenaUsuario,
                    GestorDatos.ServidorWS,
                    GestorDatos.Dominio,
                    GestorDatos.Owner,
                    GestorDatos.cnx,
                    FRmConfig.EMMPurga,
                    false);
                syn.Compactar = false;
                syn.CerrarAlFinalizar = !manual;
                syn.TitleLabelText = "Purgando Documentos ...";
                //Syn.UsarBitacora = true;
                syn.Parameters.Add(ruta);
                syn.Parameters.Add(cantidadDias.ToString());
                bool temp = GestorDatos.LicenciaValida;
                syn.Synchronize(Ruta.NombreDispositivo(),ref temp);
                GestorDatos.LicenciaValida = temp;
                errorLog = syn.ErrorLog;
            }
            catch (Exception e)
            {
                if (syn != null)
                    errorLog = syn.ErrorLog + " " + e.Message;
                else
                    errorLog = e.Message;
            }
            return errorLog;
        }

        /// <summary>
        /// Ejecuta el preceso de purga de datos
        /// </summary>
        /// <returns></returns>
        public static string EjecutarPurgaDatosDesatendido(string ruta, int cantidadDias, bool manual, Android.Content.Context contexto)
        {
            string errorLog = string.Empty;

            EMMClientManager syn = null;
            try
            {
                syn = new EMMClientManager(ObtenerFirmaDispositivo(contexto),
                    GestorDatos.NombreUsuario,
                    GestorDatos.ContrasenaUsuario,
                    GestorDatos.ServidorWS,
                    GestorDatos.Dominio,
                    GestorDatos.Owner,
                    GestorDatos.cnx,
                    FRmConfig.EMMPurga,
                    false,true);
                syn.Compactar = false;
                syn.CerrarAlFinalizar = !manual;
                syn.TitleLabelText = "Purgando Documentos ...";
                //Syn.UsarBitacora = true;
                syn.Parameters.Add(ruta);
                syn.Parameters.Add(cantidadDias.ToString());
                bool temp = GestorDatos.LicenciaValida;
                syn.Synchronize(Ruta.NombreDispositivo(), ref temp);
                GestorDatos.LicenciaValida = temp;
                errorLog = syn.ErrorLog;
            }
            catch (Exception e)
            {
                if (syn != null)
                    errorLog = syn.ErrorLog + " " + e.Message;
                else
                    errorLog = e.Message;
            }
            return errorLog;
        }

        /// <summary>
        /// Ejecuta el preceso de creación de tablas
        /// </summary>
        /// <returns></returns>
        public static string EjecutarCrearTablas(Android.Content.Context contexto,string NombreDispositivo)
        {
            string errorLog = string.Empty;

            EMMClientManager syn = null;
            try
            {
                syn = new EMMClientManager(ObtenerFirmaDispositivo(contexto), 
                    GestorDatos.NombreUsuario,
                    GestorDatos.ContrasenaUsuario,
                    GestorDatos.ServidorWS,
                    GestorDatos.Dominio,
                    GestorDatos.Owner,
                    GestorDatos.cnx,
                    FRmConfig.EMMCrear,
                    false);
                syn.Compactar = false;
                syn.CerrarAlFinalizar = true;
                syn.TitleLabelText = "Creando Tablas ...";
                bool temp = GestorDatos.LicenciaValida;
                syn.Synchronize(NombreDispositivo,ref temp);
                GestorDatos.LicenciaValida = temp;

                errorLog = syn.ErrorLog;
            }
            catch (Exception e)
            {
                if (syn != null)
                    errorLog = syn.ErrorLog + " " + e.Message;
                else
                    errorLog = e.Message;
            }
            return errorLog;
        }

        
        #endregion

        #endregion
    }
}
