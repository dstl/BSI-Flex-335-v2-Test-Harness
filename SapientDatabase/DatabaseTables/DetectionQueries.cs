// Project:           $Project: 00010855-Sapient$
// File:              $Workfile: DetectionQueries.cs$
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
    /// Database methods relating to DetectionReports
    /// </summary>
    public static class DetectionQueries
    {
        /// <summary>
        /// Log4net logger.
        /// </summary>
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Test writing to detection to database table
        /// </summary>
        /// <param name="host">database server address</param>
        /// <param name="port">database port</param>
        /// <param name="user">database user name</param>
        /// <param name="password">database password</param>
        /// <param name="name">database name</param>
        public static void Test(string host, string port, string user, string password, string name)
        {
            object thisLock = new object();
            string db_server;
            string db_port;
            string db_user;
            string db_password;
            string db_name;
            NpgsqlConnection connection;
            db_server = host;
            db_port = port;
            db_user = user;
            db_password = password;
            db_name = name;
            connection = new NpgsqlConnection("Server=" + db_server + ";Port=" + db_port + ";User Id=" + db_user + ";Password=" + db_password + ";Database=" + db_name + ";");
            connection.Open();

            SapientMessage message = new SapientMessage
            {
                Timestamp = Timestamp.FromDateTime(DateTime.UtcNow),
                NodeId = "1",
                DetectionReport = new DetectionReport()
                {
                    ReportId = "1",
                    ObjectId = "1",
                    State = "Test",
                    Location = new Location()
                    {
                        X = -2.325,
                        Y = 52.101,
                    },
                }
            };
            var associatedFile = new Google.Protobuf.Collections.RepeatedField<AssociatedFile>()
            {
                new AssociatedFile
                {
                    Type = "image",
                    Url = "testfilename.jpg",
                },
            };

            message.DetectionReport.AssociatedFile.AddRange(associatedFile);

            string sqlString = GenerateInsertQueryString(message, false);
            Database.SendToDatabase(sqlString, connection, thisLock);
            message.NodeId = "0";
            sqlString = GenerateInsertQueryString(message, true);
            Database.SendToDatabase(sqlString, connection, thisLock);
        }

        /// <summary>
        /// Generate SQL string for inserting all data from this detection report into the database
        /// </summary>
        /// <param name="message">DetectionReport object</param>
        /// <param name="dmm">whether DMM or not</param>
        /// <returns>SQL string</returns>
        public static string GenerateInsertQueryString(SapientMessage message, bool dmm)
        {
            DateTime updateTime = DateTime.UtcNow;
            StringBuilder sb = new StringBuilder();
            string tablePrefix = "detection_report";

            if (dmm)
            {
                tablePrefix = "hl_detection_report";
            }

            string commonFieldString = GenerateCommonFieldString(message, updateTime);

            Velocity vel = GenerateVelocity(message);

            GenerateLocationInsertString(message, tablePrefix, commonFieldString, ref sb, vel);
            GenerateObjectInfoInsertString(message, tablePrefix, commonFieldString, ref sb);
            GenerateClassInsertString(message, tablePrefix, commonFieldString, ref sb);
            GenerateBehaviourInsertString(message, tablePrefix, commonFieldString, ref sb);
            GenerateAssociatedFileInsertString(message, tablePrefix, commonFieldString, ref sb);

            return sb.ToString();
        }

        /// <summary>
        /// Generate Velocity based on difference between next predicted location and current detected location
        /// </summary>
        /// <param name="message">detection report object</param>
        /// <returns>velocity object</returns>
        private static Velocity GenerateVelocity(SapientMessage message)
        {
            Velocity vel = new Velocity();

            if (message.DetectionReport.Location != null && message.DetectionReport.PredictionLocation != null && message.DetectionReport.PredictionLocation.Location != null)
            {
                vel.X = message.DetectionReport.PredictionLocation.Location.X - message.DetectionReport.Location.X;
                vel.Y = message.DetectionReport.PredictionLocation.Location.Y - message.DetectionReport.Location.Y;
                vel.Heading = 123;
                vel.Speed = 10.2;
                vel.Valid = true;
            }

            return vel;
        }

        /// <summary>
        /// Generate string of fields common to all detection tables
        /// </summary>
        /// <param name="message">DetectionReport object</param>
        /// <param name="updateTime">database update time - typically time now</param>
        /// <returns>string containing database fields common to all detection tables</returns>
        private static string GenerateCommonFieldString(SapientMessage message, DateTime updateTime)
        {
            string detectionTimeStamp = message.Timestamp.ToDateTime().ToString(DatabaseUtil.DateTimeFormat);
            string updateTimeStamp = updateTime.ToString(DatabaseUtil.DateTimeFormat);

            string sqlString = string.Format($@"
                                        '{message.NodeId}',
                                        '{detectionTimeStamp}',
                                        '{updateTimeStamp}',
                                        '{message.DetectionReport.ReportId}',
                                        '{message.DetectionReport.ObjectId}' ");
            return sqlString;
        }

        /// <summary>
        /// Generate SQL string for adding location and predicted location sections of DetectionReport to database
        /// </summary>
        /// <param name="message">DetectionReport object</param>
        /// <param name="tablePrefix">database table name prefix</param>
        /// <param name="commonFieldString">string of values common to all tables</param>
        /// <param name="sb">string builder to add SQL to</param>
        private static void GenerateLocationInsertString(SapientMessage message, string tablePrefix, string commonFieldString, ref StringBuilder sb, Velocity velocity)
        {
            bool predicted = false;
            string predictedTimestamp = "NULL";
            GenerateLocation(message, message.DetectionReport.Location, tablePrefix, commonFieldString, ref sb, predicted, predictedTimestamp, velocity);
            GenerateRangeBearing(message, message.DetectionReport.RangeBearing, tablePrefix, commonFieldString, ref sb, predicted, predictedTimestamp);

            if (message.DetectionReport.PredictionLocation != null)
            {
                predicted = true;
                predictedTimestamp = DatabaseUtil.Nullable(message.DetectionReport.PredictionLocation, pl => pl.PredictedTimestamp.ToDateTime());

                GenerateLocation(message, message.DetectionReport.PredictionLocation.Location, tablePrefix, commonFieldString, ref sb, predicted, predictedTimestamp, velocity);
                GenerateRangeBearing(message, message.DetectionReport.PredictionLocation.RangeBearing, tablePrefix, commonFieldString, ref sb, predicted, predictedTimestamp);
            }
        }

        /// <summary>
        /// Generate SQL string for adding cartesian location sections of DetectionReport to database
        /// </summary>
        /// <param name="message">DetectionReport object</param>
        /// <param name="loc">cartesian location object</param>
        /// <param name="tablePrefix">database table name prefix</param>
        /// <param name="commonFieldString">string of values common to all tables</param>
        /// <param name="sb">string builder to add SQL to</param>
        /// <param name="predicted">whether actual or predicted location</param>
        /// <param name="predictedTimestamp">predicted location timestamp</param>
        private static void GenerateLocation(SapientMessage message, Location loc, string tablePrefix, string commonFieldString, ref StringBuilder sb, bool predicted, string predictedTimestamp, Velocity velocity)
        {
            if (loc != null)
            {
                var t_id = message.DetectionReport.TaskId;
                var state = message.DetectionReport.State;
                var detection_confidence = message.DetectionReport.DetectionConfidence;
                var colour = message.DetectionReport.Colour;

                var x = loc.X.ToString("F7");
                var y = loc.Y.ToString("F7");
                var z = loc.Z;
                var dx = loc.XError;
                var dy = loc.YError;
                var dz = loc.ZError;

                // Zodiac fields 2020
                string affiliation = string.Empty;

                var detSensorId = message.NodeId;
                var vel_x = DatabaseUtil.Nullable(velocity.X, velocity.Valid);
                var vel_y = DatabaseUtil.Nullable(velocity.Y, velocity.Valid);
                var speed = DatabaseUtil.Nullable(velocity.Speed, velocity.Valid);
                var heading = DatabaseUtil.Nullable(velocity.Heading, velocity.Valid);

                string sql1 = string.Format($@"INSERT INTO {tablePrefix}_location_BSIFlex335v2 VALUES(
                    {commonFieldString},
                    '{t_id}',
                    '{state}',
                    {detection_confidence},
                    '{colour}',
                    {predicted},
                    {predictedTimestamp},
                    {x}, 
                    {y},
                    {z}, 
                    {dx},
                    {dy},
                    {dz},
                    '{affiliation}',
                    '{detSensorId}',
                    {vel_x},
                    {vel_y},
                    {speed},
                    {heading}
                    );");
                sb.Append(sql1);
            }
        }

        /// <summary>
        /// Generate SQL string for adding spherical location sections of DetectionReport to database
        /// </summary>
        /// <param name="message">DetectionReport object</param>
        /// <param name="rb">rangeBearing, spherical location object</param>
        /// <param name="tablePrefix">database table name prefix</param>
        /// <param name="commonFieldString">string of values common to all tables</param>
        /// <param name="sb">string builder to add SQL to</param>
        /// <param name="predicted">whether actual or predicted location</param>
        /// <param name="predictedTimestamp">predicted location timestamp</param>
        private static void GenerateRangeBearing(SapientMessage message, RangeBearing rb, string tablePrefix, string commonFieldString, ref StringBuilder sb, bool predicted, string predictedTimestamp)
        {
            if (rb != null)
            {
                var t_id = message.DetectionReport.TaskId;
                var state = message.DetectionReport.State;
                var detection_confidence = message.DetectionReport.DetectionConfidence;
                var colour = message.DetectionReport.Colour;

                var az = rb.Azimuth.ToString("F7");
                var r = rb.Range.ToString("F7");
                var ele = DatabaseUtil.Nullable(rb, r => r.Elevation); // rb.Elevation; // DatabaseUtil.Nullable(rb.Elevation, rb.EleSpecified);
                var z = "NULL"; // DatabaseUtil.Nullable(rb.Z, rb.ZSpecified);
                var daz = DatabaseUtil.Nullable(rb, r => r.AzimuthError); // DatabaseUtil.Nullable(rb.eAz, rb.eAzSpecified);
                var dr = DatabaseUtil.Nullable(rb, r => r.RangeError); // DatabaseUtil.Nullable(rb.eR, rb.eRSpecified);
                var dele = DatabaseUtil.Nullable(rb, r => r.ElevationError); // DatabaseUtil.Nullable(rb.eEle, rb.eEleSpecified);
                var dz = "NULL"; // DatabaseUtil.Nullable(rb.eZ, rb.eZSpecified);

                // Zodiac fields 2020
                var affiliation = string.Empty;

                var detSensorId = message.NodeId;
                string sql1 = string.Format($@"INSERT INTO {tablePrefix}_range_bearing_BSIFlex335v2 VALUES(
                    {commonFieldString},
                    '{t_id}',
                    '{state}',
                    {detection_confidence},
                    '{colour}',
                    {predicted},
                    {predictedTimestamp},
                    {r}, 
                    {az},
                    {ele},
                    {z},
                    {dr},
                    {daz},
                    {dele},
                    {dz},
                    '{affiliation}',
                    '{detSensorId}'
                    );");
                sb.Append(sql1);
            }
        }

        /// <summary>
        /// Generate SQL string for adding ObjectInfo and TrackInfo sections of DetectionReport to database
        /// </summary>
        /// <param name="message">DetectionReport object</param>
        /// <param name="tablePrefix">database table name prefix</param>
        /// <param name="commonFieldString">string of values common to all tables</param>
        /// <param name="sb">string builder to add SQL to</param>
        private static void GenerateObjectInfoInsertString(SapientMessage message, string tablePrefix, string commonFieldString, ref StringBuilder sb)
        {
            if (message.DetectionReport.TrackInfo != null)
            {
                foreach (var track_info in message.DetectionReport.TrackInfo)
                {
                    var e = track_info.Error;
                    string sql1 = string.Format($@"INSERT INTO {tablePrefix}_track_info_BSIFlex335v2 VALUES(
                        {commonFieldString},
                        'trackInfo',
                        '{track_info.Type}',
                        '{track_info.Value}',
                        {e} );");
                    sb.Append(sql1);
                }
            }

            if (message.DetectionReport.ObjectInfo != null)
            {
                foreach (var object_info in message.DetectionReport.ObjectInfo)
                {
                    var value = object_info.Value; //.ToString("F7");
                    var e = object_info.Error; // DatabaseUtil.Nullable(object_info.e, object_info.eSpecified);

                    string sql1 = string.Format($@"INSERT INTO {tablePrefix}_track_info_BSIFlex335v2 VALUES(
                        {commonFieldString},
                        'objectInfo',
                        '{object_info.Type}',
                        '{value}',
                        {e} );");
                    sb.Append(sql1);
                }
            }
        }

        /// <summary>
        /// Generate SQL string for adding class section of DetectionReport to database
        /// </summary>
        /// <param name="message">DetectionReport object</param>
        /// <param name="tablePrefix">database table name prefix</param>
        /// <param name="commonFieldString">string of values common to all tables</param>
        /// <param name="sb">string builder to add SQL to</param>
        private static void GenerateClassInsertString(SapientMessage message, string tablePrefix, string commonFieldString, ref StringBuilder sb)
        {
            if (message.DetectionReport.Classification != null)
            {
                foreach (var cl in message.DetectionReport.Classification)
                {
                    var confidence = cl.Confidence; // DatabaseUtil.Nullable(cl.Confidence, cl.confidenceSpecified);

                    string sql1 = string.Format($@"INSERT INTO {tablePrefix}_class_BSIFlex335v2 VALUES(
                        {commonFieldString},
                        '{cl.Type}',
                        {confidence} );");

                    sb.Append(sql1);

                    StringBuilder subclassSb = new StringBuilder();
                    if (cl.SubClass != null)
                    {
                        int parentSubClassId = 0;
                        int nextSubClassId = 1;
                        foreach (var subclass in cl.SubClass)
                        {
                            nextSubClassId = AddSubclass(commonFieldString, cl, subclass, parentSubClassId, nextSubClassId, tablePrefix, ref subclassSb);
                        }
                    }

                    sb.Append(subclassSb.ToString()); // append sub class sql to main sql
                }
            }
        }

        /// <summary>
        /// Generate SQL string for adding behaviour section of DetectionReport to database
        /// </summary>
        /// <param name="message">DetectionReport object</param>
        /// <param name="tablePrefix">database table name prefix</param>
        /// <param name="commonFieldString">string of values common to all tables</param>
        /// <param name="sb">string builder to add SQL to</param>
        private static void GenerateBehaviourInsertString(SapientMessage message, string tablePrefix, string commonFieldString, ref StringBuilder sb)
        {
            if (message.DetectionReport.Behaviour != null)
            {
                foreach (var behaviour in message.DetectionReport.Behaviour)
                {
                    var confidence = behaviour.Confidence; // DatabaseUtil.Nullable(behaviour.Confidence, behaviour.confidenceSpecified);

                    string sql1 = string.Format($@"INSERT INTO {tablePrefix}_behaviour_BSIFlex335v2 VALUES(
                        {commonFieldString},
                        '{behaviour.Type}',
                        {confidence} );");

                    sb.Append(sql1);
                }
            }
        }

        /// <summary>
        /// Generate SQL string for adding associated file section of DetectionReport to database
        /// </summary>
        /// <param name="message">DetectionReport object</param>
        /// <param name="tablePrefix">database table name prefix</param>
        /// <param name="commonFieldString">string of values common to all tables</param>
        /// <param name="sb">string builder to add SQL to</param>
        private static void GenerateAssociatedFileInsertString(SapientMessage message, string tablePrefix, string commonFieldString, ref StringBuilder sb)
        {
            if (message.DetectionReport.AssociatedFile != null)
            {
                foreach (var aft in message.DetectionReport.AssociatedFile)
                {
                    string sql1 = string.Format($@"INSERT INTO {tablePrefix}_assoc_file_BSIFlex335v2 VALUES(
                        {commonFieldString},
                        '{aft.Type}', 
                        '{aft.Url}' );");

                    sb.Append(sql1);
                }
            }
        }

        /// <summary>
        /// Generate SQL string for adding sub class section of DetectionReport to database
        /// </summary>
        /// <param name="commonFieldString">string of values common to all tables</param>
        /// <param name="cl">parent class object</param>
        /// <param name="subclass">sub class to add</param>
        /// <param name="parentSubClassId">identifier of parent class/sub class</param>
        /// <param name="nextSubClassId">identifier of this sub class object</param>
        /// <param name="tablePrefix">database table name prefix</param>
        /// <param name="sb">string builder to add SQL to</param>
        /// <returns>next sub class identifier to use</returns>
        private static int AddSubclass(string commonFieldString, DetectionReport.Types.DetectionReportClassification cl, DetectionReport.Types.SubClass subclass, int parentSubClassId, int nextSubClassId, string tablePrefix, ref StringBuilder sb)
        {
            int thisSubClassId = nextSubClassId;
            nextSubClassId++;

            var level = subclass.Level; // DatabaseUtil.Nullable(subclass.Level, subclass.levelSpecified);
            var confidence = subclass.Confidence; // DatabaseUtil.Nullable(subclass.confidence, subclass.confidenceSpecified);

            string sql1 = string.Format($@"INSERT INTO {tablePrefix}_subclass_BSIFlex335v2 VALUES(
                {commonFieldString},
                '{cl.Type}',
                {level},
                '{subclass.Type}', 
                '{subclass.Level}',
                {confidence}, 
                {thisSubClassId}, 
                {parentSubClassId} );");

            sb.Append(sql1);

            if (subclass.SubClass_ != null)
            {
                foreach (var sub in subclass.SubClass_)
                {
                    nextSubClassId = AddSubclass(commonFieldString, cl, sub, thisSubClassId, nextSubClassId, tablePrefix, ref sb);
                }
            }

            return nextSubClassId;
        }

        /// <summary>
        /// The velocity.
        /// </summary>
        private class Velocity
        {
            public double X { get; set; }

            public double Y { get; set; }

            public double Speed { get; set; }

            public double Heading { get; set; }

            public bool Valid { get; set; }
        }
    }
}