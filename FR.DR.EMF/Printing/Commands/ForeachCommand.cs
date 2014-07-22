using System;
using System.Text;
using System.Collections;
using EMF.Printing.RDL;
using EMF.Printing.Text;

namespace EMF.Printing.Commands
{
	/// <summary>
	/// Summary description for cmdForeach.
	/// </summary>
	public class ForeachCommand:RDLCommand
	{

		public ForeachCommand(StringTokenizer tokenizer):base(tokenizer)
		{
			this.type = CommandType.ForEach;	
			this.argumentKind =  new TokenKind[]{TokenKind.Word,TokenKind.Word,TokenKind.Word};
			ParseArguments(tokenizer);
		}

		public override string Execute(RDLEngine engine, RDLContext context)
		{		
			StringBuilder output = new StringBuilder();
			Token iteratorObject;
			Token iterableObject;
			string[] words;
			string iterableObjectName;
			string iterableFieldName;
			string iteratorName;
			ArrayList forItems;			 
			
			iteratorObject = (Token)this.argumentList[0];
			iteratorName = iteratorObject.Value;

			iterableObject = (Token)this.argumentList[2];
			words = iterableObject.Value.Split('.');
			
			if (words.Length==2)
			{
				iterableObjectName = words[0];
				iterableFieldName =	words[1];

				forItems = (ArrayList)((IPrintable)context[iterableObjectName]).GetField(iterableFieldName);

				if (forItems!=null)
				{
					foreach(IPrintable printable in forItems)
					{	
						RDLContext newContext = new RDLContext(context);
						newContext.AddSymbol(iteratorName,printable);
						output.Append(engine.Execute(this.Child,newContext));
					}

					return output.ToString();
				}
				else			
					throw new Exception("Symbol not found:" + iterableObjectName);

			}else			
				throw new Exception("Symbol not found:" + iterableObject.Value);
		}

	}
}
