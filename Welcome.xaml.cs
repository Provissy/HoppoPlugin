using System.Windows;
using System;
using System.IO;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using System.Management;
using Grabacr07.KanColleViewer.Composition;
using System.Security.Cryptography;
using System.Text;

namespace HoppoPlugin
{
    /// <summary>
    /// Interaction logic for Welcome.xaml
    /// </summary>
    public partial class Welcome
    {
        public Welcome()
        {
            InitializeComponent();
        }

        private async void CallMethodButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {
                if (retryCount <= 1)
                {
                    Pgb_Progress.Value = 0;
                    Btn_Retry.Visibility = Visibility.Hidden;
                    MainContent.Visibility = Visibility.Hidden;
                    LoadingGrid.Visibility = Visibility.Visible;
                    if (!Directory.Exists("HoppoPlugin"))
                    {
                        Directory.CreateDirectory("HoppoPlugin");
                        DirectoryInfo di = new DirectoryInfo("HoppoPlugin");
                        di.CreateSubdirectory("KanColleCache");
                    }
                    Pgb_Progress.Value += await downloadSoundDLL();
                    Pgb_Progress.Value += await downloadNekoCompareImage();
                    Pgb_Progress.Value += await downloadChartDll1();
                    Pgb_Progress.Value += await downloadChartDll2();
                    Pgb_Progress.Value += await recordUsage();
                    if (Directory.Exists("Sounds")) { Directory.Delete("Sounds", true); }
                    Directory.CreateDirectory("Sounds");
                    DirectoryInfo d = new DirectoryInfo("Sounds");
                    d.CreateSubdirectory(NotifyType.Build.ToString());
                    d.CreateSubdirectory(NotifyType.Expedition.ToString());
                    d.CreateSubdirectory(NotifyType.Rejuvenated.ToString());
                    d.CreateSubdirectory(NotifyType.Repair.ToString());
                }
                else
                {
                    MessageBoxResult m = MessageBox.Show("一直出现同一错误？点击确定前往百度网盘下载必要文件，并且解压至KCV目录，即可正常使用HoppoPlugin。\n确认么？", "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if(m == MessageBoxResult.Yes)
                    {
                        Process.Start("http://pan.baidu.com/s/1AkGkY;");
                        Btn_Finish.Visibility = Visibility.Visible;
                        retryCount = 0;
                    }
                }
            }
            catch(Exception ex)
            {
                if(ex is UnauthorizedAccessException)
                {
                    MessageBox.Show("权限不足，无法在KCV目录创建文件夹，请使用管理员身份运行KCV再试！");
                    Btn_Retry.Visibility = Visibility.Visible;
                }
                else if (ex is WebException)
                {
                    MessageBox.Show("网络出现问题！请重试");
                    Btn_Retry.Visibility = Visibility.Visible;
                }
                else
                {
                    Tbl_Introdution.Text = "错误！请重试";
                    Btn_Retry.Visibility = Visibility.Visible;
                    MessageBox.Show(ex.ToString());
                }
            }
        }

        private async Task<double> downloadSoundDLL()
        {
            return await Task.Run(() =>
             {
                 return 20;
             });
        }

        private async Task<double> downloadChartDll1()
        {
            return await Task.Run(() =>
            {
                WebClient w = new WebClient();
                w.DownloadFile("http://provissy.com/WPFToolkit.dll", UniversalConstants.CurrentDirectory + @"\WPFToolkit.dll");
                FileStream file = new FileStream(UniversalConstants.CurrentDirectory + @"\WPFToolkit.dll", FileMode.Open);
                MD5 md5 = new MD5CryptoServiceProvider();
                byte[] retVal = md5.ComputeHash(file);
                file.Close();
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < retVal.Length; i++)
                {
                    sb.Append(retVal[i].ToString("x2"));
                }
                if (string.Equals(sb.ToString().ToUpper(), "195ED09E0B4F3B09EA4A3B67A0D3F396"))
                {
                    return 20;
                }
                else
                {
                    throw new Exception("MD5不匹配！");
                }
            });
        }

        private async Task<double> downloadChartDll2()
        {
            return await Task.Run(() =>
            {
                WebClient w = new WebClient();
                w.DownloadFile("http://provissy.com/System.Windows.Controls.DataVisualization.Toolkit.dll", UniversalConstants.CurrentDirectory + @"\System.Windows.Controls.DataVisualization.Toolkit.dll");
                FileStream file = new FileStream(UniversalConstants.CurrentDirectory + @"\System.Windows.Controls.DataVisualization.Toolkit.dll", FileMode.Open);
                MD5 md5 = new MD5CryptoServiceProvider();
                byte[] retVal = md5.ComputeHash(file);
                file.Close();
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < retVal.Length; i++)
                {
                    sb.Append(retVal[i].ToString("x2"));
                }
                if (string.Equals(sb.ToString().ToUpper(), "6813EBECD58E557E1D65C08E2B1030AF"))
                {
                    return 20;
                }
                else
                {
                    throw new Exception("MD5不匹配！");
                }
            });
        }

        private async Task<double> downloadNekoCompareImage()
        {
            return await Task.Run(() =>
            {
                WebClient w = new WebClient();
                w.DownloadFile("http://provissy.com/nekoError.png", UniversalConstants.CurrentDirectory + @"\HoppoPlugin\nekoError.png");
                FileStream file = new FileStream(UniversalConstants.CurrentDirectory + @"\HoppoPlugin\nekoError.png", FileMode.Open);
                MD5 md5 = new MD5CryptoServiceProvider();
                byte[] retVal = md5.ComputeHash(file);
                file.Close();
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < retVal.Length; i++)
                {
                    sb.Append(retVal[i].ToString("x2"));
                }
                if (string.Equals(sb.ToString().ToUpper(), "3959048BC55B1C50F3A2106CF6BBA16F"))
                {
                    return 20;
                }
                else
                {
                    throw new Exception("MD5不匹配！");
                }
            });
        }

        private async Task<double> recordUsage()
        {
            return await Task.Run(() =>
            {
                if (File.Exists(HoppoPluginSettings.UsageRecordPath))
                    return 20;
                var req = WebRequest.Create("http://provissy.com/RecordUsage.php");
                req.Method = "GET";
                var rsp = req.GetResponse();
                return 20;
            });
        }

        private void PgbValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if(Pgb_Progress.Value == 100)
            {
                Btn_Finish.Visibility = Visibility.Visible;
            }
        }

        int retryCount = 0;

        private void Btn_Retry_Click(object sender, RoutedEventArgs e)
        {
            CallMethodButton_Click(null, null);
            Tbl_Introdution.Text = "正在重试...";
            retryCount++;
        }
    }
}
