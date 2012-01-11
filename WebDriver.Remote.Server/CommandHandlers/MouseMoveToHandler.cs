// <copyright file="MouseMoveToHandler.cs" company="WebDriver Committers">
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
using OpenQA.Selenium.Interactions.Internal;

namespace OpenQA.Selenium.Remote.Server.CommandHandlers
{
    /// <summary>
    /// Provides the handler for the <see cref="DriverCommand.MouseMoveTo"/> command.
    /// </summary>
    internal class MouseMoveToHandler : WebDriverCommandHandler
    {
        private static string XOFFSET = "xoffset";
        private static string YOFFSET = "yoffset";
        private static string ELEMENT = "element";
        string elementId;
        bool elementProvided = false;
        int xOffset = 0;
        int yOffset = 0;
        bool offsetsProvided = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="MouseMoveToHandler"/> class.
        /// </summary>
        /// <param name="locatorParameters">A <see cref="Dictionary{K, V}"/> containing the parameters used to match a resource in the URL.</param>
        /// <param name="parameters">A <see cref="Dictionary{K, V}"/> containing the parameters used to operate on the resource.</param>
        public MouseMoveToHandler(Dictionary<string, string> locatorParameters, Dictionary<string, object> parameters)
            : base(locatorParameters, parameters)
        {
            this.GetCommandParameter("");
            if (this.HasCommandParameter(ELEMENT) && this.GetCommandParameter(ELEMENT) != null)
            {
                elementId = this.GetCommandParameter(ELEMENT).ToString();
                elementProvided = true;
            }
            else
            {
                elementProvided = false;
            }

            if (this.HasCommandParameter(XOFFSET) && this.HasCommandParameter(YOFFSET))
            {
                xOffset = (int)GetCommandParameter(XOFFSET);
                yOffset = (int)GetCommandParameter(YOFFSET);
                offsetsProvided = true;
            }
            else
            {
                offsetsProvided = false;
            }
        }

        /// <summary>
        /// Returns a string representing the description of this <see cref="CommandHandler"/>.
        /// </summary>
        /// <returns>A string representing the description of this <see cref="CommandHandler"/>.</returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "[move mouse: {0}, {1}]", elementId, offsetsProvided);
        }

        /// <summary>
        /// Moves the mouse to the element and offset specified.
        /// </summary>
        /// <returns>This command always returns <see langword="null"/>.</returns>
        public override object Execute()
        {
            IHasInputDevices hasInputDevicesDriver = this.Session.Driver as IHasInputDevices;
            IMouse mouse = hasInputDevicesDriver.Mouse;
 
            ICoordinates elementLocation = null;
            if (elementProvided)
            {
                IWebElement element = this.Session.KnownElements.GetElement(elementId);
                ILocatable locatableElement = element as ILocatable;
                elementLocation = locatableElement.Coordinates;
            }

            if (offsetsProvided)
            {
                mouse.MouseMove(elementLocation, xOffset, yOffset);
            }
            else
            {
                mouse.MouseMove(elementLocation);
            }

            mouse.MouseDown(null);
            return null;
        }
    }
}
