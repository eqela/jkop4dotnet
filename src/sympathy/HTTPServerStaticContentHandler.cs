
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
	public class HTTPServerStaticContentHandler : HTTPServerRequestHandler
	{
		public static HTTPServerStaticContentHandler forContent(string content, string mimeType) {
			var v = new HTTPServerStaticContentHandler();
			v.setContent(content);
			v.setMimeType(mimeType);
			return(v);
		}

		public static HTTPServerStaticContentHandler forHTMLContent(string content) {
			var v = new HTTPServerStaticContentHandler();
			v.setContent(content);
			v.setMimeType("text/html");
			return(v);
		}

		public static HTTPServerStaticContentHandler forJSONContent(string content) {
			var v = new HTTPServerStaticContentHandler();
			v.setContent(content);
			v.setMimeType("application/json");
			return(v);
		}

		public static HTTPServerStaticContentHandler forRedirect(string url) {
			var v = new HTTPServerStaticContentHandler();
			v.setRedirectUrl(url);
			return(v);
		}

		private string content = null;
		private string mimeType = null;
		private string redirectUrl = null;

		public virtual void handleRequest(HTTPServerRequest req, System.Action next) {
			if(!(object.Equals(redirectUrl, null))) {
				req.sendResponse(sympathy.HTTPServerResponse.forRedirect(redirectUrl));
			}
			else {
				req.sendResponse(sympathy.HTTPServerResponse.forString(content, mimeType));
			}
		}

		public string getContent() {
			return(content);
		}

		public HTTPServerStaticContentHandler setContent(string v) {
			content = v;
			return(this);
		}

		public string getMimeType() {
			return(mimeType);
		}

		public HTTPServerStaticContentHandler setMimeType(string v) {
			mimeType = v;
			return(this);
		}

		public string getRedirectUrl() {
			return(redirectUrl);
		}

		public HTTPServerStaticContentHandler setRedirectUrl(string v) {
			redirectUrl = v;
			return(this);
		}
	}
}
