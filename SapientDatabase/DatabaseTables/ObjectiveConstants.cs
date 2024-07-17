// Project:           $Project: 00010855-Sapient$
// File:              $Workfile: ObjectiveConstants.cs$
// Crown-owned copyright, 2021-2024
//  See Release/Supply Conditions
// $NoKeywords$
namespace SapientDatabase.DatabaseTables
{
    /// <summary>
    /// The names of objective tables and sequences.
    /// </summary>
    public static class ObjectiveConstants
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
        /// The approval.
        /// </summary>
        public static class Approval
        {
            public const string Table = "hl_task_approval_BSIFlex335v2";
            public const string Pkey = "hl_task_approval_pkey";
        }
    }
}