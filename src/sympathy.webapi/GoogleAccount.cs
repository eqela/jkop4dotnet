
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
	public class GoogleAccount
	{
		private class MyHTTPClientListener : HTTPClientListener
		{
			private System.Action<cape.DynamicMap, cape.Error> listener = null;
			private byte[] body = null;

			public override void onError(string message) {
				if(listener != null) {
					listener(null, cape.Error.forMessage(message));
				}
			}

			public override void onAborted() {
				if(listener != null) {
					listener(null, cape.Error.forCode("aborted"));
				}
			}

			public override bool onDataReceived(byte[] buffer) {
				body = cape.Buffer.append(body, buffer);
				return(true);
			}

			public override void onResponseCompleted() {
				base.onResponseCompleted();
				if(listener != null) {
					var json = cape.JSONParser.parse(body) as cape.DynamicMap;
					if(json == null) {
						listener(null, cape.Error.forMessage("Invalid JSON Format from Google"));
						return;
					}
					listener(json, null);
				}
			}

			public System.Action<cape.DynamicMap, cape.Error> getListener() {
				return(listener);
			}

			public MyHTTPClientListener setListener(System.Action<cape.DynamicMap, cape.Error> v) {
				listener = v;
				return(this);
			}
		}

		public static void getTokenInfo(string token, System.Action<cape.DynamicMap, cape.Error> listener) {
			var op = new HTTPClientOperation();
			op.setAcceptInvalidCertificate(true);
			var req = sympathy.HTTPClientRequest.forGET("https://www.googleapis.com/oauth2/v3/tokeninfo?id_token=" + capex.URLEncoder.encode(token));
			op.executeRequest(req, (HTTPClientListener)new MyHTTPClientListener().setListener(listener));
		}
	}
}
