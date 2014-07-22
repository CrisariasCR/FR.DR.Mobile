using System;
using System.Collections;
using System.Text;
using EMF.Printing.RDL;
using EMF.Printing.Text;
using EMF.Printing.Symbols;

namespace EMF.Printing.Commands
{
	/// <summary>
	/// Summary description for cmdPrint.
	/// </summary>
	public class PrintRowCommand:RDLCommand
	{
		public PrintRowCommand(StringTokenizer tokenizer):base(tokenizer)
		{
			this.type = CommandType.PrintF;
			this.argumentKind =  new TokenKind[]{TokenKind.WordOrQuotedString};
			ParseArguments(tokenizer);

		}

		public override string Execute(RDLEngine engine, RDLContext context)
		{			
			StringBuilder output = new StringBuilder();	
			RowFormatSymbol rowFormatValue = null;
			ArrayList rowFormat = new ArrayList();
			Token symbolName;
			int formatIndex=0;
			int tokenIndex=0;

			symbolName = (Token)this.argumentList[0];

			rowFormatValue = (RowFormatSymbol)context[symbolName.Value];

			if (rowFormatValue!=null)
				rowFormat = (ArrayList)rowFormatValue.GetField("VALUES");
			else
				throw new Exception("Symbol " + symbolName.Value + "not found!");

			foreach(Token token in this.argumentList)
			{
				if (tokenIndex>0)
				{
					switch(token.Kind)
					{
						case TokenKind.Word:

							string[] words = token.Value.Split('.');

							IPrintable symbol = context[words[0]];

							if (symbol!=null)
							{						
								if (formatIndex<rowFormat.Count)
									output.Append(format(symbol.GetField(words[1]),(string)rowFormat[formatIndex]));
								else
									output.Append(symbol.GetField(words[1]));
							}						
							else
								throw new Exception("Symbol " + token.Value + "not found!");

							break;

						case TokenKind.QuotedString:				

							string quotedString = token.Value.Substring(1,token.Value.Length-2);
						
							quotedString = quotedString.Replace("\\n","\n");
							quotedString = quotedString.Replace("\\r","\r");
						
							if (formatIndex<rowFormat.Count)
								output.Append(format(quotedString,(string)rowFormat[formatIndex]));
							else
								output.Append(quotedString);

							break;

						case TokenKind.Number:

							if (formatIndex<rowFormat.Count)
								output.Append(format(token.Value,(string)rowFormat[formatIndex]));
							else
								output.Append(token.Value);

							break;				
					}

					formatIndex++;
				}

				tokenIndex++;
			}
			
			return output.ToString();
		}

		private string format(object inputValue, string format)
		{
			string outputValue;			
			string[] formatDetails;
			string format1 = string.Empty;
			int  length=0;
			int  alignment=0;

			format = format.Substring(1,format.Length-2);

			formatDetails = format.Split('|');
			
			length = formatDetails[0].Length;

			if (formatDetails.Length>1)
			{
				if (formatDetails[1].ToUpper()=="LEFT")
					alignment = length * -1;
				else
					alignment = length;
			}

			if (inputValue.GetType()==typeof(string))
			{

				string stringValue = (string)inputValue;

				if (stringValue.Length > length)
					stringValue = stringValue.Substring(0,length);

				outputValue = String.Format("{0," + alignment.ToString() + "}",stringValue);

			}
			else
			{
				outputValue = String.Format("{0," + alignment.ToString() + ":" + formatDetails[0] + "}",inputValue);
			}			

			return outputValue;
		}

	}
}
