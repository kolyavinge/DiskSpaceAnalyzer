using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Input;
using DiskSpaceAnalyzer.Model;
using DiskSpaceAnalyzer.Mvvm;
using DiskSpaceAnalyzer.Tool;

namespace DiskSpaceAnalyzer.ViewModel
{
    public class DiskItemsViewModel : NotificationObject
    {
        private readonly MainModel _mainModel;
        private Disk _disk;
        private Stack<DiskItem> _historyDiskItems;

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

        private bool _isEnabled;
        public bool IsEnabled
        {
            get { return _isEnabled; }
            set
            {
                _isEnabled = value;
                RaisePropertyChanged("IsEnabled");
            }
        }

        private bool _gotoUpCommandEnabled;
        public bool GotoUpCommandEnabled
        {
            get { return _gotoUpCommandEnabled; }
            set
            {
                _gotoUpCommandEnabled = value;
                RaisePropertyChanged("GotoUpCommandEnabled");
            }
        }

        private string _historyFullPath;
        public string HistoryFullPath
        {
            get { return _historyFullPath; }
            set
            {
                _historyFullPath = value;
                RaisePropertyChanged("HistoryFullPath");
            }
        }

        public ICommand GotoDirectoryCommand { get { return new DelegateCommand<DiskItemViewModel>(GotoDirectory); } }

        public ICommand GotoUpCommand { get { return new DelegateCommand(GotoUp); } }

        public DiskItemsViewModel(MainModel mainModel)
        {
            _mainModel = mainModel;
            _mainModel.OnAnalyzeDiskStart += OnAnalyzeDiskStart;
            _mainModel.OnAnalyzeDiskComplete += OnAnalyzeDiskComplete;
            IsEnabled = true;
            GotoUpCommandEnabled = false;
            _historyDiskItems = new Stack<DiskItem>();
        }

        private void OnAnalyzeDiskStart(object sender, EventArgs e)
        {
            IsEnabled = false;
            Items = Enumerable.Empty<DiskItemViewModel>();
            _historyDiskItems.Clear();
            HistoryFullPath = "";
        }

        private void OnAnalyzeDiskComplete(object sender, AnalyzeDiskCompleteEventArgs e)
        {
            _disk = e.Result.Disk;
            _historyDiskItems.Push(e.Result.Root);
            HistoryFullPath = e.Result.Root.FullPath;
            UpdateItems(e.Result.Root);
            IsEnabled = true;
        }

        private void GotoDirectory(DiskItemViewModel selectedItem)
        {
            if (selectedItem.IsDirectory)
            {
                _historyDiskItems.Push(selectedItem.DiskItem);
                HistoryFullPath = selectedItem.DiskItem.FullPath;
                UpdateItems(selectedItem.DiskItem);
                GotoUpCommandEnabled = true;
            }
        }

        private void GotoUp()
        {
            if (_historyDiskItems.Count > 1)
            {
                _historyDiskItems.Pop();
                var parent = _historyDiskItems.Peek();
                HistoryFullPath = parent.FullPath;
                UpdateItems(parent);
            }
            if (_historyDiskItems.Count == 1)
            {
                GotoUpCommandEnabled = false;
            }
        }

        private void UpdateItems(DiskItem diskItem)
        {
            Items = diskItem.Children.Select(item => new DiskItemViewModel(_disk, item)).OrderByDescending(x => x.DiskItem.SizeBytes).ToList();
        }
    }

    public class DiskItemViewModel : NotificationObject
    {
        public DiskItem DiskItem { get; }

        public string Name { get { return Path.GetFileName(DiskItem.FullPath); } }

        public bool IsDirectory { get { return DiskItem.Kind == DiskItemKind.Directory; } }

        public float Size { get; private set; }

        public SizeUnit Unit { get; private set; }

        public float TotalPercent { get; private set; }

        public DiskItemViewModel(Disk disk, DiskItem diskItem)
        {
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
