using Ark.Models.SongLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ark.Models
{
    public class History
    {
        public static SongData SelectedSong;
        public static BookData SelectedBook;
        public static ChapterData SelectedChapter;
        public static VerseData SelectedVerse;
        
        public static List<SongData> SongLibraryHistory { get; set; }
        public static List<BibleData> BibleHistory { get; set; }

        public static void AddSong(SongData song)
        {
            SongLibraryHistory.Add(song);
        }

        public static void AddBible(BibleData bibleData)
        {
            BibleHistory.Add(bibleData);
        }

    }
}