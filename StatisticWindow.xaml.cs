using Livet;
using Livet.EventListeners;
using Livet.Messaging;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace HoppoPlugin
{
    /// <summary>
    /// Interaction logic for StatisticWindow.xaml
    /// </summary>
    public partial class StatisticWindow
    {
        public StatisticWindow()
        {
            InitializeComponent();
        }

        private DataTable loadAndDisplayCsvFile(string filePath)
        {
            string content = File.ReadAllText(filePath);
            List<List<string>> lists = new List<List<string>>();
            List<string> lastList = new List<string>();
            lists.Add(lastList); //1行目を追加
            lastList.Add("");
            Regex regex = new Regex(",|\\r?\\n|[^,\"\\r\\n][^,\\r\\n]*|\"(?:[^\"]|\"\")*\"");
            MatchCollection mc = regex.Matches(Regex.Replace(content, "\\r?\\n$", ""));
            foreach (Match m in mc)
            {
                switch (m.Value)
                {
                    case ",":
                        lastList.Add("");
                        break;
                    case "\n":
                    case "\r\n":
                        //改行コードの場合は行を追加する
                        lastList = new List<string>();
                        lists.Add(lastList);
                        lastList.Add("");
                        break;
                    default:
                        if (m.Value.StartsWith("\""))
                        {
                            //ダブルクォートで囲われている場合は最初と最後のダブルクォートを外し、
                            //文字列中のダブルクォートのエスケープをアンエスケープする
                            lastList[lastList.Count - 1] =
                                m.Value.Substring(1, m.Value.Length - 2).Replace("\"\"", "\"");
                        }
                        else
                        {
                            lastList[lastList.Count - 1] = m.Value;
                        }
                        break;
                }
            }

            // データテーブルにコピーする
            DataTable dt = new DataTable();
            for (int i = 0; i < lists[0].Count; i++)
            {
                dt.Columns.Add(lists[0][i]);
            }
            foreach (List<string> list in lists)
            {
                DataRow dr = dt.NewRow();
               
                for (int i = 0; i < list.Count; i++)
                {
                    dr[i] = list[i]; 
                }
                dt.Rows.Add(dr);
            }
            return dt;
        }

        private void TabControl_SelectionChanged(object sender, RoutedEventArgs e)
        {
            try
            {
                if (MainTabControl.SelectedIndex == 0)
                {
                    BuildDataGrid.ItemsSource = null;
                    BuildDataGrid.ItemsSource = loadAndDisplayCsvFile(UniversalConstants.CurrentDirectory + "ShipBuildLog.csv").DefaultView;
                }
                if (MainTabControl.SelectedIndex == 1)
                {
                    DevelopDataGrid.ItemsSource = null;
                    DevelopDataGrid.ItemsSource = loadAndDisplayCsvFile(UniversalConstants.CurrentDirectory + "ItemBuildLog.csv").DefaultView;
                }
                if (MainTabControl.SelectedIndex == 2)
                {
                    DropDataGrid.ItemsSource = null;
                    DropDataGrid.ItemsSource = loadAndDisplayCsvFile(UniversalConstants.CurrentDirectory + "DropLog.csv").DefaultView;
                }
                if (MainTabControl.SelectedIndex == 3)
                {
                    MessageBoxResult mbr = MessageBox.Show("这是一个垃圾功能，难看而且有大量bug，乃确定还要继续用嘛？", "WARNING", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (mbr == MessageBoxResult.No)
                        return;
                    ChartWindow c = new ChartWindow();
                    c.Show();
                }
            }
            catch (FileNotFoundException)
            {
                MessageBox.Show("没有对应的记录！");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void Window_CR(object sender, EventArgs e)
        {
            MainTabControl.AddHandler(TabControl.SelectionChangedEvent, new RoutedEventHandler(TabControl_SelectionChanged));
            try
            {
                BuildDataGrid.ItemsSource = null;
                BuildDataGrid.ItemsSource = loadAndDisplayCsvFile(UniversalConstants.CurrentDirectory + "ShipBuildLog.csv").DefaultView;
            }
            catch (FileNotFoundException)
            {
                MessageBox.Show("没有对应的记录！");
            }
            catch { }
        }
    }
}
