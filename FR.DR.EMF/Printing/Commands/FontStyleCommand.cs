using System;
using System.Text;
using EMF.Printing.RDL;
using EMF.Printing.Text;

namespace EMF.Printing.Commands
{
	/// <summary>
	/// Summary description for cmdPrint.
	/// </summary>
	public class FontStyleCommand:RDLCommand
	{
		public FontStyleCommand(StringTokenizer tokenizer):base(tokenizer)
		{
			this.type = CommandType.FontStyle;
			this.argumentKind =  new TokenKind[]{TokenKind.Word,TokenKind.Word};
			ParseArguments(tokenizer);
		}

		public override string Execute(RDLEngine engine, RDLContext context)
		{			
			StringBuilder output = new StringBuilder();	

			try
			{
				bool fontBold = bool.Parse(((Token)argumentList[0]).Value);
				bool fontItalic = bool.Parse(((Token)argumentList[1]).Value);

				output.Append(engine.PrinterDriver.ChangeFontStyle(fontBold,fontItalic));

				return output.ToString();
			}
			catch
			{
				throw new Exception("Error changing font style");
			}
		}

	}
}
