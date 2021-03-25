using Ark.Model.Helper;
using Ark.Models.SongLibrary;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Ark.ViewModels
{
    public class SongLibraryViewModel : ViewModelBase
    {
        #region SongData
        //List Of Songs
        public ObservableCollection<SongData> Songs { get; set; }
        public SongInterface songInterface;
        //The selected Song;
        private SongData _selectedSong;
        public SongData SelectedSong
        {
            get { return _selectedSong; }
            set
            {
                _selectedSong = value;
                OnPropertyChanged("SelectedSong");
            }
        }

        //List of Lyrics
        public ObservableCollection<LyricData> Lyrics { get; set; }
        #endregion

        #region EditMode
        //Edit Mode Check
        private bool _editModeChecked;
        public bool EditModeChecked
        {
            get { return _editModeChecked; }
            set
            {
                _editModeChecked = value;
                OnPropertyChanged("EditModeChecked");
                OnPropertyChanged("EditModeVisible");
                OnPropertyChanged("EditModeNotVisible");
                OnPropertyChanged("EditCursorArrowBeam");
                OnPropertyChanged("EditCursorHandBeam");
            }
        }

        //Visibility
        public Visibility EditModeVisible
        {
            get
            {
                if (_editModeChecked)
                    return Visibility.Visible;
                else
                    return Visibility.Collapsed;
            }
        }
        public Visibility EditModeNotVisible
        {
            get
            {
                if (_editModeChecked)
                    return Visibility.Collapsed;
                else
                    return Visibility.Visible;
            }
        }
        //Cursor
        public Cursor EditCursorHandBeam
        {
            get
            {
                if (_editModeChecked)
                    return Cursors.IBeam;
                else
                    return Cursors.Hand;
            }
        }
        public Cursor EditCursorArrowBeam
        {
            get
            {
                if (_editModeChecked)
                    return Cursors.IBeam;
                else
                    return Cursors.Arrow;
            }
        }
        #endregion

        #region Commands
        public ICommand Create_Song { get; set; }
        public ICommand Delete_Song { get; set; }
        #endregion

        public SongLibraryViewModel()
        {
            // Commands Initializer
            Create_Song = new RelayCommand(o => CreateSong(o));
            Delete_Song = new RelayCommand(o => DeleteSong(o));

            // Song Initializer 
            songInterface = new SongInterface();
            Songs = new ObservableCollection<SongData>(songInterface.GetAllSongs());

            if (Songs.Count > 0)
            {
                //Select First Song
                SelectedSong = Songs[0];
                Lyrics = new ObservableCollection<LyricData>(songInterface.GetSongLyrics(SelectedSong));
                SelectedSong.Lyrics = Lyrics.ToList();
            }

        }

        // Update Lyric
        public void RefreshLyrics()
        {
            Lyrics.Clear();
            foreach (var lyric in songInterface.GetSongLyrics(SelectedSong))
            {
                Lyrics.Add(lyric);
            }
            SelectedSong.Lyrics = Lyrics.ToList();
        }

        // Create Song
        public void CreateSong(object sender)
        {
            songInterface.CreateSong("Title", "Author");
            RefreshSongList();
            SelectedSong = Songs[Songs.Count - 1];
        }

        // Delete Song
        public void DeleteSong(object sender)
        {
            songInterface.DeleteSong(SelectedSong.SongID);
            SelectedSong = null;
            RefreshSongList();
        }

        // Save Song
        public void SaveSong()
        {
            SelectedSong.Lyrics = Lyrics.ToList();
            songInterface.SaveSong(SelectedSong);
        }
        // Refresh Song List
        public void RefreshSongList()
        {
            Songs.Clear();
            foreach (SongData song in songInterface.GetAllSongs())
            {
                Songs.Add(song);
            }
        }

    }
}
