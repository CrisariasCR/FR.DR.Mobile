using System;
using System.Collections;
using System.Data;
//using System.Data.Common;
using System.Data.SQLiteBase;
using System.Globalization;
using System.Collections.Generic;
using System.Reflection;
using Softland.ERP.FR.Mobile.Cls.Documentos; 

namespace Softland.ERP.FR.Mobile.Cls.Utilidad
{
	/// <summary>
	/// Summary description for GestorUtilitario.
	/// Esta clase se encarga de ejecutar procesos que no son intrinsecos de un clase
	/// </summary>
	public  class GestorUtilitario
	{
		#region Variables

		//private static System.Collections.ArrayList datos= new ArrayList();
		public static string SimboloMonetario = string.Empty ;

        public static bool esDolar = false;

        public const string SimboloDolar = "$";

		private const string LIST_SEPARATOR = "Ç";

		private const string LIST_SEPARATOR2 = "~";

		#endregion

		#region Metodos

        public static string Dia(Dias d)
        {
            switch (d)
            {
                case Dias.L : return "Lunes";
                case Dias.K : return "Martes";
                case Dias.M : return "Miércoles";
                case Dias.J : return "Jueves";
                case Dias.V : return "Viernes";
                case Dias.S : return "Sábado";
                case Dias.D: return "Domingo";
                case Dias.T:
                default: return "Todos";
            }
        }
        public static Dias Dia(string d)
        {
            switch (d)
            {
                case "Lunes": return Dias.L;
                case "Martes": return Dias.K;
                case "Miércoles": return Dias.M;
                case "Jueves": return Dias.J;
                case "Viernes": return Dias.V;
                case "Sábado": return Dias.S;
                case "Domingo": return Dias.D;
                default: return Dias.T;
            }
        }

        public static string ListaDias(List<Dias> dias)
        {
            if (dias.Count == 7)
                return (Dia(Dias.T));
            else
            {
                string cadenaDias = Dia(dias[0]);
                for (int i = 1; i < dias.Count; i++)
                    cadenaDias += ", " + GestorUtilitario.Dia(dias[i]);
                return cadenaDias;
            }       
        }
        /// <summary>
		/// Metodo que se encarga de obtener la fecha Actual en dia/mes/anno
		/// </summary>
		/// <param name="ninguno">No recibe parametros</param>
		/// <returns>string  de la fecha actual</returns>
		public static string ObtenerFechaActual()
		{		
			return DateTime.Now.ToString("dd/MM/yyyy",DateTimeFormatInfo.InvariantInfo);   
		}
		
		/// <summary>
		/// Metodo que se encarga de obtener una fecha en dia/mes/anno a partir de un
		/// dato DateTime que recibe
		/// </summary>
		/// <param name="ninguno">No recibe parametros</param>
		/// <returns>string  de la fecha actual</returns>
		public static string ObtenerHoraString(DateTime fechap)
		{		
			return fechap.ToString("t",DateTimeFormatInfo.InvariantInfo);   
		}
		/// <summary>
		/// Metodo que se encarga de obtener una fecha en dia/mes/anno a partir de un
		/// dato DateTime que recibe
		/// </summary>
		/// <param name="ninguno">No recibe parametros</param>
		/// <returns>string  de la fecha actual</returns>
		public static string ObtenerFechaString(DateTime fechap)
		{		
			return fechap.ToString("dd/MM/yyyy",DateTimeFormatInfo.InvariantInfo);   
		}

		/// <summary>
		/// Metodo que se encarga de obtener la hora actual en hora militar
		/// </summary>
		/// <param name="ninguno">No recibe parametros</param>
		/// <returns> string con la hora actual</returns>
		public static string ObtenerHoraActual()
		{		
			return DateTime.Now.ToString("t",DateTimeFormatInfo.InvariantInfo);   
		}

		#region Funcion de Consecutivos

