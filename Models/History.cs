using Ark.Models.SongLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ark.Models
{
    public class History
    {
        public static event EventHandler<Object> HistoryEvent;
        public static event EventHandler<Object> AddToHistory;

        public static History Instance { get; private set; }

        static History()
        {
            Instance = new History();
        }

        private History()
        {
        }

        public void HistoryChange(Object obj)
        {
            HistoryEvent?.Invoke(this, obj);
        }
        public void AddHistory(Object obj)
        {
            AddToHistory?.Invoke(this, obj);
        }
    }
}