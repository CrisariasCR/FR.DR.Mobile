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
using System.Threading;
using Cirrious.MvvmCross.Binding.Droid.Views;
using Softland.ERP.FR.Mobile.ViewModels;
using Cirrious.MvvmCross.Commands;

namespace FR.Droid.Views.Cliente
{
    [Activity(Label = "FR - Sincronización de datos", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, ConfigurationChanges = Android.Content.PM.ConfigChanges.KeyboardHidden | Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.ScreenSize)]
    public class MenuDatosView : MvxBindingActivityView<MenuDatosViewModel>
    {
        Button btnCarga;
        Button btnEnvio;
        Button btnPurga;
        Button btnCobros;
        Button btnActualizar;
        private static bool bProcess;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            bProcess = false;
            //0: a typical phone screen (480x800 hdpi, etc).
            //1: a typical phone screen (240x320 ldpi, 320x480 mdpi).
            //2: a tweener tablet like the Streak (480x800 mdpi).
            //3: a 7” tablet (600x1024 mdpi).
            //4: a 10” tablet (720x1280 mdpi, 800x1280 mdpi, etc).
            switch (ResolutionsDisp.Resolucion(WindowManager))
            {
                case 0: SetContentView(Resource.Layout.MenuDatos); break;
                case 1: SetContentView(Resource.Layout.MenuDatos); break;
                case 2: SetContentView(Resource.Layout.MenuDatos); break;
                case 3: SetContentView(Resource.Layout.MenuDatos); break;
                case 4: SetContentView(Resource.Layout.MenuDatos); break;
                case 5: SetContentView(Resource.Layout.MenuDatos); break;
            }           

            btnCarga = FindViewById<Button>(Resource.Id.btnCarga);
            btnEnvio = FindViewById<Button>(Resource.Id.btnEnvio);
            btnPurga = FindViewById<Button>(Resource.Id.btnPurga);
            btnCobros = FindViewById<Button>(Resource.Id.btnCobros);
            btnActualizar = FindViewById<Button>(Resource.Id.btnActualizar);

            btnCarga.Click += new EventHandler(ProgressCarga);
            btnEnvio.Click += new EventHandler(ProgressEnvio);
            btnPurga.Click += new EventHandler(ProgressPurga);
            btnCobros.Click += new EventHandler(Cobros);
            btnActualizar.Click += new EventHandler(Actualizar);            

        }

        protected override void OnViewModelSet()
        {           
            
        }

        protected override void OnStart()
        {
            ViewModel.Contexto = this;
            base.OnStart();            
            if (!bProcess)
            {
                Softland.ERP.FR.Mobile.App.VerificarConexionBaseDatos(Util.cnxDefault());
            }
            Softland.ERP.FR.Mobile.App.setCurrentActivity(this);
        }

        private void ProgressCarga(object sender, EventArgs args)
        {
          var progressDialog = Android.App.ProgressDialog.Show(this, "Cargando", "Por favor espere...", true);
          bProcess = true;
		  new Thread(new ThreadStart(delegate
	        {
					Thread.Sleep(5000);
                    ViewModel.CargaDatos();
					//RunOnUiThread(() => progressDialog.Hide());
                    RunOnUiThread(delegate
                    {
                        bProcess = false;
                        progressDialog.Hide();
                    });

			})).Start();
        }

        private void ProgressEnvio(object sender, EventArgs args)
        {
            var progressDialog = Android.App.ProgressDialog.Show(this, "Enviando", "Por favor espere...", true);
            bProcess = true;
            new Thread(new ThreadStart(delegate
            {
                Thread.Sleep(5000);
                ViewModel.DescargaDatos();
                //RunOnUiThread(() => progressDialog.Hide());
                RunOnUiThread(delegate
                {
                    bProcess = false;
                    progressDialog.Hide();
                });

            })).Start();
        }

        private void ProgressPurga(object sender, EventArgs args)
        {
            var progressDialog = Android.App.ProgressDialog.Show(this, "Purgando", "Por favor espere...", true);
            bProcess = true;
            new Thread(new ThreadStart(delegate
            {
                Thread.Sleep(5000);
                ViewModel.PugarDatos(false, ViewModel.Contexto, true);
                //RunOnUiThread(() => progressDialog.Hide());
                RunOnUiThread(delegate
                {
                    bProcess = false;
                    progressDialog.Hide();
                });

            })).Start();
        }

        private void Cobros(object sender, EventArgs args)
        {
            ViewModel.Cobros();
        }

        private void Actualizar(object sender, EventArgs args)
        {
            ViewModel.Actualizar();
        }


    }
}