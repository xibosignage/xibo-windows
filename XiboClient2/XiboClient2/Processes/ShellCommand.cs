using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XiboClient2.Settings;

namespace XiboClient2.Processes
{
    public class ShellCommand
    {
        private static string _command = "";
        private static string _code = "";
        private static bool _launchThroughCmd = true;
        private static bool _terminateCommand = false;
        private static bool _useTaskKill = false;
        private static int _processId;

        public static void ShellCommandDetails(string launchThroughCmd, string terminateCommand, string useTaskkill, string windowsCommand)
        {
            _command = Uri.UnescapeDataString(windowsCommand).Replace('+', ' ');

            // Default to launching through CMS for backwards compatiblity
            _launchThroughCmd = (launchThroughCmd == "1");

            // Termination
            _terminateCommand = (terminateCommand == "1");
            _useTaskKill = (useTaskkill == "1");

            if (!string.IsNullOrEmpty(_code))
            {

            }
            else
            {
                if (PlayerSettings.EnableShellCommands)
                {
                    if (!string.IsNullOrEmpty(_command))
                    {
                        // Array of allowed commands
                        string[] allowedCommands = _command.Split(',');

                        // Check we are allowed to execute the command
                        bool found = false;

                        foreach (string allowedCommand in allowedCommands)
                        {
                            if (_command.StartsWith(allowedCommand))
                            {
                                found = true;
                                ExecuteShellCommand();
                                break;
                            }
                        }
                    }
                }
            }
        }

        private static void ExecuteShellCommand()
        {
            // Execute the commend
            if (!string.IsNullOrEmpty(_command))
            {
                using (Process process = new Process())
                {
                    ProcessStartInfo startInfo = new ProcessStartInfo();

                    if (_launchThroughCmd)
                    {
                        startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                        startInfo.FileName = "cmd.exe";
                        startInfo.Arguments = "/C " + _command;
                    }
                    else
                    {
                        // Split the command into a command string and arguments.
                        string[] splitCommand = _command.Split(new[] { ' ' }, 2);
                        startInfo.FileName = splitCommand[0];

                        if (splitCommand.Length > 1)
                            startInfo.Arguments = splitCommand[1];
                    }

                    process.StartInfo = startInfo;
                    process.Start();

                    // Grab the ID
                    _processId = process.Id;
                }
            }
        }
    }
}
