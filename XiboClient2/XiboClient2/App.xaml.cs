using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace XiboClient2
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
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
            catch
            { }
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
            Forms.Options windowMain = new Forms.Options();
            windowMain.ShowDialog();
        }
    }
}