		/// <summary>
		/// Retorna el siguiente consecutivo para la cadena de caracteres dada por parámetro.
		/// Si la cadena resultante excede la longitud máxima se retorna la cadena original.
		/// </summary>
		/// <param name="codigop">Cadena de caracteres base.</param>
		/// <param name="longMaxima">Longitud máxima para la cadena resultante.</param>
		/// <returns>Consecutivo inmediato de la cadena de caracteres base</returns>
		public static string proximoCodigo( string codigoBase , int longMaxima )
		{
			int indice = 0;
			bool resto = true;
			bool esSeparador = false;
			string proximoCod = string.Empty;
			string caracter = string.Empty;
			string proximoCaracter = string.Empty;

			System.Text.Encoding myASCII = System.Text.Encoding.ASCII; 

			byte[] myBytes = new byte[1];

			if ( codigoBase.Equals("") )
				proximoCod = "1";
			else
			{
				proximoCod = codigoBase;
				indice = codigoBase.Length - 1;

				if ( longMaxima < codigoBase.Length )
					proximoCod = codigoBase;
				else
				{
					while ( indice >= 0 && resto )
					{
						caracter = codigoBase.Substring ( indice, 1 );
						myBytes = myASCII.GetBytes(caracter.ToString()); 

						if ( myBytes[0] >= 48 && myBytes[0] <= 57 )
						{
							//Es número

							esSeparador = false;
							if (caracter.Equals("9"))
							{
								proximoCaracter = "0";
								resto = true;
							}
							else
							{
								proximoCaracter = Convert.ToString(Convert.ToInt32(caracter) + 1); 
								resto = false;
							}

							//Se reemplaza el caracter en la hileta
							proximoCod = proximoCod.Remove(indice,1);
							proximoCod = proximoCod.Insert(indice,proximoCaracter); 
						}
						else if (myBytes[0] >= 65 && myBytes[0] <= 90 || myBytes[0] >= 97 && myBytes[0] <= 122 )
						{
							//Es una letra

							esSeparador = false;
							if ( caracter.Equals("Z") )
							{
								proximoCaracter = "A";
								resto = true;
							}
							else if ( caracter.Equals("z") )
							{
								proximoCaracter = "a";
								resto = true;
							}
							else
							{
								myBytes[0] += 1;
								proximoCaracter = myASCII.GetString(myBytes,0,1);
								resto = false;
							}

							//Se reemplaza el caracter en la hileta
							proximoCod = proximoCod.Remove(indice,1);
							proximoCod = proximoCod.Insert(indice,proximoCaracter); 
						}
						else if ( caracter.Equals ( LIST_SEPARATOR ) )
						{
							//El LIST_SEPARATOR representa el caracter nulo (cero) de los alfabéticos en minuscula

							esSeparador = true;

							proximoCaracter = "A";

							resto = false;

							//Se reemplaza el caracter en la hileta
							proximoCod = proximoCod.Remove(indice,1);
							proximoCod = proximoCod.Insert(indice,proximoCaracter); 
						}
						else if ( caracter.Equals( LIST_SEPARATOR2 ) )
						{
							//El LIST_SEPARATOR2 representa el caracter nulo (cero) de los alfabéticos en mayuscula

							esSeparador = true;

							proximoCaracter = "a";

							resto = false;

							//Se reemplaza el caracter en la hileta
							proximoCod = proximoCod.Remove(indice,1);
							proximoCod = proximoCod.Insert(indice,proximoCaracter); 
						}
						else if (indice.Equals(codigoBase.Length - 1 ))
						{
							//Caracter no alfanumerico al final de la hilera
							resto = true;
						}

						// Caso limite, ej: ' ZZ ' o '999'
						// Agregar un nuevo caracter al inicio de la hilera, si era numero el ultimo
						// caracter analizado ( primero de la string ) se pone uno '1' si no 'A'
						if ( indice.Equals(0) && resto )
						{

							//Caso en que por ejemplo llega 99 y se quiere longitud = 2
							if ( proximoCod.Length.Equals( longMaxima ))
								proximoCod = codigoBase;
							else if (!esSeparador)
							{
								if ( proximoCaracter.Equals("0") )
									proximoCod = "1" + proximoCod;
								else if (proximoCaracter.Equals("A") )
									proximoCod = "A" + proximoCod;
								else 
									proximoCod = "a" + proximoCod;
							}
						}

						indice--;
					}
				}
			}
			return proximoCod;
		}

