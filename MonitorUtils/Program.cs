using log4net;
using log4net.Config;
using log4net.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitorUtils
{
    class Program
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(Program));

        private static bool debug = false;

        private static MonitorUtilsMode mode = MonitorUtilsMode.NOT_SET;



        static Program()
        {
            XmlConfigurator.Configure();
        }

        static void Main(string[] args)
        {
            try
            {
                parseArgs(args);

                if (debug)
                {
                    ((log4net.Repository.Hierarchy.Hierarchy)LogManager.GetRepository()).Root.Level = Level.Debug;
                    ((log4net.Repository.Hierarchy.Hierarchy)LogManager.GetRepository()).RaiseConfigurationChanged(EventArgs.Empty);
                }
                logger.Info("Starting MonitorUtils");

                switch (mode)
                {
                    case MonitorUtilsMode.ENABLE:
                        MonitorUtils.EnableMonitor(@"\\.\DISPLAY2", 1980, 1080, 32, 60, 1366, 0);
                        break;
                    case MonitorUtilsMode.DISABLE:
                        MonitorUtils.DisableMonitor(@"\\.\DISPLAY2");
                        break;
                    case MonitorUtilsMode.ENUMERATE:
                        MonitorUtils.EnumarateDisplayDevices();
                        break;
                }
            } catch(MonitorUtilException ex)
            {
                logger.Fatal("Exception caught", ex);
            }

            if (debug)
            {
                Console.WriteLine("Press Enter to continue...");
                Console.ReadLine();
            }
        }

        private static void parseArgs(string[] args)
        {
            foreach (string arg in args)
            {
                switch (arg.Substring(0, 2).ToUpper())
                {
                    case "/V":
                        debug = true;
                        break;
                    case "/E":
                        mode = MonitorUtilsMode.ENABLE;
                        break;
                    case "/D":
                        mode = MonitorUtilsMode.DISABLE;
                        break;
                    case "/N":
                        mode = MonitorUtilsMode.ENUMERATE;
                        break;
                }
            }

            if(mode == MonitorUtilsMode.NOT_SET)
            {
                throw new MonitorUtilException("Mode not set, switch /E (enable), /D (disable) or /N (enumerate) must be supplied");
            }
        }
    }
}
