using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using XiboClient2.Processor.Forms;
using XiboClient2.Processor.Log;

namespace XiboClient2
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        [STAThread]
        protected override void OnStartup(StartupEventArgs e)
        {

            NativeMethods.SetErrorMode(NativeMethods.SetErrorMode(0) |
                           ErrorModes.SEM_NOGPFAULTERRORBOX |
                           ErrorModes.SEM_FAILCRITICALERRORS |
                           ErrorModes.SEM_NOOPENFILEERRORBOX);

            // Ensure our process has the highest priority
            Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.RealTime;

            //Application.SetCompatibleTextRenderingDefault(false);

#if !DEBUG
            // Catch unhandled exceptions
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
#endif
            // Add the Xibo Tracelistener
            
            Trace.Listeners.Add(new XiboTraceListener());

            //e.Args is the string[] of command line argruments

            try
            {
                // Check for any passed arguments
                if (e.Args.Length > 0)
                {
                    if (e.Args[0].ToString() == "o")
                    {
                        RunSettings();
                    }
                    else
                    {
                        RunClient();
                    }
                }
                else
                {
                    RunClient();
                }
            }
            catch(Exception ex)
            {
                HandleUnhandledException(ex);
            }
        }

        private static void RunClient()
        {
            /// Trace.WriteLine(new LogMessage("Main", "Client Started"), LogType.Info.ToString());
            MainWindow windowMain = new MainWindow();
            windowMain.ShowDialog();
        }

        private static void RunSettings()
        {
            // If we are showing the options form, enable visual styles
            OptionForm windowMain = new OptionForm();
            windowMain.ShowDialog();
        }


        /// <summary>
        /// Event for unhandled exceptions
        /// </summary>
        /// <param name="o"></param>
        static void HandleUnhandledException(Object o)
        {
            Exception e = o as Exception;

            // What happens if we cannot start?
            Trace.WriteLine(new LogMessage("Main", "Unhandled Exception: " + e.Message), LogType.Error.ToString());
            Trace.WriteLine(new LogMessage("Main", "Stack Trace: " + e.StackTrace), LogType.Audit.ToString());

            try
            {
                // Also write to the event log
                //Application.ProductName remove and hardcode Xibo name (DuranIT - 2018-07-23
                try
                {
                    if (!EventLog.SourceExists("Xibo"))
                        EventLog.CreateEventSource("Xibo", "Xibo");
                    EventLog.WriteEntry("Xibo", e.ToString(), EventLogEntryType.Error);
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(new LogMessage("Main", "Couldn't write to event log: " + ex.Message), LogType.Info.ToString());
                }

                Trace.Flush();
            }
            catch (Exception ex)
            {
                Trace.WriteLine(new LogMessage("Main", "Unable to write to event log " + ex.Message), LogType.Info.ToString());
            }

            // Exit the application and allow it to be restarted by the Watchdog.
            Environment.Exit(0);
        }

        [DllImport("User32.dll")]
        public static extern int ShowWindowAsync(IntPtr hWnd, int swCommand);

        internal static class NativeMethods
        {
            [DllImport("kernel32.dll")]
            internal static extern ErrorModes SetErrorMode(ErrorModes mode);
        }

        [Flags]
        internal enum ErrorModes : uint
        {
            SYSTEM_DEFAULT = 0x0,
            SEM_FAILCRITICALERRORS = 0x0001,
            SEM_NOALIGNMENTFAULTEXCEPT = 0x0004,
            SEM_NOGPFAULTERRORBOX = 0x0002,
            SEM_NOOPENFILEERRORBOX = 0x8000
        }
    }
}
