
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
	public class SMTPClient
	{
		private static TCPSocket connect(string server, int port, cape.LoggingContext ctx) {
			if(cape.String.isEmpty(server) || (port < 1)) {
				return(null);
			}
			var address = server;
			var dns = sympathy.DNSResolver.create();
			if(dns != null) {
				address = dns.getIPAddress(server, ctx);
				if(cape.String.isEmpty(address)) {
					cape.Log.error(ctx, ("SMTPClient: Could not resolve SMTP server address: `" + server) + "'");
					return(null);
				}
			}
			cape.Log.debug(ctx, ((("SMTPClient: Connecting to SMTP server `" + address) + ":") + cape.String.forInteger(port)) + "' ..");
			var v = sympathy.TCPSocket.createAndConnect(address, port);
			if(v != null) {
				cape.Log.debug(ctx, ((("SMTPClient: Connected to SMTP server `" + address) + ":") + cape.String.forInteger(port)) + "' ..");
			}
			else {
				cape.Log.error(ctx, ((("SMTPClient: FAILED connection to SMTP server `" + address) + ":") + cape.String.forInteger(port)) + "' ..");
			}
			return(v);
		}

		private static bool writeLine(cape.PrintWriter ops, string str) {
			return(ops.print(str + "\r\n"));
		}

		private static string communicate(cape.PrintReader ins, string expectCode) {
			var sb = new cape.StringBuilder();
			var line = ins.readLine();
			if(cape.String.isEmpty(line) == false) {
				sb.append(line);
			}
			while((cape.String.isEmpty(line) == false) && (cape.String.getChar(line, 3) == '-')) {
				line = ins.readLine();
				if(cape.String.isEmpty(line) == false) {
					sb.append(line);
				}
			}
			if((cape.String.isEmpty(line) == false) && (cape.String.getChar(line, 3) == ' ')) {
				if(cape.String.isEmpty(expectCode)) {
					return(null);
				}
				var rc = cape.String.getSubString(line, 0, 3);
				var array = cape.String.split(expectCode, '|');
				if(array != null) {
					var n = 0;
					var m = array.Count;
					for(n = 0 ; n < m ; n++) {
						var cc = array[n];
						if(cc != null) {
							if(cape.String.equals(cc, rc)) {
								return(null);
							}
						}
					}
				}
			}
			var v = sb.toString();
			if(cape.String.isEmpty(v)) {
				v = "XXX Unknown SMTP server error response";
			}
			return(v);
		}

		private static string encode(string enc) {
			if(cape.String.isEmpty(enc)) {
				return(null);
			}
			return(capex.Base64Encoder.encode(cape.String.toUTF8Buffer(enc)));
		}

		private static string rcptAsEmailAddress(string ss) {
			if(cape.String.isEmpty(ss)) {
				return(ss);
			}
			var b = cape.String.getIndexOf(ss, '<');
			if(b < 0) {
				return(ss);
			}
			var e = cape.String.getIndexOf(ss, '>');
			if(e < 0) {
				return(ss);
			}
			return(cape.String.getSubString(ss, b + 1, (e - b) - 1));
		}

		private static string resolveMXServerForDomain(string domain) {
			var dns = sympathy.DNSResolver.instance();
			if(dns == null) {
				return(null);
			}
			var rcs = dns.getNSRecords(domain, "MX", null);
			if((rcs == null) || (rcs.getSize() < 1)) {
				return(null);
			}
			string v = null;
			var pr = 0;
			var array = rcs.toVector();
			if(array != null) {
				var n = 0;
				var m = array.Count;
				for(n = 0 ; n < m ; n++) {
					var mx = array[n] as DNSRecordMX;
					if(mx != null) {
						var p = mx.getPriority();
						if((object.Equals(v, null)) || (p < pr)) {
							pr = p;
							v = mx.getAddress();
						}
					}
				}
			}
			return(v);
		}

		public static SMTPClientResult sendMessage(SMTPMessage msg, URL server, string serverName, cape.LoggingContext ctx = null) {
			if(msg == null) {
				return(sympathy.SMTPClientResult.forMessage(msg).addTransaction(sympathy.SMTPClientTransactionResult.forError("No message")));
			}
			var rcpts = msg.getAllRcpts();
			if(server != null) {
				var t = executeTransaction(msg, server, rcpts, serverName, ctx);
				if(t != null) {
					t.setServer(server.getHost());
					t.setRecipients(rcpts);
				}
				return(sympathy.SMTPClientResult.forMessage(msg).addTransaction(t));
			}
			var r = sympathy.SMTPClientResult.forMessage(msg);
			var servers = new cape.DynamicMap();
			var array = rcpts.toVector();
			if(array != null) {
				var n = 0;
				var m = array.Count;
				for(n = 0 ; n < m ; n++) {
					var rcpt = array[n] as string;
					if(rcpt != null) {
						var em = rcptAsEmailAddress(rcpt);
						if(cape.String.isEmpty(em)) {
							r.addTransaction(sympathy.SMTPClientTransactionResult.forError(("Invalid recipient address: `" + rcpt) + "'"));
							break;
						}
						var at = cape.String.getIndexOf(em, '@');
						if(at < 0) {
							r.addTransaction(sympathy.SMTPClientTransactionResult.forError(("Invalid recipient address: `" + rcpt) + "'"));
							break;
						}
						var sa = cape.String.getSubString(em, at + 1);
						if(cape.String.isEmpty(sa)) {
							r.addTransaction(sympathy.SMTPClientTransactionResult.forError(("Invalid recipient address: `" + rcpt) + "'"));
							break;
						}
						var ss = servers.get(sa) as cape.DynamicVector;
						if(ss == null) {
							ss = new cape.DynamicVector();
							servers.set(sa, (object)ss);
						}
						ss.append((object)rcpt);
					}
				}
			}
			var itr = servers.iterateKeys();
			while(itr != null) {
				var domain = itr.next();
				if(cape.String.isEmpty(domain)) {
					break;
				}
				var ds = resolveMXServerForDomain(domain);
				if(cape.String.isEmpty(ds)) {
					r.addTransaction(sympathy.SMTPClientTransactionResult.forError(("Unable to determine mail server for `" + domain) + "'"));
				}
				else {
					cape.Log.debug(ctx, ((("SMTP server for domain `" + domain) + "': `") + ds) + "'");
					var trcpts = servers.get(domain) as cape.DynamicVector;
					var t1 = executeTransaction(msg, sympathy.URL.forString("smtp://" + ds), trcpts, serverName, ctx);
					if(t1 != null) {
						t1.setDomain(domain);
						t1.setServer(ds);
						t1.setRecipients(trcpts);
					}
					r.addTransaction(t1);
				}
			}
			var vt = r.getTransactions();
			if((vt == null) || (vt.getSize() < 1)) {
				r.addTransaction(sympathy.SMTPClientTransactionResult.forError("Unknown error in SMTPClient"));
			}
			return(r);
		}

		private static SMTPClientTransactionResult executeTransaction(SMTPMessage msg, URL server, cape.DynamicVector rcpts, string serverName, cape.LoggingContext ctx) {
			var url = server;
			if(url == null) {
				return(sympathy.SMTPClientTransactionResult.forError("No server URL"));
			}
			ConnectedSocket socket = null;
			var scheme = url.getScheme();
			var host = url.getHost();
			var port = url.getPortInt();
			var n = 0;
			for(n = 0 ; n < 3 ; n++) {
				if(cape.String.equals("smtp", scheme) || cape.String.equals("smtp+tls", scheme)) {
					if(port < 1) {
						port = 25;
					}
					socket = (ConnectedSocket)connect(host, port, ctx);
				}
				else if(cape.String.equals("smtp+ssl", scheme)) {
					if(port < 1) {
						port = 465;
					}
					var ts = connect(host, port, ctx);
					if(ts != null) {
						socket = (ConnectedSocket)sympathy.SSLSocket.forClient((ConnectedSocket)ts, host);
						if(socket == null) {
							return(sympathy.SMTPClientTransactionResult.forError("Failed to start SSL"));
						}
					}
				}
				else {
					return(sympathy.SMTPClientTransactionResult.forError(("SMTPClient: Unknown SMTP URI scheme `" + scheme) + "'"));
				}
				if(socket != null) {
					break;
				}
				cape.Log.debug(ctx, ((("Failed to connect to SMTP server `" + host) + ":") + cape.String.forInteger(port)) + "'. Waiting to retry ..");
				cape.Log.error(ctx, "FIXME - No sleep implementation yet, quitting instead!");
				break;
			}
			if(socket == null) {
				return(sympathy.SMTPClientTransactionResult.forError(((("Unable to connect to SMTP server `" + host) + ":") + cape.String.forInteger(port)) + "'"));
			}
			var ops = cape.PrintWriterWrapper.forWriter((cape.Writer)socket);
			var ins = new cape.PrintReader((cape.Reader)socket);
			if((ops == null) || (ins == null)) {
				return(sympathy.SMTPClientTransactionResult.forError("Unable to establish SMTP I/O streams"));
			}
			string err = null;
			if(!(object.Equals(err = communicate(ins, "220"), null))) {
				return(sympathy.SMTPClientTransactionResult.forError(err));
			}
			var sn = serverName;
			if(cape.String.isEmpty(sn)) {
				sn = "eq.net.smtpclient";
			}
			if(writeLine(ops, "EHLO " + sn) == false) {
				return(sympathy.SMTPClientTransactionResult.forNetworkError());
			}
			if(!(object.Equals(err = communicate(ins, "250"), null))) {
				return(sympathy.SMTPClientTransactionResult.forError(err));
			}
			if(cape.String.equals("smtp+tls", scheme)) {
				if(writeLine(ops, "STARTTLS") == false) {
					return(sympathy.SMTPClientTransactionResult.forNetworkError());
				}
				if(!(object.Equals(err = communicate(ins, "220"), null))) {
					return(sympathy.SMTPClientTransactionResult.forError(err));
				}
				ops = null;
				ins = null;
				socket = (ConnectedSocket)sympathy.SSLSocket.forClient(socket, host);
				if(socket == null) {
					return(sympathy.SMTPClientTransactionResult.forError("Failed to start SSL"));
				}
				ops = cape.PrintWriterWrapper.forWriter((cape.Writer)socket);
				ins = new cape.PrintReader((cape.Reader)socket);
			}
			var username = url.getUsername();
			var password = url.getPassword();
			if(cape.String.isEmpty(username) == false) {
				if(writeLine(ops, "AUTH login") == false) {
					return(sympathy.SMTPClientTransactionResult.forNetworkError());
				}
				if(!(object.Equals(err = communicate(ins, "334"), null))) {
					return(sympathy.SMTPClientTransactionResult.forError(err));
				}
				if(writeLine(ops, encode(username)) == false) {
					return(sympathy.SMTPClientTransactionResult.forNetworkError());
				}
				if(!(object.Equals(err = communicate(ins, "334"), null))) {
					return(sympathy.SMTPClientTransactionResult.forError(err));
				}
				if(writeLine(ops, encode(password)) == false) {
					return(sympathy.SMTPClientTransactionResult.forNetworkError());
				}
				if(!(object.Equals(err = communicate(ins, "235|530"), null))) {
					return(sympathy.SMTPClientTransactionResult.forError(err));
				}
			}
			if(writeLine(ops, ("MAIL FROM:<" + msg.getMyAddress()) + ">") == false) {
				return(sympathy.SMTPClientTransactionResult.forNetworkError());
			}
			if(!(object.Equals(err = communicate(ins, "250"), null))) {
				return(sympathy.SMTPClientTransactionResult.forError(err));
			}
			if(rcpts != null) {
				var array = rcpts.toVector();
				if(array != null) {
					var n2 = 0;
					var m = array.Count;
					for(n2 = 0 ; n2 < m ; n2++) {
						var rcpt = array[n2] as string;
						if(rcpt != null) {
							if(writeLine(ops, ("RCPT TO:<" + rcptAsEmailAddress(rcpt)) + ">") == false) {
								return(sympathy.SMTPClientTransactionResult.forNetworkError());
							}
							if(!(object.Equals(err = communicate(ins, "250"), null))) {
								return(sympathy.SMTPClientTransactionResult.forError(err));
							}
						}
					}
				}
			}
			if(writeLine(ops, "DATA") == false) {
				return(sympathy.SMTPClientTransactionResult.forNetworkError());
			}
			if(!(object.Equals(err = communicate(ins, "354"), null))) {
				return(sympathy.SMTPClientTransactionResult.forError(err));
			}
			if(cape.String.isEmpty(msg.getMessageID())) {
				msg.generateMessageID(sn);
			}
			var bod = msg.getMessageBody();
			cape.Log.debug(ctx, ("Sending message body: `" + bod) + "'");
			if(ops.print(bod) == false) {
				return(sympathy.SMTPClientTransactionResult.forNetworkError());
			}
			if(ops.print("\r\n.\r\n") == false) {
				return(sympathy.SMTPClientTransactionResult.forNetworkError());
			}
			if(!(object.Equals(err = communicate(ins, "250"), null))) {
				return(sympathy.SMTPClientTransactionResult.forError(err));
			}
			if(writeLine(ops, "QUIT") == false) {
				return(sympathy.SMTPClientTransactionResult.forNetworkError());
			}
			return(sympathy.SMTPClientTransactionResult.forSuccess());
		}
	}
}
