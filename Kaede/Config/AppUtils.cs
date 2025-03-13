using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kaede.Config
{
    public static class AppUtils
    {
        const string AppName = "KaedeApp";
        const string DbFileName = "kdbase.db";
        public static readonly string LocalFolder =
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        public static readonly string DbPath =
            System.IO.Path.Join(LocalFolder, AppName, DbFileName);

        public static string ConnectionString => $"Data Source={Config.AppUtils.DbPath}";
        public static void CreateLocalAppDir() =>
            System.IO.Directory.CreateDirectory(System.IO.Path.Join(LocalFolder, AppName));
    }
}
