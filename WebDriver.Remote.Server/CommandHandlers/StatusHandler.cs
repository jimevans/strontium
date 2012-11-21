// <copyright file="StatusHandler.cs" company="WebDriver Committers">
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
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;

namespace OpenQA.Selenium.Remote.Server.CommandHandlers
{
    /// <summary>
    /// Provides the handler for the <see cref="DriverCommand.Status"/> command.
    /// </summary>
    internal class StatusHandler : WebDriverCommandHandler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StatusHandler"/> class.
        /// </summary>
        /// <param name="locatorParameters">A <see cref="Dictionary{K, V}"/> containing the parameters used to match a resource in the URL.</param>
        /// <param name="parameters">A <see cref="Dictionary{K, V}"/> containing the parameters used to operate on the resource.</param>
        public StatusHandler(Dictionary<string, string> locatorParameters, Dictionary<string, object> parameters)
            : base(locatorParameters, parameters)
        {
        }

        /// <summary>
        /// Returns a string representing the description of this <see cref="CommandHandler"/>.
        /// </summary>
        /// <returns>A string representing the description of this <see cref="CommandHandler"/>.</returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "[get status]");
        }

        /// <summary>
        /// Gets the status of the current driver.
        /// </summary>
        /// <returns>A <see cref="Dictionary{K, V}"/> representing the status of the server.</returns>
        public override object Execute()
        {
            Dictionary<string, object> build = new Dictionary<string, object>();
            object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyVersionAttribute), false);
            if (attributes.Length > 0)
            {
                AssemblyVersionAttribute version = attributes[0] as AssemblyVersionAttribute;
                build["version"] = version.Version;
            }

            Dictionary<string, object> os = new Dictionary<string, object>();
            string platformName = "windows";
            if (Environment.OSVersion.Platform == PlatformID.MacOSX || Environment.OSVersion.Platform == PlatformID.Unix)
            {
                platformName = "linux";
            }

            os["name"] = platformName;
            os["version"] = Environment.OSVersion.Version.ToString();

            Dictionary<string, object> status = new Dictionary<string, object>();
            status["build"] = build;
            status["os"] = os;
            return status;
        }
    }
}
