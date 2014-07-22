using System;
using System.IO;
using System.Xml;
using System.Reflection;

namespace EMMClient.EMMClientLogic
{
	/// <summary>
	/// Clase para escribir mensajes en un archivo a modo de bitacora.
	/// </summary>
	public class Bitacora : IDisposable
	{
		public string LogFileName = "EMMClientLog.txt";

		private StreamWriter Archivo = null;

		public Bitacora()
		{
			
		}

		public void Inicializar()
		{
			try
			{
				// This is the full directory and exe name
				string	fullAppName = Assembly.GetExecutingAssembly().GetName().CodeBase;

				// This strips off the exe name
				string	fullAppPath = Path.GetDirectoryName(fullAppName);

				this.Archivo = new StreamWriter(fullAppPath + this.LogFileName,false);
				this.Archivo.AutoFlush = true;
			}
			catch
			{

			}
		}

		public void EscribirLinea(string mensaje)
		{
			try
			{
				this.Archivo.WriteLine(mensaje);
			}
			catch
			{
			}
		}

		public void Cerrar()
		{
			try
			{
				this.Archivo.Flush();
				this.Archivo.Close();
				this.Archivo = null;
			}
			catch
			{
			}
		}

		public void Dispose()
		{
			if (this.Archivo != null)
			{
				this.Cerrar();
			}
		}
	}
}
