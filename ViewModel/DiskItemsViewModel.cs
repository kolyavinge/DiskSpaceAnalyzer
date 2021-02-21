using System.Collections.Generic;
using System.IO;
using System.Linq;
using DiskSpaceAnalyzer.Model;
using DiskSpaceAnalyzer.Mvvm;
using DiskSpaceAnalyzer.Tool;

namespace DiskSpaceAnalyzer.ViewModel
{
    public class DiskItemsViewModel : NotificationObject
    {
        private readonly MainModel _mainModel;

        private IEnumerable<DiskItemViewModel> _items;
        public IEnumerable<DiskItemViewModel> Items
        {
            get { return _items; }
            set
            {
                _items = value;
                RaisePropertyChanged("Items");
            }
        }

        public DiskItemsViewModel(MainModel mainModel)
        {
            _mainModel = mainModel;
            _mainModel.OnAnalyzeDiskComplete += OnAnalyzeDiskComplete;
        }

        private void OnAnalyzeDiskComplete(object sender, AnalyzeDiskCompleteEventArgs e)
        {
            Items = e.Result.Root.Children.Select(item => new DiskItemViewModel(e.Result.Disk, item)).OrderByDescending(x => x.DiskItem.SizeBytes).ToList();
        }
    }

    public class DiskItemViewModel : NotificationObject
    {
        private Disk _disk;

        public DiskItem DiskItem { get; }

        public string Name { get { return Path.GetFileName(DiskItem.FullPath); } }

        public bool IsDirectory { get { return DiskItem.Kind == DiskItemKind.Directory; } }

        public float Size { get; private set; }

        public SizeUnit Unit { get; private set; }

        public float TotalPercent { get; private set; }

        public DiskItemViewModel(Disk disk, DiskItem diskItem)
        {
            _disk = disk;
            DiskItem = diskItem;
            SetSizeAndUnits();
            TotalPercent = 100.0f * diskItem.SizeBytes / disk.TotalSizeBytes;
        }

        private void SetSizeAndUnits()
        {
            Size = (float)DiskItem.SizeBytes / Constants._1024_pow3;
            Unit = SizeUnit.Gigabytes;
            if (Size < 1.0f)
            {
                Size = (float)DiskItem.SizeBytes / Constants._1024_pow2;
                Unit = SizeUnit.Megabytes;
            }
        }
    }

    public enum SizeUnit { Megabytes = 1, Gigabytes = 2 }
}
