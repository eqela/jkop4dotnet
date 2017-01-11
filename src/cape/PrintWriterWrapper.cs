
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
	public class PrintWriterWrapper : Writer, PrintWriter, Closable, FlushableWriter
	{
		public static PrintWriter forWriter(Writer writer) {
			if(writer == null) {
				return(null);
			}
			if(writer is PrintWriter) {
				return((PrintWriter)writer);
			}
			var v = new PrintWriterWrapper();
			v.setWriter(writer);
			return((PrintWriter)v);
		}

		private Writer writer = null;

		public virtual bool print(string str) {
			if(object.Equals(str, null)) {
				return(false);
			}
			var buffer = cape.String.toUTF8Buffer(str);
			if(buffer == null) {
				return(false);
			}
			var sz = (int)cape.Buffer.getSize(buffer);
			if(writer.write(buffer, -1) != sz) {
				return(false);
			}
			return(true);
		}

		public virtual bool println(string str) {
			return(print(str + "\n"));
		}

		public virtual int write(byte[] buf, int size = -1) {
			if(writer == null) {
				return(-1);
			}
			return(writer.write(buf, size));
		}

		public virtual void close() {
			var cw = writer as Closable;
			if(cw != null) {
				cw.close();
			}
		}

		public virtual void flush() {
			var cw = writer as FlushableWriter;
			if(cw != null) {
				cw.flush();
			}
		}

		public Writer getWriter() {
			return(writer);
		}

		public PrintWriterWrapper setWriter(Writer v) {
			writer = v;
			return(this);
		}
	}
}
