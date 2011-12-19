// <copyright file="ServerResponse.cs" company="WebDriver Committers">
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
using System.Net;
using System.Text;

namespace OpenQA.Selenium.Remote.Server
{
    /// <summary>
    /// Represents the information the server should send to the remote client in its response.
    /// </summary>
    internal class ServerResponse
    {
        #region Private members
        private Response returnedResponse = new Response();
        private HttpStatusCode statusCode = HttpStatusCode.OK;
        private string contentType = "application/json;charset=UTF-8";
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="ServerResponse"/> class.
        /// </summary>
        /// <param name="responseToReturn">A <see cref="Response"/> object to be sent to the remote client.</param>
        /// <param name="status">An <see cref="HttpStatusCode"/> value to set the status code of the HTTP repsonse.</param>
        internal ServerResponse(Response responseToReturn, HttpStatusCode status)
        {
            this.returnedResponse = responseToReturn;
            this.statusCode = status;
            if (this.statusCode >= HttpStatusCode.BadRequest && this.statusCode < HttpStatusCode.InternalServerError)
            {
                this.contentType = "text/plain";
            }
        }

        /// <summary>
        /// Gets the <see cref="Response"/> object to be sent to the remote client.
        /// </summary>
        internal Response ReturnedResponse
        {
            get { return this.returnedResponse; }
        }

        /// <summary>
        /// Gets the <see cref="HttpStatusCode"/> value to set the status code of the HTTP response.
        /// </summary>
        internal HttpStatusCode StatusCode
        {
            get { return this.statusCode; }
        }

        /// <summary>
        /// Gets the value to which to set the Content-Type header of the HTTP response.
        /// </summary>
        internal string ContentType
        {
            get { return this.contentType; }
        }
    }
}
