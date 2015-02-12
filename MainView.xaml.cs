using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Threading;
using Grabacr07.KanColleViewer.Models;

/*  
 *  Wtritten by @Provissy.
 *  Please use the code under the MIT License.
 */

namespace HoppoPlugin
{
    /// <summary>
    /// Interaction logic for MainView.xaml
    /// </summary>
    public partial class MainView : UserControl
    {
        public delegate void UploadToCloudCompleteEventHandler();
        public delegate void DownloadToLocalCompleteEventHandler();
        public delegate void SignUpCompleteEventHandler();
        public delegate void LoginCompleteEventHandler();

        public event UploadToCloudCompleteEventHandler UploadToCloudComplete;
        public event DownloadToLocalCompleteEventHandler DownloadToLocal;
        public event SignUpCompleteEventHandler SignUpComplete;
        public event LoginCompleteEventHandler LoginComplete;

        public static MainView Instance { get; private set; }

        public readonly string TimeFormat = "yyyy-MM-dd HH:mm:ss";
        
        private System.Timers.Timer timerForBBS = new System.Timers.Timer(5000);
        WebClient webClientForUpload = new WebClient();
        WebClient wClient = new WebClient();
        DispatcherTimer timer_AutoBackup = new DispatcherTimer();
        public MainView()
        {
            InitializeComponent();
            // Check license.
            // Remove it if you got the code from Github.
            if(UniversalConstants.WarningMode)
            {
                ContentGrid.Visibility = Visibility.Collapsed;
                WARNING_GRID.Visibility = Visibility.Visible;
            }
            this.Resources.MergedDictionaries.Add(Grabacr07.KanColleViewer.App.Current.Resources);
            Panel.SetZIndex(FunctionGrid, 1);
            WelcomePage.Visibility = Visibility.Visible;
            timer_AutoBackup.Interval = TimeSpan.FromMinutes(5);
            timer_AutoBackup.Tick += timer_AutoBackup_Tick;
            timer_AutoBackup.Start();
            //timerForBBS.Elapsed += new ElapsedEventHandler(timerForBBS_Elapsed);
            webClientForUpload.Headers.Add("Content-Type", "binary/octet-stream");
            loadAccount();
            //wForBBSDownlad.DownloadDataCompleted += wClient_DownloadDataCompleted;
            //wForBBSUpload.UploadFileCompleted += wu_UploadFileCompleted;
            UploadToCloudComplete += _uploadToCloudComplete;
            DownloadToLocal += downloadToLocalComplete;
            Instance = this;
        }

        // Auto upload statistic files to server per 5 minutes.
        async void timer_AutoBackup_Tick(object sender, EventArgs e)
        {
            try
            {
                await localToCloud(Tbl_Username.Text, false);
            }
            catch { }
        }

        private void loadAccount()
        {
            try
            {
                if (HoppoPluginSettings.Current.UsernameOfPT == "")
                    return;
                Tbl_Username.Text = HoppoPluginSettings.Current.UsernameOfPT;
                lb_Backup_Username.Content = HoppoPluginSettings.Current.UsernameOfPT;
                RegisterOrLogin.Visibility = Visibility.Collapsed;
                Btn_NaviToCloudBackup.IsEnabled = true;
            }
            catch { }
        }

        public void ErrorHandler(Exception errorMessage)
        {
            ErrorMessageTextBox.Text = errorMessage.ToString();
            closeAllTabs();
            closeFuncTab();
            ErrorHandle.Visibility = Visibility.Visible;
        }

        //private void timerForBBS_Elapsed(object sender, EventArgs e)
        //{
        //    Dispatcher.Invoke
        //    (
        //        DispatcherPriority.Normal, (Action)delegate()
        //        {
        //            timerForBBS.Stop();
        //            btn_BBS_GetComments_Click(null, null);
        //        }
        //    );
        //}

        #region Check update.

        private bool newUpdateAvailable = false;

        /// <summary>
        /// Check update by downloading a page.
        /// </summary>
        private async void CallMethodButton_Click_1(object sender, RoutedEventArgs e)
        {
            if (!newUpdateAvailable)
            {
                chkUpdateButton.Content = "检查中";
                chkUpdateButton.Content = await updateChecker();
            }
            else
            {
                chkUpdateButton.Content = "更新中";
                await updaterDownloader();
            }
        }

