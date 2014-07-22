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
using Softland.ERP.FR.Mobile.Cls;
using Softland.ERP.FR.Mobile.Cls.Corporativo;
using Softland.ERP.FR.Mobile.Cls.Sincronizar;
using Softland.ERP.FR.Mobile.UI;

namespace Softland.ERP.FR.Mobile.ViewModels
{
    public class SincroDesatendida
    {
        public Android.Content.Context Contexto { get; set; }

        public SincroDesatendida(Android.Content.Context ctx)
        {
            this.Contexto = ctx;
            GestorSincronizar.sincroDesatendida = true;
        }

        #region Purga de datos
        /// <summary>
        /// Purga los documentos asociados a la HH de acuerdo con la cantidad de días
        /// y el código de la ruta.
        /// </summary>
        /// <param name="utilizarCantidadDias"></param>
        /// <returns></returns>
        public bool PugarDatos(bool utilizarCantidadDias, Android.Content.Context contexto, bool soloPurga)
        {
            bool resultado = false;
            int cantidadDias = utilizarCantidadDias ? FRmConfig.PurgarCantidadDias : -1;

            GlobalUI.Rutas = Ruta.ObtenerRutas();
            foreach (Ruta ruta in Ruta.ObtenerRutas())
            {
                resultado = GestorSincronizar.PurgarDatosDesatendida(ruta.Codigo, cantidadDias, utilizarCantidadDias, contexto, soloPurga);
            }
            return resultado;
        }

        #endregion Purga de datos

        #region Descarga de datos

        public void DescargaDatos()
        {
            try
            {
                //estaEnviando = true;
                GestorSincronizar.sincroDesatendida = true;
                bool continuar = GestorSincronizar.DescargaDatosDesatendida(GlobalUI.Rutas, this.Contexto);
                System.Threading.Thread.Sleep(2000);
                if (continuar && FRmConfig.PurgarDocumentosAutomaticamente)
                {
                    //MessageBox.Show("Purgando documentos sincronizados. Espere por favor ...");
                    PugarDatos(true, this.Contexto, false);
                }
                GestorSincronizar.sincroDesatendida = false;
            }
            catch 
            {
                //Fallo
                GestorSincronizar.sincroDesatendida = false;
            }
        }

        #endregion Descarga de datos

    }
}