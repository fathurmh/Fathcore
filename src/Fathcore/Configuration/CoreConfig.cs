namespace Fathcore.Configuration
{
    /// <summary>
    /// Represents startup core configuration parameters
    /// </summary>
    public class CoreConfig
    {
        /// <summary>
        /// Gets the core connection strings
        /// </summary>
        public ConnectionStrings ConnectionStrings => EngineContext.Current.Resolve<ConnectionStrings>();

        /// <summary>
        /// Gets the core culture info
        /// </summary>
        public CultureInfo CultureInfo => EngineContext.Current.Resolve<CultureInfo>();

        /// <summary>
        /// Gets the core JWT authentication
        /// </summary>
        public JwtAuthentication JwtAuthentication => EngineContext.Current.Resolve<JwtAuthentication>();
    }
}
