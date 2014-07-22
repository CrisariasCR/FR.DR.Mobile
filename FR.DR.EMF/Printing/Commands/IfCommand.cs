using System;
using EMF.Printing.RDL;
using EMF.Printing.Text;

namespace EMF.Printing.Commands
{
	public class IfCommand:RDLCommand
	{
		public IfCommand(StringTokenizer tokenizer):base(tokenizer)
		{
			this.type = CommandType.If;	
			this.argumentKind =  new TokenKind[]{TokenKind.Word};
			ParseArguments(tokenizer);
		}

		public override string Execute(RDLEngine engine, RDLContext context)
		{
			Token condition = (Token)this.argumentList[0];
			string[] words;
			IPrintable symbol;
			bool result;
			
			if (condition.Kind==TokenKind.Word)
			{
				words = condition.Value.Split('.');

				if (words.Length!=2)
					throw new Exception("Invalid condition");

				symbol = context[words[0]];							

				if (symbol!=null)
				{
					try
					{						
						result = bool.Parse(symbol.GetField(words[1]).ToString());
					}
					catch
					{
						throw new Exception("Error evaluating condition");
					}

					if (result)
						return engine.Execute(this.child,context);
						else
						return "";
				}
				else
					throw new Exception("Symbol not found:" + words[0]);
				

			}else throw new Exception("Invalid condition");

		}
	}
}
