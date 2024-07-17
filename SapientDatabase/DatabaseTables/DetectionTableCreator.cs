// Project:           $Project: 00010855-Sapient$
// File:              $Workfile: DetectionTableCreator.cs$
// Crown-owned copyright, 2021-2024
//  See Release/Supply Conditions
// $NoKeywords$
namespace SapientDatabase.DatabaseTables
{
    using System;

    using log4net;
    using Npgsql;

    /// <summary>
    /// Helper functions for creating the detection report tables and sequences.
    /// </summary>
    public static class DetectionTableCreator
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
                DatabaseUtil.CreateSequence(DetectionConstants.DetectionReportClass.Seq, command, logger);
                DatabaseUtil.CreateSequence(DetectionConstants.HLDetectionReportClass.Seq, command, logger);

                CreateRangeBearingTable(
                    DetectionConstants.DetectionReportRangeBearing.Table,
                    DetectionConstants.DetectionReportRangeBearing.DetectionIdSeq,
                    DetectionConstants.DetectionReportRangeBearing.Pkey,
                    command,
                    logger);

                CreateRangeBearingTable(
                    DetectionConstants.HLDetectionReportRangeBearing.Table,
                    DetectionConstants.HLDetectionReportRangeBearing.DetectionIdSeq,
                    DetectionConstants.HLDetectionReportRangeBearing.Pkey,
                    command,
                    logger);

                CreateLocationTable(
                    DetectionConstants.DetectionReportLocation.Table,
                    DetectionConstants.DetectionReportLocation.DetectionIdSeq,
                    DetectionConstants.DetectionReportLocation.Pkey,
                    command,
                    logger);

                CreateLocationTable(
                    DetectionConstants.HLDetectionReportLocation.Table,
                    DetectionConstants.HLDetectionReportLocation.DetectionIdSeq,
                    DetectionConstants.HLDetectionReportLocation.Pkey,
                    command,
                    logger);

                CreateTrackInfoTable(
                    DetectionConstants.DetectionReportTrackInfo.Table,
                    DetectionConstants.DetectionReportTrackInfo.DetectionIdSeq,
                    DetectionConstants.DetectionReportTrackInfo.Pkey,
                    command,
                    logger);

                CreateTrackInfoTable(
                    DetectionConstants.HLDetectionReportTrackInfo.Table,
                    DetectionConstants.HLDetectionReportTrackInfo.DetectionIdSeq,
                    DetectionConstants.HLDetectionReportTrackInfo.Pkey,
                    command,
                    logger);

                CreateClassTable(
                    DetectionConstants.DetectionReportClass.Table,
                    DetectionConstants.DetectionReportClass.Seq,
                    DetectionConstants.DetectionReportClass.Pkey,
                    command,
                    logger);

                CreateClassTable(
                    DetectionConstants.HLDetectionReportClass.Table,
                    DetectionConstants.HLDetectionReportClass.Seq,
                    DetectionConstants.HLDetectionReportClass.Pkey,
                    command,
                    logger);

                CreateSubclassTable(
                    DetectionConstants.DetectionReportSubclass.Table,
                    DetectionConstants.DetectionReportSubclass.DetectionIdSeq,
                    DetectionConstants.DetectionReportSubclass.Pkey,
                    command,
                    logger);

                CreateSubclassTable(
                    DetectionConstants.HLDetectionReportSubclass.Table,
                    DetectionConstants.HLDetectionReportSubclass.DetectionIdSeq,
                    DetectionConstants.HLDetectionReportSubclass.Pkey,
                    command,
                    logger);

                CreateBehaviourTable(
                    DetectionConstants.DetectionReportBehaviour.Table,
                    DetectionConstants.DetectionReportBehaviour.DetectionIdSeq,
                    DetectionConstants.DetectionReportBehaviour.Pkey,
                    command,
                    logger);

                CreateBehaviourTable(
                    DetectionConstants.HLDetectionReportBehaviour.Table,
                    DetectionConstants.HLDetectionReportBehaviour.DetectionIdSeq,
                    DetectionConstants.HLDetectionReportBehaviour.Pkey,
                    command,
                    logger);

