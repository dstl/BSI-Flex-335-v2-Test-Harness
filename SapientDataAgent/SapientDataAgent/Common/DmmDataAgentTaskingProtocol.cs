// Project:           $Project: 00010855-Sapient$
// File:              $Workfile: dmmDataAgentTaskingProtocol.cs$
// Crown-owned copyright, 2021-2024
//  See Release/Supply Conditions
// $NoKeywords$

using Google.Protobuf;

namespace SapientMiddleware.Common
{
    using Google.Protobuf.WellKnownTypes;
    using log4net;
    using Sapient.Data;
    using SapientDatabase;
    using SapientServices;
    using System;
    using System.Diagnostics;

    /// <summary>
    /// class to handle the Sapient DMM Data Agent Tasking Protocol.
    /// </summary>
    public class DmmDataAgentTaskingProtocol : SapientProtocol
    {
        /// <summary>
        /// Log4net logger.
        /// </summary>
        private static readonly new ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// time histogram of database latency.
        /// </summary>
        protected Histogram databaseLatencyHistogram;

        /// <summary>
        /// time histogram of communication latency.
        /// </summary>
        protected Histogram communicationLatencyHistogram;

        /// <summary>
        /// Holds task permission information.
        /// </summary>
        protected TaskPermissions taskPermissions;

        /// <summary>
        /// Initializes a new instance of the <see cref="DmmDataAgentTaskingProtocol" /> class.
        /// </summary>
        public DmmDataAgentTaskingProtocol(TaskPermissions taskPermissions)
        {
            this.taskPermissions = taskPermissions;
            databaseLatencyHistogram = new Histogram();
            communicationLatencyHistogram = new Histogram();
            Supported.Add(SapientMessageType.Detection);
            Supported.Add(SapientMessageType.Status);
            Supported.Add(SapientMessageType.Alert);
            Supported.Add(SapientMessageType.Task);
            Supported.Add(SapientMessageType.TaskACK);
            Supported.Add(SapientMessageType.AlertResponse);
            Supported.Add(SapientMessageType.Objective);
            Supported.Add(SapientMessageType.Error);
        }

