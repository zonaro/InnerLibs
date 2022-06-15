using System;
using System.Linq;

namespace InnerLibs
{
    /// <summary>
    /// Define a pair of words for quantify items
    /// </summary>
    public class QuantityTextPair
    {
        /// <summary>
        /// Create a QuantityTextPair with a Plural word and, optionally, Singular Word
        /// </summary>
        /// <param name="Plural">the plural word for the specified item</param>
        /// <param name="Singular">the singular word for specified item. If ommited, is automatically computed by <see cref="Text.QuantifyText(decimal, string)"/>.</param>
        public QuantityTextPair(string Plural, string Singular = "")
        {
            this.Plural = Plural;
            this.Singular = Singular.IfBlank(Plural.QuantifyText(1));
        }

        /// <summary>
        /// Create a default Items/Item pair
        /// </summary>
        public QuantityTextPair()
        {
        }

        public string Plural { get; set; } = "Items";

        public string Singular { get; set; } = "Item";

        /// <summary>
        /// Return the singular or plural word based on <paramref name="Number"/>
        /// </summary>
        /// <param name="Number"></param>
        /// <returns></returns>
        public string this[IComparable Number]
        {
            get
            {
                if (Number.IsNumber())
                {
                    return Number.ToDecimal().Floor().IsIn(1m, -1m) ? Singular : Plural;
                }
                else
                {
                    return "";
                }
            }

            set
            {
                if (Number.IsNumber() && Number.ToDecimal().Floor().IsIn(1m, -1m))
                {
                    Singular = value;
                }
                else
                {
                    Plural = value;
                }
            }
        }

        public override string ToString() => Plural;

        public string ToString(long Number) => this[Number];

        public string ToString(decimal Number) => this[Number];

        public string ToString(short Number) => this[Number];

        public string ToString(int Number) => this[Number];

        public string ToString(double Number) => this[Number];

        public string ToString(float Number) => this[Number];

        public static implicit operator string(QuantityTextPair b) => b.ToString();

        public static implicit operator QuantityTextPair(string b)
        {
            var parts = b.SplitAny(":", "/", ";");
            return new QuantityTextPair(parts.FirstOrDefault().IfBlank("Items"), parts.LastOrDefault().NullIf(parts.FirstOrDefault().IfBlank("")));
        }
    }
}