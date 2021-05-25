using Ark.Models;
using Ark.Models.Hotkeys;
using Ark.Models.SongLibrary;
using Ark.ViewModels;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace Ark.Views
{
    public partial class MainWindow : Window
    {
        private MainWindowViewModel _viewModel;

        private Hotkey song, bible, message, blankDisplayWindow, historyWindow;
        public object Controls { get; private set; }

        private UserControl SongLibrary, Bible, TheTable;

        public MainWindow()
        {
            _viewModel = new MainWindowViewModel();
            DataContext = _viewModel;

            InitializeComponent();

            SourceInitialized += (s, e) =>
            {
                IntPtr handle = (new WindowInteropHelper(this)).Handle;
                HwndSource.FromHwnd(handle).AddHook(new HwndSourceHook(WindowProc));
            };

            SongLibrary = new SongLibrary();
            Bible = new Bible();
            TheTable = new TheTable();

            //Initialize with SongLibrary UserControl
            SongLibrary_RadioButton.IsChecked = true;

            MinimizeButton.Click += (s, e) => WindowState = WindowState.Minimized;
            MaximizeButton.Click += (s, e) => WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
            CloseButton.Click += (s, e) => Close();

            Ark.Models.History.HistoryEventI += HistoryEvent;

            MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
            MaxWidth = SystemParameters.MaximizedPrimaryScreenWidth;

            HistoryWindow.Instance.Show();
            HistoryWindow.Instance.Close();
        }

        private void HistoryEvent(object sender, object e)
        {
            this.Focus();

            if (e is BibleData)
            {
                Bible_RadioButton.IsChecked = true;
                Ark.Models.History.Instance.HistoryChangeII(e);
            }
            else if(e is SongData)
            {
                SongLibrary_RadioButton.IsChecked = true;
                Ark.Models.History.Instance.HistoryChangeII(e);
            }
        }

        #region Window Buttons
        private static IntPtr WindowProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case 0x0024:
                    WmGetMinMaxInfo(hwnd, lParam);
                    handled = true;
                    break;
            }
            return (IntPtr)0;
        }

        private static void WmGetMinMaxInfo(IntPtr hwnd, IntPtr lParam)
        {
            MINMAXINFO mmi = (MINMAXINFO)Marshal.PtrToStructure(lParam, typeof(MINMAXINFO));
            int MONITOR_DEFAULTTONEAREST = 0x00000002;
            IntPtr monitor = MonitorFromWindow(hwnd, MONITOR_DEFAULTTONEAREST);
            if (monitor != IntPtr.Zero)
            {
                MONITORINFO monitorInfo = new MONITORINFO();
                GetMonitorInfo(monitor, monitorInfo);
                RECT rcWorkArea = monitorInfo.rcWork;
                RECT rcMonitorArea = monitorInfo.rcMonitor;
                mmi.ptMaxPosition.x = Math.Abs(rcWorkArea.left - rcMonitorArea.left);
                mmi.ptMaxPosition.y = Math.Abs(rcWorkArea.top - rcMonitorArea.top);
                mmi.ptMaxSize.x = Math.Abs(rcWorkArea.right - rcWorkArea.left);
                mmi.ptMaxSize.y = Math.Abs(rcWorkArea.bottom - rcWorkArea.top);
            }
            Marshal.StructureToPtr(mmi, lParam, true);
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            /// <summary>x coordinate of point.</summary>
            public int x;
            /// <summary>y coordinate of point.</summary>
            public int y;
            /// <summary>Construct a point of coordinates (x,y).</summary>
            public POINT(int x, int y)
            {
                this.x = x;
                this.y = y;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MINMAXINFO
        {
            public POINT ptReserved;
            public POINT ptMaxSize;
            public POINT ptMaxPosition;
            public POINT ptMinTrackSize;
            public POINT ptMaxTrackSize;
        };

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public class MONITORINFO
        {
            public int cbSize = Marshal.SizeOf(typeof(MONITORINFO));
            public RECT rcMonitor = new RECT();
            public RECT rcWork = new RECT();
            public int dwFlags = 0;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 0)]
        public struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
            public static readonly RECT Empty = new RECT();
            public int Width { get { return Math.Abs(right - left); } }
            public int Height { get { return bottom - top; } }
            public RECT(int left, int top, int right, int bottom)
            {
                this.left = left;
                this.top = top;
                this.right = right;
                this.bottom = bottom;
            }
            public RECT(RECT rcSrc)
            {
                left = rcSrc.left;
                top = rcSrc.top;
                right = rcSrc.right;
                bottom = rcSrc.bottom;
            }
            public bool IsEmpty { get { return left >= right || top >= bottom; } }
            public override string ToString()
            {
                if (this == Empty) { return "RECT {Empty}"; }
                return "RECT { left : " + left + " / top : " + top + " / right : " + right + " / bottom : " + bottom + " }";
            }
            public override bool Equals(object obj)
            {
                if (!(obj is Rect)) { return false; }
                return (this == (RECT)obj);
            }
            /// <summary>Return the HashCode for this struct (not garanteed to be unique)</summary>
            public override int GetHashCode() => left.GetHashCode() + top.GetHashCode() + right.GetHashCode() + bottom.GetHashCode();
            /// <summary> Determine if 2 RECT are equal (deep compare)</summary>
            public static bool operator ==(RECT rect1, RECT rect2) { return (rect1.left == rect2.left && rect1.top == rect2.top && rect1.right == rect2.right && rect1.bottom == rect2.bottom); }
            /// <summary> Determine if 2 RECT are different(deep compare)</summary>
            public static bool operator !=(RECT rect1, RECT rect2) { return !(rect1 == rect2); }
        }

        [DllImport("user32")]
        internal static extern bool GetMonitorInfo(IntPtr hMonitor, MONITORINFO lpmi);

        [DllImport("User32")]
        internal static extern IntPtr MonitorFromWindow(IntPtr handle, int flags);

        //Close le Window
        //private void CloseButton_Click(object sender, RoutedEventArgs e)
        //{
        //    Window window = GetParentWindow((DependencyObject)sender);
        //    window.Close();
        //}
        ////Resize le window accordingly
        //private void MaximizeButton_Click(object sender, RoutedEventArgs e)
        //{
        //    Window window = GetParentWindow((DependencyObject)sender);
        //    if (window.WindowState == WindowState.Normal)
        //    {
        //        window.WindowState = WindowState.Maximized;
        //    }
        //    else
        //    {
        //        window.WindowState = WindowState.Normal;
        //    }
        //}
        ////Minimize le window to taskbar
        //private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        //{
        //    Window window = GetParentWindow((DependencyObject)sender);
        //    window.WindowState = WindowState.Minimized;
        //}
        //public static Window GetParentWindow(DependencyObject child)
        //{
        //    DependencyObject parentObject = VisualTreeHelper.GetParent(child);

        //    if (parentObject == null)
        //    {
        //        return null;
        //    }

        //    Window parent = parentObject as Window;
        //    if (parent != null)
        //    {
        //        return parent;
        //    }
        //    else
        //    {
        //        return GetParentWindow(parentObject);
        //    }
        //}

        //private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        //{
        //    if (this.WindowState == WindowState.Maximized)
        //    {
        //        this.BorderThickness = new System.Windows.Thickness(8);
        //    }
        //    else
        //    {
        //        this.BorderThickness = new System.Windows.Thickness(0);
        //    }
        //}
        #endregion

        private void navItem_Checked(object sender, RoutedEventArgs e)
        {
            if (e.Source is RadioButton rb)
            {
                switch (rb.Name)
                {
                    case "SongLibrary_RadioButton":
                        ContentFrame.Content = SongLibrary;
                        break;
                    case "Bible_RadioButton":
                        ContentFrame.Content = Bible;
                        break;
                    case "Message_RadioButton":
                        ContentFrame.Content = TheTable;
                        break;
                }
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            song.Dispose();
            bible.Dispose();
            message.Dispose();
            blankDisplayWindow.Dispose();
            historyWindow.Dispose();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            song = new Hotkey(Modifiers.Alt, Keys.Z, this, registerImmediately: true);
            bible = new Hotkey(Modifiers.Alt, Keys.X, this, registerImmediately: true);
            message = new Hotkey(Modifiers.Alt, Keys.C, this, registerImmediately: true);
            blankDisplayWindow = new Hotkey(Modifiers.Alt, Keys.W, this, registerImmediately: true);
            historyWindow = new Hotkey(Modifiers.Alt, Keys.H, this, registerImmediately: true);
            historyWindow.HotkeyPressed += History;
            song.HotkeyPressed += SwitchTabs;
            bible.HotkeyPressed += SwitchTabs;
            message.HotkeyPressed += SwitchTabs;
            blankDisplayWindow.HotkeyPressed += BlankDisplayWindow;

            var hwnd = new WindowInteropHelper(this).Handle;
            SetWindowLong(hwnd, GWL_STYLE, GetWindowLong(hwnd, GWL_STYLE) & ~WS_SYSMENU);
        }
        private void History(object sender, HotkeyEventArgs e)
        {
            if (!HistoryWindow.Instance.IsVisible)
            {
                HistoryWindow.Instance.Show();
            }
            else
            {
                if (HistoryWindow.Instance.WindowState == WindowState.Minimized)
                {
                    HistoryWindow.Instance.WindowState = WindowState.Normal;
                }
                else
                {
                    HistoryWindow.Instance.Close();
                }
            }

        }
        private void BlankDisplayWindow(object sender, HotkeyEventArgs e)
        {
            if (!DisplayWindow.Instance.isBlank)
            {
                DisplayWindow.Instance.isBlank = true;
                DisplayWindow.Instance.Close();
                Logo.Source = new BitmapImage(new Uri(@"pack://application:,,,/Ark_LogoBW.ico", UriKind.Absolute));
            }
            else
            {
                DisplayWindow.Instance.isBlank = false;
                DisplayWindow.Instance.Show();
                Logo.Source = new BitmapImage(new Uri(@"pack://application:,,,/Ark_Logo.ico", UriKind.Absolute));
            }
        }
        private void SwitchTabs(object sender, HotkeyEventArgs e)
        {
            switch (e.HotkeyInfo.Key.ToString())
            {
                case "Z":
                    SongLibrary_RadioButton.IsChecked = true;
                    break;
                case "X":
                    Bible_RadioButton.IsChecked = true;
                    break;
                case "C":
                    Message_RadioButton.IsChecked = true;
                    break;
            }

        }


        // Annoying Alt thingy on the window
        private const int GWL_STYLE = -16; //WPF's Message code for Title Bar's Style 

        private void ContentFrame_Navigating(object sender, System.Windows.Navigation.NavigatingCancelEventArgs e)
        {
            if (e.NavigationMode == NavigationMode.Forward)
            {
                e.Cancel = true;
            }
            if (e.NavigationMode == NavigationMode.Back)
            {
                e.Cancel = true;
            }

        }

        private const int WS_SYSMENU = 0x80000; //WPF's Message code for System Menu
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

    }
}
