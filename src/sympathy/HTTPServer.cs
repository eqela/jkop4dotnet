
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
	public class HTTPServer : HTTPServerBase
	{
		private System.Func<HTTPServerRequest, HTTPServerResponse> createOptionsResponseHandler = null;
		private System.Collections.Generic.List<System.Action<HTTPServerRequest, HTTPServerResponse, int, string>> requestHandlerListenerFunctions = null;
		private System.Collections.Generic.List<HTTPServerRequestHandlerListener> requestHandlerListenerObjects = null;
		private HTTPServerRequestHandlerStack handlerStack = null;

		public HTTPServer() {
			handlerStack = new HTTPServerRequestHandlerStack();
		}

		public override bool initialize() {
			if(base.initialize() == false) {
				return(false);
			}
			handlerStack.initialize((HTTPServerBase)this);
			if(requestHandlerListenerObjects != null) {
				var n = 0;
				var m = requestHandlerListenerObjects.Count;
				for(n = 0 ; n < m ; n++) {
					var listener = requestHandlerListenerObjects[n];
					if(listener != null) {
						if(listener is HTTPServerComponent) {
							((HTTPServerComponent)listener).initialize((HTTPServerBase)this);
						}
					}
				}
			}
			return(true);
		}

		public override void onRefresh() {
			base.onRefresh();
			handlerStack.onRefresh();
			if(requestHandlerListenerObjects != null) {
				var n = 0;
				var m = requestHandlerListenerObjects.Count;
				for(n = 0 ; n < m ; n++) {
					var listener = requestHandlerListenerObjects[n];
					if(listener != null) {
						if(listener is HTTPServerComponent) {
							((HTTPServerComponent)listener).onRefresh();
						}
					}
				}
			}
		}

		public override void onMaintenance() {
			base.onMaintenance();
			handlerStack.onMaintenance();
			if(requestHandlerListenerObjects != null) {
				var n = 0;
				var m = requestHandlerListenerObjects.Count;
				for(n = 0 ; n < m ; n++) {
					var listener = requestHandlerListenerObjects[n];
					if(listener != null) {
						if(listener is HTTPServerComponent) {
							((HTTPServerComponent)listener).onMaintenance();
						}
					}
				}
			}
		}

		public override void cleanup() {
			base.cleanup();
			handlerStack.cleanup();
			if(requestHandlerListenerObjects != null) {
				var n = 0;
				var m = requestHandlerListenerObjects.Count;
				for(n = 0 ; n < m ; n++) {
					var listener = requestHandlerListenerObjects[n];
					if(listener != null) {
						if(listener is HTTPServerComponent) {
							((HTTPServerComponent)listener).cleanup();
						}
					}
				}
			}
		}

		public void pushRequestHandler(System.Action<HTTPServerRequest, System.Action> handler) {
			handlerStack.pushRequestHandler(handler);
		}

		public void pushRequestHandler(HTTPServerRequestHandler handler) {
			handlerStack.pushRequestHandler(handler);
		}

		public void addRequestHandlerListener(System.Action<HTTPServerRequest, HTTPServerResponse, int, string> handler) {
			if(requestHandlerListenerFunctions == null) {
				requestHandlerListenerFunctions = new System.Collections.Generic.List<System.Action<HTTPServerRequest, HTTPServerResponse, int, string>>();
			}
			requestHandlerListenerFunctions.Add(handler);
		}

		public void addRequestHandlerListener(HTTPServerRequestHandlerListener handler) {
			if(requestHandlerListenerObjects == null) {
				requestHandlerListenerObjects = new System.Collections.Generic.List<HTTPServerRequestHandlerListener>();
			}
			requestHandlerListenerObjects.Add(handler);
			if((handler is HTTPServerComponent) && isInitialized()) {
				((HTTPServerComponent)handler).initialize((HTTPServerBase)this);
			}
		}

		public override HTTPServerResponse createOptionsResponse(HTTPServerRequest req) {
			if(createOptionsResponseHandler != null) {
				return(createOptionsResponseHandler(req));
			}
			return(base.createOptionsResponse(req));
		}

		public override void onRequest(HTTPServerRequest req) {
			var rq = req;
			handlerStack.handleRequest(req as HTTPServerRequest, () => {
				rq.sendResponse(sympathy.HTTPServerResponse.forHTTPNotFound());
			});
		}

		public override void onRequestComplete(HTTPServerRequest request, HTTPServerResponse resp, int bytesSent, string remoteAddress) {
			base.onRequestComplete(request, resp, bytesSent, remoteAddress);
			if(requestHandlerListenerFunctions != null) {
				var n = 0;
				var m = requestHandlerListenerFunctions.Count;
				for(n = 0 ; n < m ; n++) {
					var handler = requestHandlerListenerFunctions[n];
					if(handler != null) {
						handler(request, resp, bytesSent, remoteAddress);
					}
				}
			}
			if(requestHandlerListenerObjects != null) {
				var n2 = 0;
				var m2 = requestHandlerListenerObjects.Count;
				for(n2 = 0 ; n2 < m2 ; n2++) {
					var handler1 = requestHandlerListenerObjects[n2];
					if(handler1 != null) {
						handler1.onRequestHandled(request, resp, bytesSent, remoteAddress);
					}
				}
			}
		}

		public System.Func<HTTPServerRequest, HTTPServerResponse> getCreateOptionsResponseHandler() {
			return(createOptionsResponseHandler);
		}

		public HTTPServer setCreateOptionsResponseHandler(System.Func<HTTPServerRequest, HTTPServerResponse> v) {
			createOptionsResponseHandler = v;
			return(this);
		}
	}
}
