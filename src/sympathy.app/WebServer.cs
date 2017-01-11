
/*
 * This file is part of Jkop for .NET
 * Copyright (c) 2016-2017 Job and Esther Technologies, Inc.
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

namespace sympathy.app
{
	public class WebServer
	{
		private class MyHTTPServer : HTTPServer
		{
			private WebServer myParent = null;

			public override void onMaintenance() {
				base.onMaintenance();
				myParent.onMaintenance();
			}

			public WebServer getMyParent() {
				return(myParent);
			}

			public MyHTTPServer setMyParent(WebServer v) {
				myParent = v;
				return(this);
			}
		}

		private bool enableRequestLogging = true;
		private int listenPort = 8080;
		private cape.File loggingDirectory = null;
		private cape.File configFile = null;
		private bool enableDebugMessages = false;
		private int maintenanceTimerDelay = 60;
		private string enableRateLimit = null;

		public cape.File getConfigFileDirectory() {
			if(configFile == null) {
				return(null);
			}
			return(configFile.getParent());
		}

		public virtual void onMaintenance() {
		}

		public virtual bool configure(string key, string value, cape.File relativeTo, cape.Error error) {
			if(object.Equals(key, "enableRateLimit")) {
				enableRateLimit = value;
				return(true);
			}
			if(object.Equals(key, "maintenanceTimerDelay")) {
				maintenanceTimerDelay = cape.Integer.asInteger(value);
				return(true);
			}
			if(object.Equals(key, "listenPort")) {
				listenPort = cape.Integer.asInteger(value);
				return(true);
			}
			if(object.Equals(key, "enableRequestLogging")) {
				enableRequestLogging = cape.Boolean.asBoolean((object)value);
				return(true);
			}
			if(object.Equals(key, "loggingDirectory")) {
				loggingDirectory = cape.FileInstance.forRelativePath(value, relativeTo);
				return(true);
			}
			if(object.Equals(key, "enableDebugMessages")) {
				enableDebugMessages = cape.Boolean.asBoolean((object)value);
				return(true);
			}
			return(false);
		}

		private bool doConfigure(cape.LoggingContext ctx, string key, string value, cape.File relativeTo) {
			var error = new cape.Error();
			if(configure(key, value, relativeTo, error) == false) {
				cape.Log.error(ctx, error.toStringWithDefault(("Unsupported configuration option: `" + key) + "'"));
				return(false);
			}
			return(true);
		}

		public virtual bool initialize(cape.LoggingContext ctx, HTTPServer server) {
			if(!(object.Equals(enableRateLimit, null))) {
				var limit = new HTTPServerRateLimitHandler();
				if((object.Equals(enableRateLimit, "true")) || (object.Equals(enableRateLimit, "yes"))) {
					;
				}
				else {
					var comps = cape.String.split(enableRateLimit, ':', 3);
					limit.setCountLimit(cape.Integer.asInteger(cape.Vector.get(comps, 0)));
					limit.setCountDuration(cape.Integer.asInteger(cape.Vector.get(comps, 1)));
					limit.setIgnoreDuration(cape.Integer.asInteger(cape.Vector.get(comps, 2)));
				}
				server.pushRequestHandler((HTTPServerRequestHandler)limit);
			}
			return(true);
		}

		public virtual void printUsage(cape.PrintWriter stdout) {
			var flags = new string[][] {
				new string[] {
					"v|version|vv",
					"Display version information"
				},
				new string[] {
					"h|help",
					"Usage information"
				}
			};
			var options = new string[][] {
				new string[] {
					"config",
					"Specify a configuration file to use"
				},
				new string[] {
					"O<option>",
					"Manually specify configuration parameters (key/value pairs)"
				}
			};
			capex.console.ConsoleApplication.printUsage(stdout, null, null, flags, options);
		}

		public virtual void printHeader(cape.PrintWriter stdout) {
		}

		public virtual void printVersion(cape.PrintWriter stdout, bool longFormat) {
		}

		public bool execute(string[] args) {
			var stdout = capex.console.Stdout.instance();
			var arguments = capex.console.ConsoleApplication.parseCommandLineArguments(args);
			if(arguments != null) {
				var n = 0;
				var m = arguments.Count;
				for(n = 0 ; n < m ; n++) {
					var arg = arguments[n];
					if(arg != null) {
						if(arg.isFlag("v") || arg.isFlag("version")) {
							printVersion(stdout, false);
							return(true);
						}
						else if(arg.isFlag("vv")) {
							printVersion(stdout, true);
							return(true);
						}
					}
				}
			}
			printHeader(stdout);
			var ctx = (cape.LoggingContext)new capex.console.ConsoleApplicationContext();
			var showHelp = false;
			var options = new cape.KeyValueListForStrings();
			if(arguments != null) {
				var n2 = 0;
				var m2 = arguments.Count;
				for(n2 = 0 ; n2 < m2 ; n2++) {
					var arg1 = arguments[n2];
					if(arg1 != null) {
						if(arg1.isOption("config")) {
							if(configFile != null) {
								cape.Log.error(ctx, ("Duplicate config file parameter supplied: `" + arg1.getComplete()) + "'");
								return(false);
							}
							configFile = cape.FileInstance.forPath(arg1.getStringValue());
						}
						else if(arg1.isFlag("help") || arg1.isFlag("h")) {
							showHelp = true;
						}
						else if(!(object.Equals(arg1.key, null)) && cape.String.startsWith(arg1.key, "O")) {
							var key = cape.String.getSubString(arg1.key, 1);
							if(cape.String.isEmpty(key)) {
								cape.Log.error(ctx, ("Invalid option parameter: `" + arg1.getComplete()) + "'");
								return(false);
							}
							options.add(key, arg1.getStringValue());
						}
						else {
							arg1.reportAsUnsupported(ctx);
							return(false);
						}
					}
				}
			}
			if(showHelp) {
				printUsage(stdout);
				return(true);
			}
			if(configFile != null) {
				var config = capex.SimpleConfigFile.forFile(configFile);
				if(config == null) {
					cape.Log.error(ctx, ("Failed to read configuration file: `" + configFile.getPath()) + "'");
					return(false);
				}
				var it = config.iterate();
				while(it != null) {
					var kvp = it.next();
					if(kvp == null) {
						break;
					}
					var key1 = kvp.key;
					var value = kvp.value;
					if(doConfigure(ctx, key1, value, null) == false) {
						return(false);
					}
				}
			}
			cape.Iterator<cape.KeyValuePair<string, string>> oit = options.iterate();
			while(oit != null) {
				var kvp1 = oit.next();
				if(kvp1 == null) {
					break;
				}
				var key2 = kvp1.key;
				var value1 = kvp1.value;
				if(doConfigure(ctx, key2, value1, null) == false) {
					return(false);
				}
			}
			if(loggingDirectory != null) {
				cape.Log.debug(ctx, ("Configuring logging to directory: `" + loggingDirectory.getPath()) + "'");
				var dlc = sympathy.DirectoryLogContext.create(loggingDirectory);
				dlc.setEnableDebugMessages(enableDebugMessages);
				ctx = (cape.LoggingContext)dlc;
			}
			else {
				((capex.console.ConsoleApplicationContext)ctx).setEnableDebugMessages(enableDebugMessages);
			}
			var server = new MyHTTPServer();
			server.setMyParent(this);
			server.setPort(listenPort);
			if(initialize(ctx, (HTTPServer)server) == false) {
				cape.Log.error(ctx, "Failed to configure HTTP server");
				return(false);
			}
			if(enableRequestLogging) {
				server.addRequestHandlerListener((HTTPServerRequestHandlerListener)new HTTPServerRequestLogger().setLogContext(ctx).setLogdir(loggingDirectory));
			}
			var ioManager = sympathy.IOManager.createDefault();
			if(ioManager == null) {
				cape.Log.error(ctx, "Failed to create IO manager");
				return(false);
			}
			if(server.initialize(ioManager, ctx) == false) {
				cape.Log.error(ctx, "Failed to initialize HTTP server.");
				return(false);
			}
			if(ioManager.execute(ctx) == false) {
				cape.Log.error(ctx, "Failed to execute IO manager");
				return(false);
			}
			cape.Log.info(ctx, "Exiting normally.");
			return(true);
		}

		public bool getEnableRequestLogging() {
			return(enableRequestLogging);
		}

		public WebServer setEnableRequestLogging(bool v) {
			enableRequestLogging = v;
			return(this);
		}

		public int getListenPort() {
			return(listenPort);
		}

		public WebServer setListenPort(int v) {
			listenPort = v;
			return(this);
		}

		public cape.File getLoggingDirectory() {
			return(loggingDirectory);
		}

		public WebServer setLoggingDirectory(cape.File v) {
			loggingDirectory = v;
			return(this);
		}

		public cape.File getConfigFile() {
			return(configFile);
		}

		public WebServer setConfigFile(cape.File v) {
			configFile = v;
			return(this);
		}

		public bool getEnableDebugMessages() {
			return(enableDebugMessages);
		}

		public WebServer setEnableDebugMessages(bool v) {
			enableDebugMessages = v;
			return(this);
		}

		public int getMaintenanceTimerDelay() {
			return(maintenanceTimerDelay);
		}

		public WebServer setMaintenanceTimerDelay(int v) {
			maintenanceTimerDelay = v;
			return(this);
		}

		public string getEnableRateLimit() {
			return(enableRateLimit);
		}

		public WebServer setEnableRateLimit(string v) {
			enableRateLimit = v;
			return(this);
		}
	}
}