		#endregion

        public static string Formato(decimal monto)
        {
            string formato = "{0,1:###0.00}";

            string retorno = String.Format(CultureInfo.InvariantCulture, formato, monto);

            return retorno;
        }

		public static string FormatNumero(decimal monto)
		{
            string formato = string.Empty;
            if (!esDolar)
            {
                formato = "{0,1:" + SimboloMonetario + "#,###,##0.00}";
            }
            else
            {
                formato = "{0,1:" + SimboloDolar + "#,###,##0.00}";
            }

			string retorno = String.Format(CultureInfo.InvariantCulture,formato,monto);

			return retorno;
		}

        public static string FormatPorcentaje(decimal monto)
        {
            string formato = "{0}%";

            string retorno = String.Format(CultureInfo.InvariantCulture, formato, monto);

            return retorno;
        }

        public static string FormatEntero(decimal monto)
        {
            string formato = "{0,1:" + "###0}";

            string retorno = String.Format(CultureInfo.InvariantCulture, formato, monto);

            return retorno;
        }

        public static string FormatTextBox(TipoMoneda moneda)
        {
            string formato = string.Empty;

            if(moneda == TipoMoneda.DOLAR)
                formato = "$###0.00";
            else
                formato = SimboloMonetario + "###0.00";

            return formato;
        }



        public static string FormatNumero(decimal monto, bool dolar)
        {
            string retorno = string.Empty;

            if (dolar)
                retorno = FormatNumeroDolar(monto);
            else
                retorno = FormatNumero(monto);

            return retorno;
        }

		public static string FormatNumeroDolar(decimal monto)
		{
			string formato = "{0,1:$###0.00}";

			string retorno = String.Format(CultureInfo.InvariantCulture,formato,monto);

			return retorno;
		}

		/// <summary>
		/// Parsea el valor a un entero utilizando invariantCulture.
		/// Levanta una excepcion si el valor no corresponde a un entero.
		/// </summary>
		/// <param name="valor"></param>
		/// <returns></returns>
		public static int ParseInt(string valor)
		{
			valor = valor.Replace(",",".");
			
			int retorno = int.Parse(valor,NumberStyles.Any, CultureInfo.InvariantCulture);

			return retorno;
		}

		/// <summary>
		/// Parsea el valor a un flotante utilizando invariantCulture.
		/// Levanta una excepcion si el valor no corresponde a un flotante.
		/// </summary>
		/// <param name="valor"></param>
		/// <returns></returns>
		public static decimal ParseDecimal(string valor)
		{
			//Invariant Culture utiliza . como separador de decimales
			valor = valor.Replace(",",".");

			decimal retorno = decimal.Parse(valor,NumberStyles.Any, CultureInfo.InvariantCulture);

			return retorno;
		}

		/// <summary>
		/// Parsea el valor a un flotante utilizando invariantCulture y
		/// redondea el numero con los decimales especificados en variablesGlobales
		/// Levanta una excepcion si el valor no corresponde a un flotante.
		/// </summary>
		/// <param name="valor"></param>
		/// <returns></returns>
		public static decimal ParseAndRoundDecimal(string valor)
		{
			//Invariant Culture utiliza . como separador de decimales
			valor = valor.Replace(",",".");

			//Aca utilizamos float pues Math.Round solo redondea con Doubles
			decimal number = decimal.Parse(valor,NumberStyles.Any, CultureInfo.InvariantCulture);

			//number = Decimal.Round(number, VariablesGlobales.CantidadDecimales);

			return number;
		}

