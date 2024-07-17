// Project:           $Project: 00010855-Sapient$
// File:              $Workfile: RegistrationQueries.cs$
// Crown-owned copyright, 2021-2024
//  See Release/Supply Conditions
// $NoKeywords$
namespace SapientDatabase.DatabaseTables
{
    using Google.Protobuf.WellKnownTypes;
    using log4net;
    using Npgsql;
    using Sapient.Data;
    using System;
    using System.Reflection;

    /// <summary>
    /// Database methods relating to Registration messages
    /// </summary>
    public class RegistrationQueries
    {
        /// <summary>
        /// Log4net logger.
        /// </summary>
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        ///  Method to write registration message to database
        /// </summary>
        /// <param name="message">registration object</param>
        /// <returns>SQL string</returns>
        public static string GenerateInsertQueryString(SapientMessage message)
        {
            DateTime updateTime = DateTime.UtcNow;
            string messageTimeStamp = message.Timestamp.ToDateTime().ToString(DatabaseUtil.DateTimeFormat);
            string updateTimeStamp = updateTime.ToString(DatabaseUtil.DateTimeFormat);
            Registration.Types.NodeType platformType;

            if (!message.Registration.NodeDefinition.Any())
            {
                platformType = Registration.Types.NodeType.Other;
            }
            else
            {
                var nodeDefinition = message.Registration.NodeDefinition[0];
                platformType = nodeDefinition.NodeType;
            }

            var str_msg = message.ToString();
            string sqlString = $"INSERT INTO sensor_registration_BSIFlex335v2 VALUES('{messageTimeStamp}', '{updateTimeStamp}', '{message.NodeId}', '{message.Registration.NodeDefinition}', '{str_msg}', '{string.Join(",", platformType)}');";

            return sqlString;
        }
    }
}
