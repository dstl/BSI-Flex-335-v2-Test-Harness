// Project:           $Project: 00010855-Sapient$
// File:              $Workfile: StatusReportInsertQueries.cs$
// Crown-owned copyright, 2021-2024
//  See Release/Supply Conditions
// $NoKeywords$
namespace SapientDatabase.DatabaseTables
{
    using Google.Protobuf.WellKnownTypes;
    using log4net;
    using Npgsql;
    using Sapient.Common;
    using Sapient.Data;
    using System;
    using System.Reflection;
    using System.Text;

    /// <summary>
    /// Database methods relating to Status Report messages
    /// </summary>
    public class StatusReportInsertQueries
    {
        /// <summary>
        /// Log4net logger.
        /// </summary>
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Method to Populate Status Report Tables
        /// </summary>
        /// <param name="message">status report object</param>
        /// <returns>SQL string</returns>
        public static string GenerateInsertQueryString(SapientMessage message)
        {
            try
            {

                DateTime updateTime = DateTime.UtcNow;
                string commonFieldString = GenerateCommonFieldString(message, updateTime);
                StringBuilder sb = new StringBuilder();

                var report = message.StatusReport;

                GenerateTopLevelStatus(report, commonFieldString, ref sb);
                GenerateStatusMessages(report, commonFieldString, ref sb);
                GenerateRangeBearingRegions(report, commonFieldString, ref sb);
                GenerateCartesianRegions(report, commonFieldString, ref sb);

                return sb.ToString();
            }
            catch (Exception ex)
            {
                return string.Empty;
                throw ex;
            }
        }

        /// <summary>
        /// Method to Populate HL Status Report Tables
        /// </summary>
        /// <param name="message">status report object</param>
        /// <returns>SQL string</returns>
        public static string GenerateHLInsertQueryString(SapientMessage message)
        {
            DateTime updateTime = DateTime.UtcNow;
            StringBuilder sb = new StringBuilder();

            // status regions for DMM
            /*if (message.StatusReport.StatusRegion != null)
            {
                foreach (var region in message.StatusReport.StatusRegion)
                {
                    if (region.Region_.LocationList != null)
                    {
                        string sql = InitStatusReportHLLocationList(message, region.Region_.LocationList, region.RegionType, region.RegionId, updateTime, region.RegionName, region.RegionStatus, region.Description);
                        sb.Append(sql);
                    }
                }
            }*/

            return sb.ToString();
        }

        /// <summary>
        /// method to Initialize Status Report Location List
        /// </summary>
        /// <param name="report">report</param>
        /// <param name="location_list">location list</param>
        /// <param name="type">type</param>
        /// <param name="regionId">region Identifier</param>
        /// <param name="updateTime">time now of last update of record</param>
        private static string InitStatusReportHLLocationList(SapientMessage report, LocationList location_list, string type, string regionId, DateTime updateTime, string regionName, string regionStatus, string description)
        {
            string sql = string.Empty;

            if (location_list.Locations != null)
            {
                StringBuilder polygon_builder = new StringBuilder();
                polygon_builder.Append("( ");

                foreach (var location in location_list.Locations)
                {
                    polygon_builder.Append(string.Format("( {0:F7}, {1:F7} ), ", location.X, location.Y));
                }

                polygon_builder.Append(string.Format("( {0:F7}, {1:F7} ) )", location_list.Locations[0].X, location_list.Locations[0].Y));

                StringBuilder e_polygon_builder = polygon_builder; // for now just copy main polygon for error

                const string tableName = HLStatusReportRegionConstants.Table;
                string commonFieldString = GenerateCommonFieldString(report, updateTime);
                sql = string.Format($@"INSERT INTO {tableName} VALUES(
                            {commonFieldString},
                            '{type}',
                            '{polygon_builder}',
                            '{regionId}',
                            '{e_polygon_builder}',
                            '{regionName}',
                            '{regionStatus}',
                            '{description}'
                            );");
            }

            return sql;
        }

        /// <summary>
        /// Generate SQL string for adding top level status fields of StatusReport to database
        /// </summary>
        /// <param name="report">status report object</param>
        /// <param name="commonFieldString">string of values common to all tables</param>
        /// <param name="sb">string builder to add SQL to</param>
        private static void GenerateTopLevelStatus(StatusReport report, string commonFieldString, ref StringBuilder sb)
        {
            string sql = string.Empty;
            const string mainTableName = StatusReportConstants.Table;

            var power_source = DatabaseUtil.Nullable(report.Power, ptr => ptr.Source.ToString());
            var power_status = DatabaseUtil.Nullable(report.Power, ptr => ptr.Status.ToString());
            var power_level = DatabaseUtil.Nullable(report.Power, ptr => ptr.Level);

            if (report.NodeLocation != null) 
            {
                var loc = report.NodeLocation; 
                var x = loc.X.ToString("F7");
                var y = loc.Y.ToString("F7");
                var z = loc.Z;
                var dx = loc.XError;
                var dy = loc.YError;
                var dz = loc.ZError;

                sql = string.Format($@"INSERT INTO {mainTableName} VALUES(
                            {commonFieldString},
                            '{report.System}',
                            '{report.Info}',
                            '{report.ActiveTaskId}',
                            '{report.Mode}',
                            {power_source},
                            {power_status},
                            {power_level},
                            {x}, 
                            {y},
                            {z}, 
                            {dx},
                            {dy},
                            {dz}
                            );");
            }
            else
            {
                sql = string.Format($@"INSERT INTO {mainTableName} VALUES(
                            {commonFieldString},
                            '{report.System}',
                            '{report.Info}',
                            '{report.ActiveTaskId}',
                            '{report.Mode}',
                            {power_source},
                            {power_status},
                            {power_level}
                            );");
            }

            sb.Append(sql);
        }

