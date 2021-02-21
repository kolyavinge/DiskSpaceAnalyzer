using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DiskSpaceAnalyzer.Model
{
    public class Disk
    {
        public Disk(DriveInfo driveInfo)
        {
            Name = driveInfo.Name;
            RootPath = driveInfo.RootDirectory.FullName;
            TotalSizeBytes = driveInfo.TotalSize;
            FreeSizeBytes = driveInfo.AvailableFreeSpace;
            UsedSizeBytes = TotalSizeBytes - FreeSizeBytes;
        }

        public string Name { get; }

        public string RootPath { get; }

        public long TotalSizeBytes { get; }

        public long FreeSizeBytes { get; }

        public long UsedSizeBytes { get; }
    }

    public class DiskProvider
    {
        public IEnumerable<Disk> GetDisks()
        {
            return DriveInfo.GetDrives().Where(x => x.DriveType == DriveType.Fixed).Select(x => new Disk(x));
        }
    }
}
