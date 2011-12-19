// <copyright file="RemoteWebDriverServer.cs" company="WebDriver Committers">
// Copyright 2007-2011 WebDriver committers
// Copyright 2007-2011 Google Inc.
// Portions copyright 2007 ThoughtWorks, Inc
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
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using OpenQA.Selenium.Remote.Server.CommandHandlers;
using OpenQA.Selenium.Remote.Server.Loggers;

namespace OpenQA.Selenium.Remote.Server
{
    /// <summary>
    /// Handles requests from remote clients to perform automated tests.
    /// </summary>
    public class RemoteWebDriverServer : RemoteServer
    {
        #region Private members
        private Dictionary<ICapabilities, string> defaultDrivers = new Dictionary<ICapabilities, string>();
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteWebDriverServer"/> class using the specifed port and relative path.
        /// </summary>
        /// <param name="port">The port to listen on.</param>
        /// <param name="path">The relative path to connect to.</param>
        public RemoteWebDriverServer(int port, string path)
            : this(port, path, new ConsoleLogger(LogLevel.Info))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteWebDriverServer"/> class using the specifed port, relative path, and logger.
        /// </summary>
        /// <param name="port">The port to listen on.</param>
        /// <param name="path">The relative path to connect to.</param>
        /// <param name="log">A <see cref="Logger"/> object describing how to log information about commands executed.</param>
        public RemoteWebDriverServer(int port, string path, Logger log)
            : this(port, path, new RemoteWebDriverServerCommandHandlerFactory(), log)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteWebDriverServer"/> class using the specifed port, relative path, and logger.
        /// </summary>
        /// <param name="port">The port to listen on.</param>
        /// <param name="path">The relative path to connect to.</param>
        /// <param name="handlerFactory">A <see cref="CommandHandlerFactory"/> used to create <see cref="CommandHandler"/> instances for handling commands.</param>
        /// <param name="log">A <see cref="Logger"/> object describing how to log information about commands executed.</param>
        private RemoteWebDriverServer(int port, string path, CommandHandlerFactory handlerFactory, Logger log)
            : base(port, path, handlerFactory, log)
        {
            SessionManager.Instance.DriverRegistrationFailed += new EventHandler<DriverRegistrationFailedEventArgs>(this.Instance_DriverRegistrationFailed);
            this.InitializeSessionManager();
        }
        #endregion

        #region Private support methods
        private void Instance_DriverRegistrationFailed(object sender, DriverRegistrationFailedEventArgs e)
        {
            string logMessage = string.Format(CultureInfo.InvariantCulture, "Could not register driver with type '{0}'.\nThe reason given was:\n{1}", e.DriverClass, e.Reason);
            this.ServerLogger.Log(logMessage, LogLevel.Error);
        }

        private void InitializeSessionManager()
        {
            this.defaultDrivers.Add(DesiredCapabilities.InternetExplorer(), "OpenQA.Selenium.IE.InternetExplorerDriver, WebDriver.IE");
            this.defaultDrivers.Add(DesiredCapabilities.Firefox(), "OpenQA.Selenium.Firefox.FirefoxDriver, WebDriver.Firefox");
            this.defaultDrivers.Add(DesiredCapabilities.Chrome(), "OpenQA.Selenium.Chrome.ChromeDriver, WebDriver.Chrome");
            this.RegisterDefaultDrivers();
        }

        private void RegisterDefaultDrivers()
        {
            foreach (ICapabilities capabilities in this.defaultDrivers.Keys)
            {
                if (capabilities.Platform != null && Platform.CurrentPlatform.IsPlatformType(capabilities.Platform.PlatformType))
                {
                    SessionManager.Instance.RegisterDriver(capabilities, this.defaultDrivers[capabilities]);
                }
                else if (capabilities.Platform == null)
                {
                    SessionManager.Instance.RegisterDriver(capabilities, this.defaultDrivers[capabilities]);
                }
            }
        }
        #endregion
    }
}
