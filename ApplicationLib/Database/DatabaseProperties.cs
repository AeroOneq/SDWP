
namespace ApplicationLib.Database
{
    public static class DatabaseProperties
    {
        #region Connection Parameters
        private static string ServerName { get; } = "odmainserver.database.windows.net";
        private static string InitialCatalog { get; } = "ODdb";
        private static string IntegratedSecurity { get; } = "True";
        private static string UserID { get; } = "Aero";
        private static string Password { get; } = "OdWyitBuodKidV6";
        private static string TrustedConnection { get; } = "False";
        private static string Encrypt { get; } = "True";
        #endregion
        public static string ConnectionString { get; } = 
            $"Data Source={ServerName}; Initial Catalog={InitialCatalog}; " +
            $"Integrated Security={IntegratedSecurity}; User ID={UserID}; " +
            $"Password={Password}; Trusted_Connection={TrustedConnection}; Encrypt={Encrypt};";
        public static char Devider { get; } = ':';
    }
}
