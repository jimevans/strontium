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
using System.Linq;
using System.Text;

namespace OpenQA.Selenium.Remote.Server
{
    /// <summary>
    /// Represents the group of known <see cref="IWebElement">IWebElements</see> for a session.
    /// </summary>
    internal class KnownElementRepository
    {
        private Dictionary<string, IWebElement> elements = new Dictionary<string, IWebElement>();

        /// <summary>
        /// Adds an element to the repository.
        /// </summary>
        /// <param name="element">The <see cref="IWebElement"/> to add.</param>
        /// <returns>The ID of the added element.</returns>
        internal string Add(IWebElement element)
        {
            string id = Guid.NewGuid().ToString();
            elements.Add(id, element);
            return id;
        }

        /// <summary>
        /// Gets an element from the repository
        /// </summary>
        /// <param name="id">The ID of the element to return.</param>
        /// <returns>An <see cref="IWebElement"/> from the repository, or returns <see langword="null"/>
        /// if no element with the specified ID exists.</returns>
        internal IWebElement GetElement(string id)
        {
            IWebElement element = null;
            if (HasElement(id))
            {
                element = elements[id];
            }

            return element;
        }

        private bool HasElement(string id)
        {
            return elements.ContainsKey(id);
        }
    }
}
