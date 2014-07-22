using System;
using System.Collections;
using System.Text;
using EMF.Printing.RDL;
using EMF.Printing.Text;
using EMF.Printing.Symbols;

namespace EMF.Printing.Commands
{
	/// <summary>
	/// Summary description for DefineColumnsCommand.
	/// </summary>
	public class DefineCommand:RDLCommand
	{
		public DefineCommand(StringTokenizer tokenizer):base(tokenizer)
		{
			this.type = CommandType.Define;
			this.argumentKind =  new TokenKind[]{TokenKind.Word,TokenKind.WordOrQuotedString};
			ParseArguments(tokenizer);
		}

		public override string Execute(RDLEngine engine, RDLContext context)
		{
			int tokenIndex=0;
			ArrayList rowFormat = new ArrayList();
			string symbolName="";

			foreach(Token token in this.argumentList)
			{			
				if (tokenIndex!=0)
					rowFormat.Add(token.Value);			
					else
					symbolName = (string)token.Value;
				
				tokenIndex++;
			}
			
			context.AddSymbol(new RowFormatSymbol(symbolName,rowFormat));

			return "";
		}
	}
}
