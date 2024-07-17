// Project:           $Project: 00010855-Sapient$
// File:              $Workfile: HeartbeatGenerator.cs$
// Crown-owned copyright, 2021-2024
//  See Release/Supply Conditions

using Google.Protobuf.WellKnownTypes;
using log4net;
using Sapient.Common;
using Sapient.Data;
using SAPIENT.ReadSampleMessage;
using SapientServices.Communication;

namespace SapientDmmSimulator.Common
{
    /// <summary>
    /// Generate SAPIENT heartbeat (status report messages).
    /// </summary>
    public class HeartbeatGenerator
    {
        /// <summary>
        /// log4net logger.
        /// </summary>
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Gets or sets the heartbeat time.
        /// </summary>
        /// <value>
        /// The heartbeat time.
        /// </value>
        public int HeartbeatTime { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [loop heatbeat].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [loop heatbeat]; otherwise, <c>false</c>.
        /// </value>
        public bool LoopHeatbeat { get; set; }

        /// <summary>
        /// Gets or sets the azimuth.
        /// </summary>
        /// <value>
        /// The azimuth.
        /// </value>
        public int Azimuth { get; set; }

        /// <summary>
        /// The heartbeat identifier.
        /// </summary>
        protected int heartbeat_id = 1;

        /// <summary>
        /// sends out health messages, will loop until manually told not too.
        /// </summary>
        /// <param name="comms_connection">IConnection messenger object used to send messages over.</param>
        public void GenerateHLStatus(object comms_connection)
        {
            do
            {

                Log.Info("Create Status Report.");
                ReadSampleMessage readSampleMessage = new ReadSampleMessage();
                string error = string.Empty;

                SapientMessage? message = readSampleMessage.ReadSampleMessageFromFile(SapientMessage.ContentOneofCase.StatusReport, string.Empty, out error);
                if (message != null)
                {
                    message.NodeId = Properties.Settings.Default.FixedDMMId;
                    message.Timestamp = Timestamp.FromDateTime(DateTime.Now.ToUniversalTime());
                    if (message.StatusReport != null)
                    {
                        message.StatusReport.ReportId = Ulid.NewUlid().ToString();
                    }
                }
                else
                {
                    Log.Error(error);
                }

                if ((message != null) && (message.StatusReport != null))
                {

                    var messenger = (IConnection)comms_connection;
                    if (messenger.SendMessage(message))
                    {
                        Log.InfoFormat("Send Status Report: {0}", message.StatusReport.ReportId);
                    }
                    else
                    {
                        Log.ErrorFormat("Send Status Report Failed: {0}", message.StatusReport.ReportId);
                    }
                }

                if (LoopHeatbeat) Thread.Sleep(HeartbeatTime);
            }
            while (LoopHeatbeat);
        }
    }
}
