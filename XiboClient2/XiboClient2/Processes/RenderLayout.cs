using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using XiboClient2.Settings;

namespace XiboClient2.Processes
{
    public class RenderLayout
    {
        
        public RenderLayout()
        {

        }

        /// <summary>
        /// Prepare Layout(Get all Details in Layout)
        /// </summary>
        /// <param name="layoutId"></param>
        public static void PrepareLayout(string layoutId)
        {
            try
            {
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


                LayoutOption.layoutId = int.Parse(layoutId);

                // Attributes of the main layout node
                XmlNode layoutNode = layoutXml.SelectSingleNode("/layout");

                XmlAttributeCollection layoutAttributes = layoutNode.Attributes;

                LayoutOption.layoutWidth = int.Parse(layoutAttributes["width"].Value);
                LayoutOption.layoutHeight = int.Parse(layoutAttributes["height"].Value);

                //Layout Background Color
                if (layoutAttributes["bgcolor"] != null && layoutAttributes["bgcolor"].Value != "")
                {
                    LayoutOption.backgroundColor = layoutAttributes["bgcolor"].Value;
                }
                else
                {
                    LayoutOption.backgroundColor = "#000000";
                }

                //Layout Background Color Image
                if (layoutAttributes["background"] != null && layoutAttributes["background"].Value != "")
                {
                    LayoutOption.backgroundImage = PlayerSettings.libraryPath + @"\" + layoutAttributes["background"].Value;
                }
                else
                {
                    LayoutOption.backgroundImage = "";
                }


                // Get the regions
                XmlNodeList listRegions = layoutXml.SelectNodes("/layout/region");

                //get region details
                foreach (XmlNode region in listRegions)
                {
                    try
                    {
                        RenderRegion.RenderRegionDetails(region);
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
