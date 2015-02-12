using Grabacr07.KanColleViewer.ViewModels;
using Grabacr07.KanColleViewer.ViewModels.Contents;
using Grabacr07.KanColleViewer.ViewModels.Contents.Fleets;
using Grabacr07.KanColleViewer.Views;
using Grabacr07.KanColleViewer.Views.Settings;
using Grabacr07.KanColleWrapper;
using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using KCVApp = Grabacr07.KanColleViewer.App;

namespace HoppoPlugin.LoSCal
{
    public class CalcExtension
    {
        private static readonly CalcExtension instance = new CalcExtension();

        private ContentPresenter contentView;
        private ScrollViewer startSettingsView;
        private ContentControl mainSettingsView;
        private ContentPresenter statusBarView;

        private CalcExtension()
        {
        }

        public static CalcExtension Instance
        {
            get { return instance; }
        }
        
        public void Initialize()
        {
            KanColleClient.Current.PropertyChanged += CurrentKanColleClient_PropertyChanged;
            
            KCVUIHelper.OperateMainWindow(() =>
            {
                KCVUIHelper.KCVWindow.ContentRendered += KCVWindow_ContentRendered;

                KCVApp.ViewModelRoot.Settings.ToolPlugins = KCVApp.ViewModelRoot.Settings.ToolPlugins.Where(x => x.ToolName != "ViewRangeCalc").ToList();
            });
        }

        private void CurrentKanColleClient_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName=="IsStarted")
            {
                var proxy = KanColleClient.Current.Proxy;
                proxy.api_port.Subscribe(x => ViewRangeSelectorViewModel.Instance.CalcAllTotalViewRange());
                proxy.api_get_member_ship.Subscribe(x => ViewRangeSelectorViewModel.Instance.CalcAllTotalViewRange());
                proxy.api_get_member_ship2.Subscribe(x => ViewRangeSelectorViewModel.Instance.CalcAllTotalViewRange());
                proxy.api_get_member_ship3.Subscribe(x => ViewRangeSelectorViewModel.Instance.CalcAllTotalViewRange());
                proxy.api_get_member_deck.Subscribe(x => ViewRangeSelectorViewModel.Instance.CalcAllTotalViewRange());
                proxy.api_get_member_deck_port.Subscribe(x => ViewRangeSelectorViewModel.Instance.CalcAllTotalViewRange());
                proxy.api_req_hensei_change.Subscribe(x => ViewRangeSelectorViewModel.Instance.CalcAllTotalViewRange());
                proxy.api_req_hokyu_charge.Subscribe(x => ViewRangeSelectorViewModel.Instance.CalcAllTotalViewRange());

                ViewRangeSelectorViewModel.Instance.CalcAllTotalViewRange();
            }
        }

        private void KCVWindow_ContentRendered(object sender, EventArgs e)
        {
            KCVUIHelper.KCVWindow.ContentRendered -= KCVWindow_ContentRendered;

            KCVUIHelper.KCVContent = KCVUIHelper.KCVWindow.FindVisualChildren<ContentControl>().Where(x => x.Content is StartContentViewModel || x.Content is MainContentViewModel).First();

            KCVUIHelper.KCVContent.FindVisualChildren<RadioButton>().Where(x => x.Name == "SettingsTab").First().Checked += StartSettingsTab_Checked;
            KCVUIHelper.KCVContent.FindVisualChildren<ContentPresenter>().Where(x => x.DataContext is StartContentViewModel || x.DataContext is MainContentViewModel).First().DataContextChanged += ContentView_DataContextChanged;

            KCVUIHelper.KCVWindow.FindVisualChildren<StatusBar>().First().FindVisualChildren<ContentPresenter>().Last().DataContextChanged += StatusBar_DataContextChanged;
        }

        private void StatusBar_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var bar = sender as ContentPresenter;
            if (bar.Content is FleetsViewModel)
            {
                statusBarView = bar;
                bar.LayoutUpdated += StatusBar_LayoutUpdated;
            }
        }

        private void StatusBar_LayoutUpdated(object sender, EventArgs e)
        {
            statusBarView.LayoutUpdated -= StatusBar_LayoutUpdated;

            var textBlock = statusBarView.FindVisualChildren<TextBlock>()
                .Where(x => x.GetBindingExpression(TextBlock.TextProperty) == null ? false : x.GetBindingExpression(TextBlock.TextProperty).ParentBinding.Path.Path == "TotalViewRange")
                .First();
            
            var losBinding = new Binding();
            losBinding.Path = new PropertyPath("TotalViewRange");
            losBinding.Converter = new FleetToViewRangeConverter();
            textBlock.SetBinding(TextBlock.TextProperty, losBinding);

            statusBarView = null;
        }

        private void ContentView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            contentView = sender as ContentPresenter;
            contentView.LayoutUpdated += ContentView_LayoutUpdated;
        }

        private void ContentView_LayoutUpdated(object sender, EventArgs e)
        {
            contentView.LayoutUpdated -= ContentView_LayoutUpdated;
            contentView = null;

            setContentListener();
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
                toolsViewModel.Tools = toolsViewModel.Tools.Where(x => x.ToolName != "ViewRangeCalc").ToList();
            }
        }

        private void StartSettingsTab_Checked(object sender, RoutedEventArgs e)
        {
            var radioButton = sender as RadioButton;
            if (radioButton.IsChecked == true)
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

            var settingsTab = (from view in KCVUIHelper.KCVContent.FindVisualChildren<TabControl>()
                               where view.DataContext is SettingsViewModel
                               select view).First();

            settingsTab.MouseMove += settingsTab_MouseMove;
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

            var settingsTab = (from view in KCVUIHelper.KCVContent.FindVisualChildren<TabControl>()
                               where view.DataContext is SettingsViewModel
                               select view).First();

            settingsTab.MouseMove += settingsTab_MouseMove;
        }

        private void settingsTab_MouseMove(object sender, MouseEventArgs e)
        {
            var tab = sender as TabControl;

            if (tab.SelectedContent is Operation)
            {
                tab.MouseMove -= settingsTab_MouseMove;

                var panel = tab.FindVisualChildren<RadioButton>().First().Parent as StackPanel;
                var panelParent = panel.Parent as StackPanel;
                panelParent.Children.Remove(panel);
                panelParent.Children.Add(new ViewRangeSelector { DataContext = ViewRangeSelectorViewModel.Instance });
            }
        }
    }
}
