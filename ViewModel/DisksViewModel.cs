using System;
using System.Collections.Generic;
using System.Linq;
using DiskSpaceAnalyzer.Model;
using DiskSpaceAnalyzer.Mvvm;
using DiskSpaceAnalyzer.Tool;

namespace DiskSpaceAnalyzer.ViewModel
{
    public class DisksViewModel : NotificationObject
    {
        private readonly MainModel _mainModel;

        public IEnumerable<DiskViewModel> Disks { get; }

        private DiskViewModel _selectedDisk;
        public DiskViewModel SelectedDisk
        {
            get { return _selectedDisk; }
            set
            {
                _selectedDisk = value;
                RaisePropertyChanged("SelectedDisk");
                _mainModel.SelectDisk(_selectedDisk.Disk);
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

        public DisksViewModel(MainModel mainModel)
        {
            _mainModel = mainModel;
            _mainModel.OnAnalyzeDiskStart += OnAnalyzeDiskStart;
            _mainModel.OnAnalyzeDiskComplete += OnAnalyzeDiskComplete;
            IsEnabled = true;
            Disks = mainModel.GetDisks().Select(x => new DiskViewModel(x)).ToList();
        }

        private void OnAnalyzeDiskStart(object sender, EventArgs e)
        {
            IsEnabled = false;
        }

        private void OnAnalyzeDiskComplete(object sender, AnalyzeDiskCompleteEventArgs e)
        {
            IsEnabled = true;
        }
    }

    public class DiskViewModel : NotificationObject
    {
        public Disk Disk { get; }

        public string Name { get { return Disk.Name; } }

        public float TotalSizeGigabytes { get { return (float)Disk.TotalSizeBytes / Constants._1024_pow3; } }

        public float FreeSizeGigabytes { get { return (float)Disk.FreeSizeBytes / Constants._1024_pow3; } }

        public float UsedSizeGigabytes { get { return (float)Disk.UsedSizeBytes / Constants._1024_pow3; } }

        public float UsedPercent { get { return 100.0f * Disk.UsedSizeBytes / Disk.TotalSizeBytes; } }

        public DiskViewModel(Disk disk)
        {
            Disk = disk;
        }
    }
}
