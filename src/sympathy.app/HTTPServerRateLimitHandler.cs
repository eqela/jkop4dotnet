
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
	public class HTTPServerRateLimitHandler : HTTPServerRequestHandler, HTTPServerComponent
	{
		private class RequestCounter
		{
			private string ipAddress = null;
			private int startCountTimeStamp = 0;
			private int startIgnoreTimeStamp = 0;
			private int requestCount = 1;

			public int getCountAfterIncrement() {
				return(++requestCount);
			}

			public string getIpAddress() {
				return(ipAddress);
			}

			public RequestCounter setIpAddress(string v) {
				ipAddress = v;
				return(this);
			}

			public int getStartCountTimeStamp() {
				return(startCountTimeStamp);
			}

			public RequestCounter setStartCountTimeStamp(int v) {
				startCountTimeStamp = v;
				return(this);
			}

			public int getStartIgnoreTimeStamp() {
				return(startIgnoreTimeStamp);
			}

			public RequestCounter setStartIgnoreTimeStamp(int v) {
				startIgnoreTimeStamp = v;
				return(this);
			}
		}

		private int countLimit = 5;
		private int countDuration = 5;
		private int ignoreDuration = 60;
		private int maintenanceCleanupDelay = 60;
		private int lastCleanupTimeStamp = 0;
		private cape.DynamicMap addresses = null;

		public HTTPServerRateLimitHandler() {
			addresses = new cape.DynamicMap();
		}

		public virtual void initialize(HTTPServerBase server) {
		}

		public virtual void onMaintenance() {
			if(addresses == null) {
				return;
			}
			var now = cape.SystemClock.asSeconds();
			if((now - lastCleanupTimeStamp) < maintenanceCleanupDelay) {
				return;
			}
			lastCleanupTimeStamp = (int)now;
			var keys = new cape.DynamicVector();
			var itr = addresses.iterateValues();
			if(itr == null) {
				return;
			}
			while(true) {
				var rc = itr.next() as RequestCounter;
				if(rc == null) {
					break;
				}
				if((rc.getStartIgnoreTimeStamp() == 0) && ((now - rc.getStartCountTimeStamp()) > countDuration)) {
					keys.append((object)rc.getIpAddress());
				}
			}
			var array = keys.toVector();
			if(array != null) {
				var n = 0;
				var m = array.Count;
				for(n = 0 ; n < m ; n++) {
					var k = array[n] as string;
					if(k != null) {
						addresses.remove(k);
					}
				}
			}
		}

		public virtual void onRefresh() {
		}

		public virtual void cleanup() {
		}

		public virtual void handleRequest(HTTPServerRequest req, System.Action next) {
			if(isRequestRejected(req)) {
				req.sendJSONObject((object)sympathy.JSONResponse.forNotAllowed());
				return;
			}
			next();
		}

		private bool isRequestRejected(HTTPServerRequest req) {
			var connection = req.getConnection();
			if(connection == null) {
				return(true);
			}
			var socket = connection.getSocket();
			if(socket == null) {
				return(true);
			}
			string ipAddress = null;
			if(socket is TCPSocket) {
				ipAddress = ((TCPSocket)socket).getRemoteAddress();
			}
			else {
				ipAddress = req.getRemoteAddress();
			}
			if(cape.String.isEmpty(ipAddress)) {
				return(true);
			}
			var now = cape.SystemClock.asSeconds();
			var rc = addresses.get(ipAddress) as RequestCounter;
			if(rc == null) {
				addresses.set(ipAddress, (object)new RequestCounter().setIpAddress(ipAddress).setStartCountTimeStamp((int)now));
				return(false);
			}
			if(rc.getStartIgnoreTimeStamp() > 0) {
				if((now - rc.getStartIgnoreTimeStamp()) <= ignoreDuration) {
					return(true);
				}
				addresses.remove(ipAddress);
				return(false);
			}
			if((now - rc.getStartCountTimeStamp()) <= countDuration) {
				var count = rc.getCountAfterIncrement();
				if(count >= countLimit) {
					rc.setStartIgnoreTimeStamp((int)now);
					return(true);
				}
				return(false);
			}
			addresses.remove(ipAddress);
			return(false);
		}

		public int getCountLimit() {
			return(countLimit);
		}

		public HTTPServerRateLimitHandler setCountLimit(int v) {
			countLimit = v;
			return(this);
		}

		public int getCountDuration() {
			return(countDuration);
		}

		public HTTPServerRateLimitHandler setCountDuration(int v) {
			countDuration = v;
			return(this);
		}

		public int getIgnoreDuration() {
			return(ignoreDuration);
		}

		public HTTPServerRateLimitHandler setIgnoreDuration(int v) {
			ignoreDuration = v;
			return(this);
		}

		public int getMaintenanceCleanupDelay() {
			return(maintenanceCleanupDelay);
		}

		public HTTPServerRateLimitHandler setMaintenanceCleanupDelay(int v) {
			maintenanceCleanupDelay = v;
			return(this);
		}
	}
}
