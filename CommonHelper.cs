using Grabacr07.KanColleViewer;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Windows;

namespace HoppoPlugin
{
    class CommonHelper
    {
        public static void UpdateChecker()
        {
            try
            {
                var req = WebRequest.Create("http://120.24.165.103/HPVersionChecker.php");
                req.Method = "GET";
                req.Timeout = 2000;
                var rsp = req.GetResponse();
                StreamReader sr = new StreamReader(rsp.GetResponseStream());
                string version = sr.ReadLine();

                if(version == "SERVICE_STOPPED")
                {
                    UniversalConstants.WarningMode = true;
                }

                if(version != UniversalConstants.CurrentVersion)
                {
                    if (File.Exists(UniversalConstants.CurrentDirectory + @"\HoppoPluginUpdater.exe"))
                        File.Delete(UniversalConstants.CurrentDirectory + @"\HoppoPluginUpdater.exe");
                    Stream updaterStream = App.GetResourceStream(new Uri("pack://application:,,,/HoppoPlugin;component/HoppoPluginUpdater.exe")).Stream;
                    Byte[] b = new Byte[updaterStream.Length];
                    updaterStream.Read(b, 0, b.Length);
                    File.WriteAllBytes(UniversalConstants.CurrentDirectory + @"\HoppoPluginUpdater.exe", b);
                    Process.Start(UniversalConstants.CurrentDirectory + @"\HoppoPluginUpdater.exe", "http://provissy.com/HoppoPlugin.dll" + " " + UniversalConstants.CurrentDirectory + @"\Plugins\HoppoPlugin.dll");
                    (Process.GetProcessesByName("KanColleViewer")[0]).Kill();
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("自动更新检查失败\n" + ex.Message);
            }
        }
    }
}
