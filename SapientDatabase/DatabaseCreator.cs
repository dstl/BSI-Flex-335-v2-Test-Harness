// Project:           $Project: 00010855-Sapient$
// File:              $Workfile: DatabaseCreator.cs$
// Crown-owned copyright, 2021-2024
//  See Release/Supply Conditions

namespace SapientDatabase
{
    using System;
    using System.Reflection;
    using log4net;
    using Npgsql;

    /// <summary>
    /// Create Database
    /// </summary>
    public static class DatabaseCreator
    {
        /// <summary>
        /// Log4net logger.
        /// </summary>
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Method to check if database exists
        /// </summary>
        /// <param name="host">server on which to connect</param>
        /// <param name="port">port to access database</param>
        /// <param name="user">user name for the database</param>
        /// <param name="password">password to access the database</param>
        /// <param name="databasename">new database</param>
        public static bool DatabaseExists(string host, string port, string user, string password, string databasename)
        {
            bool exists = false;
            try
            {
                string connectionString = "Server=" + host + ";Port=" + port + ";User Id=" + user + ";Password=" + password + ";Database=postgres;";

                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    string existsCommandString = string.Format(@"select exists(SELECT datname FROM pg_catalog.pg_database WHERE datname = '{0}');", databasename);

                    connection.Open();

                    using (NpgsqlCommand existsCmd = new NpgsqlCommand(existsCommandString, connection))
                    {
                        exists = (bool)existsCmd.ExecuteScalar();
                        if (!exists)
                        {
                            Log.WarnFormat("Database:{0} does not exist", databasename);
                        }
                        else
                        {
                            Log.InfoFormat("Database {0} Exists", databasename);
                        }
                    }
                    connection.Close();
                }
            }
            catch (NpgsqlException nex)
            {
                if ((uint)nex.ErrorCode == 0x80004005)
                {
                    Log.ErrorFormat("Cannot connect to database server on Host:{0} Port:{1}. Check these settings and then check that the database server is installed and running.", host, port);
                }
                else
                {
                    Log.Error("SQL Error checking database existence");
                    Log.Error(nex);
                }
            }
            catch (Exception e)
            {
                Log.ErrorFormat("Error checking database existence");
                Log.Error(e);
            }

            return exists;
        }