        private async Task<string> updateChecker()
        {
            return await Task.Run(() =>
            {
                try
                {
                    if (File.Exists(UniversalConstants.CurrentDirectory + "check"))
                    {
                        File.Delete(UniversalConstants.CurrentDirectory + "check");
                    }
                    string pathURL = "http://www.cnblogs.com/provissy/p/4056570.html";
                    string pathLocal = UniversalConstants.CurrentDirectory + "check";
                    string fileContent;
                    wClient.DownloadFile(pathURL, pathLocal);
                    System.IO.FileStream myStreama = new FileStream(UniversalConstants.CurrentDirectory + "check", FileMode.Open);       //Read File.
                    System.IO.StreamReader myStreamReader = new StreamReader(myStreama);
                    fileContent = myStreamReader.ReadToEnd();
                    myStreamReader.Close();
                    Regex reg = new Regex(UniversalConstants.CurrentVersion);     //keyword.
                    Match mat = reg.Match(fileContent);
                    if (mat.Success)
                    {
                        return "无更新";
                    }
                    else
                    {
                        newUpdateAvailable = true;
                        return "更新可用";
                    }

                }
                catch
                {
                    return "错误";
                }

                });

        }

        private async Task updaterDownloader()
        {
            await Task.Run(() =>
            {
                if(File.Exists(UniversalConstants.CurrentDirectory + "UpdaterForPrvTools.exe"))
                {
                    File.Delete(UniversalConstants.CurrentDirectory + "UpdaterForPrvTools.exe");
                }
                WebClient wClient = new WebClient();
                string pathURL = "http://provissy.com/UpdaterForPrvTools.exe";
                string pathLocal = UniversalConstants.CurrentDirectory + "UpdaterForPrvTools.exe";
                try
                {
                    WebRequest myre = WebRequest.Create(pathURL);
                    wClient.DownloadFile(pathURL, pathLocal);
                    Process.Start(UniversalConstants.CurrentDirectory + "UpdaterForPrvTools.exe");
                }
                catch (WebException exp)
                {
                    MessageBox.Show(exp.Message, "Error");
                }
            });
        }

        #endregion 

        #region Control fucntion tabs.

        //Open menu.
        private void CallMethodButton_Click(object sender, RoutedEventArgs e)
        {
            btn_OpenFunc.Visibility = Visibility.Hidden;
            btn_BackToHomePage.Visibility = Visibility.Hidden;
            //Function buttons
            funcbtn_EventMapViewer.Visibility = Visibility.Visible;
            funcbtn_Landscape.Visibility = Visibility.Visible;
            funcbtn_Cal.Visibility = Visibility.Visible;
            funcbtn_OpenDataView.Visibility = Visibility.Visible;
            funcbtn_Settings.Visibility = Visibility.Visible;
            funcbtn_Donate.Visibility = Visibility.Visible;
            funcbtn_OpenTwitter.Visibility = Visibility.Visible;
            funcbtn_OpenWiki.Visibility = Visibility.Visible;
            funcbtn_Counter.Visibility = Visibility.Visible;
            //End
            btn_ClickToClose.Visibility = Visibility.Visible;
        }

        private void btn_ClickToClose_Click(object sender, RoutedEventArgs e)
        {
            closeFuncTab();
        }

        private void closeFuncTab()
        {
            //Functions buttons
            funcbtn_EventMapViewer.Visibility = Visibility.Hidden;
            funcbtn_Landscape.Visibility = Visibility.Hidden;
            funcbtn_Cal.Visibility = Visibility.Hidden;
            funcbtn_OpenDataView.Visibility = Visibility.Hidden;
            funcbtn_Settings.Visibility = Visibility.Hidden;
            funcbtn_Donate.Visibility = Visibility.Hidden;
            funcbtn_OpenTwitter.Visibility = Visibility.Hidden;
            funcbtn_OpenWiki.Visibility = Visibility.Hidden;
            funcbtn_Counter.Visibility = Visibility.Hidden;
            
            // End.
            btn_ClickToClose.Visibility = Visibility.Hidden;
            btn_BackToHomePage.Visibility = Visibility.Visible;
            btn_OpenFunc.Visibility = Visibility.Visible;
        }

