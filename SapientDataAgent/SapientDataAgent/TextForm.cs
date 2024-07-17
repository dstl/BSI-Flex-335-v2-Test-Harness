// Project:           $Project: 00010855-Sapient$
// File:              $Workfile: TextForm.cs$
// Crown-owned copyright, 2021-2024
//  See Release/Supply Conditions
// $NoKeywords$

namespace SapientMiddleware
{
    using System;
    using System.Windows.Forms;

    /// <summary>
    /// Class for form displaying text information.
    /// </summary>
    public partial class TextForm : Form
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TextForm" /> class.
        /// </summary>
        public TextForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Delegate for AddText method.
        /// </summary>
        /// <param name="text">text string to add to form.</param>
        private delegate void AddTextDelegate(string text);

        /// <summary>
        /// Set form to show only specified text.
        /// </summary>
        /// <param name="text">text string.</param>
        public void SetText(string text)
        {
            this.textBox1.Text = text;
        }

        /// <summary>
        /// Add text to form.
        /// </summary>
        /// <param name="text">text string to add to form.</param>
        public void AddText(string text)
        {
            if (this.InvokeRequired)
            {
                // Create a delegate to self
                AddTextDelegate doAddText = new AddTextDelegate(this.AddText);

                // "Recurse once, onto another thread"
                this.Invoke(doAddText, new object[] { text });
            }
            else
            {
                if (this.textBox1.Text != string.Empty)
                {
                    this.textBox1.AppendText(Environment.NewLine);
                }

                this.textBox1.AppendText(text);
            }
        }

        /// <summary>
        /// Clearr all text from form.
        /// </summary>
        public void Clear()
        {
            this.textBox1.Clear();
        }
    }
}
