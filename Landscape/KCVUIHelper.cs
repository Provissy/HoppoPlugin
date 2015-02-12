using Grabacr07.KanColleViewer.Views;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace HoppoPlugin.Landscape
{
    public static class KCVUIHelper
    {
        public static MainWindow KCVWindow { get; private set; }
        public static ContentControl KCVContent { get; internal set; }

        private static Task task;

        static KCVUIHelper()
        {
            Retry();
        }

        public static void Retry()
        {
            int retryCount = 0;
            CancellationTokenSource tokenSource = new CancellationTokenSource();
            task = Task.Run(async () =>
            {
                while (KCVWindow == null && retryCount < 120)
                {
                    await Task.Delay(500);
                    KCVWindow = MainWindow.Current;
                    retryCount++;
                }
                if (retryCount >= 120)
                {
                    tokenSource.Cancel();
                    PluginLoader.hasInitialized = false;
                }
            }, tokenSource.Token);
        }

        public static void OperateMainWindow(Action operation)
        {
            task = task.ContinueWith(t => 
            {
                if (!t.IsCanceled)
                    KCVWindow.Dispatcher.Invoke(operation);
            });
        }

        public static IEnumerable<T> FindVisualChildren<T>(this DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        yield return (T)child;
                    }

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }
    }
}
