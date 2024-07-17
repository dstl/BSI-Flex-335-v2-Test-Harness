// File:              $Workfile: ClientForm.cs$
// <copyright file="ClientForm.cs">
// Crown-owned copyright, 2021-2024
// </copyright>

namespace SapientASMsimulator
{
    using System;
    using System.IO;
    using System.Windows.Forms;
    using log4net;
    using SapientASMsimulator.Common;

    /// <summary>
    /// class to display the ASM Client Form
    /// </summary>
    public partial class ClientForm : Form, IGUIInterface
    {
        #region Private Data Members

        /// <summary>
        /// Log4net logger
        /// </summary>
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// count for detection messages sent
        /// </summary>
        private long guiDetectionCount = 0;

        /// <summary>
        /// count for heartbeat messages sent
        /// </summary>
        private long guiHeartBeatCount = 0;

        /// <summary>
        /// count for alert messages sent
        /// </summary>
        private long guiAlertCount = 0;

        /// <summary>
        /// Main Processing Class
        /// </summary>
        private ASMMainProcess mainProcess;

        #endregion

        #region public methods

        /// <summary>
        /// Initializes a new instance of the ClientForm class
        /// </summary>
        public ClientForm()
        {
            this.mainProcess = new ASMMainProcess(this);
            this.InitializeComponent();
            this.ASMText.Text = ASMMainProcess.AsmId;
            BaseGenerators.ASMId = ASMMainProcess.AsmId;
        }

        /// <summary>
        /// delegate for Updater Callback
        /// </summary>
        /// <param name="t">text to update output window with</param>
        public delegate void UpdaterCallback(string t);

        /// <summary>
        /// function and delegate for updating the GUI from outside the UI thread
        /// </summary>
        /// <param name="text">message to print on GUI</param>
        public void StringUpdater(string text)
        {
            OutputWindow.AppendText(text + "\r\n");
        }

        /// <summary>
        /// method to Update Output Window that can be called from outside the UI thread
        /// </summary>
        /// <param name="message">message to update with</param>
        public void UpdateOutputText(string message)
        {
            if (this.OutputWindow.InvokeRequired)
            {
                this.OutputWindow.Invoke(new UpdaterCallback(this.StringUpdater), message);
            }
            else
            {
                this.StringUpdater(message);
            }
        }

        /// <summary>
        /// Update ASM ID text
        /// </summary>
        /// <param name="text">text to update</param>
        public void UpdateASMText(string text)
        {
            if (this.ASMText.InvokeRequired)
            {
                this.ASMText.Invoke(new UpdaterCallback(this.UpdateASMText), text);
            }
            else
            {
                this.ASMText.Text = text;
            }
        }

        #endregion

        #region private methods

        /// <summary>
        /// sets up the connections for client object
        /// </summary>
        /// <param name="sender">The parameter is not used.</param>
        /// <param name="e">The parameter is not used.</param>
        private void Form1Load(object sender, EventArgs e)
        {
            var server_name = Properties.Settings.Default.SapientDataAgentAddress;
            int server_port = Properties.Settings.Default.SapientDataAgentPort;

            if (!string.IsNullOrWhiteSpace(ASMMainProcess.PortId) && int.Parse(ASMMainProcess.PortId) > 0)
            {
                server_port = int.Parse(ASMMainProcess.PortId);
            }

            this.mainProcess.Initialise();
            this.mainProcess.SetAssociatedFilename(filenameTextBox.Text);

            this.Text = this.Text + ": " + server_port;
            openFile.InitialDirectory = Application.StartupPath;
            openFile.Filter = "Json Files (.json)|*.json|All Files (*.*)|*.*";
            openFile.FilterIndex = 1;

            openLogFile.InitialDirectory = Application.StartupPath;
            openLogFile.Filter = "Log Files (.log)|*.log|All Files (*.*)|*.*";
            openLogFile.FilterIndex = 1;
        }

        #region GUI event functions

        /// <summary>
        /// Calls the generator for the registration message
        /// </summary>
        /// <param name="sender">The parameter is not used.</param>
        /// <param name="e">The parameter is not used.</param>
        private void SendRegistrationClick(object sender, EventArgs e)
        {
            this.mainProcess.SendRegistration();
        }

        /// <summary>
        /// start thread to send detections. 
        /// </summary>
        /// <param name="sender">The parameter is not used.</param>
        /// <param name="e">The parameter is not used.</param>
        private void SendDetectionClick(object sender, EventArgs e)
        {
            this.mainProcess.SendDetectionLoop();
        }

        /// <summary>
        /// Starts thread to heart gen. 
        /// reads time from textbox on UI
        /// </summary>
        /// <param name="sender">The parameter is not used.</param>
        /// <param name="e">The parameter is not used.</param>
        private void SendHeartbeatClick(object sender, EventArgs e)
        {
            this.mainProcess.SendHeartbeatLoop();
        }

        /// <summary>
        /// Read and send an file
        /// </summary>
        /// <param name="sender">The parameter is not used.</param>
        /// <param name="e">The parameter is not used.</param>
        private void ReadFileClick(object sender, EventArgs e)
        {
            var result = openFile.ShowDialog(); // Show the dialog.
            if (result == DialogResult.OK)
            {
                var file_path = openFile.FileName;
                openFile.InitialDirectory = Path.GetDirectoryName(openFile.FileName);
                this.mainProcess.ReadAndSendFile(file_path);
            }
        }

