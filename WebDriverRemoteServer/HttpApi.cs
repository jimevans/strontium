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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using OpenQA.Selenium.Remote.Server.Internal;

namespace OpenQA.Selenium.Remote.Server
{
    internal static class HttpApi
    {
        private const int GenericExecute = 0x20000000;

        internal static ReadOnlyCollection<string> GetReservedUrlList()
        {
            List<string> revs = new List<string>();

            ErrorCode retVal = ErrorCode.Success; // NOERROR = 0

            retVal = NativeMethods.HttpInitialize(HttpApiConstants.Version1, HttpApiConstants.InitializeConfig, IntPtr.Zero);
            if (ErrorCode.Success == retVal)
            {
                HttpServiceConfigUrlAclQuery inputConfigInfoSet = new HttpServiceConfigUrlAclQuery();
                inputConfigInfoSet.QueryDesc = HttpServiceConfigQueryType.HttpServiceConfigQueryNext;

                int i = 0;
                while (retVal == 0)
                {
                    inputConfigInfoSet.Token = (uint)i;

                    IntPtr inputConfigInfo = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(HttpServiceConfigUrlAclQuery)));
                    Marshal.StructureToPtr(inputConfigInfoSet, inputConfigInfo, false);

                    HttpServiceConfigUrlAclSet outputConfigInfo = new HttpServiceConfigUrlAclSet();
                    IntPtr outputConfigInfoBuffer = Marshal.AllocHGlobal(0);

                    int returnLength = 0;
                    retVal = NativeMethods.HttpQueryServiceConfiguration(
                        IntPtr.Zero,
                        HttpServiceConfigId.HttpServiceConfigUrlAclInfo,
                        inputConfigInfo,
                        Marshal.SizeOf(inputConfigInfoSet),
                        outputConfigInfoBuffer,
                        returnLength,
                        out returnLength,
                        IntPtr.Zero);

                    if (retVal == ErrorCode.InsufficientBuffer)
                    {
                        Marshal.FreeHGlobal(outputConfigInfoBuffer);
                        outputConfigInfoBuffer = Marshal.AllocHGlobal(Convert.ToInt32(returnLength));

                        retVal = NativeMethods.HttpQueryServiceConfiguration(
                            IntPtr.Zero,
                            HttpServiceConfigId.HttpServiceConfigUrlAclInfo,
                            inputConfigInfo,
                            Marshal.SizeOf(inputConfigInfoSet),
                            outputConfigInfoBuffer,
                            returnLength,
                            out returnLength,
                            IntPtr.Zero);
                    }

                    if (ErrorCode.Success == retVal)
                    {
                        outputConfigInfo = (HttpServiceConfigUrlAclSet)Marshal.PtrToStructure(outputConfigInfoBuffer, typeof(HttpServiceConfigUrlAclSet));
                        string urlPrefix = outputConfigInfo.KeyDesc.UrlPrefix;
                        revs.Add(urlPrefix);
                    }

                    Marshal.FreeHGlobal(outputConfigInfoBuffer);
                    Marshal.FreeHGlobal(inputConfigInfo);

                    i++;
                }

                retVal = NativeMethods.HttpTerminate(HttpApiConstants.InitializeConfig, IntPtr.Zero);
            }

            if (ErrorCode.Success != retVal)
            {
                throw new Win32Exception(Convert.ToInt32(retVal, CultureInfo.InvariantCulture));
            }

            return new ReadOnlyCollection<string>(revs);
        }

        internal static void AddReservation(string urlPrefix, string user)
        {
            NTAccount account = new NTAccount(user);
            SecurityIdentifier sid = (SecurityIdentifier)account.Translate(typeof(SecurityIdentifier));
            string sddl = GenerateSddl(sid);
            ErrorCode retVal = ErrorCode.Success; // NOERROR = 0

            retVal = NativeMethods.HttpInitialize(HttpApiConstants.Version1, HttpApiConstants.InitializeConfig, IntPtr.Zero);
            if (ErrorCode.Success == retVal)
            {
                HttpServiceConfigUrlAclKey keyDesc = new HttpServiceConfigUrlAclKey(urlPrefix);
                HttpServiceConfigUrlAclParam paramDesc = new HttpServiceConfigUrlAclParam(sddl);

                HttpServiceConfigUrlAclSet inputConfigInfoSet = new HttpServiceConfigUrlAclSet();
                inputConfigInfoSet.KeyDesc = keyDesc;
                inputConfigInfoSet.ParamDesc = paramDesc;

                IntPtr inputConfigInfoBuffer = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(HttpServiceConfigUrlAclSet)));
                Marshal.StructureToPtr(inputConfigInfoSet, inputConfigInfoBuffer, false);

                retVal = NativeMethods.HttpSetServiceConfiguration(
                    IntPtr.Zero,
                    HttpServiceConfigId.HttpServiceConfigUrlAclInfo,
                    inputConfigInfoBuffer,
                    Marshal.SizeOf(inputConfigInfoSet),
                    IntPtr.Zero);

                if (ErrorCode.AlreadyExists == retVal)
                {
                    retVal = NativeMethods.HttpDeleteServiceConfiguration(
                        IntPtr.Zero,
                        HttpServiceConfigId.HttpServiceConfigUrlAclInfo,
                        inputConfigInfoBuffer,
                        Marshal.SizeOf(inputConfigInfoSet),
                        IntPtr.Zero);

                    if (ErrorCode.Success == retVal)
                    {
                        retVal = NativeMethods.HttpSetServiceConfiguration(
                            IntPtr.Zero,
                            HttpServiceConfigId.HttpServiceConfigUrlAclInfo,
                            inputConfigInfoBuffer,
                            Marshal.SizeOf(inputConfigInfoSet),
                            IntPtr.Zero);
                    }
                }

                Marshal.FreeHGlobal(inputConfigInfoBuffer);
                NativeMethods.HttpTerminate(HttpApiConstants.InitializeConfig, IntPtr.Zero);
            }

            if (ErrorCode.Success != retVal)
            {
                throw new Win32Exception(Convert.ToInt32(retVal, CultureInfo.InvariantCulture));
            }
        }

        private static CommonSecurityDescriptor GetSecurityDescriptor(SecurityIdentifier securityIdentifiers)
        {
            DiscretionaryAcl dacl = GetDacl(securityIdentifiers);

            CommonSecurityDescriptor securityDescriptor = new CommonSecurityDescriptor(
                false, 
                false,
                ControlFlags.GroupDefaulted | ControlFlags.OwnerDefaulted | ControlFlags.DiscretionaryAclPresent,
                null, 
                null, 
                null, 
                dacl);
            return securityDescriptor;
        }

        private static DiscretionaryAcl GetDacl(SecurityIdentifier securityIdentifiers)
        {
            DiscretionaryAcl dacl = new DiscretionaryAcl(false, false, 16);

            dacl.AddAccess(AccessControlType.Allow, securityIdentifiers, GenericExecute, InheritanceFlags.None, PropagationFlags.None);
            return dacl;
        }

        private static string GenerateSddl(SecurityIdentifier securityIdentifiers)
        {
            return GetSecurityDescriptor(securityIdentifiers).GetSddlForm(AccessControlSections.Access);
        }
    }
}
