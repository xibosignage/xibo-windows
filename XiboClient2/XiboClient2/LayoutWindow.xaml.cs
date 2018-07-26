using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using XiboClient2.Media;
using XiboClient2.Processes;
using XiboClient2.Processor.Settings;
using XiboClient2.Settings;

namespace XiboClient2
{
    public delegate void FinishRegionCallback(int regionId);

    /// <summary>
    /// Interaction logic for LayoutWindow.xaml
    /// </summary>
    public partial class LayoutWindow : Window
    {
        double _layoutDuration = 0;

        //Loop Details
        int _layoutListID = 0;

        FinishRegionCallback callback;

        /// <summary>
        /// Time for Layout Change
        /// </summary>
        DispatcherTimer dispatcherTimer = new DispatcherTimer();

        //Region media list
        private List<MediaOption> RegionsMedia = new List<MediaOption>();
        //Region duration list
        private List<double> RegionDurationList = new List<double>();

        private AppStartup _appStart = new AppStartup();


        //private ConcurrentStack<int> FinishedRegionList = new ConcurrentStack<int>();
        private List<int> FinishedRegionList = new List<int>();

        public LayoutWindow()
        {
            InitializeComponent();
            Loaded += LayoutWindow_Loaded;
            this.Closing += LayoutWindow_Closing;
        }

        private void LayoutWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //throw new NotImplementedException();
            _appStart.FormClosing();
        }

        private void LayoutWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                //Read Layout List
                RenderLayout.PrepareLayout(RenderSchedule.ListLayouts[_layoutListID]);

                //Reoder
                RenderRegion.RegionList = RenderRegion.RegionList.OrderBy(x => x.zIndex).ToList();

                //Timer
                dispatcherTimer.Tick += DispatcherTimer_Tick;

                //Stat view
                ViewLayout();

                this.Cursor = Cursors.None;
                var bc = new BrushConverter();
                this.Background = (Brush)bc.ConvertFrom(LayoutOption.backgroundColor);

