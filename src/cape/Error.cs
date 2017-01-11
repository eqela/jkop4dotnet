
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

namespace cape
{
	public class Error : StringObject
	{
		public static Error forCode(string code, string detail = null) {
			return(new Error().setCode(code).setDetail(detail));
		}

		public static Error forMessage(string message) {
			return(new Error().setMessage(message));
		}

		public static Error instance(string code, string message = null, string detail = null) {
			return(new Error().setCode(code).setMessage(message).setDetail(detail));
		}

		public static Error set(Error error, string code, string message = null) {
			if(error == null) {
				return(null);
			}
			error.setCode(code);
			error.setMessage(message);
			return(error);
		}

		public static Error setErrorCode(Error error, string code) {
			return(set(error, code, null));
		}

		public static Error setErrorMessage(Error error, string message) {
			return(set(error, null, message));
		}

		public static bool isError(object o) {
			if(o == null) {
				return(false);
			}
			if((o is Error) == false) {
				return(false);
			}
			var e = o as Error;
			if(cape.String.isEmpty(e.getCode()) && cape.String.isEmpty(e.getMessage())) {
				return(false);
			}
			return(true);
		}

		public static string asString(Error error) {
			if(error == null) {
				return(null);
			}
			return(error.toString());
		}

		private string code = null;
		private string message = null;
		private string detail = null;

		public Error clear() {
			code = null;
			message = null;
			detail = null;
			return(this);
		}

		public string toStringWithDefault(string defaultError) {
			if(cape.String.isEmpty(message) == false) {
				return(message);
			}
			if(cape.String.isEmpty(code) == false) {
				if(cape.String.isEmpty(detail) == false) {
					return((code + ":") + detail);
				}
				return(code);
			}
			if(cape.String.isEmpty(detail) == false) {
				return("Error with detail: " + detail);
			}
			return(defaultError);
		}

		public virtual string toString() {
			return(toStringWithDefault("Unknown Error"));
		}

		public string getCode() {
			return(code);
		}

		public Error setCode(string v) {
			code = v;
			return(this);
		}

		public string getMessage() {
			return(message);
		}

		public Error setMessage(string v) {
			message = v;
			return(this);
		}

		public string getDetail() {
			return(detail);
		}

		public Error setDetail(string v) {
			detail = v;
			return(this);
		}
	}
}
