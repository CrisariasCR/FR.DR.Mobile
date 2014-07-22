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
    public class AsciiCE : PrinterCE_Base
    {
        // Methods
        public AsciiCE()
        {
            this.AsciiCE_start(PrinterCE_Base.EXCEPTION_LEVEL.ALL);
            base.Init();
            base.TestForException();
        }

        public AsciiCE(PrinterCE_Base.EXCEPTION_LEVEL exclevel)
        {
            this.AsciiCE_start(exclevel);
            base.Init();
            base.TestForException();
        }

        public AsciiCE(string initstr)
        {
            this.AsciiCE_start(PrinterCE_Base.EXCEPTION_LEVEL.ALL);
            base.Init(initstr);
            base.TestForException();
        }

        public AsciiCE(PrinterCE_Base.EXCEPTION_LEVEL exclevel, string initstr)
        {
            this.AsciiCE_start(exclevel);
            base.Init(initstr);
            base.TestForException();
        }

        private void AsciiCE_start(PrinterCE_Base.EXCEPTION_LEVEL exclevel)
        {
            if (base.PrBase_FirstInit(exclevel))
            {
                AsciiCE_DLL.ASC_StartUp(ref this.AbortPrintErr);
            }
        }

        public void Char(char ch)
        {
            AsciiCE_DLL.ASC_Char(ch, ref this.AbortPrintErr);
            base.TestForException();
        }

        public void ClosePort()
        {
            AsciiCE_DLL.ASC_ClosePort(ref this.AbortPrintErr);
            base.TestForException();
        }

        public void CrLf()
        {
            AsciiCE_DLL.ASC_CrLf(ref this.AbortPrintErr);
            base.TestForException();
        }

        public void FormFeed()
        {
            AsciiCE_DLL.ASC_FormFeed(ref this.AbortPrintErr);
            base.TestForException();
        }

        public int Peek()
        {
            int num = AsciiCE_DLL.ASC_Peek(ref this.AbortPrintErr);
            base.TestForException();
            return num;
        }

        public byte[] Read(int MaxBytes, bool NormalRead, int nTimeToWait, ref READ_RESULT result)
        {
            int lpResultFlags = 0;
            byte[] lpBuffer = new byte[MaxBytes];
            int length = AsciiCE_DLL.ASC_Read(MaxBytes, -1, !NormalRead ? 1 : 0, nTimeToWait, ref lpResultFlags, lpBuffer, ref this.AbortPrintErr);
            base.TestForException();
            byte[] destinationArray = new byte[length];
            Array.Copy(lpBuffer, 0, destinationArray, 0, length);
            result = (READ_RESULT)lpResultFlags;
            return destinationArray;
        }

        public byte[] Read(int MaxBytes, byte LastByte, bool NormalRead, int nTimeToWait, ref READ_RESULT result)
        {
            int lpResultFlags = 0;
            byte[] lpBuffer = new byte[MaxBytes];
            int length = AsciiCE_DLL.ASC_Read(MaxBytes, LastByte, !NormalRead ? 1 : 0, nTimeToWait, ref lpResultFlags, lpBuffer, ref this.AbortPrintErr);
            base.TestForException();
            byte[] destinationArray = new byte[length];
            Array.Copy(lpBuffer, 0, destinationArray, 0, length);
            result = (READ_RESULT)lpResultFlags;
            return destinationArray;
        }

        public bool ReadChar(ref char newch)
        {
            int num = AsciiCE_DLL.ASC_ReadChar(ref this.AbortPrintErr);
            base.TestForException();
            if (num == -1)
            {
                return false;
            }
            newch = (char)num;
            return true;
        }

        public string ReadString(int MaxBytes, bool NormalRead, int nTimeToWait, ref READ_RESULT result)
        {
            int lpResultFlags = 0;
            IntPtr lpString = new IntPtr(0);
            AsciiCE_DLL.ASC_ReadString(MaxBytes, -1, !NormalRead ? 1 : 0, nTimeToWait, ref lpResultFlags, ref lpString, ref this.AbortPrintErr);
            base.TestForException();
            string str = null;
            try
            {
                str = Marshal.PtrToStringUni(lpString);
            }
            finally
            {
                PrinterCE_DLL.PRCE_LocalFreeIntPtr(lpString);
            }
            result = (READ_RESULT)lpResultFlags;
            return str;
        }

        public string ReadString(int MaxBytes, char LastChar, bool NormalRead, int nTimeToWait, ref READ_RESULT result)
        {
            int lpResultFlags = 0;
            IntPtr lpString = new IntPtr(0);
            AsciiCE_DLL.ASC_ReadString(MaxBytes, LastChar, !NormalRead ? 1 : 0, nTimeToWait, ref lpResultFlags, ref lpString, ref this.AbortPrintErr);
            base.TestForException();
            string str = null;
            try
            {
                str = Marshal.PtrToStringUni(lpString);
            }
            finally
            {
                PrinterCE_DLL.PRCE_LocalFreeIntPtr(lpString);
            }
            result = (READ_RESULT)lpResultFlags;
            return str;
        }

        public void RepeatChar(char ch, int repeatcnt)
        {
            AsciiCE_DLL.ASC_RepeatChar(ch, repeatcnt, ref this.AbortPrintErr);
            base.TestForException();
        }

        public bool SelectPort(ASCIIPORT Port)
        {
            return this.SelectPort(Port, PrinterCE_Base.PORT_SPEED.USE_CURRENT, PrinterCE_Base.SERIAL_HANDSHAKE.USE_CURRENT, 0);
        }

        public bool SelectPort(ASCIIPORT Port, int ReadBufferSize)
        {
            return this.SelectPort(Port, PrinterCE_Base.PORT_SPEED.USE_CURRENT, PrinterCE_Base.SERIAL_HANDSHAKE.USE_CURRENT, ReadBufferSize);
        }

        public bool SelectPort(ASCIIPORT Port, PrinterCE_Base.PORT_SPEED BaudRate, PrinterCE_Base.SERIAL_HANDSHAKE Handshake)
        {
            return this.SelectPort(Port, BaudRate, Handshake, 0);
        }

        public bool SelectPort(ASCIIPORT Port, PrinterCE_Base.PORT_SPEED BaudRate, PrinterCE_Base.SERIAL_HANDSHAKE Handshake, int ReadBufferSize)
        {
            int num = AsciiCE_DLL.ASC_SelectPort((int)Port, (int)BaudRate, (int)Handshake, ReadBufferSize, ref this.AbortPrintErr);
            base.TestForException();
            return (num != 0);
        }

        public void ShutDown()
        {
            AsciiCE_DLL.ASC_ShutDown(ref this.AbortPrintErr);
            base.TestForException();
        }

        public void Text(string TextString)
        {
            AsciiCE_DLL.ASC_Text(TextString, ref this.AbortPrintErr);
            base.TestForException();
        }

        public int Write(byte[] buffer)
        {
            int num = AsciiCE_DLL.ASC_Write(buffer, buffer.Length, ref this.AbortPrintErr);
            base.TestForException();
            return num;
        }

        public int Write(byte[] buffer, int NumberOfBytesToWrite)
        {
            int num = AsciiCE_DLL.ASC_Write(buffer, NumberOfBytesToWrite, ref this.AbortPrintErr);
            base.TestForException();
            return num;
        }

        // Nested Types
        public enum ASCIIPORT
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
            PRINTERCE_SHARE = -1,
            SOCKETCOM_BT = 11,
            TOFILE = 8,
            WIDCOMM_BT = 0x10
        }

        public enum READ_RESULT
        {
            ERROR,
            SUCCESS,
            FULLBUFFER,
            TIMEOUT,
            USERCANCEL
        }
    }


    internal class AsciiCE_DLL
    {
        // Methods
        public AsciiCE_DLL() { }
        //[DllImport("PrCE_NetCF.dll", CharSet = CharSet.Unicode)]
        public static void ASC_Char(int ch, ref int AbortPrintErr) { }
        ////[DllImport("PrCE_NetCF.dll", CharSet = CharSet.Unicode)]
        public static void ASC_ClosePort(ref int AbortPrintErr) { }
        ////[DllImport("PrCE_NetCF.dll", CharSet = CharSet.Unicode)]
        public static void ASC_CrLf(ref int AbortPrintErr) { }
        ////[DllImport("PrCE_NetCF.dll", CharSet = CharSet.Unicode)]
        public static void ASC_FormFeed(ref int AbortPrintErr) { }
        ////[DllImport("PrCE_NetCF.dll", CharSet = CharSet.Unicode)]
        public static int ASC_Peek(ref int AbortPrintErr) { return 0; }
        ////[DllImport("PrCE_NetCF.dll", CharSet = CharSet.Unicode)]
        public static int ASC_Read(int nMaxBytes, int nLastChar, int nModeFlags, int nTimeToWait, ref int lpResultFlags, byte[] lpBuffer, ref int AbortPrintErr) { return 0; }
        ////[DllImport("PrCE_NetCF.dll", CharSet = CharSet.Unicode)]
        public static int ASC_ReadChar(ref int AbortPrintErr) { return 0; }
        ////[DllImport("PrCE_NetCF.dll", CharSet = CharSet.Unicode)]
        public static int ASC_ReadString(int nMaxChars, int nLastChar, int nModeFlags, int nTimeToWait, ref int lpResultFlags, ref IntPtr lpString, ref int AbortPrintErr) { return 0; }
        ////[DllImport("PrCE_NetCF.dll", CharSet = CharSet.Unicode)]
        public static void ASC_RepeatChar(int ch, int repeatcnt, ref int AbortPrintErr) { }
        ////[DllImport("PrCE_NetCF.dll", CharSet = CharSet.Unicode)]
        public static int ASC_SelectPort(int Port, int BaudRate, int Handshake, int ReadBufferSize, ref int AbortPrintErr) { return 0; }
        ////[DllImport("PrCE_NetCF.dll", CharSet = CharSet.Unicode)]
        public static void ASC_ShutDown(ref int AbortPrintErr) { }
        ////[DllImport("PrCE_NetCF.dll", CharSet = CharSet.Unicode)]
        public static void ASC_StartUp(ref int AbortPrintErr) { }
        ////[DllImport("PrCE_NetCF.dll", CharSet = CharSet.Unicode)]
        public static void ASC_Text(string TextString, ref int AbortPrintErr) { }
        ////[DllImport("PrCE_NetCF.dll", CharSet = CharSet.Unicode)]
        public static int ASC_Write(byte[] lpBuffer, int nNumberOfBytesToWrite, ref int AbortPrintErr) { return 0; }
    }

}