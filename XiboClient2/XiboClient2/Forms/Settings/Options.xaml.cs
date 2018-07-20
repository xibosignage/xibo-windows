using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
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
using System.Web;
using XiboClient2.Processor.xmds;

namespace XiboClient2.Forms
{
    /// <summary>
    /// Interaction logic for Options.xaml
    /// </summary>
    public partial class Options : Window
    {

        //XiboClient2.xmds objWebservice = new XiboClient2.xmds.WebService();

        public string cmsAddress { get; set; }
        public string key { get; set; }
        public string localLibrary { get; set; }
        public string userName { get; set; }
        public string password { get; set; }
        public string domain { get; set; }
        public string displayId { get; set; }
        public string splashScreen { get; set; }

        public Options()
        {
            InitializeComponent();


            Debug.WriteLine("Loaded Options Form", "OptionForm");
        }

        private void xmds_RegisterDisplayCompleted(object sender, RegisterDisplayCompletedEventArgs e)
        {
            
        }

        private void btnSave_Click(object sender, RoutedEventArgs e) => save();

        private void save()
        {
            cmsAddress = txtCmsAddress.Text ?? "";
            key = txtKey.Text ?? "";
            localLibrary = txtLocalLibrary.Text ?? "";
            userName = txtUserName.Text ?? "";
            password = txtPassword.Text ?? "";
            domain = txtDomain.Text ?? "";
            displayId = txtDisplayId.Text ?? "";
            splashScreen = txtSplashScreen.Text ?? "";


            tbStatus.Text = "";
            try
            {
                

                //Call register
                //xmds1.RegisterDisplayAsync(
                //    ApplicationSettings.Default.ServerKey,
                //    ApplicationSettings.Default.HardwareKey,
                //    ApplicationSettings.Default.DisplayName,
                //    "windows",
                //    ApplicationSettings.Default.ClientVersion,
                //    ApplicationSettings.Default.ClientCodeVersion,
                //    Environment.OSVersion.ToString(),
                //    _hardwareKey.MacAddress,
                //    _hardwareKey.Channel,
                //    _hardwareKey.getXmrPublicKey());
            }
            catch (Exception ex)
            {
                tbStatus.AppendText(ex.Message);
            }
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void btnSplashBrowse_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                txtSplashScreen.Text = openFileDialog.FileName;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                txtLocalLibrary.Text = openFileDialog.FileName;
            }
        }
    }
}
