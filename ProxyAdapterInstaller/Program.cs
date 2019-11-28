using System;
using System.Linq;
using System.Windows.Forms;

namespace ProxyAdapter
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static int Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var arg0 = args.FirstOrDefault();
            if (arg0 == "-i")
            {
                ProxyAdapterInstaller.InstallProxyAdapter();
            }
            else
            {
                Application.Run(new ProxyAdapterInstallerContext());
            }

            return 0;
        }
    }
}
