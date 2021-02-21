using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DiskSpaceAnalyzer.Model
{
    public class DiskItem
    {
        public DiskItem(DriveInfo driveInfo)
        {
            Name = driveInfo.Name;
            RootPath = driveInfo.RootDirectory.FullName;
            TotalSizeBytes = driveInfo.TotalSize;
            AvailableFreeSpaceBytes = driveInfo.AvailableFreeSpace;
            UsedSpaceBytes = TotalSizeBytes - AvailableFreeSpaceBytes;
        }

        public string Name { get; }

        public string RootPath { get; }

        public long TotalSizeBytes { get; }

        public long AvailableFreeSpaceBytes { get; }

        public long UsedSpaceBytes { get; }
    }

    public class DiskProvider
    {
        public IEnumerable<DiskItem> GetDisks()
        {
            return DriveInfo.GetDrives().Where(x => x.DriveType == DriveType.Fixed).Select(x => new DiskItem(x));
        }
    }
}
