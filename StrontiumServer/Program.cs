﻿// <copyright file="Program.cs" company="WebDriver Committers">
//
// Copyright 2010-2013 Jim Evans (james.h.evans.jr@gmail.com)
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Text;
using OpenQA.Selenium.Remote.Server;
using OpenQA.Selenium.Remote.Server.Loggers;

namespace StrontiumServer
{
    /// <summary>
    /// The class used to launch the application.
    /// </summary>
    internal class Program
    {
        private static RemoteWebDriverServer httpServer;
        private static ConsoleLogger logger = null;
        private static string userName = string.Empty;
        private static string password = string.Empty;
        private static bool continueRunning = true;
        private static bool ignoreRemoteShutdown;

        /// <summary>
        /// The main entry point into the application.
        /// </summary>
        /// <param name="args">The set of arguments passed in on the command line.</param>
        public static void Main(string[] args)
        {
            Options commandLineOptions = new Options(args);
            logger = new ConsoleLogger(commandLineOptions.LoggingLevel);
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
                ignoreRemoteShutdown = commandLineOptions.IgnoreRemoteShutdown;
                httpServer = new RemoteWebDriverServer(commandLineOptions.Port, "wd/hub/", logger);
                httpServer.ShutdownRequested += new EventHandler(OnRemoteServerShutdownRequested);
                bool urlReservationExists = CheckForUrlReservation(commandLineOptions);
                if (urlReservationExists)
                {
                    httpServer.StartListening();
                    logger.Log(string.Format(CultureInfo.InvariantCulture, "Server started. RemoteWebDriver instances connect to http://<this machine name>:{0}/wd/hub/", commandLineOptions.Port), LogLevel.Info);
                    if (!string.IsNullOrEmpty(commandLineOptions.HubLocation))
                    {
                        httpServer.RegisterWithHub(commandLineOptions.HubLocation);
                    }

                    Console.CancelKeyPress += new ConsoleCancelEventHandler(Console_CancelKeyPress);
                    while (continueRunning)
                    {
                        System.Threading.Thread.Sleep(100);
                    }
                }
            }
        }

        private static void OnRemoteServerShutdownRequested(object sender, EventArgs e)
        {
            logger.Log("Remote server shutdown requested...", LogLevel.Info);
            if (ignoreRemoteShutdown)
            {
                logger.Log("Remote server shutdown request ignored", LogLevel.Info);
            }

            ShutdownServer();
        }

        private static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            logger.Log("Shutting down server...", LogLevel.Info);
            ShutdownServer();
            e.Cancel = true;
        }

        private static void ShutdownServer()
        {
            httpServer.Dispose();
            continueRunning = false;
        }

        private static void LogVersionDetails(Options commandLineOptions)
        {
            string serverVersion = commandLineOptions.ServerVersion;
            string operatingSystemVersion = commandLineOptions.OSVersion;
            logger.Log(serverVersion);
            logger.Log(".NET runtime version: " + Environment.Version.ToString());
            logger.Log("OS version: " + operatingSystemVersion);
        }

        private static bool CheckForUrlReservation(Options commandLineOptions)
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
                    urlReservationExists = ReserveUrl(httpServer.ListenerPrefix, commandLineOptions.CurrentUserIsAdmin);
                }
            }

            return urlReservationExists;
        }

        private static bool ReserveUrl(string reservePath, bool isAdmin)
        {
            bool urlReserved = true;
            if (!isAdmin)
            {
                using (Process reserverProcess = new Process())
                {
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
                        using (SecureString securePassword = new SecureString())
                        {
                            foreach (char passwordChar in password.ToCharArray())
                            {
                                securePassword.AppendChar(passwordChar);
                            }

                            reserveInfo.Password = securePassword;
                        }
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
