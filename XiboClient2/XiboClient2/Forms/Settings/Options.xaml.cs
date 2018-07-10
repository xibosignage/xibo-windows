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

namespace XiboClient2.Forms
{
    /// <summary>
    /// Interaction logic for Options.xaml
    /// </summary>
    public partial class Options : Window
    {
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
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            save();
        }

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
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }
    }
}
