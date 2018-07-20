using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XiboClient2.Processor.Models
{
    public class RequiredFile
    {
        public string FileType;
        public int Id;
        public DateTime LastChecked;
        public string Md5;
        public string Path;
        public string SaveAs;

        public bool Downloading;
        public bool Complete;
        public bool Http;

        public double ChunkOffset;
        public double ChunkSize;
        public double Size;
        public int Retrys;

        // Resource nodes
        public int LayoutId;
        public string RegionId;
        public string MediaId;
    }
}
