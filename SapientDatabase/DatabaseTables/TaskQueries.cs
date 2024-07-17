// Project:           $Project: 00010855-Sapient$
// File:              $Workfile: TaskQueries.cs$
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
    /// Database methods relating to Task messages
    /// </summary>
    public class TaskQueries
    {
        /// <summary>
        /// Log4net logger.
        /// </summary>
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Test writing to task database table
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
                Task = new Task
                {
                    TaskId = "123",
                    Command = new Task.Types.Command
                    {
                        Request = "Registration",
                    },
                },
            };

            string sqlString = GenerateInsertQueryString(message, false);
            Database.SendToDatabase(sqlString, connection, thisLock);
        }

        /// <summary>
        /// Method to write task message to database
        /// </summary>
        /// <param name="message">Object containing all the task information</param>
        /// <param name="task_message">string of entire task message xml</param>
        /// <param name="dmm">if high level database table</param>
        /// <returns>SQL string</returns>
        public static string GenerateInsertQueryString(SapientMessage message, bool dmm)
        {
            bool deleteRegionTask = false;

            if ((message.Task.Region?.Count > 0) && message.Task.Region[0].Type == Task.Types.RegionType.Unspecified)
            {
                deleteRegionTask = true;
            }

            string tableName = TaskConstants.SensorTask.Table;

            if (dmm)
            {
                tableName = TaskConstants.HLTask.Table;
            }

            string taskType = string.Empty;

            if ((message.Task.TaskName != null) && message.Task.TaskName.ToLowerInvariant().Contains("manual task"))
            {
                taskType = "manual task";
            }

            string sqlString = string.Empty;

            if (!deleteRegionTask)
            {
                DateTime updateTime = DateTime.UtcNow;
                string messageTimeStamp = message.Timestamp.ToDateTime().ToString(DatabaseUtil.DateTimeFormat);
                string updateTimeStamp = updateTime.ToString(DatabaseUtil.DateTimeFormat);

                sqlString = $@"INSERT INTO {tableName} VALUES('NA', '{messageTimeStamp}','{updateTimeStamp}', '{message.Task.TaskId}', '{message}', '{taskType}');";
            }

            if (message.Task.Region?.Count > 0)
            {
                sqlString += DbRegion(message);
            }

            Log.Debug(sqlString);
            return sqlString;
        }

        /// <summary>
        /// Dbs the region.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>A string.</returns>
        private static string DbRegion(SapientMessage message)
        {
            string sqlString = string.Empty;

            if (message.Task.Region[0].Type != Task.Types.RegionType.Unspecified)
            {
                sqlString = AddRegions(message);
            }
            else
            {
                sqlString = DeleteRegion(message.Task.Region[0].RegionId);
            }

            return sqlString;
        }

        /// <summary>
        /// Adds the regions.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>A string.</returns>
        private static string AddRegions(SapientMessage message)
        {
            string sqlString = string.Empty;

            if ((message.Task.Region != null) && (message.Task.Region.Count > 0))
            {
                for (int i = 0; i < message.Task.Region.Count; i++)
                {
                    sqlString += AddRegion(message, i);
                }
            }

            return sqlString;
        }

        /// <summary>
        /// Adds the region.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="j">The j.</param>
        /// <returns>A string.</returns>
        private static string AddRegion(SapientMessage message, int j)
        {
            string sqlString = string.Empty;

            if ((message.Task.Region?.Count > 0) && (message.Task.Region[j].RegionArea != null))
            {
                DateTime updateTime = DateTime.UtcNow;
                string messageTimeStamp = message.Timestamp.ToDateTime().ToString(DatabaseUtil.DateTimeFormat);
                string updateTimeStamp = updateTime.ToString(DatabaseUtil.DateTimeFormat);
                string task_id = message.Task.TaskId;

                string type = message.Task.Region[j].Type.ToString();

                string tableName = TaskConstants.HLTaskRegion.Table;

                // Build Location string
                string polygon = string.Empty;
                StringBuilder polygonSb = new StringBuilder();

                if (message.Task.Region[j].RegionArea?.LocationList != null && message.Task.Region[j].RegionArea?.LocationList.Locations != null)
                {
                    var latLonList = message.Task.Region[j].RegionArea?.LocationList.Locations
                                            .Select(l => $"({l.X},{l.Y})");

                    if (latLonList != null)
                    {
                        polygonSb.Append("'(");
                        polygonSb.Append(string.Join(',', latLonList));
                        polygonSb.Append(")'");
                    }

                    polygon = polygonSb.ToString();
                }

                polygon = string.IsNullOrWhiteSpace(polygon) ? "'((0,0))'" : polygon;

                string region_id = message.Task.Region[j].RegionId.ToString();

                sqlString = string.Format($@"INSERT INTO {tableName} VALUES(
                'NA',
                '{messageTimeStamp}',
                '{updateTimeStamp}', 
                '{message.Task.TaskId}',
                '{type}', 
                {polygon},
                '{region_id}');");
            }

            return sqlString;
        }

        /// <summary>
        /// Deletes the region.
        /// </summary>
        /// <param name="regionID">The region i d.</param>
        /// <returns>A string.</returns>
        private static string DeleteRegion(string regionID)
        {
            string sqlString = DeleteRegionFromTaskTable(regionID);
            sqlString += DeleteRegionFromRegionTable(regionID);
            return sqlString;
        }

        /// <summary>
        /// Deletes the region from region table.
        /// </summary>
        /// <param name="regionID">The region i d.</param>
        /// <returns>A string.</returns>
        private static string DeleteRegionFromRegionTable(string regionID)
        {
            // Delete from hl_task_region_BSIFlex335v2
            string sqlString = "DELETE FROM hl_task_region_BSIFlex335v2 WHERE region_id=" + regionID;
            return sqlString;
        }

        /// <summary>
        /// Deletes the region from task table.
        /// </summary>
        /// <param name="regionID">The region i d.</param>
        /// <returns>A string.</returns>
        private static string DeleteRegionFromTaskTable(string regionID)
        {
            // Delete from hl_task FIRST
            string deleteString = "DELETE FROM hl_task_BSIFlex335v2 WHERE task_id IN (SELECT task_id FROM hl_task_region_BSIFlex335v2 WHERE region_id=" + regionID + ")";
            return deleteString;
        }
    }
}