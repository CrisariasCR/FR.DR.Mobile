using System;
using System.Net;
using System.Windows;
//using System.Windows.Controls;

using Softland.ERP.FR.Mobile.Cls.FRArticulo;
using FR.Core.Model;

namespace FR.WP.Core.DataAccess
{
    public interface IDataStore
    {
        #region Articulos

        void CreateArticulo(Articulo Articulo);
        void UpdateArticulo(Articulo Articulo);
        void DeleteArticulo(string CodigoArticulo);
        Articulo GetArticulo(string CodigoArticulo);
        IObservableCollection<Articulo> Articulos { get; }

        #endregion
    }
}
