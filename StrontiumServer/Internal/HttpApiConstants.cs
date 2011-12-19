// <copyright file="HttpApiConstants.cs" company="WebDriver Committers">
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
using System.Linq;
using System.Text;

namespace StrontiumServer.Internal
{
    /// <summary>
    /// Represents the constants for use with the HTTP API
    /// </summary>
    internal static class HttpApiConstants
    {
        /// <summary>
        /// The constant value used for initializing the HTTP configuration,
        /// </summary>
        public const uint InitializeConfig = 0x00000002;

        /// <summary>
        /// Gets the HttpApiVersion for HTTP 1.0
        /// </summary>
        public static HttpApiVersion Version1
        {
            get
            {
                return new HttpApiVersion(1, 0);
            }
        }
    }
}