        private void closeAllTabs()
        {
            WelcomePage.Visibility = Visibility.Hidden;
            PrvToolsSettings.Visibility = Visibility.Hidden;
            ExpCal.Visibility = Visibility.Hidden;
            MyAccount.Visibility = Visibility.Hidden;
            DonateMe.Visibility = Visibility.Hidden;
            ErrorHandle.Visibility = Visibility.Hidden;
            EventMapViewer.Visibility = Visibility.Hidden;
            Counter.Visibility = Visibility.Hidden;
            CloudBackup.Visibility = Visibility.Hidden;
            LandscapeView.Visibility = Visibility.Hidden;
            LintkWiki.Visibility = Visibility.Hidden;
            CacherView.Visibility = Visibility.Hidden;
            UtilityView.Visibility = Visibility.Hidden;
            EventMapViewer.Visibility = Visibility.Hidden;
            //Stop BBS refreshing.
            timerForBBS.Stop();
        }

        private void funcbtn_Landscape_Click(object sender, RoutedEventArgs e)
        {
            closeAllTabs();
            closeFuncTab();
            LandscapeView.Visibility = Visibility.Visible;
        }

        private void funcbtn_Donate_Click(object sender, RoutedEventArgs e)
        {
            closeAllTabs();
            closeFuncTab();
            DonateMe.Visibility = Visibility.Visible;
        }

        private void btn_AkiEvent_MapViewer_Click(object sender, RoutedEventArgs e)
        {
            closeAllTabs();
            closeFuncTab();
            EventMapViewer.Visibility = Visibility.Visible;
        }

        private void funcbtn_Counter_Click(object sender, RoutedEventArgs e)
        {
            closeAllTabs();
            closeFuncTab();
            CacherView.Visibility = Visibility.Visible;

        }

        private void funcbtn_Cal_Click(object sender, RoutedEventArgs e)
        {
            closeAllTabs();
            closeFuncTab();
            ExpCal.Visibility = Visibility.Visible;
        }

        private void btn_BackToHomePage_Click(object sender, RoutedEventArgs e)
        {
            closeAllTabs();
            btn_BackToHomePage.Visibility = Visibility.Hidden;
            WelcomePage.Visibility = Visibility.Visible;
        }

        private void funcbtn_Settings_Click(object sender, RoutedEventArgs e)
        {
            closeAllTabs();
            closeFuncTab();
            PrvToolsSettings.Visibility = Visibility.Visible;
        }

        private void Btn_NaviToCloudBackup_Click(object sender, RoutedEventArgs e)
        {
            closeAllTabs();
            closeFuncTab();
            CloudBackup.Visibility = Visibility.Visible;
        }

        private void Btn_MyAccount_Click(object sender, RoutedEventArgs e)
        {
            closeAllTabs();
            closeFuncTab();
            MyAccount.Visibility = Visibility.Visible;
        }

        private void funcbtn_OpenTwitter_Click(object sender, RoutedEventArgs e)
        {
            closeAllTabs();
            closeFuncTab();
            UtilityView.Visibility = Visibility.Visible;
        }

        private void funcbtn_OpenWiki_Click(object sender, RoutedEventArgs e)
        {
            closeAllTabs();
            closeFuncTab();
            LintkWiki.Visibility = Visibility.Visible;
            Wb_Wiki.Navigate("http://provissy.com/JumpToWikiLintk.php");
        }

        private void funcbtn_OpenDataView_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                (new StatisticWindow{DataContext = new StatisticWindowViewModel()}).Show();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void CallMethodButton_Click_4(object sender, RoutedEventArgs e)
        {
            closeAllTabs();
            closeFuncTab();
            EventMapViewer.Visibility = Visibility.Visible;
        }

        #endregion 

        //#region BBS center.
        //WebClient wForBBSUpload = new WebClient();

        //// Run a auto-check loop.
        //// Event fires when get comments complete.
        //void wClient_DownloadDataCompleted(object sender, DownloadDataCompletedEventArgs e)
        //{
        //    timerForBBS.Start();
        //}

        //void wu_UploadFileCompleted(object sender, UploadFileCompletedEventArgs e)
        //{
        //    lb_BBS_ShowState.Content = "发送成功";
        //}

        //private async void btn_BBS_SendComment_Click(object sender, RoutedEventArgs e)
        //{
        //    if (tb_BBS_Username.Text == "输入你的名字" || tb_BBS_Username.Text == "")
        //    {
        //        MessageBox.Show("请输入用户名！或者注册一个HoppoPlugin账户！");
        //        return;
        //    }
        //    try
        //    {
        //        lb_BBS_ShowState.Content = "送信中...";
        //        WebClient w = new WebClient();
                
