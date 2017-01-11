
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
	public class SMTPClientTransactionResult
	{
		public static SMTPClientTransactionResult forError(string error) {
			return(new SMTPClientTransactionResult().setStatus(false).setErrorMessage(error));
		}

		public static SMTPClientTransactionResult forNetworkError() {
			return(forError("Network communications error"));
		}

		public static SMTPClientTransactionResult forSuccess() {
			return(new SMTPClientTransactionResult().setStatus(true));
		}

		private bool status = false;
		private string errorMessage = null;
		private string domain = null;
		private string server = null;
		private cape.DynamicVector recipients = null;

		public bool getStatus() {
			return(status);
		}

		public SMTPClientTransactionResult setStatus(bool v) {
			status = v;
			return(this);
		}

		public string getErrorMessage() {
			return(errorMessage);
		}

		public SMTPClientTransactionResult setErrorMessage(string v) {
			errorMessage = v;
			return(this);
		}

		public string getDomain() {
			return(domain);
		}

		public SMTPClientTransactionResult setDomain(string v) {
			domain = v;
			return(this);
		}

		public string getServer() {
			return(server);
		}

		public SMTPClientTransactionResult setServer(string v) {
			server = v;
			return(this);
		}

		public cape.DynamicVector getRecipients() {
			return(recipients);
		}

		public SMTPClientTransactionResult setRecipients(cape.DynamicVector v) {
			recipients = v;
			return(this);
		}
	}
}