                //set background image
                if (LayoutOption.backgroundImage != "")
                {
                    string BackgroundImage = LayoutOption.backgroundImage;
                    this.Background = new ImageBrush(new BitmapImage(new Uri(BackgroundImage)));
                }
            }
            catch (Exception ex)
            {
                PlayerSettings.ErrorLog(ex);
            }
        }

        /// <summary>
        /// Layout Change timer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                if (FinishedRegionList.Count >= RenderRegion.RegionList.Count)
                {
                    NextLayout();
                }
                else
                {
                    dispatcherTimer.Stop();
                    dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
                    dispatcherTimer.Start();

                }
            }
            catch (Exception ex)
            {

            }

        }

        /// <summary>
        /// Next Layout
        /// </summary>
        private void NextLayout()
        {
            Console.WriteLine("New Layout");
            try
            {
                callback = null;

                PlayerSettings.firstLoadCheck = 1;
                _layoutListID++;
                FinishedRegionList.Clear();
                RenderRegion.RegionList.Clear();
                RenderLayout.MediaNodeList.Clear();
                RenderLayout.AudioNodeList.Clear();
                this.LayoutRoot.Children.Clear();


                _layoutDuration = 0;

                if (_layoutListID < RenderSchedule.ListLayouts.Count)
                {
                    RenderLayout.PrepareLayout(RenderSchedule.ListLayouts[_layoutListID]);
                    RenderRegion.RegionList = RenderRegion.RegionList.OrderBy(x => x.zIndex).ToList();
                    dispatcherTimer.Stop();
                    ViewLayout();

                    if (LayoutOption.backgroundImage != "")
                    {
                        var bc = new BrushConverter();
                        this.Background = (Brush)bc.ConvertFrom(LayoutOption.backgroundColor);
                        string BackgroundImage = LayoutOption.backgroundImage;
                        this.Background = new ImageBrush(new BitmapImage(new Uri(BackgroundImage)));
                    }
                    else
                    {
                        this.Background = null;
                        var bc = new BrushConverter();
                        this.Background = (Brush)bc.ConvertFrom(LayoutOption.backgroundColor);
                    }

                }
                else
                {
                    _layoutListID = 0;
                    //Recheck Scheduler 
                    RenderSchedule.ReadSchedule(PlayerSettings.scheduleName);
                    RenderLayout.PrepareLayout(RenderSchedule.ListLayouts[_layoutListID]);
                    RenderRegion.RegionList = RenderRegion.RegionList.OrderBy(x => x.zIndex).ToList();
                    dispatcherTimer.Stop();
                    ViewLayout();
                    if (LayoutOption.backgroundImage != "")
                    {
                        var bc = new BrushConverter();
                        this.Background = (Brush)bc.ConvertFrom(LayoutOption.backgroundColor);
                        string BackgroundImage = LayoutOption.backgroundImage;
                        this.Background = new ImageBrush(new BitmapImage(new Uri(BackgroundImage)));
                    }
                    else
                    {
                        this.Background = null;
                        var bc = new BrushConverter();
                        this.Background = (Brush)bc.ConvertFrom(LayoutOption.backgroundColor);
                    }
                }

                Console.WriteLine("Region Count " + RenderRegion.RegionList.Count);

            }
            catch (Exception ex)
            {
                PlayerSettings.ErrorLog(ex);
            }
        }

        /// <summary>
        /// View Layouts
        /// </summary>
        /// <param name="LayoutID"></param>
        private void ViewLayout()
        {
            try
            {
                if (RegionDurationList.Count > 0)
                {
                    RegionDurationList.Clear();
                }
                if (RenderRegion.RegionList.Count > 0)
                {
                    try
                    {
                        for (int listIndex = 0; listIndex < RenderRegion.RegionList.Count; listIndex++)
                        {
                            RegionDuraion(listIndex);
                            SetRegionPanel(listIndex);
                        }

                        _layoutDuration = RegionDurationList.Max();

                        Console.WriteLine("Layout Durayion " + _layoutDuration);

                        int _totolSecounds = Convert.ToInt32(_layoutDuration);
                        dispatcherTimer.Interval = new TimeSpan(0, 0, _totolSecounds);
                        dispatcherTimer.Start();
                    }
                    catch (Exception ex)
                    {
                        PlayerSettings.ErrorLog(ex);
                        DefaultSplashScreen();
                    }
                }
                else
                {
                    DefaultSplashScreen();
                }
            }
            catch (Exception ex)
            {
                PlayerSettings.ErrorLog(ex);
            }


        }

        /// <summary>
        /// get all Media duration in one region
        /// </summary>
        /// <param name="region"></param>
        private void RegionDuraion(int region)
        {
            try
            {
                if (RegionsMedia.Count > 0)
                {
                    RegionsMedia.Clear();
                }
                //Region ID
                string regionId = RenderRegion.RegionList[region].regionId;
                //Get Media in to one region
                RegionsMedia = RenderLayout.MediaNodeList.Where(x => x.regionId == regionId).ToList();

                double regionDuration = 0;

                for (int length = 0; length < RegionsMedia.Count; length++)
                {
                    string type = RegionsMedia[length].type;
                    int duratuin = RegionsMedia[length].duration;
                    if (type == "video" && duratuin == 0)
                    {
                        //IsVideo = true;
                        TimeSpan videoDuration = PlayerSettings.GetVideoDuration(RegionsMedia[length].uri);
                        regionDuration += videoDuration.TotalSeconds;
                    }
                    else
                    {
                        try
                        {
                            regionDuration += RegionsMedia[length].duration;
                        }
                        catch (Exception ex)
                        {
                            regionDuration += 0;
                        }

                    }
                    if (RegionsMedia[length].transInDuration != 0)
                    {
                        regionDuration += (RegionsMedia[length].transInDuration / 1000);
                    }
                    if (RegionsMedia[length].transOutDuration != 0)
                    {
                        regionDuration += (RegionsMedia[length].transOutDuration / 1000);
                    }
                }

                RegionDurationList.Add(regionDuration);
            }
            catch (Exception ex)
            {
                PlayerSettings.ErrorLog(ex);
                DefaultSplashScreen();
            }

        }

        /// <summary>
        /// Set Panle using Region Details
        /// </summary>
        /// <param name="index"></param>
        private void SetRegionPanel(int index)
        {
            try
            {
                callback = new FinishRegionCallback(CompleateRegion);

                string _regionID = RenderRegion.RegionList[index].regionId;

                int _widht = RenderRegion.RegionList[index].width;
                int _height = RenderRegion.RegionList[index].height;
                int _Top = RenderRegion.RegionList[index].top;
                int _left = RenderRegion.RegionList[index].left;

                PanelControl panelCon = new PanelControl(_regionID, callback)
                {
                    Width = _widht,
                    Height = _height,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Top,
                    Margin = new Thickness(_left, _Top, 0, 0)
                };

                this.LayoutRoot.Children.Add(panelCon);
            }
            catch (Exception ex)
            {
                PlayerSettings.ErrorLog(ex);
                DefaultSplashScreen();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void DefaultSplashScreen()
        {
            this.LayoutRoot.Children.Clear();
            //DefaultScreenControl defaultScreen = new DefaultScreenControl();
            //this.LayoutRoot.Children.Add(defaultScreen);
        }


        public void CompleateRegion(int regionID)
        {
            try
            {
                ///Thread.Sleep(10000);
                Console.WriteLine("Finished Region " + regionID);

                bool isInList = FinishedRegionList.IndexOf(regionID) != -1;

                if (!isInList)
                {
                    FinishedRegionList.Add(regionID);
                    Console.WriteLine("Add Finished Region List in " + regionID);
                }

                //
                //FinishedRegionList = FinishedRegionList.Distinct().ToList();

                Console.WriteLine("Finished Region List Count " + FinishedRegionList.Count);
                // critical section

            }
            catch (Exception ex)
            {
                PlayerSettings.ErrorLog(ex);
            }

        }
    }
}
