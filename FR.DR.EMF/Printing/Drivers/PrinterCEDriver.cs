using System;
//using FieldSoftware.PrinterCE_NetCF;
using System.Drawing;

namespace EMF.Printing.Drivers
{
	/// <summary>
	/// Clase que permite imprimir texto e imagenes a un impresora movil.
	/// </summary>
	public class PrinterCEDriver:IPrinterDriver
	{
		const string LICENSE_KEY = "937S5H393B";

        private PrinterCE_Base Driver;

        private Printer Impresora = Printer.CANONBJ600;
        private Port Puerto = Port.INFRARED;

        private int cantidadLineasXPagina = 10;

        /// <summary>
        /// Cantidad de línea que se imprimen por página.
        /// Valor utilizado únicamente con PrinterCE driver.
        /// </summary>
        /// <exception cref="">
        /// ArgumentException cuando el valor no es mayor a cero.
        /// </exception>
        public int CantidadLineasXPagina
        {
            get
            {
                return this.cantidadLineasXPagina;
            }
            set
            {
                if (value < 1)
                    throw new ArgumentException("Solo se permiten valores mayores a cero.");

                this.cantidadLineasXPagina = value;
            }
        }

        /// <summary>
        /// Crea un nuevo driver tipo AsciiCE que solo permite imprimir texto.
        /// </summary>
        /// <param name="port"></param>
        /// <exception cref="">
        /// ArgumentException en caso de que el puerto seleccionado sea
        /// Port.NetIp, Port.NetPath o Port.Use_Current
        /// </exception>
        public PrinterCEDriver(Port port)
        {
            this.Driver = new AsciiCE(LICENSE_KEY);
            this.Puerto = port;

            if (this.Puerto == Port.NETIP ||
                this.Puerto == Port.NETPATH ||
                this.Puerto == Port.USE_CURRENT)
            {
                throw new ArgumentException("Invalid port for Ascii driver.");
            }
        }

        /// <summary>
        /// Crea un nuevo driver tipo PrinterCE que permite imprimir texto e imagenes.
        /// Se imprimen 10 líneas por página.
        /// </summary>
        /// <param name="printer"></param>
        /// <param name="port"></param>
        /// <exception cref="">
        /// ArgumentException en caso de que el puerto seleccionado sea
        /// Port.PRINTERCE_SHARE
        /// </exception>
        public PrinterCEDriver(Printer printer, Port port)
        {
            this.Driver = new PrinterCE(LICENSE_KEY);
            this.Impresora = printer;
            this.Puerto = port;

            if (this.Puerto == Port.PRINTERCE_SHARE)
                throw new ArgumentException("Invalid port for PrinterCE driver.");
        }

        /// <summary>
        /// Crea un nuevo driver tipo PrinterCE que permite imprimir texto e imagenes.
        /// </summary>
        /// <param name="printer"></param>
        /// <param name="port"></param>
        /// <param name="cantidadLineasXPagina">
        /// Cantidad de lineas que se imprimen por página.
        /// </param>
        /// <exception cref="">
        /// ArgumentException en caso de que el puerto seleccionado sea
        /// Port.PRINTERCE_SHARE.
        /// ArgumentException cuando el valor no es mayor a cero.
        /// </exception>
        public PrinterCEDriver(Printer printer, Port port, int cantidadLineasXPagina)
        {
            this.Driver = new PrinterCE(LICENSE_KEY);
            this.Impresora = printer;
            this.Puerto = port;

            this.CantidadLineasXPagina = cantidadLineasXPagina;

            if (this.Puerto == Port.PRINTERCE_SHARE)
                throw new ArgumentException("Invalid port for PrinterCE driver.");
        }

		#region IPrinterDriver Members

		public string ChangeFontStyle(bool bold, bool italic)
		{
			// TODO:  Add PrinterCEDriver.ChangeFontStyle implementation
			return null;
		}

		public string ChangeFont(string fontName, int fontSize)
		{		
			
			string font="";

			fontName = fontName.Replace("\"","");

			if (fontName=="MF107")
				font = "&";
			if (fontName=="MF204")
				font = "!";
			if (fontName=="MF226")
				font = "%";
			if (fontName=="UNICD")
				font = "u";
			if (fontName=="MF340")
				font = ">";			
			if (fontName=="MF185")
				font = "$";
			if (fontName=="MF102")
				font = " ";
			if (fontName=="MF072")
				font = "\"";
			if (fontName=="MF055")
				font = "#";
			if (fontName=="IS340")
				font = "[";
			if (fontName=="IS204")
				font = "P";
			
			//return "\x1B"+"R7\n";
			return "\x1B" + "w" + font + "\n";
		}

