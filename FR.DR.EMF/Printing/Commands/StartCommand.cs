using System;
using EMF.Printing.RDL;
using EMF.Printing.Text;

namespace EMF.Printing.Commands
{
	/// <summary>
	/// Summary description for cmdStart.
	/// </summary>
	public class StartCommand:RDLCommand
	{

		public StartCommand(StringTokenizer tokenizer):base(tokenizer)
		{
			this.type = CommandType.Start;
			this.argumentKind =  new TokenKind[]{TokenKind.EOL};
		}

		public override string Execute(RDLEngine engine, RDLContext context)
		{
			return "";
		}
	}
}
