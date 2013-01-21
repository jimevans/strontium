// <copyright file="UploadFileHandler.cs" company="WebDriver Committers">
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
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Ionic.Zip;

namespace OpenQA.Selenium.Remote.Server.CommandHandlers
{
    /// <summary>
    /// Provides the handler for the <see cref="DriverCommand.UploadFile"/> command.
    /// </summary>
    internal class UploadFileHandler : WebDriverCommandHandler
    {
        private byte[] fileContent;
        private string tempFileName;

        /// <summary>
        /// Initializes a new instance of the <see cref="UploadFileHandler"/> class.
        /// </summary>
        /// <param name="locatorParameters">A <see cref="Dictionary{K, V}"/> containing the parameters used to match a resource in the URL.</param>
        /// <param name="parameters">A <see cref="Dictionary{K, V}"/> containing the parameters used to operate on the resource.</param>
        public UploadFileHandler(Dictionary<string, string> locatorParameters, Dictionary<string, object> parameters)
            : base(locatorParameters, parameters)
        {
            string fileContentString = this.GetCommandParameter("file").ToString();
            this.fileContent = Convert.FromBase64String(fileContentString);
            this.GenerateTempFileName();
        }

        /// <summary>
        /// Returns a string representing the description of this <see cref="CommandHandler"/>.
        /// </summary>
        /// <returns>A string representing the description of this <see cref="CommandHandler"/>.</returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "[upload file to {0}]", this.tempFileName);
        }

        /// <summary>
        /// Sends keys to the element referenced by this <see cref="CommandHandler"/>.
        /// </summary>
        /// <returns>This command always returns <see langword="null"/>.</returns>
        public override object Execute()
        {
            using (MemoryStream zipStream = new MemoryStream(this.fileContent))
            {
                using (ZipFile zipArchive = ZipFile.Read(zipStream))
                {
                    // We already determined that this should only contain a single file
                    // in the archive, so this foreach should only run once.
                    foreach (ZipEntry entry in zipArchive.Entries)
                    {
                        entry.Extract(Path.GetDirectoryName(this.tempFileName), ExtractExistingFileAction.OverwriteSilently);
                    }
                }
            }

            return this.tempFileName;
        }

        private void GenerateTempFileName()
        {
            string tempFileName = string.Empty;
            string directoryName = string.Format(CultureInfo.InvariantCulture, "strontiumsession{0}", SessionId.ToString());
            string destinationDirectory = Path.Combine(Path.GetTempPath(), directoryName);
            using (MemoryStream zipStream = new MemoryStream(this.fileContent))
            {
                using (ZipFile zipArchive = ZipFile.Read(zipStream))
                {
                    // We expect one (and only one!) file to be uploaded in the
                    // zip file for use with sendKeys.
                    int fileNameCount = zipArchive.EntryFileNames.Count;
                    if (fileNameCount != 1)
                    {
                        throw new InvalidOperationException("Sending multiple files is not supported.");
                    }

                    List<string> names = new List<string>(zipArchive.EntryFileNames);
                    tempFileName = Path.Combine(destinationDirectory, names[0]);
                }
            }

            this.tempFileName = tempFileName;
        }
    }
}
