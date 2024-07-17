// Project:           $Project: 00010855-Sapient$
// File:              $Workfile: RoutePlanTableCreator.cs$
// Crown-owned copyright, 2021-2024
//  See Release/Supply Conditions
// $NoKeywords$
namespace SapientDatabase.DatabaseTables
{
    using log4net;
    using Npgsql;

    /// <summary>
    /// Helper functions for creating the RoutePlan tables and sequences.
    /// </summary>
    public static class RoutePlanTableCreator
    {
        /// <summary>
        /// Creates all the Route Plan sequences, indices and tables.
        /// </summary>
        /// <param name="connection">The database connection.</param>
        /// <param name="logger">The message logger.</param>
        public static void Create(NpgsqlConnection connection, ILog logger)
        {
            using (var command = new NpgsqlCommand { Connection = connection })
            {
                DatabaseUtil.CreateSequence(RoutePlanConstants.Seq, command, logger);

                CreateRoutePlanTable(
                    RoutePlanConstants.Table,
                    RoutePlanConstants.Seq,
                    RoutePlanConstants.Pkey,
                    command,
                    logger);

                StatusReportTableCreator.CreateRangeBearingTable(
                    RoutePlanRangeBearingConstants.Table,
                    RoutePlanRangeBearingConstants.IdSeq,
                    RoutePlanRangeBearingConstants.Pkey,
                    RoutePlanRangeBearingConstants.CommonKeyName,
                    command,
                    logger);

                CreateIndices(command, logger);
            }
        }

        /// <summary>
        /// Creates the Route Plan table.
        /// </summary>
        /// <param name="tableName">The table name.</param>
        /// <param name="idSeq">The sequence for the primary key.</param> 
        /// <param name="tablePkey">The table primary key constraint.</param>
        /// <param name="command">The database command to use.</param>
        /// <param name="logger">The message logger.</param>
        private static void CreateRoutePlanTable(string tableName, string idSeq, string tablePkey, NpgsqlCommand command, ILog logger)
        {
            command.CommandText = string.Format($@"CREATE TABLE IF NOT EXISTS {tableName} ( 
                    source_id text NOT NULL,
                    message_time timestamp without time zone NOT NULL,
                    update_time timestamp without time zone NOT NULL,
                    sensor_id text NOT NULL,
                    task_id text NOT NULL,
                    objective_id text NOT NULL,
                    route_name text,
                    description text,
                    location path NOT NULL,
                    eta timestamp without time zone,
                    route_status text,
                    xy_fov polygon,
                    key_id bigint DEFAULT nextval('{idSeq}') NOT NULL,
                    CONSTRAINT {tablePkey} PRIMARY KEY (key_id));");
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
                    CREATE INDEX route_plan_source_id
                    ON route_plan_BSIFlex335v2 USING btree (source_id);

                    CREATE INDEX route_plan_range_bearing_source_id
                    ON route_plan_fov_range_bearing_BSIFlex335v2 USING btree (source_id);";
                command.ExecuteNonQuery();

                command.CommandText = @"
                    CREATE INDEX route_plan_time_id
                    ON route_plan_BSIFlex335v2 USING btree (update_time);

                    CREATE INDEX route_plan_range_bearing_time_id
                    ON route_plan_fov_range_bearing_BSIFlex335v2 USING btree (update_time);";

                logger.Info("Created route_plan indices in db.");
            }
            catch (NpgsqlException e)
            {
                logger.Error("Can't create route_plan indices in db.", e);
            }
        }
    }
}
