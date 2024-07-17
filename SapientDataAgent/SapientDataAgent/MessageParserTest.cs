// Project:           $Project: 00010855-Sapient$
// File:              $Workfile: MessageParserTest.cs$
// Crown-owned copyright, 2021-2024
//  See Release/Supply Conditions
// $NoKeywords$

namespace SapientMiddleware
{
    using System;
    using SapientDatabase;
    using SapientMiddleware.Common;

    /// <summary>
    /// Unit test class to wrap message parser.
    public class MessageParserTest
    {
        private SapientMessageParser messageParser;

        private string msg = @"<?xml version=""1.0""?><DetectionReport>
            <timestamp>2019-05-29T07:41:59.106Z</timestamp>
	        <sourceID>501</sourceID>
	        <reportID>677</reportID>
	        <objectID>1</objectID>
	        <taskID>0</taskID>
	        <rangeBearing>
		        <R>1000</R>
		        <Az>133.384</Az>
		        <eR>1000</eR>
	        </rangeBearing>
	        <objectInfo type = ""StartFrequency(MHz)"" value=""2469.116""/>
	        <objectInfo type = ""CentreFrequency(MHz)"" value=""2469.986""/>
	        <objectInfo type = ""StopFrequency(MHz)"" value=""2470.856""/>
	        <class type=""RF Comms""/>
        </DetectionReport>";

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageParserTest" /> class.
        /// </summary>
        public MessageParserTest()
        {
            messageParser = new SapientMessageParser(new SapientDataAgentClientProtocol(Properties.Settings.Default.ForwardAlerts, Guid.NewGuid().ToString()), true);
            Program.previouslyConnectedASMs.AddOrUpdate("501", DateTime.Now, (key, previousValue) => DateTime.Now);
        }

        /// <summary>
        /// Unit test.
        /// </summary>
        /// <param name="database">Database Object.</param>
        public void Test(Database database)
        {
            int loop = 0;
            for (loop = 0; loop < 100000; loop++)
            {
                string msgwithterm = msg + "\0" + msg + "\0" + msg + "\0" + msg + "\0" + msg + "\0" + msg + "\0" + msg + "\0";
                GC.Collect();
                // messageParser.BuildAndNotify(msgwithterm, database, 1);
            }
        }
    }
}
