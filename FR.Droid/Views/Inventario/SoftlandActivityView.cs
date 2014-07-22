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

namespace FR.Droid.Views
{
    /// <summary>
    /// esta clase extiene a MvxBindingActivityView, pues lleva el control del CurrentActivity
    /// que es necesario para implementar el despliegue de los MessageBoxes en Android
    /// Otra opcion para no heredar de esta clase, es implementar la funcion
    /// OnResume, de manera identica a como se muestra en esta clase
    /// </summary>
    /// <typeparam name="TViewModel"></typeparam>
    public abstract class ExtendedActivityView<TViewModel> : MvxBindingActivityView<TViewModel> where TViewModel : Cirrious.MvvmCross.ViewModels.MvxViewModel, Cirrious.MvvmCross.Interfaces.ViewModels.IMvxViewModel
    {
        protected override void OnResume()
        {
            base.OnResume();
            Softland.ERP.FR.Mobile.App.setCurrentActivity(this);
        }

        // http://stackoverflow.com/questions/11411395/how-to-get-current-foreground-activity-context-in-android
        // se usa un static, aunque parece que hay una mejor forma de hacerlo, ver opcion que cambian App y Activity
        // codigo comentado

    //    protected Softland.ERP.FR.Mobile.App mMyApp;

    //    protected MvxBindingActivityView()
    //    {
    //        Android.Content.Context
    //        mMyApp = (Softland.ERP.FR.Mobile.App)ApplicationContext();
    //    }
    
    //protected void onResume() {
    //    super.onResume();
    //    mMyApp.setCurrentActivity(this);
    //}
    //protected void onPause() {
    //    clearReferences();
    //    super.onPause();
    //}
    //protected void onDestroy() {        
    //    clearReferences();
    //    super.onDestroy();
    //}

    //private void clearReferences(){
    //    Activity currActivity = mMyApp.getCurrentActivity();
    //    if (currActivity != null && currActivity.equals(this))
    //        mMyApp.setCurrentActivity(null);
    //}

        /// <summary>
        /// permite cambiar el comportamiento del BackButton de Android
        /// Pues de otra manera no hay posibilidad de hacer validaciones cuando el usuario le da Back y se sale de la pantalla
        /// 
        /// Cuando se desea hacer eso, lo correcto es asignar  OverrideBackButton = true;
        /// y en el ViewModel redefinir CanClose para que haga la validacion correspondiente
        /// y en el DoClose hacer operaciones de cierre (si las hay) y no olvidar invocar base.DoClose() 
        /// para que efectivamente la pantalla salga
        /// </summary>
        private bool overrideBackButton = false;
        public bool OverrideBackButton { 
            get { return overrideBackButton; }
            set { overrideBackButton = value;  }
        }
        /// <summary>
        /// se invoca el close command directamente cuando presionan back, y de esta manera logramos que
        /// se ejecute DoClose() si CanClose()!
        /// pues el back no está ejecutandolos
        /// </summary>
        /// <param name="keyCode"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        public override bool OnKeyDown(Keycode keyCode, KeyEvent e)
        {
            if (this.OverrideBackButton)
            {
                if (keyCode == Keycode.Back)
                {
                    this.ViewModel.CloseCommand.Execute(null);
                    return true;
                }
            }
            return base.OnKeyDown(keyCode, e);
        }
    }
}