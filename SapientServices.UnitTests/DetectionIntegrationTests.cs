// Crown-owned copyright, 2021-2024
using Google.Protobuf.WellKnownTypes;
using NUnit.Framework;
using Sapient.Common;
using Sapient.Data;
using SapientServices.Data.Validation;
using System.Text.Json.Serialization;
using System.Text.Json;
using static Sapient.Data.DetectionReport.Types;

namespace SapientServices.Tests
{
    /// <summary>
    /// The detection integration tests.
    /// </summary>
    public class DetectionIntegrationTests : BaseTests
    {
        private readonly DetectionValidator _sut = new DetectionValidator();

        /// <summary>
        /// Validate_S the only mandatory fields set_ ok.
        /// </summary>
        [Test]
        public void Validate_OnlyMandatoryFieldsSet_Ok()
        {
            // Arrange
            var testDetection = GetSimpleDetection();

            // Act
            var result = _sut.Validate(testDetection);

            // Assert
            Assert.IsTrue(result.IsValid);
        }

        /// <summary>
        /// Validate_S the full message with point set_ ok.
        /// </summary>
        [Test]
        public void Validate_FullMessageWithPointSet_Ok()
        {
            // Arrange
            var testDetection = GetFullDetection(isPoint: true);

            // Act
            var result = _sut.Validate(testDetection);

            // Assert
            Assert.IsTrue(result.IsValid);
        }

        /// <summary>
        /// Validate_S the full message with strobe set_ ok.
        /// </summary>
        [Test]
        public void Validate_FullMessageWithStrobeSet_Ok()
        {
            // Arrange
            var testDetection = GetFullDetection(isPoint: false);

            // Act
            var result = _sut.Validate(testDetection);

            // Assert
            Assert.IsTrue(result.IsValid);
        }

        /// <summary>
        /// Validate_S the full detection message storing to database_ integration_ test.
        /// </summary>
        [Test]
        public void Validate_FullDetectionMessageStoringToDatabase_Integration_Test()
        {
            var requiredFieldsPointMessage = GetSapientMessage(GetSimpleDetection(isPoint: true));
            var requiredFieldsStrobeMessage = GetSapientMessage(GetSimpleDetection(isPoint: false));
            var allFieldsPointMessage = GetSapientMessage(GetFullDetection(isPoint: true));
            var allFieldsStrobeMessage = GetSapientMessage(GetFullDetection(isPoint: false));

            // Only Required fields Point SDA
            sapientDatabase.DbDetection(requiredFieldsPointMessage, false);
            // Only Required fields Point HDA
            sapientDatabase.DbDetection(requiredFieldsPointMessage, true);

            // Only Required fields Strobe SDA
            sapientDatabase.DbDetection(requiredFieldsStrobeMessage, false);
            // Only Required fields Strobe HDA
            sapientDatabase.DbDetection(requiredFieldsStrobeMessage, true);

            // All Fields with Location SDA
            sapientDatabase.DbDetection(allFieldsPointMessage, false);
            // All Fields with Location HDA
            sapientDatabase.DbDetection(allFieldsPointMessage, true);

            // All Fields with RangeBearing SDA
            sapientDatabase.DbDetection(allFieldsStrobeMessage, false);
            // All Fields with RangeBearing SDA
            sapientDatabase.DbDetection(allFieldsStrobeMessage, true);
        }

        // Enable it when test data files required
        // [Test]
        public void Write_Json()
        {
            string folderPath = @"C:\Work\Sapient\Sapient_V3_Test_Data";
            string nodeId = "284436D8-E0B7-4E9F-B5E0-E3C347C2CA90";
            
            var requiredFieldsPointMessage = GetSapientMessage(GetSimpleDetection(isPoint: true));
            var requiredFieldsStrobeMessage = GetSapientMessage(GetSimpleDetection(isPoint: false));
            var allFieldsPointMessage = GetSapientMessage(GetFullDetection(isPoint: true));
            var allFieldsStrobeMessage = GetSapientMessage(GetFullDetection(isPoint: false));
            
            requiredFieldsPointMessage.NodeId = nodeId;
            requiredFieldsStrobeMessage.NodeId = nodeId;
            allFieldsPointMessage.NodeId = nodeId;
            allFieldsStrobeMessage.NodeId = nodeId;

            CreateFile(Path.Combine(folderPath, "detection_basic_with_point.json"), requiredFieldsPointMessage.ToString());
            CreateFile(Path.Combine(folderPath, "detection_basic_with_strobe.json"), requiredFieldsStrobeMessage.ToString());
            CreateFile(Path.Combine(folderPath, "detection_full_with_point.json"), allFieldsPointMessage.ToString());
            CreateFile(Path.Combine(folderPath, "detection_full_with_strobe.json"), allFieldsStrobeMessage.ToString());
        }

