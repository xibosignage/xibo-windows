using Microsoft.WindowsAPICodePack.Shell;
using Microsoft.WindowsAPICodePack.Shell.PropertySystem;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XiboClient2.Processor.Settings;

namespace XiboClient2.Settings
{
    public class PlayerSettings
    {
        //public static string defalutLocation = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        //public static string libraryPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Xibo Library\";
        public static string libraryPath = ApplicationSettings.Default.LibraryPath + @"\";
        public static string scheduleName = "";
        public static int firstLoadCheck = 0;
        public static bool EnableShellCommands = true;

        public static List<RegionOptions> RegionList = new List<RegionOptions>();
        public static List<MediaOption> MediaNodeList = new List<MediaOption>();
        public static List<AudioOption> AudioNodeList = new List<AudioOption>();

        /// <summary>
        /// Get Video Audio duraion
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static TimeSpan GetVideoDuration(string filePath)
        {
            using (var shell = ShellObject.FromParsingName(filePath))
            {
                IShellProperty prop = shell.Properties.System.Media.Duration;
                var t = (ulong)prop.ValueAsObject;
                return TimeSpan.FromTicks((long)t);
            }
        }

        /// <summary>
        /// Error log
        /// </summary>
        /// <param name="ex"></param>
        public static void ErrorLog(Exception ex)
        {
            string filePath = libraryPath + "Error.txt";

            using (StreamWriter writer = new StreamWriter(filePath, true))
            {
                writer.WriteLine("Message :" + ex.Message + "<br/>" + Environment.NewLine + "StackTrace :" + ex.StackTrace +
                   "" + Environment.NewLine + "Date :" + DateTime.Now.ToString());
                writer.WriteLine(Environment.NewLine + "-----------------------------------------------------------------------------" + Environment.NewLine);
            }
        }
    }
}
