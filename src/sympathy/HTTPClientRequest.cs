
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
	public class HTTPClientRequest : cape.StringObject
	{
		public static HTTPClientRequest forGET(string url) {
			var v = new HTTPClientRequest();
			v.setMethod("GET");
			v.setUrl(url);
			return(v);
		}

		public static HTTPClientRequest forPOST(string url, string mimeType, cape.SizedReader data) {
			var v = new HTTPClientRequest();
			v.setMethod("POST");
			v.setUrl(url);
			if(cape.String.isEmpty(mimeType) == false) {
				v.addHeader("Content-Type", mimeType);
			}
			if(data != null) {
				v.setBody(data);
			}
			return(v);
		}

		public static HTTPClientRequest forPOST(string url, string mimeType, byte[] data) {
			var v = new HTTPClientRequest();
			v.setMethod("POST");
			v.setUrl(url);
			if(cape.String.isEmpty(mimeType) == false) {
				v.addHeader("Content-Type", mimeType);
			}
			if(data != null) {
				v.setBody((cape.SizedReader)cape.BufferReader.forBuffer(data));
			}
			return(v);
		}

		private string method = null;
		private string protocol = null;
		private string username = null;
		private string password = null;
		private string serverAddress = null;
		private int serverPort = 0;
		private string requestPath = null;
		private cape.SizedReader body = null;
		private cape.KeyValueList<string, string> queryParams = null;
		private cape.KeyValueListForStrings rawHeaders = null;
		private System.Collections.Generic.Dictionary<string,string> headers = null;

		public HTTPClientRequest() {
			protocol = "http";
			serverPort = 80;
			requestPath = "/";
			method = "GET";
		}

		public void setUrl(string url) {
			var uu = sympathy.URL.forString(url);
			setProtocol(uu.getScheme());
			setUsername(uu.getUsername());
			setPassword(uu.getPassword());
			setServerAddress(uu.getHost());
			var pp = cape.String.toInteger(uu.getPort());
			if(pp < 1) {
				if(object.Equals(protocol, "https")) {
					pp = 443;
				}
				else if(object.Equals(protocol, "http")) {
					pp = 80;
				}
			}
			setServerPort(pp);
			setRequestPath(uu.getPath());
			queryParams = uu.getRawQueryParameters();
		}

		public void addHeader(string key, string value) {
			if(rawHeaders == null) {
				rawHeaders = new cape.KeyValueListForStrings();
			}
			if(headers == null) {
				headers = new System.Collections.Generic.Dictionary<string,string>();
			}
			rawHeaders.add(key, value);
			headers[cape.String.toLowerCase(key)] = value;
		}

		public string getHeader(string key) {
			if(headers == null) {
				return(null);
			}
			return(cape.Map.get(headers, key));
		}

		public void setUserAgent(string agent) {
			addHeader("User-Agent", agent);
		}

		public string toString(string defaultUserAgent) {
			var rq = new cape.StringBuilder();
			var body = getBody();
			var path = getRequestPath();
			if(cape.String.isEmpty(path)) {
				path = "/";
			}
			rq.append(getMethod());
			rq.append(' ');
			rq.append(path);
			var first = true;
			if(queryParams != null) {
				var it = queryParams.iterate();
				while(it != null) {
					var kv = it.next();
					if(kv == null) {
						break;
					}
					if(first) {
						rq.append('?');
						first = false;
					}
					else {
						rq.append('&');
					}
					rq.append(kv.key);
					var val = kv.value;
					if(!(object.Equals(val, null))) {
						rq.append('=');
						rq.append(capex.URLEncoder.encode(val, false, false));
					}
				}
			}
			rq.append(' ');
			rq.append("HTTP/1.1\r\n");
			var hasUserAgent = false;
			var hasHost = false;
			var hasContentLength = false;
			if(rawHeaders != null) {
				var it1 = ((cape.KeyValueList<string, string>)rawHeaders).iterate();
				while(true) {
					var kvp = it1.next();
					if(kvp == null) {
						break;
					}
					var key = kvp.key;
					if(cape.String.equalsIgnoreCase(key, "user-agent")) {
						hasUserAgent = true;
					}
					else if(cape.String.equalsIgnoreCase(key, "host")) {
						hasHost = true;
					}
					else if(cape.String.equalsIgnoreCase(key, "content-length")) {
						hasContentLength = true;
					}
					rq.append(key);
					rq.append(": ");
					rq.append(kvp.value);
					rq.append("\r\n");
				}
			}
			if((hasUserAgent == false) && !(object.Equals(defaultUserAgent, null))) {
				rq.append("User-Agent: ");
				rq.append(defaultUserAgent);
				rq.append("\r\n");
			}
			if(hasHost == false) {
				rq.append(("Host: " + getServerAddress()) + "\r\n");
			}
			if((body != null) && (hasContentLength == false)) {
				var bs = body.getSize();
				var bss = cape.String.forInteger(bs);
				rq.append(("Content-Length: " + bss) + "\r\n");
			}
			rq.append("\r\n");
			return(rq.toString());
		}

		public virtual string toString() {
			return(toString(null));
		}

		public string getMethod() {
			return(method);
		}

		public HTTPClientRequest setMethod(string v) {
			method = v;
			return(this);
		}

		public string getProtocol() {
			return(protocol);
		}

		public HTTPClientRequest setProtocol(string v) {
			protocol = v;
			return(this);
		}

		public string getUsername() {
			return(username);
		}

		public HTTPClientRequest setUsername(string v) {
			username = v;
			return(this);
		}

		public string getPassword() {
			return(password);
		}

		public HTTPClientRequest setPassword(string v) {
			password = v;
			return(this);
		}

		public string getServerAddress() {
			return(serverAddress);
		}

		public HTTPClientRequest setServerAddress(string v) {
			serverAddress = v;
			return(this);
		}

		public int getServerPort() {
			return(serverPort);
		}

		public HTTPClientRequest setServerPort(int v) {
			serverPort = v;
			return(this);
		}

		public string getRequestPath() {
			return(requestPath);
		}

		public HTTPClientRequest setRequestPath(string v) {
			requestPath = v;
			return(this);
		}

		public cape.SizedReader getBody() {
			return(body);
		}

		public HTTPClientRequest setBody(cape.SizedReader v) {
			body = v;
			return(this);
		}

		public cape.KeyValueListForStrings getRawHeaders() {
			return(rawHeaders);
		}

		public HTTPClientRequest setRawHeaders(cape.KeyValueListForStrings v) {
			rawHeaders = v;
			return(this);
		}

		public System.Collections.Generic.Dictionary<string,string> getHeaders() {
			return(headers);
		}

		public HTTPClientRequest setHeaders(System.Collections.Generic.Dictionary<string,string> v) {
			headers = v;
			return(this);
		}
	}
}
