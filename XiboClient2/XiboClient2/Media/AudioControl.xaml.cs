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
using XiboClient2.Settings;

namespace XiboClient2.Media
{
    /// <summary>
    /// Interaction logic for AudioControl.xaml
    /// </summary>
    public partial class AudioControl : UserControl
    {
        private MediaOption obj;
        private MediaElement audioElemnt;
        private System.Timers.Timer TimerMediaAui;

        //default durauion region
        double duration = 0;

        MediaFinish _MediaFinish;

        public AudioControl(MediaOption item, MediaFinish _MediaFinish)
        {
            InitializeComponent();
            this.obj = item;
            this._MediaFinish = _MediaFinish;
            Loaded += AudioControl_Loaded;
            Unloaded += AudioControl_Unloaded;
        }

        private void AudioControl_Unloaded(object sender, RoutedEventArgs e)
        {
            audioElemnt = null;
            TimerMediaAui.Stop();
            this.AudioPanel.Children.Clear();
        }

        private void AudioControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                RenderAudio();
            }
            catch (Exception ex)
            {
                PlayerSettings.ErrorLog(ex);
            }
        }

        private void RenderAudio()
        {
            int mediaId = obj.mediaId;
            string uri = obj.uri;

            Uri uri2 = new Uri(uri);
            string _filePath = Uri.UnescapeDataString(uri).Replace('+', ' ');

            duration = obj.duration;
            if (duration == 0)
            {
                TimeSpan videoDuration = PlayerSettings.GetVideoDuration(_filePath);
                duration = videoDuration.TotalSeconds;
            }

            audioElemnt = new MediaElement();
            audioElemnt.Source = uri2;
            //medaiElemnt.Volume = 0;
            if (obj.loop == 1)
            {
                audioElemnt.MediaEnded += AudioElemnt_MediaEnded;
            }

            TimerMediaAui = new System.Timers.Timer();
            TimerMediaAui.Elapsed += new ElapsedEventHandler(MediaTimerAui_Tick);
            TimerMediaAui.Interval = 1000 * duration;
            TimerMediaAui.Start();

            this.AudioPanel.Children.Add(audioElemnt);
        }

        private void MediaTimerAui_Tick(object sender, ElapsedEventArgs e)
        {
            CallToBackRegion();
            TimerMediaAui.Stop();
        }

        private void AudioElemnt_MediaEnded(object sender, RoutedEventArgs e)
        {
            audioElemnt.Position = TimeSpan.FromSeconds(0);
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
