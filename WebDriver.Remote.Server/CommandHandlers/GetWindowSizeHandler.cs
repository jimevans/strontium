﻿// <copyright file="GetWindowSizeHandler.cs" company="WebDriver Committers">
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
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;

namespace OpenQA.Selenium.Remote.Server.CommandHandlers
{
    /// <summary>
    /// Provides the handler for the <see cref="DriverCommand.GetWindowSize"/> command.
    /// </summary>
    internal class GetWindowSizeHandler : WebDriverCommandHandler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetWindowSizeHandler"/> class.
        /// </summary>
        /// <param name="locatorParameters">A <see cref="Dictionary{K, V}"/> containing the parameters used to match a resource in the URL.</param>
        /// <param name="parameters">A <see cref="Dictionary{K, V}"/> containing the parameters used to operate on the resource.</param>
        public GetWindowSizeHandler(Dictionary<string, string> locatorParameters, Dictionary<string, object> parameters)
            : base(locatorParameters, parameters)
        {
        }

        /// <summary>
        /// Returns a string representing the description of this <see cref="CommandHandler"/>.
        /// </summary>
        /// <returns>A string representing the description of this <see cref="CommandHandler"/>.</returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "[get window size]");
        }

        /// <summary>
        /// Gets the size of the window for the current driver.
        /// </summary>
        /// <returns>A <see cref="Dictionary{K, V}"/> containing the height and width of the window.</returns>
        public override object Execute()
        {
            Size windowSize = Session.Driver.Manage().Window.Size;
            Dictionary<string, object> windowSizeValues = new Dictionary<string, object>();
            windowSizeValues.Add("height", windowSize.Height);
            windowSizeValues.Add("width", windowSize.Width);
            return windowSizeValues;
        }
    }
}
