using Ark.Models.SongLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ark.ViewModels
{
    class HistoryWindowViewModel : ViewModelBase
    {
        public List<SongData> SongHistory { get; set; }
        public List<BibleData> BibleHistory { get; set; }
    }
}
