using Grabacr07.KanColleViewer.ViewModels;
using Grabacr07.KanColleViewer.ViewModels.Contents;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using KCVApp = Grabacr07.KanColleViewer.App;

namespace HoppoPlugin.Landscape
{
    public enum KCVContentLayout
    {
        Portrait,
        LandscapeLeft,
        LandscapeRight,
        Separate
    }

    public class LandscapeExtention
    {
        private static readonly LandscapeExtention instance = new LandscapeExtention();

        private ContentPresenter contentView;
        private ScrollViewer startSettingsView;
        private ContentControl mainSettingsView;

        private LandscapeExtention()
        {
        }

        public static LandscapeExtention Instance
        {
            get { return instance; }
        }

        public void Initialize()
        {
            KCVUIHelper.OperateMainWindow(() =>
            {
                KCVUIHelper.KCVWindow.ContentRendered += KCVWindow_ContentRendered;

                KCVApp.ViewModelRoot.Settings.ToolPlugins = KCVApp.ViewModelRoot.Settings.ToolPlugins.Where(x => x.ToolName != "Landscape").ToList();
            });
        }

        private void setContentListener()
        {
            if (KCVApp.ViewModelRoot.Content is StartContentViewModel)
            {
                KCVUIHelper.KCVContent.FindVisualChildren<RadioButton>().Where(x => x.Name == "SettingsTab").First().Checked += StartSettingsTab_Checked;
            }
            else
            {
                KCVUIHelper.KCVContent.FindVisualChildren<ListBoxItem>().Where(x => x.DataContext is SettingsViewModel).First().Selected += MainSettingsTab_Selected;

                var toolsViewModel = (KCVApp.ViewModelRoot.Content as MainContentViewModel).TabItems.Where(x => x is ToolsViewModel).First() as ToolsViewModel;
                toolsViewModel.Tools = toolsViewModel.Tools.Where(x => x.ToolName != "Landscape").ToList();

                LandscapeHacker.Instance.Hack();
            }
        }

        private void insertSettings()
        {
            var landscapeTab = new TabItem();
            var tabHeader = new TextBlock { Text = "Landscape" };
            tabHeader.SetResourceReference(Control.StyleProperty, "TabHeaderTextStyleKey");
            landscapeTab.Header = tabHeader;
            landscapeTab.Content = new LandscapeView
            {
                DataContext = LandscapeViewModel.Instance,
                Margin = KCVApp.ViewModelRoot.Content is StartContentViewModel ? new Thickness(10, 9, 10, 9) : new Thickness(10, 0, 10, 0)
            };

            var settingsTab = (from view in KCVUIHelper.KCVContent.FindVisualChildren<TabControl>()
                               where view.DataContext is SettingsViewModel
                               select view).First();
            settingsTab.Items.Add(landscapeTab);
        }

        private void ContentView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            contentView = sender as ContentPresenter;
            if (PluginSettings.Current.Layout == KCVContentLayout.Separate && !MainContentWindow.Current.IsLoaded)
            {
                MainContentWindow.Current.ContentRendered += MainContentWindow_ContentRendered;
            }
            else
            {
                contentView.LayoutUpdated += ContentView_LayoutUpdated;
            }
        }

        private void MainContentWindow_ContentRendered(object sender, EventArgs e)
        {
            MainContentWindow.Current.ContentRendered -= MainContentWindow_ContentRendered;

            setContentListener();
        }

        private void ContentView_LayoutUpdated(object sender, EventArgs e)
        {
            contentView.LayoutUpdated -= ContentView_LayoutUpdated;

            setContentListener();
        }

        private void StartSettingsTab_Checked(object sender, RoutedEventArgs e)
        {
            var radioButton = sender as RadioButton;
            if(radioButton.IsChecked == true)
            {
                radioButton.Checked -= StartSettingsTab_Checked;
                startSettingsView = KCVUIHelper.KCVContent.FindVisualChildren<ScrollViewer>().Where(x => x.DataContext is SettingsViewModel).First();
                startSettingsView.LayoutUpdated += StartSettingsView_LayoutUpdated;
            }
        }

        private void StartSettingsView_LayoutUpdated(object sender, EventArgs e)
        {
            startSettingsView.LayoutUpdated -= StartSettingsView_LayoutUpdated;
            startSettingsView = null;
            KCVUIHelper.OperateMainWindow(() => insertSettings());
        }

        private void MainSettingsTab_Selected(object sender, RoutedEventArgs e)
        {
            var listBoxItem = sender as ListBoxItem;
            listBoxItem.Selected -= MainSettingsTab_Selected;
            mainSettingsView = KCVUIHelper.KCVContent.FindVisualChildren<ContentControl>().Where(x => x.DataContext is SettingsViewModel).Last();
            mainSettingsView.LayoutUpdated += MainSettingsView_LayoutUpdated;
        }

        private void MainSettingsView_LayoutUpdated(object sender, EventArgs e)
        {
            mainSettingsView.LayoutUpdated -= MainSettingsView_LayoutUpdated;
            mainSettingsView = null;
            KCVUIHelper.OperateMainWindow(() => insertSettings());
        }

        private void KCVWindow_ContentRendered(object sender, EventArgs e)
        {
            KCVUIHelper.KCVWindow.ContentRendered -= KCVWindow_ContentRendered;

            KCVUIHelper.KCVContent = KCVUIHelper.KCVWindow.FindVisualChildren<ContentControl>().Where(x => x.Content is StartContentViewModel || x.Content is MainContentViewModel).First();

            KCVUIHelper.KCVContent.FindVisualChildren<RadioButton>().Where(x => x.Name == "SettingsTab").First().Checked += StartSettingsTab_Checked;
            KCVUIHelper.KCVContent.FindVisualChildren<ContentPresenter>().Where(x => x.DataContext is StartContentViewModel || x.DataContext is MainContentViewModel).First().DataContextChanged += ContentView_DataContextChanged;

            KCVUIHelper.OperateMainWindow(async () =>
            {
                await Task.Delay(5000);
                LandscapeViewModel.Instance.Initialize();
            });
        }
    }
}