        private AsciiCE.ASCIIPORT PortToAsciiPort()
        {
            switch (this.Puerto)
            {
                case Port.ANYCOM_BT:
                    return AsciiCE.ASCIIPORT.ANYCOM_BT;
                case Port.BELKIN_BT:
                    return AsciiCE.ASCIIPORT.BELKIN_BT;
                case Port.COMPAQ_BT:
                    return AsciiCE.ASCIIPORT.COMPAQ_BT;
                case Port.IPAQ_BT:
                    return AsciiCE.ASCIIPORT.IPAQ_BT;
                case Port.SOCKETCOM_BT:
                    return AsciiCE.ASCIIPORT.SOCKETCOM_BT;
                case Port.WIDCOMM_BT:
                    return AsciiCE.ASCIIPORT.WIDCOMM_BT;
                case Port.BTQUIKPRINT:
                    return AsciiCE.ASCIIPORT.BTQUIKPRINT;
                case Port.COM1:
                    return AsciiCE.ASCIIPORT.COM1;
                case Port.COM2:
                    return AsciiCE.ASCIIPORT.COM2;
                case Port.COM3:
                    return AsciiCE.ASCIIPORT.COM3;
                case Port.COM4:
                    return AsciiCE.ASCIIPORT.COM4;
                case Port.COM5:
                    return AsciiCE.ASCIIPORT.COM5;
                case Port.COM6:
                    return AsciiCE.ASCIIPORT.COM6;
                case Port.COM7:
                    return AsciiCE.ASCIIPORT.COM7;
                case Port.COM8:
                    return AsciiCE.ASCIIPORT.COM8;
                case Port.COM9:
                    return AsciiCE.ASCIIPORT.COM9;
                case Port.LPT:
                    return AsciiCE.ASCIIPORT.LPT;
                case Port.TOFILE:
                    return AsciiCE.ASCIIPORT.TOFILE;
                case Port.PRINTERCE_SHARE:
                    return AsciiCE.ASCIIPORT.PRINTERCE_SHARE;
                default:
                    return AsciiCE.ASCIIPORT.INFRARED;
            }
        }

        private PrinterCE_Base.PORT PortToBasePort()
        {
            switch (this.Puerto)
            {
                case Port.ANYCOM_BT:
                    return PrinterCE_Base.PORT.ANYCOM_BT;
                case Port.BELKIN_BT:
                    return PrinterCE_Base.PORT.BELKIN_BT;
                case Port.COMPAQ_BT:
                    return PrinterCE_Base.PORT.COMPAQ_BT;
                case Port.IPAQ_BT:
                    return PrinterCE_Base.PORT.IPAQ_BT;
                case Port.SOCKETCOM_BT:
                    return PrinterCE_Base.PORT.SOCKETCOM_BT;
                case Port.WIDCOMM_BT:
                    return PrinterCE_Base.PORT.WIDCOMM_BT;
                case Port.BTQUIKPRINT:
                    return PrinterCE_Base.PORT.BTQUIKPRINT;
                case Port.INFRARED:
                    return PrinterCE_Base.PORT.INFRARED;
                case Port.COM1:
                    return PrinterCE_Base.PORT.COM1;
                case Port.COM2:
                    return PrinterCE_Base.PORT.COM2;
                case Port.COM3:
                    return PrinterCE_Base.PORT.COM3;
                case Port.COM4:
                    return PrinterCE_Base.PORT.COM4;
                case Port.COM5:
                    return PrinterCE_Base.PORT.COM5;
                case Port.COM6:
                    return PrinterCE_Base.PORT.COM6;
                case Port.COM7:
                    return PrinterCE_Base.PORT.COM7;
                case Port.COM8:
                    return PrinterCE_Base.PORT.COM8;
                case Port.COM9:
                    return PrinterCE_Base.PORT.COM9;
                case Port.LPT:
                    return PrinterCE_Base.PORT.LPT;
                case Port.NETIP:
                    return PrinterCE_Base.PORT.NETIP;
                case Port.NETPATH:
                    return PrinterCE_Base.PORT.NETPATH;
                case Port.TOFILE:
                    return PrinterCE_Base.PORT.TOFILE;
                default:
                    return PrinterCE_Base.PORT.USE_CURRENT;
            }
        }

