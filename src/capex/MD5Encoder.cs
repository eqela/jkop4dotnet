
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

namespace capex
{
	public class MD5Encoder
	{
		public static string encode(byte[] buffer) {
			if((buffer == null) || (cape.Buffer.getSize(buffer) < 1)) {
				return(null);
			}
			string v = null;
			var md5 = System.Security.Cryptography.MD5.Create();
			byte[] hash = md5.ComputeHash(buffer);
			var sb = new System.Text.StringBuilder();
			for (int i = 0; i < hash.Length; i++) {
				sb.Append(hash[i].ToString("X2"));
			}
			v = sb.ToString();
			return(v);
		}

		public static string encode(string @string) {
			return(capex.MD5Encoder.encode(cape.String.toUTF8Buffer(@string)));
		}

		public static string encode(object obj) {
			var bb = cape.Buffer.asBuffer(obj);
			if(bb != null) {
				return(capex.MD5Encoder.encode(bb));
			}
			var ss = cape.String.asString(obj);
			if(!(object.Equals(ss, null))) {
				return(capex.MD5Encoder.encode(ss));
			}
			return(null);
		}
	}
}
