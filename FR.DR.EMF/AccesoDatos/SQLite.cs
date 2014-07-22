//
// Copyright (c) 2009-2011 Krueger Systems, Inc.
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//

using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Reflection;
using System.Linq;
using System.Linq.Expressions;

namespace System.Data.SQLite {
    public class PrimaryKeyAttribute : Attribute {
    }

    public class AutoIncrementAttribute : Attribute {
    }

    public class IndexedAttribute : Attribute {
    }

    public class IgnoreAttribute : Attribute {
    }

    public class MaxLengthAttribute : Attribute {
        public int Value { get; private set; }

        public MaxLengthAttribute(int length)
        {
            Value = length;
        }
    }

    public class CollationAttribute : Attribute {
        public string Value { get; private set; }

        public CollationAttribute(string collation)
        {
            Value = collation;
        }
    }
}

namespace System.Data.SQLiteBase {

    public abstract class SQLiteConnection : IDisposable
    {

        #region Softland Code

        public abstract bool isOpen { get; }
        //public abstract SQLiteCommand CreateCommand(string cmdText);
        public abstract SQLiteCommand CreateCommand(string cmdText, SQLiteParameterList pars);

        public abstract SQLiteDataReader ExecuteDataReader(string query, SQLiteParameterList pars);

        public abstract object ExecuteScalar(string query);
        public abstract object ExecuteScalar(string query, SQLiteParameterList pars);
        public abstract object ExecuteNonQuery(string query);
        public abstract object ExecuteNonQuery(string query, SQLiteParameterList pars);

        public abstract SQLiteCommand CreateCommand();
        #endregion

        public string DatabasePath { get;private set; }

        public string DatabaseName 
        {
            get
            {
                if (DatabasePath != null)
                {
                    string[] arr = DatabasePath.Split('.', '\\', '/');
                    if (arr.Length > 1)
                    {
                        return arr[arr.Length - 1];
                    }
                }
                return null;
            }
        }

        public bool TimeExecution { get; set; }

        public bool Trace { get; set; }

        public SQLiteConnection(string databasePath)
        {
            DatabasePath = databasePath;
        }


        public abstract void Open();

        //public abstract int CreateTable<T>();
 
        public abstract SQLiteCommand CreateCommand(string cmdText, params object[] ps);

        public abstract int Execute(string query, params object[] args);

        //public abstract List<T> Query<T>(string query, params object[] args) where T : new();
        
        //public abstract IEnumerable<T> DeferredQuery<T>(string query, params object[] args) where T : new();
        
        public abstract List<object> Query(TableMapping map, string query, params object[] args);
        
        //public abstract IEnumerable<object> DeferredQuery(TableMapping map, string query, params object[] args);
        
        public abstract TableQuery<T> Table<T>() where T : new();
        
        //public abstract T Get<T>(object pk) where T : new();
        
        public bool IsInTransaction { get; protected set; }

        public abstract void BeginTransaction();
        
        public abstract void Rollback();
        
        public abstract void Commit();
        
        public abstract void RunInTransaction(Action action);
        
        public abstract int InsertAll(System.Collections.IEnumerable objects);
        
        public abstract int Insert(object obj);
        
        public abstract int Insert(object obj, Type objType);
        
        public abstract int Insert(object obj, string extra);
        
        public abstract int Insert(object obj, string extra, Type objType);
        
        public abstract int Update(object obj);
        
        public abstract int Update(object obj, Type objType);
        
        public abstract int Delete<T>(T obj);
        
        public void Dispose()
        {
            Close();
        }

        public abstract void Close();
    }

    public abstract class TableMapping {}

    public abstract class SQLiteCommand : IDisposable
    {
        protected internal SQLiteConnection _conn;

        public SQLiteCommand() { }
        public SQLiteCommand(SQLiteConnection cnx) { }
        public SQLiteCommand(string sentencia, SQLiteConnection cnx) {}
        public string CommandText { get; set; }
        public virtual int ExecuteNonQuery() { return -1; }
        public virtual SQLiteDataReader ExecuteDataReader() { return null; }
        public virtual object ExecuteScalar() { return null; }
        public abstract void Prepare(SQLiteParameterList pars);
        public abstract void Dispose();
    }

    public abstract class TableQuery<T> : IEnumerable<T> where T : new() {
        public virtual IEnumerator<T> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }

    #region Softland Code

    public abstract class SQLiteDataReader 
    {
        //METODOS
        public virtual bool Read() { throw new NotImplementedException(); }
        public virtual void Close() { throw new NotImplementedException(); }
        public virtual void NextResult() { throw new NotImplementedException(); }
        
        //INDEXERS
        public virtual object this[int index] { get { return null; } set { } }
        public virtual object this[string Column] { get { return null; } set { } }

