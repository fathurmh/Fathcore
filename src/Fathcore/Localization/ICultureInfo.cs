namespace Fathcore.Localization
{
    /// <summary>
    /// Represents culture info.
    /// </summary>
    public interface ICultureInfo
    {
        /// <summary>
        /// Gets a default culture info.
        /// </summary>
        string Default { get; }

        /// <summary>
        /// Gets a default for UI culture info.
        /// </summary>
        string DefaultUI { get; }
    }
}
