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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Ark.Views
{
    /// <summary>
    /// Interaction logic for SongLibrary.xaml
    /// </summary>
    public partial class SongLibrary : UserControl
    {

        private SongLibraryViewModel _viewModel;

        public SongLibrary()
        {
            // Set DataContext
            _viewModel = new SongLibraryViewModel();
            DataContext = _viewModel;

            InitializeComponent();
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
            }
        }

        private void EditSongButton_Unchecked(object sender, RoutedEventArgs e)
        {
            _viewModel.SaveSong();
        }

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
    }
}