        private PrinterCE_Base.PRINTER PrinterToBase()
        {
            switch (this.Impresora)
            {
                case Printer.ABLE_AP1300:
                    return PrinterCE_Base.PRINTER.ABLE_AP1300;
                case Printer.AXIOHM_A631:
                    return PrinterCE_Base.PRINTER.AXIOHM_A631;
                case Printer.BROTHER:
                    return PrinterCE_Base.PRINTER.BROTHER;
                case Printer.CANONBJ300:
                    return PrinterCE_Base.PRINTER.CANONBJ300;
                case Printer.CANONBJ360:
                    return PrinterCE_Base.PRINTER.CANONBJ360;
                case Printer.CANONBJ600:
                    return PrinterCE_Base.PRINTER.CANONBJ600;
                case Printer.CITIZEN_203:
                    return PrinterCE_Base.PRINTER.CITIZEN_203;
                case Printer.CITIZEN_CMP10:
                    return PrinterCE_Base.PRINTER.CITIZEN_CMP10;
                case Printer.CITIZEN_PD04:
                    return PrinterCE_Base.PRINTER.CITIZEN_PD04;
                case Printer.CITIZEN_PD22:
                    return PrinterCE_Base.PRINTER.CITIZEN_PD22;
                case Printer.CITIZEN_PN60:
                    return PrinterCE_Base.PRINTER.CITIZEN_PN60;
                case Printer.DYMOCOSTAR:
                    return PrinterCE_Base.PRINTER.DYMOCOSTAR;
                case Printer.ELTRADE:
                    return PrinterCE_Base.PRINTER.ELTRADE;
                case Printer.EPSON_ESCP2:
                    return PrinterCE_Base.PRINTER.EPSON_ESCP2;
                case Printer.EPSON_STYLUS:
                    return PrinterCE_Base.PRINTER.EPSON_STYLUS;
                case Printer.EPSON_TM_P60:
                    return PrinterCE_Base.PRINTER.EPSON_TM_P60;
                case Printer.EXTECH_2:
                    return PrinterCE_Base.PRINTER.EXTECH_2;
                case Printer.EXTECH_3:
                    return PrinterCE_Base.PRINTER.EXTECH_3;
                case Printer.EXTECH_4:
                    return PrinterCE_Base.PRINTER.EXTECH_4;
                case Printer.FUJITSU_FTP628:
                    return PrinterCE_Base.PRINTER.FUJITSU_FTP628;
                case Printer.GEBE_FLASH:
                    return PrinterCE_Base.PRINTER.GEBE_FLASH;
                case Printer.GENERIC24_180:
                    return PrinterCE_Base.PRINTER.GENERIC24_180;
                case Printer.GENERIC24_203:
                    return PrinterCE_Base.PRINTER.GENERIC24_203;
                case Printer.GENERIC24_360:
                    return PrinterCE_Base.PRINTER.GENERIC24_360;
                case Printer.HP_PCL:
                    return PrinterCE_Base.PRINTER.HP_PCL;
                case Printer.INTERMEC:
                    return PrinterCE_Base.PRINTER.INTERMEC;
                case Printer.IPC_PP50:
                    return PrinterCE_Base.PRINTER.IPC_PP50;
                case Printer.IPC_PP55:
                    return PrinterCE_Base.PRINTER.IPC_PP55;
                case Printer.OMNIPRINT:
                    return PrinterCE_Base.PRINTER.OMNIPRINT;
                case Printer.ONEIL:
                    return PrinterCE_Base.PRINTER.ONEIL;
                case Printer.PANASONIC_JTH200PR:
                    return PrinterCE_Base.PRINTER.PANASONIC_JTH200PR;
                case Printer.PENTAX_200:
                    return PrinterCE_Base.PRINTER.PENTAX_200;
                case Printer.PENTAX_300:
                    return PrinterCE_Base.PRINTER.PENTAX_300;
                case Printer.PENTAX_II:
                    return PrinterCE_Base.PRINTER.PENTAX_II;
                case Printer.PENTAX_RUGGEDJET:
                    return PrinterCE_Base.PRINTER.PENTAX_RUGGEDJET;
                case Printer.PERIPHERON_NOMAD:
                    return PrinterCE_Base.PRINTER.PERIPHERON_NOMAD;
                case Printer.POCKET_SPECTRUM:
                    return PrinterCE_Base.PRINTER.POCKET_SPECTRUM;
                case Printer.S_PRINT:
                    return PrinterCE_Base.PRINTER.S_PRINT;
                case Printer.SATO:
                    return PrinterCE_Base.PRINTER.SATO;
                case Printer.SEIKO_L465:
                    return PrinterCE_Base.PRINTER.SEIKO_L465;
                case Printer.SEIKO3445:
                    return PrinterCE_Base.PRINTER.SEIKO3445;
                case Printer.SEIKOLABELWRITER:
                    return PrinterCE_Base.PRINTER.SEIKOLABELWRITER;
                case Printer.SIPIX:
                    return PrinterCE_Base.PRINTER.SIPIX;
                case Printer.TALLY_MIP360:
                    return PrinterCE_Base.PRINTER.TALLY_MIP360;
                case Printer.TALLY_MTP4:
                    return PrinterCE_Base.PRINTER.TALLY_MTP4;
                case Printer.ZEBRA:
                    return PrinterCE_Base.PRINTER.ZEBRA;
                default:
                    return PrinterCE_Base.PRINTER.USE_CURRENT;
            }
        }

