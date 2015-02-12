using Grabacr07.KanColleViewer.Models;
using Grabacr07.KanColleViewer.Models.Data.Xml;
using MetroRadiance;
using System;
using System.IO;
using Livet.Messaging;
using Livet;
using Livet.Commands;
using Livet.EventListeners;
using Livet.Behaviors;
using Livet.Converters;
using System.Windows.Forms;
using System.Diagnostics;

namespace HoppoPlugin
{
    [Serializable]

    public class HoppoPluginSettings : NotificationObject
    {
        public static readonly string HPSettingsPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "grabacr.net",
            "KanColleViewer",
            "HoppoPluginSettings.xml");

        public static readonly string UsageRecordPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "grabacr.net",
        "KanColleViewer",
        "HoppoPluginUsageRecord");

        public static HoppoPluginSettings Current { get; set; }

        public static void Load()
        {
            try
            {
                // Load settings to instance from XML file.
                Current = HPSettingsPath.ReadXml<HoppoPluginSettings>();
            }
            catch
            {
                Current = new HoppoPluginSettings();
            }
        }

        public void Save()
        {
            try
            {
                // Save settings to XML file.
                this.WriteXml(HPSettingsPath);
            }
            catch { }
        }

        public void FirstTimeSave()
        {
            try
            {
                StreamWriter s = new StreamWriter(HoppoPluginSettings.UsageRecordPath);
                s.WriteLine("5.0");
                s.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR" + ex.ToString());
            }
            this.WriteXml(HPSettingsPath);
            MessageBox.Show("即将关闭KanColleViewer！\n请重新启动KanColleViewer！\n在“Sounds”文件夹内可设置声音文件");
            Process[] killprocess = Process.GetProcessesByName("KanColleViewer");
            foreach (System.Diagnostics.Process p in killprocess)
            {
                p.Kill();
            }
        }

        // Settings detail.

        #region EnableSoundNotify 変更通知プロパティ

        private bool _EnableSoundNotify = false;

        public bool EnableSoundNotify
        {
            get { return this._EnableSoundNotify; }
            set
            {
                if (this._EnableSoundNotify != value)
                {
                    this._EnableSoundNotify = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        #endregion

        #region IsDarkTheme 変更通知プロパティ

        private bool _IsDarkTheme = true;

        public bool IsDarkTheme
        {
            get { return this._IsDarkTheme; }
            set
            {
                if (this._IsDarkTheme != value)
                {
                    this._IsDarkTheme = value;
                    if (value) ThemeService.Current.ChangeTheme(Theme.Dark);
                    this.RaisePropertyChanged();
                }
            }
        }

        #endregion

        #region IsLightTheme 変更通知プロパティ

        private bool _IsLightTheme;
        public bool IsLightTheme
        {
            get { return this._IsLightTheme; }
            set
            {
                if (this._IsLightTheme != value)
                {
                    this._IsLightTheme = value;
                    if (value) ThemeService.Current.ChangeTheme(Theme.Light);
                    this.RaisePropertyChanged();
                }
            }
        }

        #endregion

        #region EnableNotifyIcon 変更通知プロパティ

        private bool _EnableNotifyIcon = true;

        public bool EnableNotifyIcon
        {
            get { return this._EnableNotifyIcon; }
            set
            {
                if (this._EnableNotifyIcon != value)
                {
                    this._EnableNotifyIcon = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        #endregion

        #region CurrentSettingsVersion 変更通知プロパティ

        private string _CurrentSettingsVersion = "3.1.4";

        public string CurrentSettingsVersion
        {
            get { return this._CurrentSettingsVersion; }
            set
            {
                if (this._CurrentSettingsVersion != value)
                {
                    this._CurrentSettingsVersion = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        #endregion

        #region EnableNullDropLogging 変更通知プロパティ

        private bool _EnableNullDropLogging = false;

        public bool EnableNullDropLogging
        {
            get { return this._EnableNullDropLogging; }
            set
            {
                if (this._EnableNullDropLogging != value)
                {
                    this._EnableNullDropLogging = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        #endregion

        #region UsernameOfPT 変更通知プロパティ

        private string _UsernameOfPT = "";

        public string UsernameOfPT
        {
            get { return this._UsernameOfPT; }
            set
            {
                if (this._UsernameOfPT != value)
                {
                    this._UsernameOfPT = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        #endregion

        #region Volume 変更通知プロパティ

        private double _Volume = 100;

        public double Volume
        {
            get { return this._Volume; }
            set
            {
                if (this._Volume != value)
                {
                    this._Volume = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        #endregion
    }
}