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

namespace OpenQA.Selenium.Remote.Server.CommandHandlers
{
    /// <summary>
    /// Base class for handling all <see cref="DriverCommand"/> values where the URL
    /// specifies an element id. In other words, the URL must contain "/element/{id}/".
    /// </summary>
    internal abstract class WebElementCommandHandler : WebDriverCommandHandler
    {
        private string elementId = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="WebElementCommandHandler"/> class.
        /// </summary>
        /// <param name="locatorParameters">A <see cref="Dictionary{K, V}"/> containing the parameters used to match a resource in the URL.</param>
        /// <param name="parameters">A <see cref="Dictionary{K, V}"/> containing the parameters used to operate on the resource.</param>
        protected WebElementCommandHandler(Dictionary<string, string> locatorParameters, Dictionary<string, object> parameters)
            : base(locatorParameters, parameters)
        {
            elementId = GetLocatorParameter(IdParameterName);
        }

        /// <summary>
        /// Gets the ID of the element on which this <see cref="WebElementCommandHandler"/> will execute.
        /// </summary>
        protected string ElementId
        {
            get { return elementId; }
        }

        /// <summary>
        /// Gets the <see cref="IWebElement"/> on which to execute the command, using the specified <see cref="DriverSession"/>.
        /// </summary>
        /// <returns>An <see cref="IWebElement"/> on which to execute the command.</returns>
        /// <exception cref="ResourceNotFoundException">Thrown when the element identified
        /// by this <see cref="WebElementCommandHandler"/> cannot be found in the repository
        /// of known elements for the specified <see cref="DriverSession"/>.</exception>
        protected IWebElement GetElement()
        {
            IWebElement element = Session.KnownElements.GetElement(elementId);
            if (element == null)
            {
                throw new ResourceNotFoundException("Could not find element with id '" + elementId + "'");
            }

            return element;
        }

        /// <summary>
        /// Gets the <see cref="IRenderedWebElement"/> on which to execute the command, 
        /// using the specified <see cref="DriverSession"/>.
        /// </summary>
        /// <returns>An <see cref="IRenderedWebElement"/> on which to execute the command.</returns>
        /// <exception cref="ResourceNotFoundException">Thrown when the element identified 
        /// by this <see cref="WebElementCommandHandler"/> cannot be found in the repository
        /// of known elements for the specified <see cref="DriverSession"/>.</exception>
        /// <exception cref="InvalidCommandException">Thrown when the element identified
        /// by this <see cref="WebElementCommandHandler"/> is not an <see cref="IRenderedWebElement"/>.</exception>
        protected IRenderedWebElement GetRenderedElement()
        {
            IWebElement element = GetElement();
            IRenderedWebElement renderedElement = element as IRenderedWebElement;
            if (renderedElement == null)
            {
                throw new InvalidCommandException("Cannot cast element to IRenderedWebElement");
            }

            return renderedElement;
        }
    }
}
