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
using System.Reflection;
using System.Text;
using OpenQA.Selenium.Remote.Server.CommandHandlers;

namespace OpenQA.Selenium.Remote.Server
{
    /// <summary>
    /// Creates instances of <see cref="CommandHandler"/> objects for the desired <see cref="DriverCommand"/>.
    /// </summary>
    internal class CommandHandlerFactory
    {
        #region Private members
        private static CommandHandlerFactory factoryInstance;
        private static object lockObject = new object();
        private Dictionary<DriverCommand, ConstructorInfo> handlers = new Dictionary<DriverCommand, ConstructorInfo>();
        private ConstructorInfo commandNotImplementedHandlerConstructor; 
        #endregion

        #region Constructor
        /// <summary>
        /// Prevents a default instance of the <see cref="CommandHandlerFactory"/> class from being created.
        /// </summary>
        private CommandHandlerFactory()
        {
            AddHandlers();
            commandNotImplementedHandlerConstructor = GetConstructor(typeof(CommandNotImplementedHandler));
        }
        #endregion

        #region Instance property
        /// <summary>
        /// Gets the singleton instance of the <see cref="CommandHandlerFactory"/>.
        /// </summary>
        internal static CommandHandlerFactory Instance
        {
            get
            {
                lock (lockObject)
                {
                    if (factoryInstance == null)
                    {
                        factoryInstance = new CommandHandlerFactory();
                    }

                    return factoryInstance;
                }
            }
        }
        #endregion

        #region Internal methods
        /// <summary>
        /// Creates a new <see cref="CommandHandler"/> given the <see cref="DriverCommand"/>, locator parameters, and body parameters.
        /// </summary>
        /// <param name="commandName">The <see cref="DriverCommand"/> value to create a handler for.</param>
        /// <param name="locatorParameters">The parameters in the URL used to locate the resource.</param>
        /// <param name="parameters">The parameters in the body of the request used to act on the resource.</param>
        /// <returns>A <see cref="CommandHandler"/> capable of executing the desired command.</returns>
        internal CommandHandler CreateHandler(DriverCommand commandName, Dictionary<string, string> locatorParameters, Dictionary<string, object> parameters)
        {
            // Use reflection. We could refactor to use the new operator, but
            // the perf hit for this shouldn't be too bad.
            ConstructorInfo constructor = GetHandlerConstructor(commandName);
            CommandHandler handler = constructor.Invoke(new object[] { locatorParameters, parameters }) as CommandHandler;
            return handler;
        }

        /// <summary>
        /// Gets a value indicating whether the factory can create a handler for the given command.
        /// </summary>
        /// <param name="commandName">The <see cref="DriverCommand"/> value to create a handler for.</param>
        /// <returns><see langword="true"/> if the factory can create a handler for the command; otherwise <see langword="false"/>.</returns>
        internal bool CanCreateHandler(DriverCommand commandName)
        {
            return handlers.ContainsKey(commandName);
        }
        #endregion

        #region Private support methods
        private static ConstructorInfo GetConstructor(Type handlerType)
        {
            Type[] parameterTypes = new Type[] { typeof(Dictionary<string, string>), typeof(Dictionary<string, object>) };
            ConstructorInfo constructor = handlerType.GetConstructor(parameterTypes);
            return constructor;
        }

        private ConstructorInfo GetHandlerConstructor(DriverCommand commandName)
        {
            ConstructorInfo handlerConstructor = null;
            if (!CanCreateHandler(commandName))
            {
                handlerConstructor = commandNotImplementedHandlerConstructor;
            }
            else
            {
                handlerConstructor = handlers[commandName];
            }

            return handlerConstructor;
        }

