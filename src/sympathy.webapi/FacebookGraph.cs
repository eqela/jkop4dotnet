
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

namespace sympathy.webapi
{
	public class FacebookGraph
	{
		private class FacebookHTTPClientListener : HTTPClientListener
		{
			private System.Action<cape.DynamicMap, cape.Error> listener = null;
			private byte[] body = null;

			public override void onError(string message) {
				if(listener != null) {
					listener(null, cape.Error.forMessage(message));
				}
			}

			public override bool onDataReceived(byte[] buffer) {
				body = buffer;
				return(true);
			}

			public string replaceUnicodeChars(string response) {
				var str = response;
				if(cape.String.contains(response, "\\u0040")) {
					str = cape.String.replace(response, "\\u0040", "@");
				}
				if(cape.String.contains(response, "\\u00f1")) {
					str = cape.String.replace(str, "\\u00f1", "ñ");
				}
				return(str);
			}

			public override void onResponseCompleted() {
				base.onResponseCompleted();
				if(listener != null) {
					var json = cape.JSONParser.parse(sympathy.webapi.DataValidator.toValidJSONString(replaceUnicodeChars(cape.String.forUTF8Buffer(body)))) as cape.DynamicMap;
					if(json == null) {
						listener(null, cape.Error.forMessage("Invalid response JSON Format from Facebook"));
						return;
					}
					var error = json.getDynamicMap("error");
					if(error != null) {
						var e = cape.Error.forMessage(error.getString("message"));
						e.setCode(error.getString("code"));
						e.setDetail(error.getString("fbtrace_id"));
						listener(null, e);
						return;
					}
					listener(json, null);
				}
			}

			public System.Action<cape.DynamicMap, cape.Error> getListener() {
				return(listener);
			}

			public FacebookHTTPClientListener setListener(System.Action<cape.DynamicMap, cape.Error> v) {
				listener = v;
				return(this);
			}
		}

		public static void getUserProfile(string fbUserId, string fbAccessToken, System.Action<cape.DynamicMap, cape.Error> listener) {
			var op = new HTTPClientOperation();
			op.setAcceptInvalidCertificate(true);
			var req = sympathy.HTTPClientRequest.forGET((("https://graph.facebook.com/" + fbUserId) + "?fields=id,last_name,first_name,email&access_token=") + fbAccessToken);
			op.executeRequest(req, (HTTPClientListener)new FacebookHTTPClientListener().setListener(listener));
		}
	}
}
