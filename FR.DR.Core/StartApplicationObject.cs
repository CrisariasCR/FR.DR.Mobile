using System;
using System.Net;
using System.Windows;
//using System.Windows.Controls;

using Cirrious.MvvmCross.Interfaces.ViewModels;
using Cirrious.MvvmCross.ViewModels;
using Softland.ERP.FR.Mobile.ViewModels;

namespace Softland.ERP.FR.Mobile
{
    public class StartApplicationObject : MvxApplicationObject, IMvxStartNavigation
    {
        public void Start()
        {
            this.RequestNavigate<LoginViewModel>();
        }

        public bool ApplicationCanOpenBookmarks
        {
            get { return false; }
        }
    }
}
