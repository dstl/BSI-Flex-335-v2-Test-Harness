// Project:           $Project: 00010855-Sapient$
// File:              $Workfile: TaskGenerator.cs$
// Crown-owned copyright, 2021-2024
//  See Release/Supply Conditions

using Google.Protobuf.WellKnownTypes;
using log4net;
using Sapient.Data;
using SAPIENT.ReadSampleMessage;
using SapientServices.Communication;
using System.Threading.Tasks;

namespace SapientDmmSimulator.Common
{
    /// <summary>
    /// Generate Sensor Task messages.
    /// </summary>
    public class TaskGenerator : BaseGenerators
    {
        /// <summary>
        /// log4net logger.
        /// </summary>
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskGenerator"/> class.
        /// Constructor.
        /// </summary>
        public TaskGenerator()
        {
            ChangeTaskID = true;
        }

        /// <summary>
        /// Gets or sets a value indicating whether [send to DMM].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [send to DMM]; otherwise, <c>false</c>.
        /// </value>
        public static bool SendToDmm { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [change task identifier].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [change task identifier]; otherwise, <c>false</c>.
        /// </value>
        public bool ChangeTaskID { get; set; }

        /// <summary>
        /// Gets or sets the command string.
        /// </summary>
        /// <value>
        /// The command string.
        /// </value>
        public string CommandString { get; set; }

        /// <summary>
        /// Gets or sets the base sensor identifier.
        /// </summary>
        /// <value>
        /// The base sensor identifier.
        /// </value>
        public string BaseSensorID { get; set; }

        /// <summary>
        /// Gets or sets the number sensors.
        /// </summary>
        /// <value>
        /// The number sensors.
        /// </value>
        public int NumSensors { get; set; }

        /// <summary>
        /// Generate and send command to one or more sensors.
        /// </summary>
        /// <param name="comms_connection">IConnection messenger object used to send messages over.</param>
        public void GenerateCommand(object comms_connection)
        {
            IConnection messenger = (IConnection)comms_connection;
            do
            {
                SendTaskToMultipleSensors(comms_connection, CommandString, BaseSensorID, NumSensors);

                if (LoopMessages) Thread.Sleep(LoopTime);
            }
            while (LoopMessages);
        }

        /// <summary>
        /// Send Task to one or more sensors.
        /// </summary>
        /// <param name="comms_connection">IConnection messenger object used to send messages over.</param>
        /// <param name="commandString">command string.</param>
        /// <param name="baseSensorID">base sensor Identifier.</param>
        /// <param name="numSensors">Number of sensors.</param>
        private static void SendTaskToMultipleSensors(object comms_connection, string commandString, string baseSensorID, int numSensors)
        {
            var messenger = (IConnection)comms_connection;

            int sensorNum;
            for (sensorNum = 0; sensorNum < numSensors; sensorNum++)
            {
                string sensorId = baseSensorID;
                var message = GenerateCommand(commandString, sensorId);
                if (message != null)
                {
                    bool retval = MessageSender.Send(messenger, message);

                    if (retval)
                    {
                        Log.InfoFormat("Sent Command:{0} to SensorID:{1}", message.Task.TaskId, sensorId);
                    }
                    else
                    {
                        Log.InfoFormat("Sent Command:{0} to SensorID:{1} FAILED", message.Task.TaskId, sensorId);
                    }
                }
            }
        }

        private static SapientMessage? GenerateCommand(string commandString, string sensorId)
        {
            Log.Info(string.Format("Create task message for: {0}", commandString));
            ReadSampleMessage readSampleMessage = new ReadSampleMessage();
            string error = string.Empty;
            string additional = string.Empty;
            switch (commandString)
            {
                case "Request Registration":
                    additional = ".Request.Registration";
                    break;
                case "Request Status":
                    additional = ".Request.Status";
                    break;
                case "Request Reset":
                    additional = ".Request.Reset";
                    break;
                case "Request Stop":
                    additional = ".Request.Stop";
                    break;
                case "Request Start":
                    additional = ".Request.Start";
                    break;
                case "DetectionThreshold Low":
                    additional = ".DetectionThreshold.Low";
                    break;
                case "DetectionThreshold Medium":
                    additional = ".DetectionThreshold.Medium";
                    break;
                case "DetectionThreshold High":
                    additional = ".DetectionThreshold.High";
                    break;
                case "DetectionReportRate Low":
                    additional = ".DetectionReportRate.Low";
                    break;
                case "DetectionReportRate Medium":
                    additional = ".DetectionReportRate.Medium";
                    break;
                case "DetectionReportRate High":
                    additional = ".DetectionReportRate.High";
                    break;
                case "ClassificationThreshold Low":
                    additional = ".ClassificationThreshold.Low";
                    break;
                case "ClassificationThreshold Medium":
                    additional = ".ClassificationThreshold.Medium";
                    break;
                case "ClassificationThreshold High":
                    additional = ".ClassificationThreshold.High";
                    break;
                case "Mode":
                    additional = ".Mode";
                    break;
                case "LookAt":
                    additional = ".LookAt";
                    break;
                case "MoveTo":
                    additional = ".MoveTo";
                    break;
                case "Patrol":
                    additional = ".Patrol";
                    break;
                case "Follow":
                    additional = ".Follow";
                    break;
                default:
                    additional = ".Unknown";
                    break;
            }

            SapientMessage? message = readSampleMessage.ReadSampleMessageFromFile(SapientMessage.ContentOneofCase.Task, additional, out error);
            if (message != null)
            {
                message.DestinationId = sensorId;
                message.NodeId = Properties.Settings.Default.FixedDMMId;
                message.Timestamp = Timestamp.FromDateTime(DateTime.Now.ToUniversalTime());
                if (message.Task != null)
                {
                    message.Task.TaskId = Ulid.NewUlid().ToString();
                }
            }
            else
            {
                Log.Error(error);
            }

            return message;
        }
    }
}
