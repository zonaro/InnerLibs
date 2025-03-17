namespace Extensions
{




    /// <summary>
    /// Represents the level of a password.
    /// </summary>
    public enum PasswordLevel
    {
        /// <summary>
        /// No password level.
        /// </summary>
        None,

        /// <summary>
        /// Very weak password level.
        /// </summary>
        VeryWeak = 2,

        /// <summary>
        /// Weak password level.
        /// </summary>
        Weak = 3,

        /// <summary>
        /// Medium Weak level.
        /// </summary>
        MediumWeak = 4,

        /// <summary>
        /// Medium level.
        /// </summary>
        Medium = 5,

        /// <summary>
        /// Medium Strong level.
        /// </summary>
        MediumStrong = 6,

        /// <summary>
        /// Strong password level.
        /// </summary>
        Strong = 7,

        /// <summary>
        /// Very strong password level.
        /// </summary>
        VeryStrong = 8
    }

}