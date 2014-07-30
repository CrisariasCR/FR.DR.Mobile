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
using FR.Droid.CustomViews;

namespace FR.Droid.Views.Principal
{
    [Activity(Label = "FR - Par¨¢metros", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, ConfigurationChanges = Android.Content.PM.ConfigChanges.KeyboardHidden | Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.ScreenSize)]
    public class MenuParametrosViewX : Activity
    {
        ListView listaOpciones;

        #region lyfecycle

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.MenuParametrosX);

            MenuParametrosAdapter adapter = new MenuParametrosAdapter(this);
            listaOpciones =FindViewById<ListView>(Resource.Id.listaOpciones);
            listaOpciones.Adapter = adapter;
            
        }

        protected override void OnStart()
        {
            base.OnStart();            
        }

        protected override void OnPause()
        {
            listaOpciones.ItemClick -= listaOpciones_ItemClick;
            base.OnPause();
        }

        protected override void OnResume()
        {
            listaOpciones.ItemClick += listaOpciones_ItemClick;
            Softland.ERP.FR.Mobile.App.VerificarConexionBaseDatos(Util.cnxDefault());
            Softland.ERP.FR.Mobile.App.setCurrentActivity(this);
            base.OnResume();
        }

        #region eventos

        void listaOpciones_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            switch (listaOpciones.GetItemIdAtPosition(e.Position))
            {
                case 0:
                    {
                        var intent = new Intent(this, typeof(MainMenuViewX));
                        intent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.SingleTop);
                        StartActivity(intent);
                        break;
                    }
                
                default:
                    ShowMessage sm = new ShowMessage(this, "Opci¨®n inv¨¢lida.",false);
                    RunOnUiThread(() => sm.Mostrar());       
                    break;
            }
        }

        public override void OnBackPressed()
        {
            var intent = new Intent(this, typeof(MainMenuViewX));
            intent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.SingleTop);
            this.StartActivity(intent);
        }

        #endregion


        #endregion
    }
}