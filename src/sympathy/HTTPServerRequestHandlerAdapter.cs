
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
	public class HTTPServerRequestHandlerAdapter : HTTPServerRequestHandler, HTTPServerComponent
	{
		private HTTPServerBase server = null;
		protected cape.LoggingContext logContext = null;

		public HTTPServerBase getServer() {
			return(server);
		}

		public bool isInitialized() {
			if(server == null) {
				return(false);
			}
			return(true);
		}

		public virtual void initialize(HTTPServerBase server) {
			this.server = server;
			if(server != null) {
				this.logContext = server.getLogContext();
			}
			else {
				this.logContext = null;
			}
		}

		public virtual void onMaintenance() {
		}

		public virtual void onRefresh() {
		}

		public virtual void cleanup() {
			this.server = null;
		}

		public virtual bool onGET(HTTPServerRequest req) {
			return(false);
		}

		public virtual void onGET(HTTPServerRequest req, System.Action next) {
			if(onGET(req) == false) {
				next();
			}
		}

		public virtual bool onPOST(HTTPServerRequest req) {
			return(false);
		}

		public virtual void onPOST(HTTPServerRequest req, System.Action next) {
			if(onPOST(req) == false) {
				next();
			}
		}

		public virtual bool onPUT(HTTPServerRequest req) {
			return(false);
		}

		public virtual void onPUT(HTTPServerRequest req, System.Action next) {
			if(onPUT(req) == false) {
				next();
			}
		}

		public virtual bool onDELETE(HTTPServerRequest req) {
			return(false);
		}

		public virtual void onDELETE(HTTPServerRequest req, System.Action next) {
			if(onDELETE(req) == false) {
				next();
			}
		}

		public virtual bool onPATCH(HTTPServerRequest req) {
			return(false);
		}

		public virtual void onPATCH(HTTPServerRequest req, System.Action next) {
			if(onPATCH(req) == false) {
				next();
			}
		}

		public virtual void handleRequest(HTTPServerRequest req, System.Action next) {
			if(req == null) {
				next();
			}
			else if(req.isGET()) {
				onGET(req, next);
			}
			else if(req.isPOST()) {
				onPOST(req, next);
			}
			else if(req.isPUT()) {
				onPUT(req, next);
			}
			else if(req.isDELETE()) {
				onDELETE(req, next);
			}
			else if(req.isPATCH()) {
				onPATCH(req, next);
			}
			else {
				next();
			}
		}
	}
}
