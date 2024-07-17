// File:              $Workfile: ScriptForm.cs$
// <copyright file="ScriptForm.cs" >
// Crown-owned copyright, 2021-2024
// See Release/Supply Conditions
// </copyright>


namespace ScriptReader
{
    using log4net;
    using Sapient.Data;
    using SapientServices;
    using SapientServices.Communication;
    using SapientServices.Data.Validation;
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Threading;
    using System.Windows.Forms;

    /// <summary>
    /// The ScriptForm class.
    /// </summary>
    /// <seealso cref="System.Windows.Forms.Form" />
    public partial class ScriptForm : Form
    {
        #region Class Parameters

        /// <summary>
        /// Log4net logger
        /// </summary>
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private string directory;

        /// <summary>
        /// used for passing filename from single file click to process thread
        /// </summary>
        private string currentFilename;

        private ISender messenger;

        private Dictionary<string, string> expectedErrors;

        private List<string> messageList = new List<string>();

        private string output;

        /// <summary>
        /// current message string buffer
        /// </summary>
        private readonly StringBuilder currentMessage = new StringBuilder();

        private BlockingCollection<string> messageQueue;

        private List<string> ASMFolders = new List<string>()
        {
            "DetectionReport",
            "SensorRegistration",
            "StatusReport",
            "SensorTaskAck"
        };
        #endregion Class Parameters

        #region Public Methods
        /// <summary>
        /// Initialise the scriptform window and build the validation list from file
        /// </summary>
        /// <param name="messenger"></param>
        public ScriptForm(ISender messenger)
        {
            InitializeComponent();
            directory = string.Empty;
            this.messenger = messenger;
            this.messageQueue = new BlockingCollection<string>();
        }

        /// <summary>
        /// callback method for communication 
        /// </summary>
        /// <param name="msg_buffer">message buffer</param>
        /// <param name="size">message size</param>
        /// <param name="client">client connection</param>
        public void DataCallback(SapientMessage message, IConnection client)
        {
            ProtobufMessageParser(message);
        }

