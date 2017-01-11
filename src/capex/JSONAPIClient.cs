
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

namespace capex
{
	public class JSONAPIClient
	{
		private string apiUrl = null;

		public virtual string getFullURL(string api) {
			var url = apiUrl;
			if(cape.String.isEmpty(url)) {
				url = "/";
			}
			if(object.Equals(url, "/")) {
				if(cape.String.startsWith(api, "/")) {
					return(api);
				}
				return(url + api);
			}
			if(cape.String.endsWith(url, "/")) {
				if(cape.String.startsWith(api, "/")) {
					return(url + cape.String.getSubString(api, 1));
				}
				return(url + api);
			}
			if(cape.String.startsWith(api, "/")) {
				return(url + api);
			}
			return((url + "/") + api);
		}

		private byte[] toUTF8Buffer(cape.DynamicMap data) {
			if(data == null) {
				return(null);
			}
			return(cape.String.toUTF8Buffer(cape.JSONEncoder.encode((object)data)));
		}

		public virtual void customizeRequestHeaders(cape.KeyValueList<string, string> headers) {
		}

		public virtual void onStartSendRequest() {
		}

		public virtual void onEndSendRequest() {
		}

		public virtual void onDefaultErrorHandler(cape.Error error) {
		}

		public bool handleAsError(cape.DynamicMap response, System.Action<cape.Error> callback = null) {
			var error = toError(response);
			if(error != null) {
				onError(error, callback);
			}
			return(false);
		}

		public cape.Error toError(cape.DynamicMap response) {
			if(response == null) {
				return(cape.Error.forCode("noServerResponse"));
			}
			if(object.Equals(response.getString("status"), "error")) {
				var v = new cape.Error();
				v.setCode(response.getString("code"));
				v.setMessage(response.getString("message"));
				v.setDetail(response.getString("detail"));
				return(v);
			}
			return(null);
		}

		public virtual void onError(cape.Error error, System.Action<cape.Error> callback) {
			if(callback != null) {
				callback(error);
			}
			else {
				onDefaultErrorHandler(error);
			}
		}

		public virtual void onError(cape.Error error) {
			onError(error, null);
		}

		public void sendRequest(string method, string url, cape.KeyValueList<string, string> headers, byte[] data, System.Action<cape.DynamicMap> callback, System.Action<cape.Error> errorCallback = null) {
			if(callback == null) {
				return;
			}
			var hrs = headers;
			if(headers == null) {
				hrs = new cape.KeyValueList<string, string>();
				hrs.add((string)"Content-Type", (string)"application/json");
			}
			var webClient = capex.NativeWebClient.instance();
			if(webClient == null) {
				onError(cape.Error.forCode("noWebClient"), errorCallback);
				return;
			}
			var ll = callback;
			customizeRequestHeaders(hrs);
			onStartSendRequest();
			var ecb = errorCallback;
			webClient.query(method, url, hrs, data, (string status, cape.KeyValueList<string, string> responseHeaders, byte[] data1) => {
				onEndSendRequest();
				if(data1 == null) {
					onError(cape.Error.forCode("failedToConnect"), ecb);
					return;
				}
				var jsonResponseBody = cape.JSONParser.parse(cape.String.forUTF8Buffer(data1)) as cape.DynamicMap;
				if(jsonResponseBody == null) {
					onError(cape.Error.forCode("invalidServerResponse"), ecb);
					return;
				}
				ll(jsonResponseBody);
			});
		}

		public void doGet(string url, System.Action<cape.DynamicMap> callback, System.Action<cape.Error> errorCallback = null) {
			sendRequest("GET", getFullURL(url), null, null, callback, errorCallback);
		}

		public void doPost(string url, cape.DynamicMap data, System.Action<cape.DynamicMap> callback, System.Action<cape.Error> errorCallback = null) {
			sendRequest("POST", getFullURL(url), null, toUTF8Buffer(data), callback, errorCallback);
		}

		public void doPut(string url, cape.DynamicMap data, System.Action<cape.DynamicMap> callback, System.Action<cape.Error> errorCallback = null) {
			sendRequest("PUT", getFullURL(url), null, toUTF8Buffer(data), callback, errorCallback);
		}

		public void doDelete(string url, System.Action<cape.DynamicMap> callback, System.Action<cape.Error> errorCallback = null) {
			sendRequest("DELETE", getFullURL(url), null, null, callback, errorCallback);
		}

		public void uploadFile(string url, byte[] data, string mimeType, System.Action<cape.DynamicMap> callback, System.Action<cape.Error> errorCallback = null) {
			var mt = mimeType;
			if(cape.String.isEmpty(mt)) {
				mt = "application/octet-stream";
			}
			var hdrs = new cape.KeyValueList<string, string>();
			hdrs.add((string)"content-type", (string)mt);
			sendRequest("POST", getFullURL(url), hdrs, data, callback, errorCallback);
		}

		public string getApiUrl() {
			return(apiUrl);
		}

		public JSONAPIClient setApiUrl(string v) {
			apiUrl = v;
			return(this);
		}
	}
}
