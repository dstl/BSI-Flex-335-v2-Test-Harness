// Project:           $Project: 00010855-Sapient$
// File:              $Workfile: XmlGenerators.cs$
// Crown-owned copyright, 2021-2024
//  See Release/Supply Conditions

namespace SapientDmmSimulator.Common
{
    using log4net;

    /// <summary>
    /// Base class for  message generation.
    /// </summary>
    public class BaseGenerators
    {
        /// <summary>
        /// log4net logger.
        /// </summary>
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// The start longitude.
        /// </summary>
        protected static double start_longitude = Properties.Settings.Default.startLongitude;

        /// <summary>
        /// The start latitude.
        /// </summary>
        protected static double start_latitude = Properties.Settings.Default.startLatitude;

        /// <summary>
        /// The is utm.
        /// </summary>
        protected static bool isUTM = start_longitude > 180 || start_longitude < -180 || start_latitude > 90 || start_latitude < -90;

        /// <summary>
        /// Gets or sets the message count.
        /// </summary>
        /// <value>
        /// The message count.
        /// </value>
        public long MessageCount { get; protected set; }

        /// <summary>
        /// Gets or sets a value indicating whether [loop messages].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [loop messages]; otherwise, <c>false</c>.
        /// </value>
        public bool LoopMessages { get; set; }

        /// <summary>
        /// Gets or sets the loop time.
        /// </summary>
        /// <value>
        /// The loop time.
        /// </value>
        public int LoopTime { get; set; }
    }
}
