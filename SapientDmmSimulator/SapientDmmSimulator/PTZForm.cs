// Project:           $Project: 00010855-Sapient$
// File:              $Workfile: PTZForm.cs$
// Crown-owned copyright, 2021-2024
//  See Release/Supply Conditions

namespace SapientDmmSimulator
{
    using System;
    using System.Windows.Forms;

    /// <summary>
    /// form for entry of Pan/Tilt/Zoom or Azimuth/Range/Zoom command parameters.
    /// </summary>
    public partial class PTZForm : Form
    {
        public static double Az;
        public static double Elevation;
        public static double Zoom;
        public static bool PTZ;

        private static readonly Random random = new Random();

        /// <summary>
        /// Initializes a new instance of the <see cref="PTZForm"/> class.
        /// constructor for PTZ Form.
        /// </summary>
        public PTZForm(bool showPTZcontrols)
        {
            InitializeComponent();
            Azimuth_txt.Text = Az.ToString("F2");
            Elevation_txt.Text = Elevation.ToString("F2");
            Zoom_txt.Text = Zoom.ToString("F2");
            SendAsPTZ.Checked = PTZ;
            if (showPTZcontrols == false)
            {
                Azimuth_txt.Hide();
                Elevation_txt.Hide();
                Zoom_txt.Hide();
                SendAsPTZ.Hide();
                button2.Hide();
                label1.Hide();
                label2.Hide();
                label4.Hide();
                //command.Items.RemoveAt(0); // remove lookAt
            }
            else
            {
                command.Items.Clear();
                command.Items.Add("lookAt");
            }
        }

        /// <summary>
        /// method to randomize azimuth and range.
        /// </summary>
        public static void Randomize()
        {
            Az = (random.NextDouble() * 360.0);
            Zoom = (6.0 + random.NextDouble() * (100.0 - 6.0));
        }

        /// <summary>
        /// click event on Randomize button.
        /// </summary>
        /// <param name="sender">sender button.</param>
        /// <param name="e">click event.</param>
        private void RandomizeClick(object sender, EventArgs e)
        {
            Randomize();
            Azimuth_txt.Text = Az.ToString("F2");
            Elevation_txt.Text = Elevation.ToString("F2");
        }

        /// <summary>
        /// click event on OK button.
        /// </summary>
        /// <param name="sender">OK button.</param>
        /// <param name="e">click event.</param>
        private void OnOk(object sender, EventArgs e)
        {
            Az = double.Parse(Azimuth_txt.Text);
            Elevation = double.Parse(Elevation_txt.Text);
            Zoom = double.Parse(Zoom_txt.Text);
            PTZ = SendAsPTZ.Checked;
        }
    }
}
