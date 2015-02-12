using Grabacr07.KanColleViewer.ViewModels;
using Livet;

namespace HoppoPlugin
{
    class StatisticWindowViewModel : WindowViewModel
    {

        #region CurrentEnableNullDropLogging 変更通知プロパティ

        public bool CurrentEnableNullDropLogging
        {
            get { return HoppoPluginSettings.Current.EnableNullDropLogging; }
            set
            {
                if (HoppoPluginSettings.Current.EnableNullDropLogging != value)
                {
                    HoppoPluginSettings.Current.EnableNullDropLogging = value;
                    HoppoPluginSettings.Current.Save();
                    this.RaisePropertyChanged();
                }
            }
        }

        #endregion

    }
}
