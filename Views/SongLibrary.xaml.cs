using Ark.Models.Helper;
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
        TypeAssistant assistant;

        public SongLibrary()
        {
            // Set DataContext
            _viewModel = new SongLibraryViewModel();
            DataContext = _viewModel;

            InitializeComponent();

            SongList.SelectedIndex = 0;
            LanguageList.SelectedIndex = 0;

            assistant = new TypeAssistant();
            assistant.Idled += assistant_Idled;
        }

        private void SongList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 0)
            {
            }
            else
            {
                SongData selectedItem = (SongData)e.AddedItems[0];
                _viewModel.SelectedSong = selectedItem;
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
                
                _viewModel.RefreshLyrics();
            }
        }

        private void EditSongButton_Unchecked(object sender, RoutedEventArgs e)
        {
            _viewModel.SelectedSong.Language = LanguageTextBox.Text;
            int songID = _viewModel.SelectedSong.SongID;
            int index = SongList.Items.Cast<SongData>().ToList().FindIndex(x => x.SongID == songID);
            SongList.SelectedIndex = index;
            _viewModel.SaveSong();
            _viewModel.RefreshLanguages();
        }
        void assistant_Idled(object sender, EventArgs e)
        {
            this.Dispatcher.Invoke(
            new MethodInvoker(() =>
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
            }));
        }
        private void SongSearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            assistant.TextChanged();
        }
    }
}
