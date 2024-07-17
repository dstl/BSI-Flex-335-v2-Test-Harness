// Project:           $Project: 00010855-Sapient$
// File:              $Workfile: ServerForm.cs$
// Crown-owned copyright, 2021-2024
//  See Release/Supply Conditions
// $NoKeywords$

namespace SapientMiddleware
{
    using System;
    using System.Windows.Forms;
    using SapientDatabase;
    using SapientServices;

    /// <summary>
    /// Class for form displaying dynamic message counts.
    /// </summary>
    public partial class ServerForm : Form, IConnectionMonitor
    {
        private double commsLatencyWarningThreshold = 100;

        private double commsLatencyErrorThreshold = 1000;

        private double databaseLatencyWarningThreshold = 20;

        private double databaseLatencyErrorThreshold = 100;

        //// private TextForm textForm;

        private LocationOffsetForm offsetForm;

        private SapientMessageMonitor SapientMessageMonitor;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServerForm" /> class.
        /// </summary>
        /// <param name="x">window x screen coordinate.</param>
        /// <param name="y">window y screen coordinate.</param>
        /// <param name="showTaskForm">show task form.</param>
        /// <param name="sapientMessageMonitor">message monitor object.</param>
        public ServerForm(int x, int y, bool showTaskForm, SapientMessageMonitor sapientMessageMonitor)
        {
            if (x > -1)
            {
                this.StartPosition = FormStartPosition.Manual;
                this.DesktopLocation = new System.Drawing.Point(x, y);
            }

            InitializeComponent();
            SetTaskManagerConnectedStatus(false);
            SetClientConnectedStatus(false);
            commsLatencyWarningThreshold = Properties.Settings.Default.CommsLatencyWarningThreshold;
            commsLatencyErrorThreshold = Properties.Settings.Default.CommsLatencyErrorThreshold;
            ////if (showTaskForm)
            ////{
            ////    textForm = new TextForm();
            ////    textForm.Show();
            ////    textForm.Text = "Task List";
            ////}

            ////this.resetBtn.Hide();
            SapientMessageMonitor = sapientMessageMonitor;
        }

        /// <summary>
        /// Set client connected indicator.
        /// </summary>
        /// <param name="connected">whether connected.</param>
        public void SetClientConnectedStatus(bool connected)
        {
            this.clientStatusIndicator.UIThread(() => this.clientStatusIndicator.SetStatus(connected));
        }

        /// <summary>
        /// Set Number of Clients.
        /// </summary>
        /// <param name="numClients">Number of Clients.</param>
        public void SetNumClients(int numClients)
        {
            this.SetClientConnectedStatus(numClients > 0);
            this.ClientsConnected.UIThread(() => this.ClientsConnected.Text = numClients.ToString());
        }

        /// <summary>
        /// Set Number of GUI Clients Connected.
        /// </summary>
        /// <param name="numClients">Number of GUI Clients.</param>
        public void SetNumGuiClients(int numClients)
        {
            this.GUIclientsConnected.UIThread(() => this.GUIclientsConnected.Text = numClients.ToString());
        }

        /// <summary>
        /// Hide indications for GUI connections.
        /// </summary>
        public void DisableGUIConnectionReporting()
        {
            GUIclientsConnected.Hide();
            guiClientsLabel.Hide();
        }

        /// <summary>
        /// Set window title text.
        /// </summary>
        /// <param name="text">text string.</param>
        public void SetWindowText(string text)
        {
            this.UIThread(() => this.Text = text);
        }

        /// <summary>
        /// Update status indicators on GUI.
        /// </summary>
        public void UpdateGUI()
        {
            UpdateMessageTimes();
            UpdateStatusText();
            UpdateMessageCounts();
            this.SapientMessageMonitor.ClearRecent();
        }

