
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

namespace capex.console
{
	public class CommandLineArgument
	{
		public string arg = null;
		public string parameter = null;
		public string flag = null;
		public string key = null;
		public string value = null;

		public CommandLineArgument(string arg, string parameter, string flag, string key, string value) {
			this.arg = arg;
			this.parameter = parameter;
			this.flag = flag;
			this.key = key;
			this.value = value;
		}

		public bool isParameter() {
			if(!(object.Equals(parameter, null))) {
				return(true);
			}
			return(false);
		}

		public bool isFlag() {
			if(!(object.Equals(flag, null))) {
				return(true);
			}
			return(false);
		}

		public bool isOption() {
			if(!(object.Equals(key, null))) {
				return(true);
			}
			return(false);
		}

		public bool isFlag(string text) {
			if(!(object.Equals(text, null)) && (object.Equals(text, flag))) {
				return(true);
			}
			return(false);
		}

		public bool isOption(string text) {
			if(!(object.Equals(text, null)) && (object.Equals(text, key))) {
				return(true);
			}
			return(false);
		}

		public bool hasValue() {
			if(!(object.Equals(value, null))) {
				return(true);
			}
			return(false);
		}

		public string getComplete() {
			return(arg);
		}

		public string getStringValue() {
			return(value);
		}

		public int getIntegerValue() {
			return(cape.Integer.asInteger(value));
		}

		public bool getBooleanValue() {
			return(cape.Boolean.asBoolean((object)value));
		}

		public void reportAsUnsupported(cape.LoggingContext ctx) {
			cape.Log.error(ctx, ("Unsupported command line parameter: `" + arg) + "'");
		}
	}
}
