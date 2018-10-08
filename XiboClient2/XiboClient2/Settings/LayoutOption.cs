using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XiboClient2.Settings
{
    public class LayoutOption
    {
        //general options
        public int scheduleId;
        public int layoutId;
        public string backgroundImage;
        public string backgroundColor;
        public int layoutWidth;
        public int layoutHeight;
        public int layoutLeft = 0;
        public int layoutTop = 0;

        //public List<RegionOptions MyProperty { get; set; }

        public List<RegionOptions> RegionList = new List<RegionOptions>();
        public List<MediaOption> MediaNodeList = new List<MediaOption>();
        public List<AudioOption> AudioNodeList = new List<AudioOption>();
    }
}
