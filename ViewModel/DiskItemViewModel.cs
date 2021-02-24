using System.IO;
using DiskSpaceAnalyzer.Model;
using DiskSpaceAnalyzer.Mvvm;
using DiskSpaceAnalyzer.Tool;

namespace DiskSpaceAnalyzer.ViewModel
{
    public class DiskItemViewModel : NotificationObject
    {
        public DiskItem DiskItem { get; private set; }

        public string Name { get; private set; }

        public bool IsDirectory { get; private set; }

        public float Size { get; private set; }

        public SizeUnit Unit { get; private set; }

        public float TotalPercent { get; private set; }

        public DiskItemViewModel(Disk disk, DiskItem diskItem)
        {
            DiskItem = diskItem;
            Name = Path.GetFileName(DiskItem.FullPath);
            IsDirectory = DiskItem.Kind == DiskItemKind.Directory;
            SetSizeAndUnits();
            TotalPercent = 100.0f * diskItem.SizeBytes / disk.TotalSizeBytes;
        }

        private void SetSizeAndUnits()
        {
            Size = (float)DiskItem.SizeBytes / Constants._1024_pow3;
            Unit = SizeUnit.Gigabytes;
            if (Size < 0.9f)
            {
                Size = (float)DiskItem.SizeBytes / Constants._1024_pow2;
                Unit = SizeUnit.Megabytes;
            }
        }
    }

    public enum SizeUnit { Megabytes = 1, Gigabytes = 2 }
}
