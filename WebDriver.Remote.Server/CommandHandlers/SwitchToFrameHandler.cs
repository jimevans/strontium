// <copyright file="SwitchToFrameHandler.cs" company="WebDriver Committers">
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

namespace OpenQA.Selenium.Remote.Server.CommandHandlers
{
    /// <summary>
    /// Provides the handler for the <see cref="DriverCommand.SwitchToFrame"/> command.
    /// </summary>
    internal class SwitchToFrameHandler : WebDriverCommandHandler
    {
        private object frameId;

        /// <summary>
        /// Initializes a new instance of the <see cref="SwitchToFrameHandler"/> class.
        /// </summary>
        /// <param name="locatorParameters">A <see cref="Dictionary{K, V}"/> containing the parameters used to match a resource in the URL.</param>
        /// <param name="parameters">A <see cref="Dictionary{K, V}"/> containing the parameters used to operate on the resource.</param>
        public SwitchToFrameHandler(Dictionary<string, string> locatorParameters, Dictionary<string, object> parameters)
            : base(locatorParameters, parameters)
        {
            this.frameId = this.GetCommandParameter("id");
        }

        /// <summary>
        /// Returns a string representing the description of this <see cref="CommandHandler"/>.
        /// </summary>
        /// <returns>A string representing the description of this <see cref="CommandHandler"/>.</returns>
        public override string ToString()
        {
            string frameIdDescription = "default content";
            if (this.frameId != null)
            {
                frameIdDescription = this.frameId.ToString();
            }

            return string.Format(CultureInfo.InvariantCulture, "[switch to frame: {0}]", frameIdDescription);
        }

        /// <summary>
        /// Switches focus to the specified frame in the current driver session.
        /// </summary>
        /// <returns>This command always returns <see langword="null"/>.</returns>
        public override object Execute()
        {
            if (this.frameId == null)
            {
                Session.Driver.SwitchTo().DefaultContent();
            }
            else
            {
                Dictionary<string, object> element = this.frameId as Dictionary<string, object>;
                if (element != null)
                {
                    if (element.ContainsKey("ELEMENT"))
                    {
                        IWebElement frameElement = Session.KnownElements.GetElement(element["ELEMENT"].ToString());
                        Session.Driver.SwitchTo().Frame(frameElement);
                    }
                }
                else
                {
                    string frameName = this.frameId.ToString();
                    int frameNumber = 0;
                    bool isInt = int.TryParse(frameName, out frameNumber);
                    if (isInt)
                    {
                        Session.Driver.SwitchTo().Frame(frameNumber);
                    }
                    else
                    {
                        Session.Driver.SwitchTo().Frame(frameName);
                    }
                }
            }

            return null;
        }
    }
}
