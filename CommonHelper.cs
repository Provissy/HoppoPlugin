using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
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
                    if (File.Exists(UniversalConstants.CurrentDirectory + "UpdaterForPrvTools.exe"))
                        File.Delete(UniversalConstants.CurrentDirectory + "UpdaterForPrvTools.exe");
                    (new WebClient()).DownloadFile("http://provissy.com/UpdaterForPrvTools.exe", UniversalConstants.CurrentDirectory + "UpdaterForPrvTools.exe");
                    Process.Start(UniversalConstants.CurrentDirectory + "UpdaterForPrvTools.exe");
                }                
            }
            catch
            {
                MessageBox.Show("自动更新检查失败");
            }
        }
    }
}
