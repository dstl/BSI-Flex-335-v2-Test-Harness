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
    /// The task integration tests.
    /// </summary>
    public class TaskIntegrationTests : BaseTests
    {
        private readonly TaskValidator _sut = new TaskValidator();

        /// <summary>
        /// Validate_S the only mandatory fields set_ ok.
        /// </summary>
        [Test]
        public void Validate_OnlyMandatoryFieldsSet_Ok()
        {
            // Arrange
            var testMsg = GetSimpleTask();

            // Act
            var result = _sut.Validate(testMsg);

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
            var testMsg = GetFullTask(isPoint: true);

            // Act
            var result = _sut.Validate(testMsg);

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
            var testMsg = GetFullTask(isPoint: false);

            // Act
            var result = _sut.Validate(testMsg);

            // Assert
            Assert.IsTrue(result.IsValid);
        }

        /// <summary>
        /// Validate_S the full task message storing to database_ integration_ test.
        /// </summary>
        [Test]
        public void Validate_FullTaskMessageStoringToDatabase_Integration_Test()
        {
            var requiredFieldsMessage = GetSapientMessage(GetSimpleTask());
            var allFieldsPointMessage = GetSapientMessage(GetFullTask(isPoint: true));
            var allFieldsStrobeMessage = GetSapientMessage(GetFullTask(isPoint: false));

            // Only Required fields SDA
            sapientDatabase.DbTask(requiredFieldsMessage, false);
            // Only Required fields HDA
            sapientDatabase.DbTask(requiredFieldsMessage, true);

            // All Fields with Location SDA
            sapientDatabase.DbTask(allFieldsPointMessage, false);
            // All Fields with Location HDA
            sapientDatabase.DbTask(allFieldsPointMessage, true);

            // All Fields with RangeBearing SDA
            sapientDatabase.DbTask(allFieldsStrobeMessage, false);
            // All Fields with RangeBearing SDA
            sapientDatabase.DbTask(allFieldsStrobeMessage, true);

            var taskAck = GetTaskAck();
            sapientDatabase.DbAcknowledgeTask(taskAck, SapientDatabase.DatabaseTables.TaskConstants.HLTaskAck.Table);
            sapientDatabase.DbAcknowledgeTask(taskAck, SapientDatabase.DatabaseTables.TaskConstants.SensorTaskAck.Table);
            sapientDatabase.DbAcknowledgeTask(taskAck, SapientDatabase.DatabaseTables.TaskConstants.GUITaskAck.Table);
        }

        // Enable it when test data files required
        // [Test]
        public void Write_Json()
        {
            string folderPath = @"C:\Work\Sapient\Sapient_V3_Test_Data";
            string asmId = "284436D8-E0B7-4E9F-B5E0-E3C347C2CA90";
            string dmmId = "58BA8ABA-94C3-4202-A6B2-9BE86F32C059";

            var requiredFieldsMessage = GetSapientMessage(GetSimpleTask());
            var allFieldsPointMessage = GetSapientMessage(GetFullTask(isPoint: true));
            var allFieldsStrobeMessage = GetSapientMessage(GetFullTask(isPoint: false));

            requiredFieldsMessage.Task.DestinationId = asmId;
            allFieldsPointMessage.Task.DestinationId = asmId;
            allFieldsStrobeMessage.Task.DestinationId = asmId;

            requiredFieldsMessage.NodeId = dmmId;
            allFieldsPointMessage.NodeId = dmmId;
            allFieldsStrobeMessage.NodeId = dmmId;

            CreateFile(Path.Combine(folderPath, "task_basic_with_point.json"), requiredFieldsMessage.ToString());
            CreateFile(Path.Combine(folderPath, "task_full_with_point.json"), allFieldsPointMessage.ToString());
            CreateFile(Path.Combine(folderPath, "task_full_with_strobe.json"), allFieldsStrobeMessage.ToString());
        }

        private void CreateFile(string filePath, string json)
        {
            File.WriteAllText(filePath, json);
        }

        /// <summary>
        /// Gets the sapient message.
        /// </summary>
        /// <param name="task">The task.</param>
        /// <returns>A SapientMessage.</returns>
        private SapientMessage GetSapientMessage(Sapient.Data.Task task) =>
            new SapientMessage
            {
                Timestamp = Timestamp.FromDateTime(DateTime.UtcNow),
                NodeId = Guid.NewGuid().ToString(),
                Task = task,
            };

        /// <summary>
        /// Gets the simple task.
        /// </summary>
        /// <returns>A Sapient.Data.Task.</returns>
        private Sapient.Data.Task GetSimpleTask() =>
         new Sapient.Data.Task
         {
             DestinationId = Guid.NewGuid().ToString(),
             TaskId = Ulid.NewUlid().ToString(),
             Control = Sapient.Data.Task.Types.Control.Stop,
         };

        /// <summary>
        /// Gets the full task.
        /// </summary>
        /// <param name="isPoint">If true, is point.</param>
        /// <returns>A Sapient.Data.Task.</returns>
        private Sapient.Data.Task GetFullTask(bool isPoint)
        {
            var task = GetSimpleTask();

            task.TaskName = "Stop ASM";
            task.TaskDescription = "Task request to Stop ASM";
            task.TaskStartTime = Timestamp.FromDateTime(DateTime.UtcNow);
            task.TaskEndTime = Timestamp.FromDateTime(DateTime.UtcNow.AddHours(2));
            ///task.Command.Add( "Stop");
            task.Command = new Sapient.Data.Task.Types.Command
            {
                Request = "Stop",
            };
            task.Region.Add(new Sapient.Data.Task.Types.Region
            {
                Type = Sapient.Data.Task.Types.RegionType.AreaOfInterest, //"Region Info",
                RegionId = Guid.NewGuid().ToString(),
                RegionName = "Threat Region",
            });

            task.Region[0].ClassFilter.Add(new Sapient.Data.Task.Types.ClassFilter
            {
                Parameter = new Sapient.Data.Task.Types.Parameter
                {
                    Name = "Range",
                    Value = 1.20F,
                    Operator = Sapient.Data.Task.Types.Operator.LessThan,
                },
                Priority = Sapient.Data.Task.Types.DiscreteThreshold.NoThreshold,
                Type = "Range",
            });

            task.Region[0].ClassFilter[0].SubClassFilter.Add(new Sapient.Data.Task.Types.SubClassFilter
            {
                Parameter = new Sapient.Data.Task.Types.Parameter
                {
                    Name = "Range",
                    Value = 1.20F,
                    Operator = Sapient.Data.Task.Types.Operator.LessThan,
                },
                Priority = Sapient.Data.Task.Types.DiscreteThreshold.NoThreshold,
                Type = "Range"
            });

            task.Region[0].BehaviourFilter.Add(new Sapient.Data.Task.Types.BehaviourFilter
            {
                Parameter = new Sapient.Data.Task.Types.Parameter
                {
                    Name = "Range",
                    Value = 1.20F,
                    Operator = Sapient.Data.Task.Types.Operator.LessThan,
                },
                Priority = Sapient.Data.Task.Types.DiscreteThreshold.NoThreshold,
                Type = "Range"
            });

            if (isPoint)
            {
                task.Region[0].RegionArea = new Sapient.Data.Task.Types.LocationOrRangeBearing();
                task.Region[0].RegionArea.LocationList = new Sapient.Common.LocationList();
                task.Region[0].RegionArea.LocationList.Locations.Add(DefaultLocation);
            }
            else
            {
                task.Region[0].RegionArea = new Sapient.Data.Task.Types.LocationOrRangeBearing
                {
                    RangeBearing = DefaultRangeBearingCone,
                };
            }

            return task;
        }

        /// <summary>
        /// Gets the task ack.
        /// </summary>
        /// <returns>A SapientMessage.</returns>
        private SapientMessage GetTaskAck()
        {
            return new SapientMessage
            {
                NodeId = Guid.NewGuid().ToString(),
                Timestamp = Timestamp.FromDateTime(DateTime.UtcNow),
                TaskAck = new TaskAck
                {
                    TaskId = Ulid.NewUlid().ToString(),
                    DestinationId = Guid.NewGuid().ToString(),
                    Status = TaskAck.Types.Status.Accepted,
                    Reason = "Accepted",
                },
            };
        }
    }
}
