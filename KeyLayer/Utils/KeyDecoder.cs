namespace KeyLayer.utils
{
    public static class KeyDecoder
    {
        private static readonly Dictionary<byte, string> keyMap = new()
        {
            { 0x04, "A" }, { 0x05, "B" }, { 0x06, "C" }, { 0x07, "D" }, { 0x08, "E" }, { 0x09, "F" },
            { 0x0A, "G" }, { 0x0B, "H" }, { 0x0C, "I" }, { 0x0D, "J" }, { 0x0E, "K" }, { 0x0F, "L" },
            { 0x10, "M" }, { 0x11, "N" }, { 0x12, "O" }, { 0x13, "P" }, { 0x14, "Q" }, { 0x15, "R" },
            { 0x16, "S" }, { 0x17, "T" }, { 0x18, "U" }, { 0x19, "V" }, { 0x1A, "W" }, { 0x1B, "X" },
            { 0x1C, "Y" }, { 0x1D, "Z" }, { 0x1E, "1" }, { 0x1F, "2" }, { 0x20, "3" }, { 0x21, "4" },
            { 0x22, "5" }, { 0x23, "6" }, { 0x24, "7" }, { 0x25, "8" }, { 0x26, "9" }, { 0x27, "0" },
            { 0x28, "Enter" }, { 0x29, "Escape" }, { 0x2A, "Backspace" }, { 0x2B, "Tab" },
            { 0x2C, "Space" }, { 0x2D, "-" }, { 0x2E, "=" }, { 0x2F, "[" }, { 0x30, "]" },
            { 0x31, "\\" }, { 0x32, "Non-US #" }, { 0x33, ";" }, { 0x34, "'" }, { 0x35, "`" },
            { 0x36, "," }, { 0x37, "." }, { 0x38, "/" }, { 0x39, "Caps Lock" },
            { 0x3A, "F1" }, { 0x3B, "F2" }, { 0x3C, "F3" }, { 0x3D, "F4" }, { 0x3E, "F5" }, { 0x3F, "F6" },
            { 0x40, "F7" }, { 0x41, "F8" }, { 0x42, "F9" }, { 0x43, "F10" }, { 0x44, "F11" }, { 0x45, "F12" },
            { 0x46, "Print Screen" }, { 0x47, "Scroll Lock" }, { 0x48, "Pause" },
            { 0x49, "Insert" }, { 0x4A, "Home" }, { 0x4B, "Page Up" }, { 0x4C, "Delete" },
            { 0x4D, "End" }, { 0x4E, "Page Down" }, { 0x4F, "Right" }, { 0x50, "Left" },
            { 0x51, "Down" }, { 0x52, "Up" }, { 0x53, "Num Lock" }, { 0x54, "Keypad /" },
            { 0x55, "Keypad *" }, { 0x56, "Keypad -" }, { 0x57, "Keypad +" }, { 0x58, "Keypad Enter" },
            { 0x59, "Keypad 1" }, { 0x5A, "Keypad 2" }, { 0x5B, "Keypad 3" }, { 0x5C, "Keypad 4" },
            { 0x5D, "Keypad 5" }, { 0x5E, "Keypad 6" }, { 0x5F, "Keypad 7" }, { 0x60, "Keypad 8" },
            { 0x61, "Keypad 9" }, { 0x62, "Keypad 0" }, { 0x63, "Keypad ." }
        };

        public static string Decode(byte[] report)
        {
            if (report.Length < 3) return "Unknown";
            byte keyCode = report[2];
            return keyMap.TryGetValue(keyCode, out var result) ? result : $"Unknown ({keyCode:X2})";
        }
    }
}
