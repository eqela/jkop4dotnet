
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
	public abstract class TCPClient
	{
		public static TCPClient forServerAndConnection(cape.LoggingContext ctx, string address, int port, NetworkConnection connection) {
			var v = new TCPClientImpl();
			v.setConnection(connection);
			if(v.start(ctx, address, port) == false) {
				v = null;
			}
			return((TCPClient)v);
		}

		protected NetworkConnection connection = null;

		public TCPClient setConnection(NetworkConnection p) {
			connection = p;
			return(this);
		}

		public NetworkConnection getConnection() {
			return(connection);
		}

		public abstract bool start(cape.LoggingContext ctx, string address, int port);
		public abstract void stop();
	}
}
