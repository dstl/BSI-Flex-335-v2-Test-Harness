// Project:           $Project: 00010855-Sapient$
// File:              $Workfile: GUIProtocol.cs$
// Crown-owned copyright, 2021-2024
//  See Release/Supply Conditions
// $NoKeywords$

namespace SapientMiddleware.Common
{
    using log4net;
    using Sapient.Data;
    using SapientDatabase;
    using SapientServices;
    using System;
    using System.Diagnostics;

    /// <summary>
    /// Protocol for GUI connection.
    /// </summary>
    public class GUIProtocol : SapientProtocol
    {
        /// <summary>
        /// Log4net logger.
        /// </summary>
        private static readonly new ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Holds task permissions. This is both used and updated by GUIProtocol.
        /// </summary>
        protected TaskPermissions taskPermissions;

        /// <summary>
        /// Initializes a new instance of the <see cref="GUIProtocol" /> class.
        /// </summary>
        public GUIProtocol(TaskPermissions taskPermissions)
        {
            this.taskPermissions = taskPermissions;

            Supported.Add(SapientMessageType.Detection);
            Supported.Add(SapientMessageType.Task);
            Supported.Add(SapientMessageType.AlertResponse);
            Supported.Add(SapientMessageType.TaskACK);
            Supported.Add(SapientMessageType.Approval); 
            Supported.Add(SapientMessageType.RegistrationACK);
        }

        /// <summary>
        /// Send response message triggered from parsed message.
        /// </summary>
        /// <param name="output">The parsing result.</param>
        /// <param name="connection">The connection the message was received on.</param>
        public override void SendResponse(SapientMessageType output, uint connection)
        {
            switch (output)
            {
                case SapientMessageType.Task:
                    lock (Program.ClientMessageParsers)
                    {
                        Task sensorTask = TaskMessage.Task;
                        ForwardMessage(TaskMessage, Program.ClientComms, connection, "Sensor Task Forward", "Sensor Task Forward failed");
                    }

                    Program.MessageMonitor.IncrementMessageCount(SapientMessageType.Task);
                    break;

                case SapientMessageType.SensorTaskDropped:
                    Program.MessageMonitor.IncrementMessageCount(SapientMessageType.Task);
                    break;

                case SapientMessageType.SensorTaskTakeControl:
                    Program.MessageMonitor.IncrementMessageCount(SapientMessageType.Task);
                    break;

                case SapientMessageType.SensorTaskReleaseControl:
                    Program.MessageMonitor.IncrementMessageCount(SapientMessageType.Task);
                    break;

                case SapientMessageType.AlertResponse:
                    lock (Program.ClientMessageParsers)
                    {
                        ForwardMessage(TaskMessage, Program.ClientComms, connection, "Alert Response Forwarded", "Alert Response Forward failed");
                    }

                    Program.MessageMonitor.IncrementMessageCount(SapientMessageType.AlertResponse);
                    break;

                case SapientMessageType.TaskACK:
                    if (Program.TaskingCommsConnection != null)
                    {
                        ForwardMessage(TaskMessage, Program.TaskingCommsConnection, "Sensor Task Ack");
                    }
                    else
                    {
                        Log.Info("No DMM connected to send task ack to");
                    }

                    Program.MessageMonitor.IncrementMessageCount(SapientMessageType.TaskACK);
                    break;

                case SapientMessageType.Detection:
                    Program.MessageMonitor.IncrementMessageCount(SapientMessageType.Detection);
                    break;

                case SapientMessageType.ResponseIdError:
                    Log.ErrorFormat("Invalid response message received from GUI:Error on response ID: {0}", ErrorString);
                    Program.MessageMonitor.IncrementMessageCount(SapientMessageType.IdError);
                    break;

                case SapientMessageType.IdError:
                    Log.ErrorFormat("Invalid message received from GUI:Error on ID: {0}", ErrorString);
                    Program.MessageMonitor.IncrementMessageCount(SapientMessageType.IdError);
                    break;

                case SapientMessageType.InvalidTasking:
                    Log.ErrorFormat("Invalid message received from GUI: {0}", ErrorString);
                    Program.MessageMonitor.IncrementMessageCount(SapientMessageType.InvalidTasking);
                    break;

                case SapientMessageType.Unknown:
                    Log.ErrorFormat("Unrecognised message received from GUI: {0}", TaskMessage?.ContentCase);
                    Program.MessageMonitor.IncrementMessageCount(SapientMessageType.Unknown);
                    break;

                case SapientMessageType.InternalError:
                    Log.ErrorFormat("GUI Client Error: {0}", ErrorString);
                    Program.MessageMonitor.IncrementMessageCount(SapientMessageType.InternalError);
                    break;

                case SapientMessageType.Unsupported:
                    Log.ErrorFormat("Unsupported Message received from GUI: {0}", TaskMessage?.ContentCase);
                    Program.MessageMonitor.IncrementMessageCount(SapientMessageType.Unknown);
                    break;

                case SapientMessageType.Approval:
                    if (Program.TaskingCommsConnection != null)
                    {
                        ForwardMessage(TaskMessage, Program.TaskingCommsConnection, "Approval");
                    }
                    else
                    {
                        Log.Info("No DMM connected to forward approval to");
                    }

                    break;

                default:
                    Log.ErrorFormat("Unknown Middleware Response: {0}", output);
                    break;
            }
        }

