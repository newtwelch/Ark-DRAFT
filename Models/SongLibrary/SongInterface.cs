using System;
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

        public SongInterface()
        {
            reader = new Reader();
        }

        public List<SongData> GetAllSongs()
        {
            return reader.GetSongs();
        }

        public List<LyricData> GetSongLyrics(SongData selectedSong)
        {
            return reader.GetLyrics(selectedSong);
        }
    }
}
