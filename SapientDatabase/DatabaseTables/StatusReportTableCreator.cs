// Project:           $Project: 00010855-Sapient$
// File:              $Workfile: StatusReportTableCreator.cs$
// Crown-owned copyright, 2021-2024
//  See Release/Supply Conditions
// $NoKeywords$
namespace SapientDatabase.DatabaseTables
{
    using log4net;
    using Npgsql;

    /// <summary>
    /// Helper functions for creating the status report tables and sequences.
    /// </summary>
    public static class StatusReportTableCreator
    {
        /// <summary>
        /// Creates all the detection sequences, indices and tables.
        /// </summary>
        /// <param name="connection">The database connection.</param>
        /// <param name="logger">The message logger.</param>
        public static void Create(NpgsqlConnection connection, ILog logger)
        {
            using (var command = new NpgsqlCommand { Connection = connection })
            {
                DatabaseUtil.CreateSequence(StatusReportConstants.Seq, command, logger);

                CreateStatusReportTable(
                    StatusReportConstants.Table,
                    StatusReportConstants.Seq,
                    StatusReportConstants.Pkey,
                    StatusReportConstants.CommonKeyName,
                    command,
                    logger);

                CreateMessagesTable(command, logger);

                CreateRangeBearingTable(
                    StatusReportRangeBearingConstants.Table,
                    StatusReportRangeBearingConstants.IdSeq,
                    StatusReportRangeBearingConstants.Pkey,
                    StatusReportRangeBearingConstants.CommonKeyName,
                    command,
                    logger);

                CreateRegionTables(
                    StatusReportRegionConstants.Table,
                    StatusReportRegionConstants.IdSeq,
                    StatusReportRegionConstants.Pkey,
                    StatusReportRegionConstants.CommonKeyName,
                    false,
                    command, 
                    logger);

                CreateRegionTables(
                    HLStatusReportRegionConstants.Table,
                    string.Empty,
                    HLStatusReportRegionConstants.Pkey,
                    string.Empty,
                    true,
                    command,
                    logger);

                CreateIndices(command, logger);
            }
        }

        /// <summary>
        /// Creates the status report table.
        /// </summary>
        /// <param name="tableName">The table name.</param>
        /// <param name="idSeq">The sequence for the primary key.</param>
        /// <param name="tablePkey">The table primary key constraint.</param>
        /// <param name="keyFieldName">name of table column name for common key</param> 
        /// <param name="command">The database command to use.</param>
        /// <param name="logger">The message logger.</param>
        private static void CreateStatusReportTable(string tableName, string idSeq, string tablePkey, string keyFieldName, NpgsqlCommand command, ILog logger)
        {
            command.CommandText = string.Format($@"CREATE TABLE IF NOT EXISTS {tableName} (
                source_id text NOT NULL,
                message_time Timestamp without time Zone NOT NULL,
                update_time Timestamp without time Zone NOT NULL,                            
                report_id text NOT NULL,                              
                system text,
                info text,
                active_task_id text,
                mode text,
                power_source text,
                power_status text,
                power_level int,
                x double precision,
                y double precision,
                z double precision,
                eX double precision,
                eY double precision,
                eZ double precision,
                velocity_x double precision,
                velocity_y double precision,
                platform_speed double precision,
                platform_heading double precision,
                {keyFieldName} bigint DEFAULT nextval('{idSeq}') NOT NULL,
                CONSTRAINT {tablePkey} PRIMARY KEY ({keyFieldName}));");
            command.ExecuteNonQuery();

            logger.Info("Created table " + tableName);
        }

        /// <summary>
        /// Creates the messages table.
        /// </summary>
        /// <param name="command">The database command to use.</param>
        /// <param name="logger">The message logger.</param>
        private static void CreateMessagesTable(NpgsqlCommand command, ILog logger)
        {
            string tableName = StatusReportMessagesConstants.Table;
            string idSeq = StatusReportMessagesConstants.IdSeq;
            string tablePkey = StatusReportMessagesConstants.Pkey;
            string keyFieldName = StatusReportMessagesConstants.CommonKeyName;

            command.CommandText = string.Format($@"CREATE TABLE IF NOT EXISTS {tableName} ( 
                source_id text NOT NULL,
                message_time Timestamp without time Zone NOT NULL,
                update_time Timestamp without time Zone NOT NULL,                             
                report_id text NOT NULL,                              
                level text,
                type text,
                value text,
                key_id bigserial,
                {keyFieldName} bigint DEFAULT currval('{idSeq}') NOT NULL,
                CONSTRAINT {tablePkey} PRIMARY KEY (key_id));");
            command.ExecuteNonQuery();

            logger.Info("Created table " + tableName);
        }

        /// <summary>
        /// Creates the range bearing table.
        /// </summary>
        /// <param name="tableName">The table name.</param>
        /// <param name="idSeq">The sequence for the primary key.</param>
        /// <param name="tablePkey">The table primary key constraint.</param>
        /// <param name="keyFieldName">name of table column name for common key</param>
        /// <param name="command">The database command to use.</param>
        /// <param name="logger">The message logger.</param>
        public static void CreateRangeBearingTable(string tableName, string idSeq, string tablePkey, string keyFieldName, NpgsqlCommand command, ILog logger)
        {
            command.CommandText = string.Format($@"CREATE TABLE IF NOT EXISTS {tableName} ( 
                source_id text NOT NULL,
                message_time Timestamp without time Zone NOT NULL,
                update_time Timestamp without time Zone NOT NULL,                            
                report_id text NOT NULL,                              
                type text NOT NULL,
                R double precision NOT NULL,
                Az double precision NOT NULL,
                Ele double precision,
                hExtent double precision NOT NULL,
                vExtent double precision,
                eR double precision,
                eAz double precision,
                eEle double precision,
                ehExtent double precision,
                evExtent double precision,
                key_id bigserial,
                {keyFieldName} bigint DEFAULT currval('{idSeq}') NOT NULL,
                CONSTRAINT {tablePkey} PRIMARY KEY (key_id));");
            command.ExecuteNonQuery();

            logger.Info("Created table " + tableName);
        }

