using Grabacr07.KanColleViewer;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace HoppoPlugin
{
    class GameWatcher
    {

        public static GameWatcher Instance { get; set; }

        const int WM_LBUTTONDOWN = 0x201;
        const int WM_LBUTTONUP = 0x202;

        [DllImport("user32.dll")]
        static extern bool SetCursorPos(int X, int Y);
        [DllImport("user32.dll")]
        static extern void mouse_event(MouseEventFlag flags, int dx, int dy, uint data, UIntPtr extraInfo);
        [Flags]
        enum MouseEventFlag : uint
        {
            Move = 0x0001,
            LeftDown = 0x0002,
            LeftUp = 0x0004,
            RightDown = 0x0008,
            RightUp = 0x0010,
            MiddleDown = 0x0020,
            MiddleUp = 0x0040,
            XDown = 0x0080,
            XUp = 0x0100,
            Wheel = 0x0800,
            VirtualDesk = 0x4000,
            Absolute = 0x8000
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetWindowRect(IntPtr hWnd, ref RECT lpRect);

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left; 
            public int Top; 
            public int Right; 
            public int Bottom; 
        }


        private bool _isEnabled = false;
        public bool isEnabled { get { return _isEnabled; } set { _isEnabled = value; if (value) { timer.Start(); } } }
        public IntPtr HWND { get; set; }

        #region Get bitmap of web browser

        /// <summary>  
        /// 全屏截图   
        /// </summary>  
        /// <returns></returns>  
        public System.Drawing.Image CaptureScreen()
        {
            return CaptureWindow(User32.GetDesktopWindow());
        }
        /// <summary>  
        /// 指定窗口截图  
        /// </summary>  
        /// <param name="handle">窗口句柄. (在windows应用程序中, 从Handle属性获得)</param>  
        /// <returns></returns>  
        public System.Drawing.Image CaptureWindow(IntPtr handle)
        {
            IntPtr hdcSrc = User32.GetWindowDC(handle);
            User32.RECT windowRect = new User32.RECT();
            User32.GetWindowRect(handle, ref windowRect);
            int width = windowRect.right - windowRect.left;
            int height = windowRect.bottom - windowRect.top;
            IntPtr hdcDest = GDI32.CreateCompatibleDC(hdcSrc);
            IntPtr hBitmap = GDI32.CreateCompatibleBitmap(hdcSrc, width, height);
            IntPtr hOld = GDI32.SelectObject(hdcDest, hBitmap);
            GDI32.BitBlt(hdcDest, 0, 0, width, height, hdcSrc, 0, 0, GDI32.SRCCOPY);
            GDI32.SelectObject(hdcDest, hOld);
            GDI32.DeleteDC(hdcDest);
            User32.ReleaseDC(handle, hdcSrc);
            System.Drawing.Image img = System.Drawing.Image.FromHbitmap(hBitmap);
            GDI32.DeleteObject(hBitmap);
            return img;
        }
        /// <summary>  
        /// 指定窗口截图 保存为图片文件  
        /// </summary>  
        /// <param name="handle"></param>  
        /// <param name="filename"></param>  
        /// <param name="format"></param>  
        public int[] CaptureWindowToFile(IntPtr handle) ////////////////////////////////////////////////
        {
            System.Drawing.Image img = CaptureWindow(handle);
            //img.Save(filename, format);
            return GetHisogram((Bitmap)img);
        }

        /// <summary>  
        /// 辅助类 定义 Gdi32 API 函数  
        /// </summary>  
        private class GDI32
        {

            public const int SRCCOPY = 0x00CC0020;
            [DllImport("gdi32.dll")]
            public static extern bool BitBlt(IntPtr hObject, int nXDest, int nYDest,
                int nWidth, int nHeight, IntPtr hObjectSource,
                int nXSrc, int nYSrc, int dwRop);
            [DllImport("gdi32.dll")]
            public static extern IntPtr CreateCompatibleBitmap(IntPtr hDC, int nWidth,
                int nHeight);
            [DllImport("gdi32.dll")]
            public static extern IntPtr CreateCompatibleDC(IntPtr hDC);
            [DllImport("gdi32.dll")]
            public static extern bool DeleteDC(IntPtr hDC);
            [DllImport("gdi32.dll")]
            public static extern bool DeleteObject(IntPtr hObject);
            [DllImport("gdi32.dll")]
            public static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObject);
        }

        /// <summary>  
        /// 辅助类 定义User32 API函数  
        /// </summary>  
        private class User32
        {
            [StructLayout(LayoutKind.Sequential)]
            public struct RECT
            {
                public int left;
                public int top;
                public int right;
                public int bottom;
            }
            [DllImport("user32.dll")]
            public static extern IntPtr GetDesktopWindow();
            [DllImport("user32.dll")]
            public static extern IntPtr GetWindowDC(IntPtr hWnd);
            [DllImport("user32.dll")]
            public static extern IntPtr ReleaseDC(IntPtr hWnd, IntPtr hDC);
            [DllImport("user32.dll")]
            public static extern IntPtr GetWindowRect(IntPtr hWnd, ref RECT rect);
        }

        //private System.Timers.Timer timer = new System.Timers.Timer(30000);

        #endregion


        DispatcherTimer timer = new DispatcherTimer();
        DispatcherTimer timer2 = new DispatcherTimer();

        public GameWatcher()
        {
            Instance = this;
        }

        public void Start (IntPtr hWnd)
        {
            HWND = hWnd;
            timer.Tick += new EventHandler(timer_Tick);
            timer.Interval = TimeSpan.FromSeconds(5);
            timer2.Tick += new EventHandler(timer2_Tick);
            timer2.Interval = TimeSpan.FromSeconds(60);
            isEnabled = true;
        }

        public void Stop()
        {
            isEnabled = false;
            timer.Stop();
            timer2.Stop();
            HWND = IntPtr.Zero;
        }

        //uint MAKELONG(ushort x, ushort y)
        //{
        //    return ((((uint)x) << 16) | y); 
        //}

        Bitmap b = new Bitmap(UniversalConstants.CurrentDirectory + @"\HoppoPlugin\nekoError.png");

        void timer_Tick(object sender, EventArgs e)
        {
            if (isEnabled)
            {
                
                int[] i = CaptureWindowToFile(HWND);
                int[] n = GetHisogram(b);
                float f = GetResult(i, n);
                if (f >= 0.55)
                {
                    App.ViewModelRoot.Navigator.ReNavigate();
                    timer2.Start();
                }
            }
            else
            {
                timer.Stop();
            }
        }

        void timer2_Tick(object sender, EventArgs e)
        {
            IntPtr awin = HWND; 
            RECT rect = new RECT();
            GetWindowRect(awin, ref rect);
            int width = rect.Right - rect.Left; 
            int height = rect.Bottom - rect.Top; 
            int x = rect.Left;
            int y = rect.Top;

            // ROLL !

            SetCursorPos(x + 623, y + 406);
            mouse_event(MouseEventFlag.LeftDown, 0, 0, 0, UIntPtr.Zero);
            Thread.Sleep(100);
            mouse_event(MouseEventFlag.LeftDown, 0, 0, 0, UIntPtr.Zero);
            Thread.Sleep(100);
            mouse_event(MouseEventFlag.LeftDown, 0, 0, 0, UIntPtr.Zero);
            Thread.Sleep(100);
            mouse_event(MouseEventFlag.LeftDown, 0, 0, 0, UIntPtr.Zero);
            Thread.Sleep(100);
            mouse_event(MouseEventFlag.LeftDown, 0, 0, 0, UIntPtr.Zero);
            Thread.Sleep(100);


            timer2.Stop();
            timer.Start();
        }



        public int[] GetHisogram(Bitmap img)
        {
            BitmapData data = img.LockBits(new System.Drawing.Rectangle(0, 0, img.Width, img.Height), ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            int[] histogram = new int[256];
            unsafe
            {
                byte* ptr = (byte*)data.Scan0;
                int remain = data.Stride - data.Width * 3;
                for (int i = 0; i < histogram.Length; i++)
                    histogram[i] = 0;
                for (int i = 0; i < data.Height; i++)
                {
                    for (int j = 0; j < data.Width; j++)
                    {
                        int mean = ptr[0] + ptr[1] + ptr[2];
                        mean /= 3;
                        histogram[mean]++;
                        ptr += 3;
                    }
                    ptr += remain;
                }
            }
            img.UnlockBits(data);
            return histogram;
        }

        private float GetAbs(int firstNum, int secondNum)
        {
            float abs = Math.Abs((float)firstNum - (float)secondNum);
            float result = Math.Max(firstNum, secondNum);
            if (result == 0)
                result = 1;
            return abs / result;
        }

        public float GetResult(int[] firstNum, int[] scondNum)
        {
            if (firstNum.Length != scondNum.Length)
            {
                return 0;
            }
            else
            {
                float result = 0;
                int j = firstNum.Length;
                for (int i = 0; i < j; i++)
                {
                    result += 1 - GetAbs(firstNum[i], scondNum[i]);
                    Console.WriteLine(i + "----" + result);
                }
                return result / j;
            }
        }      
    }
}
