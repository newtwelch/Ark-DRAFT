using Ark.Models;
using Ark.Models.SongLibrary;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ark.ViewModels
{
    class HistoryWindowViewModel : ViewModelBase
    {
        public List<SongData> SongHistory { get; set; }
        public List<BibleData> BibleHistory { get; set; }

        private BibleData prevBible;
        private SongData prevSong;

        public ObservableDictionary<string, Object> HistoryObjects { get; set; }

        public HistoryWindowViewModel()
        {
            HistoryObjects = new ObservableDictionary<string, Object>();
            BibleHistory = new List<BibleData>();
            
            prevBible = new BibleData() { BookData = new BookData(),
                                          ChapterData = new ChapterData(),
                                           VerseData = new VerseData()};
            prevSong = new SongData();

            History.AddToHistory += AddToHistory;

        }

        public void AddToHistory(object sender, object obj)
        {
            if (obj is SongData)
            {
                SongData song = (SongData)obj;
                if (song.SongID == prevSong.SongID)
                {
                }
                else
                {
                    int i = HistoryObjects.Count;
                    HistoryObjects.Add($"[{i++}] {song.Title}", song);
                    HistoryObjects.Move(HistoryObjects.Count - 1, 0);
                    prevSong = song;
                }
            }
            else if (obj is BibleData)
            {
                BibleData bible = (BibleData)obj;

                if (bible.BookData.BookNumber == prevBible.BookData.BookNumber &&
                    bible.ChapterData.ChapterNumber == prevBible.ChapterData.ChapterNumber &&
                    bible.VerseData.VerseNumber == prevBible.VerseData.VerseNumber)
                {
                }
                else
                {
                    prevBible = bible;
                    BibleHistory.Add(bible);
                    int i = BibleHistory.Count;
                    HistoryObjects.Add($"{ bible.BookData.Name } { bible.ChapterData.ChapterNumber }:{ bible.VerseData.VerseNumber }                                               {i++}",
                        bible);
                    HistoryObjects.Move(HistoryObjects.Count - 1, 0);
                }
            }
        }
    }
}
