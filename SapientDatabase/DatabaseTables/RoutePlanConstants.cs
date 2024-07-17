// Project:           $Project: 00010855-Sapient$
// File:              $Workfile: RoutePlanConstants.cs$
// Crown-owned copyright, 2021-2024
//  See Release/Supply Conditions
// $NoKeywords$
namespace SapientDatabase.DatabaseTables
{
    /// <summary>
    /// The names of route plan tables and sequences.
    /// </summary>
    internal static class RoutePlanConstants
    {
        public const string Table = "route_plan_BSIFlex335v2";
        public const string Seq = "route_plan_id_seq";
        public const string Pkey = "route_plan_pkey";
        public const string CommonKeyName = "route_plan_id";
    }

    /// <summary>
    /// The names of route plan field of view table and sequences.
    /// </summary>
    internal static class RoutePlanRangeBearingConstants
    {
        public const string Table = "route_plan_fov_range_bearing_BSIFlex335v2";
        public const string IdSeq = RoutePlanConstants.Seq;
        public const string Pkey = "route_plan_range_bearing_pkey";
        public const string CommonKeyName = RoutePlanConstants.CommonKeyName;
    }
}