        //        wForBBSUpload.Headers.Add("Content-Type", "binary/octet-stream");
        //        FlowDocument downloadedDoc = XamlReader.Load(new MemoryStream(await w.DownloadDataTaskAsync("http://provissy.com/BBSCenter/BBS_Test_4"))) as FlowDocument;
        //        FlowDocument sourceDocumentToSend = rtb_BBS_CommentToSend.Document;
        //        Paragraph p = new Paragraph(new Run(tb_BBS_Username.Text + "---" + DateTime.Now.ToString(TimeFormat)));
        //        p.Foreground = Brushes.Blue;
        //        sourceDocumentToSend.Blocks.InsertBefore(sourceDocumentToSend.Blocks.FirstBlock, p);

        //        // Combine 2 flow documents.
        //        FlowDocument finalDoc = new FlowDocument();
        //        List<Block> flowDocumetnBlocks1 = new List<Block>(sourceDocumentToSend.Blocks);
        //        List<Block> flowDocumetnBlocks2 = new List<Block>(downloadedDoc.Blocks);
        //        foreach (Block item in flowDocumetnBlocks1)
        //        {
        //            finalDoc.Blocks.Add(item);
        //        }
        //        foreach (Block item in flowDocumetnBlocks2)
        //        {
        //            finalDoc.Blocks.Add(item);
        //        }


        //        Stream s = new MemoryStream();
        //        XamlWriter.Save(finalDoc, s);

        //        byte[] data = new byte[s.Length];
        //        s.Position = 0;
        //        s.Read(data, 0, data.Length);
        //        s.Close();
        //        File.WriteAllBytes(UniversalConstants.CurrentDirectory + @"\HoppoPlugin\BBS_Test_4", data);

        //        await wForBBSUpload.UploadFileTaskAsync("http://provissy.com/UpdateToBBSCenter.php", "POST", UniversalConstants.CurrentDirectory + @"\HoppoPlugin\BBS_Test_4");
        //        File.Delete(UniversalConstants.CurrentDirectory + @"\HoppoPlugin\BBS_Test_4");
        //        btn_BBS_GetComments_Click(null, null);

        //    }
        //    catch(Exception ex)
        //    {
        //        ErrorHandler(ex);
        //    }
        //}

        //WebClient wForBBSDownlad = new WebClient();

        //private async void btn_BBS_GetComments_Click(object sender, RoutedEventArgs e)
        //{
        //    try
        //    {
        //        rtb_BBS_Comment.Document = XamlReader.Load(new MemoryStream( await wForBBSDownlad.DownloadDataTaskAsync("http://provissy.com/BBSCenter/BBS_Test_4"))) as FlowDocument;
        //        //lb_BBS_ShowState.Content = "自动受信";
        //    }
        //    catch
        //    {
        //        //lb_BBS_ShowState.Content = "受信失敗";
        //        timerForBBS.Start();
        //    }
        //}

        //private void rtb_BBS_CommentToSend_GotFocus(object sender, RoutedEventArgs e)
        //{
        //    Paragraph p = new Paragraph(new Run(""));
        //    FlowDocument f = new FlowDocument(p);
        //    rtb_BBS_CommentToSend.Document = f;
        //    rtb_BBS_CommentToSend.GotFocus -= rtb_BBS_CommentToSend_GotFocus;
        //}

        //private void rtb_BBS_CommentToSend_KeyDown(object sender, KeyEventArgs e)
        //{
        //    if (e.KeyboardDevice.IsKeyDown(Key.LeftCtrl) && e.Key == Key.Enter)
        //    {
        //        btn_BBS_SendComment_Click(null, null);
        //    }
        //    else if (e.KeyboardDevice.IsKeyDown(Key.RightCtrl) && e.Key == Key.Enter)
        //    {
        //        btn_BBS_SendComment_Click(null, null);
        //    }
        //}

        //private void Button_Click_1(object sender, RoutedEventArgs e)
        //{
        //    int fontSize = Convert.ToInt32(rtb_BBS_CommentToSend.Selection.GetPropertyValue(TextElement.FontSizeProperty));
        //    fontSize++;
        //    rtb_BBS_CommentToSend.Selection.ApplyPropertyValue(TextElement.FontSizeProperty, fontSize.ToString());
        //}

