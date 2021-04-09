using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ark.Models.SongLibrary
{
    public class AdderRemover
    {
        public int CreateSong(string Title, string Author)
        {
            try
            {
                using var con = new SQLiteConnection(DataAccessConfiguration.ConnectionString, true);
                con.Open();

                using var cmd = new SQLiteCommand(con);

                //inserting the song into the song list table
                cmd.CommandText = "INSERT INTO Songs (SongNum,Title,Author) VALUES ((select max(SongNum) + 1 from Songs),@title,@author);";
                cmd.Parameters.AddWithValue("@title", Title);
                cmd.Parameters.AddWithValue("@author", Author);
                cmd.Prepare();
                cmd.ExecuteNonQuery();

                //read the id of the song
                cmd.CommandText = "SELECT SongID FROM Songs WHERE Title = @title AND Author = @author;";
                cmd.Parameters.AddWithValue("@title", Title);
                cmd.Parameters.AddWithValue("@author", Author);
                cmd.Prepare();
                using SQLiteDataReader rdr = cmd.ExecuteReader();
                int id = -1;
                _ = rdr.Read();
                id = rdr.GetInt32(0);//check if works!
                rdr.Close();

                con.Close();
                return id;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                System.Diagnostics.Debug.WriteLine(ex.Message);
                System.Diagnostics.Debug.WriteLine(ex.StackTrace);
                System.Diagnostics.Debug.WriteLine(ex.InnerException);
                return -1;
            }
        }

        public int CreateSongLanguage(SongData selectedSong)
        {
            try
            {
                using var con = new SQLiteConnection(DataAccessConfiguration.ConnectionString, true);
                con.Open();

                using var cmd = new SQLiteCommand(con);

                //inserting the song into the song list table
                cmd.CommandText = "INSERT INTO Songs (SongNum,Title,Author) VALUES (@songNum,@title,@author);";
                cmd.Parameters.AddWithValue("@title", selectedSong.Title + " - Language");
                cmd.Parameters.AddWithValue("@author", selectedSong.Author); 
                cmd.Parameters.AddWithValue("@songNum", selectedSong.SongNum);
                cmd.Prepare();
                cmd.ExecuteNonQuery();

                //read the id of the song
                cmd.CommandText = "SELECT SongID FROM Songs WHERE Title = @title AND Author = @author;";
                cmd.Parameters.AddWithValue("@title", selectedSong.Title + " - Language");
                cmd.Parameters.AddWithValue("@author", selectedSong.Author);
                cmd.Prepare();
                using SQLiteDataReader rdr = cmd.ExecuteReader();
                int id = -1;
                _ = rdr.Read();
                id = rdr.GetInt32(0);//check if works!
                rdr.Close();

                con.Close();
                return id;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                System.Diagnostics.Debug.WriteLine(ex.Message);
                System.Diagnostics.Debug.WriteLine(ex.StackTrace);
                System.Diagnostics.Debug.WriteLine(ex.InnerException);
                return -1;
            }
        }

        public void DeleteSong(int songID)
        {
            try
            {
                using var con = new SQLiteConnection(DataAccessConfiguration.ConnectionString, true);
                con.Open();

                using var cmd = new SQLiteCommand(con);

                //delete song from the song list table
                cmd.CommandText = "DELETE FROM Songs WHERE SongID = @songID;";
                cmd.Parameters.AddWithValue("@songID", songID);
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
                return;
            }
        }
    }
}
