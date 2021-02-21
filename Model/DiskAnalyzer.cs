using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiskSpaceAnalyzer.Model
{
    public class DiskAnalyzer
    {
        private long _diskUsedSpaceBytes;
        private long _totalAnalyzedBytes;
        private int _lastProgressPercentValue;
        private readonly FileSystem _fileSystem = new FileSystem();

        public event EventHandler<AnalyzeProgressEventArgs> OnAnalyzeProgress;

        public event EventHandler OnAnalyzeComplete;

        public async Task<DiskAnalyzeResult> AnalyzeAsync(Disk disk)
        {
            return await Task.Run(() => Analyze(disk));
        }

        public DiskAnalyzeResult Analyze(Disk disk)
        {
            _diskUsedSpaceBytes = disk.UsedSizeBytes;
            var root = new DiskItem(disk.RootPath, DiskItemKind.Directory);
            Analyze(root);
            _totalAnalyzedBytes = _diskUsedSpaceBytes;
            RaiseOnAnalyzeProgress();
            RaiseOnAnalyzeComplete();

            return new DiskAnalyzeResult(disk, root);
        }

        private bool Analyze(DiskItem parent)
        {
            var directoryPaths = _fileSystem.GetDirectories(parent.FullPath);
            if (directoryPaths == null) return false;
            if (directoryPaths.Any())
            {
                parent.Children = new List<DiskItem>(directoryPaths.Length);
            }
            foreach (var directoryPath in directoryPaths)
            {
                var directory = new DiskItem(directoryPath, DiskItemKind.Directory);
                if (Analyze(directory))
                {
                    parent.Children.Add(directory);
                    parent.SizeBytes += directory.SizeBytes;
                }
            }
            var filePaths = _fileSystem.GetFiles(parent.FullPath);
            if (filePaths == null) return false;
            if (filePaths.Any() && parent.Children == null)
            {
                parent.Children = new List<DiskItem>(filePaths.Length);
            }
            foreach (var filePath in filePaths)
            {
                var file = new DiskItem(filePath, DiskItemKind.File);
                parent.Children.Add(file);
                file.SizeBytes = _fileSystem.GetFileSize(filePath);
                parent.SizeBytes += file.SizeBytes;
                _totalAnalyzedBytes += file.SizeBytes;
            }
            RaiseOnAnalyzeProgress();

            return true;
        }

        private void RaiseOnAnalyzeProgress()
        {
            if (OnAnalyzeProgress != null)
            {
                var currentProgressPercentValue = (int)(100 * _totalAnalyzedBytes / _diskUsedSpaceBytes);
                if (currentProgressPercentValue > _lastProgressPercentValue)
                {
                    _lastProgressPercentValue = currentProgressPercentValue;
                    OnAnalyzeProgress(this, new AnalyzeProgressEventArgs { ProgressPercent = currentProgressPercentValue });
                }
            }
        }

        private void RaiseOnAnalyzeComplete()
        {
            if (OnAnalyzeComplete != null)
            {
                OnAnalyzeComplete(this, EventArgs.Empty);
            }
        }
    }

    public class AnalyzeProgressEventArgs : EventArgs
    {
        public int ProgressPercent { get; set; }
    }

    public class DiskAnalyzeResult
    {
        public Disk Disk { get; }

        public DiskItem Root { get; }

        public DiskAnalyzeResult(Disk disk, DiskItem root)
        {
            Disk = disk;
            Root = root;
        }
    }
}
