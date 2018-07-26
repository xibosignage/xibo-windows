using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XiboClient2.Settings
{
    public class MediaOption
    {
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

        public int mediaId;
        public string render;
        public string type;
        public string uri;
        public int duration;

        public int loop;
        public int mute;


        //rss options
        public string direction;
        public string text;
        public string documentTemplate;
        public string copyrightNotice;
        public string javaScript;
        public int updateInterval;
        public int scrollSpeed;
        public string modeid;

        //background audio
        public bool audio;

        //Transitions Details
        public string transIn;
        public double transInDuration;
        public string transInDirection;
        public string transOut;
        public double transOutDuration;
        public string transOutDirection;

        public string launchThroughCmd;
        public string scName;
        public string terminateCommand;
        public string useTaskkill;
        public string windowsCommand;

        public MediaOption()
        {

        }

        public MediaOption(string layoutId, string regionId, int width, int height, int top, int left,
            int mediaId, string render, string type, string uri, int duration, int loop, int mute, string direction, string text, string documentTemplate,
            string copyrightNotice, string javaScript, int updateInterval, int scrollSpeed, string modeid, bool audio,
            string transIn, double transInDuration, string transInDirection, string transOut, double transOutDuration, string transOutDirection,
            string launchThroughCmd, string scName, string terminateCommand, string useTaskkill, string windowsCommand)
        {
            this.layoutId = layoutId;
            this.regionId = regionId;
            this.width = width;
            this.height = height;
            this.top = top;
            this.left = left;

            this.mediaId = mediaId;
            this.render = render;
            this.type = type;
            this.uri = uri;
            this.duration = duration;

            this.loop = loop;
            this.mute = mute;

            //rss options
            this.direction = direction;
            this.text = text;
            this.documentTemplate = documentTemplate;
            this.copyrightNotice = copyrightNotice;
            this.javaScript = javaScript;
            this.updateInterval = updateInterval;
            this.scrollSpeed = scrollSpeed;
            this.modeid = modeid;

            this.audio = audio;


            //Transition
            this.transIn = transIn;
            this.transInDuration = transInDuration;
            this.transInDirection = transInDirection;
            this.transOut = transOut;
            this.transOutDuration = transOutDuration;
            this.transOutDirection = transOutDirection;

            //Shell Commnad
            this.launchThroughCmd = launchThroughCmd;
            this.scName = scName;
            this.terminateCommand = terminateCommand;
            this.useTaskkill = useTaskkill;
            this.windowsCommand = windowsCommand;

        }
    }
}
