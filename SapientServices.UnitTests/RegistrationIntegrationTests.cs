// Crown-owned copyright, 2021-2024
using Google.Protobuf.WellKnownTypes;
using NUnit.Framework;
using Sapient.Data;
using SapientServices.Data.Validation;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace SapientServices.Tests
{
    /// <summary>
    /// The registration integration tests.
    /// </summary>
    public class RegistrationIntegrationTests: BaseTests
    {
        private readonly RegistrationValidator _sut = new();

        /// <summary>
        /// Validate_S the only mandatory fields set_ ok.
        /// </summary>
        [Test]
        public void Validate_OnlyMandatoryFieldsSet_Ok()
        {
            // Arrange
            var testMsg = GetSimpleRegistration(isPoint: true);

            // Act
            var result = _sut.Validate(testMsg.Registration);

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
            var testMsg = GetFullRegistration(isPoint: true);

            // Act
            var result = _sut.Validate(testMsg.Registration);

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
            var testMsg = GetFullRegistration(isPoint: false);

            // Act
            var result = _sut.Validate(testMsg.Registration);

            // Assert
            Assert.IsTrue(result.IsValid);
        }

        /// <summary>
        /// Validate_S the full task message storing to database_ integration_ test.
        /// </summary>
        [Test]
        public void Validate_FullTaskMessageStoringToDatabase_Integration_Test()
        {
            var requiredFieldsMessage = GetSimpleRegistration(isPoint: true);
            var allFieldsPointMessage = GetFullRegistration(isPoint: true);
            var allFieldsStrobeMessage = GetFullRegistration(isPoint: false);

            // Only Required fields SDA
            sapientDatabase.DbRegistration(requiredFieldsMessage);

            // All Fields with Location SDA
            sapientDatabase.DbRegistration(allFieldsPointMessage);

            // All Fields with RangeBearing SDA
            sapientDatabase.DbRegistration(allFieldsStrobeMessage);
        }

        // Enable it when test data files required
        // [Test]
        public void Write_Json()
        {
            string folderPath = @"C:\Work\Sapient\Sapient_V3_Test_Data";
            string nodeId = "284436D8-E0B7-4E9F-B5E0-E3C347C2CA90";

            var requiredFieldsPointMessage = GetSimpleRegistration(isPoint: true);
            var requiredFieldsStrobeMessage = GetSimpleRegistration(isPoint: false);
            var allFieldsPointMessage = GetFullRegistration(isPoint: true);
            var allFieldsStrobeMessage = GetFullRegistration(isPoint: false);

            requiredFieldsPointMessage.NodeId = nodeId;
            requiredFieldsStrobeMessage.NodeId = nodeId;
            allFieldsPointMessage.NodeId = nodeId;
            allFieldsStrobeMessage.NodeId = nodeId;

            CreateFile(Path.Combine(folderPath, "01_registration_basic_with_point.json"), requiredFieldsPointMessage.ToString());
            CreateFile(Path.Combine(folderPath, "01_registration_basic_with_strobe.json"), requiredFieldsStrobeMessage.ToString());
            CreateFile(Path.Combine(folderPath, "01_registration_full_with_point.json"), allFieldsPointMessage.ToString());
            CreateFile(Path.Combine(folderPath, "01_registration_full_with_strobe.json"), allFieldsStrobeMessage.ToString());
        }

        private void CreateFile(string filePath, string json)
        {
            File.WriteAllText(filePath, json);
        }

        /// <summary>
        /// Gets the simple registration.
        /// </summary>
        /// <param name="isPoint">If true, is point.</param>
        /// <returns>A SapientMessage.</returns>
        private SapientMessage GetSimpleRegistration(bool isPoint)
        {
            var registration = new Registration
            {
                HeartbeatDefinition = new Registration.Types.HeartbeatDefinition
                {
                    HeartbeatInterval = DefaultDuration,
                },
            };

            registration.NodeType.Add(Registration.Types.NodeType.Radar);

            registration.HeartbeatDefinition.HeartbeatReport.Add(new Registration.Types.HeartbeatReport
            {
                Category = Registration.Types.HeartbeatReportCategory.Sensor,
                Type = "sensorLocation",
            });

            var tasks = new Google.Protobuf.Collections.RepeatedField<Registration.Types.TaskDefinition>
            {
                new Registration.Types.TaskDefinition
                {
                    Type = "Request",
                    RegionDefinition = new Registration.Types.RegionDefinition
                    {
                       SettleTime  = DefaultDuration,
                    },
                }
            };

            registration.ModeDefinition.Add(new Registration.Types.ModeDefinition
            {
                ModeName = "Follow",
                SettleTime = DefaultDuration,
                DetectionDefinition = new(),
                ModeType = Registration.Types.ModeType.Permanent,
            });

            registration.ModeDefinition[0].Task.AddRange(tasks);

            if (isPoint)
            {
                registration.HeartbeatDefinition.LocationDefinition = DefaultLocationTypePoint;
                registration.ModeDefinition[0].DetectionDefinition.LocationType = DefaultLocationTypePoint;
                registration.ModeDefinition[0].Task[0].RegionDefinition.RegionArea.Add(DefaultLocationTypePoint);
            }
            else
            {
                registration.HeartbeatDefinition.LocationDefinition = DefaultLocationTypeStrobe;
                registration.ModeDefinition[0].DetectionDefinition.LocationType = DefaultLocationTypeStrobe;
                registration.ModeDefinition[0].Task[0].RegionDefinition.RegionArea.Add(DefaultLocationTypeStrobe);
            }

            return new SapientMessage
            {
                Timestamp = Timestamp.FromDateTime(DateTime.UtcNow),
                NodeId = Guid.NewGuid().ToString(),
                Registration = registration,
            };
        }

        /// <summary>
        /// Gets the full registration.
        /// </summary>
        /// <param name="isPoint">If true, is point.</param>
        /// <returns>A SapientMessage.</returns>
        private SapientMessage GetFullRegistration(bool isPoint)
        {
            var msg = GetSimpleRegistration(isPoint);

            return msg;
        }
    }
}
