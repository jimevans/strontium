/* Copyright notice and license
Copyright 2007-2010 WebDriver committers
Copyright 2007-2010 Google Inc.
Portions copyright 2007 ThoughtWorks, Inc

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

     http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace OpenQA.Selenium.Remote.Server.CommandHandlers
{
    /// <summary>
    /// Provides the handler for the <see cref="DriverCommand.FindElement"/> command.
    /// </summary>
    internal class FindElementHandler : WebDriverCommandHandler
    {
        private string mechanism;
        private string findValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="FindElementHandler"/> class.
        /// </summary>
        /// <param name="locatorParameters">A <see cref="Dictionary{K, V}"/> containing the parameters used to match a resource in the URL.</param>
        /// <param name="parameters">A <see cref="Dictionary{K, V}"/> containing the parameters used to operate on the resource.</param>
        public FindElementHandler(Dictionary<string, string> locatorParameters, Dictionary<string, object> parameters)
            : base(locatorParameters, parameters)
        {
            mechanism = GetCommandParameter("using").ToString();
            findValue = GetCommandParameter("value").ToString();
        }

        /// <summary>
        /// Returns a string representing the description of this <see cref="CommandHandler"/>.
        /// </summary>
        /// <returns>A string representing the description of this <see cref="CommandHandler"/>.</returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "[find element by: {0}='{1}']", mechanism, findValue);
        }

        /// <summary>
        /// Finds an element on the page meeting the specified criteria.
        /// </summary>
        /// <returns>A <see cref="Dictionary{K, V}"/> containing a key of "ELEMENT" and the value
        /// containing the ID of the element meeting the criteria.</returns>
        public override object Execute()
        {
            By finder = GetLocator(mechanism, findValue);

            IWebElement childElement = Session.Driver.FindElement(finder);
            Dictionary<string, object> element = WrapElement(childElement);
            return element;
        }
    }
}
