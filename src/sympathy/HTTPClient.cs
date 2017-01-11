
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
	public abstract class HTTPClient
	{
		public static HTTPClient createDefault() {
			return((HTTPClient)new HTTPClientOperation());
		}

		public static void execute(HTTPClient client, HTTPClientRequest request, HTTPClientListener listener) {
			if(client == null) {
				return;
			}
			client.executeRequest(request, listener);
		}

		private class MyListener : HTTPClientListener
		{
			private HTTPClientResponse response = null;
			private byte[] buffer = null;

			public override bool onResponseReceived(HTTPClientResponse response) {
				this.response = response;
				return(true);
			}

			public override bool onDataReceived(byte[] buffer) {
				this.buffer = cape.Buffer.append(this.buffer, buffer);
				return(true);
			}

			public override void onAborted() {
			}

			public override void onError(string message) {
			}

			public override void onResponseCompleted() {
			}

			public HTTPClientResponse getResponse() {
				return(response);
			}

			public MyListener setResponse(HTTPClientResponse v) {
				response = v;
				return(this);
			}

			public byte[] getBuffer() {
				return(buffer);
			}

			public MyListener setBuffer(byte[] v) {
				buffer = v;
				return(this);
			}
		}

		public static void execute(HTTPClient client, HTTPClientRequest request, System.Action<HTTPClientResponse> listener) {
			var ll = new MyListener();
			sympathy.HTTPClient.execute(client, request, (HTTPClientListener)ll);
			if(listener != null) {
				listener(ll.getResponse());
			}
		}

		public static void executeForBuffer(HTTPClient client, HTTPClientRequest request, System.Action<HTTPClientResponse, byte[]> listener) {
			var ll = new MyListener();
			execute(client, request, (HTTPClientListener)ll);
			if(listener != null) {
				listener(ll.getResponse(), ll.getBuffer());
			}
		}

		public static void executeForString(HTTPClient client, HTTPClientRequest request, System.Action<HTTPClientResponse, string> listener) {
			var ll = new MyListener();
			execute(client, request, (HTTPClientListener)ll);
			if(listener != null) {
				listener(ll.getResponse(), cape.String.forUTF8Buffer(ll.getBuffer()));
			}
		}

		public abstract void executeRequest(HTTPClientRequest request, HTTPClientListener listener);
	}
}
