
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
	public class Base64Decoder
	{
		public static byte[] appendByte(byte[] buffer, byte @byte) {
			var nbyte = cape.Buffer.allocate((long)1);
			cape.Buffer.setByte(nbyte, (long)0, @byte);
			return(cape.Buffer.append(buffer, nbyte, cape.Buffer.getSize(nbyte)));
		}

		public static int fromLookupChar(char ascii) {
			var c = 0;
			if(ascii == 43) {
				c = 62;
			}
			else if(ascii == 47) {
				c = 63;
			}
			else if((ascii >= 48) && (ascii <= 57)) {
				c = (int)(ascii + 4);
			}
			else if((ascii >= 65) && (ascii <= 90)) {
				c = (int)(ascii - 65);
			}
			else if((ascii >= 97) && (ascii <= 122)) {
				c = (int)(ascii - 71);
			}
			return(c);
		}

		public static byte[] decode(string text) {
			if(object.Equals(text, null)) {
				return(null);
			}
			var data = new byte[0];
			var iter = cape.String.iterate(text);
			var done = false;
			if(iter != null) {
				var ch = (char)0;
				while((ch = iter.getNextChar()) > 0) {
					var c1 = 0;
					var c2 = 0;
					var c3 = 0;
					var c4 = 0;
					var byte1 = (byte)0;
					var byte2 = (byte)0;
					var byte3 = (byte)0;
					c1 = fromLookupChar(ch);
					c2 = fromLookupChar(ch = iter.getNextChar());
					byte1 = (byte)((c1 << 2) + (c2 >> 4));
					data = appendByte(data, byte1);
					ch = iter.getNextChar();
					if(ch == '=') {
						done = true;
					}
					else {
						c3 = fromLookupChar(ch);
					}
					byte2 = (byte)(((c2 & 15) << 4) + (c3 >> 2));
					data = appendByte(data, byte2);
					if(!done) {
						ch = iter.getNextChar();
						if(ch == '=') {
							done = true;
						}
						else {
							c4 = fromLookupChar(ch);
						}
						byte3 = (byte)(((c3 & 3) << 6) + c4);
						data = appendByte(data, byte3);
					}
					if(done) {
						break;
					}
				}
			}
			return(data);
		}
	}
}
