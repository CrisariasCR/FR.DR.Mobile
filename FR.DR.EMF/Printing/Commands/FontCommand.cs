using System;
using System.Text;
using EMF.Printing.RDL;
using EMF.Printing.Text;

namespace EMF.Printing.Commands
{
	/// <summary>
	/// Summary description for cmdPrint.
	/// </summary>
	public class FontCommand:RDLCommand
	{
		public FontCommand(StringTokenizer tokenizer):base(tokenizer)
		{
			this.type = CommandType.Font;
			this.argumentKind =  new TokenKind[]{TokenKind.QuotedString,TokenKind.Number};
			ParseArguments(tokenizer);
		}

		public override string Execute(RDLEngine engine, RDLContext context)
		{			
			StringBuilder output = new StringBuilder();	

			try
			{

				string fontName = ((Token)argumentList[0]).Value;
				int fontSize = int.Parse(((Token)argumentList[1]).Value);

				output.Append(engine.PrinterDriver.ChangeFont(fontName,fontSize));

				return output.ToString();
			}
			catch
			{
				throw new Exception("Error changing font");
			}
		}

	}
}
