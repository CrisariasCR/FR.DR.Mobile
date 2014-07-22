using System;

namespace EMF.Printing.Drivers
{
	/// <summary>
	/// Summary description for IPrinterDriver.
	/// </summary>
	public interface IPrinterDriver
	{

		string ChangeFontStyle(bool bold, bool italic);
		string ChangeFont(string fontName,int fontSize);
		
		void Open();
		void Print(string text);
		void Configure();
		void Close();
	}
}
