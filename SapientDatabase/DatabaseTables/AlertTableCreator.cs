// Project:           $Project: 00010855-Sapient$
// File:              $Workfile: AlertTableCreator.cs$
// Crown-owned copyright, 2021-2024
//  See Release/Supply Conditions
// $NoKeywords$
namespace SapientDatabase.DatabaseTables
{
    using System;
    using log4net;
    using Npgsql;

    /// <summary>
    /// Helper functions for creating the alert tables and sequences.
    /// </summary>
    public static class AlertTableCreator
    {
        /// <summary>
        /// Creates all the alert sequences, indices and tables.
        /// </summary>
        /// <param name="connection">The database connection.</param>
        /// <param name="logger">The message logger.</param>
        public static void Create(NpgsqlConnection connection, ILog logger)
        {
            using (var command = new NpgsqlCommand { Connection = connection })
            {
                DatabaseUtil.CreateSequence(AlertConstants.Alert.Seq, command, logger);
                DatabaseUtil.CreateSequence(AlertConstants.HLAlert.Seq, command, logger);

                CreateAlertTable(AlertConstants.Alert.Table, AlertConstants.Alert.Seq, AlertConstants.Alert.Pkey, command, logger);
                CreateAlertTable(AlertConstants.HLAlert.Table, AlertConstants.HLAlert.Seq, AlertConstants.HLAlert.Pkey, command, logger);

                CreateLocationTable(AlertConstants.AlertLocation.Table, AlertConstants.AlertLocation.KeyIdSeq, AlertConstants.AlertLocation.Pkey, command, logger);
                CreateLocationTable(AlertConstants.HLAlertLocation.Table, AlertConstants.HLAlertLocation.KeyIdSeq, AlertConstants.HLAlertLocation.Pkey, command, logger);

                CreateRangeBearingTable(AlertConstants.AlertRangeBearing.Table, AlertConstants.AlertRangeBearing.Seq, AlertConstants.AlertRangeBearing.Pkey, command, logger);
                CreateRangeBearingTable(AlertConstants.HLAlertRangeBearing.Table, AlertConstants.HLAlertRangeBearing.Seq, AlertConstants.HLAlertRangeBearing.Pkey, command, logger);

                CreateAssocFileTable(AlertConstants.AlertAssocFile.Table, AlertConstants.AlertAssocFile.KeyIdSeq, AlertConstants.AlertAssocFile.Pkey, command, logger);
                CreateAssocFileTable(AlertConstants.HLAlertAssocFile.Table, AlertConstants.HLAlertAssocFile.KeyIdSeq, AlertConstants.HLAlertAssocFile.Pkey, command, logger);

                CreateAssocDetectionTable(AlertConstants.AlertAssocDetection.Table, AlertConstants.AlertAssocDetection.KeyIdSeq, AlertConstants.AlertAssocDetection.Pkey, command, logger);
                CreateAssocDetectionTable(AlertConstants.HLAlertAssocDetection.Table, AlertConstants.HLAlertAssocDetection.KeyIdSeq, AlertConstants.HLAlertAssocDetection.Pkey, command, logger);

                CreateIndices("alert", command, logger);
                CreateIndices("hl_alert", command, logger);
            }
        }

        /// <summary>
        /// Creates the alert table.
        /// </summary>
        /// <param name="tableName">The table name.</param>
        /// <param name="idSeq">The sequence for the primary key.</param>
        /// <param name="tablePkey">The table primary key constraint.</param>
        /// <param name="command">The database command to use.</param>
        /// <param name="logger">The message logger.</param>
        public static void CreateAlertTable(string tableName, string idSeq, string tablePkey, NpgsqlCommand command, ILog logger)
        {
            command.CommandText = @"CREATE TABLE IF NOT EXISTS " + tableName + @" ( 
                    source_id text NOT NULL,
                    alert_time timestamp without time Zone NOT NULL,
                    update_time Timestamp without time Zone NOT NULL,                          
                    alert_id text NOT NULL,
                    alert_type text,                            
                    status text,
                    description text,
                    priority text,
                    ranking double precision,
                    region_id text,
                    confidence double precision,
                    debug_text text,
                    response_timestamp timestamp without time Zone,
                    response_reason text,
                    key_id bigint DEFAULT nextval('" + idSeq + @"') NOT NULL,
                    CONSTRAINT " + tablePkey + @" PRIMARY KEY (key_id));";

            command.ExecuteNonQuery();

            logger.Info("Created table " + tableName);
        }

        /// <summary>
        /// Creates the alert location table.
        /// </summary>
        /// <param name="tableName">The table name.</param>
        /// <param name="keyIdSeq">The sequence for the key id column.</param>
        /// <param name="tablePkey">The table primary key constraint.</param>
        /// <param name="command">The database command to use.</param>
        /// <param name="logger">The message logger.</param>
        public static void CreateLocationTable(string tableName, string keyIdSeq, string tablePkey, NpgsqlCommand command, ILog logger)
        {
            command.CommandText = @"CREATE TABLE IF NOT EXISTS " + tableName + @" ( 
                    source_id text NOT NULL,
                    alert_time timestamp without time Zone NOT NULL,
                    update_time Timestamp without time Zone NOT NULL,                       
                    alert_id text NOT NULL,
                    x double precision NOT NULL,
                    y double precision NOT NULL,
                    z double precision,
                    eX double precision,
                    eY double precision,
                    eZ double precision,
                    description text,
                    object_id text,
                    table_key_id bigserial,
                    key_id bigint DEFAULT currval('" + keyIdSeq + @"') NOT NULL,
                    CONSTRAINT " + tablePkey + @" PRIMARY KEY (table_key_id));";

