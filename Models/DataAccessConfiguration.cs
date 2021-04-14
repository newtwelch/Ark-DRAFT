using System.IO;

namespace Ark
{
    class DataAccessConfiguration
    {
        public static readonly string ConnectionString;
        public static readonly string ConnectionStringXML;

        static DataAccessConfiguration()
        {
            string relativePath = @"Databases\SongDatabase.db";
            string absolutePath = Path.Combine(Directory.GetCurrentDirectory(), relativePath);
            ConnectionString = string.Format("Data Source={0};Version=3;", absolutePath);


            relativePath = @"Databases\Bible_English_AKJV.xml";
            ConnectionStringXML = Path.Combine(Directory.GetCurrentDirectory(), relativePath);
        }
    }
}