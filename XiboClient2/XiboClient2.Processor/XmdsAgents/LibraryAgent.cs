using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XiboClient2.Processor.Log;
using XiboClient2.Processor.Logic;
using XiboClient2.Processor.Models;
using XiboClient2.Processor.Settings;

namespace XiboClient2.Processor.XmdsAgents
{
    class LibraryAgent
    {
        private object _locker = new object();
        private bool _forceStop = false;
        private ManualResetEvent _manualReset = new ManualResetEvent(false);

        private List<string> _persistentFiles = new List<string>();

        /// <summary>
        /// The Current CacheManager for this Xibo Client
        /// </summary>
        public CacheManager CurrentCacheManager
        {
            set
            {
                _cacheManager = value;
            }
        }
        private CacheManager _cacheManager;

        /// <summary>
        /// Required Files Object
        /// </summary>
        private RequiredFiles _requiredFiles;

        public LibraryAgent()
        {
            _persistentFiles.Add("cacheManager.xml");
            _persistentFiles.Add("requiredFiles.xml");
            _persistentFiles.Add("schedule.xml");
            _persistentFiles.Add("status.json");
            _persistentFiles.Add("hardwarekey");
            _persistentFiles.Add("config.xml");
            _persistentFiles.Add("id_rsa");
        }

        /// <summary>
        /// Stops the thread
        /// </summary>
        public void Stop()
        {
            _forceStop = true;
            _manualReset.Set();
        }

        /// <summary>
        /// Run Thread
        /// </summary>
        public void Run()
        {
            Trace.WriteLine(new LogMessage("LibraryAgent - Run", "Thread Started"), LogType.Info.ToString());

            while (!_forceStop)
            {
                lock (_locker)
                {
                    try
                    {
                        // If we are restarting, reset
                        _manualReset.Reset();

                        // Only do something if enabled
                        if (!ApplicationSettings.Default.EnableExpiredFileDeletion)
                        {
                            Trace.WriteLine(new LogMessage("LibraryAgent - Run", "Expired File Deletion Disabled"), LogType.Audit.ToString());
                            return;
                        }

                        // Test Date
                        DateTime testDate = DateTime.Now.AddDays(ApplicationSettings.Default.LibraryAgentInterval * -1);

                        // Get required files from disk
                        _requiredFiles = RequiredFiles.LoadFromDisk();

                        Trace.WriteLine(new LogMessage("LibraryAgent - Run", "Number of required files = " + _requiredFiles.RequiredFileList.Count), LogType.Audit.ToString());

                        // Build a list of files in the library
                        DirectoryInfo directory = new DirectoryInfo(ApplicationSettings.Default.LibraryPath);

                        // Check each one and see if it is in required files
                        foreach (FileInfo fileInfo in directory.GetFiles())
                        {
                            // Never delete certain system files
                            // Also do not delete log/stat files as they are managed by their respective agents
                            if (_persistentFiles.Contains(fileInfo.Name) ||
                                fileInfo.Name.Contains(ApplicationSettings.Default.LogLocation) ||
                                fileInfo.Name.Contains(ApplicationSettings.Default.StatsLogFile)
                                )
                                continue;

                            // Delete files that were accessed over N days ago
                            try
                            {
                                RequiredFile file = _requiredFiles.GetRequiredFile(fileInfo.Name);
                            }
                            catch
                            {
                                // It is a bad idea to log in here - it can cause a build up of log files.
                                //Debug.WriteLine(new LogMessage("LibraryAgent - Run", fileInfo.Name + " is not in Required Files, testing last accessed date [" + fileInfo.LastAccessTime + "] is earlier than " + testDate), LogType.Audit.ToString());

                                // Not a required file
                                if (fileInfo.LastAccessTime < testDate)
                                {
                                    Trace.WriteLine(new LogMessage("LibraryAgent - Run", "Deleting old file: " + fileInfo.Name), LogType.Info.ToString());
                                    File.Delete(fileInfo.FullName);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        // Log this message, but dont abort the thread
                        Trace.WriteLine(new LogMessage("LibraryAgent - Run", "Exception in Run: " + ex.Message), LogType.Error.ToString());
                    }
                }

                // Sleep this thread for 15 minutes
                _manualReset.WaitOne(2700 * 1000);
            }

            Trace.WriteLine(new LogMessage("LibraryAgent - Run", "Thread Stopped"), LogType.Info.ToString());
        }
    }
}
