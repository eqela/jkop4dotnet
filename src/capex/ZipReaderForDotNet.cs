
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
	public class ZipReaderForDotNet : ZipReader
	{
		private class MyZipReaderEntry : ZipReaderEntry
		{
			private System.IO.Compression.ZipArchiveEntry entry = null;

			public override cape.Reader getContentReader() {
				System.IO.Stream stream = null;
				try {
					stream = entry.Open();
				}
				catch(System.Exception e) {
					stream = null;
				}
				if(stream == null) {
					return(null);
				}
				return((cape.Reader)cape.DotNetStreamReader.forStream(stream));
			}

			public System.IO.Compression.ZipArchiveEntry getEntry() {
				return(entry);
			}

			public MyZipReaderEntry setEntry(System.IO.Compression.ZipArchiveEntry v) {
				entry = v;
				return(this);
			}
		}

		private cape.File file = null;
		private System.IO.Compression.ZipArchive archive = null;

		public ZipReaderForDotNet initialize() {
			if(file == null) {
				return(null);
			}
			var fp = file.getPath();
			if(object.Equals(fp, null)) {
				return(null);
			}
			archive = System.IO.Compression.ZipFile.Open(fp, System.IO.Compression.ZipArchiveMode.Read);
			if(archive == null) {
				return(null);
			}
			return(this);
		}

		public override System.Collections.Generic.List<ZipReaderEntry> getEntries() {
			var v = new System.Collections.Generic.List<ZipReaderEntry>();
			if(archive != null) {
				foreach(System.IO.Compression.ZipArchiveEntry entry in archive.Entries) {
					var ee = new MyZipReaderEntry();
					ee.setName(entry.FullName);
					ee.setCompressedSize(entry.CompressedLength);
					ee.setUncompressedSize(entry.Length);
					ee.setEntry(entry);
					v.Add(ee);
				}
			}
			return(v);
		}

		public cape.File getFile() {
			return(file);
		}

		public ZipReaderForDotNet setFile(cape.File v) {
			file = v;
			return(this);
		}
	}
}
