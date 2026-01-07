using System;
using System.Collections;
using Extensions.Dates;

namespace Extensions
{
    public static partial class Util
    {

        /// <inheritdoc cref="CalculateInterest"/>
        public static V CalculateInterest<V>(this V Capital,  V Rate, V Time, bool Compound=true, bool Difference = false) where V : struct
        {
            if (typeof(V) == typeof(string))
            {
                return CalculateInterest<double>(Capital.ToDouble(), Rate.ToString().ParsePercent<double>(), Time.ToDouble(), Compound, Difference).ChangeType<V>();
            }
            if (!typeof(V).IsNumber() ) throw new ArgumentException("V needs to be a numeric type",nameof(V));
            return CalculateInterest(Capital.ChangeType<double>(),  Rate.ChangeType<double>(), Time.ChangeType<double>(), Compound, Difference).ChangeType<V>();
        }

        /// <summary>
        /// Calculates the interest or final amount based on the specified capital, interest rate, time period, and
        /// interest type (simple or compound).
        /// </summary>
        /// <remarks>The method parses the rate parameter as a percentage. For compound interest, the
        /// calculation uses the formula: Capital * (1 + rate)^Time. For simple interest, the calculation uses: Capital
        /// * rate * Time. If difference is true, the returned value is the interest earned; otherwise, it is the total
        /// amount.</remarks>
        /// <param name="Capital">The initial principal amount on which interest is calculated.</param>
        /// <param name="Compound">true to calculate compound interest; false to calculate simple interest.</param>
        /// <param name="Rate">The interest rate, expressed as a string in percent format (e.g., "5%" or "0.05"). Must represent a
        /// non-negative percentage.</param>
        /// <param name="Time">The time period over which interest is calculated, in years. Must be greater than zero to accrue interest.</param>
        /// <param name="Difference">true to return only the interest earned; false to return the total amount (principal plus interest). The
        /// default is false.</param>
        /// <returns>A double representing either the total amount (principal plus interest) or just the interest earned,
        /// depending on the value of difference. Returns the original capital if the rate or time is not positive.</returns>
        public static double CalculateInterest(this double Capital,  double Rate, double Time, bool Compound =true, bool Difference = false)

        {
            double final = Capital;        

            if (Rate > 0 && Time > 0)
            {
                if (Compound)
                {
                    final = Capital * Math.Pow(1 + Rate, Time);
                }
                else
                {
                    final = Capital + (Capital * Rate * Time);
                }
            }

            if (Difference)
            {
                final = final - Capital;
            }

            return final;
        }

        public static V CalculateInterest<V>(this V Capital,  V Rate, DateRangeInterval interval, DateTime StartDate, DateTime? EndDate = null, bool Compound= true, bool Difference = false)
       where V : struct
        {
            EndDate = EndDate ?? DateTime.Today;
            var Time = new DateRange(StartDate, EndDate.Value).Count(interval);
            return CalculateInterest(Capital,  Rate, Time.ChangeType<V>(), Compound, Difference);
        }
                

    }
}