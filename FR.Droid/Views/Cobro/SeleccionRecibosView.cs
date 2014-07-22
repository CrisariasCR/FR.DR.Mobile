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
    [Activity(Label = "FR - Realizar Depósito", WindowSoftInputMode = SoftInput.AdjustPan, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, ConfigurationChanges = Android.Content.PM.ConfigChanges.KeyboardHidden | Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.ScreenSize)]
    public class SeleccionRecibosView : MvxBindingActivityView<SeleccionRecibosViewModel>
    {
        RadioButton rdLocal;
        RadioButton rdDolar;
        MvxBindableSpinner cmbCompanias, cmbEntidades;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.SeleccionRecibos);
            rdLocal = (RadioButton)this.FindViewById(Resource.Id.rdLocal);
            rdDolar = (RadioButton)this.FindViewById(Resource.Id.rdDolar);
            cmbCompanias = FindViewById<MvxBindableSpinner>(Resource.Id.cmbCompaniassr);
            cmbEntidades = FindViewById<MvxBindableSpinner>(Resource.Id.cmbEntidadessr);
            rdLocal.CheckedChange += new EventHandler<CompoundButton.CheckedChangeEventArgs>(rdLocal_CheckedChange);
            rdDolar.CheckedChange += new EventHandler<CompoundButton.CheckedChangeEventArgs>(rdDolar_CheckedChange);            

            cmbCompanias.Adapter.DropDownItemTemplateId = Resource.Layout.OpcionDropDownEnum;
            cmbEntidades.Adapter.DropDownItemTemplateId = Resource.Layout.OpcionDropDownEnum;
        
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

        private void rdLocal_CheckedChange(object sender, Android.Widget.CompoundButton.CheckedChangeEventArgs args)
        {
            //((EditText)sender).Text;
            if (((RadioButton)sender).Checked)
            {
                ViewModel.LocalSeleccionado = true;
            }
            else 
            {
                ViewModel.LocalSeleccionado = false;
            }
        }

        private void rdDolar_CheckedChange(object sender, Android.Widget.CompoundButton.CheckedChangeEventArgs args)
        {
            //((EditText)sender).Text;
            if (((RadioButton)sender).Checked)
            {
                ViewModel.DolarSeleccionado = true;
            }
            else
            {
                ViewModel.DolarSeleccionado = false;
            }
        }
        
              
    }
}