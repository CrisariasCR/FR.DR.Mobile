using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EMF.GPS
{
    public class Location: IDisposable
    {

        public int RunningTime;
        public string Error;

        public void StartPositionSearch()
        { }

        public void StopPositionSearch()
        {}

        public void Dispose()
        {
        }

        public DeviceStateChangedEventHandler DeviceStateChanged;
        public LocationChangedEventHandler LocationChanged;
    }

    public delegate void DeviceStateChangedEventHandler(object sender, DeviceStateChangedEventArgs args);
    public delegate void LocationChangedEventHandler(object sender, LocationChangedEventArgs args);

    public class DeviceStateChangedEventArgs : EventArgs
    {
        public GpsServiceState DeviceState;
    }

    public class LocationChangedEventArgs : EventArgs
    {
        public GpsPosition Position;
    }

}
