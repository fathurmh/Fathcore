namespace Fathcore.Infrastructure
{
    /// <summary>
    /// Provides access to the singleton instance of the helper.
    /// </summary>
    public static class HelperContext
    {
        /// <summary>
        /// Sets the static helper instance to the supplied helper. Use this method to supply your own helper implementation.
        /// <remarks>Only use this method if you know what you're doing.</remarks>
        /// </summary>
        /// <param name="helper">The helper to use.</param>
        public static void Replace(IHelper helper)
        {
            Singleton<IHelper>.Instance = helper;
        }

        /// <summary>
        /// Gets the singleton helper.
        /// </summary>
        public static IHelper Current
        {
            get
            {
                if (Singleton<IHelper>.Instance == null)
                    _ = Singleton<IHelper>.Instance ?? (Singleton<IHelper>.Instance = new Helper());

                return Singleton<IHelper>.Instance;
            }
        }
    }
}
