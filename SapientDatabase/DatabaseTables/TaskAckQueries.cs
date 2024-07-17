// Project:           $Project: 00010855-Sapient$
// File:              $Workfile: TaskAckQueries.cs$
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

    /// <summary>
    /// Database methods relating to Task Acknowledgement messages
    /// </summary>
    public class TaskAckQueries
    {
        /// <summary>
        /// Log4net logger.
        /// </summary>
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Test writing to Task Acknowledgement database table
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

            int taskId = 123;

            SapientMessage message = new SapientMessage
            {
                Timestamp = Timestamp.FromDateTime(DateTime.UtcNow),
                NodeId = Guid.NewGuid().ToString(),
                TaskAck = new TaskAck
                {
                    TaskId = $"{taskId}",
                },
            };

            string tableName = TaskConstants.HLTaskAck.Table;
            string sqlString = GenerateInsertQueryString(message, tableName);
            Database.SendToDatabase(sqlString, connection, thisLock);

            message.TaskAck.TaskId = $"{taskId++}";

            sqlString = GenerateInsertQueryString(message, tableName);
            Database.SendToDatabase(sqlString, connection, thisLock);

            tableName = TaskConstants.SensorTaskAck.Table;
            sqlString = GenerateInsertQueryString(message, tableName);
            Database.SendToDatabase(sqlString, connection, thisLock);

            tableName = TaskConstants.GUITaskAck.Table;
            sqlString = GenerateInsertQueryString(message, tableName);
            Database.SendToDatabase(sqlString, connection, thisLock);
        }

        /// <summary>
        /// Method to write task acknowledgement message to database
        /// </summary>
        /// <param name="message">task acknowledgement object</param>
        /// <param name="tableName">database table to write to</param>
        /// <returns>SQL string</returns>
        public static string GenerateInsertQueryString(SapientMessage message, string tableName)
        {
            string sqlString = string.Empty;
            DateTime updateTime = DateTime.UtcNow;
            string messageTimeStamp = message.Timestamp.ToDateTime().ToString(DatabaseUtil.DateTimeFormat);
            string updateTimeStamp = updateTime.ToString(DatabaseUtil.DateTimeFormat);
            int task_key_id = 0;

            sqlString = string.Format($@"INSERT INTO  {tableName} VALUES(
                    '{message.NodeId}',
                    '{messageTimeStamp}',
                    '{updateTimeStamp}', 
                    '{message.TaskAck.TaskId}',
                    '{message.TaskAck.TaskStatus}',
                    '{message.TaskAck.Reason}',
                    {task_key_id});");

            return sqlString;
        }
    }
}