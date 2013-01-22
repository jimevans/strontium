Strontium
=========
A .NET implementation of a Selenium WebDriver Remote Server

The Strontium project is a .NET implementation of the Selenium WebDriver Remote Server.
This means that it starts an HTTP server that responds to the [WebDriver JSON Wire Protocol]
(http://code.google.com/p/selenium/wiki/JsonWireProtocol). It is intended to be a drop-in
replacement for the selenium-server-standalone.jar for WebDriver code. This allows you to 
execute WebDriver commands on a remote computer using only the .NET Framework.

Usage
-----
Using Strontium is very easy. Either download a binary .zip package, or build the project
from source, and deploy the binaries into a folder on your remote computer and launch
StrontiumServer.exe. This will then allow you to use the following WebDriver code:

```csharp
     DesiredCapabilities capabilities = DesiredCapabilities.Firefox();
     IWebDriver driver = new RemoteWebDriver("http://<your remote machine>:<port>/wd/hub",
                                             capabilities);
     driver.Url = "http://www.google.com";
     driver.Quit();
```

The Strontium server will launch Firefox on the remote machine, navigate to Google, and
shut the browser down.

Command-line arguments
----------------------
The StrontiumServer.exe supports several command-line arguments to control its behavior.

<table>
<tr><th>Argument</th><th>Description</th></tr>
<tr><td>/port:&lt;portNumber&gt;</td><td>Specifies the port on which the Strontium HTTP server will listen for commands. The default is 4444.</td></tr>
<tr><td>/loglevel:&lt;loggingLevel&gt;</td><td>Specifies the logging level for the server. Valid values (from most verbose to least) are Debug, Info, Warning, Error, and None. The default is `Info`.</td></tr>
<tr><td>/hub:http://&lt;hubServer&gt;:&lt;hubPort&gt;</td><td>Specifies the location and port for a Selenium Grid hub.</td></tr>
<tr><td>/remoteshutdown:ignore</td><td>Specifies that the Strontium server should ignore remote requests for it to shut down.</td></tr>
</table>

Prerequisites
-------------
The Strontium server requires the .NET Framework version 4.0 to run. For developing on
Strontium, a Microsoft Visual Studio 2010 solution and project files are provided. The
server depends on the .NET bindings of the Selenium WebDriver project.

A word about NuGet
------------------
There is no NuGet package provided for the Strontium project, and there is unlikely to
be. NuGet is a packaging system designed to keep project dependencies for code development
up to date. The Strontium server is a standalone executable, and it's highly unlikely
that there will ever be a need to use it as a referenced assembly for another .NET project.
If you find this to not be the case, and you need to establish a reference to the assemblies
of the Strontium project, please contact us so we can better understand your use case.