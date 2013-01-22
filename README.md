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

Issues and Pull Requests
------------------------
Feel free to file issues here, but be aware that without a full reproducible case, they
will be summarily closed. A full reproducible case includes the WebDriver code you are
using with the Strontium server, as well as an [HTML page or URL to a public page]
(http://jimevansmusic.blogspot.com/2012/12/not-providing-html-page-is-bogus.html)
demonstrating the issue. Please remember that at present, there is only one developer
working on this project, and it is very much a side project. Your patience and cooperation
is greatly apprecitated.

If you get frustrated waiting for me to fix an issue that you've encountered with Strontium,
pull requests are always welcome. Please make sure you have run [FxCop] 
(http://www.microsoft.com/en-us/download/details.aspx?id=6544) and [StyleCop]
(http://stylecop.codeplex.com/) on your code and resolved all issues before submission.
StyleCop settings files with the proper style rules enabled are provided as part of the code
repository. 

A word about NuGet
------------------
There is no NuGet package provided for the Strontium project, and there is unlikely to
be. NuGet is a packaging system designed to keep project dependencies for code development
up to date. The Strontium server is a standalone executable, and it's highly unlikely
that there will ever be a need to use it as a referenced assembly for another .NET project.
If you find this to not be the case, and you need to establish a reference to the assemblies
of the Strontium project, please contact us so we can better understand your use case.
