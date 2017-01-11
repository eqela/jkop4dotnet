
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
	public class SSLSocketDotNet : SSLSocket
	{
		private cape.LoggingContext ctx = null;
		private TCPSocket socket = null;
		private string serverAddress = null;
		private bool acceptInvalidCertificate = false;

		public override void setAcceptInvalidCertificate(bool v) {
			acceptInvalidCertificate = v;
		}

		System.Net.Security.SslStream sslStream;
		System.Collections.Hashtable certificateErrors = new System.Collections.Hashtable();
		public bool ValidateServerCertificate(object sender, System.Security.Cryptography.X509Certificates.X509Certificate certificate, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors) {
			if(sslPolicyErrors == System.Net.Security.SslPolicyErrors.None) {
				return(true);
			}
			if(acceptInvalidCertificate) {
				System.Console.WriteLine("Certificate error: {0} (but accepting it)", sslPolicyErrors);
				return(true);
			}
			System.Console.WriteLine("Certificate error: {0} (rejecting it)", sslPolicyErrors);
			return(false);
		}

		public bool open(ConnectedSocket cSocket, cape.File certFile = null, cape.File keyFile = null, bool isServer = false) {
			socket = cSocket as TCPSocket;
			if(socket == null) {
				return(false);
			}
			var nsocket = ((TCPSocketImpl)socket).getNetSocket();
			sslStream = new System.Net.Security.SslStream(new System.Net.Sockets.NetworkStream(nsocket), false, new System.Net.Security.RemoteCertificateValidationCallback (ValidateServerCertificate), null);
			try {
				sslStream.AuthenticateAsClient(serverAddress);
			}
			catch (System.Security.Authentication.AuthenticationException e)
			{
				System.Console.WriteLine("Exception: {0}", e.Message);
				if (e.InnerException != null)
				{
					System.Console.WriteLine("Inner exception: {0}", e.InnerException.Message);
				}
				System.Console.WriteLine ("Authentication failed - closing the connection.");
				close();
				return(false);
			}
			return(true);
		}

		public override int read(byte[] buffer) {
			return(readWithTimeout(buffer, -1));
		}

		public override void close() {
			if(sslStream != null) {
				sslStream.Close();
				sslStream = null;
			}
			if(socket != null) {
				socket.close();
				socket = null;
			}
		}

		public override int readWithTimeout(byte[] buffer, int timeout) {
			if(buffer == null) {
				return(0);
			}
			var v = 0;
			if(sslStream != null && socket != null) {
				try {
					sslStream.ReadTimeout = timeout;
					v = sslStream.Read(buffer, 0, buffer.Length);
				}
				catch(System.Exception e) {
					v = -1;
				}
				if(v < 1) {
					close();
					v = -1;
				}
			}
			return(v);
		}

		public override int write(byte[] buffer, int size) {
			var sz = size;
			if(sslStream != null && socket != null) {
				try {
					if(size < 0) {
						sslStream.Write(buffer, 0, buffer.Length);
						sz = buffer.Length;
					}
					else {
						sslStream.Write(buffer, 0, size);
					}
				}
				catch (System.Security.Authentication.AuthenticationException e)
				{
					System.Console.WriteLine("Exception: {0}", e.Message);
					if (e.InnerException != null)
					{
						System.Console.WriteLine("Inner exception: {0}", e.InnerException.Message);
					}
					System.Console.WriteLine ("Authentication failed - closing the connection.");
					close();
					return(-1);
				}
			}
			return(sz);
		}

		public override ConnectedSocket getSocket() {
			return((ConnectedSocket)socket);
		}

		public cape.LoggingContext getCtx() {
			return(ctx);
		}

		public SSLSocketDotNet setCtx(cape.LoggingContext v) {
			ctx = v;
			return(this);
		}

		public SSLSocketDotNet setSocket(TCPSocket v) {
			socket = v;
			return(this);
		}

		public string getServerAddress() {
			return(serverAddress);
		}

		public SSLSocketDotNet setServerAddress(string v) {
			serverAddress = v;
			return(this);
		}
	}
}
