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
    /// Interaction logic for VideoControl.xaml
    /// </summary>
    public partial class VideoControl : UserControl
    {
        private MediaOption obj;
        private MediaElement medaiElemnt;
        private System.Timers.Timer TimerMediaVideo;

        //default durauion region
        double duration = 0;

        MediaFinish _MediaFinish;

        public VideoControl(MediaOption item, MediaFinish _MediaFinish)
        {
            //videoPanel
            InitializeComponent();
            this.obj = item;
            this._MediaFinish = _MediaFinish;
            Loaded += VideoControl_Loaded;
            Unloaded += VideoControl_Unloaded;
        }

        private void VideoControl_Unloaded(object sender, RoutedEventArgs e)
        {
            TimerMediaVideo.Stop();
            medaiElemnt = null;
        }

        private void VideoControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                ViewVideo();
            }
            catch (Exception ex)
            {
                PlayerSettings.ErrorLog(ex);
            }
        }


        /// <summary>
        /// View video 
        /// </summary>
        private void ViewVideo()
        {
            //media details
            int mediaId = obj.mediaId;
            string uri = obj.uri;
            int _top = obj.top;
            int _left = obj.left;

            string _filePath = Uri.UnescapeDataString(uri).Replace('+', ' ');
            Uri uriVideo = new Uri(uri);

            duration = obj.duration;
            if (duration == 0)
            {
                TimeSpan videoDuration = PlayerSettings.GetVideoDuration(_filePath);
                duration = videoDuration.TotalSeconds;
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

            medaiElemnt = new MediaElement();
            medaiElemnt.Source = uriVideo;
            int volume = 0;
            if (obj.mute == 1)
            {
                volume = 0;
            }
            else
            {
                volume = 100;
            }
            medaiElemnt.Volume = volume;
            if (obj.loop == 1)
            {
                medaiElemnt.MediaEnded += MedaiElemnt_MediaEnded;
            }

            this.videoPanel.Children.Add(medaiElemnt);

            TimerMediaVideo = new System.Timers.Timer();
            TimerMediaVideo.Elapsed += new ElapsedEventHandler(MediaTimer_Tick);
            TimerMediaVideo.Interval = 1000 * duration;
            TimerMediaVideo.Start();

            if (transIn != null)
            {
                MediaSupport.MoveAnimation(medaiElemnt, OpacityProperty, transIn, transInDirection, transInDuration, "in", _top, _left);
            }
            double transOutStartTime = duration - (transOutDuration / 1000);
            if (transOut != null)
            {
                var timerTransition = new DispatcherTimer { Interval = TimeSpan.FromSeconds(transOutStartTime) };
                timerTransition.Start();
                timerTransition.Tick += (sender1, args) =>
                {
                    timerTransition.Stop();
                    MediaSupport.MoveAnimation(medaiElemnt, OpacityProperty, transOut, transOutDirection, transOutDuration, "out", _top, _left);
                };
            }
        }

        private void MediaTimer_Tick(object sender, ElapsedEventArgs e)
        {
            CallToBackRegion();
            TimerMediaVideo.Stop();
        }

        private void MedaiElemnt_MediaEnded(object sender, RoutedEventArgs e)
        {
            medaiElemnt.Position = TimeSpan.FromSeconds(0);
        }

        /// <summary>
        /// Call Back to Region
        /// </summary>
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
