using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace FR.Droid.Views
{
    public class ResolutionsDisp
    {
        public static int Resolucion(IWindowManager wm)
        {
            int dips = 0;
            Android.Views.Display d = wm.DefaultDisplay;
            Android.Util.DisplayMetrics m = new Android.Util.DisplayMetrics();
            d.GetMetrics(m);
            int width = d.Width;
            int height = d.Height;
            float scaleFactor = m.Density;
            float widthDp = width / scaleFactor;
            float heightDp = height / scaleFactor;
            float smallestWidth = Math.Min(widthDp, heightDp);


            //320dp: a typical phone screen (240x320 ldpi, 320x480 mdpi, 480x800 hdpi, etc).
            //480dp: a tweener tablet like the Streak (480x800 mdpi).
            //600dp: a 7” tablet (600x1024 mdpi).
            //720dp: a 10” tablet (720x1280 mdpi, 800x1280 mdpi, etc).
            if (smallestWidth >= 320 && smallestWidth < 480)
            {
                if (width <= 240 || height <= 320)
                {
                    dips = 1;
                }
                else
                {
                    if (width == 720 || height == 1280)
                    {
                        dips = 5;
                    }
                    else
                    {
                        //Kindle Fire
                        if (width == 1024 || height == 600)
                        {
                            dips = 3;
                        }
                        else
                        {
                            dips = 0;
                        }
                    }
                }
                
            }
            else
            {
                if (smallestWidth >= 480 && smallestWidth < 600)
                {
                    dips = 2;
                }
                else
                {
                    if (smallestWidth >= 600 && smallestWidth < 720)
                    {
                        dips = 3;
                    }
                    else
                    {
                        if (smallestWidth >= 720)
                        {
                            dips = 4;
                        }
                        else
                        {
                            dips = 0;
                        }
                    }
                }
            }
            return dips;
        }
    }
}