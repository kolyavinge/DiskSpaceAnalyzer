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

        private IEnumerable<DiskViewModel> _disks;
        public IEnumerable<DiskViewModel> Disks
        {
            get { return _disks; }
            set
            {
                _disks = value;
                RaisePropertyChanged("Disks");
            }
        }

        private DiskViewModel _selectedDisk;
        public DiskViewModel SelectedDisk
        {
            get { return _selectedDisk; }
            set
            {
                _selectedDisk = value;
                RaisePropertyChanged("SelectedDisk");
                if (_selectedDisk != null)
                {
                    _mainModel.SelectDisk(_selectedDisk.Disk);
                }
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
            _mainModel.OnRefresh += OnRefresh;
            IsEnabled = true;
            UpdateDisks();
        }

        private void OnAnalyzeDiskStart(object sender, EventArgs e)
        {
            IsEnabled = false;
        }

        private void OnAnalyzeDiskComplete(object sender, AnalyzeDiskCompleteEventArgs e)
        {
            IsEnabled = true;
        }

        private void OnRefresh(object sender, EventArgs e)
        {
            UpdateDisks();
        }

        private void UpdateDisks()
        {
            Disks = _mainModel.Disks.Select(x => new DiskViewModel(x)).ToList();
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
