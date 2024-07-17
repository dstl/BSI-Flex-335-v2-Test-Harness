// Project:           $Project: 00010855-Sapient$
// File:              $Workfile: SapientDataAgentClientProtocol.cs$
// Crown-owned copyright, 2021-2024
//  See Release/Supply Conditions
// $NoKeywords$

using Google.Protobuf;

namespace SapientMiddleware.Common
{
    using Google.Protobuf.WellKnownTypes;
    using log4net;
    using Sapient.Common;
    using Sapient.Data;
    using SapientDatabase;
    using SapientServices;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    /// <summary>
    /// class to handle the Sapient Data Data Agent Client Protocol.
    /// </summary>
    public class SapientDataAgentClientProtocol : SapientProtocol
    {
        /// <summary>
        /// Log4net logger.
        /// </summary>
        private static readonly new ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// sensor locations.
        /// </summary>
        protected Dictionary<string, Location> sensorLocations = new Dictionary<string, Location>();

        /// <summary>
        /// report ID of last detection received.
        /// </summary>
        protected string lastDetectionReportID;

        /// <summary>
        /// report ID of last status report received.
        /// </summary>
        protected string lastStatusReportID;

        /// <summary>
        /// time histogram of database latency.
        /// </summary>
        protected Histogram databaseLatencyHistogram;

        /// <summary>
        /// time histogram of communication latency.
        /// </summary>
        protected Histogram communicationLatencyHistogram;

        /// <summary>
        /// whether to forward ASM alerts to DMM.
        /// </summary>
        private bool forwardAsmAlerts;

        /// <summary>
        /// Fixed asm id specified in the config file.
        /// </summary>
        private string fixedAsmId;

