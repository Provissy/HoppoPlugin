using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Grabacr07.KanColleViewer.ViewModels.Catalogs;
using Grabacr07.KanColleWrapper;
using Grabacr07.KanColleWrapper.Models;
using Grabacr07.KanColleViewer.Models;
using Livet.EventListeners;
using System.Threading;
using System.Timers;
using System.Windows.Threading;
using System.IO;
using System.Windows;
using System.Net;
using System.Text.RegularExpressions;
using System.ComponentModel.Composition;
using Grabacr07.KanColleViewer.Composition;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Management;
using MetroRadiance;
using Grabacr07.KanColleViewer;
using System.Reflection;

namespace HoppoPlugin
{
    public class MainViewViewModel : Grabacr07.KanColleViewer.ViewModels.TabItemViewModel
    {

        [DllImport("user32.dll", EntryPoint = "WindowFromPoint")]
        public static extern int WindowFromPoint(
            int xPoint,
            int yPoint
        );


        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetWindowRect(IntPtr hWnd, ref RECT lpRect);

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }


        #region Storie data.
        /// <summary>
        /// 全舰娘等级，一行20个。
        /// </summary>
        public static int[] ExpTable = new int[] { 0, 0, 100, 300, 600, 1000, 1500, 2100, 2800, 3600, 4500, 5500, 6600, 7800, 9100, 10500, 12000, 13600, 15300, 17100, 19000, 
			21000, 23100, 25300, 27600, 30000, 32500, 35100, 37800, 40600, 43500, 46500, 49600, 52800, 56100, 59500, 63000, 66600, 70300, 74100, 78000, 
			82000, 86100, 90300, 94600, 99000, 103500, 108100, 112800, 117600, 122500, 127500, 132700, 138100, 143700, 149500, 155500, 161700, 168100, 174700, 181500, 
			188500, 195800, 203400, 211300, 219500, 228000, 236800, 245900, 255300, 265000, 275000, 285400, 296200, 307400, 319000, 331000, 343400, 356200, 369400, 383000, 
			397000, 411500, 426500, 442000, 458000, 474500, 491500, 509000, 527000, 545500, 564500, 584500, 606500, 631500, 661500, 701500, 761500, 851500, 1000000, 1000000, 
			1010000, 1011000, 1013000, 1016000, 1020000, 1025000, 1031000, 1038000, 1046000, 1055000, 1065000, 1077000, 1091000, 1107000, 1125000, 1145000, 1168000, 1194000, 1223000, 1255000, 
			1290000, 1329000, 1372000, 1419000, 1470000, 1525000, 1584000, 1647000, 1714000, 1785000, 1860000, 1940000, 2025000, 2115000, 2210000, 2310000, 2415000, 2525000, 2640000, 2760000, 
			2887000, 3021000, 3162000, 3310000, 3465000, 3628000, 3799000, 3978000, 4165000, 4360000 };

        /// <summary>
        /// 海域EXP。
        /// </summary>
        public IEnumerable<string> SeaList { get; private set; }
        public static Dictionary<string, int> SeaExpTable = new Dictionary<string, int> 
		{
			{"1-1", 30}, {"1-2", 50}, {"1-3", 80}, {"1-4", 100}, {"1-5", 150},
			{"2-1", 120}, {"2-2", 150}, {"2-3", 200},{"2-4", 300},
			{"3-1", 310}, {"3-2", 320}, {"3-3", 330}, {"3-4", 350},
			{"4-1", 310}, {"4-2", 320}, {"4-3", 330}, {"4-4", 340},
			{"5-1", 360}, {"5-2", 380}, {"5-3", 400}, {"5-4", 420}, {"5-5", 450}
		};

        #endregion

        public IEnumerable<string> ResultList { get; private set; }
        public string[] Results = { "S", "A", "B", "C", "D", "E" };

        private readonly Subject<Unit> updateSource = new Subject<Unit>();
        private Homeport homeport = null;

