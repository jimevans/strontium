// <copyright file="RemoteServer.cs" company="WebDriver Committers">
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
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using OpenQA.Selenium.Remote.Server.Loggers;

namespace OpenQA.Selenium.Remote.Server
{
    /// <summary>
    /// Handles requests from remote clients to perform automated tests.
    /// </summary>
    public class RemoteServer : IDisposable
    {
        #region Constants
        private const int AccessDenied = 5;
        private const int SharingViolation = 32;
        private const string ShutdownUrlFragment = "SHUTDOWN";
        private const string NodeConfiguration = @"{{
""capabilities"":
  [
    {{
      ""browserName"": ""firefox"",
      ""maxInstances"": 5,
      ""seleniumProtocol"": ""WebDriver""
    }},
    {{
      ""browserName"": ""chrome"",
      ""maxInstances"": 5,
      ""seleniumProtocol"": ""WebDriver""
    }},
    {{
      ""platform"": ""WINDOWS"",
      ""browserName"": ""internet explorer"",
      ""maxInstances"": 1,
      ""seleniumProtocol"": ""WebDriver""
    }}
  ],
""configuration"":
  {{
    ""remoteHost"": ""http://{0}:{1}""
  }}
}}";
        #endregion

        #region Private members
        private HttpListener listener = new HttpListener();
        private UriTemplateTable getDispatcherTable;
        private UriTemplateTable postDispatcherTable;
        private UriTemplateTable deleteDispatcherTable;
        private CommandHandlerFactory handlerFactory;
        private Logger serverLogger;
        private string listenerPrefix;
        private string listenerPath;
        private int listenerPort;
        private bool listenerStopping;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteServer"/> class using the specified port and relative path.
        /// </summary>
        /// <param name="port">The port to listen on.</param>
        /// <param name="path">The relative path to connect to.</param>
        /// <param name="handlerFactory">A <see cref="CommandHandlerFactory"/> used to create <see cref="CommandHandler"/> instances for handling commands.</param>
        protected RemoteServer(int port, string path, CommandHandlerFactory handlerFactory)
            : this(port, path, handlerFactory, new ConsoleLogger(LogLevel.Info))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteServer"/> class using the specified port and relative path.
        /// </summary>
        /// <param name="port">The port to listen on.</param>
        /// <param name="path">The relative path to connect to.</param>
        /// <param name="handlerFactory">A <see cref="CommandHandlerFactory"/> used to create <see cref="CommandHandler"/> instances for handling commands.</param>
        /// <param name="log">A <see cref="Logger"/> used to log information in the server.</param>
        protected RemoteServer(int port, string path, CommandHandlerFactory handlerFactory, Logger log)
            : this("*", port, path, handlerFactory, log)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteServer"/> class using the specified port, relative path, and logger.
        /// </summary>
        /// <param name="basePath">The base path of the server to listen on.</param>
        /// <param name="port">The port to listen on.</param>
        /// <param name="path">The relative path to connect to.</param>
        /// <param name="handlerFactory">A <see cref="CommandHandlerFactory"/> used to create <see cref="CommandHandler"/> instances for handling commands.</param>
        /// <param name="log">A <see cref="Logger"/> object describing how to log information about commands executed.</param>
        protected RemoteServer(string basePath, int port, string path, CommandHandlerFactory handlerFactory, Logger log)
        {
            if (path == null)
            {
                throw new ArgumentNullException("path", "path cannot be null");
            }

            this.handlerFactory = handlerFactory;

            this.listenerPort = port;

            if (!path.EndsWith("/", StringComparison.OrdinalIgnoreCase))
            {
                path = path + "/";
            }

            if (!path.StartsWith("/", StringComparison.OrdinalIgnoreCase))
            {
                path = "/" + path;
            }

            this.listenerPath = path;
            this.listenerPrefix = string.Format(CultureInfo.InvariantCulture, "http://{0}:{1}{2}", basePath, this.listenerPort, this.listenerPath);

            this.serverLogger = log;
        }
        #endregion

        /// <summary>
        /// Event raised when a shutdown of the server is requested via the shutdown URL.
        /// </summary>
        public event EventHandler ShutdownRequested;

        #region Properties
        /// <summary>
        /// Gets the full base URL prefix on which all requests are matched.
        /// </summary>
        public string ListenerPrefix
        {
            get { return this.listenerPrefix; }
        }

        /// <summary>
        /// Gets the logger used for this remote server.
        /// </summary>
        protected Logger ServerLogger
        {
            get { return this.serverLogger; }
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Starts the remote server listening for requests.
        /// </summary>
        public void StartListening()
        {
            try
            {
                this.listenerStopping = false;
                this.ConstructDispatcherTables(this.listenerPrefix);
                this.listener.Prefixes.Add(this.listenerPrefix);
                this.listener.Start();
                this.listener.BeginGetContext(this.OnClientConnect, this.listener);
            }
            catch (HttpListenerException ex)
            {
                if (ex.ErrorCode == AccessDenied)
                {
                    this.serverLogger.Log("Access was denied to listen on added prefixes. Attempt elevation.", LogLevel.Error);
                }
                else if (ex.ErrorCode == SharingViolation)
                {
                    this.serverLogger.Log(string.Format(CultureInfo.InvariantCulture, "Another application is already listening on port {0}", this.listenerPort), LogLevel.Error);
                }
                else
                {
                    this.serverLogger.Log(string.Format(CultureInfo.InvariantCulture, "An unexpected error with error code {0} occurred.", ex.ErrorCode), LogLevel.Error);
                }
            }
        }

        /// <summary>
        /// Stops the remote server listening for requests.
        /// </summary>
        public void StopListening()
        {
            this.listenerStopping = true;
            if (this.listener.IsListening)
            {
                this.listener.Stop();
            }
        }

        /// <summary>
        /// Registers this server with a grid hub.
        /// </summary>
        /// <param name="hubLocation">The location of the hub.</param>
        public void RegisterWithHub(string hubLocation)
        {
            if (!string.IsNullOrEmpty(hubLocation))
            {
                Uri hubUri = null;
                bool isValidHubUri = Uri.TryCreate(hubLocation, UriKind.Absolute, out hubUri);
                if (isValidHubUri)
                {
                    HttpWebRequest request = WebRequest.Create(hubUri) as HttpWebRequest;
                    request.Method = "POST";
                    string nodeConfig = string.Format(CultureInfo.InvariantCulture, NodeConfiguration, GetIPAddressList()[0].ToString(), this.listenerPort);
                    byte[] requestBodyBuffer = Encoding.UTF8.GetBytes(nodeConfig);
                    Stream requestStream = request.GetRequestStream();
                    requestStream.Write(requestBodyBuffer, 0, requestBodyBuffer.Length);
                    try
                    {
                        HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            this.serverLogger.Log(string.Format(CultureInfo.InvariantCulture, "Registered as a node with hub located at {0}", hubLocation));
                        }
                        else
                        {
                            this.serverLogger.Log(string.Format(CultureInfo.InvariantCulture, "Received error HTTP status code registering as a node with hub located at {0}: ", hubLocation, response.StatusCode.ToString()));
                        }
                    }
                    catch (WebException ex)
                    {
                        this.serverLogger.Log(string.Format(CultureInfo.InvariantCulture, "Error registring as a node with hub located at {0}: {1}", hubLocation, ex.Message));
                    }
                }
            }
        }
        #endregion

        #region IDisposable Members
        /// <summary>
        /// Releases all resources associated with this <see cref="RemoteServer"/>.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases all managed and unmanaged resources associated with this <see cref="RemoteServer"/>.
        /// </summary>
        /// <param name="disposing"><see langword="true"/> to dispose of managed and 
        /// unmanaged resources; <see langword="false"/> to dispose of only unmanaged 
        /// resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.StopListening();
                this.listener.Close();
            }
        }
        #endregion

        /// <summary>
        /// Raises the ShutdownRequested event.
        /// </summary>
        /// <param name="e">An <see cref="EventArgs"/> object describing the event.</param>
        protected void OnShutdownRequested(EventArgs e)
        {
            if (this.ShutdownRequested != null)
            {
                this.ShutdownRequested(this, e);
            }
        }

        #region Private support methods
        private static List<IPAddress> GetIPAddressList()
        {
            List<IPAddress> addresses = new List<IPAddress>();

            // Obtain a reference to all network interfaces in the machine
            NetworkInterface[] adapters = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface adapter in adapters)
            {
                IPInterfaceProperties properties = adapter.GetIPProperties();
                foreach (IPAddressInformation uniCast in properties.UnicastAddresses)
                {
                    // Ignoring IPv6 addresses and loopback addresses.
                    // TODO: Make the address configurable.
                    if (uniCast.Address.AddressFamily == AddressFamily.InterNetwork && !IPAddress.IsLoopback(uniCast.Address))
                    {
                        addresses.Add(uniCast.Address);
                    }
                }
            }

            return addresses;
        }

        private static ErrorResponse CreateErrorResponse(string commandName, Exception ex)
        {
            List<StackTraceElement> stackTraceElementList = new List<StackTraceElement>();
            StackTrace trace = new StackTrace(ex, true);
            StackFrame[] frames = trace.GetFrames();
            foreach (StackFrame frame in frames)
            {
                Dictionary<string, object> stackElements = new Dictionary<string, object>();
                stackElements.Add("methodName", frame.GetMethod().Name);
                stackElements.Add("className", frame.GetMethod().ReflectedType.ToString());

                string fileName = frame.GetFileName();
                if (string.IsNullOrEmpty(fileName))
                {
                    fileName = string.Empty;
                }

                stackElements.Add("fileName", fileName);
                stackElements.Add("lineNumber", frame.GetFileLineNumber());
                stackTraceElementList.Add(new StackTraceElement(stackElements));
            }

            ErrorResponse error = new ErrorResponse();
            error.Message = commandName.ToString() + ": " + ex.Message;
            error.ClassName = ex.GetType().Name;
            error.StackTrace = stackTraceElementList.ToArray();
            error.Screenshot = string.Empty;
            return error;
        }

        private void ConstructDispatcherTables(string prefix)
        {
            this.getDispatcherTable = new UriTemplateTable(new Uri(prefix.Replace("*", "localhost")));
            this.postDispatcherTable = new UriTemplateTable(new Uri(prefix.Replace("*", "localhost")));
            this.deleteDispatcherTable = new UriTemplateTable(new Uri(prefix.Replace("*", "localhost")));

            // DriverCommand is a static class with static fields containing the command names.
            // Since this is initialization code only, we'll take the perf hit in using reflection.
            FieldInfo[] fields = typeof(DriverCommand).GetFields(BindingFlags.Public | BindingFlags.Static);
            foreach (FieldInfo field in fields)
            {
                string commandName = field.GetValue(null).ToString();
                CommandInfo commandInformation = CommandInfoRepository.Instance.GetCommandInfo(commandName);
                UriTemplate commandUriTemplate = new UriTemplate(commandInformation.ResourcePath);
                if (!this.handlerFactory.CanCreateHandler(commandName))
                {
                    this.serverLogger.Log("No command handler implemented for " + commandName.ToString(), LogLevel.Warning);
                }

                UriTemplateTable templateTable = this.FindDispatcherTable(commandInformation.Method);
                templateTable.KeyValuePairs.Add(new KeyValuePair<UriTemplate, object>(commandUriTemplate, commandName));
            }

            this.getDispatcherTable.MakeReadOnly(false);
            this.postDispatcherTable.MakeReadOnly(false);
            this.deleteDispatcherTable.MakeReadOnly(false);
        }

        private UriTemplateTable FindDispatcherTable(string httpMethod)
        {
            UriTemplateTable tableToReturn = null;
            switch (httpMethod)
            {
                case CommandInfo.GetCommand:
                    tableToReturn = this.getDispatcherTable;
                    break;

                case CommandInfo.PostCommand:
                    tableToReturn = this.postDispatcherTable;
                    break;

                case CommandInfo.DeleteCommand:
                    tableToReturn = this.deleteDispatcherTable;
                    break;
            }

            return tableToReturn;
        }

        private void OnClientConnect(IAsyncResult result)
        {
            try
            {
                HttpListener localListener = (HttpListener)result.AsyncState;

                // Call EndGetContext to complete the asynchronous operation.
                HttpListenerContext context = localListener.EndGetContext(result);
                this.listener.BeginGetContext(new AsyncCallback(this.OnClientConnect), localListener);

                this.ProcessContext(context);
            }
            catch (ObjectDisposedException)
            {
                System.Diagnostics.Debugger.Log(0, "1", "\n OnClientConnection: Socket has been closed\n");
            }
            catch (SocketException se)
            {
                this.serverLogger.Log("SocketException:" + se.Message, LogLevel.Error);
            }
            catch (HttpListenerException hle)
            {
                // When we shut the HttpListener down, there will always still be
                // a thread pending listening for a request. If there is no client
                // connected, we may have a real problem here.
                if (!this.listenerStopping)
                {
                    Console.WriteLine("HttpListenerException:" + hle.Message);
                }
            }
        }

        private void ProcessContext(HttpListenerContext context)
        {
            HttpListenerRequest request = context.Request;
            string httpMethod = request.HttpMethod;
            string requestBody = string.Empty;
            if (string.Compare(httpMethod, "POST", StringComparison.OrdinalIgnoreCase) == 0)
            {
                int totalBytesRead = 0;
                byte[] bodyData = new byte[request.ContentLength64];
                while (totalBytesRead < request.ContentLength64)
                {
                    totalBytesRead += request.InputStream.Read(bodyData, totalBytesRead, (int)request.ContentLength64 - totalBytesRead);
                }

                requestBody = Encoding.UTF8.GetString(bodyData);
            }

            // Obtain a response object.
            HttpListenerResponse response = context.Response;
            ServerResponse result = this.DispatchRequest(request.Url, httpMethod, requestBody);
            if (result.StatusCode == HttpStatusCode.SeeOther)
            {
                response.AddHeader("Location", request.Url.AbsoluteUri + "/" + result.ReturnedResponse.Value.ToString());
                result.ReturnedResponse.Value = string.Empty;
            }
            
            string responseString = result.ReturnedResponse.ToJson();
            
            // Construct a response.
            byte[] buffer = Encoding.UTF8.GetBytes(responseString);
            response.ContentType = result.ContentType;
            response.StatusCode = (int)result.StatusCode;
            response.StatusDescription = result.StatusCode.ToString();

            // Get a response stream and write the response to it.
            response.ContentLength64 = buffer.Length;
            Stream output = response.OutputStream;
            output.Write(buffer, 0, buffer.Length);
            
            // We must close the output stream.
            output.Close();

            if (request.Url.AbsolutePath.ToUpperInvariant().Contains(ShutdownUrlFragment))
            {
                this.OnShutdownRequested(new EventArgs());
            }
        }

        private ServerResponse DispatchRequest(Uri resourcePath, string httpMethod, string requestBody)
        {
            HttpStatusCode codeToReturn = HttpStatusCode.OK;
            Response commandResponse = new Response();
            Dictionary<string, string> locatorParameters = new Dictionary<string, string>();
            UriTemplateTable templateTable = this.FindDispatcherTable(httpMethod);
            UriTemplateMatch match = templateTable.MatchSingle(resourcePath);
            if (resourcePath.AbsolutePath.ToUpperInvariant().Contains(ShutdownUrlFragment))
            {
                this.serverLogger.Log("Executing: [Shutdown] at URL: " + resourcePath.AbsolutePath);
            }
            else if (match == null)
            {
                codeToReturn = HttpStatusCode.NotFound;
                commandResponse.Value = "No command associated with " + resourcePath.AbsolutePath;
            }
            else
            {
                string relativeUrl = match.RequestUri.AbsoluteUri.Substring(match.RequestUri.AbsoluteUri.IndexOf(this.listenerPath, StringComparison.OrdinalIgnoreCase) + this.listenerPath.Length - 1);
                SessionId sessionIdValue = null;

                string commandName = (string)match.Data;
                foreach (string key in match.BoundVariables.Keys)
                {
                    string value = match.BoundVariables[key];
                    locatorParameters.Add(key, value);
                }

                Command commandToExecute = new Command(commandName, requestBody);

                object resultValue = null;
                WebDriverResult resultCode = WebDriverResult.Success;
                try
                {
                    CommandHandler handler = this.handlerFactory.CreateHandler(commandToExecute.Name, locatorParameters, commandToExecute.Parameters);
                    sessionIdValue = handler.SessionId;
                    this.serverLogger.Log("Executing: " + handler.ToString() + " at URL: " + relativeUrl);
                    resultValue = handler.Execute();
                    codeToReturn = handler.StatusCode;
                }
                catch (CommandNotImplementedException ex)
                {
                    codeToReturn = HttpStatusCode.NotImplemented;
                    resultValue = ex.Message + " (" + commandName + ")";
                }
                catch (InvalidCommandException ex)
                {
                    codeToReturn = HttpStatusCode.MethodNotAllowed;
                    resultValue = ex.Message;
                }
                catch (ResourceNotFoundException ex)
                {
                    codeToReturn = HttpStatusCode.NotFound;
                    resultValue = ex.Message + " (" + resourcePath.AbsolutePath + ")";
                }
                catch (InvalidParameterException ex)
                {
                    codeToReturn = HttpStatusCode.BadRequest;
                    resultValue = ex.Message;
                }
                catch (NoSuchWindowException ex)
                {
                    resultCode = WebDriverResult.NoSuchWindow;
                    resultValue = CreateErrorResponse(commandName, ex);
                }
                catch (NoSuchElementException ex)
                {
                    resultCode = WebDriverResult.NoSuchElement;
                    resultValue = CreateErrorResponse(commandName, ex);
                }
                catch (NoSuchFrameException ex)
                {
                    resultCode = WebDriverResult.NoSuchFrame;
                    resultValue = CreateErrorResponse(commandName, ex);
                }
                catch (StaleElementReferenceException ex)
                {
                    resultCode = WebDriverResult.ObsoleteElement;
                    resultValue = CreateErrorResponse(commandName, ex);
                }
                catch (ElementNotVisibleException ex)
                {
                    resultCode = WebDriverResult.ElementNotDisplayed;
                    resultValue = CreateErrorResponse(commandName, ex);
                }
                catch (InvalidElementStateException ex)
                {
                    resultCode = WebDriverResult.InvalidElementState;
                    resultValue = CreateErrorResponse(commandName, ex);
                }
                catch (NotImplementedException ex)
                {
                    resultCode = WebDriverResult.InvalidElementState;
                    resultValue = CreateErrorResponse(commandName, ex);
                }
                catch (XPathLookupException ex)
                {
                    resultCode = WebDriverResult.XPathLookupError;
                    resultValue = CreateErrorResponse(commandName, ex);
                }
                catch (WebDriverTimeoutException ex)
                {
                    resultCode = WebDriverResult.Timeout;
                    resultValue = CreateErrorResponse(commandName, ex);
                }
                catch (UnhandledAlertException ex)
                {
                    resultCode = WebDriverResult.UnexpectedAlertOpen;

                    // This is ridiculously ugly. To be refactored.
                    ErrorResponse err = CreateErrorResponse(commandName, ex);
                    Dictionary<string, object> resp = new Dictionary<string, object>();
                    resp["class"] = err.ClassName;
                    resp["message"] = err.Message;
                    resp["screen"] = err.Screenshot;
                    resp["stackTrace"] = err.StackTrace;
                    Dictionary<string, object> alert = new Dictionary<string, object>();
                    alert["text"] = ex.Alert.Text;
                    resp["alert"] = alert;
                    resultValue = resp;
                }
                catch (Exception ex)
                {
                    resultCode = WebDriverResult.UnhandledError;
                    resultValue = CreateErrorResponse(commandName, ex);
                }

                commandResponse = new Response(sessionIdValue);
                commandResponse.Status = resultCode;
                commandResponse.Value = resultValue;

                this.serverLogger.Log("Done: " + relativeUrl);
            }

            return new ServerResponse(commandResponse, codeToReturn);
        }
        #endregion
    }
}
