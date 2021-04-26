using System.Collections.Generic;
using System.Xml.Linq;
using System.Linq;
using System;

namespace Ark
{
    public class BibleInterface
    {
        XElement bible;

        public BibleInterface() 
        {
            bible = XElement.Load(DataAccessConfiguration.ConnectionStringXMLENG); 
        }

        // Change Language
        public void ChangeLanguage(string language)
        {
            switch (language)
            {
                case "ENGLISH":
                    bible = XElement.Load(DataAccessConfiguration.ConnectionStringXMLENG);
                    break;
                case "TAGALOG":
                    bible = XElement.Load(DataAccessConfiguration.ConnectionStringXMLTAG);
                    break;
            }
        }

        // Find Verses
        public List<BibleData> GetAllBibleData(string Text)
        {
            List<BibleData> list = new List<BibleData>();

            var queryBook = from b in bible.Descendants("VERS") where b.Value.Contains(Text, StringComparison.OrdinalIgnoreCase) select b;

            foreach(var q in queryBook)
            {
                list.Add(new BibleData() { VerseData = new VerseData() { Text = q.Value, VerseNumber = int.Parse(q.Attribute("vnumber").Value) },
                                           ChapterData = new ChapterData() { ChapterNumber = int.Parse(q.Parent.Attribute("cnumber").Value) },
                                           BookData = new BookData() { Name = q.Parent.Parent.Attribute("bname").Value, BookNumber = int.Parse(q.Parent.Parent.Attribute("bnumber").Value) }
                });
            }

            return list;
        }

        // Get books list
        public List<BookData> GetBooks() 
        {
            List<BookData> list = new List<BookData>();

            foreach (var q in bible.Elements())
            {
                
                list.Add(
                        new BookData() 
                        {
                            BookNumber = int.Parse(q.Attribute("bnumber").Value),
                            Name = q.Attribute("bname").Value
                        }
                    );
            }
            return list;
        }

        // Get chapters list 
        public List<ChapterData> GetChapters(int bookNo)
        {
            List<ChapterData> list = new List<ChapterData>();

            var query = from b in bible.Elements() where int.Parse(b.Attribute("bnumber").Value) == bookNo select b;

            foreach (var q in query.Elements())
            {

                list.Add(
                        new ChapterData()
                        {
                            ChapterNumber = int.Parse(q.Attribute("cnumber").Value)
                        }
                    );
            }
            return list;
        }

        // Get verses list 
        public List<VerseData> GetVerses(int bookNo,int chapNo)
        {
            List<VerseData> list = new List<VerseData>();

            var query = from b in bible.Elements() where int.Parse(b.Attribute("bnumber").Value) == bookNo 
                        select b;

            var query1 = from b in query.Elements()
                         where int.Parse(b.Attribute("cnumber").Value) == chapNo
                         select b;


            foreach (var q in query1.Elements())
            {

                list.Add(
                        new VerseData()
                        {
                            VerseNumber = int.Parse(q.Attribute("vnumber").Value),
                            Text = q.Value
                        }
                    );
            }
            return list;
        }
    }
}
