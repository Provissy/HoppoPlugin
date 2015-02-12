using Grabacr07.Desktop.Metro.Controls;
using Grabacr07.KanColleViewer.ViewModels.Contents;
using Grabacr07.KanColleViewer.Views.Controls;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace HoppoPlugin.Landscape
{
    public class LandscapeHacker
    {
        private static readonly LandscapeHacker instance = new LandscapeHacker();

        private ContentControl mainToolsView;

        private LandscapeHacker()
        {
        }

        public static LandscapeHacker Instance
        {
            get { return instance; }
        }
        
        public void Hack()
        {
            var resDict = new ResourceDictionary();
            resDict.Source = new Uri("pack://application:,,,/HoppoPlugin;component/Landscape/LandscapeHacker.Resource.xaml");
            KCVUIHelper.KCVWindow.Resources.MergedDictionaries.Add(resDict);

            if (PluginSettings.Current.InsertScrollBarToPluginTab)
            {
                KCVUIHelper.KCVContent.FindVisualChildren<ListBoxItem>().Where(x => x.DataContext is ToolsViewModel).First().Selected += MainToolsTab_Selected;
            }
            if (PluginSettings.Current.AddExtensionButtonToCaptionBar)
            {
                addExtensionButtonToCaptionBar();
            }
        }

        private void insertScrollBarToPluginTab()
        {
            var listBox = KCVUIHelper.KCVContent.FindVisualChildren<ListBox>().Where(x => x.DataContext is ToolsViewModel).First();
            listBox.Template = KCVUIHelper.KCVWindow.Resources["LandscapePluginTabListBoxKey"] as ControlTemplate;
            ScrollViewer.SetCanContentScroll(listBox, false);
        }

        private void addExtensionButtonToCaptionBar()
        {
            var topButtonContainer = KCVUIHelper.KCVWindow.FindVisualChildren<ZoomFactorSelector>().First().Parent as StackPanel;
            var extensionButton = new LandscapeExtensionButton();
            topButtonContainer.Children.Insert(0, extensionButton);
        }

        private void MainToolsTab_Selected(object sender, RoutedEventArgs e)
        {
            var listBoxItem = sender as ListBoxItem;
            listBoxItem.Selected -= MainToolsTab_Selected;
            mainToolsView = KCVUIHelper.KCVContent.FindVisualChildren<ContentControl>().Where(x => x.DataContext is ToolsViewModel).Last();
            mainToolsView.LayoutUpdated += MainToolsView_LayoutUpdated;
        }

        private void MainToolsView_LayoutUpdated(object sender, EventArgs e)
        {
            mainToolsView.LayoutUpdated -= MainToolsView_LayoutUpdated;
            mainToolsView = null;
            KCVUIHelper.OperateMainWindow(() => insertScrollBarToPluginTab());
        }
    }
}
