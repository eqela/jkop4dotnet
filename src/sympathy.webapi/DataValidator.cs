
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

namespace sympathy.webapi
{
	internal class DataValidator
	{
		public static string toValidJSONString(string response) {
			if(cape.String.isEmpty(response)) {
				return(null);
			}
			var prev = ' ';
			var sb = new cape.StringBuilder();
			var iter = new cape.CharacterIteratorForString(response);
			var hasOpeningQuote = false;
			var hasClosingQuote = false;
			while(true) {
				if(iter.hasEnded()) {
					break;
				}
				var c = iter.getCurrentChar();
				if((((((((c == '\"') || (c == ' ')) || (c == '\n')) || (c == '\t')) || (c == '{')) || (c == '[')) || (c == '}')) || (c == ']')) {
					if((hasOpeningQuote == false) && (c == '\"')) {
						hasOpeningQuote = true;
					}
					else if(((hasOpeningQuote == true) && (hasClosingQuote == false)) && (c == '\"')) {
						hasClosingQuote = true;
					}
					else if((hasClosingQuote == false) && (hasOpeningQuote == false)) {
						if(((((((c == '\n') && (prev != '\"')) && (prev != '}')) && (prev != ']')) && (prev != ',')) && (prev != '[')) && (prev != '{')) {
							sb.append('\"');
						}
					}
					if(hasOpeningQuote && hasClosingQuote) {
						hasOpeningQuote = false;
						hasClosingQuote = false;
					}
					prev = c;
					sb.append(c);
					iter.moveToNextChar();
					continue;
				}
				if(c == ':') {
					sb.append(c);
					sb.append(' ');
					iter.moveToNextChar();
					iter.moveToNextChar();
					c = iter.getCurrentChar();
					if(((c != '\"') && (c != '{')) && (c != '[')) {
						sb.append('\"');
						sb.append(c);
						iter.moveToNextChar();
					}
					continue;
				}
				sb.append(c);
				iter.moveToNextChar();
			}
			return(sb.toString());
		}
	}
}