        //GETTERS
        public virtual int GetInt32(int pos) { return 0; }
        public virtual long GetInt64(int pos) { return 0; }
        public virtual short GetInt16(int pos) { return 0; }
        public virtual string GetGuid(int pos) { return null; }
        public virtual byte GetByte(int pos) { return 0; }
        public virtual bool GetBoolean(int pos) { return false; }
        public virtual float GetFloat(int pos) { return 0; }
        public virtual double GetDouble(int pos) { return 0; }
        public virtual string GetString(int pos) { return null; }
        public virtual object GetValue(int pos) { return null; }
        public virtual decimal GetDecimal(int pos) { return -1; }
        public virtual DateTime GetDateTime(int pos) { return DateTime.Now; }
        public virtual bool IsDBNull(int pos) { return true; }

        //PROPIEDADES
        public virtual int FieldCount { get; set; }
        protected Dictionary<string, object> Values = new Dictionary<string, object>();

        public virtual string GetName(int pos) { throw new NotImplementedException(); }
        public virtual int GetDataTypeName(int pos) { throw new NotImplementedException(); }
    }

    public class SQLiteParameterList
    {
        public Dictionary<string, SQLiteParameter> Parametros = null;

        public SQLiteParameterList()
        {
            Parametros = new Dictionary<string, SQLiteParameter>();
        }

        public SQLiteParameterList(SQLiteParameter[] parameters): this()
        {
            foreach(SQLiteParameter p in parameters)
            {
                this.Parametros.Add(p.Name, p);
            }
        }

        public void Add(SQLiteParameter par)
        {
            this.Parametros.Add(par.Name, par);
        }
        public void Add(string name, object value)
        {
            if (name[0] != '@')
                name = "@" + name;
            SQLiteParameter p = new SQLiteParameter(name, value);
            this.Parametros.Add(name, p);
        }

        public void Add(string name, SqlDbType type, object value)
        {
            Add(name, value);
        }

        public void Remove(string name)
        {
            this.Parametros.Remove(name);
        }

        public void Clear()
        {
            this.Parametros.Clear();
        }

        public SQLiteParameter this[string name]
        {
            get
            {
                return Parametros[name];
            }
        }

        public SQLiteParameter this[int index]
        {
            get
            {
                // TODO!
                return null; // Parametros.get[index];
            }
        }
    }

    public class SQLiteDataAdapter
    {
        public SQLiteDataAdapter()
        {
        }
        public SQLiteDataAdapter(string s, SQLiteConnection cnx)
        {
        }

        public void FillSchema(DataSet schema, SchemaType st, string table)
        {
        }
    }

    #endregion Softland Code

    public class SQLiteParameter
    {
        public SQLiteParameter(string name, SqlDbType type, object value) : this(name, value)
        {
            this.Type = type;
        }

        public SQLiteParameter(string name, object value)
        {
            if (name[0] != '@')
                name = "@" + name;
            this.Name = name;
            this.Value = value;
        }
        public string Name { get; set; }
        public object Value { get; set; }
        public SqlDbType Type { get; set; }
    }

    //public class SQLiteParameters
    //{
    //    public List<object> Parameters;

    //    public void AddRange(Array array)
    //    {
    //        foreach (object element in array)
    //        {
    //            Parameters.Add(element);
    //        }
    //    }
    //}

    public enum SQLiteResult : int
    {
        OK = 0,
        Error = 1,
        Internal = 2,
        Perm = 3,
        Abort = 4,
        Busy = 5,
        Locked = 6,
        NoMem = 7,
        ReadOnly = 8,
        Interrupt = 9,
        IOError = 10,
        Corrupt = 11,
        NotFound = 12,
        TooBig = 18,
        Constraint = 19,
        Row = 100,
        Done = 101
    }

    public class SQLiteException : Exception
    {
        public int NativeError { get { return -1;  } }

        public SQLiteResult Result { get; private set; }

        protected SQLiteException(SQLiteResult r, string message)
            : base(message)
        {
            Result = r;
        }

        public static SQLiteException New(SQLiteResult r, string message)
        {
            return new SQLiteException(r, message);
        }
    }

    public static class SQLiteEngine
    {
        /// <summary>
        /// Crea un archivo con el nombre indicado en la ruta 
        /// </summary>
        /// <param name="databasePathName">ruta y nombre del archivo</param>
        public static void CreateDatabase(string databasePathName)
        {
            System.IO.File.Create(databasePathName).Dispose();
        }

        /// <summary>
        /// Compacta la BD indicada
        /// </summary>
        /// <param name="cnx"></param>
        public static void Compact(SQLiteConnection cnx)
        {
            cnx.ExecuteNonQuery("vacuum;");
        }
    }

}



