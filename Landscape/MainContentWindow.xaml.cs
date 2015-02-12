using Grabacr07.KanColleViewer.Views;
using System.ComponentModel;
using System.Windows.Controls;

namespace HoppoPlugin.Landscape
{
    public partial class MainContentWindow
    {
        public static MainContentWindow Current { get; private set; }
        
        public MainContentWindow()
        {
            InitializeComponent();

            Current = this;
            MainWindow.Current.Closed += (sender, args) => this.Close();

            var content = KCVUIHelper.KCVContent;
            (this.Content as Grid).Children.Add(content);
            Grid.SetRow(content, 1);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);

            (this.Content as Grid).Children.Remove(KCVUIHelper.KCVContent);

            if (PluginSettings.Current.Layout == KCVContentLayout.Separate)
                LandscapeViewModel.Instance.IsWindowOpenButtonShow = true;
            Current = null;
        }
    }
}
