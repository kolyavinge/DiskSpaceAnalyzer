using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DiskSpaceAnalyzer.Model
{
    public class MainModel
    {
        public event EventHandler OnAnalyzeDiskStart;

        public event EventHandler<AnalyzeProgressEventArgs> OnAnalyzeDiskProgress;

        public event EventHandler<AnalyzeDiskCompleteEventArgs> OnAnalyzeDiskComplete;

        public Disk SelectedDist { get; private set; }

        public IEnumerable<Disk> GetDisks()
        {
            var diskProvider = new DiskProvider();
            return diskProvider.GetDisks();
        }

        public async void AnalyzeDisk(Disk disk)
        {
            SelectedDist = disk;
            if (OnAnalyzeDiskStart != null) OnAnalyzeDiskStart(this, EventArgs.Empty);
            var analyzer = new DiskAnalyzer();
            analyzer.OnAnalyzeProgress += (s, e) => { if (OnAnalyzeDiskProgress != null) OnAnalyzeDiskProgress(this, e); };
            var result = await analyzer.AnalyzeAsync(disk);
            if (OnAnalyzeDiskComplete != null) OnAnalyzeDiskComplete(this, new AnalyzeDiskCompleteEventArgs { Result = result });
        }

        public async void Reanalyze()
        {
            AnalyzeDisk(SelectedDist);
        }
    }

    public class AnalyzeDiskCompleteEventArgs : EventArgs
    {
        public DiskAnalyzeResult Result { get; set; }
    }
}
