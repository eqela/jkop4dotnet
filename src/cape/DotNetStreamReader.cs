
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
	public class DotNetStreamReader : FileReader, SizedReader, Reader, Closable, SeekableReader
	{
		public static DotNetStreamReader forStream(System.IO.Stream stream) {
			var v = new DotNetStreamReader();
			v.setStream(stream);
			return(v);
		}

		private System.IO.Stream stream = null;

		public virtual int read(byte[] buf) {
			if((buf == null) || (stream == null)) {
				return(0);
			}
			var mb = buf.Length;
			var v = 0;
			try {
				v = stream.Read(buf, 0, mb);
			}
			catch(System.Exception e) {
				// System.Console.WriteLine(e.ToString());
				v = -1;
			}
			if(v < 1) {
				stream.Close();
			}
			return(v);
		}

		public virtual bool setCurrentPosition(long n) {
			if(stream == null) {
				return(false);
			}
			var v = false;
			var np = stream.Seek(n, System.IO.SeekOrigin.Begin);
			if(np == n) {
				v = true;
			}
			return(v);
		}

		public virtual long getCurrentPosition() {
			if(stream == null) {
				return((long)0);
			}
			return(stream.Position);
		}

		public virtual int getSize() {
			if(stream == null) {
				return(0);
			}
			var v = 0;
			v = (int)stream.Length;
			return(v);
		}

		public virtual void close() {
			if(stream != null) {
				stream.Close();
				stream = null;
			}
		}

		public System.IO.Stream getStream() {
			return(stream);
		}

		public DotNetStreamReader setStream(System.IO.Stream v) {
			stream = v;
			return(this);
		}
	}
}
