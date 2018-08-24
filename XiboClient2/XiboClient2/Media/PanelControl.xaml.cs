using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using XiboClient2.Processes;
using XiboClient2.Processor.Log;
using XiboClient2.Settings;

namespace XiboClient2.Media
{
    public delegate void MediaFinish(int MediaId);
    /// <summary>
    /// Interaction logic for PanelControl.xaml
    /// </summary>
    public partial class PanelControl : UserControl
    {
        private string RegionId;
        //private System.Timers.Timer TimerRegionLoop;
        private System.Timers.Timer BgAudioLoop;
        private int loopCounter = 0;
        private int audioLoop = 0;


        private MediaElement audioBgElemnt;

        //Region Media List
        List<MediaOption> MediaList = new List<MediaOption>();
        List<AudioOption> AudioList = new List<AudioOption>();

        //callback method
        FinishRegionCallback _callbackMethod;

        MediaFinish MediaFinish;

        //default durauion region
        public double duration = 10;

        public PanelControl()
        {
            InitializeComponent();
        }

        public PanelControl(string index, FinishRegionCallback _callbackMethod)
        {
            InitializeComponent();
            this.RegionId = index;
            this._callbackMethod = _callbackMethod;
            Loaded += PanelControl_Loaded;
            Window window = Window.GetWindow(this);
            Unloaded += PanelControl_Unloaded;
        }

        private void PanelControl_Unloaded(object sender, RoutedEventArgs e)
        {
            try
            {
                this.LayoutPanel.Children.Clear();
                //TimerRegionLoop.Stop();
                MediaList.Clear();
                AudioList.Clear();

                Console.WriteLine("Region " + RegionId + " unload");
            }
            catch (Exception ex)
            {
                PlayerSettings.ErrorLog(ex);
            }
        }

        private void PanelControl_Loaded(object sender, RoutedEventArgs e)
        {
            loopCounter = 0;
            try
            {
                //Filter Media in this Region
                MediaList = PlayerSettings.MediaNodeList.Where(find => find.regionId == RegionId).ToList();
                ViewRegionMedia(loopCounter);
            }
            catch (Exception ex)
            {
                PlayerSettings.ErrorLog(ex);
            }
        }

        /// <summary>
        /// Call Back when this refion finished
        /// </summary>
        private void CallToBack()
        {
            int regionid = Convert.ToInt16(RegionId);
            if (_callbackMethod != null)
            {
                _callbackMethod(regionid);

                Console.WriteLine("Call Back with " + regionid);
            }
        }


        /// <summary>
        /// View Region Details
        /// </summary>
        /// <param name="index"></param>
        private void ViewRegionMedia(int index)
        {
            //int index = Convert.ToInt32(indexObj);
            try
            {
                int mediaId = MediaList[index].mediaId;

                if (MediaList[index].audio)
                {
                    BackgroundAudio(mediaId);
                }

                string renderType = MediaList[index].render;
                string type = MediaList[index].type;

                if (renderType == "html")
                {
                    //RenderHtmlWeb(index);
                    RenderWeb(index);
                }
                else
                {
                    switch (type)
                    {
                        case "image":
                            RenderImage(index);
                            break;

                        case "powerpoint":
                            break;

                        case "video":
                            RenderVideo(index);
                            break;

                        case "localvideo":
                            RenderLocalVideo(index);
                            break;

                        case "audio":
                            RenderAudio(index);
                            break;

                        case "datasetview":
                        case "embedded":
                        case "ticker":
                        case "text":
                        case "webpage":
                            RenderWeb(index);
                            break;

                        case "flash":
                            RenderFlash(index);
                            break;

                        case "shellcommand":
                            RenderSellCmd(index);
                            break;

                        default:
                            throw new InvalidOperationException("Not a valid media node type:");
                    }
                }

                Console.WriteLine("Region " + RegionId + " Loaded");
            }
            catch (Exception ex)
            {
                Trace.WriteLine(new LogMessage("Media Load", "Media Load Fail"), LogType.Audit.ToString());
                PlayerSettings.ErrorLog(ex);
            }

        }


        /// <summary>
        /// Local Video 
        /// </summary>
        /// <param name="index"></param>
        private void RenderLocalVideo(int index)
        {
            //media details
            MediaFinish = new MediaFinish(CompleateMedia);

            LocalVideoControl localVideo = new LocalVideoControl(MediaList[index], MediaFinish);
            this.LayoutPanel.Children.Add(localVideo);
        }

        /// <summary>
        /// Background audio play
        /// </summary>
        /// <param name="mediaId"></param>
        private void BackgroundAudio(int mediaId)
        {
            AudioList = PlayerSettings.AudioNodeList.Where(x => x.mediaId == mediaId).ToList();
            if (AudioList.Count > 1)
            {
                audioBgElemnt = new MediaElement()
                {
                    Name = "bg"
                };

                string uri = AudioList[audioLoop].audioUrl;
                string _filePath = Uri.UnescapeDataString(uri).Replace('+', ' ');

                TimeSpan audioDuration = PlayerSettings.GetVideoDuration(_filePath);
                double audioLength = audioDuration.TotalSeconds;
                BgAudioLoop = new System.Timers.Timer();
                BgAudioLoop.Elapsed += new ElapsedEventHandler(audioLoopTimer_Tick);
                BgAudioLoop.Interval = 1000 * audioLength;
                BgAudioLoop.Start();

                PlayBackgroundAudio(AudioList[audioLoop].audioUrl);
            }
            else if (AudioList.Count == 1)
            {
                audioBgElemnt = new MediaElement()
                {
                    Name = "bg"
                };
                if (AudioList[audioLoop].loop == 1)
                {
                    audioBgElemnt.MediaEnded += AudioBgElemnt_MediaEnded;
                }
                PlayBackgroundAudio(AudioList[audioLoop].audioUrl);
            }
            this.LayoutPanel.Children.Add(audioBgElemnt);
        }