        /// <summary>
        /// Method to create empty database with given name
        /// </summary>
        /// <param name="host">server on which to connect</param>
        /// <param name="port">port to access database</param>
        /// <param name="user">user name for the database</param>
        /// <param name="password">password to access the database</param>
        /// <param name="databasename">new database</param>
        public static void CreateDatabase(string host, string port, string user, string password, string databasename)
        {
            try
            {
                string connectionString = "Server=" + host + ";Port=" + port + ";User Id=" + user + ";Password=" + password + ";Database=postgres;";

                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    string existsCommandString = string.Format(@"select exists(SELECT datname FROM pg_catalog.pg_database WHERE datname = '{0}');", databasename);

                    string createCommandString = string.Format("CREATE DATABASE \"{0}\" WITH OWNER = postgres ENCODING = 'UTF8' CONNECTION LIMIT = -1;", databasename);
                    connection.Open();

                    using (NpgsqlCommand existsCmd = new NpgsqlCommand(existsCommandString, connection))
                    using (NpgsqlCommand createCmd = new NpgsqlCommand(createCommandString, connection))
                    {
                        bool exists = (bool)existsCmd.ExecuteScalar();
                        if (!exists)
                        {
                            Log.WarnFormat("Creating Database:{0}", databasename);
                            Log.Info(createCommandString);
                            createCmd.ExecuteNonQuery();
                        }
                        else
                        {
                            Log.InfoFormat("Database {0} Exists", databasename);
                        }
                    }
                    connection.Close();
                }
            }
            catch (NpgsqlException nex)
            {
                if ((uint)nex.ErrorCode == 0x80004005)
                {
                    Log.ErrorFormat("Cannot connect to database server on Host:{0} Port:{1}. Check these settings and then check that the database server is installed and running.", host, port);
                }
                else
                {
                    Log.Error("SQL Error creating database");
                    Log.Error(nex);
                }
            }
            catch (Exception e)
            {
                Log.Error("Error creating database");
                Log.Error(e);
            }
        }

        /// <summary>
        /// method to built all the tables needed for the test harness.
        /// Does a simple check to make sure we have the correct number of tables already
        /// </summary>
        /// <param name="connection">The database connection to use.</param>
        public static void CreateTables(NpgsqlConnection connection)
        {
            using (var command = new NpgsqlCommand())
            {
                command.Connection = connection;
                command.CommandText = @"SELECT count(*) FROM pg_tables WHERE schemaname = 'public';";

                long count = 0;
                using (var da = command.ExecuteReader())
                {
                    while (da.Read())
                    {
                        count = da.GetInt64(0);
                    }
                }

                // this stops the re-running of the table build commands if we already have the correct number of tables
                if (count < 16)
                {
                    DatabaseTables.DetectionTableCreator.Create(connection, Log);
                    DatabaseTables.StatusReportTableCreator.Create(connection, Log);
                    DatabaseTables.AlertTableCreator.Create(connection, Log);
                    DatabaseTables.TaskingTablesCreator.Create(connection, Log);

                    // Zodiac tables
                    DatabaseTables.ObjectiveTableCreator.Create(connection, Log);
                    DatabaseTables.RoutePlanTableCreator.Create(connection, Log);

                    DatabaseTables.SensorLocationOffsetTableCreator.Create(connection, Log);

                    ////CreateGroundTruthTable(connection);
                    CreateTriggers(connection);

                    command.CommandText = @"CREATE TABLE IF NOT EXISTS sensor_registration_BSIFlex335v2 ( 
                    message_time Timestamp without time Zone NOT NULL,
                    update_time Timestamp without time Zone NOT NULL,
                    sensor_id text NOT NULL,	            
                    sensor_type text,
                    text xml,
                    platform_type text,
                    key_id bigserial,
                    CONSTRAINT sensor_registration_pkey PRIMARY KEY (key_id));";

                    command.ExecuteNonQuery();
                    Log.Info("sensor_registration_BSIFlex335v2 in db");

                    // additional indices for Cubica DMM
                    command.CommandText = @"CREATE INDEX sensor_registration_time_source_id
                    ON sensor_registration_BSIFlex335v2 USING btree (message_time, sensor_id);";
                    command.ExecuteNonQuery();
                    Log.Info("sensor_registration_BSIFlex335v2 index in db");
                }
            }
        }

        /// <summary>
        /// Creates a table for holding ground truth information.
        /// </summary>
        /// <param name="connection">The database connection to use.</param>
        public static void CreateGroundTruthTable(NpgsqlConnection connection)
        {
            var command = new NpgsqlCommand { Connection = connection };
            var table_prefix = "ground_truth";

            // create sequence to provide key for all ground truth rows.
            command.CommandText = @"CREATE SEQUENCE " + table_prefix + "_id_seq";
            Log.Debug(command.CommandText);
            command.ExecuteNonQuery();

            // create the ground truth table.
            command.CommandText = @"CREATE TABLE IF NOT EXISTS " + table_prefix + @"_BSIFlex335v2 ( 
                source_id bigint NOT NULL,
                report_time Timestamp without time Zone NOT NULL,
                update_time Timestamp without time Zone NOT NULL,                             
                report_id bigint NOT NULL,
                object_id bigint NOT NULL,
                X double precision NOT NULL,
                Y double precision NOT NULL,
                Z double precision,
                detection_id bigint DEFAULT nextval('" + table_prefix + @"_id_seq') NOT NULL,
                CONSTRAINT " + table_prefix + @"_pkey PRIMARY KEY (detection_id));";
            Log.Debug(table_prefix + @"_BSIFlex335v2 in db");
            command.ExecuteNonQuery();
        }

        /// <summary>
        /// Creates Table Trigger Functions
        /// </summary>
        /// <param name="connection">The database connection to use.</param>
        public static void CreateTriggers(NpgsqlConnection connection)
        {
            using (var command = new NpgsqlCommand { Connection = connection })
            {
                // Create the Region Trigger Function
                command.CommandText = @"CREATE OR REPLACE FUNCTION notify_region()
				RETURNS trigger AS
			$BODY$
			BEGIN
				PERFORM pg_notify('notify_region', 'Region Updated');
				RETURN NULL;
			END;
			$BODY$
				LANGUAGE plpgsql VOLATILE
				COST 100;
			ALTER FUNCTION notify_region()
				OWNER TO postgres;";
                Log.Debug("notify_region Trigger Function Created");
                command.ExecuteNonQuery();

                command.CommandText = "CREATE TRIGGER \"Notify_Region\" AFTER INSERT OR UPDATE OR DELETE ON hl_task_region_BSIFlex335v2 FOR EACH ROW EXECUTE PROCEDURE notify_region();";
                Log.Debug("notify_region Trigger bound to hl_task_region_BSIFlex335v2");
                command.ExecuteNonQuery();
            }
        }
    }
}
