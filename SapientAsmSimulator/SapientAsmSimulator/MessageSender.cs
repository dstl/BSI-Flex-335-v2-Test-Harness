// File:              $Workfile: MessageSender.cs$
// <copyright file="MessageSender.cs" >
// Crown-owned copyright, 2021-2024
// See Release/Supply Conditions
// </copyright>

namespace SapientASMsimulator
{
    using System;
    using System.Threading;
    using SapientServices;
    using SapientServices.Communication;
    using Sapient;
    using Google.Protobuf;
    using Sapient.Data;

    /// <summary>
    /// Send Message
    /// </summary>
    public class MessageSender
    {
        /// <summary>
        /// Gets or sets a value indicating whether to fragment data to test communication resilience
        /// </summary>
        public static bool FragmentData { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to put a delay between data fragments
        /// </summary>
        public static int PacketDelay { get; set; }

        /// <summary>
        /// Send Data over Sapient connection
        /// </summary>
        /// <param name="messenger">connection object</param>
        /// <param name="message">message string to send</param>
        /// <param name="logger">file data logger</param>
        /// <returns>true if successful</returns>
        public static bool Send(IConnection messenger, SapientMessage message, SapientLogger logger)
        {
            bool retval = messenger.SendMessage(message);

            if (logger != null && Properties.Settings.Default.Log)
            {
                logger.Log(message.ToString());
            }

            return retval;
        }
    }
}
