
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
	public class PaperOrientation : cape.StringObject
	{
		public static System.Collections.Generic.List<PaperOrientation> getAll() {
			var v = new System.Collections.Generic.List<PaperOrientation>();
			var n = 0;
			for(n = 0 ; n < COUNT ; n++) {
				cape.Vector.append(v, forValue(n));
			}
			return(v);
		}

		public static bool matches(PaperOrientation oo, int value) {
			if((oo != null) && (oo.getValue() == value)) {
				return(true);
			}
			return(false);
		}

		public static PaperOrientation forValue(int value) {
			return(new PaperOrientation().setValue(value));
		}

		public const int LANDSCAPE = 0;
		public const int PORTRAIT = 1;
		public const int COUNT = 2;
		private int value = 0;

		public virtual string toString() {
			if(value == LANDSCAPE) {
				return("Landscape");
			}
			if(value == PORTRAIT) {
				return("Portrait");
			}
			return("Unknown orientation");
		}

		public int getValue() {
			return(value);
		}

		public PaperOrientation setValue(int v) {
			value = v;
			return(this);
		}
	}
}
