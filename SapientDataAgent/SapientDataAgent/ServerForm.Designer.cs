// Project:           $Project: 00010855-Sapient$
// File:              $Workfile: ServerForm.Designer.cs$
// Crown-owned copyright, 2021-2024
//  See Release/Supply Conditions
// $NoKeywords$

namespace SapientMiddleware
{
    /// <summary>
    /// Class for form displaying dynamic message counts
    /// </summary>
    public partial class ServerForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ServerForm));
            this.label1 = new System.Windows.Forms.Label();
            this.SensorRegistration = new System.Windows.Forms.TextBox();
            this.StatusReport = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.SensorTask = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.DetectionReport = new System.Windows.Forms.TextBox();
            this.SensorTaskAck = new System.Windows.Forms.TextBox();
            this.ErrorOnId = new System.Windows.Forms.TextBox();
            this.UnknownClient = new System.Windows.Forms.TextBox();
            this.UnknownTask = new System.Windows.Forms.TextBox();
            this.Unrecognised = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.TaskManagerConnected = new System.Windows.Forms.CheckBox();
            this.ClientsConnected = new System.Windows.Forms.TextBox();
            this.AlertResponse = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.Alert = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.GUIclientsConnected = new System.Windows.Forms.TextBox();
            this.guiClientsLabel = new System.Windows.Forms.Label();
            this.resetBtn = new System.Windows.Forms.Button();
            this.InternalError = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.RegistrationTime = new System.Windows.Forms.TextBox();
            this.statusReportTime = new System.Windows.Forms.TextBox();
            this.detectionReportTime = new System.Windows.Forms.TextBox();
            this.sensorTaskTime = new System.Windows.Forms.TextBox();
            this.taskAckTime = new System.Windows.Forms.TextBox();
            this.alertTime = new System.Windows.Forms.TextBox();
            this.alertResponseTime = new System.Windows.Forms.TextBox();
            this.idErrorTime = new System.Windows.Forms.TextBox();
            this.invalidClientTime = new System.Windows.Forms.TextBox();
            this.invalidTaskTime = new System.Windows.Forms.TextBox();
            this.unrecognisedTime = new System.Windows.Forms.TextBox();
            this.internalErrorTime = new System.Windows.Forms.TextBox();
            this.databaseLatencyTextBox = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.commsLatencyTextBox = new System.Windows.Forms.TextBox();
            this.label16 = new System.Windows.Forms.Label();
            this.clientInfoTextBox = new System.Windows.Forms.TextBox();
            this.label17 = new System.Windows.Forms.Label();
            this.statusTextBox = new System.Windows.Forms.TextBox();
            this.label18 = new System.Windows.Forms.Label();
            this.recentMsgStatusIndicator = new SapientMiddleware.StatusIndicator();
            this.databaseLatencyStatusIndicator = new SapientMiddleware.StatusIndicator();
            this.commsLatencyStatusIndicator = new SapientMiddleware.StatusIndicator();
            this.clientStatusIndicator = new SapientMiddleware.StatusIndicator();
            this.taskConnectionStatusIndicator = new SapientMiddleware.StatusIndicator();
            this.btnOffsets = new System.Windows.Forms.Button();
            this.label19 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 366);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(89, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "Registration";
            // 
            // SensorRegistration
            // 
            this.SensorRegistration.Enabled = false;
            this.SensorRegistration.Location = new System.Drawing.Point(124, 362);
            this.SensorRegistration.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.SensorRegistration.Name = "SensorRegistration";
            this.SensorRegistration.ReadOnly = true;
            this.SensorRegistration.Size = new System.Drawing.Size(65, 27);
            this.SensorRegistration.TabIndex = 1;
            this.SensorRegistration.Text = "0";
            // 
            // StatusReport
            // 
            this.StatusReport.Enabled = false;
            this.StatusReport.Location = new System.Drawing.Point(124, 402);
            this.StatusReport.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.StatusReport.Name = "StatusReport";
            this.StatusReport.ReadOnly = true;
            this.StatusReport.Size = new System.Drawing.Size(65, 27);
            this.StatusReport.TabIndex = 3;
            this.StatusReport.Text = "0";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(7, 406);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(98, 20);
            this.label2.TabIndex = 2;
            this.label2.Text = "Status Report";
            // 
            // SensorTask
            // 
            this.SensorTask.Enabled = false;
            this.SensorTask.Location = new System.Drawing.Point(124, 482);
            this.SensorTask.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.SensorTask.Name = "SensorTask";
            this.SensorTask.ReadOnly = true;
            this.SensorTask.Size = new System.Drawing.Size(65, 27);
            this.SensorTask.TabIndex = 5;
            this.SensorTask.Text = "0";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(7, 486);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(84, 20);
            this.label3.TabIndex = 4;
            this.label3.Text = "Sensor Task";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(7, 526);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(112, 20);
            this.label4.TabIndex = 6;
            this.label4.Text = "Sensor Task Ack";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(7, 446);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(123, 20);
            this.label5.TabIndex = 7;
            this.label5.Text = "Detection Report";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(7, 646);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(81, 20);
            this.label6.TabIndex = 8;
            this.label6.Text = "Error On Id";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(7, 686);
            this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(95, 20);
            this.label7.TabIndex = 9;
            this.label7.Text = "Invalid Client";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(7, 726);
            this.label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(105, 20);
            this.label8.TabIndex = 10;
            this.label8.Text = "Invalid Tasking";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(7, 766);
            this.label9.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(100, 20);
            this.label9.TabIndex = 11;
            this.label9.Text = "Unrecognised";
            // 
            // DetectionReport
            // 
            this.DetectionReport.Enabled = false;
            this.DetectionReport.Location = new System.Drawing.Point(124, 442);
            this.DetectionReport.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.DetectionReport.Name = "DetectionReport";
            this.DetectionReport.ReadOnly = true;
            this.DetectionReport.Size = new System.Drawing.Size(65, 27);
            this.DetectionReport.TabIndex = 12;
            this.DetectionReport.Text = "0";
            // 
            // SensorTaskAck
            // 
            this.SensorTaskAck.Enabled = false;
            this.SensorTaskAck.Location = new System.Drawing.Point(124, 522);
            this.SensorTaskAck.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.SensorTaskAck.Name = "SensorTaskAck";
            this.SensorTaskAck.ReadOnly = true;
            this.SensorTaskAck.Size = new System.Drawing.Size(65, 27);
            this.SensorTaskAck.TabIndex = 13;
            this.SensorTaskAck.Text = "0";
            // 
            // ErrorOnId
            // 
            this.ErrorOnId.Enabled = false;
            this.ErrorOnId.Location = new System.Drawing.Point(124, 642);
            this.ErrorOnId.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.ErrorOnId.Name = "ErrorOnId";
            this.ErrorOnId.ReadOnly = true;
            this.ErrorOnId.Size = new System.Drawing.Size(65, 27);
            this.ErrorOnId.TabIndex = 14;
            this.ErrorOnId.Text = "0";
            // 
            // UnknownClient
            // 
            this.UnknownClient.Enabled = false;
            this.UnknownClient.Location = new System.Drawing.Point(124, 682);
            this.UnknownClient.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.UnknownClient.Name = "UnknownClient";
            this.UnknownClient.ReadOnly = true;
            this.UnknownClient.Size = new System.Drawing.Size(65, 27);
            this.UnknownClient.TabIndex = 15;
            this.UnknownClient.Text = "0";
            // 
            // UnknownTask
            // 
            this.UnknownTask.Enabled = false;
            this.UnknownTask.Location = new System.Drawing.Point(124, 722);
            this.UnknownTask.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.UnknownTask.Name = "UnknownTask";
            this.UnknownTask.ReadOnly = true;
            this.UnknownTask.Size = new System.Drawing.Size(65, 27);
            this.UnknownTask.TabIndex = 16;
            this.UnknownTask.Text = "0";
            // 
            // Unrecognised
            // 
            this.Unrecognised.Enabled = false;
            this.Unrecognised.Location = new System.Drawing.Point(124, 762);
            this.Unrecognised.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Unrecognised.Name = "Unrecognised";
            this.Unrecognised.ReadOnly = true;
            this.Unrecognised.Size = new System.Drawing.Size(65, 27);
            this.Unrecognised.TabIndex = 17;
            this.Unrecognised.Text = "0";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(16, 14);
            this.label10.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(128, 20);
            this.label10.TabIndex = 18;
            this.label10.Text = "Clients Connected";
            // 
            // TaskManagerConnected
            // 
            this.TaskManagerConnected.AutoSize = true;
            this.TaskManagerConnected.Enabled = false;
            this.TaskManagerConnected.Location = new System.Drawing.Point(16, 55);
            this.TaskManagerConnected.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.TaskManagerConnected.Name = "TaskManagerConnected";
            this.TaskManagerConnected.Size = new System.Drawing.Size(154, 24);
            this.TaskManagerConnected.TabIndex = 19;
            this.TaskManagerConnected.Text = "Tasking Connected";
            this.TaskManagerConnected.UseVisualStyleBackColor = true;
            // 
            // ClientsConnected
            // 
            this.ClientsConnected.Enabled = false;
            this.ClientsConnected.Location = new System.Drawing.Point(135, 9);
            this.ClientsConnected.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.ClientsConnected.Name = "ClientsConnected";
            this.ClientsConnected.ReadOnly = true;
            this.ClientsConnected.Size = new System.Drawing.Size(65, 27);
            this.ClientsConnected.TabIndex = 20;
            this.ClientsConnected.Text = "0";
            // 
            // AlertResponse
            // 
            this.AlertResponse.Enabled = false;
            this.AlertResponse.Location = new System.Drawing.Point(124, 602);
            this.AlertResponse.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.AlertResponse.Name = "AlertResponse";
            this.AlertResponse.ReadOnly = true;
            this.AlertResponse.Size = new System.Drawing.Size(65, 27);
            this.AlertResponse.TabIndex = 24;
            this.AlertResponse.Text = "0";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(7, 606);
            this.label11.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(108, 20);
            this.label11.TabIndex = 23;
            this.label11.Text = "Alert Response";
            // 
            // Alert
            // 
            this.Alert.Enabled = false;
            this.Alert.Location = new System.Drawing.Point(124, 562);
            this.Alert.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Alert.Name = "Alert";
            this.Alert.ReadOnly = true;
            this.Alert.Size = new System.Drawing.Size(65, 27);
            this.Alert.TabIndex = 22;
            this.Alert.Text = "0";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(7, 566);
            this.label12.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(41, 20);
            this.label12.TabIndex = 21;
            this.label12.Text = "Alert";
            // 
            // GUIclientsConnected
            // 
            this.GUIclientsConnected.Enabled = false;
            this.GUIclientsConnected.Location = new System.Drawing.Point(124, 832);
            this.GUIclientsConnected.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.GUIclientsConnected.Name = "GUIclientsConnected";
            this.GUIclientsConnected.ReadOnly = true;
            this.GUIclientsConnected.Size = new System.Drawing.Size(65, 27);
            this.GUIclientsConnected.TabIndex = 26;
            this.GUIclientsConnected.Text = "0";
            // 
            // guiClientsLabel
            // 
            this.guiClientsLabel.AutoSize = true;
            this.guiClientsLabel.Location = new System.Drawing.Point(9, 837);
            this.guiClientsLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.guiClientsLabel.Name = "guiClientsLabel";
            this.guiClientsLabel.Size = new System.Drawing.Size(114, 20);
            this.guiClientsLabel.TabIndex = 25;
            this.guiClientsLabel.Text = "GUIs Connected";
            // 
            // resetBtn
            // 
            this.resetBtn.Location = new System.Drawing.Point(169, 55);
            this.resetBtn.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.resetBtn.Name = "resetBtn";
            this.resetBtn.Size = new System.Drawing.Size(67, 35);
            this.resetBtn.TabIndex = 27;
            this.resetBtn.Text = "Resize";
            this.resetBtn.UseVisualStyleBackColor = true;
            this.resetBtn.Click += new System.EventHandler(this.Button1_Click);
            // 
            // InternalError
            // 
            this.InternalError.Enabled = false;
            this.InternalError.Location = new System.Drawing.Point(124, 797);
            this.InternalError.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.InternalError.Name = "InternalError";
            this.InternalError.ReadOnly = true;
            this.InternalError.Size = new System.Drawing.Size(65, 27);
            this.InternalError.TabIndex = 31;
            this.InternalError.Text = "0";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(7, 802);
            this.label13.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(124, 20);
            this.label13.TabIndex = 30;
            this.label13.Text = "Middleware Error";
            // 
            // RegistrationTime
            // 
            this.RegistrationTime.Enabled = false;
            this.RegistrationTime.Location = new System.Drawing.Point(199, 362);
            this.RegistrationTime.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.RegistrationTime.Name = "RegistrationTime";
            this.RegistrationTime.ReadOnly = true;
            this.RegistrationTime.Size = new System.Drawing.Size(85, 27);
            this.RegistrationTime.TabIndex = 32;
            // 
            // statusReportTime
            // 
            this.statusReportTime.Enabled = false;
            this.statusReportTime.Location = new System.Drawing.Point(199, 402);
            this.statusReportTime.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.statusReportTime.Name = "statusReportTime";
            this.statusReportTime.ReadOnly = true;
            this.statusReportTime.Size = new System.Drawing.Size(85, 27);
            this.statusReportTime.TabIndex = 33;
            // 
            // detectionReportTime
            // 
            this.detectionReportTime.Enabled = false;
            this.detectionReportTime.Location = new System.Drawing.Point(199, 442);
            this.detectionReportTime.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.detectionReportTime.Name = "detectionReportTime";
            this.detectionReportTime.ReadOnly = true;
            this.detectionReportTime.Size = new System.Drawing.Size(85, 27);
            this.detectionReportTime.TabIndex = 34;
            // 
            // sensorTaskTime
            // 
            this.sensorTaskTime.Enabled = false;
            this.sensorTaskTime.Location = new System.Drawing.Point(199, 482);
            this.sensorTaskTime.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.sensorTaskTime.Name = "sensorTaskTime";
            this.sensorTaskTime.ReadOnly = true;
            this.sensorTaskTime.Size = new System.Drawing.Size(85, 27);
            this.sensorTaskTime.TabIndex = 35;
            // 
            // taskAckTime
            // 
            this.taskAckTime.Enabled = false;
            this.taskAckTime.Location = new System.Drawing.Point(199, 522);
            this.taskAckTime.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.taskAckTime.Name = "taskAckTime";
            this.taskAckTime.ReadOnly = true;
            this.taskAckTime.Size = new System.Drawing.Size(85, 27);
            this.taskAckTime.TabIndex = 36;
            // 
            // alertTime
            // 
            this.alertTime.Enabled = false;
            this.alertTime.Location = new System.Drawing.Point(199, 562);
            this.alertTime.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.alertTime.Name = "alertTime";
            this.alertTime.ReadOnly = true;
            this.alertTime.Size = new System.Drawing.Size(85, 27);
            this.alertTime.TabIndex = 37;
            // 
            // alertResponseTime
            // 
            this.alertResponseTime.Enabled = false;
            this.alertResponseTime.Location = new System.Drawing.Point(199, 602);
            this.alertResponseTime.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.alertResponseTime.Name = "alertResponseTime";
            this.alertResponseTime.ReadOnly = true;
            this.alertResponseTime.Size = new System.Drawing.Size(85, 27);
            this.alertResponseTime.TabIndex = 38;
            // 
            // idErrorTime
            // 
            this.idErrorTime.Enabled = false;
            this.idErrorTime.Location = new System.Drawing.Point(199, 642);
            this.idErrorTime.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.idErrorTime.Name = "idErrorTime";
            this.idErrorTime.ReadOnly = true;
            this.idErrorTime.Size = new System.Drawing.Size(85, 27);
            this.idErrorTime.TabIndex = 39;
            // 
            // invalidClientTime
            // 
            this.invalidClientTime.Enabled = false;
            this.invalidClientTime.Location = new System.Drawing.Point(199, 682);
            this.invalidClientTime.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.invalidClientTime.Name = "invalidClientTime";
            this.invalidClientTime.ReadOnly = true;
            this.invalidClientTime.Size = new System.Drawing.Size(85, 27);
            this.invalidClientTime.TabIndex = 40;
            // 
            // invalidTaskTime
            // 
            this.invalidTaskTime.Enabled = false;
            this.invalidTaskTime.Location = new System.Drawing.Point(199, 722);
            this.invalidTaskTime.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.invalidTaskTime.Name = "invalidTaskTime";
            this.invalidTaskTime.ReadOnly = true;
            this.invalidTaskTime.Size = new System.Drawing.Size(85, 27);
            this.invalidTaskTime.TabIndex = 41;
            // 
            // unrecognisedTime
            // 
            this.unrecognisedTime.Enabled = false;
            this.unrecognisedTime.Location = new System.Drawing.Point(199, 762);
            this.unrecognisedTime.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.unrecognisedTime.Name = "unrecognisedTime";
            this.unrecognisedTime.ReadOnly = true;
            this.unrecognisedTime.Size = new System.Drawing.Size(85, 27);
            this.unrecognisedTime.TabIndex = 42;
            // 
            // internalErrorTime
            // 
            this.internalErrorTime.Enabled = false;
            this.internalErrorTime.Location = new System.Drawing.Point(199, 797);
            this.internalErrorTime.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.internalErrorTime.Name = "internalErrorTime";
            this.internalErrorTime.ReadOnly = true;
            this.internalErrorTime.Size = new System.Drawing.Size(85, 27);
            this.internalErrorTime.TabIndex = 43;
            // 
            // databaseLatencyTextBox
            // 
            this.databaseLatencyTextBox.Enabled = false;
            this.databaseLatencyTextBox.Location = new System.Drawing.Point(135, 245);
            this.databaseLatencyTextBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.databaseLatencyTextBox.Name = "databaseLatencyTextBox";
            this.databaseLatencyTextBox.ReadOnly = true;
            this.databaseLatencyTextBox.Size = new System.Drawing.Size(85, 27);
            this.databaseLatencyTextBox.TabIndex = 44;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(9, 249);
            this.label14.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(126, 20);
            this.label14.TabIndex = 45;
            this.label14.Text = "Database Latency";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(12, 191);
            this.label15.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(113, 20);
            this.label15.TabIndex = 47;
            this.label15.Text = "Comms Latency";
            // 
            // commsLatencyTextBox
            // 
            this.commsLatencyTextBox.Enabled = false;
            this.commsLatencyTextBox.Location = new System.Drawing.Point(135, 186);
            this.commsLatencyTextBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.commsLatencyTextBox.Name = "commsLatencyTextBox";
            this.commsLatencyTextBox.ReadOnly = true;
            this.commsLatencyTextBox.Size = new System.Drawing.Size(85, 27);
            this.commsLatencyTextBox.TabIndex = 46;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(12, 115);
            this.label16.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(50, 20);
            this.label16.TabIndex = 51;
            this.label16.Text = "Client:";
            // 
            // clientInfoTextBox
            // 
            this.clientInfoTextBox.Enabled = false;
            this.clientInfoTextBox.Location = new System.Drawing.Point(115, 111);
            this.clientInfoTextBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.clientInfoTextBox.Name = "clientInfoTextBox";
            this.clientInfoTextBox.ReadOnly = true;
            this.clientInfoTextBox.Size = new System.Drawing.Size(180, 27);
            this.clientInfoTextBox.TabIndex = 50;
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(9, 151);
            this.label17.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(52, 20);
            this.label17.TabIndex = 49;
            this.label17.Text = "Status:";
            // 
            // statusTextBox
            // 
            this.statusTextBox.Enabled = false;
            this.statusTextBox.Location = new System.Drawing.Point(115, 151);
            this.statusTextBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.statusTextBox.Name = "statusTextBox";
            this.statusTextBox.ReadOnly = true;
            this.statusTextBox.Size = new System.Drawing.Size(180, 27);
            this.statusTextBox.TabIndex = 48;
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(120, 303);
            this.label18.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(122, 20);
            this.label18.TabIndex = 55;
            this.label18.Text = "Recent Messages";
            // 
            // recentMsgStatusIndicator
            // 
            this.recentMsgStatusIndicator.BackgroundImage = global::SapientMiddleware.Properties.Resources.redLight;
            this.recentMsgStatusIndicator.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.recentMsgStatusIndicator.Location = new System.Drawing.Point(253, 303);
            this.recentMsgStatusIndicator.Margin = new System.Windows.Forms.Padding(5, 8, 5, 8);
            this.recentMsgStatusIndicator.Name = "recentMsgStatusIndicator";
            this.recentMsgStatusIndicator.Size = new System.Drawing.Size(42, 48);
            this.recentMsgStatusIndicator.TabIndex = 54;
            // 
            // databaseLatencyStatusIndicator
            // 
            this.databaseLatencyStatusIndicator.BackgroundImage = global::SapientMiddleware.Properties.Resources.redLight;
            this.databaseLatencyStatusIndicator.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.databaseLatencyStatusIndicator.Location = new System.Drawing.Point(253, 245);
            this.databaseLatencyStatusIndicator.Margin = new System.Windows.Forms.Padding(5, 8, 5, 8);
            this.databaseLatencyStatusIndicator.Name = "databaseLatencyStatusIndicator";
            this.databaseLatencyStatusIndicator.Size = new System.Drawing.Size(42, 48);
            this.databaseLatencyStatusIndicator.TabIndex = 53;
            // 
            // commsLatencyStatusIndicator
            // 
            this.commsLatencyStatusIndicator.BackgroundImage = global::SapientMiddleware.Properties.Resources.redLight;
            this.commsLatencyStatusIndicator.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.commsLatencyStatusIndicator.Location = new System.Drawing.Point(253, 186);
            this.commsLatencyStatusIndicator.Margin = new System.Windows.Forms.Padding(5, 8, 5, 8);
            this.commsLatencyStatusIndicator.Name = "commsLatencyStatusIndicator";
            this.commsLatencyStatusIndicator.Size = new System.Drawing.Size(42, 48);
            this.commsLatencyStatusIndicator.TabIndex = 52;
            // 
            // clientStatusIndicator
            // 
            this.clientStatusIndicator.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("clientStatusIndicator.BackgroundImage")));
            this.clientStatusIndicator.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.clientStatusIndicator.Location = new System.Drawing.Point(253, 3);
            this.clientStatusIndicator.Margin = new System.Windows.Forms.Padding(5, 8, 5, 8);
            this.clientStatusIndicator.Name = "clientStatusIndicator";
            this.clientStatusIndicator.Size = new System.Drawing.Size(42, 48);
            this.clientStatusIndicator.TabIndex = 29;
            // 
            // taskConnectionStatusIndicator
            // 
            this.taskConnectionStatusIndicator.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("taskConnectionStatusIndicator.BackgroundImage")));
            this.taskConnectionStatusIndicator.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.taskConnectionStatusIndicator.Location = new System.Drawing.Point(253, 55);
            this.taskConnectionStatusIndicator.Margin = new System.Windows.Forms.Padding(5, 8, 5, 8);
            this.taskConnectionStatusIndicator.Name = "taskConnectionStatusIndicator";
            this.taskConnectionStatusIndicator.Size = new System.Drawing.Size(42, 48);
            this.taskConnectionStatusIndicator.TabIndex = 28;
            // 
            // btnOffsets
            // 
            this.btnOffsets.Location = new System.Drawing.Point(199, 834);
            this.btnOffsets.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnOffsets.Name = "btnOffsets";
            this.btnOffsets.Size = new System.Drawing.Size(85, 28);
            this.btnOffsets.TabIndex = 56;
            this.btnOffsets.Text = "Offsets";
            this.btnOffsets.UseVisualStyleBackColor = true;
            this.btnOffsets.Click += new System.EventHandler(this.btnOffsets_Click);
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Font = new System.Drawing.Font("Segoe UI", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label19.ForeColor = System.Drawing.SystemColors.ButtonShadow;
            this.label19.Location = new System.Drawing.Point(37, 872);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(219, 15);
            this.label19.TabIndex = 58;
            this.label19.Text = "Developed by Operational Solutions Ltd.";
            // 
            // ServerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(299, 898);
            this.Controls.Add(this.label19);
            this.Controls.Add(this.btnOffsets);
            this.Controls.Add(this.label18);
            this.Controls.Add(this.recentMsgStatusIndicator);
            this.Controls.Add(this.databaseLatencyStatusIndicator);
            this.Controls.Add(this.commsLatencyStatusIndicator);
            this.Controls.Add(this.label16);
            this.Controls.Add(this.clientInfoTextBox);
            this.Controls.Add(this.label17);
            this.Controls.Add(this.statusTextBox);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.commsLatencyTextBox);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.databaseLatencyTextBox);
            this.Controls.Add(this.internalErrorTime);
            this.Controls.Add(this.unrecognisedTime);
            this.Controls.Add(this.invalidTaskTime);
            this.Controls.Add(this.invalidClientTime);
            this.Controls.Add(this.idErrorTime);
            this.Controls.Add(this.alertResponseTime);
            this.Controls.Add(this.alertTime);
            this.Controls.Add(this.taskAckTime);
            this.Controls.Add(this.sensorTaskTime);
            this.Controls.Add(this.detectionReportTime);
            this.Controls.Add(this.statusReportTime);
            this.Controls.Add(this.RegistrationTime);
            this.Controls.Add(this.InternalError);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.clientStatusIndicator);
            this.Controls.Add(this.taskConnectionStatusIndicator);
            this.Controls.Add(this.resetBtn);
            this.Controls.Add(this.GUIclientsConnected);
            this.Controls.Add(this.guiClientsLabel);
            this.Controls.Add(this.AlertResponse);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.Alert);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.ClientsConnected);
            this.Controls.Add(this.TaskManagerConnected);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.Unrecognised);
            this.Controls.Add(this.UnknownTask);
            this.Controls.Add(this.UnknownClient);
            this.Controls.Add(this.ErrorOnId);
            this.Controls.Add(this.SensorTaskAck);
            this.Controls.Add(this.DetectionReport);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.SensorTask);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.StatusReport);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.SensorRegistration);
            this.Controls.Add(this.label1);
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.MaximizeBox = false;
            this.Name = "ServerForm";
            this.Text = "Sapient Data Agent";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ServerForm_FormClosed);
            this.Load += new System.EventHandler(this.ServerFormLoad);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.TextBox SensorRegistration;
        internal System.Windows.Forms.TextBox StatusReport;
        private System.Windows.Forms.Label label2;
        internal System.Windows.Forms.TextBox SensorTask;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        internal System.Windows.Forms.TextBox DetectionReport;
        internal System.Windows.Forms.TextBox SensorTaskAck;
        internal System.Windows.Forms.TextBox ErrorOnId;
        internal System.Windows.Forms.TextBox UnknownClient;
        internal System.Windows.Forms.TextBox UnknownTask;
        internal System.Windows.Forms.TextBox Unrecognised;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.CheckBox TaskManagerConnected;
        private System.Windows.Forms.TextBox ClientsConnected;
        internal System.Windows.Forms.TextBox AlertResponse;
        private System.Windows.Forms.Label label11;
        internal System.Windows.Forms.TextBox Alert;
        private System.Windows.Forms.Label label12;
        public System.Windows.Forms.TextBox GUIclientsConnected;
        public System.Windows.Forms.Label guiClientsLabel;
        private System.Windows.Forms.Button resetBtn;
        internal StatusIndicator taskConnectionStatusIndicator;
        internal StatusIndicator clientStatusIndicator;
        internal System.Windows.Forms.TextBox InternalError;
        private System.Windows.Forms.Label label13;
        internal System.Windows.Forms.TextBox RegistrationTime;
        internal System.Windows.Forms.TextBox statusReportTime;
        internal System.Windows.Forms.TextBox detectionReportTime;
        internal System.Windows.Forms.TextBox sensorTaskTime;
        internal System.Windows.Forms.TextBox taskAckTime;
        internal System.Windows.Forms.TextBox alertTime;
        internal System.Windows.Forms.TextBox alertResponseTime;
        internal System.Windows.Forms.TextBox idErrorTime;
        internal System.Windows.Forms.TextBox invalidClientTime;
        internal System.Windows.Forms.TextBox invalidTaskTime;
        internal System.Windows.Forms.TextBox unrecognisedTime;
        internal System.Windows.Forms.TextBox internalErrorTime;
        internal System.Windows.Forms.TextBox databaseLatencyTextBox;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label15;
        internal System.Windows.Forms.TextBox commsLatencyTextBox;
        private System.Windows.Forms.Label label16;
        internal System.Windows.Forms.TextBox clientInfoTextBox;
        private System.Windows.Forms.Label label17;
        internal System.Windows.Forms.TextBox statusTextBox;
        internal StatusIndicator commsLatencyStatusIndicator;
        internal StatusIndicator databaseLatencyStatusIndicator;
        internal StatusIndicator recentMsgStatusIndicator;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Button btnOffsets;
        private Label label19;
    }
}