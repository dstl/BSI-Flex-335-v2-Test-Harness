// Project:           $Project: 00010855-Sapient$
// File:              $Workfile: LookAtTaskGenerator.cs$
// Crown-owned copyright, 2021-2024
//  See Release/Supply Conditions

namespace SapientDmmSimulator.Common
{
    using Google.Protobuf.WellKnownTypes;
    using log4net;
    using Sapient.Common;
    using Sapient.Data;
    using SapientServices.Communication;

    /// <summary>
    /// Generate Look At Sensor Task messages.
    /// </summary>
    public class LookAtTaskGenerator : TaskGenerator
    {
        /// <summary>
        /// log4net logger.
        /// </summary>
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Generate and send command to one or more sensors.
        /// </summary>
        /// <param name="az">azimuth in degrees.</param>
        /// <param name="ele">ele in meters.</param>
        /// <param name="ptz">raw PTZ command.</param>
        /// <param name="zoom">zoom value.</param>
        /// <param name="comms_connection">IConnection messenger object used to send messages over.</param>
        /// <param name="baseSensorID">base sensor Identifier.</param>
        /// <param name="numSensors">Number of sensors.</param>
        public void GeneratePTZTask(double az, double ele, bool ptz, double zoom, object comms_connection, string baseSensorID, int numSensors)
        {
            IConnection messenger = (IConnection)comms_connection;
            do
            {
                SendTaskToMultipleSensors(az, ele, ptz, zoom, comms_connection, baseSensorID, numSensors);

                if (LoopMessages) Thread.Sleep(LoopTime);
            }
            while (LoopMessages);
        }

        /// <summary>
        /// Send Task to one or more sensors.
        /// </summary>
        /// <param name="az">azimuth in degrees.</param>
        /// <param name="ele">ele in meters.</param>
        /// <param name="ptz">raw PTZ command.</param>
        /// <param name="zoom">zoom value.</param>
        /// <param name="comms_connection">IConnection messenger object used to send messages over.</param>
        /// <param name="baseSensorID">base sensor Identifier.</param>
        /// <param name="numSensors">Number of sensors.</param>
        private static void SendTaskToMultipleSensors(double az, double ele, bool ptz, double zoom, object comms_connection, string baseSensorID, int numSensors)
        {
            var messenger = (IConnection)comms_connection;

            int sensorNum;
            for (sensorNum = 0; sensorNum < numSensors; sensorNum++)
            {
                string sensorId = baseSensorID;
                Sapient.Data.Task sensorTask = GeneratePTZTask(az, ele, ptz, zoom);

                SapientMessage message = new SapientMessage
                {
                    Timestamp = Timestamp.FromDateTime(DateTime.UtcNow),
                    NodeId = Properties.Settings.Default.FixedDMMId,
                    DestinationId = sensorId,
                    Task = sensorTask,
                };

                bool retval = MessageSender.Send(messenger, message);

                if (retval)
                {
                    Log.InfoFormat("Sent LookAtCommand:{0} to SensorID:{1}", sensorTask.TaskId, sensorId);
                }
                else
                {
                    Log.InfoFormat("Sent LookAtCommand:{0} to SensorID:{1} FAILED", sensorTask.TaskId, sensorId);
                }
            }
        }

        /// <summary>
        /// method to generate the PTZ sensor task message.
        /// </summary>
        /// <param name="az">azimuth in degrees.</param>
        /// <param name="range">range in meters.</param>
        /// <param name="ptz">raw PTZ command.</param>
        /// <param name="zoom">zoom value.</param>
        /// <returns>sensor task message.</returns>
        private static Sapient.Data.Task GeneratePTZTask(double az, double ele, bool ptz, double zoom)
        {
            var sensor_task = new Sapient.Data.Task
            {
                TaskId = Ulid.NewUlid().ToString(),
                Control = Sapient.Data.Task.Types.Control.Start,
            };

            sensor_task.Command = new Sapient.Data.Task.Types.Command();

            if (SendToDmm)
            {
                sensor_task.TaskName = "Manual Task";
            }

            if (ptz)
            {
                sensor_task.Command.LookAt = new LocationOrRangeBearing
                {
                    RangeBearing = new RangeBearingCone
                    {
                        Azimuth = az,
                        Elevation = ele,
                        Range = 0,
                        HorizontalExtent = zoom,
                        VerticalExtent = 0.0,
                        CoordinateSystem = RangeBearingCoordinateSystem.DegreesM,
                    },
                };
            }
            else
            {
                // now use zoom value for horizontal field of view
                sensor_task.Command.LookAt = new LocationOrRangeBearing
                {
                    RangeBearing = new RangeBearingCone
                    {
                        Azimuth = az,
                        Elevation = ele,
                        Range = zoom,
                        HorizontalExtent = 0,
                        CoordinateSystem = RangeBearingCoordinateSystem.DegreesM,
                    },
                };
            }

            return sensor_task;
        }
    }
}
