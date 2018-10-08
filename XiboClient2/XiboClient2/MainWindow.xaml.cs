using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
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
using System.Xml.Serialization;
using System.Runtime.InteropServices;
using XiboClient2.Processor.Log;
using XiboClient2.Processor.XmdsAgents;
using XiboClient2.Processor.Settings;
using XiboClient2.Processor.Forms;
using System.Threading;
using XiboClient2.Processor.Logic;
using System.Windows.Forms;
using XiboClient2.Settings;
using XiboClient2.Processes;
using System.Reflection;
using XiboClient2.Processor.Control;
using System.ComponentModel;
using XiboClient2.Processor.Error;
using XiboClient2.Media;

namespace XiboClient2
{

    public delegate void FinishLayoutCallback();

    public partial class MainWindow : Window
    {
        FinishLayoutCallback finishLayout;

        //private string scheduleName = "schedule.xml";
        private bool _screenSaver = false;

        private Schedule _schedule;

        private bool _changingLayout = false;
        private int _scheduleId;
        private int _layoutId;
        private bool _showingSplash = false;

        //double _layoutWidth;
        //double _layoutHeight;
        //double _scaleFactor;
        //private Size _clientSize;

        private StatLog _statLog;
        private Stat _stat;
        private CacheManager _cacheManager;

        private ClientInfo _clientInfoForm;

        private delegate void ChangeToNextLayoutDelegate(string layoutPath);
        private delegate void ManageOverlaysDelegate(Collection<ScheduleItem> overlays);

        private LayoutControl layout;

        /// <summary>
        /// Border style - usually none, but useful for debugging.
        /// </summary>
        //private BorderStyle _borderStyle = BorderStyle.None;

        [FlagsAttribute]
        enum EXECUTION_STATE : uint
        {
            ES_AWAYMODE_REQUIRED = 0x00000040,
            ES_CONTINUOUS = 0x80000000,
            ES_DISPLAY_REQUIRED = 0x00000002,
            ES_SYSTEM_REQUIRED = 0x00000001
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern EXECUTION_STATE SetThreadExecutionState(EXECUTION_STATE esFlags);

        // Changes the parent window of the specified child window
        [DllImport("user32.dll")]
        private static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        // Changes an attribute of the specified window
        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        // Retrieves information about the specified window
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        // Retrieves the coordinates of a window's client area
        [DllImport("user32.dll")]
        private static extern bool GetClientRect(IntPtr hWnd, out Rectangle lpRect);


        public MainWindow(IntPtr previewHandle)
        {
            InitializeComponent();

            IntPtr hwnd = this.Handle;

            //AppStartup.ScreenSaver(previewHandle, hwnd);

            InitializeScreenSaver(true);
            InitializeXibo();
        }

        public IntPtr Handle

        {

            get { return (new System.Windows.Interop.WindowInteropHelper(this)).Handle; }

        }

        public MainWindow()
        {
            InitializeComponent();
            InitializeXibo();
        }

        public MainWindow(bool screenSaver)
        {
            InitializeComponent();

            if (screenSaver)
                InitializeScreenSaver(false);

            InitializeXibo();
        }

        private void InitializeXibo()
        {
            this.Name = ApplicationSettings.AppProductName;


            Initialize();

            this.Loaded += MainWindow_Loaded;
            this.Closing += MainWindow_Closing;
            this.ContentRendered += MainWindow_ContentRendered;
            this.KeyUp += MainWindow_KeyUp;
        }

        private void MainWindow_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if(e.Key == Key.I)
            {
                Instance_KeyPress("ClientInfo");
            }
            
        }

        /// <summary>
        /// main window closing - All therds stop
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            // We want to tidy up some stuff as this form closes.
            Trace.Listeners.Remove("ClientInfo TraceListener");

            try
            {
                // Close the client info screen
                if (_clientInfoForm != null)
                    _clientInfoForm.Hide();

                // Stop the schedule object
                if (_schedule != null)
                    _schedule.Stop();

                // Flush the stats
                if (_statLog != null)
                    _statLog.Flush();

                // Write the CacheManager to disk
                if (_cacheManager != null)
                    _cacheManager.WriteCacheManager();
            }
            catch (NullReferenceException)
            {
                // Stopped before we really started, nothing to do
            }