        /// <summary>
        /// Generate SQL string for adding status message fields of StatusReport to database
        /// </summary>
        /// <param name="report">status report object</param>
        /// <param name="commonFieldString">string of values common to all tables</param>
        /// <param name="sb">string builder to add SQL to</param>
        private static void GenerateStatusMessages(StatusReport report, string commonFieldString, ref StringBuilder sb)
        {
            if (report.Status != null)
            {
                foreach (var status in report.Status)
                {
                    const string tableName = StatusReportMessagesConstants.Table;
                    string sql = string.Format($@"INSERT INTO {tableName} VALUES(
                            {commonFieldString},
                            '{status.StatusLevel}',
                            '{status.StatusType}',
                            '{status.StatusValue}'
                            );");

                    sb.Append(sql);
                }
            }
        }

        /// <summary>
        /// Generate SQL string for adding range bearing regions, coverages and field of views of StatusReport to database
        /// </summary>
        /// <param name="report">status report object</param>
        /// <param name="commonFieldString">string of values common to all tables</param>
        /// <param name="sb">string builder to add SQL to</param>
        private static void GenerateRangeBearingRegions(StatusReport report, string commonFieldString, ref StringBuilder sb)
        {
            if (report.FieldOfView != null && report.FieldOfView.RangeBearing != null)
            {
                string sql = InitStatusReportRangeBearingCone(report, report.FieldOfView.RangeBearing, "FieldOfView", commonFieldString);
                sb.Append(sql);
            }

            if ((report.Coverage != null) 
                && (report.Coverage.Count > 0) 
                && (report.Coverage[0] != null)
                && (report.Coverage[0].FovOneofCase == LocationOrRangeBearing.FovOneofOneofCase.RangeBearing))
            {
                string sql = InitStatusReportRangeBearingCone(report, report.Coverage[0].RangeBearing, "Coverage", commonFieldString);
                sb.Append(sql);
            }

            if (report.Obscuration != null)
            {
                foreach (var obscuration in report.Obscuration)
                {
                    if (obscuration.RangeBearing != null)
                    {
                        string sql = InitStatusReportRangeBearingCone(report, obscuration.RangeBearing, "Obscuration", commonFieldString);
                        sb.Append(sql);
                    }
                }
            }
        }

        /// <summary>
        /// Generate SQL string for adding cartesian regions, coverages and field of views of StatusReport to database
        /// </summary>
        /// <param name="report">status report object</param>
        /// <param name="commonFieldString">string of values common to all tables</param>
        /// <param name="sb">string builder to add SQL to</param>
        private static void GenerateCartesianRegions(StatusReport report, string commonFieldString, ref StringBuilder sb)
        {
            int regionID = 1;
            if (report.FieldOfView != null && report.FieldOfView.LocationList != null)
            {
                string sql = InitStatusReportLocationList(report, report.FieldOfView.LocationList, "FieldOfView", regionID++, commonFieldString);
                sb.Append(sql);
            }

            if ((report.Coverage != null) 
                && (report.Coverage.Count > 0)
                && (report.Coverage[0].FovOneofCase == LocationOrRangeBearing.FovOneofOneofCase.LocationList))
            {
                string sql = InitStatusReportLocationList(report, report.Coverage[0].LocationList, "Coverage", regionID++, commonFieldString);
                sb.Append(sql);
            }

            if (report.Obscuration != null)
            {
                foreach (var obscuration in report.Obscuration)
                {
                    if (obscuration.LocationList != null)
                    {
                        string sql = InitStatusReportLocationList(report, obscuration.LocationList, "Obscuration", regionID++, commonFieldString);
                        sb.Append(sql);
                    }
                }
            }

            GenerateStatusRegions(report, commonFieldString, ref sb, ref regionID);
        }

        /// <summary>
        /// Generate SQL string for adding miscellaneous status regions of StatusReport to database
        /// </summary>
        /// <param name="report">status report object</param>
        /// <param name="commonFieldString">string of values common to all tables</param>
        /// <param name="sb">string builder to add SQL to</param>
        /// <param name="regionID">next region ID to use</param>
        private static void GenerateStatusRegions(StatusReport report, string commonFieldString, ref StringBuilder sb, ref int regionID)
        {
            /*if (report.StatusRegion != null)
            {
                foreach (var region in report.StatusRegion)
                {
                    if (region.Region_.LocationList != null)
                    {
                        string sql = InitStatusReportLocationList(report, region.Region_.LocationList, region.RegionType, regionID++, commonFieldString);
                        sb.Append(sql);
                    }
                }
            }*/
        }

