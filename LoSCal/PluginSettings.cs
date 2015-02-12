using Grabacr07.KanColleViewer.Models.Data.Xml;
using System;
using System.IO;

namespace HoppoPlugin.LoSCal
{
    [Serializable]
    public class PluginSettings
    {
        private static readonly string filePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "grabacr.net",
            "KanColleViewer",
            "KCV.ViewRangeCalc.xml");

        public static PluginSettings Current { get; set; }

        public static void Load()
        {
            try
            {
                Current = filePath.ReadXml<PluginSettings>();
            }
            catch
            {
                Current = GetInitialSettings();
            }
        }

        public void Save()
        {
            try
            {
                this.WriteXml(filePath);
            }
            catch { }
        }

        public static PluginSettings GetInitialSettings()
        {
            return new PluginSettings
            {
                ViewRangeType1 = false,
                ViewRangeType2 = true,
                ViewRangeType3 = false,
                ViewRangeType4 = false
            };
        }

        public bool ViewRangeType1 { get; set; }
        public bool ViewRangeType2 { get; set; }
        public bool ViewRangeType3 { get; set; }
        public bool ViewRangeType4 { get; set; }
    }
}
