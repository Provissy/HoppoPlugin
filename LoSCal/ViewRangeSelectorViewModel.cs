using Grabacr07.KanColleWrapper;
using Grabacr07.KanColleWrapper.Models;
using Livet;
using System;
using System.Linq;

namespace HoppoPlugin.LoSCal
{
    public class ViewRangeSelectorViewModel : ViewModel
    {
        private static readonly ViewRangeSelectorViewModel instance = new ViewRangeSelectorViewModel();

        private ViewRangeSelectorViewModel()
        {
        }

        public static ViewRangeSelectorViewModel Instance
        {
            get { return instance; }
        }

        public bool ViewRangeType1
        {
            get { return PluginSettings.Current.ViewRangeType1; }
            set
            {
                if (PluginSettings.Current.ViewRangeType1 != value)
                {
                    PluginSettings.Current.ViewRangeType1 = value;
                    PluginSettings.Current.Save();

                    this.RaisePropertyChanged();

                    if (!value)
                        this.CalcAllTotalViewRange();
                }
            }
        }

        public bool ViewRangeType2
        {
            get { return PluginSettings.Current.ViewRangeType2; }
            set
            {
                if (PluginSettings.Current.ViewRangeType2 != value)
                {
                    PluginSettings.Current.ViewRangeType2 = value;
                    PluginSettings.Current.Save();

                    this.RaisePropertyChanged();

                    if (!value)
                        this.CalcAllTotalViewRange();
                }
            }
        }

        public bool ViewRangeType3
        {
            get { return PluginSettings.Current.ViewRangeType3; }
            set
            {
                if (PluginSettings.Current.ViewRangeType3 != value)
                {
                    PluginSettings.Current.ViewRangeType3 = value;
                    PluginSettings.Current.Save();

                    this.RaisePropertyChanged();

                    if (!value)
                        this.CalcAllTotalViewRange();
                }
            }
        }

        public bool ViewRangeType4
        {
            get { return PluginSettings.Current.ViewRangeType4; }
            set
            {
                if (PluginSettings.Current.ViewRangeType4 != value)
                {
                    PluginSettings.Current.ViewRangeType4 = value;
                    PluginSettings.Current.Save();

                    this.RaisePropertyChanged();

                    if (!value)
                        this.CalcAllTotalViewRange();
                }
            }
        }

        public void CalcAllTotalViewRange()
        {
            foreach (var fleet in KanColleClient.Current.Homeport.Organization.Fleets)
            {
                var los = this.CalcTotalViewRange(fleet.Value);
                fleet.Value.SetTotalViewRange((int)los);
            }
        }

