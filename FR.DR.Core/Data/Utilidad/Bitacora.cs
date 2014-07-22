using System;
using System.IO;

namespace Softland.ERP.FR.Mobile.Cls.Utilidad
{
	/// <summary>
	/// Clase que se encarga de manipular la bitacora de FR.
	/// </summary>
	public class Bitacora
	{
        public static string NombreArchivo = Path.Combine(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath, "FRmLog.txt");

		private static StreamWriter bitacora;

		/// <summary>
		/// Escribe un mensaje nuevo en la bitacora.
		/// </summary>
		/// <param name="mensaje"></param>
		public static void Escribir(string mensaje)
		{
			try
			{
				bitacora.WriteLine(mensaje);
			}
			catch
			{
				//Ignore
			}
		}

		/// <summary>
		/// Inicializa la bitacora del sistema.
		/// </summary>
		public static void Inicializar()
		{
			try
			{
				bitacora = new StreamWriter(NombreArchivo,true);
				bitacora.AutoFlush = true;
			}
			catch
			{
				//Ignore
			}
		}

		/// <summary>
		/// Realiza es respectivo cierre de la bitacora.
		/// </summary>
		public static void Finalizar()
		{
			try
			{
				bitacora.Flush();
				bitacora.Close();
				bitacora = null;
			}
			catch
			{
				//Ignore
			}
		}

		private static string ConvertMem(long memoria)
		{
			if (memoria < 1024)
			{
				//Desplegamos la memoria en bytes
				return memoria + "B";
			}
			else if (memoria < 1048576)
			{
				//Desplegamos la memoria en Kbs
				double mem = memoria / 1024; 

				return Math.Round(mem,2) + "K";
			}
			else
			{
				//Desplegamos la memoria en MBs

				double mem = memoria / 1048576; 

				return Math.Round(mem,2) + "M";
			}

		}

		/// <summary>
		/// Guarda en la bitacora el estado de la memoria antes y despues 
		/// de llamar al recolector de basura.
		/// </summary>
		public static void LogMemoria()
		{
			Bitacora.Escribir("Memoria asignada actual: " + ConvertMem(GC.GetTotalMemory(true)));

			Bitacora.Escribir("Limpiamos la memoria...");
			
			//Limpiamos la memoria del sistema
			//System.GC.Collect();

			Bitacora.Escribir("Memoria asignada actual: " + ConvertMem(GC.GetTotalMemory(true)));
		}
	}
}
