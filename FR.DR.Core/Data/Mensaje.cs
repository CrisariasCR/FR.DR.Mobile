using System;
using System.Windows.Forms;
using Softland.ERP.FR.Mobile.ViewModels;

namespace Softland.ERP.FR.Mobile
{
	/// <summary>
	/// Clase para encapsular el despligue de mensajes.
	/// </summary>
    public static class Mensaje
	{

        public enum Accion
		{
			Anular,
			Retirar,
            Imprimir,
			Terminar,
			BusquedaMala,
			EntradaInvalida,
			UsuarioInvalido,
			ContrasenaInvalida,
			SeleccionNula,
			Cancelar,
			Informacion,
			Alerta,
			NoExisteBD,
			Decision,
            ErrorConfiguracion
		}

        /// <summary>
        /// Indica el titulo de la caja de mensajes
        /// </summary>
        internal const string titulo = "Facturación de Rutero.";

		/// <summary>
		/// Indica la accion a ejecutar
		/// </summary>
		private static Accion accion;

		/// <summary>
		/// Indica el proceso relacionado
		/// </summary>
		private static string proceso = string.Empty; 
        
		private static void definirAccionProceso(Accion accionp , string procesop)
		{
            accion = accionp;
			proceso = procesop;
		}

        internal static DialogResult mostrarAlerta(BaseViewModel viewModel, string alerta, Action<System.Windows.Forms.DialogResult> callback)
        {
            Mensaje.definirAccionProceso(Accion.Alerta,alerta);

            viewModel.ShowMessage(Mensaje.titulo, Mensaje.obtenerMensaje(), Mensaje.obtenerBoton(), Mensaje.obtenerIcono(), callback);

            return DialogResult.None;
        }

        // se va a eliminar
        //internal static DialogResult mostrarAlerta(string alerta)
        //{
        //    Mensaje.definirAccionProceso(Accion.Alerta, alerta);
        //    throw new ApplicationException("No se ha implementado MessageBox en mostrarMensaje(string alerta)");

        //    return DialogResult.Cancel;
        //}

        internal static DialogResult mostrarMensaje(Accion accion)
        {
            Mensaje.definirAccionProceso(accion,"");
            //return MessageBox.Show(Mensaje.obtenerMensaje(),Mensaje.titulo,Mensaje.obtenerBoton(),Mensaje.obtenerIcono(),Mensaje.obtenerDefaultButton());
            return DialogResult.None;
        }

        internal static DialogResult mostrarMensaje(BaseViewModel viewModel, Accion accion, Action<System.Windows.Forms.DialogResult> callback)
        {
            Mensaje.definirAccionProceso(accion, "");
            viewModel.ShowMessage(Mensaje.titulo, Mensaje.obtenerMensaje(), Mensaje.obtenerBoton(), Mensaje.obtenerIcono(), callback);
            return DialogResult.None;
        }

        internal static DialogResult mostrarMensaje(BaseViewModel viewModel, Accion accion, string procesoP, Action<System.Windows.Forms.DialogResult> callback)
        {
            Mensaje.definirAccionProceso(accion,procesoP);

            viewModel.ShowMessage(Mensaje.titulo, Mensaje.obtenerMensaje(), Mensaje.obtenerBoton(), Mensaje.obtenerIcono(), callback);
            //return MessageBox.Show(Mensaje.obtenerMensaje(),Mensaje.titulo,Mensaje.obtenerBoton(),Mensaje.obtenerIcono(),Mensaje.obtenerDefaultButton());
            return DialogResult.None;
        }

        // se va a eliminar
        //internal static DialogResult mostrarMensaje(Accion accion, string procesoP)
        //{
        //    Mensaje.definirAccionProceso(accion, procesoP);

        //    throw new ApplicationException("No se ha implementado MessageBox en mostrarMensaje(string alerta)");
            
        //    return DialogResult.Cancel;
        //}

