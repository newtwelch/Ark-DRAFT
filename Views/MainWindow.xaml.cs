﻿using Ark.Models.Hotkeys;
using Ark.ViewModels;
using System;
using System.ComponentModel;
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

        public MainWindow()
        {
            _viewModel = new MainWindowViewModel();
            DataContext = _viewModel;

            InitializeComponent();

            //Initialize with SongLibrary UserControl
            SongLibrary_RadioButton.IsChecked = true;

            MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
            MaxWidth = SystemParameters.MaximizedPrimaryScreenWidth;
        }

        #region Window Buttons
        //Close le Window
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Window window = GetParentWindow((DependencyObject)sender);
            window.Close();
        }
        //Resize le window accordingly
        private void MaximizeButton_Click(object sender, RoutedEventArgs e)
        {
            Window window = GetParentWindow((DependencyObject)sender);
            if (window.WindowState == WindowState.Normal)
            {
                window.WindowState = WindowState.Maximized;
            }
            else
            {
                window.WindowState = WindowState.Normal;
            }
        }
        //Minimize le window to taskbar
        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            Window window = GetParentWindow((DependencyObject)sender);
            window.WindowState = WindowState.Minimized;
        }
        public static Window GetParentWindow(DependencyObject child)
        {
            DependencyObject parentObject = VisualTreeHelper.GetParent(child);

            if (parentObject == null)
            {
                return null;
            }

            Window parent = parentObject as Window;
            if (parent != null)
            {
                return parent;
            }
            else
            {
                return GetParentWindow(parentObject);
            }
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //if (this.WindowState == WindowState.Maximized)
            //{
            //    this.BorderThickness = new System.Windows.Thickness(7);
            //}
            //else
            //{
            //    this.BorderThickness = new System.Windows.Thickness(0);
            //}
        }
        #endregion

        private void navItem_Checked(object sender, RoutedEventArgs e)
        {
            if (e.Source is RadioButton rb)
            {
                switch (rb.Name)
                {
                    case "SongLibrary_RadioButton":
                        ContentFrame.Content = new SongLibrary();
                        break;
                    case "Bible_RadioButton":
                        ContentFrame.Content = new Bible();
                        break;
                    case "Message_RadioButton":
                        ContentFrame.Content = new TheTable();
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
