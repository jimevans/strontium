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
using System.Text;

namespace OpenQA.Selenium.Remote.Server
{
    /// <summary>
    /// Provides the services required to log messages.
    /// </summary>
    public abstract class Logger
    {
        private LogLevel currentLogLevel;

        /// <summary>
        /// Initializes a new instance of the <see cref="Logger"/> class.
        /// </summary>
        /// <param name="level">A <see cref="LogLevel"/> value specifying the level of messages to log.</param>
        protected Logger(LogLevel level)
        {
            this.currentLogLevel = level;
        }

        /// <summary>
        /// Writes an informational message to the log.
        /// </summary>
        /// <param name="message">The message to write to the log.</param>
        public void Log(string message)
        {
            this.Log(message, LogLevel.Info);
        }

        /// <summary>
        /// Writes a message to the log with a given logging level.
        /// </summary>
        /// <param name="message">The message to write to the log.</param>
        /// <param name="level">The logging level of the message.</param>
        public void Log(string message, LogLevel level)
        {
            if (level >= this.currentLogLevel)
            {
                this.WriteMessage(FormatLogMessage(message, level));
            }
        }

        /// <summary>
        /// Writes a message to the log.
        /// </summary>
        /// <param name="message">The message to write to the log.</param>
        protected abstract void WriteMessage(string message);

        private static string FormatLogMessage(string message, LogLevel level)
        {
            string logTime = DateTime.Now.ToString("HH:mm:ss.fff", CultureInfo.InvariantCulture);
            string formattedMessage = string.Format(CultureInfo.InvariantCulture, "{0} {1} - {2}", logTime, level.ToString().ToUpperInvariant(), message);
            return formattedMessage;
        }
    }
}
