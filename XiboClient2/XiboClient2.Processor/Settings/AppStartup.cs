using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;
using XiboClient2.Processor.Forms;
using XiboClient2.Processor.Log;
using XiboClient2.Processor.Logic;
using System.Windows.Input;
using XiboClient2.Processor.Control;

namespace XiboClient2.Processor.Settings
{
    public class AppStartup
    {
       
        public static void Notify(string _layoutId)
        {
            if (ApplicationSettings.Default.SendCurrentLayoutAsStatusUpdate)
            {
                using (xmds.xmds statusXmds = new xmds.xmds())
                {
                    statusXmds.Url = ApplicationSettings.Default.XiboClient_xmds_xmds + "&method=notifyStatus";
                    statusXmds.NotifyStatusAsync(ApplicationSettings.Default.ServerKey, ApplicationSettings.Default.HardwareKey, "{\"currentLayoutId\":" + _layoutId + "}");
                }
            }
        }


    }
}
