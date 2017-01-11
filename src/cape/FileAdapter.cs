
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
	public abstract class FileAdapter : File
	{
		public abstract File entry(string name);
		public abstract Iterator<File> entries();
		public abstract bool move(File dest, bool replace);
		public abstract bool rename(string newname, bool replace);
		public abstract bool touch();
		public abstract FileReader read();
		public abstract FileWriter write();
		public abstract FileWriter append();
		public abstract FileInfo stat();
		public abstract bool exists();
		public abstract bool isExecutable();
		public abstract bool createFifo();
		public abstract bool createDirectory();
		public abstract bool createDirectoryRecursive();
		public abstract bool removeDirectory();
		public abstract string getPath();
		public abstract bool remove();
		public abstract int compareModificationTime(File bf);
		public abstract string directoryName();
		public abstract bool isIdentical(File file);
		public abstract bool makeExecutable();
		public abstract bool isNewerThan(File bf);
		public abstract bool isOlderThan(File bf);
		public abstract bool writeFromReader(Reader reader, bool append);

		private class ReadLineIterator : Iterator<string>
		{
			private PrintReader reader = null;

			public virtual string next() {
				if(reader == null) {
					return(null);
				}
				var v = reader.readLine();
				if(object.Equals(v, null)) {
					reader.close();
					reader = null;
				}
				return(v);
			}

			public PrintReader getReader() {
				return(reader);
			}

			public ReadLineIterator setReader(PrintReader v) {
				reader = v;
				return(this);
			}
		}

		public virtual Iterator<string> readLines() {
			var rd = read();
			if(rd == null) {
				return(null);
			}
			return((Iterator<string>)new ReadLineIterator().setReader(new PrintReader((Reader)rd)));
		}

		public virtual bool hasChangedSince(long originalTimeStamp) {
			var nts = getLastModifiedTimeStamp();
			if(nts > originalTimeStamp) {
				return(true);
			}
			return(false);
		}

		public virtual long getLastModifiedTimeStamp() {
			if(isFile() == false) {
				return((long)0);
			}
			var st = stat();
			if(st == null) {
				return((long)0);
			}
			return((long)st.getModifyTime());
		}

		public virtual bool isSame(File file) {
			if(file == null) {
				return(false);
			}
			var path = getPath();
			if(!(object.Equals(path, null)) && (object.Equals(path, file.getPath()))) {
				return(true);
			}
			return(false);
		}

		public virtual bool removeRecursive() {
			var finfo = stat();
			if(finfo == null) {
				return(true);
			}
			if((finfo.isDirectory() == false) || (finfo.isLink() == true)) {
				return(remove());
			}
			var it = entries();
			while(it != null) {
				var f = it.next();
				if(f == null) {
					break;
				}
				if(f.removeRecursive() == false) {
					return(false);
				}
			}
			return(removeDirectory());
		}

		public virtual bool isFile() {
			var st = stat();
			if(st == null) {
				return(false);
			}
			return(st.isFile());
		}

		public virtual bool isDirectory() {
			var st = stat();
			if(st == null) {
				return(false);
			}
			return(st.isDirectory());
		}

		public virtual bool isLink() {
			var st = stat();
			if(st == null) {
				return(false);
			}
			return(st.isLink());
		}

		public virtual int getSize() {
			var st = stat();
			if(st == null) {
				return(0);
			}
			return(st.getSize());
		}

		public virtual bool setMode(int mode) {
			return(false);
		}

		public virtual bool setOwnerUser(int uid) {
			return(false);
		}

		public virtual bool setOwnerGroup(int gid) {
			return(false);
		}

		public virtual File asExecutable() {
			if(cape.OS.isWindows()) {
				if(((hasExtension("exe") == false) && (hasExtension("bat") == false)) && (hasExtension("com") == false)) {
					var bn = baseName();
					var exe = getSibling(bn + ".exe");
					if(exe.isFile()) {
						return(exe);
					}
					var bat = getSibling(bn + ".bat");
					if(bat.isFile()) {
						return(bat);
					}
					var com = getSibling(bn + ".com");
					if(com.isFile()) {
						return(com);
					}
					return(exe);
				}
			}
			return((File)this);
		}

		public virtual File getParent() {
			var path = dirName();
			if(object.Equals(path, null)) {
				return((File)new FileInvalid());
			}
			return(cape.FileInstance.forPath(path));
		}

		public virtual File getSibling(string name) {
			var pp = getParent();
			if(pp == null) {
				return(null);
			}
			return(pp.entry(name));
		}

		public virtual bool hasExtension(string ext) {
			var xx = extension();
			if(object.Equals(xx, null)) {
				return(false);
			}
			return(cape.String.equalsIgnoreCase(xx, ext));
		}

		public virtual string extension() {
			var bn = baseName();
			if(object.Equals(bn, null)) {
				return(null);
			}
			var dot = cape.String.lastIndexOf(bn, '.');
			if(dot < 0) {
				return(null);
			}
			return(cape.String.subString(bn, dot + 1));
		}

		public virtual string baseNameWithoutExtension() {
			var bn = baseName();
			if(object.Equals(bn, null)) {
				return(null);
			}
			var dot = cape.String.lastIndexOf(bn, '.');
			if(dot < 0) {
				return(bn);
			}
			return(cape.String.subString(bn, 0, dot));
		}

		public virtual string dirName() {
			var path = getPath();
			if(object.Equals(path, null)) {
				return(null);
			}
			var delim = cape.Environment.getPathSeparator();
			var dp = cape.String.lastIndexOf(path, delim);
			if(dp < 0) {
				return(".");
			}
			return(cape.String.subString(path, 0, dp));
		}

		public virtual string baseName() {
			var path = getPath();
			if(object.Equals(path, null)) {
				return(null);
			}
			var delim = cape.Environment.getPathSeparator();
			var dp = cape.String.lastIndexOf(path, delim);
			if(dp < 0) {
				return(path);
			}
			return(cape.String.subString(path, dp + 1));
		}

		public virtual bool copyFileTo(File dest) {
			if(dest == null) {
				return(false);
			}
			if(this.isSame(dest)) {
				return(true);
			}
			var buf = new byte[4096 * 4];
			if(buf == null) {
				return(false);
			}
			var reader = this.read();
			if(reader == null) {
				return(false);
			}
			var writer = dest.write();
			if(writer == null) {
				if(reader is Closable) {
					((Closable)reader).close();
				}
				return(false);
			}
			var v = true;
			var n = 0;
			while((n = reader.read(buf)) > 0) {
				var nr = writer.write(buf, n);
				if(nr != n) {
					v = false;
					break;
				}
			}
			if(v) {
				var fi = this.stat();
				if(fi != null) {
					var mode = fi.getMode();
					if(mode != 0) {
						dest.setMode(mode);
					}
				}
			}
			else {
				dest.remove();
			}
			if((reader != null) && (reader is Closable)) {
				((Closable)reader).close();
			}
			if((writer != null) && (writer is Closable)) {
				((Closable)writer).close();
			}
			return(v);
		}

		public virtual bool setContentsString(string str, string encoding) {
			if(cape.String.isEmpty(encoding)) {
				return(false);
			}
			return(setContentsBuffer(cape.String.toBuffer(str, encoding)));
		}

		public virtual bool setContentsBuffer(byte[] buffer) {
			if(buffer == null) {
				return(false);
			}
			var writer = write();
			if(writer == null) {
				return(false);
			}
			if(writer.write(buffer, buffer.Length) < 0) {
				return(false);
			}
			writer.close();
			return(true);
		}

		public virtual string getContentsString(string encoding) {
			if(cape.String.isEmpty(encoding)) {
				return(null);
			}
			return(cape.String.forBuffer(getContentsBuffer(), encoding));
		}

		public virtual byte[] getContentsBuffer() {
			var reader = read();
			if(reader == null) {
				return(null);
			}
			var sz = reader.getSize();
			var b = new byte[sz];
			if(reader.read(b) < sz) {
				return(null);
			}
			reader.close();
			return(b);
		}
	}
}
