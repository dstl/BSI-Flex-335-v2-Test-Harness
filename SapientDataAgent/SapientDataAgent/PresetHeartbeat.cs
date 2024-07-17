// -----------------------------------------------------------------------
// <copyright file="PresetHeartbeat.cs" >
// Crown-Owned Copyright (c)
// </copyright>
// -----------------------------------------------------------------------

namespace SapientMiddleware
{
    using System;
    using System.IO;
    using Sapient.Data;

    /// <summary>
    /// The PresetHeartbeat class.
    /// </summary>
    public class PresetHeartbeat
    {
        private StatusReport statusReport;

        private Registration registration;

        /// <summary>
        /// Initializes a new instance of the <see cref="PresetHeartbeat"/> class.
        /// </summary>
        public PresetHeartbeat()
        {
            statusReport = new StatusReport();
            registration = new Registration();
        }

        /// <summary>
        /// Loads the registration.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        public void LoadRegistration(string fileName)
        {
            try
            {
                Console.WriteLine("Opening Reg File:" + fileName + "\n");
                StreamReader sr = new StreamReader(new FileStream(fileName, FileMode.Open, FileAccess.Read));
                string message = sr.ReadToEnd();
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("File Not Found: " + fileName);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