        public double CalcTotalViewRange(Fleet fleet)
        {
            if (fleet == null || fleet.Ships.Length == 0) return 0;

            if (ViewRangeType1)
            {
                return fleet.Ships.Sum(x => x.ViewRange);
            }

            if (ViewRangeType2)
            {
                var spotter = fleet.Ships.SelectMany(
                    x => x.SlotItems
                        .Zip(x.OnSlot, (i, o) => new { Item = i.Info, Slot = o })
                        .Where(a => a.Item.GetRawData().api_type[1] == 7)
                        .Where(a => a.Slot > 0)
                        .Select(a => a.Item.GetRawData().api_saku)
                ).Sum();

                var radar = fleet.Ships.SelectMany(
                    x => x.SlotItems
                        .Where(i => i.Info.GetRawData().api_type[1] == 8)
                        .Select(i => i.Info.GetRawData().api_saku)
                ).Sum();

                return (spotter * 2) + radar + (int)Math.Sqrt(fleet.Ships.Sum(x => x.ViewRange) - spotter - radar);
            }

            if (ViewRangeType3)
            {
                var result = 0.0;

                var jb = fleet.Ships.SelectMany(
                    x => x.SlotItems
                        .Zip(x.OnSlot, (i, o) => new { Item = i.Info, Slot = o })
                        .Where(a => a.Item.GetRawData().api_type[2] == 7)
                        .Where(a => a.Slot > 0)
                        .Select(a => a.Item.GetRawData().api_saku)
                 ).Sum();
                result += jb * 1.04;

                var jg = fleet.Ships.SelectMany(
                    x => x.SlotItems
                        .Zip(x.OnSlot, (i, o) => new { Item = i.Info, Slot = o })
                        .Where(a => a.Item.GetRawData().api_type[2] == 8)
                        .Where(a => a.Slot > 0)
                        .Select(a => a.Item.GetRawData().api_saku)
                 ).Sum();
                result += jg * 1.37;

                var jz = fleet.Ships.SelectMany(
                    x => x.SlotItems
                        .Zip(x.OnSlot, (i, o) => new { Item = i.Info, Slot = o })
                        .Where(a => a.Item.GetRawData().api_type[2] == 9)
                        .Where(a => a.Slot > 0)
                        .Select(a => a.Item.GetRawData().api_saku)
                 ).Sum();
                result += jz * 1.66;

                var sz = fleet.Ships.SelectMany(
                    x => x.SlotItems
                        .Zip(x.OnSlot, (i, o) => new { Item = i.Info, Slot = o })
                        .Where(a => a.Item.GetRawData().api_type[2] == 10)
                        .Where(a => a.Slot > 0)
                        .Select(a => a.Item.GetRawData().api_saku)
                 ).Sum();
                result += sz * 2.0;

                var sb = fleet.Ships.SelectMany(
                    x => x.SlotItems
                        .Zip(x.OnSlot, (i, o) => new { Item = i.Info, Slot = o })
                        .Where(a => a.Item.GetRawData().api_type[2] == 11)
                        .Where(a => a.Slot > 0)
                        .Select(a => a.Item.GetRawData().api_saku)
                 ).Sum();
                result += sb * 1.78;

                var xdt = fleet.Ships.SelectMany(
                    x => x.SlotItems
                        .Where(i => i.Info.GetRawData().api_type[2] == 12)
                        .Select(i => i.Info.GetRawData().api_saku)
                ).Sum();
                result += xdt * 1.0;

                var ddt = fleet.Ships.SelectMany(
                    x => x.SlotItems
                        .Where(i => i.Info.GetRawData().api_type[2] == 13)
                        .Select(i => i.Info.GetRawData().api_saku)
                ).Sum();
                result += ddt * 0.99;

                var tzd = fleet.Ships.SelectMany(
                    x => x.SlotItems
                        .Where(i => i.Info.GetRawData().api_type[2] == 29)
                        .Select(i => i.Info.GetRawData().api_saku)
                ).Sum();
                result += tzd * 0.91;

                result += fleet.Ships.Sum(f => {
                    var spotter = f.SlotItems
                        .Zip(f.OnSlot, (i, o) => new { Item = i.Info, Slot = o })
                        .Where(a => a.Item.GetRawData().api_type[2] >= 7 && a.Item.GetRawData().api_type[2] <= 11)
                        .Where(a => a.Slot > 0)
                        .Select(a => a.Item.GetRawData().api_saku)
                        .Sum();

                    var radar = f.SlotItems
                        .Where(i => i.Info.GetRawData().api_type[1] == 8)
                        .Select(i => i.Info.GetRawData().api_saku)
                        .Sum();

                    return Math.Sqrt(f.ViewRange - spotter - radar);
                }) * 1.69 - Math.Ceiling(KanColleClient.Current.Homeport.Admiral.Level / 5.0) * 5.0 * 0.61;

                return result;
            }

            if (ViewRangeType4)
            {
                var result = 0.0;

                var jb = fleet.Ships.SelectMany(
                    x => x.SlotItems
                        .Zip(x.OnSlot, (i, o) => new { Item = i.Info, Slot = o })
                        .Where(a => a.Item.GetRawData().api_type[2] == 7)
                        .Where(a => a.Slot > 0)
                        .Select(a => a.Item.GetRawData().api_saku)
                 ).Sum();
                result += jb * 0.6;

                var jg = fleet.Ships.SelectMany(
                    x => x.SlotItems
                        .Zip(x.OnSlot, (i, o) => new { Item = i.Info, Slot = o })
                        .Where(a => a.Item.GetRawData().api_type[2] == 8)
                        .Where(a => a.Slot > 0)
                        .Select(a => a.Item.GetRawData().api_saku)
                 ).Sum();
                result += jg * 0.8;

                var jz = fleet.Ships.SelectMany(
                    x => x.SlotItems
                        .Zip(x.OnSlot, (i, o) => new { Item = i.Info, Slot = o })
                        .Where(a => a.Item.GetRawData().api_type[2] == 9)
                        .Where(a => a.Slot > 0)
                        .Select(a => a.Item.GetRawData().api_saku)
                 ).Sum();
                result += jz * 1.0;

                var sz = fleet.Ships.SelectMany(
                    x => x.SlotItems
                        .Zip(x.OnSlot, (i, o) => new { Item = i.Info, Slot = o })
                        .Where(a => a.Item.GetRawData().api_type[2] == 10)
                        .Where(a => a.Slot > 0)
                        .Select(a => a.Item.GetRawData().api_saku)
                 ).Sum();
                result += sz * 1.2;

                var sb = fleet.Ships.SelectMany(
                    x => x.SlotItems
                        .Zip(x.OnSlot, (i, o) => new { Item = i.Info, Slot = o })
                        .Where(a => a.Item.GetRawData().api_type[2] == 11)
                        .Where(a => a.Slot > 0)
                        .Select(a => a.Item.GetRawData().api_saku)
                 ).Sum();
                result += sb * 1.1;

                var xdt = fleet.Ships.SelectMany(
                    x => x.SlotItems
                        .Where(i => i.Info.GetRawData().api_type[2] == 12)
                        .Select(i => i.Info.GetRawData().api_saku)
                ).Sum();
                result += xdt * 0.6;

                var ddt = fleet.Ships.SelectMany(
                    x => x.SlotItems
                        .Where(i => i.Info.GetRawData().api_type[2] == 13)
                        .Select(i => i.Info.GetRawData().api_saku)
                ).Sum();
                result += ddt * 0.6;

                var tzd = fleet.Ships.SelectMany(
                    x => x.SlotItems
                        .Where(i => i.Info.GetRawData().api_type[2] == 29)
                        .Select(i => i.Info.GetRawData().api_saku)
                ).Sum();
                result += tzd * 0.5;

                result += fleet.Ships.Sum(f =>
                {
                    var spotter = f.SlotItems
                        .Zip(f.OnSlot, (i, o) => new { Item = i.Info, Slot = o })
                        .Where(a => a.Item.GetRawData().api_type[2] >= 7 && a.Item.GetRawData().api_type[2] <= 11)
                        .Where(a => a.Slot > 0)
                        .Select(a => a.Item.GetRawData().api_saku)
                        .Sum();

                    var radar = f.SlotItems
                        .Where(i => i.Info.GetRawData().api_type[1] == 8)
                        .Select(i => i.Info.GetRawData().api_saku)
                        .Sum();

                    return Math.Sqrt(f.ViewRange - spotter - radar);
                }) - KanColleClient.Current.Homeport.Admiral.Level * 0.4;

                return result;
            }

            return 0;
        }
    }
}