        public ShipCatalogSortWorker SortWorker { get; private set; }

        private bool isInitialised = false;

        #region Ships 変更通知プロパティ

        private IReadOnlyCollection<ShipViewModel> _Ships;

        public IReadOnlyCollection<ShipViewModel> Ships
        {
            get { return this._Ships; }
            set
            {
                if (this._Ships != value)
                {
                    this._Ships = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        #endregion

        #region CurrentShip 変更通知プロパティ

        private Ship _CurrentShip;

        public Ship CurrentShip
        {
            get { return this._CurrentShip; }
            set
            {
                if (this._CurrentShip != value)
                {
                    this._CurrentShip = value;
                    if (value != null)
                    {
                        this.CurrentLevel = this.CurrentShip.Level;
                        this.TargetLevel = Math.Min(this.CurrentShip.Level + 1, 150);
                        this.CurrentExp = this.CurrentShip.Exp;
                        this.UpdateExpCalculator();
                        this.RaisePropertyChanged();
                    }
                }
            }
        }

        #endregion

        #region CurrentLevel 変更通知プロパティ

        private int _CurrentLevel;

        public int CurrentLevel
        {
            get { return this._CurrentLevel; }
            set
            {
                if (this._CurrentLevel != value && value >= 1 && value <= 150)
                {
                    this._CurrentLevel = value;
                    this.CurrentExp = ExpTable[value];
                    this.TargetLevel = Math.Max(this.TargetLevel, Math.Min(value + 1, 150));
                    this.RaisePropertyChanged();
                    this.UpdateExpCalculator();
                }
            }
        }

        #endregion

        #region TargetLevel 変更通知プロパティ

        private int _TargetLevel;

        public int TargetLevel
        {
            get { return this._TargetLevel; }
            set
            {
                if (this._TargetLevel != value && value >= 1 && value <= 150)
                {
                    this._TargetLevel = value;
                    this.TargetExp = ExpTable[value];
                    this.CurrentLevel = Math.Min(this.CurrentLevel, Math.Max(value - 1, 1));
                    this.RaisePropertyChanged();
                    this.UpdateExpCalculator();
                }
            }
        }

        #endregion

        #region SelectedSea 変更通知プロパティ

        private string _SelectedSea;

        public string SelectedSea
        {
            get { return this._SelectedSea; }
            set
            {
                if (_SelectedSea != value)
                {
                    this._SelectedSea = value;
                    this.RaisePropertyChanged();
                    this.UpdateExpCalculator();
                }
            }
        }

        #endregion

        #region SelectedResult 変更通知プロパティ

        private string _SelectedResult;

        public string SelectedResult
        {
            get { return this._SelectedResult; }
            set
            {
                if (this._SelectedResult != value)
                {
                    this._SelectedResult = value;
                    this.RaisePropertyChanged();
                    this.UpdateExpCalculator();
                }
            }
        }

        #endregion

        #region IsFlagship 変更通知プロパティ

        private bool _IsFlagship;

        public bool IsFlagship
        {
            get { return this._IsFlagship; }
            set
            {
                if (this._IsFlagship != value)
                {
                    this._IsFlagship = value;
                    this.RaisePropertyChanged();
                    this.UpdateExpCalculator();
                }
            }
        }

        #endregion

        #region IsMVP 変更通知プロパティ

        private bool _IsMVP;

        public bool IsMVP
        {
            get { return this._IsMVP; }
            set
            {
                if (this._IsMVP != value)
                {
                    this._IsMVP = value;
                    this.RaisePropertyChanged();
                    this.UpdateExpCalculator();
                }
            }
        }

        #endregion

        #region IsReloading 変更通知プロパティ

        private bool _IsReloading;

        public bool IsReloading
        {
            get { return this._IsReloading; }
            set
            {
                if (this._IsReloading != value)
                {
                    this._IsReloading = value;
                    this.RaisePropertyChanged();
                    this.UpdateExpCalculator();
                }
            }
        }

        #endregion

        #region CurrentExp 変更通知プロパティ

        private int _CurrentExp;

        public int CurrentExp
        {
            get { return this._CurrentExp; }
            private set
            {
                if (this._CurrentExp != value)
                {
                    this._CurrentExp = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        #endregion

        #region TargetExp 変更通知プロパティ

        private int _TargetExp;

        public int TargetExp
        {
            get { return this._TargetExp; }
            private set
            {
                if (this._TargetExp != value)
                {
                    this._TargetExp = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        #endregion

        #region SortieExp 変更通知プロパティ

        private int _SortieExp;

        public int SortieExp
        {
            get { return this._SortieExp; }
            private set
            {
                if (this._SortieExp != value)
                {
                    this._SortieExp = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        #endregion

        #region RemainingExp 変更通知プロパティ

        private int _RemainingExp;

        public int RemainingExp
        {
            get { return this._RemainingExp; }
            private set
            {
                if (this._RemainingExp != value)
                {
                    this._RemainingExp = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        #endregion

        #region RunCount 変更通知プロパティ

        private int _RunCount;

        public int RunCount
        {
            get { return this._RunCount; }
            private set
            {
                if (this._RunCount != value)
                {
                    this._RunCount = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        #endregion

        #region Initialised 変更通知プロパティ

        public bool Initialised
        {
            get { return this.isInitialised; }
            set
            {
                this.isInitialised = value;
                this.RaisePropertyChanged();
            }
        }

        #endregion

        #region CurrentEnableSoundNotify 変更通知プロパティ

        public bool CurrentEnableSoundNotify
        {
            get { return HoppoPluginSettings.Current.EnableSoundNotify; }
            set
            {
                if (HoppoPluginSettings.Current.EnableSoundNotify != value)
                {
                    HoppoPluginSettings.Current.EnableSoundNotify = value;
                    HoppoPluginSettings.Current.Save();
                    this.RaisePropertyChanged();
                }
            }
        }

        #endregion

        #region NekoDetector 変更通知プロパティ

        public bool _CurrentNekoDetector;

        public bool CurrentNekoDetector
        {
            get { return _CurrentNekoDetector; }
            set
            {
                if (_CurrentNekoDetector != value)
                {
                    nekoDetector(value);
                    _CurrentNekoDetector = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        #endregion

        #region CurrentIsDarkTheme 変更通知プロパティ

        public bool CurrentIsDarkTheme
        {
            get { return HoppoPluginSettings.Current.IsDarkTheme; }
            set
            {
                if (HoppoPluginSettings.Current.IsDarkTheme != value)
                {
                    if (value) ThemeService.Current.ChangeTheme(Theme.Dark);
                    HoppoPluginSettings.Current.IsDarkTheme = value;
                    HoppoPluginSettings.Current.Save();
                    this.RaisePropertyChanged();
                }
            }
        }

        #endregion

        #region CurrentIsLightTheme 変更通知プロパティ

        public bool CurrentIsLightTheme
        {
            get { return HoppoPluginSettings.Current.IsLightTheme; }
            set
            {
                if (HoppoPluginSettings.Current.IsLightTheme != value)
                {
                    if (value) ThemeService.Current.ChangeTheme(Theme.Light);
                    HoppoPluginSettings.Current.IsLightTheme = value;
                    HoppoPluginSettings.Current.Save();
                    this.RaisePropertyChanged();
                }
            }
        }

        #endregion

        #region CurrentVolume 変更通知プロパティ

        public double CurrentVolume
        {
            get { return HoppoPluginSettings.Current.Volume; }
            set
            {
                if (HoppoPluginSettings.Current.Volume != value)
                {
                    HoppoPluginSettings.Current.Volume = value;
                    HoppoPluginSettings.Current.Save();
                    this.RaisePropertyChanged();
                }
            }
        }

        #endregion

        #region CurrentEnableNotifyIcon 変更通知プロパティ

        public bool CurrentEnableNotifyIcon
        {
            get { return HoppoPluginSettings.Current.EnableNotifyIcon; }
            set
            {
                if (HoppoPluginSettings.Current.EnableNotifyIcon != value)
                {
                    HoppoPluginSettings.Current.EnableNotifyIcon = value;
                    HoppoPluginSettings.Current.Save();
                    this.RaisePropertyChanged();
                }
            }
        }

        #endregion

        public override string Name
        {
            get { return "HoppoPlugin"; }
            protected set { throw new NotImplementedException(); }
        }

        public Logger Logger { get; private set; }
        public Counter Counter { get; private set; }
        public BattleWatcher BattleWatcher { get; private set; }

        WebClient wClient = new WebClient();

        GameWatcher gameWatcher = new GameWatcher();
        
        DispatcherTimer timer = new DispatcherTimer();


        public MainViewViewModel()
        {
            this.CompositeDisposable.Add(new PropertyChangedEventListener(KanColleClient.Current)
			{
				{ () => KanColleClient.Current.IsStarted, (sender, args) => this.UpdateMode() },
			});

            this.SeaList = SeaExpTable.Keys.ToList();
            this.ResultList = Results.ToList();

            this.SortWorker = new ShipCatalogSortWorker();
            this.SortWorker.SetTarget(ShipCatalogSortTarget.Level, true);

            this.updateSource
                .Do(_ => this.IsReloading = true)
                .Throttle(TimeSpan.FromMilliseconds(7.0))
                .Do(_ => this.UpdateCore())
                .Subscribe(_ => this.IsReloading = false);
            this.CompositeDisposable.Add(this.updateSource);

            SelectedSea = SeaExpTable.Keys.FirstOrDefault();
            SelectedResult = Results.FirstOrDefault();
            this.Update();
            this.Logger = new Logger(KanColleClient.Current.Proxy);
            timer.Tick += new EventHandler(timer_Tick);
            timer.Interval = TimeSpan.FromSeconds(5);
            timer.Start();
        }

        //public TabItemViewModel

        void timer_Tick(object sender, EventArgs e)
        {
            timer.Stop();
            Grabacr07.KanColleViewer.App.Current.MainWindow.Closing += new System.ComponentModel.CancelEventHandler(kcvCanceling);
        }

        private void kcvCanceling(object o ,System.ComponentModel.CancelEventArgs e)
        {
            MessageBoxResult mbr = MessageBox.Show("确定要退出KCV么？", "HoppoPlugin提示", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if(mbr == MessageBoxResult.No)
            {
                e.Cancel = true;
            }
        }

        public void clearHoppoPlugin()
        {
            try
            {
                if (MessageBoxResult.No == MessageBox.Show("此操作会强制终止KCV，确定继续么？", "WARNING", MessageBoxButton.YesNo, MessageBoxImage.Warning))
                    return;
                MessageBox.Show("清空设置后后将会关闭KanColleViewer！");
                File.Delete(HoppoPluginSettings.HPSettingsPath);
                File.Delete(HoppoPluginSettings.UsageRecordPath);
                Process[] killprocess = Process.GetProcessesByName("KanColleViewer");
                foreach (System.Diagnostics.Process p in killprocess)
                {
                    p.Kill();
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("Error ! " + ex.ToString());
            }
           
        }

        public void Update()
        {
            this.RaisePropertyChanged("AllShipTypes");
            this.updateSource.OnNext(Unit.Default);
        }

        private void UpdateCore()
        {
            if (!this.Initialised)
            {
                return;
            }

            var list = this.homeport.Organization.Ships.Values
                .Where(x => x.Level != 1);

            this.Ships = this.SortWorker.Sort(list)
                .Select((x, i) => new ShipViewModel(i + 1, x)).ToList();
        }

        private void UpdateMode()
        {
            if (!this.Initialised)
            {
                this.Initialised = true;
                this.homeport = KanColleClient.Current.Homeport;
                this.CompositeDisposable.Add(new PropertyChangedEventListener(this.homeport.Organization)
				{
					{ () => this.homeport.Organization.Ships, (sender, args) => this.Update() },
				});
            }
        }

        public void UpdateExpCalculator()
        {
            if (this.TargetLevel < this.CurrentLevel || this.TargetExp < this.CurrentExp)
                return;

            double Multiplier = (this.IsFlagship ? 1.5 : 1) * (this.IsMVP ? 2 : 1) * (this.SelectedResult == "S" ? 1.2 : (this.SelectedResult == "C" ? 0.8 : (this.SelectedResult == "D" ? 0.7 : (this.SelectedResult == "E" ? 0.5 : 1))));

            this.SortieExp = (int)Math.Round(SeaExpTable[this.SelectedSea] * Multiplier);
            this.RemainingExp = this.TargetExp - this.CurrentExp;
            this.RunCount = (int)Math.Round(this.RemainingExp / (double)this.SortieExp);
        }

        private void nekoDetector(bool b)
        {
            IntPtr hWnd = GetForegroundWindow();
            RECT rect = new RECT();
            GetWindowRect(hWnd, ref rect);
            int width = rect.Right - rect.Left;
            int height = rect.Bottom - rect.Top;
            int x = rect.Left;
            int y = rect.Top;
            try
            {
                if (b)
                {
                    int a = Convert.ToInt32(WindowFromPoint(x + 100, y + 100));
                    GameWatcher.Instance.Start((IntPtr)a);
                }
                else
                {
                    GameWatcher.Instance.Stop();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR" + ex.ToString());
            }
        }


        #region MapInfoProxy変更通知プロパティ
        private MapInfoProxy _MapInfoProxy;

        public MapInfoProxy MapInfoProxy
        {
            get
            { return _MapInfoProxy; }
            set
            {
                if (_MapInfoProxy == value)
                    return;
                _MapInfoProxy = value;
                if (_MapInfoProxy != null)
                {
                    _MapInfoProxy.PropertyChanged += (sender, e) =>
                    {
                        if (e.PropertyName == "Maps")
                        {
                            RaisePropertyChanged(() => NextEventMapHp);
                            RaisePropertyChanged(() => RemainingCount);
                        }
                    };
                }
                RaisePropertyChanged();
            }
        }
        #endregion

        public string NextEventMapHp
        {
            get
            {
                if (MapInfoProxy.Maps == null) return "No Data";
                var map = MapInfoProxy.Maps.api_data.LastOrDefault(x => x.api_eventmap != null);
                if (map == null) return "No Map";
                return map.api_eventmap.api_now_maphp + "/" + map.api_eventmap.api_max_maphp;
            }
        }

        public string RemainingCount
        {
            get
            {
                var shipMaster = KanColleClient.Current.Master.Ships;
                if (MapInfoProxy == null || MapInfoProxy.Maps == null) return "No Data";
                var map = MapInfoProxy.Maps.api_data.LastOrDefault(x => x.api_eventmap != null);
                if (map == null) return "No Map";
                if (!MapInfo.EventBossDictionary.ContainsKey(map.api_id)) return "Unknow";
                var lastBossHp = shipMaster
                                .Single(x => x.Key == MapInfo.EventBossDictionary[map.api_id].Last())
                                .Value.HP;
                var normalBossHp = shipMaster
                                .Single(x => x.Key == MapInfo.EventBossDictionary[map.api_id].First())
                                .Value.HP;
                if (map.api_eventmap.api_now_maphp <= lastBossHp) return "1回";
                return (Math.Ceiling((double)(map.api_eventmap.api_now_maphp - lastBossHp) / normalBossHp) + 1) + "回";
            }
        }
    }
}
