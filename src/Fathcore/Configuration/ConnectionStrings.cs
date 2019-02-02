using System;
using System.Collections.Generic;
using System.Text;

namespace Fathcore.Configuration
{
    /// <summary>
    /// Represents list of connection strings
    /// </summary>
    public class ConnectionStrings : List<ConnectionString>
    {
        /// <summary>
        /// Get default connection strings.
        /// </summary>
        /// <returns>Returns default connection strings.</returns>
        public string Default => FindLast(nameof(Default));

        /// <summary>
        /// Get connection strings with specified name.
        /// </summary>
        /// <param name="name">Specified name connection strings.</param>
        /// <returns></returns>
        public string this[string name] => FindLast(name);

        private string FindLast(string name)
        {
            if(string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));
            
            return this.FindLast(prop => prop.Name == name).ToString() ?? string.Empty;
        }
    }

    /// <summary>
    /// Represents connection strings
    /// </summary>
    public class ConnectionString
    {
        public string Name { get; set; }
        public string Server { get; set; }
        public string InitialCatalog { get; set; }
        public bool PersistSecurityInfo { get; set; }
        public string UserId { get; set; }
        public string Password { get; set; }
        public bool MultipleActiveResultSets { get; set; }
        public bool Encrypt { get; set; } = true;
        public bool TrustServerCertificate { get; set; }
        public int ConnectionTimeout { get; set; } = 30;

        public override string ToString()
        {
            StringBuilder connectionString = new StringBuilder();
            if (!string.IsNullOrWhiteSpace(Server)) connectionString.Append($"Server={Server};");
            if (!string.IsNullOrWhiteSpace(InitialCatalog)) connectionString.Append($"Initial Catalog={InitialCatalog};");
            if (!string.IsNullOrWhiteSpace(UserId)) connectionString.Append($"User Id={UserId};");
            if (!string.IsNullOrWhiteSpace(Password)) connectionString.Append($"Password={Password};");
            connectionString.Append($"Persist Security Info={PersistSecurityInfo};");
            connectionString.Append($"MultipleActiveResultSets={MultipleActiveResultSets};");
            connectionString.Append($"Encrypt={Encrypt};");
            connectionString.Append($"TrustServerCertificate={TrustServerCertificate};");
            connectionString.Append($"Connection Timeout={ConnectionTimeout};");

            return connectionString.ToString();
        }
    }
}