		public void Open()
		{
            try
            {
                if (this.Driver.GetType() == typeof(AsciiCE))
                    ((AsciiCE)this.Driver).SelectPort(PortToAsciiPort());
                else
                    ((PrinterCE)this.Driver).SetupPrinter(PrinterToBase(), PortToBasePort(), true);
            }
            catch (PrinterCEException ex)
            {
                if (this.Driver.GetType() == typeof(AsciiCE))
                    ((AsciiCE)this.Driver).ClosePort();
                else
                    ((PrinterCE)this.Driver).ShutDown();

                throw new Exception("Error estableciendo la conexión con la impresora. " + ex.Message);
            }
            catch (Exception ex)
            {
                if (this.Driver.GetType() == typeof(AsciiCE))
                    ((AsciiCE)this.Driver).ClosePort();
                else
                    ((PrinterCE)this.Driver).ShutDown();

                throw new Exception("Error no esperado conectando con la impresora. " + ex.Message);
            }
		}

		public void Print(string text)
		{
            try
            {
                if (this.Driver.GetType() == typeof(AsciiCE))
                {
                    ((AsciiCE)Driver).Text(text);
                }
                else
                {
                    ((PrinterCE)Driver).FontName = "Courier New";

                    string[] arrayText;
                    char[] descriminar = { '\n' };
                    int cont = 1;

                    text = text.Replace("\r", "");
                    arrayText = text.Split(descriminar);

                    foreach (string linea in arrayText)
                    {
                        ((PrinterCE)Driver).DrawText(linea + "\n\r");

                        if (cont++ % this.cantidadLineasXPagina == 0)
                            ((PrinterCE)Driver).NewPage();
                    }
                    
                    ((PrinterCE)Driver).EndDoc();
                }
            }
            catch (PrinterCEException ex)
            {
                if (this.Driver.GetType() == typeof(AsciiCE))
                    ((AsciiCE)this.Driver).ClosePort();
                else
                    ((PrinterCE)this.Driver).ShutDown();

                throw new Exception("Error realizando la impresión del documento. " + ex.Message);
            }
            catch (Exception ex)
            {
                if (this.Driver.GetType() == typeof(AsciiCE))
                    ((AsciiCE)this.Driver).ClosePort();
                else
                    ((PrinterCE)this.Driver).ShutDown();

                throw new Exception("Error no esperado en la impresión del documento. " + ex.Message);
            }

		}

		public void Configure()
		{
			// TODO:  Add PrinterCEDriver.Configure implementation
		}

		public void Close()
		{
            try
            {
                if (this.Driver.GetType() == typeof(AsciiCE))
                    ((AsciiCE)Driver).ClosePort();
                else
                    ((PrinterCE)Driver).ShutDown();
            }
            catch (Exception ex)
            {
                throw new Exception("Error cerrando la comunicación con la impresora. " + ex.Message);
            }
            finally
            {
                //GC.Collect();
            }
		}

		#endregion
	}
}
