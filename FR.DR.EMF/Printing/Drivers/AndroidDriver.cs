using System;
//using FieldSoftware.PrinterCE_NetCF;
using System.Drawing;
using Com.Printinglibrary;
using Com.Impresion;
using Java.IO;
using Java.Util;
using Java.Lang;

namespace EMF.Printing.Drivers
{
	/// <summary>
	/// Clase que permite imprimir texto e imagenes a un impresora movil.
	/// </summary>
	public class AndroidDriver:IPrinterDriver
	{
        //const string LICENSE_KEY = "937S5H393B";
#pragma warning disable 169
        private PrinterCE_Base Driver;
        public static PrintingLibrary ptLibrary = new PrintingLibrary();
        public static bool isOpen = false;
        public static bool esDolar = false;
        public static string simboloMoneda;

        //private Printer Impresora = Printer.CANONBJ600;
        //private Port Puerto = Port.INFRARED;

        private int cantidadLineasXPagina = 10;

        private string deviceName = string.Empty;

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
        public AndroidDriver(string  DeviceName)
        {
            this.deviceName = DeviceName;
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
        public AndroidDriver(string DeviceName, int cantidadLineasXPagina)
        {
            this.deviceName = DeviceName;
            this.CantidadLineasXPagina = cantidadLineasXPagina;
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

		public void Open()
		{
            try
            {
                isOpen=ptLibrary.Inicializar(this.deviceName);
                Thread.Sleep(1500);
                if (isOpen)
                {
                    ptLibrary.SendData("\n");
                }
            }
            catch (System.Exception ex)
            {
                string error="Error no esperado al abrir impresora. " + ex.Message;
            }
           //Implementar
		}

		public void Print(string text)
		{
            try
            {
                if (isOpen)
                {
                    //int index = text.IndexOf("wP");
                    //if (index != -1)
                    //{
                    //    string temp = text.Substring(0, index - 1);
                    //    text = temp + text.Substring(index + 3);
                    //}
                    if (!esDolar)
                    {
                        string temp = string.Empty;
                        switch (simboloMoneda)
                        {
                            case "¢": temp = "C"; break;
                            case "₡": temp = "C"; break;
                            case "RD": temp = "RD"; break;
                            case "rd": temp = "rd"; break;
                            case "MX": temp = "MX"; break;
                            case "mx": temp = "mx"; break;
                            default: temp = "$"; break;
                        }
                        text = text.Replace("$", temp);
                    }
                    ptLibrary.SendData(text);
                }
            }
            catch (System.Exception ex)
            {
                string error = "Error no esperado en la impresión del documento. " + ex.Message;
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
                    ptLibrary.SendData("\n");
                    ptLibrary.CloseBT();
                    Thread.Sleep(1500);
                    isOpen = false;
            }
            catch (System.Exception ex)
            {
                string error = "Error no esperado al cerrar impresora. " + ex.Message;
            }
            //Implementar
		}

		#endregion
	}
}