        /// <summary>
        /// Update Message count displays.
        /// </summary>
        private void UpdateMessageCounts()
        {
            if (SapientMessageMonitor != null)
            {
                this.DetectionReport.UIThread(() => this.DetectionReport.Text = SapientMessageMonitor.MessageCount[(int)SapientMessageType.Detection].ToString());
                this.StatusReport.UIThread(() => this.StatusReport.Text = SapientMessageMonitor.MessageCount[(int)SapientMessageType.Status].ToString());
                this.Alert.UIThread(() => this.Alert.Text = SapientMessageMonitor.MessageCount[(int)SapientMessageType.Alert].ToString());
                this.SensorTask.UIThread(() => this.SensorTask.Text = SapientMessageMonitor.MessageCount[(int)SapientMessageType.Task].ToString());
                this.SensorTaskAck.UIThread(() => this.SensorTaskAck.Text = SapientMessageMonitor.MessageCount[(int)SapientMessageType.TaskACK].ToString());
                this.AlertResponse.UIThread(() => this.AlertResponse.Text = SapientMessageMonitor.MessageCount[(int)SapientMessageType.AlertResponse].ToString());
                this.SensorRegistration.UIThread(() => this.SensorRegistration.Text = SapientMessageMonitor.MessageCount[(int)SapientMessageType.Registration].ToString());
                this.ErrorOnId.UIThread(() => this.ErrorOnId.Text = SapientMessageMonitor.MessageCount[(int)SapientMessageType.IdError].ToString());
                this.InternalError.UIThread(() => this.InternalError.Text = SapientMessageMonitor.MessageCount[(int)SapientMessageType.InternalError].ToString());
                this.UnknownTask.UIThread(() => this.UnknownTask.Text = SapientMessageMonitor.MessageCount[(int)SapientMessageType.InvalidTasking].ToString());
                this.Unrecognised.UIThread(() => this.Unrecognised.Text = SapientMessageMonitor.MessageCount[(int)SapientMessageType.Unknown].ToString());
                this.UnknownClient.UIThread(() => this.UnknownClient.Text = SapientMessageMonitor.MessageCount[(int)SapientMessageType.InvalidClient].ToString());
            }
        }

        /// <summary>
        /// Update Message time displays.
        /// </summary>
        private void UpdateMessageTimes()
        {
            if (SapientMessageMonitor != null)
            {
                this.detectionReportTime.UIThread(() => this.detectionReportTime.Text = SapientMessageMonitor.MessageTime[(int)SapientMessageType.Detection].ToString("HH:mm:ss"));
                this.statusReportTime.UIThread(() => this.statusReportTime.Text = SapientMessageMonitor.MessageTime[(int)SapientMessageType.Status].ToString("HH:mm:ss"));
                this.alertTime.UIThread(() => this.alertTime.Text = SapientMessageMonitor.MessageTime[(int)SapientMessageType.Alert].ToString("HH:mm:ss"));
                this.sensorTaskTime.UIThread(() => this.sensorTaskTime.Text = SapientMessageMonitor.MessageTime[(int)SapientMessageType.Task].ToString("HH:mm:ss"));
                this.taskAckTime.UIThread(() => this.taskAckTime.Text = SapientMessageMonitor.MessageTime[(int)SapientMessageType.TaskACK].ToString("HH:mm:ss"));
                this.alertResponseTime.UIThread(() => this.alertResponseTime.Text = SapientMessageMonitor.MessageTime[(int)SapientMessageType.AlertResponse].ToString("HH:mm:ss"));
                this.RegistrationTime.UIThread(() => this.RegistrationTime.Text = SapientMessageMonitor.MessageTime[(int)SapientMessageType.Registration].ToString("HH:mm:ss"));
                this.idErrorTime.UIThread(() => this.idErrorTime.Text = SapientMessageMonitor.MessageTime[(int)SapientMessageType.IdError].ToString("HH:mm:ss"));
                this.internalErrorTime.UIThread(() => this.internalErrorTime.Text = SapientMessageMonitor.MessageTime[(int)SapientMessageType.InternalError].ToString("HH:mm:ss"));
                this.invalidTaskTime.UIThread(() => this.invalidTaskTime.Text = SapientMessageMonitor.MessageTime[(int)SapientMessageType.InvalidTasking].ToString("HH:mm:ss"));
                this.unrecognisedTime.UIThread(() => this.unrecognisedTime.Text = SapientMessageMonitor.MessageTime[(int)SapientMessageType.Unknown].ToString("HH:mm:ss"));
                this.invalidClientTime.UIThread(() => this.invalidClientTime.Text = SapientMessageMonitor.MessageTime[(int)SapientMessageType.InvalidClient].ToString("HH:mm:ss"));
            }
        }

