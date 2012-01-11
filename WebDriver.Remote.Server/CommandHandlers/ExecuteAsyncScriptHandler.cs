// <copyright file="ExecuteAsyncScriptHandler.cs" company="WebDriver Committers">
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
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace OpenQA.Selenium.Remote.Server.CommandHandlers
{
    /// <summary>
    /// Provides the handler for the <see cref="DriverCommand.ExecuteAsyncScript"/> command.
    /// </summary>
    internal class ExecuteAsyncScriptHandler : ExecuteScriptHandler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExecuteAsyncScriptHandler"/> class.
        /// </summary>
        /// <param name="locatorParameters">A <see cref="Dictionary{K, V}"/> containing the parameters used to match a resource in the URL.</param>
        /// <param name="parameters">A <see cref="Dictionary{K, V}"/> containing the parameters used to operate on the resource.</param>
        public ExecuteAsyncScriptHandler(Dictionary<string, string> locatorParameters, Dictionary<string, object> parameters)
            : base(locatorParameters, parameters)
        {
            this.Description = "[execute async script: {0}, args: {1}]";
        }

        /// <summary>
        /// Asynchronously executes an arbitrary JavaScript function on the page.
        /// </summary>
        /// <returns>The object value returned by the JavaScript function.</returns>
        public override object Execute()
        {
            IJavaScriptExecutor javascriptDriver = Session.Driver as IJavaScriptExecutor;

            if (javascriptDriver == null)
            {
                throw new InvalidCommandException("Driver does not support JavaScript");
            }

            object[] argsArray = this.ParseArguments();

            object returnValue = javascriptDriver.ExecuteAsyncScript(this.Script, argsArray);
            returnValue = this.ParseJavaScriptReturnValue(returnValue);
            return returnValue;
        }
    }
}
