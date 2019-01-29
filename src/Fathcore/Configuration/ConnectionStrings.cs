using System.Collections.Generic;
using System.Text;

namespace Fathcore.Configuration
{
    public class ConnectionStrings : List<ConnectionString>
    {
        public const string Default = "Default";
        public string this[string name] => FindLast(prop => prop.Name == name).ToString();
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
