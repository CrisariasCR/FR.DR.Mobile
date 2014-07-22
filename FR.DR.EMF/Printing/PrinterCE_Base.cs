using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Runtime.InteropServices;
using System.Drawing;
//using FieldSoftware;
using System.Reflection;
using System.Windows.Forms;
using System.IO;

namespace EMF.Printing
{
    public class PrinterCE : PrinterCE_Base
    {
        // Methods
        public PrinterCE()
        {
            this.PrinterCE_start(PrinterCE_Base.EXCEPTION_LEVEL.ALL);
            base.Init();
        }

        public PrinterCE(PrinterCE_Base.EXCEPTION_LEVEL exclevel)
        {
            this.PrinterCE_start(exclevel);
            base.Init();
            base.TestForException();
        }

        public PrinterCE(string initstr)
        {
            this.PrinterCE_start(PrinterCE_Base.EXCEPTION_LEVEL.ALL);
            base.Init(initstr);
            base.TestForException();
        }

        public PrinterCE(PrinterCE_Base.EXCEPTION_LEVEL exclevel, string initstr)
        {
            this.PrinterCE_start(exclevel);
            base.Init(initstr);
            base.TestForException();
        }

        public double ConvertCoord(double val, PrinterCE_Base.MEASUREMENT_UNITS fromscalemode, PrinterCE_Base.MEASUREMENT_UNITS toscalemode)
        {
            float num = (float)val;
            float retval = 0f;
            PrinterCE_DLL.PRCE_ConvertCoord(ref num, (int)fromscalemode, (int)toscalemode, ref retval, ref this.AbortPrintErr);
            base.TestForException();
            return (double)retval;
        }

        //public void DrawCircle(double x, double y, double radius)
        //{
        //    this.DrawCircle(x, y, radius, (double)1.0);
        //}

        //public void DrawCircle(double x, double y, double radius, double aspect)
        //{
        //    float num = (float)x;
        //    float num2 = (float)y;
        //    float num3 = (float)radius;
        //    float num4 = (float)aspect;
        //    PrinterCE_DLL.PRCE_DrawCircle(ref num, ref num2, ref num3, null, ref num4, ref this.AbortPrintErr);
        //    base.TestForException();
        //}

        //public void DrawCircle(double x, double y, double radius, Color pencolor)
        //{
        //    this.DrawCircle(x, y, radius, pencolor, 1.0);
        //}

        //unsafe
        //public void DrawCircle(double x, double y, double radius, Color pencolor, double aspect)
        //{
        //    float num = (float)x;
        //    float num2 = (float)y;
        //    float num3 = (float)radius;
        //    float num4 = (float)aspect;
        //    int color = base.XvertRGB(pencolor);
        //    PrinterCE_DLL.PRCE_DrawCircle(ref num, ref num2, ref num3, &color, ref num4, ref this.AbortPrintErr);
        //    base.TestForException();
        //}

        //public void DrawEllipse(double x1, double y1, double x2, double y2)
        //{
        //    float num = (float)x1;
        //    float num2 = (float)x2;
        //    float num3 = (float)y1;
        //    float num4 = (float)y2;
        //    PrinterCE_DLL.PRCE_DrawEllipse(ref num, ref num3, ref num2, ref num4, null, ref this.AbortPrintErr);
        //    base.TestForException();
        //}

        //public /*unsafe*/ void DrawEllipse(double x1, double y1, double x2, double y2, Color pencolor)
        //{
        //    float num = (float)x1;
        //    float num2 = (float)x2;
        //    float num3 = (float)y1;
        //    float num4 = (float)y2;
        //    int color = base.XvertRGB(pencolor);
        //    PrinterCE_DLL.PRCE_DrawEllipse(ref num, ref num3, ref num2, ref num4, ref color, ref this.AbortPrintErr);
        //    base.TestForException();
        //}

        public void DrawLine(double x1, double y1, double x2, double y2)
        {
            float num = (float)x1;
            float num3 = (float)x2;
            float num2 = (float)y1;
            float num4 = (float)y2;
            PrinterCE_DLL.PRCE_DrawLine(ref num, ref num2, ref num3, ref num4, ref this.AbortPrintErr);
            base.TestForException();
        }

        public /*unsafe*/ void DrawLine(double x1, double y1, double x2, double y2, Color pencolor)
        {
            float num = (float)x1;
            float num3 = (float)x2;
            float num2 = (float)y1;
            float num4 = (float)y2;
            int color = base.XvertRGB(pencolor);
            PrinterCE_DLL.PRCE_DrawLine(ref num, ref num2, ref num3, ref num4, ref color, ref this.AbortPrintErr);
            base.TestForException();
        }

        public void DrawPicture(string picture, double x, double y)
        {
            this.DrawPicture(picture, x, y, 0.0, 0.0, true);
        }

        public void DrawPicture(string picture, double x, double y, double width, double height, bool keepaspect)
        {
            float num = (float)x;
            float num2 = (float)y;
            float num3 = (float)width;
            float num4 = (float)height;
            PrinterCE_DLL.PRCE_DrawPicture(picture, ref num, ref num2, ref num3, ref num4, !keepaspect ? 0 : 1, ref this.AbortPrintErr);
            base.TestForException();
        }

        //public void DrawPoint(double x, double y)
        //{
        //    float num = (float)x;
        //    float num2 = (float)y;
        //    PrinterCE_DLL.PRCE_DrawPoint(ref num, ref num2, null, ref this.AbortPrintErr);
        //    base.TestForException();
        //}

        //public /*unsafe*/ void DrawPoint(double x, double y, Color pencolor)
        //{
        //    float num = (float)x;
        //    float num2 = (float)y;
        //    int num3 = base.XvertRGB(pencolor);
        //    PrinterCE_DLL.PRCE_DrawPoint(ref num, ref num2, &num3, ref this.AbortPrintErr);
        //    base.TestForException();
        //}

        //public void DrawRect(double x1, double y1, double x2, double y2)
        //{
        //    float num = (float)x1;
        //    float num2 = (float)x2;
        //    float num3 = (float)y1;
        //    float num4 = (float)y2;
        //    PrinterCE_DLL.PRCE_DrawRect(ref num, ref num3, ref num2, ref num4, null, ref this.AbortPrintErr);
        //    base.TestForException();
        //}

        //public /*unsafe*/ void DrawRect(double x1, double y1, double x2, double y2, Color pencolor)
        //{
        //    float num = (float)x1;
        //    float num2 = (float)x2;
        //    float num3 = (float)y1;
        //    float num4 = (float)y2;
        //    int num5 = base.XvertRGB(pencolor);
        //    PrinterCE_DLL.PRCE_DrawRect(ref num, ref num3, ref num2, ref num4, &num5, ref this.AbortPrintErr);
        //    base.TestForException();
        //}

        //public void DrawRoundedRect(double x1, double y1, double x2, double y2, double corner_width, double corner_height)
        //{
        //    float num = (float)x1;
        //    float num2 = (float)x2;
        //    float num3 = (float)y1;
        //    float num4 = (float)y2;
        //    float num5 = (float)corner_width;
        //    float num6 = (float)corner_height;
        //    PrinterCE_DLL.PRCE_DrawRoundedRect(ref num, ref num3, ref num2, ref num4, ref num5, ref num6, null, ref this.AbortPrintErr);
        //    base.TestForException();
        //}

        //public /*unsafe*/ void DrawRoundedRect(double x1, double y1, double x2, double y2, double corner_width, double corner_height, Color pencolor)
        //{
        //    float num = (float)x1;
        //    float num2 = (float)x2;
        //    float num3 = (float)y1;
        //    float num4 = (float)y2;
        //    float num5 = (float)corner_width;
        //    float num6 = (float)corner_height;
        //    int num7 = base.XvertRGB(pencolor);
        //    PrinterCE_DLL.PRCE_DrawRoundedRect(ref num, ref num3, ref num2, ref num4, ref num5, ref num6, &num7, ref this.AbortPrintErr);
        //    base.TestForException();
        //}

        public void DrawText(string drawstr)
        {
            PrinterCE_DLL.PRCE_DrawText(drawstr, 0, ref this.AbortPrintErr);
            base.TestForException();
        }

        //public void DrawText(string drawstr, int ccnt)
        //{
        //    PrinterCE_DLL.PRCE_DrawText(drawstr, null, null, ccnt, ref this.AbortPrintErr);
        //    base.TestForException();
        //}

        public /*unsafe*/ void DrawText(string drawstr, double x, double y)
        {
            float fx = (float)x;
            float fy = (float)y;
            PrinterCE_DLL.PRCE_DrawText(drawstr, ref fx, ref fy, 0, ref this.AbortPrintErr);
            base.TestForException();
        }

        //public /*unsafe*/ void DrawText(string drawstr, double x, double y, int ccnt)
        //{
        //    if (drawstr != null)
        //    {
        //        float fx = (float)x;
        //        float fy = (float)y;
        //        PrinterCE_DLL.PRCE_DrawText(drawstr, &fx, &fy, ccnt, ref this.AbortPrintErr);
        //        base.TestForException();
        //    }
        //}

