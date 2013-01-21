// <copyright file="HttpServiceConfigUrlAclKey.cs" company="WebDriver Committers">
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
    /// Represents the struct used to configure URL ACLs through the HTTP service.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct HttpServiceConfigUrlAclKey
    {
        /// <summary>
        /// The URL prefix for the ACL.
        /// </summary>
        [MarshalAs(UnmanagedType.LPWStr)]
        public string UrlPrefix;

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpServiceConfigUrlAclKey"/> struct.
        /// </summary>
        /// <param name="urlPrefix">The URL prefix to configure.</param>
        public HttpServiceConfigUrlAclKey(string urlPrefix)
        {
            this.UrlPrefix = urlPrefix;
        }
    }
}
