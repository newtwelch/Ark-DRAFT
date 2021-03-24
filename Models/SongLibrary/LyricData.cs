using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Ark.Models.SongLibrary
{
    public class LyricData
    {
        public string Line { get; set; }
        public string Text { get; set; }
        public LyricType Type { get; set; }
    }

    //Enum for the type of Lyrics
    public enum LyricType { Bridge, Stanza, Chorus }

    class EnumConvertor
    {
        public static string SongEnumToString(LyricType e)
        {
            return e switch
            {
                LyricType.Bridge => "Bridge",
                LyricType.Chorus => "Chorus",
                LyricType.Stanza => "Stanza",
                _ => throw new ArgumentNullException()
            };
        }

        public static LyricType StringToSongEnum(string s)
        {
            return s switch
            {
                "Bridge" => LyricType.Bridge,
                "Chorus" => LyricType.Chorus,
                "Stanza" => LyricType.Stanza,
                "bridge" => LyricType.Bridge,
                "chorus" => LyricType.Chorus,
                "stanza" => LyricType.Stanza,
                _ => throw new ArgumentException()
            };
        }
    }
}
