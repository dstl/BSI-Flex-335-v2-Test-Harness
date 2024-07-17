// Project:           $Project: 00010855-Sapient$
// File:              $Workfile: TaskConstants.cs$
// Crown-owned copyright, 2021-2024
//  See Release/Supply Conditions
// $NoKeywords$
namespace SapientDatabase.DatabaseTables
{
    /// <summary>
    /// The names of tasking tables and sequences.
    /// </summary>
    public static class TaskConstants
    {
        /// <summary>
        /// The objective.
        /// </summary>
        public static class Objective
        {
            public const string Table = "hl_objective_BSIFlex335v2";
            public const string Pkey = "hl_objective_pkey";
        }

        /// <summary>
        /// The sensor task.
        /// </summary>
        public static class SensorTask
        {
            public const string Table = "sensor_task_BSIFlex335v2";
            public const string Seq = "sensor_task_id_seq";
            public const string Pkey = "sensor_task_pkey";
        }

        /// <summary>
        /// The h l task.
        /// </summary>
        public static class HLTask
        {
            public const string Table = "hl_task_BSIFlex335v2";
            public const string Seq = "hl_task_id_seq";
            public const string Pkey = "hl_task_pkey";
        }

        /// <summary>
        /// The sensor task ack.
        /// </summary>
        public static class SensorTaskAck
        {
            public const string Table = "sensor_taskack_BSIFlex335v2";
            public const string Seq = "sensor_taskack_BSIFlex335v2_key_id_seq";
            public const string Pkey = "sensor_taskack_pkey";
        }

        /// <summary>
        /// The h l task ack.
        /// </summary>
        public static class HLTaskAck
        {
            public const string Table = "hl_taskack_BSIFlex335v2";
            public const string Seq = "hl_taskack_BSIFlex335v2_key_id_seq";
            public const string Pkey = "hl_taskack_pkey";
        }

        /// <summary>
        /// The g u i task ack.
        /// </summary>
        public static class GUITaskAck
        {
            public const string Table = "gui_taskack_BSIFlex335v2";
            public const string Seq = "gui_taskack_BSIFlex335v2_key_id_seq";
            public const string Pkey = "gui_taskack_pkey";
        }

        /// <summary>
        /// The sensor task region.
        /// </summary>
        public static class SensorTaskRegion
        {
            public const string Table = "sensor_task_region_BSIFlex335v2";
            public const string Seq = "sensor_task_region_BSIFlex335v2_key_id_seq";
            public const string Pkey = "sensor_task_region_BSIFlex335v2_pkey";
            public const string TaskKeyIdSeq = SensorTask.Seq;
        }

        /// <summary>
        /// The h l task region.
        /// </summary>
        public static class HLTaskRegion
        {
            public const string Table = "hl_task_region_BSIFlex335v2";
            public const string Seq = "hl_task_region_BSIFlex335v2_key_id_seq";
            public const string Pkey = "hl_task_region_BSIFlex335v2_pkey";
        }

        /// <summary>
        /// The h l task approval.
        /// </summary>
        public static class HLTaskApproval
        {
            public const string Table = "hl_task_approval_BSIFlex335v2";
            public const string Pkey = "hl_task_approval_pkey";
            public const string Accepted = "Accepted";
            public const string Rejected = "Rejected";
        }
    }
}
