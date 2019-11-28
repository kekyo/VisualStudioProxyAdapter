using System;
using System.Runtime.InteropServices;

namespace ProxyAdapter
{
    internal sealed class UacUtilities
    {
        [DllImport("shell32.dll", EntryPoint = "#680", CharSet = CharSet.Unicode)]
        private static extern bool IsUserAnAdmin();

        public static bool IsAdministrativeUser =>
            IsUserAnAdmin();

        public static bool IsUacCapable =>
            (Environment.OSVersion.Platform == PlatformID.Win32NT && Environment.OSVersion.Version.Major >= 6);
    }
}
