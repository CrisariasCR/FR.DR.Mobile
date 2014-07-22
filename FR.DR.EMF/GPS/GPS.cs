using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EMF.GPS
{
    public class GpsPosition
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double SeaLevelAltitude { get; set; }
        public float PositionDilutionOfPrecision { get; set; }
        public DateTime Time{ get; set; }

        public bool LatitudeValid { get; set; }
        public bool LongitudeValid { get; set; }
        public bool SeaLevelAltitudeValid { get; set; }
        public bool PositionDilutionOfPrecisionValid { get; set; }
        public bool TimeValid { get; set; }
        

    }

    public enum GpsServiceState 
    {
        Off,
        On,
        StartingUp,
        ShuttingDown,
        Uninitialized,
        Unloading,
        Unknown,
    }

    public class FRGpsPosition
    {
        #region Atributos
        /// <summary>
        /// Fecha del sistema cuando se toma la posición
        /// </summary>
        protected DateTime localSystemTime;
        /// <summary>
        /// Posición gps
        /// </summary>
        protected GpsPosition gpsPosition;
        #endregion

        #region Propiedades
        /// <summary>
        /// System datime when the position was took.
        /// </summary>
        public DateTime LocalSystemTime
        {
            get
            {
                return localSystemTime;
            }
            set
            {
                localSystemTime = value;
            }
        }
        /// <summary>
        /// Handled GPS Position
        /// </summary>
        public GpsPosition GpsPosition
        {
            get
            {
                return gpsPosition;
            }
        }
        #endregion

        #region Constructor
        public FRGpsPosition(GpsPosition position, DateTime localSystemTime)
        {
            gpsPosition = position;
            this.localSystemTime = localSystemTime;
        }
        #endregion
    }

}
