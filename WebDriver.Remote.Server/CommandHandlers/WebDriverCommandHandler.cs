// <copyright file="WebDriverCommandHandler.cs" company="WebDriver Committers">
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
using System.Globalization;
using System.Linq;
using System.Text;

namespace OpenQA.Selenium.Remote.Server.CommandHandlers
{
    /// <summary>
    /// Base class for handling all <see cref="DriverCommand"/> values where the URL
    /// specifies a session ID. In other words, the URL must contain "/session/{sessionId}/".
    /// </summary>
    internal abstract class WebDriverCommandHandler : CommandHandler
    {
        private DriverSession currentSession;
        private string description = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="WebDriverCommandHandler"/> class.
        /// </summary>
        /// <param name="locatorParameters">A <see cref="Dictionary{K, V}"/> containing the parameters used to match a resource in the URL.</param>
        /// <param name="parameters">A <see cref="Dictionary{K, V}"/> containing the parameters used to operate on the resource.</param>
        protected WebDriverCommandHandler(Dictionary<string, string> locatorParameters, Dictionary<string, object> parameters)
            : base(locatorParameters, parameters)
        {
            if (!locatorParameters.ContainsKey(CommandHandler.SessionIdParameterName))
            {
                throw new InvalidCommandException("Command requires a session ID");
            }

            string sessionIdValue = locatorParameters[CommandHandler.SessionIdParameterName];
            this.SessionId = new SessionId(sessionIdValue);
            this.currentSession = SessionManager.Instance.GetSession(this.SessionId);
            if (this.currentSession == null)
            {
                throw new ResourceNotFoundException("Could not find active session with id '" + SessionId.ToString() + "'");
            }
        }

        /// <summary>
        /// Gets or sets the description for the log.
        /// </summary>
        protected string Description
        {
            get { return this.description; }
            set { this.description = value; }
        }

        /// <summary>
        /// Gets the <see cref="DriverSession"/> used to execute this command.
        /// </summary>
        protected DriverSession Session
        {
            get { return this.currentSession; }
        }

        /// <summary>
        /// Returns a string representing the description of this <see cref="CommandHandler"/>.
        /// </summary>
        /// <returns>A string representing the description of this <see cref="CommandHandler"/>.</returns>
        public override string ToString()
        {
            return this.description;
        }

        /// <summary>
        /// Gets a <see cref="By"/> object to be used to locate elements on the page.
        /// </summary>
        /// <param name="mechanism">The mechanism to use, such as ID, name or link text to find elements.</param>
        /// <param name="findValue">The value to match in finding elements.</param>
        /// <returns>A <see cref="By"/> object to be used to locate elements on the page.</returns>
        protected static By GetLocator(string mechanism, string findValue)
        {
            By finder = null;
            switch (mechanism)
            {
                case "id":
                    finder = By.Id(findValue);
                    break;

                case "name":
                    finder = By.Name(findValue);
                    break;

                case "link text":
                    finder = By.LinkText(findValue);
                    break;

                case "partial link text":
                    finder = By.PartialLinkText(findValue);
                    break;

                case "xpath":
                    finder = By.XPath(findValue);
                    break;

                case "tag name":
                    finder = By.TagName(findValue);
                    break;

                case "class name":
                    finder = By.ClassName(findValue);
                    break;

                case "css selector":
                    finder = By.CssSelector(findValue);
                    break;

                default:
                    throw new InvalidParameterException("Cannot find on '" + mechanism + "'");
            }

            return finder;
        }

        /// <summary>
        /// Wraps an <see cref="IWebElement"/> for transmission back to a remote client.
        /// </summary>
        /// <param name="element">The <see cref="IWebElement"/> to wrap for transmission.</param>
        /// <returns>A <see cref="Dictionary{K, V}"/> containing a string key of "ELEMENT",
        /// and a value of the ID of the <see cref="IWebElement"/> to return.</returns>
        protected Dictionary<string, object> WrapElement(IWebElement element)
        {
            string elementId = this.Session.KnownElements.Add(element);
            Dictionary<string, object> wrappedElement = new Dictionary<string, object>();
            wrappedElement.Add("ELEMENT", elementId);
            return wrappedElement;
        }
    }
}
