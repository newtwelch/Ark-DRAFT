using Ark.Models.Hotkeys;
using Ark.ViewModels;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Ark.Views
{
    /// <summary>
    /// Interaction logic for Bible.xaml
    /// </summary>
    public partial class Bible : UserControl
    {
        private BibleViewModel _viewModel;

        private Hotkey closeDisplay, switchLanguage;

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
                string[] versePortions = Regex.Split(verse.Text, @"(?<=[\.,;:!\?])\s+");
                smallVerse = new ObservableCollection<string>();
                foreach (string sentence in versePortions)
                {
                    if (sentence != "")
                    {
                        smallVerse.Add(sentence);
                    }
                }
                smallVerseList.ItemsSource = smallVerse;

                // Display stuff on the Second Window
                if (!DisplayWindow.Instance.isBlank)
                {
                    DisplayWindow.Instance.BibleDisplay.Text = verse.Text;
                    DisplayWindow.Instance.BibleBookText.Text = $"{ _viewModel.SelectedBook.Name} {_viewModel.SelectedChapter.ChapterNumber}:{verse.VerseNumber}";
                    DisplayWindow.Instance.BibleBookText.Visibility = Visibility.Visible;
                    DisplayWindow.Instance.BibleDisplay.Visibility = Visibility.Visible;
                    DisplayWindow.Instance.SongDisplay.Visibility = Visibility.Collapsed;
                    DisplayWindow.Instance.Show();
                }
            }
        }

        private void smallVerseList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 0)
                return;
            if (_viewModel.SelectedBook != null)
            {
                string selectedItem = (string)e.AddedItems[0];
                ENGLISH.Tag = selectedItem;
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
                        ChapterSearch.Text = "";
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
                        VerseSearch.Text = "";
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
                    if (Keyboard.Modifiers == ModifierKeys.Shift && e.Key == Key.Enter)
                    {
                        if (VerseList.SelectedItem == null)
                        {
                            int vindex = VerseList.Items.Cast<VerseData>().ToList().FindIndex(x => x.VerseNumber.ToString() == tb.Text);
                            VerseList.SelectedIndex = vindex;
                        }
                        smallVerseList.SelectedIndex = 0;
                        smallVerseList.Focus();
                        e.Handled = true;
                    }
                    else if (e.Key == Key.Enter)
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
                    if (e.Key == Key.Enter)
                    {
                        smallVerseList.SelectedIndex = 0;
                        smallVerseList.Focus();
                        e.Handled = true;
                    }
                    break;
                case "smallVerseList":
                    int idx = lb.Items.Count - 1;
                    if (e.Key == Key.Down && lb.SelectedIndex == idx)
                    {
                        int t = VerseList.Items.Count - 1;
                        if (VerseList.SelectedIndex == t)
                        {
                            ChapterList.SelectedIndex++;
                            VerseList.SelectedIndex = 0;
                        }
                        else
                        {
                            VerseList.SelectedIndex++;
                        }
                        smallVerseList.Focus();
                    }
                    if (e.Key == Key.Up && lb.SelectedIndex == 0 && VerseList.SelectedIndex != 0)
                    {
                        VerseList.SelectedIndex--;
                        smallVerseList.SelectedIndex = smallVerseList.Items.Count - 1;
                        smallVerseList.Focus();
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
            switchLanguage = new Hotkey(Modifiers.Shift, Models.Hotkeys.Keys.S, Window.GetWindow(this), registerImmediately: true);
            closeDisplay.HotkeyPressed += CloseDisplay;
            switchLanguage.HotkeyPressed += SwitchLanguage;
        }

        // Close Second Window or the Display Window
        private void CloseDisplay(object sender, HotkeyEventArgs e)
        {
            DisplayWindow.Instance.BibleBookText.Visibility = Visibility.Collapsed;
            DisplayWindow.Instance.BibleDisplay.Visibility = Visibility.Collapsed;
            DisplayWindow.Instance.HighlightPhrase.Text = "";
            DisplayWindow.Instance.Close();
        }
        private void SwitchLanguage(object sender, HotkeyEventArgs e)
        {
            if (ENGLISH.IsChecked == true)
            {
                TAGALOG.IsChecked = true;
            }
            else
            {
                ENGLISH.IsChecked = true;
            }
        }

        // Clean Hotkey cache(?)
        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            closeDisplay.Dispose();
            switchLanguage.Dispose();
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            BookData book = BookList.SelectedItem as BookData;
            ChapterData chapter = ChapterList.SelectedItem as ChapterData;
            VerseData verse = VerseList.SelectedItem as VerseData;

            if (e.Source is RadioButton rb)
            {
                switch (rb.Name)
                {
                    case "ENGLISH":
                        _viewModel.ChangeLanguage("ENGLISH");
                        break;
                    case "TAGALOG":
                        _viewModel.ChangeLanguage("TAGALOG");
                        break;
                }

                if (book != null)
                {
                    BookList.SelectedIndex = BookList.Items.Cast<BookData>().ToList().FindIndex(x => x.BookNumber == book.BookNumber);
                }
                if (chapter != null)
                {
                    ChapterList.SelectedIndex = ChapterList.Items.Cast<ChapterData>().ToList().FindIndex(x => x.ChapterNumber == chapter.ChapterNumber);
                }
                if (verse != null)
                {
                    // This is a cheat, Had to do this for listbox behavior stuff
                    ChapterList.Focus();
                    VerseList.SelectedIndex = VerseList.Items.Cast<VerseData>().ToList().FindIndex(x => x.VerseNumber == verse.VerseNumber);
                    VerseList.Focus();
                }

            }
        }
    }
}