        /// <summary>
        /// Parses and processes the detection report message.
        /// </summary>
        /// <param name="message">The received message.</param>
        /// <param name="database">The database connection.</param>
        /// <returns>Parsed message type or failure mode for further action.</returns>
        protected override SapientMessageType ProcessDetectionReport(SapientMessage message, Database database)
        {
            Log.Info("Detection Message Received from GUI");
            try
            {
                DetectionReport detectionReport;
                string errorString;
                SapientMessageType retval = SapientMessageValidator.ParseDetection(message, out errorString);

                if (retval == SapientMessageType.Detection && database != null)
                {
                    // write detection to HL table in database.
                    database.DbDetection(message, true);
                }

                // parser method returns InvalidClient but we need InvalidTasking to show the source of the error.
                if (retval == SapientMessageType.InvalidClient)
                {
                    retval = SapientMessageType.InvalidTasking;
                }

                return retval;
            }
            catch (Exception e)
            {
                ErrorString = "Internal:GUI:DetectionReport:" + e.Message;
                return SapientMessageType.InternalError;
            }
        }

        /// <summary>
        /// Parses and processes the sensor task message.
        /// </summary>
        /// <param name="message">The received message.</param>
        /// <param name="database">The database connection.</param>
        /// <returns>Parsed message type or failure mode for further action.</returns>
        protected override SapientMessageType ProcessSensorTask(SapientMessage message, Database database)
        {
            Log.Info("Task Message Received from GUI");
            try
            {
                string errorString;
                SapientMessageType retval = SapientMessageValidator.ParseSensorTask(message, out errorString);
                ErrorString = errorString;
                if (retval == SapientMessageType.Task)
                {
                    Task task = message.Task;
                }

                return retval;
            }
            catch (Exception e)
            {
                ErrorString = "Internal:GUI:SensorTask:" + e.Message;
                return SapientMessageType.InternalError;
            }
        }

        /// <summary>
        /// Parses and processes the alert response message.
        /// </summary>
        /// <param name="message">The received message.</param>
        /// <param name="database">The database connection.</param>
        /// <returns>Parsed message type or failure mode for further action.</returns>
        protected override SapientMessageType ProcessAlertResponse(SapientMessage message, Database database)
        {
            Log.Info("Alert Response Message Received from GUI");
            try
            {
                string errorString;
                SapientMessageType retval = SapientMessageValidator.ParseAlertResponse(message, out errorString);
                ErrorString = errorString;

                if (retval == SapientMessageType.AlertResponse)
                {
                    AlertAck alertResponse = message.AlertAck;
                    bool foundClient;
                }

                return retval;
            }
            catch (Exception e)
            {
                ErrorString = "Internal:GUI:AlertResponse:" + e.Message;
                return SapientMessageType.InternalError;
            }
        }

        /// <summary>
        /// Parses and processes a sensor task acknowledgement message.
        /// </summary>
        /// <param name="message">The received message.</param>
        /// <param name="database">The database connection.</param>
        /// <returns>Parsed message type or failure mode for further action.</returns>
        protected override SapientMessageType ProcessSensorTaskAck(SapientMessage message, Database database)
        {
            Log.Info("Sensor Task Ack Message Received from GUI");
            try
            {
                TaskAck sensorTaskAck;
                string errorString;
                SapientMessageType retval = SapientMessageValidator.ParseSensorTaskAck(message, out errorString);
                ErrorString = errorString;

                if (retval == SapientMessageType.TaskACK)
                {
                    // start stop watch for timing diagnostics.
                    Stopwatch stopWatch = new Stopwatch();
                    stopWatch.Start();

                    if (database != null)
                    {
                        // write sensor_task_ack to database
                        database.DbAcknowledgeTask(message, SapientDatabase.DatabaseTables.TaskConstants.GUITaskAck.Table);
                    }

                    // update objective information status from acknowledgement.
                    // ObjectiveInformation.ApproveObjective(sensorTaskAck.TaskId, sensorTaskAck.DestinationId, sensorTaskAck.Status, database);

                    // measure database latency.
                    long databaseLatencyMilliseconds = stopWatch.ElapsedMilliseconds;
                    stopWatch.Stop();
                    Log.Info("GUI TaskAck Database Latency, " + databaseLatencyMilliseconds.ToString("D") + ", ms");
                }

                return retval;
            }
            catch (Exception e)
            {
                ErrorString = "Internal:GUI:SensorTaskACK:" + e.Message;
                return SapientMessageType.InternalError;
            }
        }
    }
}
