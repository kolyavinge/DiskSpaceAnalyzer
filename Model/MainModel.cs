using System;
using System.Collections.Generic;

namespace DiskSpaceAnalyzer.Model
{
    public class MainModel
    {
        public event EventHandler OnAnalyzeDiskStart;

        public event EventHandler<AnalyzeProgressEventArgs> OnAnalyzeDiskProgress;

        public event EventHandler<AnalyzeDiskCompleteEventArgs> OnAnalyzeDiskComplete;

        public IEnumerable<Disk> GetDisks()
        {
            var diskProvider = new DiskProvider();
            return diskProvider.GetDisks();
        }

        public async void SelectDisk(Disk disk)
        {
            if (OnAnalyzeDiskStart != null) OnAnalyzeDiskStart(this, EventArgs.Empty);
            var analyzer = new DiskAnalyzer();
            analyzer.OnAnalyzeProgress += (s, e) => { if (OnAnalyzeDiskProgress != null) OnAnalyzeDiskProgress(this, e); };
            var result = await analyzer.AnalyzeAsync(disk);
            if (OnAnalyzeDiskComplete != null) OnAnalyzeDiskComplete(this, new AnalyzeDiskCompleteEventArgs { Result = result });
        }
    }

    public class AnalyzeDiskCompleteEventArgs : EventArgs
    {
        public DiskAnalyzeResult Result { get; set; }
    }
}
