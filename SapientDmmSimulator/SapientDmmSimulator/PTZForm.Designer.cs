// Project:           $Project: 00010855-Sapient$
// File:              $Workfile: PTZForm.Designer.cs$
// Crown-owned copyright, 2021-2024
//  See Release/Supply Conditions

namespace SapientDmmSimulator
{
    partial class PTZForm
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
            label1 = new Label();
            Azimuth_txt = new TextBox();
            Elevation_txt = new TextBox();
            label2 = new Label();
            button1 = new Button();
            button2 = new Button();
            command = new ComboBox();
            label3 = new Label();
            SendAsPTZ = new CheckBox();
            Zoom_txt = new TextBox();
            label4 = new Label();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(25, 58);
            label1.Margin = new Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new Size(52, 15);
            label1.TabIndex = 0;
            label1.Text = "Azimuth";
            // 
            // Azimuth_txt
            // 
            Azimuth_txt.Location = new Point(202, 54);
            Azimuth_txt.Margin = new Padding(4, 4, 4, 4);
            Azimuth_txt.Name = "Azimuth_txt";
            Azimuth_txt.Size = new Size(116, 23);
            Azimuth_txt.TabIndex = 1;
            // 
            // Elevation_txt
            // 
            Elevation_txt.Location = new Point(202, 84);
            Elevation_txt.Margin = new Padding(4, 4, 4, 4);
            Elevation_txt.Name = "Elevation_txt";
            Elevation_txt.Size = new Size(116, 23);
            Elevation_txt.TabIndex = 3;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(25, 88);
            label2.Margin = new Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new Size(55, 15);
            label2.TabIndex = 2;
            label2.Text = "Elevation";
            // 
            // button1
            // 
            button1.DialogResult = DialogResult.OK;
            button1.Location = new Point(231, 172);
            button1.Margin = new Padding(4, 4, 4, 4);
            button1.Name = "button1";
            button1.Size = new Size(88, 26);
            button1.TabIndex = 6;
            button1.Text = "OK";
            button1.UseVisualStyleBackColor = true;
            button1.Click += OnOk;
            // 
            // button2
            // 
            button2.Location = new Point(14, 172);
            button2.Margin = new Padding(4, 4, 4, 4);
            button2.Name = "button2";
            button2.Size = new Size(88, 26);
            button2.TabIndex = 7;
            button2.Text = "Randomize";
            button2.UseVisualStyleBackColor = true;
            button2.Click += RandomizeClick;
            // 
            // command
            // 
            command.FormattingEnabled = true;
            command.Items.AddRange(new object[] { 
                "Request Registration", 
                "Request Status",
                "Request Reset", 
                "Request Stop", 
                "Request Start", 
                "DetectionThreshold Low", 
                "DetectionThreshold Medium", 
                "DetectionThreshold High", 
                "DetectionReportRate Low", 
                "DetectionReportRate Medium", 
                "DetectionReportRate High", 
                "ClassificationThreshold Low",
                "ClassificationThreshold Medium", 
                "ClassificationThreshold High", 
                "Mode", 
                "LookAt",
                "MoveTo",
                "Patrol",
                "Follow"
            });
            command.Location = new Point(88, 14);
            command.Margin = new Padding(4, 4, 4, 4);
            command.Name = "command";
            command.Size = new Size(229, 23);
            command.TabIndex = 9;
            command.Text = "lookAt";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(25, 17);
            label3.Margin = new Padding(4, 0, 4, 0);
            label3.Name = "label3";
            label3.Size = new Size(64, 15);
            label3.TabIndex = 10;
            label3.Text = "Command";
            // 
            // SendAsPTZ
            // 
            SendAsPTZ.AutoSize = true;
            SendAsPTZ.Location = new Point(214, 146);
            SendAsPTZ.Margin = new Padding(4, 4, 4, 4);
            SendAsPTZ.Name = "SendAsPTZ";
            SendAsPTZ.Size = new Size(67, 19);
            SendAsPTZ.TabIndex = 11;
            SendAsPTZ.Text = "HExtent";
            SendAsPTZ.UseVisualStyleBackColor = true;
            // 
            // Zoom_txt
            // 
            Zoom_txt.Location = new Point(202, 114);
            Zoom_txt.Margin = new Padding(4, 4, 4, 4);
            Zoom_txt.Name = "Zoom_txt";
            Zoom_txt.Size = new Size(116, 23);
            Zoom_txt.TabIndex = 12;
            Zoom_txt.Text = "0";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(25, 122);
            label4.Margin = new Padding(4, 0, 4, 0);
            label4.Name = "label4";
            label4.Size = new Size(89, 15);
            label4.TabIndex = 13;
            label4.Text = "Range/ HExtent";
            // 
            // PTZForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(332, 208);
            Controls.Add(label4);
            Controls.Add(Zoom_txt);
            Controls.Add(SendAsPTZ);
            Controls.Add(label3);
            Controls.Add(command);
            Controls.Add(button2);
            Controls.Add(button1);
            Controls.Add(Elevation_txt);
            Controls.Add(label2);
            Controls.Add(Azimuth_txt);
            Controls.Add(label1);
            Margin = new Padding(4, 4, 4, 4);
            Name = "PTZForm";
            Text = "Azimuth";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        public TextBox Azimuth_txt;
        public TextBox Elevation_txt;
        private Label label2;
        private Button button1;
        private Button button2;
        private Label label3;
        public ComboBox command;
        public TextBox Zoom_txt;
        private Label label4;
        public CheckBox SendAsPTZ;
    }
}