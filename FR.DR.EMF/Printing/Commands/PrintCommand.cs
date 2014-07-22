using System;
using System.Text;
using EMF.Printing.RDL;
using EMF.Printing.Text;

namespace EMF.Printing.Commands
{
	/// <summary>
	/// Summary description for cmdPrint.
	/// </summary>
	public class PrintCommand:RDLCommand
	{
		public PrintCommand(StringTokenizer tokenizer):base(tokenizer)
		{
			this.type = CommandType.Print;
			this.argumentKind =  new TokenKind[]{TokenKind.WordOrQuotedString};
			ParseArguments(tokenizer);
		}

		public override string Execute(RDLEngine engine, RDLContext context)
		{			
			StringBuilder output = new StringBuilder();	

			foreach(Token token in this.argumentList)
			{
				switch(token.Kind)
				{
					case TokenKind.Word:

						string[] words = token.Value.Split('.');

						IPrintable symbol = context[words[0]];

						if (symbol!=null)
							output.Append(symbol.GetField(words[1]));
							else
							throw new Exception("Symbol " + token.Value + "not found!");
					break;

					case TokenKind.QuotedString:				

						string quotedString = token.Value.Substring(1,token.Value.Length-2);
						
						quotedString = quotedString.Replace("\\n","\n");
						quotedString = quotedString.Replace("\\r","\r");
						
						output.Append(quotedString);

					break;

					case TokenKind.Number:
						output.Append(token.Value);
					break;				
				}
			}

			return output.ToString();
		}

	}
}
