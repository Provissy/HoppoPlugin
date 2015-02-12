using Grabacr07.Desktop.Metro.Controls;
using Grabacr07.Desktop.Metro.Converters;
using Grabacr07.KanColleViewer.Models;
using Grabacr07.KanColleViewer.ViewModels;
using Grabacr07.KanColleViewer.Views.Controls;
using Livet;
using MetroRadiance.Controls;
using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using KCVApp = Grabacr07.KanColleViewer.App;

namespace HoppoPlugin.Landscape
{
    public class LandscapeViewModel : NotificationObject
    {
        private readonly static LandscapeViewModel instance = new LandscapeViewModel();

        private CallMethodButton windowOpenButton;
        private Grid contentContainer;
        private KanColleHost hostControl;
        private FrameworkElement pluginControl;

        private LandscapeViewModel()
        {
            CacherSettings.Current.PropertyChanged += Settings_PropertyChanged;
        }

        public static LandscapeViewModel Instance
        {
            get { return instance; }
        }

        public void Initialize()
        {
            createButton();

            hostControl = KCVUIHelper.KCVWindow.FindVisualChildren<KanColleHost>().First();            
            contentContainer = hostControl.Parent as Grid;
            pluginControl = KCVUIHelper.KCVContent;

            hostControl.ZoomFactor = this.BrowserZoomFactor / 100.0;
            switchLayout(CurrentLayout);
        }

        public KCVContentLayout CurrentLayout
        {
            get { return PluginSettings.Current.Layout; }
            set
            {
                if (PluginSettings.Current.Layout != value)
                {
                    switchLayout(PluginSettings.Current.Layout, value);

                    PluginSettings.Current.Layout = value;
                    PluginSettings.Current.Save();

                    this.RaisePropertyChanged();
                }
            }
        }

        public int BrowserZoomFactor
        {
            get { return PluginSettings.Current.BrowserZoomFactor; }
            set
            {
                if (PluginSettings.Current.BrowserZoomFactor != value)
                {
                    PluginSettings.Current.BrowserZoomFactor = value;
                    PluginSettings.Current.Save();

                    hostControl.ZoomFactor = value / 100.0;

                    this.RaisePropertyChanged();
                }
            }
        }

        public bool InsertScrollBarToPluginTab
        {
            get { return PluginSettings.Current.InsertScrollBarToPluginTab; }
            set
            {
                if (PluginSettings.Current.InsertScrollBarToPluginTab != value)
                {
                    PluginSettings.Current.InsertScrollBarToPluginTab = value;
                    PluginSettings.Current.Save();

                    this.RaisePropertyChanged();
                }
            }
        }

        public bool AddExtensionButtonToCaptionBar
        {
            get { return PluginSettings.Current.AddExtensionButtonToCaptionBar; }
            set
            {
                if (PluginSettings.Current.AddExtensionButtonToCaptionBar != value)
                {
                    PluginSettings.Current.AddExtensionButtonToCaptionBar = value;
                    PluginSettings.Current.Save();

                    this.RaisePropertyChanged();
                }
            }
        }

        public void AdjustWindow()
        {
            if (CurrentLayout == KCVContentLayout.Separate)
            {
                resizeWindow(this.HostWidth, this.HostHeight);
            }
        }

        public void AdjustHost()
        {
            if (CurrentLayout == KCVContentLayout.Separate)
            {
                this.BrowserZoomFactor = (int)Math.Floor(100.0 * Math.Min(KCVUIHelper.KCVWindow.ActualWidth / 800, (KCVUIHelper.KCVWindow.ActualHeight - 59) / 480));
            }
        }

        public void OpenWindow()
        {
            if (CurrentLayout == KCVContentLayout.Separate && MainContentWindow.Current == null)
            {
                var window = new MainContentWindow { DataContext = KCVApp.ViewModelRoot };
                window.Show();
                this.IsWindowOpenButtonShow = false;
            }
        }

