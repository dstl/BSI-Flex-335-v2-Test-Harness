// Crown-owned copyright, 2021-2024
namespace SapientASMsimulator.Common
{
    using Google.Protobuf.WellKnownTypes;
    using log4net;
    using Sapient.Data;
    using SAPIENT.ReadSampleMessage;
    using SapientServices;
    using SapientServices.Communication;
    using System;
    using System.Threading.Tasks;
    using Task = Sapient.Data.Task;

    public class TaskAckGenerator
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static string AsmId { get; set; }

        public void GenerateTaskAck(IConnection messenger, IGUIInterface form, SapientMessage msg, SapientLogger logger)
        {

            if ((msg != null) && (msg.Task != null))
            {
                Log.Info(string.Format("Process task ack for: {0}", msg.Task.Command.CommandCase.ToString()));
                ReadSampleMessage readSampleMessage = new ReadSampleMessage();
                string error = string.Empty;
                string additional = string.Empty;
                switch (msg.Task.Command.CommandCase)
                {
                    case Task.Types.Command.CommandOneofCase.None:
                        additional = ".NotSupported";
                        break;
                    case Task.Types.Command.CommandOneofCase.Request:                    
                        switch (msg?.Task?.Command?.Request?.ToLower())
                        {
                            case "registration":
                                additional = ".Request.Registration";
                                break;
                            case "status":
                                additional = ".Request.Status";
                                break;
                            case "reset":
                                additional = ".Request.Reset";
                                break;
                            case "stop":
                                additional = ".Request.Stop";
                                break;
                            case "start":
                                additional = ".Request.Start";
                                break;
                            default:
                                additional = ".Request.Unknown";
                                break;
                        }
                        break;
                    case Task.Types.Command.CommandOneofCase.DetectionThreshold:
                        if (msg?.Task?.Command?.DetectionThreshold == Sapient.Data.Task.Types.DiscreteThreshold.Unspecified)
                        {
                            additional = ".NotSupported";
                        }
                        else
                        {
                            additional = ".DetectionThreshold";
                        }
                        break;
                    case Task.Types.Command.CommandOneofCase.DetectionReportRate:
                        if (msg?.Task?.Command?.DetectionReportRate == Sapient.Data.Task.Types.DiscreteThreshold.Unspecified)
                        {
                            additional = ".NotSupported";
                        }
                        else
                        {
                            additional = ".DetectionReportRate";
                        }
                        break;
                    case Task.Types.Command.CommandOneofCase.ClassificationThreshold:
                        if (msg?.Task?.Command?.ClassificationThreshold == Sapient.Data.Task.Types.DiscreteThreshold.Unspecified)
                        {
                            additional = ".NotSupported";
                        }
                        else
                        {
                            additional = ".ClassificationThreshold";
                        }
                        break;
                    case Task.Types.Command.CommandOneofCase.ModeChange:
                        additional = ".ModeChange";
                        break;
                    case Task.Types.Command.CommandOneofCase.LookAt:
                        additional = ".LookAt";
                        break;
                    case Task.Types.Command.CommandOneofCase.MoveTo:
                        additional = ".MoveTo";
                        break;
                    case Task.Types.Command.CommandOneofCase.Patrol:
                        additional = ".Patrol";
                        break;
                    case Task.Types.Command.CommandOneofCase.Follow:
                        additional = ".Follow";
                        break;
                    default:
                        additional = ".NotSupported";
                        break;
                }

                SapientMessage? message = readSampleMessage.ReadSampleMessageFromFile(SapientMessage.ContentOneofCase.TaskAck, additional, out error);
                if (message != null)
                {
                    message.DestinationId = msg.NodeId;
                    message.NodeId = AsmId;
                    message.Timestamp = Timestamp.FromDateTime(DateTime.Now.ToUniversalTime());
                    if (message.TaskAck != null)
                    {
                        message.TaskAck.TaskId = msg.Task.TaskId;
                    }

                    bool retval = MessageSender.Send(messenger, message, logger);

                    if (retval)
                    {
                        Log.InfoFormat("Send SensorTaskACK {0} Succeeded", msg.Task.TaskId);
                    }
                    else
                    {
                        Log.ErrorFormat("Send SensorTaskACK {0} Failed", msg.Task.TaskId);
                    }
                }
                else
                {
                    Log.Error(error);
                }
            }
        }
    }
}
