// Project:           $Project: 00010855-Sapient$
// File:              $Workfile: MessageSender.cs$
// Crown-owned copyright, 2021-2024
//  See Release/Supply Conditions

namespace SapientDmmSimulator
{
    using System;
    using System.Threading;
    using Google.Protobuf;
    using Sapient;
    using Sapient.Data;
    using SapientServices.Communication;

    /// <summary>
    /// Send Message.
    /// </summary>
    public class MessageSender
    {
        public static bool FragmentData { get; set; }

        public static int PacketDelay { get; set; }

        /// <summary>
        /// Sends the specified messenger.
        /// </summary>
        /// <param name="messenger">The messenger.</param>
        /// <param name="message">The message.</param>
        /// <returns>Message send status true/false.</returns>
        public static bool Send(IConnection messenger, SapientMessage message)
        {
            return messenger.SendMessage(message);
        }
    }
}