            // Flush the logs
            Trace.Flush();

            this.root.Children.Clear();

            Environment.Exit(0);
        }

        private void MainWindow_ContentRendered(object sender, EventArgs e)
        {
            // Create a cachemanager
            SetCacheManager();

            try
            {
                // Create the Schedule
                _schedule = new Schedule(ApplicationSettings.Default.LibraryPath + @"\" + ApplicationSettings.Default.ScheduleFile, ref _cacheManager, ref _clientInfoForm);

                // Bind to the schedule change event - notifys of changes to the schedule
                _schedule.ScheduleChangeEvent += ScheduleChangeEvent;

                // Bind to the overlay change event
                _schedule.OverlayChangeEvent += ScheduleOverlayChangeEvent;

                // Initialize the other schedule components
                _schedule.InitializeComponents();

                // Set this form to topmost
#if !DEBUG
                //if (!_screenSaver)
                //    TopMost = true;
#endif
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message, LogType.Error.ToString());
                Close();
                //Dispose();
            }
        }

        /// <summary>
        /// main window loding event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Is the mouse enabled?
            if (!ApplicationSettings.Default.EnableMouse)
            {
                // Hide the cursor
            }
            
            // Move the cursor to the starting place
            if (!_screenSaver)
            {
                //SetCursorStartPosition();
            }

            // Show the splash screen
            ShowSplashScreen();

            // Change the default Proxy class
            OptionForm.SetGlobalProxy();

