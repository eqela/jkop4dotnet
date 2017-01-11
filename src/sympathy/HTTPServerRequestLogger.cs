
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
	public class HTTPServerRequestLogger : HTTPServerRequestHandlerListener
	{
		private cape.File logdir = null;
		private cape.LoggingContext logContext = null;

		public virtual void onRequestHandled(HTTPServerRequest request, HTTPServerResponse resp, int written, string aremoteAddress) {
			var remoteAddress = aremoteAddress;
			if(cape.String.isEmpty(remoteAddress)) {
				remoteAddress = "-";
			}
			string username = null;
			if(cape.String.isEmpty(username)) {
				username = "-";
			}
			string sessionid = null;
			if(cape.String.isEmpty(sessionid)) {
				sessionid = "-";
			}
			var dt = cape.DateTime.forNow();
			string logTime = null;
			if(dt != null) {
				logTime = ((((((((((cape.String.forInteger(dt.getDayOfMonth()) + "/") + cape.String.forInteger(dt.getMonth())) + "/") + cape.String.forInteger(dt.getYear())) + "/") + cape.String.forInteger(dt.getHours())) + "/") + cape.String.forInteger(dt.getMinutes())) + "/") + cape.String.forInteger(dt.getSeconds())) + " UTC";
			}
			else {
				logTime = "[DATE/TIME]";
			}
			var rf = request.getHeader("referer");
			if(cape.String.isEmpty(rf)) {
				rf = "-";
			}
			var logLine = ((((((((((((((((((((remoteAddress + " ") + username) + " ") + sessionid) + " [") + logTime) + "] \"") + request.getMethod()) + " ") + request.getURLPath()) + " ") + request.getVersion()) + "\" ") + resp.getStatus()) + " ") + cape.String.forInteger(written)) + " \"") + rf) + "\" \"") + request.getHeader("user-agent")) + "\"";
			if(logdir != null) {
				string logidname = null;
				if(dt != null) {
					logidname = ((("accesslog_" + cape.String.forInteger(dt.getYear())) + cape.String.forInteger(dt.getMonth())) + cape.String.forInteger(dt.getDayOfMonth())) + ".log";
				}
				else {
					logidname = "accesslog.log";
				}
				var os = cape.PrintWriterWrapper.forWriter((cape.Writer)logdir.entry(logidname).append());
				if((os == null) && (logdir.isDirectory() == false)) {
					logdir.createDirectoryRecursive();
					os = cape.PrintWriterWrapper.forWriter((cape.Writer)logdir.entry(logidname).append());
				}
				if(os != null) {
					os.println(logLine);
				}
				cape.Log.debug(logContext, logLine);
			}
			else if(logContext != null) {
				cape.Log.info(logContext, logLine);
			}
			else {
				System.Console.WriteLine(logLine);
			}
		}

		public cape.File getLogdir() {
			return(logdir);
		}

		public HTTPServerRequestLogger setLogdir(cape.File v) {
			logdir = v;
			return(this);
		}

		public cape.LoggingContext getLogContext() {
			return(logContext);
		}

		public HTTPServerRequestLogger setLogContext(cape.LoggingContext v) {
			logContext = v;
			return(this);
		}
	}
}
