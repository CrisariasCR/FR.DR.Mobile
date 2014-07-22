using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cirrious.MvvmCross.Commands;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.Windows.Input;
using Cirrious.MvvmCross.Droid.Interfaces;
using Cirrious.MvvmCross.ExtensionMethods;
using Cirrious.MvvmCross.Interfaces.ServiceProvider;
using Cirrious.MvvmCross.Binding.Droid.Interfaces.Views;

using Cirrious.MvvmCross.Binding.Droid.Views;
using Softland.ERP.FR.Mobile.ViewModels;
using Softland.ERP.FR.Mobile.Cls.FRArticulo;
using FR.DR.Core.Helper;



namespace FR.Droid.Views
{
    [Activity(Label = "FR - Notas de Crédito", WindowSoftInputMode = SoftInput.AdjustPan, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, ConfigurationChanges = Android.Content.PM.ConfigChanges.KeyboardHidden | Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.ScreenSize)]
    public class SeleccionNotasCreditoView : MvxBindingActivityView<SeleccionNotasCreditoViewModel>
    {
        //MvxBindableListView items;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.SeleccionNotasCredito);
            //items = FindViewById<MvxBindableListView>(Resource.Id.items);
            //items.ChoiceMode = ChoiceMode.Multiple;
            //items.ChoiceMode = (ChoiceMode)AbsListViewChoiceMode.MultipleModal;
            //items.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(items_ItemSelected);
            //items.ItemClick = ComandoSeleccionar;
            
          
        }

        protected override void OnStart()
        {
            base.OnStart();
            Softland.ERP.FR.Mobile.App.VerificarConexionBaseDatos(Util.cnxDefault());
            Softland.ERP.FR.Mobile.App.setCurrentActivity(this);
        }

        protected override void OnViewModelSet()
        {
            
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.Abar, menu);
            return true;
        }

        public override bool OnMenuItemSelected(int featureId, IMenuItem item)
        {
            var menucli = FindViewById(Resource.Id.menucli);
            if (menucli.Id == item.ItemId)
            {
                Toast tx = Toast.MakeText(this, ViewModel.NameCliente, ToastLength.Short);
                tx.SetGravity(GravityFlags.Center, 0, 0);
                tx.Show();
            }
            return base.OnMenuItemSelected(featureId, item);
        }        

        public override void OnBackPressed()
        {
            ViewModel.Regresar();
        }
    }
}