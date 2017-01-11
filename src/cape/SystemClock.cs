
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
	public class SystemClock
	{
		public static long asSeconds() {
			var v = (long)0;
			var epoch = new System.DateTime(1970, 1, 1);
			var now = System.DateTime.Now.AddTicks(-epoch.Ticks);
			v = (long)(now.Ticks/10000000);
			return(v);
		}

		public static TimeValue asTimeValue() {
			var v = new TimeValue();
			update(v);
			return(v);
		}

		public static void update(TimeValue v) {
			if(v == null) {
				return;
			}
			var epoch = new System.DateTime(1970, 1, 1).Ticks;
			var now = System.DateTime.Now.AddSeconds(-(epoch/10000000));
			var ct = (long)(now.Ticks)/10;
			v.setSeconds((int)(ct / 1000000));
			long us = ct % 1000000;
			v.setMicroSeconds((int)(us));
		}
	}
}
