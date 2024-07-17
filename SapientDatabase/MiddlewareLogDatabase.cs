// Project:           $Project: 00010855-Sapient$
// File:              $Workfile: MiddlewareLogDatabase.cs$
// Crown-owned copyright, 2021-2024
//  See Release/Supply Conditions

namespace SapientDatabase
{
    using System;
    using System.Diagnostics;
    using System.Reflection;
    using System.Text;
    using log4net;
    using Npgsql;
    using SapientServices;

    /// <summary>
    /// The middleware log database.
    /// </summary>
    public class MiddlewareLogDatabase
    {
        private const int NumMessageTypes = 19; // size of SapientMessageType enum

        private static string database_server;
        private static string database_port;
        private static string database_user;
        private static string database_password;
        private static string database_name;

        /// <summary>
        /// Log4net logger.
        /// </summary>
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Initializes a new instance of the <see cref="MiddlewareLogDatabase" /> class.
        /// Configure log database connection
        /// </summary>
        /// <param name="host">server on which to connect</param>
        /// <param name="port">port to access database</param>
        /// <param name="user">user name for the database</param>
        /// <param name="password">password to access the database</param>
        /// <param name="name">name of the database</param>
        /// <param name="instanceID">ID of this SDA</param>
        public MiddlewareLogDatabase(string host, string port, string user, string password, string name, int instanceID)
        {
            database_server = host;
            database_port = port;
            database_user = user;
            database_password = password;
            database_name = name;
            InstanceID = instanceID;

            DateTime timenow = DateTime.UtcNow;
            int year = timenow.Year * 10000;
            int month = timenow.Month * 100;
            int day = timenow.Day;
            SessionID = year + month + day;

            MessageTime = new DateTime[NumMessageTypes];
            MessageCount = new int[NumMessageTypes];
            Updated = new bool[NumMessageTypes];

            DatabaseCreator.CreateDatabase(host, port, user, password, name);
            CreateTables(host, port, user, password, name);
            WriteInitialMessageCountsToDatabase();
        }

        /// <summary>
        /// Gets list of times of message types being received
        /// </summary>
        public DateTime[] MessageTime { get; private set; }

        /// <summary>
        /// Gets list of message counts
        /// </summary>
        public int[] MessageCount { get; private set; }

        /// <summary>
        /// Gets data agent instance ID
        /// </summary>
        public int InstanceID { get; private set; }

        /// <summary>
        /// Gets data agent session ID
        /// </summary>
        public int SessionID { get; private set; }

        /// <summary>
        /// Gets where count has updated
        /// </summary>
        private bool[] Updated { get; set; }

        /// <summary>
        /// Set Message Time
        /// </summary>
        /// <param name="index">message type enumerator index into message time array</param>
        public void SetMessageTime(SapientMessageType type, DateTime time)
        {
            MessageTime[(int)type] = time;
        }

        /// <summary>
        /// Sets the message count.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="count">The count.</param>
        public void SetMessageCount(SapientMessageType type, int count)
        {
            MessageCount[(int)type] = count;
            Updated[(int)type] = true;
        }

        /// <summary>
        /// Writes the initial message counts to database.
        /// </summary>
        public void WriteInitialMessageCountsToDatabase()
        {
            string connectionString = GetConnectionString();
            StringBuilder cmd = new StringBuilder();
            string updateTimestamp = DateTime.UtcNow.ToString(DatabaseUtil.DateTimeFormat);

            for (int i = 0; i < MessageCount.Length; i++)
            {
                string lastMessageTimestamp = this.MessageTime[i].ToString(DatabaseUtil.DateTimeFormat); // this.MessageTime[i].ToString("dd/MM/yyyy HH:mm:ss.fff");
                cmd.AppendFormat($@"INSERT INTO message_count VALUES ({InstanceID}, {SessionID}, '{updateTimestamp}', {i}, {this.MessageCount[i]}, '{lastMessageTimestamp}');");
            }

            cmd.AppendFormat($@"INSERT INTO connections VALUES({InstanceID}, {SessionID}, '{updateTimestamp}',0,0,0, FALSE, 0, 0);");

            WriteToDatabase(cmd.ToString(), connectionString);
        }

