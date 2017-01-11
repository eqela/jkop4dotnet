
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
	public class HTTPServerBase : NetworkServer
	{
		private int writeBufferSize = 1024 * 512;
		private int smallBodyLimit = 32 * 1024;
		private int timeoutDelay = 30;
		private int maintenanceTimerDelay = 60;
		private string serverName = null;
		private bool enableCaching = true;
		private bool allowCORS = true;
		private ContentCache cache = null;
		private IOManagerTimer timeoutTimer = null;
		private IOManagerTimer maintenanceTimer = null;

		public HTTPServerBase() {
			setPort(8080);
			setServerName(("Jkop for .NET" + "/") + "1.0.20170111");
		}

		public override NetworkConnection createConnectionObject() {
			return((NetworkConnection)new HTTPServerConnection());
		}

		public virtual void onRefresh() {
		}

		public bool onTimeoutTimer() {
			var cfc = new System.Collections.Generic.List<HTTPServerConnection>();
			var now = cape.SystemClock.asSeconds();
			forEachConnection((NetworkConnection connection) => {
				var httpc = connection as HTTPServerConnection;
				if(httpc == null) {
					return;
				}
				if(((httpc.getResponses() >= httpc.getRequests()) || httpc.getIsWaitingForBodyReceiver()) && ((now - httpc.getLastActivity()) >= timeoutDelay)) {
					cfc.Add(httpc);
				}
			});
			if(cfc != null) {
				var n = 0;
				var m = cfc.Count;
				for(n = 0 ; n < m ; n++) {
					var wsc = cfc[n];
					if(wsc != null) {
						wsc.close();
					}
				}
			}
			return(true);
		}

		public bool onMaintenanceTimer() {
			if(cache != null) {
				cache.onMaintenance();
			}
			onMaintenance();
			return(true);
		}

		public virtual void onMaintenance() {
		}

		public IOManagerTimer startTimer(long delay, System.Func<bool> handler) {
			if(ioManager == null) {
				return(null);
			}
			return(ioManager.startTimer(delay, handler));
		}

		public override bool initialize() {
			if(base.initialize() == false) {
				return(false);
			}
			if(timeoutDelay < 1) {
				cape.Log.debug(logContext, "HTTPServerBase" + ": Timeout timer disabled");
			}
			else {
				cape.Log.debug(logContext, (("HTTPServerBase" + ": Starting a timeout timer with a ") + cape.String.forInteger(timeoutDelay)) + " second delay.");
				timeoutTimer = ioManager.startTimer(((long)timeoutDelay) * 1000000, onTimeoutTimer);
				if(timeoutTimer == null) {
					cape.Log.error(logContext, "HTTPServerBase" + ": Failed to start timeout timer");
				}
			}
			if(maintenanceTimerDelay < 1) {
				cape.Log.debug(logContext, "Maintenance timer disabled");
			}
			else {
				cape.Log.debug(logContext, (("HTTPServerBase" + ": Starting a maintenance timer with a ") + cape.String.forInteger(maintenanceTimerDelay)) + " second delay.");
				maintenanceTimer = ioManager.startTimer(((long)maintenanceTimerDelay) * 1000000, onMaintenanceTimer);
				if(maintenanceTimer == null) {
					cape.Log.error(logContext, "HTTPServerBase" + ": Failed to start maintenance timer");
				}
			}
			cape.Log.info(logContext, (("HTTPServerBase" + ": initialized: `") + getServerName()) + "'");
			return(true);
		}

		public override void cleanup() {
			base.cleanup();
			if(maintenanceTimer != null) {
				maintenanceTimer.stop();
				maintenanceTimer = null;
			}
			if(timeoutTimer != null) {
				timeoutTimer.stop();
				timeoutTimer = null;
			}
		}

		public virtual HTTPServerResponse createOptionsResponse(HTTPServerRequest req) {
			return(new HTTPServerResponse().setStatus("200").addHeader("Content-Length", "0"));
		}

		public virtual void onRequest(HTTPServerRequest req) {
			req.sendResponse(sympathy.HTTPServerResponse.forHTTPNotFound());
		}

		public void handleIncomingRequest(HTTPServerRequest req) {
			if(req == null) {
				return;
			}
			if(cache != null) {
				var cid = req.getCacheId();
				if(!(object.Equals(cid, null))) {
					var resp = cache.get(cid) as HTTPServerResponse;
					if(resp != null) {
						req.sendResponse(resp);
						return;
					}
				}
			}
			if(object.Equals(req.getMethod(), "OPTIONS")) {
				var resp1 = createOptionsResponse(req);
				if(resp1 != null) {
					req.sendResponse(resp1);
					return;
				}
			}
			onRequest(req);
		}

		public void sendResponse(HTTPServerConnection connection, HTTPServerRequest req, HTTPServerResponse resp) {
			if(connection == null) {
				return;
			}
			if(allowCORS) {
				resp.enableCORS(req);
			}
			if(enableCaching && (resp.getCacheTtl() > 0)) {
				var cid = req.getCacheId();
				if(!(object.Equals(cid, null))) {
					if(cache == null) {
						cache = new ContentCache();
					}
					cache.set(cid, (object)resp, resp.getCacheTtl());
				}
			}
			connection.sendResponse(req, resp);
		}

		public virtual void onRequestComplete(HTTPServerRequest request, HTTPServerResponse resp, int bytesSent, string remoteAddress) {
		}

		public int getWriteBufferSize() {
			return(writeBufferSize);
		}

		public HTTPServerBase setWriteBufferSize(int v) {
			writeBufferSize = v;
			return(this);
		}

		public int getSmallBodyLimit() {
			return(smallBodyLimit);
		}

		public HTTPServerBase setSmallBodyLimit(int v) {
			smallBodyLimit = v;
			return(this);
		}

		public int getTimeoutDelay() {
			return(timeoutDelay);
		}

		public HTTPServerBase setTimeoutDelay(int v) {
			timeoutDelay = v;
			return(this);
		}

		public int getMaintenanceTimerDelay() {
			return(maintenanceTimerDelay);
		}

		public HTTPServerBase setMaintenanceTimerDelay(int v) {
			maintenanceTimerDelay = v;
			return(this);
		}

		public string getServerName() {
			return(serverName);
		}

		public HTTPServerBase setServerName(string v) {
			serverName = v;
			return(this);
		}

		public bool getEnableCaching() {
			return(enableCaching);
		}

		public HTTPServerBase setEnableCaching(bool v) {
			enableCaching = v;
			return(this);
		}

		public bool getAllowCORS() {
			return(allowCORS);
		}

		public HTTPServerBase setAllowCORS(bool v) {
			allowCORS = v;
			return(this);
		}
	}
}
