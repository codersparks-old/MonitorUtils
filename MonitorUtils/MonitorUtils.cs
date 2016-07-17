using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using WinApi;

namespace MonitorUtils
{
    class MonitorUtils
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(MonitorUtils));

        public static void EnableMonitor(string name, int resX, int resY, int bits, int frequency, int posX, int posY)
        {
            logger.Info(String.Format("Attempting to enable monitor with name: {0}", name));
            if (logger.IsDebugEnabled)
            {
                logger.Debug("Parameters supplied to function:");
                logger.Debug(String.Format("\t=>name: {0}", name));
                logger.Debug(String.Format("\t=>resX: {0}", resX));
                logger.Debug(String.Format("\t=>resY: {0}", resY));
                logger.Debug(String.Format("\t=>bits: {0}", bits));
                logger.Debug(String.Format("\t=>frequency: {0}", frequency));
                logger.Debug(String.Format("\t=>posX: {0}", posX));
                logger.Debug(String.Format("\t=>posY: {0}", posY));
            }
            WinApi.DISPLAY_DEVICE device = new WinApi.DISPLAY_DEVICE();

            device.cb = Marshal.SizeOf(device);
            WinApi.User_32.EnumDisplayDevices(name, 0, ref device, 0);
            if (logger.IsDebugEnabled) {
                logger.Debug("Device details returned from EnumDisplayDevices:");
                logger.Debug(String.Format("{0}", device));
            }

            if(device.StateFlags.HasFlag(DisplayDeviceStateFlags.AttachedToDesktop))
            {
                logger.Warn("Specified monitor ({0}) is already ena")
            }
        }
    }
}
