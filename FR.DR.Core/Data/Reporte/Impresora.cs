using System;
using EMF.Printing;
using EMF.Printing.Drivers;
using System.Xml;
using System.IO;

namespace Softland.ERP.FR.Mobile.Cls.Reporte
{
	/// <summary>
	/// Summary description for Impresora.
	/// </summary>
	public class Impresora
	{
		#region Constantes
		//Caso 32256 LDS 24/04/2008
		/// <summary>
		/// Debe ser utilizada para impresoras Ascii.
		/// </summary>
		public const string ImpresoraGenerica = "GENERICA";
		#endregion

		#region Variables de clase

		#region Variables privadas
		//Caso 32256 LDS 24/04/2008
		/// <summary>
		/// Tipo de impresora.
		/// </summary>
		protected static Printer impresora = Printer.ZEBRA;
		//Caso 32256 LDS 24/04/2008
		/// <summary>
		/// Puerto de comunicación.
		/// </summary>
		protected static Port puerto = Port.INFRARED;
		
		//Caso 25452 LDS 18/10/2007
		/// <summary>
		/// Cantidad de copias.
		/// </summary>
		protected static int cantidadCopias = 0;
		//Caso 32426 ABC 13/05/2008
		/// <summary>
		/// Cantidad de líneas de detalle.
		/// </summary>
        // mvega: si cantidad lineas es cero ponerle 50 para que no de error mientrastanto
		protected static int cantidadLineas = 50;
        //Caso 35723 LJR 04/08/2009
        /// <summary>
        /// Sugerir Imprimir.
        /// </summary>
        private static bool sugerirImprimir = true;

		#endregion

		#region Variables públicas
		//Caso 32256 LDS 24/04/2008
		public static bool UtilizaImpresoraGenerica = false;
		#endregion

		#endregion

		#region Propiedades de la clase
		//Caso 32256 LDS 24/04/2008
		/// <summary>
		/// Tipo de impresora, se debe utilizar solo cuando no se utiliza impresoras Ascii.
		/// </summary>
		public static Printer Tipo
		{
			get
			{
				return Impresora.impresora;
			}
		}
		//Caso 32256 LDS 24/04/2008
		public static Port Puerto
		{
			get
			{
				return Impresora.puerto;
			}
            set
            {
                puerto = value;
            }
		}

		//Caso 25452 LDS 18/10/2007
		public static int CantidadCopias
		{
			get
			{
				return cantidadCopias;
			}
			set
			{
				cantidadCopias = value;
			}
		}
		//Caso 32426 ABC 13/05/2008
		public static int CantidadLineas
		{
			get
			{
				return cantidadLineas;
			}
			set
			{
				cantidadLineas = value;
			}
		}
        //Caso 35723 LJR 04/08/2009
        public static bool SugerirImprimir
        {
            get 
            { 
                return sugerirImprimir; 
            }
            set 
            { 
                sugerirImprimir = value; 
            }
        }
		#endregion
		
		#region Metodos de configuracion de impresora y puerto
	
		/// <summary>
		/// Indica a la impresora el puerto que debe usar
		/// </summary>
		/// <param name="puerto">puerto seleccionado</param>
        public static void IndicaPuerto(string puerto)
        {
            switch (puerto)
            {
                case "SERIAL COM1":
                case "COM1":
                    Impresora.Puerto = Port.COM1;
                    break;
                case "SERIAL COM2":
                case "COM2":
                    Impresora.Puerto = Port.COM2;
                    break;
                case "SERIAL COM3":
                case "COM3":
                    Impresora.Puerto = Port.COM3;
                    break;
                case "SERIAL COM4":
                case "COM4":
                    Impresora.Puerto = Port.COM4;
                    break;
                case "SERIAL COM5":
                case "COM5":
                    Impresora.Puerto = Port.COM5;
                    break;
                case "SERIAL COM6":
                case "COM6":
                    Impresora.Puerto = Port.COM6;
                    break;
                case "SERIAL COM7":
                case "COM7":
                    Impresora.Puerto = Port.COM7;
                    break;
                case "SERIAL COM8":
                case "COM8":
                    Impresora.Puerto = Port.COM8;
                    break;
                case "SERIAL COM9":
                case "COM9":
                    Impresora.Puerto = Port.COM9;
                    break;

                /*case Port.WIDCOMM_BT:
                    return "BT WIDCOMM";
                case Port.IPAQ_BT:
                    return "BT IPAQ";
                case Port.BTQUIKPRINT:
                    return "BT QUICKPRINT";
                case Port.SOCKETCOM_BT:
                    return "BT SOCKETCOM";*/

                case "LPT1":
                    Impresora.Puerto = Port.LPT;
                    break;

                case "INFRARROJO":
                case "INFRARED":
                    Impresora.Puerto = Port.INFRARED;
                    break;

                case "BT SOCKETCOM":
                case "SOCKETCOM_BT":
                    Impresora.Puerto = Port.SOCKETCOM_BT;
                    break;

                case "BLUETOOTH":
                case "ANYCOM_BT":
                    Impresora.Puerto = Port.ANYCOM_BT;
                    break;

                case "BT QUICKPRINT":
                case "BTQUIKPRINT":
                    Impresora.Puerto = Port.BTQUIKPRINT;
                    break;

                case "BT WIDCOMM":
                    Impresora.Puerto = Port.WIDCOMM_BT;
                    break;

                case "BT IPAQ":
                    Impresora.Puerto = Port.IPAQ_BT;
                    break;


            }
        }
        
