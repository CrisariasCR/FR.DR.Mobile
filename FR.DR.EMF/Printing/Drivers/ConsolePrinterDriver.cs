using System;
using System.IO;

namespace EMF.Printing.Drivers
{
	/// <summary>
	/// Dummy driver for console testing
	/// </summary>
	public class ConsolePrinterDriver:IPrinterDriver
	{
		public ConsolePrinterDriver()
		{
			//Do nothing
		}
		#region IPrinterDriver Members

		public string ChangeFontStyle(bool bold, bool italic)
		{
			// TODO:  Add ConsolePrinterDriver.ChangeFontStyle implementation
			return "(CHANGE FONT TO BOLD:" + bold.ToString() + " ITALIC: " + italic.ToString() + "\n";
		}

		public string ChangeFont(string fontName, int fontSize)
		{
			// TODO:  Add ConsolePrinterDriver.ChangeFont implementation
			return "(CHANGE FONT TO NAME:" + fontName.ToString() + " SIZE: " + fontSize.ToString()+ "\n";
		}

		public void Open()
		{
			// TODO:  Add ConsolePrinterDriver.Open implementation
		}

		public void Print(string text)
		{
			Console.Write(text);
		}

		public void Configure()
		{
			// TODO:  Add ConsolePrinterDriver.Configure implementation
		}

		public void Close()
		{
			// TODO:  Add ConsolePrinterDriver.Close implementation
		}

		#endregion
	}
}
