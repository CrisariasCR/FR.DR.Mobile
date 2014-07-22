using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using EMF.Printing.Text;
using EMF.Printing.Commands;
using EMF.Printing.Drivers;

namespace EMF.Printing.RDL
{
    public class RDLEngine
    {
        private RDLContext rootContext;
        private IPrinterDriver printerDriver;
        private RDLCommand rootCommand;

        //Con esta variable sabemos si ya se imprimio
        private bool yaSeImprimio;

        public RDLEngine(IPrinterDriver printerDriver)
        {
            this.printerDriver = printerDriver;
            this.rootCommand = null;
            this.yaSeImprimio = false;
        }

        public RDLCommand Parse(string input)
        {
            string[] stringLines;
            StringTokenizer tokenizer;
            Token token;
            Stack commandStack = new Stack();
            RDLCommand rootCommand = null;
            RDLCommand currentNode = null;
            RDLCommand newCommand = null;
            int lineNumber = 0;

            rootCommand = new StartCommand(null);

            currentNode = rootCommand;

            input = input.Replace("\r", "");
            input = input.Replace("\t", "	");
            stringLines = input.Split('\n');

            try
            {
                foreach (string line in stringLines)
                {
                    lineNumber++;
                    tokenizer = new StringTokenizer(line);
                    tokenizer.IgnoreWhiteSpace = true;
                    token = tokenizer.Next();
                    newCommand = null;

                    if (token.Kind == TokenKind.Word)
                    {
                        if (token.Value.ToUpper() == "PRINT")
                        {
                            newCommand = new PrintCommand(tokenizer);
                            currentNode.Next = newCommand;
                            currentNode = newCommand;
                        }
                        else if (token.Value.ToUpper() == "PRINTF")
                        {
                            newCommand = new PrintRowCommand(tokenizer);
                            currentNode.Next = newCommand;
                            currentNode = newCommand;
                        }
                        else if (token.Value.ToUpper() == "FONT")
                        {
                            newCommand = new FontCommand(tokenizer);
                            currentNode.Next = newCommand;
                            currentNode = newCommand;
                        }
                        else if (token.Value.ToUpper() == "DEFINE")
                        {
                            newCommand = new DefineCommand(tokenizer);
                            currentNode.Next = newCommand;
                            currentNode = newCommand;
                        }
                        else if (token.Value.ToUpper() == "FONTSTYLE")
                        {
                            newCommand = new FontStyleCommand(tokenizer);
                            currentNode.Next = newCommand;
                            currentNode = newCommand;
                        }
                        else if (token.Value.ToUpper() == "FOREACH")
                        {
                            newCommand = new ForeachCommand(tokenizer);
                            newCommand.Child = new StartCommand(null);
                            currentNode.Next = newCommand;
                            commandStack.Push(newCommand);
                            currentNode = newCommand.Child;

                        }
                        else if (token.Value.ToUpper() == "IF")
                        {
                            newCommand = new IfCommand(tokenizer);
                            newCommand.Child = new StartCommand(null);
                            currentNode.Next = newCommand;
                            commandStack.Push(newCommand);
                            currentNode = newCommand.Child;

                        }
                        else if (token.Value.ToUpper() == "IMAGE")
                        {
                            newCommand = new ImageCommand(tokenizer);
                            currentNode.Next = newCommand;
                            currentNode = newCommand;
                        }
                        else if (token.Value.ToUpper() == "NEXT")
                        {
                            currentNode = (RDLCommand)commandStack.Pop();
                            if (currentNode.Type != CommandType.ForEach)
                                throw new Exception("FOREACH expected");
                        }
                        else if (token.Value.ToUpper() == "ENDIF")
                        {
                            currentNode = (RDLCommand)commandStack.Pop();

                            if (currentNode.Type != CommandType.If)
                                throw new Exception("IF expected");

                        }
                        else throw new Exception("Command unknown");
                    }
                    else if (token.Kind != TokenKind.EOF && token.Kind != TokenKind.EOL)
                    {
                        throw new Exception("Command expected");
                    }

                    if (newCommand != null)
                    {
                        token = tokenizer.Next();

                        while (token.Kind != TokenKind.EOF && token.Kind != TokenKind.EOL)
                        {
                            newCommand.AddArgument(token);
                            token = tokenizer.Next();
                        }
                    }

                }
            }
            catch (Exception e)
            {
                throw new Exception("LINE " + lineNumber.ToString() + " : " + e.Message);
            }

            return rootCommand;
        }

