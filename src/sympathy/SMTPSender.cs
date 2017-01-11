
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
	public class SMTPSender
	{
		public static SMTPSender forServerAddress(string name, cape.LoggingContext ctx) {
			return(new SMTPSender().setThisServerAddress(name).setCtx(ctx));
		}

		public static SMTPSender forConfiguration(cape.DynamicMap config, cape.LoggingContext ctx) {
			return(new SMTPSender().setCtx(ctx).configure(config));
		}

		private string thisServerAddress = null;
		private string server = null;
		private string myName = null;
		private string myAddress = null;
		private cape.LoggingContext ctx = null;
		private int maxSenderCount = 0;
		private string serverInternal = null;
		private int senderCount = 0;

		public SMTPSender() {
			thisServerAddress = "unknown.server.com";
		}

		public string getDescription() {
			var sb = new cape.StringBuilder();
			if(cape.String.isEmpty(myName) == false) {
				sb.append('\"');
				sb.append(myName);
				sb.append('\"');
			}
			if(cape.String.isEmpty(myAddress) == false) {
				var hasName = false;
				if(sb.count() > 0) {
					hasName = true;
				}
				if(hasName) {
					sb.append(' ');
					sb.append('<');
				}
				sb.append(myAddress);
				if(hasName) {
					sb.append('>');
				}
			}
			var s = serverInternal;
			if(cape.String.isEmpty(s)) {
				s = server;
			}
			if(cape.String.isEmpty(s) == false) {
				sb.append(' ');
				sb.append('(');
				sb.append(s);
				sb.append(')');
			}
			if(sb.count() < 1) {
				sb.append("(no configuration; raw passhtrough of messages)");
			}
			return(sb.toString());
		}

		public SMTPSender configure(cape.DynamicMap config) {
			if(config == null) {
				return(this);
			}
			var defaultPort = "25";
			var scheme = config.getString("server_type", "smtp");
			if(cape.String.equals("smtp+ssl", scheme)) {
				defaultPort = "465";
			}
			var url = new URL().setScheme(scheme).setUsername(capex.URLEncoder.encode(config.getString("server_username"))).setPassword(capex.URLEncoder.encode(config.getString("server_password"))).setHost(config.getString("server_address")).setPort(config.getString("server_port", defaultPort));
			setServer(url.toString());
			url.setPassword(null);
			serverInternal = url.toString();
			setMyName(config.getString("sender_name", "SMTP"));
			setMyAddress(config.getString("sender_address", "my@address.com"));
			setThisServerAddress(config.getString("this_server_address", thisServerAddress));
			return(this);
		}

		public void onSendStart() {
			senderCount++;
			cape.Log.debug(ctx, ("SMTP send start: Now " + cape.String.forInteger(senderCount)) + " sender(s)");
		}

		public void onSendEnd() {
			senderCount--;
			cape.Log.debug(ctx, ("SMTP send end: Now " + cape.String.forInteger(senderCount)) + " sender(s)");
		}

		public void send(SMTPMessage msg, SMTPSenderListener listener) {
			if(msg == null) {
				if(listener != null) {
					listener.onSMTPSendComplete(msg, sympathy.SMTPClientResult.forError("No message to send"));
				}
				return;
			}
			var rcpts = msg.getAllRcpts();
			if((rcpts == null) || (rcpts.getSize() < 1)) {
				if(listener != null) {
					listener.onSMTPSendComplete(msg, sympathy.SMTPClientResult.forSuccess());
				}
				return;
			}
			if((maxSenderCount > 0) && (senderCount > maxSenderCount)) {
				cape.Log.warning(ctx, "Reached maximum sender count " + cape.String.forInteger(maxSenderCount));
				if(listener != null) {
					listener.onSMTPSendComplete(msg, sympathy.SMTPClientResult.forError("Maximum number of SMTP senders has been exceeded."));
				}
				return;
			}
			if(cape.String.isEmpty(myName) == false) {
				msg.setMyName(myName);
			}
			if(cape.String.isEmpty(myAddress) == false) {
				msg.setMyAddress(myAddress);
			}
			var sct = new SMTPClientTask();
			if(cape.String.isEmpty(server) == false) {
				sct.setServer(sympathy.URL.forString(server));
			}
			sct.setCtx(ctx);
			sct.setServerAddress(thisServerAddress);
			sct.setMsg(msg);
			sct.setListener(listener);
			sct.setSender(this);
			if(cape.Thread.start((cape.Runnable)sct) == false) {
				cape.Log.error(ctx, "Failed to start SMTP sender background task");
				if(listener != null) {
					listener.onSMTPSendComplete(msg, sympathy.SMTPClientResult.forError("Failed to start SMTP sender background task"));
				}
				return;
			}
			onSendStart();
		}

		public string getThisServerAddress() {
			return(thisServerAddress);
		}

		public SMTPSender setThisServerAddress(string v) {
			thisServerAddress = v;
			return(this);
		}

		public string getServer() {
			return(server);
		}

		public SMTPSender setServer(string v) {
			server = v;
			return(this);
		}

		public string getMyName() {
			return(myName);
		}

		public SMTPSender setMyName(string v) {
			myName = v;
			return(this);
		}

		public string getMyAddress() {
			return(myAddress);
		}

		public SMTPSender setMyAddress(string v) {
			myAddress = v;
			return(this);
		}

		public cape.LoggingContext getCtx() {
			return(ctx);
		}

		public SMTPSender setCtx(cape.LoggingContext v) {
			ctx = v;
			return(this);
		}

		public int getMaxSenderCount() {
			return(maxSenderCount);
		}

		public SMTPSender setMaxSenderCount(int v) {
			maxSenderCount = v;
			return(this);
		}
	}
}
