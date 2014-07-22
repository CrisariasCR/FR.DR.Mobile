using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Softland.ERP.FR.Mobile.Cls.FRArticulo;
using EMF.Printing;
using Softland.ERP.FR.Mobile.Cls.Corporativo;

namespace Softland.ERP.FR.Mobile.Cls.Documentos.FRInventario
{
    /// <summary>
    /// Representa la Gestion de inventarios para un cliente Multi cia
    /// </summary>
    public class Inventarios
    {
        #region Variables de la clase

        private List<Inventario> gestionados = new List<Inventario>();
        /// <summary>
        /// Inventarios gestionados para el cliente multi cia en cada compania
        /// </summary>
        public List<Inventario> Gestionados
        {
            get { return gestionados; }
            set { gestionados = value; }
        }

        #endregion

        #region Metodos de la logica del negocio

        /// <summary>
        /// Verifica si existen articulos gestionados
        /// </summary>
        /// <returns>existen articulos gestionados</returns>
        public bool ExistenArticulosGestionados()
        {
            foreach (Inventario inv in gestionados)
            {
                if (!inv.Detalles.Vacio())
                    return true;
            }
            return false; 
        }

        /// <summary>
        /// Guardar todos los inventarios gestionados
        /// </summary>
        public void Guardar()
        {
            foreach (Inventario inv in gestionados)
            {
                inv.HoraFin = DateTime.Now;
                inv.Guardar();
            }
        }

        /// <summary>
        /// Actualizar todos los inventarios gestionados
        /// </summary>
        public void Actualizar()
        {
            foreach (Inventario inv in gestionados)
            {
                inv.Actualizar();
            }
        }
    
        /// <summary>
        /// Busca una linea dentro de los detalles de los inventarios gestionados.
        /// </summary>
        /// <param name="articulo">Articulo a buscar.</param>
        /// <returns></returns>
        public DetalleInventario BuscarDetalleEnInventario(Articulo articulo)
        {
            Inventario inv = this.Buscar(articulo.Compania);
            if (inv != null)
                return inv.Detalles.Buscar(articulo.Codigo);
            else
                return null;
        }

        /// <summary>
        /// Busca un inventario dentro de una lista de inventarios.
        /// </summary>
        /// <param name="compania">Codigo de compania del inventario</param>
        /// <returns></returns>
        public Inventario Buscar(string compania)
        {
            foreach (Inventario inventario in gestionados)
            {
                if (inventario.Compania.Equals(compania))
                    return inventario;
            }
            return null;
        }

        /// <summary>
        /// Borra un inventario en gestión utilizando el codigo de la compania.
        /// </summary>
        /// <param name="compania">Codigo de compania del pedido</param>
        public void Borrar(string compania)
        {
            int pos = -1;
            int cont = 0;

            foreach (Inventario inventario in gestionados)
            {
                if (inventario.Compania.Equals(compania))
                {
                    pos = cont;
                    break;
                }
                cont++;
            }

            if (pos > -1)
                gestionados.RemoveAt(pos);
        }

        /// <summary>
        /// Metodo encargado de realizar la gestion del inventario y la gestion del
        /// detalle de inventario.
        /// </summary>
        /// </summary>
        /// <param name="articulo">articulo a gestionar</param>
        /// <param name="cliente">cliente asociado en la gestion</param>
        /// <param name="zona">zona asociada en la gestion</param>
        /// <param name="cantidadDetalle">unidades de detalle del articulo a gestionar</param>
        /// <param name="cantidadAlmacen">unidades de almacen del articulo a gestionar</param>
        public void Gestionar(Articulo articulo, string cliente, string zona, decimal cantidadDetalle, decimal cantidadAlmacen)
        {
            Inventario inventario = Buscar(articulo.Compania);
            //El inventario no existe	
            if (inventario == null)
            {
                //Si la cantidad es 0 ignore
                if (cantidadDetalle == 0 && cantidadAlmacen == 0)
                    return;

                //se crea un inventario nuevo
                inventario = new Inventario();
                inventario.Compania = articulo.Compania;
                inventario.Zona = zona;
                inventario.Cliente = cliente;
                inventario.Numero = ParametroSistema.ObtenerInventario(articulo.Compania, zona);

                if (inventario.Numero == string.Empty)
                    throw new Exception("No se obtuvo consecutivo de inventario.");

                inventario.HoraInicio = DateTime.Now;
                inventario.FechaRealizacion = DateTime.Now.Date;

                //Agregar el detalle
                inventario.Detalles.Gestionar(articulo, cantidadDetalle, cantidadAlmacen);
                gestionados.Add(inventario);

            }
            //El inventario existe
            else
            {
                //Se elimina el detalle
                if (cantidadAlmacen == 0 && cantidadDetalle == 0)
                {
                    inventario.Detalles.Eliminar(articulo.Codigo);
                    //Si No quedan detalles
                    if (inventario.Detalles.Vacio())
                        Borrar(inventario.Compania);
                }
                //Se actualiza las cantidades
                else
                    inventario.Detalles.Gestionar(articulo, cantidadDetalle, cantidadAlmacen);
            }
        }

        #endregion
    }
}
