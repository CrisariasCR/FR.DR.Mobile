using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace FR.Droid.CustomViews
{
    public class ShowMessage: AlertDialog
    {
        private string texto;
        private string titulo;

        /// <summary>
        /// Crea un dialogo simple sin titulo
        /// </summary>
        /// <param name="context"></param>
        /// <param name="text">mensaje</param>
        public ShowMessage(Context context, string text)
            : base(context)
        {
            this.texto = text;
            this.titulo = string.Empty;
        }

        /// <summary>
        /// Crea un dialogo con titulo
        /// </summary>
        /// <param name="context"></param>
        /// <param name="text">mensaje</param>
        /// <param name="error">si va en true el titulo es Error si no el titulo es Atención</param>
        public ShowMessage(Context context, string text, bool error)
            : base(context)
        {
            this.texto = text;
            if (error)
            {
                titulo = "Error";
            }
            else
            {
                titulo = "Atención";
            }
            
        }

        public void Mostrar() 
        {
            base.SetTitle(titulo);
            base.SetMessage(texto);
            base.SetCanceledOnTouchOutside(true);
            base.Show();
        }
        
    }
}