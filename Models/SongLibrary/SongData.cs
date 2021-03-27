using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ark.Models.SongLibrary
{
    public class SongData
    {
        public int SongID { get; set; }
        public int SongNum { get; set; }
        public string Language { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string RawLyric { get; set; }
        public string Sequence { get; set; }
        public string SearchedLyric { get; set; }
        public List<LyricData> Lyrics { get; set; }
    }
}
