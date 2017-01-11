
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
	public class DNSResolverForDotNet : DNSResolver
	{
		public override cape.DynamicVector getNSRecords(string host, string type, cape.LoggingContext ctx) {
			return(null);
		}

		public override string getIPAddress(string hostname, cape.LoggingContext ctx) {
			var v = getIPAddresses(hostname, ctx);
			if((v == null) || (v.getSize() < 1)) {
				return(null);
			}
			var ip = v.get(0) as string;
			if(!(object.Equals(ip, null))) {
				return(ip);
			}
			return(null);
		}

		public override cape.DynamicVector getIPAddresses(string hostname, cape.LoggingContext ctx) {
			var v = new cape.DynamicVector();
			foreach(System.Net.IPAddress address in System.Net.Dns.GetHostAddresses(hostname)) {
				System.Console.WriteLine("SMTP ADDRESS: " + address.ToString());
				v.append((object)(address.ToString()));
			}
			return(v);
		}
	}
}
