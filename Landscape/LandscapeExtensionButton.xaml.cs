using Grabacr07.KanColleViewer.Models;
using Grabacr07.KanColleViewer.ViewModels;
using System.Windows.Controls;
using KCVApp = Grabacr07.KanColleViewer.App;

namespace HoppoPlugin.Landscape
{
    public partial class LandscapeExtensionButton : UserControl
    {
        public LandscapeExtensionButton()
        {
            InitializeComponent();

            this.Resources.MergedDictionaries.Add(KCVApp.Current.Resources);
        }

        public void Refresh()
        {
            KCVApp.ViewModelRoot.Navigator.ReNavigate();
            PopupButton.IsChecked = false;
        }

        public void SwitchSound()
        {
            new VolumeViewModel().ToggleMute();
            PopupButton.IsChecked = false;
        }

        public void PrintScreen()
        {
            KCVApp.ViewModelRoot.TakeScreenshot();
            PopupButton.IsChecked = false;
        }

        public void SwitchProxy()
        {
            Grabacr07.KanColleViewer.Models. Settings.Current.EnableProxy = !Grabacr07.KanColleViewer.Models.Settings.Current.EnableProxy;
            PopupButton.IsChecked = false;
        }

        public void AdjustWindow()
        {
            LandscapeViewModel.Instance.AdjustWindow();
            PopupButton.IsChecked = false;
        }

        public void AdjustHost()
        {
            LandscapeViewModel.Instance.AdjustHost();
            PopupButton.IsChecked = false;
        }
    }
}
