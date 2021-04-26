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

        public ObservableCollection<BibleData> BibleData { get; set; }

        public ObservableCollection<BookData> Books { get; set; }
        public ObservableCollection<ChapterData> Chapters { get; set; }

        private List<VerseData> localVerse;
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
        //The selected Book;
        private ChapterData _selectedChapter;
        public ChapterData SelectedChapter
        {
            get { return _selectedChapter; }
            set
            {
                _selectedChapter = value;
                OnPropertyChanged("SelectedChapter");
            }
        }

        public BibleViewModel()
        {
            bibleInterface = new BibleInterface();
            BibleData = new ObservableCollection<BibleData>(bibleInterface.GetAllBibleData(""));
            Books = new ObservableCollection<BookData>(bibleInterface.GetBooks());
            Chapters = new ObservableCollection<ChapterData>(bibleInterface.GetChapters(1));
            Verses = new ObservableCollection<VerseData>(bibleInterface.GetVerses(1, 1));
            localVerse = new List<VerseData>(Verses);
        }

        public void GetChapters(int BookNumber)
        {
            Chapters.Clear();
            foreach(var chapter in bibleInterface.GetChapters(BookNumber))
            {
                Chapters.Add(chapter);
            }
        }
        public void GetVerses(int BookNumber, int Chapter)
        {
            Verses.Clear();
            foreach (var chapter in bibleInterface.GetVerses(BookNumber, Chapter))
            {
                Verses.Add(chapter);
            }
            localVerse.Clear();
            localVerse = Verses.ToList();
        }

        public void FindBooks(string bookName)
        {
            List<BookData> aBooks = bibleInterface.GetBooks();

            Books.Clear();
            foreach(var book in aBooks)
            {
                if (book.Name.Contains(bookName, StringComparison.OrdinalIgnoreCase))
                {
                    Books.Add(book);
                }
            }
        }
        public void FindChapters(string chapterNumber)
        {
            List<ChapterData> aChapters = bibleInterface.GetChapters(SelectedBook.BookNumber);

            Chapters.Clear();
            foreach (var chapter in aChapters)
            {
                if (chapter.ChapterNumber.ToString().Contains(chapterNumber, StringComparison.OrdinalIgnoreCase))
                {
                    Chapters.Add(chapter);
                }
            }
        }

        public void FindText(string Text, string Type)
        {
            switch (Type)
            {
                case "Local":
                    List<VerseData> temp = localVerse.Where(x => x.Text.Contains(Text, StringComparison.OrdinalIgnoreCase)).ToList();
                    Verses.Clear();
                    foreach(var verse in temp)
                    {
                        Verses.Add(verse);
                    }
                    break;
                case "Global":
                    foreach (var bdata in bibleInterface.GetAllBibleData(Text))
                    {
                        BibleData.Add(bdata);
                    }
                    break;
            }
        }
        public void ChangeLanguage(string Language)
        {
            bibleInterface.ChangeLanguage(Language);
            Books.Clear();
            foreach(var book in bibleInterface.GetBooks())
            {
                Books.Add(book);
            }
        }
    }
}
