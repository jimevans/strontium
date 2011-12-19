// <copyright file="DriverSession.cs" company="WebDriver Committers">
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
using System.Linq;
using System.Text;

namespace OpenQA.Selenium.Remote.Server
{
    /// <summary>
    /// Represents a session of an <see cref="IWebDriver"/>.
    /// </summary>
    internal class DriverSession
    {
        private IWebDriver sessionDriver;
        private ICapabilities sessionCapabilities;
        private KnownElementRepository sessionKnownElements = new KnownElementRepository();

        /// <summary>
        /// Initializes a new instance of the <see cref="DriverSession"/> class.
        /// </summary>
        /// <param name="factory">The <see cref="DriverFactory"/> responsible for creating the <see cref="IWebDriver"/> for the session.</param>
        /// <param name="capabilities">The capabilities of the driver of the session.</param>
        internal DriverSession(DriverFactory factory, ICapabilities capabilities)
        {
            this.sessionDriver = factory.CreateDriverInstance(capabilities);
            DesiredCapabilities actualSessionCapabilities = new DesiredCapabilities(capabilities.BrowserName, capabilities.Version, Platform.CurrentPlatform);
            actualSessionCapabilities.IsJavaScriptEnabled = true;
            this.sessionCapabilities = actualSessionCapabilities;
        }

        /// <summary>
        /// Gets the <see cref="IWebDriver"/> associated with this session.
        /// </summary>
        internal IWebDriver Driver
        {
            get { return this.sessionDriver; }
        }

        /// <summary>
        /// Gets the <see cref="ICapabilities"/> describing what the session is capable of.
        /// </summary>
        internal ICapabilities Capabilities
        {
            get { return this.sessionCapabilities; }
        }

        /// <summary>
        /// Gets the repository of all elements known to the session.
        /// </summary>
        internal KnownElementRepository KnownElements
        {
            get { return this.sessionKnownElements; }
        }
    }
}
