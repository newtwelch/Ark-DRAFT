using Ark.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Ark.Views
{
    public partial class DisplayWindow : Window
    {
        private DisplayWindowViewModel _viewModel;
        private bool forceClose;
        public static DisplayWindow Instance { get; private set; }

        public bool isBlank;

        static DisplayWindow()
        {
            Instance = new DisplayWindow();
        }
        private DisplayWindow()
        {
            _viewModel = new DisplayWindowViewModel();
            DataContext = _viewModel;

            InitializeComponent();

            if (Screen.AllScreens.Length > 1)
            {
                MaxWidth = Screen.AllScreens[1].Bounds.Width;
                MaxHeight = Screen.AllScreens[1].Bounds.Height;
            }
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (Screen.AllScreens.Length == 1) { 
                Width = SystemParameters.PrimaryScreenWidth;
                Height = SystemParameters.PrimaryScreenHeight;
            }
            else
            {
                Width = Screen.AllScreens[1].Bounds.Width;
                Height = Screen.AllScreens[1].Bounds.Height;
            }
        }

        // Closing Thingy
        public void CloseForced()
        {
            forceClose = true;
            Close();
        }
        protected override void OnClosing(CancelEventArgs e)
        {
            if (forceClose)
            {
                e.Cancel = false;
            }
            else
            {
                Visibility = Visibility.Collapsed;
                e.Cancel = true;
            }
        }

        private void Window_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Screen.AllScreens.Length > 1)
            {
                Screen s = Screen.AllScreens[1];
                System.Drawing.Rectangle r = s.WorkingArea;
                Top = r.Top;
                Left = r.Left;
                MaxWidth = Screen.AllScreens[1].Bounds.Width;
                MaxHeight = Screen.AllScreens[1].Bounds.Height;
                Width = Screen.AllScreens[1].Bounds.Width;
                Height = Screen.AllScreens[1].Bounds.Height;
            }
            else
            {
                MaxWidth = SystemParameters.PrimaryScreenWidth;
                MaxHeight = SystemParameters.PrimaryScreenHeight;
                Top = Screen.PrimaryScreen.WorkingArea.Top;
                Left = Screen.PrimaryScreen.WorkingArea.Left;
                Width = SystemParameters.PrimaryScreenWidth;
                Height = SystemParameters.PrimaryScreenHeight;
            }

        }
    }
}
