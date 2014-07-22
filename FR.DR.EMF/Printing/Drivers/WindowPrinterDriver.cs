using System;
using System.IO;

namespace EMF.Printing.Drivers
{
	/// <summary>
	/// Dummy driver for console testing
	/// </summary>
	public class WindowPrinterDriver:IPrinterDriver
	{
		public WindowPrinterDriver()
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
			// TODO:  Add WindowPrinterDriver.Open implementation
		}

		public void Print(string text)
		{
			//OutputForm output = new OutputForm();
			text = text.Replace("\n","\r\n");
            //output.Output = text;
            //output.ShowDialog();
		}

		public void Configure()
		{
			// TODO:  Add WindowPrinterDriver.Configure implementation
		}

		public void Close()
		{
			// TODO:  Add WindowPrinterDriver.Close implementation
		}

		#endregion
	}
}