                CreateAssocFileTable(
                    DetectionConstants.DetectionReportAssocFile.Table,
                    DetectionConstants.DetectionReportAssocFile.DetectionIdSeq,
                    DetectionConstants.DetectionReportAssocFile.Pkey,
                    command,
                    logger);

                CreateAssocFileTable(
                    DetectionConstants.HLDetectionReportAssocFile.Table,
                    DetectionConstants.HLDetectionReportAssocFile.DetectionIdSeq,
                    DetectionConstants.HLDetectionReportAssocFile.Pkey,
                    command,
                    logger);

                CreateAssocDetectionTable(
                    DetectionConstants.DetectionReportAssocDetection.Table,
                    DetectionConstants.DetectionReportAssocDetection.DetectionIdSeq,
                    DetectionConstants.DetectionReportAssocDetection.Pkey,
                    command,
                    logger);

                CreateAssocDetectionTable(
                    DetectionConstants.HLDetectionReportAssocDetection.Table,
                    DetectionConstants.HLDetectionReportAssocDetection.DetectionIdSeq,
                    DetectionConstants.HLDetectionReportAssocDetection.Pkey,
                    command,
                    logger);

                CreateIndices("detection_report", command, logger);
                CreateIndices("hl_detection_report", command, logger);
            }
        }

        /// <summary>
        /// Creates the sequences used to generate primary keys for detection tables.
        /// </summary>
        /// <param name="tablePrefix">The table name prefix.</param>
        /// <param name="command">The database command to use.</param>
        /// <param name="logger">The message logger.</param>
        public static void CreateSequences(string tablePrefix, NpgsqlCommand command, ILog logger)
        {
            // Create sequence to provide key for all detection reports
            command.CommandText = @"CREATE SEQUENCE " + tablePrefix + "_id_seq";
            command.ExecuteNonQuery();

            logger.Info("Created sequence " + tablePrefix + "_id_seq");

            // Create sequence to uniquely identify each subclass
            command.CommandText = @"CREATE SEQUENCE " + tablePrefix + "_subclass_id_seq";
            command.ExecuteNonQuery();

            logger.Info("Created sequence " + tablePrefix + "_subclass_id_seq");
        }

        /// <summary>
        /// Creates the detection range bearing table.
        /// </summary>
        /// <param name="tableName">The table name.</param>
        /// <param name="detectionIdSeq">The sequence for the detection primary key.</param>
        /// <param name="tablePkey">The table primary key constraint.</param>
        /// <param name="command">The database command to use.</param>
        /// <param name="logger">The message logger.</param>
        public static void CreateRangeBearingTable(string tableName, string detectionIdSeq, string tablePkey, NpgsqlCommand command, ILog logger)
        {
            command.CommandText = @"CREATE TABLE IF NOT EXISTS " + tableName + @" ( 
                source_id text NOT NULL,
                detection_time Timestamp without time Zone NOT NULL,
                update_time Timestamp without time Zone NOT NULL,                            
                report_id text NOT NULL,
                object_id text NOT NULL,
                task_id text NOT NULL,
                state text,
                detection_confidence double precision,
                colour text,
                predicted boolean NOT NULL,
                predictedTimestamp Timestamp without time Zone,
                R  double precision NOT NULL,
                Az double precision NOT NULL,
                Ele double precision,
                Z  double precision,
                eR double precision,
                eAz double precision,
                eEle double precision,
                eZ double precision,
                affiliation text,
                detection_sensor_id text,
                velocity_x double precision,
                velocity_y double precision,
                speed double precision,
                heading double precision,
                detection_id bigint DEFAULT nextval('" + detectionIdSeq + @"') NOT NULL,
                CONSTRAINT " + tablePkey + @" PRIMARY KEY (detection_id));";
            command.ExecuteNonQuery();

            logger.Info("Created table " + tableName);
        }

        /// <summary>
        /// Creates the detection location table.
        /// </summary>
        /// <param name="tableName">The table name.</param>
        /// <param name="detectionIdSeq">The sequence for the detection primary key.</param>
        /// <param name="tablePkey">The table primary key constraint.</param>
        /// <param name="command">The database command to use.</param>
        /// <param name="logger">The message logger.</param>
        public static void CreateLocationTable(string tableName, string detectionIdSeq, string tablePkey, NpgsqlCommand command, ILog logger)
        {
            command.CommandText = @"CREATE TABLE IF NOT EXISTS " + tableName + @" ( 
                source_id text NOT NULL,
                detection_time Timestamp without time Zone NOT NULL,
                update_time Timestamp without time Zone NOT NULL,                             
                report_id text NOT NULL,
                object_id text NOT NULL,
                task_id text NOT NULL,
                state text,
                detection_confidence double precision,
                colour text,
                predicted boolean NOT NULL,
                predictedTimestamp Timestamp without time Zone,
                X double precision NOT NULL,
                Y double precision NOT NULL,
                Z double precision,
                eX double precision,
                eY double precision,
                eZ double precision,
                affiliation text,
                detection_sensor_id text,
                velocity_x double precision,
                velocity_y double precision,
                speed double precision,
                heading double precision,
                detection_id bigint DEFAULT nextval('" + detectionIdSeq + @"') NOT NULL,
                CONSTRAINT " + tablePkey + @" PRIMARY KEY (detection_id));";
            command.ExecuteNonQuery();

            logger.Info("Created table " + tableName);
        }

        /// <summary>
        /// Creates the detection track info table.
        /// </summary>
        /// <param name="tableName">The table name.</param>
        /// <param name="detectionIdSeq">The sequence for the detection primary key.</param>
        /// <param name="tablePkey">The table primary key constraint.</param>
        /// <param name="logger">The message logger.</param>
        public static void CreateTrackInfoTable(string tableName, string detectionIdSeq, string tablePkey, NpgsqlCommand command, ILog logger)
        {
            command.CommandText = @"CREATE TABLE IF NOT EXISTS " + tableName + @" ( 
                source_id text NOT NULL,
                detection_time Timestamp without time Zone NOT NULL,
                update_time Timestamp without time Zone NOT NULL,                            
                report_id text NOT NULL,
                object_id text NOT NULL,
                info_type text NOT NULL,
                type text,
                value text,
                e double precision,
                key_id bigserial,
                detection_id bigint DEFAULT currval('" + detectionIdSeq + @"') NOT NULL,
                CONSTRAINT " + tablePkey + @" PRIMARY KEY (key_id));";
            command.ExecuteNonQuery();

            logger.Info("Created table " + tableName);
        }

        /// <summary>
        /// Creates the detection class table.
        /// </summary>
        /// <param name="tableName">The table name.</param>
        /// <param name="detectionIdSeq">The sequence for the detection primary key.</param>
        /// <param name="tablePkey">The table primary key constraint.</param>
        /// <param name="command">The database command to use.</param>
        /// <param name="logger">The message logger.</param>
        public static void CreateClassTable(string tableName, string detectionIdSeq, string tablePkey, NpgsqlCommand command, ILog logger)
        {
            command.CommandText = @"CREATE TABLE IF NOT EXISTS " + tableName + @" ( 
                source_id text NOT NULL,
                detection_time Timestamp without time Zone NOT NULL,
                update_time Timestamp without time Zone NOT NULL,                            
                report_id text NOT NULL,
                object_id text NOT NULL,
                type text,
                confidence double precision,
                key_id bigserial,
                detection_id bigint DEFAULT currval('" + detectionIdSeq + @"') NOT NULL,
                CONSTRAINT " + tablePkey + @" PRIMARY KEY (key_id));";
            command.ExecuteNonQuery();

            logger.Info("Created table " + tableName);
        }

        /// <summary>
        /// Creates the detection subclass table.
        /// </summary>
        /// <param name="tableName">The table name.</param>
        /// <param name="detectionIdSeq">The sequence for the detection primary key.</param>
        /// <param name="tablePkey">The table primary key constraint.</param>
        /// <param name="command">The database command to use.</param>
        /// <param name="logger">The message logger.</param>
        public static void CreateSubclassTable(string tableName, string detectionIdSeq, string tablePkey, NpgsqlCommand command, ILog logger)
        {
            command.CommandText = @"CREATE TABLE IF NOT EXISTS " + tableName + @" ( 
                source_id text NOT NULL,
                detection_time Timestamp without time Zone NOT NULL,
                update_time Timestamp without time Zone NOT NULL,                           
                report_id text NOT NULL,
                object_id text NOT NULL,
                class text,
                level int,
                type text,
                value text,
                confidence double precision,
                subclass_id bigint,
                parent_subclass_id bigint,
                key_id bigserial,
                detection_id bigint DEFAULT currval('" + detectionIdSeq + @"') NOT NULL,
                CONSTRAINT " + tablePkey + @" PRIMARY KEY (key_id));";
            command.ExecuteNonQuery();

            logger.Info("Created table " + tableName);
        }

        /// <summary>
        /// Creates the detection behaviour table.
        /// </summary>
        /// <param name="tableName">The table name.</param>
        /// <param name="detectionIdSeq">The sequence for the detection primary key.</param>
        /// <param name="tablePkey">The table primary key constraint.</param>
        /// <param name="command">The database command to use.</param>
        /// <param name="logger">The message logger.</param>
        public static void CreateBehaviourTable(string tableName, string detectionIdSeq, string tablePkey, NpgsqlCommand command, ILog logger)
        {
            command.CommandText = @"CREATE TABLE IF NOT EXISTS " + tableName + @" ( 
                source_id text NOT NULL,
                detection_time Timestamp without time Zone NOT NULL,
                update_time Timestamp without time Zone NOT NULL,                             
                report_id text NOT NULL,
                object_id text NOT NULL,
                type text,
                confidence double precision,
                key_id bigserial,
                detection_id bigint DEFAULT currval('" + detectionIdSeq + @"') NOT NULL,
                CONSTRAINT " + tablePkey + @" PRIMARY KEY (key_id));";
            command.ExecuteNonQuery();

            logger.Info("Created table " + tableName);
        }

        /// <summary>
        /// Creates the detection associated file table.
        /// </summary>
        /// <param name="tableName">The table name.</param>
        /// <param name="detectionIdSeq">The sequence for the detection primary key.</param>
        /// <param name="tablePkey">The table primary key constraint.</param>
        /// <param name="command">The database command to use.</param>
        /// <param name="logger">The message logger.</param>
        public static void CreateAssocFileTable(string tableName, string detectionIdSeq, string tablePkey, NpgsqlCommand command, ILog logger)
        {
            command.CommandText =
                @"CREATE TABLE IF NOT EXISTS " + tableName + @" ( 
                source_id text NOT NULL,
                detection_time Timestamp without time Zone NOT NULL,
                update_time Timestamp without time Zone NOT NULL,                            
                report_id text NOT NULL,
                object_id text NOT NULL,
                type text,
                url text,
                key_id bigserial,
                detection_id bigint DEFAULT currval('" + detectionIdSeq + @"') NOT NULL,
                CONSTRAINT " + tablePkey + @" PRIMARY KEY (key_id));";
            command.ExecuteNonQuery();

            logger.Info("Created table " + tableName);
        }

        /// <summary>
        /// Creates the detection associated detection table.
        /// </summary>
        /// <param name="tableName">The table name.</param>
        /// <param name="detectionIdSeq">The sequence for the detection primary key.</param>
        /// <param name="tablePkey">The table primary key constraint.</param>
        /// <param name="command">The database command to use.</param>
        /// <param name="logger">The message logger.</param>
        public static void CreateAssocDetectionTable(string tableName, string detectionIdSeq, string tablePkey, NpgsqlCommand command, ILog logger)
        {
            command.CommandText =
                @"CREATE TABLE IF NOT EXISTS " + tableName + @" ( 
                source_id text NOT NULL,
                detection_time Timestamp without time Zone NOT NULL,
                update_time Timestamp without time Zone NOT NULL,                            
                report_id text NOT NULL,
                object_id text NOT NULL,
                assoc_type text,
                assoc_time Timestamp without time Zone NOT NULL,                          
                assoc_source_id bigint NOT NULL,
                assoc_object_id bigint NOT NULL,
                key_id bigserial,
                detection_id bigint DEFAULT currval('" + detectionIdSeq + @"') NOT NULL,
                CONSTRAINT " + tablePkey + @" PRIMARY KEY (key_id));";
            command.ExecuteNonQuery();

            logger.Info("Created table " + tableName);
        }

        /// <summary>
        /// Creates an index for each detection table.
        /// </summary>
        /// <param name="tablePrefix">The database table name prefix.</param>
        /// <param name="command">The database command to use.</param>
        /// <param name="logger">The message logger.</param>
        public static void CreateIndices(string tablePrefix, NpgsqlCommand command, ILog logger)
        {
            try
            {
                command.CommandText = @"CREATE INDEX " + tablePrefix + @"_class_detection_id
                    ON " + tablePrefix + @"_class_BSIFlex335v2 USING btree (detection_id);
                    CREATE INDEX " + tablePrefix + @"_subclass_detection_id
                    ON " + tablePrefix + @"_subclass_BSIFlex335v2 USING btree (detection_id);
                    CREATE INDEX " + tablePrefix + @"_behaviour_detection_id
                    ON " + tablePrefix + @"_behaviour_BSIFlex335v2 USING btree (detection_id);
                    CREATE INDEX " + tablePrefix + @"_location_detection_id
                    ON " + tablePrefix + @"_location_BSIFlex335v2 USING btree (detection_id);
                    CREATE INDEX " + tablePrefix + @"_range_bearing_detection_id
                    ON " + tablePrefix + @"_range_bearing_BSIFlex335v2 USING btree (detection_id);
                    CREATE INDEX " + tablePrefix + @"_track_info_detection_id
                    ON " + tablePrefix + @"_track_info_BSIFlex335v2 USING btree (detection_id);
                    CREATE INDEX " + tablePrefix + @"_associated_file_detection_id
                    ON " + tablePrefix + @"_assoc_file_BSIFlex335v2 USING btree (detection_id);";
                command.ExecuteNonQuery();

                command.CommandText = @"CREATE INDEX " + tablePrefix + @"_class_update_time_id
                    ON " + tablePrefix + @"_class_BSIFlex335v2 USING btree (update_time);
                    CREATE INDEX " + tablePrefix + @"_subclass_update_time_id
                    ON " + tablePrefix + @"_subclass_BSIFlex335v2 USING btree (update_time);
                    CREATE INDEX " + tablePrefix + @"_behaviour_update_time_id
                    ON " + tablePrefix + @"_behaviour_BSIFlex335v2 USING btree (update_time);
                    CREATE INDEX " + tablePrefix + @"_location_update_time_id
                    ON " + tablePrefix + @"_location_BSIFlex335v2 USING btree (update_time);
                    CREATE INDEX " + tablePrefix + @"_range_bearing_update_time_id
                    ON " + tablePrefix + @"_range_bearing_BSIFlex335v2 USING btree (update_time);
                    CREATE INDEX " + tablePrefix + @"_track_info_update_time_id
                    ON " + tablePrefix + @"_track_info_BSIFlex335v2 USING btree (update_time);
                    CREATE INDEX " + tablePrefix + @"_associated_file_update_time_id
                    ON " + tablePrefix + @"_assoc_file_BSIFlex335v2 USING btree (update_time);";
                command.ExecuteNonQuery();

                // additional indices for Cubica DMM
                command.CommandText = @"CREATE INDEX " + tablePrefix + @"_location_source_det_id
                    ON " + tablePrefix + @"_location_BSIFlex335v2 USING btree (detection_time, source_id);
                    CREATE INDEX " + tablePrefix + @"_range_bearing_source_det_id
                    ON " + tablePrefix + @"_range_bearing_BSIFlex335v2 USING btree (detection_time, source_id);";
                command.ExecuteNonQuery();

                logger.Info("Created detection indices in db.");
            }
            catch (Exception e)
            {
                logger.Error("Can't create detection indices in db.", e);
            }
        }
    }
}
