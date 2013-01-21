// <copyright file="RemoteWebDriverServerCommandHandlerFactory.cs" company="WebDriver Committers">
//
// Copyright 2010-2013 Jim Evans (james.h.evans.jr@gmail.com)
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
        /// <summary>
        /// Adds handler constructors to the dictionary of handler creators.
        /// </summary>
        protected override void AddHandlers()
        {
            // We could reduce the coupling of classes by loading the types
            // dynamically using strings instead of the typeof operator; 
            // however, this is marginally faster.
            this.MapCommandHandler(DriverCommand.DefineDriverMapping, typeof(DefineDriverMappingHandler));
            this.MapCommandHandler(DriverCommand.Status, typeof(StatusHandler));
            this.MapCommandHandler(DriverCommand.NewSession, typeof(NewSessionHandler));
            this.MapCommandHandler(DriverCommand.GetSessionCapabilities, typeof(GetSessionCapabilitiesHandler));
            this.MapCommandHandler(DriverCommand.GetSessionList, typeof(GetSessionListHandler));
            this.MapCommandHandler(DriverCommand.Close, typeof(CloseHandler));
            this.MapCommandHandler(DriverCommand.Quit, typeof(QuitHandler));
            this.MapCommandHandler(DriverCommand.Get, typeof(ChangeUrlHandler));
            this.MapCommandHandler(DriverCommand.GoBack, typeof(GoBackHandler));
            this.MapCommandHandler(DriverCommand.GoForward, typeof(GoForwardHandler));
            this.MapCommandHandler(DriverCommand.Refresh, typeof(RefreshHandler));
            this.MapCommandHandler(DriverCommand.AddCookie, typeof(AddCookieHandler));
            this.MapCommandHandler(DriverCommand.GetAllCookies, typeof(GetAllCookiesHandler));
            this.MapCommandHandler(DriverCommand.DeleteCookie, typeof(DeleteCookieNamedHandler));
            this.MapCommandHandler(DriverCommand.DeleteAllCookies, typeof(DeleteAllCookiesHandler));
            this.MapCommandHandler(DriverCommand.FindElement, typeof(FindElementHandler));
            this.MapCommandHandler(DriverCommand.FindElements, typeof(FindElementsHandler));
            this.MapCommandHandler(DriverCommand.FindChildElement, typeof(FindChildElementHandler));
            this.MapCommandHandler(DriverCommand.FindChildElements, typeof(FindChildElementsHandler));
            this.MapCommandHandler(DriverCommand.DescribeElement, typeof(DescribeElementHandler));
            this.MapCommandHandler(DriverCommand.ClearElement, typeof(ClearElementHandler));
            this.MapCommandHandler(DriverCommand.ClickElement, typeof(ClickElementHandler));
            this.MapCommandHandler(DriverCommand.SendKeysToElement, typeof(SendKeysToElementHandler));
            this.MapCommandHandler(DriverCommand.SubmitElement, typeof(SubmitElementHandler));
            this.MapCommandHandler(DriverCommand.GetCurrentWindowHandle, typeof(GetCurrentWindowHandler));
            this.MapCommandHandler(DriverCommand.GetWindowHandles, typeof(GetAllWindowsHandler));
            this.MapCommandHandler(DriverCommand.SwitchToWindow, typeof(SwitchToWindowHandler));
            this.MapCommandHandler(DriverCommand.SwitchToFrame, typeof(SwitchToFrameHandler));
            this.MapCommandHandler(DriverCommand.GetActiveElement, typeof(GetActiveElementHandler));
            this.MapCommandHandler(DriverCommand.GetCurrentUrl, typeof(GetCurrentUrlHandler));
            this.MapCommandHandler(DriverCommand.GetPageSource, typeof(GetPageSourceHandler));
            this.MapCommandHandler(DriverCommand.GetTitle, typeof(GetTitleHandler));
            this.MapCommandHandler(DriverCommand.ExecuteScript, typeof(ExecuteScriptHandler));
            this.MapCommandHandler(DriverCommand.ExecuteAsyncScript, typeof(ExecuteAsyncScriptHandler));
            this.MapCommandHandler(DriverCommand.GetElementText, typeof(GetElementTextHandler));
            this.MapCommandHandler(DriverCommand.GetElementTagName, typeof(GetElementTagNameHandler));
            this.MapCommandHandler(DriverCommand.IsElementSelected, typeof(GetElementSelectedHandler));
            this.MapCommandHandler(DriverCommand.IsElementEnabled, typeof(GetElementEnabledHandler));
            this.MapCommandHandler(DriverCommand.IsElementDisplayed, typeof(GetElementDisplayedHandler));
            this.MapCommandHandler(DriverCommand.GetElementLocation, typeof(GetElementLocationHandler));
            this.MapCommandHandler(DriverCommand.GetElementLocationOnceScrolledIntoView, typeof(GetElementScrolledIntoViewLocationHandler));
            this.MapCommandHandler(DriverCommand.GetElementSize, typeof(GetElementSizeHandler));
            this.MapCommandHandler(DriverCommand.GetElementAttribute, typeof(GetElementAttributeHandler));
            this.MapCommandHandler(DriverCommand.GetElementValueOfCssProperty, typeof(GetElementValueOfCssPropertyHandler));
            this.MapCommandHandler(DriverCommand.ElementEquals, typeof(ElementEqualsHandler));
            this.MapCommandHandler(DriverCommand.Screenshot, typeof(ScreenshotHandler));
            this.MapCommandHandler(DriverCommand.ImplicitlyWait, typeof(ImplicitlyWaitHandler));
            this.MapCommandHandler(DriverCommand.SetAsyncScriptTimeout, typeof(SetAsyncScriptTimeoutHandler));
            this.MapCommandHandler(DriverCommand.SetTimeout, typeof(SetTimeoutHandler));
            this.MapCommandHandler(DriverCommand.AcceptAlert, typeof(AcceptAlertHandler));
            this.MapCommandHandler(DriverCommand.DismissAlert, typeof(DismissAlertHandler));
            this.MapCommandHandler(DriverCommand.GetAlertText, typeof(GetAlertTextHandler));
            this.MapCommandHandler(DriverCommand.SetAlertValue, typeof(SetAlertTextHandler));
            this.MapCommandHandler(DriverCommand.GetWindowPosition, typeof(GetWindowPositionHandler));
            this.MapCommandHandler(DriverCommand.SetWindowPosition, typeof(SetWindowPositionHandler));
            this.MapCommandHandler(DriverCommand.GetWindowSize, typeof(GetWindowSizeHandler));
            this.MapCommandHandler(DriverCommand.SetWindowSize, typeof(SetWindowSizeHandler));
            this.MapCommandHandler(DriverCommand.MaximizeWindow, typeof(MaximizeWindowHandler));
            this.MapCommandHandler(DriverCommand.GetOrientation, typeof(GetOrientationHandler));
            this.MapCommandHandler(DriverCommand.SetOrientation, typeof(SetOrientationHandler));
            this.MapCommandHandler(DriverCommand.MouseClick, typeof(MouseClickHandler));
            this.MapCommandHandler(DriverCommand.MouseDoubleClick, typeof(MouseDoubleClickHandler));
            this.MapCommandHandler(DriverCommand.MouseDown, typeof(MouseDownHandler));
            this.MapCommandHandler(DriverCommand.MouseUp, typeof(MouseUpHandler));
            this.MapCommandHandler(DriverCommand.MouseMoveTo, typeof(MouseMoveToHandler));
            this.MapCommandHandler(DriverCommand.SendKeysToActiveElement, typeof(SendKeysToActiveElementHandler));
            this.MapCommandHandler(DriverCommand.UploadFile, typeof(UploadFileHandler));
        }
    }
}