        /// <summary>
        /// Update status text displays.
        /// </summary>
        private void UpdateStatusText()
        {
            if (SapientMessageMonitor != null)
            {
                this.clientInfoTextBox.UIThread(() => this.clientInfoTextBox.Text = SapientMessageMonitor.StatusText[0]);
                this.statusTextBox.UIThread(() => this.statusTextBox.Text = SapientMessageMonitor.StatusText[1]);
                this.commsLatencyTextBox.UIThread(() => this.commsLatencyTextBox.Text = SapientMessageMonitor.Latency[0].ToString("F1") + "ms");
                this.databaseLatencyTextBox.UIThread(() => this.databaseLatencyTextBox.Text = SapientMessageMonitor.Latency[1].ToString("F1") + "ms");

                // communications latency
                if (SapientMessageMonitor.Latency[0] <= commsLatencyWarningThreshold)
                {
                    this.commsLatencyStatusIndicator.SetStatus(true);
                }
                else if ((SapientMessageMonitor.Latency[0] > commsLatencyWarningThreshold) && (SapientMessageMonitor.Latency[0] < commsLatencyErrorThreshold))
                {
                    this.commsLatencyStatusIndicator.SetStatusWarning();
                }
                else if (SapientMessageMonitor.Latency[0] >= commsLatencyErrorThreshold)
                {
                    this.commsLatencyStatusIndicator.SetStatus(false);
                }

                if (Database.Connected)
                {
                    // database latency
                    if (SapientMessageMonitor.Latency[1] <= databaseLatencyWarningThreshold)
                    {
                        this.databaseLatencyStatusIndicator.SetStatus(true);
                    }
                    else if ((SapientMessageMonitor.Latency[1] > databaseLatencyWarningThreshold) && (SapientMessageMonitor.Latency[1] < databaseLatencyErrorThreshold))
                    {
                        this.databaseLatencyStatusIndicator.SetStatusWarning();
                    }
                    else if (SapientMessageMonitor.Latency[1] >= databaseLatencyErrorThreshold)
                    {
                        this.databaseLatencyStatusIndicator.SetStatus(false);
                    }
                }
                else
                {
                    // database not connected
                    this.databaseLatencyStatusIndicator.SetStatus(true);
                    this.databaseLatencyStatusIndicator.SetStatus(false);
                    this.databaseLatencyTextBox.UIThread(() => this.databaseLatencyTextBox.Text = "N/C");
                }

                if (SapientMessageMonitor.RecentError)
                {
                    this.recentMsgStatusIndicator.SetStatusWarning();
                }
                else if (SapientMessageMonitor.RecentMessage)
                {
                    this.recentMsgStatusIndicator.SetStatus(true);
                }
                else
                {
                    this.recentMsgStatusIndicator.SetStatus(false);
                }
            }
        }

        /// <summary>
        /// Set Task Manager connected tick box.
        /// </summary>
        /// <param name="connected">whether connected.</param>
        public void SetTaskManagerConnected(bool connected)
        {
            SetTaskManagerConnectedStatus(connected);
            this.TaskManagerConnected.UIThread(() => this.TaskManagerConnected.Checked = connected);
        }

        /// <summary>
        /// Set Task Manager connected indicator.
        /// </summary>
        /// <param name="connected">whether connected.</param>
        private void SetTaskManagerConnectedStatus(bool connected)
        {
            this.taskConnectionStatusIndicator.UIThread(() => this.taskConnectionStatusIndicator.SetStatus(connected));
        }

        /// <summary>
        /// start communications when form loaded.
        /// </summary>
        /// <param name="sender">event sender.</param>
        /// <param name="e">event arguments.</param>
        private void ServerFormLoad(object sender, EventArgs e)
        {
            Program.StartComms();
        }

        /// <summary>
        /// handler for server form closed event.
        /// exits the application.
        /// </summary>
        /// <param name="sender">event sender.</param>
        /// <param name="e">event arguments.</param>
        private void ServerForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        /// <summary>
        /// Reset button pressed.
        /// </summary>
        /// <param name="sender">sender object.</param>
        /// <param name="e">event arguments.</param>
        private void Button1_Click(object sender, EventArgs e)
        {
            if (this.Height < 300)
            {
                this.Size = new System.Drawing.Size(240, 600);
            }
            else
            {
                this.Size = new System.Drawing.Size(240, 270);
            }

            //// Program.Reset();
            //// ClearMessageTimes();
        }

        /// <summary>
        /// Open sensor offset form.
        /// </summary>
        /// <param name="sender">sender object.</param>
        /// <param name="e">event arguments.</param>
        private void btnOffsets_Click(object sender, EventArgs e)
        {
            offsetForm = new LocationOffsetForm();
            offsetForm.Show();
        }
    }

    /// <summary>
    /// class to contain Control Extensions.
    /// </summary>
    public static class ControlExtensions
    {
        /// <summary>
        /// Executes the Action asynchronously on the UI thread, does not block execution on the calling thread.
        /// </summary>
        /// <param name="control">extended control.</param>
        /// <param name="action">action to invoke.</param>
        public static void UIThread(this Control control, Action action)
        {
            if (control.InvokeRequired)
            {
                control.BeginInvoke(action);
            }
            else
            {
                action.Invoke();
            }
        }
    }
}
