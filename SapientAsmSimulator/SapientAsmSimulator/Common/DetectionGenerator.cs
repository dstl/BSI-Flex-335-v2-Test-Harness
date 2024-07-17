// File:              $Workfile: DetectionGenerator.cs$
// <copyright file="DetectionGenerator.cs" >
// Crown-owned copyright, 2021-2024
// See Release/Supply Conditions
// </copyright>

namespace SapientASMsimulator.Common
{
    using Google.Protobuf.WellKnownTypes;
    using log4net;
    using Sapient.Common;
    using Sapient.Data;
    using SAPIENT.ReadSampleMessage;
    using SapientServices;
    using SapientServices.Communication;

    public class DetectionGenerator : BaseGenerators
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public bool ChangeObjectID { get; set; }

        public string ImageURL { get; set; }

        public string SubClassTypeString { get; set; }

        public string ClassTypeString { get; set; }

        public string TaskId { get; set; }

        public void GenerateDetections(object comms_connection, SapientLogger logger)
        {
            var messenger = (IConnection)comms_connection;
            string objectId = Ulid.NewUlid().ToString();
            do
            {
                ReadSampleMessage readSampleMessage = new ReadSampleMessage();
                string error = string.Empty;
                SapientMessage? message = readSampleMessage.ReadSampleMessageFromFile(SapientMessage.ContentOneofCase.DetectionReport, string.Empty, out error);
                if (message != null)
                {
                    message.NodeId = ASMId;
                    message.Timestamp = Timestamp.FromDateTime(DateTime.Now.ToUniversalTime());
                    if(message.DetectionReport != null)
                    {
                        message.DetectionReport.ReportId = Ulid.NewUlid().ToString();
                        message.DetectionReport.ObjectId = objectId;
                        if (this.ChangeObjectID)
                        {
                            message.DetectionReport.ObjectId = Ulid.NewUlid().ToString();
                        }
                        message.DetectionReport.TaskId = TaskId;
                        if (!string.IsNullOrEmpty(ImageURL))
                        {
                            message.DetectionReport.AssociatedFile.Add(new AssociatedFile
                            {
                                Type = "image",
                                Url = ImageURL,
                            });
                        }
                    }

                    MessageCount++;

                    bool retval = MessageSender.Send(messenger, message, logger);

                    if (!retval)
                    {
                        Log.ErrorFormat("Send Detection Failed: {0}", MessageCount);
                    }

                    if (LoopMessages)
                    {
                        Thread.Sleep(LoopTime);
                    }
                }
                else
                {
                    Log.ErrorFormat(error);
                }

            } while (LoopMessages);
        }
    }
}
