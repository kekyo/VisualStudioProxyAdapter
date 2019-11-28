using Microsoft.Win32;
using System;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

namespace ProxyAdapter
{
    internal sealed class ProxyAdapterInstallerContext : ApplicationContext
    {
        public static readonly Assembly Assembly = typeof(ProxyAdapterInstallerContext).Assembly;
        public static readonly string Title = $"ProxyAdapter installer {Assembly.GetName().Version}";
        public static readonly string Path = System.IO.Path.GetFullPath(new Uri(Assembly.CodeBase, UriKind.RelativeOrAbsolute).LocalPath);

        private readonly NotifyIcon trayIcon;

        public ProxyAdapterInstallerContext()
        {
            var icon = new Icon(Assembly.GetManifestResourceStream("ProxyAdapter.App.ico"));
            var menuIcon = UacUtilities.IsUacCapable ? SystemIcons.Shield : icon;

            var contextMenu = new ContextMenuStrip();
            contextMenu.Items.Add("Install ProxyAdapter on Visual Studios", new Icon(menuIcon, new Size(16, 16)).ToBitmap(), InstallProxyAdapter);
            contextMenu.Items.Add("-");
            AddRunAtLogonMenu(contextMenu);
            contextMenu.Items.Add("-");
            contextMenu.Items.Add("About...", null, (s, e) =>
            {
                MessageBox.Show(
                    "ProxyAdapter installer - Install ProxyAdapter on Visual Studios utility.\r\nCopyright (c) 2019 Kouji Matsui.\r\nhttps://github.com/kekyo/VisualStudioProxyAdapter\r\nLicense under Apache-v2",
                    Title,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            });
            contextMenu.Items.Add("Exit", null, (s, e) =>
            {
                trayIcon.Visible = false;
                Application.Exit();
            });

            trayIcon = new NotifyIcon()
            {
                Icon = icon,
                ContextMenuStrip = contextMenu,
                Visible = true
            };
            trayIcon.Text = Title;
            trayIcon.DoubleClick += InstallProxyAdapter;
        }

        private void InstallProxyAdapter(object sender, EventArgs e) =>
            ProxyAdapterInstaller.InstallProxyAdapter();

        private void AddRunAtLogonMenu(ContextMenuStrip contextMenu)
        {
            var runAtLogon = (ToolStripMenuItem)contextMenu.Items.Add("Run at logon");
            runAtLogon.CheckOnClick = true;

            try
            {
                using (var key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", false))
                {
                    var path = key.GetValue("ProxyAdapterInstaller") as string ?? string.Empty;
                    runAtLogon.Checked = System.IO.Path.GetFullPath(path) == Path;
                }
            }
            catch
            {
            }

            runAtLogon.CheckedChanged += (s, e) =>
            {
                try
                {
                    using (var key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true))
                    {
                        if (runAtLogon.Checked)
                        {
                            key.SetValue("ProxyAdapterInstaller", Path);
                        }
                        else
                        {
                            key.DeleteValue("ProxyAdapterInstaller", false);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        $"Cannot set run at logon feature: {ex.Message}",
                        Title,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Exclamation);
                }
            };
        }
    }
}
