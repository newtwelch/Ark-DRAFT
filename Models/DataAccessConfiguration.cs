using System.IO;

namespace Ark
{
    class DataAccessConfiguration
    {
        public static readonly string ConnectionString;
        public static readonly string ConnectionStringXMLENG;
        public static readonly string ConnectionStringXMLTAG;

        static DataAccessConfiguration()
        {
            string relativePath = @"Databases\SongDatabase.db";
            string absolutePath = Path.Combine(Directory.GetCurrentDirectory(), relativePath);
            ConnectionString = string.Format("Data Source={0};Version=3;", absolutePath);


            relativePath = @"Databases\Bible_English_TMB.xml";
            ConnectionStringXMLENG = Path.Combine(Directory.GetCurrentDirectory(), relativePath);

            relativePath = @"Databases\Bible_Tagalog_ADB.xml";
            ConnectionStringXMLTAG = Path.Combine(Directory.GetCurrentDirectory(), relativePath);
        }
    }
}