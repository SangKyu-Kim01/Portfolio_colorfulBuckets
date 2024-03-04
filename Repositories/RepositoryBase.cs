using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PaintShopManagement.Models;

namespace PaintShopManagement.Repositories
{
    public abstract class RepositoryBase
    {
        private readonly string _connectionString;

        // Connect to Azure DB / SQL server 
        public RepositoryBase()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["PaintShopDbContext"].ConnectionString;
        }

        protected SqlConnection GetConnection()
        {
            return new SqlConnection(_connectionString);
        }
    }
}
