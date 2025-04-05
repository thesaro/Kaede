using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kaede.Services.RestorePointService
{
    public class DatabaseRestorePointService : IRestorePointService
    {
        private void _ensureLocalFolderExists() => 
            Config.AppUtils.CreateLocalAppDir();
        public void Backup(string filePath)
        {
            _ensureLocalFolderExists();
            File.Copy(Config.AppUtils.DbPath, filePath, true);
        }

        public void Restore(string filePath)
        {
            _ensureLocalFolderExists();

            bool validFile = true;

            try
            {
                using var conn = new SqliteConnection($"Data Source={filePath}");
                conn.Open();

                SqliteCommand command = new SqliteCommand(
                    "SELECT name FROM sqlite_master WHERE type='table';", conn);
                SqliteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    // check that the tables have the format we need
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while trying to connect to the provided sqlite restore point:\n{ex.Message}");
            }

            if (validFile)
            {
                File.Copy(filePath, Config.AppUtils.DbPathTemp, true);
            }
            else
            {
                // do some exception or something idk
            }
        }
    }
}
