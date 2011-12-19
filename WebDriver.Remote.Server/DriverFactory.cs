// <copyright file="DriverFactory.cs" company="WebDriver Committers">
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
using System.Linq;
using System.Reflection;
using System.Text;

namespace OpenQA.Selenium.Remote.Server
{
    /// <summary>
    /// Creates <see cref="IWebDriver"/> instances matching the given criteria.
    /// </summary>
    internal class DriverFactory
    {
        private Dictionary<ICapabilities, Type> knownDrivers = new Dictionary<ICapabilities, Type>();

        /// <summary>
        /// Registers <see cref="Type"/> objects representing types of <see cref="IWebDriver"/> objects.
        /// </summary>
        /// <param name="capabilities">An <see cref="ICapabilities"/> object describing the capabilities of the driver class.</param>
        /// <param name="driverClass">A <see cref="Type"/> used to instantiate the <see cref="IWebDriver"/>.</param>
        internal void RegisterDriver(ICapabilities capabilities, Type driverClass)
        {
            this.knownDrivers.Add(capabilities, driverClass);
        }

        /// <summary>
        /// Creates an instance of an <see cref="IWebDriver"/> most closely matching the specified capabilities.
        /// </summary>
        /// <param name="capabilities">An <see cref="ICapabilities"/> object describing the desired capabilities for the driver to be created.</param>
        /// <returns>An instance of an <see cref="IWebDriver"/> most closely matching the specified capabilities.</returns>
        internal IWebDriver CreateDriverInstance(ICapabilities capabilities)
        {
            Type driverType = this.GetBestMatchForCapabilities(capabilities);

            try
            {
                return (IWebDriver)Activator.CreateInstance(driverType);
            }
            catch (TargetInvocationException e)
            {
                throw new WebDriverException(string.Empty, e);
            }
            catch (MethodAccessException e)
            {
                throw new WebDriverException(string.Empty, e);
            }
        }

        private static bool Matches(string value, string value2)
        {
            // We don't match on null
            return value != null && string.Compare(value, value2, StringComparison.OrdinalIgnoreCase) == 0;
        }

        private Type GetBestMatchForCapabilities(ICapabilities desired)
        {
            int numberOfFieldsMatched = 0;
            Type bestMatch = null;

            foreach (ICapabilities entry in this.knownDrivers.Keys)
            {
                int count = 0;
                if (Matches(entry.BrowserName, desired.BrowserName))
                {
                    count++;
                }

                if (Matches(entry.Version, desired.Version))
                {
                    count++;
                }

                if (entry.IsJavaScriptEnabled == entry.IsJavaScriptEnabled)
                {
                    count++;
                }

                Platform capPlatform = entry.Platform;
                Platform desiredPlatform = desired.Platform;
                if (capPlatform != null && desiredPlatform != null)
                {
                    if (capPlatform.IsPlatformType(desiredPlatform.PlatformType))
                    {
                        count++;
                    }
                }

                if (count > numberOfFieldsMatched)
                {
                    numberOfFieldsMatched = count;
                    bestMatch = this.knownDrivers[entry];
                }
            }

            return bestMatch;
        }
    }
}
