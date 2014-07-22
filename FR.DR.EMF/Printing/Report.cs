using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using EMF.Printing.RDL;
using EMF.Printing.Drivers;
using System.Threading;

namespace EMF.Printing
{

    public class Report
    {
        private RDLEngine engine;
        private string reportFilename;
        private string errorLog;
        private string reportText;
        private ArrayList symbols = new ArrayList();
        private string outputText;

        public Report(string reportFilename, IPrinterDriver printerDriver)
        {
            this.engine = new RDLEngine(printerDriver);
            this.reportFilename = reportFilename;
            this.reportText = "";
        }

        public void AddObject(IPrintable symbol)
        {
            this.symbols.Add(symbol);
        }

        public bool Print()
        {
            if (this.reportText == "")
            {

                FileStream reportReader = null;
                byte[] reportBuffer;

                this.errorLog = "";

                try
                {
                    reportReader = new FileStream(this.reportFilename, FileMode.Open, FileAccess.Read);
                    reportBuffer = new byte[reportReader.Length];
                    reportReader.Read(reportBuffer, 0, (int)reportBuffer.Length);
                    reportText = Encoding.UTF8.GetString(reportBuffer, 0, reportBuffer.Length);
                    reportReader.Close();
                }
                catch (Exception e)
                {
                    this.errorLog = e.Message;
                    return false;
                }
            }

            try
            {
                outputText = this.engine.Print(this.reportText, this.symbols);
                return true;
            }
            catch (Exception e)
            {
                this.errorLog = e.Message;
                return false;
            }
        }

        public bool PrintAll(ref string textoReporte)
        {
            if (this.reportText == "")
            {

                FileStream reportReader = null;
                byte[] reportBuffer;

                this.errorLog = "";

                try
                {
                    reportReader = new FileStream(this.reportFilename, FileMode.Open, FileAccess.Read);
                    reportBuffer = new byte[reportReader.Length];
                    reportReader.Read(reportBuffer, 0, (int)reportBuffer.Length);
                    reportText = Encoding.UTF8.GetString(reportBuffer, 0, reportBuffer.Length);
                    reportReader.Close();
                }
                catch (Exception e)
                {
                    this.errorLog = e.Message;
                    return false;
                }
            }

            try
            {
                outputText = this.engine.PrintAll(this.reportText, this.symbols);
                textoReporte = textoReporte+outputText;
                //Thread.Sleep(4000); 
                return true;
            }
            catch (Exception e)
            {
                this.errorLog = e.Message;
                return false;
            }
        }

        public bool PrintText(string textoReporte)
        {
            try
            {
                this.engine.PrintText(textoReporte);
                return true;
            }
            catch (Exception e)
            {
                this.errorLog = e.Message;
                return false;
            }
        }

        public string ErrorLog
        {
            get
            {
                return this.errorLog;
            }

        }





    }
}