
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
	public class HTTPServerRequestHandlerMap : HTTPServerRequestHandlerAdapter
	{
		private System.Collections.Generic.Dictionary<string,System.Action<HTTPServerRequest>> getHandlerFunctions = null;
		private System.Collections.Generic.Dictionary<string,System.Action<HTTPServerRequest>> postHandlerFunctions = null;
		private System.Collections.Generic.Dictionary<string,System.Action<HTTPServerRequest>> putHandlerFunctions = null;
		private System.Collections.Generic.Dictionary<string,System.Action<HTTPServerRequest>> deleteHandlerFunctions = null;
		private System.Collections.Generic.Dictionary<string,System.Action<HTTPServerRequest>> patchHandlerFunctions = null;
		private System.Collections.Generic.Dictionary<string,HTTPServerRequestHandler> childObjects = null;

		public override void initialize(HTTPServerBase server) {
			base.initialize(server);
			cape.Iterator<HTTPServerRequestHandler> it = cape.Map.iterateValues(childObjects);
			while(true) {
				var child = it.next();
				if(child == null) {
					break;
				}
				if(child is HTTPServerComponent) {
					((HTTPServerComponent)child).initialize(server);
				}
			}
		}

		public override void onMaintenance() {
			base.onMaintenance();
			cape.Iterator<HTTPServerRequestHandler> it = cape.Map.iterateValues(childObjects);
			while(true) {
				var child = it.next();
				if(child == null) {
					break;
				}
				if(child is HTTPServerComponent) {
					((HTTPServerComponent)child).onMaintenance();
				}
			}
		}

		public override void onRefresh() {
			base.onRefresh();
			cape.Iterator<HTTPServerRequestHandler> it = cape.Map.iterateValues(childObjects);
			while(true) {
				var child = it.next();
				if(child == null) {
					break;
				}
				if(child is HTTPServerComponent) {
					((HTTPServerComponent)child).onRefresh();
				}
			}
		}

		public override void cleanup() {
			base.cleanup();
			cape.Iterator<HTTPServerRequestHandler> it = cape.Map.iterateValues(childObjects);
			while(true) {
				var child = it.next();
				if(child == null) {
					break;
				}
				if(child is HTTPServerComponent) {
					((HTTPServerComponent)child).cleanup();
				}
			}
		}

		public bool onHTTPMethod(HTTPServerRequest req, System.Collections.Generic.Dictionary<string,System.Action<HTTPServerRequest>> functions) {
			var rsc = req.peekResource();
			if(object.Equals(rsc, null)) {
				rsc = "";
			}
			System.Action<HTTPServerRequest> handler = null;
			var rsccount = req.getRemainingResourceCount();
			if(rsccount < 1) {
				handler = cape.Map.get(functions, rsc);
			}
			else if(rsccount == 1) {
				handler = cape.Map.get(functions, rsc + "/*");
				if(handler == null) {
					handler = cape.Map.get(functions, rsc + "/**");
				}
			}
			else {
				handler = cape.Map.get(functions, rsc + "/**");
			}
			if(handler != null) {
				req.popResource();
				handler(req);
				return(true);
			}
			return(false);
		}

		public override bool onGET(HTTPServerRequest req) {
			return(onHTTPMethod(req, getHandlerFunctions));
		}

		public override bool onPOST(HTTPServerRequest req) {
			return(onHTTPMethod(req, postHandlerFunctions));
		}

		public override bool onPUT(HTTPServerRequest req) {
			return(onHTTPMethod(req, putHandlerFunctions));
		}

		public override bool onDELETE(HTTPServerRequest req) {
			return(onHTTPMethod(req, deleteHandlerFunctions));
		}

		public override bool onPATCH(HTTPServerRequest req) {
			return(onHTTPMethod(req, patchHandlerFunctions));
		}

		public bool tryHandleRequest(HTTPServerRequest req) {
			var v = false;
			if(req == null) {
				;
			}
			else if(req.isGET()) {
				v = onGET(req);
			}
			else if(req.isPOST()) {
				v = onPOST(req);
			}
			else if(req.isPUT()) {
				v = onPUT(req);
			}
			else if(req.isDELETE()) {
				v = onDELETE(req);
			}
			else if(req.isPATCH()) {
				v = onPATCH(req);
			}
			return(v);
		}

		public override void handleRequest(HTTPServerRequest req, System.Action next) {
			if(tryHandleRequest(req)) {
				return;
			}
			var rsc = req.peekResource();
			if(object.Equals(rsc, null)) {
				rsc = "";
			}
			var sub = cape.Map.get(childObjects, rsc);
			if(sub == null) {
				sub = cape.Map.get(childObjects, rsc + "/**");
			}
			if(sub != null) {
				req.popResource();
				sub.handleRequest(req, next);
				return;
			}
			next();
			return;
		}

		public HTTPServerRequestHandlerMap child(string path, HTTPServerRequestHandler handler) {
			if(!(object.Equals(path, null))) {
				if(childObjects == null) {
					childObjects = new System.Collections.Generic.Dictionary<string,HTTPServerRequestHandler>();
				}
				childObjects[path] = handler;
				if(((handler != null) && (handler is HTTPServerComponent)) && isInitialized()) {
					((HTTPServerComponent)handler).initialize(getServer());
				}
			}
			return(this);
		}

		public HTTPServerRequestHandlerMap get(string path, System.Action<HTTPServerRequest> handler) {
			if(!(object.Equals(path, null))) {
				if(getHandlerFunctions == null) {
					getHandlerFunctions = new System.Collections.Generic.Dictionary<string,System.Action<HTTPServerRequest>>();
				}
				getHandlerFunctions[path] = handler;
			}
			return(this);
		}

		public HTTPServerRequestHandlerMap post(string path, System.Action<HTTPServerRequest> handler) {
			if(!(object.Equals(path, null))) {
				if(postHandlerFunctions == null) {
					postHandlerFunctions = new System.Collections.Generic.Dictionary<string,System.Action<HTTPServerRequest>>();
				}
				postHandlerFunctions[path] = handler;
			}
			return(this);
		}

		public HTTPServerRequestHandlerMap put(string path, System.Action<HTTPServerRequest> handler) {
			if(!(object.Equals(path, null))) {
				if(putHandlerFunctions == null) {
					putHandlerFunctions = new System.Collections.Generic.Dictionary<string,System.Action<HTTPServerRequest>>();
				}
				putHandlerFunctions[path] = handler;
			}
			return(this);
		}

		public HTTPServerRequestHandlerMap delete(string path, System.Action<HTTPServerRequest> handler) {
			if(!(object.Equals(path, null))) {
				if(deleteHandlerFunctions == null) {
					deleteHandlerFunctions = new System.Collections.Generic.Dictionary<string,System.Action<HTTPServerRequest>>();
				}
				deleteHandlerFunctions[path] = handler;
			}
			return(this);
		}

		public HTTPServerRequestHandlerMap patch(string path, System.Action<HTTPServerRequest> handler) {
			if(!(object.Equals(path, null))) {
				if(patchHandlerFunctions == null) {
					patchHandlerFunctions = new System.Collections.Generic.Dictionary<string,System.Action<HTTPServerRequest>>();
				}
				patchHandlerFunctions[path] = handler;
			}
			return(this);
		}
	}
}
