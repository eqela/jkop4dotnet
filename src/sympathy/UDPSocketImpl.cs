
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
	public class UDPSocketImpl : UDPSocket
	{
		public override int send(byte[] message, string address, int port) {
			System.Console.WriteLine("--- stub --- sympathy.UDPSocketImpl :: send");
			return(0);
		}

		public override int sendBroadcast(byte[] message, string address, int port) {
			System.Console.WriteLine("--- stub --- sympathy.UDPSocketImpl :: sendBroadcast");
			return(0);
		}

		public override int receive(byte[] message, int timeout) {
			System.Console.WriteLine("--- stub --- sympathy.UDPSocketImpl :: receive");
			return(0);
		}

		public override bool bind(int port) {
			System.Console.WriteLine("--- stub --- sympathy.UDPSocketImpl :: bind");
			return(false);
		}

		public override void close() {
			System.Console.WriteLine("--- stub --- sympathy.UDPSocketImpl :: close");
		}

		public override string getLocalAddress() {
			System.Console.WriteLine("--- stub --- sympathy.UDPSocketImpl :: getLocalAddress");
			return(null);
		}

		public override int getLocalPort() {
			System.Console.WriteLine("--- stub --- sympathy.UDPSocketImpl :: getLocalPort");
			return(0);
		}
	}
}
