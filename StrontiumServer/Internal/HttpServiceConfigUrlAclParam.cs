// <copyright file="HttpServiceConfigUrlAclParam.cs" company="WebDriver Committers">
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
using System.Runtime.InteropServices;
using System.Text;

namespace StrontiumServer.Internal
{
    /// <summary>
    /// Represents the struct used to configure URL ACL parameters through the HTTP service.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct HttpServiceConfigUrlAclParam
    {
        /// <summary>
        /// The security descriptor of the ACL as a string.
        /// </summary>
        [MarshalAs(UnmanagedType.LPWStr)]
        public string StringSecurityDescriptor;

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpServiceConfigUrlAclParam"/> struct.
        /// </summary>
        /// <param name="securityDescriptor">The Security Descriptor to configure URL ACLs.</param>
        public HttpServiceConfigUrlAclParam(string securityDescriptor)
        {
            this.StringSecurityDescriptor = securityDescriptor;
        }
    }
}
