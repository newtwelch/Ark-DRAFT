using Ark.Models.Hotkeys;
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
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Ark.Views
{
    /// <summary>
    /// Interaction logic for SongLibrary.xaml
    /// </summary>
    public partial class SongLibrary : System.Windows.Controls.UserControl
    {

        private SongLibraryViewModel _viewModel;

        private Hotkey closeDisplay;

        public SongLibrary()
        {
            // Set DataContext
            _viewModel = new SongLibraryViewModel();
            DataContext = _viewModel;

            InitializeComponent();

            SongSearchBox.Focus();
            SongList.SelectedIndex = 0;
            LanguageList.SelectedIndex = 0;
        }

        // Select Songs
        private void SongList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_viewModel.EditModeChecked)
            {
                SongData selectedSong = _viewModel.SelectedSong;
                _viewModel.SelectedSong = selectedSong;
                (sender as System.Windows.Controls.ListBox).SelectedItem = null;
            }
            else
            {
                if (e.AddedItems.Count == 0)
                    return;
                SongData selectedItem = (SongData)e.AddedItems[0];
                _viewModel.SelectedSong = selectedItem;
                string test = _viewModel.SelectedSong.Sequence;
                _viewModel.RefreshLyrics();
                if (LanguageList != null)
                {
                    _viewModel.RefreshLanguages();
                    int songID = _viewModel.SelectedSong.SongID;
                    int index = LanguageList.Items.Cast<SongData>().ToList().FindIndex(x => x.SongID == songID);
                    LanguageList.SelectedIndex = index;
                }
            }
        }

        // Language songs
        private void LanguageList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 0)
            {
            }
            else
            {
                SongData selectedItem = (SongData)e.AddedItems[0];
                _viewModel.SelectedSong = selectedItem;
                int songID = _viewModel.SelectedSong.SongID;
                int index = SongList.Items.Cast<SongData>().ToList().FindIndex(x => x.SongID == songID);
                SongList.SelectedIndex = index;
                SongList.ScrollIntoView(SongList.SelectedItem);
                _viewModel.RefreshLyrics();
            }
        }

        // Save Song
        private void EditSongButton_Unchecked(object sender, RoutedEventArgs e)
        {
            if (_viewModel.SelectedSong != null)
            {
                _viewModel.SelectedSong.Language = LanguageTextBox.Text;
                int songID = _viewModel.SelectedSong.SongID;
                _viewModel.SaveSong();
                _viewModel.RefreshSongList();
                int index = SongList.Items.Cast<SongData>().ToList().FindIndex(x => x.SongID == songID);
                SongList.SelectedIndex = index;
                _viewModel.RefreshLanguages();
            }
        }

        // Searching for Songs
        private void SongSearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (SongSearchBox.Text == "")
            {
                _viewModel.RefreshSongList();
            }
            else if (SongSearchBox.Text.StartsWith("*"))
            {
                _viewModel.GetSongsBy("Author", SongSearchBox.Text.Replace("*", ""));
            }
            else if (SongSearchBox.Text.StartsWith("."))
            {
                _viewModel.GetSongsBy("Lyrics", SongSearchBox.Text.Replace(".", ""));
            }
            else
            {
                _viewModel.GetSongsBy("Title", SongSearchBox.Text);
            }
        }

        // Removing Selected Lyric
        private void RemoveLyric_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel.Lyrics.Count > 1 && _viewModel.EditModeChecked)
            {
                LyricBox.SelectedItem = null;
                System.Windows.Controls.Button button = sender as System.Windows.Controls.Button;
                LyricData lyric = button.DataContext as LyricData;
                LyricBox.SelectedItem = lyric;

                if (lyric.Type == LyricType.Stanza)
                {
                    int selectedLyric = Int32.Parse(lyric.Line);

                    foreach (LyricData _lyric in _viewModel.Lyrics.Where(x => x.Type == LyricType.Stanza))
                    {
                        int store = Int32.Parse(_lyric.Line);
                        if (selectedLyric < store)
                        {
                            store -= 1;
                            _lyric.Line = store.ToString();
                        }
                    }
                }

                _viewModel.Lyrics.Remove(lyric);

            }
        }

        // Display text on second sreen or Display Window unless you're in edit mode
        private void LyricBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LyricBox.SelectedItem != null && !_viewModel.EditModeChecked && !DisplayWindow.Instance.isBlank)
            {
                LyricData lyric = LyricBox.SelectedItem as LyricData;
                LyricBox.SelectedItem = lyric;

                DisplayWindow.Instance.SongDisplay.Text = lyric.Text;
                DisplayWindow.Instance.SongDisplay.Visibility = Visibility.Visible;
                DisplayWindow.Instance.BibleBookText.Visibility = Visibility.Collapsed;
                DisplayWindow.Instance.BibleDisplay.Visibility = Visibility.Collapsed;
                DisplayWindow.Instance.Show();
            }
        }

        //Enter Key on Song Search
        private void SongSearchBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                SongList.SelectedIndex = 0;
                SongList.Focus();
                e.Handled = true;
            }
        }

        //Enter Key on Song List
        private void SongList_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                LyricBox.SelectedIndex = 0;
                LyricBox.Focus();
                e.Handled = true;
            }
        }

        #region Horizontal ScrollWheel
        private void ListBox_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            System.Windows.Controls.ListBox listBox = sender as System.Windows.Controls.ListBox;
            ScrollViewer scrollviewer = FindVisualChildren<ScrollViewer>(listBox).FirstOrDefault();
            if (e.Delta > 0)
                scrollviewer.LineLeft();
            else
                scrollviewer.LineRight();
            e.Handled = true;

        }

        private static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        yield return (T)child;
                    }

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }
        #endregion

        #region HotKeys
        // Load Hotkeys on UserControl Load
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            closeDisplay = new Hotkey(Modifiers.NoMod, Models.Hotkeys.Keys.Escape, Window.GetWindow(this), registerImmediately: true);
            closeDisplay.HotkeyPressed += CloseDisplay;
        }

        // Close Second Window or the Display Window
        private void CloseDisplay(object sender, HotkeyEventArgs e)
        {
            DisplayWindow.Instance.SongDisplay.Visibility = Visibility.Collapsed;
            DisplayWindow.Instance.HighlightPhrase.Text = "";
            DisplayWindow.Instance.Close();
            LyricBox.SelectedItem = null;
            if (LyricBox.IsFocused)
            {
                int i = LyricBox.SelectedIndex;
                LyricBox.SelectedIndex = i--;
            }
        }

        // Clean Hotkey cache(?)
        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            closeDisplay.Dispose();
        }
        #endregion
    }
}
