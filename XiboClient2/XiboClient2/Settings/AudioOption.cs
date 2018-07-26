using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XiboClient2.Settings
{
    public class AudioOption
    {
        public int mediaId;
        public int volume;
        public int loop;
        public string audioUrl;
        public string regionId;

        public AudioOption()
        {

        }

        public AudioOption(int mediaId, int volume, int loop, string audioUrl, string regionId)
        {
            this.mediaId = mediaId;
            this.volume = volume;
            this.loop = loop;
            this.audioUrl = audioUrl;
            this.regionId = regionId;
        }
    }
}
