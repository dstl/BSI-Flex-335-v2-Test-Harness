// Project:           $Project: 00010855-Sapient$
// File:              $Workfile: DetectionConstants.cs$
// Crown-owned copyright, 2021-2024
//  See Release/Supply Conditions
// $NoKeywords$
namespace SapientDatabase.DatabaseTables
{
    /// <summary>
    /// Constants for detection report tables.
    /// </summary>
    public static class DetectionConstants
    {
        /// <summary>
        /// The DetectionReportClass class.
        /// </summary>
        public static class DetectionReportClass
        {
            public const string Table = "detection_report_class_BSIFlex335v2";
            public const string Seq = "detection_report_id_seq";
            public const string Pkey = "detection_report_class_pkey";
        }

        /// <summary>
        /// The DetectionReportAssocDetection class.
        /// </summary>
        public static class DetectionReportAssocDetection
        {
            public const string Table = "detection_report_assoc_detection_BSIFlex335v2";
            public const string DetectionIdSeq = DetectionReportClass.Seq;
            public const string Pkey = "detection_report_assoc_detection_pkey";
        }

        /// <summary>
        /// The DetectionReportAssocFile class.
        /// </summary>
        public static class DetectionReportAssocFile
        {
            public const string Table = "detection_report_assoc_file_BSIFlex335v2";
            public const string DetectionIdSeq = DetectionReportClass.Seq;
            public const string Pkey = "detection_report_assoc_file_pkey";
        }

        /// <summary>
        /// The DetectionReportBehaviour class.
        /// </summary>
        public static class DetectionReportBehaviour
        {
            public const string Table = "detection_report_behaviour_BSIFlex335v2";
            public const string DetectionIdSeq = DetectionReportClass.Seq;
            public const string Pkey = "detection_report_behaviour_pkey";
        }

        /// <summary>
        /// The DetectionReportLocation class.
        /// </summary>
        public static class DetectionReportLocation
        {
            public const string Table = "detection_report_location_BSIFlex335v2";
            public const string DetectionIdSeq = DetectionReportClass.Seq;
            public const string Pkey = "detection_report_location_pkey";
        }

        /// <summary>
        /// The DetectionReportRangeBearing class.
        /// </summary>
        public static class DetectionReportRangeBearing
        {
            public const string Table = "detection_report_range_bearing_BSIFlex335v2";
            public const string DetectionIdSeq = DetectionReportClass.Seq;
            public const string Pkey = "detection_report_range_bearing_pkey";
        }

        /// <summary>
        /// The DetectionReportSignal class.
        /// </summary>
        public static class DetectionReportSignal
        {
            public const string Table = "detection_report_signal_BSIFlex335v2";
            public const string DetectionIdSeq = DetectionReportClass.Seq;
            public const string Pkey = "detection_report_signal_pkey";
        }

        /// <summary>
        /// The DetectionReportSubclass class.
        /// </summary>
        public static class DetectionReportSubclass
        {
            public const string Table = "detection_report_subclass_BSIFlex335v2";
            public const string DetectionIdSeq = DetectionReportClass.Seq;
            public const string Pkey = "detection_report_subclass_pkey";
        }

        /// <summary>
        /// The DetectionReportTrackInfo class.
        /// </summary>
        public static class DetectionReportTrackInfo
        {
            public const string Table = "detection_report_track_info_BSIFlex335v2";
            public const string DetectionIdSeq = DetectionReportClass.Seq;
            public const string Pkey = "detection_report_track_info_pkey";
        }

        /// <summary>
        /// The HLDetectionReportClass class.
        /// </summary>
        public static class HLDetectionReportClass
        {
            public const string Table = "hl_detection_report_class_BSIFlex335v2";
            public const string Seq = "hl_detection_report_id_seq";
            public const string Pkey = "hl_detection_report_class_pkey";
        }

        /// <summary>
        /// The HLDetectionReportAssocDetection class.
        /// </summary>
        public static class HLDetectionReportAssocDetection
        {
            public const string Table = "hl_detection_report_assoc_detection_BSIFlex335v2";
            public const string DetectionIdSeq = HLDetectionReportClass.Seq;
            public const string Pkey = "hl_detection_report_assoc_detection_pkey";
        }

        /// <summary>
        /// The HLDetectionReportAssocFile class.
        /// </summary>
        public static class HLDetectionReportAssocFile
        {
            public const string Table = "hl_detection_report_assoc_file_BSIFlex335v2";
            public const string DetectionIdSeq = HLDetectionReportClass.Seq;
            public const string Pkey = "hl_detection_report_assoc_file_pkey";
        }

        /// <summary>
        /// The HLDetectionReportBehaviour class.
        /// </summary>
        public static class HLDetectionReportBehaviour
        {
            public const string Table = "hl_detection_report_behaviour_BSIFlex335v2";
            public const string DetectionIdSeq = HLDetectionReportClass.Seq;
            public const string Pkey = "hl_detection_report_behaviour_pkey";
        }

        /// <summary>
        /// The HLDetectionReportLocation class.
        /// </summary>
        public static class HLDetectionReportLocation
        {
            public const string Table = "hl_detection_report_location_BSIFlex335v2";
            public const string DetectionIdSeq = HLDetectionReportClass.Seq;
            public const string Pkey = "hl_detection_report_location_pkey";
        }

        /// <summary>
        /// The HLDetectionReportRangeBearing class.
        /// </summary>
        public static class HLDetectionReportRangeBearing
        {
            public const string Table = "hl_detection_report_range_bearing_BSIFlex335v2";
            public const string DetectionIdSeq = HLDetectionReportClass.Seq;
            public const string Pkey = "hl_detection_report_range_bearing_pkey";
        }

        /// <summary>
        /// The HLDetectionReportSignal class.
        /// </summary>
        public static class HLDetectionReportSignal
        {
            public const string Table = "hl_detection_report_signal_BSIFlex335v2";
            public const string DetectionIdSeq = HLDetectionReportClass.Seq;
            public const string Pkey = "hl_detection_report_signal_pkey";
        }

        /// <summary>
        /// The HLDetectionReportSubclass class.
        /// </summary>
        public static class HLDetectionReportSubclass
        {
            public const string Table = "hl_detection_report_subclass_BSIFlex335v2";
            public const string DetectionIdSeq = HLDetectionReportClass.Seq;
            public const string Pkey = "hl_detection_report_subclass_pkey";
        }

        /// <summary>
        /// The HLDetectionReportTrackInfo class.
        /// </summary>
        public static class HLDetectionReportTrackInfo
        {
            public const string Table = "hl_detection_report_track_info_BSIFlex335v2";
            public const string DetectionIdSeq = HLDetectionReportClass.Seq;
            public const string Pkey = "hl_detection_report_track_info_pkey";
        }
    }
}
