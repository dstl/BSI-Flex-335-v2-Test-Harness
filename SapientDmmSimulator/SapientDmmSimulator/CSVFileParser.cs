// Project:           $Project: 00010855-Sapient$
// File:              $Workfile: CSVFileParser.cs$
// Crown-owned copyright, 2021-2024
// See Release/Supply Conditions

namespace ScriptReader
{
    using System;
    using System.IO;
    using log4net;

    /// <summary>
    /// Read a CSV File in and parse it.
    /// </summary>
    public class CSVFileParser
    {
        /// <summary>
        /// Log4net logger.
        /// </summary>
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// CSV filename to read.
        /// </summary>
        private string fileName;

        /// <summary>
        /// file stream reader.
        /// </summary>
        private StreamReader sr = null;

        /// <summary>
        /// last line of text read from CSV file.
        /// </summary>
        private string lineOfText = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="CSVFileParser"/> class.
        /// </summary>
        /// <param name="filename">script filename.</param>
        public CSVFileParser(string filename)
        {
            fileName = filename;
        }

        /// <summary>
        /// Open CSV file.
        /// </summary>
        public void Open()
        {
            try
            {
                Log.InfoFormat("Opening CSV File:{0}", fileName);
                sr = new StreamReader(new FileStream(fileName, FileMode.Open, FileAccess.Read));
            }
            catch (FileNotFoundException)
            {
                Log.ErrorFormat("File Not Found: {0}", fileName);
            }
            catch (Exception ex)
            {
                Log.Error("Error Reading Script File:", ex);
            }
        }

        /// <summary>
        /// Close CSV file.
        /// </summary>
        public void Close()
        {
            if (sr != null)
            {
                sr.Dispose();
            }
        }

        /// <summary>
        /// Parse CSV file.
        /// </summary>
        /// <returns>flag whether text successfully read.</returns>
        public bool Parse()
        {
            bool textRead = false;
            try
            {
                if (sr.Peek() >= 0)
                {
                    lineOfText = sr.ReadLine();
                    textRead = true;
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error Parsing Script File:", ex);
            }

            return textRead;
        }

        /// <summary>
        /// Split CSV file line into a list of strings.
        /// </summary>
        /// <param name="result">list of strings.</param>
        public void ParseLineString(out string[] result)
        {
            result = null;
            char[] charSeparators = new char[] { ',' };
            if (lineOfText != null)
            {
                result = lineOfText.Split(charSeparators, StringSplitOptions.None);
            }
        }

        /// <summary>
        /// Show list of strings from a line of the CSV file
        /// </summary>
        /// <param name="sArray"></param>
        ////public void Show(string[] sArray)
        ////{
        ////    for (int i = 0; i < sArray.Length; i++)
        ////    {
        ////        Console.Write(": " + sArray[i]);
        ////    }
        ////    Console.Write("\n");
        ////}
    }
}
