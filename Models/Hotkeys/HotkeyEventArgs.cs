using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ark.Models.Hotkeys
{
    class HotkeyEventArgs
    {
        public HotkeyInfo HotkeyInfo { get; private set; }
        public Hotkey Hotkey { get; private set; }

        public HotkeyEventArgs(Hotkey hotkey, HotkeyInfo info)
        {
            HotkeyInfo = info;
            Hotkey = hotkey;
        }
    }
}

