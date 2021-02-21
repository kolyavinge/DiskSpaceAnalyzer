using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DiskSpaceAnalyzer.Tool;

namespace DiskSpaceAnalyzer.Model
{
    public class AnalyzeProgressEventHandler : EventArgs
    {
        public int ProgressPercent { get; set; }
    }

    public class DiskAnalyzeResult
    {
        public DiskItem Disk { get; }

        public FileSystemItem Root { get; }

        public DiskAnalyzeResult(DiskItem disk, FileSystemItem root)
        {
            Disk = disk;
            Root = root;
        }
    }

    public class DiskAnalyzer
    {
        private long _diskUsedSpaceBytes;
        private long _totalAnalyzedBytes;
        private int _lastProgressPercentValue;

        public event EventHandler<AnalyzeProgressEventHandler> OnAnalyzeProgress;

        public async Task<DiskAnalyzeResult> AnalyzeAsync(DiskItem diskItem)
        {
            return await Task.Run(() => Analyze(diskItem));
        }

        public DiskAnalyzeResult Analyze(DiskItem diskItem)
        {
            _diskUsedSpaceBytes = diskItem.UsedSpaceBytes;
            var root = new FileSystemItem(diskItem.RootPath, FileSystemItemKind.Directory);
            Analyze(root);
            _totalAnalyzedBytes = _diskUsedSpaceBytes;
            RaiseOnAnalyzeProgress();

            return new DiskAnalyzeResult(diskItem, root);
        }

        private bool Analyze(FileSystemItem parent)
        {
            var directoryPaths = GetDirectories(parent.Path);
            if (directoryPaths == null) return false;
            if (directoryPaths.Any())
            {
                parent.Children = new List<FileSystemItem>(directoryPaths.Length);
            }
            foreach (var directoryPath in directoryPaths)
            {
                var directory = new FileSystemItem(directoryPath, FileSystemItemKind.Directory);
                if (Analyze(directory))
                {
                    parent.Children.Add(directory);
                    parent.SizeBytes += directory.SizeBytes;
                }
            }
            var filePaths = GetFiles(parent.Path);
            if (filePaths == null) return false;
            if (filePaths.Any() && parent.Children == null)
            {
                parent.Children = new List<FileSystemItem>(filePaths.Length);
            }
            foreach (var filePath in filePaths)
            {
                var file = new FileSystemItem(filePath, FileSystemItemKind.File);
                parent.Children.Add(file);
                file.SizeBytes = GetFileSize(filePath);
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
                    OnAnalyzeProgress(this, new AnalyzeProgressEventHandler { ProgressPercent = currentProgressPercentValue });
                }
            }
        }

        private static readonly string _windowsDirectory = Directory.GetParent(Environment.GetFolderPath(Environment.SpecialFolder.System)).FullName;
        private string[] GetDirectories(string path)
        {
            try
            {
                return Directory.GetDirectories(path).Where(x => x.Equals(_windowsDirectory, StringComparison.OrdinalIgnoreCase) == false).ToArray();
            }
            catch (UnauthorizedAccessException)
            {
                return null;
            }
        }

        private string[] GetFiles(string path)
        {
            try
            {
                return Directory.GetFiles(path);
            }
            catch (UnauthorizedAccessException)
            {
                return null;
            }
        }

        private long GetFileSize(string filePath)
        {
            WIN32_FIND_DATA findData;
            WinApi.FindFirstFile(filePath, out findData);

            return findData.nFileSizeLow;
        }
    }
}
