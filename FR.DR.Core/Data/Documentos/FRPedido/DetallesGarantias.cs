using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SQLiteBase;
using Softland.ERP.FR.Mobile.Cls.AccesoDatos;
using Softland.ERP.FR.Mobile.Cls.FRArticulo;
using Softland.ERP.FR.Mobile.Cls.FRCliente;
using System.Collections;

namespace Softland.ERP.FR.Mobile.Cls.Documentos.FRPedido
{
    public class DetallesGarantias
    {
        public DetallesGarantias(string compania, string zona, string numero, int nivelPrecio, string bodega, TipoPedido tipo)
        {
            this.encabezado.Compania = compania;
            this.encabezado.Zona = zona;
            this.encabezado.Numero = numero;
            this.NivelPrecio = nivelPrecio;
            this.Tipo = tipo;
            this.Bodega = bodega;
        }

        public DetallesGarantias() { }

        #region Propiedades

        private int lineasBonificadas;

        public int LineasBonificadas
        {
            get 
            {
                lineasBonificadas = 0;
                foreach (DetalleGarantia detalle in this.Lista)
                {
                    if (detalle.LineaBonificada != null)
                        lineasBonificadas++;
                    if (detalle.LineaBonificadaAdicional != null)
                        lineasBonificadas++;
                }
                return lineasBonificadas;

            }

        }

        private List<DetalleGarantia> lista = new List<DetalleGarantia>();
        /// <summary>
        /// Detalles de Pedido
        /// </summary>
        public List<DetalleGarantia> Lista
        {
            get { return lista; }
            set { lista = value; }
        }

        private TipoPedido tipo = TipoPedido.Prefactura;

        public TipoPedido Tipo
        {
            get { return tipo; }
            set { tipo = value; }
        }
        private string bodega;

        public string Bodega
        {
            get { return bodega; }
            set { bodega = value; }
        }
        private Encabezado encabezado = new Encabezado();

        public Encabezado Encabezado
        {
            get { return encabezado; }
            set { encabezado = value; }
        }

        private int nivelPrecio = -1;

        public int NivelPrecio
        {
            get { return nivelPrecio; }
            set { nivelPrecio = value; }
        }

        //private decimal totalArticulos;
        public decimal TotalArticulos
        {
            get
            {
                decimal totalArticulos = decimal.Zero;

                foreach (DetalleGarantia detalle in lista)
                {
                    totalArticulos += detalle.UnidadesAlmacen;
                    if (detalle.LineaBonificada != null)
                        totalArticulos += detalle.LineaBonificada.UnidadesAlmacen;
                    if (detalle.LineaBonificadaAdicional != null)
                        totalArticulos += detalle.LineaBonificadaAdicional.UnidadesAlmacen;
                }
                return totalArticulos;
                
            }
        }
        public int TotalLineas
        {
            get
            {
                return lista.Count + LineasBonificadas;
            }
        }

       
        //LDS 17/05/2007		
        /// <summary>
        /// Organiza los detalles del pedido tal y como se almacenan en la base de datos de la pocket.
        /// </summary>
        /// <returns>
        /// Durante el pedido o cuando se carga el pedido los detalles pueden contener líneas bonificadas,
        /// por lo que se vuelven a acomodar.
        /// </returns>
        public ArrayList ExploteLineas()
        {
            ArrayList detalles = new ArrayList();
            detalles.Clear();

            foreach (DetalleGarantia detalle in this.Lista)
            {
                detalles.Add(detalle);
                if (detalle.LineaBonificada != null)
                    detalles.Add(detalle.LineaBonificada);
                if (detalle.LineaBonificadaAdicional != null)
                    detalles.Add(detalle.LineaBonificada);
            }
            return detalles;
        }

        #endregion 

        #region Logica

        #region Detalle de Devolucion

        public bool Vacio()
        {
            return (lista.Count == 0);
        }

