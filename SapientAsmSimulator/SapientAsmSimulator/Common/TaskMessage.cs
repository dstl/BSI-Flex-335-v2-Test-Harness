// Crown-owned copyright, 2021-2024
namespace SapientASMsimulator.Common
{
    using log4net;
    using Sapient.Common;
    using Sapient.Data;
    using SapientServices;
    using SapientServices.Communication;
    using System;

    public class TaskMessage
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(TaskMessage));

        public static string AsmId { get; set; }

        public static HeartbeatGenerator HeartbeatGenerator { get; set; }

        public static RegistrationGenerator RegistrationGenerator { get; set; }

        public static TaskAckGenerator TaskAckGenerator { get; set; }

        public void ProcessSensorTaskMessage(SapientMessage message, IConnection messenger, IGUIInterface form, SapientLogger logger)
        {
            Task task = message.Task;
            if (task != null)
            {
                Log.Info(string.Format("Task Message Received: {0}", task.Command.CommandCase.ToString()));

                switch (task.Command.CommandCase)
                {
                    case Task.Types.Command.CommandOneofCase.None:
                        break;
                    case Task.Types.Command.CommandOneofCase.Request:
                        ProcessCommandRequestMessage(message, messenger, form, logger);
                        break;
                    case Task.Types.Command.CommandOneofCase.DetectionThreshold:
                        break;
                    case Task.Types.Command.CommandOneofCase.DetectionReportRate:
                        break;
                    case Task.Types.Command.CommandOneofCase.ClassificationThreshold:
                        break;
                    case Task.Types.Command.CommandOneofCase.ModeChange:
                        break;
                    case Task.Types.Command.CommandOneofCase.LookAt:
                        ProcessLookAtMessage(message, messenger, form, logger);
                        break;
                    case Task.Types.Command.CommandOneofCase.MoveTo:
                        break;
                    case Task.Types.Command.CommandOneofCase.Patrol:
                        break;
                    case Task.Types.Command.CommandOneofCase.Follow:
                        break;
                    default:
                        break;
                }

                TaskAckGenerator.GenerateTaskAck(messenger, form, message, logger);
            }
        }

        private void ProcessCommandRequestMessage(SapientMessage message, IConnection messenger, IGUIInterface form, SapientLogger logger)
        {
            if (message?.Task?.Command?.Request != null)
            {
                switch (message.Task.Command.Request.ToLower())
                {
                    case "registration":
                        RegistrationGenerator.GenerateRegistration(messenger, form, logger);
                        break;
                    case "status":
                        HeartbeatGenerator.GenerateStatus(messenger, logger);
                        break;
                    case "reset":
                        break;
                    case "stop":
                        break;
                    case "start":
                        break;
                    default:
                        break;
                }
            }
        }

        private void ProcessLookAtMessage(SapientMessage message, IConnection messenger, IGUIInterface form, SapientLogger logger)
        {
            var task = message.Task;
            if (task.Command.LookAt.RangeBearing != null)
            {
                bool isNewDefaultTask = false;
                ProcessRegion(task.Command.LookAt.RangeBearing, task, isNewDefaultTask);
            }
        }

        private void ProcessRegion(RangeBearingCone rb, Task task, bool isNewDefaultTask)
        {
            DateTime timenow = DateTime.UtcNow;

            if (rb.HorizontalExtent != 0)
            {
                HeartbeatGenerator.HExtent = rb.HorizontalExtent;
            }

            try
            {
                if (rb.Range == 0)
                {
                    Log.InfoFormat("PTFOV Task:{0},t:{1},n:{2}, P:{3}, T:{4}, HFOV:{5}", task.TaskId, task.TaskName, timenow, rb.Azimuth, rb.Elevation, rb.HorizontalExtent);
                    HeartbeatGenerator.Range = rb.Range;
                    HeartbeatGenerator.Azimuth = rb.Azimuth;
                    HeartbeatGenerator.Elevation = rb.Elevation;
                }
                else
                {
                    Log.InfoFormat("Task:{0},t:{1},n:{2}, Az:{3:F1} El:{4:F1} R:{5:F1} FOV:{6:F1}", task.TaskId, task.TaskName, timenow, rb.Azimuth, rb.Elevation, rb.Range, rb.HorizontalExtent);
                    HeartbeatGenerator.Range = rb.Range;
                    HeartbeatGenerator.Azimuth = rb.Azimuth;
                    HeartbeatGenerator.Elevation = rb.Elevation;
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error in region task:", ex);
            }
        }
    }
}
