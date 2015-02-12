using Grabacr07.KanColleViewer.ViewModels;
using System;
using System.Windows.Data;
using KCVApp = Grabacr07.KanColleViewer.App;

namespace HoppoPlugin.LoSCal
{
    public class FleetToViewRangeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var fleet = (KCVApp.ViewModelRoot.Content as MainContentViewModel).Fleets.SelectedFleet;
            if (fleet != null)
            {
                var calcEx = ViewRangeSelectorViewModel.Instance;
                if (calcEx.ViewRangeType1 || calcEx.ViewRangeType2)
                {
                    return string.Format("{0}", fleet.GetViewRange());
                }
                if (calcEx.ViewRangeType3 || calcEx.ViewRangeType4)
                {
                    return string.Format("{0:0.##}", fleet.GetExactViewRange());
                }
            }
            return "0";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
