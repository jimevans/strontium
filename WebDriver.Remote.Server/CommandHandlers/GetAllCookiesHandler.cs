// <copyright file="GetAllCookiesHandler.cs" company="WebDriver Committers">
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

namespace OpenQA.Selenium.Remote.Server.CommandHandlers
{
    /// <summary>
    /// Provides the handler for the <see cref="DriverCommand.GetAllCookies"/> command.
    /// </summary>
    internal class GetAllCookiesHandler : WebDriverCommandHandler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetAllCookiesHandler"/> class.
        /// </summary>
        /// <param name="locatorParameters">A <see cref="Dictionary{K, V}"/> containing the parameters used to match a resource in the URL.</param>
        /// <param name="parameters">A <see cref="Dictionary{K, V}"/> containing the parameters used to operate on the resource.</param>
        public GetAllCookiesHandler(Dictionary<string, string> locatorParameters, Dictionary<string, object> parameters)
            : base(locatorParameters, parameters)
        {
        }

        /// <summary>
        /// Returns a string representing the description of this <see cref="CommandHandler"/>.
        /// </summary>
        /// <returns>A string representing the description of this <see cref="CommandHandler"/>.</returns>
        public override string ToString()
        {
            return "[get all cookies]";
        }

        /// <summary>
        /// Gets all cookies for the current driver session.
        /// </summary>
        /// <returns>An array of <see cref="Dictionary{K, V}"/> objects containing
        /// representations of the cookies set for the current driver session.</returns>
        public override object Execute()
        {
            List<Dictionary<string, object>> cookieList = new List<Dictionary<string, object>>();
            ReadOnlyCollection<Cookie> allCookies = Session.Driver.Manage().Cookies.AllCookies;
            foreach (Cookie singleCookie in allCookies)
            {
                Dictionary<string, object> rawCookie = ProcessCookie(singleCookie);
                cookieList.Add(rawCookie);
            }

            return cookieList.ToArray();
        }

        private static Dictionary<string, object> ProcessCookie(Cookie singleCookie)
        {
            Dictionary<string, object> rawCookie = new Dictionary<string, object>();
            rawCookie.Add("name", singleCookie.Name);
            rawCookie.Add("value", singleCookie.Value);
            rawCookie.Add("path", singleCookie.Path);
            rawCookie.Add("domain", singleCookie.Domain);
            rawCookie.Add("secure", singleCookie.Secure);
            return rawCookie;
        }
    }
}