        private bool isWindowOpenButtonShow = false;
        public bool IsWindowOpenButtonShow
        {
            get { return isWindowOpenButtonShow; }
            internal set
            {
                if (isWindowOpenButtonShow != value)
                {
                    isWindowOpenButtonShow = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        public double HostWidth
        {
            get { return hostControl == null ? double.NaN : hostControl.WebBrowser.ActualWidth; }
        }

        public double HostHeight
        {
            get { return hostControl == null ? double.NaN : hostControl.WebBrowser.ActualHeight; }
        }

        private void switchLayout(KCVContentLayout newValue)
        {
            switchLayout(KCVContentLayout.Portrait, newValue, true);
        }

        private void switchLayout(KCVContentLayout oldValue, KCVContentLayout newValue, bool initializing = false)
        {
            if (oldValue == newValue) return;

            if (oldValue == KCVContentLayout.Separate)
            {
                if (newValue == KCVContentLayout.Portrait)
                {
                    resizeWindow(Math.Max(this.HostWidth, MainContentWindow.Current.ActualWidth), this.HostHeight + MainContentWindow.Current.ActualHeight);
                }
                else
                {
                    resizeWindow(this.HostWidth + MainContentWindow.Current.ActualWidth, Math.Max(this.HostHeight, MainContentWindow.Current.ActualHeight));
                }
                MainContentWindow.Current.Close();
                contentContainer.Children.Add(pluginControl);
                this.IsWindowOpenButtonShow = false;
            }

            contentContainer.RowDefinitions.Clear();
            contentContainer.ColumnDefinitions.Clear();

            if (newValue == KCVContentLayout.Separate)
            {
                contentContainer.Children.Remove(pluginControl);
                var window = new MainContentWindow { DataContext = KCVApp.ViewModelRoot };
                window.Show();

                resizeWindow(this.HostWidth, this.HostHeight);
            }
            else
            {
                if (newValue == KCVContentLayout.Portrait)
                {
                    if (oldValue != KCVContentLayout.Separate)
                    {
                        resizeWindow(Math.Max(this.HostWidth, pluginControl.ActualWidth), this.HostHeight + pluginControl.ActualHeight);
                    }

                    contentContainer.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Auto) });
                    contentContainer.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });

                    Grid.SetRow(pluginControl, 1);
                }
                else
                {
                    if (newValue == KCVContentLayout.LandscapeLeft)
                    {
                        if (oldValue == KCVContentLayout.Portrait && !initializing)
                        {
                            resizeWindow(this.HostWidth + pluginControl.ActualWidth, Math.Max(this.HostHeight, pluginControl.ActualHeight));
                        }

                        contentContainer.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Auto) });
                        contentContainer.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });

                        Grid.SetColumn(hostControl, 0);
                        Grid.SetColumn(pluginControl, 1);
                    }
                    else
                    {
                        if (oldValue == KCVContentLayout.Portrait && !initializing)
                        {
                            resizeWindow(this.HostWidth + pluginControl.ActualWidth, Math.Max(this.HostHeight, pluginControl.ActualHeight));
                        }

                        contentContainer.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
                        contentContainer.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Auto) });

                        Grid.SetColumn(hostControl, 1);
                        Grid.SetColumn(pluginControl, 0);
                    }
                }
            }
        }

        private void resizeWindow(double width, double height)
        {
            height += 59; // 标题栏及状态栏高度

            if (width > SystemParameters.WorkArea.Width)
            {
                KCVUIHelper.KCVWindow.Width = SystemParameters.WorkArea.Width;
                KCVUIHelper.KCVWindow.Left = 0;
            }
            else
            {
                KCVUIHelper.KCVWindow.Width = width;
            }

            if(height > SystemParameters.WorkArea.Height)
            {
                KCVUIHelper.KCVWindow.Height = SystemParameters.WorkArea.Height;
                KCVUIHelper.KCVWindow.Top = 0;
            }
            else
            {
                KCVUIHelper.KCVWindow.Height = height;
            }
        }

        private void createButton()
        {
            var resDict = new ResourceDictionary();
            resDict.Source = new Uri("pack://application:,,,/HoppoPlugin;component/Landscape/Style.Controls.WindowOpenButton.xaml");
            KCVUIHelper.KCVWindow.Resources.MergedDictionaries.Add(resDict);

            var topButtonContainer = KCVUIHelper.KCVWindow.FindVisualChildren<ZoomFactorSelector>().First().Parent as StackPanel;
            windowOpenButton = new CallMethodButton();
            windowOpenButton.ToolTip = "恢复关闭的窗口";
            windowOpenButton.SetResourceReference(Control.StyleProperty, "WindowOpenButtonStyleKey");
            windowOpenButton.MethodName = "OpenWindow";
            windowOpenButton.MethodTarget = this;

            var buttonBinding = new Binding();
            buttonBinding.Source = this;
            buttonBinding.Path = new PropertyPath("IsWindowOpenButtonShow");
            buttonBinding.Mode = BindingMode.TwoWay;
            buttonBinding.Converter = new UniversalBooleanToVisibilityConverter();
            windowOpenButton.SetBinding(CaptionButton.VisibilityProperty, buttonBinding);
            topButtonContainer.Children.Insert(0, windowOpenButton);
        }

        private void Settings_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName == "BrowserZoomFactor")
            {
                this.BrowserZoomFactor = Grabacr07.KanColleViewer.Models.Settings.Current.BrowserZoomFactorPercentage;
            }
        }
    }
}
