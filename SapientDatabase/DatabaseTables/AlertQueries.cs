// Project:           $Project: 00010855-Sapient$
// File:              $Workfile: AlertQueries.cs$
// Crown-owned copyright, 2021-2024
//  See Release/Supply Conditions
// $NoKeywords$
namespace SapientDatabase.DatabaseTables
{
    using Google.Protobuf.WellKnownTypes;
    using log4net;
    using Npgsql;
    using Sapient.Data;
    using System;
    using System.Reflection;
    using System.Text;

    /// <summary>
    /// Database methods relating to Alerts
    /// </summary>
    public static class AlertQueries
    {
        /// <summary>
        /// Log4net logger.
        /// </summary>
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Test writing to alert database table
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

            double sensorX = -2.325;
            double sensorY = 52.101;

            SapientMessage message = new SapientMessage
            {
                Timestamp = Timestamp.FromDateTime(DateTime.UtcNow),
                NodeId = "1",
                Alert = new Alert
                {
                    AlertId = "1",
                    Description = "Test Alert",
                    AlertType = Alert.Types.AlertType.Warning,
                    Location = new Sapient.Common.Location
                    {
                        X = sensorX,
                        Y = sensorY,
                    },
                    /*Position = new Alert.Types.LocationOrRangeBearing
                    {
                        Location = new Sapient.Common.Location
                        {
                            X = sensorX,
                            Y = sensorY,
                        },
                    },*/
                    Priority = Alert.Types.DiscretePriority.High,
                    Ranking = 0.9F,
                }
            };

            var associatedDetection = new Google.Protobuf.Collections.RepeatedField<Sapient.Common.AssociatedDetection>
            {
                new Sapient.Common.AssociatedDetection
                {
                    NodeId = "1",
                    ObjectId = "123",
                    Timestamp = message.Timestamp,
                    /*Description = "Test Location description",
                    Location = new Sapient.Common.Location
                    {
                        X = sensorX + 0.001,
                        Y = sensorY,
                    },*/
                },
            };

            message.Alert.AssociatedDetection.AddRange(associatedDetection);

            var assocatedFile = new Google.Protobuf.Collections.RepeatedField<Sapient.Common.AssociatedFile>
            {
                new Sapient.Common.AssociatedFile
                {
                    Type = "Image",
                    Url = "TestFilename.jpg",
                },
            };

            message.Alert.AssociatedFile.AddRange(assocatedFile);

            string sqlString = GenerateInsertQueryString(message, false);
            Database.SendToDatabase(sqlString, connection, thisLock);
            message.NodeId = "0";
            sqlString = GenerateInsertQueryString(message, true);
            Database.SendToDatabase(sqlString, connection, thisLock);
        }

