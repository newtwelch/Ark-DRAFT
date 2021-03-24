using Ark.Models.SongLibrary;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Ark.ViewModels
{
    public class SongLibraryViewModel : ViewModelBase
    {
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

        public SongLibraryViewModel()
        {
            songInterface = new SongInterface();
            Songs = new ObservableCollection<SongData>(songInterface.GetAllSongs());

            if (Songs.Count > 0)
            {
                //Select First Song
                SelectedSong = Songs[0];
                int i = SelectedSong.SongID;
                Lyrics = new ObservableCollection<LyricData>(songInterface.GetSongLyrics(SelectedSong));
            }
        }

        //Update Lyric
        public void UpdateLyric()
        {
            Lyrics.Clear();
            foreach (var lyric in songInterface.GetSongLyrics(SelectedSong))
            {
                Lyrics.Add(lyric);
            }
        }

    }
}
