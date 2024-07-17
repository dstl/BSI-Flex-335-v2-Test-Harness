// Project:           $Project: 00010855-Sapient$
// File:              $Workfile: SapientProtocol.cs$
// Crown-owned copyright, 2021-2024
//  See Release/Supply Conditions
// $NoKeywords$

namespace SapientMiddleware
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Xml.Serialization;
    using log4net;
    using Sapient.Data;
    using SapientDatabase;
    using SapientServices;
    using SapientServices.Communication;

    /// <summary>
    /// abstract class to support the Sapient Message Protocol.
    /// </summary>
    public abstract class SapientProtocol
    {
        protected readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        protected SapientMessage TaskMessage;
        protected string ErrorString;

        /// <summary>
        /// List of supported message typw.
        /// </summary>
        protected HashSet<SapientMessageType> Supported = new HashSet<SapientMessageType>();

        public List<string> SensorIds = new List<string>();

        public uint ConnectionId;

        /// <summary>
        /// Gets or sets additional database for writing to.
        /// </summary>
        public static Database AdditionalDatabase { get; set; }

        /// <summary>
        /// Parse messages and write contents to database if required.
        /// </summary>
        /// <param name="message">received message.</param>
        /// <param name="database">database connection.</param>
        /// <returns>parse result.</returns>
        public virtual SapientMessageType ParseReceivedMessage(SapientMessage message, Database database)
        {
            Log.DebugFormat("Called with message: {0}", message?.ContentCase);

            TaskMessage = message;

            SapientMessageType parsedMsgType = SapientMessageType.Unsupported;

            switch (message.ContentCase)
            {
                case SapientMessage.ContentOneofCase.DetectionReport:
                    parsedMsgType = ProcessDetectionReport(message, database);
                    break;

                case SapientMessage.ContentOneofCase.StatusReport:
                    parsedMsgType = ProcessStatusReport(message, database);
                    break;

                case SapientMessage.ContentOneofCase.Alert:
                    parsedMsgType = ProcessAlert(message, database);
                    break;

                case SapientMessage.ContentOneofCase.TaskAck:
                    parsedMsgType = ProcessSensorTaskAck(message, database);
                    break;

                case SapientMessage.ContentOneofCase.Registration:
                    parsedMsgType = ProcessRegistration(message, database);
                    break;

                case SapientMessage.ContentOneofCase.RegistrationAck:
                    parsedMsgType = ProcessRegistrationAck(message, database);
                    break;

                case SapientMessage.ContentOneofCase.Task:
                    parsedMsgType = ProcessSensorTask(message, database);
                    break;

                case SapientMessage.ContentOneofCase.AlertAck:
                    parsedMsgType = ProcessAlertResponse(message, database);
                    break;

                case SapientMessage.ContentOneofCase.Error:
                    parsedMsgType = ParseErrorMessage(message);
                    break;

                default:
                    Log.InfoFormat("Unrecognised Message Received: {0}", message?.ContentCase);
                    parsedMsgType = SapientMessageType.Unknown;
                    break;
            }

            return parsedMsgType;
        }

        /// <summary>
        /// Send response message triggered from parsed message.
        /// </summary>
        /// <param name="output">parsing result.</param>
        /// <param name="connection">connection message received on.</param>
        public abstract void SendResponse(SapientMessageType output, uint connection);

        /// <summary>
        /// Wrapper around generic deserialize.
        /// </summary>
        /// <param name="ser"> type of object to convert to.</param>
        /// <param name="xml_string">xml string of the target class type.</param>
        /// <param name="error">deserialization error message.</param>
        /// <returns>deserialized object.</returns>
        public static object Deserialize(XmlSerializer ser, string xml_string, ref string error)
        {
            try
            {
                var bytes = Encoding.UTF8.GetBytes(xml_string);
                var mem = new MemoryStream(bytes);
                return ser.Deserialize(mem);
            }
            catch (Exception e)
            {
                error = "Error Deserialize object: " + e.Message;
                return null;
            }
        }

        /// <summary>
        /// Forward message logging if succeed or fail.
        /// </summary>
        /// <param name="send">message to send.</param>
        /// <param name="connection">communications connection.</param>
        /// <param name="message">message to log.</param>
        protected void ForwardMessage(SapientMessage msg, IConnection connection, string message)
        {
            if (connection.SendMessage(msg))
            {
                Log.Info(message + " Forwarded");
            }
            else
            {
                Log.Warn(message + " Forward failed");
            }
        }

        /// <summary>
        /// Forward message logging if succeed or fail.
        /// </summary>
        /// <param name="send">message to send.</param>
        /// <param name="server">communications server.</param>
        /// <param name="connection_id">communications client connection.</param>
        /// <param name="succeed">succeed message.</param>
        /// <param name="fail">fail message.</param>
        protected void ForwardMessage(SapientMessage msg, SapientServer server, uint connection_id, string succeed, string fail)
        {
            if (server.SendMessage(msg, connection_id))
            {
                Log.Info(succeed);
            }
            else
            {
                Log.Warn(fail);
            }
        }

        /// <summary>
        /// Processes the registration.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="database">The database.</param>
        /// <returns>A SapientMessageType.</returns>
        protected virtual SapientMessageType ProcessRegistration(SapientMessage message, Database database)
        {
            return SapientMessageType.Unsupported;
        }

        /// <summary>
        /// Processes the detection report.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="database">The database.</param>
        /// <returns>A SapientMessageType.</returns>
        protected virtual SapientMessageType ProcessDetectionReport(SapientMessage message, Database database)
        {
            return SapientMessageType.Unsupported;
        }

        /// <summary>
        /// Processes the status report.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="database">The database.</param>
        /// <returns>A SapientMessageType.</returns>
        protected virtual SapientMessageType ProcessStatusReport(SapientMessage message, Database database)
        {
            return SapientMessageType.Unsupported;
        }

        /// <summary>
        /// Processes the alert.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="database">The database.</param>
        /// <returns>A SapientMessageType.</returns>
        protected virtual SapientMessageType ProcessAlert(SapientMessage message, Database database)
        {
            return SapientMessageType.Unsupported;
        }

        /// <summary>
        /// Processes the sensor task.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="database">The database.</param>
        /// <returns>A SapientMessageType.</returns>
        protected virtual SapientMessageType ProcessSensorTask(SapientMessage message, Database database)
        {
            return SapientMessageType.Unsupported;
        }

        /// <summary>
        /// Processes the registration ack.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="database">The database.</param>
        /// <returns>A SapientMessageType.</returns>
        protected virtual SapientMessageType ProcessRegistrationAck(SapientMessage message, Database database)
        {
            return SapientMessageType.RegistrationACK;
        }

        /// <summary>
        /// Processes the sensor task ack.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="database">The database.</param>
        /// <returns>A SapientMessageType.</returns>
        protected virtual SapientMessageType ProcessSensorTaskAck(SapientMessage message, Database database)
        {
            return SapientMessageType.Unsupported;
        }

        /// <summary>
        /// Processes the alert response.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="database">The database.</param>
        /// <returns>A SapientMessageType.</returns>
        protected virtual SapientMessageType ProcessAlertResponse(SapientMessage message, Database database)
        {
            return SapientMessageType.Unsupported;
        }

        /// <summary>
        /// Processes the objective.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="database">The database.</param>
        /// <returns>A SapientMessageType.</returns>
        protected virtual SapientMessageType ProcessObjective(SapientMessage message, Database database)
        {
            return SapientMessageType.Unsupported;
        }

        /// <summary>
        /// Processes the route plan.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="database">The database.</param>
        /// <returns>A SapientMessageType.</returns>
        protected virtual SapientMessageType ProcessRoutePlan(SapientMessage message, Database database)
        {
            return SapientMessageType.Unsupported;
        }

        /// <summary>
        /// Processes the approval.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="database">The database.</param>
        /// <returns>A SapientMessageType.</returns>
        protected virtual SapientMessageType ProcessApproval(SapientMessage message, Database database)
        {
            return SapientMessageType.Unsupported;
        }

        /// <summary>
        /// Parses the error message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>A SapientMessageType.</returns>
        protected virtual SapientMessageType ParseErrorMessage(SapientMessage message)
        {
            Log.Warn("SAPIENT Error Message Received");
            return SapientMessageType.Error;
        }
    }
}
