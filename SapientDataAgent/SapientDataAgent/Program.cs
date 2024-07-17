// Project:           $Project: 00010855-Sapient$
// File:              $Workfile: Program.cs$
// Crown-owned copyright, 2021-2024
//  See Release/Supply Conditions
// $NoKeywords$

namespace SapientMiddleware
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Threading;
    using System.Timers;
    using System.Windows.Forms;
    using log4net;
    using Microsoft.VisualBasic.Logging;
    using Sapient.Data;
    using SapientDatabase;
    using SapientMiddleware.Common;
    using SapientServices;
    using SapientServices.Communication;

    /// <summary>
    /// Main Program Class.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// List of message parsers for client side connections.
        /// </summary>
        public static readonly Dictionary<uint, SapientMessageParser> ClientMessageParsers = new Dictionary<uint, SapientMessageParser>();

        /// <summary>
        /// List of message parsers for GUI connections.
        /// </summary>
        private static readonly Dictionary<uint, SapientMessageParser> GuiClientMessageParsers = new Dictionary<uint, SapientMessageParser>();

        /// <summary>
        /// Log4net logger.
        /// </summary>
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Data File Logger.
        /// </summary>
        private static SapientLogger sapientLogger;

        /// <summary>
        /// message parser for tasking connection.
        /// </summary>
        private static SapientMessageParser taskingMessageParser;

        /// <summary>
        /// Database communications.
        /// </summary>
        private static Database database;

        /// <summary>
        /// Secondary Database communications.
        /// </summary>
        private static Database additionalDatabase;

        /// <summary>
        /// DMM Tasking Database communications.
        /// </summary>
        private static Database taskDatabase;

        /// <summary>
        /// GUI Database communications.
        /// </summary>
        private static Database guiDatabase;

        /// <summary>
        /// Number of clients connected.
        /// </summary>
        private static int numClients;

        /// <summary>
        /// Holds a collection of all previously collected ASMs throughout the lifetime of
        /// this data agent in with ASM id as the key and last message received time as value.
        /// </summary>
        public static ConcurrentDictionary<string, DateTime> previouslyConnectedASMs = new ConcurrentDictionary<string, DateTime>();

        /// <summary>
        /// time of last client message received.
        /// </summary>
        private static DateTime? timeOfLastMessage;

        /// <summary>
        /// time of last task message received.
        /// </summary>
        private static DateTime? timeOfLastTaskMessage;

        /// <summary>
        /// heartbeat timer object.
        /// </summary>
        private static System.Timers.Timer heartbeatTimer;

        /// <summary>
        /// Tasking communications main interface.
        /// </summary>
        private static ICommsConnection taskingComms;

        /// <summary>
        /// GUI client communications.
        /// </summary>
        private static SapientServer guiComms;

        /// <summary>
        /// GUI socket port.
        /// </summary>
        private static int guiPort;

        /// <summary>
        /// number of GUI clients connected.
        /// </summary>
        private static int numGuiClients;

        /// <summary>
        /// Whether to discard messages that do not have a null terminator.
        /// </summary>
        private static bool discardUnterminatedMessages;

        /// <summary>
        /// Fixed ASM port id specified in the config file / command line.
        /// </summary>
        private static int fixedAsmPortId;

        /// <summary>
        /// Flag as whether this is DMM Data Agent.
        /// </summary>
        private static bool isDmm;

        /// <summary>
        /// Gets Client Port number.
        /// </summary>
        public static int ClientPort { get; private set; }

        /// <summary>
        /// Gets Client communications object.
        /// </summary>
        public static SapientServer ClientComms { get; private set; }

        /// <summary>
        /// Gets Tasking communications send interface.
        /// </summary>
        public static IConnection TaskingCommsConnection { get; private set; }

        /// <summary>
        /// Gets or sets GUI form.
        /// </summary>
        private static ServerForm DataAgentForm { get; set; }

        /// <summary>
        /// Gets or sets the task permissions.
        /// </summary>
        private static TaskPermissions TaskPermissions { get; set; }

        /// <summary>
        /// Gets Message monitoring class count, latency etc.
        /// </summary>
        public static SapientMessageMonitor MessageMonitor { get; private set; }

        /// <summary>
        /// Gets Connection monitoring class: connected, number of clients etc.
        /// </summary>
        public static IConnectionMonitor ConnectionMonitor { get; private set; }

        /// <summary>
        /// Gets logging database object.
        /// </summary>
        public static MiddlewareLogDatabase LogDatabase { get; private set; }

        /// <summary>
        /// Gets or sets the last received sensor registration message.
        /// </summary>
        public static SapientMessage Registration { get; set; }

        /// <summary>
        /// Method for setting up the server (both ports) and building the database.
        /// </summary>
        /// <param name="args">Command Line Arguments.</param>
        [STAThread]
        public static void Main(string[] args)
        {
            bool showWindow = Properties.Settings.Default.ShowMainWindow;

            Initialise(showWindow, args);

            heartbeatTimer = new System.Timers.Timer(5000);
            heartbeatTimer.Elapsed += TickHeartbeat;
            heartbeatTimer.Start();

            if (showWindow)
            {
                Application.Run(DataAgentForm);
                taskingComms.Shutdown();
                ClientComms.Shutdown();

                if ((guiPort > 0) && (guiComms != null))
                {
                    guiComms.Shutdown();
                }
            }
            else
            {
                StartComms();
            }
        }

        /// <summary>
        /// Set window title text.
        /// </summary>
        /// <param name="text">title text string.</param>
        public static void SetWindowText(string text)
        {
            ConnectionMonitor.SetWindowText(text);
            DataAgentForm.UIThread(() => DataAgentForm.Text = text);
        }

        /// <summary>
        /// method to start communications listeners once form is loaded.
        /// </summary>
        public static void StartComms()
        {
            ClientComms.Start(1024 * 1024, false);
            taskingComms.Start(1024 * 1024, false);

            if ((guiPort > 0) && (guiComms != null))
            {
                guiComms.Start(1024 * 1024, false);
                guiComms.SetNoDelay(Properties.Settings.Default.NoDelay);
                guiComms.SetSendNullTermination(Properties.Settings.Default.sendNullTermination);
            }
        }

        /// <summary>
        /// Initialises the specified show window.
        /// </summary>
        /// <param name="showWindow">if set to <c>true</c> [show window].</param>
        /// <param name="args">The arguments.</param>
        private static void Initialise(bool showWindow, string[] args)
        {
            isDmm = Properties.Settings.Default.DMM;
            int windowX;
            int windowY;
            string databaseServer = Properties.Settings.Default.DatabaseServer;
            string databasePort = Properties.Settings.Default.DatabasePort;
            string databaseName = Properties.Settings.Default.DatabaseName;
            string databaseUser = Properties.Settings.Default.DatabaseUser;
            string databasePassword = Properties.Settings.Default.DatabasePassword;
            bool usingDatabase = (databaseServer != string.Empty) && (databasePort != string.Empty) && (databaseName != string.Empty);
            int taskingPort;
            guiPort = Properties.Settings.Default.GuiPort;
            discardUnterminatedMessages = Properties.Settings.Default.DiscardUnterminatedMessages;

            TaskPermissions = new TaskPermissions();

            HandleCommandLineArguments(args, out taskingPort);

            string logDirectory = CreateLogFolder();
            InitialiseLog4net(logDirectory);
            InitialiseDataLogging(logDirectory);

            CalculateWindowLocation(out windowX, out windowY, args);

            InitialiseStatusMonitoring(databaseServer, databasePort, databaseUser, databasePassword, usingDatabase, showWindow, windowX, windowY);

            InitialiseClientComms(ClientPort);

            if (isDmm && usingDatabase)
            {
                // create database if it doesn't exist.
                DatabaseCreator.CreateDatabase(databaseServer, databasePort, databaseUser, databasePassword, databaseName);
            }

            InitialiseTaskingComms(taskingPort);
            DatabaseCheck(databaseServer, databasePort, databaseUser, databasePassword, databaseName, usingDatabase);
            InitialiseGUIComms(databaseServer, databasePort, databaseUser, databasePassword, databaseName, usingDatabase);

            taskingComms.SetDataReceivedCallback(DataCallbackTask);
            taskingComms.SetConnectedCallback(TaskManagerConnected);

            InitialiseDatabase(databaseServer, databasePort, databaseUser, databasePassword, databaseName, usingDatabase);

            ////DatabaseTest(databaseServer, databasePort, databaseUser, databasePassword, databaseName, usingDatabase);
        }

        /// <summary>
        /// Handles the command line arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <param name="taskingPort">The tasking port.</param>
        private static void HandleCommandLineArguments(string[] args, out int taskingPort)
        {
            // For DMM and when no command line arguments, use client and tasking ports from config file
            if (isDmm)
            {
                ClientPort = Properties.Settings.Default.DACommunicationPort;
                taskingPort = Properties.Settings.Default.HDATaskingPort;
            }
            else
            {
                ClientPort = Properties.Settings.Default.SDAClientPort;
                taskingPort = Properties.Settings.Default.DACommunicationPort;
            }

            if (args.Length > 0)
            {
                // SDA connects to HDA via tasking connection using default client port number
                // SDA connects to ASMs with port number + fixed ASM ID
                if (!isDmm && args.Length > 0)
                {
                    // increment port based on fixed ASM ID.
                    ClientPort = int.Parse(args[0]);
                    Log.InfoFormat("Fixed ASM Port {0}", ClientPort);
                }

                // default to no GUI connection if command line parameters present.
                guiPort = 0;
            }

            // Enable gui port if G parameter is specified.
            if (args.Length > 1)
            {
                string arg2 = args[1];
                if (arg2 == "G")
                {
                    guiPort = Properties.Settings.Default.GuiPort;
                }
            }
        }

        /// <summary>
        /// Initialises the client comms.
        /// </summary>
        /// <param name="clientPort">The client port.</param>
        private static void InitialiseClientComms(int clientPort)
        {
            ClientComms = new SapientServer(Properties.Settings.Default.ClientAddress, clientPort, Properties.Settings.Default.ValidationEnabled);
            ClientComms.SetDataReceivedCallback(DataCallback);
            ClientComms.SetConnectedCallback(ClientConnected);
            ClientComms.SetNoDelay(Properties.Settings.Default.NoDelay);
            ClientComms.SetSendNullTermination(Properties.Settings.Default.sendNullTermination);
        }

        /// <summary>
        /// Initialises the tasking comms.
        /// </summary>
        /// <param name="taskingPort">The tasking port.</param>
        private static void InitialiseTaskingComms(int taskingPort)
        {
            if (isDmm)
            {
                taskingMessageParser = new SapientMessageParser(new DmmDataAgentTaskingProtocol(TaskPermissions), discardUnterminatedMessages);
                SapientServer taskingcomms = new SapientServer(Properties.Settings.Default.TaskingAddress, taskingPort, Properties.Settings.Default.ValidationEnabled);
                taskingcomms.SetNoDelay(Properties.Settings.Default.NoDelay);
                taskingcomms.SetSendNullTermination(Properties.Settings.Default.sendNullTermination);
                taskingComms = taskingcomms;
                MessageMonitor.SetStatusText(0, "DMM");
                string portText = string.Format("HDA:{0}", taskingPort);
                SetWindowText(portText);
                Log.InfoFormat(portText);
            }
            else
            {
                taskingMessageParser = new SapientMessageParser(new SapientDataAgentTaskingProtocol(), discardUnterminatedMessages);
                SapientClient taskingcomms = new SapientClient(Properties.Settings.Default.TaskingAddress, taskingPort);
                taskingcomms.ConnectionName = "HDA";
                taskingcomms.SetNoDelay(Properties.Settings.Default.NoDelay);
                taskingcomms.ValidationEnabled = Properties.Settings.Default.ValidationEnabled;
                taskingComms = taskingcomms;
                MessageMonitor.SetStatusText(0, "ASM");
                string portText = string.Format("SDA:{0}", ClientPort);
                SetWindowText(portText);
                Log.InfoFormat(portText);
            }
        }

        /// <summary>
        /// Initialises the GUI comms.
        /// </summary>
        /// <param name="databaseServer">The database server.</param>
        /// <param name="databasePort">The database port.</param>
        /// <param name="databaseUser">The database user.</param>
        /// <param name="databasePassword">The database password.</param>
        /// <param name="databaseName">Name of the database.</param>
        /// <param name="usingDatabase">if set to <c>true</c> [using database].</param>
        private static void InitialiseGUIComms(string databaseServer, string databasePort, string databaseUser, string databasePassword, string databaseName, bool usingDatabase)
        {
            if (guiPort > 0)
            {
                guiComms = new SapientServer(Properties.Settings.Default.GuiAddress, guiPort, Properties.Settings.Default.ValidationEnabled);
                guiComms.SetDataReceivedCallback(GuiDataCallback);
                guiComms.SetConnectedCallback(GuiClientConnected);
                guiComms.SetNoDelay(Properties.Settings.Default.NoDelay);
                guiComms.SetSendNullTermination(Properties.Settings.Default.sendNullTermination);

                if (usingDatabase)
                {
                    // open dedicated database connection for GUI messages.
                    guiDatabase = new Database(databaseServer, databasePort, databaseUser, databasePassword, databaseName);
                    if (string.IsNullOrEmpty(guiDatabase.DbConnectionError) == false)
                    {
                        Log.Error("Failed to connect to GUI database, running without database access: " + guiDatabase.DbConnectionError);
                    }
                }
            }
            else
            {
                ConnectionMonitor.DisableGUIConnectionReporting();
            }
        }

        /// <summary>
        /// Databases the check.
        /// </summary>
        /// <param name="databaseServer">The database server.</param>
        /// <param name="databasePort">The database port.</param>
        /// <param name="databaseUser">The database user.</param>
        /// <param name="databasePassword">The database password.</param>
        /// <param name="databaseName">Name of the database.</param>
        /// <param name="usingDatabase">if set to <c>true</c> [using database].</param>
        private static void DatabaseCheck(string databaseServer, string databasePort, string databaseUser, string databasePassword, string databaseName, bool usingDatabase)
        {
            if (usingDatabase)
            {
                int count = 0;
                bool databaseExists = DatabaseCreator.DatabaseExists(databaseServer, databasePort, databaseUser, databasePassword, databaseName);

                // wait for database to exist
                while (!databaseExists && (count < 12))
                {
                    count++;
                    Log.WarnFormat("Waiting for Creation of Database. Retry:{0}", count);
                    Thread.Sleep(5000);
                    databaseExists = DatabaseCreator.DatabaseExists(databaseServer, databasePort, databaseUser, databasePassword, databaseName);
                }

                if (!databaseExists)
                {
                    Log.ErrorFormat("Shutting Down: Configured to run with database but unable to connect to database server on Host:{0} Port{1}", databaseServer, databasePort);
                    Application.Exit();
                    return;
                }
            }
            else
            {
                Log.Info("Running without Database");
            }
        }

        /// <summary>
        /// Initialises the database.
        /// </summary>
        /// <param name="databaseServer">The database server.</param>
        /// <param name="databasePort">The database port.</param>
        /// <param name="databaseUser">The database user.</param>
        /// <param name="databasePassword">The database password.</param>
        /// <param name="databaseName">Name of the database.</param>
        /// <param name="usingDatabase">if set to <c>true</c> [using database].</param>
        private static void InitialiseDatabase(string databaseServer, string databasePort, string databaseUser, string databasePassword, string databaseName, bool usingDatabase)
        {
            if (usingDatabase)
            {
                database = new Database(databaseServer, databasePort, databaseUser, databasePassword, databaseName);
                if (string.IsNullOrEmpty(database.DbConnectionError) == false)
                {
                    Log.Error("Failed to connect to database, running without database access: " + database.DbConnectionError);
                }

                if (Properties.Settings.Default.AdditionalDatabaseName != string.Empty)
                {
                    additionalDatabase = new Database(databaseServer, databasePort, databaseUser, databasePassword, Properties.Settings.Default.AdditionalDatabaseName);
                    if (string.IsNullOrEmpty(additionalDatabase.DbConnectionError) == false)
                    {
                        Log.Error("Failed to connect to additional database, running without database access: " + additionalDatabase.DbConnectionError);
                    }

                    SapientProtocol.AdditionalDatabase = additionalDatabase;
                }

                // use dedicated database connection for tasking connection.
                {
                    taskDatabase = new Database(databaseServer, databasePort, databaseUser, databasePassword, databaseName);
                    if (string.IsNullOrEmpty(taskDatabase.DbConnectionError) == false)
                    {
                        Log.Error("Failed to connect to task database, running without database access: " + taskDatabase.DbConnectionError);
                    }
                }
            }
        }

        /// <summary>
        /// Initialises the status monitoring.
        /// </summary>
        /// <param name="databaseServer">The database server.</param>
        /// <param name="databasePort">The database port.</param>
        /// <param name="databaseUser">The database user.</param>
        /// <param name="databasePassword">The database password.</param>
        /// <param name="usingDatabase">if set to <c>true</c> [using database].</param>
        /// <param name="showWindow">if set to <c>true</c> [show window].</param>
        /// <param name="windowX">The window x.</param>
        /// <param name="windowY">The window y.</param>
        private static void InitialiseStatusMonitoring(string databaseServer, string databasePort, string databaseUser, string databasePassword, bool usingDatabase, bool showWindow, int windowX, int windowY)
        {
            LogDatabase = null;
            if (usingDatabase)
            {
                LogDatabase = new MiddlewareLogDatabase(databaseServer, databasePort, databaseUser, databasePassword, "middleware_log", ClientPort);
            }

            MessageMonitor = new SapientMessageMonitor(LogDatabase);

            if (showWindow)
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                DataAgentForm = new ServerForm(windowX, windowY, Properties.Settings.Default.ShowTaskWindow, MessageMonitor);
                ConnectionMonitor = DataAgentForm;
            }
            else
            {
                DataAgentForm = null;
                ConnectionMonitor = new SapientConnectionMonitor();
            }
        }

        /// <summary>
        /// heartbeat timer callback.
        /// </summary>
        /// <param name="sender">sending object.</param>
        /// <param name="e">event arguments.</param>
        private static void TickHeartbeat(object sender, ElapsedEventArgs e)
        {
            heartbeatTimer.Stop();

            try
            {
                ClientComms.IsConnected();
            }
            catch (Exception ex)
            {
                Log.Error("Error detecting connection state:" + ex.Message);
            }

            if (numClients != ClientComms.NumClients)
            {
                numClients = ClientComms.NumClients;
                ConnectionMonitor.SetNumClients(numClients);
                Log.InfoFormat("{0} Client Connections", numClients);
            }

            if (!isDmm && timeOfLastMessage.HasValue && (DateTime.UtcNow - timeOfLastMessage.Value) > TimeSpan.FromSeconds(Properties.Settings.Default.DisconnectIntervalSecs))
            {
                Log.InfoFormat("No data received from client connection in {0} seconds", Properties.Settings.Default.DisconnectIntervalSecs);
                ClientComms.IsConnected();
                numClients = 0;
                ConnectionMonitor.SetNumClients(numClients);
            }

            bool taskingConnected = taskingComms.IsConnected();
            ConnectionMonitor.SetTaskManagerConnected(taskingConnected);

            // update GUI text
            if (DataAgentForm != null)
            {
                DataAgentForm.UpdateGUI();
            }

            int taskingConnections = 0;
            if (taskingConnected)
            {
                taskingConnections = 1;
            }

            double commsLatency = MessageMonitor.Latency[0];
            double databaseLatency = MessageMonitor.Latency[1];
            MessageMonitor.UpdateDatabase(numClients, taskingConnections, numGuiClients, Database.Connected, commsLatency, databaseLatency);

            heartbeatTimer.Start();
        }

        /// <summary>
        /// callback method to pass information from the ASM client.
        /// </summary>
        /// <param name="message">message being passed in.</param>
        /// <param name="client">ID number of client connecting.</param>
        private static void DataCallback(SapientMessage message, IConnection client)
        {
            if (IsBlankOrInvalidMessage(message, client))
            {
                return;
            }

            if (numClients == 0 && !isDmm)
            {
                numClients = 1;
                ConnectionMonitor.SetNumClients(numClients);
            }

            timeOfLastMessage = DateTime.UtcNow;

            SapientMessageParser sapient_message_parser;
            lock (ClientMessageParsers)
            {

                // Check whether ID is currently connected.
                if (ClientMessageParsers.ContainsKey(client.ConnectionID) == false)
                {
                    if (!isDmm)
                    {
                        ConnectionMonitor.SetNumClients(1);
                        ClientMessageParsers.Clear();
                        ClientMessageParsers[client.ConnectionID] = new SapientMessageParser(new SapientDataAgentClientProtocol(Properties.Settings.Default.ForwardAlerts, message.NodeId), discardUnterminatedMessages);
                    }
                    else
                    {
                        ClientMessageParsers[client.ConnectionID] = new SapientMessageParser(new DmmDataAgentClientProtocol(message.NodeId), discardUnterminatedMessages);
                        ClientMessageParsers[client.ConnectionID].SapientProtocol.ConnectionId = client.ConnectionID;
                    }
                }

                sapient_message_parser = ClientMessageParsers[client.ConnectionID];
            }

            try
            {
                sapient_message_parser.BuildAndNotify(message, database, client.ConnectionID);
            }
            catch (Exception ex)
            {
                Log.Error("Error in Processing Client Message " + client.ConnectionID + "\n" + ex);
                sapient_message_parser.ResetMessageBuffer();
            }
        }

        /// <summary>
        /// Callback method to pass information from the tasking client.
        /// </summary>
        /// <param name="message">message being passed in.</param>
        /// <param name="client">id number of client connecting.</param>
        private static void DataCallbackTask(SapientMessage message, IConnection client)
        {
            if (IsBlankOrInvalidMessage(message, client))
            {
                return;
            }

            timeOfLastTaskMessage = DateTime.UtcNow;

            try
            {
                taskingMessageParser.BuildAndNotify(message, taskDatabase, client.ConnectionID);
            }
            catch (Exception ex)
            {
                Log.Error("Error in Processing task message " + client.ConnectionID + "\n" + ex);
                taskingMessageParser.ResetMessageBuffer();
            }
        }

        /// <summary>
        /// Are the blank or invalid message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="client">The client.</param>
        /// <returns>A bool.</returns>
        private static bool IsBlankOrInvalidMessage(SapientMessage message, IConnection client)
        {
            if (message == null || message.ToString() == "{ }")
            {
                MessageMonitor.IncrementMessageCount(SapientMessageType.InvalidClient);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Callback method to pass information from the GUI client.
        /// </summary>
        /// <param name="message">message being passed in.</param>
        /// <param name="client">id number of client connecting.</param>
        private static void GuiDataCallback(SapientMessage message, IConnection client)
        {
            if (numGuiClients == 0)
            {
                numGuiClients = 1;
                ConnectionMonitor.SetNumGuiClients(numGuiClients);
            }

            if (IsBlankOrInvalidMessage(message, client))
            {
                return;
            }

            SapientMessageParser sapient_message_parser;
            lock (GuiClientMessageParsers)
            {
                if (GuiClientMessageParsers.ContainsKey(client.ConnectionID) == false)
                {
                    GuiClientMessageParsers[client.ConnectionID] = new SapientMessageParser(new GUIProtocol(TaskPermissions), discardUnterminatedMessages);
                    GuiClientMessageParsers[client.ConnectionID].SapientProtocol.ConnectionId = client.ConnectionID;
                }

                sapient_message_parser = GuiClientMessageParsers[client.ConnectionID];
            }

            try
            {
                sapient_message_parser.BuildAndNotify(message, guiDatabase, client.ConnectionID);
            }
            catch (Exception ex)
            {
                Log.Error("Error in Processing GUI Client Message " + client.ConnectionID + "\n" + ex);
                sapient_message_parser.ResetMessageBuffer();
            }
        }

        /// <summary>
        /// Client connected or disconnected callback method.
        /// </summary>
        /// <param name="message">connection status.</param>
        /// <param name="client">client object.</param>
        private static void ClientConnected(string message, IConnection client)
        {
            if (message == "Connected")
            {
                Log.InfoFormat("Client Socket ID {0} Connected, {1} connections", client.ConnectionID, ClientComms.NumClients);
                numClients = ClientComms.NumClients;
                ConnectionMonitor.SetNumClients(numClients);
                timeOfLastMessage = DateTime.UtcNow;
            }

            if (message == "Socket Disconnected")
            {
                Log.InfoFormat("Client Socket ID: {0} Disconnected", client.ConnectionID);

                if (ClientMessageParsers.ContainsKey(client.ConnectionID))
                {
                    var sensorIDs = ClientMessageParsers[client.ConnectionID].SapientProtocol.SensorIds;
                    foreach (string i in sensorIDs)
                    {
                        Log.Info("sensorID:" + i.ToString());
                    }

                    // remove association between dead connection and ASM ID.
                    ClientMessageParsers.Remove(client.ConnectionID);
                }
            }
        }

        /// <summary>
        /// Call back when GUI client connects.
        /// </summary>
        /// <param name="message">connection message.</param>
        /// <param name="client">connection object.</param>
        private static void GuiClientConnected(string message, IConnection client)
        {
            if (message == "Connected")
            {
                numGuiClients++;
            }

            ConnectionMonitor.SetNumGuiClients(numGuiClients);
            ////DataAgentForm.GUIclientsConnected.UIThread(() => DataAgentForm.GUIclientsConnected.Text = numGuiClients.ToString());
        }

        /// <summary>
        /// Tasking Connection callback.
        /// </summary>
        /// <param name="message">connection message.</param>
        /// <param name="client">connection object.</param>
        private static void TaskManagerConnected(string message, IConnection client)
        {
            if (!isDmm)
            {
                if (message == "Connected To Server")
                {
                    Log.Info("Connected To HDA");
                    TaskingCommsConnection = client;
                    ConnectionMonitor.SetTaskManagerConnected(true);

                    // re-register with new HDA.
                    if (Registration != null)
                    {
                        client.SendMessage(Registration);
                    }
                }
                else
                {
                    Log.Error("Not Connected to HDA");
                }
            }
            else
            {
                if (message == "Connected")
                {
                    TaskingCommsConnection = client;
                    ConnectionMonitor.SetTaskManagerConnected(true);
                }
            }

            if (message == "Socket Disconnected")
            {
                ConnectionMonitor.SetTaskManagerConnected(false);
            }
        }

        /// <summary>
        /// set log path for log4net.
        /// </summary>
        private static string CreateLogFolder()
        {
            string logDirectory = Properties.Settings.Default.LogDirectory;

            // make log directory specific to this instance.
            if (isDmm)
            {
                logDirectory = string.Format("{0}-HDA", logDirectory);
            }
            else
            {
                logDirectory = string.Format("{0}-SDA {1:D3}", logDirectory, ClientPort);
            }

            // if log directory doesn't exist, make it.
            if (!System.IO.Directory.Exists(logDirectory))
            {
                System.IO.Directory.CreateDirectory(logDirectory);
            }

            return logDirectory;
        }

        /// <summary>
        /// Initialises the log4net.
        /// </summary>
        /// <param name="logDirectory">The log directory.</param>
        private static void InitialiseLog4net(string logDirectory)
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
                    string logFileLocation = System.IO.Path.Combine(logDirectory, "log.txt");
                    fa.File = logFileLocation;
                    fa.ActivateOptions();
                    break;
                }
            }

            // Output the assembly name and version number for configuration purposes.
            string[] assemblyDetails = System.Reflection.Assembly.GetExecutingAssembly().FullName.Split(',', '=');
            const int ExeName = 0;
            const int ExeVersion = 2;
            Log.Info(Environment.NewLine);
            Log.Info(assemblyDetails[ExeName] + " - Version " + assemblyDetails[ExeVersion]);
        }

        /// <summary>
        /// Initialises the data logging.
        /// </summary>
        /// <param name="logDirectory">The log directory.</param>
        private static void InitialiseDataLogging(string logDirectory)
        {
            if (Properties.Settings.Default.LogTasking || Properties.Settings.Default.LogClient)
            {
                string logPrefix = Properties.Settings.Default.LogPrefix;
                if (isDmm)
                {
                    logPrefix = "hldmm_receivelog";
                }

                sapientLogger = SapientLogger.CreateLogger(logDirectory, logPrefix, Properties.Settings.Default.IncrementIntervalSeconds);
            }
        }

        /// <summary>
        /// Calculates the window location.
        /// </summary>
        /// <param name="windowX">The window x.</param>
        /// <param name="windowY">The window y.</param>
        /// <param name="args">The arguments.</param>
        private static void CalculateWindowLocation(out int windowX, out int windowY, string[] args)
        {
            windowX = Properties.Settings.Default.WindowX;
            windowY = Properties.Settings.Default.WindowY;

            if (fixedAsmPortId >= 0)
            {
                const int windowWidth = 220;
                int windowHeight = 500;

                // override layout
                if (windowY > 0)
                {
                    windowHeight = windowY;
                }

                // For each screen, add the screen properties to a list box.
                foreach (var screen in System.Windows.Forms.Screen.AllScreens)
                {
                    int screenWidth = screen.WorkingArea.Width;
                    int screenHeight = screen.WorkingArea.Height;

                    int numWindowsX = screenWidth / windowWidth;
                    int numWindowsY = screenHeight / windowHeight;

                    Log.InfoFormat("screenWidth = {0} num windows = {1}", screenWidth, numWindowsX);
                    Log.InfoFormat("screenHeight = {0} num windows = {1}", screenHeight, numWindowsY);

                    int x = fixedAsmPortId % numWindowsX;
                    int y = fixedAsmPortId / numWindowsX;

                    windowX = x * windowWidth;
                    windowY = y * windowHeight;

                    Log.InfoFormat("Window ID:{0} x:{1} y:{2} windowX:{3} windowY:{4}", fixedAsmPortId, x, y, windowX, windowY);
                }
            }

            // override auto calculated gui window location.
            if (args.Length == 3)
            {
                windowX = int.Parse(args[1]);
                windowY = int.Parse(args[2]);
                Log.InfoFormat("Command Line Window Location X:{0} Y:{1}", windowX, windowY);
            }
        }
    }
}
