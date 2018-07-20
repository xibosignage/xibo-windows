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
using XiboClient2.Log;
using System.Runtime.InteropServices;
using XiboClient2.Processor.Log;
using XiboClient2.Processor.XmdsAgents;
using XiboClient2.Processor.Settings;

namespace XiboClient2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            InitializeXibo();
        }

        private void InitializeXibo()
        {
            this.Name = ApplicationSettings.AppProductName;

            this.Loaded += MainWindow_Loaded;
            this.Closing += MainWindow_Closing;
            

            Trace.WriteLine(new LogMessage("MainForm", "Client Initialised"), LogType.Info.ToString());
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Environment.Exit(0);
            //throw new NotImplementedException();
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            FileAgent ag = new FileAgent();
            ag.Run();
        }

    }
}