            // UserApp data
            Debug.WriteLine(new LogMessage("MainForm_Load", "User AppData Path: " + ApplicationSettings.Default.LibraryPath), LogType.Info.ToString());
        }

        public void Initialize()
        {
            //Thread.CurrentThread.Name = "UI Thread";

            // Check the directories exist
            if (!Directory.Exists(ApplicationSettings.Default.LibraryPath + @"\backgrounds\"))
            {
                // Will handle the create of everything here
                Directory.CreateDirectory(ApplicationSettings.Default.LibraryPath + @"\backgrounds");
            }

            // Default the XmdsConnection
            ApplicationSettings.Default.XmdsLastConnection = DateTime.MinValue;

            // Show in taskbar
            ShowInTaskbar = ApplicationSettings.Default.ShowInTaskbar;

            // Setup the proxy information
            OptionForm.SetGlobalProxy();

            _statLog = new StatLog();

            // Create the info form
            _clientInfoForm = new ClientInfo();
            _clientInfoForm.Hide();

            // Define the hotkey
            Keys key;
            try
            {
                key = (Keys)Enum.Parse(typeof(Keys), ApplicationSettings.Default.ClientInformationKeyCode.ToUpper());
            }
            catch
            {
                // Default back to I
                key = Keys.I;
            }

            KeyStore.Instance.AddKeyDefinition("ClientInfo", key, ((ApplicationSettings.Default.ClientInfomationCtrlKey) ? Keys.Control : Keys.None));

            // Register a handler for the key event
            KeyStore.Instance.KeyPress += Instance_KeyPress;

            // Trace listener for Client Info
            ClientInfoTraceListener clientInfoTraceListener = new ClientInfoTraceListener(_clientInfoForm);
            clientInfoTraceListener.Name = "ClientInfo TraceListener";
            Trace.Listeners.Add(clientInfoTraceListener);

            // Log to disk?
            if (!string.IsNullOrEmpty(ApplicationSettings.Default.LogToDiskLocation))
            {
                TextWriterTraceListener listener = new TextWriterTraceListener(ApplicationSettings.Default.LogToDiskLocation);
                Trace.Listeners.Add(listener);
            }

#if !DEBUG
            // Initialise the watchdog
            if (!_screenSaver)
            {
                try
                {
                    // Update/write the status.json file
                    File.WriteAllText(System.IO.Path.Combine(ApplicationSettings.Default.LibraryPath, "status.json"), "{\"lastActivity\":\"" + DateTime.Now.ToString() + "\"}");

                    // Start watchdog
                    WatchDogManager.Start();
                }
                catch (Exception e)
                {
                    Trace.WriteLine(new LogMessage("MainForm - InitializeXibo", "Cannot start watchdog. E = " + e.Message), LogType.Error.ToString());
                }
            }
#endif
            // An empty set of overlay regions
            //_overlays = new Collection<Region>();


            Trace.WriteLine(new LogMessage("MainForm", "Client Initialised"), LogType.Info.ToString());
        }
        

        private void InitializeScreenSaver(bool preview)
        {
            _screenSaver = true;

            // Configure some listeners for the mouse (to quit)
            if (!preview)
            {
                KeyStore.Instance.ScreenSaver = true;

                MouseInterceptor.Instance.MouseEvent += Instance_MouseEvent;
            }
        }

        void Instance_MouseEvent()
        {
            Close();
        }

        private void Instance_KeyPress(string name)
        {
            Debug.WriteLine("KeyPress " + name);
            if (name == "ClientInfo")
            {
                // Toggle
                if (_clientInfoForm.Visible)
                {
                    _clientInfoForm.Hide();
#if !DEBUG
                    //if (!_screenSaver)
                        //TopMost = true;
#endif
                }
                else
                {
#if !DEBUG
                    if (!_screenSaver)
                        //TopMost = false;
#endif
                    _clientInfoForm.Show();
                    _clientInfoForm.BringToFront();
                }
            }
            else if (name == "ScreenSaver")
            {
                Debug.WriteLine("Closing due to ScreenSaver key press");
                if (!_screenSaver)
                    return;

                Close();
            }
        }


        /// <summary>
        /// Sets the CacheManager
        /// </summary>
        private void SetCacheManager()
        {
            try
            {
                using (FileStream fileStream = File.Open(ApplicationSettings.Default.LibraryPath + @"\" + ApplicationSettings.Default.CacheManagerFile, FileMode.Open))
                {
                    XmlSerializer xmlSerializer = new XmlSerializer(typeof(CacheManager));

                    _cacheManager = (CacheManager)xmlSerializer.Deserialize(fileStream);
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(new LogMessage("MainForm - SetCacheManager", "Unable to reuse the Cache Manager because: " + ex.Message));

                // Create a new cache manager
                _cacheManager = new CacheManager();
            }

            try
            {
                _cacheManager.Regenerate();
            }
            catch (Exception ex)
            {
                Trace.WriteLine(new LogMessage("MainForm - SetCacheManager", "Regenerate failed because: " + ex.Message));
            }
        }

        /// <summary>
        /// Handles the ScheduleChange event
        /// </summary>
        /// <param name="layoutPath"></param>
        void ScheduleChangeEvent(string layoutPath, int scheduleId, int layoutId)
        {
            Trace.WriteLine(new LogMessage("MainForm - ScheduleChangeEvent", string.Format("Schedule Changing to {0}", layoutPath)), LogType.Audit.ToString());

            // We are changing the layout
            _changingLayout = true;

            _scheduleId = scheduleId;
            _layoutId = layoutId;

            if (_stat != null)
            {
                // Log the end of the currently running layout.
                _stat.toDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                // Record this stat event in the statLog object
                _statLog.RecordStat(_stat);
            }

            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(new ChangeToNextLayoutDelegate(ChangeToNextLayout), layoutPath);
                return;
            }

            ChangeToNextLayout(layoutPath);
        }

        /// <summary>
        /// Change to the next layout
        /// </summary>
        private void ChangeToNextLayout(string layoutPath)
        {
            if (ApplicationSettings.Default.PreventSleep)
            {
                try
                {
                    SetThreadExecutionState(EXECUTION_STATE.ES_DISPLAY_REQUIRED | EXECUTION_STATE.ES_SYSTEM_REQUIRED | EXECUTION_STATE.ES_CONTINUOUS);
                }
                catch
                {
                    Trace.WriteLine(new LogMessage("MainForm - ChangeToNextLayout", "Unable to set Thread Execution state"), LogType.Info.ToString());
                }
            }

            try
            {
                // Destroy the Current Layout
                try
                {
                    DestroyLayout();
                }
                catch (Exception e)
                {
                    Trace.WriteLine(new LogMessage("MainForm - ChangeToNextLayout", "Destroy Layout Failed. Exception raised was: " + e.Message), LogType.Info.ToString());
                    throw e;
                }

                // Prepare the next layout
                try
                {
                    PrepareLayout(_layoutId.ToString());

                    _clientInfoForm.CurrentLayoutId = layoutPath;
                    //_schedule.CurrentLayoutId = _layoutId;
                }
                catch (DefaultLayoutException)
                {
                    throw;
                }
                catch (Exception e)
                {
                    DestroyLayout();
                    Trace.WriteLine(new LogMessage("MainForm - ChangeToNextLayout", "Prepare Layout Failed. Exception raised was: " + e.Message), LogType.Info.ToString());
                    throw;
                }

               // _clientInfoForm.ControlCount = Controls.Count;

                // Do we need to notify?
                try
                {
                    AppStartup.Notify(_layoutId.ToString());
                }
                catch (Exception e)
                {
                    Trace.WriteLine(new LogMessage("MainForm - ChangeToNextLayout", "Notify Status Failed. Exception raised was: " + e.Message), LogType.Info.ToString());
                    throw;
                }
            }
            catch (Exception ex)
            {
                if (!(ex is DefaultLayoutException))
                    Trace.WriteLine(new LogMessage("MainForm - ChangeToNextLayout", "Layout Change to " + layoutPath + " failed. Exception raised was: " + ex.Message), LogType.Error.ToString());
                Trace.WriteLine(new LogMessage("MainForm - ChangeToNextLayout", "Layout Change to " + layoutPath + " failed. Exception raised was: " + ex.Message), LogType.Error.ToString());
                //if (!_showingSplash)
                //    ShowSplashScreen();

                // In 10 seconds fire the next layout
                System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
                timer.Interval = 10000;
                timer.Tick += new EventHandler(splashScreenTimer_Tick);

                // Start the timer
                timer.Start();
            }

            // We have finished changing the layout
            _changingLayout = false;
        }

        private void splashScreenTimer_Tick(object sender, EventArgs e)
        {
            Debug.WriteLine(new LogMessage("timer_Tick", "Loading next layout after splashscreen"));

            System.Windows.Forms.Timer timer = (System.Windows.Forms.Timer)sender;
            timer.Stop();
            timer.Dispose();

            _schedule.NextLayout();
        }

        /// <summary>
        /// Prepare Layout and Add layout 
        /// </summary>
        /// <param name="layoutID"></param>
        private void PrepareLayout(string layoutID)
        {
            ShowDefaultSplashScreen();
            //set delecat
            finishLayout = new FinishLayoutCallback(CompleteLayout);

            //layout panel 
            layout = new LayoutControl(layoutID, finishLayout);
            this.LayoutRoot.Children.Add(layout);
            
        }

        /// <summary>
        /// when complete layout
        /// </summary>
        private void CompleteLayout()
        {
            Trace.WriteLine(new LogMessage("MainForm - DurationElapsedEvent", "Region Elapsed"), LogType.Audit.ToString());

            // Are we already changing the layout?
            if (_changingLayout)
            {
                Trace.WriteLine(new LogMessage("MainForm - DurationElapsedEvent", "Already Changing Layout"), LogType.Audit.ToString());
                return;
            }

            Trace.WriteLine(new LogMessage("MainForm - DurationElapsedEvent", "All Regions have expired. Raising a Next layout event."), LogType.Audit.ToString());

            // We are changing the layout
            _changingLayout = true;

            // Yield and restart
            _schedule.NextLayout();
        }

        private void ShowSplashScreen()
        {
            _showingSplash = true;

            if (!string.IsNullOrEmpty(ApplicationSettings.Default.SplashOverride))
            {
                try
                {
                    Uri path = new Uri("pack://application:,,,/Resources/splash.jpg");
                    Image img = new Image()
                    {
                        Name = "Img",
                    };
                    img.Source = new BitmapImage(path);
                    this.LayoutRoot.Children.Add(img);
                }
                catch
                {
                    Trace.WriteLine(new LogMessage("ShowSplashScreen", "Unable to load user splash screen"), LogType.Error.ToString());
                    ShowDefaultSplashScreen();
                }
            }
            else
            {
                ShowDefaultSplashScreen();
            }
        }

        private void ShowDefaultSplashScreen()
        {
            Uri path = new Uri("pack://application:,,,/Resources/splash.jpg");
            Image img = new Image()
            {
                Name = "Img",
            };
            img.Source = new BitmapImage(path);
            this.LayoutRoot.Children.Add(img);
        }

        private void DestroyLayout()
        {
            try
            {      
                this.Dispatcher.Invoke(() =>
                {
                    if (layout != null)
                    {
                        this.LayoutRoot.Children.Remove(layout);
                    }
                });
                
            }
            catch(Exception ex)
            {
                PlayerSettings.ErrorLog(ex);
            }
        }

        public void FlushStats()
        {
            try
            {
                _statLog.Flush();
            }
            catch
            {
                System.Diagnostics.Trace.WriteLine(new LogMessage("MainForm - FlushStats", "Unable to Flush Stats"), LogType.Error.ToString());
            }
        }

        void ScheduleOverlayChangeEvent(Collection<ScheduleItem> overlays)
        {
            if (Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(new ManageOverlaysDelegate(ManageOverlays), overlays);
                return;
            }

            ManageOverlays(overlays);
        }


        public void ManageOverlays(Collection<ScheduleItem> overlays)
        {
            try
            {
                var _overlays = PlayerSettings.OverlayList;
                Debug.WriteLine("Arrived at Manage Overlays with " + overlays.Count + " overlay schedules to show. We're already showing " + _overlays.Count + " overlay Regions", "Overlays");

                // Take the ones we currently have up and remove them if they aren't in the new list or if they've been set to refresh
                // We use a for loop so that we are able to remove the region from the collection
                for (int i = 0; i < _overlays.Count; i++)
                {
                    Debug.WriteLine("Assessing Overlay Region " + i, "Overlays");

                    string _ovelayName = "overlay" + _overlays[i].layoutId;

                    var region = PlayerSettings.OverlayList[i];
                    bool found = false;
                    bool refresh = false;

                    foreach (ScheduleItem item in overlays)
                    {
                        if (item.scheduleid == region.scheduleId)
                        {
                            found = true;
                            refresh = item.Refresh;
                            break;
                        }
                    }

                    if (!found || refresh)
                    {
                        if (refresh)
                        {
                            Trace.WriteLine(new LogMessage("MainForm - ManageOverlays", "Refreshing item that has changed."), LogType.Info.ToString());
                        }
                        Debug.WriteLine("Removing overlay " + i + " which is no-longer required. Overlay: " + region.scheduleId, "Overlays");

                        // Remove the Region from the overlays collection
                        _overlays.Remove(region);

                        // As we've removed the thing we're iterating over, reduce i
                        i--;

                        // Clear down and dispose of the region.
                        region = null;

                        this.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            var item = root.FindName(_ovelayName) as System.Windows.Controls.UserControl;
                            try
                            {
                                this.Overlay.Children.Remove(item);
                            }
                            catch (Exception ex)
                            {

                            }
                        }));
                        
                    }
                    else
                    {
                        Debug.WriteLine("Overlay Region found and not needing refresh " + i, "Overlays");
                    }
                }

                // Take the ones that are in the new list and add them
                foreach (ScheduleItem item in overlays)
                {
                    // Check its not already added.
                    bool found = false;
                    foreach (LayoutOption region in _overlays)
                    {
                        if (region.scheduleId == item.scheduleid)
                        {
                            found = true;
                            break;
                        }
                    }

                    if (found)
                    {
                        Debug.WriteLine("Region already found for overlay - we're assuming here that if we've found one, they are all there.", "Overlays");
                        continue;
                    }

                    // Reset refresh
                    item.Refresh = false;

                    var _ovelayOptions = new LayoutOption();
                    RenderOverlays.ReadOvelyas(item, _ovelayOptions);

                    PlayerSettings.OverlayList.Add(_ovelayOptions);

                    string _ovelayName = "overlay" + _ovelayOptions.layoutId.ToString();

                    this.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        /* run your code here */
                        var overlayouts = new OverlayControl(_ovelayOptions.layoutId.ToString(), _ovelayOptions);
                        overlayouts.Name = _ovelayName;
                        root.RegisterName(_ovelayName, overlayouts);

                        this.Overlay.Children.Add(overlayouts);

                    }));

                }

            }
            catch (Exception ex)
            {

            }
        }

    }
}
