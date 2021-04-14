using Ark.ViewModels;
using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Ark.Views
{
    /// <summary>
    /// Interaction logic for Bible.xaml
    /// </summary>
    public partial class Bible : UserControl
    {
        private BibleViewModel _viewModel;

        public Bible()
        {
            _viewModel = new BibleViewModel();
            DataContext = _viewModel;

            InitializeComponent();
        }

        private void BookList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 0)
                return;
            BookData selectedItem = (BookData)e.AddedItems[0];
            _viewModel.SelectedBook = selectedItem;
            _viewModel.GetChapters(_viewModel.SelectedBook.BookNumber);
        }
    }
}
