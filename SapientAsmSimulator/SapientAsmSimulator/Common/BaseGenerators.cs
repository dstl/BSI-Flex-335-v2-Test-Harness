// File:              $Workfile: BaseGenerators.cs$
// <copyright file="BaseGenerators.cs" >
// Crown-owned copyright, 2021-2024
// See Release/Supply Conditions
// </copyright>

namespace SapientASMsimulator.Common
{
    /// <summary>
    /// Base class for message generation 
    /// </summary>
    public class BaseGenerators
    {
        /// <summary>
        /// Default Longitude or X coordinate to use in detection and sensor location
        /// </summary>
        protected static double start_longitude = Properties.Settings.Default.startLongitude;

        /// <summary>
        /// Default Latitude or Y coordinate to use in detection and sensor location
        /// </summary>
        protected static double start_latitude = Properties.Settings.Default.startLatitude;

        /// <summary>
        /// determine whether using UTM or GPS coordinates
        /// </summary>
        protected static bool isUTM = start_longitude > 180 || start_longitude < -180 || start_latitude > 90 || start_latitude < -90;

        /// <summary>
        /// Gets or sets the Sensor ID to use in SAPIENT messages
        /// </summary>
        public static string ASMId { get; set; }

        /// <summary>
        /// Gets or sets the Message count for each message type
        /// </summary>
        public long MessageCount { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to send messages periodically
        /// </summary>
        public bool LoopMessages { get; set; }

        /// <summary>
        /// Gets or sets the period between sending messages
        /// </summary>
        public int LoopTime { get; set; }
    }
}
