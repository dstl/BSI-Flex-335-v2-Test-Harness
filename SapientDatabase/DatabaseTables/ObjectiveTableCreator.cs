// Project:           $Project: 00010855-Sapient$
// File:              $Workfile: ObjectiveTableCreator.cs$
// Crown-owned copyright, 2021-2024
//  See Release/Supply Conditions
// $NoKeywords$
namespace SapientDatabase.DatabaseTables
{
    using log4net;
    using Npgsql;

    /// <summary>
    /// Helper functions for creating the objective tables and sequences.
    /// </summary>
    public static class ObjectiveTableCreator
    {
        /// <summary>
        /// Creates all the objective sequences, indices and tables.
        /// </summary>
        /// <param name="connection">The database connection.</param>
        /// <param name="logger">The message logger.</param>
        public static void Create(NpgsqlConnection connection, ILog logger)
        {
            using (var command = new NpgsqlCommand { Connection = connection })
            {
                CreateObjectiveTable(ObjectiveConstants.Objective.Table, ObjectiveConstants.Objective.Pkey, command, logger);
                CreateApprovalTable(ObjectiveConstants.Approval.Table, ObjectiveConstants.Approval.Pkey, command, logger);
            }
        }

        /// <summary>
        /// Creates the Objective table.
        /// </summary>
        /// <param name="tableName">The table name.</param>
        /// <param name="tablePkey">The table primary key constraint.</param>
        /// <param name="command">The database command to use.</param>
        /// <param name="logger">The message logger.</param>
        private static void CreateObjectiveTable(string tableName, string tablePkey, NpgsqlCommand command, ILog logger)
        {
            command.CommandText = @"CREATE TABLE IF NOT EXISTS " + tableName + @" ( 
                    objective_time timestamp without time zone NOT NULL,
                    update_time timestamp without time zone NOT NULL,
                    source_id bigint NOT NULL,
                    objective_id bigint NOT NULL,
                    sensor_id text NOT NULL,
                    type text,
                    status text,
                    description text,
                    x double precision NOT NULL,
                    y double precision NOT NULL,
                    z double precision,
                    priority text,
                    ranking double precision,
                    information text,
                    object_id bigint,
                    region_id integer,
                    key_id bigserial,
                    CONSTRAINT " + tablePkey + @" PRIMARY KEY (key_id));";
            command.ExecuteNonQuery();

            logger.Info("Created table " + tableName);
        }

        /// <summary>
        /// Creates the Approval table.
        /// </summary>
        /// <param name="tableName">The table name.</param>
        /// <param name="tablePkey">The table primary key constraint.</param>
        /// <param name="command">The database command to use.</param>
        /// <param name="logger">The message logger.</param>
        private static void CreateApprovalTable(string tableName, string tablePkey, NpgsqlCommand command, ILog logger)
        {
            command.CommandText = @"CREATE TABLE IF NOT EXISTS " + tableName + @" ( 
                    update_time timestamp without time zone NOT NULL,
                    sensor_id text NOT NULL,
                    objective_id bigint NOT NULL,
                    approval_status text,
                    key_id bigserial,
                    CONSTRAINT " + tablePkey + @" PRIMARY KEY (key_id));";
            command.ExecuteNonQuery();

            logger.Info("Created table " + tableName);
        }
    }
}
