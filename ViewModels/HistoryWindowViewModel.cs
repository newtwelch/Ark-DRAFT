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

        public ObservableDictionary<string, Object> HistoryObjects { get; set; }

        public HistoryWindowViewModel()
        {
            HistoryObjects = new ObservableDictionary<string, Object>();
            BibleHistory = new List<BibleData>();
            prevBible = new BibleData();

            History.AddToHistory += AddToHistory;

            //SongData bloop = new SongData() { Title = "Above All" };

            //BookData testing = new BookData() { Name = "Genesis" };
            //BibleData bleep = new BibleData() { BookData = testing,
            //                                    ChapterData = new ChapterData() { ChapterNumber = 1 },
            //                                    VerseData = new VerseData() { VerseNumber = 1 }
            //};

            //HistoryObjects.Add(bloop.Title, bloop);
            //HistoryObjects.Add($"{ bleep.BookData.Name } { bleep.ChapterData.ChapterNumber }:{ bleep.VerseData.VerseNumber } ", bleep);
        }

        public void AddToHistory(object sender, object obj)
        {
            if(obj is SongData)
            {
                SongData song = (SongData)obj;
                int i = HistoryObjects.Count;
                HistoryObjects.Add($"[{i++}] {song.Title}", song);
                HistoryObjects.Move(HistoryObjects.Count - 1, 0);
            }
            else if(obj is BibleData)
            {
                BibleData bible = (BibleData)obj;
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
