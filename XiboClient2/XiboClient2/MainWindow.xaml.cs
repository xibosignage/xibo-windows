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

namespace XiboClient2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private delegate void ChangeToNextLayoutDelegate(string layoutPath);
        private delegate void ManageOverlaysDelegate(Collection<ScheduleItem> overlays);

        private AppStartup _appStart = new AppStartup();

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
            //InitializeScreenSaver(true);
            InitializeXibo();
        }

        public MainWindow()
        {
            InitializeComponent();
            InitializeXibo();
        }

        private void InitializeXibo()
        {
            this.Name = ApplicationSettings.AppProductName;

            _appStart.Initialize();

            this.Loaded += MainWindow_Loaded;
            this.Closing += MainWindow_Closing;
            this.ContentRendered += MainWindow_ContentRendered;

            
        }

        private void MainWindow_ContentRendered(object sender, EventArgs e)
        {
            _appStart.FormShown();
        }

        private void InitializeScreenSaver(bool preview)
        {
            //_screenSaver = true;

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

        /// <summary>
        /// Handle the Key Event
        /// </summary>
        /// <param name="name"></param>
//        void Instance_KeyPress(string name)
//        {
//            Debug.WriteLine("KeyPress " + name);
//            if (name == "ClientInfo")
//            {
//                // Toggle
//                if (_clientInfoForm.Visible)
//                {
//                    _clientInfoForm.Hide();
//#if !DEBUG
//                    if (!_screenSaver)
//                        TopMost = true;
//#endif
//                }
//                else
//                {
//#if !DEBUG
//                    if (!_screenSaver)
//                        TopMost = false;
//#endif
//                    _clientInfoForm.Show();
//                    _clientInfoForm.BringToFront();
//                }
//            }
//            else if (name == "ScreenSaver")
//            {
//                Debug.WriteLine("Closing due to ScreenSaver key press");
//                if (!_screenSaver)
//                    return;

//                Close();
//            }
//        }

        /// <summary>
        /// Called as the Main Form starts to close
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

            _appStart.FormClossing();
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            _appStart.FormLoad();
        }
        
        
    }
}
