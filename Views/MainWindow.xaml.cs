using Ark.ViewModels;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Ark.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainWindowViewModel _viewModel;

        public object Controls { get; private set; }

        public MainWindow()
        {
            _viewModel = new MainWindowViewModel();
            DataContext = _viewModel;

            InitializeComponent();
            
            //Initialize with SongLibrary UserControl
            ContentFrame.Content = new SongLibrary();

            //MainMenu.SelectedIndex = 0;
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

        private void navItem_Click(object sender, RoutedEventArgs e)
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
                        ContentFrame.Content = new Bible();
                        break;
                }
            }

        }
    }
}
