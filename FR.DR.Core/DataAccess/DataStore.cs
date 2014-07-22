using System;
using System.Net;
using System.Windows;
//using System.Windows.Controls;

using Softland.ERP.FR.Mobile.Cls.FRArticulo;

using Cirrious.MvvmCross.Interfaces.ServiceProvider;

using FR.Core.Model;
using System.Data.SQLiteBase;

namespace FR.WP.Core.DataAccess
{
    public class DataStore : IMvxServiceConsumer
    {
#pragma warning disable 649

        #region Constructor y Propiedades

        SQLiteConnection cnx;

        private IObservableCollection<Articulo> _articulos;
        public IObservableCollection<Articulo> Articulos
        {
            get
            {
                Cargar();
                return _articulos; 
            }
        }

        public DataStore(SQLiteConnection conn)
        {
            cnx = conn;
        }

        #endregion


        #region Metodos

        private void Cargar()
        {

        }

        #endregion
    }
}