        private void AddHandlers()
        {
            // We could reduce the coupling of classes by loading the types
            // dynamically using strings instead of the typeof operator; 
            // however, this is marginally faster.
            handlers.Add(DriverCommand.DefineDriverMapping, GetConstructor(typeof(DefineDriverMappingHandler)));
            handlers.Add(DriverCommand.NewSession, GetConstructor(typeof(NewSessionHandler)));
            handlers.Add(DriverCommand.GetSessionCapabilities, GetConstructor(typeof(GetSessionCapabilitiesHandler)));
            handlers.Add(DriverCommand.Close, GetConstructor(typeof(CloseHandler)));
            handlers.Add(DriverCommand.Quit, GetConstructor(typeof(QuitHandler)));
            handlers.Add(DriverCommand.Get, GetConstructor(typeof(ChangeUrlHandler)));
            handlers.Add(DriverCommand.GoBack, GetConstructor(typeof(GoBackHandler)));
            handlers.Add(DriverCommand.GoForward, GetConstructor(typeof(GoForwardHandler)));
            handlers.Add(DriverCommand.Refresh, GetConstructor(typeof(RefreshHandler)));
            handlers.Add(DriverCommand.AddCookie, GetConstructor(typeof(AddCookieHandler)));
            handlers.Add(DriverCommand.GetAllCookies, GetConstructor(typeof(GetAllCookiesHandler)));
            handlers.Add(DriverCommand.DeleteCookie, GetConstructor(typeof(DeleteCookieNamedHandler)));
            handlers.Add(DriverCommand.DeleteAllCookies, GetConstructor(typeof(DeleteAllCookiesHandler)));
            handlers.Add(DriverCommand.FindElement, GetConstructor(typeof(FindElementHandler)));
            handlers.Add(DriverCommand.FindElements, GetConstructor(typeof(FindElementsHandler)));
            handlers.Add(DriverCommand.FindChildElement, GetConstructor(typeof(FindChildElementHandler)));
            handlers.Add(DriverCommand.FindChildElements, GetConstructor(typeof(FindChildElementsHandler)));
            handlers.Add(DriverCommand.DescribeElement, GetConstructor(typeof(DescribeElementHandler)));
            handlers.Add(DriverCommand.ClearElement, GetConstructor(typeof(ClearElementHandler)));
            handlers.Add(DriverCommand.ClickElement, GetConstructor(typeof(ClickElementHandler)));
            handlers.Add(DriverCommand.HoverOverElement, GetConstructor(typeof(HoverOverElementHandler)));
            handlers.Add(DriverCommand.SendKeysToElement, GetConstructor(typeof(SendKeysToElementHandler)));
            handlers.Add(DriverCommand.SubmitElement, GetConstructor(typeof(SubmitElementHandler)));
            handlers.Add(DriverCommand.ToggleElement, GetConstructor(typeof(ToggleElementHandler)));
            handlers.Add(DriverCommand.GetCurrentWindowHandle, GetConstructor(typeof(GetCurrentWindowHandler)));
            handlers.Add(DriverCommand.GetWindowHandles, GetConstructor(typeof(GetAllWindowsHandler)));
            handlers.Add(DriverCommand.SwitchToWindow, GetConstructor(typeof(SwitchToWindowHandler)));
            handlers.Add(DriverCommand.SwitchToFrame, GetConstructor(typeof(SwitchToFrameHandler)));
            handlers.Add(DriverCommand.GetActiveElement, GetConstructor(typeof(GetActiveElementHandler)));
            handlers.Add(DriverCommand.GetCurrentUrl, GetConstructor(typeof(GetCurrentUrlHandler)));
            handlers.Add(DriverCommand.GetPageSource, GetConstructor(typeof(GetPageSourceHandler)));
            handlers.Add(DriverCommand.GetTitle, GetConstructor(typeof(GetTitleHandler)));
            handlers.Add(DriverCommand.ExecuteScript, GetConstructor(typeof(ExecuteScriptHandler)));
            handlers.Add(DriverCommand.GetSpeed, GetConstructor(typeof(GetSpeedHandler)));
            handlers.Add(DriverCommand.SetSpeed, GetConstructor(typeof(SetSpeedHandler)));
            handlers.Add(DriverCommand.GetElementText, GetConstructor(typeof(GetElementTextHandler)));
            handlers.Add(DriverCommand.GetElementValue, GetConstructor(typeof(GetElementValueHandler)));
            handlers.Add(DriverCommand.GetElementTagName, GetConstructor(typeof(GetElementTagNameHandler)));
            handlers.Add(DriverCommand.SetElementSelected, GetConstructor(typeof(SetElementSelectedHandler)));
            handlers.Add(DriverCommand.DragElement, GetConstructor(typeof(DragElementHandler)));
            handlers.Add(DriverCommand.IsElementSelected, GetConstructor(typeof(GetElementSelectedHandler)));
            handlers.Add(DriverCommand.IsElementEnabled, GetConstructor(typeof(GetElementEnabledHandler)));
            handlers.Add(DriverCommand.IsElementDisplayed, GetConstructor(typeof(GetElementDisplayedHandler)));
            handlers.Add(DriverCommand.GetElementLocation, GetConstructor(typeof(GetElementLocationHandler)));
            handlers.Add(DriverCommand.GetElementLocationOnceScrolledIntoView, GetConstructor(typeof(GetElementScrolledIntoViewLocationHandler)));
            handlers.Add(DriverCommand.GetElementSize, GetConstructor(typeof(GetElementSizeHandler)));
            handlers.Add(DriverCommand.GetElementAttribute, GetConstructor(typeof(GetElementAttributeHandler)));
            handlers.Add(DriverCommand.GetElementValueOfCssProperty, GetConstructor(typeof(GetElementValueOfCssPropertyHandler)));
            handlers.Add(DriverCommand.ElementEquals, GetConstructor(typeof(ElementEqualsHandler)));
            handlers.Add(DriverCommand.Screenshot, GetConstructor(typeof(ScreenshotHandler)));
        }
        #endregion
    }
}
