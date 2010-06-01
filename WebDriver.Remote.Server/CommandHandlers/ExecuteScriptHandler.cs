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
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace OpenQA.Selenium.Remote.Server.CommandHandlers
{
    /// <summary>
    /// Provides the handler for the <see cref="DriverCommand.ExecuteScript"/> command.
    /// </summary>
    internal class ExecuteScriptHandler : WebDriverCommandHandler
    {
        private string script;
        private object arguments;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExecuteScriptHandler"/> class.
        /// </summary>
        /// <param name="locatorParameters">A <see cref="Dictionary{K, V}"/> containing the parameters used to match a resource in the URL.</param>
        /// <param name="parameters">A <see cref="Dictionary{K, V}"/> containing the parameters used to operate on the resource.</param>
        public ExecuteScriptHandler(Dictionary<string, string> locatorParameters, Dictionary<string, object> parameters)
            : base(locatorParameters, parameters)
        {
            script = GetCommandParameter("script").ToString();
            arguments = GetCommandParameter("args");
        }

        /// <summary>
        /// Returns a string representing the description of this <see cref="CommandHandler"/>.
        /// </summary>
        /// <returns>A string representing the description of this <see cref="CommandHandler"/>.</returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "[execute script: {0}, args: {1}]", script, arguments);
        }

        /// <summary>
        /// Executes an arbitrary JavaScript function on the page.
        /// </summary>
        /// <returns>The object value returned by the JavaScript function.</returns>
        public override object Execute()
        {
            IJavaScriptExecutor javascriptDriver = Session.Driver as IJavaScriptExecutor;

            if (javascriptDriver == null)
            {
                throw new InvalidCommandException("Driver does not support JavaScript");
            }

            object[] argsArray = ParseArguments();

            object returnValue = javascriptDriver.ExecuteScript(script, argsArray);
            returnValue = ParseJavaScriptReturnValue(returnValue);
            return returnValue;
        }

        private object[] ParseArguments()
        {
            object[] argsArray = arguments as object[];
            if (argsArray != null)
            {
                for (int i = 0; i < argsArray.Length; i++)
                {
                    Dictionary<string, object> argAsDictionary = argsArray[i] as Dictionary<string, object>;
                    if (argAsDictionary != null && argAsDictionary.ContainsKey("ELEMENT"))
                    {
                        string elementId = argAsDictionary["ELEMENT"].ToString();
                        IWebElement element = Session.KnownElements.GetElement(elementId);
                        if (element == null)
                        {
                            throw new InvalidParameterException("Element argument with id '" + elementId + "' not found");
                        }

                        argsArray[i] = element;
                    }
                }
            }

            return argsArray;
        }

        private object ParseJavaScriptReturnValue(object returnValue)
        {
            object parsedObject = null;
            IList returnValueAsArray = returnValue as IList;
            IWebElement returnValueAsElement = returnValue as IWebElement;
            if (returnValueAsArray != null)
            {
                List<object> listToReturn = new List<object>();
                for (int i = 0; i < returnValueAsArray.Count; i++)
                {
                    listToReturn.Add(ParseJavaScriptReturnValue(returnValueAsArray[i]));
                }

                parsedObject = listToReturn.ToArray();
            }
            else if (returnValueAsElement != null)
            {
                Dictionary<string, object> element = WrapElement(returnValueAsElement);
                parsedObject = element;
            }
            else
            {
                parsedObject = returnValue;
            }

            return parsedObject;
        }
    }
}
