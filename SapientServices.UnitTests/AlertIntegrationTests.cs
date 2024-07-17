// Crown-owned copyright, 2021-2024
using Google.Protobuf.WellKnownTypes;
using Newtonsoft.Json;
using NUnit.Framework;
using Sapient.Common;
using Sapient.Data;
using SapientServices.Data.Validation;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;

namespace SapientServices.Tests
{
    /// <summary>
    /// The alert integration tests.
    /// </summary>
    public class AlertIntegrationTests : BaseTests
    {
        private readonly AlertValidator _sut = new AlertValidator();

        /// <summary>
        /// Validate_S the only mandatory fields set_ ok.
        /// </summary>
        [Test]
        public void Validate_OnlyMandatoryFieldsSet_Ok()
        {
            // Arrange
            var testAlert = GetSimpleAlert();
            
            // Act
            var result = _sut.Validate(testAlert);

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
            var testAlert = GetFullAlert(isPoint: true);

            // Act
            var result = _sut.Validate(testAlert);

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
            var testAlert = GetFullAlert(isPoint: false);

            // Act
            var result = _sut.Validate(testAlert);

            // Assert
            Assert.IsTrue(result.IsValid);
        }

        /// <summary>
        /// Validate_S the full alert message storing to database_ integration_ test.
        /// </summary>
        [Test]
        public void Validate_FullAlertMessageStoringToDatabase_Integration_Test()
        {
            var requiredFieldsMessage = GetSapientMessage(GetSimpleAlert());
            var allFieldsPointMessage = GetSapientMessage(GetFullAlert(isPoint: true));
            var allFieldsStrobeMessage = GetSapientMessage(GetFullAlert(isPoint: false));

            // Only Required fields SDA
            sapientDatabase.DbAlert(requiredFieldsMessage, false);
            // Only Required fields HDA
            sapientDatabase.DbAlert(requiredFieldsMessage, true);

            // All Fields with Location SDA
            sapientDatabase.DbAlert(allFieldsPointMessage, false);
            // All Fields with Location HDA
            sapientDatabase.DbAlert(allFieldsPointMessage, true);

            // All Fields with RangeBearing SDA
            sapientDatabase.DbAlert(allFieldsStrobeMessage, false);
            // All Fields with RangeBearing SDA
            sapientDatabase.DbAlert(allFieldsStrobeMessage, true);

            var alertAck = GetAlertAck();
            sapientDatabase.DbAcknowledgeAlert(alertAck);
        }

        // Enable it when test data files required
        // [Test]
        public void Write_Json()
        {
            string folderPath = @"C:\Work\Sapient\Sapient_V3_Test_Data";
            string nodeId = "284436D8-E0B7-4E9F-B5E0-E3C347C2CA90";
            
            var requiredFieldsMessage = GetSapientMessage(GetSimpleAlert());
            var allFieldsPointMessage = GetSapientMessage(GetFullAlert(isPoint: true));
            var allFieldsStrobeMessage = GetSapientMessage(GetFullAlert(isPoint: false));

            requiredFieldsMessage.NodeId = nodeId;
            allFieldsPointMessage.NodeId = nodeId;
            allFieldsStrobeMessage.NodeId = nodeId;

            CreateFile(Path.Combine(folderPath, "alert_basic.json"), requiredFieldsMessage.ToString());
            CreateFile(Path.Combine(folderPath, "alert_full_with_point.json"), allFieldsPointMessage.ToString());
            CreateFile(Path.Combine(folderPath, "alert_full_with_strobe.json"), allFieldsStrobeMessage.ToString());
        }

        private void CreateFile(string filePath, string json)
        {
            File.WriteAllText(filePath, json);
        }

        /// <summary>
        /// Gets the sapient message.
        /// </summary>
        /// <param name="alert">The alert.</param>
        /// <returns>A SapientMessage.</returns>
        private SapientMessage GetSapientMessage(Alert alert) =>
            new SapientMessage
            {
                Timestamp = Timestamp.FromDateTime(DateTime.UtcNow),
                NodeId = Guid.NewGuid().ToString(),
                Alert = alert,
            };

        /// <summary>
        /// Gets the simple alert.
        /// </summary>
        /// <returns>An Alert.</returns>
        private Alert GetSimpleAlert() =>
           new Alert
           {
               AlertId = Ulid.NewUlid().ToString(),
           };

        /// <summary>
        /// Gets the full alert.
        /// </summary>
        /// <param name="isPoint">If true, is point.</param>
        /// <returns>An Alert.</returns>
        private Alert GetFullAlert(bool isPoint = true)
        {
            var associatedFile = new Google.Protobuf.Collections.RepeatedField<AssociatedFile>
            {
                DefaultAssociatedFile,
            };

            var associatedDetection = new Google.Protobuf.Collections.RepeatedField<AssociatedDetection>
            {
                DefaultAssociatedDetection,
            };

            var alert = new Alert
            {
                AlertId = Ulid.NewUlid().ToString(),
                AlertType = Alert.Types.AlertType.Information, 
                Status = Alert.Types.AlertStatus.Active,
                Description = "Information Alert",
                RegionId = Ulid.NewUlid().ToString(),
                Priority = Alert.Types.DiscretePriority.Medium,
                Ranking = 0.70f,
                Confidence = 0.80f,
            };

            alert.AssociatedFile.Add(associatedFile);
            alert.AssociatedDetection.Add(associatedDetection);

            if (isPoint)
            {
                alert.Position = new Alert.Types.LocationOrRangeBearing
                {
                    Location = DefaultLocation,
                };
            }
            else
            {
                alert.Position = new Alert.Types.LocationOrRangeBearing
                {
                    RangeBearing = DefaultRangeBearing,
                };
            }

            return alert;
        }

        /// <summary>
        /// Gets the alert ack.
        /// </summary>
        /// <returns>A SapientMessage.</returns>
        private SapientMessage GetAlertAck()
        {
            return new SapientMessage
            {
                NodeId = Guid.NewGuid().ToString(),
                Timestamp = Timestamp.FromDateTime(DateTime.UtcNow),
                AlertAck = new AlertAck
                {
                    AlertId = Ulid.NewUlid().ToString(),
                    DestinationId = Guid.NewGuid().ToString(),
                    Status = "Accepted",
                    Reason = AlertAck.Types.Reason.Accepted,
                },
            };
        }
    }
}