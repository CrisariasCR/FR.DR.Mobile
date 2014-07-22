using System;
using System.Collections;
using System.Linq;
using System.Text;
using EMF.Printing.Text;

namespace EMF.Printing.RDL
{
    /// <summary>
    /// Summary description for RDLContext.
    /// </summary>
    public class RDLContext
    {
        private RDLContext parent;
        private Hashtable symbols;

        public RDLContext(RDLContext parent)
        {
            this.parent = parent;
            this.symbols = new Hashtable();
        }

        public IPrintable this[string symbolName]
        {
            get
            {
                object returnValue;

                returnValue = this.symbols[symbolName];

                if (returnValue == null)
                {
                    if (this.parent != null)
                        return parent[symbolName];
                    else
                        return null;
                }
                else
                    return (IPrintable)returnValue;
            }
        }

        public void AddSymbol(IPrintable symbol)
        {
            try
            {
                this.symbols.Add(symbol.GetObjectName(), symbol);
            }
            catch
            {
                throw new Exception("Simbolo '" + symbol.GetObjectName() + "' ya definido");
            }
        }

        public void AddSymbol(string symbolName, IPrintable symbol)
        {
            this.symbols.Add(symbolName, symbol);
        }
    }
}