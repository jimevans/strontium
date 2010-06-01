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
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Scope = "member", Target = "OpenQA.Selenium.Remote.Server.RemoteServer.#DispatchRequest(System.Uri,System.String,System.String)", Justification = "This method has to interpret all of the given exception types differently, thus the high coupling.")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors", Scope = "member", Target = "OpenQA.Selenium.Remote.Server.CommandHandlerFactory.#.ctor()", Justification = "It is intended that every subclass of CommandHandlerFactory override AddHandlers")]
