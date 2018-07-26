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

namespace XiboClient2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private AppStartup _appStart = new AppStartup();
        private string scheduleName = "schedule.xml";

        private bool isHiding = false;


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
            //this.Closing += MainWindow_Closing;
            this.ContentRendered += MainWindow_ContentRendered;            
        }

        /// <summary>
        /// Window shown
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainWindow_ContentRendered(object sender, EventArgs e)
        {

            if (isHiding)
            {
                return;
            }

            //run form show
            _appStart.FormShown();

            //string _filePath = "pack://application:,,,/XiboClientWPF;component/Resources/splash.jpg";
            //Uri uriImage = new Uri(_filePath);
            //Image img = new Image()
            //{
            //    Name = "Img",
            //};
            //img.Source = new BitmapImage(uriImage);
            //this.LayoutRoot.Children.Add(img);

            //string path = PlayerSettings.libraryPath;
            try
            {
                PlayerSettings.scheduleName = scheduleName;
                RenderSchedule.ReadSchedule(scheduleName);
                ShowSplashScreen();
                this.Hide();
                isHiding = true;
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
            //_appStart.FormClosing();
        }

        /// <summary>
        /// Load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //Run From Load

            if (isHiding)
            {
                return;
            }

            _appStart.FormLoad();
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
                _appStart.Instance_KeyPress("ClientInfo");
            }
        }



    }
}
