using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.DataVisualization.Charting;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace HoppoPlugin
{
    /// <summary>
    /// Interaction logic for Window2.xaml
    /// </summary>
    public partial class ChartWindow : Window
    {
        public ChartWindow()
        {
            InitializeComponent();
            LineChart1.Title = "正在生成统计图，请稍等。。。";
            Thread t = new Thread(initializeChart);
            t.Start();
        }

        private void initializeChart()
        {
            try
            {
                loadMatChart();
            }
            catch (Exception ex)
            {
                MessageBox.Show("加载统计图错误！ " + ex.ToString());
                //MainView m = new MainView();
                //m.ErrorHandler("加载统计图错误！ " + ex.ToString());
            }
        }



        private void loadMatChart()
        {
            Action a = new Action(() => {
                List<string[]> loadedList = ReadCSV(UniversalConstants.CurrentDirectory + "MaterialsLog.csv");
                LineSeries fuelLine = LineChart1.Series[0] as LineSeries;
                fuelLine.ItemsSource = loadFuel(loadedList);
                LineSeries ammoLine = LineChart1.Series[1] as LineSeries;
                ammoLine.ItemsSource = loadAmmo(loadedList);
                LineSeries steelLine = LineChart1.Series[2] as LineSeries;
                steelLine.ItemsSource = loadSteel(loadedList);
                LineSeries bauxiteLine = LineChart1.Series[3] as LineSeries;
                bauxiteLine.ItemsSource = loadBauxite(loadedList);
            LineChart1.Title = "资源统计图";
            });
            this.Dispatcher.Invoke(a, DispatcherPriority.ApplicationIdle);
        }

        private List<MatData> loadBauxite(List<string[]> loadMat)
        {
            List<MatData> matdata = new List<MatData>();
            int lenth = loadMat.Count;
            int quarter = lenth / 4;
            //foreach (string[] ss in loadMat)
            for (int i = 0; i < loadMat.Count; i++)
            {
                if (i == 0)
                {
                    matdata.Add(new MatData(loadMat[i][0], Int32.Parse(loadMat[i][4])));
                }
                else
                {
                    if (quarter == i || quarter * 2 == i || quarter * 3 == i || quarter * 4 == i)
                    {
                        matdata.Add(new MatData(loadMat[i][0], Int32.Parse(loadMat[i][4])));
                    }
                    else
                    {
                        matdata.Add(new MatData(loadMat[i][0], Int32.Parse(loadMat[i][4])));
                        //if(ssCount%2 == 0)
                        //{
                        //    matdata.Add(new MatData(" ", Int32.Parse(ss[3])));
                        //}
                        //else
                        //{
                        //    matdata.Add(new MatData("　", Int32.Parse(ss[3])));
                        //}  
                    }
                }
            }
            return matdata;
        }

        private List<MatData> loadSteel(List<string[]> loadMat)
        {
            List<MatData> matdata = new List<MatData>();
            int lenth = loadMat.Count;
            int quarter = lenth / 4;
            //foreach (string[] ss in loadMat)
            for (int i = 0; i < loadMat.Count; i++ )
            {
                if (i == 0)
                {
                    matdata.Add(new MatData(loadMat[i][0], Int32.Parse(loadMat[i][3])));
                }
                else
                {
                    if (quarter == i || quarter * 2 == i || quarter * 3 == i || quarter * 4 == i)
                    {
                        matdata.Add(new MatData(loadMat[i][0], Int32.Parse(loadMat[i][3])));
                    }
                    else
                    {
                        matdata.Add(new MatData(loadMat[i][0], Int32.Parse(loadMat[i][3])));
                        //if(ssCount%2 == 0)
                        //{
                        //    matdata.Add(new MatData(" ", Int32.Parse(ss[3])));
                        //}
                        //else
                        //{
                        //    matdata.Add(new MatData("　", Int32.Parse(ss[3])));
                        //}  
                    }
                }
            }
            return matdata;
        }

        private List<MatData> loadAmmo(List<string[]> loadMat)
        {
            List<MatData> matdata = new List<MatData>();
            int lenth = loadMat.Count;
            int quarter = lenth / 4;
            //foreach (string[] ss in loadMat)
            for (int i = 0; i < loadMat.Count; i++)
            {
                if (i == 0)
                {
                    matdata.Add(new MatData(loadMat[i][0], Int32.Parse(loadMat[i][2])));
                }
                else
                {
                    if (quarter == i || quarter * 2 == i || quarter * 3 == i || quarter * 4 == i)
                    {
                        matdata.Add(new MatData(loadMat[i][0], Int32.Parse(loadMat[i][2])));
                    }
                    else
                    {
                        matdata.Add(new MatData(loadMat[i][0], Int32.Parse(loadMat[i][2])));
                        //if(ssCount%2 == 0)
                        //{
                        //    matdata.Add(new MatData(" ", Int32.Parse(ss[3])));
                        //}
                        //else
                        //{
                        //    matdata.Add(new MatData("　", Int32.Parse(ss[3])));
                        //}  
                    }
                }
            }
            return matdata;
        }

        private List<MatData> loadFuel(List<string[]> loadMat)
        {
            List<MatData> matdata = new List<MatData>();
            int lenth = loadMat.Count;
            int quarter = lenth / 4;
            //foreach (string[] ss in loadMat)
            for (int i = 0; i < loadMat.Count; i++)
            {
                if (i == 0)
                {
                    matdata.Add(new MatData(loadMat[i][0], Int32.Parse(loadMat[i][1])));
                }
                else
                {
                    if (quarter == i || quarter * 2 == i || quarter * 3 == i || quarter * 4 == i)
                    {
                        matdata.Add(new MatData(loadMat[i][0], Int32.Parse(loadMat[i][1])));
                    }
                    else
                    {
                        matdata.Add(new MatData(loadMat[i][0], Int32.Parse(loadMat[i][1])));
                        //if(ssCount%2 == 0)
                        //{
                        //    matdata.Add(new MatData(" ", Int32.Parse(ss[3])));
                        //}
                        //else
                        //{
                        //    matdata.Add(new MatData("　", Int32.Parse(ss[3])));
                        //}  
                    }
                }
            }
            return matdata;
        }



        /// <summary>
        /// CSV File Reader.
        /// Read a csv file to List<string[]> .
        /// </summary>
        /// <param name="filePathName"></param>
        /// <returns></returns>
        public static List<String[]> ReadCSV(string filePathName )
        {
            List<String[]> ls = new List<String[]>();
            StreamReader fileReader = new StreamReader(filePathName);
            string strLine = "";
            fileReader.ReadLine();   //Skip first row.
            while (strLine != null)
            {
                strLine = fileReader.ReadLine();
                if (strLine != null && strLine.Length > 0)
                {
                    ls.Add(strLine.Split(','));
                    //Debug.WriteLine(strLine);
                }
            }
            fileReader.Close();
            return ls;
        }


    }


    public class MatData
    {
        public string DateOF { get; set; }
        public int countOfMat { get; set; }

        public MatData(string dateof, int countofmat)
        {
            DateOF = dateof;
            countOfMat = countofmat;
        }
    }

    //public class LineSeriesEx : LineSeries
    //{
    //    protected override DataPoint CreateDataPoint()
    //    {
    //        return new EmptyDataPoint();
    //    }
    //}

    //public class EmptyDataPoint : DataPoint
    //{
    //    // As the method name says, this DataPoint is empty.
    //}
}
