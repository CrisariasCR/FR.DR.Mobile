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

namespace FR.Droid.Views.Principal
{
    [Activity(Label = "FR - Parámetros", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, ConfigurationChanges = Android.Content.PM.ConfigChanges.KeyboardHidden | Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.ScreenSize)]
    public class MenuParametrosView : MvxBindingActivityView<MenuParametrosViewModel>
    {
        
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            //0: a typical phone screen (480x800 hdpi, etc).
            //1: a typical phone screen (240x320 ldpi, 320x480 mdpi).
            //2: a tweener tablet like the Streak (480x800 mdpi).
            //3: a 7” tablet (600x1024 mdpi).
            //4: a 10” tablet (720x1280 mdpi, 800x1280 mdpi, etc).
            switch (ResolutionsDisp.Resolucion(WindowManager))
            {
                case 0: SetContentView(Resource.Layout.MenuParametros); break;
                case 1: SetContentView(Resource.Layout.MenuParametros); break;
                case 2: SetContentView(Resource.Layout.MenuParametros); break;
                case 3: SetContentView(Resource.Layout.MenuParametros); break;
                case 4: SetContentView(Resource.Layout.MenuParametros); break;
                case 5: SetContentView(Resource.Layout.MenuParametros); break;
            }
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
    }
}