        /// <summary>
        /// method to Populate Alert Tables
        /// </summary>
        /// <param name="message">alert object</param>
        /// <param name="dmm">whether high level message</param>
        /// <returns>SQL string</returns>
        public static string GenerateInsertQueryString(SapientMessage message, bool dmm)
        {
            DateTime updateTime = DateTime.UtcNow;
            StringBuilder sb = new StringBuilder();

            if (dmm)
            {
                sb.Append(GenerateAlertInsertQueryString(message, AlertConstants.HLAlert.Table, updateTime));
                sb.Append(GenerateAlertLocationInsertQueryString(message, AlertConstants.HLAlertLocation.Table, updateTime));

                // No range bearing data from DMM
                GenerateAlertAssociatedFileInsertQueryString(message, AlertConstants.HLAlertAssocFile.Table, updateTime, ref sb);
                GenerateAlertAssociatedDetectionInsertQueryString(message, AlertConstants.HLAlertAssocDetection.Table, updateTime, ref sb);
                GenerateAlertAssociatedDetectionLocationInsertQueryString(message, AlertConstants.HLAlertLocation.Table, updateTime, ref sb);
            }
            else
            {
                sb.Append(GenerateAlertInsertQueryString(message, AlertConstants.Alert.Table, updateTime));
                sb.Append(GenerateAlertLocationInsertQueryString(message, AlertConstants.AlertLocation.Table, updateTime));
                sb.Append(GenerateAlertRangeBearingInsertQueryString(message, AlertConstants.AlertRangeBearing.Table, updateTime));
                GenerateAlertAssociatedFileInsertQueryString(message, AlertConstants.AlertAssocFile.Table, updateTime, ref sb);
                GenerateAlertAssociatedDetectionInsertQueryString(message, AlertConstants.AlertAssocDetection.Table, updateTime, ref sb);
                GenerateAlertAssociatedDetectionLocationInsertQueryString(message, AlertConstants.AlertLocation.Table, updateTime, ref sb);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Generate SQL string for inserting all data from this aletr message into the database
        /// </summary>
        /// <param name="message">alert object</param>
        /// <param name="tableName">database table name</param>
        /// <param name="updateTime">dtaabase update time</param>
        /// <returns>SQL string</returns>
        private static string GenerateAlertInsertQueryString(SapientMessage message, string tableName, DateTime updateTime)
        {
            string alertTimeStamp = message.Timestamp.ToDateTime().ToString(DatabaseUtil.DateTimeFormat);
            string updateTimeStamp = updateTime.ToString(DatabaseUtil.DateTimeFormat);
            string regionId = DatabaseUtil.Nullable(message.Alert, al => al.RegionId);

            string additionalFields = string.Empty;

            if ((message.Alert.Priority != null) && message.Alert.Ranking != null)
            {
                // added extra fields for sequence alerts for SAPIENT Phase 4 May 16
                additionalFields = string.Format($@",
                    '{message.Alert.Priority}',
                    {message.Alert.Ranking},
                    {regionId},
                    {message.Alert.Confidence},
                    '{message.Alert.Description}'
                    ");
            }

            string sqlString = string.Format($@"INSERT INTO  {tableName} VALUES(
                    '{message.NodeId}',
                    '{alertTimeStamp}',
                    '{updateTimeStamp}',
                    '{message.Alert.AlertId}',
                    '{message.Alert.AlertType}',
                    '{message.Alert.Status}',
                    '{message.Alert.Description}'
                    {additionalFields}
                    );");

            return sqlString;
        }

        /// <summary>
        /// Generates the alert location insert query string.
        /// </summary>
        /// <param name="message">The alert.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="updateTime">The update time.</param>
        /// <returns></returns>
        private static string GenerateAlertLocationInsertQueryString(SapientMessage message, string tableName, DateTime updateTime)
        {
            string sqlString = string.Empty;

            if (message.Alert?.Location != null)
            {
                string alertTimeStamp = message.Timestamp.ToDateTime().ToString(DatabaseUtil.DateTimeFormat);
                string updateTimeStamp = updateTime.ToString(DatabaseUtil.DateTimeFormat);

                var x = message.Alert.Location.X.ToString("F7");
                var y = message.Alert.Location.Y.ToString("F7");
                var z = message.Alert.Location.Z; //DatabaseUtil.Nullable(message.location.Z, message.location.ZSpecified);
                var dx = message.Alert.Location.XError; // DatabaseUtil.Nullable(message.location.eX, message.location.eXSpecified);
                var dy = message.Alert.Location.YError; //DatabaseUtil.Nullable(message.location.eY, message.location.eYSpecified);
                var dz = message.Alert.Location.ZError; //DatabaseUtil.Nullable(message.location.eZ, message.location.eZSpecified);

                sqlString = string.Format($@"INSERT INTO  {tableName} VALUES(
                    '{message.NodeId}',
                    '{alertTimeStamp}',
                    '{updateTimeStamp}',
                    '{message.Alert.AlertId}',
                    {x}, 
                    {y},
                    {z}, 
                    {dx},
                    {dy},
                    {dz}
                    );");
            }

            return sqlString;
        }

        /// <summary>
        /// Generates the alert range bearing insert query string.
        /// </summary>
        /// <param name="message">The alert.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="updateTime">The update time.</param>
        /// <returns></returns>
        private static string GenerateAlertRangeBearingInsertQueryString(SapientMessage message, string tableName, DateTime updateTime)
        {
            string sqlString = string.Empty;

            if (message.Alert?.RangeBearing != null)
            {
                string alertTimeStamp = message.Timestamp.ToDateTime().ToString(DatabaseUtil.DateTimeFormat);
                string updateTimeStamp = updateTime.ToString(DatabaseUtil.DateTimeFormat);

                var az = message.Alert.RangeBearing.Azimuth;
                var r = message.Alert.RangeBearing.Range;
                var ele = DatabaseUtil.Nullable(message.Alert.RangeBearing, rb => rb.Elevation);
                var z = "NULL";
                var daz = DatabaseUtil.Nullable(message.Alert.RangeBearing, rb => rb.AzimuthError);
                var dr = DatabaseUtil.Nullable(message.Alert.RangeBearing, rb => rb.RangeError);
                var dele = DatabaseUtil.Nullable(message.Alert.RangeBearing, rb => rb.ElevationError);
                var dz = "NULL";

                sqlString = string.Format($@"INSERT INTO  {tableName} VALUES(
                    '{message.NodeId}',
                    '{alertTimeStamp}',
                    '{updateTimeStamp}',
                    '{message.Alert.AlertId}',
                    {r}, 
                    {az},
                    {ele},
                    {z},
                    {dr},
                    {daz},
                    {dele},
                    {dz}
                    );");
            }

            return sqlString;
        }

        /// <summary>
        /// Generates the alert associated file insert query string.
        /// </summary>
        /// <param name="message">The alert.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="updateTime">The update time.</param>
        /// <param name="sb">The sb.</param>
        private static void GenerateAlertAssociatedFileInsertQueryString(SapientMessage message, string tableName, DateTime updateTime, ref StringBuilder sb)
        {
            if (message.Alert.AssociatedFile != null)
            {
                string alertTimeStamp = message.Timestamp.ToDateTime().ToString(DatabaseUtil.DateTimeFormat);
                string updateTimeStamp = updateTime.ToString(DatabaseUtil.DateTimeFormat);

                foreach (var assocFile in message.Alert.AssociatedFile)
                {
                    sb.AppendFormat($@"INSERT INTO  {tableName} VALUES(
                    '{message.NodeId}',
                    '{alertTimeStamp}',
                    '{updateTimeStamp}',
                    '{message.Alert.AlertId}',
                    '{assocFile.Type}',
                    '{assocFile.Url}'
                    );");
                }
            }
        }

        /// <summary>
        /// Generates the alert associated detection insert query string.
        /// </summary>
        /// <param name="message">The alert.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="updateTime">The update time.</param>
        /// <param name="sb">The sb.</param>
        private static void GenerateAlertAssociatedDetectionInsertQueryString(SapientMessage message, string tableName, DateTime updateTime, ref StringBuilder sb)
        {
            if (message.Alert.AssociatedDetection != null)
            {
                string alertTimeStamp = message.Timestamp.ToDateTime().ToString(DatabaseUtil.DateTimeFormat);
                string updateTimeStamp = updateTime.ToString(DatabaseUtil.DateTimeFormat);

                foreach (var assocDetection in message.Alert.AssociatedDetection)
                {
                    sb.AppendFormat($@"INSERT INTO  {tableName} VALUES(
                    '{message.NodeId}',
                    '{alertTimeStamp}',
                    '{updateTimeStamp}',
                    '{message.Alert.AlertId}',
                    '{assocDetection.ObjectId}'
                    );");
                }
            }
        }

        /// <summary>
        /// Generates the alert associated detection location insert query string.
        /// </summary>
        /// <param name="message">The alert.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="updateTime">The update time.</param>
        /// <param name="sb">The sb.</param>
        private static void GenerateAlertAssociatedDetectionLocationInsertQueryString(SapientMessage message, string tableName, DateTime updateTime, ref StringBuilder sb)
        {
            if (message.Alert.AssociatedDetection != null)
            {
                string alertTimeStamp = message.Timestamp.ToDateTime().ToString(DatabaseUtil.DateTimeFormat);
                string updateTimeStamp = updateTime.ToString(DatabaseUtil.DateTimeFormat);

                /*foreach (var assocDetection in message.Alert.AssociatedDetection)
                {
                    // added associated detection locations for sequence alerts for SAPIENT Phase 4 May 16
                    if (assocDetection.Location != null)
                    {
                        var location = assocDetection.Location;
                        var x = location.X.ToString("F7");
                        var y = location.Y.ToString("F7");
                        var z = DatabaseUtil.Nullable(location, lo => lo.Z);
                        var dx = DatabaseUtil.Nullable(location, lo => lo.XError);
                        var dy = DatabaseUtil.Nullable(location, lo => lo.YError);
                        var dz = DatabaseUtil.Nullable(location, lo => lo.ZError);

                        sb.AppendFormat($@"INSERT INTO  {tableName} VALUES(
                            '{message.NodeId}',
                            '{alertTimeStamp}',
                            '{updateTimeStamp}',
                            '{message.Alert.AlertId}',
                            {x}, 
                            {y},
                            {z}, 
                            {dx},
                            {dy},
                            {dz},
                            '{assocDetection.Description}',
                            '{assocDetection.ObjectId}'
                            );");
                    }
                }*/
            }
        }
    }
}