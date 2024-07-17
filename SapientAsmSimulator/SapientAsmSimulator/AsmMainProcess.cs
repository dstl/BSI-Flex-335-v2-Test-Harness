// File:              $Workfile: AsmMainProcess.cs$
// <copyright file="AsmMainProcess.cs" >
// Crown-owned copyright, 2021-2024
// See Release/Supply Conditions
// </copyright>

namespace SapientASMsimulator
{
    using log4net;
    using Sapient.Data;
    using SapientASMsimulator.Common;
    using SapientServices;
    using SapientServices.Communication;
    using System;
    using System.IO;
    using System.Threading;

    /// <summary>
    /// Main Processing class for ASM simulator
    /// </summary>
    public class ASMMainProcess : ISender
    {
        #region Private Data Members

        /// <summary>
        /// Log4net logger
        /// </summary>
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// reference to UI interface
        /// </summary>
        private IGUIInterface guiInterface;

        /// <summary>
        /// Socket connection object
        /// </summary>
        private ICommsConnection connection;

        /// <summary>
        /// socket messaging object
        /// </summary>
        private IConnection messenger;

        /// <summary>
        /// thread for looped heartbeat messages
        /// </summary>
        private Thread heartbeatLoopThread;

        /// <summary>
        /// thread for looped detection messages
        /// </summary>
        private Thread detectionLoopThread;

        /// <summary>
        /// thread for looped alert messages
        /// </summary>
        private Thread alertLoopThread;

        /// <summary>
        /// Detection message generator
        /// </summary>
        private DetectionGenerator detectionGenerator;

        /// <summary>
        /// Heartbeat message generator
        /// </summary>
        private HeartbeatGenerator heartbeatGenerator;

        /// <summary>
        /// Registration message generator
        /// </summary>
        private RegistrationGenerator registrationGenerator;

        /// <summary>
        /// TaskAck message generator
        /// </summary>
        private TaskAckGenerator taskAckGenerator;

        /// <summary>
        /// Alert message generator
        /// </summary>
        private AlertGenerator alertGenerator;

        /// <summary>
        /// message logger
        /// </summary>
        private SapientLogger sapientLogger;

        /// <summary>
        /// Task message
        /// </summary>
        private TaskMessage taskMessage;

        /// <summary>
        /// interval time to use between sending heartbeat messages if looped
        /// </summary>
        private int heartbeatLoopTime;

        /// <summary>
        /// interval time to use between sending detection messages if looped
        /// </summary>
        private int detectionLoopTime;

        #endregion

        /// <summary>
        /// Initializes a new instance of the ASMMainProcess class
        /// </summary>
        /// <param name="guiInterface">link to user interface</param>
        public ASMMainProcess(IGUIInterface guiInterface)
        {
            //ASMMainProcess.AsmId = Properties.Settings.Default.FixedASMId;
            this.guiInterface = guiInterface;

            string logDirectory = Properties.Settings.Default.LogDirectory;
            logDirectory = string.Format("{0}-ASM{1:D3}", logDirectory, ASMMainProcess.AsmId);

            // if log directory doesn't exist, make it
            if (!System.IO.Directory.Exists(logDirectory))
            {
                System.IO.Directory.CreateDirectory(logDirectory);
            }

            SetLogPath(logDirectory);

            if (Properties.Settings.Default.Log)
            {
                this.sapientLogger = SapientLogger.CreateLogger(Properties.Settings.Default.LogDirectory, Properties.Settings.Default.LogPrefix, Properties.Settings.Default.IncrementIntervalSeconds);
            }
        }

        /// <summary>
        /// Gets or sets the fixed sensor identifier to use in all messages
        /// </summary>
        public static string AsmId { get; set; }

        public static string PortId { get; set; }

