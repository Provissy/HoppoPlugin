using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using Grabacr07.KanColleViewer.Composition;
using Grabacr07.KanColleWrapper;
using System.IO;
using System.Threading;
using System.Windows;

namespace HoppoPlugin
{
	[Export(typeof(IToolPlugin))]
    [ExportMetadata("Title", "HoppoPlugin")]
    [ExportMetadata("Description", "A plugin for KanColleViewer")]
	[ExportMetadata("Version", "5.0")]
	[ExportMetadata("Author", "@Provissy")]
	public class HoppoPluginLoader : IToolPlugin
	{
        public HoppoPluginLoader()
        {
            try
            {
                // Initialize HoppoPlugin ONCE.
                if (!UniversalConstants.Initialized)
                {
                    // Check update.
                    CommonHelper.UpdateChecker();

                    // Check version.
                    if (File.Exists(HoppoPluginSettings.UsageRecordPath))
                    {
                        StreamReader s = new StreamReader(HoppoPluginSettings.UsageRecordPath);
                        string recordedVersion = s.ReadLine();
                        s.Close();
                        if (recordedVersion == "6.0")
                        {
                            // Initialize HP Local Cache System.
                            initializeCacher();
                            // Load settings.
                            HoppoPluginSettings.Load();
                            // Start NAS.
                            NekoAvoidanceSystem.Startup();
                            // Create a instance of MainView.
                            mainView = new MainView { DataContext = new MainViewViewModel { MapInfoProxy = new MapInfoProxy() } };
                        }

                        // Version is out of date. Clear version record and lead user to welcome window.
                        else
                        {
                            File.Delete(HoppoPluginSettings.UsageRecordPath);
                            File.Delete(HoppoPluginSettings.HPSettingsPath);
                            Welcome w = new Welcome { DataContext = new HoppoPluginSettings() };
                            w.ShowDialog();
                        }
                    }
                    else
                    {
                        Welcome w = new Welcome { DataContext = new HoppoPluginSettings() };
                        w.ShowDialog();
                    }
                    UniversalConstants.Initialized = true;
                }

                // Instance was created. Do nothing.
                else
                { }
            }
            catch (Exception ex)
            {
                MessageBox.Show("初始化错误！将重置HoppoPlugin以尝试修复此问题。\n" + ex.ToString(), "警告", MessageBoxButton.OK, MessageBoxImage.Error);
                if (Directory.Exists(HoppoPluginSettings.KCVSettingsPath))
                    Directory.Delete(HoppoPluginSettings.KCVSettingsPath, true);
                if (Directory.Exists(UniversalConstants.CurrentVersion + @"\HoppoPlugin\"))
                    Directory.Delete(UniversalConstants.CurrentVersion + @"\HoppoPlugin\", true);
            }
        }

        private void initializeCacher()
        {
            CacherSettings.Load();
            // Print rules if file not exist.
            if (CacherSettings.Current.PrintGraphList &&
                    !File.Exists(CacherSettings.Current.CacheFolder + "\\GraphList.txt"))
            {
                GraphList.AppendRule();
            }
            FiddlerRules.Initialize();
        }

        ~HoppoPluginLoader()
        {
            CacherSettings.Save();
        }

        MainView mainView;
		public string ToolName
		{
            get { return "HoppoPlugin"; }
		}

		public object GetSettingsView()
		{
            return null;
		}

		public object GetToolView()
		{
            return mainView;
		}
	}


    // HoppoPlugin Notify Utility.
    [Export(typeof(INotifier))]
    [ExportMetadata("Title", "HPNotifier")]
    [ExportMetadata("Description", "Notifiy with Balloon, Sound, Popup")]
    [ExportMetadata("Version", "3.1")]
    [ExportMetadata("Author", "@?")]
    public class WindowsNotifier : INotifier
    {
        private readonly INotifier notifier;
        //private bool checker;
        
        public WindowsNotifier()
        {
            this.notifier =  new Windows7Notifier();
        }

        public void Dispose()
        {
            this.notifier.Dispose();
        }

        public void Initialize()
        {
            this.notifier.Initialize();
        }

        public void Show(NotifyType type, string header, string body, Action activated, Action<Exception> failed = null)
        {
            this.notifier.Show(type, header, body, activated, failed);
        }

        public object GetSettingsView()
        {
            return this.notifier.GetSettingsView();
        }

    }
}
