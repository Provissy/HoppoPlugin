using Grabacr07.KanColleViewer.Models;
using Grabacr07.KanColleViewer.Models.Data.Xml;
using System;
using System.IO;

namespace HoppoPlugin.Landscape
{
    [Serializable]
    public class PluginSettings
    {
        private static readonly string filePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "grabacr.net",
            "KanColleViewer",
            "KCV.Landscape.xml");

        public static PluginSettings Current { get; set; }

        public static void Load()
        {
            try
            {
                Current = filePath.ReadXml<PluginSettings>();
            }
            catch(Exception ex)
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
            catch (Exception ex) { }
        }

        public static PluginSettings GetInitialSettings()
        {
            return new PluginSettings
            {
                Layout = KCVContentLayout.Portrait,
                BrowserZoomFactor = Grabacr07.KanColleViewer.Models.Settings.Current.BrowserZoomFactorPercentage,
                InsertScrollBarToPluginTab = false,
                AddExtensionButtonToCaptionBar = false
            };
        }

        public KCVContentLayout Layout { get; set; }

        public int BrowserZoomFactor { get; set; }

        public bool InsertScrollBarToPluginTab { get; set; }

        public bool AddExtensionButtonToCaptionBar { get; set; }
    }
}
