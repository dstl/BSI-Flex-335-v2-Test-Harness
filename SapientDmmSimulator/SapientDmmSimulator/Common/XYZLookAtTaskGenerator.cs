// Project:           $Project: 00010855-Sapient$
// File:              $Workfile: XYZLookAtTaskGenerator.cs$
// Crown-owned copyright, 2021-2024
//  See Release/Supply Conditions

using Google.Protobuf.WellKnownTypes;
using log4net;
using Sapient.Common;
using Sapient.Data;
using SapientServices.Communication;

namespace SapientDmmSimulator.Common
{
    /// <summary>
    /// Generate Look At Sensor Task messages.
    /// </summary>
    public class XYZLookAtTaskGenerator : TaskGenerator
    {
        /// <summary>
        /// log4net logger.
        /// </summary>
        private static ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Generates XYZ LookAt Task.
        /// </summary>
        /// <param name="x">X coordinate.</param>
        /// <param name="y">Y coordinate.</param>
        /// <param name="z">Z coordinate.</param>
        /// <param name="useZ">whether to include Z coordinate.</param>
        /// <param name="comms_connection">IConnection messenger object used to send messages over.</param>
        /// <param name="baseSensorID">base sensor Identifier.</param>
        /// <param name="numSensors">Number of sensors.</param>
        public void GenerateXYZLookAtTask(double x, double y, double z, bool useZ, object comms_connection, string baseSensorID, int numSensors)
        {
            IConnection messenger = (IConnection)comms_connection;
            do
            {
                SendTaskToMultipleSensors(x, y, z, useZ, comms_connection, baseSensorID, numSensors);

                if (LoopMessages) Thread.Sleep(LoopTime);
            }
            while (LoopMessages);
        }

        /// <summary>
        /// Send XYZ Tasks for one or more sensors.
        /// </summary>
        /// <param name="x">X coordinate.</param>
        /// <param name="y">Y coordinate.</param>
        /// <param name="z">Z coordinate.</param>
        /// <param name="useZ">whether to include Z coordinate.</param>
        /// <param name="comms_connection">IConnection messenger object used to send messages over.</param>
        /// <param name="baseSensorID">base sensor Identifier.</param>
        /// <param name="taskId"task ID>task Identifier.</param>
        /// <param name="numSensors">Number of sensors.</param>
        private static void SendTaskToMultipleSensors(double x, double y, double z, bool useZ, object comms_connection, string baseSensorID, int numSensors)
        {
            var messenger = (IConnection)comms_connection;

            int sensorNum;
            for (sensorNum = 0; sensorNum < numSensors; sensorNum++)
            {
                string sensorId = baseSensorID;
                var sensorTask = GenerateXYZTask(x, y, z, useZ);

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
        /// generate the XYZ lookAt task message.
        /// </summary>
        /// <param name="x">X coordinate.</param>
        /// <param name="y">Y coordinate.</param>
        /// <param name="z">Z coordinate.</param>
        /// <param name="useZ">whether to include Z coordinate.</param>
        /// <returns>Populated SensorTask object.</returns>
        private static Sapient.Data.Task GenerateXYZTask(double x, double y, double z, bool useZ)
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

            var locationList = new Sapient.Common.LocationList();
            var location = new Sapient.Common.Location
            {
                X = x,
                Y = y,
                Z = z,
                CoordinateSystem = Sapient.Common.LocationCoordinateSystem.LatLngDegM,
            };
            locationList.Locations.Add(location);

            sensor_task.Command.LookAt = new LocationOrRangeBearing
            {
                LocationList = locationList,
            };

            return sensor_task;
        }
    }
}
