// <copyright file="DriverRegistrationFailedEventArgs.cs" company="WebDriver Committers">
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
using System.Linq;
using System.Text;

namespace OpenQA.Selenium.Remote.Server
{
    /// <summary>
    /// Contains information about when a driver registration fails
    /// </summary>
    public class DriverRegistrationFailedEventArgs : EventArgs
    {
        private string failureReason = string.Empty;
        private string failureDriverClass = string.Empty;

        /// <summary>
        /// Initializes a new instance of the DriverRegistrationFailedEventArgs class.
        /// </summary>
        /// <param name="driverClassName">Full type name of the driver class that failed to register.</param>
        /// <param name="reason">Reason the driver failed to register.</param>
        public DriverRegistrationFailedEventArgs(string driverClassName, string reason)
        {
            this.failureDriverClass = driverClassName;
            this.failureReason = reason;
        }

        /// <summary>
        /// Gets the full type name of the driver class that failed to register.
        /// </summary>
        public string DriverClass
        {
            get { return this.failureDriverClass; }
        }

        /// <summary>
        /// Gets the reason the driver failed to register.
        /// </summary>
        public string Reason
        {
            get { return this.failureReason; }
        }
    }
}
