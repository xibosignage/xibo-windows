using CefSharp.Wpf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using XiboClient2.Settings;

namespace XiboClient2.Media
{
    /// <summary>
    /// Interaction logic for FlashControl.xaml
    /// </summary>
    public partial class FlashControl : UserControl
    {
        private MediaOption obj;
        private ChromiumWebBrowser web;
        private System.Timers.Timer TimerMediaFlash;

        //default durauion region
        double duration = 0;

        MediaFinish _MediaFinish;

        public FlashControl(MediaOption item, MediaFinish _MediaFinish)
        {
            InitializeComponent();
            this.obj = item;
            this._MediaFinish = _MediaFinish;

            Loaded += FlashControl_Loaded;
            Unloaded += FlashControl_Unloaded;
        }

        private void FlashControl_Unloaded(object sender, RoutedEventArgs e)
        {
            web.Dispose();
            TimerMediaFlash.Stop();
            this.FlashPanel.Children.Clear();
        }

        private void FlashControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                ViewFlash();
            }
            catch (Exception ex)
            {
                PlayerSettings.ErrorLog(ex);
            }
        }

        private void ViewFlash()
        {
            string _regionID = obj.regionId;
            //media details
            int mediaId = obj.mediaId;
            string modeId = obj.modeid;

            int _top = obj.top;
            int _left = obj.left;

            string _filePath = "";
            bool nativeOpen = modeId != string.Empty && modeId == "1";


            _filePath = obj.uri;

            duration = obj.duration;

            string flashWeb = "<object classid='clsid:d27cdb6e-ae6d-11cf-96b8-444553540000' codebase='http://fpdownload.macromedia.com/pub/shockwave/cabs/flash/swflash.cab#version=7,0,0,0'  id='analog_clock' align='middle'>";
            flashWeb += "<param name='allowScriptAccess' value='sameDomain' />";
            flashWeb += "<param name='movie' value='" + _filePath + "' />";
            flashWeb += "<param name='quality' value='high' />";
            flashWeb += "<param name='bgcolor' value='#000' />";
            flashWeb += "<param name='WMODE' value='transparent' />";
            flashWeb += "<embed src='" + _filePath + "' quality='high' wmode='transparent' bgcolor='#ffffff' width='" + obj.width + "' height='" + obj.height + "' name='analog_clock' align='middle' allowScriptAccess='sameDomain' type='application/x-shockwave-flash' pluginspage='http://www.macromedia.com/go/getflashplayer' />";
            flashWeb += "</object>";

            string temSavePath = PlayerSettings.libraryPath + "temp" + mediaId + ".htm";
            using (FileStream fs = new FileStream(temSavePath, FileMode.Create))
            {
                using (StreamWriter w = new StreamWriter(fs, Encoding.UTF8))
                {
                    w.WriteLine(flashWeb);
                }
            }

            Uri uriFlash = new Uri(temSavePath);

            //transition IN details
            string transIn = obj.transIn;
            string transInDirection = obj.transInDirection;
            double transInDuration = obj.transInDuration;

            duration += (transInDuration / 1000);

            //transition OUT details
            string transOut = obj.transOut;
            string transOutDirection = obj.transOutDirection;
            double transOutDuration = obj.transOutDuration;

            duration += (transOutDuration / 1000);

            web = new ChromiumWebBrowser()
            {
                Name = "region" + _regionID
            };
            web.Address = temSavePath;

            TimerMediaFlash = new System.Timers.Timer();
            TimerMediaFlash.Elapsed += new ElapsedEventHandler(Flashtimer_Tick);
            TimerMediaFlash.Interval = 1000 * duration;
            TimerMediaFlash.Start();

            this.FlashPanel.Children.Add(web);

            if (transIn != null)
            {
                MediaSupport.MoveAnimation(web, OpacityProperty, transIn, transInDirection, transInDuration, "in", _top, _left);
            }

            double transOutStartTime = duration - (transOutDuration / 1000);
            if (transOut != null)
            {
                var timerTransition = new DispatcherTimer { Interval = TimeSpan.FromSeconds(transOutStartTime) };
                timerTransition.Start();
                timerTransition.Tick += (sender, args) =>
                {
                    timerTransition.Stop();
                    MediaSupport.MoveAnimation(web, OpacityProperty, transOut, transOutDirection, transOutDuration, "out", _top, _left);
                };
            }
        }

        private void Flashtimer_Tick(object sender, ElapsedEventArgs e)
        {
            TimerMediaFlash.Stop();
            CallToBackRegion();
        }

        private void CallToBackRegion()
        {
            int mediaId = Convert.ToInt16(obj.mediaId);
            if (_MediaFinish != null)
            {
                _MediaFinish(mediaId);

            }
        }
    }
}
