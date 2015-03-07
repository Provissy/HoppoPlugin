using Grabacr07.KanColleViewer.Composition;
using System.ComponentModel.Composition;
using Grabacr07.KanColleViewer;
using System.Windows;

namespace HoppoPlugin.Landscape
{
    [Export(typeof(IToolPlugin))]
    [ExportMetadata("Title", "KCV.Landscape")]
    [ExportMetadata("Description", "KanColleViewer布局切换插件。")]
    [ExportMetadata("Version", "1.2.1")]
    [ExportMetadata("Author", "@Gizeta")]
    public class PluginLoader : IToolPlugin
    {
        internal static bool hasInitialized = false;

        public PluginLoader()
        {
            if (App.ProductInfo.Version.Major > 3)
                return;
            if (App.ProductInfo.Version.Minor > 5)
                return;

            if(!hasInitialized)
            {
                hasInitialized = true;

                PluginSettings.Load();
                LandscapeExtention.Instance.Initialize();
            }
        }

        public string ToolName
        {
            get { return "Landscape"; }
        }

        public object GetSettingsView()
        {
            return null;
        }

        public object GetToolView()
        {
            return new LandscapeView { DataContext = LandscapeViewModel.Instance };
        }
    }
}
