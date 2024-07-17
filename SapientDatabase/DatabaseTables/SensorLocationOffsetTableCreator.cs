// Project:           $Project: 00010855-Sapient$
// File:              $Workfile: SensorLocationOffsetTableCreator.cs$
// Crown-owned copyright, 2021-2024
//  See Release/Supply Conditions
// $NoKeywords$
namespace SapientDatabase.DatabaseTables
{
    using log4net;
    using Npgsql;

    /// <summary>
    /// creator of sensor location offset table
    /// </summary>
    public class SensorLocationOffsetTableCreator
    {
        /// <summary>
        /// Creates the.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="logger">The logger.</param>
        public static void Create(NpgsqlConnection connection, ILog logger)
        {
            using (var command = new NpgsqlCommand { Connection = connection })
            {
                CreateLocationOffsetTable(SensorLocationOffsetConstants.Table, SensorLocationOffsetConstants.Pkey, command, logger);
            }
        }

        /// <summary>
        /// Creates the sensor location offset table.
        /// </summary>
        /// <param name="tableName">The table name.</param>
        /// <param name="tablePkey">The table primary key constraint.</param>
        /// <param name="command">The database command to use.</param>
        /// <param name="logger">The message logger.</param>
        private static void CreateLocationOffsetTable(string tableName, string tablePkey, NpgsqlCommand command, ILog logger)
        {
            command.CommandText = @"CREATE TABLE IF NOT EXISTS " + tableName + @" (
                    update_time timestamp without time zone NOT NULL DEFAULT CURRENT_TIMESTAMP,
                    sensor_id text NOT NULL UNIQUE,
                    x_offset  double precision NOT NULL,
                    y_offset  double precision NOT NULL,
                    z_offset  double precision NOT NULL,
                    az_offset  double precision NOT NULL,
                    ele_offset  double precision NOT NULL,
                    key_id bigserial,
                    CONSTRAINT " + tablePkey + @" PRIMARY KEY (key_id));";
            command.ExecuteNonQuery();

            logger.Info("Created table " + tableName);
        }
    }
}