		/// <summary>
		/// Convierte y redondea los valores string a decimal y calcula las unidades
		/// de detalle y almacen de acuerdo a la unidad de empaque
		/// </summary>
		/// <param name="cantidadAlmacen">Cantidad de almacen a convertir</param>
		/// <param name="cantidadDetalle">Cantidad de detalle a convertir</param>
		/// <param name="unidadEmpaque">Unidad de empaque del articulo</param>
		/// <param name="retornoAlmacen">Cantidad de almacen convertida</param>
		/// <param name="retornoDetalle">Cantidad de detalle convertida</param>
        public static void CalculaUnidades(decimal cantidadAlmacen,
                                     decimal cantidadDetalle, 
									 int unidadEmpaque,
									 out decimal retornoAlmacen,
                                     out decimal retornoDetalle)
		{
			retornoAlmacen = cantidadAlmacen;
			retornoDetalle = cantidadDetalle;
				
			if(retornoDetalle == unidadEmpaque)
			{
				retornoAlmacen += 1;
				retornoDetalle = 0 ;
			}
			else if(retornoDetalle > unidadEmpaque)
			{
				decimal uDetalle = retornoDetalle;

				while(uDetalle > 0)
				{
					if(uDetalle >= unidadEmpaque)
					{
						retornoAlmacen++;
						retornoDetalle -= unidadEmpaque;
					}
					else
					{
						break;
					}
					uDetalle -= unidadEmpaque;
				} 
			}
		}

