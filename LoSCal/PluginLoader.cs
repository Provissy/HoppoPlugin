using Grabacr07.KanColleViewer;
using Grabacr07.KanColleViewer.Composition;
using System.ComponentModel.Composition;

namespace HoppoPlugin.LoSCal
{
    [Export(typeof(IToolPlugin))]
    [ExportMetadata("Title", "KCV.ViewRangeCalc")]
    [ExportMetadata("Description", "KanColleViewer索敌值计算插件。")]
    [ExportMetadata("Version", "1.1.1")]
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

            if (!hasInitialized)
            {
                hasInitialized = true;

                PluginSettings.Load();
                CalcExtension.Instance.Initialize();
            }
        }

        public string ToolName
        {
            get { return "ViewRangeCalc"; }
        }

        public object GetSettingsView()
        {
            return null;
        }

        public object GetToolView()
        {
            return new ViewRangeSelector { DataContext = ViewRangeSelectorViewModel.Instance };
        }
    }
}
