using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ark.ViewModels
{
    class BibleViewModel : ViewModelBase
    {

        public BibleInterface bibleInterface;

        public ObservableCollection<BookData> Books { get; set; }
        public ObservableCollection<ChapterData> Chapters { get; set; }
        public ObservableCollection<VerseData> Verses { get; set; }
        //The selected Book;
        private BookData _selectedBook;
        public BookData SelectedBook
        {
            get { return _selectedBook; }
            set
            {
                _selectedBook = value;
                OnPropertyChanged("SelectedBook");
            }
        }

        public BibleViewModel()
        {
            bibleInterface = new BibleInterface();
            Books = new ObservableCollection<BookData>(bibleInterface.GetBooks());
            Chapters = new ObservableCollection<ChapterData>(bibleInterface.GetChapters(1));
            Verses = new ObservableCollection<VerseData>(bibleInterface.GetVerses(1, 1));
        }

        public void GetChapters(int BookNumber)
        {
            Chapters.Clear();
            foreach(var chapter in bibleInterface.GetChapters(BookNumber))
            {
                Chapters.Add(chapter);
            }
        }
    }
}
