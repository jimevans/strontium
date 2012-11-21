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
        private const string XOffsetParameterName = "xoffset";
        private const string YOffsetParameterName = "yoffset";
        private const string ElementParameterName = "element";
        private string elementId;
        private bool elementProvided = false;
        private int offsetX = 0;
        private int offsetY = 0;
        private bool offsetsProvided = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="MouseMoveToHandler"/> class.
        /// </summary>
        /// <param name="locatorParameters">A <see cref="Dictionary{K, V}"/> containing the parameters used to match a resource in the URL.</param>
        /// <param name="parameters">A <see cref="Dictionary{K, V}"/> containing the parameters used to operate on the resource.</param>
        public MouseMoveToHandler(Dictionary<string, string> locatorParameters, Dictionary<string, object> parameters)
            : base(locatorParameters, parameters)
        {
            if (this.HasCommandParameter(ElementParameterName) && this.GetCommandParameter(ElementParameterName) != null)
            {
                this.elementId = this.GetCommandParameter(ElementParameterName).ToString();
                this.elementProvided = true;
            }
            else
            {
                this.elementProvided = false;
            }

            if (this.HasCommandParameter(XOffsetParameterName) && this.HasCommandParameter(YOffsetParameterName))
            {
                this.offsetX = Convert.ToInt32(GetCommandParameter(XOffsetParameterName));
                this.offsetY = Convert.ToInt32(GetCommandParameter(YOffsetParameterName));
                this.offsetsProvided = true;
            }
            else
            {
                this.offsetsProvided = false;
            }
        }

        /// <summary>
        /// Returns a string representing the description of this <see cref="CommandHandler"/>.
        /// </summary>
        /// <returns>A string representing the description of this <see cref="CommandHandler"/>.</returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "[move mouse: {0}, {1}]", this.elementId, this.offsetsProvided);
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
            if (this.elementProvided)
            {
                IWebElement element = this.Session.KnownElements.GetElement(this.elementId);
                ILocatable locatableElement = element as ILocatable;
                elementLocation = locatableElement.Coordinates;
            }

            if (this.offsetsProvided)
            {
                mouse.MouseMove(elementLocation, this.offsetX, this.offsetY);
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
