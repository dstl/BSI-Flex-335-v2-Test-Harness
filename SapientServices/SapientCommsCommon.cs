// File:              $Workfile: SapientCommsCommon.cs$
// <copyright file="SapientCommsCommon.cs" >
// Crown-owned copyright, 2021-2024
//  See Release/Supply Conditions
// </copyright>

using Sapient.Data;

namespace SapientServices.Communication
{
    /// <summary>
    /// delegates class for Sapient Communications
    /// </summary>
    public class SapientCommsCommon
    {
        /// <summary>
        /// delegate for Data Received Callback
        /// </summary>
        /// <param name="msg">message data</param>
        /// <param name="size">message size in bytes</param>
        /// <param name="client">connection object</param>
        public delegate void DataReceivedCallback(SapientMessage msg, IConnection client);

        /// <summary>
        /// delegate for Status Callback
        /// </summary>
        /// <param name="statusMsg">status message</param>
        /// <param name="client">connection object</param>
        public delegate void StatusCallback(string statusMsg, IConnection client);
    }
}
