// Project:           $Project: 00010855-Sapient$
// File:              $Workfile: StatusReportConstants.cs$
// Crown-owned copyright, 2021-2024
//  See Release/Supply Conditions
// $NoKeywords$
namespace SapientDatabase.DatabaseTables
{
    /// <summary>
    /// The names of status report tables and sequences.
    /// </summary>
    internal static class StatusReportConstants
    {
        public const string Table = "status_report_BSIFlex335v2";
        public const string Seq = "status_id_seq";
        public const string Pkey = "status_report_pkey";
        public const string CommonKeyName = "status_id";

    }

    /// <summary>
    /// The names of status report messages table and sequences.
    /// </summary>
    internal static class StatusReportMessagesConstants
    {
        public const string Table = "status_report_messages_BSIFlex335v2";
        public const string IdSeq = StatusReportConstants.Seq;
        public const string Pkey = "status_report_messages_pkey";
        public const string CommonKeyName = StatusReportConstants.CommonKeyName;
    }

    /// <summary>
    /// The names of status report field of view table and sequences.
    /// </summary>
    internal static class StatusReportRangeBearingConstants
    {
        public const string Table = "status_report_range_bearing_BSIFlex335v2";
        public const string IdSeq = StatusReportConstants.Seq;
        public const string Pkey = "status_report_range_bearing_pkey";
        public const string CommonKeyName = StatusReportConstants.CommonKeyName;
    }

    /// <summary>
    /// The names of status report region table and sequences.
    /// </summary>
    internal static class StatusReportRegionConstants
    {
        public const string Table = "status_report_region_BSIFlex335v2";
        public const string IdSeq = StatusReportConstants.Seq;
        public const string Pkey = "status_report_region_pkey";
        public const string CommonKeyName = StatusReportConstants.CommonKeyName;
    }

    /// <summary>
    /// The names of High Level status report region table and sequences.
    /// </summary>
    internal static class HLStatusReportRegionConstants
    {
        public const string Table = "hl_status_report_region_BSIFlex335v2";
        public const string Pkey = "hl_status_report_region_pkey";
    }
}
