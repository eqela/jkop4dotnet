
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
	public class SMTPMessage
	{
		private cape.DynamicVector rcptsTo = null;
		private cape.DynamicVector rcptsCC = null;
		private cape.DynamicVector rcptsBCC = null;
		private string replyTo = null;
		private string subject = null;
		private string contentType = null;
		private string text = null;
		private string myName = null;
		private string myAddress = null;
		private string messageBody = null;
		private string messageID = null;
		private string date = null;
		private cape.DynamicVector excludeAddresses = null;
		private static int counter = 0;

		public SMTPMessage() {
			date = capex.VerboseDateTimeString.forNow();
		}

		private void onChanged() {
			messageBody = null;
		}

		public SMTPMessage generateMessageID(string host) {
			messageID = (((((cape.String.forInteger((int)cape.SystemClock.asSeconds()) + "-") + cape.String.forInteger((int)new cape.Random().nextInt((int)1000000))) + "-") + cape.String.forInteger(counter)) + "@") + host;
			counter++;
			onChanged();
			return(this);
		}

		public string getDate() {
			return(date);
		}

		public string getReplyTo() {
			return(replyTo);
		}

		public SMTPMessage setDate(string date) {
			this.date = date;
			onChanged();
			return(this);
		}

		public SMTPMessage setMessageID(string id) {
			messageID = id;
			onChanged();
			return(this);
		}

		public SMTPMessage setReplyTo(string v) {
			replyTo = v;
			onChanged();
			return(this);
		}

		public string getMessageID() {
			return(messageID);
		}

		private bool isExcludedAddress(string add) {
			if((excludeAddresses == null) || (excludeAddresses.getSize() < 1)) {
				return(false);
			}
			var array = excludeAddresses.toVector();
			if(array != null) {
				var n = 0;
				var m = array.Count;
				for(n = 0 ; n < m ; n++) {
					var ea = array[n] as string;
					if(ea != null) {
						if(cape.String.equals(ea, add)) {
							return(true);
						}
					}
				}
			}
			return(false);
		}

		public cape.DynamicVector getAllRcpts() {
			var rcpts = new cape.DynamicVector();
			if(rcptsTo != null) {
				var array = rcptsTo.toVector();
				if(array != null) {
					var n = 0;
					var m = array.Count;
					for(n = 0 ; n < m ; n++) {
						var r = array[n] as string;
						if(r != null) {
							if(isExcludedAddress(r)) {
								continue;
							}
							rcpts.append((object)r);
						}
					}
				}
			}
			if(rcptsCC != null) {
				var array2 = rcptsCC.toVector();
				if(array2 != null) {
					var n2 = 0;
					var m2 = array2.Count;
					for(n2 = 0 ; n2 < m2 ; n2++) {
						var r1 = array2[n2] as string;
						if(r1 != null) {
							if(isExcludedAddress(r1)) {
								continue;
							}
							rcpts.append((object)r1);
						}
					}
				}
			}
			if(rcptsBCC != null) {
				var array3 = rcptsBCC.toVector();
				if(array3 != null) {
					var n3 = 0;
					var m3 = array3.Count;
					for(n3 = 0 ; n3 < m3 ; n3++) {
						var r2 = array3[n3] as string;
						if(r2 != null) {
							if(isExcludedAddress(r2)) {
								continue;
							}
							rcpts.append((object)r2);
						}
					}
				}
			}
			return(rcpts);
		}

		public cape.DynamicVector getRcptsTo() {
			return(rcptsTo);
		}

		public cape.DynamicVector getRcptsCC() {
			return(rcptsCC);
		}

		public cape.DynamicVector getRcptsBCC() {
			return(rcptsBCC);
		}

		public string getSubject() {
			return(subject);
		}

		public string getContentType() {
			return(contentType);
		}

		public string getText() {
			return(text);
		}

		public string getMyName() {
			return(myName);
		}

		public string getMyAddress() {
			return(myAddress);
		}

		public SMTPMessage setSubject(string s) {
			subject = s;
			onChanged();
			return(this);
		}

		public SMTPMessage setContentType(string c) {
			contentType = c;
			onChanged();
			return(this);
		}

		public SMTPMessage setText(string t) {
			text = t;
			onChanged();
			return(this);
		}

		public SMTPMessage setMyName(string n) {
			myName = n;
			onChanged();
			return(this);
		}

		public SMTPMessage setMyAddress(string a) {
			myAddress = a;
			onChanged();
			return(this);
		}

		public SMTPMessage setTo(string address) {
			rcptsTo = new cape.DynamicVector();
			rcptsTo.append((object)address);
			onChanged();
			return(this);
		}

		public SMTPMessage addTo(string address) {
			if(cape.String.isEmpty(address) == false) {
				if(rcptsTo == null) {
					rcptsTo = new cape.DynamicVector();
				}
				rcptsTo.append((object)address);
			}
			onChanged();
			return(this);
		}

		public SMTPMessage addCC(string address) {
			if(cape.String.isEmpty(address) == false) {
				if(rcptsCC == null) {
					rcptsCC = new cape.DynamicVector();
				}
				rcptsCC.append((object)address);
			}
			onChanged();
			return(this);
		}

		public SMTPMessage addBCC(string address) {
			if(cape.String.isEmpty(address) == false) {
				if(rcptsBCC == null) {
					rcptsBCC = new cape.DynamicVector();
				}
				rcptsBCC.append((object)address);
			}
			onChanged();
			return(this);
		}

		public SMTPMessage setRecipients(cape.DynamicVector to, cape.DynamicVector cc, cape.DynamicVector bcc) {
			rcptsTo = to;
			rcptsCC = cc;
			rcptsBCC = bcc;
			onChanged();
			return(this);
		}

		public int getSizeBytes() {
			var b = getMessageBody();
			if(object.Equals(b, null)) {
				return(0);
			}
			var bb = cape.String.toUTF8Buffer(b);
			if(bb == null) {
				return(0);
			}
			return((int)cape.Buffer.getSize(bb));
		}

		public virtual string getMessageBody() {
			if(!(object.Equals(messageBody, null))) {
				return(messageBody);
			}
			var sb = new cape.StringBuilder();
			sb.append("From: ");
			sb.append(myName);
			sb.append(" <");
			sb.append(myAddress);
			if(cape.String.isEmpty(replyTo) == false) {
				sb.append(">\r\nReply-To: ");
				sb.append(myName);
				sb.append(" <");
				sb.append(replyTo);
			}
			sb.append(">\r\nTo: ");
			var first = true;
			if(rcptsTo != null) {
				var array = rcptsTo.toVector();
				if(array != null) {
					var n = 0;
					var m = array.Count;
					for(n = 0 ; n < m ; n++) {
						var to = array[n] as string;
						if(to != null) {
							if(first == false) {
								sb.append(", ");
							}
							sb.append(to);
							first = false;
						}
					}
				}
			}
			sb.append("\r\nCc: ");
			first = true;
			if(rcptsCC != null) {
				var array2 = rcptsCC.toVector();
				if(array2 != null) {
					var n2 = 0;
					var m2 = array2.Count;
					for(n2 = 0 ; n2 < m2 ; n2++) {
						var to1 = array2[n2] as string;
						if(to1 != null) {
							if(first == false) {
								sb.append(", ");
							}
							sb.append(to1);
							first = false;
						}
					}
				}
			}
			sb.append("\r\nSubject: ");
			sb.append(subject);
			sb.append("\r\nContent-Type: ");
			sb.append(contentType);
			sb.append("\r\nDate: ");
			sb.append(date);
			sb.append("\r\nMessage-ID: <");
			sb.append(messageID);
			sb.append(">\r\n\r\n");
			sb.append(text);
			messageBody = sb.toString();
			return(messageBody);
		}

		public cape.DynamicVector getExcludeAddresses() {
			return(excludeAddresses);
		}

		public SMTPMessage setExcludeAddresses(cape.DynamicVector v) {
			excludeAddresses = v;
			return(this);
		}
	}
}
