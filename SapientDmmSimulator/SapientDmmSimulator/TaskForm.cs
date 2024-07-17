// Crown-owned copyright, 2021-2024
namespace SapientDmmSimulator
{
    using FluentValidation.Results;
    using Google.Protobuf.WellKnownTypes;
    using log4net;
    using Sapient.Data;
    using SAPIENT.ReadSampleMessage;
    using SapientDmmSimulator.Common;
    using SapientServices;
    using SapientServices.Communication;
    using SapientServices.Data.Validation;
    using System;
    using System.IO;
    using System.Threading;
    using System.Windows.Forms;
    using ValidationResult = FluentValidation.Results.ValidationResult;

    public partial class TaskForm : Form
    {
        private IConnection messenger;

        private SapientLogger sapient_logger;

        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private ICommsConnection connection;

        private DetectionGenerator detectionGenerator;

        private AlertGenerator alertGenerator;

        private TaskGenerator taskGenerator;

        private Thread background_detections;

        private Thread background_tasks;

        public TaskForm()
        {
            PTZForm.Randomize();

            InitializeComponent();
            alertID.Text = Ulid.NewUlid().ToString();

            open_file_dialog.InitialDirectory = Application.StartupPath;
            open_file_dialog.Filter = "Json Files (.json)|*.json|All Files (*.*)|*.*";
            open_file_dialog.FilterIndex = 1;
            open_file_dialog.FileName = "*.json";

            openLogFile.InitialDirectory = Application.StartupPath;
            openLogFile.Filter = "Log Files (.log)|*.log|All Files (*.*)|*.*";
            openLogFile.FilterIndex = 1;
            openLogFile.FileName = "*.log";

            TaskGenerator.SendToDmm = Properties.Settings.Default.sendToDMM;
            Sensor_input.Text = Properties.Settings.Default.FixedASMId;

            detectionGenerator = new DetectionGenerator();
            alertGenerator = new AlertGenerator();
            taskGenerator = new TaskGenerator();
            SetAssociatedFilename(filenameTextBox.Text);
        }

        private void TaskFormLoad(object sender, EventArgs e)
        {
            string logDirectory = Properties.Settings.Default.LogDirectory;

            // if log directory doesn't exist, make it
            if (!System.IO.Directory.Exists(logDirectory))
            {
                System.IO.Directory.CreateDirectory(logDirectory);
            }

            SetLogPath(logDirectory);

            if (Properties.Settings.Default.Log)
            {
                sapient_logger = SapientLogger.CreateLogger(Properties.Settings.Default.LogDirectory, Properties.Settings.Default.LogPrefix, Properties.Settings.Default.IncrementIntervalSeconds);
            }

            var client = new SapientClient(Properties.Settings.Default.DmmDataAgentAddress, Properties.Settings.Default.DmmDataAgentPort);

            connection = client;
            messenger = client;
            messenger.SetNoDelay(true);
            this.messenger.ValidationEnabled = Properties.Settings.Default.ValidationEnabled;
            this.messenger.MessageSent += this.MessengerMessageSent;
            this.messenger.MessageReceived += this.MessengerMessageReceived;
            this.messenger.MessageError += this.MessengerMessageError;
            this.messenger.SendErrorMessage += this.MessengerSendErrorMessage;

            // if only sending data, currently the SapientComms Librray won't reconnect
            // This isn't a problem as we need bi-directional comms
            const bool SEND_ONLY_CONNECTION = false;
            connection.Start(1024 * 1024, SEND_ONLY_CONNECTION);
            connection.SetDataReceivedCallback(DataCallback);
            Log.InfoFormat("Port: {0}", Properties.Settings.Default.DmmDataAgentPort);
            Text = Text + ": " + Properties.Settings.Default.DmmDataAgentPort;
        }

        private void MessengerMessageSent(object sender, SapientMessageEventArgs e)
        {
            string messageText = string.Format("Send {0}:", e?.Message?.ContentCase.ToString());
            this.UpdateOutputWindow(messageText);
            this.UpdateOutputWindow(e?.Message?.ToString());
            this.UpdateOutputWindow(string.Empty);
            Log.Info(messageText);
        }

        private void MessengerMessageReceived(object sender, SapientMessageEventArgs e)
        {
            string messageText = string.Format("Received {0}:", e?.Message?.ContentCase.ToString());
            this.UpdateOutputWindow(messageText);
            this.UpdateOutputWindow(e?.Message?.ToString());
            this.UpdateOutputWindow(string.Empty);
            Log.Info(messageText);
        }

