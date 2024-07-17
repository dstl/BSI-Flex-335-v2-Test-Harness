// Project:           $Project: 00010855-Sapient$
// File:              $Workfile: LocationOffsetForm.Designer.cs$
// Crown-owned copyright, 2021-2024
//  See Release/Supply Conditions
// $NoKeywords$

namespace SapientMiddleware
{
    public partial class LocationOffsetForm
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
            this.btnUpdate = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.xOffset = new System.Windows.Forms.TextBox();
            this.yOffset = new System.Windows.Forms.TextBox();
            this.zOffset = new System.Windows.Forms.TextBox();
            this.azOffset = new System.Windows.Forms.TextBox();
            this.eleOffset = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.cboxSensorID = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnUpdate
            // 
            this.btnUpdate.Location = new System.Drawing.Point(105, 131);
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.Size = new System.Drawing.Size(75, 23);
            this.btnUpdate.TabIndex = 0;
            this.btnUpdate.Text = "Update";
            this.btnUpdate.UseVisualStyleBackColor = true;
            this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(186, 131);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(223, 45);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(45, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "X Offset";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(223, 73);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(45, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Y Offset";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(223, 99);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(45, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Z Offset";
            // 
            // xOffset
            // 
            this.xOffset.Location = new System.Drawing.Point(274, 41);
            this.xOffset.Name = "xOffset";
            this.xOffset.Size = new System.Drawing.Size(100, 20);
            this.xOffset.TabIndex = 5;
            // 
            // yOffset
            // 
            this.yOffset.Location = new System.Drawing.Point(274, 69);
            this.yOffset.Name = "yOffset";
            this.yOffset.Size = new System.Drawing.Size(100, 20);
            this.yOffset.TabIndex = 6;
            // 
            // zOffset
            // 
            this.zOffset.Location = new System.Drawing.Point(274, 95);
            this.zOffset.Name = "zOffset";
            this.zOffset.Size = new System.Drawing.Size(100, 20);
            this.zOffset.TabIndex = 7;
            // 
            // azOffset
            // 
            this.azOffset.Location = new System.Drawing.Point(70, 69);
            this.azOffset.Name = "azOffset";
            this.azOffset.Size = new System.Drawing.Size(100, 20);
            this.azOffset.TabIndex = 8;
            // 
            // eleOffset
            // 
            this.eleOffset.Location = new System.Drawing.Point(70, 95);
            this.eleOffset.Name = "eleOffset";
            this.eleOffset.Size = new System.Drawing.Size(100, 20);
            this.eleOffset.TabIndex = 9;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(14, 73);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(50, 13);
            this.label4.TabIndex = 10;
            this.label4.Text = "Az Offset";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(11, 99);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(53, 13);
            this.label5.TabIndex = 11;
            this.label5.Text = "Ele Offset";
            // 
            // cboxSensorID
            // 
            this.cboxSensorID.FormattingEnabled = true;
            this.cboxSensorID.Location = new System.Drawing.Point(70, 41);
            this.cboxSensorID.Name = "cboxSensorID";
            this.cboxSensorID.Size = new System.Drawing.Size(121, 21);
            this.cboxSensorID.TabIndex = 12;
            this.cboxSensorID.SelectedIndexChanged += new System.EventHandler(this.cboxSensorID_SelectedIndexChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(10, 45);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(54, 13);
            this.label6.TabIndex = 13;
            this.label6.Text = "Sensor ID";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(13, 4);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(362, 13);
            this.label7.TabIndex = 14;
            this.label7.Text = "Select a Sensor ID to modify the offset or enter a new ID to create an offset";
            // 
            // LocationOffsetForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(394, 156);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.cboxSensorID);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.eleOffset);
            this.Controls.Add(this.azOffset);
            this.Controls.Add(this.zOffset);
            this.Controls.Add(this.yOffset);
            this.Controls.Add(this.xOffset);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnUpdate);
            this.Name = "LocationOffsetForm";
            this.Text = "LocationOffsetForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnUpdate;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox xOffset;
        private System.Windows.Forms.TextBox yOffset;
        private System.Windows.Forms.TextBox zOffset;
        private System.Windows.Forms.TextBox azOffset;
        private System.Windows.Forms.TextBox eleOffset;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox cboxSensorID;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
    }
}