using System;
using System.Xml;
using Softland.ERP.FR.Mobile.Cls.AccesoDatos;
using Softland.ERP.FR.Mobile.Cls.Reporte;
using Softland.ERP.FR.Mobile.Cls.Documentos.FRDevolucion;
using Softland.ERP.FR.Mobile.Cls.Cobro;
using Softland.ERP.FR.Mobile.Cls.FRArticulo;
using Softland.ERP.FR.Mobile.Cls.Documentos;
using Softland.ERP.FR.Mobile.Cls.Documentos.FRPedido;

namespace Softland.ERP.FR.Mobile.Cls.Utilidad
{
	/// <summary>
	/// Clase que se encarga de leer la configuracion del sistema en Config.xml
	/// </summary>
	public class ConfigurationClass
	{
		#region Metodos publicos
		/// <summary>
		/// Lee toda la informacion del xml.
		/// </summary>
		public static void ReadXml(string configurationFile)
		{
            if (System.IO.File.Exists(configurationFile))
            {
                //Abre el archivo XML
                XmlTextReader ConfigurationReader = new XmlTextReader(configurationFile);

                XmlDocument documento = new XmlDocument();
                documento.Load(ConfigurationReader);

                XmlNodeList basesDatos = documento.DocumentElement.GetElementsByTagName("BasesDeDatos");
                XmlNodeList servidoresWeb = documento.DocumentElement.GetElementsByTagName("ServidoresWeb");
                XmlNodeList HandHeld = documento.DocumentElement.GetElementsByTagName("HandHeld");
                XmlNodeList GuardarenSD = documento.DocumentElement.GetElementsByTagName("GuardarenSD");

                ReadDataBaseSection((XmlElement)basesDatos.Item(0));
                ReadWebServersSection((XmlElement)servidoresWeb.Item(0));
                ReadHandHeldSection((XmlElement)HandHeld.Item(0));
                ReadGuardarenSDSection((XmlElement)GuardarenSD.Item(0));


                //Cierra el archivo XML
                ConfigurationReader.Close();


                if (GestorDatos.BaseDatos.Equals(string.Empty))
                    throw new System.Exception("Nombre de Base de datos.");

                if (GestorDatos.ServidorWS.Equals(string.Empty))
                    throw new System.Exception("Nombre servidor web.");

                //if (GestorDatos.Dominio.Equals(string.Empty))
                    //throw new System.Exception("Dominio del servidor web.");

                if (GestorDatos.Owner.Equals(string.Empty))
                    throw new System.Exception("Owner del servidor web.");
            }
            else
            {
                throw new System.Exception("No existe el archivo '" + configurationFile + "'");
            }
		}

		#endregion

		#region Metodos privados
		
		/// <summary>
		/// Lee los datos de la seccion de bases de datos. Se asume que el xml esta bien formado
		/// </summary>
		/// <param name="dataBaseSection"></param>
		private static void ReadDataBaseSection(XmlElement dataBaseSection)
		{
			XmlElement baseDatos = (XmlElement)dataBaseSection.GetElementsByTagName("BaseDeDatos")[0];

			GestorDatos.BaseDatos = baseDatos.GetElementsByTagName("Nombre")[0].InnerText;
			GestorDatos.Owner = baseDatos.GetElementsByTagName("Compañia")[0].InnerText;
		}

		/// <summary>
		/// Lee los datos de la seccion del Url Servers. Se asume que el xml esta bien formado
		/// </summary>
		/// <param name="webServersSection"></param>
		private static void ReadWebServersSection(XmlElement webServersSection)
		{
			XmlElement servidorWeb = (XmlElement)webServersSection.GetElementsByTagName("ServidorWeb")[0];

			GestorDatos.ServidorWS = servidorWeb.GetElementsByTagName("Nombre")[0].InnerText;
			GestorDatos.Dominio = servidorWeb.GetElementsByTagName("Dominio")[0].InnerText;
		}

        /// <summary>
        /// Lee los datos de la seccion HandHelds. Se asume que el xml esta bien formado
        /// </summary>
        /// <param name="webServersSection"></param>
        private static void ReadHandHeldSection(XmlElement HandHeldSection)
        {
            FRmConfig.NombreHandHeld = HandHeldSection.GetElementsByTagName("Nombre")[0].InnerText;
        }

        /// <summary>
        /// Lee los datos de la seccion GuardarenSD. Se asume que el xml esta bien formado
        /// </summary>
        /// <param name="webServersSection"></param>
        private static void ReadGuardarenSDSection(XmlElement GuardarenSDSection)
        {
            FRmConfig.GuardarenSD = GuardarenSDSection.GetElementsByTagName("Valor")[0].InnerText.Equals("Si");
        }

        
        #endregion
	}

}