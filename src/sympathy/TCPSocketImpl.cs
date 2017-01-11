
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
	public class TCPSocketImpl : TCPSocket
	{
		private System.Net.Sockets.Socket socket = null;
		private bool isConnected = false;

		~TCPSocketImpl() {
			close();
		}

		public System.Net.Sockets.Socket getNetSocket() {
			return(socket);
		}

		public override string getRemoteAddress() {
			if(socket == null) {
				return(null);
			}
			System.Net.IPEndPoint ep = null;
			try {
				ep = socket.RemoteEndPoint as System.Net.IPEndPoint;
			}
			catch(System.Exception e) {
				ep = null;
			}
			if(ep != null) {
				var add = ep.Address as System.Net.IPAddress;
				var str = add.ToString() as string;
				return(str);
			}
			return(null);
		}

		public override int getRemotePort() {
			if(socket == null) {
				return(0);
			}
			var ep = socket.RemoteEndPoint as System.Net.IPEndPoint;
			if(ep != null) {
				var port = (int)ep.Port;
				return(port);
			}
			return(0);
		}

		public override string getLocalAddress() {
			if(socket == null) {
				return(null);
			}
			var ep = socket.LocalEndPoint as System.Net.IPEndPoint;
			if(ep != null) {
				var add = (System.Net.IPAddress)ep.Address;
				var str = (string)add.ToString();
				return(str);
			}
			return(null);
		}

		public override int getLocalPort() {
			if(socket == null) {
				return(0);
			}
			var ep = socket.LocalEndPoint as System.Net.IPEndPoint;
			if(ep != null) {
				var port = (int)ep.Port;
				return(port);
			}
			return(0);
		}

		public override bool listen(int port) {
			var v = true;
			try {
				socket = new System.Net.Sockets.Socket(
				System.Net.Sockets.AddressFamily.InterNetwork,
				System.Net.Sockets.SocketType.Stream,
				System.Net.Sockets.ProtocolType.Tcp);
				socket.NoDelay = true;
				var ep = new System.Net.IPEndPoint(System.Net.IPAddress.Any, port);
				socket.Bind(ep);
				socket.Listen(256);
			}
			catch(System.Exception e) {
				v = false;
			}
			if(v == false) {
				close();
			}
			return(v);
		}

		public override TCPSocket accept() {
			if(socket == null) {
				return(null);
			}
			System.Net.Sockets.Socket nsocket = null;
			try {
				nsocket = socket.Accept();
			}
			catch(System.Exception e) {
				nsocket = null;
			}
			if(nsocket != null) {
				nsocket.NoDelay = true;
				var v = new TCPSocketImpl();
				v.socket = nsocket;
				return((TCPSocket)v);
			}
			return(null);
		}

		public override bool setBlocking(bool blocking) {
			var v = false;
			if(socket != null) {
				socket.Blocking = blocking;
				v = true;
			}
			return(v);
		}

		public override bool connect(string address, int port) {
			if(object.Equals(address, null)) {
				return(false);
			}
			try {
				socket = new System.Net.Sockets.Socket(
				System.Net.Sockets.AddressFamily.InterNetwork,
				System.Net.Sockets.SocketType.Stream,
				System.Net.Sockets.ProtocolType.Tcp);
				socket.NoDelay = true;
				socket.Connect(address, port);
				isConnected = true;
			}
			catch(System.Exception e) {
				socket = null;
			}
			return(isConnected);
		}

		public override void close() {
			if(socket == null) {
				return;
			}
			try {
				socket.Shutdown(System.Net.Sockets.SocketShutdown.Both);
			}
			catch(System.Exception e) {
				;
			}
			socket.Close();
			socket = null;
		}

		public override int read(byte[] buffer) {
			return(readWithTimeout(buffer, -1));
		}

		public override int readWithTimeout(byte[] buffer, int timeout) {
			if(buffer == null) {
				return(0);
			}
			var v = 0;
			try {
				if(timeout == 0) {
					socket.Blocking = false;
					socket.ReceiveTimeout = 1;
				}
				else {
					socket.Blocking = true;
					socket.ReceiveTimeout = timeout;
				}
				v = socket.Receive(buffer);
			}
			catch(System.Net.Sockets.SocketException e) {
				if(e.ErrorCode == 10035) { // WSAEWOULDBLOCK
				return(0);
			}
			else {
				v = -1;
			}
		}
		catch(System.Exception e) {
			v = -1;
		}
		if(v < 1) {
			close();
			v = -1;
		}
		return(v);
	}

	public override int write(byte[] buffer, int size) {
		if(buffer == null) {
			return(-1);
		}
		var v = 0;
		int nsize = size;
		if(nsize < 0) {
			nsize =  buffer.Length;
		}
		try {
			v = socket.Send(buffer, 0, nsize, 0);
		}
		catch(System.Net.Sockets.SocketException e) {
			if(e.ErrorCode == 10035) { // WSAEWOULDBLOCK
			return(0);
		}
		else {
			v = -1;
		}
	}
	catch(System.Exception e) {
		v = -1;
	}
	if(v < 1) {
		close();
		v = -1;
	}
	return(v);
}
}
}
