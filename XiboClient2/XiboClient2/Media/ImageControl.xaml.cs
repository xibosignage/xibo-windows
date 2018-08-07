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
    /// Interaction logic for ImageControl.xaml
    /// </summary>
    public partial class ImageControl : UserControl
    {

        private MediaOption obj;
        private Image img;
        private System.Timers.Timer TimerMediaImg;

        //default durauion region
        double duration = 0;

        MediaFinish _MediaFinish;

        public ImageControl(MediaOption item, MediaFinish _MediaFinish)
        {
            InitializeComponent();
            this.obj = item;
            this._MediaFinish = _MediaFinish;
            Loaded += ImageControl_Loaded;
            Unloaded += ImageControl_Unloaded;
        }

        private void ImageControl_Unloaded(object sender, RoutedEventArgs e)
        {
            img = null;
            Console.WriteLine("Region" + obj.regionId + "Media Unload");
            TimerMediaImg.Stop();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ImageControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                RenderImage();
            }
            catch (Exception ex)
            {
                PlayerSettings.ErrorLog(ex);
            }
        }

        private void RenderImage()
        {
            //media details
            int mediaId = obj.mediaId;
            string uri = obj.uri;

            Uri uriImage = new Uri(uri);
            string _filePath = Uri.UnescapeDataString(uri).Replace('+', ' ');

            int _top = obj.top;
            int _left = obj.left;

            duration = obj.duration;

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

            img = new Image()
            {
                Name = "Img" + obj.regionId,
            };

            img.Source = new BitmapImage(uriImage);

            this.ImagePanel.Children.Add(img);


            TimerMediaImg = new System.Timers.Timer();
            TimerMediaImg.Elapsed += new ElapsedEventHandler(MediaTimerImg_Tick);
            TimerMediaImg.Interval = 1000 * duration;
            TimerMediaImg.Start();

            if (transIn != null)
            {
                MediaSupport.MoveAnimation(img, OpacityProperty, transIn, transInDirection, transInDuration, "in", _top, _left);
            }
            double transOutStartTime = duration - (transOutDuration / 1000);
            if (transOut != null)
            {
                var timerTransition = new DispatcherTimer { Interval = TimeSpan.FromSeconds(transOutStartTime) };
                timerTransition.Start();
                timerTransition.Tick += (sender1, args) =>
                {
                    timerTransition.Stop();
                    MediaSupport.MoveAnimation(img, OpacityProperty, transOut, transOutDirection, transOutDuration, "out", _top, _left);
                };
            }
        }

        private void MediaTimerImg_Tick(object sender, ElapsedEventArgs e)
        {
            TimerMediaImg.Stop();
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
