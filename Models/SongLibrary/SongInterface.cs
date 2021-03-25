﻿using System;
using System.Collections.Generic;
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
        public void DeleteSong(int songID)
        {
            adderRemover.DeleteSong(songID);
        }

        // Save Song
        public void SaveSong(SongData selectedSong)
        {
            saver.SaveSong(selectedSong);
        }
    }
}
