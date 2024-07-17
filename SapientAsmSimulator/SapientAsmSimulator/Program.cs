// File:              $Workfile: Program.cs$
// <copyright file="Program.cs" >
// Crown-owned copyright, 2021-2024
// See Release/Supply Conditions
// </copyright>

[assembly: log4net.Config.XmlConfigurator(Watch = true)]

namespace SapientASMsimulator
{
    using log4net;

    /// <summary>
    /// Main Program Class
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// Log4net logger
        /// </summary>
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// <param name="args">command line argument - optionally specify fixed sensor identifier</param>
        [STAThread]
        public static void Main(string[] args)
        {
            const int ExeName = 0;
            const int ExeVersion = 2;

            // Output the assembly name and version number for configuration purposes
            string[] assemblyDetails = System.Reflection.Assembly.GetExecutingAssembly().FullName.Split(',', '=');

            Log.Info(Environment.NewLine);
            Log.Info(assemblyDetails[ExeName] + " - Version " + assemblyDetails[ExeVersion]);

            ASMMainProcess.AsmId = Properties.Settings.Default.FixedASMId;

            if (args.Length > 0)
            {
                ASMMainProcess.PortId = args[0];
            }
            if (args.Length > 1)
            {
                ASMMainProcess.AsmId = args[1];
            }
            if (string.IsNullOrEmpty(ASMMainProcess.AsmId))
            {
                ASMMainProcess.AsmId = Ulid.NewUlid().ToString();
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new ClientForm());
        }
    }
}