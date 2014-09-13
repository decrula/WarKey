using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WarKey
{
    public static class KeyboardDescription
    {
        private static char[] Alphabet = new char[] { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };
        private static Dictionary<int, string> KeyToDescriptionDictionary = new Dictionary<int, string>();
        private static Dictionary<string, int> DescriptionToKeyDictionary = new Dictionary<string, int>();

        static KeyboardDescription()
        {
            InitializeKeyToDescriptionDictionary();
            InitializeDescriptionToKeyDictionary();
        }

        private static void InitializeKeyToDescriptionDictionary()
        {
            KeyToDescriptionDictionary.Add(9, "换");
            KeyToDescriptionDictionary.Add(20, "锁");
            KeyToDescriptionDictionary.Add(32, "空");
            KeyToDescriptionDictionary.Add(112, "F1");
            KeyToDescriptionDictionary.Add(113, "F2");
            KeyToDescriptionDictionary.Add(114, "F3");
            KeyToDescriptionDictionary.Add(115, "F4");
            KeyToDescriptionDictionary.Add(192, "`");

            // 字母 A-Z
            for (int i = 65; i <= 90; i++)
            {
                KeyToDescriptionDictionary.Add(i, Alphabet[i - 65].ToString());
            }

            // 字母键盘区数字
            for (int i = 48; i <= 57; i++)
                KeyToDescriptionDictionary.Add(i, (i - 48).ToString());

            // 数字键盘区数字
            for (int i = 96; i <= 105; i++)
                KeyToDescriptionDictionary.Add(i, "D" + (i - 96));
        }

        private static void InitializeDescriptionToKeyDictionary()
        {
            foreach (var pair in KeyToDescriptionDictionary)
            {
                DescriptionToKeyDictionary.Add(pair.Value, pair.Key);
            }
        }

        public static string GetDescription(int keyValue)
        {
            if (KeyToDescriptionDictionary.ContainsKey(keyValue))
                return KeyToDescriptionDictionary[keyValue];

            return ((char)keyValue).ToString();
        }

        public static int GetKey(string description)
        {
            if (DescriptionToKeyDictionary.ContainsKey(description))
                return DescriptionToKeyDictionary[description];

            return 0;
        }
    }
}
