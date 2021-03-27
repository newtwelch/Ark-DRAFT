using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ark.Models.SongLibrary
{
    public class Saver
    {
        public void SaveSong(SongData selectedSong)
        {

            string Title = selectedSong.Title; // Title of selected song - for saving
            string Author = selectedSong.Author; // Author of selected song - for saving
            string Language = selectedSong.Language;
            int SongID = selectedSong.SongID; // ID of selected song for finding song in the table

            List<LyricData> SequencedLyrics = selectedSong.Lyrics; // Lyrics that is according to sequence - for taking the sequence and unsequencing
            List<LyricData> RawLyricsList = new List<LyricData>(); // List of Lyrics that will be converted to Raw
            string Sequence = ""; // Sequence for the song that will be saved in the database of selected song
            string RawLyrics = ""; // Raw Lyrics String that will be saved in the database of selected song

            #region Lyric Parsing 

            // Save Sequence and Unsequence the Lyrics
            foreach (LyricData lyric in SequencedLyrics)
            {
                // Check if Lyric is a Stanza in the Sequenced Lyrics
                if (lyric.Type == LyricType.Stanza)
                {
                    // If it is a Stanza then add it to the sequence string
                    Sequence += $"S{ lyric.Line },";

                    // If Stanza is first Stanza Then..
                    if (lyric.Line == "1")
                    {
                        // Add new line to the text
                        lyric.Text += "\r\n";
                        // Add it to the Raw Lyrics List
                        RawLyricsList.Add(lyric);

                        // Check if song has a Chorus..
                        if (SequencedLyrics.Any(x => x.Type == LyricType.Chorus))
                        {
                            // If Chorus exists..
                            LyricData chorus = SequencedLyrics.Find(x => x.Type == LyricType.Chorus);
                            string storeLyric = chorus.Text;
                            // Add new lines
                            chorus.Text = $"\r\nCHORUS\r\n{ storeLyric }\r\n";
                            // Then add the chorus to the Raw Lyrics List
                            RawLyricsList.Add(chorus);
                        }
                    }
                    // If Stanza is not the first..
                    else
                    {
                        // Just add it to the list
                        string storeLyric = lyric.Text;
                        // While also adding new lines around it
                        lyric.Text = $"\r\n{ storeLyric }\r\n";
                        RawLyricsList.Add(lyric);
                    }

                }
                // If Lyric is Chorus, add to the sequence
                if (lyric.Type == LyricType.Chorus)
                {
                    Sequence += "C,";
                }
                // If Lyric is Bridge, add to the sequence
                if (lyric.Type == LyricType.Bridge)
                {
                    Sequence += "B,";
                }
            }
            // If a Bridge exists then add it at the very last of Raw Lyrics List
            if (SequencedLyrics.Any(x => x.Type == LyricType.Bridge))
            {
                LyricData bridge = SequencedLyrics.Find(x => x.Type == LyricType.Bridge);
                string storeLyric = bridge.Text;
                // Add new lines around it
                bridge.Text = $"\r\nBRIDGE\r\n{ storeLyric}";
                RawLyricsList.Add(bridge);
            }

            // Combine all the text in Raw Lyrics List
            foreach (LyricData lyric in RawLyricsList)
            {
                RawLyrics += lyric.Text;
            }
            #endregion

            //SQL Stuff 
            try
            {
                using var con = new SQLiteConnection(DataAccessConfiguration.ConnectionString, true);
                con.Open();

                using var cmd = new SQLiteCommand(con);

                // Update Song
                cmd.CommandText = "UPDATE Songs SET Language = @language, Title = @title, Author = @author, Lyrics = @lyrics, Sequence = @sequence WHERE SongID = @songid ";
                cmd.Parameters.AddWithValue("@language", Language);
                cmd.Parameters.AddWithValue("@title", Title);
                cmd.Parameters.AddWithValue("@author", Author);
                cmd.Parameters.AddWithValue("@lyrics", RawLyrics);
                cmd.Parameters.AddWithValue("@songid", SongID);
                cmd.Parameters.AddWithValue("@sequence", Sequence);
                cmd.Prepare();
                cmd.ExecuteNonQuery();

                con.Close();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                System.Diagnostics.Debug.WriteLine(ex.Message);
                System.Diagnostics.Debug.WriteLine(ex.StackTrace);
                System.Diagnostics.Debug.WriteLine(ex.InnerException);
            }
        }
    }
}
