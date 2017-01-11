
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
	public class HTTPClientResponse : cape.StringObject
	{
		private string httpVersion = null;
		private string httpStatus = null;
		private string httpStatusDescription = null;
		private cape.KeyValueListForStrings rawHeaders = null;
		private System.Collections.Generic.Dictionary<string,string> headers = null;

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

		public virtual string toString() {
			return(cape.String.asString((object)rawHeaders));
		}

		public string getHttpVersion() {
			return(httpVersion);
		}

		public HTTPClientResponse setHttpVersion(string v) {
			httpVersion = v;
			return(this);
		}

		public string getHttpStatus() {
			return(httpStatus);
		}

		public HTTPClientResponse setHttpStatus(string v) {
			httpStatus = v;
			return(this);
		}

		public string getHttpStatusDescription() {
			return(httpStatusDescription);
		}

		public HTTPClientResponse setHttpStatusDescription(string v) {
			httpStatusDescription = v;
			return(this);
		}

		public cape.KeyValueListForStrings getRawHeaders() {
			return(rawHeaders);
		}

		public HTTPClientResponse setRawHeaders(cape.KeyValueListForStrings v) {
			rawHeaders = v;
			return(this);
		}

		public System.Collections.Generic.Dictionary<string,string> getHeaders() {
			return(headers);
		}

		public HTTPClientResponse setHeaders(System.Collections.Generic.Dictionary<string,string> v) {
			headers = v;
			return(this);
		}
	}
}
