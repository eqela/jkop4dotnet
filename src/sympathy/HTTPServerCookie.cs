
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
	public class HTTPServerCookie : cape.StringObject
	{
		public HTTPServerCookie(string key, string value) {
			this.key = key;
			this.value = value;
		}

		private string key = null;
		private string value = null;
		private int maxAge = -1;
		private string path = null;
		private string domain = null;

		public virtual string toString() {
			var sb = new cape.StringBuilder();
			sb.append(key);
			sb.append('=');
			sb.append(value);
			if(maxAge >= 0) {
				sb.append("; Max-Age=");
				sb.append(cape.String.forInteger(maxAge));
			}
			if(cape.String.isEmpty(path) == false) {
				sb.append("; Path=");
				sb.append(path);
			}
			if(cape.String.isEmpty(domain) == false) {
				sb.append("; Domain=");
				sb.append(domain);
			}
			return(sb.toString());
		}

		public string getKey() {
			return(key);
		}

		public HTTPServerCookie setKey(string v) {
			key = v;
			return(this);
		}

		public string getValue() {
			return(value);
		}

		public HTTPServerCookie setValue(string v) {
			value = v;
			return(this);
		}

		public int getMaxAge() {
			return(maxAge);
		}

		public HTTPServerCookie setMaxAge(int v) {
			maxAge = v;
			return(this);
		}

		public string getPath() {
			return(path);
		}

		public HTTPServerCookie setPath(string v) {
			path = v;
			return(this);
		}

		public string getDomain() {
			return(domain);
		}

		public HTTPServerCookie setDomain(string v) {
			domain = v;
			return(this);
		}
	}
}
