
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
	public class ZipReaderEntry
	{
		private string name = null;
		private long compressedSize = (long)0;
		private long uncompressedSize = (long)0;
		private bool isDirectory = false;

		public string getName() {
			return(name);
		}

		public ZipReaderEntry setName(string newName) {
			name = cape.String.replace(newName, '\\', '/');
			if(cape.String.endsWith(name, "/")) {
				isDirectory = true;
				name = cape.String.getSubString(name, 0, cape.String.getLength(name) - 1);
			}
			return(this);
		}

		public virtual cape.Reader getContentReader() {
			return(null);
		}

		public virtual bool writeToFile(cape.File file) {
			if(file == null) {
				return(false);
			}
			if(getIsDirectory()) {
				return(file.createDirectoryRecursive());
			}
			var reader = getContentReader();
			if(reader == null) {
				return(false);
			}
			var fp = file.getParent();
			if(fp != null) {
				fp.createDirectoryRecursive();
			}
			var writer = file.write();
			if(writer == null) {
				if(reader is cape.Closable) {
					((cape.Closable)reader).close();
				}
				return(false);
			}
			var buf = new byte[4096 * 4];
			var v = true;
			var n = 0;
			while((n = reader.read(buf)) > 0) {
				var nr = writer.write(buf, n);
				if(nr != n) {
					v = false;
					break;
				}
			}
			if(v == false) {
				file.remove();
			}
			if((reader != null) && (reader is cape.Closable)) {
				((cape.Closable)reader).close();
			}
			if((writer != null) && (writer is cape.Closable)) {
				((cape.Closable)writer).close();
			}
			return(v);
		}

		public cape.File writeToDir(cape.File dir, bool fullPath = true, bool overwrite = true) {
			if((dir == null) || (object.Equals(name, null))) {
				return(null);
			}
			cape.File path = null;
			if(fullPath == false) {
				string nn = null;
				var r = cape.String.lastIndexOf(name, '/');
				if(r < 1) {
					nn = name;
				}
				else {
					nn = cape.String.subString(name, r + 1);
				}
				if((object.Equals(nn, null)) || (cape.String.getLength(nn) < 1)) {
					return(null);
				}
				path = dir.entry(nn);
			}
			else {
				path = dir;
				var array = cape.String.split(name, '/');
				if(array != null) {
					var n = 0;
					var m = array.Count;
					for(n = 0 ; n < m ; n++) {
						var x = array[n];
						if(x != null) {
							if(!(object.Equals(x, null)) && (cape.String.getLength(x) > 0)) {
								path = path.entry(x);
							}
						}
					}
				}
				var dd = path.getParent();
				if(dd.isDirectory() == false) {
					dd.createDirectoryRecursive();
				}
				if(dd.isDirectory() == false) {
					return(null);
				}
			}
			if(overwrite == false) {
				if(path.exists()) {
					return(null);
				}
			}
			if(writeToFile(path) == false) {
				return(null);
			}
			return(path);
		}

		public long getCompressedSize() {
			return(compressedSize);
		}

		public ZipReaderEntry setCompressedSize(long v) {
			compressedSize = v;
			return(this);
		}

		public long getUncompressedSize() {
			return(uncompressedSize);
		}

		public ZipReaderEntry setUncompressedSize(long v) {
			uncompressedSize = v;
			return(this);
		}

		public bool getIsDirectory() {
			return(isDirectory);
		}

		public ZipReaderEntry setIsDirectory(bool v) {
			isDirectory = v;
			return(this);
		}
	}
}
