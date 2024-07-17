// File:              $Workfile: SapientLogger.cs$
// <copyright file="SapientLogger.cs" >
// Crown-owned copyright, 2021-2024
//  See Release/Supply Conditions
// </copyright>

namespace SapientServices
{
    using System;
    using System.IO;
    using System.Xml.Linq;
    using log4net;

    /// <summary>
    /// Class to log sapient messages sent or received
    /// </summary>
    public class SapientLogger
    {
        private static readonly ILog Log4net = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public readonly string Prefix;
        public DateTime? LastOpenTime;
        public readonly TimeSpan IncrementInterval;
        public string FileName;
        public readonly string Directory;
        public int CurrentIndex;
        public string LastError;

        /// <summary>
        /// Initializes a new instance of the <see cref="SapientLogger" /> class.
        /// constructor to create logger
        /// </summary>
        /// <param name="directory">directory log files placed in</param>
        /// <param name="prefix">log filename prefix</param>
        /// <param name="increment_interval">interval at which logs are closed and new ones reopened</param>
        private SapientLogger(string directory, string prefix, TimeSpan increment_interval)
        {
            Prefix = prefix;
            Directory = directory;
            IncrementInterval = increment_interval;
        }

        /// <summary>
        /// Log message to current log file
        /// </summary>
        /// <param name="message"> candidate message to be logged</param>
        /// <returns>true if message logged</returns>
        public bool Log(string message)
        {
            lock (this)
            {
                LastError = string.Empty;
                try
                {
                    ////var doc = XDocument.Parse(xml_message);
                    ////var root = doc.Elements().First();
                    ////if (ValidForLogging(root))
                    {
                        var time = DateTime.UtcNow;
                        bool open_new_file;
                        if (LastOpenTime == null)
                        {
                            // no current file
                            CurrentIndex = 0;
                            open_new_file = true;
                        }
                        else if (time.Year != LastOpenTime.Value.Year || time.Month != LastOpenTime.Value.Month || time.Day != LastOpenTime.Value.Day)
                        {
                            // day changed
                            CurrentIndex = 0;
                            open_new_file = true;
                        }
                        else
                        {
                            // reopen interval expired
                            var dt = time - LastOpenTime.Value;
                            open_new_file = dt >= IncrementInterval;
                        }

                        if (open_new_file)
                        {
                            FileName = GenerateNewFileName(time);
                            LastOpenTime = time;
                        }

                        File.AppendAllLines(FileName, new[] { message });

                        return true;
                    }
                }
                catch (Exception ex)
                {
                    LastError = ex.ToString();
                }

                return false;
            }
        }

        /// <summary>
        /// Gneerate new unique log file name
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        private string GenerateNewFileName(DateTime time)
        {
            string filename = string.Empty;
            // find next available filename index
            bool exists = true;
            while (exists)
            {
                ++CurrentIndex;
                filename = Directory + @"\" + Prefix + "_" + time.Year + time.Month.ToString().PadLeft(2, '0') + time.Day.ToString().PadLeft(2, '0') + "_" + CurrentIndex + ".Log";
                exists = File.Exists(filename);
            }

            return filename;
        }

        /// <summary>
        /// filter so only required message types are logged (not acknowledgments)
        /// </summary>
        /// <param name="root"></param>
        /// <returns>true if message to be logged</returns>
        private bool ValidForLogging(XElement root)
        {
            bool retval = root.Name.LocalName == "SensorRegistration" || root.Name.LocalName == "StatusReport" || root.Name.LocalName == "DetectionReport" || root.Name.LocalName == "SensorTask" || root.Name.LocalName == "SensorTaskACK" || root.Name.LocalName == "Alert" || root.Name.LocalName == "AlertResponse";
            if (!retval)
            {
                Log4net.Info("Invalid MessageType:" + root.Name.LocalName);
                retval = true;
            }

            return retval;
        }

        /// <summary>
        /// Function to create a logger if settings valid
        /// </summary>
        /// <param name="log_directory">directory to store log files</param>
        /// <param name="log_prefix">log filename prefix</param>
        /// <param name="increment_interval_seconds">interval at which to switch log files</param>
        /// <returns></returns>
        public static SapientLogger CreateLogger(string log_directory, string log_prefix, int increment_interval_seconds)
        {
            return !string.IsNullOrWhiteSpace(log_prefix) && System.IO.Directory.Exists(log_directory)
                       ? new SapientLogger(log_directory, log_prefix, TimeSpan.FromSeconds(increment_interval_seconds))
                       : null;
        }
    }
}
