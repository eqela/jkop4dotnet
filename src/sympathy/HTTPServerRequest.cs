
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
	public class HTTPServerRequest
	{
		public static HTTPServerRequest forDetails(string method, string url, string version, cape.KeyValueList<string, string> headers) {
			var v = new HTTPServerRequest();
			v.method = method;
			v.urlString = url;
			v.version = version;
			v.setHeaders(headers);
			return(v);
		}

		private string method = null;
		private string urlString = null;
		private string version = null;
		private cape.KeyValueList<string, string> rawHeaders = null;
		private System.Collections.Generic.Dictionary<string,string> headers = null;
		private URL url = null;
		private string cacheId = null;
		private HTTPServerConnection connection = null;
		private HTTPServerBase server = null;
		private object data = null;
		private object session = null;
		private System.Collections.Generic.Dictionary<string,string> cookies = null;
		private byte[] bodyBuffer = null;
		private string bodyString = null;
		private System.Collections.Generic.Dictionary<string,string> postParameters = null;
		private System.Collections.Generic.List<string> resources = null;
		private int currentResource = 0;
		private string relativeResourcePath = null;
		private bool responseSent = false;
		private System.Collections.Generic.List<HTTPServerCookie> responseCookies = null;

		public void setBodyReceiver(DataStream receiver) {
			if(receiver == null) {
				return;
			}
			if(bodyBuffer != null) {
				var sz = bodyBuffer.Length;
				if(receiver.onDataStreamStart((long)sz) == false) {
					return;
				}
				if(receiver.onDataStreamContent(bodyBuffer, sz) == false) {
					return;
				}
				receiver.onDataStreamEnd();
				return;
			}
			if(connection == null) {
				return;
			}
			connection.setBodyReceiver(receiver);
		}

		public string getCacheId() {
			if(object.Equals(cacheId, null)) {
				if(object.Equals(method, "GET")) {
					cacheId = (method + " ") + urlString;
				}
			}
			return(cacheId);
		}

		public void clearHeaders() {
			rawHeaders = null;
			headers = null;
		}

		public void addHeader(string key, string value) {
			if(object.Equals(key, null)) {
				return;
			}
			if(rawHeaders == null) {
				rawHeaders = new cape.KeyValueList<string, string>();
			}
			if(headers == null) {
				headers = new System.Collections.Generic.Dictionary<string,string>();
			}
			rawHeaders.add((string)key, (string)value);
			headers[cape.String.toLowerCase(key)] = value;
		}

		public void setHeaders(cape.KeyValueList<string, string> headers) {
			clearHeaders();
			if(headers == null) {
				return;
			}
			var it = headers.iterate();
			if(it == null) {
				return;
			}
			while(true) {
				var kvp = it.next();
				if(kvp == null) {
					break;
				}
				addHeader(kvp.key, kvp.value);
			}
		}

		public string getHeader(string name) {
			if(cape.String.isEmpty(name)) {
				return(null);
			}
			if(headers == null) {
				return(null);
			}
			return(cape.Map.get(headers, name));
		}

		public cape.Iterator<cape.KeyValuePair<string, string>> iterateHeaders() {
			if(rawHeaders == null) {
				return(null);
			}
			return(rawHeaders.iterate());
		}

		public URL getURL() {
			if(url == null) {
				url = sympathy.URL.forString(urlString, true);
			}
			return(url);
		}

		public System.Collections.Generic.Dictionary<string,string> getQueryParameters() {
			var url = getURL();
			if(url == null) {
				return(null);
			}
			return(url.getQueryParameters());
		}

		public cape.Iterator<cape.KeyValuePair<string, string>> iterateQueryParameters() {
			var url = getURL();
			if(url == null) {
				return(null);
			}
			var list = url.getRawQueryParameters();
			if(list == null) {
				return(null);
			}
			return(list.iterate());
		}

		public string getQueryParameter(string key) {
			var url = getURL();
			if(url == null) {
				return(null);
			}
			return(url.getQueryParameter(key));
		}

		public string getURLPath() {
			var url = getURL();
			if(url == null) {
				return(null);
			}
			return(url.getPath());
		}

		public string getRemoteIPAddress() {
			var rr = getRemoteAddress();
			if(object.Equals(rr, null)) {
				return(null);
			}
			var colon = cape.String.indexOf(rr, ':');
			if(colon < 0) {
				return(rr);
			}
			return(cape.String.getSubString(rr, 0, colon));
		}

		public string getRemoteAddress() {
			if(connection == null) {
				return(null);
			}
			return(connection.getRemoteAddress());
		}

		public bool getConnectionClose() {
			var hdr = getHeader("connection");
			if(object.Equals(hdr, null)) {
				return(false);
			}
			if(object.Equals(hdr, "close")) {
				return(true);
			}
			return(false);
		}

		public string getETag() {
			return(getHeader("if-none-match"));
		}

		public System.Collections.Generic.Dictionary<string,string> getCookieValues() {
			if(cookies == null) {
				var v = new System.Collections.Generic.Dictionary<string,string>();
				var cvals = getHeader("cookie");
				if(!(object.Equals(cvals, null))) {
					var sp = cape.String.split(cvals, ';');
					if(sp != null) {
						var n = 0;
						var m = sp.Count;
						for(n = 0 ; n < m ; n++) {
							var ck = sp[n];
							if(ck != null) {
								ck = cape.String.strip(ck);
								if(cape.String.isEmpty(ck)) {
									continue;
								}
								var e = cape.String.indexOf(ck, '=');
								if(e < 0) {
									cape.Map.set(v, ck, "");
								}
								else {
									cape.Map.set(v, cape.String.getSubString(ck, 0, e), cape.String.getSubString(ck, e + 1));
								}
							}
						}
					}
				}
				cookies = v;
			}
			return(cookies);
		}

		public string getCookieValue(string name) {
			var c = getCookieValues();
			if(c == null) {
				return(null);
			}
			return(cape.Map.get(c, name));
		}

		public string getBodyString() {
			if(object.Equals(bodyString, null)) {
				var buffer = getBodyBuffer();
				if(buffer != null) {
					bodyString = cape.String.forUTF8Buffer(buffer);
				}
				bodyBuffer = null;
			}
			return(bodyString);
		}

		public object getBodyJSONObject() {
			return(cape.JSONParser.parse(getBodyString()));
		}

		public cape.DynamicVector getBodyJSONVector() {
			return(getBodyJSONObject() as cape.DynamicVector);
		}

		public cape.DynamicMap getBodyJSONMap() {
			return(getBodyJSONObject() as cape.DynamicMap);
		}

		public string getBodyJSONMapValue(string key) {
			var map = getBodyJSONMap();
			if(map == null) {
				return(null);
			}
			return(map.getString(key));
		}

		public System.Collections.Generic.Dictionary<string,string> getPostParameters() {
			if(postParameters == null) {
				var bs = getBodyString();
				if(cape.String.isEmpty(bs)) {
					return(null);
				}
				postParameters = capex.QueryString.parse(bs);
			}
			return(postParameters);
		}

		public string getPostParameter(string key) {
			var pps = getPostParameters();
			if(pps == null) {
				return(null);
			}
			return(cape.Map.get(pps, key));
		}

		public string getRelativeRequestPath(string relativeTo) {
			var path = getURLPath();
			if(object.Equals(path, null)) {
				return(null);
			}
			if(!(object.Equals(relativeTo, null)) && cape.String.startsWith(path, relativeTo)) {
				path = cape.String.getSubString(path, cape.String.getLength(relativeTo));
			}
			else {
				return(null);
			}
			if(cape.String.isEmpty(path)) {
				path = "/";
			}
			return(path);
		}

		public void initResources() {
			var path = getURLPath();
			if(object.Equals(path, null)) {
				return;
			}
			resources = cape.String.split(path, '/');
			cape.Vector.removeFirst(resources);
			var vsz = cape.Vector.getSize(resources);
			if(vsz > 0) {
				var last = cape.Vector.get(resources, vsz - 1);
				if(cape.String.isEmpty(last)) {
					cape.Vector.removeLast(resources);
				}
			}
			currentResource = 0;
		}

		public bool hasMoreResources() {
			if(resources == null) {
				initResources();
			}
			if(resources == null) {
				return(false);
			}
			if(currentResource < cape.Vector.getSize(resources)) {
				return(true);
			}
			return(false);
		}

		public int getRemainingResourceCount() {
			if(resources == null) {
				initResources();
			}
			if(resources == null) {
				return(0);
			}
			return((cape.Vector.getSize(resources) - currentResource) - 1);
		}

		public bool acceptMethodAndResource(string methodToAccept, string resource, bool mustBeLastResource = false) {
			if(object.Equals(resource, null)) {
				return(false);
			}
			if((object.Equals(methodToAccept, null)) || (object.Equals(method, methodToAccept))) {
				var cc = peekResource();
				if(object.Equals(cc, null)) {
					return(false);
				}
				if(!(object.Equals(cc, resource))) {
					return(false);
				}
				popResource();
				if(mustBeLastResource && hasMoreResources()) {
					unpopResource();
					return(false);
				}
				return(true);
			}
			return(false);
		}

		public bool acceptResource(string resource, bool mustBeLastResource = false) {
			if(object.Equals(resource, null)) {
				return(false);
			}
			var cc = peekResource();
			if(object.Equals(cc, null)) {
				return(false);
			}
			if(!(object.Equals(cc, resource))) {
				return(false);
			}
			popResource();
			if(mustBeLastResource && hasMoreResources()) {
				unpopResource();
				return(false);
			}
			return(true);
		}

		public string peekResource() {
			if(resources == null) {
				initResources();
			}
			if(resources == null) {
				return(null);
			}
			if(currentResource < cape.Vector.getSize(resources)) {
				return(resources[currentResource]);
			}
			return(null);
		}

		public int getCurrentResource() {
			return(currentResource);
		}

		public void setCurrentResource(int value) {
			currentResource = value;
			relativeResourcePath = null;
		}

		public string popResource() {
			if(resources == null) {
				initResources();
			}
			var v = peekResource();
			if(!(object.Equals(v, null))) {
				currentResource++;
				relativeResourcePath = null;
			}
			return(v);
		}

		public void unpopResource() {
			if(currentResource > 0) {
				currentResource--;
				relativeResourcePath = null;
			}
		}

		public void resetResources() {
			resources = null;
			currentResource = 0;
			relativeResourcePath = null;
		}

		public System.Collections.Generic.List<string> getRelativeResources() {
			if(resources == null) {
				initResources();
			}
			if(resources == null) {
				return(null);
			}
			if(currentResource < 1) {
				return(resources);
			}
			var v = new System.Collections.Generic.List<string>();
			var cr = currentResource;
			while(cr < cape.Vector.getSize(resources)) {
				v.Add(resources[cr]);
				cr++;
			}
			return(v);
		}

		public string getRelativeResourcePath() {
			if(resources == null) {
				return(getURLPath());
			}
			if(object.Equals(relativeResourcePath, null)) {
				var rrs = getRelativeResources();
				if(rrs != null) {
					var sb = new cape.StringBuilder();
					if(rrs != null) {
						var n = 0;
						var m = rrs.Count;
						for(n = 0 ; n < m ; n++) {
							var rr = rrs[n];
							if(rr != null) {
								if(cape.String.isEmpty(rr) == false) {
									sb.append('/');
									sb.append(rr);
								}
							}
						}
					}
					if(sb.count() < 1) {
						sb.append('/');
					}
					relativeResourcePath = sb.toString();
				}
			}
			return(relativeResourcePath);
		}

		public bool isForResource(string res) {
			if(object.Equals(res, null)) {
				return(false);
			}
			var rrp = getRelativeResourcePath();
			if(object.Equals(rrp, null)) {
				return(false);
			}
			if(object.Equals(rrp, res)) {
				return(true);
			}
			return(false);
		}

		public bool isForDirectory() {
			var path = getURLPath();
			if(!(object.Equals(path, null)) && cape.String.endsWith(path, "/")) {
				return(true);
			}
			return(false);
		}

		public bool isForPrefix(string res) {
			if(object.Equals(res, null)) {
				return(false);
			}
			var rr = getRelativeResourcePath();
			if(!(object.Equals(rr, null)) && cape.String.startsWith(rr, res)) {
				return(true);
			}
			return(false);
		}

		public bool isGET() {
			return(object.Equals(method, "GET"));
		}

		public bool isPOST() {
			return(object.Equals(method, "POST"));
		}

		public bool isDELETE() {
			return(object.Equals(method, "DELETE"));
		}

		public bool isPUT() {
			return(object.Equals(method, "PUT"));
		}

		public bool isPATCH() {
			return(object.Equals(method, "PATCH"));
		}

		public void sendJSONObject(object o) {
			sendResponse(sympathy.HTTPServerResponse.forJSONString(cape.JSONEncoder.encode(o)));
		}

		public void sendJSONString(string json) {
			sendResponse(sympathy.HTTPServerResponse.forJSONString(json));
		}

		public void sendJSONError(cape.Error error) {
			sendResponse(sympathy.HTTPServerResponse.forJSONString(cape.JSONEncoder.encode((object)sympathy.JSONResponse.forError(error))));
		}

		public void sendJSONOK(object data = null) {
			sendResponse(sympathy.HTTPServerResponse.forJSONString(cape.JSONEncoder.encode((object)sympathy.JSONResponse.forOk(data))));
		}

		public void sendInternalError(string text = null) {
			sendResponse(sympathy.HTTPServerResponse.forHTTPInternalError(text));
		}

		public void sendNotAllowed() {
			sendResponse(sympathy.HTTPServerResponse.forHTTPNotAllowed());
		}

		public void sendNotFound() {
			sendResponse(sympathy.HTTPServerResponse.forHTTPNotFound());
		}

		public void sendInvalidRequest(string text = null) {
			sendResponse(sympathy.HTTPServerResponse.forHTTPInvalidRequest(text));
		}

		public void sendTextString(string text) {
			sendResponse(sympathy.HTTPServerResponse.forTextString(text));
		}

		public void sendHTMLString(string html) {
			sendResponse(sympathy.HTTPServerResponse.forHTMLString(html));
		}

		public void sendXMLString(string xml) {
			sendResponse(sympathy.HTTPServerResponse.forXMLString(xml));
		}

		public void sendFile(cape.File file) {
			sendResponse(sympathy.HTTPServerResponse.forFile(file));
		}

		public void sendBuffer(byte[] buffer, string mimeType = null) {
			sendResponse(sympathy.HTTPServerResponse.forBuffer(buffer, mimeType));
		}

		public void sendRedirect(string url) {
			sendResponse(sympathy.HTTPServerResponse.forHTTPMovedTemporarily(url));
		}

		public void sendRedirectAsDirectory() {
			var path = getURLPath();
			if(object.Equals(path, null)) {
				path = "";
			}
			sendRedirect(path + "/");
		}

		public bool isResponseSent() {
			return(responseSent);
		}

		public void addResponseCookie(HTTPServerCookie cookie) {
			if(cookie == null) {
				return;
			}
			if(responseCookies == null) {
				responseCookies = new System.Collections.Generic.List<HTTPServerCookie>();
			}
			responseCookies.Add(cookie);
		}

		public void sendResponse(HTTPServerResponse resp) {
			if(responseSent) {
				return;
			}
			if(server == null) {
				return;
			}
			if(responseCookies != null) {
				var n = 0;
				var m = responseCookies.Count;
				for(n = 0 ; n < m ; n++) {
					var cookie = responseCookies[n];
					if(cookie != null) {
						resp.addCookie(cookie);
					}
				}
			}
			responseCookies = null;
			server.sendResponse(connection, this, resp);
			responseSent = true;
		}

		public string getMethod() {
			return(method);
		}

		public HTTPServerRequest setMethod(string v) {
			method = v;
			return(this);
		}

		public string getUrlString() {
			return(urlString);
		}

		public HTTPServerRequest setUrlString(string v) {
			urlString = v;
			return(this);
		}

		public string getVersion() {
			return(version);
		}

		public HTTPServerRequest setVersion(string v) {
			version = v;
			return(this);
		}

		public HTTPServerConnection getConnection() {
			return(connection);
		}

		public HTTPServerRequest setConnection(HTTPServerConnection v) {
			connection = v;
			return(this);
		}

		public HTTPServerBase getServer() {
			return(server);
		}

		public HTTPServerRequest setServer(HTTPServerBase v) {
			server = v;
			return(this);
		}

		public object getData() {
			return(data);
		}

		public HTTPServerRequest setData(object v) {
			data = v;
			return(this);
		}

		public object getSession() {
			return(session);
		}

		public HTTPServerRequest setSession(object v) {
			session = v;
			return(this);
		}

		public byte[] getBodyBuffer() {
			return(bodyBuffer);
		}

		public HTTPServerRequest setBodyBuffer(byte[] v) {
			bodyBuffer = v;
			return(this);
		}
	}
}
