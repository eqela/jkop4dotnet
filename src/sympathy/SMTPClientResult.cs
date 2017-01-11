
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
	public class SMTPClientResult
	{
		public static SMTPClientResult forSuccess() {
			return(new SMTPClientResult());
		}

		public static SMTPClientResult forMessage(SMTPMessage msg) {
			return(new SMTPClientResult().setMessage(msg));
		}

		public static SMTPClientResult forError(string error, SMTPMessage msg = null) {
			return(new SMTPClientResult().setMessage(msg).addTransaction(sympathy.SMTPClientTransactionResult.forError(error)));
		}

		private SMTPMessage message = null;
		private cape.DynamicVector transactions = null;

		public bool getStatus() {
			if(transactions == null) {
				return(false);
			}
			var array = transactions.toVector();
			if(array != null) {
				var n = 0;
				var m = array.Count;
				for(n = 0 ; n < m ; n++) {
					var rr = array[n] as SMTPClientTransactionResult;
					if(rr != null) {
						if(rr.getStatus() == false) {
							return(false);
						}
					}
				}
			}
			return(true);
		}

		public SMTPClientResult addTransaction(SMTPClientTransactionResult r) {
			if(r == null) {
				return(this);
			}
			if(transactions == null) {
				transactions = new cape.DynamicVector();
			}
			transactions.append((object)r);
			return(this);
		}

		public SMTPMessage getMessage() {
			return(message);
		}

		public SMTPClientResult setMessage(SMTPMessage v) {
			message = v;
			return(this);
		}

		public cape.DynamicVector getTransactions() {
			return(transactions);
		}

		public SMTPClientResult setTransactions(cape.DynamicVector v) {
			transactions = v;
			return(this);
		}
	}
}
