using Kaede.Config;
using Kaede.DbContexts;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Kaede.Test
{
    internal static class Shared
    {
        internal static DbContextOptions<T> CreateSqliteOptions<T>() where T : DbContext
        {
            var connectionStringBuilder = new SqliteConnectionStringBuilder { DataSource = ":memory:" };
            var connectionString = connectionStringBuilder.ToString();

            // Create a SQLite connection and open it
            var connection = new SqliteConnection(connectionString);
            connection.Open();

            // Configure EF Core options to use SQLite
            var optionsBuilder = new DbContextOptionsBuilder<T>();
            optionsBuilder.UseSqlite(connection);

            return optionsBuilder.Options;
        }
    }
}
