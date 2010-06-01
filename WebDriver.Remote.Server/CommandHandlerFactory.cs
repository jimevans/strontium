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
    internal class RemoteWebDriverServerCommandHandlerFactory : CommandHandlerFactory
    {
        protected override void AddHandlers()
        {
            // We could reduce the coupling of classes by loading the types
            // dynamically using strings instead of the typeof operator; 
            // however, this is marginally faster.
            Handlers.Add(DriverCommand.DefineDriverMapping, GetConstructor(typeof(DefineDriverMappingHandler)));
            Handlers.Add(DriverCommand.NewSession, GetConstructor(typeof(NewSessionHandler)));
            Handlers.Add(DriverCommand.GetSessionCapabilities, GetConstructor(typeof(GetSessionCapabilitiesHandler)));
            Handlers.Add(DriverCommand.Close, GetConstructor(typeof(CloseHandler)));
            Handlers.Add(DriverCommand.Quit, GetConstructor(typeof(QuitHandler)));
            Handlers.Add(DriverCommand.Get, GetConstructor(typeof(ChangeUrlHandler)));
            Handlers.Add(DriverCommand.GoBack, GetConstructor(typeof(GoBackHandler)));
            Handlers.Add(DriverCommand.GoForward, GetConstructor(typeof(GoForwardHandler)));
            Handlers.Add(DriverCommand.Refresh, GetConstructor(typeof(RefreshHandler)));
            Handlers.Add(DriverCommand.AddCookie, GetConstructor(typeof(AddCookieHandler)));
            Handlers.Add(DriverCommand.GetAllCookies, GetConstructor(typeof(GetAllCookiesHandler)));
            Handlers.Add(DriverCommand.DeleteCookie, GetConstructor(typeof(DeleteCookieNamedHandler)));
            Handlers.Add(DriverCommand.DeleteAllCookies, GetConstructor(typeof(DeleteAllCookiesHandler)));
            Handlers.Add(DriverCommand.FindElement, GetConstructor(typeof(FindElementHandler)));
            Handlers.Add(DriverCommand.FindElements, GetConstructor(typeof(FindElementsHandler)));
            Handlers.Add(DriverCommand.FindChildElement, GetConstructor(typeof(FindChildElementHandler)));
            Handlers.Add(DriverCommand.FindChildElements, GetConstructor(typeof(FindChildElementsHandler)));
            Handlers.Add(DriverCommand.DescribeElement, GetConstructor(typeof(DescribeElementHandler)));
            Handlers.Add(DriverCommand.ClearElement, GetConstructor(typeof(ClearElementHandler)));
            Handlers.Add(DriverCommand.ClickElement, GetConstructor(typeof(ClickElementHandler)));
            Handlers.Add(DriverCommand.HoverOverElement, GetConstructor(typeof(HoverOverElementHandler)));
            Handlers.Add(DriverCommand.SendKeysToElement, GetConstructor(typeof(SendKeysToElementHandler)));
            Handlers.Add(DriverCommand.SubmitElement, GetConstructor(typeof(SubmitElementHandler)));
            Handlers.Add(DriverCommand.ToggleElement, GetConstructor(typeof(ToggleElementHandler)));
            Handlers.Add(DriverCommand.GetCurrentWindowHandle, GetConstructor(typeof(GetCurrentWindowHandler)));
            Handlers.Add(DriverCommand.GetWindowHandles, GetConstructor(typeof(GetAllWindowsHandler)));
            Handlers.Add(DriverCommand.SwitchToWindow, GetConstructor(typeof(SwitchToWindowHandler)));
            Handlers.Add(DriverCommand.SwitchToFrame, GetConstructor(typeof(SwitchToFrameHandler)));
            Handlers.Add(DriverCommand.GetActiveElement, GetConstructor(typeof(GetActiveElementHandler)));
            Handlers.Add(DriverCommand.GetCurrentUrl, GetConstructor(typeof(GetCurrentUrlHandler)));
            Handlers.Add(DriverCommand.GetPageSource, GetConstructor(typeof(GetPageSourceHandler)));
            Handlers.Add(DriverCommand.GetTitle, GetConstructor(typeof(GetTitleHandler)));
            Handlers.Add(DriverCommand.ExecuteScript, GetConstructor(typeof(ExecuteScriptHandler)));
            Handlers.Add(DriverCommand.GetSpeed, GetConstructor(typeof(GetSpeedHandler)));
            Handlers.Add(DriverCommand.SetSpeed, GetConstructor(typeof(SetSpeedHandler)));
            Handlers.Add(DriverCommand.GetElementText, GetConstructor(typeof(GetElementTextHandler)));
            Handlers.Add(DriverCommand.GetElementValue, GetConstructor(typeof(GetElementValueHandler)));
            Handlers.Add(DriverCommand.GetElementTagName, GetConstructor(typeof(GetElementTagNameHandler)));
            Handlers.Add(DriverCommand.SetElementSelected, GetConstructor(typeof(SetElementSelectedHandler)));
            Handlers.Add(DriverCommand.DragElement, GetConstructor(typeof(DragElementHandler)));
            Handlers.Add(DriverCommand.IsElementSelected, GetConstructor(typeof(GetElementSelectedHandler)));
            Handlers.Add(DriverCommand.IsElementEnabled, GetConstructor(typeof(GetElementEnabledHandler)));
            Handlers.Add(DriverCommand.IsElementDisplayed, GetConstructor(typeof(GetElementDisplayedHandler)));
            Handlers.Add(DriverCommand.GetElementLocation, GetConstructor(typeof(GetElementLocationHandler)));
            Handlers.Add(DriverCommand.GetElementLocationOnceScrolledIntoView, GetConstructor(typeof(GetElementScrolledIntoViewLocationHandler)));
            Handlers.Add(DriverCommand.GetElementSize, GetConstructor(typeof(GetElementSizeHandler)));
            Handlers.Add(DriverCommand.GetElementAttribute, GetConstructor(typeof(GetElementAttributeHandler)));
            Handlers.Add(DriverCommand.GetElementValueOfCssProperty, GetConstructor(typeof(GetElementValueOfCssPropertyHandler)));
            Handlers.Add(DriverCommand.ElementEquals, GetConstructor(typeof(ElementEqualsHandler)));
            Handlers.Add(DriverCommand.Screenshot, GetConstructor(typeof(ScreenshotHandler)));
            Handlers.Add(DriverCommand.ImplicitlyWait, GetConstructor(typeof(ImplicitlyWaitHandler)));
            handlers.Add(DriverCommand.ImplicitlyWait, GetConstructor(typeof(ImplicitlyWaitHandler)));
        }
    }
}
