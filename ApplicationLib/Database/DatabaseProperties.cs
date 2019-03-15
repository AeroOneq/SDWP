
namespace ApplicationLib.Database
{
    public static class DatabaseProperties
    {
        #region Connection Parameters
        private static string ServerName { get; } = "31.31.196.199";
        private static string InitialCatalog { get; } = "u0661866_sdwpdb";
        private static string IntegratedSecurity { get; } = "False";
        private static string UserID { get; } = "u0661866_AeroOne";
        private static string Password { get; } = "Aero123";
        private static string TrustedConnection { get; } = "False";
        private static string Encrypt { get; } = "False";
        #endregion

        public static string ConnectionString { get; } = 
            $"Data Source={ServerName}; Initial Catalog={InitialCatalog}; " +
            $"Integrated Security={IntegratedSecurity}; User ID={UserID}; " +
            $"Password={Password}; TrustServerCertificate={TrustedConnection}; Encrypt={Encrypt};";
    }
}
