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
                Screen s = Screen.AllScreens[1];
                System.Drawing.Rectangle r = s.WorkingArea;
                Top = r.Top;
                Left = r.Left;
            }
            else
            {
                Top = Screen.PrimaryScreen.WorkingArea.Top;
                Left = Screen.PrimaryScreen.WorkingArea.Left;
            }
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Width = SystemParameters.PrimaryScreenWidth;
            Height = SystemParameters.PrimaryScreenHeight;
        }

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
    }
}
