
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
	public class URL : cape.StringObject
	{
		public static URL forString(string str, bool normalizePath = false) {
			var v = new URL();
			v.parse(str, normalizePath);
			return(v);
		}

		private string scheme = null;
		private string username = null;
		private string password = null;
		private string host = null;
		private string port = null;
		private string path = null;
		private string fragment = null;
		private cape.KeyValueList<string, string> rawQueryParameters = null;
		private System.Collections.Generic.Dictionary<string,string> queryParameters = null;
		private string original = null;
		private bool percentOnly = false;
		private bool encodeUnreservedChars = true;

		public URL dup() {
			var v = new URL();
			v.setScheme(scheme);
			v.setUsername(username);
			v.setPassword(password);
			v.setHost(host);
			v.setPort(port);
			v.setPath(path);
			v.setFragment(fragment);
			if(rawQueryParameters != null) {
				v.setRawQueryParameters(rawQueryParameters.dup());
			}
			if(queryParameters != null) {
				v.setQueryParameters(cape.Map.dup(queryParameters));
			}
			return(v);
		}

		public virtual string toString() {
			return(toStringDo(true));
		}

		public string toStringNohost() {
			return(toStringDo(false));
		}

		private string toStringDo(bool userhost) {
			var sb = new cape.StringBuilder();
			if(userhost) {
				if(!(object.Equals(scheme, null))) {
					sb.append(scheme);
					sb.append("://");
				}
				if(!(object.Equals(username, null))) {
					sb.append(username);
					if(!(object.Equals(password, null))) {
						sb.append(':');
						sb.append(password);
					}
					sb.append('@');
				}
				if(!(object.Equals(host, null))) {
					sb.append(host);
					if(!(object.Equals(port, null))) {
						sb.append(':');
						sb.append(port);
					}
				}
			}
			if(!(object.Equals(path, null))) {
				sb.append(cape.String.replace(path, ' ', '+'));
			}
			if((rawQueryParameters != null) && (rawQueryParameters.count() > 0)) {
				var first = true;
				cape.Iterator<string> it = cape.Map.iterateKeys(queryParameters);
				while(it != null) {
					var key = it.next() as string;
					if(object.Equals(key, null)) {
						break;
					}
					if(first) {
						sb.append('?');
						first = false;
					}
					else {
						sb.append('&');
					}
					sb.append(key);
					var val = cape.Map.get(queryParameters, key);
					if(!(object.Equals(val, null))) {
						sb.append('=');
						sb.append(capex.URLEncoder.encode(val, percentOnly, encodeUnreservedChars));
					}
				}
			}
			if(!(object.Equals(fragment, null))) {
				sb.append('#');
				sb.append(fragment);
			}
			return(sb.toString());
		}

		private string normalizePath(string path) {
			if(object.Equals(path, null)) {
				return(null);
			}
			var v = new cape.StringBuilder();
			var comps = cape.String.split(path, '/');
			if(comps != null) {
				var n = 0;
				var m = comps.Count;
				for(n = 0 ; n < m ; n++) {
					var comp = comps[n];
					if(comp != null) {
						if(cape.String.isEmpty(comp)) {
							;
						}
						else if(object.Equals(comp, ".")) {
							;
						}
						else if(object.Equals(comp, "..")) {
							var str = v.toString();
							v.clear();
							if(!(object.Equals(str, null))) {
								var slash = cape.String.lastIndexOf(str, '/');
								if(slash > 0) {
									v.append(cape.String.getSubString(str, 0, slash));
								}
							}
						}
						else {
							v.append('/');
							v.append(comp);
						}
					}
				}
			}
			if(v.count() < 2) {
				return("/");
			}
			if(cape.String.endsWith(path, "/")) {
				v.append('/');
			}
			return(v.toString());
		}

		public void parse(string astr, bool doNormalizePath) {
			setOriginal(astr);
			if(object.Equals(astr, null)) {
				return;
			}
			var fsp = cape.String.split(astr, '#', 2);
			var str = cape.Vector.get(fsp, 0);
			fragment = cape.Vector.get(fsp, 1);
			var qsplit = cape.String.split(str, '?', 2);
			str = cape.Vector.get(qsplit, 0);
			var queryString = cape.Vector.get(qsplit, 1);
			if(cape.String.isEmpty(queryString) == false) {
				var qss = cape.String.split(queryString, '&');
				if(qss != null) {
					var n = 0;
					var m = qss.Count;
					for(n = 0 ; n < m ; n++) {
						var qs = qss[n];
						if(qs != null) {
							if(rawQueryParameters == null) {
								rawQueryParameters = new cape.KeyValueList<string, string>();
							}
							if(queryParameters == null) {
								queryParameters = new System.Collections.Generic.Dictionary<string,string>();
							}
							if(cape.String.indexOf(qs, '=') < 0) {
								cape.Map.set(queryParameters, qs, null);
								rawQueryParameters.add((string)qs, null);
								continue;
							}
							var qsps = cape.String.split(qs, '=', 2);
							var key = cape.Vector.get(qsps, 0);
							var val = cape.Vector.get(qsps, 1);
							if(cape.String.isEmpty(key) == false) {
								if(object.Equals(val, null)) {
									val = "";
								}
								var udv = capex.URLDecoder.decode(val);
								cape.Map.set(queryParameters, key, udv);
								rawQueryParameters.add((string)key, (string)udv);
							}
						}
					}
				}
			}
			var css = cape.String.indexOf(str, "://");
			if(css >= 0) {
				scheme = cape.String.subString(str, 0, css);
				if((cape.String.indexOf(scheme, ':') >= 0) || (cape.String.indexOf(scheme, '/') >= 0)) {
					scheme = null;
				}
				else {
					str = cape.String.subString(str, css + 3);
				}
			}
			string pp = null;
			if(cape.String.charAt(str, 0) == '/') {
				pp = capex.URLDecoder.decode(str);
			}
			else {
				if(cape.String.indexOf(str, '/') >= 0) {
					var sssplit = cape.String.split(str, '/', 2);
					str = cape.Vector.get(sssplit, 0);
					pp = cape.Vector.get(sssplit, 1);
					if(object.Equals(pp, null)) {
						pp = "";
					}
					if(cape.String.charAt(pp, 0) != '/') {
						pp = cape.String.append("/", pp);
					}
					pp = capex.URLDecoder.decode(pp);
				}
				if(cape.String.indexOf(str, '@') >= 0) {
					var asplit = cape.String.split(str, '@', 2);
					var auth = cape.Vector.get(asplit, 0);
					str = cape.Vector.get(asplit, 1);
					if(cape.String.indexOf(auth, ':') >= 0) {
						var acsplit = cape.String.split(auth, ':', 2);
						username = capex.URLDecoder.decode(cape.Vector.get(acsplit, 0));
						password = capex.URLDecoder.decode(cape.Vector.get(acsplit, 1));
					}
					else {
						username = auth;
					}
				}
				if(cape.String.indexOf(str, ':') >= 0) {
					var hcsplit = cape.String.split(str, ':', 2);
					str = cape.Vector.get(hcsplit, 0);
					port = cape.Vector.get(hcsplit, 1);
				}
				host = str;
			}
			if(doNormalizePath) {
				path = normalizePath(pp);
			}
			else {
				path = pp;
			}
		}

		public int getPortInt() {
			if(object.Equals(port, null)) {
				return(0);
			}
			return(cape.String.toInteger(port));
		}

		public string getQueryParameter(string key) {
			if(queryParameters == null) {
				return(null);
			}
			return(cape.Map.get(queryParameters, key));
		}

		public string getScheme() {
			return(scheme);
		}

		public URL setScheme(string v) {
			scheme = v;
			return(this);
		}

		public string getUsername() {
			return(username);
		}

		public URL setUsername(string v) {
			username = v;
			return(this);
		}

		public string getPassword() {
			return(password);
		}

		public URL setPassword(string v) {
			password = v;
			return(this);
		}

		public string getHost() {
			return(host);
		}

		public URL setHost(string v) {
			host = v;
			return(this);
		}

		public string getPort() {
			return(port);
		}

		public URL setPort(string v) {
			port = v;
			return(this);
		}

		public string getPath() {
			return(path);
		}

		public URL setPath(string v) {
			path = v;
			return(this);
		}

		public string getFragment() {
			return(fragment);
		}

		public URL setFragment(string v) {
			fragment = v;
			return(this);
		}

		public cape.KeyValueList<string, string> getRawQueryParameters() {
			return(rawQueryParameters);
		}

		public URL setRawQueryParameters(cape.KeyValueList<string, string> v) {
			rawQueryParameters = v;
			return(this);
		}

		public System.Collections.Generic.Dictionary<string,string> getQueryParameters() {
			return(queryParameters);
		}

		public URL setQueryParameters(System.Collections.Generic.Dictionary<string,string> v) {
			queryParameters = v;
			return(this);
		}

		public string getOriginal() {
			return(original);
		}

		public URL setOriginal(string v) {
			original = v;
			return(this);
		}

		public bool getPercentOnly() {
			return(percentOnly);
		}

		public URL setPercentOnly(bool v) {
			percentOnly = v;
			return(this);
		}

		public bool getEncodeUnreservedChars() {
			return(encodeUnreservedChars);
		}

		public URL setEncodeUnreservedChars(bool v) {
			encodeUnreservedChars = v;
			return(this);
		}
	}
}
