﻿// <copyright file="GetWindowPositionHandler.cs" company="WebDriver Committers">
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
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;

namespace OpenQA.Selenium.Remote.Server.CommandHandlers
{
    /// <summary>
    /// Provides the handler for the <see cref="DriverCommand.GetWindowPosition"/> command.
    /// </summary>
    internal class GetWindowPositionHandler : WebDriverCommandHandler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetWindowPositionHandler"/> class.
        /// </summary>
        /// <param name="locatorParameters">A <see cref="Dictionary{K, V}"/> containing the parameters used to match a resource in the URL.</param>
        /// <param name="parameters">A <see cref="Dictionary{K, V}"/> containing the parameters used to operate on the resource.</param>
        public GetWindowPositionHandler(Dictionary<string, string> locatorParameters, Dictionary<string, object> parameters)
            : base(locatorParameters, parameters)
        {
        }

        /// <summary>
        /// Returns a string representing the description of this <see cref="CommandHandler"/>.
        /// </summary>
        /// <returns>A string representing the description of this <see cref="CommandHandler"/>.</returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "[get window position]");
        }

        /// <summary>
        /// Gets the position of the window for the current driver.
        /// </summary>
        /// <returns>A <see cref="Dictionary{K, V}"/> containing the x and y coordinates of the
        /// upper left-hand corner of the window.</returns>
        public override object Execute()
        {
            Point windowPosition = Session.Driver.Manage().Window.Position;
            Dictionary<string, object> windowPositionValues = new Dictionary<string, object>();
            windowPositionValues.Add("x", windowPosition.X);
            windowPositionValues.Add("y", windowPosition.Y);
            return windowPositionValues;
        }
    }
}