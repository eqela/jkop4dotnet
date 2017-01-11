
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
	internal class FileForDotNet : FileAdapter
	{
		private class MyFileReader : DotNetStreamReader
		{
			public bool initialize(string file) {
				if(object.Equals(file, null)) {
					return(false);
				}
				var v = true;
				try {
					setStream(System.IO.File.OpenRead(file));
				}
				catch(System.Exception e) {
					// System.Console.WriteLine(e.ToString());
					setStream(null);
					v = false;
				}
				return(v);
			}
		}

		private class MyFileWriter : DotNetStreamWriter, FileWriter
		{
			private bool append = false;

			public bool initialize(string file) {
				if(object.Equals(file, null)) {
					return(false);
				}
				var v = true;
				try {
					if(append) {
						setStream(System.IO.File.Open(file, System.IO.FileMode.Append));
					}
					else {
						setStream(System.IO.File.Open(file, System.IO.FileMode.Create));
					}
				}
				catch(System.Exception e) {
					// System.Console.WriteLine(e.ToString());
					setStream(null);
					v = false;
				}
				return(v);
			}

			public bool getAppend() {
				return(append);
			}

			public MyFileWriter setAppend(bool v) {
				append = v;
				return(this);
			}
		}

		public static File forPath(string path) {
			return((File)new FileForDotNet(path));
		}

		private string completePath = null;

		public FileForDotNet() {
		}

		public FileForDotNet(string path) {
			setCompletePath(path);
		}

		public void setCompletePath(string v) {
			var x = v;
			if(x == null || x.Length < 1) {
				completePath = null;
			}
			else {
				string delim = new System.String(System.IO.Path.DirectorySeparatorChar, 1);
				while(x.EndsWith(delim) && x.Length > 1) {
					x = x.Substring(0, x.Length-1);
				}
				completePath = System.IO.Path.GetFullPath(x);
			}
		}

		public override File entry(string name) {
			if(object.Equals(completePath, null)) {
				return((File)this);
			}
			if((object.Equals(name, null)) || (cape.String.getLength(name) < 1)) {
				return((File)this);
			}
			return((File)new FileForDotNet(System.IO.Path.Combine(completePath, name)));
		}

		public override File getParent() {
			string v = null;
			if(completePath != null) {
				var di = System.IO.Directory.GetParent(completePath);
				if(di != null) {
					v = di.FullName;
				}
			}
			if(v == null) {
				v = completePath;
			}
			return((File)new FileForDotNet(v));
		}

		private class MyIterator : Iterator<File>
		{
			private string completePath = null;

			public System.Collections.IEnumerator it;

			public virtual File next() {
				string str = null;
				try {
					if(it == null) {
						return(null);
					}
					if(it.MoveNext() == false) {
						return(null);
					}
					str = it.Current as string;
					if(str == null) {
						return(null);
					}
					str = System.IO.Path.Combine(completePath, str);
				}
				catch(System.Exception e) {
					return(null);
				}
				var v = new FileForDotNet();
				v.setCompletePath(str);
				return((File)v);
			}

			public string getCompletePath() {
				return(completePath);
			}

			public MyIterator setCompletePath(string v) {
				completePath = v;
				return(this);
			}
		}

		public override Iterator<File> entries() {
			if(object.Equals(completePath, null)) {
				return(null);
			}
			var v = new MyIterator();
			v.setCompletePath(completePath);
			try {
				System.Collections.IEnumerable cc = System.IO.Directory.EnumerateFileSystemEntries(completePath);
				v.it = cc.GetEnumerator();
			}
			catch(System.Exception e) {
			}
			return((Iterator<File>)v);
		}

		public override bool move(File dest, bool replace) {
			if(dest == null) {
				return(false);
			}
			if(dest.exists()) {
				if(replace == false) {
					return(false);
				}
				dest.remove();
			}
			var destf = dest as FileForDotNet;
			if(destf == null) {
				return(false);
			}
			var v = true;
			try {
				System.IO.File.Move(completePath, destf.completePath);
			}
			catch(System.Exception e) {
				// System.Console.WriteLine(e.ToString());
				v = false;
			}
			return(v);
		}

		public override bool rename(string newname, bool replacee) {
			System.Console.WriteLine("--- stub --- cape.FileForDotNet :: rename");
			return(false);
		}

		public override bool touch() {
			if(object.Equals(completePath, null)) {
				return(false);
			}
			var v = true;
			try {
				var fi = new System.IO.FileInfo(completePath);
				if(fi.Exists) {
					System.IO.File.SetLastWriteTime(completePath, System.DateTime.Now);
				}
				else {
					System.IO.File.Create(completePath).Dispose();
				}
			}
			catch(System.Exception e) {
				// System.Console.WriteLine(e.ToString());
				v = false;
			}
			return(v);
		}

		private MyFileReader getMyReader() {
			var v = new MyFileReader();
			if(v.initialize(completePath) == false) {
				return(null);
			}
			return(v);
		}

		public override FileReader read() {
			return((FileReader)getMyReader());
		}

		public override FileWriter write() {
			var v = new MyFileWriter();
			if(v.initialize(completePath) == false) {
				v = null;
			}
			return((FileWriter)v);
		}

		public override FileWriter append() {
			var v = new MyFileWriter();
			v.setAppend(true);
			if(v.initialize(completePath) == false) {
				v = null;
			}
			return((FileWriter)v);
		}

		public override bool makeExecutable() {
			var v = true;
			if(System.Type.GetType("Mono.Runtime") != null) {
				try {
					System.IO.File.SetAttributes(completePath, (System.IO.FileAttributes)((int)System.IO.File.GetAttributes(completePath) | 0x80000000));
				}
				catch(System.Exception e) {
					v = false;
				}
			}
			return(v);
		}

		public override FileInfo stat() {
			var v = new FileInfo();
			v.setFile((File)this);
			v.setOwnerUser(0);
			v.setOwnerGroup(0);
			v.setMode(0);
			v.setExecutable(true);
			v.setLink(false);
			v.setType(cape.FileInfo.FILE_TYPE_UNKNOWN);
			if(!(object.Equals(completePath, null))) {
				try {
					var attrs = System.IO.File.GetAttributes(completePath);
					if(attrs.HasFlag(System.IO.FileAttributes.Directory)) {
						v.setType(FileInfo.FILE_TYPE_DIR);
					}
					else {
						v.setType(FileInfo.FILE_TYPE_FILE);
					}
				}
				catch(System.Exception e) {
				}
				if(v.getType() == cape.FileInfo.FILE_TYPE_FILE) {
					try {
						var dnfi = new System.IO.FileInfo(completePath);
						v.setSize((int)dnfi.Length);
						v.setAccessTime((int)dnfi.LastAccessTime.Subtract(new System.DateTime(1970, 1, 1)).TotalSeconds);
						v.setModifyTime((int)dnfi.LastWriteTime.Subtract(new System.DateTime(1970, 1, 1)).TotalSeconds);
					}
					catch(System.Exception e) {
					}
				}
			}
			return(v);
		}

		public override bool exists() {
			var fi = stat();
			return(fi.getType() != cape.FileInfo.FILE_TYPE_UNKNOWN);
		}

		public override bool isExecutable() {
			System.Console.WriteLine("--- stub --- cape.FileForDotNet :: isExecutable");
			return(false);
		}

		public override bool createFifo() {
			System.Console.WriteLine("--- stub --- cape.FileForDotNet :: createFifo");
			return(false);
		}

		public override bool createDirectory() {
			var v = false;
			try {
				System.IO.Directory.CreateDirectory(completePath);
				v = true;
			}
			catch(System.Exception e) {
				// System.Console.WriteLine(e.ToString());
				v = false;
			}
			return(v);
		}

		public override bool createDirectoryRecursive() {
			return(createDirectory());
		}

		public override bool remove() {
			var v = true;
			try {
				System.IO.File.Delete(completePath);
			}
			catch(System.IO.DirectoryNotFoundException e) {
				v = false;
			}
			catch(System.IO.FileNotFoundException e) {
				v = false;
			}
			catch(System.Exception e) {
				// System.Console.WriteLine(e.ToString());
				v = false;
			}
			return(v);
		}

		public override bool removeDirectory() {
			var v = true;
			try {
				System.IO.Directory.Delete(completePath);
			}
			catch(System.IO.DirectoryNotFoundException e) {
				v = false;
			}
			catch(System.IO.FileNotFoundException e) {
				v = false;
			}
			catch(System.Exception e) {
				// System.Console.WriteLine(e.ToString());
				v = false;
			}
			return(v);
		}

		public override string getPath() {
			return(completePath);
		}

		public override int compareModificationTime(File bf) {
			System.Console.WriteLine("--- stub --- cape.FileForDotNet :: compareModificationTime");
			return(0);
		}

		public override string directoryName() {
			System.Console.WriteLine("--- stub --- cape.FileForDotNet :: directoryName");
			return(null);
		}

		public override string baseName() {
			var path = completePath;
			if(object.Equals(path, null)) {
				return(null);
			}
			var rs = cape.String.lastIndexOf(path, System.IO.Path.DirectorySeparatorChar);
			if(rs < 0) {
				return(path);
			}
			return(cape.String.subString(path, rs + 1));
		}

		public override bool isIdentical(File file) {
			System.Console.WriteLine("--- stub --- cape.FileForDotNet :: isIdentical");
			return(false);
		}

		public override byte[] getContentsBuffer() {
			if(object.Equals(completePath, null)) {
				return(null);
			}
			return(System.IO.File.ReadAllBytes(completePath));
		}

		public override string getContentsString(string encoding) {
			if(object.Equals(completePath, null)) {
				return(null);
			}
			string v = null;
			if(object.Equals(encoding, null)) {
				try {
					v = System.IO.File.ReadAllText(completePath);
				}
				catch(System.Exception e) {
					v = null;
				}
			}
			else if(object.Equals(encoding, "UTF-8")) {
				try {
					v = System.IO.File.ReadAllText(completePath, System.Text.Encoding.UTF8);
				}
				catch(System.Exception e) {
					v = null;
				}
			}
			else if(object.Equals(encoding, "ASCII")) {
				try {
					v = System.IO.File.ReadAllText(completePath, System.Text.Encoding.ASCII);
				}
				catch(System.Exception e) {
					v = null;
				}
			}
			else if(object.Equals(encoding, "UCS2")) {
				try {
					v = System.IO.File.ReadAllText(completePath, System.Text.Encoding.Unicode);
				}
				catch(System.Exception e) {
					v = null;
				}
			}
			else if(object.Equals(encoding, "UTF-7")) {
				try {
					v = System.IO.File.ReadAllText(completePath, System.Text.Encoding.UTF7);
				}
				catch(System.Exception e) {
					v = null;
				}
			}
			else if(object.Equals(encoding, "UTF-32")) {
				try {
					v = System.IO.File.ReadAllText(completePath, System.Text.Encoding.UTF32);
				}
				catch(System.Exception e) {
					v = null;
				}
			}
			else {
				try {
					v = System.IO.File.ReadAllText(completePath);
				}
				catch(System.Exception e) {
					v = null;
				}
			}
			return(v);
		}

		public override bool setContentsBuffer(byte[] buf) {
			if(buf == null) {
				return(false);
			}
			try {
				System.IO.File.WriteAllBytes(completePath, buf);
			}
			catch(System.Exception e) {
				return(false);
			}
			return(true);
		}

		public override bool isNewerThan(File bf) {
			System.Console.WriteLine("--- stub --- cape.FileForDotNet :: isNewerThan");
			return(false);
		}

		public override bool isOlderThan(File bf) {
			System.Console.WriteLine("--- stub --- cape.FileForDotNet :: isOlderThan");
			return(false);
		}

		public override bool writeFromReader(Reader reader, bool append) {
			System.Console.WriteLine("--- stub --- cape.FileForDotNet :: writeFromReader");
			return(false);
		}

		public override bool setContentsString(string str, string encoding) {
			if(object.Equals(str, null)) {
				return(false);
			}
			System.Text.Encoding ee;
			if(encoding == null || encoding.Equals("UTF-8") || encoding.Equals("UTF8")) {
				ee = new System.Text.UTF8Encoding(false);
			}
			else if(encoding.Equals("ASCII")) {
				ee = System.Text.Encoding.ASCII;
			}
			else if(encoding.Equals("UCS-2") || encoding.Equals("UCS2")) {
				ee = new System.Text.UnicodeEncoding(false, false);
			}
			else {
				return(false);
			}
			try {
				System.IO.File.WriteAllText(completePath, str, ee);
			}
			catch(System.Exception e) {
				return(false);
			}
			return(true);
		}
	}
}
