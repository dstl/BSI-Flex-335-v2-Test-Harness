// Project:           $Project: 00010855-Sapient$
// File:              $Workfile: AlertConstants.cs$
// Crown-owned copyright, 2021-2024
//  See Release/Supply Conditions
// $NoKeywords$

namespace SapientDatabase.DatabaseTables
{
    /// <summary>
    /// The names of alert tables and sequences.
    /// </summary>
    public static class AlertConstants
    {
        /// <summary>
        /// Primary ASM Alert Table
        /// </summary>
        public static class Alert
        {
            public const string Table = "alert_BSIFlex335v2";
            public const string Seq = "alert_id_seq";
            public const string Pkey = "alert_pkey";
        }

        /// <summary>
        /// ASM Alert Associated Detection Table
        /// </summary>
        public static class AlertAssocDetection
        {
            public const string Table = "alert_assoc_detection_BSIFlex335v2";
            public const string KeyIdSeq = Alert.Seq;
            public const string Pkey = "alert_assoc_detection_pkey";
        }

        /// <summary>
        /// ASM Alert Associated File Table
        /// </summary>
        public static class AlertAssocFile
        {
            public const string Table = "alert_assoc_file_BSIFlex335v2";
            public const string KeyIdSeq = Alert.Seq;
            public const string Pkey = "alert_assoc_file_pkey";
        }

        /// <summary>
        /// ASM Alert Cartesian Location Table
        /// </summary>
        public static class AlertLocation
        {
            public const string Table = "alert_location_BSIFlex335v2";
            public const string KeyIdSeq = Alert.Seq;
            public const string Pkey = "alert_location_pkey";
        }

        /// <summary>
        /// ASM Alert Spherical Location Table
        /// </summary>
        public static class AlertRangeBearing
        {
            public const string Table = "alert_range_bearing_BSIFlex335v2";
            public const string Seq = Alert.Seq;
            public const string Pkey = "alert_range_bearing_pkey";
        }

        /// <summary>
        /// Primary DMM Alert Table
        /// </summary>
        public static class HLAlert
        {
            public const string Table = "hl_alert_BSIFlex335v2";
            public const string Seq = "hl_alert_id_seq";
            public const string Pkey = "hl_alert_pkey";
        }

        /// <summary>
        /// DMM Alert Associated Detection Table
        /// </summary>
        public static class HLAlertAssocDetection
        {
            public const string Table = "hl_alert_assoc_detection_BSIFlex335v2";
            public const string KeyIdSeq = HLAlert.Seq;
            public const string Pkey = "hl_alert_assoc_detection_pkey";
        }

        /// <summary>
        /// DMM Alert Associated File Table
        /// </summary>
        public static class HLAlertAssocFile
        {
            public const string Table = "hl_alert_assoc_file_BSIFlex335v2";
            public const string KeyIdSeq = HLAlert.Seq;
            public const string Pkey = "hl_alert_assoc_file_pkey";
        }

        /// <summary>
        /// DMM Alert Cartesian Location Table
        /// </summary>
        public static class HLAlertLocation
        {
            public const string Table = "hl_alert_location_BSIFlex335v2";
            public const string KeyIdSeq = HLAlert.Seq;
            public const string Pkey = "hl_alert_location_pkey";
        }

        /// <summary>
        /// DMM Alert Spherical Location Table
        /// </summary>
        public static class HLAlertRangeBearing
        {
            public const string Table = "hl_alert_range_bearing_BSIFlex335v2";
            public const string Seq = HLAlert.Seq;
            public const string Pkey = "hl_alert_range_bearing_pkey";
        }
    }
}
