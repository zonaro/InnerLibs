namespace InnerLibs
{
    /// <summary>
    /// Format rules for <see cref="DateRangeDisplay"/>
    /// </summary>
    public enum DateRangeFormatRules
    {
        /// <summary>
        /// Return the full string, including Zeros
        /// </summary>
        FullStringWithZero = 0,

        /// <summary>
        /// Return the full string, but skip zeros. This is the DEFAULT behavior
        /// </summary>
        FullStringSkipZero = 1,

        /// <summary>
        /// If <see cref="DateRange"/> value is greater than 1 day, the hours, minutes and seconds
        /// are ignored
        /// </summary>
        ReduceToDays = 2,

        /// <summary>
        /// If <see cref="DateRange"/> value is greater than 1 month, the days, hours, minutes and
        /// seconds are ignored
        /// </summary>
        ReduceToMonths = 3,

        /// <summary>
        /// If <see cref="DateRange"/> value is greater than 1 year, the months, days, hours,
        /// minutes and seconds are ignored
        /// </summary>
        ReduceToYears = 4
    }
}