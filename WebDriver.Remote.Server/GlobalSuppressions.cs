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

// This file is used by Code Analysis to maintain SuppressMessage 
// attributes that are applied to this project. 
// Project-level suppressions either have no target or are given 
// a specific target and scoped to a namespace, type, member, etc. 
//
// To add a suppression to this file, right-click the message in the 
// Error List, point to "Suppress Message(s)", and click 
// "In Project Suppression File". 
// You do not need to add suppressions to this file manually. 
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace", Target = "OpenQA.Selenium.Remote.Server.Loggers", Justification = "Loggers namespace should be separate, but will contain few types.")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Scope = "member", Target = "OpenQA.Selenium.Remote.Server.RemoteServer.#DispatchRequest(System.Uri,System.String,System.String)", Justification = "DispatchRequest passes all exceptions back to the caller via JSON, so catching the base Exception here is okay.")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "parameters", Scope = "member", Target = "OpenQA.Selenium.Remote.Server.CommandHandler.#.ctor(System.Collections.Generic.Dictionary`2<System.String,System.String>,System.Collections.Generic.Dictionary`2<System.String,System.Object>)", Justification = "Parameter not used by base class, but is used extensively by subclasses.")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "locatorParameters", Scope = "member", Target = "OpenQA.Selenium.Remote.Server.CommandHandler.#.ctor(System.Collections.Generic.Dictionary`2<System.String,System.String>,System.Collections.Generic.Dictionary`2<System.String,System.Object>)", Justification = "Parameter not used by base class, but is used extensively by subclasses.")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Scope = "member", Target = "OpenQA.Selenium.Remote.Server.CommandHandlerFactory.#AddHandlers()", Justification = "It's a factory method. Of course it's coupled to a large number of classes.")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Scope = "member", Target = "OpenQA.Selenium.Remote.Server.RemoteServer.#DispatchRequest(System.Uri,System.String,System.String)", Justification = "This method has to interpret all of the given exception types differently, thus the high coupling.")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2001:AvoidCallingProblematicMethods", MessageId = "System.Reflection.Assembly.LoadFile", Scope = "member", Target = "OpenQA.Selenium.Remote.Server.SessionManager.#CurrentDomain_AssemblyResolve(System.Object,System.ResolveEventArgs)", Justification="We must use Assembly.LoadFile to load the WebDriver assemblies from the subdirectory.")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands", Scope = "member", Target = "OpenQA.Selenium.Remote.Server.SessionManager.#.ctor()")]
