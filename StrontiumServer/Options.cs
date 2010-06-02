/* Copyright notice and license
Copyright 2007-2010 WebDriver committers
Copyright 2007-2010 Google Inc.
Portions copyright 2007 ThoughtWorks, Inc

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

     http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Security.Principal;
using StrontiumServer.Internal;

namespace StrontiumServer
{
    internal class Options
    {
        private const string PortCommandLineOption = "PORT";
        private const string UserNameCommandLineOption = "USERNAME";
        private const string PasswordCommandLineOption = "PASSWORD";
        private const string ReserveUrlCommandLineOption = "RESERVE";

        private int port = 4444;
        private string userName = string.Empty;
        private string password = string.Empty;
        private string urlToReserve = string.Empty;
        private string operatingSystemVersion = string.Empty;
        private string serverVersion = string.Empty;
        private bool reserveUrl;
        private bool currentUserIsAdmin;

        internal Options(string[] commandLineArguments)
        {
            GetServerVersion();
            GetOSVersion();
            GetCurrentUserAdminStatus();
            foreach (string arg in commandLineArguments)
            {
                string[] argumentValues = arg.Split(new string[] { ":" }, 2, StringSplitOptions.None);
                if (argumentValues.Length > 1)
                {
                    SetOption(argumentValues[0], argumentValues[1]);
                }
            }
        }

        internal string OSVersion
        {
            get { return operatingSystemVersion; }
        }

        internal string ServerVersion
        {
            get { return serverVersion; }
        }

        internal bool ReserveUrl
        {
            get { return reserveUrl; }
        }

        internal string UrlToReserve
        {
            get { return urlToReserve; }
        }

        internal int Port
        {
            get { return port; }
        }

        internal string UserName
        {
            get { return userName; }
        }

        internal string Password
        {
            get { return password; }
        }

        internal bool CurrentUserIsAdmin
        {
            get { return currentUserIsAdmin; }
        }

        private void SetOption(string name, string value)
        {
            string argumentName = name.Substring(1).ToUpperInvariant();
            switch (argumentName)
            {
                case PortCommandLineOption:
                    port = int.Parse(value, CultureInfo.InvariantCulture);
                    break;

                case UserNameCommandLineOption:
                    userName = value;
                    break;

                case PasswordCommandLineOption:
                    password = value;
                    break;

                case ReserveUrlCommandLineOption:
                    reserveUrl = true;
                    urlToReserve = value;
                    break;
            }
        }

        private void GetOSVersion()
        {
            operatingSystemVersion = Environment.OSVersion.VersionString;
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
                        operatingSystemVersion = "Windows XP";
                    }
                    else if (versionInfo.dwMinorVersion == 2)
                    {
                        int isServerR2 = NativeMethods.GetSystemMetrics((int)NativeMethods.SystemMetrics.ServerR2);
                        if (versionType == NativeMethods.VersionNT.Workstation && architecture == NativeMethods.ProcessorArchitecture.AMD64)
                        {
                            operatingSystemVersion = "Windows XP x64 Edition";
                        }
                        else if (versionType != NativeMethods.VersionNT.Workstation && isServerR2 == 0)
                        {
                            operatingSystemVersion = "Windows Server 2003";
                        }
                        else if (versionType == NativeMethods.VersionNT.Workstation && isServerR2 != 0)
                        {
                            operatingSystemVersion = "Windows Server 2003 R2";
                        }
                    }
                    else
                    {
                        operatingSystemVersion = "Windows 2000";
                    }
                }
                else if (versionInfo.dwMajorVersion == 6)
                {
                    if (versionInfo.dwMinorVersion == 0)
                    {
                        if (versionType == NativeMethods.VersionNT.Workstation)
                        {
                            operatingSystemVersion = "Windows Vista";
                        }
                        else
                        {
                            operatingSystemVersion = "Windows Server 2008";
                        }
                    }
                    else
                    {
                        if (versionType == NativeMethods.VersionNT.Workstation)
                        {
                            operatingSystemVersion = "Windows 7";
                        }
                        else
                        {
                            operatingSystemVersion = "Windows Server 2008 R2";
                        }
                    }
                }
                else
                {
                    operatingSystemVersion = "Unsupported Windows NT version";
                }

                if (versionInfo.szCSDVersion.Length > 0)
                {
                    operatingSystemVersion = operatingSystemVersion + " " + versionInfo.szCSDVersion;
                }

                operatingSystemVersion += " " + string.Format(CultureInfo.InvariantCulture, "({0}.{1}.{2})", versionInfo.dwMajorVersion, versionInfo.dwMinorVersion, versionInfo.dwBuildNumber);
                operatingSystemVersion += " " + architecture.ToString().ToLowerInvariant();
            }
        }

        private void GetServerVersion()
        {
            object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
            if (attributes.Length > 0)
            {
                AssemblyDescriptionAttribute description = attributes[0] as AssemblyDescriptionAttribute;
                serverVersion = description.Description;
            }
        }

        private void GetCurrentUserAdminStatus()
        {
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                WindowsIdentity identity = WindowsIdentity.GetCurrent();
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                currentUserIsAdmin = principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
            else
            {
                currentUserIsAdmin = true;
            }
        }
    }
}
