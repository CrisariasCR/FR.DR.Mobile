using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Softland.ERP.FR.Mobile.Cls.FRArticulo;
using System.Collections;
using System.Data.SQLiteBase;
using Softland.ERP.FR.Mobile.Cls.AccesoDatos;
using System.Data;
using EMF.Printing;

namespace Softland.ERP.FR.Mobile.Cls.Documentos.FRDevolucion
{

    /// <summary>
    /// Representa un detalle/Linea especifica de una devolucion
    /// </summary>
    public class DetalleDevolucion : DetalleLinea,IPrintable
    {
        #region Variables y Propiedades de instancia

        private Estado estado = Estado.NoDefinido;
        /// <summary>
        /// Estado del articulo devuelto
        /// </summary>
        public Estado Estado
        {
            get { return estado; }
            set { estado = value; }
        }

        private string lote;
        /// <summary>
        /// Indica el lote donde debe ser devuelto el artículo.
        /// </summary>
        public string Lote
        {
            get { return lote; }
            set { lote = value; }

        }
        private string observaciones = string.Empty;
        /// <summary>
        /// Observaciones que se hicieron al devolver el producto
        /// </summary
        public string Observaciones
        {
            get { return observaciones; }
            set { observaciones = value; }
        }
       
        #endregion

        #region Metodos de instancia


        #endregion

        #region BindData

        public string[] OrdenarItems(CriterioArticulo criterio)
        {
            string[] itemes = new string[6];

            itemes[0] = this.Articulo.ObtenerDato(criterio);
            switch (criterio)
            {
                case CriterioArticulo.Codigo:
                    itemes[1] = Articulo.Descripcion;
                    itemes[2] = UnidadesAlmacen.ToString("#0.00");
                    itemes[3] = UnidadesDetalle.ToString("#0.00");
                    itemes[4] = Estado.ToString();
                    itemes[5] = string.Empty;
                    break;
                case CriterioArticulo.Descripcion:
                    itemes[1] = Articulo.Codigo;
                    itemes[2] = UnidadesAlmacen.ToString("#0.00");
                    itemes[3] = UnidadesDetalle.ToString("#0.00");
                    itemes[4] = Estado.ToString();
                    itemes[5] = string.Empty;
                    break;
                case CriterioArticulo.Barras:
                case CriterioArticulo.Clase:
                case CriterioArticulo.Familia:
                    itemes[1] = Articulo.Descripcion;
                    itemes[2] = Articulo.Codigo;
                    itemes[3] = UnidadesAlmacen.ToString("#0.00");
                    itemes[4] = UnidadesDetalle.ToString("#0.00");
                    itemes[5] = Estado.ToString();
                    break;
            }
            return itemes;
        }
        #endregion

        #region Acceso Datos

        #endregion
        
        #region IPrintable Members

        public override string GetObjectName()
        {
            return "DETALLE_DEVOLUCION";
        }

        public override object GetField(string name)
        {
            switch (name)
            {
                case "ESTADO": return (estado == Estado.Bueno ? "B" : "M");
                case "NOTAS": return observaciones;
                case "LOTE": return lote;
                default:
                    return base.GetField(name);
            }

        }
        #endregion

    }


}
