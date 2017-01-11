
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
	public class HTTPServerRequestHandlerStack : HTTPServerRequestHandlerContainer
	{
		private class FunctionRequestHandler : HTTPServerRequestHandler
		{
			private System.Action<HTTPServerRequest, System.Action> handler = null;

			public virtual void handleRequest(HTTPServerRequest req, System.Action next) {
				handler(req, next);
			}

			public System.Action<HTTPServerRequest, System.Action> getHandler() {
				return(handler);
			}

			public FunctionRequestHandler setHandler(System.Action<HTTPServerRequest, System.Action> v) {
				handler = v;
				return(this);
			}
		}

		private class RequestProcessor
		{
			private System.Collections.Generic.List<HTTPServerRequestHandler> requestHandlers = null;
			private HTTPServerRequest request = null;
			private System.Action last = null;
			private int current = 0;

			public void start() {
				current = -1;
				next();
			}

			public void next() {
				current++;
				var handler = cape.Vector.get(requestHandlers, current);
				if(handler == null) {
					if(last == null) {
						defaultLast();
					}
					else {
						last();
					}
					return;
				}
				handler.handleRequest(request, next);
				request.resetResources();
			}

			public void defaultLast() {
				request.sendResponse(sympathy.HTTPServerResponse.forHTTPNotFound());
			}

			public System.Collections.Generic.List<HTTPServerRequestHandler> getRequestHandlers() {
				return(requestHandlers);
			}

			public RequestProcessor setRequestHandlers(System.Collections.Generic.List<HTTPServerRequestHandler> v) {
				requestHandlers = v;
				return(this);
			}

			public HTTPServerRequest getRequest() {
				return(request);
			}

			public RequestProcessor setRequest(HTTPServerRequest v) {
				request = v;
				return(this);
			}

			public System.Action getLast() {
				return(last);
			}

			public RequestProcessor setLast(System.Action v) {
				last = v;
				return(this);
			}
		}

		protected System.Collections.Generic.List<HTTPServerRequestHandler> requestHandlers = null;

		public override cape.Iterator<HTTPServerRequestHandler> iterateRequestHandlers() {
			if(requestHandlers == null) {
				return(null);
			}
			return((cape.Iterator<HTTPServerRequestHandler>)cape.Vector.iterate(requestHandlers));
		}

		public void pushRequestHandler(System.Action<HTTPServerRequest, System.Action> handler) {
			if(handler == null) {
				return;
			}
			pushRequestHandler((HTTPServerRequestHandler)new FunctionRequestHandler().setHandler(handler));
		}

		public void pushRequestHandler(HTTPServerRequestHandler handler) {
			if(handler == null) {
				return;
			}
			if(requestHandlers == null) {
				requestHandlers = new System.Collections.Generic.List<HTTPServerRequestHandler>();
			}
			requestHandlers.Add(handler);
			if((handler is HTTPServerComponent) && isInitialized()) {
				((HTTPServerComponent)handler).initialize(getServer());
			}
		}

		public override void handleRequest(HTTPServerRequest req, System.Action next) {
			var rp = new RequestProcessor();
			rp.setRequestHandlers(requestHandlers);
			rp.setRequest(req);
			rp.setLast(next);
			rp.start();
		}
	}
}
