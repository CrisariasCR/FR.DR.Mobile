using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EMF.WF
{
    public enum ButtonContentAlignment
    {
        MiddleLeft
    }

    public enum TextBoxInput
    {
        Calculator,
        None,
    }

    public enum TextBoxBorder
    {
        DottedUnderLine,
        DashedUnderLine,
        UnderLine,
        None,
    }

    public class EMFTextbox// : TextBox
    {
#pragma warning disable 109
        public bool AutoNextFocus { get; set; }
        public new TextBoxBorder BorderStyle { get; set; }
        public TextBoxInput Input { get; set; }
        public bool MultiTeclado { get; set; }
        public string TituloTeclado { get; set; }
        public decimal Value { get; set; }
        public string Format { get; set; }
        public string Text { get; set; }
    }


}