        /// <summary>
        /// Retorna el String segun el puerto
        /// </summary>
        /// <returns></returns>
        public static string IndicaPuerto(Port puerto)
        {
            switch (puerto)
            {
                case Port.COM1:
                    return "SERIAL COM1";
                case Port.COM2:
                    return  "SERIAL COM2";
                case Port.COM3:
                    return  "SERIAL COM3";
                case Port.COM4:
                    return  "SERIAL COM4";
                case Port.COM5:
                    return  "SERIAL COM5";
                case Port.COM6:
                    return  "SERIAL COM6";
                case Port.COM7:
                    return  "SERIAL COM7";
                case Port.COM8:
                    return  "SERIAL COM8";
                case Port.COM9:
                    return  "SERIAL COM9";
                case Port.WIDCOMM_BT:
                    return  "BT WIDCOMM";
                case Port.IPAQ_BT:
                    return  "BT IPAQ";
                case Port.LPT:
                    return  "LPT1";
                case Port.INFRARED:
                    return  "INFRARROJO";
                case Port.ANYCOM_BT:
                    return  "BLUETOOTH";
                case Port.BTQUIKPRINT:
                    return  "BT QUICKPRINT";
                case Port.SOCKETCOM_BT:
                    return  "BT SOCKETCOM";
                default: return null;
            }
        }
		/// <summary>
		/// Funcion encargada de identificar la impresora seleccionada.
		/// </summary>
		/// <param name="impresora">Nombre de la impresora seleccionada</param>
		public static void IndicaImpresora(string impresora)
		{
			Impresora.UtilizaImpresoraGenerica = impresora.Equals(Impresora.ImpresoraGenerica);

            if (Impresora.UtilizaImpresoraGenerica)
                return;            
			
            //Caso 32256 LDS 24/04/2008
            //TODO
            //foreach(Printer printer in OpenNETCF.EnumEx.GetValues(typeof(EMF.Printing.Printer)))
            //{
            //    if (printer.ToString() == impresora)
            //    {
            //        Impresora.impresora = printer;
            //        break;
            //    }
            //}
            return;
		}

		/// <summary>
		/// Obtiene el driver con el puerto indicado para realizar la impresión.
		/// </summary>
		/// <returns></returns>
		public static IPrinterDriver ObtenerDriver()
		{
			//Caso 32256 LDS 24/04/2008
            if (Impresora.UtilizaImpresoraGenerica)
                //return new PrinterCEDriver(Impresora.Puerto);
                return new AndroidDriver(FRmConfig.Impresora);
            else
            {

                //Caso 32426 ABC 13/05/2008
                //return new PrinterCEDriver(Impresora.Tipo, Impresora.Puerto, Impresora.CantidadLineas);
                //return new AndroidDriver(Impresora.Tipo.ToString(), Impresora.CantidadLineas);
                return new AndroidDriver(FRmConfig.Impresora, Impresora.CantidadLineas);
            }
		}

		#endregion

        #region Modificar Configuracion en el XML
        
        public static void ActualizarDatos()
        {
			XmlDocument doc = new XmlDocument();
			XmlNode node;

			try
			{
				doc.Load(Softland.ERP.FR.Mobile.Cls.FRmConfig.FullAppPath+"\\Config.xml");

				node = doc.DocumentElement;
 
				foreach(XmlNode node2 in node.ChildNodes)
				{
					foreach(XmlNode node3 in node2.ChildNodes)
					{
						if(node3.Name.Equals("Impresora"))
						{
							foreach(XmlNode node4 in node3.ChildNodes)
							{
								switch(node4.Name)
								{
									case"Tipo":
										if(Impresora.UtilizaImpresoraGenerica)
											node4.InnerText = Impresora.ImpresoraGenerica;
										else
											node4.InnerText = Impresora.Tipo.ToString();
										break;
									case "Puerto":
									    node4.InnerText = IndicaPuerto(Impresora.Puerto); //.ToString();
										break;
								}
							}
						}
					}
				}

				doc.Save(Softland.ERP.FR.Mobile.Cls.FRmConfig.FullAppPath+"\\Config.xml");
			}
            catch
            {
                //Mensaje.mostrarAlerta("Los cambios no fueron guardados: " + ex.Message);
            }
        }
        #endregion
    }
}
