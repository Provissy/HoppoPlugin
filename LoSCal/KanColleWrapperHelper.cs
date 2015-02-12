using Grabacr07.KanColleViewer.ViewModels.Contents.Fleets;
using Grabacr07.KanColleWrapper.Models;
using System;
using System.Reflection;

namespace HoppoPlugin.LoSCal
{
    public static class RawDataWrapperHelper
    {
        public static T GetRawData<T>(this RawDataWrapper<T> source) where T : class
        {
            Type type = typeof(RawDataWrapper<T>);
            return type.GetProperty("RawData", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).GetValue(source) as T;
        }
    }

    public static class FleetHelper
    {
        public static void SetTotalViewRange(this Fleet fleet, int value)
        {
            Type type = typeof(Fleet);
            type.GetProperty("TotalViewRange", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).SetValue(fleet, value);
        }
    }

    public static class FleetViewModelHelper
    {
        public static int GetViewRange(this FleetViewModel fleetViewModel)
        {
            Type type = typeof(FleetViewModel);
            var fleet = type.GetField("source", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).GetValue(fleetViewModel) as Fleet;
            return fleet.TotalViewRange;
        }
        
        public static double GetExactViewRange(this FleetViewModel fleetViewModel)
        {
            Type type = typeof(FleetViewModel);
            var fleet = type.GetField("source", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).GetValue(fleetViewModel) as Fleet;
            return ViewRangeSelectorViewModel.Instance.CalcTotalViewRange(fleet);
        }
    }
}
