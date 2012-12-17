// <copyright file="HttpServiceConfigSslParam.cs" company="WebDriver Committers">
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
    /// Represents the struct used to configure SSL parameters through the HTTP service.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct HttpServiceConfigSslParam
    {
        /// <summary>
        /// The hash length for the SSL encryption.
        /// </summary>
        public int SslHashLength;

        /// <summary>
        /// A pointer to the hash of the SSL encryption.
        /// </summary>
        public IntPtr SslHash;

        /// <summary>
        /// The ID of the application.
        /// </summary>
        public Guid AppId;

        /// <summary>
        /// The name of the SSL certificate store.
        /// </summary>
        [MarshalAs(UnmanagedType.LPWStr)]
        public string SslCertStoreName;

        /// <summary>
        /// The default check mode for certificates.
        /// </summary>
        public uint DefaultCertCheckMode;

        /// <summary>
        /// The default revocation time for the certificate.
        /// </summary>
        public int DefaultRevocationFreshnessTime;

        /// <summary>
        /// The default revocation timeout for URL retrieval.
        /// </summary>
        public int DefaultRevocationUrlRetrievalTimeout;

        /// <summary>
        /// The default SSL control identifier.
        /// </summary>
        [MarshalAs(UnmanagedType.LPWStr)]
        public string DefaultSslCtlIdentifier;

        /// <summary>
        /// The default SSL control store name.
        /// </summary>
        [MarshalAs(UnmanagedType.LPWStr)]
        public string DefaultSslCtlStoreName;

        /// <summary>
        /// The default flags for the SSL configuration.
        /// </summary>
        public uint DefaultFlags;
    }
}
