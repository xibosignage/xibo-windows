using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XiboClient2.Settings
{
    public class RegionOptions
    {
        public double scaleFactor;
        public int width;
        public int height;
        public int top;
        public int left;
        public string layoutId;
        public string regionId;

        public int originalWidth;
        public int originalHeight;

        public int backgroundLeft;
        public int backgroundTop;

        public int zIndex;

        // Region Loop
        public string transitionType;
        public double transitionDuration;
        public string transitionDirection;


        public RegionOptions()
        {
        }

        public RegionOptions(string layoutId, string regionId, int width, int height, int top, int left,
            string transitionType, double transitionDuration, string transitionDirection, int zIndex)
        {
            this.layoutId = layoutId;
            this.regionId = regionId;
            this.width = width;
            this.height = height;
            this.top = top;
            this.left = left;
            this.transitionType = transitionType;
            this.transitionDuration = transitionDuration;
            this.transitionDirection = transitionDirection;
            this.zIndex = zIndex;
        }
    }
}
