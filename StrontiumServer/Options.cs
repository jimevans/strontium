// <copyright file="Options.cs" company="WebDriver Committers">
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
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using StrontiumServer.Internal;

namespace StrontiumServer
{
    /// <summary>
    /// Represents the options available from the command line.
    /// </summary>
    internal class Options
    {
        private const string PortCommandLineOption = "PORT";
        private const string UserNameCommandLineOption = "USERNAME";
        private const string PasswordCommandLineOption = "PASSWORD";
        private const string ReserveUrlCommandLineOption = "RESERVE";
        private const string RemoteShutdownCommandLineOption = "REMOTESHUTDOWN";

        private int port = 4444;
        private string userName = string.Empty;
        private string password = string.Empty;
        private string urlToReserve = string.Empty;
        private string operatingSystemVersion = string.Empty;
        private string serverVersion = string.Empty;
        private bool reserveUrl;
        private bool currentUserIsAdmin;
        private bool ignoreRemoteShutdown;

        /// <summary>
        /// Initializes a new instance of the <see cref="Options"/> class.
        /// </summary>
        /// <param name="commandLineArguments">The array of arguments passed in on the command line.</param>
        internal Options(string[] commandLineArguments)
        {
            this.GetServerVersion();
            this.GetOSVersion();
            this.GetCurrentUserAdminStatus();
            foreach (string arg in commandLineArguments)
            {
                string[] argumentValues = arg.Split(new string[] { ":" }, 2, StringSplitOptions.None);
                if (argumentValues.Length > 1)
                {
                    this.SetOption(argumentValues[0], argumentValues[1]);
                }
            }
        }

        /// <summary>
        /// Gets the version of the operating system.
        /// </summary>
        internal string OSVersion
        {
            get { return this.operatingSystemVersion; }
        }

        /// <summary>
        /// Gets the version of the server.
        /// </summary>
        internal string ServerVersion
        {
            get { return this.serverVersion; }
        }

        /// <summary>
        /// Gets a value indicating whether to reserve a URL for use with the HTTP API.
        /// </summary>
        internal bool ReserveUrl
        {
            get { return this.reserveUrl; }
        }

        /// <summary>
        /// Gets a the URL to reserve for use with the HTTP API.
        /// </summary>
        internal string UrlToReserve
        {
            get { return this.urlToReserve; }
        }

        /// <summary>
        /// Gets the port on which the server should listen.
        /// </summary>
        internal int Port
        {
            get { return this.port; }
        }

        /// <summary>
        /// Gets the user name with which the server should authenticate.
        /// </summary>
        internal string UserName
        {
            get { return this.userName; }
        }

        /// <summary>
        /// Gets the password with which the server should authenticate.
        /// </summary>
        internal string Password
        {
            get { return this.password; }
        }

        /// <summary>
        /// Gets a value indicating whether the current user is an administrator.
        /// </summary>
        internal bool CurrentUserIsAdmin
        {
            get { return this.currentUserIsAdmin; }
        }

        /// <summary>
        /// Gets a value indicating whether the server should ignore remote shutdown requests.
        /// </summary>
        internal bool IgnoreRemoteShutdown
        {
            get { return this.ignoreRemoteShutdown; }
        }

        private void SetOption(string name, string value)
        {
            string argumentName = name.Substring(1).ToUpperInvariant();
            switch (argumentName)
            {
                case PortCommandLineOption:
                    this.port = int.Parse(value, CultureInfo.InvariantCulture);
                    break;

                case UserNameCommandLineOption:
                    this.userName = value;
                    break;

                case PasswordCommandLineOption:
                    this.password = value;
                    break;

                case ReserveUrlCommandLineOption:
                    this.reserveUrl = true;
                    this.urlToReserve = value;
                    break;

                case RemoteShutdownCommandLineOption:
                    this.ignoreRemoteShutdown = value.ToUpperInvariant() == "IGNORE";
                    break;
            }
        }

        private void GetOSVersion()
        {
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                NativeMethods.OSVersionInfoEx versionInfo = new NativeMethods.OSVersionInfoEx();
                versionInfo.dwOSVersionInfoSize = Marshal.SizeOf(versionInfo);
                NativeMethods.GetVersionEx(ref versionInfo);

                NativeMethods.SystemInfo sysInfo = new NativeMethods.SystemInfo();
                NativeMethods.GetSystemInfo(out sysInfo);
                NativeMethods.VersionNT versionType = (NativeMethods.VersionNT)versionInfo.wProductType;
                NativeMethods.ProcessorArchitecture architecture = (NativeMethods.ProcessorArchitecture)sysInfo.processorArchitecture;
                if (versionInfo.dwMajorVersion == 5)
                {
                    if (versionInfo.dwMinorVersion == 1)
                    {
                        this.operatingSystemVersion = "Windows XP";
                    }
                    else if (versionInfo.dwMinorVersion == 2)
                    {
                        int isServerR2 = NativeMethods.GetSystemMetrics((int)NativeMethods.SystemMetrics.ServerR2);
                        if (versionType == NativeMethods.VersionNT.Workstation && architecture == NativeMethods.ProcessorArchitecture.AMD64)
                        {
                            this.operatingSystemVersion = "Windows XP x64 Edition";
                        }
                        else if (versionType != NativeMethods.VersionNT.Workstation && isServerR2 == 0)
                        {
                            this.operatingSystemVersion = "Windows Server 2003";
                        }
                        else if (versionType == NativeMethods.VersionNT.Workstation && isServerR2 != 0)
                        {
                            this.operatingSystemVersion = "Windows Server 2003 R2";
                        }
                    }
                    else
                    {
                        this.operatingSystemVersion = "Windows 2000";
                    }
                }
                else if (versionInfo.dwMajorVersion == 6)
                {
                    if (versionInfo.dwMinorVersion == 0)
                    {
                        if (versionType == NativeMethods.VersionNT.Workstation)
                        {
                            this.operatingSystemVersion = "Windows Vista";
                        }
                        else
                        {
                            this.operatingSystemVersion = "Windows Server 2008";
                        }
                    }
                    else
                    {
                        if (versionType == NativeMethods.VersionNT.Workstation)
                        {
                            this.operatingSystemVersion = "Windows 7";
                        }
                        else
                        {
                            this.operatingSystemVersion = "Windows Server 2008 R2";
                        }
                    }
                }
                else
                {
                    this.operatingSystemVersion = "Unsupported Windows NT version";
                }

                if (versionInfo.szCSDVersion.Length > 0)
                {
                    this.operatingSystemVersion = this.operatingSystemVersion + " " + versionInfo.szCSDVersion;
                }

                this.operatingSystemVersion += " " + string.Format(CultureInfo.InvariantCulture, "({0}.{1}.{2})", versionInfo.dwMajorVersion, versionInfo.dwMinorVersion, versionInfo.dwBuildNumber);
                this.operatingSystemVersion += " " + architecture.ToString().ToLowerInvariant();
            }
        }

        private void GetServerVersion()
        {
            object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
            if (attributes.Length > 0)
            {
                AssemblyDescriptionAttribute description = attributes[0] as AssemblyDescriptionAttribute;
                this.serverVersion = description.Description;
            }
        }

        private void GetCurrentUserAdminStatus()
        {
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                WindowsIdentity identity = WindowsIdentity.GetCurrent();
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                this.currentUserIsAdmin = principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
            else
            {
                this.currentUserIsAdmin = true;
            }
        }
    }
}
