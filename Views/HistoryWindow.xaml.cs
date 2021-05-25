using Ark.Models;
using Ark.Models.SongLibrary;
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
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Ark.Views
{
    public partial class HistoryWindow : Window
    {
        private HistoryWindowViewModel _viewModel;
        private bool forceClose;

        public bool isBlank;

        private static HistoryWindow _defInstance;
        public static HistoryWindow Instance
        {
            get
            {
                if (null == _defInstance)
                {
                    _defInstance = new HistoryWindow();
                }
                return _defInstance;
            }
        }
        private HistoryWindow()
        {
            _viewModel = new HistoryWindowViewModel();
            DataContext = _viewModel;

            InitializeComponent();

        }

        //Closing Thingy
        #region Closing Thingy
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
        #endregion

        #region Window Buttons
        //Close le Window
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Window window = GetParentWindow((DependencyObject)sender);
            window.Close();
        }
        //Minimize le window to taskbar
        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            Window window = GetParentWindow((DependencyObject)sender);
            window.WindowState = WindowState.Minimized;
        }
        //Get Parent Window Thingy
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
        #endregion

        private void HistoryList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
            History.Instance.HistoryChange(HistoryList.SelectedValue);
        }
    }
}
