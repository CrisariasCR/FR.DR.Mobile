using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
using Android.App;
using Android.Content;
using Android.Views;
using Android.Widget;

using Cirrious.MvvmCross.Droid.Interfaces;
using Cirrious.MvvmCross.Binding.Droid.Interfaces.Views;
using Cirrious.MvvmCross.ExtensionMethods;
using Cirrious.MvvmCross.Interfaces.ServiceProvider;

using System.Windows.Forms;

namespace FR.Droid.NativeServices
{
    /// <summary>
    /// Clase que permite desplegar un messageBox
    /// de este link, hay alguna ideas utilizadas para su implementación
    /// http://stackoverflow.com/questions/10696769/messagebox-for-android-mono
    /// </summary>
    public class MessageDisplayer : IMvxServiceConsumer
    {
        internal readonly Context _applicationContext;

        public MessageDisplayer(Context applicationContext)
        {
            _applicationContext = applicationContext;

            var source = this.GetService<Softland.MvvmCross.MessageService.IMessageSource>();
            source.MessageReported += (sender, args) => ShowMessage(args.Title, args.Message, args.Buttons, args.Icon, args.Callback);
        }

        private void ShowMessage(string titulo, string mensaje, MessageBoxButtons buttons, MessageBoxIcon icon, Action<DialogResult> callback)
        {
            AlertDialog.Builder builder;
            if (Softland.ERP.FR.Mobile.App.getCurrentActivity() != null)
            {
                builder = new AlertDialog.Builder(Softland.ERP.FR.Mobile.App.getCurrentActivity());
            }
            else // con el applicationContext las alertas dan error! en su lugar se muestra un Toast
            {
                Toast.MakeText(_applicationContext, "Error mostrando Mensajes!", ToastLength.Long).Show();
                //builder = new AlertDialog.Builder(_applicationContext);
                return;
            }

            builder.SetTitle(titulo);
            // TODO: no se ha implementado el mostrar el icono en el mensaje.
            //builder.SetIcon(Android.Resource.Drawable.IcDialogAlert);
            builder.SetMessage(mensaje);
            switch (buttons)
            {
                case MessageBoxButtons.YesNo:
                    builder.SetPositiveButton("Si", (sender, e) => { if (callback != null) callback(DialogResult.Yes); });
                    builder.SetNegativeButton("No", (sender, e) => { if (callback != null) callback(DialogResult.No); });
                    break;
                case MessageBoxButtons.OK:
                    builder.SetPositiveButton("Aceptar", (sender, e) => { if (callback != null) callback(DialogResult.OK); });
                    break;
                case MessageBoxButtons.OKCancel:
                    builder.SetPositiveButton("Aceptar", (sender, e) => { if (callback != null)  callback(DialogResult.OK); });
                    builder.SetNegativeButton("Cancelar", (sender, e) => { if (callback != null) callback(DialogResult.Cancel); });
                    break;
                default:
                    builder.SetPositiveButton("Aceptar", (sender, e) => { if (callback != null) callback(DialogResult.OK); });
                    break;
            }

            builder.Show();
        }
    }
}