            command.ExecuteNonQuery();

            logger.Info("Created table " + tableName);
        }

        /// <summary>
        /// Creates the alert range bearing table.
        /// </summary>
        /// <param name="tableName">The table name.</param>
        /// <param name="idSeq">The sequence for the primary key.</param>
        /// <param name="tablePkey">The table primary key constraint.</param>
        /// <param name="command">The database command to use.</param>
        /// <param name="logger">The message logger.</param>
        public static void CreateRangeBearingTable(string tableName, string idSeq, string tablePkey, NpgsqlCommand command, ILog logger)
        {
            command.CommandText = @"CREATE TABLE IF NOT EXISTS " + tableName + @" ( 
                    source_id text NOT NULL,
                    alert_time timestamp without time Zone NOT NULL,
                    update_time Timestamp without time Zone NOT NULL,                          
                    alert_id text NOT NULL,
                    R double precision NOT NULL,
                    Az double precision NOT NULL,
                    Ele double precision,
                    Z double precision,
                    eR double precision,
                    eAz double precision,
                    eEle double precision,
                    eZ double precision,
                    key_id bigint DEFAULT currval('" + idSeq + @"') NOT NULL,
                    CONSTRAINT " + tablePkey + @" PRIMARY KEY (key_id));";

            command.ExecuteNonQuery();

            logger.Info("Created table " + tableName);
        }

        /// <summary>
        /// Creates the alert associated files table.
        /// </summary>
        /// <param name="tableName">The table name.</param>
        /// <param name="keyIdSeq">The sequence for the key id column.</param>
        /// <param name="tablePkey">The table primary key constraint.</param>
        /// <param name="command">The database command to use.</param>
        /// <param name="logger">The message logger.</param>
        public static void CreateAssocFileTable(string tableName, string keyIdSeq, string tablePkey, NpgsqlCommand command, ILog logger)
        {
            command.CommandText =
                @"CREATE TABLE IF NOT EXISTS " + tableName + @" ( 
                source_id text NOT NULL,
                alert_time timestamp without time Zone NOT NULL,
                update_time Timestamp without time Zone NOT NULL,                          
                alert_id text NOT NULL,
                type text,
                url text,
                table_key_id bigserial,
                key_id bigint DEFAULT currval('" + keyIdSeq + @"') NOT NULL,
                CONSTRAINT " + tablePkey + @" PRIMARY KEY (table_key_id));";

            command.ExecuteNonQuery();

            logger.Info("Created table " + tableName);
        }

        /// <summary>
        /// Creates the alert associated detection table.
        /// </summary>
        /// <param name="tableName">The table name.</param>
        /// <param name="keyIdSeq">The sequence for the key id column.</param>
        /// <param name="tablePkey">The table primary key constraint.</param>
        /// <param name="command">The database command to use.</param>
        /// <param name="logger">The message logger.</param>
        public static void CreateAssocDetectionTable(string tableName, string keyIdSeq, string tablePkey, NpgsqlCommand command, ILog logger)
        {
            command.CommandText =
                @"CREATE TABLE IF NOT EXISTS " + tableName + @" ( 
                source_id text NOT NULL,
                alert_time timestamp without time Zone NOT NULL,
                update_time Timestamp without time Zone NOT NULL,                          
                alert_id text NOT NULL,
                object_id text NOT NULL,
                table_key_id bigserial,
                key_id bigint DEFAULT currval('" + keyIdSeq + @"') NOT NULL,
                CONSTRAINT " + tablePkey + @" PRIMARY KEY (table_key_id));";

            command.ExecuteNonQuery();

            logger.Info("Created table " + tableName);
        }

        /// <summary>
        /// Creates an index for each alert table.
        /// </summary>
        /// <param name="tablePrefix">The database table name prefix.</param>
        /// <param name="command">The database command to use.</param>
        /// <param name="logger">The message logger.</param>
        public static void CreateIndices(string tablePrefix, NpgsqlCommand command, ILog logger)
        {
            try
            {
                command.CommandText = @"CREATE INDEX " + tablePrefix + @"_BSIFlex335v2_alert_id
                ON " + tablePrefix + @"_BSIFlex335v2 USING btree (alert_id);
                CREATE INDEX " + tablePrefix + @"_location_BSIFlex335v2_alert_id
                ON " + tablePrefix + @"_location_BSIFlex335v2 USING btree (alert_id);
                CREATE INDEX " + tablePrefix + @"_range_bearing_BSIFlex335v2_alert_id
                ON " + tablePrefix + @"_range_bearing_BSIFlex335v2 USING btree (alert_id);
                CREATE INDEX " + tablePrefix + @"_assoc_file_BSIFlex335v2_alert_id
                ON " + tablePrefix + @"_assoc_file_BSIFlex335v2 USING btree (alert_id);
                CREATE INDEX " + tablePrefix + @"_assoc_detection_BSIFlex335v2_alert_id
                ON " + tablePrefix + @"_assoc_detection_BSIFlex335v2 USING btree (alert_id);";

                command.ExecuteNonQuery();

                logger.Info("Created Alert indices in db.");
            }
            catch (Exception e)
            {
                logger.Error("Problem creating alert indices in db.", e);
            }
        }
    }
}