        /// <summary>
        /// Writes the message counts to database.
        /// </summary>
        public void WriteMessageCountsToDatabase()
        {
            string connectionString = GetConnectionString();
            StringBuilder cmd = new StringBuilder();

            for (int i = 0; i < MessageCount.Length; i++)
            {
                if (Updated[i])
                {
                    string updateTimestamp = DateTime.UtcNow.ToString(DatabaseUtil.DateTimeFormat);
                    string lastMessageTimestamp = this.MessageTime[i].ToString(DatabaseUtil.DateTimeFormat); //this.MessageTime[i].ToString("dd/MM/yyyy HH:mm:ss.fff");
                    cmd.AppendFormat($@"UPDATE message_count SET update_time = '{updateTimestamp}', message_count = {this.MessageCount[i]}, last_message_time = '{lastMessageTimestamp}'
                        WHERE instance_id = {InstanceID} AND session_id = {SessionID} AND type = {i};");
                }

                Updated[i] = false;
            }

            // start stop watch for timing diagnostics.
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            if (cmd.Length > 0)
            {
                WriteToDatabase(cmd.ToString(), connectionString);
            }

            long databaseLatencyMilliseconds = stopWatch.ElapsedMilliseconds;
            Log.InfoFormat("Log Database Latency, {0:D}", databaseLatencyMilliseconds);
        }

        /// <summary>
        /// Writes the connection status to database.
        /// </summary>
        /// <param name="clientConnections">The client connections.</param>
        /// <param name="taskConnections">The task connections.</param>
        /// <param name="guiConnections">The gui connections.</param>
        /// <param name="databaseConnected">If true, database connected.</param>
        /// <param name="commsLatency">The comms latency.</param>
        /// <param name="databaseLatency">The database latency.</param>
        public void WriteConnectionStatusToDatabase(int clientConnections, int taskConnections, int guiConnections, bool databaseConnected, double commsLatency, double databaseLatency)
        {
            string connectionString = GetConnectionString();
            string updateTimestamp = DateTime.UtcNow.ToString(DatabaseUtil.DateTimeFormat);
            string cmd = string.Format($@"UPDATE connections SET update_time = '{updateTimestamp}', client_connections={clientConnections}, task_connections={taskConnections}, gui_connections={guiConnections}, 
                                        database_connected={databaseConnected}, comms_latency={commsLatency}, database_latency={databaseLatency}
                                        WHERE instance_id = {InstanceID} AND session_id = {SessionID};");
            WriteToDatabase(cmd, connectionString);
        }

        /// <summary>
        /// Gets the connection string.
        /// </summary>
        /// <returns>A string.</returns>
        private static string GetConnectionString()
        {
            return string.Format($@"Server={database_server}; Port={database_port}; User Id={database_user}; Password={database_password}; Database={database_name};");
        }

        /// <summary>
        /// Creates the connection log table.
        /// </summary>
        /// <param name="tableName">The table name.</param>
        /// <param name="command">The database command to use.</param>
        /// <param name="logger">The message logger.</param>
        private static void CreateConnectionsTable(string tableName, NpgsqlCommand command, ILog logger)
        {
            command.CommandText = @"CREATE TABLE IF NOT EXISTS connections ( 
                    instance_id integer NOT NULL,
                    session_id integer,
                    update_time Timestamp without time Zone NOT NULL,   
                    client_connections integer NOT NULL,
                    task_connections integer NOT NULL,
                    gui_connections integer NOT NULL,
                    database_connected boolean,
                    comms_latency double precision,
                    database_latency double precision,
                    key_id bigserial,
                    CONSTRAINT connections_pkey PRIMARY KEY (key_id));";

            command.ExecuteNonQuery();

            logger.Info("Created table " + tableName);
        }

        /// <summary>
        /// Creates the message count table.
        /// </summary>
        /// <param name="tableName">The table name.</param>
        /// <param name="command">The database command to use.</param>
        /// <param name="logger">The message logger.</param>
        private static void CreateMessageCountTable(string tableName, NpgsqlCommand command, ILog logger)
        {
            command.CommandText = @"CREATE TABLE IF NOT EXISTS " + tableName + @" ( 
                    instance_id bigint NOT NULL,
                    session_id integer NOT NULL,
                    update_time Timestamp without time Zone NOT NULL,  
                    type integer NOT NULL,
                    message_count integer, 
                    last_message_time timestamp without time Zone,
                    key_id bigserial,
                    CONSTRAINT message_count_pkey PRIMARY KEY (key_id)
                    );";
            command.ExecuteNonQuery();

            logger.Info("Created table " + tableName);
        }

        /// <summary>
        /// Method to create tables in log database
        /// </summary>
        /// <param name="host">server on which to connect</param>
        /// <param name="port">port to access database</param>
        /// <param name="user">user name for the database</param>
        /// <param name="password">password to access the database</param>
        /// <param name="databasename">log database name</param>
        private static void CreateTables(string host, string port, string user, string password, string databaseName)
        {
            try
            {
                string connectionString = ("Server=" + host + ";Port=" + port + ";User Id=" + user + ";Password=" + password + ";Database=" + databaseName + ";");

                NpgsqlConnection connection = new NpgsqlConnection(connectionString);
                connection.Open();

                var command = new NpgsqlCommand
                {
                    Connection = connection,
                };

                CreateConnectionsTable("connections", command, Log);
                CreateMessageCountTable("message_count", command, Log);

                connection.Close();
            }
            catch (NpgsqlException nex)
            {
                if ((uint)nex.ErrorCode == 0x80004005)
                {
                    Log.ErrorFormat("Cannot connect to log database server on Host:{0} Port:{1}. Check these settings and then check that the database server is installed and running.", host, port);
                    Log.Error(nex);
                }
                else
                {
                    Log.Error("SQL Error creating log database");
                    Log.Error(nex);
                }
            }
            catch (Exception e)
            {
                Log.ErrorFormat("Error creating log database tables");
                Log.Error(e);
            }
        }

        /// <summary>
        /// Method to insert into table in log database
        /// </summary>
        /// <param name="sqlString">SQL string</param>
        /// <param name="connectionString">Connection string</param>
        private static void WriteToDatabase(string sqlString, string connectionString)
        {
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (var command = new NpgsqlCommand
                    {
                        Connection = connection,
                        CommandText = sqlString,
                    })
                    {
                        command.ExecuteNonQuery();
                    }
                    connection.Close();
                }
            }
            catch (NpgsqlException nex)
            {
                if ((uint)nex.ErrorCode == 0x80004005)
                {
                    Log.ErrorFormat("Cannot write to log database server. Check these settings and then check that the database server is installed and running.");
                }
                else
                {
                    Log.Error("SQL Error creating database");
                    Log.Error(nex);
                }
            }
            catch (Exception e)
            {
                Log.ErrorFormat("Error writing to log database tables");
                Log.Error(e);
            }
        }
    }
}
