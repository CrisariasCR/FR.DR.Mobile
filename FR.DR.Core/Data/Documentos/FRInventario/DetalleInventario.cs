using System;
using System.Data;
using System.Data.SQLiteBase;
using Softland.ERP.FR.Mobile.Cls.AccesoDatos;
using Softland.ERP.FR.Mobile.Cls.Documentos;
using System.Collections.Generic;
using Softland.ERP.FR.Mobile.Cls.FRArticulo;
using EMF.Printing;

namespace Softland.ERP.FR.Mobile.Cls.Documentos.FRInventario
{
    /// <summary>
    /// Representa un detalle/Linea especifica de un inventario
    /// </summary>
    public class DetalleInventario : DetalleDocumento,IPrintable
    {
        private decimal cantFacturadaTomaFisica = decimal.Zero;

        public decimal CantidadFacturadaTomaFisica
        {
            get { return cantFacturadaTomaFisica; }
            set { cantFacturadaTomaFisica = value; }
        }

        private decimal cantDiferenciasTomaFisica = decimal.Zero;

        public decimal DiferenciasTomaFisica
        {
            get { return cantDiferenciasTomaFisica; }
            set { cantDiferenciasTomaFisica = value; }
        }

        private string estado = string.Empty;

        public string Estado
        {
            get { return estado; }
            set { estado = value; }
        }

        public DetalleInventario()
        {
            Articulo = new Articulo();
        }

        /// <summary>
        /// Crea una linea de detalle de inventario
        /// </summary>
        /// <param name="articulo">Articulo de la linea</param>
        /// <param name="unidadesDetalle">unidades de detalle a inventariar del articulo</param>
        /// <param name="unidadesAlmacen">unidades de almacen a inventariar del articulo</param>
        public DetalleInventario(Articulo articulo, decimal unidadesDetalle, decimal unidadesAlmacen)
        {
            this.Articulo = articulo;
            this.UnidadesAlmacen = unidadesAlmacen;
            this.UnidadesDetalle = unidadesDetalle;

        }

        #region BindData

        /// <summary>
        /// Ordenar Datos del inventario por criterio para mostrar en los list view
        /// </summary>
        /// <param name="criterio"></param>
        /// <returns></returns>
        public string[] OrdenarItems(CriterioArticulo criterio)
        {
            //Caso: 32617 ABC 16/05/2008
            //Agregar Criterio de Busqueda Reporte Inventario FR
            string[] itemes = new string[6];

            itemes[0] = this.Articulo.ObtenerDato(criterio);
            switch (criterio)
            {
                case CriterioArticulo.Codigo:
                    itemes[1] = Articulo.Descripcion;
                    itemes[2] = UnidadesAlmacen.ToString("#0.00");
                    itemes[3] = UnidadesDetalle.ToString("#0.00");
                    itemes[4] = string.Empty;
                    break;
                case CriterioArticulo.Descripcion:
                    itemes[1] = Articulo.Codigo;
                    itemes[2] = UnidadesAlmacen.ToString("#0.00");
                    itemes[3] = UnidadesDetalle.ToString("#0.00");
                    itemes[4] = string.Empty;
                    break;
                case CriterioArticulo.Barras:
                case CriterioArticulo.Clase:
                case CriterioArticulo.Familia:
                    itemes[1] = Articulo.Descripcion;
                    itemes[2] = Articulo.Codigo;
                    itemes[3] = UnidadesAlmacen.ToString("#0.00");
                    itemes[4] = UnidadesDetalle.ToString("#0.00");
                    break;
            }
            return itemes;
        }
        #endregion

        #region IPrintable Members

        public override string GetObjectName()
        {
            return "LINEA_INVENTARIO";
        }

        public override object GetField(string name)
        {
            switch (name)
            {
                case "CANTIDADFACTURA": return this.CantidadFacturadaTomaFisica;
                case "DIFERENCIA": return this.DiferenciasTomaFisica;
                case "ESTADO": return this.Estado;                
                default: return base.GetField(name);
            }
            //return base.GetField(name);
        }
        #endregion

    }
}
