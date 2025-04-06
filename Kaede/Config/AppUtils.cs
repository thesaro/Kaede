using Kaede.DbContexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kaede.Config
{
    public static class AppUtils
    {
        #region Appplication Paths
        const string AppName = "KaedeApp";
        const string DbFileName = "kdbase.db";
        const string DbTempFileName = "kdbasetemp.db";

        public static readonly string LocalFolder =
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        public static readonly string DbPath =
            Path.Join(LocalFolder, AppName, DbFileName);
        public static readonly string DbPathTemp =
            Path.Join(LocalFolder, AppName, DbTempFileName);
        #endregion

        #region Static DB Utility Methods
        public static void LoadAppData()
        {
            CreateLocalAppDir();

            if (File.Exists(DbPathTemp))
            {
                File.Delete(DbPath);
                File.Move(DbPathTemp, DbPath);
            }

            var options = new DbContextOptionsBuilder<KaedeDbContext>()
                .UseSqlite(ConnectionString).Options;
            using var context = new KaedeDbContext(options);

            context.Database.EnsureCreated();
            context.Database.Migrate();
        }

        public static string ConnectionString => $"Data Source={Config.AppUtils.DbPath}";
        public static void CreateLocalAppDir() =>
            Directory.CreateDirectory(Path.Join(LocalFolder, AppName));
        #endregion
    }
}
