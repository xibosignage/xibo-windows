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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using XiboClient2.Processes;
using XiboClient2.Settings;

namespace XiboClient2.Media
{
    public delegate void FinishRegionCallback(int regionId);
    

    public partial class LayoutControl : UserControl
    {
        double _layoutDuration = 0;
        
        FinishRegionCallback callback;
        LayoutOption _layout = new LayoutOption();

        /// <summary>
        /// Time for Layout Change
        /// </summary>
        DispatcherTimer dispatcherTimer = new DispatcherTimer();

        //Region media list
        private List<MediaOption> RegionsMedia = new List<MediaOption>();
        //Region duration list
        private List<double> RegionDurationList = new List<double>();

        
        private List<int> FinishedRegionList = new List<int>();

        private string _layoutID;
        FinishLayoutCallback _finishLayout;

        public LayoutControl()
        {
            InitializeComponent();
            Loaded += LayoutControl_Loaded;
            Unloaded += LayoutControl_Unloaded;
        }

        public LayoutControl(string layoutID, FinishLayoutCallback finishLayout)
        {
            InitializeComponent();

            this._layoutID = layoutID;
            this._finishLayout = finishLayout;


            Loaded += LayoutControl_Loaded;
            Unloaded += LayoutControl_Unloaded;
        }

        private void LayoutControl_Unloaded(object sender, RoutedEventArgs e)
        {
            try
            {
                this.LayoutRoot.Children.Clear();
            }
            catch(Exception ex)
            {
                PlayerSettings.ErrorLog(ex);
            }
            
        }

        private void LayoutControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                
                FinishedRegionList.Clear();
                //this.LayoutRoot.Children.Clear();

                //Read Layout List
                //RenderLayout.PrepareLayout(RenderSchedule.ListLayouts[_layoutListID]);
                RenderLayout.PrepareLayout(_layoutID, _layout);

                //Reoder
                _layout.RegionList = _layout.RegionList.OrderBy(x => x.zIndex).ToList();

                //Timer
                dispatcherTimer.Tick += DispatcherTimer_Tick;

                //Stat view
                ViewLayout();

                //PlayerSettings.firstLoadCheck = 1;

                this.Cursor = Cursors.None;
                var bc = new BrushConverter();
                this.Background = (Brush)bc.ConvertFrom(_layout.backgroundColor);

                //set background image
                if (_layout.backgroundImage != "")
                {
                    string BackgroundImage = _layout.backgroundImage;
                    this.Background = new ImageBrush(new BitmapImage(new Uri(BackgroundImage)));
                }
            }
            catch (Exception ex)
            {
                PlayerSettings.ErrorLog(ex);
            }
        }

        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                if (FinishedRegionList.Count >= _layout.RegionList.Count)
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
                PlayerSettings.ErrorLog(ex);
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
                FinishedRegionList.Clear();
                //PlayerSettings.RegionList.Clear();
                //PlayerSettings.MediaNodeList.Clear();
                //PlayerSettings.AudioNodeList.Clear();
                this.LayoutRoot.Children.Clear();


                _layoutDuration = 0;

                if(_finishLayout != null)
                {
                    _finishLayout();
                }

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
                if (_layout.RegionList.Count > 0)
                {
                    try
                    {
                        for (int listIndex = 0; listIndex < _layout.RegionList.Count; listIndex++)
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
                        //DefaultSplashScreen();
                    }
                }
                else
                {
                   // DefaultSplashScreen();
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
                string regionId = _layout.RegionList[region].regionId;
                //Get Media in to one region
                RegionsMedia = _layout.MediaNodeList.Where(x => x.regionId == regionId).ToList();

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
                //DefaultSplashScreen();
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
                callback = new FinishRegionCallback(CompleteRegion);

                string _regionID = _layout.RegionList[index].regionId;

                int _widht = _layout.RegionList[index].width;
                int _height = _layout.RegionList[index].height;
                int _Top = _layout.RegionList[index].top;
                int _left = _layout.RegionList[index].left;

                PanelControl panelCon = new PanelControl(_regionID, _layout, callback)
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
            }
        }

        public void CompleteRegion(int regionID)
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
