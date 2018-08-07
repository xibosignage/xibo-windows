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
    /// Interaction logic for LocalVideoControl.xaml
    /// </summary>
    public partial class LocalVideoControl : UserControl
    {
        private MediaOption obj;
        private MediaElement medaiElemnt;
        private System.Timers.Timer TimerMedia;

        //default durauion region
        double duration = 0;

        MediaFinish _MediaFinish;

        public LocalVideoControl(MediaOption item, MediaFinish _MediaFinish)
        {
            InitializeComponent();
            this.obj = item;
            this._MediaFinish = _MediaFinish;
            Loaded += LocalVideoControl_Loaded;
            Unloaded += LocalVideoControl_Unloaded;
        }

        private void LocalVideoControl_Unloaded(object sender, RoutedEventArgs e)
        {
            TimerMedia.Stop();
            medaiElemnt = null;
        }

        private void LocalVideoControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                RenderLocalVideo();
            }
            catch (Exception ex)
            {
                PlayerSettings.ErrorLog(ex);
            }
        }

        /// <summary>
        /// view Local video
        /// </summary>
        private void RenderLocalVideo()
        {
            int mediaID = obj.mediaId;
            string uri = obj.uri;

            string _filePath = Uri.UnescapeDataString(uri);
            Uri file = new Uri(_filePath);

            int _top = obj.top;
            int _left = obj.left;

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

            medaiElemnt = new MediaElement
            {
                Source = file
            };
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
            this.LocalVideoPanel.Children.Add(medaiElemnt);

            TimerMedia = new System.Timers.Timer();
            TimerMedia.Elapsed += new ElapsedEventHandler(MediaTimer_Tick);
            TimerMedia.Interval = 1000 * duration;
            TimerMedia.Start();

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
            TimerMedia.Stop();
        }

        private void CallToBackRegion()
        {
            int mediaId = Convert.ToInt16(obj.mediaId);
            if (_MediaFinish != null)
            {
                _MediaFinish(mediaId);

            }
            TimerMedia.Stop();
        }

        private void MedaiElemnt_MediaEnded(object sender, RoutedEventArgs e)
        {
            medaiElemnt.Position = TimeSpan.FromSeconds(0);
        }

    }
}
