using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Grabacr07.KanColleViewer.Composition;
using Application = System.Windows.Application;
using Grabacr07.KanColleViewer;
using System.Diagnostics;
using System.IO;
using System.Windows.Media;
using System.Windows.Threading;

namespace HoppoPlugin
{
    internal class Windows7Notifier : INotifier
    {
        private MediaPlayer mp;
        private NotifyIcon notifyIcon;
        private EventHandler activatedAction;
        private DispatcherTimer timer = new DispatcherTimer();

        public void Initialize()
        {
            mp = new MediaPlayer();
            timer.Tick += new EventHandler(timer_Tick);
            timer.Interval = TimeSpan.FromSeconds(3);
            timer.Start();
            const string iconUri = "pack://application:,,,/KanColleViewer;Component/Assets/app.ico";

            Uri uri;
            if (!Uri.TryCreate(iconUri, UriKind.Absolute, out uri))
                return;

            var streamResourceInfo = Application.GetResourceStream(uri);
            if (streamResourceInfo == null)
                return;

            using (var stream = streamResourceInfo.Stream)
            {
                this.notifyIcon = new NotifyIcon
                {
                    Text = App.ProductInfo.Title,
                    Icon = new Icon(stream),
                };
                ContextMenu menu = new ContextMenu();

                MenuItem closeItem = new MenuItem();
                closeItem.Text = "退出 KanColleViewer（强制）";
                closeItem.Click += new EventHandler(delegate
                    {
                        System.Diagnostics.Process[] killprocess = System.Diagnostics.Process.GetProcessesByName("KanColleViewer");
                        foreach (System.Diagnostics.Process p in killprocess)
                        {
                            p.Kill();
                        }
                    });

                MenuItem addItem = new MenuItem();
                addItem.Text = "关闭计算机";
                addItem.Click += new EventHandler(delegate { Process.Start("shutdown.exe", "-s -t 00"); });

                menu.MenuItems.Add(addItem);
                menu.MenuItems.Add(closeItem);

                notifyIcon.ContextMenu = menu;
            }

            

        }

        private void timer_Tick(object sender, EventArgs e)
        {
            try
            {
                timer.Stop();
                if (HoppoPluginSettings.Current.EnableNotifyIcon)
                    notifyIcon.Visible = true;
            }
            catch { }
        }

        public void Show(NotifyType type, string header, string body, Action activated, Action<Exception> failed = null)
        {
            try
            {
                if (HoppoPluginSettings.Current.EnableSoundNotify)
                {
                    mp.Dispatcher.Invoke(new Action(() =>
                    {
                        var Audiofile = GetRandomSound(type.ToString());
                        mp.Close();
                        mp.Volume = HoppoPluginSettings.Current.Volume;
                        mp.Open(new Uri(UniversalConstants.CurrentDirectory + Audiofile));
                        mp.Play();
                    }));

                }
            }

            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString());
                notifyIcon.ShowBalloonTip(5000, "ERROR", ex.ToString(), ToolTipIcon.Error);
            }

            if (this.notifyIcon == null)
                return;

            if (activated != null)
            {
                this.notifyIcon.BalloonTipClicked -= this.activatedAction;

                this.activatedAction = (sender, args) => activated();
                this.notifyIcon.BalloonTipClicked += this.activatedAction;
            }
            notifyIcon.ShowBalloonTip(5000, header, body, ToolTipIcon.Info);


        }

        public string GetRandomSound(string type)
        {
            try
            {
                if (!Directory.Exists("Sounds\\"))
                {
                    Directory.CreateDirectory("Sounds");
                }

                if (!Directory.Exists("Sounds\\" + type))
                {
                    Directory.CreateDirectory("Sounds\\" + type);
                    return null;
                }

                List<string> FileList = Directory.GetFiles("Sounds\\" + type, "*.wav", SearchOption.AllDirectories)
                    .Concat(Directory.GetFiles("Sounds\\" + type, "*.mp3", SearchOption.AllDirectories)).ToList();

                if (FileList.Count > 0)
                {
                    Random Rnd = new Random();
                    return FileList[Rnd.Next(0, FileList.Count)];
                }
            }
            catch (Exception ex)
            {

            }

            return null;
        }

        public object GetSettingsView()
        {
            return null;
        }

        public void Dispose()
        {
            if (this.notifyIcon != null)
            {
                this.notifyIcon.Dispose();
                this.mp.Close();
            }
        }
    }
}
