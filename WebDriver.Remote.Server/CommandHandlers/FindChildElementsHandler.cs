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
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;

namespace OpenQA.Selenium.Remote.Server.CommandHandlers
{
    /// <summary>
    /// Provides the handler for the <see cref="DriverCommand.FindChildElements"/> command.
    /// </summary>
    internal class FindChildElementsHandler : WebElementCommandHandler
    {
        private string mechanism;
        private string findValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="FindChildElementsHandler"/> class.
        /// </summary>
        /// <param name="locatorParameters">A <see cref="Dictionary{K, V}"/> containing the parameters used to match a resource in the URL.</param>
        /// <param name="parameters">A <see cref="Dictionary{K, V}"/> containing the parameters used to operate on the resource.</param>
        public FindChildElementsHandler(Dictionary<string, string> locatorParameters, Dictionary<string, object> parameters)
            : base(locatorParameters, parameters)
        {
            mechanism = GetParameter("using", parameters).ToString();
            findValue = GetParameter("value", parameters).ToString();
        }

        /// <summary>
        /// Returns a string representing the description of this <see cref="CommandHandler"/>.
        /// </summary>
        /// <returns>A string representing the description of this <see cref="CommandHandler"/>.</returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "[find child elements of: {0} by: {1}='{2}']", ElementId, mechanism, findValue);
        }

        /// <summary>
        /// Finds all child elements of the element associated with this <see cref="CommandHandler"/> meeting the specified criteria.
        /// </summary>
        /// <returns>An array of <see cref="Dictionary{K, V}"/> objects containing a key of 
        /// "ELEMENT" and the value containing the ID of the element meeting the criteria.</returns>
        internal override object Execute()
        {
            By finder = GetLocator(mechanism, findValue);

            IWebElement parentElement = GetElement();
            ReadOnlyCollection<IWebElement> childElements = parentElement.FindElements(finder);
            List<Dictionary<string, object>> elementList = new List<Dictionary<string, object>>();
            foreach (IWebElement childElement in childElements)
            {
                Dictionary<string, object> element = WrapElement(childElement);
                elementList.Add(element);
            }

            return elementList.ToArray();
        }
    }
}
