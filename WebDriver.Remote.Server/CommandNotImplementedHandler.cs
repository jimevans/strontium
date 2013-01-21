// <copyright file="CommandNotImplementedHandler.cs" company="WebDriver Committers">
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
using System.Net;
using System.Text;

namespace OpenQA.Selenium.Remote.Server.CommandHandlers
{
    /// <summary>
    /// Provides the handler for the an instance where the command handler has not been implemented.
    /// </summary>
    internal class CommandNotImplementedHandler : CommandHandler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandNotImplementedHandler"/> class.
        /// </summary>
        /// <param name="locatorParameters">A <see cref="Dictionary{K, V}"/> containing the parameters used to match a resource in the URL.</param>
        /// <param name="parameters">A <see cref="Dictionary{K, V}"/> containing the parameters used to operate on the resource.</param>
        public CommandNotImplementedHandler(Dictionary<string, string> locatorParameters, Dictionary<string, object> parameters)
            : base(locatorParameters, parameters)
        {
        }

        /// <summary>
        /// Gets the <see cref="HttpStatusCode"/> to be returned to the client.
        /// </summary>
        public override HttpStatusCode StatusCode
        {
            get { return HttpStatusCode.NotImplemented; }
        }

        /// <summary>
        /// This handler represents a command that has not been implemented. It is a no-op.
        /// </summary>
        /// <returns>This command always returns <see langword="null"/>.</returns>
        /// <exception cref="CommandNotImplementedException">always thrown with this handler</exception>
        public override object Execute()
        {
            throw new CommandNotImplementedException("Command not implemented");
        }
    }
}