        private void MessengerMessageError(object sender, SapientMessageEventArgs e)
        {
            this.DisplayError(e.Error, e.Message);
        }

        private void MessengerSendErrorMessage(object sender, SapientMessageEventArgs e)
        {
            if (e.Message != null)
            {
                e.Message.NodeId = Properties.Settings.Default.FixedDMMId;
                if (this.messenger.SendMessage(e.Message))
                {
                    Log.InfoFormat("Send Error: {0}", e.Message.DestinationId);
                }
                else
                {
                    Log.ErrorFormat("Send Error Failed: {0}", e.Message.DestinationId);
                }
            }
        }

        private void DisplayError(string errorMessage, SapientMessage message)
        {
            this.UpdateOutputWindow(message == null ? "Error:" : string.Format("Error {0}:", message?.ContentCase));
            this.UpdateOutputWindow(errorMessage);
            this.UpdateOutputWindow(string.Empty);
            Log.Error(string.Format("Error: {0}", errorMessage));
        }

        private void DataCallback(SapientMessage message, IConnection client)
        {
            if (message == null)
            {
                return;
            }


            switch (message.ContentCase)
            {
                case SapientMessage.ContentOneofCase.None:
                    Log.InfoFormat("Unknown Message Received: {0}", message?.ContentCase);
                    break;
                case SapientMessage.ContentOneofCase.Registration:
                    try
                    {
                        Log.Debug(string.Format("Registration Message Received: {0}", message.NodeId));
                        this.GenerateRegistrationAck(message);
                    }
                    catch (Exception ex)
                    {
                        Log.Error("Parse Registration Message Failed ", ex);
                    }
                    break;
                case SapientMessage.ContentOneofCase.RegistrationAck:
                    break;
                case SapientMessage.ContentOneofCase.StatusReport:
                    try
                    {
                        Log.Debug(string.Format("Status Report Message Received: {0}", message.NodeId));
                    }
                    catch (Exception ex)
                    {
                        Log.Error("Parse StatusReport Message Failed ", ex);
                    }
                    break;
                case SapientMessage.ContentOneofCase.DetectionReport:
                    try
                    {
                        Log.Debug(string.Format("Detection Report Message Received: {0}", message.NodeId));
                    }
                    catch (Exception ex)
                    {
                        Log.Error("Parse DetectionReport Message Failed ", ex);
                    }
                    break;
                case SapientMessage.ContentOneofCase.Task:
                    try
                    {
                        Log.Info("Task Message Received");
                        var task = message.Task;
                        this.GenerateTaskAck(message.NodeId, task.TaskId);
                        Log.InfoFormat("Task ID: {0}", task.TaskId);
                    }
                    catch (Exception ex)
                    {
                        Log.Error("Parse Task Message Failed ", ex);
                    }
                    break;
                case SapientMessage.ContentOneofCase.TaskAck:
                    try
                    {
                        var taskAck = message.TaskAck;
                        Log.InfoFormat("{0}:SensorTaskACK Task ID: {1}:{2}:{3}", 1, taskAck.TaskId, taskAck.TaskStatus, taskAck.Reason);
                    }
                    catch (Exception ex)
                    {
                        Log.ErrorFormat("Parse Task ACK Failed: {0} {1}", message?.ContentCase, ex);
                    }
                    break;
                case SapientMessage.ContentOneofCase.Alert:
                    try
                    {
                        Log.Info("Alert Message Received");
                        var alert = message.Alert;
                        Log.InfoFormat("Alert alertID: {0} from sensor: {1}", alert.AlertId, message.NodeId);
                    }
                    catch (Exception ex)
                    {
                        Log.Error("Parse Alert Message Failed ", ex);
                    }
                    break;
                case SapientMessage.ContentOneofCase.AlertAck:
                    try
                    {
                        Log.Info("AlertResponse Message Received");
                        var response = message.AlertAck;
                        Log.InfoFormat("AlertResponse alertID: {0}", response.AlertId);
                    }
                    catch (Exception ex)
                    {
                        Log.Error("Parse AlertResponse Message Failed ", ex);
                    }
                    break;
                case SapientMessage.ContentOneofCase.Error:
                    try
                    {
                        Log.Debug(string.Format("Error Message Received: {0}", message?.Error?.ErrorMessage.ToString()));
                    }
                    catch (Exception ex)
                    {
                        Log.Error("Parse Error Message Failed ", ex);
                    }
                    break;
                default:
                    Log.InfoFormat("Unknown Message Received:{0}", message?.ContentCase);
                    break;
            }

        }

