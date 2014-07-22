using Softland.ERP.FR.Mobile.Cls.Cobro;
using Softland.ERP.FR.Mobile.Cls.Documentos.FRConsignacion;
using Softland.ERP.FR.Mobile.Cls.Documentos.FRDevolucion;
using Softland.ERP.FR.Mobile.Cls.Documentos.FRInventario;
using Softland.ERP.FR.Mobile.Cls.Documentos.FRPedido;
using Softland.ERP.FR.Mobile.Cls.AccesoDatos;
using System.Data.SQLiteBase;
using System;
using System.Data;
using Softland.ERP.FR.Mobile.Cls.FRCliente;
namespace Softland.ERP.FR.Mobile.Cls.Documentos
{
	/// <summary>
	/// RegistroDocumentos.
	/// Clase que contiene la logica necesaria para la validacion de multiples documentos
	/// </summary>
	public class GestorDocumentos
	{
		#region Enums
		
        /// <summary>
        /// Opciones que validan la generacion de documentos
        /// </summary>
		public enum OpcionMultiple : byte
		{
			NoPermitido=0,
			Permitido=1,
			Advertido=2
		}
        /// <summary>
        /// Gestiones de documentos a validar
        /// </summary>
		public enum Proceso 
		{
			Inventario,
			Cobro,
			Pedido,
			Devolucion,
			Consignacion
		}
		
		#endregion

		#region variables publicas

		/// <summary>
		/// Indica si se permitira el registro multiple de documentos
		/// Variables cargada del archivo de Configuracion
		/// </summary>
		public static OpcionMultiple PermiteRegistroMultiplesDocs;

        public static float tamanoLetra;
        
		#endregion
		
		#region metodos privados
		
        /// <summary>
		/// Retorna la tabla asociada en la base de datos con el proceso respectivo
		/// </summary>
		/// <param name="proceso">documento que se desea verificar</param>
		/// <returns>el nombre de la tabla respectiva al proceso solicitado</returns>
		private static Table TablaEncabezadoDocumento(Proceso proceso)
		{

			switch(proceso)
			{
				case Proceso.Cobro:
                    return Table.ERPADMIN_alCXC_DOC_APL;

				case Proceso.Consignacion:
                    return Table.ERPADMIN_alFAC_ENC_CONSIG;

				case Proceso.Devolucion:
					return Table.ERPADMIN_alFAC_ENC_DEV;

				case Proceso.Inventario:
                    return Table.ERPADMIN_alFAC_ENC_INV;
				case Proceso.Pedido:
                    return Table.ERPADMIN_alFAC_ENC_PED;
				default: 
					return Table.NO_DEF;
			}
		}
		#endregion

		#region metodos publicos
		/// <summary>
		/// Verificar si existen documentos previos, de tipo proceso
		/// </summary>
		/// <param name="proceso">Tipo de documento a verificar</param>
		/// <returns>Retorna verdadero si ya existe un documento registrado previamente, Falso en caso contrario</returns>
		public static bool ExisteDocumentoPrevio(Proceso proceso, Cliente cliente)
		{			
			SQLiteDataReader reader = null;
			//Se obtiene el encabezado del documento
			try
			{
				string numero = string.Empty;
                string sentencia =
                    "SELECT COUNT(*) FROM " + TablaEncabezadoDocumento(proceso) + " WHERE DOC_PRO IS NULL" +
                    " AND COD_CLT = @CLIENTE" +
                    " AND COD_ZON = @ZONA ";
				
				if(proceso.Equals(Proceso.Pedido))
					sentencia+= " AND ESTADO='N'";
				else if(proceso.Equals(Proceso.Devolucion))
					sentencia += " AND EST_DEV='A'";

                SQLiteParameterList parametros = new SQLiteParameterList(new SQLiteParameter[] {
                GestorDatos.SQLiteParameter("@CLIENTE",SqlDbType.NVarChar,cliente.Codigo),
                GestorDatos.SQLiteParameter("@ZONA",SqlDbType.NVarChar,cliente.Zona)});
					
				object valor = GestorDatos.EjecutarScalar(sentencia,parametros);
								
				if (valor != null && !(valor is DBNull))
					numero = valor.ToString();
				
				if(!numero.Equals(string.Empty))
				{					
					return Int32.Parse(numero)>0; 
				}
			}
			catch(Exception ex)
			{
				throw ex;
			}
			finally
			{
				if (reader != null)
					reader.Close();
			}
			return false;
		}	
		#endregion
    }
}
