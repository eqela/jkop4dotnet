
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

namespace sympathy
{
	public class DirectoryLogContext : cape.LoggingContext
	{
		public static DirectoryLogContext create(cape.File logDir, string logIdPrefix = null, bool dbg = true) {
			var v = new DirectoryLogContext();
			v.setLogDir(logDir);
			v.setEnableDebugMessages(dbg);
			if(cape.String.isEmpty(logIdPrefix) == false) {
				v.setLogIdPrefix(logIdPrefix);
			}
			return(v);
		}

		private bool enableDebugMessages = true;
		private cape.File logDir = null;
		private string logIdPrefix = "messages";
		private cape.PrintWriter os = null;
		private string currentLogIdName = null;
		private bool alsoPrintOnConsole = false;

		public virtual void logError(string text) {
			message("ERROR", text);
		}

		public virtual void logWarning(string text) {
			message("WARNING", text);
		}

		public virtual void logInfo(string text) {
			message("INFO", text);
		}

		public virtual void logDebug(string text) {
			message("DEBUG", text);
		}

		public void message(string type, string text) {
			if((enableDebugMessages == false) && cape.String.equalsIgnoreCase("debug", type)) {
				return;
			}
			var dt = cape.DateTime.forNow();
			string logTime = null;
			if(dt != null) {
				logTime = ((((((((((cape.String.forInteger(dt.getYear()) + "-") + cape.String.forIntegerWithPadding(dt.getMonth(), 2)) + "-") + cape.String.forIntegerWithPadding(dt.getDayOfMonth(), 2)) + " ") + cape.String.forIntegerWithPadding(dt.getHours(), 2)) + ":") + cape.String.forIntegerWithPadding(dt.getMinutes(), 2)) + ":") + cape.String.forIntegerWithPadding(dt.getSeconds(), 2)) + " UTC";
			}
			else {
				logTime = "DATE/TIME";
			}
			var logLine = (((("[" + cape.String.padToLength(type, 7)) + "] [") + logTime) + "]: ") + text;
			if(logDir != null) {
				string logIdName = null;
				if(dt != null) {
					logIdName = ((((logIdPrefix + "_") + cape.String.forInteger(dt.getYear())) + cape.String.forIntegerWithPadding(dt.getMonth(), 2)) + cape.String.forIntegerWithPadding(dt.getDayOfMonth(), 2)) + ".log";
				}
				else {
					logIdName = logIdPrefix + ".log";
				}
				if((os == null) || !(object.Equals(currentLogIdName, logIdName))) {
					currentLogIdName = logIdName;
					os = cape.PrintWriterWrapper.forWriter((cape.Writer)logDir.entry(currentLogIdName).append());
					if((os == null) && (logDir.isDirectory() == false)) {
						logDir.createDirectoryRecursive();
						os = cape.PrintWriterWrapper.forWriter((cape.Writer)logDir.entry(currentLogIdName).append());
					}
				}
				if(os != null) {
					if(os.println(logLine) == false) {
						return;
					}
					if(os is cape.FlushableWriter) {
						((cape.FlushableWriter)os).flush();
					}
				}
			}
			if(alsoPrintOnConsole) {
				System.Console.WriteLine(logLine);
			}
		}

		~DirectoryLogContext() {
			if(os != null) {
				if(os is cape.Closable) {
					((cape.Closable)os).close();
				}
				os = null;
			}
		}

		public bool getEnableDebugMessages() {
			return(enableDebugMessages);
		}

		public DirectoryLogContext setEnableDebugMessages(bool v) {
			enableDebugMessages = v;
			return(this);
		}

		public cape.File getLogDir() {
			return(logDir);
		}

		public DirectoryLogContext setLogDir(cape.File v) {
			logDir = v;
			return(this);
		}

		public string getLogIdPrefix() {
			return(logIdPrefix);
		}

		public DirectoryLogContext setLogIdPrefix(string v) {
			logIdPrefix = v;
			return(this);
		}

		public bool getAlsoPrintOnConsole() {
			return(alsoPrintOnConsole);
		}

		public DirectoryLogContext setAlsoPrintOnConsole(bool v) {
			alsoPrintOnConsole = v;
			return(this);
		}
	}
}
