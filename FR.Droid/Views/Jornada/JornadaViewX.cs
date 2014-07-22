using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Net;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Locations;
using System.Threading.Tasks;
using System.Threading;
using Softland.ERP.FR.Mobile.Cls;
using Cirrious.MvvmCross.Binding.Droid.Views;
using Softland.ERP.FR.Mobile.Cls.FRCliente.FRJornada;
using FR.Droid.CustomViews;

namespace FR.Droid.Views
{
    [Activity(Label = "FR - Rubros de Jornada",NoHistory=true, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, ConfigurationChanges = Android.Content.PM.ConfigChanges.KeyboardHidden | Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.ScreenSize)]
    public class JornadaViewX : Activity
    {        
        private Jornada jornada;
        ImageButton btnAceptar;
        TextView lblUno, lblDos, lblTres, lblCuatro, lblCinco;
        EditText txtUno, txtDos, txtTres, txtCuatro, txtCinco;
        LinearLayout lyUno, lyDos, lyTres, lyCuatro, lyCinco;
        

        #region lifecycle

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.RubrosJornada);

            lblUno = FindViewById<TextView>(Resource.Id.lblUno);
            lblDos = FindViewById<TextView>(Resource.Id.lblDos);
            lblTres = FindViewById<TextView>(Resource.Id.lblTres);
            lblCuatro = FindViewById<TextView>(Resource.Id.lblCuatro);
            lblCinco = FindViewById<TextView>(Resource.Id.lblCinco);
            txtUno = FindViewById<EditText>(Resource.Id.txtUno);
            txtDos = FindViewById<EditText>(Resource.Id.txtDos);
            txtTres = FindViewById<EditText>(Resource.Id.txtTres);
            txtCuatro = FindViewById<EditText>(Resource.Id.txtCuatro);
            txtCinco = FindViewById<EditText>(Resource.Id.txtCinco);
            lyUno = FindViewById<LinearLayout>(Resource.Id.lyUno);
            lyDos = FindViewById<LinearLayout>(Resource.Id.lyDos);
            lyTres = FindViewById<LinearLayout>(Resource.Id.lyTres);
            lyCuatro = FindViewById<LinearLayout>(Resource.Id.lyCuatro);
            lyCinco = FindViewById<LinearLayout>(Resource.Id.lyCinco);
            btnAceptar = FindViewById<ImageButton>(Resource.Id.btnAceptar);
        }

        protected override void OnStart()
        {
            base.OnStart();            
        }

        protected override void OnPause()
        {
            btnAceptar.Click -= btnAceptar_Click;
            base.OnPause();
        }

        protected override void OnResume()
        {
            btnAceptar.Click += btnAceptar_Click;
            Softland.ERP.FR.Mobile.App.VerificarConexionBaseDatos(Util.cnxDefault());
            Softland.ERP.FR.Mobile.App.setCurrentActivity(this);
            base.OnResume();
        }        

        #endregion

        #region eventos

        public override void OnBackPressed()
        {
           //Do Nothing
        }

        void btnAceptar_Click(object sender, EventArgs e)
        {
            this.Aceptar();
        }

        #endregion

        #region métodos

        /// <summary>
        /// Carga Inicial
        /// </summary>
        public void CargaInicial()
        {
            jornada = new Jornada();
            if (string.IsNullOrEmpty(FRdConfig.Rubro1Jornada))
            {
                lyUno.Visibility = ViewStates.Gone;
            }
            else
            {
                lblUno.Text = FRdConfig.Rubro1Jornada;
            }

            if (string.IsNullOrEmpty(FRdConfig.Rubro2Jornada))
            {
                lyDos.Visibility = ViewStates.Gone;
            }
            else
            {
                lblDos.Text = FRdConfig.Rubro2Jornada;
            }

            if (string.IsNullOrEmpty(FRdConfig.Rubro3Jornada))
            {
                lyTres.Visibility = ViewStates.Gone;
            }
            else
            {
                lblTres.Text = FRdConfig.Rubro3Jornada;
            }

            if (string.IsNullOrEmpty(FRdConfig.Rubro4Jornada))
            {
                lyCuatro.Visibility = ViewStates.Gone;
            }
            else
            {
                lblCuatro.Text = FRdConfig.Rubro4Jornada;
            }

            if (string.IsNullOrEmpty(FRdConfig.Rubro5Jornada))
            {
                lyCinco.Visibility = ViewStates.Gone;
            }
            else
            {
                lblCinco.Text = FRdConfig.Rubro5Jornada;
            }
        }

        /// <summary>
        /// Valida que los campos esten llenos
        /// </summary>
        /// <returns></returns>
        public bool Validar()
        {
            bool res = false;
            bool res1 = !lyUno.Visibility.Equals(ViewStates.Gone) || (!string.IsNullOrEmpty(txtUno.Text));
            bool res2 = !lyDos.Visibility.Equals(ViewStates.Gone) || (!string.IsNullOrEmpty(txtDos.Text));
            bool res3 = !lyTres.Visibility.Equals(ViewStates.Gone) || (!string.IsNullOrEmpty(txtTres.Text));
            bool res4 = !lyCuatro.Visibility.Equals(ViewStates.Gone) || (!string.IsNullOrEmpty(txtCuatro.Text));
            bool res5 = !lyCinco.Visibility.Equals(ViewStates.Gone) || (!string.IsNullOrEmpty(txtCinco.Text));
            res = res1 && res2 && res3 && res4 && res5;
            return res;
        }

        /// <summary>
        /// Guarda los valores
        /// </summary>
        public void Aceptar()
        {
            if (Validar())
            {
                AlertDialog.Builder alert = new AlertDialog.Builder(this);
                alert.SetTitle("Atención");
                alert.SetMessage("¿Desea continuar con estos rubros, no los podrá cambiar más tarde?");
                alert.SetPositiveButton("Si", delegate
                {
                    jornada.Guardar();
                    var intent = new Intent(this, typeof(MainMenuViewX));
                    intent.SetFlags(ActivityFlags.ReorderToFront);
                    StartActivity(intent);
                });
                alert.SetNegativeButton("No", delegate { return; });
                RunOnUiThread(() => { alert.Show(); });
                
            }
            else
            {                
                ShowMessage sm = new ShowMessage(this, "Debe llenar los rubros antes de continuar",false);
                RunOnUiThread(() => sm.Mostrar());  
            }
        }

        #endregion
    }
}