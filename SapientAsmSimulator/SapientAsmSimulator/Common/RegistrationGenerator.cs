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

    public class RegistrationGenerator
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static string AsmId { get; set; }

        public static DateTime SendRegistrationTime { get; protected set; }

        public void GenerateRegistration(IConnection messenger, IGUIInterface form, SapientLogger logger)
        {
            ReadSampleMessage readSampleMessage = new ReadSampleMessage();
            string error = string.Empty;
            SapientMessage? message = readSampleMessage.ReadSampleMessageFromFile(SapientMessage.ContentOneofCase.Registration, string.Empty, out error);
            if (message != null)
            {
                message.NodeId = AsmId;
                message.Timestamp = Timestamp.FromDateTime(DateTime.Now.ToUniversalTime());
                bool retval = MessageSender.Send(messenger, message, logger);

                if (retval)
                {
                    SendRegistrationTime = DateTime.UtcNow;
                    Log.Info("Send Registration Succeeded");
                }
                else
                {
                    Log.Error("Send Registration Failed");
                }
            }
            else
            {
                Log.Error(error);
            }
        }
    }
}
