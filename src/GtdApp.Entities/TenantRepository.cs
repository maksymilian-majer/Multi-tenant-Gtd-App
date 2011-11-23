namespace GtdApp.Entities
{
    using System;
    using System.Data;
    using System.Data.SqlClient;

    public class TenantRepository
    {
        private readonly string _connectionString;

        public TenantRepository(string connectionString)
        {
            this._connectionString = connectionString;
        }

        public Guid GetTenantIdBySubdomain(string subdomain)
        {
            using (var connection = new SqlConnection(this._connectionString))
            {
                var command = new SqlCommand("[dbo].[GetTenantIdBySubdomain]", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.AddWithValue("@subdomain", subdomain);

                connection.Open();
                var result = command.ExecuteScalar();
                    
                return result == null ? Guid.Empty : Guid.Parse(result.ToString());
            }
        }
    }
}
