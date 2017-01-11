
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
	public abstract class Terminal
	{
		class TerminalImpl : Terminal
		{
			public override void clear() {
				System.Console.Clear();
			}

			public override void moveTo(int x, int y) {
				System.Console.CursorLeft = x;
				System.Console.CursorTop = y;
			}

			public override void print(string text) {
				System.Console.Write(text);
			}

			public override void printTo(int x, int y, string text) {
				System.Console.CursorLeft = x;
				System.Console.CursorTop = y;
				System.Console.Write(text);
			}

			public override int readKey() {
				var v = 0;
				var ck = System.Console.ReadKey(false);
				if(ck == null) {
					return(-1);
				}
				v = (int)ck.KeyChar;
				return(v);
			}
		}

		public static Terminal forCurrent() {
			return((Terminal)new TerminalImpl());
		}

		public abstract void clear();
		public abstract void moveTo(int x, int y);
		public abstract void print(string text);
		public abstract void printTo(int x, int y, string text);
		public abstract int readKey();
	}
}
