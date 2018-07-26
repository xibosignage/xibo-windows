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
    public class RenderSchedule
    {
        public static List<string> ListLayouts = new List<string>();

        /// <summary>
        /// Read Scheduler and find valid layouts
        /// </summary>
        public static void ReadSchedule(string scheduleName)
        {
            ListLayouts.Clear();
            RenderLayout.MediaNodeList.Clear();
            try
            {
                string _layoutPath = PlayerSettings.libraryPath + scheduleName;
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

                XmlNode scheduleNode = layoutXml.SelectSingleNode("/schedule");

                XmlAttributeCollection scheduleAttributes = scheduleNode.Attributes;

                DateTime genarateDateTime = Convert.ToDateTime(scheduleAttributes["generated"].Value);

                XmlNodeList listLayouts = layoutXml.SelectNodes("/schedule/layout");

                foreach (XmlNode layout in listLayouts)
                {
                    try
                    {
                        XmlAttributeCollection layoutAttributes = layout.Attributes;
                        string id = layoutAttributes["file"].Value;
                        DateTime fromDate = Convert.ToDateTime(layoutAttributes["fromdt"].Value);
                        DateTime toDate = Convert.ToDateTime(layoutAttributes["todt"].Value);
                        XmlNodeList listFiles = layout.SelectNodes("dependents/file");

                        //Check layout in the data range and all dependents in the libary folder
                        if ((fromDate <= DateTime.Now) && (DateTime.Now <= toDate) && CheckDependents(listFiles))
                        {
                            ListLayouts.Add(id);
                        }
                    }
                    catch (Exception ex)
                    {
                        PlayerSettings.ErrorLog(ex);
                    }

                }


            }
            catch (Exception ex)
            {
                PlayerSettings.ErrorLog(ex);
            }
        }

        /// <summary>
        /// Check layout depandents
        /// </summary>
        /// <param name="listFiles"></param>
        /// <returns></returns>
        private static bool CheckDependents(XmlNodeList listFiles)
        {

            foreach (XmlNode file in listFiles)
            {
                string filePath = PlayerSettings.libraryPath + file.InnerText;
                if (!File.Exists(filePath))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
