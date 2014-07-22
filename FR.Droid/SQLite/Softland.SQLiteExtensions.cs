using System;
using System.Net;
using System.Windows;



using Sqlite3DatabaseHandle = System.IntPtr;
using Sqlite3Statement = System.IntPtr;
using System.Collections.Generic;

namespace System.Data.SQLite
{
    public class SQLiteDataReader : SQLiteBase.SQLiteDataReader
    {
        #region Atributos y Propiedades

        string[] keys;
        Sqlite3Statement Stmt;

        public SQLiteDataReader(Sqlite3Statement stm)
        {
            this.Stmt = stm;
            int CantColumnas = SQLite3.ColumnCount(Stmt);
            keys = new string[CantColumnas];

            for (int i = 0; i < CantColumnas; i++)
            {
                var name = SQLite3.ColumnName16(Stmt, i);
                Values.Add(name, null);
                keys[i] = name;
            }
        }

        public override int FieldCount
        {
            get
            {
                return Values.Count;
                //throw new Exception("SqliteDataReader FieldCount Not Implemented");
            }
            set
            {
            }
        }

        #endregion

        #region Metodos

        public override bool Read()
        {
            if (SQLite3.Step(this.Stmt) == SQLiteBase.SQLiteResult.Row)
            {
                for (int i = 0; i < Values.Count; i++)
                {
                    this[i] = SQLite3.ReadCol(Stmt, i);
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        public override void Close()
        {
            SQLite3.Finalize(Stmt);
        }

        #endregion Metodos

        #region Indexers

        public override object this[string Column]
        {
            get
            {
                return Values[Column];
            }
            set
            {
                Values[Column] = value;
            }
        }

        public override object this[int index]
        {
            get
            {

                return this.Values[keys[index]];
            }
            set
            {
                Values[keys[index]] = value;
            }
        }

        #endregion

        #region Getters

        public override int GetInt32(int pos)
        {
            return (int)this[pos];
        }

        public override long GetInt64(int pos)
        {
            return (long)this[pos];
        }

        public override short GetInt16(int pos)
        {
            return (short)this[pos];
        }

        public override string GetGuid(int pos) 
        {
            if (this[pos] == null)
                return null;
            return this[pos].ToString(); 
        }

        public override byte GetByte(int pos) 
        {
            return (byte)this[pos]; 
        }

        public override bool GetBoolean(int pos) 
        {
            return (bool)this[pos]; 
        }

        public override float GetFloat(int pos) 
        {
            return (float)this[pos]; 
        }

        public override double GetDouble(int pos)
        {
            return (double)this[pos];
        }

        public override string GetString(int pos)
        {
            if (this[pos] == null)
                return null;
            return this[pos].ToString();
        }

        public override object GetValue(int pos)
        {
            if (this[pos] == null)
                return string.Empty;
            return this[pos];
        }

        public override decimal GetDecimal(int pos)
        {
            return decimal.Parse(this[pos].ToString());
        }

        public override DateTime GetDateTime(int pos)
        {
            try
            {
                string date = GetString(pos);                
                DateTime time = Convert.ToDateTime(date);
                //return (DateTime)this[pos];
                return time;
            }
            catch
            {
                string date = GetString(pos);
                int pointIndex = date.IndexOf('.');
                if (pointIndex != -1)
                {
                    date = date.Substring(0, pointIndex);
                }
                DateTime time = Convert.ToDateTime(date);
                //return (DateTime)this[pos];
                return time;
            }
            
        }

        public override bool IsDBNull(int pos)
        {
            try
            {
                object st = GetString(pos);
                return string.IsNullOrEmpty((string)st);
                //return this[pos] == null || GetString(pos).Equals("NULL") || GetString(pos).Equals("null");
            }
            catch
            {
                return false;
            }
        }

        #endregion

        /// <summary>
        /// Obtiene el nombre de la columna i del reader
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public override string GetName(int i)
        {
            string name = SQLite3.ColumnName16(this.Stmt, i);

            return name;
        }

        
        /// <summary>
        /// Obtiene el nombre del tipo de la columna i del reader
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public override int GetDataTypeName(int i)
        {
            
            Type colType = SQLite3.GetColumnType(this.Stmt, i);
            if (colType == typeof(DateTime)) { return 4; }
            if (colType == typeof(bool)) { return 2; }
            if (colType == typeof(int)) { return 8; }
            if (colType == typeof(decimal)) { return 13; }
            if (colType == typeof(string)) { return 12; }
            //return colType.ToString();
            return 12;
        }



        public enum SqlDbType
        {
            BigInt = 0,
            Binary = 1,
            Bit = 2,
            Char = 3,
            DateTime = 4,
            Decimal = 5,
            Float = 6,
            Image = 7,
            Int = 8,
            Money = 9,
            NChar = 10,
            NText = 11,
            NVarChar = 12,
            Real = 13,
            UniqueIdentifier = 14,
            SmallDateTime = 15,
            SmallInt = 16,
            SmallMoney = 17,
            Text = 18,
            Timestamp = 19,
            TinyInt = 20,
            VarBinary = 21,
            VarChar = 22,
            Variant = 23,
            Xml = 25,
            Udt = 29,
            Date = 31,
            Time = 32,
        }
    } // SQLiteDataReader
}
