// Project:           $Project: 00010855-Sapient$
// File:              $Workfile: SapientMessageMonitor.cs$
// Crown-owned copyright, 2021-2024
//  See Release/Supply Conditions
// $NoKeywords$

namespace SapientMiddleware
{
    using System;
    using SapientDatabase;
    using SapientServices;

    /// <summary>
    /// Class for capturing Data Agent Message status.
    /// </summary>
    public class SapientMessageMonitor
    {
        /// <summary>
        /// Log Database Object.
        /// </summary>
        private static MiddlewareLogDatabase LogDatabase;

        private const int NumMessageTypes = 19; // size of SapientMessageType enum

        /// <summary>
        /// Initializes a new instance of the <see cref="SapientMessageMonitor" /> class.
        /// </summary>
        /// <param name="logDatabase">log database object.</param>
        public SapientMessageMonitor(MiddlewareLogDatabase logDatabase)
        {
            LogDatabase = logDatabase;
            MessageTime = new DateTime[NumMessageTypes];
            MessageCount = new int[NumMessageTypes];
            StatusText = new string[2];
            Latency = new double[2];
        }

        /// <summary>
        /// Gets list of times of message types being received.
        /// </summary>
        public DateTime[] MessageTime { get; private set; }

        /// <summary>
        /// Gets list of message counts.
        /// </summary>
        public int[] MessageCount { get; private set; }

        /// <summary>
        /// Gets list of status text.
        /// </summary>
        public string[] StatusText { get; private set; }

        /// <summary>
        /// Gets list of latency Values.
        /// </summary>
        public double[] Latency { get; private set; }

        /// <summary>
        /// Gets a value indicating whether we have received a message since we last checked.
        /// </summary>
        public bool RecentMessage { get; private set; }

        /// <summary>
        /// Gets a value indicating whether we have had and error since we last checked.
        /// </summary>
        public bool RecentError { get; private set; }

        /// <summary>
        /// Set Message Time.
        /// </summary>
        /// <param name="type">message type index into message time array.</param>
        public void SetMessageTime(SapientMessageType type)
        {
            DateTime time = DateTime.UtcNow;
            MessageTime[(int)type] = time;

            if (LogDatabase != null)
            {
                LogDatabase.SetMessageTime(type, time);
            }
        }

        /// <summary>
        /// Set Message Count.
        /// </summary>
        /// <param name="type">message type index into message count array.</param>
        public void IncrementMessageCount(SapientMessageType type)
        {
            MessageCount[(int)type]++;
            RecentMessage = true;

            switch (type)
            {
                case SapientMessageType.Error:
                case SapientMessageType.IdError:
                case SapientMessageType.InternalError:
                case SapientMessageType.InvalidClient:
                case SapientMessageType.InvalidTasking:
                case SapientMessageType.ResponseIdError:
                case SapientMessageType.Unknown:
                case SapientMessageType.Unsupported:
                    RecentError = true;
                    break;
            }

            // reduce number of calls by doing this here.
            SetMessageTime(type);

            if (LogDatabase != null)
            {
                LogDatabase.SetMessageCount(type, MessageCount[(int)type]);
            }
        }

        /// <summary>
        /// Clear recent error and message display.
        /// </summary>
        public void ClearRecent()
        {
            RecentError = false;
            RecentMessage = false;
        }

        /// <summary>
        /// Clear message time display.
        /// </summary>
        public void ClearMessageTimes()
        {
            int i = 0;
            for (i = 0; i < MessageTime.Length; i++)
            {
                MessageTime[i] = DateTime.MinValue;
            }
            ////UpdateMessageTimes();
        }

        /// <summary>
        /// Set Status Time Text.
        /// </summary>
        /// <param name="index">text box enumerator index into string array.</param>
        /// <param name="text">text to set status to - typically a time stamp.</param>
        public void SetStatusText(int index, string text)
        {
            if (index < StatusText.Length)
            {
                StatusText[index] = text;
            }
        }

        /// <summary>
        /// Set Latency Time.
        /// </summary>
        /// <param name="index">text box enumerator index into array.</param>
        /// <param name="value">new latency value.</param>
        public void SetLatency(int index, double value)
        {
            if (index < Latency.Length)
            {
                Latency[index] = value;
            }
        }

        /// <summary>
        /// Write counts to database.
        /// </summary>
        /// <param name="clientConnections">number of client connections.</param>
        /// <param name="taskConnections">number of tasking connections.</param>
        /// <param name="guiConnections">number of GUI connections.</param>
        /// <param name="databaseConnected">whether database connected.</param>
        /// <param name="commsLatency">communication latency.</param>
        /// <param name="databaseLatency"database latency></param>
        public void UpdateDatabase(int clientConnections, int taskConnections, int guiConnections, bool databaseConnected, double commsLatency, double databaseLatency)
        {
            if (LogDatabase != null)
            {
                LogDatabase.WriteConnectionStatusToDatabase(clientConnections, taskConnections, guiConnections, databaseConnected, commsLatency, databaseLatency);
                LogDatabase.WriteMessageCountsToDatabase();
            }
        }
    }
}
