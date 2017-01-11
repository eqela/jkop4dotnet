
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
	public class GoogleMaps
	{
		private class GoogleMapsHTTPClientListener : HTTPClientListener
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

			public override void onResponseCompleted() {
				base.onResponseCompleted();
				if(listener != null) {
					var json = cape.JSONParser.parse(sympathy.webapi.DataValidator.toValidJSONString(cape.String.forUTF8Buffer(body))) as cape.DynamicMap;
					if(json == null) {
						listener(null, cape.Error.forCode("invalidResponse"));
						return;
					}
					if(cape.String.equals("INVALID_REQUEST", json.getString("status"))) {
						listener(null, cape.Error.forCode("invalidRequest"));
						return;
					}
					listener(json, null);
				}
			}

			public System.Action<cape.DynamicMap, cape.Error> getListener() {
				return(listener);
			}

			public GoogleMapsHTTPClientListener setListener(System.Action<cape.DynamicMap, cape.Error> v) {
				listener = v;
				return(this);
			}
		}

		public static void getTravelDetails(string apiKey, string origin, string destination, System.Action<cape.DynamicMap, cape.Error> listener) {
			if(cape.String.isEmpty(apiKey)) {
				if(listener != null) {
					listener(null, cape.Error.forCode("apiKeyMissing"));
				}
				return;
			}
			var op = new HTTPClientOperation();
			op.setAcceptInvalidCertificate(true);
			HTTPClientRequest req = null;
			req = sympathy.HTTPClientRequest.forGET((((("https://maps.googleapis.com/maps/api/distancematrix/json?units=imperial&origins=" + capex.URLEncoder.encode(origin)) + "&destinations=") + capex.URLEncoder.encode(destination)) + "&key=") + apiKey);
			op.executeRequest(req, (HTTPClientListener)new GoogleMapsHTTPClientListener().setListener(listener));
		}
	}
}
