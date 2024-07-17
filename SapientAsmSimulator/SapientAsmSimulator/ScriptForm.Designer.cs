
// File:              $Workfile: ScriptForm.Designer.cs$
// <copyright file="ScriptForm.Designer.cs" >
// Crown-owned copyright, 2021-2024
// See Release/Supply Conditions
// </copyright>

namespace ScriptReader
{
    partial class ScriptForm
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
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.NextButton = new System.Windows.Forms.Button();
            this.outputWindow = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.SendList = new System.Windows.Forms.Button();
            this.LoadList = new System.Windows.Forms.Button();
            this.SendAll = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.ItemHeight = 20;
            this.listBox1.Location = new System.Drawing.Point(16, 63);
            this.listBox1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(744, 404);
            this.listBox1.TabIndex = 0;
            // 
            // NextButton
            // 
            this.NextButton.Location = new System.Drawing.Point(203, 18);
            this.NextButton.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.NextButton.Name = "NextButton";
            this.NextButton.Size = new System.Drawing.Size(100, 35);
            this.NextButton.TabIndex = 1;
            this.NextButton.Text = "Send";
            this.NextButton.UseVisualStyleBackColor = true;
            this.NextButton.Click += new System.EventHandler(this.NextButton_Click);
            // 
            // outputWindow
            // 
            this.outputWindow.Location = new System.Drawing.Point(16, 505);
            this.outputWindow.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.outputWindow.Multiline = true;
            this.outputWindow.Name = "outputWindow";
            this.outputWindow.ReadOnly = true;
            this.outputWindow.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.outputWindow.Size = new System.Drawing.Size(744, 484);
            this.outputWindow.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 38);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(77, 20);
            this.label1.TabIndex = 3;
            this.label1.Text = "JSON Files";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(16, 475);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(55, 20);
            this.label2.TabIndex = 4;
            this.label2.Text = "Output";
            // 
            // SendList
            // 
            this.SendList.Location = new System.Drawing.Point(317, 18);
            this.SendList.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.SendList.Name = "SendList";
            this.SendList.Size = new System.Drawing.Size(129, 35);
            this.SendList.TabIndex = 5;
            this.SendList.Text = "Send List";
            this.SendList.UseVisualStyleBackColor = true;
            this.SendList.Click += new System.EventHandler(this.SendList_Click);
            // 
            // LoadList
            // 
            this.LoadList.Location = new System.Drawing.Point(95, 18);
            this.LoadList.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.LoadList.Name = "LoadList";
            this.LoadList.Size = new System.Drawing.Size(100, 35);
            this.LoadList.TabIndex = 6;
            this.LoadList.Text = "Load List";
            this.LoadList.UseVisualStyleBackColor = true;
            this.LoadList.Click += new System.EventHandler(this.LoadList_Click);
            // 
            // SendAll
            // 
            this.SendAll.Location = new System.Drawing.Point(456, 17);
            this.SendAll.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.SendAll.Name = "SendAll";
            this.SendAll.Size = new System.Drawing.Size(115, 35);
            this.SendAll.TabIndex = 7;
            this.SendAll.Text = "Send All";
            this.SendAll.UseVisualStyleBackColor = true;
            this.SendAll.Click += new System.EventHandler(this.SendAll_Click);
            // 
            // ScriptForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(777, 997);
            this.Controls.Add(this.SendAll);
            this.Controls.Add(this.LoadList);
            this.Controls.Add(this.SendList);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.outputWindow);
            this.Controls.Add(this.NextButton);
            this.Controls.Add(this.listBox1);
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "ScriptForm";
            this.Text = "Read File List Script";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.Button NextButton;
        private System.Windows.Forms.TextBox outputWindow;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button SendList;
        private System.Windows.Forms.Button LoadList;
        private System.Windows.Forms.Button SendAll;
    }
}