
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
	public class SMTPMultipartMessage : SMTPMessage
	{
		public SMTPMultipartMessage() {
			setContentType("multipart/mixed");
		}

		private cape.DynamicVector attachments = null;
		private string message = null;

		public override string getMessageBody() {
			if((attachments == null) || (attachments.getSize() < 1)) {
				return(null);
			}
			if(cape.String.isEmpty(message) == false) {
				return(message);
			}
			var subject = getSubject();
			var date = getDate();
			var myName = getMyName();
			var myAddress = getMyAddress();
			var text = getText();
			var recipientsTo = getRcptsTo();
			var recipientsCC = getRcptsCC();
			var messageID = getMessageID();
			var replyTo = getReplyTo();
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
			if(recipientsTo != null) {
				var array = recipientsTo.toVector();
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
			if(recipientsCC != null) {
				var array2 = recipientsCC.toVector();
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
			sb.append("\r\nMIME-Version: 1.0");
			sb.append("\r\nContent-Type: ");
			sb.append("multipart/mixed");
			sb.append("; boundary=\"XXXXboundarytext\"");
			sb.append("\r\nDate: ");
			sb.append(date);
			sb.append("\r\nMessage-ID: <");
			sb.append(messageID);
			sb.append(">\r\n\r\n");
			sb.append("This is a multipart message in MIME format.");
			sb.append("\r\n");
			sb.append("\r\n--XXXXboundarytext");
			sb.append("\r\nContent-Type: text/plain");
			sb.append("\r\n");
			sb.append("\r\n");
			sb.append(text);
			var array3 = attachments.toVector();
			if(array3 != null) {
				var n3 = 0;
				var m3 = array3.Count;
				for(n3 = 0 ; n3 < m3 ; n3++) {
					var file = array3[n3] as cape.File;
					if(file != null) {
						sb.append("\r\n--XXXXboundarytext");
						sb.append("\r\nContent-Type: ");
						var contentType = capex.MimeTypeRegistry.typeForFile(file);
						if((cape.String.isEmpty(contentType) == false) && (cape.String.getIndexOf(contentType, "text") == 0)) {
							sb.append(contentType);
							sb.append("\r\nContent-Disposition: attachment; filename=");
							sb.append(file.baseName());
							sb.append("\r\n");
							sb.append("\r\n");
							sb.append(file.getContentsString("UTF8"));
						}
						else {
							sb.append(contentType);
							sb.append("\r\nContent-Transfer-Encoding: Base64");
							sb.append("\r\nContent-Disposition: attachment filename=");
							sb.append(file.baseName());
							sb.append("\r\n");
							sb.append("\r\n");
							sb.append(capex.Base64Encoder.encode(file.getContentsBuffer()));
						}
					}
				}
			}
			sb.append("\r\n");
			sb.append("\r\n--XXXXboundarytext--");
			return(message = sb.toString());
		}

		public cape.DynamicVector getAttachments() {
			return(attachments);
		}

		public SMTPMultipartMessage setAttachments(cape.DynamicVector v) {
			attachments = v;
			return(this);
		}
	}
}
