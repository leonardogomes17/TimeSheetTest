using System.Data;
using Oracle.ManagedDataAccess.Client;

namespace TimeSheetTest.Data
{
    public sealed class DatabaseConnection : IDisposable
    {
        public IDbConnection Connection { get; }
        public IDbTransaction Transaction { get; set; }

        public DatabaseConnection()
        {
            string connectionString = "Data Source=localhost:1521; User Id=SYS; Password=OraclePassword123; DBA Privilege=SYSDBA";
            Connection = new OracleConnection(connectionString);
            Connection.Open();
        }
        public void Dispose()
        {
            Connection?.Dispose();
        }
    }
}
