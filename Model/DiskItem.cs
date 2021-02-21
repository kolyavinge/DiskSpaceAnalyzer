using System.Collections.Generic;

namespace DiskSpaceAnalyzer.Model
{
    public enum DiskItemKind { File, Directory }

    public class DiskItem
    {
        public string FullPath { get; }

        public DiskItemKind Kind { get; }

        public List<DiskItem> Children { get; set; }

        public long SizeBytes { get; set; }

        public DiskItem(string path, DiskItemKind kind)
        {
            FullPath = path;
            Kind = kind;
        }
    }
}
