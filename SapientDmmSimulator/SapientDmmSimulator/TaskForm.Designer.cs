// Project:           $Project: 00010855-Sapient$
// File:              $Workfile: TaskForm.Designer.cs$
// Crown-owned copyright, 2021-2024
//  See Release/Supply Conditions

namespace SapientDmmSimulator
{
    partial class TaskForm
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
            Send_task = new Button();
            Output_box = new TextBox();
            label1 = new Label();
            Sensor_input = new TextBox();
            Read_file = new Button();
            open_file_dialog = new OpenFileDialog();
            clear = new Button();
            openLogFile = new OpenFileDialog();
            send_detection = new Button();
            sendPTZTask = new Button();
            alertResponse = new Button();
            alertID = new TextBox();
            label2 = new Label();
            DMMHeartbeatButton = new Button();
            DMMAlertButton = new Button();
            fileScriptButton = new Button();
            loopTasksCheckBox = new CheckBox();
            loopDetectionCheckBox = new CheckBox();
            filenameTextBox = new TextBox();
            label6 = new Label();
            sendToDMMcheckBox = new CheckBox();
            label19 = new Label();
            SuspendLayout();
            // 
            // Send_task
            // 
            Send_task.Location = new Point(8, 64);
            Send_task.Margin = new Padding(4);
            Send_task.Name = "Send_task";
            Send_task.Size = new Size(123, 28);
            Send_task.TabIndex = 0;
            Send_task.Text = "Send Task";
            Send_task.UseVisualStyleBackColor = true;
            Send_task.Click += SendTaskClick;
            // 
            // Output_box
            // 
            Output_box.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            Output_box.Location = new Point(144, 40);
            Output_box.Margin = new Padding(4);
            Output_box.Multiline = true;
            Output_box.Name = "Output_box";
            Output_box.ScrollBars = ScrollBars.Both;
            Output_box.Size = new Size(426, 632);
            Output_box.TabIndex = 1;
            Output_box.WordWrap = false;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(10, 14);
            label1.Margin = new Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new Size(56, 15);
            label1.TabIndex = 2;
            label1.Text = "SensorID:";
            // 
            // Sensor_input
            // 
            Sensor_input.Location = new Point(80, 10);
            Sensor_input.Margin = new Padding(4);
            Sensor_input.Name = "Sensor_input";
            Sensor_input.Size = new Size(490, 23);
            Sensor_input.TabIndex = 3;
            // 
            // Read_file
            // 
            Read_file.Location = new Point(8, 457);
            Read_file.Margin = new Padding(4);
            Read_file.Name = "Read_file";
            Read_file.Size = new Size(123, 41);
            Read_file.TabIndex = 9;
            Read_file.Text = "Send File";
            Read_file.UseVisualStyleBackColor = true;
            Read_file.Click += ReadFileClick;
            // 
            // open_file_dialog
            // 
            open_file_dialog.FileName = "openFileDialog1";
            // 
            // clear
            // 
            clear.Location = new Point(8, 562);
            clear.Margin = new Padding(4);
            clear.Name = "clear";
            clear.Size = new Size(123, 41);
            clear.TabIndex = 11;
            clear.Text = "Clear";
            clear.UseVisualStyleBackColor = true;
            clear.Click += ClearClick;
            // 
            // openLogFile
            // 
            openLogFile.FileName = "*.log";
            // 
            // send_detection
            // 
            send_detection.Location = new Point(8, 276);
            send_detection.Margin = new Padding(4);
            send_detection.Name = "send_detection";
            send_detection.Size = new Size(123, 44);
            send_detection.TabIndex = 12;
            send_detection.Text = "Send Detection";
            send_detection.UseVisualStyleBackColor = true;
            send_detection.Click += SendDetectionClick;
            // 
            // sendPTZTask
            // 
            sendPTZTask.Location = new Point(8, 119);
            sendPTZTask.Margin = new Padding(4);
            sendPTZTask.Name = "sendPTZTask";
            sendPTZTask.Size = new Size(123, 44);
            sendPTZTask.TabIndex = 13;
            sendPTZTask.Text = "Send PTZ Task";
            sendPTZTask.UseVisualStyleBackColor = true;
            sendPTZTask.Visible = false;
            sendPTZTask.Click += SendPTZTaskClick;
            // 
            // alertResponse
            // 
            alertResponse.Location = new Point(8, 225);
            alertResponse.Margin = new Padding(4);
            alertResponse.Name = "alertResponse";
            alertResponse.Size = new Size(123, 44);
            alertResponse.TabIndex = 14;
            alertResponse.Text = "Send Alert Response";
            alertResponse.UseVisualStyleBackColor = true;
            alertResponse.Click += AlertResponseClick;
            // 
            // alertID
            // 
            alertID.Location = new Point(10, 195);
            alertID.Margin = new Padding(4);
            alertID.Name = "alertID";
            alertID.Size = new Size(122, 23);
            alertID.TabIndex = 16;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(8, 178);
            label2.Margin = new Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new Size(46, 15);
            label2.TabIndex = 15;
            label2.Text = "Alert ID";
            // 
            // DMMHeartbeatButton
            // 
            DMMHeartbeatButton.Location = new Point(8, 405);
            DMMHeartbeatButton.Margin = new Padding(4);
            DMMHeartbeatButton.Name = "DMMHeartbeatButton";
            DMMHeartbeatButton.Size = new Size(123, 44);
            DMMHeartbeatButton.TabIndex = 17;
            DMMHeartbeatButton.Text = "Send DMM Status Report";
            DMMHeartbeatButton.UseVisualStyleBackColor = true;
            DMMHeartbeatButton.Click += HLHeartbeatButton_Click;
            // 
            // DMMAlertButton
            // 
            DMMAlertButton.Location = new Point(8, 353);
            DMMAlertButton.Margin = new Padding(4);
            DMMAlertButton.Name = "DMMAlertButton";
            DMMAlertButton.Size = new Size(123, 41);
            DMMAlertButton.TabIndex = 18;
            DMMAlertButton.Text = "Send DMM Alert";
            DMMAlertButton.UseVisualStyleBackColor = true;
            DMMAlertButton.Click += HLAlertButton_Click;
            // 
            // fileScriptButton
            // 
            fileScriptButton.Location = new Point(8, 508);
            fileScriptButton.Margin = new Padding(4);
            fileScriptButton.Name = "fileScriptButton";
            fileScriptButton.Size = new Size(123, 41);
            fileScriptButton.TabIndex = 19;
            fileScriptButton.Text = "Send List of Files";
            fileScriptButton.UseVisualStyleBackColor = true;
            fileScriptButton.Click += fileScriptButton_Click;
            // 
            // loopTasksCheckBox
            // 
            loopTasksCheckBox.AutoSize = true;
            loopTasksCheckBox.Location = new Point(14, 99);
            loopTasksCheckBox.Margin = new Padding(4);
            loopTasksCheckBox.Name = "loopTasksCheckBox";
            loopTasksCheckBox.Size = new Size(84, 19);
            loopTasksCheckBox.TabIndex = 22;
            loopTasksCheckBox.Text = "Loop Tasks";
            loopTasksCheckBox.UseVisualStyleBackColor = true;
            loopTasksCheckBox.CheckedChanged += loopTasksCheckBox_CheckedChanged;
            // 
            // loopDetectionCheckBox
            // 
            loopDetectionCheckBox.AutoSize = true;
            loopDetectionCheckBox.Location = new Point(14, 326);
            loopDetectionCheckBox.Margin = new Padding(4);
            loopDetectionCheckBox.Name = "loopDetectionCheckBox";
            loopDetectionCheckBox.Size = new Size(112, 19);
            loopDetectionCheckBox.TabIndex = 22;
            loopDetectionCheckBox.Text = "Loop Detections";
            loopDetectionCheckBox.UseVisualStyleBackColor = true;
            loopDetectionCheckBox.CheckedChanged += loopDetectionCheckBox_CheckedChanged;
            // 
            // filenameTextBox
            // 
            filenameTextBox.Location = new Point(8, 651);
            filenameTextBox.Margin = new Padding(4);
            filenameTextBox.Name = "filenameTextBox";
            filenameTextBox.Size = new Size(124, 23);
            filenameTextBox.TabIndex = 28;
            filenameTextBox.Text = "testfile.jpg";
            filenameTextBox.TextChanged += filenameTextBox_TextChanged;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(4, 633);
            label6.Margin = new Padding(4, 0, 4, 0);
            label6.Name = "label6";
            label6.Size = new Size(118, 15);
            label6.TabIndex = 29;
            label6.Text = "Associated Filename:";
            // 
            // sendToDMMcheckBox
            // 
            sendToDMMcheckBox.AutoSize = true;
            sendToDMMcheckBox.Location = new Point(14, 40);
            sendToDMMcheckBox.Margin = new Padding(4);
            sendToDMMcheckBox.Name = "sendToDMMcheckBox";
            sendToDMMcheckBox.Size = new Size(101, 19);
            sendToDMMcheckBox.TabIndex = 30;
            sendToDMMcheckBox.Text = "Send To DMM";
            sendToDMMcheckBox.UseVisualStyleBackColor = true;
            sendToDMMcheckBox.CheckedChanged += sendToDMMcheckBox_CheckedChanged;
            // 
            // label19
            // 
            label19.AutoSize = true;
            label19.Font = new Font("Segoe UI", 7F, FontStyle.Regular, GraphicsUnit.Point);
            label19.ForeColor = SystemColors.ButtonShadow;
            label19.Location = new Point(4, 682);
            label19.Name = "label19";
            label19.Size = new Size(182, 12);
            label19.TabIndex = 59;
            label19.Text = "Developed by Operational Solutions Ltd.";
            // 
            // TaskForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(582, 700);
            Controls.Add(label19);
            Controls.Add(sendToDMMcheckBox);
            Controls.Add(label6);
            Controls.Add(filenameTextBox);
            Controls.Add(loopDetectionCheckBox);
            Controls.Add(loopTasksCheckBox);
            Controls.Add(fileScriptButton);
            Controls.Add(DMMAlertButton);
            Controls.Add(DMMHeartbeatButton);
            Controls.Add(alertID);
            Controls.Add(label2);
            Controls.Add(alertResponse);
            Controls.Add(sendPTZTask);
            Controls.Add(send_detection);
            Controls.Add(clear);
            Controls.Add(Read_file);
            Controls.Add(Sensor_input);
            Controls.Add(label1);
            Controls.Add(Output_box);
            Controls.Add(Send_task);
            Margin = new Padding(4);
            Name = "TaskForm";
            Text = "DMM Simulator";
            FormClosed += TaskFormFormClosed;
            Load += TaskFormLoad;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button Send_task;
        private TextBox Output_box;
        private Label label1;
        private TextBox Sensor_input;
        private Button Read_file;
        private OpenFileDialog open_file_dialog;
        private Button clear;
        private OpenFileDialog openLogFile;
        private Button send_detection;
        private Button sendPTZTask;
        private Button alertResponse;
        private TextBox alertID;
        private Label label2;
        private Button DMMHeartbeatButton;
        private Button DMMAlertButton;
        private Button fileScriptButton;
        private CheckBox loopTasksCheckBox;
        private CheckBox loopDetectionCheckBox;
        public TextBox filenameTextBox;
        private Label label6;
        private CheckBox sendToDMMcheckBox;
        private Label label19;
    }
}