        /// <summary>
        /// method to Initialize Status Report Location List
        /// </summary>
        /// <param name="report">report</param>
        /// <param name="location_list">location list</param>
        /// <param name="type">type</param>
        /// <param name="index">index</param>
        /// <param name="commonFieldString">string of values common to all tables</param>
        private static string InitStatusReportLocationList(StatusReport report, LocationList location_list, string type, int index, string commonFieldString)
        {
            string sql = string.Empty;
            if (location_list.Locations != null)
            {
                StringBuilder polygon_builder = new StringBuilder();
                polygon_builder.Append("( ");

                foreach (var location in location_list.Locations)
                {
                    polygon_builder.Append(string.Format("( {0:F7}, {1:F7} ), ", location.X, location.Y));
                }

                polygon_builder.Append(string.Format("( {0:F7}, {1:F7} ) )", location_list.Locations[0].X, location_list.Locations[0].Y));

                const string tableName = StatusReportRegionConstants.Table;
                sql = string.Format($@"INSERT INTO {tableName} VALUES(
                            {commonFieldString},
                            '{type}',
                            '{polygon_builder}',
                            {index}
                            );");
            }

            return sql;
        }

        /// <summary>
        /// method to Initialize Status Report Range Bearing Cone
        /// </summary>
        /// <param name="report">report</param>
        /// <param name="range_bearing_cone">range bearing cone</param>
        /// <param name="type">type</param>
        /// <param name="updateTime">time now of last update of record</param>
        private static string InitStatusReportRangeBearingCone(StatusReport report, RangeBearingCone range_bearing_cone, string type, string commonFieldString)
        {
            var r = range_bearing_cone.Range.ToString("F7");
            var az = range_bearing_cone.Azimuth.ToString("F7");
            var ele = range_bearing_cone.Elevation;

            var vExtent = range_bearing_cone.VerticalExtent;

            var dr = range_bearing_cone.RangeError;
            var daz = range_bearing_cone.AzimuthError;
            var dele = range_bearing_cone.ElevationError;

            var ehExtent = range_bearing_cone.HorizontalExtentError;
            var evExtent = range_bearing_cone.VerticalExtentError;

            const string tableName = StatusReportRangeBearingConstants.Table;
            string sql = string.Format($@"INSERT INTO {tableName} VALUES(
                            {commonFieldString},
                            '{type}',
                            {r}, 
                            {az},
                            {ele},
                            {range_bearing_cone.HorizontalExtent},
                            {vExtent},
                            {dr},
                            {daz},
                            {dele},
                            {ehExtent},
                            {evExtent}
                            );");
            return sql;
        }

        /// <summary>
        /// Generate string of fields common to all status report tables
        /// </summary>
        /// <param name="report">StatusReport object</param>
        /// <param name="updateTime">database update time - typically time now</param>
        /// <returns>string containing database fields common to all detection tables</returns>
        private static string GenerateCommonFieldString(SapientMessage report, DateTime updateTime)
        {
            string messageTimeStamp = report.Timestamp.ToDateTime().ToString(DatabaseUtil.DateTimeFormat);
            string updateTimeStamp = updateTime.ToString(DatabaseUtil.DateTimeFormat);

            string sqlString = string.Format($@"
                                        '{report.NodeId}',
                                        '{messageTimeStamp}',
                                        '{updateTimeStamp}',
                                        '{report.StatusReport.ReportId}' ");
            return sqlString;
        }

        /// <summary>
        /// Generates the coverage.
        /// </summary>
        /// <param name="sensorX">The sensor x.</param>
        /// <param name="sensorY">The sensor y.</param>
        /// <returns>A LocationList.</returns>
        private static LocationList GenerateCoverage(double sensorX, double sensorY)
        {
            const double CoverageX = 30;
            const double CoverageY = 30;

            var locations = new Google.Protobuf.Collections.RepeatedField<Location>
                {
                    new Location { X = sensorX - CoverageX, Y = sensorY - CoverageY },
                    new Location { X = sensorX - CoverageX, Y = sensorY + CoverageY },
                    new Location { X = sensorX + CoverageX, Y = sensorY + CoverageY },
                    new Location { X = sensorX + CoverageX, Y = sensorY - CoverageY },
                    new Location { X = sensorX - CoverageX, Y = sensorY - CoverageY },
                };

            LocationList sensorCoverage = new LocationList();

            sensorCoverage.Locations.AddRange(locations);

            return sensorCoverage;
        }

        /// <summary>
        /// Return a field of cone of the current range, azimuth and elevation
        /// </summary>
        /// <returns>populated rangeBearingCone object</returns>
        private static RangeBearingCone GetFieldOfViewCone()
        {
            RangeBearingCone cone = new RangeBearingCone();
            cone.Azimuth = 127;
            cone.Range = 100;
            cone.HorizontalExtent = 30;
            cone.Elevation = 5;
            return cone;
        }
    }
}
