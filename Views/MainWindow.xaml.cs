using Ark.Models.Hotkeys;
using Ark.ViewModels;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Ark.Views
{
    public partial class MainWindow : Window
    {
        private MainWindowViewModel _viewModel;

        private Hotkey song, bible, message, blankDisplayWindow;
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
            if (this.WindowState == WindowState.Maximized)
            {
                this.BorderThickness = new System.Windows.Thickness(7);
            }
            else
            {
                this.BorderThickness = new System.Windows.Thickness(0);
            }
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
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            song = new Hotkey(Modifiers.Alt, Keys.Z, this, registerImmediately: true);
            bible = new Hotkey(Modifiers.Alt, Keys.X, this, registerImmediately: true);
            message = new Hotkey(Modifiers.Alt, Keys.C, this, registerImmediately: true);
            blankDisplayWindow = new Hotkey(Modifiers.Alt, Keys.W, this, registerImmediately: true);
            song.HotkeyPressed += SwitchTabs;
            bible.HotkeyPressed += SwitchTabs;
            message.HotkeyPressed += SwitchTabs;
            blankDisplayWindow.HotkeyPressed += BlankDisplayWindow;
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
    }
}
