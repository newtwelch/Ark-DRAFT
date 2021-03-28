using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace Ark.Models.SongLibrary
{
    public class Reader
    {
        // Get Songs
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
                        SongNum = rdr.GetInt32(1),
                        Language = rdr.GetString(2),
                        Title = rdr.GetString(3),
                        Author = rdr.GetString(4),
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

        // Get Songs by
        public List<SongData> GetSongsBy(string songVariable, string variableValue)
        {

            variableValue = $"%{variableValue}%";

            List<SongData> list = new List<SongData>();
            try
            {
                using var con = new SQLiteConnection(DataAccessConfiguration.ConnectionString, true);
                con.Open();

                using var cmd = new SQLiteCommand(con);

                switch (songVariable)
                {
                    case "Title":
                        // Get song by Title
                        cmd.CommandText = "SELECT * FROM Songs WHERE REPLACE(REPLACE(REPLACE(lower(Title), ',', ''), '.', ''),'!','')  LIKE lower(@title);";
                        cmd.Parameters.AddWithValue("@title", variableValue);
                        cmd.Prepare();
                        using (SQLiteDataReader rdr = cmd.ExecuteReader())
                        {
                            while (rdr.Read())
                            {
                                list.Add(new SongData
                                {
                                    SongID = rdr.GetInt32(0),
                                    SongNum = rdr.GetInt32(1),
                                    Language = rdr.GetString(2),
                                    Title = rdr.GetString(3),
                                    Author = rdr.GetString(4),
                                    RawLyric = rdr.GetString(5),
                                    Sequence = rdr.GetString(6),
                                }
                                );
                            }
                            rdr.Close();
                        }

                        break;

                    case "Author":

                        // Get song by Author
                        cmd.CommandText = "SELECT * FROM Songs WHERE lower(Author) LIKE lower(@author);";
                        cmd.Parameters.AddWithValue("@author", variableValue);
                        cmd.Prepare();
                        using (SQLiteDataReader rdr = cmd.ExecuteReader())
                        {
                            while (rdr.Read())
                            {
                                list.Add(new SongData
                                {
                                    SongID = rdr.GetInt32(0),
                                    SongNum = rdr.GetInt32(1),
                                    Language = rdr.GetString(2),
                                    Title = rdr.GetString(3),
                                    Author = rdr.GetString(4),
                                    RawLyric = rdr.GetString(5),
                                    Sequence = rdr.GetString(6)
                                }
                                );
                            }
                            rdr.Close();
                        }

                        break;

                    case "Lyrics":
                        //variableValue = variableValue.Replace("!", "").Replace(",", "");
                        // Get song by Author
                        cmd.CommandText = $"SELECT * FROM Songs WHERE lower(Lyrics) LIKE lower(@lyrics);";
                        cmd.Parameters.AddWithValue("@lyrics", variableValue);
                        cmd.Prepare();
                        using (SQLiteDataReader rdr = cmd.ExecuteReader())
                        {
                            while (rdr.Read())
                            {
                                variableValue = variableValue.Replace("%", "");
                                string lyric = rdr.GetString(5);
                                lyric = lyric.Substring(lyric.IndexOf(variableValue, StringComparison.InvariantCultureIgnoreCase)).Replace("\n", " ").Replace("\r", " "); ;
                                list.Add(new SongData
                                {
                                    SongID = rdr.GetInt32(0),
                                    SongNum = rdr.GetInt32(1),
                                    Language = rdr.GetString(2),
                                    Title = rdr.GetString(3),
                                    Author = rdr.GetString(4),
                                    RawLyric = rdr.GetString(5),
                                    Sequence = rdr.GetString(6),
                                    SearchedLyric = $"{ lyric.Substring(0, Math.Min(lyric.Length, 35)) }..."
                                }
                                );
                            }
                            rdr.Close();
                        }

                        break;
                }


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
        // Get Lyrics
        public List<LyricData> GetLyrics(SongData selectedSong)
        {
            List<LyricData> Lyrics = new List<LyricData>(); // Lyric list converted from raw lyric
            List<LyricData> SequencedLyrics = new List<LyricData>(); // Lyric list that will be returned
            string rawlyric = "";
            string sequence = "";

            try
            {
                using var con = new SQLiteConnection(DataAccessConfiguration.ConnectionString, true);
                con.Open();

                using var cmd = new SQLiteCommand(con);

                //read the title and authors of the song
                cmd.CommandText = "SELECT * FROM Songs WHERE SongID = @songID;";
                cmd.Parameters.AddWithValue("@songID", selectedSong.SongID);
                cmd.Prepare();
                using SQLiteDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    rawlyric = rdr.GetString(5);
                    sequence = rdr.GetString(6);
                }
                rdr.Close();

                con.Close();

                int verseid = 1; //To determine stanza numbers

                // Parse the RawLyric converting it into a List
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

                // If sequence doesn't exist then do a default one
                if (sequence == "o")
                {
                    // Check for stanzas
                    List<LyricData> stanzas = Lyrics.FindAll(x => x.Type == LyricType.Stanza);

                    // For each stanza add a Chorus if Chorus Exists
                    foreach (LyricData stanza in stanzas)
                    {
                        // Add Stanza 
                        SequencedLyrics.Add(stanza);

                        // Check and add chorus
                        if (Lyrics.Any(x => x.Type == LyricType.Chorus))
                        {
                            LyricData chorus = Lyrics.Find(x => x.Type == LyricType.Chorus);
                            SequencedLyrics.Add(chorus);
                        }
                    }
                    // If Bridge Exists then put it on the very last
                    if (Lyrics.Any(x => x.Type == LyricType.Bridge))
                    {
                        LyricData bridge = Lyrics.Find(x => x.Type == LyricType.Bridge);
                        SequencedLyrics.Add(bridge);
                    }
                }
                else
                // Sequence the lyrics if sequence exists
                {
                    string[] sequencer = sequence.Split(",");

                    foreach (var line in sequencer)
                    {
                        if (SequencedLyrics.Count < paragraphs.Length)
                        {
                            LyricData lyric = Lyrics.Find(x => x.Line == line.ToUpper().Replace("S", ""));
                            SequencedLyrics.Add(lyric);
                        }
                    }
                }
                return SequencedLyrics;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                System.Diagnostics.Debug.WriteLine(ex.Message);
                System.Diagnostics.Debug.WriteLine(ex.StackTrace);
                System.Diagnostics.Debug.WriteLine(ex.InnerException);
                return new List<LyricData>();
            }
        }
    }
}