        private void SendTaskClick(object sender, EventArgs e)
        {
            SendCommand(false, 1);
        }

        private void SendCommand(bool ptzCommand, int numSensors)
        {
            PTZForm ptzform = new PTZForm(ptzCommand);
            DialogResult result = ptzform.ShowDialog();
            if (result == DialogResult.OK)
            {
                string sensorId = Sensor_input.Text;
                this.SendTaskLoop(ptzform.command.Text, sensorId, numSensors);
            }
        }

        private void SendPTZTaskClick(object sender, EventArgs e)
        {
            SendCommand(true, 1);
        }

        public void UpdateOutputWindow(string message)
        {
            if (Output_box.InvokeRequired)
            {
                Output_box.Invoke(new UpdaterCallback(StringUpdater), message);
            }
            else
            {
                StringUpdater(message);
            }
        }

        public void StringUpdater(string text)
        {
            Output_box.AppendText(text + "\r\n");
        }

        public delegate void UpdaterCallback(string t);

        private void ReadFileClick(object sender, EventArgs e)
        {
            var result = open_file_dialog.ShowDialog(); // Show the dialog.
            if (result == DialogResult.OK) // Test result.
            {
                var json_file_location = open_file_dialog.FileName;
                ReadAll(json_file_location);
                open_file_dialog.InitialDirectory = Path.GetDirectoryName(open_file_dialog.FileName);
            }
        }

        private void ReadAll(string input_filename)
        {
            using (var sr = new StreamReader(input_filename))
            {
                var whole = sr.ReadToEnd();
                try
                {
                    SapientMessage message = SapientMessage.Parser.ParseJson(whole);
                    this.messenger.SendMessage(message);
                }
                catch (Exception ex)
                {
                    this.DisplayError(ex.Message, null);
                    Console.WriteLine(ex);
                }
            }
        }

        private void ClearClick(object sender, EventArgs e)
        {
            Output_box.Text = string.Empty;
        }

        private void SendDetectionClick(object sender, EventArgs e)
        {
            if (background_detections == null || background_detections.IsAlive == false)
            {
                detectionGenerator.LoopTime = 100;
                background_detections = new Thread(o => detectionGenerator.GenerateHLDetections(o, this));
                background_detections.Start(messenger);
            }
        }

        private void TaskFormFormClosed(object sender, FormClosedEventArgs e)
        {
            connection.Shutdown();
        }

        private void AlertResponseClick(object sender, EventArgs e)
        {
            Log.Info(string.Format("Create Alart Ack message: {0}", this.alertID.Text));
            ReadSampleMessage readSampleMessage = new ReadSampleMessage();
            string error = string.Empty;
            SapientMessage? message = readSampleMessage.ReadSampleMessageFromFile(SapientMessage.ContentOneofCase.AlertAck, string.Empty, out error);
            if (message != null)
            {
                message.DestinationId = Sensor_input.Text;
                message.NodeId = Properties.Settings.Default.FixedDMMId;
                message.Timestamp = Timestamp.FromDateTime(DateTime.Now.ToUniversalTime());
                if (message.AlertAck != null)
                {
                    message.AlertAck.AlertId = this.alertID.Text;
                }
            }
            else
            {
                this.DisplayError(error, message);
            }

            if (message != null)
            {
                if (messenger.SendMessage(message))
                {
                    if (sapient_logger != null && Properties.Settings.Default.Log) sapient_logger.Log(message.ToString());
                    Log.Info("Send Alert Ack Succeeded");
                }
                else
                {
                    Log.Error("Send Alert Ack Failed");
                }
            }
        }

