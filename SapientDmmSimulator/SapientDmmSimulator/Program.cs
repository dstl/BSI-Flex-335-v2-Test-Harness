// Project:           $Project: 00010855-Sapient$
// File:              $Workfile: Program.cs$
// Crown-owned copyright, 2021-2024
//  See Release/Supply Conditions

[assembly: log4net.Config.XmlConfigurator(Watch = true)]

namespace SapientDmmSimulator
{
    using log4net;

    /// <summary>
    /// The Program class.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// log4net logger.
        /// </summary>
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Defines the entry point of the application.
        /// </summary>
        [STAThread]
        public static void Main()
        {
            const int ExeName = 0;
            const int ExeVersion = 2;

            // Output the assembly name and version number for configuration purposes
            string[] assemblyDetails = System.Reflection.Assembly.GetExecutingAssembly().FullName.Split(',', '=');

            Log.Info(Environment.NewLine);
            Log.Info(assemblyDetails[ExeName] + " - Version " + assemblyDetails[ExeVersion]);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new TaskForm());
        }
    }
}