        //public int DrawTextFlow(string drawstr)
        //{
        //    if (drawstr == null)
        //    {
        //        return 0;
        //    }
        //    int num = PrinterCE_DLL.PRCE_DrawTextFlowBasic(drawstr, -1, 0, ref this.AbortPrintErr);
        //    base.TestForException();
        //    return num;
        //}

        //public int DrawTextFlow(string drawstr, FLOW_OPTIONS flowtype)
        //{
        //    if (drawstr == null)
        //    {
        //        return 0;
        //    }
        //    int num = PrinterCE_DLL.PRCE_DrawTextFlowBasic(drawstr, -1, (int)flowtype, ref this.AbortPrintErr);
        //    base.TestForException();
        //    return num;
        //}

        //public int DrawTextFlow(string drawstr, double left, double top, double width, double height, int ccnt, FLOW_OPTIONS flowtype)
        //{
        //    return this.DrawTextFlow(drawstr, -1.0, -1.0, left, top, width, height, ccnt, flowtype);
        //}

        //public int DrawTextFlow(string drawstr, double x, double y, double left, double top, double width, double height, int ccnt, FLOW_OPTIONS flowtype)
        //{
        //    if (drawstr == null)
        //    {
        //        return 0;
        //    }
        //    float fxcoord = (float)x;
        //    float fycoord = (float)y;
        //    float fleft = (float)left;
        //    float ftop = (float)top;
        //    float fwidth = (float)width;
        //    float fheight = (float)height;
        //    int num = PrinterCE_DLL.PRCE_DrawTextFlow(drawstr, ref fxcoord, ref fycoord, ref fleft, ref ftop, ref fwidth, ref fheight, ccnt, (int)flowtype, ref this.AbortPrintErr);
        //    base.TestForException();
        //    return num;
        //}

        public void EndDoc()
        {
            PrinterCE_DLL.PRCE_EndDoc(ref this.AbortPrintErr);
            base.TestForException();
        }

        public bool GetPictureDims(string picture, out double width, out double height)
        {
            float x = 0f;
            float y = 0f;
            float num3 = 0f;
            float num4 = 0f;
            PrinterCE_DLL.PRCE_DrawPicture(picture, ref x, ref y, ref num3, ref num4, 2, ref this.AbortPrintErr);
            width = num3;
            height = num4;
            base.TestForException();
            if (num3 == 0f)
            {
                return (num4 != 0f);
            }
            return true;
        }

        public double GetStringWidth(string drawstr)
        {
            float retval = 0f;
            PrinterCE_DLL.PRCE_get_GetStringWidth(drawstr, ref retval, ref this.AbortPrintErr);
            base.TestForException();
            return (double)retval;
        }

        public void KillDoc()
        {
            PrinterCE_DLL.PRCE_KillDoc(ref this.AbortPrintErr);
            base.TestForException();
        }

        public void NewPage()
        {
            PrinterCE_DLL.PRCE_NewPage(ref this.AbortPrintErr);
            base.TestForException();
        }

        protected void PrinterCE_start(PrinterCE_Base.EXCEPTION_LEVEL exclevel)
        {
            if (base.PrBase_FirstInit(exclevel))
            {
                PrinterCE_DLL.PRCE_StartUp(ref this.AbortPrintErr);
            }
        }

        public void PrSetDefaults()
        {
            PrinterCE_DLL.PRCE_PrSetDefaults(ref this.AbortPrintErr);
            base.TestForException();
        }

        public bool SelectPrinter(bool StartPrinting)
        {
            PrinterCE_DLL.PRCE_SelectPrinter(StartPrinting ? 1 : 0, ref this.AbortPrintErr);
            base.TestForException();
            return (base.AbortPrintErr == 0);
        }

        public void ShutDown()
        {
            PrinterCE_DLL.PRCE_ShutDown(ref this.AbortPrintErr);
            base.TestForException();
        }

        // Properties
        public DRAWSTYLE DrawStyle
        {
            get
            {
                return (DRAWSTYLE)PrinterCE_DLL.PRCE_get_DrawStyle();
            }
            set
            {
                PrinterCE_DLL.PRCE_set_DrawStyle((int)value);
            }
        }

        public double DrawWidth
        {
            get
            {
                float retval = 0f;
                PrinterCE_DLL.PRCE_get_DrawWidth(ref retval);
                return (double)retval;
            }
            set
            {
                float newVal = (float)value;
                PrinterCE_DLL.PRCE_set_DrawWidth(ref newVal);
            }
        }

        public Color FillColor
        {
            get
            {
                return base.XvertFromRGB(PrinterCE_DLL.PRCE_get_FillColor());
            }
            set
            {
                PrinterCE_DLL.PRCE_set_FillColor(base.XvertRGB(value));
            }
        }

        public FILL_STYLE FillStyle
        {
            get
            {
                return (FILL_STYLE)PrinterCE_DLL.PRCE_get_FillStyle();
            }
            set
            {
                PrinterCE_DLL.PRCE_set_FillStyle((int)value, ref this.AbortPrintErr);
                base.TestForException();
            }
        }

        public bool FontBold
        {
            get
            {
                return (PrinterCE_DLL.PRCE_get_FontBold() != 0);
            }
            set
            {
                PrinterCE_DLL.PRCE_set_FontBold(value ? 1 : 0);
            }
        }

        public int FontBoldVal
        {
            get
            {
                return PrinterCE_DLL.PRCE_get_FontBoldVal();
            }
            set
            {
                PrinterCE_DLL.PRCE_set_FontBoldVal(value);
            }
        }

        public bool FontItalic
        {
            get
            {
                return (PrinterCE_DLL.PRCE_get_FontItalic() != 0);
            }
            set
            {
                PrinterCE_DLL.PRCE_set_FontItalic(value ? 1 : 0);
            }
        }

        public string FontName
        {
            get
            {
                IntPtr rbz = new IntPtr(0);
                PrinterCE_DLL.PRCE_get_FontName(ref rbz);
                return Marshal.PtrToStringUni(rbz);
            }
            set
            {
                PrinterCE_DLL.PRCE_set_FontName(value, ref this.AbortPrintErr);
                base.TestForException();
            }
        }

        public int FontSize
        {
            get
            {
                return PrinterCE_DLL.PRCE_get_FontSize();
            }
            set
            {
                PrinterCE_DLL.PRCE_set_FontSize(value, ref this.AbortPrintErr);
                base.TestForException();
            }
        }

        public bool FontStrikethru
        {
            get
            {
                return (PrinterCE_DLL.PRCE_get_FontStrikethru() != 0);
            }
            set
            {
                PrinterCE_DLL.PRCE_set_FontStrikethru(value ? 1 : 0);
            }
        }

        public bool FontUnderline
        {
            get
            {
                return (PrinterCE_DLL.PRCE_get_FontUnderline() != 0);
            }
            set
            {
                PrinterCE_DLL.PRCE_set_FontUnderline(value ? 1 : 0);
            }
        }

        public Color ForeColor
        {
            get
            {
                return base.XvertFromRGB(PrinterCE_DLL.PRCE_get_ForeColor());
            }
            set
            {
                PrinterCE_DLL.PRCE_set_ForeColor(base.XvertRGB(value));
            }
        }

        public double GetStringHeight
        {
            get
            {
                float retval = 0f;
                PrinterCE_DLL.PRCE_get_GetStringHeight(ref retval, ref this.AbortPrintErr);
                base.TestForException();
                return (double)retval;
            }
        }

        public bool IsColor
        {
            get
            {
                return (PrinterCE_DLL.PRCE_get_IsColor() != 0);
            }
        }

        public int PgIndentLeft
        {
            get
            {
                return PrinterCE_DLL.PRCE_get_PgIndentLeft();
            }
            set
            {
                PrinterCE_DLL.PRCE_set_PgIndentLeft(value);
            }
        }

        public int PgIndentTop
        {
            get
            {
                return PrinterCE_DLL.PRCE_get_PgIndentTop();
            }
            set
            {
                PrinterCE_DLL.PRCE_set_PgIndentTop(value);
            }
        }

        public double PrBottomMargin
        {
            get
            {
                float retval = 0f;
                PrinterCE_DLL.PRCE_get_PrBottomMargin(ref retval);
                return (double)retval;
            }
            set
            {
                float newVal = (float)value;
                PrinterCE_DLL.PRCE_set_PrBottomMargin(ref newVal, ref this.AbortPrintErr);
                base.TestForException();
            }
        }

        public int PrinterResolution
        {
            get
            {
                return PrinterCE_DLL.PRCE_get_PrinterResolution();
            }
        }

        public double PrLeftMargin
        {
            get
            {
                float retval = 0f;
                PrinterCE_DLL.PRCE_get_PrLeftMargin(ref retval);
                return (double)retval;
            }
            set
            {
                float newVal = (float)value;
                PrinterCE_DLL.PRCE_set_PrLeftMargin(ref newVal, ref this.AbortPrintErr);
                base.TestForException();
            }
        }

