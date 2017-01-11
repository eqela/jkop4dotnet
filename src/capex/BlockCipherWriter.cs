
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
	public class BlockCipherWriter : cape.Writer, cape.SeekableWriter
	{
		public static BlockCipherWriter create(cape.Writer writer, BlockCipher cipher) {
			if((writer == null) || (cipher == null)) {
				return(null);
			}
			var v = new BlockCipherWriter();
			v.writer = writer;
			v.cipher = cipher;
			v.bsize = cipher.getBlockSize();
			v.bcurr = 0;
			v.bdata = cape.Buffer.allocate((long)cipher.getBlockSize());
			v.outbuf = cape.Buffer.allocate((long)cipher.getBlockSize());
			return(v);
		}

		private BlockCipher cipher = null;
		private cape.Writer writer = null;
		private int bsize = 0;
		private int bcurr = 0;
		private byte[] bdata = null;
		private byte[] outbuf = null;

		~BlockCipherWriter() {
			close();
		}

		public void close() {
			if((writer != null) && (bdata != null)) {
				var bb = cape.Buffer.allocate((long)1);
				var bbptr = bb;
				if(bcurr > 0) {
					var n = 0;
					for(n = bcurr ; n < bsize ; n++) {
						cape.Buffer.setByte(bdata, (long)n, (byte)0);
					}
					writeCompleteBlock(bdata);
					cape.Buffer.setByte(bbptr, (long)0, (byte)(bsize - bcurr));
					writer.write(bb, -1);
				}
				else {
					cape.Buffer.setByte(bbptr, (long)0, (byte)0);
					writer.write(bb, -1);
				}
			}
			writer = null;
			cipher = null;
			bdata = null;
		}

		public virtual bool setCurrentPosition(long n) {
			if((writer != null) && (writer is cape.SeekableWriter)) {
				return(((cape.SeekableWriter)writer).setCurrentPosition(n));
			}
			return(false);
		}

		public virtual long getCurrentPosition() {
			if((writer != null) && (writer is cape.SeekableWriter)) {
				return(((cape.SeekableWriter)writer).getCurrentPosition());
			}
			return((long)-1);
		}

		public bool writeCompleteBlock(byte[] buf) {
			cipher.encryptBlock(buf, outbuf);
			if(writer.write(outbuf, -1) == cape.Buffer.getSize(outbuf)) {
				return(true);
			}
			return(false);
		}

		public int writeBlock(byte[] buf) {
			var size = cape.Buffer.getSize(buf);
			if((bcurr + size) < bsize) {
				var bufptr = buf;
				cape.Buffer.copyFrom(bufptr, bdata, (long)0, (long)bcurr, size);
				bcurr += (int)size;
				return((int)size);
			}
			if(bcurr > 0) {
				var bufptr1 = buf;
				var x = bsize - bcurr;
				cape.Buffer.copyFrom(bufptr1, bdata, (long)0, (long)bcurr, (long)x);
				if(writeCompleteBlock(bdata) == false) {
					return(0);
				}
				bcurr = 0;
				if(x == size) {
					return(x);
				}
				return(x + writeBlock(cape.Buffer.getSubBuffer(buf, (long)x, size - x)));
			}
			if(writeCompleteBlock(buf) == false) {
				return(0);
			}
			return(bsize);
		}

		public virtual int write(byte[] buf, int asize = -1) {
			if(buf == null) {
				return(0);
			}
			var bufptr = buf;
			if(bufptr == null) {
				return(0);
			}
			var size = asize;
			if(size < 0) {
				size = (int)cape.Buffer.getSize(buf);
			}
			if(size < 1) {
				return(0);
			}
			var v = 0;
			var n = 0;
			for(n = 0 ; n < size ; n += bsize) {
				var x = bsize;
				if((n + x) > size) {
					x = size - n;
				}
				v += writeBlock(cape.Buffer.getSubBuffer(buf, (long)n, (long)x));
			}
			return(v);
		}
	}
}