        //private void btn_BBS_SetColorToRed_Click(object sender, RoutedEventArgs e)
        //{
        //    rtb_BBS_CommentToSend.Selection.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.Red);
        //}

        //// Fix bbs.
        //private void CallMethodButton_Click_10(object sender, RoutedEventArgs e)
        //{
        //    System.IO.Stream s = new System.IO.MemoryStream();
        //    System.Windows.Markup.XamlWriter.Save((FlowDocument)rtb_BBS_CommentToSend.Document, s);
        //    byte[] data = new byte[s.Length];
        //    s.Position = 0;
        //    s.Read(data, 0, data.Length);
        //    s.Close();
        //    File.WriteAllBytes(UniversalConstants.CurrentDirectory + @"\HoppoPlugin\BBS_Test_4", data);
        //    webClientForUpload.UploadFile("http://provissy.com/UpdateToBBSCenter.php", "POST", UniversalConstants.CurrentDirectory + @"\HoppoPlugin\BBS_Test_4");
        //    File.Delete(UniversalConstants.CurrentDirectory + @"\HoppoPlugin\BBS_Test_4");
        //}

        //#endregion

        #region MyAccount Function.

        private bool isInLoginPage = false;

        private void Btn_ClickToLogin_Click(object sender, RoutedEventArgs e)
        {
            if (!isInLoginPage)
            {
                isInLoginPage = true;
                Tbl_PTAccountIntroduce.Text = "输入用户名和密码登录\n登录后可手动进行同步";
                Btn_CreateAccount.Content = "登录";
                Btn_ClickToLogin.Content = "注册新账户";
            }
            else
            {
                isInLoginPage = false;
                Tbl_PTAccountIntroduce.Text = "注册一个HoppoPlugin账户,\n您将可以将统计信息和设置同步到云端\n以及享受云服务带来的便利";
                Btn_CreateAccount.Content = "注册";
                Btn_ClickToLogin.Content = "已有账户？点击登录";
            }
        }

        private async void Btn_CreateAccount_Click(object sender, RoutedEventArgs e)
        {
            if (isInLoginPage)
            {
                if (tb_Backup_Username.Text == "" || psd_Backup_Password.Password == "")
                {
                    MessageBox.Show("用户名或密码不能为空！");
                    return;
                }
                Btn_CreateAccount.IsEnabled = false;
                Btn_CreateAccount.Content = "正在登录...";
                LoginComplete += new LoginCompleteEventHandler(loginComplete);
                try
                {
                    await login(tb_Backup_Username.Text, psd_Backup_Password.Password);
                }
                catch (Exception ex)
                {
                    ErrorHandler(ex);
                    Btn_CreateAccount.IsEnabled = true;
                    Btn_CreateAccount.Content = "登录";
                }
            }

            else
            {
                if (tb_Backup_Username.Text == "" || psd_Backup_Password.Password == "")
                {
                    MessageBox.Show("用户名或密码不能为空！");
                    return;
                }
                Btn_CreateAccount.IsEnabled = false;
                Btn_CreateAccount.Content = "注册中...";
                SignUpComplete += new SignUpCompleteEventHandler(signUpComplete);
                try
                {
                    await createAccount(tb_Backup_Username.Text, psd_Backup_Password.Password);
                }
                catch (Exception ex)
                {
                    ErrorHandler(ex);
                }
            }
        }

