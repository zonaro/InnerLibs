using System;
using System.Text.RegularExpressions;

namespace Extensions.ComplexText
{

    internal class Like
    {
        #region Private Fields

        private RegexOptions _Options;
        private String sPattern;

        #endregion Private Fields

        #region Private Methods

        private static bool IsLike(String value, String mask, RegexOptions options = RegexOptions.Multiline & RegexOptions.IgnorePatternWhitespace)
        {
            String usepattern = "^" + Regex.Escape(mask).Replace("\\*", ".*").Replace("\\?", ".") + "$";
            return Regex.IsMatch(value, usepattern, options);
        }

        #endregion Private Methods

        #region Public Constructors

        public Like(String sPattern, RegexOptions pOptions = RegexOptions.Multiline)
        {
            Pattern = sPattern;
            Options = pOptions;
        }

        #endregion Public Constructors

        #region Public Properties

        public RegexOptions Options
        { get => _Options; private set => _Options = value; }

        public String Pattern
        { get => sPattern; private set => sPattern = value; }

        #endregion Public Properties

        #region Public Methods

        public static explicit operator Like(String Source) => new Like(Source);

        public static bool operator !=(string First, Like second) => !(First == second);

        public static bool operator ==(String first, Like second) => second != null ? second.Matches(first) : false;

        public override bool Equals(object obj) => obj?.ToString() == this;

        public override int GetHashCode() => 0;

        public bool Matches(String test)
        {
            if (test == null) return false;
            return IsLike(test, Pattern, Options);
        }

        #endregion Public Methods
    }

}