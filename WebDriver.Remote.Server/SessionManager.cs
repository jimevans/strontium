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
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace OpenQA.Selenium.Remote.Server
{
    /// <summary>
    /// Manages the sessions active in the remote server.
    /// </summary>
    internal class SessionManager
    {
        private const string LibraryPath = "DriverLibraries";
        private static object lockObject = new object();
        private static SessionManager managerInstance;
        private Dictionary<SessionId, DriverSession> sessionDictionary = new Dictionary<SessionId, DriverSession>();
        private DriverFactory factory = new DriverFactory();

        /// <summary>
        /// Prevents a default instance of the <see cref="SessionManager"/> class from being created.
        /// </summary>
        private SessionManager()
        {
            sessionDictionary = new Dictionary<SessionId, DriverSession>();
        }

        /// <summary>
        /// Event raised when the registration of a driver fails.
        /// </summary>
        public event EventHandler<DriverRegistrationFailedEventArgs> DriverRegistrationFailed;

        /// <summary>
        /// Gets the singleton instance of the <see cref="SessionManager"/> class.
        /// </summary>
        internal static SessionManager Instance
        {
            get
            {
                lock (lockObject)
                {
                    if (managerInstance == null)
                    {
                        managerInstance = new SessionManager();
                    }

                    return managerInstance;
                }
            }
        }

        /// <summary>
        /// Registers a driver for use with the <see cref="SessionManager"/>.
        /// </summary>
        /// <param name="capabilities">An <see cref="ICapabilities"/> object describing the capabilities of the driver class.</param>
        /// <param name="className">The fully-qualified class name of the class implementing <see cref="IWebDriver"/>.</param>
        internal void RegisterDriver(ICapabilities capabilities, string className)
        {
            try
            {
                Type driverType = LoadDriverType(className);
                Type interfaceType = driverType.GetInterface("OpenQA.Selenium.IWebDriver", true);
                if (interfaceType == null)
                {
                   OnDriverRegistrationFailed(new DriverRegistrationFailedEventArgs(className, "Class does not implement IWebDriver"));
                }
                else
                {
                    factory.RegisterDriver(capabilities, driverType);
                }
            }
            catch (TypeLoadException typeLoadEx)
            {
                OnDriverRegistrationFailed(new DriverRegistrationFailedEventArgs(className, typeLoadEx.Message));
            }
            catch (FileNotFoundException fileNotFoundEx)
            {
                OnDriverRegistrationFailed(new DriverRegistrationFailedEventArgs(className, fileNotFoundEx.Message));
            }
        }

        /// <summary>
        /// Gets an existing session given the specified <see cref="SessionId"/>.
        /// </summary>
        /// <param name="sessionId">The <see cref="SessionId"/> identifying the session.</param>
        /// <returns>The <see cref="DriverSession"/> corresponding to the ID.</returns>
        internal DriverSession GetSession(SessionId sessionId)
        {
            DriverSession existingSession = null;
            if (SessionExists(sessionId))
            {
                existingSession = sessionDictionary[sessionId];
            }

            return existingSession;
        }

        /// <summary>
        /// Creates a new session with the desired capabilities.
        /// </summary>
        /// <param name="desiredCapabilities">An <see cref="ICapabilities"/> object describing the desired capabilities of the session.</param>
        /// <returns>The <see cref="SessionId"/> of the created session.</returns>
        internal SessionId CreateSession(ICapabilities desiredCapabilities)
        {
            DriverSession newSession = new DriverSession(factory, desiredCapabilities);

            SessionId newSessionId = new SessionId(Guid.NewGuid().ToString());
            sessionDictionary.Add(newSessionId, newSession);
            return newSessionId;
        }

        /// <summary>
        /// Removes a session.
        /// </summary>
        /// <param name="sessionId">The <see cref="SessionId"/> identifying the session to remove.</param>
        internal void RemoveSession(SessionId sessionId)
        {
            if (SessionExists(sessionId))
            {
                sessionDictionary.Remove(sessionId);
            }
        }

        /// <summary>
        /// Raises the event for driver registration failure.
        /// </summary>
        /// <param name="e">A <see cref="DriverRegistrationFailedEventArgs"/> object describing
        /// the reason the driver registration failed.</param>
        protected void OnDriverRegistrationFailed(DriverRegistrationFailedEventArgs e)
        {
            if (DriverRegistrationFailed != null)
            {
                DriverRegistrationFailed(this, e);
            }
        }

        private bool SessionExists(SessionId sessionId)
        {
            return sessionId != null && sessionDictionary.ContainsKey(sessionId);
        }

        private static Type LoadDriverType(string typeDescriptor)
        {
            string[] typeDescriptorParts = typeDescriptor.Split(new char[] { ',' });
            string typeFriendlyName = typeDescriptorParts[0].Trim();
            Assembly driverAssembly = null;
            if (typeDescriptorParts.Length > 1)
            {
                string assemblyName = typeDescriptorParts[1].Trim();
                Assembly[] loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();
                foreach (Assembly loadedAssembly in loadedAssemblies)
                {
                    if (loadedAssembly.GetName().Name == assemblyName)
                    {
                        driverAssembly = loadedAssembly;
                        break;
                    }
                }

                if (driverAssembly == null)
                {
                    string currentDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                    string assemblyFileName = string.Format(CultureInfo.InvariantCulture, "{0}.dll", assemblyName);
                    driverAssembly = Assembly.LoadFrom(Path.Combine(Path.Combine(currentDir, LibraryPath), assemblyFileName));
                }
            }

            Type driverType = driverAssembly.GetType(typeFriendlyName, true, true);
            return driverType;
        }
    }
}
