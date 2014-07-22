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

using Cirrious.MvvmCross.Binding.Droid.Views;
using Softland.ERP.FR.Mobile.ViewModels;
using Softland.ERP.FR.Mobile.Cls.FRArticulo;
using FR.DR.Core.Helper;
using Android.Text;
using Softland.ERP.FR.Mobile.Cls;

namespace FR.Droid.Views.TomaFisica
{
    [Activity(Label = "FR - Conexión Toma Física", WindowSoftInputMode = SoftInput.AdjustPan, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, ConfigurationChanges = Android.Content.PM.ConfigChanges.KeyboardHidden | Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.ScreenSize)]
    public class ConexionTomFisicaView : MvxBindingActivityView<ConexionTomFisicaViewModel>
    {

        Android.Widget.ImageButton login;
        EditText txtUsuario;
        EditText txtPass;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.ConexionTomFisica);
            login = (ImageButton)this.FindViewById(Resource.Id.btnLogin);
            txtUsuario = (EditText)this.FindViewById(Resource.Id.TxtUsuario);
            txtPass = (EditText)this.FindViewById(Resource.Id.TxtPassword);            
        }

        protected override void OnViewModelSet()
        {            
        }

        protected override void OnStart()
        {
            base.OnStart();
            Softland.ERP.FR.Mobile.App.VerificarConexionBaseDatos(Util.cnxDefault());
            Softland.ERP.FR.Mobile.App.setCurrentActivity(this);
        }

        private void btnLogin_Click(object sender, EventArgs args)
        {          
            if (ViewModel.UsuarioFocus)
            {
                this.txtUsuario.RequestFocus();
            }
            if (ViewModel.PasswordFocus)
            {
                this.txtPass.RequestFocus();
            }
        }
    
    }
}