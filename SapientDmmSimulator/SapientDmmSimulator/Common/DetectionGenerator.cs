// Project:           $Project: 00010855-Sapient$
// File:              $Workfile: DetectionGenerator.cs$
// Crown-owned copyright, 2021-2024
//  See Release/Supply Conditions

using Google.Protobuf.WellKnownTypes;
using log4net;
using Sapient.Common;
using Sapient.Data;
using SAPIENT.ReadSampleMessage;
using SapientServices.Communication;

namespace SapientDmmSimulator.Common
{
    /// <summary>
    /// Generate Detection messages.
    /// </summary>
    public class DetectionGenerator : BaseGenerators
    {
        /// <summary>
        /// Log4net logger.
        /// </summary>
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Default range value in meters.
        /// </summary>
        protected static double startRange = 5;

        /// <summary>
        /// Default azimuth value in degrees.
        /// </summary>
        protected static double startAzimuth = 50;

        /// <summary>
        /// Maximum range to use for looped messages.
        /// </summary>
        protected static double maxRange = 100;

        /// <summary>
        /// Maximum latitude or Y coordinate to use for looped messages.
        /// </summary>
        protected double maxLoopLatitude = Properties.Settings.Default.maxLatitude;

        /// <summary>
        /// Longitude or X coordinate to use in detection location.
        /// </summary>
        protected double longitude = start_longitude;

        /// <summary>
        /// Latitude or Y coordinate to use in detection location.
        /// </summary>
        protected double latitude = start_latitude;

        /// <summary>
        /// Range to use if providing spherical location.
        /// </summary>
        protected double range = startRange;

        /// <summary>
        /// Azimuth to use if providing spherical location.
        /// </summary>
        protected double azimuth = startAzimuth;

        /// <summary>
        /// Object ID to use in the detection message.
        /// </summary>
        protected int detectionObjectId = 1;

        /// <summary>
        /// Gets or sets a value indicating whether to change object Id after each message.
        /// </summary>
        public bool ChangeObjectID { get; set; }

        /// <summary>
        /// Gets or sets an Image URL to provide with the detection message.
        /// </summary>
        public string ImageURL { get; set; }

        public void GenerateHLDetections(object comms_connection, TaskForm form)
        {
            var messenger = (IConnection)comms_connection;

            Log.Info("Create Detection Report.");
            ReadSampleMessage readSampleMessage = new ReadSampleMessage();
            string error = string.Empty;

            SapientMessage? message = readSampleMessage.ReadSampleMessageFromFile(SapientMessage.ContentOneofCase.DetectionReport, string.Empty, out error);
            if (message != null)
            {
                message.NodeId = Properties.Settings.Default.FixedDMMId;
                message.Timestamp = Timestamp.FromDateTime(DateTime.Now.ToUniversalTime());
                if (message.DetectionReport != null)
                {
                    message.DetectionReport.TaskId = Ulid.NewUlid().ToString();
                }
            }
            else
            {
                Log.Error(error);
            }

            do
            {
                if ((message != null) && (message.DetectionReport != null))
                {
                    if (!string.IsNullOrEmpty(ImageURL))
                    {
                        message.DetectionReport.AssociatedFile.AddRange(new Google.Protobuf.Collections.RepeatedField<AssociatedFile>
                    {
                        new AssociatedFile
                        {
                            Type = "image",
                            Url = ImageURL,
                        },
                    });
                    }

                    bool retval = MessageSender.Send(messenger, message);

                    if (retval)
                    {
                        MessageCount++;
                        Log.ErrorFormat("Send Detection Success {0}", message.DetectionReport.ReportId);
                    }
                    else
                    {
                        Log.ErrorFormat("Send Detection Failed {0}", message.DetectionReport.ReportId);
                    }

                    if (LoopMessages)
                    {
                        Thread.Sleep(LoopTime);
                    }

                }
            }
            while (LoopMessages);
        }