        /// <summary>
        /// Protobufs the message parser.
        /// </summary>
        /// <param name="msg_buffer">The MSG buffer.</param>
        /// <param name="size">The size.</param>
        private void ProtobufMessageParser(SapientMessage message)
        {
            if (message != null)
            {
                if (message.Error != null)
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
                        Log.ErrorFormat("Unable to deserialise error message:{0}", e);
                    }

                    currentMessage.Clear();
                }
                else
                {
                    Log.InfoFormat("non-error message received:{0}", message?.ContentCase);
                    currentMessage.Clear();
                }
            }
        }

        /// <summary>
        /// Parse test txt file to get list of files under test
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
                    this.listBox1.Items.Add(linedata[0]);
                    messageList.Add(directory + "\\" + linedata[0]);
                }
            }
            this.listBox1.Items.Add("END");
            this.listBox1.SelectedIndex = 0;
            parser.Close();
            expectedErrors = BuildListOfExpectedErrors();
        }

        #endregion Public Methods

        #region Private Methods
        /// <summary>
        /// Clear the list of files to be tested
        /// </summary>
        private void ClearFileList()
        {
            this.listBox1.Items.Clear();
            messageList.Clear();
        }

        /// <summary>
        /// Clear message queue ready for new data
        /// </summary>
        private void ClearMessageQueue()
        {
            while (this.messageQueue.Count > 0)
            {
                string oldString;
                this.messageQueue.TryTake(out oldString);
            }
        }

        /// <summary>
        /// Write the contents to the message stream
        /// </summary>
        /// <param name="filename"></param>
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

                Log.InfoFormat("Sending: {0}", filename);
                messenger.ReadAndSendFile(filename);
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
        /// Append a message to the Output textbox
        /// </summary>
        /// <param name="value"></param>
        private void AppendTextBox(string value)
        {
            //allow background thread to write to the output window
            if (InvokeRequired)
            {
                this.Invoke(new Action<string>(AppendTextBox), new object[] { value });
                return;
            }
            outputWindow.Text += value + "\r\n";
        }

        /// <summary>
        /// Add a message to the beginning of the output textbox, used to summarise test
        /// </summary>
        /// <param name="value"></param>
        private void PrependTextBox(string value)
        {
            if (InvokeRequired)
            {
                this.Invoke(new Action<string>(PrependTextBox), new object[] { value });
                return;
            }
            outputWindow.Text = value + "\r\n" + outputWindow.Text;
        }

        /// <summary>
        /// Cycle through the list sending files
        /// </summary>
        private void ProcessList()
        {
            bool testSuccess = true;

            if (messageList.Count == 0)
            {
                PrependTextBox("No messages in test data\r\n");
                return;
            }

            ClearMessageQueue();

            for (int i = 0; i < messageList.Count; i++)
            {
                if (messageList[i] != "")
                {
                    string filename = messageList[i];
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
        /// handle response from file sent, checking validity and flagging if no response received
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        private bool HandleResponse(string filename)
        {
            string expectedErrorMessage = null;
            string queuedResponse = null;

            try
            {
                //error messages may contain carriage returns, these are removed for comparison
                expectedErrorMessage = GetExpectedErrorMessage(Path.GetFileName(filename)).Replace("\n", " ");

                Log.InfoFormat("Awaiting Response for {0}", filename);

                this.messageQueue.TryTake(out queuedResponse, 1000);

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
                    AppendTextBox("Do you need to include an expected response in: " + SapientASMsimulator.Properties.Settings.Default.ExpectedResponseFileName);
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
        /// Returns the expected error message from the expected responses file, or throws and error if not found
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private string GetExpectedErrorMessage(string key)
        {
            string expectedMessage;
            if (expectedErrors.ContainsKey(key))
            {
                expectedMessage = expectedErrors[key];
            }
            else
            {
                throw new Exception("Expected error message could not be found");
            }
            return expectedMessage;
        }

        /// <summary>
        /// Builds expected error list from file specified in settings
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, string> BuildListOfExpectedErrors()
        {
            // init building dictionary
            Dictionary<string, string> buildingDictionary = new Dictionary<string, string>();

            try
            {
                var filename = SapientASMsimulator.Properties.Settings.Default.ExpectedResponseFileName;
                if (!File.Exists(filename) && !string.IsNullOrWhiteSpace(directory))
                {
                    if (!directory.EndsWith("/"))
                    {
                        directory += "/";
                    }
                    filename = directory + filename;
                }

                if (File.Exists(filename))
                {
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
                else
                {
                    Log.Warn($"Expected Response file not found {filename}");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error Reading file");
                Console.WriteLine(e);
            }

            return buildingDictionary;
        }
        #endregion Private Methods

        #region Button Actions
        /// <summary>
        /// Validate the responses when the loaded files are sent
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SendList_Click(object sender, EventArgs e)
        {
            outputWindow.Clear();
            new Thread(ProcessList).Start();
        }

        /// <summary>
        /// Load a list of files for sending
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LoadList_Click(object sender, EventArgs e)
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
        /// Send all files pertaining to the ASM from the folder specified in settings
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SendAll_Click(object sender, EventArgs e)
        {
            outputWindow.Clear();

            if (Directory.Exists(SapientASMsimulator.Properties.Settings.Default.TestMessageRoot))
            {
                string[] allfiles = Directory.GetFiles(SapientASMsimulator.Properties.Settings.Default.TestMessageRoot, "*.txt", SearchOption.AllDirectories);
                messageList = new List<string>();
                ClearFileList();
                foreach (string file in allfiles)
                {
                    // Files for both the ASM and HLDMM are stored in sibling directories, only want to send ASM ones
                    string parentFolder = Path.GetDirectoryName(file).Replace(Path.GetDirectoryName(Path.GetDirectoryName(file)), "").Trim('\\');
                    if (ASMFolders.Contains(parentFolder))
                    {
                        LoadScript(file);
                    }
                }

                new Thread(ProcessList).Start();
            }
            else
            {
                Log.Warn($"Directory not found {SapientASMsimulator.Properties.Settings.Default.TestMessageRoot}");
            }
        }


        /// <summary>
        /// Manually step through the file list and pass file name for sending
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NextButton_Click(object sender, EventArgs e)
        {
            if (this.listBox1.SelectedIndex < this.listBox1.Items.Count - 1)
            {
                string filename = this.listBox1.SelectedItem.ToString();
                string fileExtension = ".json";
                if (!filename.ToLower().Contains(fileExtension))
                {
                    filename += fileExtension;
                }

                ReadFile(filename);

                currentFilename = filename;

                new Thread(HandleSingleFileResponse).Start();

                this.listBox1.SelectedIndex++;
            }
        }

        /// <summary>
        /// Handle Response for single file click button
        /// </summary>
        private void HandleSingleFileResponse()
        {
            HandleResponse(currentFilename);
        }

        #endregion Button Actions
    }
}
