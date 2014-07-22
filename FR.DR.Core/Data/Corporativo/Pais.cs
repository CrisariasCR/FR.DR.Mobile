using System;
using System.Collections.Generic;
using System.Data.SQLiteBase;
using FR.DR.Core.Data.Corporativo;
using Softland.ERP.FR.Mobile.Cls.AccesoDatos;
using Softland.ERP.FR.Mobile.Cls.FRCliente;

namespace Softland.ERP.FR.Mobile.Cls.Corporativo
{
    /// <summary>
    /// Datos de un pais
    /// </summary>
    public class Pais
    {
        #region Variables y Propiedades de instancia

        private string codigo = string.Empty;
        /// <summary>
        /// Codigo de pais
        /// </summary>
        public string Codigo
        {
            get { return codigo; }
            set { codigo = value; }
        }

        private string divisionGeografica1 = string.Empty;
        /// <summary>
        /// Codigo de DivisionGeografica1
        /// </summary>
        public string DivisionGeografica1
        {
            get { return divisionGeografica1; }
            set { divisionGeografica1 = value; }
        }

        private string lbldivisionGeografica1 = string.Empty;
        /// <summary>
        /// Label de DivisionGeografica1
        /// </summary>
        public string LblDivisionGeografica1
        {
            get { return lbldivisionGeografica1; }
            set { lbldivisionGeografica1 = value; }
        }

        private string lbldivisionGeografica2 = string.Empty;
        /// <summary>
        /// Label de DivisionGeografica2
        /// </summary>
        public string LblDivisionGeografica2
        {
            get { return lbldivisionGeografica2; }
            set { lbldivisionGeografica2 = value; }
        }

        private string divisionGeografica2 = string.Empty;
        /// <summary>
        /// Codigo de DivisionGeografica1
        /// </summary>
        public string DivisionGeografica2
        {
            get { return divisionGeografica2; }
            set { divisionGeografica2 = value; }
        }

        private string nombreDivisionGeografica1 = string.Empty;
        /// <summary>
        /// Codigo de DivisionGeografica1
        /// </summary>
        public string NombreDivisionGeografica1
        {
            get { return nombreDivisionGeografica1; }
            set { nombreDivisionGeografica1 = value; }
        }

        private string nombreDivisionGeografica2 = string.Empty;
        /// <summary>
        /// Codigo de DivisionGeografica1
        /// </summary>
        public string NombreDivisionGeografica2
        {
            get { return nombreDivisionGeografica2; }
            set { nombreDivisionGeografica2 = value; }
        }

        private string compania = string.Empty;
        /// <summary>
        /// Compania asociada al pais
        /// </summary>
        public string Compania
        {
            get { return compania.ToUpper(); }
            set { compania = value.ToUpper(); }
        }

        private string nombre = string.Empty;
        /// <summary>
        /// Nombre del pais
        /// </summary>
        public string Nombre
        {
            get { return nombre; }
            set { nombre = value; }
        }

