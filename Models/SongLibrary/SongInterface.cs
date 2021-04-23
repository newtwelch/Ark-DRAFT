using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Ark.Models.SongLibrary
{
    public class SongInterface
    {
        private readonly Reader reader;
        private readonly AdderRemover adderRemover;
        private readonly Saver saver;

        public int CurrentSongID = -1;

        public SongInterface()
        {
            reader = new Reader();
            adderRemover = new AdderRemover();
            saver = new Saver();
        }

        // Getters
        public List<SongData> GetAllSongs()
        {
            return reader.GetSongs();
        }
        // Getters
        public List<SongData> GetAllLanguages(SongData selectedSong)
        {
            return reader.GetSongLanguages(selectedSong);
        }
        public ObservableCollection<SongData> GetSongsBy(string songVariable, string variableValue)
        {
            return new ObservableCollection<SongData>(reader.GetSongsBy(songVariable, variableValue));
        }

        public List<LyricData> GetSongLyrics(SongData selectedSong)
        {
            return reader.GetLyrics(selectedSong);
        }

        // Create and Delete
        public int CreateSong(string Title, string Author)
        {
            CurrentSongID = adderRemover.CreateSong(Title, Author);
            return CurrentSongID;
        }
        public int CreateSongLanguage(SongData selectedSong)
        {
            CurrentSongID = adderRemover.CreateSongLanguage(selectedSong);
            return CurrentSongID;
        }
        public void DeleteSong(int songID)
        {
            adderRemover.DeleteSong(songID);
        }

        // Save Song
        public void SaveSong(SongData selectedSong, bool overrideRawLyric)
        {
            saver.SaveSong(selectedSong, overrideRawLyric);
        }
    }
}
