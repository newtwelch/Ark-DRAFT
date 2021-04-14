using Ark.Model.Helper;
using Ark.Models.SongLibrary;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Ark.ViewModels
{
    public class SongLibraryViewModel : ViewModelBase
    {
        #region SongData
        public SongInterface songInterface;

        //List Of Songs
        public ObservableCollection<SongData> Songs { get; set; }
        public ObservableCollection<SongData> SongLanguages { get; set; }
        //The selected Song;
        private SongData _selectedSong;
        public SongData SelectedSong
        {
            get { return _selectedSong; }
            set
            {
                _selectedSong = value;
                OnPropertyChanged("SelectedSong");
            }
        }

        //List of Lyrics
        public ObservableCollection<LyricData> Lyrics { get; set; }
        #endregion

        #region EditMode
        //Edit Mode Check
        private bool _editModeChecked;
        public bool EditModeChecked
        {
            get { return _editModeChecked; }
            set
            {
                _editModeChecked = value;
                OnPropertyChanged("EditModeChecked");
                OnPropertyChanged("EditModeVisible");
                OnPropertyChanged("EditModeNotVisible");
                OnPropertyChanged("EditCursorArrowBeam");
                OnPropertyChanged("EditCursorHandBeam");
            }
        }

        //Visibility
        public Visibility EditModeVisible
        {
            get
            {
                if (_editModeChecked)
                    return Visibility.Visible;
                else
                    return Visibility.Collapsed;
            }
        }
        public Visibility EditModeNotVisible
        {
            get
            {
                if (_editModeChecked)
                    return Visibility.Collapsed;
                else
                    return Visibility.Visible;
            }
        }
        //Cursor
        public Cursor EditCursorHandBeam
        {
            get
            {
                if (_editModeChecked)
                    return Cursors.IBeam;
                else
                    return Cursors.Hand;
            }
        }
        public Cursor EditCursorArrowBeam
        {
            get
            {
                if (_editModeChecked)
                    return Cursors.IBeam;
                else
                    return Cursors.Arrow;
            }
        }
        #endregion

        #region Commands
        public ICommand Create_Song { get; set; }
        public ICommand Add_SongLanguage { get; set; }
        public ICommand Delete_Song { get; set; }
        public ICommand Add_Lyric { get; set; }
        #endregion

        public SongLibraryViewModel()
        {
            // Commands Initializer
            Create_Song = new RelayCommand(o => CreateSong(o));
            Add_SongLanguage = new RelayCommand(o => AddSongLanguage(o));
            Delete_Song = new RelayCommand(o => DeleteSong(o));
            Add_Lyric = new RelayCommand(o => AddLyric(o));

            // Song Initializer 
            songInterface = new SongInterface();
            Songs = new ObservableCollection<SongData>(songInterface.GetAllSongs());

            if (Songs.Count > 0)
            {
                //Select First Song
                SelectedSong = Songs[0];
                Lyrics = new ObservableCollection<LyricData>(songInterface.GetSongLyrics(SelectedSong));
                SongLanguages = new ObservableCollection<SongData>(songInterface.GetAllLanguages(SelectedSong));
                SelectedSong.Lyrics = Lyrics.ToList();
            }

        }

        // Refresh Lyric
        public void RefreshLyrics()
        {
            Lyrics.Clear();
            foreach (var lyric in songInterface.GetSongLyrics(SelectedSong))
            {
                Lyrics.Add(lyric);
            }
            SelectedSong.Lyrics = Lyrics.ToList();
        }  
        public void RefreshLocalLyrics()
        {
            List<LyricData> store = Lyrics.ToList();
            Lyrics.Clear();
            foreach(var lyric in store)
            {
                Lyrics.Add(lyric);
            }
        }
        // Refresh Languages
        public void RefreshLanguages()
        {
            SongLanguages.Clear();
            foreach (SongData song in songInterface.GetAllLanguages(SelectedSong))
            {
                SongLanguages.Add(song);
            }
        }

        // Create Song
        public void CreateSong(object sender)
        {
            songInterface.CreateSong("Title", "Author");
            RefreshSongList();
            SelectedSong = Songs[Songs.Count - 1];
            RefreshLanguages();
            RefreshLyrics();
            // For some reason this helps with auto selecting last index when a song is created
            // I should look for a more proper solution to this!
            EditModeChecked = true;
            EditModeChecked = false;
            EditModeChecked = true;
        }
        // Add Song Language
        public void AddSongLanguage(object sender)
        {
            songInterface.CreateSongLanguage(SelectedSong);
            RefreshSongList();
            SelectedSong = Songs[Songs.Count - 1];
            RefreshLanguages();
            RefreshLyrics();
            // For some reason this helps with auto selecting last index when a song is created
            // I should look for a more proper solution to this!
            EditModeChecked = true;
            EditModeChecked = false;
            EditModeChecked = true;
        }
        public void AddLyric(object sender)
        {
            switch (sender.ToString())
            {
                case "Stanza":

                    int lineNumber = 1;
                    foreach (LyricData lyric in Lyrics)
                    {
                        if (lyric.Type == LyricType.Stanza)
                            lineNumber++;
                    }

                    Lyrics.Add(new LyricData
                    {
                        Line = lineNumber++.ToString(),
                        Text = "Stanza",
                        Type = LyricType.Stanza
                    });
                    break;

                case "Chorus":

                    if (Lyrics.Any(x => x.Type == LyricType.Chorus))
                    {
                        LyricData chorus = Lyrics.ToList().Find(x => x.Type == LyricType.Chorus);
                        Lyrics.Add(chorus);
                    }
                    else
                    {
                        Lyrics.Add(new LyricData
                        {
                            Line = "C",
                            Text = "Chorus",
                            Type = LyricType.Chorus
                        });
                    }

                    break;

                case "Bridge":

                    if (Lyrics.Any(x => x.Type == LyricType.Bridge))
                    {
                        LyricData bridge = Lyrics.ToList().Find(x => x.Type == LyricType.Bridge);
                        Lyrics.Add(bridge);
                    }
                    else
                    {
                        Lyrics.Add(new LyricData
                        {
                            Line = "B",
                            Text = "Bridge",
                            Type = LyricType.Bridge
                        });
                    }

                    break;

            }
        }
        // Delete Song
        public void DeleteSong(object sender)
        {
            songInterface.DeleteSong(SelectedSong.SongID);
            SelectedSong = null;
            RefreshSongList();
        }

        // Save Song
        public void SaveSong()
        {
            SelectedSong.Lyrics = Lyrics.ToList();
            songInterface.SaveSong(SelectedSong);
        }

        // Get Song By
        public void GetSongsBy(string songVariable, string variableValue)
        {
           Songs.Clear();
           foreach (SongData song in songInterface.GetSongsBy(songVariable, variableValue))
           {
               Songs.Add(song);
           }
        }

        // Refresh Song List
        public void RefreshSongList()
        {
            Songs.Clear();
            foreach (SongData song in songInterface.GetAllSongs())
            {
                Songs.Add(song);
            }
        }

    }
}
