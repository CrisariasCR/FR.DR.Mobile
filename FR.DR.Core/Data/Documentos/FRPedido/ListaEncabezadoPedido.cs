using System;
using System.Collections;
//using System.Windows.Forms;
using Softland.ERP.FR.Mobile.Cls.Utilidad;

namespace Softland.ERP.FR.Mobile.Cls.Documentos.FRPedido
{
	/// <summary>
	/// Summary description for ListaEncabezadoPedido.
	/// </summary>
	public class ListaEncabezadoPedido: Hashtable
	{
#pragma warning disable 114
		//public ListView listbox;

		public ListaEncabezadoPedido(): base()
		{
			//
			// TODO: Add constructor logic here
			//
		}
		/// <summary>
		/// Metodo que busca en la lista de encabezados de pedidos un pedido
		/// </summary>
		
        public Pedido this [string pedido,string compania]
		{
			get
			{
				string Llave = pedido+""+compania;
				Pedido item = null;

				item = (Pedido)this[Llave];
				return item;
			}
		}
		
        /// <summary>
		/// Agrega el encabezado del pedido a la lista de pedidos
		/// </summary>
		/// <param name="encPed">objeto pedido a agregar</param>
		public void AgregaEncabezadoPedido(Pedido encPed)
		{
			string Llave = ""+encPed.Numero+""+encPed.Compania;
			this.Add(Llave,encPed);
		}

		/// <summary>
		/// Metodo que llena el listview que despliega la lista de encabezados del pedido
		/// </summary>
		public void FillListBox()
		{			
            //if (listbox!=null)
            //{
            //    if (listbox.Items.Count!=this.Count)
            //    {
            //        listbox.BeginUpdate();
            //        listbox.Items.Clear();

            //        IDictionaryEnumerator enumerator = this.GetEnumerator();

            //        while (enumerator.MoveNext())
            //        {
            //            string [] items = {
            //                                  ((Pedido)enumerator.Value).Compania,
            //                                  ((Pedido)enumerator.Value).Numero,
            //                                  ((Pedido)enumerator.Value).Estado.ToString(),
            //                                  GestorUtilitario.FormatNumero(((Pedido)enumerator.Value).MontoBruto),
            //                                  ((Pedido)enumerator.Value).FechaRealizacion.ToString("MM/dd/yyyy")
            //                              };
            //            ListViewItem item = new ListViewItem(items);
            //            listbox.Items.Add(item);	
            //        }
            //        listbox.EndUpdate();			
            //    }
            //}
		}

        private IDictionaryEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }


		/// <summary>
		/// Llama a la funcion que carga el arreglo de los pedidos.
		/// </summary>
		public ArrayList this [EstadoPedido estado, int num]
		{
			get
			{			
				return busquedaPorEstado(estado); 
			}
		}

		/// <summary>
		/// Metodo que se encarga de realizar
		/// la busqueda de los pedidos por estados.
		/// </summary>
		/// <param name="estado"></param>
		/// <returns>El arreglo con los pedidos que contengan este estado</returns>
		private ArrayList busquedaPorEstado(EstadoPedido estado)
		{

            //ArrayList busqueda = new ArrayList();;
            //IDictionaryEnumerator itemenumerator=this.GetEnumerator();
            //Pedido currentitem;
			
            //if(this.Keys.Count!=0)
            //{
            //    while(itemenumerator.MoveNext())
            //    {
            //        currentitem = (Pedido)itemenumerator.Value;

            //        if (currentitem.Estado == estado)
            //            busqueda.Add(currentitem);
            //    }
            //}

            //return busqueda;

            ArrayList busqueda = new ArrayList();

            foreach (Pedido current in this.Values)
            {
                if(current.Estado == estado)
                    busqueda.Add(current);
            }

            return busqueda;
		}


        public int Count { get; set; }
    }
}
