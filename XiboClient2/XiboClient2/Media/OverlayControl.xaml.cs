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

namespace XiboClient2.Media
{
    /// <summary>
    /// Interaction logic for OverlayControl.xaml
    /// </summary>
    public partial class OverlayControl : UserControl
    {

        private string _layoutPath;
        private int _width;
        private int _height;
        private int _top;
        private int _left;

        public OverlayControl()
        {
            InitializeComponent();
        }

        public OverlayControl(string LayoutPath)
        {
            InitializeComponent();

            this._layoutPath = LayoutPath;

            Loaded += OverlayControl_Loaded;
            Unloaded += OverlayControl_Unloaded;
        }

        private void OverlayControl_Unloaded(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void OverlayControl_Loaded(object sender, RoutedEventArgs e)
        {
            //throw new NotImplementedException();
            this.Width = 1000;
            this.Height = 10000;
            this.HorizontalAlignment = HorizontalAlignment.Center;
        }
    }
}
