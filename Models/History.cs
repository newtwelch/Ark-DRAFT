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

        public BookData SelectedBook;
        public ChapterData SelectedChapter;
        public VerseData SelectedVerse;

        public List<SongData> SongLibraryHistory;
        public List<BibleData> BibleHistory;
        public static History Instance { get; private set; }

        static History()
        {
            Instance = new History();
        }

        private History()
        {
            SelectedBook = new BookData();
            SelectedChapter = new ChapterData();
            SelectedVerse = new VerseData();

            SongLibraryHistory = new List<SongData>();
            BibleHistory = new List<BibleData>();
        }

        public void AddSong(SongData song)
        {
            SongLibraryHistory.Add(song);
        }

        public void AddBible(BibleData bibleData)
        {
            BibleHistory.Add(bibleData);
        }
    }
}