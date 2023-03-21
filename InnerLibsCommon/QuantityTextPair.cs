using System;
using System.Collections;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Extensions.ComplexText
{
    /// <summary>
    /// Define a pair of words for quantify items
    /// </summary>
    public class QuantityTextPair : ITuple
    {
        /// <summary>
        /// Create a QuantityTextPair with a Plural word and, optionally, Singular Word
        /// </summary>
        /// <param name="Plural">the plural word for the specified item</param>
        /// <param name="Singular">
        /// the singular word for specified item. If ommited, is automatically computed by <see
        /// cref="Util.QuantifyText(decimal, string)"/>.
        /// </param>
        public QuantityTextPair(string Plural, string Singular = Util.EmptyString)
        {
            this.Plural = Plural;
            this.Singular = Singular.IfBlank(Plural.QuantifyText(1));
        }

        /// <summary>
        /// Create a default Items/Item pair
        /// </summary>
        public QuantityTextPair()
        {
            Plural = "Items";
            Singular = "Item";
        }

        /// <summary>
        /// Return the singular or plural word based on <paramref name="Number"/>
        /// </summary>
        /// <param name="Number"></param>
        /// <returns></returns>
        public string this[IComparable Number]
        {
            get => Number.IsNumber() ? Number.ToDecimal().Floor().IsIn(1m, -1m) ? Singular : Plural : Util.EmptyString;

            set
            {
                if (Number.IsNumber() && Number.ToDecimal().Floor().IsIn(1m, -1m))
                {
                    Singular = value?.ChangeType<string>();
                }
                else
                {
                    Plural = value?.ChangeType<string>();
                }
            }
        }

        public object this[int Number] => this[Number.ToDecimal()];

        public object this[IEnumerable Items] => this[Items?.Cast<object>()?.Count() ?? 0];

        public int Length => 2;
        public string Plural { get; set; }

        public string Singular { get; set; }

        public static implicit operator QuantityTextPair(string[] b) => (QuantityTextPair)b?.SelectJoinString("/");

        public static implicit operator QuantityTextPair(string b)
        {
            var parts = b?.SplitAny(":", "/", ";", ",").WhereNotBlank().Distinct().ToArray() ?? Array.Empty<string>();

            if (parts.Length == 0)
            {
                parts = new[] { "Items", "Item" };
            }

            if (parts.Length == 1)
            {
                parts = new[] { parts.FirstOrDefault(), parts.FirstOrDefault().Singularize() };
            }

            if (parts.Length > 2)
            {
                var reference = parts.FirstOrDefault();

                var otherwords = parts.Skip(1).ToArray();

                var maybeother = otherwords.OrderBy(x => x.LevenshteinDistance(reference)).FirstOrDefault();

                parts = new[] { reference, maybeother };
            }

            parts = parts.OrderBy(x => x.Length).ToArray();

            return new QuantityTextPair(parts.LastOrDefault(), parts.FirstOrDefault());
        }

        public static implicit operator string(QuantityTextPair b) => b?.ToString();

        public override string ToString() => Plural;

        public string ToString(long Number) => this[Number];

        public string ToString(decimal Number) => this[Number];

        public string ToString(short Number) => this[Number] as string;

        public string ToString(int Number) => this[Number] as string;

        public string ToString(double Number) => this[Number];

        public string ToString(float Number) => this[Number];
    }
}