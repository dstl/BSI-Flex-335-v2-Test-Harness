// Project:           $Project: 00010855-Sapient$
// File:              $Workfile: IConnectionMonitor.cs$
// Crown-owned copyright, 2021-2024
//  See Release/Supply Conditions
// $NoKeywords$

namespace SapientMiddleware
{
    /// <summary>
    /// Interface Class for capturing Data Agent Connection status.
    /// </summary>
    public interface IConnectionMonitor
    {
        /// <summary>
        /// Set Task Manager connected tick box.
        /// </summary>
        /// <param name="connected">whether connected.</param>
        void SetTaskManagerConnected(bool connected);

        /// <summary>
        /// Set client connected indicator.
        /// </summary>
        /// <param name="connected">whether connected.</param>
        void SetClientConnectedStatus(bool connected);

        /// <summary>
        /// Set Number of Clients.
        /// </summary>
        /// <param name="numClients">Number of Clients.</param>
        void SetNumClients(int numClients);

        /// <summary>
        /// Set Number of GUI Clients Connected.
        /// </summary>
        /// <param name="numClients">Number of GUI Clients.</param>
        void SetNumGuiClients(int numClients);

        /// <summary>
        /// Hide indications for GUI connections.
        /// </summary>
        void DisableGUIConnectionReporting();

        /// <summary>
        /// Set window title text.
        /// </summary>
        /// <param name="text">text string.</param>
        void SetWindowText(string text);
    }
}
