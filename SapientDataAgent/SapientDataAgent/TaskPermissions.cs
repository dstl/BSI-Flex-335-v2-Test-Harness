// Project:           $Project: 00010855-Sapient$
// File:              $Workfile: TaskPermissions.cs$
// Crown-owned copyright, 2021-2024
//  See Release/Supply Conditions
// $NoKeywords$

namespace SapientMiddleware
{
    using System.Collections.Generic;
    using log4net;

    /// <summary>
    /// Holds the identities of sensors for which the GUI has taken control.
    /// </summary>
    public class TaskPermissions
    {
        /// <summary>
        /// Log4net logger.
        /// </summary>
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Contains the ids of any sensors that the GUI has taken control of. Once released by the GUI, the id is removed from this set.
        /// </summary>
        private ISet<string> controlledIds;

        /// <summary>
        /// Used to protect access to controlledIds.
        /// </summary>
        private object locker = new object();

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskPermissions"/> class.
        /// </summary>
        public TaskPermissions()
        {
            controlledIds = new SortedSet<string>();
        }

        /// <summary>
        /// Checks if the DMM currently has control of this ASM.
        /// </summary>
        /// <param name="asmId">The identifier of the ASM.</param>
        /// <returns>True if the DMM has control of the ASM and should forward these messages, false if it should reject them.</returns>
        public bool DmmHasControl(string asmId)
        {
            bool hasControl = true;
            lock (locker)
            {
                // DMM has control is the ASM ID is NOT in the set.
                hasControl = !controlledIds.Contains(asmId);
            }

            return hasControl;
        }

        /// <summary>
        /// Checks if the GuiProtocol class should forward sensor task messages with this id.
        /// </summary>
        /// <param name="asmId">The identifier for the ASM.</param>
        /// <returns>True if the GuiProtocol class should forward, false if it should reject it.</returns>
        public bool GuiProtocolShouldForward(string asmId)
        {
            bool shouldForward = true;
            lock (locker)
            {
                // sensor task messages should only be forwarded if the GUI has taken control, which means the ASM ID
                // be in the set.
                shouldForward = controlledIds.Contains(asmId);
            }

            return shouldForward;
        }

        /// <summary>
        /// Records that the GUI has taken control of the given sensor.
        /// </summary>
        /// <param name="asmId">The identifier of the sensor.</param>
        public void TakeControl(string asmId)
        {
            lock (locker)
            {
                if(controlledIds.Add(asmId))
                {
                    Log.Info($"GUI taking control of sensor {asmId}.");
                }
            }
        }

        /// <summary>
        /// Records that the GUI has released control of the given sensor.
        /// </summary>
        /// <param name="asmId">The identifier of the sensor.</param>
        public void ReleaseControl(string asmId)
        {
            lock (locker)
            {
                if(controlledIds.Remove(asmId))
                {
                    Log.Info($"GUI releasing control of sensor {asmId}.");
                }
            }
        }
    }
}
