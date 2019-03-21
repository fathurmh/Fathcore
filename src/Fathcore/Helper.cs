using Fathcore.Infrastructure;

namespace Fathcore
{
    public static class Helper
    {
        /// <summary>
        /// Sets the static helper instance to the supplied engine. Use this method to supply your own helper implementation.
        /// </summary>
        /// <param name="helper">The helper to use.</param>
        /// <remarks>Only use this method if you know what you're doing.</remarks>
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
                    _ = Singleton<IHelper>.Instance ?? (Singleton<IHelper>.Instance = new Infrastructure.Helper());

                return Singleton<IHelper>.Instance;
            }
        }
    }
}
