
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
	public class HTTPServerResponse
	{
		public static HTTPServerResponse forFile(cape.File file, int maxCachedSize = -1) {
			if((file == null) || (file.isFile() == false)) {
				return(forHTTPNotFound());
			}
			var bodyset = false;
			var resp = new HTTPServerResponse();
			resp.setStatus("200");
			resp.addHeader("Content-Type", capex.MimeTypeRegistry.typeForFile(file));
			var st = file.stat();
			if(st != null) {
				var lm = st.getModifyTime();
				if(lm > 0) {
					var dts = capex.VerboseDateTimeString.forDateTime(cape.DateTime.forTimeSeconds((long)lm));
					resp.addHeader("Last-Modified", dts);
					resp.setETag(capex.MD5Encoder.encode(dts));
				}
				var mcs = maxCachedSize;
				if(mcs < 0) {
					mcs = 32 * 1024;
				}
				if(st.getSize() < mcs) {
					resp.setBody(file.getContentsBuffer());
					bodyset = true;
				}
			}
			if(bodyset == false) {
				resp.setBody(file);
			}
			return(resp);
		}

		public static HTTPServerResponse forBuffer(byte[] data, string mimetype = null) {
			var mt = mimetype;
			if(cape.String.isEmpty(mt)) {
				mt = "application/binary";
			}
			var resp = new HTTPServerResponse();
			resp.setStatus("200");
			resp.addHeader("Content-Type", mt);
			resp.setBody(data);
			return(resp);
		}

		public static HTTPServerResponse forString(string text, string mimetype) {
			var resp = new HTTPServerResponse();
			resp.setStatus("200");
			if(cape.String.isEmpty(mimetype) == false) {
				resp.addHeader("Content-Type", mimetype);
			}
			resp.setBody(text);
			return(resp);
		}

		public static HTTPServerResponse forTextString(string text) {
			var resp = new HTTPServerResponse();
			resp.setStatus("200");
			resp.addHeader("Content-Type", "text/plain; charset=\"UTF-8\"");
			resp.setBody(text);
			return(resp);
		}

		public static HTTPServerResponse forHTMLString(string html) {
			var resp = new HTTPServerResponse();
			resp.setStatus("200");
			resp.addHeader("Content-Type", "text/html; charset=\"UTF-8\"");
			resp.setBody(html);
			return(resp);
		}

		public static HTTPServerResponse forXMLString(string xml) {
			var resp = new HTTPServerResponse();
			resp.setStatus("200");
			resp.addHeader("Content-Type", "text/xml; charset=\"UTF-8\"");
			resp.setBody(xml);
			return(resp);
		}

		public static HTTPServerResponse forJSONObject(object o) {
			return(forJSONString(cape.JSONEncoder.encode(o)));
		}

		public static HTTPServerResponse forJSONString(string json) {
			var resp = new HTTPServerResponse();
			resp.setStatus("200");
			resp.addHeader("Content-Type", "application/json; charset=\"UTF-8\"");
			resp.setBody(json);
			return(resp);
		}

		private static string stringWithMessage(string str, string message) {
			if(cape.String.isEmpty(message)) {
				return(str);
			}
			return((str + ": ") + message);
		}

		public static HTTPServerResponse forHTTPInvalidRequest(string message = null) {
			var resp = forTextString(stringWithMessage("Invalid request", message));
			resp.setStatus("400");
			resp.addHeader("Connection", "close");
			resp.setMessage(message);
			return(resp);
		}

		public static HTTPServerResponse forHTTPInternalError(string message = null) {
			var resp = forTextString(stringWithMessage("Internal server error", message));
			resp.setStatus("500");
			resp.addHeader("Connection", "close");
			resp.setMessage(message);
			return(resp);
		}

		public static HTTPServerResponse forHTTPNotImplemented(string message = null) {
			var resp = forTextString(stringWithMessage("Not implemented", message));
			resp.setStatus("501");
			resp.addHeader("Connection", "close");
			resp.setMessage(message);
			return(resp);
		}

		public static HTTPServerResponse forHTTPNotAllowed(string message = null) {
			var resp = forTextString(stringWithMessage("Not allowed", message));
			resp.setStatus("405");
			resp.setMessage(message);
			return(resp);
		}

		public static HTTPServerResponse forHTTPNotFound(string message = null) {
			var resp = forTextString(stringWithMessage("Not found", message));
			resp.setStatus("404");
			resp.setMessage(message);
			return(resp);
		}

		public static HTTPServerResponse forHTTPForbidden(string message = null) {
			var resp = forTextString(stringWithMessage("Forbidden", message));
			resp.setStatus("403");
			resp.setMessage(message);
			return(resp);
		}

		public static HTTPServerResponse forRedirect(string url) {
			return(forHTTPMovedTemporarily(url));
		}

		public static HTTPServerResponse forHTTPMovedPermanently(string url) {
			var resp = new HTTPServerResponse();
			resp.setStatus("301");
			resp.addHeader("Location", url);
			resp.setBody(url);
			return(resp);
		}

		public static HTTPServerResponse forHTTPMovedTemporarily(string url) {
			var resp = new HTTPServerResponse();
			resp.setStatus("303");
			resp.addHeader("Location", url);
			resp.setBody(url);
			return(resp);
		}

		private cape.KeyValueList<string, string> headers = null;
		private string message = null;
		private int cacheTtl = 0;
		private string status = null;
		private bool statusIsOk = false;
		private cape.Reader body = null;
		private string eTag = null;

		public HTTPServerResponse setETag(string eTag) {
			this.eTag = eTag;
			addHeader("ETag", eTag);
			return(this);
		}

		public string getETag() {
			return(eTag);
		}

		public HTTPServerResponse setStatus(string status) {
			this.status = status;
			if(object.Equals(status, "200")) {
				statusIsOk = true;
			}
			return(this);
		}

		public string getStatus() {
			return(status);
		}

		public int getCacheTtl() {
			if(statusIsOk) {
				return(cacheTtl);
			}
			return(0);
		}

		public HTTPServerResponse enableCaching(int ttl = 3600) {
			cacheTtl = ttl;
			return(this);
		}

		public HTTPServerResponse disableCaching() {
			cacheTtl = 0;
			return(this);
		}

		public HTTPServerResponse enableCORS(HTTPServerRequest req = null) {
			addHeader("Access-Control-Allow-Origin", "*");
			if(req != null) {
				addHeader("Access-Control-Allow-Methods", req.getHeader("access-control-request-method"));
				addHeader("Access-Control-Allow-Headers", req.getHeader("access-control-request-headers"));
			}
			addHeader("Access-Control-Max-Age", "1728000");
			return(this);
		}

		public HTTPServerResponse addHeader(string key, string value) {
			if(headers == null) {
				headers = new cape.KeyValueList<string, string>();
			}
			headers.add((string)key, (string)value);
			return(this);
		}

		public void addCookie(HTTPServerCookie cookie) {
			if(cookie == null) {
				return;
			}
			addHeader("Set-Cookie", cookie.toString());
		}

		public HTTPServerResponse setBody(byte[] buf) {
			if(buf == null) {
				body = null;
				addHeader("Content-Length", "0");
			}
			else {
				body = (cape.Reader)cape.BufferReader.forBuffer(buf);
				addHeader("Content-Length", cape.String.forInteger(buf.Length));
			}
			return(this);
		}

		public HTTPServerResponse setBody(string str) {
			if(object.Equals(str, null)) {
				body = null;
				addHeader("Content-Length", "0");
			}
			else {
				var buf = cape.String.toUTF8Buffer(str);
				body = (cape.Reader)cape.BufferReader.forBuffer(buf);
				addHeader("Content-Length", cape.String.forInteger((int)cape.Buffer.getSize(buf)));
			}
			return(this);
		}

		public HTTPServerResponse setBody(cape.File file) {
			if((file == null) || (file.isFile() == false)) {
				body = null;
				addHeader("Content-Length", "0");
			}
			else {
				body = (cape.Reader)file.read();
				addHeader("Content-Length", cape.String.forInteger(file.getSize()));
			}
			return(this);
		}

		public HTTPServerResponse setBody(cape.SizedReader reader) {
			if(reader == null) {
				body = null;
				addHeader("Content-Length", "0");
			}
			else {
				body = (cape.Reader)reader;
				addHeader("Content-Length", cape.String.forInteger(reader.getSize()));
			}
			return(this);
		}

		public cape.Reader getBody() {
			return(body);
		}

		public cape.KeyValueList<string, string> getHeaders() {
			return(headers);
		}

		public HTTPServerResponse setHeaders(cape.KeyValueList<string, string> v) {
			headers = v;
			return(this);
		}

		public string getMessage() {
			return(message);
		}

		public HTTPServerResponse setMessage(string v) {
			message = v;
			return(this);
		}
	}
}
