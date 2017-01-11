
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
	public class StringSet
	{
		private System.Collections.Generic.Dictionary<string,BooleanObject> data = null;

		public StringSet() {
			data = new System.Collections.Generic.Dictionary<string,BooleanObject>();
		}

		public void add(string @string) {
			cape.Map.setValue(data, @string, cape.Boolean.asObject(true));
		}

		public void remove(string @string) {
			cape.Map.remove(data, @string);
		}

		public int count() {
			return(cape.Map.count(data));
		}

		public bool contains(string @string) {
			if(cape.Map.getValue(data, @string) != null) {
				return(true);
			}
			return(false);
		}

		public System.Collections.Generic.List<string> getAll() {
			System.Collections.Generic.List<string> v = cape.Map.getKeys(data);
			return(v);
		}

		public void clear() {
			cape.Map.clear(data);
		}
	}
}