        /// <summary>
        /// Replay audio
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AudioBgElemnt_MediaEnded(object sender, RoutedEventArgs e)
        {
            audioBgElemnt.Position = TimeSpan.FromSeconds(0);
            audioBgElemnt.Play();
        }

        /// <summary>
        /// Multiple audio paly
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void audioLoopTimer_Tick(object sender, ElapsedEventArgs e)
        {
            BgAudioLoop.Stop();
            audioLoop++;
            if (audioLoop > AudioList.Count)
            {
                audioLoop = 0;
                string uri = AudioList[audioLoop].audioUrl;
                string _filePath = Uri.UnescapeDataString(uri).Replace('+', ' ');
                TimeSpan audioDuration = PlayerSettings.GetVideoDuration(_filePath);
                double audioLength = audioDuration.TotalSeconds;
                BgAudioLoop.Interval = 1000 * audioLength;
                PlayBackgroundAudio(AudioList[audioLoop].audioUrl);

            }
            else
            {
                string uri = AudioList[audioLoop].audioUrl;
                string _filePath = Uri.UnescapeDataString(uri).Replace('+', ' ');
                TimeSpan audioDuration = PlayerSettings.GetVideoDuration(_filePath);
                double audioLength = audioDuration.TotalSeconds;
                BgAudioLoop.Interval = 1000 * audioLength;
                PlayBackgroundAudio(AudioList[audioLoop].audioUrl);
            }
            BgAudioLoop.Start();
        }

        /// <summary>
        /// Play Background audio
        /// </summary>
        /// <param name="url"></param>
        private void PlayBackgroundAudio(string url)
        {
            this.Dispatcher.Invoke(() =>
            {
                Uri uribg = new Uri(url);
                audioBgElemnt.Source = uribg;
            });


        }

        /// <summary>
        /// Render Shell command
        /// </summary>
        /// <param name="index"></param>
        private void RenderSellCmd(int index)
        {
            int mediaId = MediaList[index].mediaId;

            string text = Uri.UnescapeDataString(MediaList[index].windowsCommand).Replace('+', ' ');

            ShellCommand.ShellCommandDetails(
                MediaList[index].launchThroughCmd,
                MediaList[index].terminateCommand,
                MediaList[index].useTaskkill,
                MediaList[index].windowsCommand
                );

        }

        /// <summary>
        /// Render audio track.
        /// </summary>
        /// <param name="index"></param>
        private void RenderAudio(int index)
        {
            MediaFinish = new MediaFinish(CompleateMedia);
            AudioControl AudioPanel = new AudioControl(MediaList[index], MediaFinish);
            this.LayoutPanel.Children.Add(AudioPanel);
        }

        /// <summary>
        /// Flash
        /// </summary>
        /// <param name="index"></param>
        private void RenderFlash(int index)
        {
            MediaFinish = new MediaFinish(CompleateMedia);
            FlashControl FlashPanel = new FlashControl(MediaList[index], MediaFinish);
            this.LayoutPanel.Children.Add(FlashPanel);
        }

        /// <summary>
        /// View Web pages
        /// </summary>
        /// <param name="index"></param>
        private void RenderWeb(int index)
        {
            MediaFinish = new MediaFinish(CompleateMedia);
            WebControl webPanel = new WebControl(MediaList[index], MediaFinish);
            this.LayoutPanel.Children.Add(webPanel);
        }

        /// <summary>
        /// View Image
        /// </summary>
        /// <param name="index"></param>
        private void RenderImage(int index)
        {
            MediaFinish = new MediaFinish(CompleateMedia);
            ImageControl webPanel = new ImageControl(MediaList[index], MediaFinish);
            Console.WriteLine("add new image");
            this.LayoutPanel.Children.Add(webPanel);
        }

        /// <summary>
        /// View Video
        /// </summary>
        /// <param name="index"></param>
        private void RenderVideo(int index)
        {
            MediaFinish = new MediaFinish(CompleateMedia);
            VideoControl video = new VideoControl(MediaList[index], MediaFinish);
            this.LayoutPanel.Children.Add(video);
        }

        /// <summary>
        /// Call back method when media compleated
        /// </summary>
        /// <param name="regionId"></param>
        private void CompleateMedia(int regionId)
        {
            try
            {
                if (MediaList.Count == 1)
                {
                    Trace.WriteLine(new LogMessage("Media - SignalElapsedEvent", "Media Complete"), LogType.Audit.ToString());
                    loopCounter = 0;
                    CallToBack();
                }
                else
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        this.LayoutPanel.Children.Clear();
                        loopCounter++;
                        if (MediaList.Count <= loopCounter)
                        {
                            loopCounter = 0;
                            CallToBack();
                        }
                        if (MediaList.Count >= loopCounter)
                        {

                            ViewRegionMedia(loopCounter);

                        }

                    });
                }

            }
            catch (Exception ex)
            {
                PlayerSettings.ErrorLog(ex);
            }
        }
    }
}
