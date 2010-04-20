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
    /// Provides the handler for the <see cref="DriverCommand.DefineDriverMapping"/> command.
    /// </summary>
    internal class DefineDriverMappingHandler : CommandHandler
    {
        private DesiredCapabilities capabilities;
        private string className;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefineDriverMappingHandler"/> class.
        /// </summary>
        /// <param name="locatorParameters">A <see cref="Dictionary{K, V}"/> containing the parameters used to match a resource in the URL.</param>
        /// <param name="parameters">A <see cref="Dictionary{K, V}"/> containing the parameters used to operate on the resource.</param>
        public DefineDriverMappingHandler(Dictionary<string, string> locatorParameters, Dictionary<string, object> parameters)
            : base(locatorParameters, parameters)
        {
            Dictionary<string, object> capabilitiesDictionary = GetParameter("capabilities", parameters) as Dictionary<string, object>;
            if (capabilitiesDictionary == null)
            {
                throw new InvalidParameterException("Parameter 'capabilities' is not a dictionary");
            }

            try
            {
                capabilities = new DesiredCapabilities(capabilitiesDictionary);
            }
            catch (Exception ex)
            {
                throw new InvalidParameterException("Error found parsing parameter 'desiredCapabilities': " + ex.Message, ex);
            }

            className = GetParameter("class", parameters).ToString();
        }

        /// <summary>
        /// Returns a string representing the description of this <see cref="CommandHandler"/>.
        /// </summary>
        /// <returns>A string representing the description of this <see cref="CommandHandler"/>.</returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "[define driver mapping: {0}]", className);
        }

        /// <summary>
        /// Defines a new driver mapping for the server.
        /// </summary>
        /// <returns>This command always returns <see langword="null"/>.</returns>
        internal override object Execute()
        {
            SessionManager.Instance.RegisterDriver(capabilities, className);
            return null;
        }
    }
}
