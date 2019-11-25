using System;

namespace kk33.RbxStreamSniper
{
    public class ProgressBar
    {
        public static string Generate(int percentage, int maxwidth, string emptyChar = "░", string fullChar = "█")
        {
            int filledChars = (int)Math.Round((decimal)maxwidth / 100 * percentage);
            return RepeatString(fullChar, filledChars) + RepeatString(emptyChar, maxwidth - filledChars);
        }

        static string RepeatString(string s, int n)
        {
            if (n == 0) return "";

            var result = s;

            for (var i = 0; i < n - 1; i++)
            {
                result += s;
            }

            return result;
        }
    }
}