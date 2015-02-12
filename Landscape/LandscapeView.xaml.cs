using System.Windows.Controls;
using KCVApp = Grabacr07.KanColleViewer.App;

namespace HoppoPlugin.Landscape
{
    /// <summary>
    /// LandscaperView.xaml 的交互逻辑
    /// </summary>
    public partial class LandscapeView : UserControl
    {
        public LandscapeView()
        {
            InitializeComponent();

            this.Resources.MergedDictionaries.Add(KCVApp.Current.Resources);
        }
    }
}