        public DetallesGarantias Buscar(CriterioArticulo criterio, string valor, bool exacto, bool incluirBonificados)
        {
            DetallesGarantias coincidencias = new DetallesGarantias();
            coincidencias.Encabezado = this.encabezado;
            coincidencias.Lista.Clear();

            foreach (DetalleGarantia detalle in lista)
            {
                if (detalle.Articulo.BusquedaDesconectada(criterio, valor, exacto) )
                {
                    //Total Lineas + sumar ...
                    coincidencias.Lista.Add(detalle);
                }
                //if (detalle.LineaBonificada != null && incluirBonificados)
                //{
                //    if (detalle.LineaBonificada.Articulo.BusquedaDesconectada(criterio, valor, exacto))
                //    {
                //        coincidencias.lineasBonificadas++;
                //        coincidencias.Lista.Add(detalle.LineaBonificada);
                //    }
                //}
                //if (detalle.LineaBonificadaAdicional != null && incluirBonificados)
                //{
                //    if (detalle.LineaBonificadaAdicional.Articulo.BusquedaDesconectada(criterio, valor, exacto))
                //    {
                //        coincidencias.lineasBonificadas++;
                //        coincidencias.Lista.Add(detalle.LineaBonificadaAdicional);
                //    }
                //}
            }
            return coincidencias;
        }
        public DetalleGarantia Buscar(string articulo)
        {
            foreach (DetalleGarantia detalle in lista)
            {
                if (detalle.Articulo.Codigo.Equals(articulo))
                    return detalle;
            }
            return null;
        }

        /// <summary>
        /// Busca la linea de descuento
        /// </summary>
        /// <param name="articulo">Articulo</param>
        /// <returns>Retorna la linea, NULL si no tiene descuento</returns>
        public DetalleGarantia BuscarDescuento(string articulo, out int indice)
        {
            indice = 0;

            foreach (DetalleGarantia detalle in lista)
            {
                if (detalle.Articulo.Codigo.Equals(articulo) && detalle.Descuento != null) //&& detalle.Descuento.Monto > 0 && detalle.MontoDescuento > 0)
                    return detalle;

                indice++;
            }
            return null;
        }

        //ABC 35137
        public DetalleGarantia BuscarBonificado(string articulo, out int indice)
        {
            indice = 0;

            foreach (DetalleGarantia detalle in this.lista)
            {
                if (detalle.Articulo.Codigo.Equals(articulo) && detalle.LineaBonificada != null)
                {
                    return detalle.LineaBonificada;
                }
                indice++;
            }

            return null;
        }

        //ABC 35137
        public void CambiaCantidadBonificar(int indice, decimal cantidadAlmacen, decimal cantidadDetalle)
        {
            Lista[indice].LineaBonificada.UnidadesAlmacen = cantidadAlmacen;
            Lista[indice].LineaBonificada.UnidadesDetalle = cantidadDetalle;
        }

        public DetalleGarantia CambiarDescuento(int indice, decimal descuento, ClienteCia cliente, decimal descuentoGeneral)
        {
            lista[indice].Descuento.Monto = descuento;
            lista[indice].CalcularImpuestos(cliente.ExoneracionImp1,cliente.ExoneracionImp2, descuentoGeneral);
            lista[indice].CalcularMontoDescuento();
            lista[indice].CalcularMontos(cliente, false);
            return lista[indice];
        }

        public int BuscarPos(string articulo)
        {
            int pos = -1;
            int cont = 0;
            foreach (DetalleGarantia detalle in lista)
            {
                if (detalle.Articulo.Codigo.Equals(articulo))
                {
                    pos = cont;
                    break;
                }
                cont++;
            }
            return pos;
        }
        public DetalleGarantia Eliminar(string articulo)
        {
            int pos = -1;
            int cont = 0;
            DetalleGarantia detalleEliminado = null;

            foreach (DetalleGarantia detalle in lista)
            {
                if (detalle.Articulo.Codigo.Equals(articulo))
                {
                    pos = cont;
                    break;
                }
                cont++;
            }
            if (pos != -1)
            {
                detalleEliminado = lista[pos];
                lista.RemoveAt(pos);
            }
            else
                throw new Exception("No se pudo eliminar el detalle de la Garantía.");

            return detalleEliminado;
        }

        
        #endregion
        #endregion

        #region Acceso Datos

