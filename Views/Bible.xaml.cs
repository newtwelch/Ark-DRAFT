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

            BookList.SelectedIndex = 1;
            ChapterList.SelectedIndex = 1;
        }

        private void BookList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 0)
                return;
            BookData selectedItem = (BookData)e.AddedItems[0];
            _viewModel.SelectedBook = selectedItem;
            int chapterIndex = ChapterList.SelectedIndex;
            _viewModel.GetChapters(_viewModel.SelectedBook.BookNumber);
            ChapterList.SelectedIndex = chapterIndex;
        }

        private void Chapter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 0)
                return;
            if (_viewModel.SelectedBook != null)
            {
                ChapterData selectedItem = (ChapterData)e.AddedItems[0];
                _viewModel.GetVerses(_viewModel.SelectedBook.BookNumber, selectedItem.ChapterNumber);
            }
        }

        private void VerseList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (VerseList.SelectedItem != null)
            {
                VerseData verse = VerseList.SelectedItem as VerseData;
                VerseList.SelectedItem = verse;
                ChapterData chapter = ChapterList.SelectedItem as ChapterData;

                DisplayWindow.Instance.TextDisplay.Text = verse.Text;
                DisplayWindow.Instance.BibleText.Text = $"{ _viewModel.SelectedBook.Name} {chapter.ChapterNumber}:{verse.VerseNumber}";
                DisplayWindow.Instance.BibleText.Visibility = Visibility.Visible;
                DisplayWindow.Instance.Show();
            }
        }

        private void Search_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            switch (tb.Name)
            {
                case "BookSearch":
                    _viewModel.SortBooks(tb.Text);
                    
                    break;
                case "ChapterSearch":
                    break;
                case "VerseSearch":
                    break;
            }
        }
    }
}
