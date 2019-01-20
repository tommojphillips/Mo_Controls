namespace TommoJProductions.Debugging
{
    /// <summary>
    /// Represents debug data to display in the mod console.
    /// </summary>
    internal enum DebugTypeEnum
    {
        /// <summary>
        /// Represents no debugging data.
        /// </summary>
        none,
        /// <summary>
        /// Represents partial debugging data. (sorta user friendly).
        /// </summary>
        partial,
        /// <summary>
        /// Represents full debugging data. (spam; dev; bug tracking).
        /// </summary>
        full,
    }
}