        /// <summary>
        /// Event change method for the loop check box
        /// </summary>
        /// <param name="sender">The parameter is not used.</param>
        /// <param name="e">The parameter is not used.</param>
        private void LoopDetectionCheckedChanged(object sender, EventArgs e)
        {
            this.mainProcess.SetDetectionLoopState(this.LoopDetection.Checked);
        }

        /// <summary>
        /// method to kill either(or both) of the heartbeat or detection threads. 
        /// </summary>
        /// <param name="sender">The parameter is not used.</param>
        /// <param name="e">The parameter is not used.</param>
        private void LoopHeartbeatCheckedChanged(object sender, EventArgs e)
        {
            this.mainProcess.SetHeartbeatLoopState(this.Heartbeat.Checked);
        }

        #endregion

        /// <summary>
        /// Clear output window
        /// </summary>
        /// <param name="sender">The parameter is not used.</param>
        /// <param name="e">The parameter is not used.</param>
        private void ClearClick(object sender, EventArgs e)
        {
            this.OutputWindow.Text = string.Empty;
        }

        #endregion

        /// <summary>
        /// Client Form Closed Event from UI
        /// shuts down the connection
        /// </summary>
        /// <param name="sender">The parameter is not used.</param>
        /// <param name="e">The parameter is not used.</param>
        private void ClientForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.mainProcess.Shutdown();
        }

        /// <summary>
        /// Click event from the Send Alert button
        /// generates and sends the Alert message
        /// </summary>
        /// <param name="sender">The parameter is not used.</param>
        /// <param name="e">The parameter is not used.</param>
        private void SendAlertClick(object sender, EventArgs e)
        {
            this.mainProcess.SendAlertLoop();
        }

        /// <summary>
        /// Update detection count text box
        /// </summary>
        /// <param name="sender">The parameter is not used.</param>
        /// <param name="e">The parameter is not used.</param>
        private void GUITimer_Tick(object sender, EventArgs e)
        {
            long detectionCount = this.mainProcess.DetectionCount();
            long heartbeatCount = this.mainProcess.HeartBeatCount();
            long alertCount = this.mainProcess.AlertCount();

            if (this.guiDetectionCount != detectionCount)
            {
                this.guiDetectionCount = detectionCount;
                this.detectionCountTextBox.Text = this.guiDetectionCount.ToString();
            }

            if (this.guiHeartBeatCount != heartbeatCount)
            {
                this.guiHeartBeatCount = heartbeatCount;
                this.heartbeatCountTextBox.Text = this.guiHeartBeatCount.ToString();
            }

            if (this.guiAlertCount != alertCount)
            {
                this.guiAlertCount = alertCount;
                this.alertCountTextBox.Text = this.guiAlertCount.ToString();
            }
        }

        /// <summary>
        /// Toggle alert message sending loop check box
        /// </summary>
        /// <param name="sender">The parameter is not used.</param>
        /// <param name="e">The parameter is not used.</param>
        private void LoopAlerts_CheckedChanged(object sender, EventArgs e)
        {
            this.mainProcess.SetAlertLoopState(LoopAlerts.Checked);
        }

        /// <summary>
        /// Send list of files button
        /// </summary>
        /// <param name="sender">The parameter is not used.</param>
        /// <param name="e">The parameter is not used.</param>
        private void fileScriptButton_Click(object sender, EventArgs e)
        {
            ScriptReader.ScriptForm scriptForm = new ScriptReader.ScriptForm(this.mainProcess);
            this.mainProcess.AddDataReceivedCallback(scriptForm.DataCallback);
            scriptForm.ShowDialog();

        }

        /// <summary>
        /// ASM ID text box changed by user
        /// </summary>
        /// <param name="sender">The parameter is not used.</param>
        /// <param name="e">The parameter is not used.</param>
        private void ASMText_TextChanged(object sender, EventArgs e)
        {
            ASMMainProcess.AsmId = this.ASMText.Text;
            BaseGenerators.ASMId = this.ASMText.Text;
            TaskMessage.AsmId = this.ASMText.Text;
            RegistrationGenerator.AsmId = this.ASMText.Text;
            TaskAckGenerator.AsmId = this.ASMText.Text;
        }

        /// <summary>
        /// Heartbeat time text box changed by user
        /// </summary>
        /// <param name="sender">The parameter is not used.</param>
        /// <param name="e">The parameter is not used.</param>
        private void HeartbeatTime_TextChanged(object sender, EventArgs e)
        {
            int heartbeatTime = 0;

            if (int.TryParse(this.HeartbeatTime.Text, out heartbeatTime))
            {
                this.mainProcess.HeartbeatLoopTime = heartbeatTime;
            }
            else
            {
                Log.ErrorFormat("Error parsing heartbeat loop time: {0}", this.HeartbeatTime.Text);
                this.HeartbeatTime.Text = this.mainProcess.HeartbeatLoopTime.ToString();
            }
        }

        /// <summary>
        /// Detection time text box changed by user
        /// </summary>
        /// <param name="sender">The parameter is not used.</param>
        /// <param name="e">The parameter is not used.</param>
        private void DetectionTime_TextChanged(object sender, EventArgs e)
        {
            int detectionTime = 0;

            if (int.TryParse(this.DetectionTime.Text, out detectionTime))
            {
                this.mainProcess.DetectionLoopTime = detectionTime;
            }
            else
            {
                Log.ErrorFormat("Error parsing detection loop time: {0}", this.DetectionTime.Text);
                this.DetectionTime.Text = this.mainProcess.DetectionLoopTime.ToString();
            }
        }

        private void filenameTextBox_TextChanged(object sender, EventArgs e)
        {
            this.mainProcess.SetAssociatedFilename(filenameTextBox.Text);
        }
    }
}
