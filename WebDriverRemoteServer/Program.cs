/* Copyright notice and license
Copyright 2007-2010 WebDriver committers
Copyright 2007-2010 Google Inc.
Portions copyright 2007 ThoughtWorks, Inc

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

     http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Principal;
using System.Text;
using OpenQA.Selenium.Remote.Server.Internal;
using OpenQA.Selenium.Remote.Server.Loggers;

namespace OpenQA.Selenium.Remote.Server
{
    internal class Program
    {
        private static RemoteServer httpServer;
        private static ConsoleLogger logger = new ConsoleLogger(LogLevel.Info);
        private static string userName = string.Empty;
        private static string password = string.Empty;
        private static bool continueRunning = true;

        public static void Main(string[] args)
        {
            Options commandLineOptions = new Options(args);

            if (commandLineOptions.ReserveUrl)
            {
                bool urlReserved = ReserveUrl(commandLineOptions.UrlToReserve, true);
                if (!urlReserved)
                {
                    Environment.ExitCode = 1;
                }
            }
            else
            {
                LogVersionDetails(commandLineOptions);
                userName = commandLineOptions.UserName;
                password = commandLineOptions.Password;
                httpServer = new RemoteServer(commandLineOptions.Port, "wd/hub/", logger);
                bool urlReservationExists = CheckForUrlReservation();
                if (urlReservationExists)
                {
                    httpServer.StartListening();
                    logger.Log(string.Format(CultureInfo.InvariantCulture, "Server started. RemoteWebDriver instances connect to http://<this machine name>:{0}/wd/hub/", commandLineOptions.Port), LogLevel.Info);
                    Console.CancelKeyPress += new ConsoleCancelEventHandler(Console_CancelKeyPress);
                    while (continueRunning)
                    {
                        System.Threading.Thread.Sleep(100);
                    }
                }
            }
        }

        private static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            logger.Log("Shutting down server...", LogLevel.Info);
            httpServer.Dispose();
            continueRunning = false;
            e.Cancel = true;
        }

        private static void LogVersionDetails(Options commandLineOptions)
        {
            string serverVersion = commandLineOptions.ServerVersion;
            string operatingSystemVersion = commandLineOptions.OSVersion;
            logger.Log(serverVersion);
            logger.Log(".NET runtime version: " + Environment.Version.ToString());
            logger.Log("OS version: " + operatingSystemVersion);
        }

        private static bool CheckForUrlReservation()
        {
            bool urlReservationExists = true;
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                urlReservationExists = false;
                ReadOnlyCollection<string> urlReservations = HttpApi.GetReservedUrlList();
                foreach (string reservation in urlReservations)
                {
                    if (reservation == httpServer.ListenerPrefix)
                    {
                        urlReservationExists = true;
                        break;
                    }
                }

                if (!urlReservationExists)
                {
                    logger.Log(string.Format(CultureInfo.InvariantCulture, "URL reservation for '{0}' does not exist. Reserving URL.", httpServer.ListenerPrefix));
                    WindowsIdentity identity = WindowsIdentity.GetCurrent();
                    WindowsPrincipal principal = new WindowsPrincipal(identity);
                    bool isAdmin = principal.IsInRole(WindowsBuiltInRole.Administrator);
                    urlReservationExists = ReserveUrl(httpServer.ListenerPrefix, isAdmin);
                }
            }

            return urlReservationExists;
        }

        private static bool ReserveUrl(string reservePath, bool isAdmin)
        {
            bool urlReserved = true;
            if (!isAdmin)
            {
                Process reserverProcess = new Process();
                ProcessStartInfo reserveInfo = new ProcessStartInfo();
                string fileName = Assembly.GetExecutingAssembly().Location;
                reserveInfo.WorkingDirectory = Environment.CurrentDirectory;
                reserveInfo.FileName = fileName;
                reserveInfo.Arguments = string.Format(CultureInfo.InvariantCulture, "/reserve:{0}", reservePath);
                if (Environment.OSVersion.Version.Major >= 6)
                {
                    logger.Log("Current user is not an administrator. Requesting elevation.");
                    reserveInfo.Verb = "runas";
                    reserveInfo.ErrorDialog = true;
                    reserveInfo.WindowStyle = ProcessWindowStyle.Hidden;
                }
                else
                {
                    logger.Log("Current user is not an administrator. Attempting login.");
                    reserveInfo.UseShellExecute = false;
                    reserveInfo.CreateNoWindow = true;
                    reserveInfo.UserName = userName;
                    SecureString securePassword = new SecureString();
                    foreach (char passwordChar in password.ToCharArray())
                    {
                        securePassword.AppendChar(passwordChar);
                    }

                    reserveInfo.Password = securePassword;
                }

                reserverProcess.StartInfo = reserveInfo;
                try
                {
                    reserverProcess.Start();
                    reserverProcess.WaitForExit();
                    if (reserverProcess.ExitCode != 0)
                    {
                        urlReserved = false;
                    }
                }
                catch (Win32Exception ex)
                {
                    logger.Log("Error reserving URL: " + ex.Message, LogLevel.Error);
                    urlReserved = false;
                }
            }
            else
            {
                try
                {
                    HttpApi.AddReservation(reservePath, "BUILTIN\\Users");
                }
                catch (Win32Exception)
                {
                    urlReserved = false;
                }
            }

            return urlReserved;
        }
    }
}
