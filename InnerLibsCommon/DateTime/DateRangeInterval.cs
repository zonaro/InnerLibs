

/// <summary>
/// Intervals used <see cref="DateRange.AddInterval(DateRangeInterval, double)(DateRangeInterval)"/>
/// </summary>
public enum DateRangeInterval
{
    /// <summary>
    /// <see cref="DateRange"/> will use the most hight <see cref="DateRangeInterval"/> avaible.
    /// </summary>
    LessAccurate = -1,

    /// <summary>
    /// Milliseconds
    /// </summary>
    Milliseconds = 0,

    /// <summary>
    /// Seconds
    /// </summary>
    Seconds = 1,

    /// <summary>
    /// Minutes
    /// </summary>
    Minutes = 2,

    /// <summary>
    /// Hours
    /// </summary>
    Hours = 3,

    /// <summary>
    /// Days
    /// </summary>
    Days = 4,

    /// <summary>
    /// Weeks
    /// </summary>
    Weeks = 5,

    /// <summary>
    /// Months
    /// </summary>
    Months = 6,

    /// <summary>
    /// Years
    /// </summary>
    Years = 7
}
