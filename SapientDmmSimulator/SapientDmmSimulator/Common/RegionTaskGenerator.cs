// Project:           $Project: 00010855-Sapient$
// File:              $Workfile: RegionTaskGenerator.cs$
// Crown-owned copyright, 2021-2024
//  See Release/Supply Conditions

namespace SapientDmmSimulator.Common
{
    using Google.Protobuf.WellKnownTypes;
    using log4net;
    using Sapient.Common;
    using Sapient.Data;
    using SapientServices.Communication;

    public class RegionTaskGenerator : TaskGenerator
    {
        /// <summary>
        /// log4net logger.
        /// </summary>
        private static ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        ///
        /// </summary>
        /// <param name="x">X coordinate.</param>
        /// <param name="y">Y coordinate.</param>
        /// <param name="z">Z coordinate.</param>
        /// <param name="useZ">whether to include Z coordinate.</param>
        /// <param name="comms_connection">IConnection messenger object used to send messages over.</param>
        /// <param name="baseSensorID">base sensor Identifier.</param>
        /// <param name="numSensors">Number of sensors.</param>
        public void Generate(double x, double y, double z, bool useZ, object comms_connection, string baseSensorID, int numSensors)
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
        /// Send region Tasks for one or more sensors.
        /// </summary>
        /// <param name="x">X coordinate.</param>
        /// <param name="y">Y coordinate.</param>
        /// <param name="z">Z coordinate.</param>
        /// <param name="useZ">whether to include Z coordinate.</param>
        /// <param name="comms_connection">IConnection messenger object used to send messages over.</param>
        /// <param name="baseSensorID">base sensor Identifier.</param>
        /// <param name="numSensors">Number of sensors.</param>
        private static void SendTaskToMultipleSensors(double x, double y, double z, bool useZ, object comms_connection, string baseSensorID, int numSensors)
        {
            var messenger = (IConnection)comms_connection;

            int sensorNum;
            for (sensorNum = 0; sensorNum < numSensors; sensorNum++)
            {
                string sensorId = baseSensorID;
                Sapient.Data.Task sensorTask = GenerateRegionTask(x, y, z, useZ);

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
        /// generate the region task message.
        /// </summary>
        /// <param name="x">X coordinate.</param>
        /// <param name="y">Y coordinate.</param>
        /// <param name="z">Z coordinate.</param>
        /// <param name="useZ">whether to include Z coordinate.</param>
        /// <returns>Populated SensorTask object.</returns>
        private static Sapient.Data.Task GenerateRegionTask(double x, double y, double z, bool useZ)
        {
            var sensor_task = new Sapient.Data.Task
            {
                TaskId = Ulid.NewUlid().ToString(),
                Control = Sapient.Data.Task.Types.Control.Start
            };

            double offset = 10;

            if (x < 90)
            {
                offset = 0.0001;
            }

            var locationList = new Sapient.Common.LocationList();
            var location = new Sapient.Common.Location
            {
                X = x - offset,
                Y = y - offset,
                Z = z,
                CoordinateSystem = Sapient.Common.LocationCoordinateSystem.LatLngDegM,
            };
            locationList.Locations.Add(location);

            sensor_task.Region.AddRange(new Google.Protobuf.Collections.RepeatedField<Sapient.Data.Task.Types.Region>
            {
                new Sapient.Data.Task.Types.Region
                {
                    RegionId = Ulid.NewUlid().ToString(),
                    RegionName = "region 1",
                    Type = Task.Types.RegionType.AreaOfInterest,
                    RegionArea = new LocationOrRangeBearing
                    {
                        LocationList = locationList,
                    },
                },
                new Sapient.Data.Task.Types.Region
                {
                    RegionId = Ulid.NewUlid().ToString(),
                    RegionName = "region 2",
                    Type = Task.Types.RegionType.AreaOfInterest,
                    RegionArea = new LocationOrRangeBearing
                    {
                        LocationList = locationList,
                    },
                },
            });

            return sensor_task;
        }
    }
}
