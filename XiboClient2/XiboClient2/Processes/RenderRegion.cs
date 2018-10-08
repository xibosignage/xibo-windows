using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;
using XiboClient2.Settings;

namespace XiboClient2.Processes
{
    public class RenderRegion
    {

        //RenderMedia _media = new RenderMedia();
        private static System.Windows.Size _clientSize;

        public RenderRegion()
        {

        }

        /// <summary>
        /// Get region Details
        /// </summary>
        /// <param name="region"></param>
        public static void RenderRegionDetails(XmlNode region, LayoutOption _layoutOption)
        {
            RegionOptions regionOption = new RegionOptions();
            MediaOption options = new MediaOption();

            // Override the default size if necessary
            _clientSize.Height = SystemParameters.PrimaryScreenHeight;
            _clientSize.Width = SystemParameters.PrimaryScreenWidth;

            // Set the background and size of the form
            double layoutWidth = int.Parse(_layoutOption.layoutWidth.ToString(), CultureInfo.InvariantCulture);
            double layoutHeight = int.Parse(_layoutOption.layoutHeight.ToString(), CultureInfo.InvariantCulture);

            // Scaling factor, will be applied to all regions
            double scaleFactor = Math.Min(_clientSize.Width / layoutWidth, _clientSize.Height / layoutHeight);

            // Want to be able to center this shiv - therefore work out which one of these is going to have left overs
            int backgroundWidth = (int)(layoutWidth * scaleFactor);
            int backgroundHeight = (int)(layoutHeight * scaleFactor);

            double leftOverX;
            double leftOverY;

            try
            {
                leftOverX = Math.Abs(_clientSize.Width - backgroundWidth);
                leftOverY = Math.Abs(_clientSize.Height - backgroundHeight);

                if (leftOverX != 0) leftOverX = leftOverX / 2;
                if (leftOverY != 0) leftOverY = leftOverY / 2;
            }
            catch
            {
                leftOverX = 0;
                leftOverY = 0;
            }

            //region attributes

            XmlAttributeCollection regionAttributes = region.Attributes;
            if (regionAttributes.Count > 0)
            {
                //options.scheduleId = _scheduleId;
                options.layoutId = _layoutOption.layoutId.ToString();
                options.regionId = regionAttributes["id"].Value.ToString();
                options.width = (int)(Convert.ToDouble(regionAttributes["width"].Value, CultureInfo.InvariantCulture) * scaleFactor);
                options.height = (int)(Convert.ToDouble(regionAttributes["height"].Value, CultureInfo.InvariantCulture) * scaleFactor);
                options.left = (int)(Convert.ToDouble(regionAttributes["left"].Value, CultureInfo.InvariantCulture) * scaleFactor);
                options.top = (int)(Convert.ToDouble(regionAttributes["top"].Value, CultureInfo.InvariantCulture) * scaleFactor);
                regionOption.scaleFactor = scaleFactor;

                if (regionAttributes["zindex"] != null)
                {
                    regionOption.zIndex = Convert.ToInt16(regionAttributes["zindex"].Value);
                }
                else
                {
                    regionOption.zIndex = 0;
                }
                // Store the original width and original height for scaling
                options.originalWidth = (int)Convert.ToDouble(regionAttributes["width"].Value, CultureInfo.InvariantCulture);
                options.originalHeight = (int)Convert.ToDouble(regionAttributes["height"].Value, CultureInfo.InvariantCulture);

                // Set the backgrounds (used for Web content offsets)
                options.backgroundLeft = options.left * -1;
                options.backgroundTop = options.top * -1;

                // Account for scaling
                options.left = options.left + (int)leftOverX;
                options.top = options.top + (int)leftOverY;


            }

            XmlNode optionNode = region.SelectSingleNode("options");
            foreach (XmlNode optionN in optionNode.ChildNodes)
            {
                if (optionN.Name == "transitionType")
                {
                    regionOption.transitionType = optionN.InnerText;
                }
                else if (optionN.Name == "transitionDuration")
                {
                    regionOption.transitionDuration = double.Parse(optionN.InnerText);
                }
                else if (optionN.Name == "transitionDirection")
                {
                    regionOption.transitionDirection = optionN.InnerText;
                }
            }

            _layoutOption.RegionList.Add(
                new RegionOptions(
                    options.layoutId,
                    options.regionId,
                    options.width,
                    options.height,
                    options.top,
                    options.left,
                    regionOption.transitionType,
                    regionOption.transitionDuration,
                    regionOption.transitionDirection,
                    regionOption.zIndex
                    ));

            if (region.ChildNodes.Count != 0)
            {
                XmlNodeList listMedia = region.SelectNodes("media");
                if (listMedia.Count > 0)
                {
                    RenderMedia.MediaList(listMedia, options, _layoutOption);
                }
                else
                {
                    return;
                }
            }
        }

        
    }
}
