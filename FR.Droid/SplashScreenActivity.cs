using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

using Cirrious.MvvmCross.Droid.Platform;
using Cirrious.MvvmCross.Droid.Views;

namespace FR.Droid
{
    [Activity(Label = "Facturación de Rutero", Icon = "@drawable/icon")]
    public class SplashScreenActivity : MvxBaseSplashScreenActivity
    {
        public SplashScreenActivity()
            : base(Resource.Layout.SplashScreen)
        {
        }
    }
}