        /// <summary>
        /// send out detection messages, will loop until latitude is greater than 52.69
        /// there is a wait of 100ms between each send.
        /// </summary>
        /// <param name="comms_connection">IConnection messenger object used to send messages over.</param>
        /// <param name="form">main windows form.</param>
        public void GenerateDetections(object comms_connection) // , ClientForm form)
        {
            var messenger = (IConnection)comms_connection;
            var sub = new DetectionReport.Types.SubClass
            {
                Level = 1,
                Type = "Vehicle Class",
                Confidence = 0.7f,
            };

            var subClassSub = new DetectionReport.Types.SubClass
            {
                Level = 2,
                Type = "Size",
                Confidence = 0.6f,
            };

            subClassSub.SubClass_.AddRange(new Google.Protobuf.Collections.RepeatedField<DetectionReport.Types.SubClass>
            {
                new DetectionReport.Types.SubClass { Level = 3, Type = "Vehicle Type", },
                new DetectionReport.Types.SubClass { Level = 3, Type = "Vehicle Type", },
            });

            sub.SubClass_.AddRange(new Google.Protobuf.Collections.RepeatedField<DetectionReport.Types.SubClass>
            {
                subClassSub,
            });

            var detection = new DetectionReport
            {
                ReportId = Ulid.NewUlid().ToString(),
                ObjectId = Ulid.NewUlid().ToString(),
                TaskId = Ulid.NewUlid().ToString(),
                State = "lostInView",
                DetectionConfidence = 1.0f,
                Colour = "red",
            };

            detection.TrackInfo.AddRange(new Google.Protobuf.Collections.RepeatedField<DetectionReport.Types.TrackObjectInfo>
            {
                new DetectionReport.Types.TrackObjectInfo { Type = "confidence", Value = "0.9", Error = 0.01f, },
                new DetectionReport.Types.TrackObjectInfo { Type = "speed", Value = "2.0" },
            });

            detection.ObjectInfo.AddRange(new Google.Protobuf.Collections.RepeatedField<DetectionReport.Types.TrackObjectInfo>
            {
                new DetectionReport.Types.TrackObjectInfo { Type = "height", Value = "1.8", Error = 0.1f },
                new DetectionReport.Types.TrackObjectInfo { Type = "majorLength", Value = "1.2" },
            });

            var detectionRepot = new DetectionReport.Types.DetectionReportClassification { Type = "Vehicle", Confidence = 0.1f, };
            detectionRepot.SubClass.AddRange(new Google.Protobuf.Collections.RepeatedField<DetectionReport.Types.SubClass> { sub });

            detection.Classification.AddRange(new Google.Protobuf.Collections.RepeatedField<DetectionReport.Types.DetectionReportClassification>
            {
                new DetectionReport.Types.DetectionReportClassification { Type = "Human", Confidence = 0.9f, },
                detectionRepot,
            });

            detection.Behaviour.AddRange(new Google.Protobuf.Collections.RepeatedField<DetectionReport.Types.Behaviour>
            {
                new DetectionReport.Types.Behaviour { Type = "Walking", Confidence = 0.6f, },
                new DetectionReport.Types.Behaviour { Type = "Running", Confidence = 0.4f, },
            });

            if (!string.IsNullOrEmpty(ImageURL))
            {
                detection.AssociatedFile.AddRange(new Google.Protobuf.Collections.RepeatedField<AssociatedFile>
                {
                    new AssociatedFile
                    {
                        Type = "image",
                        Url = ImageURL,
                    },
                });
            }

            do
            {
                detection.ReportId = Ulid.NewUlid().ToString();
                detection.ObjectId = Ulid.NewUlid().ToString();

                detection.Location = new Location { X = longitude, Y = latitude, CoordinateSystem = LocationCoordinateSystem.LatLngDegM };

                if (isUTM)
                {
                    longitude += 1.0;
                    latitude += 1.0;
                }
                else
                {
                    longitude += 0.00001;
                    latitude += 0.00001;
                }

                if (latitude > maxLoopLatitude)
                {
                    latitude = start_latitude;
                    longitude = start_longitude;
                    detectionObjectId++;
                }
                else if (range > maxRange)
                {
                    range = startRange;
                    detectionObjectId++;
                }
                else if (ChangeObjectID)
                {
                    detectionObjectId++;
                }

                SapientMessage message = new SapientMessage
                {
                    Timestamp = Timestamp.FromDateTime(DateTime.UtcNow),
                    NodeId = Properties.Settings.Default.FixedDMMId,
                    DetectionReport = detection,
                };

                bool retval = MessageSender.Send(messenger, message);

                if (retval)
                {
                    MessageCount++;
                }
                else
                {
                    Log.ErrorFormat("Send Detection Failed {0}", detection.ReportId);
                }

                if (LoopMessages)
                {
                    Thread.Sleep(LoopTime);
                }
            }
            while (LoopMessages);
        }
    }
}