        public IPrinterDriver PrinterDriver
        {
            get
            {
                return this.printerDriver;
            }
        }

		public static string EliminaAcentos(string texto)	
		{
			string textoOriginal = texto;//transformación UNICODE
			string textoNormalizado = textoOriginal.Normalize(NormalizationForm.FormD);
			return textoNormalizado;
		}

		public static string RemoveAccentsWithRegEx(string inputString)
		{
			Regex replace_a_Accents = new Regex("[á|à|ä|â]", RegexOptions.Compiled);
			Regex replace_e_Accents = new Regex("[é|è|ë|ê]", RegexOptions.Compiled);
			Regex replace_i_Accents = new Regex("[í|ì|ï|î]", RegexOptions.Compiled);
			Regex replace_o_Accents = new Regex("[ó|ò|ö|ô]", RegexOptions.Compiled);
			Regex replace_u_Accents = new Regex("[ú|ù|ü|û]", RegexOptions.Compiled);
			Regex replace_ñ_Accents = new Regex("[ñ]", RegexOptions.Compiled);
			Regex replace_Ñ_Accents = new Regex("[Ñ]", RegexOptions.Compiled);
			inputString = replace_a_Accents.Replace(inputString, "a");
			inputString = replace_e_Accents.Replace(inputString, "e");
			inputString = replace_i_Accents.Replace(inputString, "i");
			inputString = replace_o_Accents.Replace(inputString, "o");
			inputString = replace_u_Accents.Replace(inputString, "u");
			inputString = replace_ñ_Accents.Replace(inputString, "n");
			inputString = replace_Ñ_Accents.Replace(inputString, "N");
			return inputString;
		}

        public string Print(string input, ArrayList simbolosIniciales)
        {
            //Volvemos a crear el contexto
            this.rootContext = new RDLContext(null);

            //Agregamos los simbolos iniciales para la impresion
            for (int i = 0; i < simbolosIniciales.Count; i++)
            {
                this.rootContext.AddSymbol((EMF.Printing.IPrintable)simbolosIniciales[i]);
            }
            
            //Si no se ha impreso el reporte entonces parsemos la entrada
            if (!yaSeImprimio)
                rootCommand = Parse(input);
            
            this.printerDriver.Open();
            //LJR R2SP2
            string printText = Execute(rootCommand, rootContext);
			printText = RemoveAccentsWithRegEx (printText);
            //Remueve el Encabezado
            int index = printText.IndexOf("wP");
            if (index != -1)
            {
                string temp = printText.Substring(0, index - 1);
                printText = temp + printText.Substring(index + 3);
            }
            this.printerDriver.Print(printText);
            System.Threading.Thread.Sleep(2500);
            this.printerDriver.Close();
            return printText;
        }

        public string PrintAll(string input, ArrayList simbolosIniciales)
        {
            //Volvemos a crear el contexto
            this.rootContext = new RDLContext(null);

            //Agregamos los simbolos iniciales para la impresion
            for (int i = 0; i < simbolosIniciales.Count; i++)
            {
                this.rootContext.AddSymbol((EMF.Printing.IPrintable)simbolosIniciales[i]);
            }

            //Si no se ha impreso el reporte entonces parsemos la entrada
            if (!yaSeImprimio)
                rootCommand = Parse(input);

            //this.printerDriver.Open();
            //LJR R2SP2
            string printText = Execute(rootCommand, rootContext);
            printText = RemoveAccentsWithRegEx(printText);
            //Remueve el Encabezado
            int index = printText.IndexOf("wP");           
            if (index != -1)
            {
                string temp = printText.Substring(0, index - 1);
                printText = temp + printText.Substring(index + 3);
            }
            //this.printerDriver.Print(printText);
            //this.printerDriver.Close();
            return printText;
        }

        public bool PrintText(string input)
        {
            try
            {
                this.printerDriver.Open();
                this.printerDriver.Print(input);
                this.printerDriver.Close();            
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public string Execute(RDLCommand rootCommand, RDLContext parentContext)
        {
            RDLCommand currentCommand = rootCommand;
            StringBuilder output = new StringBuilder();

            try
            {
                while (currentCommand != null)
                {
                    output.Append(currentCommand.Execute(this, parentContext));
                    currentCommand = currentCommand.Next;
                }
            }
            catch (Exception e)
            {
                output.Append("Runtime error: " + e.Message);
            }

            return output.ToString();
        }

    }

}