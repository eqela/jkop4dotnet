
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
	public class QueryString
	{
		public static System.Collections.Generic.Dictionary<string,string> parse(string queryString) {
			var v = new System.Collections.Generic.Dictionary<string,string>();
			var array = cape.String.split(queryString, '&');
			if(array != null) {
				var n = 0;
				var m = array.Count;
				for(n = 0 ; n < m ; n++) {
					var qs = array[n];
					if(qs != null) {
						if(cape.String.isEmpty(qs)) {
							continue;
						}
						if(cape.String.indexOf(qs, '=') < 0) {
							cape.Map.set(v, qs, null);
							continue;
						}
						var qsps = cape.String.split(qs, '=', 2);
						var key = qsps[0];
						var val = qsps[1];
						if(object.Equals(val, null)) {
							val = "";
						}
						if(cape.String.isEmpty(key) == false) {
							cape.Map.set(v, key, capex.URLDecoder.decode(val));
						}
					}
				}
			}
			return(v);
		}
	}
}
