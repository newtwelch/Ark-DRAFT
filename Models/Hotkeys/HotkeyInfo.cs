using Ark.Models.Hotkeys.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ark.Models.Hotkeys
{
    class HotkeyInfo
    {
        public Keys Key { get; private set; }
        public Modifiers Modifier { get; private set; }

        private HotkeyInfo(IntPtr lParam)
        {
            var lpInt = (int)lParam;
            Key = (Keys)((lpInt >> 16) & 0xFFFF);
            Modifier = (Modifiers)(lpInt & 0xFFFF);
        }

        public static HotkeyInfo GetFromMessage(int msg, IntPtr lParam)
        {
            return !IsHotkeyMessage(msg) ? null : new HotkeyInfo(lParam);
        }

        public static bool IsHotkeyMessage(int m)
        {
            return m == Constants.WM_HOTKEY_MSG_ID;
        }
    }
}