        /// <summary>
        /// Metodo que define el mensaje a mostrar
        /// </summary>
		private static string obtenerMensaje()
		{
			string mensaje = string.Empty;
 
			switch((Accion)accion)
			{
				case Accion.Anular:
					mensaje = "¿Desea anular " + proceso + "?";
					break;

				case Accion.Imprimir:
				    mensaje = "¿Desea imprimir " + proceso + "?";
					break;

				case Accion.Retirar:
					mensaje = "¿Desea retirar " + proceso + "?";
					break;

			    case Accion.Terminar:
					mensaje = "¿Desea terminar " + proceso + "?";
					break;

			    case Accion.BusquedaMala:
					mensaje = "No hay resultados de búsqueda.";
					break;

				case Accion.EntradaInvalida:
					mensaje = "Por favor verifique los datos ingresados.";
					break;
				
				case Accion.UsuarioInvalido:
					mensaje = "Usuario inválido.";
					break;

				case Accion.ContrasenaInvalida:
					mensaje = "Contraseña inválida.";
					break;

				case Accion.SeleccionNula:
					mensaje = "Debe seleccionar "+ proceso + ".";
					break;

				case Accion.Cancelar:
					mensaje = "¿Desea cancelar " + proceso + "?";
					break;
				
				case Accion.Informacion:
					  mensaje = proceso;
					break;

				case Accion.Alerta:
					mensaje = proceso;
					break;

				case Accion.Decision:
					mensaje = "¿Desea " + proceso + "?";
					break;
				default:
					mensaje = proceso;
					break;
			} 
 
			return mensaje;
		}


        private static System.Windows.Forms.MessageBoxButtons obtenerBoton()
        {	
            System.Windows.Forms.MessageBoxButtons boton;
					
            switch((Accion)accion)
            {				
                case Accion.BusquedaMala:
                case Accion.EntradaInvalida:
                case Accion.Alerta:
                case Accion.UsuarioInvalido:
                case Accion.ContrasenaInvalida:
                case Accion.SeleccionNula:
                case Accion.Informacion:
                    boton = System.Windows.Forms.MessageBoxButtons.OK; 
                    break;

                default:
                    boton = System.Windows.Forms.MessageBoxButtons.YesNo;
                    break;
            }

            return boton;
        }


        private static System.Windows.Forms.MessageBoxIcon obtenerIcono()
        {
            System.Windows.Forms.MessageBoxIcon icono;

            switch ((Accion)accion)
            {
                case Accion.BusquedaMala:
                    icono = System.Windows.Forms.MessageBoxIcon.Asterisk;
                    break;

                case Accion.SeleccionNula:

                    icono = System.Windows.Forms.MessageBoxIcon.Asterisk;
                    break;

                case Accion.Informacion:
                    icono = System.Windows.Forms.MessageBoxIcon.Asterisk;
                    break;

                case Accion.EntradaInvalida:
                    icono = System.Windows.Forms.MessageBoxIcon.Exclamation;
                    break;

                case Accion.Alerta:
                    icono = System.Windows.Forms.MessageBoxIcon.Exclamation;
                    break;

                case Accion.UsuarioInvalido:
                    icono = System.Windows.Forms.MessageBoxIcon.Exclamation;
                    break;

                case Accion.ContrasenaInvalida:
                    icono = System.Windows.Forms.MessageBoxIcon.Exclamation;
                    break;

                default:
                    icono = System.Windows.Forms.MessageBoxIcon.Question;
                    break;

            }

            return icono;
        }


        // de momento esta no se implementa, pues parece que en Android/WinPhone/iOS no tiene sentido
        //private static System.Windows.Forms.MessageBoxDefaultButton obtenerDefaultButton()
        //{

        //    System.Windows.Forms.MessageBoxDefaultButton botonDefecto;

        //    switch ((Accion)accion)
        //    {
        //        case Accion.BusquedaMala:
        //            botonDefecto = System.Windows.Forms.MessageBoxDefaultButton.Button1;
        //            break;

        //        case Accion.EntradaInvalida:
        //            botonDefecto = System.Windows.Forms.MessageBoxDefaultButton.Button1;
        //            break;

        //        case Accion.UsuarioInvalido:
        //            botonDefecto = System.Windows.Forms.MessageBoxDefaultButton.Button1;
        //            break;

        //        case Accion.ContrasenaInvalida:
        //            botonDefecto = System.Windows.Forms.MessageBoxDefaultButton.Button1;
        //            break;

        //        case Accion.Alerta:
        //            botonDefecto = System.Windows.Forms.MessageBoxDefaultButton.Button1;
        //            break;

        //        case Accion.SeleccionNula:
        //            botonDefecto = System.Windows.Forms.MessageBoxDefaultButton.Button1;
        //            break;

        //        case Accion.Informacion:
        //            botonDefecto = System.Windows.Forms.MessageBoxDefaultButton.Button1;
        //            break;

        //        default:
        //            botonDefecto = System.Windows.Forms.MessageBoxDefaultButton.Button2;
        //            break;

        //    }

        //    return botonDefecto;
        //}


	}
}