        public void GenerateRegistrationAck(SapientMessage registrationMessage)
        {
            if (registrationMessage != null)
            {
                ReadSampleMessage readSampleMessage = new ReadSampleMessage();
                string error = string.Empty;

                SapientMessage? message = readSampleMessage.ReadSampleMessageFromFile(SapientMessage.ContentOneofCase.RegistrationAck, string.Empty, out error);
                if (message != null)
                {
                    message.NodeId = Properties.Settings.Default.FixedDMMId;
                    message.DestinationId = registrationMessage.NodeId;
                    message.Timestamp = Timestamp.FromDateTime(DateTime.Now.ToUniversalTime());
                    if (this.messenger.SendMessage(message))
                    {
                        Log.InfoFormat("Send Registration Ack: {0}", message.DestinationId);
                    }
                    else
                    {
                        Log.ErrorFormat("Send Registration Ack Failed: {0}", message.DestinationId);
                    }
                }
                else
                {
                    this.DisplayError(error, message);
                }
            }
        }

        public void GenerateTaskAck(string sensorId, string taskId)
        {
            var message = new SapientMessage
            {
                Timestamp = Timestamp.FromDateTime(DateTime.UtcNow),
                NodeId = Properties.Settings.Default.FixedDMMId,
                DestinationId = sensorId,
                TaskAck = new TaskAck
                {
                    TaskId = taskId,
                    TaskStatus = TaskAck.Types.TaskStatus.Accepted,
                },
            };

            File.WriteAllLines(@".\SensorTaskACK1.json", new[] { message.ToString() });

            Log.Info(messenger.SendMessage(message)
                                  ? "Send Task Ack Succeeded"
                                  : "Send Task Ack  Failed");
        }

        private void HLHeartbeatButton_Click(object sender, EventArgs e)
        {
            HeartbeatGenerator heartbeatGenerator = new HeartbeatGenerator();
            heartbeatGenerator.GenerateHLStatus(messenger);
        }

        private void HLAlertButton_Click(object sender, EventArgs e)
        {
            alertGenerator.GenerateAlert(messenger);
        }

        private void fileScriptButton_Click(object sender, EventArgs e)
        {
            ScriptReader.ScriptForm scriptForm = new ScriptReader.ScriptForm(messenger);
            connection.SetDataReceivedCallback(scriptForm.DataCallback);
            scriptForm.ShowDialog();
        }

        private void SendTaskLoop(string commandString, string sensorId, int numSensors)
        {
            if (background_tasks == null || background_tasks.IsAlive == false)
            {
                taskGenerator.LoopTime = 100;
                taskGenerator.CommandString = commandString;
                taskGenerator.BaseSensorID = sensorId;
                taskGenerator.NumSensors = numSensors;
                background_tasks = new Thread(o => taskGenerator.GenerateCommand(o));
                background_tasks.Start(messenger);
            }
        }

        private void loopTasksCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (loopTasksCheckBox.Checked == false)
            {
                try
                {
                    background_tasks.Abort();
                }
                catch { }
                taskGenerator.LoopMessages = false;
            }
            else
            {
                taskGenerator.LoopMessages = true;
            }
        }

        private static void SetLogPath(string logDirectory)
        {
            log4net.Config.XmlConfigurator.Configure();
            log4net.Repository.Hierarchy.Hierarchy h =
            (log4net.Repository.Hierarchy.Hierarchy)LogManager.GetRepository();
            foreach (log4net.Appender.IAppender a in h.Root.Appenders)
            {
                if (a is log4net.Appender.FileAppender)
                {
                    log4net.Appender.FileAppender fa = (log4net.Appender.FileAppender)a;

                    // Programmatically set this to the desired location here
                    string logFileLocation = System.IO.Path.Combine(logDirectory, "dmmlog.txt");

                    fa.File = logFileLocation;
                    fa.ActivateOptions();
                    break;
                }
            }
        }

        private void loopDetectionCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (loopDetectionCheckBox.Checked == false)
            {
                try
                {
                    background_tasks.Abort();
                }
                catch { }
                detectionGenerator.LoopMessages = false;
            }
            else
            {
                detectionGenerator.LoopMessages = true;
            }
        }

        private void filenameTextBox_TextChanged(object sender, EventArgs e)
        {
            SetAssociatedFilename(filenameTextBox.Text);
        }

        private void SetAssociatedFilename(string filename)
        {
            if (detectionGenerator != null)
            {
                detectionGenerator.ImageURL = filename;
            }

            AlertGenerator.ImageURL = filename;
        }

        private void sendToDMMcheckBox_CheckedChanged(object sender, EventArgs e)
        {
            bool sendToDmm = sendToDMMcheckBox.Checked;
            TaskGenerator.SendToDmm = sendToDmm; 
        }
    }
}