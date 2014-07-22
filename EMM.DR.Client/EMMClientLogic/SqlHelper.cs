using System;
using System.Data;
using EMM.DR.Client.EMMService;

namespace EMMClient.EMMClientLogic
{

		/// <summary>
	/// Summary description for IStatusForm.
	/// </summary>
    public static class SqlHelper
	{
        /// <summary>
        ///Descripcion
        ///Retorna una constante indicando la equivalencia entre tipos
        ///del framework y SQLCE
        ///
        ///Entradas
        ///ColumnType	Nombre del tipo de la columna
        ///
        ///Salidas
        ///SynchronizationLogic.SQLCEType Tipo equivalente en SQLCE
        ///
        /// </summary>
        public static SQLCEType GetSQLCEType(SqlDbType ColumnType)
        {
            switch (ColumnType)
            {
                case SqlDbType.Decimal:
                    return SQLCEType.db_real;

                case SqlDbType.Real:
                    return SQLCEType.db_numeric;

                case SqlDbType.DateTime:
                    return SQLCEType.db_datetime;

                case SqlDbType.Char:
                    return SQLCEType.db_nchar;

                case SqlDbType.TinyInt:
                    return SQLCEType.db_tinyint;

                case SqlDbType.SmallInt:
                    return SQLCEType.db_smallint;

                case SqlDbType.NVarChar:
                    return SQLCEType.db_nvarchar;

                case SqlDbType.Int:
                    return SQLCEType.db_int;

                case SqlDbType.BigInt:
                    return SQLCEType.db_bigint;

                case SqlDbType.Money:
                    return SQLCEType.db_money;

                case SqlDbType.Bit:
                    return SQLCEType.db_bit;

                case SqlDbType.Float:
                    return SQLCEType.db_float;

                case SqlDbType.NText:
                    return SQLCEType.db_ntext;

                case SqlDbType.NChar:
                    return SQLCEType.db_nchar;

                case SqlDbType.Binary:
                    return SQLCEType.db_binary;

                case SqlDbType.VarBinary:
                    return SQLCEType.db_varbinary;

                case SqlDbType.Image:
                    return SQLCEType.db_image;

                case SqlDbType.UniqueIdentifier:
                    return SQLCEType.db_uniqueidentifier;

                default:
                    return SQLCEType.db_nvarchar;
            }

        }
    }
}
