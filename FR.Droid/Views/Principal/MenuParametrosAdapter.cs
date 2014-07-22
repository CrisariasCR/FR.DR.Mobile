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

namespace FR.Droid.Views.Principal
{
    public class MenuParametrosAdapter : BaseAdapter
    {
        List<string> _parametrostList;
        Activity _activity;

        public MenuParametrosAdapter(Activity activity)
        {
            _activity = activity;
            FillParametros();
        }

        void FillParametros()
        {
            _parametrostList = new List<string>();
            _parametrostList.Add("Parámetros del Sistema");
            _parametrostList.Add("Consulta NCF");
            _parametrostList.Add("Parámetros del Dispositivo");
            _parametrostList.Add("Corporación");
            _parametrostList.Add("Parámetros de Impresión");
        }

        public override int Count
        {
            get { return _parametrostList.Count; }
        }

        public override Java.Lang.Object GetItem(int position)
        {
            // could wrap a Contact in a Java.Lang.Object
            // to return it here if needed
            return null;
        }

        public override long GetItemId(int position)
        {
            switch (_parametrostList[position])
            {
                case "Parámetros del Sistema": return 0;
                case "Consulta NCF": return 1;
                case "Parámetros del Dispositivo": return 2;
                case "Corporación": return 3;
                case "Parámetros de Impresión": return 4;
                default: return -1;
            }
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var view = convertView ?? _activity.LayoutInflater.Inflate(
                Resource.Layout.OpcionMenuItemX, parent, false);
            var opcionMenu = view.FindViewById<TextView>(Resource.Id.txtOpcionMenuParametro);
            opcionMenu.Text = _parametrostList[position];
            return view;
        }
    }
}