		//Caso 29713 LDS 11/09/2007
		/// <summary>
		/// Obtiene un valor decimal y retorna el respectivo valor porcentual.
		/// </summary>
		/// <param name="expresionDecimal">
		/// Corresponde a la expresión decimal que se desea convertir a porcentual.
		/// </param>
		/// <returns>
		/// Retorna el valor porcentual de la expresión decimal.
		/// </returns>
		public static decimal ObtenerPorcentaje(decimal expresionDecimal)
		{
			decimal porcentaje = expresionDecimal * 100;

			return porcentaje;
		}
		//Caso: 32380 ABC 14/05/2008
		//Disponibilizar nuevos datos para reporte Venta en Consignación (Monto en letras)
		/// <summary>
		/// Obtiene el valor en letras en español de una cifra.
		/// </summary>
		/// <param name="cifra">Cifra.</param>
		/// <returns></returns>
		public static string NumeroALetras(decimal cifra)
		{
			string dec = string.Empty;
			string negativo = string.Empty;
			if (cifra < 0)
			{
				cifra *= -1;
				negativo = "MENOS ";
			}
			decimal entero = decimal.Truncate(cifra);
			decimal fraccion = cifra - entero;
			int decimales = Convert.ToInt32(Math.Round(fraccion * 100, 2));
			if (decimales > 0)
				dec = " CON " + decimales.ToString() + "/100";
			string resultado = negativo + EnteroToLetras(Decimal.ToInt64(entero)) + dec;
			return resultado;
		}
		private static string EnteroToLetras(Int64 value)
		{
			string Num2Text = "";
			if (value == 0) Num2Text = "CERO";
			else if (value == 1) Num2Text = "UNO";
			else if (value == 2) Num2Text = "DOS";
			else if (value == 3) Num2Text = "TRES";
			else if (value == 4) Num2Text = "CUATRO";
			else if (value == 5) Num2Text = "CINCO";
			else if (value == 6) Num2Text = "SEIS";
			else if (value == 7) Num2Text = "SIETE";
			else if (value == 8) Num2Text = "OCHO";
			else if (value == 9) Num2Text = "NUEVE";
			else if (value == 10) Num2Text = "DIEZ";
			else if (value == 11) Num2Text = "ONCE";
			else if (value == 12) Num2Text = "DOCE";
			else if (value == 13) Num2Text = "TRECE";
			else if (value == 14) Num2Text = "CATORCE";
			else if (value == 15) Num2Text = "QUINCE";
			else if (value < 20) Num2Text = "DIECI" + EnteroToLetras(value - 10);
			else if (value == 20)
				Num2Text = "VEINTE";
			else if (value < 30) Num2Text = "VEINTI" + EnteroToLetras(value - 20);
			else if (value == 30) Num2Text = "TREINTA";
			else if (value == 40) Num2Text = "CUARENTA";
			else if (value == 50) Num2Text = "CINCUENTA";
			else if (value == 60) Num2Text = "SESENTA";
			else if (value == 70) Num2Text = "SETENTA";
			else if (value == 80) Num2Text = "OCHENTA";
			else if (value == 90) Num2Text = "NOVENTA";
			else if (value < 100)
			{
				decimal parteMenor = decimal.Truncate(decimal.Divide(value, 10)) * 10;
				decimal resto = decimal.Remainder(value, 10);
				Num2Text = EnteroToLetras(Decimal.ToInt64(parteMenor)) + " Y " + EnteroToLetras(decimal.ToInt64(resto));
			}
			else if (value == 100) Num2Text = "CIEN";
			else if (value < 200) Num2Text = "CIENTO " + EnteroToLetras(value - 100);
			else if (value == 200 || value == 300 || value == 400 || value == 600 || value == 800)
			{
				decimal entero = decimal.Divide(value, 100);
				Num2Text = EnteroToLetras(decimal.ToInt64(entero)) + "CIENTOS";
			}
			else if (value == 500) Num2Text = "QUINIENTOS";
			else if (value == 700) Num2Text = "SETECIENTOS";
			else if (value == 900) Num2Text = "NOVECIENTOS";
			else if (value < 1000)
			{
				decimal parteMenor = decimal.Truncate(decimal.Divide(value, 100)) * 100;
				decimal resto = decimal.Remainder(value, 100);
				Num2Text = EnteroToLetras(Decimal.ToInt64(parteMenor)) + " " + EnteroToLetras(decimal.ToInt64(resto));
			}
			else if (value == 1000) Num2Text = "MIL";
			else if (value < 2000) Num2Text = "MIL " + EnteroToLetras(value % 1000);
			else if (value < 1000000)
			{
				decimal valor = decimal.Truncate(decimal.Divide(value, 1000));
				Num2Text = EnteroToLetras(decimal.ToInt64(valor)) + " MIL";
				if ((value % 1000) > 0) Num2Text = Num2Text + " " + EnteroToLetras(value % 1000);
			}
			else if (value == 1000000) Num2Text = "UN MILLON";
			else if (value < 2000000) Num2Text = "UN MILLON " + EnteroToLetras(value % 1000000);
			else if (value < 1000000000000)
			{
				decimal millones = decimal.Truncate(decimal.Divide(value, 1000000));
				Num2Text = EnteroToLetras(decimal.ToInt64(millones)) + " MILLONES ";
				decimal resto = value - millones * 1000000;
				if (resto > 0)
					Num2Text += " " + EnteroToLetras(decimal.ToInt64(resto));
			}
			else if (value == 1000000000000) Num2Text = "UN BILLON";
			else if (value < 2000000000000)
			{
				decimal resto = decimal.Truncate(decimal.Divide(value, 1000000000000)) * 1000000000000;
				Num2Text = "UN BILLON " + EnteroToLetras(decimal.ToInt64(resto));
			}
			else
			{
				decimal billones = decimal.Truncate(decimal.Divide(value, 1000000000000));
				Num2Text = EnteroToLetras(decimal.ToInt64(billones)) + " BILLONES";
				decimal resto = value - billones * 1000000000000;
				if (resto > 0)
					Num2Text += " " + EnteroToLetras(decimal.ToInt64(resto));
			}
			return Num2Text;
		}

