using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;

namespace ProxyAdapter
{
    internal static class ProxyAdapterInstaller
    {
        private static IEnumerable<XElement> EnumerateVisualStudios()
        {
            var vswherePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.exe");
            try
            {
                using (var fs = new FileStream(vswherePath, FileMode.Create, FileAccess.ReadWrite, FileShare.None))
                {
                    using (var s = ProxyAdapterInstallerContext.Assembly.GetManifestResourceStream("ProxyAdapter.vswhere.exe"))
                    {
                        s.CopyTo(fs);
                    }
                    fs.Flush();
                }

                using (var process = new Process())
                {
                    process.StartInfo = new ProcessStartInfo
                    {
                        Arguments = "-legacy -utf8 -format xml",
                        FileName = vswherePath,
                        UseShellExecute = false,
                        CreateNoWindow = false,
                        WindowStyle = ProcessWindowStyle.Hidden,
                        RedirectStandardOutput = true,
                        RedirectStandardInput = false,
                        StandardOutputEncoding = Encoding.UTF8
                    };

                    process.Start();

                    var document = XDocument.Load(process.StandardOutput);
                    process.WaitForExit();

                    return document.Root.Elements("instance");
                }
            }
            finally
            {
                try
                {
                    File.Delete(vswherePath);
                }
                catch
                {
                }
            }
        }

        private static void InstallProxyAdapterOnToVisualStudios()
        {
            try
            {
                var visualStudios = EnumerateVisualStudios().
                    ToArray();

                var installed = new List<string>();
                foreach (var entry in
                    from instance in visualStudios
                    let productPath = instance.Element("productPath")
                    let path = (string)productPath
                    let displayName = instance.Element("displayName")
                    let instanceId = instance.Element("instanceId")
                    let name = (string)displayName ?? (string)instanceId
                    where !string.IsNullOrWhiteSpace(path) && File.Exists(path) && !string.IsNullOrWhiteSpace(name)
                    select new { name, path })
                {
                    var basePath = Path.GetDirectoryName(entry.path);
                    var configPath = Path.Combine(basePath, "devenv.exe.config");
                    if (File.Exists(configPath))
                    {
                        InstallProxyAdapterOnToVisualStudio(basePath, configPath);
                        installed.Add(entry.name);
                    }
                }

                var i = string.Join(",\r\n", installed);
                MessageBox.Show(
                    $"ProxyAdapter installed onto:\r\n{i}",
                    ProxyAdapterInstallerContext.Title,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Cannot install ProxyAdapter onto Visual Studio: {ex.Message}",
                    ProxyAdapterInstallerContext.Title,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private static readonly string typeName = "ProxyAdapter.FromEnvironment, ProxyAdapter";

        private static void InstallProxyAdapterOnToVisualStudio(string basePath, string configPath)
        {
            var configDocument = XDocument.Load(configPath);

            foreach (var module in configDocument.Root.
                Elements("system.net").
                Elements("defaultProxy").
                Elements("module").
                Where(e => (string)e.Attribute("type") == typeName))
            {
                module.Remove();
            }

            var systemNets = configDocument.Root.
                Elements("system.net").
                ToArray();
            if (systemNets.Length == 0)
            {
                systemNets = new[] { new XElement("system.net") };
                configDocument.Root.Add(systemNets[0]);
            }

            var defaultProxies = systemNets[0].
                Elements("defaultProxy").
                Where(e => ((bool?)e.Attribute("enabled") ?? true) && !((bool?)e.Attribute("useDefaultCredentials") ?? false)).
                ToArray();
            if (defaultProxies.Length == 0)
            {
                defaultProxies = new[] { new XElement("defaultProxy",
                    new XAttribute("enabled", "true"),
                    new XAttribute("useDefaultCredentials", "false")) };
                systemNets[0].Add(defaultProxies[0]);
            }

            defaultProxies[0].Add(
                new XElement("module", new XAttribute("type", typeName)));

            foreach (var fileName in new[] { "ProxyAdapter.dll", "ProxyAdapter.pdb" })
            {
                var path = Path.Combine(basePath, fileName);
                using (var fs = new FileStream(path, FileMode.Create, FileAccess.ReadWrite, FileShare.None))
                {
                    using (var s = ProxyAdapterInstallerContext.Assembly.GetManifestResourceStream($"ProxyAdapter.{fileName}"))
                    {
                        s.CopyTo(fs);
                    }
                    fs.Flush();
                }
            }

            configDocument.Save(configPath + ".tmp");

            try
            {
                File.Delete(configPath + ".orig");
            }
            catch
            {
            }

            File.Move(configPath, configPath + ".orig");

            try
            {
                File.Move(configPath + ".tmp", configPath);
            }
            catch
            {
                try
                {
                    File.Delete(configPath);
                }
                catch
                {
                }
                try
                {
                    File.Delete(configPath + ".tmp");
                }
                catch
                {
                }

                File.Move(configPath + ".orig", configPath);
                throw;
            }
        }

        public static void InstallProxyAdapter()
        {
            if (UacUtilities.IsAdministrativeUser)
            {
                InstallProxyAdapterOnToVisualStudios();
                return;
            }
            else
            {
                try
                {
                    Process.Start(new ProcessStartInfo
                    {
                        Arguments = "-i",
                        FileName = ProxyAdapterInstallerContext.Path,
                        Verb = "runas"
                    });
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        $"Cannot elevate administrator privilege: {ex.Message}",
                        ProxyAdapterInstallerContext.Title,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
        }
    }
}
