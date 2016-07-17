using log4net;
using log4net.Config;
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

        static Program()
        {
            XmlConfigurator.Configure();
        }

        static void Main(string[] args)
        {
            logger.Info("Starting MonitorUtils");

            MonitorUtils.EnableMonitor(@"\\.\DISPLAY2", 1980, 1080, 32, 60, 1366, 0);
            Console.ReadLine();
        }
    }
}