        /// <summary>
        /// Creates the region tables.
        /// </summary>
        /// <param name="tableName">The table name.</param>
        /// <param name="idSeq">The sequence for the primary key.</param>
        /// <param name="tablePkey">The table primary key constraint.</param>
        /// <param name="keyFieldName">name of table column name for common key</param> 
        /// <param name="dmm">whether DMM or not</param> 
        /// <param name="command">The database command to use.</param>
        /// <param name="logger">The message logger.</param>
        private static void CreateRegionTables(string tableName, string idSeq, string tablePkey, string keyFieldName, bool dmm, NpgsqlCommand command, ILog logger)
        {
            string additionalFields = string.Empty;

            // High level table has additional fields following Counter mobility work in 2016
            if (dmm)
            {
                additionalFields = string.Format($@"region_name text,
                                                    status text,
                                                    description text,
                                                    key_id bigserial,
                                                CONSTRAINT {tablePkey} PRIMARY KEY (key_id)");
            }
            else
            {
                additionalFields = string.Format($@"key_id bigserial,
                                                {keyFieldName} bigint DEFAULT currval('{idSeq}') NOT NULL,
                                                CONSTRAINT {tablePkey} PRIMARY KEY (key_id)");
            }

            string sqlString = string.Format($@"CREATE TABLE IF NOT EXISTS {tableName} ( 
                source_id text NOT NULL,
                message_time Timestamp without time Zone NOT NULL,
                update_time Timestamp without time Zone NOT NULL,                             
                report_id text NOT NULL,                              
                type text NOT NULL,
                location polygon NOT NULL,
                region_id text NOT NULL,
                location_e polygon,
                {additionalFields}
                );");

            command.CommandText = sqlString;
            command.ExecuteNonQuery();
            logger.Info("Created table " + tableName);
        }

        /// <summary>
        /// Creates indices for sensor report tables.
        /// </summary>
        /// <param name="command">The database command to use.</param>
        /// <param name="logger">The message logger.</param>
        private static void CreateIndices(NpgsqlCommand command, ILog logger)
        {
            try
            {
                command.CommandText = @"
                    CREATE INDEX status_report_source_id
                    ON status_report_BSIFlex335v2 USING btree (source_id);

                    CREATE INDEX status_report_messages_source_id
                    ON status_report_messages_BSIFlex335v2 USING btree (source_id);

                    CREATE INDEX status_report_range_bearing_source_id
                    ON status_report_range_bearing_BSIFlex335v2 USING btree (source_id);

                    CREATE INDEX status_report_region_source_id
                    ON status_report_region_BSIFlex335v2 USING btree (source_id);

                    CREATE INDEX hl_status_report_region_source_id
                    ON hl_status_report_region_BSIFlex335v2 USING btree (source_id);

                    CREATE INDEX status_report_status_id
                    ON status_report_BSIFlex335v2 USING btree (status_id);

                    CREATE INDEX status_report_messages_status_id
                    ON status_report_messages_BSIFlex335v2 USING btree (status_id);

                    CREATE INDEX status_report_range_bearing_status_id
                    ON status_report_range_bearing_BSIFlex335v2 USING btree (status_id);

                    CREATE INDEX status_report_region_status_id
                    ON status_report_region_BSIFlex335v2 USING btree (status_id);";
                command.ExecuteNonQuery();

                command.CommandText = @"
                    CREATE INDEX status_report_update_time_id
                    ON status_report_BSIFlex335v2 USING btree (update_time);

                    CREATE INDEX status_report_messages_update_time_id
                    ON status_report_messages_BSIFlex335v2 USING btree (update_time);

                    CREATE INDEX status_report_range_bearing_update_time_id
                    ON status_report_range_bearing_BSIFlex335v2 USING btree (update_time);

                    CREATE INDEX status_report_region_update_time_id
                    ON status_report_region_BSIFlex335v2 USING btree (update_time);

                    CREATE INDEX hl_status_report_region_update_time_id
                    ON hl_status_report_region_BSIFlex335v2 USING btree (update_time);";
                command.ExecuteNonQuery();

                // additional indices for Cubica DMM
                command.CommandText = @"CREATE INDEX status_report_time_source_id
                    ON status_report_BSIFlex335v2 USING btree (message_time, source_id);";
                command.ExecuteNonQuery();

                command.CommandText = @"CREATE INDEX status_report_info_source_time_id
                    ON status_report_BSIFlex335v2 USING btree (info, source_id, message_time DESC);";
                command.ExecuteNonQuery();

                logger.Info("Created StatusReport indices in db.");
            }
            catch (NpgsqlException e)
            {
                logger.Error("Can't create StatusReport indices in db.", e);
            }
        }
    }
}
