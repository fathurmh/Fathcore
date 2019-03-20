namespace Fathcore.Infrastructure.StartupTask
{
    /// <summary>
    /// Represents startup task interface.
    /// </summary>
    public interface IStartupTask
    {
        /// <summary>
        /// Gets order of this configuration implementation.
        /// </summary>
        int Order { get; }

        /// <summary>
        /// Execute a startup task.
        /// </summary>
        void Execute();
    }
}
