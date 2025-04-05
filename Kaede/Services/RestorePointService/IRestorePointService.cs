using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kaede.Services.RestorePointService
{
    public interface IRestorePointService
    {
        // these parameters can be later abstracted into BackupOptions, RestoreOptions structs
        // but since we only have one type we just do it plain file paths for now
        void Backup(string filePath);
        void Restore(string filePath);
    }

}
