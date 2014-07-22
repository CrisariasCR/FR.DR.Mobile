using System;
using System.Collections;
using System.Linq;
using System.Text;
using EMF.Printing.Text;

namespace EMF.Printing.RDL
{
    /// <summary>
    /// Summary description for RDLCommand.
    /// </summary>	
    public enum CommandType
    {
        Unknown = 0,
        Start = 1,
        If = 2,
        ForEach = 3,
        Print = 4,
        PrintF = 5,
        Font = 5,
        FontStyle,
        Image = 5,
        Define = 6
    }

    /// <summary>
    /// Summary description for RDLCommand.
    /// </summary>	
    public abstract class RDLCommand
    {
        protected ArrayList argumentList = new ArrayList();
        protected RDLCommand next = null;
        protected RDLCommand child = null;
        protected CommandType type = CommandType.Unknown;
        protected TokenKind[] argumentKind;

        public RDLCommand(StringTokenizer tokenizer) { }

        protected void ParseArguments(StringTokenizer tokenizer)
        {
            Token argument;
            int argumentCount = 0;

            argument = tokenizer.Next();

            while (argument.Kind != TokenKind.EOF && argument.Kind != TokenKind.EOL)
            {
                if (argumentKind[argumentCount] == argument.Kind)
                {

                    this.argumentList.Add(argument);
                    argumentCount++;
                    argument = tokenizer.Next();
                }
                else
                {
                    if (argumentKind[argumentCount] == TokenKind.WordOrQuotedString && (argument.Kind == TokenKind.Word || argument.Kind == TokenKind.QuotedString))
                    {
                        this.argumentList.Add(argument);
                        argument = tokenizer.Next();

                    }
                    else
                        throw new Exception("Invalid argument type");
                }
            }

            //			if (argumentCount>argumentKind.Length&&argumentKind[0]!=TokenKind.WordOrQuotedString)
            //				throw new Exception("Invalid argument number");
            //
            //			if (argumentCount<argumentKind.Length&&argumentKind[0]!=TokenKind.WordOrQuotedString)
            //				throw new Exception("Invalid argument number");			
        }

        public void AddArgument(object argument)
        {
            this.argumentList.Add(argument);
        }

        public RDLCommand Next
        {
            get
            {
                return this.next;
            }

            set
            {

                this.next = value;
            }
        }

        public CommandType Type
        {
            get
            {
                return this.type;
            }
        }

        public RDLCommand Child
        {
            get
            {
                return this.child;
            }

            set
            {

                this.child = value;
            }
        }

        public abstract string Execute(RDLEngine engine, RDLContext context);
    }

}