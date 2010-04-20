using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenQA.Selenium.Remote.Server
{
    /// <summary>
    /// Contains information about when a driver registration fails
    /// </summary>
    public class DriverRegistrationFailedEventArgs : EventArgs
    {
        private string failureReason = string.Empty;
        private string failureDriverClass = string.Empty;

        /// <summary>
        /// Initializes a new instance of the DriverRegistrationFailedEventArgs class.
        /// </summary>
        /// <param name="driverClassName">Full type name of the driver class that failed to register.</param>
        /// <param name="reason">Reason the driver failed to register.</param>
        public DriverRegistrationFailedEventArgs(string driverClassName, string reason)
        {
            failureDriverClass = driverClassName;
            failureReason = reason;
        }

        /// <summary>
        /// Gets the full type name of the driver class that failed to register.
        /// </summary>
        public string DriverClass
        {
            get { return failureDriverClass; }
        }

        /// <summary>
        /// Gets the reason the driver failed to register.
        /// </summary>
        public string Reason
        {
            get { return failureReason; }
        }
    }
}
