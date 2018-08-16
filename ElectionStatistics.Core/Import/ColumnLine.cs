using System;

namespace ElectionStatistics.Core.Import
{
    public class ColumnLine
    {
        // Returns one-based index
        public static int Humanize(int value)
        {
            return value + 1;
        }

        // Returns zero-based index
        public static int Dehumanize(int value)
        {
            return value - 1;
        }

        // Receives zero-based index
        public static string ToLetters(int value)
        {
            if (value <= 0)
            {
                return "A";
            }

            string name = "";
            int mod;
            int div = value + 1;

            while (div > 0)
            {
                mod = (div - 1) % letterBase;
                name = Convert.ToChar('A' + mod).ToString() + name;
                div = (int)((div - mod) / letterBase);
            }

            return name;
        }

        // Returns one-based index
        public static int FromLetters(string column)
        {
            if (String.IsNullOrEmpty(column))
            {
                return 1;
            }

            int value = 0;
            int offset = (int) 'A' - 1;

            int magnitude = 1;
            for (int i = column.Length - 1; i >= 0; i--)
            {
                int code = ((int) column[i]) - offset;
                value = value + code * magnitude;
                magnitude = magnitude * letterBase;
            }

            return value;
        }

        private const int letterBase = 'Z' - 'A' + 1;
    }
}
