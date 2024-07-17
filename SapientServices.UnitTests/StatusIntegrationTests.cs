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
    /// The status integration tests.
    /// </summary>
    public class StatusIntegrationTests : BaseTests
    {
        private readonly StatusReportValidator _sut = new();

        /// <summary>
        /// Validate_S the only mandatory fields set_ ok.
        /// </summary>
        [Test]
        public void Validate_OnlyMandatoryFieldsSet_Ok()
        {
            // Arrange
            var testMsg = GetSimpleStatus();

            // Act
            var result = _sut.Validate(testMsg.StatusReport);

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
            var testMsg = GetFullStatus(isPoint: true);

            // Act
            var result = _sut.Validate(testMsg.StatusReport);

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
            var testMsg = GetFullStatus(isPoint: false);

            // Act
            var result = _sut.Validate(testMsg.StatusReport);

            // Assert
            Assert.IsTrue(result.IsValid);
        }

        /// <summary>
        /// Validate_S the full status message storing to database_ integration_ test.
        /// </summary>
        [Test]
        public void Validate_FullStatusMessageStoringToDatabase_Integration_Test()
        {
            var requiredFieldsMessage = GetSimpleStatus();
            var allFieldsPointMessage = GetFullStatus(isPoint: true);
            var allFieldsStrobeMessage = GetFullStatus(isPoint: false);

            // Only Required fields SDA
            sapientDatabase.DbStatus(requiredFieldsMessage);

            // All Fields with Location SDA
            sapientDatabase.DbStatus(allFieldsPointMessage);

            // All Fields with RangeBearing SDA
            sapientDatabase.DbStatus(allFieldsStrobeMessage);
        }

        // Enable it when test data files required
        // [Test]
        public void Write_Json()
        {
            string folderPath = @"C:\Work\Sapient\Sapient_V3_Test_Data";
            string nodeId = "284436D8-E0B7-4E9F-B5E0-E3C347C2CA90";

            var requiredFieldsMessage = GetSimpleStatus();
            var allFieldsPointMessage = GetFullStatus(isPoint: true);
            var allFieldsStrobeMessage = GetFullStatus(isPoint: false);

            requiredFieldsMessage.NodeId = nodeId;
            allFieldsPointMessage.NodeId = nodeId;
            allFieldsStrobeMessage.NodeId = nodeId;

            CreateFile(Path.Combine(folderPath, "status_basic_with_point.json"), requiredFieldsMessage.ToString());
            CreateFile(Path.Combine(folderPath, "status_full_with_point.json"), allFieldsPointMessage.ToString());
            CreateFile(Path.Combine(folderPath, "status_full_with_strobe.json"), allFieldsStrobeMessage.ToString());
        }

        private void CreateFile(string filePath, string json)
        {
            File.WriteAllText(filePath, json);
        }

        /// <summary>
        /// Gets the simple status.
        /// </summary>
        /// <returns>A SapientMessage.</returns>
        private SapientMessage GetSimpleStatus()
        {
            return new SapientMessage
            {
                Timestamp = Timestamp.FromDateTime(DateTime.UtcNow),
                NodeId = Guid.NewGuid().ToString(),
                StatusReport = new StatusReport
                {
                    ReportId = Ulid.NewUlid().ToString(),
                    System = StatusReport.Types.System.Warning,
                    Info = StatusReport.Types.Info.New,
                },
            };
        }

        /// <summary>
        /// Gets the full status.
        /// </summary>
        /// <param name="isPoint">If true, is point.</param>
        /// <returns>A SapientMessage.</returns>
        private SapientMessage GetFullStatus(bool isPoint)
        {
            var status = GetSimpleStatus();
            status.StatusReport.ActiveTaskId = Ulid.NewUlid().ToString();
            status.StatusReport.Mode = "Transition";
            status.StatusReport.Power = new StatusReport.Types.Power
            {
                Source = "Battery",
                Status = "Fault",
                Level = 40,
            };
            status.StatusReport.SensorLocation = DefaultLocation;
            status.StatusReport.Status.Add(new StatusReport.Types.Status()
            {
                StatusLevel = StatusReport.Types.StatusLevel.WarningStatus,
                StatusType = "Power status",
                StatusValue = "0.40",
            });
            status.StatusReport.StatusRegion.Add(new StatusReport.Types.Region
            {
                RegionType = "Region",
                RegionId = Ulid.NewUlid().ToString(),
                RegionName = "Power Region",
                RegionStatus = "Running Low",
                Description = "Power running low",
            });

            status.StatusReport.FieldOfView = new();
            status.StatusReport.Coverage = new();
            status.StatusReport.Obscuration.Add(new StatusReport.Types.LocationOrRangeBearing());
            status.StatusReport.StatusRegion[0].Region_ = new();

            if (isPoint)
            {

                status.StatusReport.FieldOfView.LocationList = new();
                status.StatusReport.FieldOfView.LocationList.Locations.Add(DefaultLocation);

                status.StatusReport.Coverage.LocationList = new();
                status.StatusReport.Coverage.LocationList.Locations.Add(DefaultLocation);

                status.StatusReport.Obscuration[0].LocationList = new();
                status.StatusReport.Obscuration[0].LocationList.Locations.Add(DefaultLocation);

                status.StatusReport.StatusRegion[0].Region_.LocationList = new();
                status.StatusReport.StatusRegion[0].Region_.LocationList.Locations.Add(DefaultLocation);
            }
            else
            {
                status.StatusReport.FieldOfView.RangeBearing = DefaultRangeBearingCone;
                
                status.StatusReport.Coverage.RangeBearing = DefaultRangeBearingCone;

                status.StatusReport.Obscuration[0].RangeBearing = DefaultRangeBearingCone;

                status.StatusReport.StatusRegion[0].Region_.RangeBearing = DefaultRangeBearingCone;
            }

            return status;
        }
    }
}