        private async Task createAccount(string username , string password)
        {
            await Task.Run(() =>
            {
                if (!File.Exists(UniversalConstants.CurrentDirectory + @"\HoppoPlugin\PrvToolsUsrCfg"))
                {
                    var w = File.AppendText(UniversalConstants.CurrentDirectory + @"\HoppoPlugin\PrvToolsUsrCfg");
                    //w.WriteLine(username);
                    w.WriteLine(password);
                    w.Close();
                }
                else
                {
                    File.Delete(UniversalConstants.CurrentDirectory + @"\HoppoPlugin\PrvToolsUsrCfg");
                    var w = File.AppendText(UniversalConstants.CurrentDirectory + @"\HoppoPlugin\PrvToolsUsrCfg");
                    //w.WriteLine(username);
                    w.WriteLine(password);
                    w.Close();
                }
                File.Copy(UniversalConstants.CurrentDirectory + @"\HoppoPlugin\PrvToolsUsrCfg", UniversalConstants.CurrentDirectory + @"\HoppoPlugin\" + username, true);
                webClientForUpload.UploadFile("http://provissy.com/UploadToUserAccountsFolder.php", "POST", UniversalConstants.CurrentDirectory + @"\HoppoPlugin\" + username);
                File.Delete(UniversalConstants.CurrentDirectory + @"\HoppoPlugin\" + username);
                SignUpComplete();
            });
        }

        private void signUpComplete()
        {
            Action a = new Action(() =>
            {
                RegisterOrLogin.Visibility = Visibility.Hidden;
                Btn_NaviToCloudBackup.IsEnabled = true;
                Tbl_Username.Text = tb_Backup_Username.Text;
                HoppoPluginSettings.Current.UsernameOfPT = tb_Backup_Username.Text;
                HoppoPluginSettings.Current.Save();
            });
            this.Dispatcher.Invoke(a, DispatcherPriority.ApplicationIdle);
        }


        private async Task login(string username, string password)
        {
            await Task.Run(() =>
            {
                string downPass1 = wClient.DownloadString("http://provissy.com/UserAccounts/" + username);
                string downPass2 = downPass1.Substring(0,password.Length);
                //MessageBox.Show("NET::" + downPass2 +"::");
                //MessageBox.Show("PSD::" + password + "::");
                if(String.Equals(downPass2,password))
                {                 
                    LoginComplete();
                }
                else
                {
                    MessageBox.Show("用户名或密码错误！");
                    Action a = new Action(() =>
                    {
                        Btn_CreateAccount.IsEnabled = true;
                        Btn_CreateAccount.Content = "登录";
                    });
                    this.Dispatcher.Invoke(a, DispatcherPriority.ApplicationIdle);
                }
            });
        }

        private void loginComplete()
        {
            Action a = new Action(() =>
            {
                RegisterOrLogin.Visibility = Visibility.Hidden;
                Btn_NaviToCloudBackup.IsEnabled = true;
                Tbl_Username.Text = tb_Backup_Username.Text;
                HoppoPluginSettings.Current.UsernameOfPT = tb_Backup_Username.Text;
                HoppoPluginSettings.Current.Save();
            });
            this.Dispatcher.Invoke(a, DispatcherPriority.ApplicationIdle);
        }


        private async void btn_Backup_CloudToLocal_Click(object sender, RoutedEventArgs e)
        {
            tbl_Backup_Introdution.Text = "同步中...";
            
            try { 
            await cloudToLocal(Tbl_Username.Text);
            }
            catch (Exception ex)
            {
                ErrorHandler(ex);
            }
        }

        private async Task cloudToLocal(string username)
        {
             await Task.Run(() =>
                {
                    string l = UniversalConstants.CurrentDirectory;
                    string adress = "http://provissy.com/UserStatisticData/";
                    //Auto replace.
                    wClient.DownloadFile(adress + "ItemBuildLog" + " - " + username + ".csv", l + "ItemBuildLog.csv");
                    wClient.DownloadFile(adress + "ShipBuildLog" + " - " + username + ".csv", l + "ShipBuildLog.csv");
                    wClient.DownloadFile(adress + "DropLog" + " - " + username + ".csv", l + "DropLog.csv");
                    wClient.DownloadFile(adress + "MaterialsLog" + " - " + username + ".csv", l + "MaterialsLog.csv");
                    DownloadToLocal();
                });
        }

        private void downloadToLocalComplete()
        {
            
            Action a = new Action(() =>
            {
                tbl_Backup_Introdution.Text = "同步成功！";
            });
            this.Dispatcher.Invoke(a, DispatcherPriority.ApplicationIdle);
        }

        private async void btn_Backup_LocalToCloud_Click(object sender, RoutedEventArgs e)
        {
            tbl_Backup_Introdution.Text = "备份中...";
            try
            {
                
                await localToCloud(Tbl_Username.Text, true);
            }
            catch(Exception ex)
            {
                ErrorHandler(ex);
            }
        }

        private async Task localToCloud(string username, bool notifyUser)
        {
            await Task.Run(() =>
            {

                string c = UniversalConstants.CurrentDirectory;
                if (File.Exists(c + "ItemBuildLog.csv"))
                {
                    File.Copy(c + "ItemBuildLog.csv", c + "ItemBuildLog" + " - " + username + ".csv", true);
                    webClientForUpload.UploadFile("http://provissy.com/UploadToStatisticFolder.php", "POST", c + "ItemBuildLog" + " - " + username + ".csv");
                    File.Delete(c + "ItemBuildLog" + " - " + username + ".csv");
                }
                if (File.Exists(c + "ShipBuildLog.csv"))
                {
                    File.Copy(c + "ShipBuildLog.csv", c + "ShipBuildLog" + " - " + username + ".csv", true);
                    webClientForUpload.UploadFile("http://provissy.com/UploadToStatisticFolder.php", "POST", c + "ShipBuildLog" + " - " + username + ".csv");
                    File.Delete(c + "ShipBuildLog" + " - " + username + ".csv");
                }
                if (File.Exists(c + "DropLog.csv"))
                {
                    File.Copy(c + "DropLog.csv", c + "DropLog" + " - " + username + ".csv", true);
                    webClientForUpload.UploadFile("http://provissy.com/UploadToStatisticFolder.php", "POST", c + "DropLog" + " - " + username + ".csv");
                    File.Delete(c + "DropLog" + " - " + username + ".csv");
                }
                if (File.Exists(c + "MaterialsLog.csv"))
                {
                    File.Copy(c + "MaterialsLog.csv", c + "MaterialsLog" + " - " + username + ".csv", true);
                    webClientForUpload.UploadFile("http://provissy.com/UploadToStatisticFolder.php", "POST", c + "MaterialsLog" + " - " + username + ".csv");
                    File.Delete(c + "MaterialsLog" + " - " + username + ".csv");
                }
                if (notifyUser)
                    UploadToCloudComplete();
            });
        }


        private void _uploadToCloudComplete()
        {
            
            Action a = new Action(() =>
            {
                tbl_Backup_Introdution.Text = "备份成功！";
            });
            this.Dispatcher.Invoke(a, DispatcherPriority.ApplicationIdle);
        }

        #endregion

        #region Others.
        private void Label_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Process.Start("http://kcc.moe/progit/");
        }

        private void TextBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Clipboard.SetDataObject("provissy@gmail.com");
            MessageBox.Show("支付宝地址已复制！");
        }

        private void btn_UpdateLog_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("http://provissy.com/");
        }

