// Project:           $Project: 00010855-Sapient$
// File:              $Workfile: DatabaseUtil.cs$
// Crown-owned copyright, 2021-2024
//  See Release/Supply Conditions
// $NoKeywords$
namespace SapientDatabase
{
    using System;
    using log4net;
    using Npgsql;

    /// <summary>
    /// Database Utility Methods
    /// </summary>
    public static class DatabaseUtil
    {
        /// <summary>
        /// Date Time string format to use when writing to database
        /// </summary>
        public const string DateTimeFormat = "yyyy-MM-dd HH:mm:ss.fff";

        /// <summary>
        /// Nullable method
        /// </summary>
        /// <typeparam name="T">template key</typeparam>
        /// <typeparam name="U">template value</typeparam>
        /// <param name="obj">object</param>
        /// <param name="func">lambda function</param>
        /// <param name="specified">whether specified</param>
        /// <returns>parameter value or NULL string</returns>
        public static string Nullable<T, U>(T obj, Func<T, U> func, Func<T, bool> specified) where T : class
        {
            if (obj == null)
            {
                return "NULL";
            }

            return specified(obj) ? func(obj).ToString() : "NULL";
        }

        /// <summary>
        /// Nullable method
        /// </summary>
        /// <typeparam name="T">template</typeparam>
        /// <param name="obj">object</param>
        /// <param name="func">lambda function</param>
        /// <returns>parameter value or NULL string</returns>
        public static string Nullable<T>(T obj, Func<T, string> func) where T : class
        {
            if (obj == null)
            {
                return "NULL";
            }

            var val = func(obj);
            return string.IsNullOrEmpty(val) == false ? '\'' + val + '\'' : "NULL";
        }

        /// <summary>
        /// Nullable method
        /// </summary>
        /// <typeparam name="T">template key</typeparam>
        /// <typeparam name="U">template value</typeparam>
        /// <param name="obj">object</param>
        /// <param name="func">lambda function</param>
        /// <returns>parameter value or NULL string</returns>
        public static string Nullable<T>(T obj, Func<T, double> func) where T : class
        {
            if (obj == null)
            {
                return "NULL";
            }

            return func(obj) != null ? func(obj).ToString() : "NULL";
        }

        /// <summary>
        /// Nullable method
        /// </summary>
        /// <typeparam name="T">template key</typeparam>
        /// <typeparam name="U">template value</typeparam>
        /// <param name="obj">object</param>
        /// <param name="func">lambda function</param>
        /// <returns>parameter value or NULL string</returns>
        public static string Nullable<T>(T obj, Func<T, DateTime> func) where T : class
        {
            if (obj == null)
            {
                return "NULL";
            }

            var val = func(obj);
            return val != null ? '\'' + val.ToString() + '\'' : "NULL";
        }

        /// <summary>
        /// Nullable method
        /// </summary>
        /// <typeparam name="T">template</typeparam>
        /// <param name="val">parameter value</param>
        /// <param name="specified">whether parameter specified</param>
        /// <returns>parameter value or NULL string</returns>
        public static string Nullable<T>(T val, bool specified)
        {
            return specified ? val.ToString() : "NULL";
        }

        /// <summary>
        /// Nullable method
        /// </summary>
        /// <param name="val">parameter value</param>
        /// <param name="specified">whether parameter specified</param>
        /// <returns>parameter value or NULL string</returns>
        public static string Nullable(DateTime val, bool specified)
        {
            return specified ? '\'' + val.ToString(DateTimeFormat) + '\'' : "NULL";
        }

        /// <summary>
        /// Creates a database connection string for accessing the postgres database.
        /// </summary>
        /// <param name="host">The host where the database is located.</param>
        /// <param name="port">The connection port.</param>
        /// <param name="user">The database user.</param>
        /// <param name="password">The database user password.</param>
        /// <returns>The connection string.</returns>
        public static string CreatePostgresDbString(string host, string port, string user, string password)
        {
            string connectionString = "Server=" + host + ";Port=" + port + ";User Id=" + user + ";Password=" + password + ";Database=postgres;";

            return connectionString;
        }

        /// <summary>
        /// Checks if the named database exists.
        /// </summary>
        /// <param name="connection">The database connection to use.</param>
        /// <param name="databaseName">The name of the database.</param>
        /// <returns>True if the database is exists.</returns>
        public static bool CheckDatabaseExists(NpgsqlConnection connection, string databaseName)
        {
            bool exists = false;

            string existsCommandString = string.Format(@"select exists(SELECT datname FROM pg_catalog.pg_database WHERE datname = '{0}');", databaseName);

            using (var cmd = new NpgsqlCommand(existsCommandString, connection))
            {
                exists = (bool)cmd.ExecuteScalar();
            }

            return exists;
        }

        /// <summary>
        /// Gets a count of the number of tables in the database.
        /// </summary>
        /// <param name="connection">The database connection.</param>
        /// <returns>The number of tables.
        /// </returns>
        public static long CountTables(NpgsqlConnection connection)
        {
            long count = 0;

            var countString = @"SELECT count(*) FROM pg_tables WHERE schemaname = 'public';";

            using (var command = new NpgsqlCommand(countString, connection))
            {
                count = (long)command.ExecuteScalar();
            }

            return count;
        }

        /// <summary>
        /// Creates the specified database.
        /// </summary>
        /// <param name="connection">The database connection to use.</param>
        /// <param name="databaseName">The name of the database.</param>
        /// <param name="user">The owner of the database.</param>
        public static void CreateDatabase(NpgsqlConnection connection, string databaseName, string user)
        {
            string createCommandString = string.Format("CREATE DATABASE \"{0}\" WITH OWNER = \"{1}\" ENCODING = 'UTF8' CONNECTION LIMIT = -1;", databaseName, user);

            using (var cmd = new NpgsqlCommand(createCommandString, connection))
            {
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Drops the specified database if it exists.
        /// </summary>
        /// <param name="connection">The database connection to use.</param>
        /// <param name="databaseName">The name of the database.</param>
        public static void DropDatabase(NpgsqlConnection connection, string databaseName)
        {
            string dropCmdString = string.Format("DROP DATABASE IF EXISTS \"{0}\";", databaseName);

            using (var cmd = new NpgsqlCommand(dropCmdString, connection))
            {
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Creates the specified sequence.
        /// </summary>
        /// <param name="sequenceName">The name of the sequence.</param>
        /// <param name="command">The database command to use.</param>
        /// <param name="logger">The message logger.</param>
        public static void CreateSequence(string sequenceName, NpgsqlCommand command, ILog logger)
        {
            command.CommandText = @"CREATE SEQUENCE " + sequenceName;
            command.ExecuteNonQuery();

            logger.Info("Created sequence " + sequenceName);
        }
    }
}
