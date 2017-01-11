
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

namespace capex.console
{
	public class ConsoleApplicationContext : cape.ApplicationContext, cape.LoggingContext
	{
		public static ConsoleApplicationContext forApplicationName(string name) {
			var v = new ConsoleApplicationContext();
			v.setApplicationName(name);
			return(v);
		}

		private string applicationName = null;
		private bool enableDebugMessages = false;
		private int errorCounter = 0;
		private int warningCounter = 0;
		private int infoCounter = 0;
		private int debugCounter = 0;

		public void resetCounters() {
			errorCounter = 0;
			warningCounter = 0;
			infoCounter = 0;
			debugCounter = 0;
		}

		public int getErrorCount() {
			return(errorCounter);
		}

		public int getWarningCount() {
			return(warningCounter);
		}

		public int getInfoCount() {
			return(infoCounter);
		}

		public int getDebugCount() {
			return(debugCounter);
		}

		public virtual void logError(string message) {
			System.Console.WriteLine("[ERROR] " + message);
			errorCounter++;
		}

		public virtual void logWarning(string message) {
			System.Console.WriteLine("[WARNING] " + message);
			warningCounter++;
		}

		public virtual void logInfo(string message) {
			System.Console.WriteLine("[INFO] " + message);
			infoCounter++;
		}

		public virtual void logDebug(string message) {
			if(enableDebugMessages) {
				System.Console.WriteLine("[DEBUG] " + message);
			}
			debugCounter++;
		}

		public virtual cape.File getApplicationDataDirectory() {
			var appName = applicationName;
			if(cape.String.isEmpty(appName)) {
				var cp = cape.CurrentProcess.getExecutableFile();
				if(cp != null) {
					appName = cp.baseNameWithoutExtension();
				}
			}
			if(cape.String.isEmpty(appName)) {
				return(null);
			}
			return(cape.FileInstance.forRelativePath("." + appName, cape.Environment.getHomeDirectory()));
		}

		public string getApplicationName() {
			return(applicationName);
		}

		public ConsoleApplicationContext setApplicationName(string v) {
			applicationName = v;
			return(this);
		}

		public bool getEnableDebugMessages() {
			return(enableDebugMessages);
		}

		public ConsoleApplicationContext setEnableDebugMessages(bool v) {
			enableDebugMessages = v;
			return(this);
		}
	}
}
