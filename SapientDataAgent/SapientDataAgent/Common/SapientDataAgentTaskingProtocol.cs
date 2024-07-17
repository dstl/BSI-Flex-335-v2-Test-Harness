// Project:           $Project: 00010855-Sapient$
// File:              $Workfile: SapientDataAgentTaskingProtocol.cs$
// Crown-owned copyright, 2021-2024
//  See Release/Supply Conditions
// $NoKeywords$

namespace SapientMiddleware.Common
{
    using Google.Protobuf.WellKnownTypes;
    using log4net;
    using Sapient.Data;
    using SapientDatabase;
    using SapientServices;
    using System;
    using System.Linq;

    /// <summary>
    /// class to handle the Sapient Data Data Agent Tasking Protocol.
    /// </summary>
    public class SapientDataAgentTaskingProtocol : SapientProtocol
    {
        /// <summary>
        /// Log4net logger.
        /// </summary>
        private static readonly new ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Initializes a new instance of the <see cref="SapientDataAgentTaskingProtocol"/> class.
        /// </summary>
        public SapientDataAgentTaskingProtocol()
        {
            Supported.Add(SapientMessageType.Task);
            Supported.Add(SapientMessageType.RegistrationACK);
            Supported.Add(SapientMessageType.AlertResponse);
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
                    uint client_connection;
                    lock (Program.ClientMessageParsers)
                    {
                        client_connection = Program.ClientMessageParsers.Keys.Single();
                    }

                    ForwardMessage(TaskMessage, Program.ClientComms, client_connection, "Sensor Task Forwarded", "Sensor Task Forward failed");
                    Program.MessageMonitor.IncrementMessageCount(SapientMessageType.Task);
                    break;

                case SapientMessageType.AlertResponse:
                    lock (Program.ClientMessageParsers)
                    {
                        client_connection = Program.ClientMessageParsers.Keys.Single();
                    }

                    ForwardMessage(TaskMessage, Program.ClientComms, client_connection, "Alert Response Forwarded", "Alert Response Forward failed");
                    Program.MessageMonitor.IncrementMessageCount(SapientMessageType.AlertResponse);
                    break;

                case SapientMessageType.Error:
                    lock (Program.ClientMessageParsers)
                    {
                        client_connection = Program.ClientMessageParsers.Keys.Single();
                    }

                    ForwardMessage(TaskMessage, Program.ClientComms, client_connection, "Error Forwarded", "Error Forward failed");
                    Program.MessageMonitor.IncrementMessageCount(SapientMessageType.Error);
                    break;

                case SapientMessageType.IdError:
                    var task_message = TaskMessage.Task;
                    var task = new SapientMessage
                    {
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
                        Log.Warn("DMM data agent not connected");
                    }

                    Program.MessageMonitor.IncrementMessageCount(SapientMessageType.IdError);
                    break;

                case SapientMessageType.RegistrationACK:
                    lock (Program.ClientMessageParsers)
                    {
                        client_connection = Program.ClientMessageParsers.Keys.Single();
                    }

                    ForwardMessage(TaskMessage, Program.ClientComms, client_connection, "Sensor Registration Ack Forwarded", "Sensor Registration Ack Forward failed");
                    Program.MessageMonitor.IncrementMessageCount(SapientMessageType.RegistrationACK);
                    break;

                case SapientMessageType.InvalidTasking:
                    Log.ErrorFormat("Invalid message received from DMM: {0}", TaskMessage?.ContentCase);
                    Program.MessageMonitor.IncrementMessageCount(SapientMessageType.InvalidTasking);
                    break;

                case SapientMessageType.Unknown:
                    Log.ErrorFormat("Unrecognised message received from DMM: {0}", TaskMessage?.ContentCase);
                    Program.MessageMonitor.IncrementMessageCount(SapientMessageType.Unknown);
                    break;

                case SapientMessageType.InternalError:
                    Log.ErrorFormat("SDA Task Error: {0}", ErrorString);
                    Program.MessageMonitor.IncrementMessageCount(SapientMessageType.InternalError);
                    break;

                case SapientMessageType.Unsupported:
                    Log.ErrorFormat("Unsupported Message received from DMM: {0}", TaskMessage?.ContentCase);
                    Program.MessageMonitor.IncrementMessageCount(SapientMessageType.Unknown);
                    break;

                default:
                    Log.ErrorFormat("Unknown Middleware Response:{0}", output);
                    break;
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
            SapientMessageType retval = SapientMessageType.InvalidTasking;
            try
            {
                //string sensorId = message.Task.DestinationId;
                // bool foundClient;
                //
                // lock (Program.ClientMessageParsers)
                // {
                //     foundClient = Program.ClientMessageParsers.Any(a => a.Value.SapientProtocol.SensorIds.Contains(sensorId));
                // }

                //if (foundClient && (message.Task != null))
                if (message.Task != null)
                {
                    try
                    {
                        if (database != null)
                        {
                            // Task for ASM.
                            // When multiple ASMs are tasked simultaneously, writing the ACKs to the database at the same time causes concurrency errors.
                            // Therefore, move writing of task and ack from DMM Data Agent to Sensor Data Agent so that Acks from different sensors are written to the database on different connections.
                            database.DbTask(message, false);
                        }

                        retval = SapientMessageType.Task;
                    }
                    catch (Exception e)
                    {
                        ErrorString = "Internal:SensorTaskDB:" + e.Message;
                        retval = SapientMessageType.InternalError;
                    }
                }
                // else
                // {
                //     ErrorString = string.Format("No ASM with this ID: {0}", sensorId);
                //     Log.Warn("Sensor Id not on record");
                //     retval = SapientMessageType.IdError;
                // }
            }
            catch (Exception e)
            {
                ErrorString = "Internal:SensorTask:" + e.Message;
                retval = SapientMessageType.InternalError;
            }

            return retval;
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
            string sensorId;
            AlertAck alert;
            SapientMessageType retval = SapientMessageType.InvalidTasking;
            try
            {
                //sensorId = message.AlertAck.DestinationId;

                // bool foundClient;
                // lock (Program.ClientMessageParsers)
                // {
                //     foundClient = Program.ClientMessageParsers.Any(a => a.Value.SapientProtocol.SensorIds.Contains(sensorId));
                // }

                //if (foundClient)
                if (message.AlertAck != null)
                {
                    try
                    {
                        if (database != null)
                        {
                            database.DbAcknowledgeAlert(message);
                        }

                        retval = SapientMessageType.AlertResponse;
                    }
                    catch (Exception e)
                    {
                        Log.Error("Exception parsing AlertResponse message", e);
                        ErrorString = "Internal:AlertResponseDB:" + e.Message;
                        retval = SapientMessageType.InternalError;
                    }
                }
                // else
                // {
                //     Log.Warn("AlertResponse:Sensor Id not on record");
                //     retval = SapientMessageType.IdError;
                // }
            }
            catch (Exception e)
            {
                ErrorString = "Internal:AlertResponse:" + e.Message;
                retval = SapientMessageType.InternalError;
            }

            return retval;
        }

        /// <summary>
        /// Parses and processes a sensor registration acknowledgement message.
        /// </summary>
        /// <param name="message">The received message.</param>
        /// <param name="database">The database connection.</param>
        /// <returns>Parsed message type or failure mode for further action.</returns>
        protected override SapientMessageType ProcessRegistrationAck(SapientMessage message, Database database)
        {
            Log.Info("Sensor Registration Ack Message Received");
            SapientMessageType retval = SapientMessageType.InvalidTasking;
            var deserializeError = string.Empty;
            if (message.RegistrationAck != null)
            {
                try
                {
                    var client = Program.ClientMessageParsers.First();
                    string portText = string.Format("SDA:{0}: Port {1}", message.NodeId, Program.ClientPort);
                    Program.SetWindowText(portText);
                    retval = SapientMessageType.RegistrationACK;
                }
                catch (Exception e)
                {
                    Log.Error("Exception parsing SensorRegistrationACK message", e);
                    ErrorString = "Internal:SensorRegistrationACK:" + e.Message;
                    retval = SapientMessageType.InternalError;
                }
            }
            else
            {
                ErrorString = "Deserialize: " + deserializeError;
                retval = SapientMessageType.InvalidTasking;
            }

            return retval;
        }
    }
}
