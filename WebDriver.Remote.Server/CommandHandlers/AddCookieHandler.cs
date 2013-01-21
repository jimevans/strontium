// <copyright file="AddCookieHandler.cs" company="WebDriver Committers">
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
using System.Globalization;
using System.Linq;
using System.Text;

namespace OpenQA.Selenium.Remote.Server.CommandHandlers
{
    /// <summary>
    /// Provides the handler for the <see cref="DriverCommand.AddCookie"/> command.
    /// </summary>
    internal class AddCookieHandler : WebDriverCommandHandler
    {
        private Cookie cookieToAdd;

        /// <summary>
        /// Initializes a new instance of the <see cref="AddCookieHandler"/> class.
        /// </summary>
        /// <param name="locatorParameters">A <see cref="Dictionary{K, V}"/> containing the parameters used to match a resource in the URL.</param>
        /// <param name="parameters">A <see cref="Dictionary{K, V}"/> containing the parameters used to operate on the resource.</param>
        public AddCookieHandler(Dictionary<string, string> locatorParameters, Dictionary<string, object> parameters)
            : base(locatorParameters, parameters)
        {
            Dictionary<string, object> rawCookie = GetCommandParameter("cookie") as Dictionary<string, object>;
            this.cookieToAdd = ProcessCookieParameter(rawCookie);
        }

        /// <summary>
        /// Returns a string representing the description of this <see cref="CommandHandler"/>.
        /// </summary>
        /// <returns>A string representing the description of this <see cref="CommandHandler"/>.</returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "[add cookie: {0}]", this.cookieToAdd.ToString());
        }

        /// <summary>
        /// Adds a cookie to the current driver.
        /// </summary>
        /// <returns>This command always returns <see langword="null"/>.</returns>
        public override object Execute()
        {
            Session.Driver.Manage().Cookies.AddCookie(this.cookieToAdd);
            return null;
        }

        private static Cookie ProcessCookieParameter(Dictionary<string, object> rawCookie)
        {
            if (!rawCookie.ContainsKey("name"))
            {
                throw new InvalidParameterException("Cookie must have a name property");
            }

            if (!rawCookie.ContainsKey("value"))
            {
                throw new InvalidParameterException("Cookie must have a value property");
            }

            string name = rawCookie["name"].ToString();
            string value = rawCookie["value"].ToString();

            string path = string.Empty;
            if (rawCookie.ContainsKey("path") && rawCookie["path"] != null)
            {
                path = rawCookie["path"].ToString();
            }

            string domain = string.Empty;
            if (rawCookie.ContainsKey("domain") && rawCookie["domain"] != null)
            {
                domain = rawCookie["domain"].ToString();
            }

            DateTime? expiry = null;
            if (rawCookie.ContainsKey("expiry") && rawCookie["expiry"] != null)
            {
                string expireString = rawCookie["expiry"].ToString().Replace("UTC", string.Empty);
                expiry = DateTime.Parse(expireString, CultureInfo.InvariantCulture);
            }

            Cookie cookedCookie = new Cookie(name, value, domain, path, expiry);
            return cookedCookie;
        }
    }
}
