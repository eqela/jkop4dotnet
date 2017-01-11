
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
	public class DotNetStreamWriter : Writer, PrintWriter, Closable, SeekableWriter, FlushableWriter
	{
		public static DotNetStreamWriter forStream(System.IO.Stream stream) {
			var v = new DotNetStreamWriter();
			v.setStream(stream);
			return(v);
		}

		private System.IO.Stream stream = null;

		public virtual int write(byte[] buf, int size) {
			if(buf == null) {
				return(0);
			}
			var sz = size;
			if(sz < 1) {
				sz = (int)cape.Buffer.getSize(buf);
			}
			try {
				stream.Write(buf, 0, sz);
			}
			catch(System.Exception e) {
				// System.Console.WriteLine(e.ToString());
				sz = -1;
			}
			return(sz);
		}

		public virtual bool print(string str) {
			if(object.Equals(str, null)) {
				return(false);
			}
			if(stream == null) {
				return(false);
			}
			var buffer = cape.String.toUTF8Buffer(str);
			if(buffer == null) {
				return(false);
			}
			var sz = buffer.Length;
			if(write(buffer, -1) != sz) {
				return(false);
			}
			return(true);
		}

		public virtual bool println(string str) {
			return(print(str + "\n"));
		}

		public virtual bool setCurrentPosition(long n) {
			var v = false;
			var np = stream.Seek(n, System.IO.SeekOrigin.Begin);
			if(np == n) {
				v = true;
			}
			return(v);
		}

		public virtual long getCurrentPosition() {
			return(stream.Position);
		}

		public virtual void flush() {
			stream.Flush();
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

		public DotNetStreamWriter setStream(System.IO.Stream v) {
			stream = v;
			return(this);
		}
	}
}
