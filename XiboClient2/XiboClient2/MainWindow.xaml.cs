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

namespace XiboClient2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private string scheduleName = "schedule.xml";
        private bool _screenSaver = false;

        

        public MainWindow(IntPtr previewHandle)
        {
            InitializeComponent();

            IntPtr hwnd = this.Handle;

            AppStartup.ScreenSaver(previewHandle, hwnd);

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
            PlayerSettings._appStart.Initialize();

            this.Loaded += MainWindow_Loaded;
            //this.Closing += MainWindow_Closing;
            this.ContentRendered += MainWindow_ContentRendered;            
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

        private void Instance_MouseEvent()
        {
            Close();
        }

        /// <summary>
        /// Window shown
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainWindow_ContentRendered(object sender, EventArgs e)
        {
            //run form show
            PlayerSettings._appStart.FormShown();
            
            Uri path = new Uri("pack://application:,,,/Resources/splash.jpg");
            Image img = new Image()
            {
                Name = "Img",
            };
            img.Source = new BitmapImage(path);
            this.LayoutRoot.Children.Add(img);

            //string path = PlayerSettings.libraryPath;
            try
            {
                PlayerSettings.scheduleName = scheduleName;
                RenderSchedule.ReadSchedule(scheduleName);
                ShowSplashScreen();
                this.Close();
            }
            catch (Exception ex)
            {
                PlayerSettings.ErrorLog(ex);
            }
            
        }

        /// <summary>
        /// Load Screen
        /// </summary>
        public void ShowSplashScreen()
        {
            try
            {
                LayoutWindow layoutWindow = new LayoutWindow();
                layoutWindow.Show();                
            }
            catch (Exception ex)
            {
                PlayerSettings.ErrorLog(ex);
                //ShowDefaultSplashScreen();
            }

        }

        /// <summary>
        /// Called as the Main Form starts to close
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //run form closing
            PlayerSettings._appStart.FormClosing();
        }

        /// <summary>
        /// Load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //Run From Load
            PlayerSettings._appStart.FormLoad();
            this.KeyUp += MainWindow_KeyUp;
        }

        /// <summary>
        /// press key
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainWindow_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            string Key = e.Key.ToString();

            if(Key.ToUpper() == "I")
            {
                PlayerSettings._appStart.Instance_KeyPress("ClientInfo");
            }
        }

        private void ChangeIcon()
        {
            //this.Icon = new bitma
        }

    }
}
