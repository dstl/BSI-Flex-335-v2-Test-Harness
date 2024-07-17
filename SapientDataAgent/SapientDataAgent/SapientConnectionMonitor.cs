// Project:           $Project: 00010855-Sapient$
// File:              $Workfile: SapientConnectionMonitor.cs$
// Crown-owned copyright, 2021-2024
//  See Release/Supply Conditions
// $NoKeywords$

namespace SapientMiddleware
{
    using log4net;

    /// <summary>
    /// Class for capturing Data Agent Connection status.
    /// </summary>
    public class SapientConnectionMonitor : IConnectionMonitor
    {
        /// <summary>
        /// Log4net logger.
        /// </summary>
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Set Task Manager connected tick box.
        /// </summary>
        /// <param name="connected">whether connected.</param>
        public void SetTaskManagerConnected(bool connected)
        {
            if (connected)
            {
                Log.Warn("Tasking Connection Connected");
            }
            else
            {
                Log.Warn("Tasking Connection Disconnected");
            }
        }

        /// <summary>
        /// Set client connected indicator.
        /// </summary>
        /// <param name="connected">whether connected.</param>
        public void SetClientConnectedStatus(bool connected)
        {
            if (connected)
            {
                Log.Warn("Client Connection Connected");
            }
            else
            {
                Log.Warn("Client Connection Disconnected");
            }
        }

        /// <summary>
        /// Set Number of Clients.
        /// </summary>
        /// <param name="numClients">Number of Clients.</param>
        public void SetNumClients(int numClients)
        {
            Log.WarnFormat("{0} Client(s) Connected", numClients);
        }

        /// <summary>
        /// Set Number of GUI Clients Connected.
        /// </summary>
        /// <param name="numClients">Number of GUI Clients.</param>
        public void SetNumGuiClients(int numClients)
        {
            Log.WarnFormat("{0} GUI Client(s) Connected", numClients);
        }

        /// <summary>
        /// Disable logging of GUI connections - nothing to do at the moment.
        /// </summary>
        public void DisableGUIConnectionReporting()
        {
           // Do nothing at the moment.
        }

        /// <summary>
        /// Set window title text.
        /// </summary>
        /// <param name="text">text string.</param>
        public void SetWindowText(string text)
        {
            Log.Warn(text);
        }
    }
}
