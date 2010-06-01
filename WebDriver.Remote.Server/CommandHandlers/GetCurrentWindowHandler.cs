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
    /// Provides the handler for the <see cref="DriverCommand.GetCurrentWindowHandle"/> command.
    /// </summary>
    internal class GetCurrentWindowHandler : WebDriverCommandHandler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetCurrentWindowHandler"/> class.
        /// </summary>
        /// <param name="locatorParameters">A <see cref="Dictionary{K, V}"/> containing the parameters used to match a resource in the URL.</param>
        /// <param name="parameters">A <see cref="Dictionary{K, V}"/> containing the parameters used to operate on the resource.</param>
        public GetCurrentWindowHandler(Dictionary<string, string> locatorParameters, Dictionary<string, object> parameters)
            : base(locatorParameters, parameters)
        {
        }

        /// <summary>
        /// Returns a string representing the description of this <see cref="CommandHandler"/>.
        /// </summary>
        /// <returns>A string representing the description of this <see cref="CommandHandler"/>.</returns>
        public override string ToString()
        {
            return "[get current window handle]";
        }

        /// <summary>
        /// Gets the current window handle for the current driver session.
        /// </summary>
        /// <returns>A string representing an opaque identifier for the current driver session window.</returns>
        public override object Execute()
        {
            string windowHandle = Session.Driver.GetWindowHandle();
            return windowHandle;
        }
    }
}
