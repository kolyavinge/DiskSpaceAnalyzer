using System.Collections.Generic;

namespace DiskSpaceAnalyzer.Model
{
    public enum FileSystemItemKind { File, Directory }

    public class FileSystemItem
    {
        public string Path { get; }

        public FileSystemItemKind Kind { get; }

        public List<FileSystemItem> Children { get; set; }

        public long SizeBytes { get; set; }

        public FileSystemItem(string path, FileSystemItemKind kind)
        {
            Path = path;
            Kind = kind;
        }
    }
}
