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

        public static void EnableMonitor(string name, uint resX, uint resY, uint bits, uint frequency, int posX, int posY)
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
            if (logger.IsDebugEnabled)
            {
                logger.Debug("Device details returned from EnumDisplayDevices:");
                logger.Debug(String.Format("{0}", device));
            }

            if (device.StateFlags.HasFlag(DisplayDeviceStateFlags.AttachedToDesktop))
            {
                logger.Warn(String.Format("Specified monitor ({0}) is already enabled therefore taking no action", name));
            }
            else
            {
                logger.Info(String.Format("Specified monitor {0} is not enabled, attempting to enable...", name));

                DEVMODE devMode = new DEVMODE();
                devMode.dmSize = (ushort)Marshal.SizeOf(devMode);
                devMode.dmPelsWidth = resX;
                devMode.dmPelsHeight = resY;
                devMode.dmBitsPerPel = bits;
                devMode.dmDisplayFrequency = frequency;
                POINTL pos = new POINTL();
                pos.x = posX;
                pos.y = posY;
                devMode.dmPosition = pos;

                logger.Debug("Attempting to save settings (1)");
                DISP_CHANGE result1 = User_32.ChangeDisplaySettingsEx(name, ref devMode, (IntPtr)null, (ChangeDisplaySettingsFlags.CDS_UPDATEREGISTRY | ChangeDisplaySettingsFlags.CDS_NORESET), IntPtr.Zero);

                logger.Debug(String.Format("Result of save settings (1): {0}", result1));

                if (result1 != DISP_CHANGE.Successful)
                {
                    logger.Error(String.Format("First execution of ChangeDisplaySettingsEx was not successful, result: {0}", result1));
                    if (result1 == DISP_CHANGE.BadMode)
                    {
                        logger.Error("Result is BadMode - Is this monitor connected?");
                    }
                    throw new MonitorUtilException(String.Format("ChangeDisplaySettingsEx function calls is not successful, result: {0}", result1));
                }


                logger.Debug("Attempting to save settings (2)");
                DISP_CHANGE result2 = User_32.ChangeDisplaySettingsEx(null, IntPtr.Zero, (IntPtr)null, ChangeDisplaySettingsFlags.CDS_NONE, (IntPtr)null);
                logger.Debug(String.Format("Result of save settings (2): {0}", result2));

                if (result1 == DISP_CHANGE.Successful && result2 == DISP_CHANGE.Successful)
                {
                    logger.Info("...Monitor Enable Finished");
                }
                else
                {
                    logger.Error(String.Format("Second execution of ChangeDisplaySettingsEx was not successful, result: {0}", result2));
                    throw new MonitorUtilException(String.Format("ChangeDisplaySettingsEx function calls is not successful, result: {0}", result2));
                }
            }
        }

        public static void DisableMonitor(string name)
        {
            logger.Info(String.Format("Attempting to disable monitor with name: {0}", name));
            if (logger.IsDebugEnabled)
            {
                logger.Debug("Parameters supplied to function:");
                logger.Debug(String.Format("\t=>name: {0}", name));
            }
            WinApi.DISPLAY_DEVICE device = new WinApi.DISPLAY_DEVICE();

            device.cb = Marshal.SizeOf(device);
            WinApi.User_32.EnumDisplayDevices(name, 0, ref device, 0);
            if (logger.IsDebugEnabled)
            {
                logger.Debug("Device details returned from EnumDisplayDevices:");
                logger.Debug(String.Format("{0}", device));
            }

            if (!device.StateFlags.HasFlag(DisplayDeviceStateFlags.AttachedToDesktop))
            {
                logger.Warn(String.Format("Specified monitor ({0}) is already disabled (or not connected) therefore taking no action", name));
            }
            else
            {
                logger.Info(String.Format("Specified monitor {0} is enabled, attempting to disable...", name));
                DEVMODE devMode = new DEVMODE();

                User_32.EnumDisplaySettings(name, -1, ref devMode);


                devMode.dmPelsWidth = 0;
                devMode.dmPelsHeight = 0;
                POINTL pos = new POINTL();
                pos.x = 0;
                pos.y = 0;
                devMode.dmPosition = pos;
                devMode.dmSize = (ushort)Marshal.SizeOf(devMode);

                DISP_CHANGE result1 = User_32.ChangeDisplaySettingsEx(name, ref devMode, (IntPtr)null, (ChangeDisplaySettingsFlags.CDS_UPDATEREGISTRY), IntPtr.Zero);
                logger.Debug(String.Format("Result of save settings (1): {0}", result1));

                if (result1 != DISP_CHANGE.Successful)
                {
                    logger.Error(String.Format("First execution of ChangeDisplaySettingsEx was not successful, result: {0}", result1));
                    throw new MonitorUtilException(String.Format("ChangeDisplaySettingsEx function calls is not successful, result: {0}", result1));
                }

                DISP_CHANGE result2 = User_32.ChangeDisplaySettingsEx(null, IntPtr.Zero, (IntPtr)null, ChangeDisplaySettingsFlags.CDS_NONE, (IntPtr)null);
                logger.Debug(String.Format("Result of save settings (2): {0}", result2));

                if (result1 == DISP_CHANGE.Successful && result2 == DISP_CHANGE.Successful)
                {
                    logger.Info("...Monitor Enable Finished");
                }
                else
                {
                    logger.Error(String.Format("Second execution of ChangeDisplaySettingsEx was not successful, result: {0}", result2));
                    throw new MonitorUtilException(String.Format("ChangeDisplaySettingsEx function calls is not successful, result: {0}", result2));
                }
            }
        }

        public static void EnumarateDisplayDevices()
        {
            try
            {
                DISPLAY_DEVICE device = new DISPLAY_DEVICE();
                device.cb = Marshal.SizeOf(device);

                for (int id = 0; User_32.EnumDisplayDevices(null, id, ref device, 0); id++)
                {
                    Console.WriteLine(String.Format("ID: {0}", id));
                    Console.WriteLine(String.Format("DISPLAY_DEVICE (using id): {0}", device));

                    if (device.StateFlags.HasFlag(DisplayDeviceStateFlags.AttachedToDesktop))
                    {
                        if (device.StateFlags.HasFlag(DisplayDeviceStateFlags.PrimaryDevice))
                        {
                            Console.WriteLine("\tDevice Connected: Primary Display");
                        }
                        else
                        {
                            Console.WriteLine("\tDevice connected: Not Primary Display");
                        }
                    } else
                    {
                        if(device.StateFlags.HasFlag(DisplayDeviceStateFlags.ModesPruned))
                        {
                            Console.WriteLine("\tDevice Connected: Disabled");
                        } else
                        {
                            Console.WriteLine("\tDevice not used by system");
                        }
                    }


                    device.cb = Marshal.SizeOf(device);
                    User_32.EnumDisplayDevices(device.DeviceName, 0, ref device, 0);
                    Console.WriteLine(
                        String.Format("DISPLAY_DEVICE (using name): {0}", device)
                        );

                    


                    //Console.WriteLine("\tSupported Modes:");

                    //int modeIndex = 0;
                    //while(User_32.EnumDisplaySettings(device.DeviceName, modeIndex, ref devMode))
                    //{
                    //    Console.WriteLine("\t\t" +
                    //        "{0} by {1}, " +
                    //        "{2} bit, " +
                    //        "{3} degrees, " +
                    //        "{4} hertz",
                    //        devMode.dmPelsWidth,
                    //        devMode.dmPelsHeight,
                    //        devMode.dmBitsPerPel,
                    //        devMode.dmOrientation * 90,
                    //        devMode.dmDisplayFrequency);

                    //    modeIndex++; // The next mode
                    //}
                }
                device.cb = Marshal.SizeOf(device);

            }
            catch (Exception e)
            {
                throw new MonitorUtilException(String.Format("{0}", e.ToString()), e);
            }
        }
    }
}