        ///Caso 36136
        /// <summary>
        /// Desglosa las cantides en unides de almacen y unidades de detalle
        /// </summary>
        /// <param name="cantidad">Cantidad desglosar</param>
        /// <param name="UnidadEmpaque">Unidad de empaque del articulo</param>
        public static void CalcularCantidadBonificada(decimal cantidad, decimal UnidadEmpaque, ref decimal cantBonAlm, ref decimal cantBonDet )
        {
            //Redondeamos pues según el factor de bonificaciún puede dar números muy precisos.
            // Se agrega el "=", Anterior if (cantidad > 1) - KFC
            if (cantidad >= 0)
                cantBonAlm = Math.Round(cantidad, 2);
            //Sacamos la parte entera de la cantidad.
            int cantEntera = (int)cantBonAlm;
            //Sacamos la parte decimal de la cantidad.
            decimal cantDecimal = cantBonAlm - cantEntera;
            //La cantidad de almacén es solo la parte entera.
            cantBonAlm = cantEntera;
            //La cantidad en detalle es la parte decimal por el factor de conversión.
            cantBonDet = cantDecimal * UnidadEmpaque;

            //LDA
            //RD1-01091-PZJL
            //No se mostraban las bonificaciones en unidades de detalle
            cantBonDet = Math.Round(cantBonDet, 0);
        }

        //public static void FormatCalcEntero(ref EMF.WF.EMFTextbox text)
        //{
        //    text.Format = "0";
        //    text.Text = "0";
        //    //text.Value = "";
        //}
        public static void FormatCalcDecimal(ref EMF.WF.EMFTextbox text)
        {
            text.Format = "0.00";
            text.Text = "0.00";
            //text.Value = "";
        }

        /// <summary>
        /// Resive un SI o un NO y retorna el valor booleano
        /// </summary>
        /// <param name="valor"></param>
        /// <returns></returns>
        public static bool ParseBoolean(string valor)
        {
            if (valor.ToUpper() == "SI")
                return true;
            return false;
        }

        //Cesar Iglesias
        /// <summary>
        /// Total en Unidades de Almacen de la línea
        /// </summary>
        /// <param name="cantidadAlmacen">Cantidad Almacen</param>
        /// <param name="cantidadDetalle">Cantidad Detalle</param>
        /// <param name="UnidadEmpaque">Unidad de Empaque</param>
        /// <returns>Unidades de Almance</returns>
        
        public static decimal TotalAlmacen(string cantidadAlmacen, string cantidadDetalle, int UnidadEmpaque)
        {
            decimal Almacen = decimal.Zero;
            decimal Detalle = decimal.Zero;

            if (cantidadAlmacen.Equals(string.Empty))
                cantidadAlmacen = "0";

            if (cantidadDetalle.Equals(string.Empty))
                cantidadDetalle = "0";

            Almacen = GestorUtilitario.ParseAndRoundDecimal(cantidadAlmacen);
            Detalle = GestorUtilitario.ParseAndRoundDecimal(cantidadDetalle);

            return Almacen + (Detalle / UnidadEmpaque);
        }

        #region Calculo Multiplos de Venta
        /// <summary>
        /// Calcula el multiplo de venta para la cantidad dada con el factor de venta del articulo
        /// </summary>
        /// <param name="textoCantidaAlmacen"></param>
        /// <param name="textoCantidadDetalle"></param>
        /// <param name="unidadEmpaque"></param>
        /// <param name="factorVenta"></param>
        /// <param name="outCantidadAlmacen"></param>
        /// <param name="outCantidadDetalle"></param>
        public static void CalcularUnidadesMultipoVenta(decimal cantidadAlmacen, decimal cantidadDetalle
            , int unidadEmpaque, decimal factorVenta
            , ref decimal outCantidadAlmacen, ref decimal outCantidadDetalle)
        {
            
            decimal cantidad = cantidadAlmacen / factorVenta;
            if (Decimal.Remainder(cantidadAlmacen, factorVenta) > 0)
            {
                cantidad = Decimal.Round(cantidad - 0.5m, 0) + 1;
            }
            cantidad *= factorVenta;
            GestorUtilitario.CalculaUnidades(cantidad, cantidadDetalle, unidadEmpaque, out outCantidadAlmacen, out outCantidadDetalle);
            if (cantidad > 0)
                outCantidadDetalle = 0;
        }

        #endregion
        #endregion



    }
}