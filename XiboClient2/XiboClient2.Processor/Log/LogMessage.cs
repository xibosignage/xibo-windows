using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace XiboClient2.Processor.Log
{
    public class LogMessage
    {
        public string _thread;
        public string _method;
        public string _message;
        public int _scheduleId;
        public int _layoutId;
        public int _mediaId;
        public DateTime LogDate;

        public LogMessage(String method, String message)
        {
            LogDate = DateTime.Now;
            _method = method;
            _message = message;
            _thread = Thread.CurrentThread.Name;
        }

        public LogMessage(String method, String message, int scheduleId, int layoutId)
        {
            LogDate = DateTime.Now;
            _method = method;
            _message = message;
            _scheduleId = scheduleId;
            _layoutId = layoutId;
            _thread = Thread.CurrentThread.Name;
        }

        public LogMessage(String method, String message, int scheduleId, int layoutId, int mediaId)
        {
            LogDate = DateTime.Now;
            _method = method;
            _message = message;
            _scheduleId = scheduleId;
            _layoutId = layoutId;
            _mediaId = mediaId;
            _thread = Thread.CurrentThread.Name;
        }

        /// <summary>
        /// Load the log message via XML
        /// </summary>
        /// <param name="xmlMessage"></param>
        public LogMessage(string xmlMessage)
        {
            XmlDocument xml = new XmlDocument();
            xml.LoadXml("<xml>" + xmlMessage + "</xml>");

            try
            {
                LogDate = DateTime.Parse(xml.GetElementsByTagName("logdate").Item(0).InnerText.ToString());
                _message = xml.GetElementsByTagName("message").Item(0).InnerText.ToString();
                _method = xml.GetElementsByTagName("method").Item(0).InnerText.ToString();
                _thread = xml.GetElementsByTagName("thread").Item(0).InnerText.ToString();
            }
            catch (NullReferenceException)
            {
                LogDate = DateTime.Now;
                _message = xmlMessage;
                _method = "Unknown";
                _thread = Thread.CurrentThread.Name;
            }
        }

        public override string ToString()
        {
            // Format the message into the expected XML sub nodes.
            // Just do this with a string builder rather than an XML builder.
            String theMessage;

            theMessage = String.Format("<logdate>{0}</logdate>", LogDate.ToString("yyyy-MM-dd HH:mm:ss"));
            theMessage += String.Format("<thread>{0}</thread>", _thread);
            theMessage += String.Format("<method>{0}</method>", _method);
            theMessage += String.Format("<message>{0}</message>", SecurityElement.Escape(_message));

            if (_scheduleId != 0) theMessage += String.Format("<scheduleid>{0}</scheduleid>", _scheduleId.ToString());
            if (_layoutId != 0) theMessage += String.Format("<layoutid>{0}</layoutid>", _scheduleId.ToString());
            if (_mediaId != 0) theMessage += String.Format("<mediaid>{0}</mediaid>", _scheduleId.ToString());

            return theMessage;
        }
    }
    public enum LogType { Info, Audit, Error }
}
