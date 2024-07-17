// File:              $Workfile: ICommsConnection.cs$
// <copyright file="ICommsConnection.cs" >
// Crown-owned copyright, 2021-2024
//  See Release/Supply Conditions
// </copyright>

namespace SapientServices.Communication
{
    /// <summary>
    /// interface definition for Communications Connection
    /// </summary>
    public interface ICommsConnection
    {
        /// <summary>
        /// interfatart Communications Connection
        /// </summary>
        /// <param name="maximumPacketSize">maximum packet size in bytes</param>ce method to S
        /// <param name="sendOnly">whether a send only connection</param>
        void Start(uint maximumPacketSize, bool sendOnly);

        /// <summary>
        /// interface method to Start Communications Connection
        /// </summary>
        void Start();

        /// <summary>
        /// interface method to Shutdown Communications Connection
        /// </summary>
        void Shutdown();

        /// <summary>
        /// interface method to Set Data Received Callback for Communications Connection
        /// </summary>
        /// <param name="callback">callback method</param>
        void SetDataReceivedCallback(SapientCommsCommon.DataReceivedCallback callback);

        /// <summary>
        /// interface method to Set Connected Callback for Communications Connection
        /// </summary>
        /// <param name="statuscallback">callback method</param>
        void SetConnectedCallback(SapientCommsCommon.StatusCallback statuscallback);

        /// <summary>
        /// Poll Communications connection status
        /// </summary>
        /// <returns>true for connected</returns>
        bool IsConnected();
    }
}
