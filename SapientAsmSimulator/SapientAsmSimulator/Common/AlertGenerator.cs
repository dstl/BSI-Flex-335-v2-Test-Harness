// File:              $Workfile: AlertGenerator.cs$
// <copyright file="AlertGenerator.cs" >
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

    /// <summary>
    /// The AlertGenerator class.
    /// </summary>
    /// <seealso cref="SapientASMsimulator.Common.BaseGenerators" />
    public class AlertGenerator : BaseGenerators
    {
        /// <summary>
        /// Log4net logger
        /// </summary>
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Gets or sets an Image URL to provide with the detection message
        /// </summary>
        public static string ImageURL { get; set; }


        public void GenerateAlert(object comms_connection, SapientLogger logger)
        {
            IConnection messenger = (IConnection)comms_connection;
            do
            {
                ReadSampleMessage readSampleMessage = new ReadSampleMessage();
                string error = string.Empty;
                SapientMessage? message = readSampleMessage.ReadSampleMessageFromFile(SapientMessage.ContentOneofCase.Alert, string.Empty, out error);
                if (message != null)
                {
                    message.NodeId = ASMId;
                    message.Timestamp = Timestamp.FromDateTime(DateTime.Now.ToUniversalTime());
                    if (!string.IsNullOrEmpty(ImageURL) && (message?.Alert?.AssociatedFile != null))
                    {
                        message.Alert.AssociatedFile.Add(new AssociatedFile() 
                        { 
                            Type = "URL",
                            Url = ImageURL
                        });
                    }
                    MessageCount++;
                    Console.WriteLine(message);
                    bool retval = MessageSender.Send(messenger, message, logger);
                    if (retval)
                    {
                        Log.InfoFormat("Send Alert:{0}", message.Alert.AlertId);
                    }
                    else
                    {
                        Log.ErrorFormat("Send Alert Failed:{0}", message.Alert.AlertId);
                    }

                    if (LoopMessages)
                    {
                        Thread.Sleep(LoopTime);
                    }
                }
                else
                {
                    Log.Error(error);
                }
            } while (LoopMessages);
        }
    }
}
