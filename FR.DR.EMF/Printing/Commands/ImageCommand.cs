using System;
using EMF.Printing.RDL;
using EMF.Printing.Text;


namespace EMF.Printing.Commands
{
	/// <summary>
	/// Summary description for cmdImage.
	/// </summary>
	public class ImageCommand:RDLCommand
	{
		public ImageCommand(StringTokenizer tokenizer):base(tokenizer)
		{
			this.type = CommandType.Image;
			this.argumentKind =  new TokenKind[]{TokenKind.Word};
			ParseArguments(tokenizer);
		}

		public override string Execute(RDLEngine engine, RDLContext context)
		{
			return "";
		}
	}
}
