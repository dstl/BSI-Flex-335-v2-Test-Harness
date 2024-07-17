// Project:           $Project: 00010855-Sapient$
// File:              $Workfile: SapientMessageValidator.cs$
// Crown-owned copyright, 2021-2024
//  See Release/Supply Conditions
// $NoKeywords$

namespace SapientMiddleware
{
    using System;
    using log4net;
    using Sapient.Data;
    using SapientServices;
    using SapientServices.Data.Validation;

    /// <summary>
    /// The SapientMessageValidator class.
    /// </summary>
    public class SapientMessageValidator
    {
        /// <summary>
        /// The log.
        /// </summary>
        private static ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Parse SAPIENT Detection Message and return success or otherwise.
        /// </summary>
        /// <param name="message">XML message string.</param>
        /// <param name="errorString">error message text.</param>
        /// <returns>Parsed message type or failure mode for further action.</returns>
        public static SapientMessageType ParseSapientMessage(SapientMessage message, out string errorString)
        {
            errorString = string.Empty;
            try
            {
                if (message.DetectionReport == null)
                {
                    errorString = "DetectionReport object should not be null";
                    return SapientMessageType.InvalidClient;
                }

                return SapientMessageType.Detection;
            }
            catch (Exception e)
            {
                Log.Error("Exception parsing Detection message", e);
                errorString = "Internal:DetectionReport:" + e.Message;
                return SapientMessageType.InternalError;
            }
        }

        /// <summary>
        /// Parse SAPIENT Registration Message and return success or otherwise.
        /// </summary>
        /// <param name="message">XML message string.</param>
        /// <param name="errorString">error message text.</param>
        /// <returns>Parsed message type or failure mode for further action.</returns>
        public static SapientMessageType ParseRegistration(SapientMessage message, out string errorString)
        {
            errorString = string.Empty;

            try
            {
                if (message.Registration == null)
                {
                    errorString = "Registration message missing";
                    return SapientMessageType.InvalidClient;
                }
                return SapientMessageType.Registration;
            }
            catch (Exception e)
            {
                errorString = "Internal:SensorRegistration:" + e.Message;
                return SapientMessageType.InternalError;
            }
        }

        /// <summary>
        /// Parse SAPIENT Detection Message and return success or otherwise.
        /// </summary>
        /// <param name="message">XML message string.</param>
        /// <param name="errorString">error message text.</param>
        /// <returns>Parsed message type or failure mode for further action.</returns>
        public static SapientMessageType ParseDetection(SapientMessage message, out string errorString)
        {
            errorString = string.Empty;
            try
            {
                if (message.DetectionReport == null)
                {
                    errorString = "DetectionReport object should not be null";
                    return SapientMessageType.InvalidClient;
                }
                return SapientMessageType.Detection;
            }
            catch (Exception e)
            {
                Log.Error("Exception parsing Detection message", e);
                errorString = "Internal:DetectionReport:" + e.Message;
                return SapientMessageType.InternalError;
            }
        }

        /// <summary>
        /// Parse SAPIENT Status Report Message and return success or otherwise.
        /// </summary>
        /// <param name="message">SapientMessage protobuf message.</param>
        /// <param name="errorString">error message text.</param>
        /// <returns>Parsed message type or failure mode for further action.</returns>
        public static SapientMessageType ParseStatusReport(SapientMessage message, out string errorString)
        {
            errorString = string.Empty;
            try
            {
                if (message.StatusReport == null)
                {
                    errorString = "StatusReport object should not be null";
                    return SapientMessageType.InvalidClient;
                }
                return SapientMessageType.Status;
            }
            catch (Exception e)
            {
                Log.Error("Exception parsing Status message", e);
                errorString = "Internal:StatusReport:" + e.Message;
                return SapientMessageType.InternalError;
            }
        }

        /// <summary>
        /// Parse SAPIENT Alert Message and return success or otherwise.
        /// </summary>
        /// <param name="message">XML message string.</param>
        /// <param name="errorString">error message text.</param>
        /// <returns>Parsed message type or failure mode for further action.</returns>
        public static SapientMessageType ParseAlert(SapientMessage message, out string errorString)
        {
            errorString = string.Empty;
            try
            {
                if (message.Alert == null)
                {
                    errorString = "Alert object should not be null";
                    return SapientMessageType.InvalidClient;
                }
                return SapientMessageType.Alert;
            }
            catch (Exception e)
            {
                Log.Error("Exception parsing Alert message", e);
                errorString = "Internal:Alert:" + e.Message;
                return SapientMessageType.InternalError;
            }
        }

        /// <summary>
        /// Parse SAPIENT SensorTask Message and return success or otherwise.
        /// </summary>
        /// <param name="message">XML message string.</param>
        /// <param name="task">parsed message object.</param>
        /// <param name="errorString">error message text.</param>
        /// <returns>Parsed message type or failure mode for further action.</returns>
        public static SapientMessageType ParseSensorTask(SapientMessage message, out string errorString)
        {
            errorString = string.Empty;
            try
            {
                if (message.Task == null)
                {
                    errorString = "Task object should not be null";
                    return SapientMessageType.InvalidTasking;
                }
                return SapientMessageType.Task;
            }
            catch (Exception e)
            {
                Log.Error("Exception parsing SensorTask message", e);
                errorString = "Internal:SensorTask:" + e.Message;
                return SapientMessageType.InternalError;
            }
        }

        /// <summary>
        /// Parse SAPIENT Task acknowledgement Message and return success or otherwise.
        /// </summary>
        /// <param name="message">XML message string.</param>
        /// <param name="sensor_task_ack">parsed message object.</param>
        /// <param name="errorString">error message text.</param>
        /// <returns>Parsed message type or failure mode for further action.</returns>
        public static SapientMessageType ParseSensorTaskAck(SapientMessage message, out string errorString)
        {
            errorString = string.Empty;
            try
            {
                if (message.TaskAck == null)
                {
                    errorString = "TaskAck object should not be null";
                    return SapientMessageType.InvalidClient;
                }
                return SapientMessageType.TaskACK;
            }
            catch (Exception e)
            {
                Log.Error("Exception parsing SensorTaskACK message", e);
                errorString = "Internal:SensorTaskACK:" + e.Message;
                return SapientMessageType.InternalError;
            }
        }

        /// <summary>
        /// Parse SAPIENT Alert Response Message and return success or otherwise.
        /// </summary>
        /// <param name="message">XML message string.</param>
        /// <param name="alertResponse">parsed message object.</param>
        /// <param name="errorString">error message text.</param>
        /// <returns>Parsed message type or failure mode for further action.</returns>
        public static SapientMessageType ParseAlertResponse(SapientMessage message, out string errorString)
        {
            errorString = string.Empty;
            try
            {
                if (message.AlertAck == null)
                {
                    errorString = "AlertAck object should not be null";
                    return SapientMessageType.InvalidClient;
                }
                return SapientMessageType.AlertResponse;
            }
            catch (Exception e)
            {
                Log.Error("Exception parsing AlertResponse message", e);
                errorString = "Internal:AlertResponse: " + e.Message;
                return SapientMessageType.InternalError;
            }
        }
    }
}