        /// <summary>
        /// Initializes a new instance of the <see cref="SapientDataAgentClientProtocol" /> class.
        /// </summary>
        /// <param name="forwardASMalerts">whether to forward ASM alerts to DMM.</param>
        /// <param name="fixedASMId">fixed ASM ID from config file.</param>
        public SapientDataAgentClientProtocol(bool forwardASMalerts, string fixedASMId)
        {
            forwardAsmAlerts = forwardASMalerts;
            fixedAsmId = fixedASMId;
            databaseLatencyHistogram = new Histogram();
            communicationLatencyHistogram = new Histogram();
            Supported.Add(SapientMessageType.Registration);
            Supported.Add(SapientMessageType.Detection);
            Supported.Add(SapientMessageType.Status);
            Supported.Add(SapientMessageType.Alert);
            Supported.Add(SapientMessageType.TaskACK);
            Supported.Add(SapientMessageType.RoutePlan);
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
                    if (Program.TaskingCommsConnection != null)
                    {
                        ForwardMessage(TaskMessage, Program.TaskingCommsConnection, "Sensor Registration");
                    }
                    else
                    {
                        Log.Warn("DMM data agent not connected");
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
                        Log.Warn("DMM data agent not connected");
                    }

                    Program.MessageMonitor.IncrementMessageCount(SapientMessageType.TaskACK);
                    break;

                case SapientMessageType.Unknown:
                    {
                        Log.ErrorFormat("Unrecognised Message received from ASM: {0}", TaskMessage?.ContentCase);
                        var errorMessage = new SapientMessage
                        {
                            Timestamp = Timestamp.FromDateTime(DateTime.UtcNow),
                            NodeId = fixedAsmId,
                            Error = new Error
                            {
                                Packet = TaskMessage.ToByteString()
                            },
                        };
                        errorMessage.Error.ErrorMessage.Add(ErrorString);

                        ForwardMessage(errorMessage, Program.ClientComms, connection, "Unrecognised Message - Error returned: " + TaskMessage, "Unrecognised message - Error return failed: " + TaskMessage);

                        Program.MessageMonitor.IncrementMessageCount(SapientMessageType.Unknown);
                    }
                    break;

                case SapientMessageType.InvalidClient:
                    {
                        Log.ErrorFormat("Invalid Message received:{0}:{1}", this.ErrorString, TaskMessage?.ContentCase);
                        SapientMessage errorMessage = new SapientMessage
                        {
                            Timestamp = Timestamp.FromDateTime(DateTime.UtcNow),
                            NodeId = fixedAsmId,
                            Error = new Error
                            {
                                Packet = TaskMessage.ToByteString()
                            },
                        };
                        errorMessage.Error.ErrorMessage.Add(ErrorString);
                        ForwardMessage(errorMessage, Program.ClientComms, connection, "Invalid Message :" + this.ErrorString + ": Error returned: " + TaskMessage, "Invalid message - Error return failed: " + TaskMessage);

                        Program.MessageMonitor.IncrementMessageCount(SapientMessageType.InvalidClient);
                    }
                    break;

                case SapientMessageType.Detection:
                case SapientMessageType.Status:
                case SapientMessageType.RoutePlan:
                case SapientMessageType.Error:
                    Program.MessageMonitor.IncrementMessageCount(output);
                    if (Program.TaskingCommsConnection != null)
                    {
                        ForwardMessage(TaskMessage, Program.TaskingCommsConnection, output.ToString());
                    }
                    else
                    {
                        Log.Warn("DMM data agent not connected");
                    }
                    break;

                case SapientMessageType.Alert:
                    Program.MessageMonitor.IncrementMessageCount(SapientMessageType.Alert);
                    if (forwardAsmAlerts)
                    {
                        if (Program.TaskingCommsConnection != null)
                        {
                            ForwardMessage(TaskMessage, Program.TaskingCommsConnection, "Alert");
                        }
                        else
                        {
                            Log.Warn("DMM data agent not connected");
                        }
                    }

                    break;

                case SapientMessageType.IdError:
                    Log.ErrorFormat("Message with unregistered ASM ID received from ASM: {0}", ErrorString);
                    Program.MessageMonitor.IncrementMessageCount(SapientMessageType.IdError);
                    break;

                case SapientMessageType.InternalError:
                    Log.ErrorFormat("SDA Client Error:{0}", ErrorString);
                    Program.MessageMonitor.IncrementMessageCount(SapientMessageType.InternalError);
                    break;

                case SapientMessageType.Unsupported:
                    Log.ErrorFormat("Unsupported Message received from ASM: {0}", TaskMessage?.ContentCase);
                    Program.MessageMonitor.IncrementMessageCount(SapientMessageType.Unknown);
                    break;

                default:
                    Log.ErrorFormat("Unknown Middleware Response:{0}", output);
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
                string errorString;
                SapientMessageType retval = SapientMessageValidator.ParseRegistration(message, out errorString);
                ErrorString = errorString;

                if (retval == SapientMessageType.Registration)
                {
                    Program.MessageMonitor.SetStatusText(0, string.Join(",", message.Registration.NodeDefinition));
                    Program.Registration = message;
                    if (string.IsNullOrWhiteSpace(fixedAsmId))
                    {
                        // enforce fixed sensor ID by always registering with the same Identifier
                        message.NodeId = fixedAsmId;

                        TaskMessage = message;
                    }

                    // Log registered ASMs for recovery of dropped connections
                    LogRegisteredSensor(message.NodeId);
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
        /// Parses and processes a detection report message.
        /// </summary>
        /// <param name="message">The received message.</param>
        /// <param name="database">The database connection.</param>
        /// <returns>Parsed message type or failure mode for further action.</returns>
        protected override SapientMessageType ProcessDetectionReport(SapientMessage message, Database database)
        {
            Log.Info("ASM Detection Message Received:");
            try
            {
                string errorString;
                SapientMessageType retval = SapientMessageValidator.ParseDetection(message, out errorString);
                ErrorString = errorString;

                if (retval == SapientMessageType.Detection)
                {
                    var sourceId = message.NodeId;

                    var detectionReport = message.DetectionReport;

                    // If the sensor is not currently registered with us, check if it has been in
                    // the past and assume it is the same sensor having recovered from a network drop.
                    bool found = CurrentOrPreviousASM(sourceId);

                    if (!string.IsNullOrWhiteSpace(lastDetectionReportID) && !string.IsNullOrWhiteSpace(detectionReport.ReportId) && Ulid.Parse(lastDetectionReportID).Time > Ulid.Parse(detectionReport.ReportId).Time)
                    {
                        Log.InfoFormat("Non Consecutive Detection ReportID {0} {1}", sourceId, detectionReport.ReportId);
                    }

                    Log.InfoFormat("source:{0} det reportID:{1}", sourceId, detectionReport.ReportId);
                    lastDetectionReportID = detectionReport.ReportId;

                    if (found)
                    {
                        // Update last contact from sensor.
                        LogRegisteredSensor(sourceId);

                        // start stop watch for timing diagnostics.
                        Stopwatch stopWatch = new Stopwatch();
                        stopWatch.Start();

                        if (database != null)
                        {
                            if (detectionReport.Location != null)
                            {
                                detectionReport.Location = CartesianOffset(detectionReport.Location, message.NodeId, database);
                            }

                            if (detectionReport.RangeBearing != null)
                            {
                                detectionReport.RangeBearing = BearingOffset(detectionReport.RangeBearing, message.NodeId, database);
                            }
                        }

                        if (database != null)
                        {
                            // Write to standard database.
                            database.DbDetection(message, false);

                            if (AdditionalDatabase != null)
                            {
                                AdditionalDatabase.DbDetection(message, false);
                            }
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
                    else
                    {
                        ErrorString = string.Format("ASM ID: {0} not registered", sourceId);
                        retval = SapientMessageType.IdError;
                    }
                }

                return retval;
            }
            catch (Exception e)
            {
                ErrorString = "Internal:DetectionReport:" + e.Message;
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
            Log.Info("Status Report Message Received");

            try
            {
                string errorString;
                SapientMessageType retval = SapientMessageValidator.ParseStatusReport(message, out errorString);
                ErrorString = errorString;

                if (retval == SapientMessageType.Status)
                {
                    StatusReport statusReport = message.StatusReport;
                    var sourceId = message.NodeId;

                    // If the sensor is not currently registered with us, check if it has been in
                    // the past and assume it is the same sensor having recovered from a network drop.
                    bool found = CurrentOrPreviousASM(sourceId);

                    if (!string.IsNullOrWhiteSpace(lastStatusReportID) && !string.IsNullOrWhiteSpace(statusReport.ReportId) && Ulid.Parse(lastStatusReportID).Time > Ulid.Parse(statusReport.ReportId).Time)
                    {
                        Log.InfoFormat("Non Consecutive Status ReportID {0} {1}", sourceId, statusReport.ReportId);
                    }

                    Log.InfoFormat("source:{0} sta reportID:{1}", sourceId, statusReport.ReportId);
                    lastStatusReportID = statusReport.ReportId;

                    if (database != null)
                    {
                        if (statusReport.NodeLocation != null)
                        {
                            statusReport.NodeLocation = CartesianOffset(statusReport.NodeLocation, message.NodeId, database);
                        }

                        if (statusReport.FieldOfView != null)
                        {
                            if (statusReport.FieldOfView.RangeBearing != null)
                            {
                                statusReport.FieldOfView.RangeBearing = BearingOffset(statusReport.FieldOfView.RangeBearing, message.NodeId, database);
                            }

                            if (statusReport.FieldOfView.LocationList != null)
                            {
                                statusReport.FieldOfView.LocationList = CartesianOffset(statusReport.FieldOfView.LocationList, message.NodeId, database);
                            }
                        }
                    }

                    if (found)
                    {
                        // Update last contact from sensor.
                        LogRegisteredSensor(sourceId);

                        string sensorStatus = statusReport.System.ToString();
                        Program.MessageMonitor.SetStatusText(1, sensorStatus);

                        // start stop watch for timing diagnostics.
                        Stopwatch stopWatch = new Stopwatch();
                        stopWatch.Start();


                        if ((statusReport.FieldOfView != null) && (statusReport.FieldOfView.RangeBearing != null))
                        {
                            double azOffset = Properties.Settings.Default.AzimuthOffset;

                            double fixedRange = Properties.Settings.Default.FixedRange;

                            Log.Info(" StatusReport Az:" + statusReport.FieldOfView.RangeBearing.Azimuth.ToString() + " AzOff:" + azOffset.ToString());

                            if (azOffset != 0)
                            {
                                statusReport.FieldOfView.RangeBearing.Azimuth += azOffset;
                                Log.Info(" StatusReport Applied Azo:" + statusReport.FieldOfView.RangeBearing.Azimuth.ToString());
                            }

                            if (fixedRange > 0)
                            {
                                statusReport.FieldOfView.RangeBearing.Range = fixedRange;
                                Log.Info(" StatusReport Fixed Range:" + fixedRange.ToString());
                            }
                        }

                        // Update sensorLocation dictionary to hold sensor positions.
                        if (statusReport.NodeLocation != null)
                        {
                            sensorLocations[message.NodeId] = statusReport.NodeLocation;
                        }

                        if (database != null)
                        {
                            // Write data to database.
                            database.DbStatus(message);

                            if (AdditionalDatabase != null)
                            {
                                AdditionalDatabase.DbStatus(message);
                            }
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
                    else
                    {
                        ErrorString = string.Format("ASM ID: {0} not registered", sourceId);
                        retval = SapientMessageType.IdError;
                    }
                }

                return retval;
            }
            catch (Exception e)
            {
                ErrorString = "Internal:StatusReport:" + e.Message;
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
            Log.Info("Alert Message Received:");
            try
            {
                string errorString;
                SapientMessageType retval = SapientMessageValidator.ParseAlert(message, out errorString);
                ErrorString = errorString;

                if (retval == SapientMessageType.Alert)
                {
                    Alert alert = message.Alert;
                    var sourceId = message.NodeId;

                    // If the sensor is not currently registered with us
                    // check if it has been in the past and assume it is the same sensor having recovered from a network drop.
                    bool found = CurrentOrPreviousASM(sourceId);
                    if (alert.Location != null && database != null)
                    {
                        alert.Location = CartesianOffset(alert.Location, message.NodeId, database);
                    }

                    if (alert.RangeBearing != null && database != null)
                    {
                        alert.RangeBearing = BearingOffset(alert.RangeBearing, message.NodeId, database);
                    }

                    if (found)
                    {
                        // Update last contact from sensor.
                        LogRegisteredSensor(sourceId);

                        // start stop watch for timing diagnostics.
                        Stopwatch stopWatch = new Stopwatch();
                        stopWatch.Start();

                        if (database != null)
                        {
                            // write data to database.
                            database.DbAlert(message, false);
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
                    else
                    {
                        ErrorString = string.Format("ASM ID: {0} not registered", sourceId);
                        retval = SapientMessageType.IdError;
                    }
                }

                return retval;
            }
            catch (Exception e)
            {
                ErrorString = "Internal:Alert:" + e.Message;
                return SapientMessageType.InternalError;
            }
        }

        /// <summary>
        /// Parses and processes a sensor task ack message.
        /// </summary>
        /// <param name="message">The received message.</param>
        /// <param name="database">The database connection.</param>
        /// <returns>Parsed message type or failure mode for further action.</returns>
        protected override SapientMessageType ProcessSensorTaskAck(SapientMessage message, Database database)
        {
            Log.Info("Sensor Task Ack Message Received");
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
                        // write sensor_task_ack to database.
                        database.DbAcknowledgeTask(message, SapientDatabase.DatabaseTables.TaskConstants.SensorTaskAck.Table);
                    }

                    // measure database latency.
                    long databaseLatencyMilliseconds = stopWatch.ElapsedMilliseconds;
                    stopWatch.Stop();
                    Log.Info("taskAck Database Latency, " + databaseLatencyMilliseconds.ToString("D") + ", ms");
                }

                return retval;
            }
            catch (Exception e)
            {
                ErrorString = "Internal:SensorTaskACK:" + e.Message;
                return SapientMessageType.InternalError;
            }
        }

        /// <summary>
        /// Checks given id against a list of previously registered sensors to see if the current
        /// sensor has registered with this SDA within a time frame provided in the config.
        /// </summary>
        /// <param name="sensorId">The ID of an ASM to verify.</param>
        /// <returns>Has the sensor previously registered with this SDA within the configured time limit.</returns>
        private bool HasRegisteredPreviously(string sensorId)
        {
            bool found = false;

            if (Program.previouslyConnectedASMs.ContainsKey(sensorId))
            {
                if ((DateTime.Now - Program.previouslyConnectedASMs[sensorId]) < Properties.Settings.Default.AsmReconnectionTimeout)
                {
                    found = true;
                }
            }

            return found;
        }

        /// <summary>
        /// whether this is a current or previously registered ASM ID.
        /// </summary>
        /// <param name="source_id">ASM or sensor ID.</param>
        /// <returns>true if currently registered.</returns>
        private bool CurrentOrPreviousASM(string source_id)
        {
            bool found = false;
            lock (Program.ClientMessageParsers)
            {
                found = SensorIds.Contains(source_id);
            }

            // If the sensor is not currently registered with us, check if it has been in
            // the past and assume it is the same sensor having recovered from a network drop.
            if (!found)
            {
                found = HasRegisteredPreviously(source_id);
            }

            return found;
        }

        /// <summary>
        /// Adds (or updates) a sensor in a collection with the current DateTime. This is used for reconnecting ASMs which have dropped connection.
        /// </summary>
        /// <param name="sensorId">ASM or sensor ID.</param>
        protected void LogRegisteredSensor(string sensorId)
        {
            Program.previouslyConnectedASMs.AddOrUpdate(sensorId, DateTime.Now, (key, previousValue) => DateTime.Now);
        }

        /// <summary>
        /// Offset a location in cartesian coordinates.
        /// </summary>
        /// <param name="location">location object to apply offset to.</param>
        /// <param name="sensor_id">ASM or sensor ID.</param>
        /// <param name="database">offset database object.</param>
        /// <returns>location object with offset applied.</returns>
        private Location CartesianOffset(Location location, string sensor_id, Database database)
        {
            Dictionary<string, Dictionary<string, double>> locationOffset = database.GetSensorOffsetFromDB(sensor_id);
            if (locationOffset.ContainsKey(sensor_id))
            {
                location.X += locationOffset[sensor_id]["x_offset"];
                location.Y += locationOffset[sensor_id]["y_offset"];
                location.Z += locationOffset[sensor_id]["z_offset"];
            }

            return location;
        }

        /// <summary>
        /// offset a set of locations in cartesian.
        /// </summary>
        /// <param name="locations">list of locations to apply offset to.</param>
        /// <param name="sensor_id">ASM or sensor ID.</param>
        /// <param name="database">offset database object.</param>
        /// <returns>list of locations with offset applied.</returns>
        private LocationList CartesianOffset(LocationList locations, string sensor_id, Database database)
        {
            for (int i = 0; i < locations.Locations.Count; i++)
            {
                locations.Locations[i] = CartesianOffset(locations.Locations[i], sensor_id, database);
            }

            return locations;
        }

        /// <summary>
        /// offset a bearing location in azimuth and elevation angles.
        /// </summary>
        /// <param name="bearing">range bearing object to apply offset to.</param>
        /// <param name="sensor_id">sensor ID.</param>
        /// <param name="database">database object.</param>
        /// <returns>range bearing object with offset applied.</returns>
        private RangeBearing BearingOffset(RangeBearing bearing, string sensor_id, Database database)
        {
            Dictionary<string, Dictionary<string, double>> locationOffset = database.GetSensorOffsetFromDB(sensor_id);
            if (locationOffset.ContainsKey(sensor_id))
            {
                bearing.Azimuth += locationOffset[sensor_id]["az_offset"];
                bearing.Elevation += locationOffset[sensor_id]["ele_offset"];
            }

            return bearing;
        }

        /// <summary>
        /// offset a bearing cone location in azimuth and elevation angles.
        /// </summary>
        /// <param name="bearing">range bearing cone object to apply offset to.</param>
        /// <param name="sensor_id">sensor ID.</param>
        /// <param name="database">database object.</param>
        /// <returns>>range bearing cone object with offset applied.</returns>
        private RangeBearingCone BearingOffset(RangeBearingCone bearing, string sensor_id, Database database)
        {
            Dictionary<string, Dictionary<string, double>> locationOffset = database.GetSensorOffsetFromDB(sensor_id);
            if (locationOffset.ContainsKey(sensor_id))
            {
                bearing.Azimuth += locationOffset[sensor_id]["az_offset"];
                bearing.Elevation += locationOffset[sensor_id]["ele_offset"];
            }

            return bearing;
        }
    }
}
