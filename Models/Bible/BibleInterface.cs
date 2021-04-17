using System.Collections.Generic;
using System.Xml.Linq;
using System.Linq;

namespace Ark
{
    public class BibleInterface
    {
        XElement bible;

        public BibleInterface() 
        {
            bible = XElement.Load(DataAccessConfiguration.ConnectionStringXMLENG); 
        }

        //Change Language
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

        //get books list
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

        //get chapters list 
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

        //get verses list 
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