        public PrinterCE_Base.ORIENTATION PrOrientation
        {
            get
            {
                return (PrinterCE_Base.ORIENTATION)PrinterCE_DLL.PRCE_get_PrOrientation();
            }
            set
            {
                if (value != PrinterCE_Base.ORIENTATION.USE_CURRENT)
                {
                    PrinterCE_DLL.PRCE_set_PrOrientation((int)value, ref this.AbortPrintErr);
                    base.TestForException();
                }
            }
        }

        public PrinterCE_Base.PAPER_SELECTION PrPaperSelection
        {
            get
            {
                return (PrinterCE_Base.PAPER_SELECTION)PrinterCE_DLL.PRCE_get_PrPaperSelection();
            }
            set
            {
                if (value != PrinterCE_Base.PAPER_SELECTION.USE_CURRENT)
                {
                    PrinterCE_DLL.PRCE_set_PrPaperSelection((int)value);
                }
            }
        }

        public double PrPgHeight
        {
            get
            {
                float retval = 0f;
                PrinterCE_DLL.PRCE_get_PrPgHeight(ref retval, ref this.AbortPrintErr);
                base.TestForException();
                return (double)retval;
            }
        }

        public double PrPgWidth
        {
            get
            {
                float retval = 0f;
                PrinterCE_DLL.PRCE_get_PrPgWidth(ref retval, ref this.AbortPrintErr);
                base.TestForException();
                return (double)retval;
            }
        }

        public PrinterCE_Base.PRINT_QUALITY PrPrintQuality
        {
            get
            {
                return (PrinterCE_Base.PRINT_QUALITY)PrinterCE_DLL.PRCE_get_PrPrintQuality();
            }
            set
            {
                PrinterCE_DLL.PRCE_set_PrPrintQuality((int)value, ref this.AbortPrintErr);
                base.TestForException();
            }
        }

        public double PrRightMargin
        {
            get
            {
                float retval = 0f;
                PrinterCE_DLL.PRCE_get_PrRightMargin(ref retval);
                return (double)retval;
            }
            set
            {
                float newVal = (float)value;
                PrinterCE_DLL.PRCE_set_PrRightMargin(ref newVal, ref this.AbortPrintErr);
                base.TestForException();
            }
        }

        public double PrTopMargin
        {
            get
            {
                float retval = 0f;
                PrinterCE_DLL.PRCE_get_PrTopMargin(ref retval);
                return (double)retval;
            }
            set
            {
                float newVal = (float)value;
                PrinterCE_DLL.PRCE_set_PrTopMargin(ref newVal, ref this.AbortPrintErr);
                base.TestForException();
            }
        }

        public ROTATION_TYPE Rotation
        {
            get
            {
                return (ROTATION_TYPE)PrinterCE_DLL.PRCE_get_Rotation();
            }
            set
            {
                PrinterCE_DLL.PRCE_set_Rotation((int)value, ref this.AbortPrintErr);
                base.TestForException();
            }
        }

        public double TextX
        {
            get
            {
                float retval = 0f;
                PrinterCE_DLL.PRCE_get_TextX(ref retval, ref this.AbortPrintErr);
                base.TestForException();
                return (double)retval;
            }
            set
            {
                float newVal = (float)value;
                PrinterCE_DLL.PRCE_set_TextX(ref newVal, ref this.AbortPrintErr);
                base.TestForException();
            }
        }

        public double TextY
        {
            get
            {
                float retval = 0f;
                PrinterCE_DLL.PRCE_get_TextY(ref retval, ref this.AbortPrintErr);
                base.TestForException();
                return (double)retval;
            }
            set
            {
                float newVal = (float)value;
                PrinterCE_DLL.PRCE_set_TextY(ref newVal, ref this.AbortPrintErr);
                base.TestForException();
            }
        }

        // Nested Types
        public enum DRAWSTYLE
        {
            SOLID,
            DASHED
        }

        public enum FILL_STYLE
        {
            SOLID,
            TRANSPARENT
        }

        public enum FLOW_OPTIONS
        {
            DEFAULT = 0,
            HARDBREAK = 0x40,
            NOENDINGLINEFEED = 0x20,
            NOFORMFEED = 0x10,
            NOWORDWRAP = 1
        }

        public enum ROTATION_TYPE
        {
            NORTH,
            EAST,
            SOUTH,
            WEST
        }
    }

    public class PrinterCE_Base : IDisposable
    {
        // Fields
        protected int AbortPrintErr = 0;
        protected EXCEPTION_LEVEL CurExceptionLevel = EXCEPTION_LEVEL.ALL;
        protected bool disposed = false;
        private ENABLED_TYPE enabledtype = ENABLED_TYPE.ENABLED_NONE;
        public const int NIL = -1;
        public const int PRINTERCE_NETCF_BUILD = 5;
        public const int PRINTERCE_NETCF_MAJOR = 1;
        public const int PRINTERCE_NETCF_MINOR = 2;
        public const int PRINTERCE_NETCF_REVISION = 5;
        public static string VERSION_STRING = "PrinterCE.NetCF v1.2.5.5 (c) FieldSoftware Products 2003-2007";