        private void Btn_OpenWikiLintk_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("http://wiki.lintk.me/");
        }
        #endregion

        private void Btn_AutoSetLogbook_Click(object sender, RoutedEventArgs e)
        {
            if(Process.GetProcessesByName("javaw").Length == 0)
            {
                MessageBoxResult mbr = MessageBox.Show("您并未运行航海日志？请运行航海日志或点击确定前往下载，点击否则继续设置，点击取消终止设置。","Warning",MessageBoxButton.YesNoCancel,MessageBoxImage.Question);
                if(mbr == MessageBoxResult.Yes)
                {
                    Process.Start("http://nekopanda.blog.jp/");
                }
                else if (mbr == MessageBoxResult.No)
                {
                    Settings.Current.EnableProxy = true;
                    Settings.Current.ProxyHost = "localhost";
                    Settings.Current.ProxyPort = 8888;
                    Settings.Current.ProxySettings.IsEnabled = true;
                    Settings.Current.ProxySettings.Host = "localhost";
                    Settings.Current.ProxySettings.Port = 8888;
                    MessageBox.Show("Success !");
                }
                else { }
            }
            else
            {
                Settings.Current.EnableProxy = true;
                Settings.Current.ProxyHost = "localhost";
                Settings.Current.ProxyPort = 8888;
                Settings.Current.ProxySettings.IsEnabled = true;
                Settings.Current.ProxySettings.Host = "localhost";
                Settings.Current.ProxySettings.Port = 8888;
                MessageBox.Show("Success !");
            }
        }

        private void Btn_AutoSetProxyByPort_Click(object sender, RoutedEventArgs e)
        {
            if(Ptb_ProxyPort.Text == "")
            {
                MessageBox.Show("请输入端口！");
                return;
            }
            try
            {
                Settings.Current.EnableProxy = true;
                Settings.Current.ProxyHost = "localhost";
                Settings.Current.ProxyPort = Convert.ToUInt16(Ptb_ProxyPort.Text);
                Settings.Current.ProxySettings.IsEnabled = true;
                Settings.Current.ProxySettings.Host = "localhost";
                Settings.Current.ProxySettings.Port = Convert.ToUInt16(Ptb_ProxyPort.Text);
                MessageBox.Show("Success !");
            }
            catch
            {
                MessageBox.Show("端口错误！");
            }
        }
    }
}
