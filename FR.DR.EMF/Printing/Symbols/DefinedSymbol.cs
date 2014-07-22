using System;
using System.Collections;

namespace EMF.Printing.Symbols
{
	/// <summary>
	/// Summary description for RowFormatSymbol.
	/// </summary>
	public class RowFormatSymbol:IPrintable
	{

		private string name;
		private ArrayList values;

		public RowFormatSymbol(string name,ArrayList values)
		{
			this.name = name;
			this.values = values;
		}

		#region IPrintable Members

		public string GetObjectName()
		{
			return name;
		}

		public object GetField(string name)
		{
			if (name=="VALUES")
				return this.values;

			return null;
		}

		#endregion
	}
}
