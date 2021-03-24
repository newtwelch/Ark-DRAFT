using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text.RegularExpressions;

namespace Ark.Models.SongLibrary
{
    public class Reader
    {
        public List<SongData> GetSongs()
        {
            List<SongData> list = new List<SongData>();
            try
            {
                using var con = new SQLiteConnection(DataAccessConfiguration.ConnectionString, true);
                con.Open();

                using var cmd = new SQLiteCommand(con);

                //read the title and authors of the song
                cmd.CommandText = "SELECT * FROM Songs;";
                cmd.Prepare();
                using SQLiteDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    list.Add(new SongData
                    {
                        SongID = rdr.GetInt32(0),
                        Title = rdr.GetString(2),
                        Author = rdr.GetString(3),
                        RawLyric = rdr.GetString(5),
                        Sequence = rdr.GetString(6)
                    }
                    );
                }
                rdr.Close();

                con.Close();
                return list;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                System.Diagnostics.Debug.WriteLine(ex.Message);
                System.Diagnostics.Debug.WriteLine(ex.StackTrace);
                System.Diagnostics.Debug.WriteLine(ex.InnerException);
                return new List<SongData>();
            }
        }

        //Get Lyrics
        public List<LyricData> GetLyrics(SongData selectedSong)
        {
            string rawlyric = selectedSong.RawLyric;
            string sequence = selectedSong.Sequence;

            int verseid = 1;
            List<LyricData> Lyrics = new List<LyricData>();
            List<LyricData> SequencedLyrics = new List<LyricData>();


            // PARSE THE STRING ARRAY AND MAKE INTO LIST
            string[] paragraphs = Array.FindAll(Regex.Split(rawlyric, "(\r?\n){2,}", RegexOptions.Multiline), p => !String.IsNullOrWhiteSpace(p));
            foreach (string paragraph in paragraphs)
            {
                if (paragraph.StartsWith("CHORUS"))
                {
                    Lyrics.Add(new LyricData() { Line = "C", Text = System.Text.RegularExpressions.Regex.Replace(paragraph, "^(.*\n){1}", ""), Type = LyricType.Chorus });
                }
                else if (paragraph.StartsWith("BRIDGE"))
                {
                    Lyrics.Add(new LyricData() { Line = "B", Text = System.Text.RegularExpressions.Regex.Replace(paragraph, "^(.*\n){1}", ""), Type = LyricType.Bridge });
                }
                else
                {
                    Lyrics.Add(new LyricData() { Line = verseid++.ToString(), Text = paragraph, Type = LyricType.Stanza });
                }
            }

            if (sequence == null || sequence == " ")
            {
                sequence = "S1,C,S2,C,S3,C,S4";
            }

            if (!Lyrics.Any(x => x.Type == LyricType.Chorus))
            {
                sequence = "S1,S2,S3,S4,S5";
            }

            // SEQUENCE THE LYRICS
            string[] sequencer = sequence.Split(",");

            foreach (var line in sequencer)
            {
                if (SequencedLyrics.Count < paragraphs.Length)
                {
                    LyricData lyric = Lyrics.Find(x => x.Line == line.ToUpper().Replace("S", ""));
                    SequencedLyrics.Add(lyric);
                }
            }

            return SequencedLyrics;
        }
    }
}
