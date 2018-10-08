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
using XiboClient2.Settings;

namespace XiboClient2.Media
{
    /// <summary>
    /// Interaction logic for OverlayControl.xaml
    /// </summary>
    public partial class OverlayControl : UserControl
    {

        private string _layoutPath;
        LayoutOption _ovelays = new LayoutOption();

        public OverlayControl()
        {
            InitializeComponent();
        }

        public OverlayControl(string LayoutPath, LayoutOption _ovelay)
        {
            InitializeComponent();

            this._layoutPath = LayoutPath;
            _ovelays = _ovelay;

            Loaded += OverlayControl_Loaded;
            Unloaded += OverlayControl_Unloaded;
        }

        private void OverlayControl_Unloaded(object sender, RoutedEventArgs e)
        {
            try
            {
                this.OvelaysRoot.Children.Clear();
            }
            catch (Exception ex)
            {
                PlayerSettings.ErrorLog(ex);
            }
        }

        private void OverlayControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                //Reoder
                _ovelays.RegionList = _ovelays.RegionList.OrderBy(x => x.zIndex).ToList();

                //Stat view
                ViewLayout();

                this.Cursor = Cursors.None;
            }
            catch (Exception ex)
            {
                PlayerSettings.ErrorLog(ex);
            }
        }

        private void ViewLayout()
        {
            try
            {
                for (int listIndex = 0; listIndex < _ovelays.RegionList.Count; listIndex++)
                {
                    SetRegionPanel(listIndex);
                }
            }
            catch (Exception ex)
            {
                PlayerSettings.ErrorLog(ex);
            }
        }

        private void SetRegionPanel(int index)
        {
            try
            {
                string _regionID = _ovelays.RegionList[index].regionId;

                int _widht = _ovelays.RegionList[index].width;
                int _height = _ovelays.RegionList[index].height;
                int _Top = _ovelays.RegionList[index].top;
                int _left = _ovelays.RegionList[index].left;

                PanelControl panelCon = new PanelControl(_regionID, _ovelays, null)
                {
                    Width = _widht,
                    Height = _height,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Top,
                    Margin = new Thickness(_left, _Top, 0, 0)
                };

                this.OvelaysRoot.Children.Add(panelCon);
            }
            catch (Exception ex)
            {
                PlayerSettings.ErrorLog(ex);
            }
        }
    }
}