        /// <summary>
        /// Carga los detalles de la devolucion desde la base datos.
        /// </summary>
        public void Obtener(ClienteCia cliente, decimal porcentajeDescuentoGeneral)
        {
            lista.Clear();
            SQLiteDataReader reader = null;

            string sentencia =
                " SELECT D.COD_ART,D.CNT_MAX,D.MON_TOT,D.MON_PRC_MX,D.MON_PRC_MN,D.CNT_MIN " +// Articulo.CAMPOS_ARTICULO +
                @" FROM " + Table.ERPADMIN_alFAC_DET_GARANTIA  + " D " + //Articulo.INNER_JOIN +
                @" WHERE D.NUM_GAR = @CONSECUTIVO " +
                @" AND   UPPER(D.COD_CIA) = @COMPANIA " +
                @" ORDER BY NUM_LIN" ;
            
            SQLiteParameterList parametros = new SQLiteParameterList(new SQLiteParameter[] {
                GestorDatos.SQLiteParameter("@COMPANIA",SqlDbType.NVarChar,Encabezado.Compania.ToUpper()),
                GestorDatos.SQLiteParameter("@CONSECUTIVO",SqlDbType.NVarChar, Encabezado.Numero)});
            try
            {
                reader = GestorDatos.EjecutarConsulta(sentencia, parametros);

                while (reader.Read())
                {
                    DetalleGarantia detalle = new DetalleGarantia();
                    detalle.Articulo.Codigo = reader.GetString(0);
                    detalle.Articulo.Compania = Encabezado.Compania;
                    
                    detalle.Articulo.Cargar();
                    detalle.Articulo.Bodega.Codigo = this.Bodega;
                    detalle.UnidadesAlmacen = reader.GetDecimal(1);
                    detalle.UnidadesDetalle = reader.GetDecimal(5);
                    detalle.MontoTotal = reader.GetDecimal(2);
                    detalle.Precio = new Precio(reader.GetDecimal(3), reader.GetDecimal(4));

                    if (detalle.EsBonificada && lista.Count > 0)
                        lista[lista.Count - 1].LineaBonificada = detalle;
                    else if (detalle.EsBonificadaAdicional && lista.Count > 0 && Garantias.BonificacionAdicional)
                        lista[lista.Count - 1].LineaBonificadaAdicional = detalle;
                    else //No es bonificada
                    {
                        //Calculamos nuevamente los impuestos
                        //detalle.CalcularImpuestos(cliente.ExoneracionImp1, cliente.ExoneracionImp2, porcentajeDescuentoGeneral);

                        lista.Add(detalle);                        
                    }

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }
        }

        //ABC 36136
        /// <summary>
        /// Funcion que carga los detalles de un pedido historico.
        /// </summary>
        /// <returns></returns>
        public void ObtenerDetallesHistoricoGarantia()
        {
            SQLiteDataReader reader = null;
            lista.Clear();

            string sentencia =
                " SELECT DISTINCT DET.COD_ART,DET.NUM_LN,DET.EST_BON,DET.CANT_FAC," +
                " DET.CANT_DEV,DET.MONT_LN,DET.PRE_UNI " +
                " FROM "+ Table.ERPADMIN_alFAC_DET_HIST_FAC  + " DET " +
                " WHERE UPPER(DET.NUM_FAC) =  @CONSECUTIVO" +
                " AND UPPER(DET.COD_CIA) = @COMPANIA" +
                " ORDER BY NUM_LN";

            SQLiteParameterList parametros = new SQLiteParameterList(new SQLiteParameter[] {
                GestorDatos.SQLiteParameter("@COMPANIA",SqlDbType.NVarChar,Encabezado.Compania.ToUpper()),
                GestorDatos.SQLiteParameter("@CONSECUTIVO",SqlDbType.NVarChar, Encabezado.Numero.ToUpper())});
            try
            {
                reader = GestorDatos.EjecutarConsulta(sentencia, parametros);

                while (reader.Read())
                {
                    DetalleGarantia det = new DetalleGarantia();

                    det.Articulo.Codigo = reader.GetString(0);
                    det.Articulo.Compania = Encabezado.Compania;                    
                    det.NumeroLinea = reader.GetInt32(1);
                    //LDA
                    if (!reader.IsDBNull(2))
                    {
                        if (reader.GetString(2).Equals("B"))
                            det.EsBonificada = true;
                        if (reader.GetString(2).Equals("A"))
                            det.EsBonificada = false;
                    }
                    //det.EsBonificada = !reader.IsDBNull(2);
                    det.CantidadFacturada = reader.GetDecimal(3);
                    det.CantidadDevuelta = reader.GetDecimal(4);
                    det.MontoTotal = reader.GetDecimal(5);

                    //Define Informacion del Articulo
                    det.Articulo.Cargar();
                    det.Articulo.PrecioMaximo = reader.GetDecimal(6);
                    det.Articulo.Precio.CalcularPrecio(det.Articulo.UnidadEmpaque);

                    if (det.EsBonificada && lista.Count > 0)
                        lista[lista.Count - 1].LineaBonificada = det;
                    else
                        lista.Add(det);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }
        }

        private void Guardar(DetalleGarantia detalle)
        {            
            string COD_ART_RFR = string.Empty;
            //string tipoBonif = "0";            

            string sentencia =
                "INSERT INTO " + Table.ERPADMIN_alFAC_DET_GARANTIA +
                "        ( COD_CIA,  NUM_GAR,  LST_PRE,  NUM_LIN, COD_ART,CNT_MAX,CNT_MIN,MON_TOT, MON_PRC_MX,  MON_PRC_MN) " +
                " VALUES (@COD_CIA, @NUM_GAR, @NVL_PRE, @NUM_LN, @COD_ART,@CNT_MAX,@CNT_MIN, @MON_TOT, @MON_PRC_MX, @MON_PRC_MN)";

            
            //No Bonifica por que es depósito grarantía
            //if (!detalle.EsBonificada && !detalle.EsBonificadaAdicional)
              //  POR_DSC_AP = detalle.CalcularPorcentajeDescuento();

            //if (detalle.EsBonificada)
            //{
            //    tipoBonif = DetalleGarantia.BONIFICADA;
            //    COD_ART_RFR = (detalle.NumeroLinea - 1).ToString();               
            //}
            //else if (detalle.EsBonificadaAdicional)
            //{
            //    tipoBonif = DetalleGarantia.BONIFICADAADICIONAL;
            //    COD_ART_RFR = (detalle.NumeroLinea - 1).ToString();
            //}
            //else
            //    tipoBonif = DetalleGarantia.NOBONIFICADA;

            SQLiteParameterList parametros = new SQLiteParameterList(new SQLiteParameter[] {
                    GestorDatos.SQLiteParameter("@COD_CIA",SqlDbType.NVarChar, encabezado.Compania),
                    GestorDatos.SQLiteParameter("@NUM_GAR",SqlDbType.NVarChar, encabezado.Numero),
                    GestorDatos.SQLiteParameter("@NVL_PRE",SqlDbType.Int , nivelPrecio),
                    GestorDatos.SQLiteParameter("@NUM_LN", SqlDbType.Int,detalle.NumeroLinea),
                    GestorDatos.SQLiteParameter("@COD_ART", SqlDbType.NVarChar,detalle.Articulo.Codigo),
                    GestorDatos.SQLiteParameter("@CNT_MAX",SqlDbType.Decimal,detalle.UnidadesAlmacen),
                    GestorDatos.SQLiteParameter("@CNT_MIN",SqlDbType.Decimal,detalle.UnidadesDetalle),
                    GestorDatos.SQLiteParameter("@MON_TOT",SqlDbType.Decimal,detalle.MontoTotal),                                       
                    GestorDatos.SQLiteParameter("@MON_PRC_MX",SqlDbType.Decimal,detalle.Precio.Maximo),   
                    GestorDatos.SQLiteParameter("@MON_PRC_MN",SqlDbType.Decimal,detalle.Precio.Minimo)
                    });

            int linea = GestorDatos.EjecutarComando(sentencia, parametros);
            if (linea != 1)
                throw new Exception("No se puede guardar el detalle '" + detalle.Articulo.Codigo + "' de la garantía '" + encabezado.Numero + "'.");
  
        }
        
        public void Guardar()
        {
            int contador = 0;
            //Primero se guardan los detalles
            foreach (DetalleGarantia detalle in this.lista)
            {          
                detalle.NumeroLinea = ++contador;
                Guardar(detalle);

                //Si es facturado
                if (Tipo == TipoPedido.Factura)
                {
                    if(this.Bodega!= string.Empty)
                        detalle.Articulo.Bodega.Codigo = this.Bodega;
                    detalle.DisminuirExistencia();
                }
            }
        }
        
        public void Eliminar()
        {
            string sentencia =
                " DELETE FROM " + Table.ERPADMIN_alFAC_DET_GARANTIA +
                " WHERE COD_CIA = @COD_CIA" +
                " AND NUM_GAR = @NUM_GAR";

            SQLiteParameterList parametros = new SQLiteParameterList(new SQLiteParameter[] {
                GestorDatos.SQLiteParameter("@COD_CIA", SqlDbType.NVarChar, encabezado.Compania),
                GestorDatos.SQLiteParameter("@NUM_GAR", SqlDbType.NVarChar, encabezado.Numero)});

            int rows = GestorDatos.EjecutarComando(sentencia, parametros);

            if (rows < 1)
                throw new Exception("No se afectó ninguna línea de Garantía.");
        }        

        #endregion

        public void Anular(bool consignacion)
        {
            foreach (DetalleGarantia detalle in Lista)
            {
                if (!consignacion)
                {
                    detalle.DisminuirExistencia();
                    if (detalle.LotesLinea.Count > 0)
                    {
                        foreach (Lotes lote in detalle.LotesLinea)
                        {
                            lote.actualizarLote(detalle.Articulo.Compania, detalle.Articulo.Bodega.Codigo, detalle.Articulo.Codigo, true);
                        }
                    }
                }
                //if (detalle.LineaBonificada != null)
                //    detalle.LineaBonificada.DisminuirExistencia(); 
            }
        }

    }
}
