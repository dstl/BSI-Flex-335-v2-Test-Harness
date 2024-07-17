// Project:           $Project: 00010855-Sapient$
// File:              $Workfile: TaskingTablesCreator.cs$
// Crown-owned copyright, 2021-2024
//  See Release/Supply Conditions
// $NoKeywords$
namespace SapientDatabase.DatabaseTables
{
    using log4net;
    using Npgsql;

    /// <summary>
    /// Helper functions for creating the tasking database tables and sequences.
    /// </summary>
    public static class TaskingTablesCreator
    {
        /// <summary>
        /// Creates the tasking tables.
        /// </summary>
        /// <param name="connection">The database connection.</param>
        /// <param name="logger">The logger to use for logging messages.</param>
        public static void Create(NpgsqlConnection connection, ILog logger)
        {
            using (var command = new NpgsqlCommand { Connection = connection })
            {
                DatabaseUtil.CreateSequence(TaskConstants.HLTask.Seq, command, logger);
                DatabaseUtil.CreateSequence(TaskConstants.SensorTask.Seq, command, logger);

                CreateTaskTable(TaskConstants.SensorTask.Table, TaskConstants.SensorTask.Seq, TaskConstants.SensorTask.Pkey, command, logger);
                CreateTaskTable(TaskConstants.HLTask.Table, TaskConstants.HLTask.Seq, TaskConstants.HLTask.Pkey, command, logger);

                CreateTaskAckTable(TaskConstants.SensorTaskAck.Table, TaskConstants.SensorTaskAck.Pkey, command, logger);
                CreateTaskAckTable(TaskConstants.HLTaskAck.Table, TaskConstants.HLTaskAck.Pkey, command, logger);
                CreateTaskAckTable(TaskConstants.GUITaskAck.Table, TaskConstants.GUITaskAck.Pkey, command, logger);

                CreateSensorTaskRegionTable(
                    TaskConstants.SensorTaskRegion.Table, TaskConstants.SensorTaskRegion.TaskKeyIdSeq, TaskConstants.SensorTaskRegion.Pkey, command, logger);

                CreateHLTaskRegionTable(TaskConstants.HLTaskRegion.Table, TaskConstants.HLTaskRegion.Pkey, command, logger);
            }
        }

        /// <summary>
        /// Creates a task table in the database.
        /// </summary>
        /// <param name="tableName">The table name.</param>
        /// <param name="idSeq">The table primary key sequence.</param>
        /// <param name="tablePkey">The table primary key constraint.</param>
        /// <param name="command">The database command to use.</param>
        /// <param name="logger">The message logger.</param>
        public static void CreateTaskTable(string tableName, string idSeq, string tablePkey, NpgsqlCommand command, ILog logger)
        {
            command.CommandText = @"CREATE TABLE IF NOT EXISTS " + tableName + @" ( 
                    sensor_id text NOT NULL,
                    message_time Timestamp without time Zone NOT NULL,
                    update_time Timestamp without time Zone NOT NULL,
                    task_id text NOT NULL,                           
                    taskXml text,
                    task_type text,
                    key_id bigint DEFAULT nextval('" + idSeq + @"') NOT NULL,
                    CONSTRAINT " + tablePkey + @" PRIMARY KEY (key_id));";
            command.ExecuteNonQuery();

            logger.Info("Created task table " + tableName);
        }

        /// <summary>
        /// Creates a task acknowledgement table in the database.
        /// </summary>
        /// <param name="tableName">The table name.</param>
        /// <param name="tablePkey">The table primary key constraint.</param>
        /// <param name="command">The database command to use.</param>
        /// <param name="logger">The message logger.</param>
        public static void CreateTaskAckTable(string tableName, string tablePkey, NpgsqlCommand command, ILog logger)
        {
            command.CommandText = @"CREATE TABLE IF NOT EXISTS " + tableName + @" ( 
                    sensor_id text NOT NULL,
                    ack_timestamp timestamp without time zone NOT NULL,
                    update_time Timestamp without time Zone NOT NULL,
                    task_id text NOT NULL,                           
                    ack_status text,
                    ack_reason text,
                    task_key_id bigint,
                    file_type text,
                    file_url text,
                    key_id bigserial,
                    CONSTRAINT " + tablePkey + @" PRIMARY KEY (key_id));";
            command.ExecuteNonQuery();

            logger.Info("Created table " + tableName);
        }

        /// <summary>
        /// Creates the sensor task region table.
        /// </summary>
        /// <param name="tableName">The table name.</param>
        /// <param name="taskKeyIdSeq">The task key id primary key sequence.</param>
        /// <param name="tablePkey">The table primary key constraint.</param>
        /// <param name="command">The database command to use.</param>
        /// <param name="logger">The message logger.</param>
        public static void CreateSensorTaskRegionTable(
            string tableName, string taskKeyIdSeq, string tablePkey, NpgsqlCommand command, ILog logger)
        {
            command.CommandText = @"CREATE TABLE IF NOT EXISTS " + tableName + @" ( 
                      sensor_id text NOT NULL,
                      message_time timestamp without time zone NOT NULL,
                      update_time timestamp without time zone NOT NULL,
                      task_id text NOT NULL,
                      type text NOT NULL,
                      location polygon NOT NULL,
                      region_id text NOT NULL,
                      task_key_id bigint DEFAULT currval('" + taskKeyIdSeq + @"') NOT NULL,
                      key_id bigserial,
                      CONSTRAINT " + tablePkey + @" PRIMARY KEY (key_id));";
            command.ExecuteNonQuery();

            logger.Info("Created table " + tableName);
        }

        /// <summary>
        /// Creates the HL task region table.
        /// </summary>
        /// <param name="tableName">The table name.</param>
        /// <param name="tablePkey">The table primary key constraint.</param>
        /// <param name="command">The database command to use.</param>
        /// <param name="logger">The message logger.</param>
        private static void CreateHLTaskRegionTable(string tableName, string tablePkey, NpgsqlCommand command, ILog logger)
        {
            // don't have foreign key constraint on high level regions as these are only used bu the GUI
            command.CommandText = @"CREATE TABLE IF NOT EXISTS " + tableName + @" ( 
                      sensor_id text NOT NULL,
                      message_time timestamp without time zone NOT NULL,
                      update_time timestamp without time zone NOT NULL,
                      task_id text NOT NULL,
                      type text NOT NULL,
                      location polygon NOT NULL,
                      region_id text NOT NULL,
                      key_id bigserial,
                      CONSTRAINT " + tablePkey + @" PRIMARY KEY (key_id));";
            command.ExecuteNonQuery();

            logger.Info("Created table " + tableName);
        }
    }
}
