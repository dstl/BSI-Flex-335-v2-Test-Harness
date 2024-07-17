// Project:           $Project: 00010855-Sapient$
// File:              $Workfile: DmmDataAgentClientProtocol.cs$
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
    using System.Linq;

    /// <summary>
    /// class to handle the Sapient DMM Data Agent Client Protocol.
    /// </summary>
    public class DmmDataAgentClientProtocol : SapientProtocol
    {
        /// <summary>
        /// Log4net logger.
        /// </summary>
        private static readonly new ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Fixed ASM id specified in the config file.
        /// </summary>
        protected string fixedHDAId;

        /// <summary>
        /// client sensor Identifier.
        /// </summary>
        protected string sensorId;

        /// <summary>
        /// Initializes a new instance of the <see cref="DmmDataAgentClientProtocol" /> class.
        /// </summary>
        /// <param name="fixedASMId">fixed ASM ID from config file.</param>
        public DmmDataAgentClientProtocol(string fixedASMId)
        {
            this.sensorId = fixedASMId;
            this.fixedHDAId = Properties.Settings.Default.FixedHDAId;
            Supported.Add(SapientMessageType.Registration);
            Supported.Add(SapientMessageType.Alert);
            Supported.Add(SapientMessageType.Status);
            Supported.Add(SapientMessageType.Detection);
            Supported.Add(SapientMessageType.TaskACK);
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
                case SapientMessageType.Registration:
                    lock (Program.ClientMessageParsers)
                    {
                        if (SensorIds.Contains(sensorId) == false)
                        {
                            SensorIds.Add(sensorId);
                        }
                    }

                    if (Program.TaskingCommsConnection != null)
                    {
                        ForwardMessage(TaskMessage, Program.TaskingCommsConnection, "Registration");
                    }
                    else
                    {
                        Log.Warn("DMM not connected");
                    }

                    Program.MessageMonitor.IncrementMessageCount(SapientMessageType.Registration);
                    break;

                case SapientMessageType.TaskACK:
                    if (Program.TaskingCommsConnection != null)
                    {
                        ForwardMessage(TaskMessage, Program.TaskingCommsConnection, "Sensor Task Ack");
                    }
                    else
                    {
                        Log.Warn("DMM not connected");
                    }

                    Program.MessageMonitor.IncrementMessageCount(SapientMessageType.TaskACK);
                    break;

                case SapientMessageType.Alert:
                    if (Program.TaskingCommsConnection != null)
                    {
                        ForwardMessage(TaskMessage, Program.TaskingCommsConnection, "Alert");
                    }
                    else
                    {
                        Log.Warn("DMM not connected");
                    }
                    Program.MessageMonitor.IncrementMessageCount(SapientMessageType.Alert);
                    break;

                case SapientMessageType.Status:
                    if (Program.TaskingCommsConnection != null)
                    {
                        ForwardMessage(TaskMessage, Program.TaskingCommsConnection, "Status");
                    }
                    else
                    {
                        Log.Warn("DMM not connected");
                    }
                    Program.MessageMonitor.IncrementMessageCount(SapientMessageType.Status);
                    break;

                case SapientMessageType.Detection:
                    if (Program.TaskingCommsConnection != null)
                    {
                        ForwardMessage(TaskMessage, Program.TaskingCommsConnection, "Detection");
                    }
                    else
                    {
                        Log.Warn("DMM not connected");
                    }
                    Program.MessageMonitor.IncrementMessageCount(SapientMessageType.Detection);
                    break;

                case SapientMessageType.Error:
                    if (Program.TaskingCommsConnection != null)
                    {
                        ForwardMessage(TaskMessage, Program.TaskingCommsConnection, "Error");
                    }
                    else
                    {
                        Log.Warn("DMM not connected");
                    }
                    Program.MessageMonitor.IncrementMessageCount(SapientMessageType.Error);
                    break;

                case SapientMessageType.IdError:
                    {
                        string nodeId = Ulid.NewUlid().ToString();
                        Ulid temp;
                        if (!string.IsNullOrEmpty(TaskMessage.NodeId.Trim()) && Ulid.TryParse(TaskMessage.NodeId, out temp))
                        {
                            nodeId = TaskMessage.NodeId;
                        }
                        var error_message = new SapientMessage
                        {
                            Timestamp = Timestamp.FromDateTime(DateTime.UtcNow),
                            NodeId = nodeId,
                            Error = new Error
                            {
                                Packet = TaskMessage.ToByteString(),
                            },
                        };
                        error_message.Error.ErrorMessage.Add(ErrorString);

                        ForwardMessage(error_message, Program.ClientComms, connection, "Invalid Message - Error returned: " + ErrorString, "Invalid message - Error return failed: " + ErrorString);
                        Log.ErrorFormat("Invalid message received from SDA:Error on ID: {0}", ErrorString);

                        Program.MessageMonitor.IncrementMessageCount(SapientMessageType.IdError);
                    }
                    break;

                case SapientMessageType.InvalidClient:
                    Log.ErrorFormat("Invalid message received from SDA:InvalidClient: {0}", ErrorString);
                    Program.MessageMonitor.IncrementMessageCount(SapientMessageType.InvalidClient);
                    break;

                case SapientMessageType.Unknown:
                    Log.ErrorFormat("Unrecognised message received from SDA: {0}", TaskMessage?.ContentCase);
                    Program.MessageMonitor.IncrementMessageCount(SapientMessageType.Unknown);
                    break;

                case SapientMessageType.InternalError:
                    Log.ErrorFormat("HDA Client Error: {0}", ErrorString);
                    Program.MessageMonitor.IncrementMessageCount(SapientMessageType.InternalError);
                    break;

                case SapientMessageType.Unsupported:
                    Log.ErrorFormat("Unsupported Message received from SDA: {0}", TaskMessage?.ContentCase);
                    Program.MessageMonitor.IncrementMessageCount(SapientMessageType.Unknown);
                    break;

                default:
                    Log.ErrorFormat("Unknown Middleware Response: {0}", output);
                    break;
            }
        }

        /// <summary>
        /// Parses and processes a sensor registration message.
        /// </summary>
        /// <param name="message">The received message.</param>
        /// <param name="database">The database connection.</param>
        /// <returns>Parsed message type or failure mode for further action.</returns>
        protected override SapientMessageType ProcessRegistration(SapientMessage message, Database database)
        {
            Log.Info("Sensor Registration Message Received");
            try
            {
                Registration registration;
                string errorString;
                SapientMessageType retval = SapientMessageValidator.ParseRegistration(message, out errorString);
                ErrorString = errorString;

                if (retval == SapientMessageType.Registration)
                {
                    var sensor_id = message.NodeId;
                    lock (Program.ClientMessageParsers)
                    {
                        var all_ids = Program.ClientMessageParsers.Values.SelectMany(a => a.SapientProtocol.SensorIds);
                        sensorId = sensor_id;
                        if (SensorIds.Contains(sensor_id))
                        {
                        }
                        else if (all_ids.Contains(sensor_id))
                        {
                            // id in use by another client
                            ErrorString = "Another ASM is using this ID: " + sensor_id;
                            retval = SapientMessageType.IdError;
                        }
                    }

                    if (database != null)
                    {
                        database.DbRegistration(message);

                        if (AdditionalDatabase != null)
                        {
                            AdditionalDatabase.DbRegistration(message);
                        }
                    }
                }

                return retval;
            }
            catch (Exception e)
            {
                ErrorString = "Internal:SensorRegistration:" + e.Message;
                return SapientMessageType.InternalError;
            }
        }

        /// <summary>
        /// Parses and processes an alert message.
        /// </summary>
        /// <param name="message">The received message.</param>
        /// <param name="database">database to write to.</param>
        /// <returns>Parsed message type or failure mode for further action.</returns>
        protected override SapientMessageType ProcessAlert(SapientMessage message, Database database)
        {
            Log.Info("Alert Message Received:");
            try
            {
                string errorString;
                SapientMessageType retval = SapientMessageValidator.ParseAlert(message, out errorString);
                ErrorString = errorString;

                if (retval == SapientMessageType.Alert)
                {
                    var sourceId = message.NodeId;
                    bool found;
                    lock (Program.ClientMessageParsers)
                    {
                        found = SensorIds.Contains(sourceId);
                    }

                    if (!found)
                    {
                        ErrorString = string.Format("No ASM with this ID: {0}", sourceId);
                        retval = SapientMessageType.IdError;
                    }

                    // if OK, do nothing other than forward on message in Send Response.
                }

                return retval;
            }
            catch (Exception e)
            {
                ErrorString = "Internal:Alert:" + e.Message;
                return SapientMessageType.InternalError;
            }
        }

        protected override SapientMessageType ProcessStatusReport(SapientMessage message, Database database)
        {
            Log.Info("Status Message Received:");
            try
            {
                string errorString;
                SapientMessageType retval = SapientMessageValidator.ParseStatusReport(message, out errorString);
                ErrorString = errorString;

                if (retval == SapientMessageType.Status)
                {
                    var sourceId = message.NodeId;
                    bool found;
                    lock (Program.ClientMessageParsers)
                    {
                        found = SensorIds.Contains(sourceId);
                    }

                    if (!found)
                    {
                        ErrorString = string.Format("No ASM with this ID: {0}", sourceId);
                        retval = SapientMessageType.IdError;
                    }
                }

                return retval;
            }
            catch (Exception e)
            {
                ErrorString = "Internal:Status:" + e.Message;
                return SapientMessageType.InternalError;
            }
        }

        protected override SapientMessageType ProcessDetectionReport(SapientMessage message, Database database)
        {
            Log.Info("Status Message Received:");
            try
            {
                string errorString;
                SapientMessageType retval = SapientMessageValidator.ParseDetection(message, out errorString);
                ErrorString = errorString;

                if (retval == SapientMessageType.Status)
                {
                    var sourceId = message.NodeId;
                    bool found;
                    lock (Program.ClientMessageParsers)
                    {
                        found = SensorIds.Contains(sourceId);
                    }

                    if (!found)
                    {
                        ErrorString = string.Format("No ASM with this ID: {0}", sourceId);
                        retval = SapientMessageType.IdError;
                    }
                }

                return retval;
            }
            catch (Exception e)
            {
                ErrorString = "Internal:Status:" + e.Message;
                return SapientMessageType.InternalError;
            }
        }

        /// <summary>
        /// Parses and processes a sensor task acknowledgement message.
        /// </summary>
        /// <param name="message">The received message.</param>
        /// <param name="database">database to write to.</param>
        /// <returns>Parsed message type or failure mode for further action.</returns>
        protected override SapientMessageType ProcessSensorTaskAck(SapientMessage message, Database database)
        {
            Log.Info("Sensor Task Ack Message Received");
            string errorString;
            TaskAck sensor_task_ack;
            SapientMessageType retval = SapientMessageValidator.ParseSensorTaskAck(message, out errorString);
            ErrorString = errorString;
            return retval;
        }
    }
}
