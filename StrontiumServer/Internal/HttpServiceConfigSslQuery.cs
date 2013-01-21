// <copyright file="HttpServiceConfigSslQuery.cs" company="WebDriver Committers">
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
using System.Runtime.InteropServices;
using System.Text;

namespace StrontiumServer.Internal
{
    /// <summary>
    /// Represents the struct used to query SSL configuration through the HTTP service.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct HttpServiceConfigSslQuery
    {
        /// <summary>
        /// The service configuration query type.
        /// </summary>
        public HttpServiceConfigQueryType QueryDesc;

        /// <summary>
        /// The service configuration SSL key.
        /// </summary>
        public HttpServiceConfigSslKey KeyDesc;

        /// <summary>
        /// The token to be used.
        /// </summary>
        public uint Token;
    }
}