        /// <summary>
        /// Gets or sets the interval time to use between sending heartbeat messages if looped
        /// </summary>
        public int HeartbeatLoopTime
        {
            get
            {
                return this.heartbeatLoopTime;
            }

            set
            {
                if (value > 0)
                {
                    this.heartbeatLoopTime = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the interval time to use between sending detection messages if looped
        /// </summary>
        public int DetectionLoopTime
        {
            get
            {
                return this.detectionLoopTime;
            }

            set
            {
                if (value > 0)
                {
                    this.detectionLoopTime = value;
                }
            }
        }

        /// <summary>
        /// Initialize main processing
        /// </summary>
        public void Initialise()
        {
            var serverName = Properties.Settings.Default.SapientDataAgentAddress;
            int serverPort = Properties.Settings.Default.SapientDataAgentPort;

            if (!string.IsNullOrWhiteSpace(ASMMainProcess.PortId) && int.Parse(ASMMainProcess.PortId) > 0)
            {
                serverPort = int.Parse(ASMMainProcess.PortId);
            }

            var client = new SapientClient(serverName, serverPort);

            this.connection = client;
            this.messenger = client;
            this.messenger.SetNoDelay(true);
            this.messenger.ValidationEnabled = Properties.Settings.Default.ValidationEnabled;
            this.messenger.MessageSent += this.MessengerMessageSent;
            this.messenger.MessageReceived += this.MessengerMessageReceived;
            this.messenger.MessageError += this.MessengerMessageError;
            this.messenger.SendErrorMessage += this.MessengerSendErrorMessage;

            // if only sending data, currently the SapientComms Librray won't reconnect
            // This isn't a problem as we need bi-directional comms
            const bool SEND_ONLY_CONNECTION = false;
            this.connection.Start(1024 * 1024, SEND_ONLY_CONNECTION);
            this.connection.SetDataReceivedCallback(this.DataCallback);
            this.taskMessage = new TaskMessage();
            this.registrationGenerator = new RegistrationGenerator();
            TaskMessage.RegistrationGenerator = this.registrationGenerator;
            this.detectionGenerator = new DetectionGenerator();
            this.heartbeatGenerator = new HeartbeatGenerator();
            this.heartbeatGenerator.TaskId = Ulid.NewUlid().ToString();
            TaskMessage.HeartbeatGenerator = this.heartbeatGenerator;
            this.alertGenerator = new AlertGenerator();
            this.taskAckGenerator = new TaskAckGenerator();
            TaskMessage.TaskAckGenerator = this.taskAckGenerator;
            this.detectionGenerator.ChangeObjectID = Properties.Settings.Default.IndividualDetectionIDs;
            this.detectionGenerator.ImageURL = Properties.Settings.Default.DetectionImageURL;
            this.detectionGenerator.SubClassTypeString = Properties.Settings.Default.DetectionSubClassType;
            this.detectionGenerator.ClassTypeString = Properties.Settings.Default.DetectionClassType;
            this.detectionGenerator.TaskId = Ulid.NewUlid().ToString();

            this.detectionLoopTime = 100;
            this.heartbeatLoopTime = 5;
            MessageSender.FragmentData = Properties.Settings.Default.PacketFragmentDelay > 0;
            MessageSender.PacketDelay = Properties.Settings.Default.PacketFragmentDelay;
        }

        private void MessengerMessageSent(object sender, SapientMessageEventArgs e)
        {
            string messageText = string.Format("Send {0}:", e?.Message?.ContentCase);
            this.guiInterface.UpdateOutputText(messageText); 
            this.guiInterface.UpdateOutputText(e?.Message?.ToString());
            this.guiInterface.UpdateOutputText(string.Empty);
            Log.Info(messageText);
        }

        private void MessengerMessageReceived(object sender, SapientMessageEventArgs e)
        {
            string messageText = string.Format("Received {0}:", e?.Message?.ContentCase);
            this.guiInterface.UpdateOutputText(messageText);
            this.guiInterface.UpdateOutputText(e?.Message?.ToString());
            this.guiInterface.UpdateOutputText(string.Empty);
            Log.Info(messageText);
        }

        private void MessengerMessageError(object sender, SapientMessageEventArgs e)
        {
            this.DisplayError(e.Error, e.Message);
        }

        private void DisplayError(string errorMessage, SapientMessage message)
        {
            this.guiInterface.UpdateOutputText(message == null ? "Error:" : string.Format("Error in {0}:", message?.ContentCase));
            this.guiInterface.UpdateOutputText(errorMessage);
            this.guiInterface.UpdateOutputText(string.Empty);
            Log.Error(string.Format("Error: {0}", errorMessage));
        }

        private void MessengerSendErrorMessage(object sender, SapientMessageEventArgs e)
        {
            if (e.Message != null)
            {
                e.Message.NodeId = ASMMainProcess.AsmId;
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

        /// <summary>
        /// Shut down main processing
        /// </summary>
        public void Shutdown()
        {
            this.connection.Shutdown();
        }

        /// <summary>
        /// Return number of detection messages sent
        /// </summary>
        /// <returns>detection count</returns>
        public long DetectionCount()
        {
            long retval = 0;
            if (this.detectionGenerator != null)
            {
                retval = this.detectionGenerator.MessageCount;
            }

            return retval;
        }

        /// <summary>
        /// Return number of heartbeat messages sent
        /// </summary>
        /// <returns>heartbeat count</returns>
        public long HeartBeatCount()
        {
            long retval = 0;
            if (this.heartbeatGenerator != null)
            {
                retval = this.heartbeatGenerator.MessageCount;
            }

            return retval;
        }

        /// <summary>
        /// Return number of alert messages sent
        /// </summary>
        /// <returns>alert count</returns>
        public long AlertCount()
        {
            long retval = 0;
            if (this.alertGenerator != null)
            {
                retval = this.alertGenerator.MessageCount;
            }

            return retval;
        }

        /// <summary>
        /// Send single registration message
        /// </summary>
        public void SendRegistration()
        {
            this.registrationGenerator.GenerateRegistration(this.messenger, this.guiInterface, this.sapientLogger);
        }

        /// <summary>
        /// Send detection message once or in a loop
        /// </summary>
        public void SendDetectionLoop()
        {
            if (this.detectionLoopThread == null || this.detectionLoopThread.IsAlive == false)
            {
                this.detectionGenerator.LoopTime = this.DetectionLoopTime;
                this.detectionLoopThread = new Thread(o => this.detectionGenerator.GenerateDetections(o, this.sapientLogger));
                this.detectionLoopThread.Start(this.messenger);
            }
        }

        /// <summary>
        /// Send heartbeat message once or in a loop
        /// </summary>
        public void SendHeartbeatLoop()
        {
            if (this.heartbeatLoopThread == null || this.heartbeatLoopThread.IsAlive == false)
            {
                this.heartbeatGenerator.LoopTime = this.HeartbeatLoopTime * 1000;
                this.heartbeatLoopThread = new Thread(o => this.heartbeatGenerator.GenerateStatus(o, this.sapientLogger));
                this.heartbeatLoopThread.Start(this.messenger);
            }
        }

        /// <summary>
        /// Send alert message once or in a loop
        /// </summary>
        public void SendAlertLoop()
        {
            if (this.alertLoopThread == null || this.alertLoopThread.IsAlive == false)
            {
                this.alertGenerator.LoopTime = this.DetectionLoopTime;
                this.alertLoopThread = new Thread(o => this.alertGenerator.GenerateAlert(o, this.sapientLogger));
                this.alertLoopThread.Start(this.messenger);
            }
        }

        /// <summary>
        /// Toggle whether to loop sending detection messages
        /// </summary>
        /// <param name="loop">true if looping</param>
        public void SetDetectionLoopState(bool loop)
        {
            this.detectionGenerator.LoopMessages = loop;
            if (!loop)
            {
                try
                {
                    this.detectionLoopThread.Abort();
                }
                catch
                {
                }
            }
        }

        /// <summary>
        /// Toggle whether to loop sending heartbeat messages
        /// </summary>
        /// <param name="loop">true if looping</param>
        public void SetHeartbeatLoopState(bool loop)
        {
            this.heartbeatGenerator.LoopMessages = loop;
            if (!loop)
            {
                try
                {
                    this.heartbeatLoopThread.Abort();
                }
                catch
                {
                }
            }
        }

        /// <summary>
        /// Toggle whether to loop sending alert messages
        /// </summary>
        /// <param name="loop">true if looping</param>
        public void SetAlertLoopState(bool loop)
        {
            this.alertGenerator.LoopMessages = loop;
            if (!loop)
            {
                try
                {
                    this.alertLoopThread.Abort();
                }
                catch
                {
                }
            }
        }

        /// <summary>
        /// Method to read message from file.
        /// Checking could be done here to make sure message is correctly formatted
        /// However for testing this is not being done
        /// </summary>
        /// <param name="input_filename">Path to file to be loaded.</param>
        public void ReadAndSendFile(string input_filename)
        {
            string message = File.ReadAllText(input_filename);
            try
            {
                var parsedMessage = SapientMessage.Parser.ParseJson(message);
                this.guiInterface.UpdateOutputText("Read File");
                this.guiInterface.UpdateOutputText(Path.GetFileName(input_filename));
                this.guiInterface.UpdateOutputText(string.Empty);
                this.messenger.SendMessage(parsedMessage);
            }
            catch (Exception ex)
            {
                this.guiInterface.UpdateOutputText("Error Reading File ");
                this.guiInterface.UpdateOutputText(Path.GetFileName(input_filename));
                this.guiInterface.UpdateOutputText(ex.Message);
                this.guiInterface.UpdateOutputText(string.Empty);
            }
        }

        public void AddDataReceivedCallback(SapientCommsCommon.DataReceivedCallback callback)
        {
            this.connection.SetDataReceivedCallback(callback);
        }

        /// <summary>
        /// Set associated filename for detections and alerts
        /// </summary>
        /// <param name="filename">filename string</param>
        public void SetAssociatedFilename(string filename)
        {
            if (this.detectionGenerator != null)
            {
                this.detectionGenerator.ImageURL = filename;
            }
            AlertGenerator.ImageURL = filename;
        }

        /// <summary>
        /// callback method for communication 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="client">client connection</param>
        private void DataCallback(SapientMessage message, IConnection client)
        {
            this.ProcessIncomingProtobufMessage(message);
        }

        /// <summary>
        /// function to process the message coming in
        /// </summary>
        /// <param name="message">the string from the client</param>
        private void ProcessIncomingProtobufMessage(SapientMessage message)
        {
            if (message != null)
            {
                switch (message.ContentCase)
                {
                    case SapientMessage.ContentOneofCase.None:
                        Log.Error("Unknow message type.");
                        break;
                    case SapientMessage.ContentOneofCase.Registration:
                        Log.Error("Can not process registration message.");
                        break;
                    case SapientMessage.ContentOneofCase.RegistrationAck:
                        try
                        {
                            Log.Info("SensorRegistrationACK Received");
                            this.guiInterface.UpdateASMText(ASMMainProcess.AsmId);

                            BaseGenerators.ASMId = ASMMainProcess.AsmId;
                            TaskMessage.AsmId = ASMMainProcess.AsmId;
                            RegistrationGenerator.AsmId = ASMMainProcess.AsmId;
                            TaskAckGenerator.AsmId = ASMMainProcess.AsmId;

                            Log.Info("ASM ID: " + ASMMainProcess.AsmId + " Latency(ms): " + (DateTime.UtcNow - RegistrationGenerator.SendRegistrationTime).TotalMilliseconds);
                        }
                        catch (Exception ex)
                        {
                            Log.Error("Parse Registration ACK Failed " + ex);
                        }
                        break;
                    case SapientMessage.ContentOneofCase.StatusReport:
                        Log.Error("Can not process status report message.");
                        break;
                    case SapientMessage.ContentOneofCase.DetectionReport:
                        Log.Error("Can not process detection report message.");
                        break;
                    case SapientMessage.ContentOneofCase.Task:
                        try
                        {
                            Log.Info("Task Message Received");
                            this.detectionGenerator.TaskId = message.Task.TaskId;
                            this.heartbeatGenerator.TaskId = message.Task.TaskId;
                            taskMessage.ProcessSensorTaskMessage(message, this.messenger, this.guiInterface, this.sapientLogger);
                        }
                        catch (Exception ex)
                        {
                            Log.Error("Parse Task Message Failed " + ex);
                        }
                        break;
                    case SapientMessage.ContentOneofCase.TaskAck:
                        Log.Error("Can not process task ack message.");
                        break;
                    case SapientMessage.ContentOneofCase.Alert:
                        Log.Error("Can not process alert message.");
                        break;
                    case SapientMessage.ContentOneofCase.AlertAck:
                        try
                        {
                            Log.Info("AlertResponse Message Received");
                            var response = message.AlertAck;
                            Log.Info("AlertResponse alertID:" + response.AlertId);
                        }
                        catch (Exception ex)
                        {
                            Log.Error("Parse Task Message Failed " + ex);
                        }
                        break;
                    case SapientMessage.ContentOneofCase.Error:
                        try
                        {
                            var e = message.Error;
                            Log.Error("Error Received\n" + e.ErrorMessage);
                        }
                        catch (Exception ex)
                        {
                            Log.Error("Parse Error Message Failed " + ex);
                        }
                        break;
                    default:
                        Log.Error("Unknow message type.");
                        break;
                }
            }
        }

        /// <summary>
        /// set log path for log4net
        /// </summary>
        /// <param name="logDirectory">folder to log to</param>
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
                    string logFileLocation = System.IO.Path.Combine(logDirectory, "asmlog.txt");

                    fa.File = logFileLocation;
                    fa.ActivateOptions();
                    break;
                }
            }
        }
    }
}
