using System;
using System.Collections;
using Extensions.Dates;

namespace Extensions
{
    public static partial class Util
    {

        /// <inheritdoc cref="CalculateInterest"/>
        public static V CalculateInterest<V>(this V Capital, string Rate, int Time, bool IsSimpleInterest = true) where V : struct => CalculateInterest(Capital.ChangeType<decimal>(), Rate.ParsePercent<decimal>(), Time, IsSimpleInterest).ChangeType<V>();

        /// <summary>
        /// Calculates the interest or final amount based on the specified capital, interest rate, time period, and
        /// interest type (simple or compound).
        /// </summary>
        /// <remarks>The method parses the rate parameter as a percentage. For compound interest, the
        /// calculation uses the formula: Capital * (1 + rate)^Time. For simple interest, the calculation uses: Capital
        /// * rate * Time. If difference is true, the returned value is the interest earned; otherwise, it is the total
        /// amount.</remarks>
        /// <param name="Capital">The initial principal amount on which interest is calculated.</param>
        /// <param name="Rate">The interest rate, expressed as decimal (e.g., "5%" = "0.05"). Must represent a
        /// non-negative percentage.</param>
        /// <param name="Time">The time period over which interest is calculated, in years. Must be greater than zero to accrue interest.</param>
        /// <param name="IsCompoundInterest">false to calculate simple interest; true to calculate compound interest. The
        /// default is true.</param>
        /// <returns>A double representing either the total amount (principal plus interest) or just the interest earned,
        /// depending on the value of difference. Returns the original capital if the rate or time is not positive.</returns>
        public static decimal CalculateInterest(this decimal Capital, decimal Rate, int Time, bool IsCompoundInterest = true)
        {


            if (IsCompoundInterest)
            {
                // Compound Interest: J = P * ((1 + i)^t - 1)
                decimal amount = Capital * (decimal)Math.Pow((double)(1 + Rate), Time);
                return amount - Capital;
            }
            else
            {
                // Simple Interest: J = P * i * t
                return Capital * Rate * Time;
            }
        }


        public static V CalculateInterest<V>(this V Capital, string Rate, DateRangeInterval interval, DateTime StartDate, DateTime? EndDate = null, bool IsSimpleInterest = true)
         where V : struct
        => CalculateInterest<V>(Capital, Rate.ParsePercent<decimal>(), interval, StartDate, EndDate, IsSimpleInterest);
        public static V CalculateInterest<V>(this V Capital, decimal Rate, DateRangeInterval interval, DateTime StartDate, DateTime? EndDate = null, bool IsSimpleInterest = true)
       where V : struct
        {
            EndDate = EndDate ?? DateTime.Today;
            var Time = new DateRange(StartDate, EndDate.Value).Count(interval);
            return CalculateInterest(Capital.ChangeType<decimal>(), Rate, Time, IsSimpleInterest).ChangeType<V>();
        }


    }
}