        /// <summary>
        /// Send response message triggered from parsed message.
        /// </summary>
        /// <param name="output">parsing result.</param>
        /// <param name="connection">connection message received on.</param>
        public override void SendResponse(SapientMessageType output, uint connection)
        {

            switch (output)
            {
                case SapientMessageType.Task:
                    {
                        uint destinationConnection = 0;
                        bool destinationConenctionFound = false;
                        if (!string.IsNullOrEmpty(TaskMessage.DestinationId))
                        {
                            lock (Program.ClientMessageParsers)
                            {
                                foreach (KeyValuePair<uint, SapientMessageParser> clientMessageParser in Program.ClientMessageParsers)
                                {
                                    if (clientMessageParser.Value.SapientProtocol.SensorIds.Contains(TaskMessage.DestinationId))
                                    {
                                        destinationConnection = clientMessageParser.Key;
                                        destinationConenctionFound = true;
                                        break;
                                    }
                                }
                            }
                        }

                        if (!destinationConenctionFound)
                        {
                            Log.Error($"Unable to find a sensor for destination id '{TaskMessage!.DestinationId}' in task message.");
                        }

                        ForwardMessage(TaskMessage, Program.ClientComms, destinationConnection, "Sensor Task Forwarded", "Sensor Task Forward failed");

                        Program.MessageMonitor.IncrementMessageCount(SapientMessageType.Task);
                    }
                    break;

                case SapientMessageType.RegistrationACK:
                    {
                        uint destinationConnection = 0;
                        bool destinationConenctionFound = false;
                        if (!string.IsNullOrEmpty(TaskMessage.DestinationId))
                        {
                            lock (Program.ClientMessageParsers)
                            {
                                foreach (KeyValuePair<uint, SapientMessageParser> clientMessageParser in Program.ClientMessageParsers)
                                {
                                    if (clientMessageParser.Value.SapientProtocol.SensorIds.Contains(TaskMessage.DestinationId))
                                    {
                                        destinationConnection = clientMessageParser.Key;
                                        destinationConenctionFound = true;
                                        break;
                                    }
                                }
                            }
                        }

                        if (!destinationConenctionFound)
                        {
                            Log.Error($"Unable to find a sensor for destination id '{TaskMessage!.DestinationId}' in registration Ack message.");
                        }

                        ForwardMessage(TaskMessage, Program.ClientComms, destinationConnection, "Sensor Registration Ack Forwarded", "Sensor Registration Ack Forward failed");

                        Program.MessageMonitor.IncrementMessageCount(SapientMessageType.RegistrationACK);
                    }
                    break;

                case SapientMessageType.Error:
                    {
                        uint destinationConnection = 0;
                        bool destinationConenctionFound = false;
                        if (!string.IsNullOrEmpty(TaskMessage.DestinationId))
                        {
                            lock (Program.ClientMessageParsers)
                            {
                                foreach (KeyValuePair<uint, SapientMessageParser> clientMessageParser in Program.ClientMessageParsers)
                                {
                                    if (clientMessageParser.Value.SapientProtocol.SensorIds.Contains(TaskMessage.DestinationId))
                                    {
                                        destinationConnection = clientMessageParser.Key;
                                        destinationConenctionFound = true;
                                        break;
                                    }
                                }
                            }
                        }

                        if (!destinationConenctionFound)
                        {
                            Log.Error($"Unable to find a sensor for destination id '{TaskMessage!.DestinationId}' in registration Ack message.");
                        }

                        ForwardMessage(TaskMessage, Program.ClientComms, destinationConnection, "Sensor Error Forwarded", "Sensor Erro Forward failed");

                        Program.MessageMonitor.IncrementMessageCount(SapientMessageType.Error);
                    }
                    break;


                case SapientMessageType.AlertResponse:
                    {
                        uint destinationConnection = 0;
                        bool destinationConenctionFound = false;
                        if (!string.IsNullOrEmpty(TaskMessage.DestinationId))
                        {
                            lock (Program.ClientMessageParsers)
                            {
                                foreach (KeyValuePair<uint, SapientMessageParser> clientMessageParser in Program.ClientMessageParsers)
                                {
                                    if (clientMessageParser.Value.SapientProtocol.SensorIds.Contains(TaskMessage.DestinationId))
                                    {
                                        destinationConnection = clientMessageParser.Key;
                                        destinationConenctionFound = true;
                                        break;
                                    }
                                }
                            }
                        }

                        if (!destinationConenctionFound)
                        {
                            Log.Error($"Unable to find a sensor for destination id '{TaskMessage!.DestinationId}' in alert response message.");
                        }

                        ForwardMessage(TaskMessage, Program.ClientComms, destinationConnection, "Alert Response Forwarded", "Alert Response Forward failed");

                        Program.MessageMonitor.IncrementMessageCount(SapientMessageType.AlertResponse);
                    }
                    break;

                case SapientMessageType.ResponseIdError:
                    {
                        string nodeId = Ulid.NewUlid().ToString();
                        Ulid temp;
                        if (!string.IsNullOrEmpty(TaskMessage.NodeId.Trim()) && Ulid.TryParse(TaskMessage.NodeId, out temp))
                        {
                            nodeId = TaskMessage.NodeId;
                        }
                        var error = new SapientMessage
                        {
                            Timestamp = Timestamp.FromDateTime(DateTime.UtcNow),
                            NodeId = nodeId,
                            Error = new Error
                            {
                                Packet = TaskMessage.ToByteString()
                            },
                        };
                        error.Error.ErrorMessage.Add($"No ASM with this Id: {TaskMessage.NodeId}");

                        if (Program.TaskingCommsConnection != null)
                        {
                            ForwardMessage(error, Program.TaskingCommsConnection, "Error");
                        }
                        else
                        {
                            Log.Warn("DMM not connected");
                        }

                        Log.ErrorFormat("Invalid response message received from DMM:Error on response ID: {0}", ErrorString);
                        Program.MessageMonitor.IncrementMessageCount(SapientMessageType.IdError);
                    }
                    break;

                case SapientMessageType.IdError:
                    var task_message = TaskMessage.Task;

                    var task = new SapientMessage
                    {
                        NodeId = TaskMessage.NodeId,
                        Timestamp = Timestamp.FromDateTime(DateTime.UtcNow),
                        TaskAck = new TaskAck
                        {
                            TaskId = task_message.TaskId,
                            TaskStatus = TaskAck.Types.TaskStatus.Rejected
                        },
                    };
                    task.TaskAck.Reason.Add("No ASM with this ID");

                    if (Program.TaskingCommsConnection != null)
                    {
                        ForwardMessage(task, Program.TaskingCommsConnection, "Sensor Task Ack");
                    }
                    else
                    {
                        Log.Warn("DMM not connected");
                    }

                    Log.ErrorFormat("Invalid message received from DMM:Error on ID: {0}", ErrorString);
                    Program.MessageMonitor.IncrementMessageCount(SapientMessageType.IdError);
                    break;

                case SapientMessageType.InvalidTasking:
                    {
                        string nodeId = Ulid.NewUlid().ToString();
                        Ulid temp;
                        if (!string.IsNullOrEmpty(TaskMessage.NodeId.Trim()) && Ulid.TryParse(TaskMessage.NodeId, out temp))
                        {
                            nodeId = TaskMessage.NodeId;
                        }
                        var e_task = new SapientMessage
                        {
                            Timestamp = Timestamp.FromDateTime(DateTime.UtcNow),
                            NodeId = nodeId,
                            Error = new Error
                            {
                                Packet = TaskMessage.ToByteString()
                            },
                        };
                        e_task.Error.ErrorMessage.Add(ErrorString);

                        if (Program.TaskingCommsConnection != null)
                        {
                            ForwardMessage(e_task, Program.TaskingCommsConnection, "Error");
                        }
                        else
                        {
                            Log.Warn("DMM not connected");
                        }

                        Log.ErrorFormat("Invalid message received from DMM: {0}", TaskMessage?.ContentCase);
                        Program.MessageMonitor.IncrementMessageCount(SapientMessageType.InvalidTasking);
                    }
                    break;

                case SapientMessageType.Unknown:
                    {
                        string nodeId = Ulid.NewUlid().ToString();
                        Ulid temp;
                        if (!string.IsNullOrEmpty(TaskMessage.NodeId.Trim()) && Ulid.TryParse(TaskMessage.NodeId, out temp))
                        {
                            nodeId = TaskMessage.NodeId;
                        }
                        var e_task = new SapientMessage
                        {
                            Timestamp = Timestamp.FromDateTime(DateTime.UtcNow),
                            NodeId = nodeId,
                            Error = new Error
                            {
                                Packet = TaskMessage.ToByteString()
                            },
                        };
                        e_task.Error.ErrorMessage.Add("Unrecognised message received");

                        if (Program.TaskingCommsConnection != null)
                        {
                            ForwardMessage(e_task, Program.TaskingCommsConnection, "Error");
                        }
                        else
                        {
                            Log.Warn("DMM not connected");
                        }

                        Log.ErrorFormat("Unrecognised message received from DMM: {0}", TaskMessage?.ContentCase);
                        Program.MessageMonitor.IncrementMessageCount(SapientMessageType.Unknown);
                    }
                    break;

                case SapientMessageType.Detection:
                case SapientMessageType.Status:
                case SapientMessageType.Alert:
                case SapientMessageType.Objective:
                case SapientMessageType.TaskACK:
                    Program.MessageMonitor.IncrementMessageCount(output);
                    break;

                case SapientMessageType.SensorTaskDropped:
                    // Do we want to record this as a 'task' or add a new category for dropped tasks?
                    Program.MessageMonitor.IncrementMessageCount(SapientMessageType.Task);
                    break;

                case SapientMessageType.InternalError:
                    Log.ErrorFormat("HDA Task Error: {0}", ErrorString);
                    Program.MessageMonitor.IncrementMessageCount(SapientMessageType.InternalError);
                    break;

                case SapientMessageType.Unsupported:
                    Log.ErrorFormat("Unsupported Message received from DMM: {0}", TaskMessage?.ContentCase);
                    Program.MessageMonitor.IncrementMessageCount(SapientMessageType.Unknown);
                    break;

                default:
                    Log.ErrorFormat("Unknown Middleware Response: {0}", output);
                    break;
            }
        }