        private void CreateFile(string filePath, string json)
        {
            File.WriteAllText(filePath, json);
        }


        /// <summary>
        /// Gets the sapient message.
        /// </summary>
        /// <param name="detection">The detection.</param>
        /// <returns>A SapientMessage.</returns>
        private SapientMessage GetSapientMessage(DetectionReport detection) =>
            new SapientMessage
            {
                Timestamp = Timestamp.FromDateTime(DateTime.UtcNow),
                NodeId = Guid.NewGuid().ToString(),
                DetectionReport = detection,
            };

        /// <summary>
        /// Gets the simple detection.
        /// </summary>
        /// <param name="isPoint">If true, is point.</param>
        /// <returns>A DetectionReport.</returns>
        private DetectionReport GetSimpleDetection(bool isPoint = true)
        {
            var detection = new DetectionReport
            {
                ReportId = Ulid.NewUlid().ToString(),
                ObjectId = Ulid.NewUlid().ToString(),
                TaskId = Ulid.NewUlid().ToString(),
            };

            if (isPoint)
            {
                detection.Location = DefaultLocation;
            }
            else
            {
                detection.RangeBearing = DefaultRangeBearing;
            }

            return detection;
        }

        /// <summary>
        /// Gets the full detection.
        /// </summary>
        /// <param name="isPoint">If true, is point.</param>
        /// <returns>A DetectionReport.</returns>
        private DetectionReport GetFullDetection(bool isPoint = true)
        {
            var detection = GetSimpleDetection(isPoint);

            var trackObjectInfo = new TrackObjectInfo
            {
                Type = "colour",
                Value = "red",
                Error = 0.10f,
            };

            var predictionLocation = isPoint ? 
                new PredictedLocation
                {
                    Location = DefaultLocation,
                    PredictedTimestamp = Timestamp.FromDateTime(DateTime.UtcNow),
                } : new PredictedLocation
                {
                    RangeBearing = DefaultRangeBearing,
                    PredictedTimestamp = Timestamp.FromDateTime(DateTime.UtcNow),
                };

            Class reportClass = new Class
            {
                Type = "Animal",
                Confidence = 0.7f
            };

            reportClass.SubClass.Add(new SubClass
            {
                Type = "Bird",
                Confidence = 0.7f
            });

            detection.State = "Object Lost";
            detection.DetectionConfidence = 0.80f;
            detection.TrackInfo.Add(trackObjectInfo);
            detection.PredictionLocation = predictionLocation;
            detection.ObjectInfo.Add(trackObjectInfo);
            detection.Colour = "red";
            detection.Id = "112211";
            detection.Classification.Add(reportClass);
            detection.Behaviour.Add(new Behaviour
            {
                Type = "Crawling",
                Confidence = 0.30f,
            });
            detection.AssociatedFile.Add(DefaultAssociatedFile);
            detection.Signal.Add(new Signal
            {
                Amplitude = 1.0f,
                StartFrequency = 10.00f,
                CentreFrequency = 10.00f,
                StopFrequency = 10.00f,
                PulseDuration = 10.00f,
            });

            detection.AssociatedDetection.Add(new AssociatedDetection
            {
                Timestamp = Timestamp.FromDateTime(DateTime.UtcNow),
                NodeId = Ulid.NewUlid().ToString(),
                ObjectId = Ulid.NewUlid().ToString(),
                AssociationType = AssociationRelation.Sibling,
                Location = DefaultLocation,
            });

            detection.DerivedDetection.Add(new DerivedDetection
            {
                Timestamp = Timestamp.FromDateTime(DateTime.UtcNow),
                NodeId = Ulid.NewUlid().ToString(), 
                ObjectId = Ulid.NewUlid().ToString(),
            });

            if (isPoint)
            {
                detection.LocationVelocity = new Sapient.Common.LocationVelocity
                {
                    X = 20.00,
                    Y = 20.00,
                    Z = 100.00,
                    XError = 30.00,
                    YError = 20.00,
                    ZError = 30.00,
                    Units = Sapient.Common.LocationVelocityUnits.Kmh,
                };
            }
            else
            {
                detection.RangeBearingVelocity = new Sapient.Common.RangeBearingVelocity
                {
                    Azimuth = 100.00,
                    Elevation = 20.00,
                    Range = 30.00,
                    AngularUnits = Sapient.Common.RangeBearingVelocityUnits.Ds,
                    AzimuthError = 2.00,
                    ElevationError = 1.00,
                    RangeError = 2.00,
                };
            }

            return detection;
        }
    }
}