        public override string ToString()
        {
            return Nombre;
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor de la instancia pais
        /// </summary>
        public Pais()
        {
        }

        #endregion

        #region AccesoDatos

        /// <summary>
        /// Cargar los datos de un pais para una compania determinada
        /// </summary>
        /// <param name="compania">codigo de la compania</param>
        public void Cargar(string compania)
        {
            string sentencia =
                @" SELECT NOMBRE " +
                @" FROM " + Table.ERPADMIN_alFAC_PAIS  +
                @" WHERE UPPER(COD_CIA) = @COMPANIA" +
                @" AND COD_PAIS = @PAIS";

            SQLiteDataReader reader = null;
            SQLiteParameterList parametros = new SQLiteParameterList(new SQLiteParameter[] { new SQLiteParameter("@COMPANIA", compania.ToUpper()),
                                            new SQLiteParameter("@PAIS", codigo)});

            try
            {
                reader = GestorDatos.EjecutarConsulta(sentencia, parametros);
                if(reader.Read())
                    this.Nombre = reader.GetString(0);
                this.Compania = compania;
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
        /// <summary>
        /// Obtener lista de paises para una compania determinada
        /// </summary>
        /// <param name="compania">codigo de la compania</param>
        /// <returns>lista de paises</returns>
        public static List<Pais> ObtenerPaises(string compania)
        {
            List<Pais> paises = new List<Pais>();
            paises.Clear();

            string sentencia =
                @" SELECT COD_PAIS,NOMBRE,ETIQUETA_DIV_GEO1,ETIQUETA_DIV_GEO2 " +
                @" FROM " + Table.ERPADMIN_alFAC_PAIS +
                @" WHERE UPPER(COD_CIA) = @COMPANIA" +
                @" ORDER BY NOMBRE";

            SQLiteDataReader reader = null;
            SQLiteParameterList parametros = new SQLiteParameterList(new SQLiteParameter[] {new SQLiteParameter("@COMPANIA", compania.ToUpper())});
                       
            try
            {
                reader = GestorDatos.EjecutarConsulta(sentencia,parametros);

                while (reader.Read())
                {
                    Pais pais = new Pais();

                    pais.Compania = compania;
                    pais.codigo = reader.GetString(0);
                    pais.Nombre = reader.GetString(1);
                    if (!reader.IsDBNull(2))
                    {
                        pais.lbldivisionGeografica1 = reader.GetString(2)+":";
                    }
                    else
                    {
                        pais.lbldivisionGeografica1 = "Región 1:";
                    }
                    if (!reader.IsDBNull(3))
                    {
                        pais.lbldivisionGeografica2 = reader.GetString(3)+":";
                    }
                    else
                    {
                        pais.lbldivisionGeografica2 = "Región 2:";
                    }
                    paises.Add(pais);
                }

                return paises;
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

        /// <summary>
        /// Obtener lista de divisiones geográficas 1 para una companía y pais determinado
        /// </summary>
        /// <param name="compania">codigo de la compania</param>
        /// <returns>lista de paises</returns>
        public static List<DivisionGeografica> ObtenerDivisionesGeograficas1(string compania, string Pais)
        {
            List<DivisionGeografica> divs = new List<DivisionGeografica>();
            divs.Clear();

            string sentencia =
                @" SELECT DIVISION_GEOGRAFICA1,NOMBRE " +
                @" FROM " + Table.ERPADMIN_DIVISION_GEOGRAFICA1 +
                @" WHERE UPPER(COMPANIA) = @COMPANIA AND  UPPER(PAIS)=@PAIS" +
                @" ORDER BY NOMBRE";

            SQLiteDataReader reader = null;
            SQLiteParameterList parametros = new SQLiteParameterList(new SQLiteParameter[] { new SQLiteParameter("@COMPANIA", compania.ToUpper()),
            new SQLiteParameter("@PAIS", Pais.ToUpper())});

            try
            {
                reader = GestorDatos.EjecutarConsulta(sentencia, parametros);

                while (reader.Read())
                {
                    DivisionGeografica div = new DivisionGeografica();

                    div.Compania = compania;
                    div.Codigo = reader.GetString(0);
                    div.Nombre = reader.GetString(1);

                    divs.Add(div);
                }

                return divs;
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

        /// <summary>
        /// Obtener lista de divisiones geográficas 2 para una companía,división geográfica y pais determinado
        /// </summary>
        /// <param name="compania">codigo de la compania</param>
        /// <returns>lista de paises</returns>
        public static List<DivisionGeografica> ObtenerDivisionesGeograficas2(string compania,string div1, string Pais)
        {
            List<DivisionGeografica> divs = new List<DivisionGeografica>();
            divs.Clear();

            string sentencia =
                @" SELECT DIVISION_GEOGRAFICA2,NOMBRE " +
                @" FROM " + Table.ERPADMIN_DIVISION_GEOGRAFICA2 +
                @" WHERE UPPER(COMPANIA) = @COMPANIA AND DIVISION_GEOGRAFICA1=@DIV AND  UPPER(PAIS)=@PAIS" +
                @" ORDER BY NOMBRE";

            SQLiteDataReader reader = null;
            SQLiteParameterList parametros = new SQLiteParameterList(new SQLiteParameter[] { new SQLiteParameter("@COMPANIA", compania.ToUpper()),
            new SQLiteParameter("@PAIS", Pais.ToUpper()),
            new SQLiteParameter("@DIV", div1)});

            try
            {
                reader = GestorDatos.EjecutarConsulta(sentencia, parametros);

                while (reader.Read())
                {
                    DivisionGeografica div = new DivisionGeografica();

                    div.Compania = compania;
                    div.Codigo = reader.GetString(0);
                    div.Nombre = reader.GetString(1);

                    divs.Add(div);
                }

                return divs;
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


        
        /// <summary>
        /// Obtener pais para un cliente en una compania determinada
        /// </summary>
        /// <param name="cia">codigo de la compania</param>
        /// <param name="cliente">codigo del cliente</param>
        public void ObtenerPais(string cia, string cliente)
        {
            string sentencia =
                @" SELECT P.COD_PAIS,P.NOMBRE,P.ETIQUETA_DIV_GEO1,P.ETIQUETA_DIV_GEO2 " +
                @" FROM " + Table.ERPADMIN_alFAC_PAIS + " P, " + Table.ERPADMIN_CLIENTE_CIA + " C " +
                @" WHERE UPPER(P.COD_CIA) = @COMPANIA " + 
                @" AND C.COD_CLT = @CLIENTE "+
                @" AND P.COD_PAIS = C.COD_PAIS "+
                @" AND UPPER(P.COD_CIA) = UPPER(C.COD_CIA) ";
            
            SQLiteDataReader reader = null;
            SQLiteParameterList parametros = new SQLiteParameterList(new SQLiteParameter[] {
                new SQLiteParameter("@COMPANIA", cia),
                new SQLiteParameter("@CLIENTE", cliente)});

            try
            {
                reader = GestorDatos.EjecutarConsulta(sentencia, parametros);

                if (reader.Read())
                {
                    compania = cia;
                    codigo = reader.GetString(0);
                    nombre = reader.GetString(1);
                    if (!reader.IsDBNull(2))
                    {
                        lbldivisionGeografica1 = reader.GetString(2)+":";
                    }
                    else
                    {
                        lbldivisionGeografica1 = "Región 1:"; 
                    }
                    if (!reader.IsDBNull(3))
                    {
                        lbldivisionGeografica2 = reader.GetString(3)+":";
                    }
                    else
                    {
                        lbldivisionGeografica2 = "Región 2:";
                    }

                }
                else
                    throw new Exception("No se encontró el país del cliente");
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

        #endregion
    }
}
