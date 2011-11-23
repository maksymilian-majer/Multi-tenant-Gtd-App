namespace GtdApp.Entities
{
    using System;
    using System.Data;
    using System.Data.EntityClient;
    using System.Data.SqlClient;

    partial class GtdAppDataContext
    {
        public GtdAppDataContext(Guid? tenantId) : this ()
        {
            if (this.Connection.State != ConnectionState.Open)
            {
                this.Connection.Open(); // open connection if not already open
            }

            var connection = ((EntityConnection)this.Connection).StoreConnection;

            using (var cmd = connection.CreateCommand())
            {
                // run stored procedure to set ContextInfo to tenantId
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "[dbo].[SetCurrentTenantID]";
                cmd.Parameters.Add(new SqlParameter("@tenantId", tenantId));
                cmd.ExecuteNonQuery();
            }
        }
    }
}