        /// <summary>
        /// Parses and processes a detection report message.
        /// </summary>
        /// <param name="message">The received message.</param>
        /// <param name="database">The database connection.</param>
        /// <returns>Parsed message type or failure mode for further action.</returns>
        protected override SapientMessageType ProcessDetectionReport(SapientMessage message, Database database)
        {
            Log.Info("HL Detection Message Received:");
            try
            {
                DetectionReport detectionReport;
                string errorString;
                SapientMessageType retval = SapientMessageValidator.ParseDetection(message, out errorString);

                if (retval == SapientMessageType.Detection)
                {
                    // start stop watch for timing diagnostics.
                    Stopwatch stopWatch = new Stopwatch();
                    stopWatch.Start();

                    if (database != null)
                    {
                        database.DbDetection(message, true);
                    }

                    // measure communications and database latency.
                    long databaseLatencyMilliseconds = stopWatch.ElapsedMilliseconds;
                    databaseLatencyHistogram.Add(databaseLatencyMilliseconds);

                    double latency = (DateTime.UtcNow - message.Timestamp.ToDateTime()).TotalMilliseconds;
                    communicationLatencyHistogram.Add(latency);

                    Log.Info("DetectionReport Latency, " + latency.ToString("F1") + ", ms, " + communicationLatencyHistogram.Print());
                    Log.Info("Database Latency, " + databaseLatencyMilliseconds.ToString("D") + ", ms, " + databaseLatencyHistogram.Print());

                    // Don't update latency based on detection reports as these are dependent on original data so there is additional latency to just comms.
                    Program.MessageMonitor.SetLatency(1, databaseLatencyMilliseconds);
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
                ErrorString = "Internal:HL DetectionReport:" + e.Message;
                return SapientMessageType.InternalError;
            }
        }

        /// <summary>
        /// Parses and processes a status report message.
        /// </summary>
        /// <param name="message">The received message.</param>
        /// <param name="database">The database connection.</param>
        /// <returns>Parsed message type or failure mode for further action.</returns>
        protected override SapientMessageType ProcessStatusReport(SapientMessage message, Database database)
        {
            Log.Info("HL Status Report Message Received");
            try
            {
                string errorString;
                SapientMessageType retval = SapientMessageValidator.ParseStatusReport(message, out errorString);
                ErrorString = errorString;

                if (retval == SapientMessageType.Status)
                {
                    string sensorStatus = message.StatusReport.System.ToString();
                    Program.MessageMonitor.SetStatusText(1, sensorStatus);

                    // start stop watch for timing diagnostics.
                    Stopwatch stopWatch = new Stopwatch();
                    stopWatch.Start();

                    if (database != null)
                    {
                        // write data to database.
                        database.DbHLStatus(message);
                    }

                    // measure communications and database latency.
                    long databaseLatencyMilliseconds = stopWatch.ElapsedMilliseconds;
                    databaseLatencyHistogram.Add(databaseLatencyMilliseconds);

                    double latency = (DateTime.UtcNow - message.Timestamp.ToDateTime()).TotalMilliseconds;
                    communicationLatencyHistogram.Add(latency);

                    Log.Info("StatusReport Latency, " + latency.ToString("F1") + ", ms, " + communicationLatencyHistogram.Print());
                    Log.Info("Database Latency, " + databaseLatencyMilliseconds.ToString("D") + ", ms, " + databaseLatencyHistogram.Print());
                    Program.MessageMonitor.SetLatency(0, latency);
                    Program.MessageMonitor.SetLatency(1, databaseLatencyMilliseconds);
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
                ErrorString = "Internal:HL Status Report:" + e.Message;
                return SapientMessageType.InternalError;
            }
        }

        /// <summary>
        /// Parses and processes an alert message.
        /// </summary>
        /// <param name="message">The received message.</param>
        /// <param name="database">The database connection.</param>
        /// <returns>Parsed message type or failure mode for further action.</returns>
        protected override SapientMessageType ProcessAlert(SapientMessage message, Database database)
        {
            Log.Info("HL Alert Message Received:");
            try
            {
                Alert alert;
                string errorString;
                SapientMessageType retval = SapientMessageValidator.ParseAlert(message, out errorString);
                ErrorString = errorString;

                if (retval == SapientMessageType.Alert)
                {
                    // start stop watch for timing diagnostics.
                    Stopwatch stopWatch = new Stopwatch();
                    stopWatch.Start();

                    if (database != null)
                    {
                        // write data to database.
                        database.DbAlert(message, true);
                    }

                    // measure communications and database latency.
                    long databaseLatencyMilliseconds = stopWatch.ElapsedMilliseconds;
                    databaseLatencyHistogram.Add(databaseLatencyMilliseconds);

                    double latency = (DateTime.UtcNow - message.Timestamp.ToDateTime()).TotalMilliseconds;
                    communicationLatencyHistogram.Add(latency);

                    Log.Info("Alert Latency, " + latency.ToString("F1") + ", ms, " + communicationLatencyHistogram.Print());
                    Log.Info("Database Latency, " + databaseLatencyMilliseconds.ToString("D") + ", ms, " + databaseLatencyHistogram.Print());
                    Program.MessageMonitor.SetLatency(0, latency);
                    Program.MessageMonitor.SetLatency(1, databaseLatencyMilliseconds);
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
                ErrorString = "Internal:HL Alert:" + e.Message;
                return SapientMessageType.InternalError;
            }
        }

        /// <summary>
        /// Parses and processes a sensor task message.
        /// </summary>
        /// <param name="message">The received message.</param>
        /// <param name="database">The database connection.</param>
        /// <returns>Parsed message type or failure mode for further action.</returns>
        protected override SapientMessageType ProcessSensorTask(SapientMessage message, Database database)
        {
            Log.Info("Sensor Task Message Received");
            try
            {
                string errorString;
                SapientMessageType retval = SapientMessageValidator.ParseSensorTask(message, out errorString);
                ErrorString = errorString;
                if (retval == SapientMessageType.Task)
                {
                    //var sensorId = message.Task.DestinationId;
                    /*if (taskPermissions.DmmHasControl(sensorId))
                    {
                        bool foundClient;
                        lock (Program.ClientMessageParsers)
                        {
                            foundClient = Program.ClientMessageParsers.Any(a => a.Value.SapientProtocol.SensorIds.Contains(sensorId));
                        }

                        if (!foundClient)
                        {
                            ErrorString = string.Format("No ASM with this ID: {0}", sensorId);
                            retval = SapientMessageType.IdError;
                        }
                    }
                    else
                    {
                        // GUI has control over this sensor, so the DMM will drop sensor tasks from it.
                        Log.Info($"GUI currently has control of sensor with ID {sensorId}. Rejecting this task.");

                        retval = SapientMessageType.SensorTaskDropped;
                    }*/
                }

                return retval;
            }
            catch (Exception e)
            {
                ErrorString = "Internal:HL SensorTask:" + e.Message;
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
            Log.Info("Sensor Task Ack Message Received from DMM");
            try
            {
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
                        // write sensor_task_ack to database.
                        database.DbAcknowledgeTask(message, SapientDatabase.DatabaseTables.TaskConstants.HLTaskAck.Table);
                    }

                    // measure database latency.
                    long databaseLatencyMilliseconds = stopWatch.ElapsedMilliseconds;
                    stopWatch.Stop();
                    Log.Info("HL TaskAck Database Latency, " + databaseLatencyMilliseconds.ToString("D") + ", ms");
                }

                return retval;
            }
            catch (Exception e)
            {
                ErrorString = "Internal:HL SensorTaskACK:" + e.Message;
                return SapientMessageType.InternalError;
            }
        }

        /// <summary>
        /// Parses and processes an alert response message.
        /// </summary>
        /// <param name="message">The received message.</param>
        /// <param name="database">The database connection.</param>
        /// <returns>Parsed message type or failure mode for further action.</returns>
        protected override SapientMessageType ProcessAlertResponse(SapientMessage message, Database database)
        {
            Log.Info("Alert Response Message Received");
            try
            {
                string errorString;
                SapientMessageType retval = SapientMessageValidator.ParseAlertResponse(message, out errorString);
                ErrorString = errorString;

                if (retval == SapientMessageType.AlertResponse)
                {
                    //var sensorId = message.AlertAck.DestinationId;
                    bool foundClient;
                    /*lock (Program.ClientMessageParsers)
                    {
                        foundClient = Program.ClientMessageParsers.Any(a => a.Value.SapientProtocol.SensorIds.Contains(sensorId));
                    }

                    if (!foundClient)
                    {
                        Log.Warn("AlertResponse:Sensor Id not on record");
                        retval = SapientMessageType.ResponseIdError;
                    }*/
                }

                return retval;
            }
            catch (Exception e)
            {
                ErrorString = "Internal:HL AlertResponse:" + e.Message;
                return SapientMessageType.InternalError;
            }
        }
    }
}
