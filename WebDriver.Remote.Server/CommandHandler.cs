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
using System.Net;
using System.Text;

namespace OpenQA.Selenium.Remote.Server
{
    /// <summary>
    /// Represents the base class for all command handlers.
    /// </summary>
    internal abstract class CommandHandler
    {
        #region Constants
        /// <summary>
        /// The URL parameter name for session IDs.
        /// </summary>
        protected const string SessionIdParameterName = "SESSIONID";

        /// <summary>
        /// The URL parameter name for an element ID.
        /// </summary>
        protected const string IdParameterName = "ID";

        /// <summary>
        /// The URL parameter name for the name of a CSS property.
        /// </summary>
        protected const string CssPropertyNameParameterName = "PROPERTYNAME";

        /// <summary>
        /// The URL parameter for a name.
        /// </summary>
        protected const string NameParameterName = "NAME";

        /// <summary>
        /// The URL parameter for an element ID to compare to.
        /// </summary>
        protected const string OtherParameterName = "OTHER";
        #endregion

        #region Private members
        private HttpStatusCode handlerStatusCode = HttpStatusCode.OK;
        private SessionId handlerSessionId;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandHandler"/> class.
        /// </summary>
        /// <param name="locatorParameters">A <see cref="Dictionary{K, V}"/> containing the parameters used to match a resource in the URL.</param>
        /// <param name="parameters">A <see cref="Dictionary{K, V}"/> containing the parameters used to operate on the resource.</param>
        protected CommandHandler(Dictionary<string, string> locatorParameters, Dictionary<string, object> parameters)
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the <see cref="HttpStatusCode"/> to be returned to the client.
        /// </summary>
        internal virtual HttpStatusCode StatusCode
        {
            get { return handlerStatusCode; }
        }

        /// <summary>
        /// Gets or sets the <see cref="SessionId"/> executing <see cref="CommandHandler"/>.
        /// </summary>
        internal SessionId SessionId
        {
            get { return handlerSessionId; }
            set { handlerSessionId = value; }
        }
        #endregion

        #region Abstract methods
        /// <summary>
        /// Executes this command.
        /// </summary>
        /// <returns>An object returned by the command.</returns>
        internal abstract object Execute();
        #endregion

        #region Protected methods
        /// <summary>
        /// Gets a locator parameter from the set of parameters used to locate the resource via the URL.
        /// </summary>
        /// <param name="parameterName">The name of the parameter to get the value of.</param>
        /// <param name="locatorParameters">A <see cref="Dictionary{K, V}"/> containing the locator parameters.</param>
        /// <returns>The value of the parameter.</returns>
        /// <exception cref="ResourceNotFoundException">Thrown if the parameter is not found in the dictionary of parameters.</exception>
        protected static string GetLocatorParameter(string parameterName, Dictionary<string, string> locatorParameters)
        {
            if (!locatorParameters.ContainsKey(parameterName))
            {
                throw new ResourceNotFoundException("Parameter '" + parameterName + "' not specified in locator parameters");
            }

            string parameterValue = locatorParameters[parameterName];
            return parameterValue;
        }

        /// <summary>
        /// Gets a locator parameter from the set of parameters sent in the body of the request and used to operate on the resource.
        /// </summary>
        /// <param name="parameterName">The name of the parameter to get the value of.</param>
        /// <param name="parameters">A <see cref="Dictionary{K, V}"/> containing the parameters.</param>
        /// <returns>The value of the parameter.</returns>
        /// <exception cref="InvalidParameterException">Thrown if the parameter is not found in the dictionary of parameters.</exception>
        protected static object GetParameter(string parameterName, Dictionary<string, object> parameters)
        {
            if (!parameters.ContainsKey(parameterName))
            {
                throw new InvalidParameterException("Parameter '" + parameterName + "' not found");
            }

            return parameters[parameterName];
        }
        #endregion
    }
}
