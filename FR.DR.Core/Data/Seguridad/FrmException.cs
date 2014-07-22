using System;

namespace Softland.ERP.FR.Mobile.Cls.Seguridad
{
	/// <summary>
	/// Excepciones utilizadas para el manejo de mensajes.
	/// </summary>
	public class FrmException : Exception
	{
		private string mensaje;

		public override string Message
		{
			get
			{
				return this.mensaje;
			}
		}
		
		public FrmException(string mensaje)
		{
			this.mensaje = mensaje;
		}
	}
}
