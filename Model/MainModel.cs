using System;
using System.Collections.Generic;
using System.Linq;

namespace DiskSpaceAnalyzer.Model
{
    public class MainModel
    {
        private Stack<DiskItem> _historyDiskItems;

        public event EventHandler OnAnalyzeDiskStart;

        public event EventHandler<AnalyzeProgressEventArgs> OnAnalyzeDiskProgress;

        public event EventHandler<AnalyzeDiskCompleteEventArgs> OnAnalyzeDiskComplete;

        public event EventHandler OnRefresh;

        public Disk SelectedDisk { get; private set; }

        public string HistoryFullPath
        {
            get { return _historyDiskItems.Any() ? _historyDiskItems.Peek().FullPath : ""; }
        }

        public bool IsGotoUpEnabled { get { return _historyDiskItems.Count > 1; } }

        public IEnumerable<Disk> Disks { get; private set; }

        public MainModel()
        {
            UpdateDisks();
        }

        public async void SelectDisk(Disk disk)
        {
            SelectedDisk = disk;
            if (SelectedDisk == null) return;
            _historyDiskItems = new Stack<DiskItem>();
            if (OnAnalyzeDiskStart != null) OnAnalyzeDiskStart(this, EventArgs.Empty);
            var analyzer = new DiskAnalyzer();
            analyzer.OnAnalyzeProgress += (s, e) => { if (OnAnalyzeDiskProgress != null) OnAnalyzeDiskProgress(this, e); };
            var result = await analyzer.AnalyzeAsync(disk);
            _historyDiskItems.Push(result.Root);
            if (OnAnalyzeDiskComplete != null) OnAnalyzeDiskComplete(this, new AnalyzeDiskCompleteEventArgs { Result = result });
        }

        public async void Refresh()
        {
            var selectedDiskName = SelectedDisk.Name;
            UpdateDisks();
            SelectedDisk = Disks.First(x => x.Name == selectedDiskName);
            SelectDisk(SelectedDisk);
            if (OnRefresh != null) OnRefresh(this, EventArgs.Empty);
        }

        public void GotoDirectory(DiskItem diskItem)
        {
            _historyDiskItems.Push(diskItem);
        }

        public DiskItem GotoUp()
        {
            if (_historyDiskItems.Count > 1)
            {
                _historyDiskItems.Pop();
                return _historyDiskItems.Peek();
            }
            else
            {
                return null;
            }
        }

        public void OpenCurrentDirectory()
        {
            System.Diagnostics.Process.Start("explorer.exe", HistoryFullPath);
        }

        private void UpdateDisks()
        {
            var diskProvider = new DiskProvider();
            Disks = diskProvider.GetDisks();
        }
    }

    public class AnalyzeDiskCompleteEventArgs : EventArgs
    {
        public DiskAnalyzeResult Result { get; set; }
    }
}
