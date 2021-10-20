using System;
using Microsoft.VisualBasic.CompilerServices;

namespace InnerLibs
{
    public class QuantityTextPair
    {
        public QuantityTextPair(string Plural, string Singular = "")
        {
            this.Plural = Plural;
            this.Singular = Singular.IfBlank(Plural.QuantifyText(1));
        }

        public QuantityTextPair()
        {
        }

        public string this[IComparable Number]
        {
            get
            {
                return ToString(Conversions.ToDecimal(Number));
            }

            set
            {
                if (Number.IsNumber() && Conversions.ToDecimal(Number).Floor().IsIn(1m, -1))
                {
                    Singular = value;
                }
                else
                {
                    Plural = value;
                }
            }
        }

        public string Singular { get; set; } = "Item";
        public string Plural { get; set; } = "Items";

        public override string ToString()
        {
            return Plural;
        }

        public string ToString(long Number)
        {
            return Number.IsIn(1L, -1L) ? Singular : Plural;
        }

        public string ToString(decimal Number)
        {
            return Number.Floor().IsIn(1m, -1m) ? Singular : Plural;
        }

        public string ToString(short Number)
        {
            return Number.IsIn((short)1, (short)-1) ? Singular : Plural;
        }

        public string ToString(int Number)
        {
            return Number.IsIn(1, -1) ? Singular : Plural;
        }

        public string ToString(double Number)
        {
            return Number.Floor().IsIn(1d, -1d) ? Singular : Plural;
        }

        public string ToString(float Number)
        {
            return Number.IsIn(1f, -1f) ? Singular : Plural;
        }
    }
}