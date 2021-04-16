using Ark.Models.Hotkeys;
using Ark.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

        private Hotkey closeDisplay;

        private ObservableCollection<string> smallVerse;
 
        public Bible()
        {
            _viewModel = new BibleViewModel();
            DataContext = _viewModel;

            InitializeComponent();

            BookList.SelectedIndex = 0;
            ChapterList.SelectedIndex = 0;
            BookSearch.Focus();
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
                _viewModel.SelectedChapter = selectedItem;
                ChapterList.ScrollIntoView(ChapterList.SelectedItem);
                _viewModel.GetVerses(_viewModel.SelectedBook.BookNumber, selectedItem.ChapterNumber);
            }
        }

        private void VerseList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (VerseList.SelectedItem != null)
            {
                // Verse Selection Stuff
                VerseList.ScrollIntoView(VerseList.SelectedItem);
                VerseData verse = VerseList.SelectedItem as VerseData;
                VerseList.SelectedItem = verse;

                // Cut the Verse into Sizeable Chunks
                            // First Split The Verse in 7 words
                string[] versePortions = Regex.Split(verse.Text, @"\s+");
                int nElements;
                smallVerse = new ObservableCollection<string>();
                for (int i = 0; i < versePortions.Length; i += 7)
                {
                    if (i + 3 < versePortions.Length)
                    {
                        nElements = 7;
                    }
                    else
                    {
                        nElements = versePortions.Length - i;
                    }

                    smallVerse.Add(versePortions.Skip(i).Take(nElements).Aggregate((current, next) => current + " " + next));
                }

                smallVerseList.ItemsSource = smallVerse;   

                // Display stuff on the Second Window
                DisplayWindow.Instance.BibleDisplay.Text = verse.Text;
                DisplayWindow.Instance.BibleBookText.Text = $"{ _viewModel.SelectedBook.Name} {_viewModel.SelectedChapter.ChapterNumber}:{verse.VerseNumber}";
                DisplayWindow.Instance.BibleBookText.Visibility = Visibility.Visible;
                DisplayWindow.Instance.BibleDisplay.Visibility = Visibility.Visible;
                DisplayWindow.Instance.SongDisplay.Visibility = Visibility.Collapsed;
                DisplayWindow.Instance.Show();
            }
        }

        private void smallVerseList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 0)
                return;
            if (_viewModel.SelectedBook != null)
            {
                string selectedItem = (string)e.AddedItems[0];
                DisplayWindow.Instance.BibleDisplay.HighlightPhrase = selectedItem;
            }

        }

        private void Search_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            switch (tb.Name)
            {
                case "BookSearch":
                    _viewModel.FindBooks(tb.Text);
                    if(BookList.Items.Count == 1)
                    {
                        BookList.SelectedIndex = 0;
                    }
                    break;
                case "ChapterSearch":
                    int cindex = ChapterList.Items.Cast<ChapterData>().ToList().FindIndex(x => x.ChapterNumber.ToString() == tb.Text);
                    ChapterList.SelectedIndex = cindex;
                    break;
                case "VerseSearch":
                    int vindex = VerseList.Items.Cast<VerseData>().ToList().FindIndex(x => x.VerseNumber.ToString() == tb.Text);
                    VerseList.SelectedIndex = vindex;
                    break;
            }
        }

        private void Search_KeyDown(object sender, KeyEventArgs e)
        {
            TextBox tb = sender as TextBox;

            switch (tb.Name)
            {
                case "BookSearch":
                    if (e.Key == Key.Enter)
                    {
                        BookList.SelectedIndex = 0;
                        ChapterSearch.Focus();
                        e.Handled = true;
                    }
                    if (e.Key == Key.Down)
                    {
                        BookList.Focus();
                        e.Handled = true;
                    }
                    break;
                case "ChapterSearch":
                    if (e.Key == Key.Enter)
                    {
                        VerseSearch.Focus();
                        e.Handled = true;
                    }
                    if (e.Key == Key.Tab)
                    {
                        BookSearch.Focus();
                        e.Handled = true;
                    }
                    if (e.Key == Key.Down)
                    {
                        ChapterList.Focus();
                        e.Handled = true;
                    }
                    break;
                case "VerseSearch":
                    if (e.Key == Key.Tab)
                    {
                        ChapterSearch.Focus();
                        e.Handled = true;
                    }
                    if (e.Key == Key.Enter)
                    {
                        int vindex = VerseList.Items.Cast<VerseData>().ToList().FindIndex(x => x.VerseNumber.ToString() == tb.Text);
                        VerseList.SelectedIndex = vindex;
                        VerseList.Focus();
                        e.Handled = true;
                    }
                    if (e.Key == Key.Down)
                    {
                        VerseList.Focus();
                    }
                    break;
            }

        }
        private void List_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            ListBox lb = sender as ListBox;

            switch (lb.Name)
            {
                case "BookList":
                    if (e.Key == Key.Enter)
                    {
                        ChapterSearch.Focus();
                        e.Handled = true;
                    }
                    if (e.Key == Key.Up && lb.SelectedIndex == 0)
                    {
                        lb.SelectedItem = null;
                        BookSearch.Focus();
                        e.Handled = true;
                    }
                    break;
                case "ChapterList":
                    if (e.Key == Key.Enter)
                    {
                        VerseSearch.Focus();
                        e.Handled = true;
                    }
                    if (e.Key == Key.Up && lb.SelectedIndex == 0)
                    {
                        lb.SelectedItem = null;
                        ChapterSearch.Focus();
                        e.Handled = true;
                    }
                    break;
                case "VerseList":
                    int i = lb.Items.Count - 1;
                    if (e.Key == Key.Down && lb.SelectedIndex == i )
                    {
                        ChapterList.SelectedIndex++;
                        lb.SelectedIndex = 0;
                        e.Handled = true;
                    }
                    if (e.Key == Key.Up && lb.SelectedIndex == 0 && ChapterList.SelectedIndex != 0)
                    {
                        ChapterList.SelectedIndex--;
                        VerseList.SelectedIndex = VerseList.Items.Count - 1;
                        VerseList.Focus();
                        e.Handled = true;
                    }
                    if (e.Key == Key.Tab)
                    {
                        lb.SelectedItem = null;
                        VerseSearch.Focus();
                        e.Handled = true;
                    }
                    break;
            }
        }

        // Load Hotkeys on UserControl Load
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            closeDisplay = new Hotkey(Modifiers.NoMod, Models.Hotkeys.Keys.Escape, Window.GetWindow(this), registerImmediately: true);
            closeDisplay.HotkeyPressed += CloseDisplay;
        }

        // Close Second Window or the Display Window
        private void CloseDisplay(object sender, HotkeyEventArgs e)
        {
            DisplayWindow.Instance.BibleBookText.Visibility = Visibility.Collapsed;
            DisplayWindow.Instance.BibleDisplay.Visibility = Visibility.Collapsed;
            DisplayWindow.Instance.HighlightPhrase.Text = "";
            DisplayWindow.Instance.Close();
            VerseList.SelectedItem = null;
        }

        // Clean Hotkey cache(?)
        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            closeDisplay.Dispose();
        }

    }
}
