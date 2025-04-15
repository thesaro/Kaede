using Kaede.DbContexts;
using Microsoft.EntityFrameworkCore;
using Serilog;
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
        const string LogFileName = "kdlog.txt";

        public static readonly string LocalFolder =
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        public static readonly string DbPath =
            Path.Join(LocalFolder, AppName, DbFileName);
        public static readonly string DbPathTemp =
            Path.Join(LocalFolder, AppName, DbTempFileName);
        public static readonly string LogPath =
            Path.Join(LocalFolder, AppName, LogFileName);
        #endregion

        #region Static DB Utility Methods
        public static void LoadAppData()
        {
            Log.Debug("Creating local app dir");
            CreateLocalAppDir();

            if (File.Exists(DbPathTemp))
            {
                Log.Debug("Temp database file (recovery) detected at {DbTempPath}. Moving it to {DbPath}", DbPathTemp, DbPath);
                File.Delete(DbPath);
                File.Move(DbPathTemp, DbPath);
            }

            var options = new DbContextOptionsBuilder<KaedeDbContext>()
                .UseSqlite(ConnectionString).Options;
            using var context = new KaedeDbContext(options);

            try
            {
                Log.Debug("Ensuring database is created");
                context.Database.EnsureCreated();
                context.Database.Migrate();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Failed to load application data.");
                Environment.Exit(exitCode: 7);
            }

        }

        public static string ConnectionString => $"Data Source={Config.AppUtils.DbPath}";
        public static void CreateLocalAppDir() =>
            Directory.CreateDirectory(Path.Join(LocalFolder, AppName));
        #endregion
    }
}
