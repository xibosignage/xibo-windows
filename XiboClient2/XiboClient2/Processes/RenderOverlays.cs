using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;
using XiboClient2.Processor.Logic;
using XiboClient2.Settings;

namespace XiboClient2.Processes
{
    public class RenderOverlays
    {
        private static System.Windows.Size _clientSize;

        
        public static void ReadOvelyas(ScheduleItem overlays, LayoutOption _overlayOption)
        {
            try
            {
                string overlayPath = overlays.layoutFile;
                string layoutId = overlays.id.ToString();
                int _scheduleId = overlays.scheduleid;
                


                //var _overlayOption = new OverlaysOptions();
                string _layoutPath = PlayerSettings.libraryPath + layoutId + ".xlf";
                //string _layoutPath = layoutId;
                XmlDocument layoutXml = new XmlDocument();

                if (!string.IsNullOrEmpty(_layoutPath))
                {
                    using (FileStream fs = File.Open(_layoutPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        using (XmlReader reader = XmlReader.Create(fs))
                        {
                            layoutXml.Load(reader);
                            reader.Close();
                        }
                        fs.Close();
                    }
                }

                _overlayOption.scheduleId = _scheduleId;
                _overlayOption.layoutId = int.Parse(layoutId);

                // Attributes of the main layout node
                XmlNode layoutNode = layoutXml.SelectSingleNode("/layout");

                XmlAttributeCollection layoutAttributes = layoutNode.Attributes;

                _overlayOption.layoutWidth = int.Parse(layoutAttributes["width"].Value);
                _overlayOption.layoutHeight = int.Parse(layoutAttributes["height"].Value);

                //Layout Background Color
                if (layoutAttributes["bgcolor"] != null && layoutAttributes["bgcolor"].Value != "")
                {
                    _overlayOption.backgroundColor = layoutAttributes["bgcolor"].Value;
                }
                else
                {
                    _overlayOption.backgroundColor = "#000000";
                }

                //Layout Background Color Image
                if (layoutAttributes["background"] != null && layoutAttributes["background"].Value != "")
                {
                    _overlayOption.backgroundImage = PlayerSettings.libraryPath + @"\" + layoutAttributes["background"].Value;
                }
                else
                {
                    _overlayOption.backgroundImage = "";
                }


                // Get the regions
                XmlNodeList listRegions = layoutXml.SelectNodes("/layout/region");

                //get region details
                foreach (XmlNode region in listRegions)
                {
                    try
                    {
                        //RenderOverlayRegionDetails(region, _overlayOption);
                        RenderRegion.RenderRegionDetails(region, _overlayOption);

                        //PlayerSettings.OverlayList.Add(_overlayOption);
                    }
                    catch (Exception ex)
                    {
                        PlayerSettings.ErrorLog(ex);
                    }

                }

            }
            catch (Exception e)
            {
                PlayerSettings.ErrorLog(e);
            }
        }        
        
    }
}
