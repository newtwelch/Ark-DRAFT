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
        public static event EventHandler<Object> HistoryEventI;
        public static event EventHandler<Object> HistoryEventII;
        public static event EventHandler<Object> AddToHistory;

        public bool shouldAdd = true;

        public static History Instance { get; private set; }

        static History()
        {
            Instance = new History();
        }

        private History()
        {
        }

        public void HistoryChangeI(Object obj)
        {
            HistoryEventI?.Invoke(this, obj);
        }
        public void HistoryChangeII(Object obj)
        {
            HistoryEventII?.Invoke(this, obj);
            
        }
        public void AddHistory(Object obj)
        {
            if (shouldAdd)
            {
                AddToHistory?.Invoke(this, obj);
            }
            shouldAdd = true;
        }
    }
}