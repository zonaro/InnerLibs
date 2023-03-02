using System;

namespace Extensions.Dates
{


    /// <summary>
    /// Behavior for <see cref="DateRange.FilterBehavior"/>
    /// </summary>
    public enum DateRangeFilterBehavior
    {
        /// <summary>
        /// Check if <see cref="DateTime"/> is between <see cref="DateRange.StartDate"/> and <see
        /// cref="DateRange.EndDate"/> but not equal them
        /// </summary>
        BetweenExclusive,

        /// <summary>
        /// Check if <see cref="DateTime"/> is Between or equal <see cref="DateRange.StartDate"/>
        /// and <see cref="DateRange.EndDate"/>
        /// </summary>
        BetweenOrEqual,

        /// <summary>
        /// Check if <see cref="DateTime"/> is greater than or equal <see
        /// cref="DateRange.StartDate"/> and less than <see cref="DateRange.EndDate"/>
        /// </summary>
        Between,
    }

}