        // Methods
        protected PrinterCE_Base()
        {
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    PrinterCE_DLL.PRCE_ShutDown(ref this.AbortPrintErr);
                    AsciiCE_DLL.ASC_ShutDown(ref this.AbortPrintErr);
                }
                this.disposed = true;
            }
        }

        ~PrinterCE_Base()
        {
            this.Dispose(false);
        }

        public void GetVersion(out int major, out int minor, out int build, out int revision)
        {
            PrinterCE_DLL.PRCE_GetVersion(out major, out minor, out build, out revision);
        }

        protected bool Init()
        {
            return this.Init(null);
        }

        protected bool Init(string str)
        {
            if (this.AbortPrintErr != 0)
            {
                return false;
            }
            this.enabledtype = (ENABLED_TYPE)PrinterCE_DLL.PRCE_Init(str, ref this.AbortPrintErr);
            this.TestForException();
            return (this.enabledtype != ENABLED_TYPE.ENABLED_NONE);
        }

        protected bool PrBase_FirstInit(EXCEPTION_LEVEL exclevel)
        {
            this.AbortPrintErr = 0;
            this.CurExceptionLevel = exclevel;
            string text = null;
            Version version = Assembly.GetExecutingAssembly().GetName().Version;
            if (((1 != version.Major) || (2 != version.Minor)) || ((5 != version.Build) || (5 != version.Revision)))
            {
                // TODO
                // MessageBox.Show("Caution: AssemblyVersion not same as internal version number... notify techsupport@fieldsoftware.com");
            }
            string directoryName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase);
            string strA = @"file:\";
            if (string.Compare(strA, 0, directoryName, 0, strA.Length) == 0)
            {
                directoryName = directoryName.Remove(0, strA.Length);
            }
            if (!PrinterCE_DLL.LookForDLL(directoryName))
            {
                text = "PrCE_NetCF.dll not found";
            }
            else
            {
                try
                {
                    int num;
                    int num2;
                    int num3;
                    int num4;
                    PrinterCE_DLL.PRCE_GetVersion(out num, out num2, out num3, out num4);
                    if (((num != 1) || (num2 != 2)) || (num3 != 5))
                    {
                        text = string.Format("PrinterCE.NetCF dll versions do not match:\n\r-PrinterCE.NetCF.dll v{0:0}.{1:0}.{2:0}.{3:0}\n\r-PrCE_NetCF.dll v{4:0}.{5:0}.{6:0}.{7:0}", new object[] { 1, 2, 5, 5, num, num2, num3, num4 });
                    }
                }
                catch (MissingMethodException)
                {
                    text = "Invalid or incorrect version of PrCE_NetCF.dll";
                }
                catch (DllNotFoundException)
                {
                    text = "PrCE_NetCF.dll not found";
                }
            }
            if (text != null)
            {
                //todo
                //MessageBox.Show(text, "PrinterCE.NetCF Error");
                this.AbortPrintErr = 3;
                this.TestForException();
                return false;
            }
            return true;
        }

        public void PrDialogBoxText(string MainText, string TitleText, string CancelBtnText)
        {
            PrinterCE_DLL.PRCE_PrDialogBoxText(MainText, TitleText, CancelBtnText, ref this.AbortPrintErr);
            this.TestForException();
        }

        public void SetupNetIPPrinter(string netstr, int port, bool OpenPort)
        {
            PrinterCE_DLL.PRCE_SetupIPprinter(netstr, port, OpenPort ? 1 : 0, ref this.AbortPrintErr);
            this.TestForException();
        }

        public void SetupNetSharedPrinter(string netstr, bool OpenPort)
        {
            PrinterCE_DLL.PRCE_SetupNetSharedPrinter(netstr, OpenPort ? 1 : 0, ref this.AbortPrintErr);
            this.TestForException();
        }

        public void SetupPaper(PAPER_SELECTION PaperSize, ORIENTATION Orientation)
        {
            float paperWidth = -1f;
            PrinterCE_DLL.PRCE_SetupPaper((int)PaperSize, (int)Orientation, ref paperWidth, ref paperWidth, ref paperWidth, ref paperWidth, ref paperWidth, ref paperWidth, ref this.AbortPrintErr);
            this.TestForException();
        }

        public void SetupPaper(PAPER_SELECTION PaperSize, ORIENTATION Orientation, double LeftMgn, double TopMgn, double RightMgn, double BottomMgn)
        {
            float leftMgn = (float)LeftMgn;
            float topMgn = (float)TopMgn;
            float rightMgn = (float)RightMgn;
            float bottomMgn = (float)BottomMgn;
            float paperWidth = -1f;
            PrinterCE_DLL.PRCE_SetupPaper((int)PaperSize, (int)Orientation, ref paperWidth, ref paperWidth, ref leftMgn, ref topMgn, ref rightMgn, ref bottomMgn, ref this.AbortPrintErr);
            this.TestForException();
        }

        public void SetupPaperCustom(ORIENTATION Orientation, double PaperWidth, double PaperHeight)
        {
            float paperWidth = (float)PaperWidth;
            float paperHeight = (float)PaperHeight;
            float leftMgn = -1f;
            PrinterCE_DLL.PRCE_SetupPaper(5, (int)Orientation, ref paperWidth, ref paperHeight, ref leftMgn, ref leftMgn, ref leftMgn, ref leftMgn, ref this.AbortPrintErr);
            this.TestForException();
        }

        public void SetupPaperCustom(ORIENTATION Orientation, double PaperWidth, double PaperHeight, double LeftMgn, double TopMgn, double RightMgn, double BottomMgn)
        {
            float paperWidth = (float)PaperWidth;
            float paperHeight = (float)PaperHeight;
            float leftMgn = (float)LeftMgn;
            float topMgn = (float)TopMgn;
            float rightMgn = (float)RightMgn;
            float bottomMgn = (float)BottomMgn;
            PrinterCE_DLL.PRCE_SetupPaper(5, (int)Orientation, ref paperWidth, ref paperHeight, ref leftMgn, ref topMgn, ref rightMgn, ref bottomMgn, ref this.AbortPrintErr);
            this.TestForException();
        }

        public void SetupPrinter(PRINTER Printer, PORT Port, bool OpenPort)
        {
            PrinterCE_DLL.PRCE_SetupPrinter((int)Printer, (int)Port, -1, OpenPort ? 1 : 0, ref this.AbortPrintErr);
            this.TestForException();
        }

        public void SetupPrinter(PRINTER Printer, PORT Port, PORT_SPEED Baudrate, bool OpenPort)
        {
            PrinterCE_DLL.PRCE_SetupPrinter((int)Printer, (int)Port, (int)Baudrate, OpenPort ? 1 : 0, ref this.AbortPrintErr);
            this.TestForException();
        }

        public void SetupPrinterOther(FORMFEED_SETTING FFSetting, double FFScroll, DENSITY Density, SERIAL_HANDSHAKE Handshake, BITFLAG BitFlags, COMPRESSION Compression, DITHER Dither, PRINTQUALITY PrintQuality)
        {
            float fFScroll = (float)FFScroll;
            this.TestForException();
            PrinterCE_DLL.PRCE_SetupPrinterOther((int)FFSetting, ref fFScroll, (int)Density, (int)Handshake, (int)BitFlags, (int)Compression, (int)Dither, (int)PrintQuality, ref this.AbortPrintErr);
        }

        protected void TestForException()
        {
            if (this.enabledtype == ENABLED_TYPE.ENABLED_NONE)
            {
                this.AbortPrintErr = 3;
            }
            if (((this.CurExceptionLevel != EXCEPTION_LEVEL.NONE) && (this.AbortPrintErr != 0)) && (((this.AbortPrintErr == 3) || (this.AbortPrintErr == 1)) || (this.CurExceptionLevel == EXCEPTION_LEVEL.ALL)))
            {
                // throw new PrinterCEException(); mvega: para que no de excepcion
            }
        }

        protected Color XvertFromRGB(int cval)
        {
            return Color.FromArgb(cval & 0xff, (cval >> 8) & 0xff, (cval >> 0x10) & 0xff);
        }

        protected int XvertRGB(Color cval)
        {
            return ((cval.R | (cval.G << 8)) | (cval.B << 0x10));
        }

        // Properties
        public string About
        {
            get
            {
                IntPtr szAbout = new IntPtr(0);
                PrinterCE_DLL.PRCE_get_About(ref szAbout, ref this.AbortPrintErr);
                this.TestForException();
                string str = null;
                try
                {
                    str = Marshal.PtrToStringUni(szAbout);
                }
                finally
                {
                    PrinterCE_DLL.PRCE_LocalFreeIntPtr(szAbout);
                }
                return str;
            }
        }

        public ERROR GetLastError
        {
            get
            {
                return (ERROR)PrinterCE_DLL.PRCE_get_LastError();
            }
        }

        public JUSTIFY_HORIZ JustifyHoriz
        {
            get
            {
                return (JUSTIFY_HORIZ)PrinterCE_DLL.PRCE_get_JustifyHoriz();
            }
            set
            {
                PrinterCE_DLL.PRCE_set_JustifyHoriz((int)value, ref this.AbortPrintErr);
                this.TestForException();
            }
        }

        public JUSTIFY_VERT JustifyVert
        {
            get
            {
                return (JUSTIFY_VERT)PrinterCE_DLL.PRCE_get_JustifyVert();
            }
            set
            {
                PrinterCE_DLL.PRCE_set_JustifyVert((int)value, ref this.AbortPrintErr);
                this.TestForException();
            }
        }

        public DIALOGBOX_ACTION PrDialogBox
        {
            get
            {
                return (DIALOGBOX_ACTION)PrinterCE_DLL.PRCE_get_PrDialogBox();
            }
            set
            {
                PrinterCE_DLL.PRCE_set_PrDialogBox((int)value, ref this.AbortPrintErr);
                this.TestForException();
            }
        }

        public MEASUREMENT_UNITS ScaleMode
        {
            get
            {
                return (MEASUREMENT_UNITS)PrinterCE_DLL.PRCE_get_ScaleMode();
            }
            set
            {
                PrinterCE_DLL.PRCE_set_ScaleMode((int)value);
            }
        }

        protected EXCEPTION_LEVEL SetExceptionLevel
        {
            set
            {
                this.CurExceptionLevel = value;
                this.AbortPrintErr = PrinterCE_DLL.PRCE_get_StatusCheck();
                this.TestForException();
            }
        }

        public REPORT_LEVEL SetReportLevel
        {
            set
            {
                PrinterCE_DLL.PRCE_set_SetReportLevel((int)value, ref this.AbortPrintErr);
                this.TestForException();
            }
        }

        //public PRINTER_SETUP SetupPrSettings_All
        //{
        //    get
        //    {
        //        PRINTER_SETUP printer_setup = new PRINTER_SETUP();
        //        int size = Marshal.SizeOf(new DUMMY_PRINTER_SETUP());
        //        IntPtr retptr = new IntPtr(0);
        //        if (PrinterCE_DLL.PRCE_LocalAllocIntPtr(size, ref retptr) == 1)
        //        {
        //            IntPtr sharedNetStr = new IntPtr(0);
        //            IntPtr iPNetStr = new IntPtr(0);
        //            PrinterCE_DLL.PRCE_getFullPrSettings(retptr, ref sharedNetStr, ref iPNetStr, ref this.AbortPrintErr);
        //            this.TestForException();
        //            DUMMY_PRINTER_SETUP dummy_printer_setup = (DUMMY_PRINTER_SETUP)Marshal.PtrToStructure(retptr, typeof(DUMMY_PRINTER_SETUP));
        //            printer_setup.Printer = dummy_printer_setup.Printer;
        //            printer_setup.Port = dummy_printer_setup.Port;
        //            printer_setup.PortSpeed = dummy_printer_setup.PortSpeed;
        //            printer_setup.PaperSelection = dummy_printer_setup.PaperSelection;
        //            printer_setup.Orientation = dummy_printer_setup.Orientation;
        //            printer_setup.FormFeedSetting = dummy_printer_setup.FormFeedSetting;
        //            printer_setup.Density = dummy_printer_setup.Density;
        //            printer_setup.SerialHandshake = dummy_printer_setup.SerialHandshake;
        //            printer_setup.Compression = dummy_printer_setup.Compression;
        //            printer_setup.Dither = dummy_printer_setup.Dither;
        //            printer_setup.PrintQuality = dummy_printer_setup.PrintQuality;
        //            printer_setup.AdjustIR = dummy_printer_setup.AdjustIR;
        //            printer_setup.OtherIR = dummy_printer_setup.OtherIR;
        //            printer_setup.ColorMono = dummy_printer_setup.ColorMono;
        //            printer_setup.Thread = dummy_printer_setup.Thread;
        //            printer_setup.IP_port = dummy_printer_setup.IP_port;
        //            printer_setup.LeftMargin = dummy_printer_setup.LeftMargin;
        //            printer_setup.TopMargin = dummy_printer_setup.TopMargin;
        //            printer_setup.RightMargin = dummy_printer_setup.RightMargin;
        //            printer_setup.BottomMargin = dummy_printer_setup.BottomMargin;
        //            printer_setup.CustomPaperWidth = dummy_printer_setup.CustomPaperWidth;
        //            printer_setup.CustomPaperHeight = dummy_printer_setup.CustomPaperHeight;
        //            printer_setup.FormFeedScrollDistance = dummy_printer_setup.FormFeedScrollDistance;
        //            printer_setup.SharedPrinter_NetStr = Marshal.PtrToStringUni(sharedNetStr);
        //            printer_setup.IP_NetStr = Marshal.PtrToStringUni(iPNetStr);
        //            PrinterCE_DLL.PRCE_LocalFreeIntPtr(retptr);
        //        }
        //        this.TestForException();
        //        return printer_setup;
        //    }
        //    set
        //    {
        //        DUMMY_PRINTER_SETUP structure = new DUMMY_PRINTER_SETUP();
        //        Marshal.SizeOf(structure);
        //        structure.Printer = value.Printer;
        //        structure.Port = value.Port;
        //        structure.PortSpeed = value.PortSpeed;
        //        structure.PaperSelection = value.PaperSelection;
        //        structure.Orientation = value.Orientation;
        //        structure.FormFeedSetting = value.FormFeedSetting;
        //        structure.Density = value.Density;
        //        structure.SerialHandshake = value.SerialHandshake;
        //        structure.Compression = value.Compression;
        //        structure.Dither = value.Dither;
        //        structure.PrintQuality = value.PrintQuality;
        //        structure.AdjustIR = value.AdjustIR;
        //        structure.OtherIR = value.OtherIR;
        //        structure.ColorMono = value.ColorMono;
        //        structure.Thread = value.Thread;
        //        structure.IP_port = value.IP_port;
        //        structure.LeftMargin = value.LeftMargin;
        //        structure.TopMargin = value.TopMargin;
        //        structure.RightMargin = value.RightMargin;
        //        structure.BottomMargin = value.BottomMargin;
        //        structure.CustomPaperWidth = value.CustomPaperWidth;
        //        structure.CustomPaperHeight = value.CustomPaperHeight;
        //        structure.FormFeedScrollDistance = value.FormFeedScrollDistance;
        //        int* retptr = null;
        //        if (PrinterCE_DLL.PRCE_LocalAlloc(Marshal.SizeOf(structure), &retptr) == 1)
        //        {
        //            Marshal.StructureToPtr(structure, (IntPtr)retptr, false);
        //            PrinterCE_DLL.PRCE_setFullPrSettings(retptr, value.SharedPrinter_NetStr, value.IP_NetStr, ref this.AbortPrintErr);
        //            PrinterCE_DLL.PRCE_LocalFree(retptr);
        //        }
        //        this.TestForException();
        //    }
        //}

        public PRINT_STATUS StatusCheck
        {
            get
            {
                if (this.AbortPrintErr == 0)
                {
                    this.AbortPrintErr = PrinterCE_DLL.PRCE_get_StatusCheck();
                }
                return (PRINT_STATUS)this.AbortPrintErr;
            }
        }

        // Nested Types
        public enum ADJUST_IR
        {
            OPTIONAL = 1,
            STANDARD = 0,
            USE_CURRENT = -1
        }

        public enum BITFLAG
        {
            ADJUST_IR = 1,
            ALTERNATE_IR = 2,
            CMYONLY = 8,
            COLOR = 4,
            SINGLETHREAD = 0x40,
            USE_CURRENT = -1
        }

        protected enum CAPABILITIES
        {
            ASCIICE = 0x10,
            BARCODECE = 8,
            PRINTDC = 0x20,
            PRINTERCE = 2
        }

        public enum COLOR_MONO
        {
            CMY_3COLOR = 2,
            CMYK_4COLOR = 1,
            MONOCHROME = 0,
            USE_CURRENT = -1
        }

        public enum COMPRESSION
        {
            NONE = 0,
            STANDARD = 1,
            USE_CURRENT = -1
        }

        public enum DENSITY
        {
            DARKER = 3,
            EXTRADARK = 4,
            EXTRALIGHT = 0,
            LIGHTER = 1,
            NORMAL = 2,
            USE_CURRENT = -1
        }

        public enum DIALOGBOX_ACTION
        {
            DEBUG_OFF = -2,
            DEBUG_ON = -1,
            DISABLE = 2,
            DOWN = 1,
            STATUS = 3,
            UP = 0,
            UP_NOTPERSISTENT = 6
        }

        public enum DITHER
        {
            DIFFUSION = 0,
            DITHER = 1,
            USE_CURRENT = -1
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct DUMMY_PRINTER_SETUP
        {
            public PrinterCE_Base.PRINTER Printer;
            public PrinterCE_Base.PORT Port;
            public PrinterCE_Base.PORT_SPEED PortSpeed;
            public PrinterCE_Base.PAPER_SELECTION PaperSelection;
            public PrinterCE_Base.ORIENTATION Orientation;
            public PrinterCE_Base.FORMFEED_SETTING FormFeedSetting;
            public PrinterCE_Base.DENSITY Density;
            public PrinterCE_Base.SERIAL_HANDSHAKE SerialHandshake;
            public PrinterCE_Base.COMPRESSION Compression;
            public PrinterCE_Base.DITHER Dither;
            public PrinterCE_Base.PRINTQUALITY PrintQuality;
            public PrinterCE_Base.ADJUST_IR AdjustIR;
            public PrinterCE_Base.OTHER_IR OtherIR;
            public PrinterCE_Base.COLOR_MONO ColorMono;
            public PrinterCE_Base.THREAD Thread;
            public int IP_port;
            public double LeftMargin;
            public double TopMargin;
            public double RightMargin;
            public double BottomMargin;
            public double CustomPaperWidth;
            public double CustomPaperHeight;
            public double FormFeedScrollDistance;
            public IntPtr SharedPrinter_NetStr;
            public IntPtr IP_NetStr;
        }

        public enum ENABLED_TYPE
        {
            ENABLED_NONE,
            ENABLED_PRINTERCE,
            ENABLED_BARCODECE
        }

        public enum ERROR
        {
            ABORT = 4,
            BLUETOOTH = 0x11,
            COMMUNICATION_LINK = 10,
            COMMUNICATION_LOST = 11,
            DIALOG = 12,
            FONT = 5,
            IMAGE = 7,
            INFRARED = 9,
            INVALID_ARGUMENT = 3,
            NET_CHECKALL = 15,
            NET_NOSERVER = 0x10,
            NET_NOWNET = 14,
            NETWORK = 13,
            NONE = -1,
            OUTOFMEMORY = 2,
            PRINT_TASK = 0,
            PRINTER_DIALOG = 6,
            PRINTER_SETTINGS = 8,
            USER_CANCELLED = 1
        }

        public enum EXCEPTION_LEVEL
        {
            NONE,
            ABORT_JOB,
            ALL
        }

        public enum FORMFEED_SETTING
        {
            NORMAL = 0,
            PAPERHEIGHT = 1,
            SCROLL = 2,
            USE_CURRENT = -1
        }

        public enum JUSTIFY_HORIZ
        {
            LEFT,
            RIGHT,
            CENTER
        }

        public enum JUSTIFY_VERT
        {
            TOP,
            BOTTOM,
            CENTER,
            BASELINE
        }

        public enum MEASUREMENT_UNITS
        {
            CENTIMETERS = 7,
            INCHES = 5,
            MILLIMETERS = 6,
            PIXELS = 3,
            POINTS = 2,
            TWIPS = 1
        }

        public enum ORIENTATION
        {
            LANDSCAPE = 2,
            PORTRAIT = 1,
            USE_CURRENT = -1
        }

        public enum OTHER_IR
        {
            OPTIONAL = 1,
            STANDARD = 0,
            USE_CURRENT = -1
        }

        public enum PAPER_SELECTION
        {
            A4 = 2,
            B5 = 3,
            CUSTOM = 5,
            LEGAL = 4,
            LETTER = 1,
            USE_CURRENT = -1
        }

        public enum PORT
        {
            ANYCOM_BT = 12,
            BELKIN_BT = 0x10,
            BTQUIKPRINT = 0x12,
            COM1 = 0,
            COM2 = 1,
            COM3 = 4,
            COM4 = 5,
            COM5 = 6,
            COM6 = 7,
            COM7 = 13,
            COM8 = 14,
            COM9 = 0x11,
            COMPAQ_BT = 15,
            INFRARED = 3,
            IPAQ_BT = 15,
            LPT = 2,
            NETIP = 10,
            NETPATH = 9,
            SOCKETCOM_BT = 11,
            TOFILE = 8,
            USE_CURRENT = -1,
            WIDCOMM_BT = 0x10
        }

        public enum PORT_SPEED
        {
            DONT_CARE = -1,
            S_115200 = 5,
            S_19200 = 2,
            S_38400 = 3,
            S_4800 = 0,
            S_57600 = 4,
            S_9600 = 1,
            USE_CURRENT = -1
        }

        public enum PRINT_QUALITY
        {
            DRAFT = 2,
            HIGH = 1
        }

        public enum PRINT_STATUS
        {
            NO_ERROR,
            USER_CANCEL,
            ABORT_OPERATION,
            ABORT_PRINT
        }

        public enum PRINTER
        {
            ABLE_AP1300 = 0x21,
            AXIOHM_A631 = 0x22,
            BROTHER = 0x15,
            CANONBJ300 = 0x16,
            CANONBJ360 = 0,
            CANONBJ600 = 0x25,
            CITIZEN_203 = 0x13,
            CITIZEN_CMP10 = 0x1a,
            CITIZEN_PD04 = 1,
            CITIZEN_PD22 = 0x17,
            CITIZEN_PN60 = 2,
            DYMOCOSTAR = 15,
            ELTRADE = 0x23,
            EPSON_ESCP2 = 3,
            EPSON_STYLUS = 4,
            EPSON_TM_P60 = 0x1f,
            EXTECH_2 = 12,
            EXTECH_3 = 13,
            EXTECH_4 = 0x11,
            FUJITSU_FTP628 = 30,
            GEBE_FLASH = 40,
            GENERIC24_180 = 9,
            GENERIC24_203 = 11,
            GENERIC24_360 = 10,
            HP_PCL = 5,
            INTERMEC = 0x20,
            IPC_PP50 = 0x19,
            IPC_PP55 = 0x24,
            OMNIPRINT = 0x1b,
            ONEIL = 14,
            PANASONIC_JTH200PR = 0x2b,
            PENTAX_200 = 6,
            PENTAX_300 = 7,
            PENTAX_II = 7,
            PENTAX_RUGGEDJET = 0x2f,
            PERIPHERON_NOMAD = 0x2e,
            POCKET_SPECTRUM = 0x2a,
            S_PRINT = 0x1d,
            SATO = 0x29,
            SEIKO_L465 = 0x1c,
            SEIKO3445 = 8,
            SEIKOLABELWRITER = 0x10,
            SIPIX = 0x12,
            TALLY_MIP360 = 0x27,
            TALLY_MTP4 = 0x26,
            USE_CURRENT = -1,
            ZEBRA = 20
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct PRINTER_SETUP
        {
            public PrinterCE_Base.PRINTER Printer;
            public PrinterCE_Base.PORT Port;
            public PrinterCE_Base.PORT_SPEED PortSpeed;
            public PrinterCE_Base.PAPER_SELECTION PaperSelection;
            public PrinterCE_Base.ORIENTATION Orientation;
            public PrinterCE_Base.FORMFEED_SETTING FormFeedSetting;
            public PrinterCE_Base.DENSITY Density;
            public PrinterCE_Base.SERIAL_HANDSHAKE SerialHandshake;
            public PrinterCE_Base.COMPRESSION Compression;
            public PrinterCE_Base.DITHER Dither;
            public PrinterCE_Base.PRINTQUALITY PrintQuality;
            public PrinterCE_Base.ADJUST_IR AdjustIR;
            public PrinterCE_Base.OTHER_IR OtherIR;
            public PrinterCE_Base.COLOR_MONO ColorMono;
            public PrinterCE_Base.THREAD Thread;
            public int IP_port;
            public double LeftMargin;
            public double TopMargin;
            public double RightMargin;
            public double BottomMargin;
            public double CustomPaperWidth;
            public double CustomPaperHeight;
            public double FormFeedScrollDistance;
            public string SharedPrinter_NetStr;
            public string IP_NetStr;
        }

        public enum PRINTQUALITY
        {
            DRAFT = 1,
            STANDARD = 0,
            USE_CURRENT = -1
        }

        public enum REPORT_LEVEL
        {
            ALL_ERRORS,
            NO_ERRORS,
            SERIOUS_ERRORS,
            NO_ERRORS_SKIP_CANCEL
        }

        public enum SERIAL_HANDSHAKE
        {
            DONT_CARE = -1,
            HARDWARE = 1,
            NO_HANDSHAKE = 2,
            SOFTWARE = 0,
            USE_CURRENT = -1
        }

        public enum THREAD
        {
            MULTIPLE = 0,
            SINGLE = 1,
            USE_CURRENT = -1
        }
    }

    public class PrinterCE_DLL
    {
        // Fields unsafe
        //private static void* PrCEptr = null;

        // Methods
        public PrinterCE_DLL()
        {
            //MessageBox.Show("Constructor - PrinterCE_DLL");
        }

        public static bool LookForDLL(string justpath)
        {
            justpath = justpath + @"\PrCE_NetCF.dll";
            bool flag = File.Exists(justpath);
            if (!flag)
            {
                flag = File.Exists(@"\Windows\PrCE_NetCF.dll");
            }
            // mvega: de momento retorna siempre true
            flag = true;
            return flag;
        }

        //[DllImport("PrCE_NetCF.dll", CharSet = CharSet.Unicode)]
        public static /*extern*/ void PRCE_ConvertCoord(ref float val, int fromscalemode, int toscalemode, ref float retval, ref int AbortPrintErr) { }
        //[DllImport("PrCE_NetCF.dll", CharSet = CharSet.Unicode)]
        public static /*extern*/ void PRCE_DrawBitmap(IntPtr bitmap, ref float x, ref float y, ref float width, ref float height, int keepaspect, ref int AbortPrintErr) { }
        //[DllImport("PrCE_NetCF.dll", CharSet = CharSet.Unicode)]
        //public static /*extern*/ /*unsafe*/ void PRCE_DrawCircle(ref float x, ref float y, ref float radius, int* color, ref float aspect, ref int AbortPrintErr) { }
        //[DllImport("PrCE_NetCF.dll", CharSet = CharSet.Unicode)]
        public static /*extern*/ /*unsafe*/ void PRCE_DrawEllipse(ref float x1, ref float y1, ref float x2, ref float y2, ref int color, ref int AbortPrintErr) { }
        //[DllImport("PrCE_NetCF.dll", CharSet = CharSet.Unicode)]
        public static /*extern*/ void PRCE_DrawJustText(string drawstr, ref int AbortPrintErr) { }
        //[DllImport("PrCE_NetCF.dll", CharSet = CharSet.Unicode)]
        public static /*extern*/ /*unsafe*/ void PRCE_DrawLine(ref float x1, ref float y1, ref float x2, ref float y2, ref int AbortPrintErr) { }
        public static /*extern*/ /*unsafe*/ void PRCE_DrawLine(ref float x1, ref float y1, ref float x2, ref float y2, ref int color, ref int AbortPrintErr) { }
        //[DllImport("PrCE_NetCF.dll", CharSet = CharSet.Unicode)]
        public static /*extern*/ void PRCE_DrawPicture(string picture, ref float x, ref float y, ref float width, ref float height, int keepaspect, ref int AbortPrintErr) { }
        //[DllImport("PrCE_NetCF.dll", CharSet = CharSet.Unicode)]
        public static /*extern*/ /*unsafe*/ void PRCE_DrawPoint(ref float x, ref float y, ref int pencolor, ref int AbortPrintErr) { }
        //[DllImport("PrCE_NetCF.dll", CharSet = CharSet.Unicode)]
        public static /*extern*/ /*unsafe*/ void PRCE_DrawRect(ref float x1, ref float y1, ref float x2, ref float y2, ref int pencolor, ref int AbortPrintErr) { }
        //[DllImport("PrCE_NetCF.dll", CharSet = CharSet.Unicode)]
        public static /*extern*/ /*unsafe*/ void PRCE_DrawRoundedRect(ref float x1, ref float y1, ref float x2, ref float y2, ref float corner_width, ref float corner_height, ref int pencolor, ref int AbortPrintErr) { }
        //[DllImport("PrCE_NetCF.dll", CharSet = CharSet.Unicode)]
        public static /*extern*/ /*unsafe*/ void PRCE_DrawText(string drawstr, int ccnt, ref int AbortPrintErr) { }
        public static /*extern*/ /*unsafe*/ void PRCE_DrawText(string drawstr, ref float fx, ref float fy, int ccnt, ref int AbortPrintErr) { }
        //[DllImport("PrCE_NetCF.dll", CharSet = CharSet.Unicode)]
        public static /*extern*/ int PRCE_DrawTextFlow(string pstr, ref float fxcoord, ref float fycoord, ref float fleft, ref float ftop, ref float fwidth, ref float fheight, int ccnt, int SpecialType, ref int AbortPrintErr) { return 0; }
        //[DllImport("PrCE_NetCF.dll", CharSet = CharSet.Unicode)]
        public static /*extern*/ int PRCE_DrawTextFlowBasic(string pstr, int ccnt, int SpecialType, ref int iAbort) { return 0; }
        //[DllImport("PrCE_NetCF.dll", CharSet = CharSet.Unicode)]
        public static /*extern*/ void PRCE_EndDoc(ref int AbortPrintErr) { }
        //[DllImport("PrCE_NetCF.dll", CharSet = CharSet.Unicode)]
        public static /*extern*/ void PRCE_get_About(ref IntPtr szAbout, ref int AbortPrintErr) { }
        //[DllImport("PrCE_NetCF.dll", CharSet = CharSet.Unicode)]
        public static /*extern*/ int PRCE_get_Capabilities(ref int AbortPrintErr) { return 0; }
        //[DllImport("PrCE_NetCF.dll", CharSet = CharSet.Unicode)]
        public static /*extern*/ int PRCE_get_DrawStyle() { return 0; }
        //[DllImport("PrCE_NetCF.dll", CharSet = CharSet.Unicode)]
        public static /*extern*/ void PRCE_get_DrawWidth(ref float retval) { }
        //[DllImport("PrCE_NetCF.dll", CharSet = CharSet.Unicode)]
        public static /*extern*/ int PRCE_get_FillColor() { return 0; }
        //[DllImport("PrCE_NetCF.dll", CharSet = CharSet.Unicode)]
        public static /*extern*/ int PRCE_get_FillStyle() { return 0; }
        //[DllImport("PrCE_NetCF.dll", CharSet = CharSet.Unicode)]
        public static /*extern*/ int PRCE_get_FontBold() { return 0; }
        //[DllImport("PrCE_NetCF.dll", CharSet = CharSet.Unicode)]
        public static /*extern*/ int PRCE_get_FontBoldVal() { return 0; }
        //[DllImport("PrCE_NetCF.dll", CharSet = CharSet.Unicode)]
        public static /*extern*/ int PRCE_get_FontItalic() { return 0; }
        //[DllImport("PrCE_NetCF.dll", CharSet = CharSet.Unicode)]
        public static /*extern*/ void PRCE_get_FontName(ref IntPtr rbz) { }
        //[DllImport("PrCE_NetCF.dll", CharSet = CharSet.Unicode)]
        public static /*extern*/ int PRCE_get_FontSize() { return 0; }
        //[DllImport("PrCE_NetCF.dll", CharSet = CharSet.Unicode)]
        public static /*extern*/ int PRCE_get_FontStrikethru() { return 0; }
        //[DllImport("PrCE_NetCF.dll", CharSet = CharSet.Unicode)]
        public static /*extern*/ int PRCE_get_FontUnderline() { return 0; }
        //[DllImport("PrCE_NetCF.dll", CharSet = CharSet.Unicode)]
        public static /*extern*/ int PRCE_get_ForeColor() { return 0; }
        //[DllImport("PrCE_NetCF.dll", CharSet = CharSet.Unicode)]
        public static /*extern*/ void PRCE_get_GetStringHeight(ref float retval, ref int AbortPrintErr) { }
        //[DllImport("PrCE_NetCF.dll", CharSet = CharSet.Unicode)]
        public static /*extern*/ void PRCE_get_GetStringWidth(string drawstr, ref float retval, ref int AbortPrintErr) { }
        //[DllImport("PrCE_NetCF.dll", CharSet = CharSet.Unicode)]
        public static /*extern*/ int PRCE_get_IsColor() { return 0; }
        //[DllImport("PrCE_NetCF.dll", CharSet = CharSet.Unicode)]
        public static /*extern*/ int PRCE_get_JustifyHoriz() { return 0; }
        //[DllImport("PrCE_NetCF.dll", CharSet = CharSet.Unicode)]
        public static /*extern*/ int PRCE_get_JustifyVert() { return 0; }
        //[DllImport("PrCE_NetCF.dll", CharSet = CharSet.Unicode)]
        public static /*extern*/ int PRCE_get_LastError() { return 0; }
        //[DllImport("PrCE_NetCF.dll", CharSet = CharSet.Unicode)]
        public static /*extern*/ int PRCE_get_PgIndentLeft() { return 0; }
        //[DllImport("PrCE_NetCF.dll", CharSet = CharSet.Unicode)]
        public static /*extern*/ int PRCE_get_PgIndentTop() { return 0; }
        //[DllImport("PrCE_NetCF.dll", CharSet = CharSet.Unicode)]
        public static /*extern*/ void PRCE_get_PrBottomMargin(ref float retval) { }
        //[DllImport("PrCE_NetCF.dll", CharSet = CharSet.Unicode)]
        public static /*extern*/ int PRCE_get_PrDialogBox() { return 0; }
        //[DllImport("PrCE_NetCF.dll", CharSet = CharSet.Unicode)]
        public static /*extern*/ int PRCE_get_PrinterResolution() { return 0; }
        //[DllImport("PrCE_NetCF.dll", CharSet = CharSet.Unicode)]
        public static /*extern*/ void PRCE_get_PrLeftMargin(ref float retval) { }
        //[DllImport("PrCE_NetCF.dll", CharSet = CharSet.Unicode)]
        public static /*extern*/ int PRCE_get_PrOrientation() { return 0; }
        //[DllImport("PrCE_NetCF.dll", CharSet = CharSet.Unicode)]
        public static /*extern*/ int PRCE_get_PrPaperSelection() { return 0; }
        //[DllImport("PrCE_NetCF.dll", CharSet = CharSet.Unicode)]
        public static /*extern*/ void PRCE_get_PrPgHeight(ref float retval, ref int AbortPrintErr) { }
        //[DllImport("PrCE_NetCF.dll", CharSet = CharSet.Unicode)]
        public static /*extern*/ void PRCE_get_PrPgWidth(ref float retval, ref int AbortPrintErr) { }
        //[DllImport("PrCE_NetCF.dll", CharSet = CharSet.Unicode)]
        public static /*extern*/ int PRCE_get_PrPrintQuality() { return 0; }
        //[DllImport("PrCE_NetCF.dll", CharSet = CharSet.Unicode)]
        public static /*extern*/ void PRCE_get_PrRightMargin(ref float retval) { }
        //[DllImport("PrCE_NetCF.dll", CharSet = CharSet.Unicode)]
        public static /*extern*/ void PRCE_get_PrTopMargin(ref float retval) { }
        //[DllImport("PrCE_NetCF.dll", CharSet = CharSet.Unicode)]
        public static /*extern*/ int PRCE_get_Rotation() { return 0; }
        //[DllImport("PrCE_NetCF.dll", CharSet = CharSet.Unicode)]
        public static /*extern*/ int PRCE_get_ScaleMode() { return 0; }
        //[DllImport("PrCE_NetCF.dll", CharSet = CharSet.Unicode)]
        public static /*extern*/ int PRCE_get_StatusCheck() { return 0; }
        //[DllImport("PrCE_NetCF.dll", CharSet = CharSet.Unicode)]
        public static /*extern*/ void PRCE_get_TextX(ref float retval, ref int AbortPrintErr) { }
        //[DllImport("PrCE_NetCF.dll", CharSet = CharSet.Unicode)]
        public static /*extern*/ void PRCE_get_TextY(ref float retval, ref int AbortPrintErr) { }
        //[DllImport("PrCE_NetCF.dll", CharSet = CharSet.Unicode)]
        public static /*extern*/ void PRCE_getFullPrSettings(IntPtr lppd, ref IntPtr SharedNetStr, ref IntPtr IPNetStr, ref int iAbort) { }
        //[DllImport("PrCE_NetCF.dll", CharSet = CharSet.Unicode)]
        public static /*extern*/ void PRCE_GetVersion(out int major, out int minor, out int build, out int revision)
        { major = 1; minor = 2; build = 5;  revision = 0; } // mvega para que coincida con la version comparada
        //[DllImport("PrCE_NetCF.dll", CharSet = CharSet.Unicode)]
        public static /*extern*/ int PRCE_Init(string str, ref int AbortPrintErr) { return 0; }
        //[DllImport("PrCE_NetCF.dll", CharSet = CharSet.Unicode)]
        public static /*extern*/ void PRCE_KillDoc(ref int AbortPrintErr) { }
        //[DllImport("PrCE_NetCF.dll", CharSet = CharSet.Unicode)]
        //public static /*extern*/ /*unsafe*/ int PRCE_LocalAlloc(int size, int** retptr) { }
        //[DllImport("PrCE_NetCF.dll", CharSet = CharSet.Unicode)]
        public static /*extern*/ int PRCE_LocalAllocIntPtr(int size, ref IntPtr retptr) { return 0; }
        //[DllImport("PrCE_NetCF.dll", CharSet = CharSet.Unicode)]
        //public static /*extern*/ /*unsafe*/ void PRCE_LocalFree(int* szptr) { }
        //[DllImport("PrCE_NetCF.dll", CharSet = CharSet.Unicode)]
        public static /*extern*/ void PRCE_LocalFreeIntPtr(IntPtr szptr) { }
        //[DllImport("PrCE_NetCF.dll", CharSet = CharSet.Unicode)]
        public static /*extern*/ void PRCE_NewPage(ref int AbortPrintErr) { }
        //[DllImport("PrCE_NetCF.dll", CharSet = CharSet.Unicode)]
        public static /*extern*/ int PRCE_PrDialogBox(int newOp, ref int AbortPrintErr) { return 0; }
        //[DllImport("PrCE_NetCF.dll", CharSet = CharSet.Unicode)]
        public static /*extern*/ void PRCE_PrDialogBoxText(string MainText, string TitleText, string CancelBtnText, ref int AbortPrintErr) { }
        //[DllImport("PrCE_NetCF.dll", CharSet = CharSet.Unicode)]
        public static /*extern*/ void PRCE_PrSetDefaults(ref int AbortPrintErr) { }
        //[DllImport("PrCE_NetCF.dll", CharSet = CharSet.Unicode)]
        public static /*extern*/ void PRCE_SelectPrinter(int StartPrinting, ref int AbortPrintErr) { }
        //[DllImport("PrCE_NetCF.dll", CharSet = CharSet.Unicode)]
        public static /*extern*/ void PRCE_set_DrawStyle(int newVal) { }
        //[DllImport("PrCE_NetCF.dll", CharSet = CharSet.Unicode)]
        public static /*extern*/ void PRCE_set_DrawWidth(ref float newVal) { }
        //[DllImport("PrCE_NetCF.dll", CharSet = CharSet.Unicode)]
        public static /*extern*/ void PRCE_set_FillColor(int newVal) { }
        //[DllImport("PrCE_NetCF.dll", CharSet = CharSet.Unicode)]
        public static /*extern*/ void PRCE_set_FillStyle(int newVal, ref int AbortPrintErr) { }
        //[DllImport("PrCE_NetCF.dll", CharSet = CharSet.Unicode)]
        public static /*extern*/ void PRCE_set_FontBold(int newVal) { }
        //[DllImport("PrCE_NetCF.dll", CharSet = CharSet.Unicode)]
        public static /*extern*/ void PRCE_set_FontBoldVal(int newVal) { }
        //[DllImport("PrCE_NetCF.dll", CharSet = CharSet.Unicode)]
        public static /*extern*/ void PRCE_set_FontItalic(int newVal) { }
        //[DllImport("PrCE_NetCF.dll", CharSet = CharSet.Unicode)]
        public static /*extern*/ void PRCE_set_FontName(string newVal, ref int AbortPrintErr) { }
        //[DllImport("PrCE_NetCF.dll", CharSet = CharSet.Unicode)]
        public static /*extern*/ void PRCE_set_FontSize(int newVal, ref int AbortPrintErr) { }
        //[DllImport("PrCE_NetCF.dll", CharSet = CharSet.Unicode)]
        public static /*extern*/ void PRCE_set_FontStrikethru(int newVal) { }
        //[DllImport("PrCE_NetCF.dll", CharSet = CharSet.Unicode)]
        public static /*extern*/ void PRCE_set_FontUnderline(int newVal) { }
        //[DllImport("PrCE_NetCF.dll", CharSet = CharSet.Unicode)]
        public static /*extern*/ void PRCE_set_ForeColor(int newVal) { }
        //[DllImport("PrCE_NetCF.dll", CharSet = CharSet.Unicode)]
        public static /*extern*/ void PRCE_set_JustifyHoriz(int newVal, ref int AbortPrintErr) { }
        //[DllImport("PrCE_NetCF.dll", CharSet = CharSet.Unicode)]
        public static /*extern*/ void PRCE_set_JustifyVert(int newVal, ref int AbortPrintErr) { }
        //[DllImport("PrCE_NetCF.dll", CharSet = CharSet.Unicode)]
        public static /*extern*/ void PRCE_set_PgIndentLeft(int newVal) { }
        //[DllImport("PrCE_NetCF.dll", CharSet = CharSet.Unicode)]
        public static /*extern*/ void PRCE_set_PgIndentTop(int newVal) { }
        //[DllImport("PrCE_NetCF.dll", CharSet = CharSet.Unicode)]
        public static /*extern*/ void PRCE_set_PrBottomMargin(ref float newVal, ref int AbortPrintErr) { }
        //[DllImport("PrCE_NetCF.dll", CharSet = CharSet.Unicode)]
        public static /*extern*/ void PRCE_set_PrDialogBox(int value, ref int AbortPrintErr) { }
        //[DllImport("PrCE_NetCF.dll", CharSet = CharSet.Unicode)]
        public static /*extern*/ void PRCE_set_PrLeftMargin(ref float newVal, ref int AbortPrintErr) { }
        //[DllImport("PrCE_NetCF.dll", CharSet = CharSet.Unicode)]
        public static /*extern*/ void PRCE_set_PrOrientation(int newVal, ref int AbortPrintErr) { }
        //[DllImport("PrCE_NetCF.dll", CharSet = CharSet.Unicode)]
        public static /*extern*/ void PRCE_set_PrPaperSelection(int newVal) { }
        //[DllImport("PrCE_NetCF.dll", CharSet = CharSet.Unicode)]
        public static /*extern*/ void PRCE_set_PrPrintQuality(int newVal, ref int AbortPrintErr) { }
        //[DllImport("PrCE_NetCF.dll", CharSet = CharSet.Unicode)]
        public static /*extern*/ void PRCE_set_PrRightMargin(ref float newVal, ref int AbortPrintErr) { }
        //[DllImport("PrCE_NetCF.dll", CharSet = CharSet.Unicode)]
        public static /*extern*/ void PRCE_set_PrTopMargin(ref float newVal, ref int AbortPrintErr) { }
        //[DllImport("PrCE_NetCF.dll", CharSet = CharSet.Unicode)]
        public static /*extern*/ void PRCE_set_Rotation(int newVal, ref int AbortPrintErr) { }
        //[DllImport("PrCE_NetCF.dll", CharSet = CharSet.Unicode)]
        public static /*extern*/ void PRCE_set_ScaleMode(int newVal) { }
        //[DllImport("PrCE_NetCF.dll", CharSet = CharSet.Unicode)]
        public static /*extern*/ int PRCE_set_SetErrorLevel(int newVal, ref int AbortPrintErr) { return 0; }
        //[DllImport("PrCE_NetCF.dll", CharSet = CharSet.Unicode)]
        public static /*extern*/ int PRCE_set_SetReportLevel(int newVal, ref int AbortPrintErr) { return 0; }
        //[DllImport("PrCE_NetCF.dll", CharSet = CharSet.Unicode)]
        public static /*extern*/ void PRCE_set_TextX(ref float newVal, ref int AbortPrintErr) { }
        //[DllImport("PrCE_NetCF.dll", CharSet = CharSet.Unicode)]
        public static /*extern*/ void PRCE_set_TextY(ref float newVal, ref int AbortPrintErr) { }
        //[DllImport("PrCE_NetCF.dll", CharSet = CharSet.Unicode)]
        public static /*extern*/ /*unsafe*/ void PRCE_setFullPrSettings(ref int lppd, string SharedNetStr, string IPNetStr, ref int iAbort) { }
        //[DllImport("PrCE_NetCF.dll", CharSet = CharSet.Unicode)]
        public static /*extern*/ void PRCE_SetupIPprinter(string netstr, int port, int OpenPort, ref int AbortPrintErr) { }
        //[DllImport("PrCE_NetCF.dll", CharSet = CharSet.Unicode)]
        public static /*extern*/ void PRCE_SetupNetSharedPrinter(string netstr, int OpenPort, ref int AbortPrintErr) { }
        //[DllImport("PrCE_NetCF.dll", CharSet = CharSet.Unicode)]
        public static /*extern*/ void PRCE_SetupPaper(int PaperType, int Orientation, ref float PaperWidth, ref float PaperHeight, ref float LeftMgn, ref float TopMgn, ref float RightMgn, ref float BottomMgn, ref int AbortPrintErr) { }
        //[DllImport("PrCE_NetCF.dll", CharSet = CharSet.Unicode)]
        public static /*extern*/ void PRCE_SetupPrinter(int Printer, int Port, int Baudrate, int OpenPort, ref int AbortPrintErr) { }
        //[DllImport("PrCE_NetCF.dll", CharSet = CharSet.Unicode)]
        public static /*extern*/ void PRCE_SetupPrinterOther(int FFSetting, ref float FFScroll, int Density, int Handshake, int BitFlags, int Compressed, int Dither, int DraftMode, ref int AbortPrintErr) { }
        //[DllImport("PrCE_NetCF.dll", CharSet = CharSet.Unicode)]
        public static /*extern*/ void PRCE_ShutDown(ref int AbortPrintErr) { }
        //[DllImport("PrCE_NetCF.dll", CharSet = CharSet.Unicode)]
        public static /*extern*/ void PRCE_StartUp(ref int AbortPrintErr) { }
    }

    public class PrinterCEException : ApplicationException
    {
        // Methods
        public PrinterCEException()
        {
        }

        public PrinterCEException(string message)
            : base(message)
        {
        }

        public PrinterCEException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}