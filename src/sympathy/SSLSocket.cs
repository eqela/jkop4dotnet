
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
	public abstract class SSLSocket : ConnectedSocket
	{
		public static SSLSocket createInstance(ConnectedSocket cSocket, string serverAddress = null, cape.LoggingContext ctx = null, cape.File certFile = null, cape.File keyFile = null, bool isServer = false, bool acceptInvalidCertificate = false) {
			if(cSocket == null) {
				return(null);
			}
			SSLSocket v = null;
			var sslStream = new SSLSocketDotNet();
			sslStream.setAcceptInvalidCertificate(acceptInvalidCertificate);
			sslStream.setCtx(ctx);
			sslStream.setServerAddress(serverAddress);
			if(sslStream.open(cSocket, certFile, keyFile, isServer)) {
				v = (SSLSocket)sslStream;
			}
			return(v);
		}

		public static SSLSocket forClient(ConnectedSocket cSocket, string hostAddress, cape.LoggingContext ctx = null, bool acceptInvalidCertificate = false) {
			return(createInstance(cSocket, hostAddress, ctx, null, null, false, acceptInvalidCertificate));
		}

		public static SSLSocket forServer(ConnectedSocket cSocket, cape.File certFile = null, cape.File keyFile = null, cape.LoggingContext ctx = null) {
			return(createInstance(cSocket, null, ctx, certFile, keyFile, true));
		}

		public abstract void setAcceptInvalidCertificate(bool accept);
		public abstract void close();
		public abstract int read(byte[] buffer);
		public abstract int readWithTimeout(byte[] buffer, int timeout);
		public abstract int write(byte[] buffer, int size);
		public abstract ConnectedSocket getSocket();
	}
}
