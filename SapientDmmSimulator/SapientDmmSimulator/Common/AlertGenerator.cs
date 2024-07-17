// Project:           $Project: 00010855-Sapient$
// File:              $Workfile: AlertGenerator.cs$
// Crown-owned copyright, 2021-2024
//  See Release/Supply Conditions

namespace SapientDmmSimulator.Common
{
    using Google.Protobuf.WellKnownTypes;
    using log4net;
    using Sapient.Common;
    using Sapient.Data;
    using SAPIENT.ReadSampleMessage;
    using SapientServices.Communication;

    /// <summary>
    /// Generate Alert messages.
    /// </summary>
    public class AlertGenerator : BaseGenerators
    {
        /// <summary>
        /// Gets or sets an Image URL to provide with the detection message.
        /// </summary>
        public static string ImageURL { get; set; }

        /// <summary>
        /// log4net logger.
        /// </summary>
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Send an Alert message.
        /// </summary>
        /// <param name="comms_connection">connection to use to send message.</param>
        public void GenerateAlert(object comms_connection)
        {
            IConnection messenger = (IConnection)comms_connection;
            do
            {
                Log.Info("Create Alert.");
                ReadSampleMessage readSampleMessage = new ReadSampleMessage();
                string error = string.Empty;

                SapientMessage? message = readSampleMessage.ReadSampleMessageFromFile(SapientMessage.ContentOneofCase.Alert, string.Empty, out error);
                if (message != null)
                {
                    message.NodeId = Properties.Settings.Default.FixedDMMId;
                    message.Timestamp = Timestamp.FromDateTime(DateTime.Now.ToUniversalTime());
                    if (message.Alert != null)
                    {
                        message.Alert.AlertId = Ulid.NewUlid().ToString();
                    }
                }
                else
                {
                    Log.Error(error);
                }

                if ((message != null) && (message.Alert != null))
                {
                    if (!string.IsNullOrEmpty(ImageURL))
                    {
                        message.Alert.AssociatedFile.AddRange(new Google.Protobuf.Collections.RepeatedField<AssociatedFile>
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

                        Log.InfoFormat("Send Alert: {0}", message.Alert.AlertId);
                    }
                    else
                    {
                        Log.ErrorFormat("Send Alert Failed: {0}", message.Alert.AlertId);
                    }
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
