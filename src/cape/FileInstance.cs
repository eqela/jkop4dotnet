
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
	public class FileInstance
	{
		public static File forPath(string path) {
			if((object.Equals(path, null)) || (cape.String.getLength(path) < 1)) {
				return((File)new FileInvalid());
			}
			return(cape.FileForDotNet.forPath(path));
		}

		public static File forRelativePath(string path, File relativeTo, bool alwaysSupportWindowsPathnames = false) {
			if(relativeTo == null) {
				return(forPath(path));
			}
			if(object.Equals(path, null)) {
				return((File)new FileInvalid());
			}
			if(cape.Environment.isAbsolutePath(path)) {
				return(forPath(path));
			}
			var sep = cape.Environment.getPathSeparator();
			if(sep != '/') {
				if((cape.String.indexOf(path, sep) < 0) && (cape.String.indexOf(path, '/') >= 0)) {
					sep = '/';
				}
			}
			else if(alwaysSupportWindowsPathnames) {
				if((cape.String.indexOf(path, sep) < 0) && (cape.String.indexOf(path, '\\') >= 0)) {
					sep = '\\';
				}
			}
			var f = relativeTo;
			var comps = cape.String.split(path, sep);
			if(comps != null) {
				var n = 0;
				var m = comps.Count;
				for(n = 0 ; n < m ; n++) {
					var comp = comps[n];
					if(comp != null) {
						if(cape.String.isEmpty(comp)) {
							continue;
						}
						f = f.entry(comp);
					}
				}
			}
			return(f);
		}
	}
}
