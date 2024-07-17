// Project:           $Project: 00010855-Sapient$
// File:              $Workfile: ScriptForm.cs$
// Crown-owned copyright, 2021-2024
//  See Release/Supply Conditions

namespace ScriptReader
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Threading;
    using System.Windows.Forms;
    using log4net;
    using Sapient.Data;
    using SapientServices;
    using SapientServices.Communication;
    using SapientServices.Data;
    using SapientServices.Data.Validation;

    /// <summary>
    /// The ScriptForm class.
    /// </summary>
    /// <seealso cref="System.Windows.Forms.Form" />
    public partial class ScriptForm : Form
    {
        /// <summary>
        /// Log4net logger.
        /// </summary>
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private string directory;

        /// <summary>
        /// used for passing filename from single file click to process thread.
        /// </summary>
        private string currentFilename;

        private IConnection messenger;
        private Dictionary<string, string> expectedErrors;

        private List<string> jsonList = new List<string>();

        private string output;

        /// <summary>
        /// current message string buffer.
        /// </summary>
        private readonly StringBuilder currentMessage = new StringBuilder();

        private BlockingCollection<string> messageQueue;

        private List<string> DMMFolders = new List<string>()
        {
            "Alert",
            "AlertResponse",
            "SensorTask",
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="ScriptForm" /> class.
        /// Initialise the scriptform window and build the validation list from file.
        /// </summary>
        /// <param name="messenger">The messenger.</param>
        public ScriptForm(IConnection messenger)
        {
            InitializeComponent();
            directory = string.Empty;
            this.messenger = messenger;
            messageQueue = new BlockingCollection<string>();
        }

        /// <summary>
        /// callback method for communication.
        /// </summary>
        /// <param name="msg_buffer">message buffer.</param>
        /// <param name="size">message size.</param>
        /// <param name="client">client connection.</param>
        public void DataCallback(SapientMessage message, IConnection client)
        {
            if (message != null)
            {
                if (message.ContentCase == SapientMessage.ContentOneofCase.Error)
                {
                    try
                    {
                        var e = message.Error;
                        foreach (string errorMessage in e.ErrorMessage)
                        {
                            messageQueue.Add(errorMessage);
                            Log.InfoFormat("queue size:{0} {1}", messageQueue.Count, errorMessage);
                        }
                    }
                    catch (Exception e)
                    {
                        Log.ErrorFormat("Unable to deserialise error message:{0} Exception:{1}", message, e);
                    }

                    currentMessage.Clear();
                }
                else
                {
                    Log.InfoFormat("non-error message received:{0}", message);
                    currentMessage.Clear();
                }
            }
        }

        /// <summary>
        /// Parse test txt file to get list of XML files under test.
        /// </summary>
        /// <param name="filename"></param>
        public void LoadScript(string filename)
        {
            CSVFileParser parser = new CSVFileParser(filename);
            directory = Path.GetDirectoryName(filename);
            output += "Directory:" + directory + "\r\n";

            parser.Open();

            while (parser.Parse())
            {
                string[] linedata;

                // Parse a line of data
                parser.ParseLineString(out linedata);
                if (linedata[0] != "")
                {
                    listBox1.Items.Add(linedata[0]);
                    jsonList.Add(directory + "\\" + linedata[0]);
                }
            }

            listBox1.Items.Add("END");
            listBox1.SelectedIndex = 0;
            parser.Close();
            expectedErrors = BuildListOfExpectedErrors();
        }

        /// <summary>
        /// Clear the list of xml files to be tested.
        /// </summary>
        private void ClearFileList()
        {
            listBox1.Items.Clear();
            jsonList.Clear();
        }

        /// <summary>
        /// Clear message queue ready for new data.
        /// </summary>
        private void ClearMessageQueue()
        {
            while (messageQueue.Count > 0)
            {
                string oldString;
                messageQueue.TryTake(out oldString);
            }
        }

        /// <summary>
        /// Write the JSON contents to the message stream.
        /// </summary>
        /// <param name="filename">Json input filename.</param>
        private void ReadFile(string filename)
        {
            try
            {
                if (!filename.Contains("\\") && directory != string.Empty)
                {
                    if (!directory.EndsWith("/"))
                    {
                        directory += "/";
                    }

                    filename = directory + filename;
                }

                string message = File.ReadAllText(filename);

                var parsedMessage = SapientMessage.Parser.ParseJson(message);

                messenger.SendMessage(parsedMessage);

                Log.InfoFormat("Sent " + Path.GetFileName(filename));
            }
            catch (FileNotFoundException fex)
            {
                AppendTextBox("file not found:" + fex.Message);
            }
            catch (DirectoryNotFoundException direx)
            {
                AppendTextBox("directory not found:" + direx.Message);
            }
            catch (Exception ex)
            {
                AppendTextBox("error reading file:" + filename);
                AppendTextBox(ex.Message);
            }
        }

        /// <summary>
        /// Append a message to the Output textbox.
        /// </summary>
        /// <param name="value"></param>
        private void AppendTextBox(string value)
        {
            // allow background thread to write to the output window
            if (InvokeRequired)
            {
                Invoke(new Action<string>(AppendTextBox), new object[] { value });
                return;
            }

            outputWindow.Text += value + "\r\n";
        }

        /// <summary>
        /// Add a message to the beginning of the output textbox, used to summarise test.
        /// </summary>
        /// <param name="value"></param>
        private void PrependTextBox(string value)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<string>(PrependTextBox), new object[] { value });
                return;
            }

            outputWindow.Text = value + "\r\n" + outputWindow.Text;
        }

        /// <summary>
        /// Cycle through the json list sending files.
        /// </summary>
        private void ProcessJsonList()
        {
            bool testSuccess = true;

            if (jsonList.Count == 0)
            {
                PrependTextBox("No messages in json test data\r\n");
                return;
            }

            ClearMessageQueue();

            for (int i = 0; i < jsonList.Count; i++)
            {
                if (jsonList[i] != "")
                {
                    string filename = jsonList[i];

                    string fileExtension = ".json";
                    if (!filename.ToLower().Contains(fileExtension))
                    {
                        filename += fileExtension;
                    }

                    ReadFile(filename);
                    Thread.Sleep(100);
                    if (!HandleResponse(filename)) testSuccess = false;
                }
            }

            if (testSuccess)
            {
                PrependTextBox("PASS: All messages returned as expected\r\n");
            }
            else
            {
                PrependTextBox("FAIL: Some messages did not return the expected response\r\n");
            }
        }

        /// <summary>
        /// handle response from file sent, checking validity and flagging if no response received.
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        private bool HandleResponse(string filename)
        {
            string expectedErrorMessage = null;
            string queuedResponse = null;

            try
            {
                // error messages may contain carriage returns, these are removed for comparison
                expectedErrorMessage = GetExpectedErrorMessage(Path.GetFileName(filename)).Replace("\n", " ");

                Log.InfoFormat("Awaiting Response for {0}", filename);

                messageQueue.TryTake(out queuedResponse, 1000);

                if (queuedResponse == null)
                {
                    AppendTextBox("");
                    AppendTextBox("Fail: " + Path.GetFileName(filename));
                    AppendTextBox("Expected: " + expectedErrorMessage);
                    AppendTextBox("Did not receive a response");
                    AppendTextBox("");
                    Log.Info("Did not receive a response");
                    return false;
                }

                if (expectedErrorMessage == null)
                {
                    AppendTextBox("");
                    AppendTextBox("Fail: Unknown response expected from file: " + Path.GetFileName(filename));
                    AppendTextBox("Do you need to include an expected response in: " + SapientDmmSimulator.Properties.Settings.Default.ExpectedResponseFileName);
                    AppendTextBox("");
                    return false;
                }

                queuedResponse = queuedResponse.Replace("\n", " ");
                if (expectedErrorMessage == queuedResponse)
                {
                    AppendTextBox("Pass: " + Path.GetFileName(filename));
                    return true;
                }
                else
                {
                    AppendTextBox("");
                    AppendTextBox("Fail: " + Path.GetFileName(filename));
                    AppendTextBox("Expected: " + expectedErrorMessage);
                    AppendTextBox("Received: " + queuedResponse);
                    AppendTextBox("");
                    return false;
                }
            }
            catch (Exception e)
            {
                Log.Error("Error Parsing file:", e);
                AppendTextBox("Error Parsing file. See log for details.");
            }

            return false;
        }

        /// <summary>
        /// Returns the expected error message from the expected responses file, or throws and error if not found.
        /// </summary>
        /// <param name="jsonKey"></param>
        /// <returns></returns>
        private string GetExpectedErrorMessage(string jsonKey)
        {
            string expectedMessage;
            if (expectedErrors.ContainsKey(jsonKey))
            {
                expectedMessage = expectedErrors[jsonKey];
            }
            else
            {
                throw new Exception("Expected error message could not be found");
            }

            return expectedMessage;
        }

        /// <summary>
        /// Builds expected error list from file specified in settings.
        /// </summary>
        /// <returns>The dictionary of text lines.</returns>
        private Dictionary<string, string> BuildListOfExpectedErrors()
        {
            // init building dictionary
            Dictionary<string, string> buildingDictionary = new Dictionary<string, string>();

            try
            {
                var filename = SapientDmmSimulator.Properties.Settings.Default.ExpectedResponseFileName;
                if (!string.IsNullOrWhiteSpace(directory))
                {
                    if (!directory.EndsWith("/"))
                    {
                        directory += "/";
                    }

                    filename = directory + filename;
                }

                // get all txt lines from file
                var responses = File.ReadLines(filename);

                char DELIM = '~';

                // iterate through lines
                foreach (var line in responses)
                {
                    // max of two elements (key and value)
                    const int MAX = 1;
                    string[] obtained = new string[MAX];

                    try
                    {
                        // only add if the ~ deliminator is in the line
                        if (line.Contains(DELIM.ToString()))
                        {
                            // split line into a @string[] array with @string key and @string value
                            obtained = line.Split(DELIM);

                            // add obtained key and values to dictionary
                            buildingDictionary.Add(obtained[0], obtained[1]);
                        }
                        else
                        {
                            Console.WriteLine("Required delim not available, continuing...");
                        }
                    }
                    catch (IndexOutOfRangeException)
                    {
                        Console.WriteLine("Trying to access an element out of range, continuing...");
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error Reading file");
                Console.WriteLine(e);
            }

            return buildingDictionary;
        }

        /// <summary>
        /// Validate the responses when the loaded XML files are sent.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void SendJsonList_Click(object sender, EventArgs e)
        {
            outputWindow.Clear();
            new Thread(ProcessJsonList).Start();
        }

        /// <summary>
        /// Load a list of XML files for sending.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void LoadJsonList_Click(object sender, EventArgs e)
        {
            outputWindow.Clear();
            OpenFileDialog dlg = new OpenFileDialog();
            DialogResult result = dlg.ShowDialog();
            if (result == DialogResult.OK)
            {
                string filename = dlg.FileName;
                ClearFileList();
                LoadScript(filename);
            }
        }

        /// <summary>
        /// Send all JSON files pertaining to the DMM from the folder specified in settings.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void SendAllJson_Click(object sender, EventArgs e)
        {
            outputWindow.Clear();
            string[] allfiles = Directory.GetFiles(SapientDmmSimulator.Properties.Settings.Default.JSONTestMessagesRoot, "*.txt", SearchOption.AllDirectories);
            jsonList = new List<string>();
            ClearFileList();
            foreach (string file in allfiles)
            {
                // JSON files for both the ASM and DMM are stored in sibling directories, only want to send DMM ones
                string parentFolder = Path.GetDirectoryName(file).Replace(Path.GetDirectoryName(Path.GetDirectoryName(file)), "").Trim('\\');
                if (DMMFolders.Contains(parentFolder))
                {
                    LoadScript(file);
                }
            }

            new Thread(ProcessJsonList).Start();
        }

        /// <summary>
        /// Manually step through the XML file list and pass file name for sending.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void NextButton_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex < listBox1.Items.Count - 1)
            {
                string filename = listBox1.SelectedItem.ToString();
                string fileExtension = ".json";
                if (!filename.ToLower().Contains(fileExtension))
                {
                    filename += fileExtension;
                }

                ReadFile(filename);

                currentFilename = filename;

                new Thread(HandleSingleFileResponse).Start();

                listBox1.SelectedIndex++;
            }
        }

        /// <summary>
        /// Handle Response for single file click button.
        /// </summary>
        private void HandleSingleFileResponse()
        {
            HandleResponse(currentFilename);
        }
    }
}
