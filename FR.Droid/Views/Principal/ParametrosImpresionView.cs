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

namespace FR.Droid.Views.Cobro
{
    [Activity(Label = "FR - Configuración de la Impresora", WindowSoftInputMode = SoftInput.AdjustPan, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, ConfigurationChanges = Android.Content.PM.ConfigChanges.KeyboardHidden | Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.ScreenSize)]
    public class ParametrosImpresionView : MvxBindingActivityView<ParametrosImpresoraViewModel>
    {
        RadioButton rdSugerir;
        RadioButton rdNoSugerir;
        MvxBindableSpinner cmbPapeles;
        MvxBindableSpinner cmbImpresoras;
        

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.ParametrosImpresion);
            rdSugerir = (RadioButton)this.FindViewById(Resource.Id.rdSugerir);
            rdNoSugerir = (RadioButton)this.FindViewById(Resource.Id.rdNoSugerir);
            cmbPapeles = (MvxBindableSpinner)this.FindViewById(Resource.Id.cmbPapeles);
            cmbImpresoras = (MvxBindableSpinner)this.FindViewById(Resource.Id.cmbImpresoras);        
            

            rdSugerir.CheckedChange += new EventHandler<CompoundButton.CheckedChangeEventArgs>(rdSugerir_CheckedChange);
            rdNoSugerir.CheckedChange += new EventHandler<CompoundButton.CheckedChangeEventArgs>(rdNoSugerir_CheckedChange);                               
            cmbPapeles.Adapter.DropDownItemTemplateId = Resource.Layout.OpcionDropDownEnum;
            cmbImpresoras.Adapter.DropDownItemTemplateId = Resource.Layout.OpcionDropDownEnum;
        }

        protected override void OnViewModelSet()
        {                        
        }

        protected override void OnStart()
        {
            base.OnStart();
            Softland.ERP.FR.Mobile.App.VerificarConexionBaseDatos(Util.cnxDefault());
            Softland.ERP.FR.Mobile.App.setCurrentActivity(this);
            if (ViewModel.SugerirImpresion)
            {
                rdSugerir.Checked = true;
                rdNoSugerir.Checked = false;
            }
            else
            {
                rdSugerir.Checked = false;
                rdNoSugerir.Checked = true;
            }   
        }

        private void rdSugerir_CheckedChange(object sender, Android.Widget.CompoundButton.CheckedChangeEventArgs args)
        {
            //((EditText)sender).Text;
            if (((RadioButton)sender).Checked)
            {
                ViewModel.Sugerir();
            }
        }

        private void rdNoSugerir_CheckedChange(object sender, Android.Widget.CompoundButton.CheckedChangeEventArgs args)
        {
            //((EditText)sender).Text;
            if (((RadioButton)sender).Checked)
            {
                ViewModel.NoSugerir();
            }
        }
        
    
    }
}