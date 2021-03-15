using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ark.ViewModels
{
    public class SongLibraryViewModel
    {
        public List<string> Lyrics { get; set; }

        public SongLibraryViewModel()
        {
            Lyrics = new List<string>();
            Lyrics.Add("Hanlo");
            Lyrics.Add("Hanlo");
            Lyrics.Add("Hanlo");
            Lyrics.Add("Hanlo");
            Lyrics.Add("Hanlo");
            Lyrics.Add("Hanlo");
        }
    }
}
