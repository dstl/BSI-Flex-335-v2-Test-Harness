
// File:              $Workfile: ClientForm.Designer.cs$
// <copyright file="ClientForm.Designer.cs" >
// Crown-owned copyright, 2021-2024
// See Release/Supply Conditions
// </copyright>

namespace SapientASMsimulator
{
    /// <summary>
    /// class to display the ASM Client Form
    /// </summary>
    partial class ClientForm
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
            components = new System.ComponentModel.Container();
            Send_Reg = new Button();
            Send_heart = new Button();
            Send_dec = new Button();
            OutputWindow = new TextBox();
            Read_file = new Button();
            LoopDetection = new CheckBox();
            label1 = new Label();
            HeartbeatTime = new TextBox();
            Heartbeat = new CheckBox();
            openFile = new OpenFileDialog();
            ClearButton = new Button();
            openLogFile = new OpenFileDialog();
            DetectionTime = new TextBox();
            label2 = new Label();
            ASMText = new TextBox();
            label3 = new Label();
            sendAlert = new Button();
            GUITimer = new System.Windows.Forms.Timer(components);
            detectionCountTextBox = new TextBox();
            detectionsSentLabel = new Label();
            heartbeatCountTextBox = new TextBox();
            label4 = new Label();
            LoopAlerts = new CheckBox();
            label5 = new Label();
            alertCountTextBox = new TextBox();
            fileScriptButton = new Button();
            filenameTextBox = new TextBox();
            label6 = new Label();
            label19 = new Label();
            SuspendLayout();
            // 
            // Send_Reg
            // 
            Send_Reg.Location = new Point(13, 42);
            Send_Reg.Margin = new Padding(4);
            Send_Reg.Name = "Send_Reg";
            Send_Reg.Size = new Size(116, 41);
            Send_Reg.TabIndex = 0;
            Send_Reg.Text = "Send Registration";
            Send_Reg.UseVisualStyleBackColor = true;
            Send_Reg.Click += SendRegistrationClick;
            // 
            // Send_heart
            // 
            Send_heart.Location = new Point(14, 91);
            Send_heart.Margin = new Padding(4);
            Send_heart.Name = "Send_heart";
            Send_heart.Size = new Size(116, 41);
            Send_heart.TabIndex = 1;
            Send_heart.Text = "Send Status";
            Send_heart.UseVisualStyleBackColor = true;
            Send_heart.Click += SendHeartbeatClick;
            // 
            // Send_dec
            // 
            Send_dec.Location = new Point(14, 139);
            Send_dec.Margin = new Padding(4);
            Send_dec.Name = "Send_dec";
            Send_dec.Size = new Size(116, 41);
            Send_dec.TabIndex = 2;
            Send_dec.Text = "Send Detection";
            Send_dec.UseVisualStyleBackColor = true;
            Send_dec.Click += SendDetectionClick;
            // 
            // OutputWindow
            // 
            OutputWindow.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            OutputWindow.Location = new Point(137, 42);
            OutputWindow.Margin = new Padding(4);
            OutputWindow.Multiline = true;
            OutputWindow.Name = "OutputWindow";
            OutputWindow.ScrollBars = ScrollBars.Both;
            OutputWindow.Size = new Size(372, 334);
            OutputWindow.TabIndex = 4;
            OutputWindow.WordWrap = false;
            // 
            // Read_file
            // 
            Read_file.Location = new Point(13, 236);
            Read_file.Margin = new Padding(4);
            Read_file.Name = "Read_file";
            Read_file.Size = new Size(116, 41);
            Read_file.TabIndex = 7;
            Read_file.Text = "Send File";
            Read_file.UseVisualStyleBackColor = true;
            Read_file.Click += ReadFileClick;
            // 
            // LoopDetection
            // 
            LoopDetection.AutoSize = true;
            LoopDetection.Location = new Point(13, 431);
            LoopDetection.Margin = new Padding(4);
            LoopDetection.Name = "LoopDetection";
            LoopDetection.Size = new Size(112, 19);
            LoopDetection.TabIndex = 8;
            LoopDetection.Text = "Loop Detections";
            LoopDetection.UseVisualStyleBackColor = true;
            LoopDetection.CheckedChanged += LoopDetectionCheckedChanged;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(207, 392);
            label1.Margin = new Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new Size(62, 15);
            label1.TabIndex = 9;
            label1.Text = "Interval (s)";
            // 
            // HeartbeatTime
            // 
            HeartbeatTime.Location = new Point(154, 388);
            HeartbeatTime.Margin = new Padding(4);
            HeartbeatTime.Name = "HeartbeatTime";
            HeartbeatTime.Size = new Size(49, 23);
            HeartbeatTime.TabIndex = 10;
            HeartbeatTime.Text = "5";
            HeartbeatTime.TextChanged += HeartbeatTime_TextChanged;
            // 
            // Heartbeat
            // 
            Heartbeat.AutoSize = true;
            Heartbeat.Location = new Point(13, 388);
            Heartbeat.Margin = new Padding(4);
            Heartbeat.Name = "Heartbeat";
            Heartbeat.Size = new Size(123, 19);
            Heartbeat.TabIndex = 11;
            Heartbeat.Text = "Loop StatusReport";
            Heartbeat.UseVisualStyleBackColor = true;
            Heartbeat.CheckedChanged += LoopHeartbeatCheckedChanged;
            // 
            // openFile
            // 
            openFile.FileName = "openFileDialog1";
            // 
            // ClearButton
            // 
            ClearButton.Location = new Point(13, 334);
            ClearButton.Margin = new Padding(4);
            ClearButton.Name = "ClearButton";
            ClearButton.Size = new Size(116, 41);
            ClearButton.TabIndex = 13;
            ClearButton.Text = "Clear";
            ClearButton.UseVisualStyleBackColor = true;
            ClearButton.Click += ClearClick;
            // 
            // openLogFile
            // 
            openLogFile.FileName = "openFileDialog2";
            // 
            // DetectionTime
            // 
            DetectionTime.Location = new Point(154, 429);
            DetectionTime.Margin = new Padding(4);
            DetectionTime.Name = "DetectionTime";
            DetectionTime.Size = new Size(49, 23);
            DetectionTime.TabIndex = 14;
            DetectionTime.Text = "100";
            DetectionTime.TextChanged += DetectionTime_TextChanged;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(207, 430);
            label2.Margin = new Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new Size(76, 15);
            label2.TabIndex = 15;
            label2.Text = "Interval  (ms)";
            // 
            // ASMText
            // 
            ASMText.Location = new Point(78, 10);
            ASMText.Margin = new Padding(4);
            ASMText.Name = "ASMText";
            ASMText.Size = new Size(431, 23);
            ASMText.TabIndex = 17;
            ASMText.TextChanged += ASMText_TextChanged;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(15, 14);
            label3.Margin = new Padding(4, 0, 4, 0);
            label3.Name = "label3";
            label3.Size = new Size(49, 15);
            label3.TabIndex = 16;
            label3.Text = "ASM ID:";
            // 
            // sendAlert
            // 
            sendAlert.Location = new Point(13, 188);
            sendAlert.Margin = new Padding(4);
            sendAlert.Name = "sendAlert";
            sendAlert.Size = new Size(116, 41);
            sendAlert.TabIndex = 18;
            sendAlert.Text = "Send Alert";
            sendAlert.UseVisualStyleBackColor = true;
            sendAlert.Click += SendAlertClick;
            // 
            // GUITimer
            // 
            GUITimer.Enabled = true;
            GUITimer.Tick += GUITimer_Tick;
            // 
            // detectionCountTextBox
            // 
            detectionCountTextBox.Location = new Point(400, 427);
            detectionCountTextBox.Margin = new Padding(4);
            detectionCountTextBox.Name = "detectionCountTextBox";
            detectionCountTextBox.ReadOnly = true;
            detectionCountTextBox.Size = new Size(116, 23);
            detectionCountTextBox.TabIndex = 18;
            // 
            // detectionsSentLabel
            // 
            detectionsSentLabel.AutoSize = true;
            detectionsSentLabel.Location = new Point(293, 433);
            detectionsSentLabel.Margin = new Padding(4, 0, 4, 0);
            detectionsSentLabel.Name = "detectionsSentLabel";
            detectionsSentLabel.Size = new Size(92, 15);
            detectionsSentLabel.TabIndex = 19;
            detectionsSentLabel.Text = "Detections Sent:";
            // 
            // heartbeatCountTextBox
            // 
            heartbeatCountTextBox.Location = new Point(400, 392);
            heartbeatCountTextBox.Margin = new Padding(4);
            heartbeatCountTextBox.Name = "heartbeatCountTextBox";
            heartbeatCountTextBox.ReadOnly = true;
            heartbeatCountTextBox.Size = new Size(116, 23);
            heartbeatCountTextBox.TabIndex = 20;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(293, 395);
            label4.Margin = new Padding(4, 0, 4, 0);
            label4.Name = "label4";
            label4.Size = new Size(93, 15);
            label4.TabIndex = 21;
            label4.Text = "Heartbeats Sent:";
            // 
            // LoopAlerts
            // 
            LoopAlerts.AutoSize = true;
            LoopAlerts.Location = new Point(13, 467);
            LoopAlerts.Margin = new Padding(4);
            LoopAlerts.Name = "LoopAlerts";
            LoopAlerts.Size = new Size(86, 19);
            LoopAlerts.TabIndex = 22;
            LoopAlerts.Text = "Loop Alerts";
            LoopAlerts.UseVisualStyleBackColor = true;
            LoopAlerts.CheckedChanged += LoopAlerts_CheckedChanged;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(293, 470);
            label5.Margin = new Padding(4, 0, 4, 0);
            label5.Name = "label5";
            label5.Size = new Size(66, 15);
            label5.TabIndex = 24;
            label5.Text = "Alerts Sent:";
            // 
            // alertCountTextBox
            // 
            alertCountTextBox.Location = new Point(400, 464);
            alertCountTextBox.Margin = new Padding(4);
            alertCountTextBox.Name = "alertCountTextBox";
            alertCountTextBox.ReadOnly = true;
            alertCountTextBox.Size = new Size(116, 23);
            alertCountTextBox.TabIndex = 23;
            // 
            // fileScriptButton
            // 
            fileScriptButton.Location = new Point(13, 284);
            fileScriptButton.Margin = new Padding(4);
            fileScriptButton.Name = "fileScriptButton";
            fileScriptButton.Size = new Size(116, 41);
            fileScriptButton.TabIndex = 25;
            fileScriptButton.Text = "Send List of Files";
            fileScriptButton.UseVisualStyleBackColor = true;
            fileScriptButton.Click += fileScriptButton_Click;
            // 
            // filenameTextBox
            // 
            filenameTextBox.Location = new Point(343, 494);
            filenameTextBox.Margin = new Padding(4);
            filenameTextBox.Name = "filenameTextBox";
            filenameTextBox.Size = new Size(173, 23);
            filenameTextBox.TabIndex = 27;
            filenameTextBox.Text = "testfile.jpg";
            filenameTextBox.TextChanged += filenameTextBox_TextChanged;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(211, 494);
            label6.Margin = new Padding(4, 0, 4, 0);
            label6.Name = "label6";
            label6.Size = new Size(118, 15);
            label6.TabIndex = 28;
            label6.Text = "Associated Filename:";
            // 
            // label19
            // 
            label19.AutoSize = true;
            label19.Font = new Font("Segoe UI", 7F, FontStyle.Regular, GraphicsUnit.Point);
            label19.ForeColor = SystemColors.ButtonShadow;
            label19.Location = new Point(11, 523);
            label19.Name = "label19";
            label19.Size = new Size(182, 12);
            label19.TabIndex = 59;
            label19.Text = "Developed by Operational Solutions Ltd.";
            // 
            // ClientForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(524, 539);
            Controls.Add(label19);
            Controls.Add(label6);
            Controls.Add(filenameTextBox);
            Controls.Add(fileScriptButton);
            Controls.Add(label5);
            Controls.Add(alertCountTextBox);
            Controls.Add(LoopAlerts);
            Controls.Add(label4);
            Controls.Add(heartbeatCountTextBox);
            Controls.Add(detectionsSentLabel);
            Controls.Add(detectionCountTextBox);
            Controls.Add(sendAlert);
            Controls.Add(ASMText);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(DetectionTime);
            Controls.Add(ClearButton);
            Controls.Add(Heartbeat);
            Controls.Add(HeartbeatTime);
            Controls.Add(label1);
            Controls.Add(LoopDetection);
            Controls.Add(Read_file);
            Controls.Add(OutputWindow);
            Controls.Add(Send_dec);
            Controls.Add(Send_heart);
            Controls.Add(Send_Reg);
            Margin = new Padding(4);
            Name = "ClientForm";
            Text = "Sapient ASM Simulator";
            FormClosed += ClientForm_FormClosed;
            Load += Form1Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button Send_Reg;
        private Button Send_heart;
        private Button Send_dec;
        private TextBox OutputWindow;
        private Button Read_file;
        private CheckBox LoopDetection;
        private Label label1;
        private TextBox HeartbeatTime;
        private CheckBox Heartbeat;
        private OpenFileDialog openFile;
        private Button ClearButton;
        private OpenFileDialog openLogFile;
        private Label label2;
        public TextBox DetectionTime;
        public TextBox ASMText;
        private Label label3;
        private Button sendAlert;
        private System.Windows.Forms.Timer GUITimer;
        private TextBox detectionCountTextBox;
        private Label detectionsSentLabel;
        private TextBox heartbeatCountTextBox;
        private Label label4;
        private CheckBox LoopAlerts;
        private Label label5;
        private TextBox alertCountTextBox;
        private Button fileScriptButton;
        public TextBox filenameTextBox;
        private Label label6;
        private Label label19;
    }
}