
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
	public class HTTPClientListener
	{
		private bool completed = false;

		public virtual void onError(string message) {
		}

		public virtual void onStatus(string message) {
		}

		public virtual bool onStartRequest(HTTPClientRequest request) {
			return(true);
		}

		public virtual bool onEndRequest() {
			return(true);
		}

		public virtual bool onResponseReceived(HTTPClientResponse response) {
			return(true);
		}

		public virtual bool onDataReceived(byte[] buffer) {
			return(true);
		}

		public virtual void onAborted() {
		}

		public virtual void onResponseCompleted() {
			completed = true;
		}

		public bool getCompleted() {
			return(completed);
		}

		public HTTPClientListener setCompleted(bool v) {
			completed = v;
			return(this);
		}
	}
}
