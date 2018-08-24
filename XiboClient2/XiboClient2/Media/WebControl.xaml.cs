using CefSharp.Wpf;
using System;
using System.Collections.Generic;
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
    /// Interaction logic for WebControl.xaml
    /// </summary>
    public partial class WebControl : UserControl
    {
        private MediaOption obj;
        private ChromiumWebBrowser web;
        private System.Timers.Timer TimerMedia;

        //default durauion region
        double duration = 0;

        MediaFinish _MediaFinish;

        public WebControl(MediaOption item, MediaFinish _MediaFinish)
        {
            InitializeComponent();

            //set media option
            this.obj = item;
            //call back methos set
            this._MediaFinish = _MediaFinish;

            Loaded += WebControl_Loaded;
            Unloaded += WebControl_Unloaded;
        }

        /// <summary>
        /// Panel Unload
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WebControl_Unloaded(object sender, RoutedEventArgs e)
        {
            try
            {
                //Dispose web
                web.Dispose();

                //Stop timer
                TimerMedia.Stop();
            }
            catch(Exception ex)
            {
                PlayerSettings.ErrorLog(ex);
            }
            
            
        }

        /// <summary>
        /// Panel load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WebControl_Loaded(object sender, RoutedEventArgs e)
        {
            ViewWeb();
        }

        /// <summary>
        /// View web site
        /// </summary>
        private void ViewWeb()
        {
            string _regionID = obj.regionId;
            //media details
            int mediaId = obj.mediaId;
            string modeId = obj.modeid;
            int _top = obj.top;
            int _left = obj.left;

            string _filePath = "";
            bool nativeOpen = modeId != string.Empty && modeId == "1";
            string url = obj.uri;
            if (nativeOpen)
            {
                // If we are modeid == 1, then just open the webpage without adjusting the file path
                _filePath = Uri.UnescapeDataString(obj.uri).Replace('+', ' ');
            }
            else
            {
                // Set the file path
                _filePath = PlayerSettings.libraryPath + mediaId + ".htm";
            }

            duration = MediaSupport.ReadControlMetaDuration(_filePath);
            if (duration == 0)
            {
                duration = obj.duration;
            }

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

            if (PlayerSettings.firstLoadCheck == 0)
            {
                var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(2) };
                timer.Start();
                timer.Tick += (sender1, args) =>
                {
                    timer.Stop();
                    web.Address = _filePath;
                };
            }
            else
            {
                this.Dispatcher.Invoke(() =>
                {
                    web.Address = _filePath;
                });
            }

            this.webPanel.Children.Add(web);

            TimerMedia = new System.Timers.Timer();
            TimerMedia.Elapsed += new ElapsedEventHandler(webtimer_Tick);
            TimerMedia.Interval = 1000 * duration;
            TimerMedia.Start();

            //transition IN render
            if (transIn != null)
            {
                MediaSupport.MoveAnimation(web, OpacityProperty, transIn, transInDirection, transInDuration, "in", _top, _left);
            }

            ////transition OUT render
            double transOutStartTime = duration - (transOutDuration / 1000);
            if (transOut != null)
            {
                var timerTransition = new DispatcherTimer { Interval = TimeSpan.FromSeconds(transOutStartTime) };
                timerTransition.Start();
                timerTransition.Tick += (sender1, args) =>
                {
                    timerTransition.Stop();
                    MediaSupport.MoveAnimation(web, OpacityProperty, transOut, transOutDirection, transOutDuration, "out", _top, _left);
                };
            }
        }

        private void webtimer_Tick(object sender, ElapsedEventArgs e)
        {
            CallToBackRegion();
        }

        /// <summary>
        /// Call back function when meadi complated
        /// </summary>
        private void CallToBackRegion()
        {
            int mediaId = Convert.ToInt16(obj.mediaId);
            if (_MediaFinish != null)
            {
                _MediaFinish(mediaId);

            }
            TimerMedia.Stop();
        }

    }
}
