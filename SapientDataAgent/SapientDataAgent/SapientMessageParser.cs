// Project:           $Project: 00010855-Sapient$
// File:              $Workfile: SapientMessageParser.cs$
// Crown-owned copyright, 2021-2024
//  See Release/Supply Conditions
// $NoKeywords$

[assembly: log4net.Config.XmlConfigurator(Watch = true)]

namespace SapientMiddleware
{
    using System.Text;
    using log4net;
    using Microsoft.VisualBasic.Logging;
    using Sapient.Data;
    using SapientDatabase;
    using SapientServices;

    /// <summary>
    /// class to handle parsing of input messages from bytes via xml to class.
    /// </summary>
    public class SapientMessageParser
    {
        /// <summary>
        /// Underlying message protocol.
        /// </summary>
        public readonly SapientProtocol SapientProtocol;

        /// <summary>
        /// Log4net logger.
        /// </summary>
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// current message string buffer.
        /// </summary>
        private readonly StringBuilder currentMessage = new StringBuilder();

        /// <summary>
        /// Initializes a new instance of the <see cref="SapientMessageParser" /> class.
        /// </summary>
        /// <param name="sapient_protocol">protocol to apply.</param>
        /// <param name="discardUnterminatedMessages">whether to discard unterminated messages.</param>
        public SapientMessageParser(SapientProtocol sapient_protocol, bool discardUnterminatedMessages)
        {
            SapientProtocol = sapient_protocol;
        }

        /// <summary>
        /// Builds the and notify.
        /// Targeted to protobuf message handling.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="db">The database.</param>
        /// <param name="connection_id">The connection identifier.</param>
        public void BuildAndNotify(SapientMessage message, Database db, uint connection_id)
        {
            if (message != null)
            {
                // Report the complete message.
                SapientMessageType msgResponse = SapientProtocol.ParseReceivedMessage(message, db);
                SapientProtocol.SendResponse(msgResponse, connection_id);
            }
        }

        /// <summary>
        /// Attempt recovery from a problem with parsing input messages by clearing the buffer.
        /// </summary>
        public void ResetMessageBuffer()
        {
            log.Error("Clear Message Buffer");
            currentMessage.Clear();
        }
    }
}