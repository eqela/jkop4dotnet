
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
	public class ZipWriterForDotNet : ZipWriter
	{
		private cape.File file = null;
		private System.IO.Compression.ZipArchive archive = null;

		public ZipWriterForDotNet initialize() {
			if(file == null) {
				return(null);
			}
			var fp = file.getPath();
			if(object.Equals(fp, null)) {
				return(null);
			}
			archive = System.IO.Compression.ZipFile.Open(fp, System.IO.Compression.ZipArchiveMode.Create);
			if(archive == null) {
				return(null);
			}
			return(this);
		}

		public override bool addFile(cape.File file, string filename) {
			if((archive == null) || (file == null)) {
				return(false);
			}
			if(System.IO.Compression.ZipFileExtensions.CreateEntryFromFile(archive, file.getPath(), filename) == null) {
				return(false);
			}
			return(true);
		}

		public override bool close() {
			if(archive == null) {
				return(false);
			}
			archive.Dispose();
			archive = null;
			return(true);
		}

		public cape.File getFile() {
			return(file);
		}

		public ZipWriterForDotNet setFile(cape.File v) {
			file = v;
			return(this);
		}
	}
}
