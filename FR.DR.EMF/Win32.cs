using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EMF.Win32
{
    public class WinAPI
    {
        public enum SystemMetric
        {
            SM_CXSCREEN,
            SM_CYSCREEN
        }

        public static int GetSystemMetrics(SystemMetric sm)
        {
            // TODO
            return -1;
        }


        
    }

    //public class ControlDecorator
    //{
    //    public ControlDecorator(System.Windows.Forms.ListView lstArticulos,
    //        System.Drawing.Size designScreenSize,
    //        System.Drawing.Size currentScreenSize)
    //    {
    //    }

    //    public int YPixel;

    //    public void Move(System.Windows.Forms.Control c, System.Drawing.Point pos)
    //    {
    //    }

    //    public void Resize(System.Windows.Forms.Control c, System.Drawing.Size size)
    //    {
    //    }

    //}
}
