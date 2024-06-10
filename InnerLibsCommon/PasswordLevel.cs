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
        /// Medium password level.
        /// </summary>
        Medium = 4,

        /// <summary>
        /// Strong password level.
        /// </summary>
        Strong = 5,

        /// <summary>
        /// Very strong password level.
        /// </summary>
        VeryStrong = 6
    }

}