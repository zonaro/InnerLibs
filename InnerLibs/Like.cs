using System;
using System.Text.RegularExpressions;

namespace InnerLibs
{
    internal class Like
    {
        private RegexOptions _Options;
        private String sPattern;

        private static bool IsLike(String value, String mask, RegexOptions options = RegexOptions.Multiline & RegexOptions.IgnorePatternWhitespace)
        {
            String usepattern = "^" + Regex.Escape(mask).Replace("\\*", ".*").Replace("\\?", ".") + "$";
            return Regex.IsMatch(value, usepattern, options);
        }

        public Like(String sPattern, RegexOptions pOptions = RegexOptions.Multiline)
        {
            Pattern = sPattern;
            Options = pOptions;
        }

        public RegexOptions Options
        { get => _Options; private set => _Options = value; }

        public String Pattern
        { get => sPattern; private set => sPattern = value; }

        public static explicit operator Like(String Source)
        {
            return new Like(Source);
        }

        public static bool operator !=(string First, Like second) => !(First == second);

        public static bool operator ==(String first, Like second) => second != null ? second.Matches(first) : false;

        public override bool Equals(object obj) => obj?.ToString() == this;

        public override int GetHashCode() => 0;

        public bool Matches(String test)
        {
            if (test == null) return false;
            return IsLike(test, Pattern, Options);
        }
    }
}