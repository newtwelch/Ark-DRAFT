using Ark.Models;
using Ark.Models.Helper;
using Ark.Models.Hotkeys;
using Ark.ViewModels;
using System;
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

        private Hotkey closeDisplay, switchLanguage, focusSearch, focusSearchSpecific;

        private TypeAssistant assistant;

        private ObservableCollection<string> smallVerse;

        public Bible()
        {
            _viewModel = new BibleViewModel();
            DataContext = _viewModel;

            InitializeComponent();

            assistant = new TypeAssistant();
            assistant.Idled += assistant_Idled;

            BookList.SelectedIndex = 0;
            ChapterList.SelectedIndex = 0;
            BookSearch.Focus();
        }
        void assistant_Idled(object sender, EventArgs e)
        {
            this.Dispatcher.Invoke(
            new System.Windows.Forms.MethodInvoker(() =>
            {
                if (ChapterSearch.IsFocused && ChapterSearch.Text != "")
                {
                    VerseSearch.Text = "";
                    VerseSearch.Focus();
                }
                if (VerseSearch.IsFocused && VerseSearch.Text != "")
                {
                    VerseList.Focus();
                }
                if (TextSearch.IsFocused)
                {
                    if (TextSearch.Text.StartsWith("."))
                    {
                        _viewModel.FindText(TextSearch.Text.TrimStart('.'), "Global");
                        if (BibleDataList.Items.Count == 0)
                        {
                            Empty.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            Empty.Visibility = Visibility.Collapsed;
                        }
                        Searching.Visibility = Visibility.Collapsed;
                        BibleDataList.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        _viewModel.FindText(TextSearch.Text, "Local");
                        if (VerseList.Items.Count == 0)
                        {
                            Empty.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            Empty.Visibility = Visibility.Collapsed;
                        }
                        VerseList.Visibility = Visibility.Visible;
                        BibleDataList.Visibility = Visibility.Collapsed;
                    }
                    ENGLISH.Tag = TextSearch.Text.TrimStart('.');
                }
            }));
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
            if (e.AddedItems.Count == 0)
                return;
            if (VerseList.Visibility == Visibility.Visible)
            {
                if (VerseList.SelectedItem != null)
                {
                    // Verse Selection Stuff
                    ENGLISH.Tag = "";
                    VerseList.ScrollIntoView(VerseList.SelectedItem);
                    VerseData verse = VerseList.SelectedItem as VerseData;

                    // Add to History
                    History.Instance.AddBible(new BibleData() { BookData = BookList.SelectedItem as BookData,
                                                       ChapterData = ChapterList.SelectedItem as ChapterData,
                                                       VerseData = verse });

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

                    DisplayWindow.Instance.BibleDisplay.Text = verse.Text;
                    DisplayWindow.Instance.BibleBookText.Text = $"{ _viewModel.SelectedBook.Name } { _viewModel.SelectedChapter.ChapterNumber }:{ verse.VerseNumber }";
                }
            }
            else if (BibleDataList.Visibility == Visibility.Visible)
            {
                if (BibleDataList.SelectedItem != null)
                {
                    // Verse Selection Stuff
                    BibleDataList.ScrollIntoView(BibleDataList.SelectedItem);
                    BibleData bibleData = BibleDataList.SelectedItem as BibleData;
                    VerseData verse = bibleData.VerseData;
                    ChapterData chapter = bibleData.ChapterData;
                    BookData book = bibleData.BookData;


                    DisplayWindow.Instance.BibleDisplay.Text = verse.Text;
                    DisplayWindow.Instance.BibleBookText.Text = $"{ book.Name } { chapter.ChapterNumber }:{ verse.VerseNumber }";
                }
            }

            // Display stuff on the Second Window
            if (!DisplayWindow.Instance.isBlank)
            {
                DisplayWindow.Instance.BibleBookText.Visibility = Visibility.Visible;
                DisplayWindow.Instance.BibleDisplay.Visibility = Visibility.Visible;
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
                ENGLISH.Tag = selectedItem;
                DisplayWindow.Instance.BibleDisplay.HighlightPhrase = selectedItem;
            }
        }

        private void Search_TextChanged(object sender, TextChangedEventArgs e)
        {
            assistant.TextChanged();
            Empty.Visibility = Visibility.Collapsed;

            TextBox tb = sender as TextBox;

            switch (tb.Name)
            {
                case "BookSearch":
                    _viewModel.FindBooks(tb.Text);
                    if (BookList.Items.Count == 1)
                    {
                        BookList.SelectedIndex = 0;
                        ChapterSearch.Text = "";
                        ChapterSearch.Focus();
                    }
                    break;
                case "TextSearch":
                    if (tb.Text == "")
                    {
                        _viewModel.GetVerses(_viewModel.SelectedBook.BookNumber, _viewModel.SelectedChapter.ChapterNumber);
                        VerseList.Visibility = Visibility.Visible;
                        BibleDataList.Visibility = Visibility.Collapsed;
                        Searching.Visibility = Visibility.Collapsed;
                        ENGLISH.Tag = "";
                    }
                    else if (tb.Text.StartsWith("."))
                    {
                        _viewModel.BibleData.Clear();
                        VerseList.Visibility = Visibility.Collapsed;
                        Searching.Visibility = Visibility.Visible;
                    }
                    break;
                case "ChapterSearch":
                    TextSearch.Text = "";
                    int cindex = ChapterList.Items.Cast<ChapterData>().ToList().FindIndex(x => x.ChapterNumber.ToString() == ChapterSearch.Text);
                    ChapterList.SelectedIndex = cindex;
                    VerseList.SelectedItem = null;
                    break;
                case "VerseSearch":
                    TextSearch.Text = "";
                    int vindex = VerseList.Items.Cast<VerseData>().ToList().FindIndex(x => x.VerseNumber.ToString() == VerseSearch.Text);
                    VerseList.SelectedIndex = vindex;
                    break;
            }
            //e.Handled = true;
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
                        int vindex = VerseList.Items.Cast<VerseData>().ToList().FindIndex(x => x.VerseNumber.ToString() == tb.Text);
                        VerseList.SelectedIndex = vindex;
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
                        e.Handled = true;
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
                    if (e.Key == Key.Down && lb.SelectedIndex == i)
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
                        ChapterList.Focus();
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
            switchLanguage = new Hotkey(Modifiers.Ctrl, Models.Hotkeys.Keys.D, Window.GetWindow(this), registerImmediately: true);
            focusSearch = new Hotkey(Modifiers.Alt, Models.Hotkeys.Keys.S, Window.GetWindow(this), registerImmediately: true);
            focusSearchSpecific = new Hotkey(Modifiers.Ctrl | Modifiers.Alt, Models.Hotkeys.Keys.S, Window.GetWindow(this), registerImmediately: true);
            closeDisplay.HotkeyPressed += CloseDisplay;
            switchLanguage.HotkeyPressed += SwitchLanguage;
            focusSearch.HotkeyPressed += FocusSearch;
            focusSearchSpecific.HotkeyPressed += FocusSearchSpecific;

            DisplayWindow.Instance.BibleBookText.Visibility = Visibility.Visible;
            DisplayWindow.Instance.BibleDisplay.Visibility = Visibility.Visible;
        }

        // Close Second Window or the Display Window
        private void CloseDisplay(object sender, HotkeyEventArgs e)
        {
            DisplayWindow.Instance.HighlightPhrase.Text = "";
            DisplayWindow.Instance.Close();
            smallVerseList.SelectedItem = null;
            ENGLISH.Tag = "";
            VerseList.Focus();
        }
        // Put focus on the search box, Book Search
        private void FocusSearch(object sender, HotkeyEventArgs e)
        {
            BookSearch.Text = "";
            BookSearch.Focus();
        }
        // Put focus on the search box, Entire Bible Search
        private void FocusSearchSpecific(object sender, HotkeyEventArgs e)
        {
            TextSearch.Text = "";
            TextSearch.Focus();
        }
        // Switch Language
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
            DisplayWindow.Instance.BibleBookText.Visibility = Visibility.Collapsed;
            DisplayWindow.Instance.BibleDisplay.Visibility = Visibility.Collapsed;
            closeDisplay.Dispose();
            switchLanguage.Dispose();
            focusSearch.Dispose();
            focusSearchSpecific.Dispose();
        }

        #region numberOnly
        private static readonly Regex _regex = new Regex("[^0-9.-]+"); //regex that matches disallowed text
        private static bool IsTextAllowed(string text)
        {
            return !_regex.IsMatch(text);
        }

        private void Numbers_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }
        #endregion

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
                        if (_viewModel.SelectedBook != null)
                        {
                            BookList.SelectedIndex = BookList.Items.Cast<BookData>().ToList().FindIndex(x => x.BookNumber == _viewModel.SelectedBook.BookNumber);
                        }
                        if (_viewModel.SelectedChapter != null)
                        {
                            ChapterList.SelectedIndex = ChapterList.Items.Cast<ChapterData>().ToList().FindIndex(x => x.ChapterNumber == _viewModel.SelectedChapter.ChapterNumber);
                        }
                        if (verse != null)
                        {
                            // This is a cheat, Had to do this for listbox behavior stuff
                            ChapterList.Focus();
                            // End of Cheat Kek
                            VerseList.SelectedIndex = VerseList.Items.Cast<VerseData>().ToList().FindIndex(x => x.VerseNumber == verse.VerseNumber);
                            VerseList.Focus();
                        }
                        break;
                    case "TAGALOG":
                        _viewModel.ChangeLanguage("TAGALOG");
                        if (_viewModel.SelectedBook != null)
                        {
                            BookList.SelectedIndex = BookList.Items.Cast<BookData>().ToList().FindIndex(x => x.BookNumber == _viewModel.SelectedBook.BookNumber);
                        }
                        if (_viewModel.SelectedChapter != null)
                        {
                            ChapterList.SelectedIndex = ChapterList.Items.Cast<ChapterData>().ToList().FindIndex(x => x.ChapterNumber == _viewModel.SelectedChapter.ChapterNumber);
                        }
                        if (verse != null)
                        {
                            // This is a cheat, Had to do this for listbox behavior stuff
                            ChapterList.Focus();
                            // End of Cheat Kek
                            VerseList.SelectedIndex = VerseList.Items.Cast<VerseData>().ToList().FindIndex(x => x.VerseNumber == verse.VerseNumber);
                            VerseList.Focus();
                        }
                        break;
                }

            }
        }
        private void BibleData_Click(object sender, RoutedEventArgs e)
        {
            BibleDataList.Visibility = Visibility.Collapsed;
            VerseList.Visibility = Visibility.Visible;
            Button button = sender as Button;
            BibleData bibleData = button.DataContext as BibleData;

            TextSearch.Text = "";

            BookList.SelectedIndex = BookList.Items.Cast<BookData>().ToList().FindIndex(x => x.BookNumber == bibleData.BookData.BookNumber);
            ChapterList.SelectedIndex = ChapterList.Items.Cast<ChapterData>().ToList().FindIndex(x => x.ChapterNumber == bibleData.ChapterData.ChapterNumber);
            VerseList.SelectedIndex = VerseList.Items.Cast<VerseData>().ToList().FindIndex(x => x.VerseNumber == bibleData.VerseData.VerseNumber);

            VerseList.Focus();
        }
    }
}
