
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
	public abstract class ZipReader
	{
		public static ZipReader forFile(cape.File file) {
			return((ZipReader)new ZipReaderForDotNet().setFile(file).initialize());
			return(null);
		}

		public static bool extractZipFileToDirectory(cape.File zipFile, cape.File destDir, System.Action<cape.File> listener = null) {
			if((zipFile == null) || (destDir == null)) {
				return(false);
			}
			var zf = forFile(zipFile);
			if(zf == null) {
				return(false);
			}
			if(destDir.isDirectory() == false) {
				destDir.createDirectoryRecursive();
			}
			if(destDir.isDirectory() == false) {
				return(false);
			}
			var array = zf.getEntries();
			if(array != null) {
				var n = 0;
				var m = array.Count;
				for(n = 0 ; n < m ; n++) {
					var entry = array[n];
					if(entry != null) {
						var ename = entry.getName();
						if(cape.String.isEmpty(ename)) {
							continue;
						}
						var dd = destDir;
						ename = cape.String.replace(ename, '\\', '/');
						var array2 = cape.String.split(ename, '/');
						if(array2 != null) {
							var n2 = 0;
							var m2 = array2.Count;
							for(n2 = 0 ; n2 < m2 ; n2++) {
								var comp = array2[n2];
								if(comp != null) {
									if((object.Equals(comp, ".")) || (object.Equals(comp, ".."))) {
										continue;
									}
									dd = dd.entry(comp);
								}
							}
						}
						if(listener != null) {
							listener(dd);
						}
						if(entry.writeToFile(dd) == false) {
							return(false);
						}
					}
				}
			}
			return(true);
		}

		public abstract System.Collections.Generic.List<ZipReaderEntry> getEntries();
	}
}
