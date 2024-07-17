// Crown-owned copyright, 2021-2024
namespace SapientASMsimulator
{
    using Google.Protobuf.WellKnownTypes;
    using log4net;
    using Sapient.Data;
    using SAPIENT.ReadSampleMessage;
    using SapientASMsimulator.Common;
    using SapientServices;
    using SapientServices.Communication;

    public class HeartbeatGenerator : BaseGenerators
    {

        private static readonly ILog Log = LogManager.GetLogger(typeof(HeartbeatGenerator));

        public static double Azimuth { get; set; }

        public static double Elevation { get; set; }

        public static double Range { get; set; }

        public static double HExtent { get; set; }

        public string TaskId { get; set; }

        public HeartbeatGenerator()
        {
            Azimuth = 125;
            Elevation = 0;
            Range = 50;
            HExtent = 30;
        }

        public void GenerateStatus(object comms_connection, SapientLogger logger)
        {
            IConnection messenger = (IConnection)comms_connection;
            do
            {
                ReadSampleMessage readSampleMessage = new ReadSampleMessage();
                string error = string.Empty;
                SapientMessage? message = readSampleMessage.ReadSampleMessageFromFile(SapientMessage.ContentOneofCase.StatusReport, string.Empty, out error);
                if (message != null)
                {
                    message.NodeId = ASMId;
                    message.Timestamp = Timestamp.FromDateTime(DateTime.Now.ToUniversalTime());
                    if (message.StatusReport != null)
                    {
                        message.StatusReport.ReportId = Ulid.NewUlid().ToString();
                        message.StatusReport.ActiveTaskId = this.TaskId;
                    }
                    this.MessageCount++;
                    bool retval = MessageSender.Send(messenger, message, logger);

                    if (retval)
                    {
                        Log.InfoFormat("Sent StatusReport:{0}", message.StatusReport.ReportId);
                    }
                    else
                    {
                        Log.ErrorFormat("Sent StatusReport Failed:{0}", message.StatusReport.ReportId);
                    }

                    if (this.LoopMessages)
                    {
                        Thread.Sleep(this.LoopTime);
                    }
                }
                else
                {
                    Log.Error(error);
                }
            } while (this.LoopMessages);
        }
    }
}
