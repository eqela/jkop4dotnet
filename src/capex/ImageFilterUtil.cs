
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
	public class ImageFilterUtil
	{
		public static int clamp(double v) {
			if(v > 255) {
				return(255);
			}
			if(v < 0) {
				return(0);
			}
			return((int)v);
		}

		public static int getSafeByte(byte[] p, int sz, int idx) {
			var i = idx;
			if(i >= sz) {
				i = sz - 1;
			}
			else if(i < 0) {
				i = 0;
			}
			return((int)cape.Buffer.getByte(p, (long)i));
		}

		public static BitmapBuffer createForArrayFilter(BitmapBuffer bmpbuf, double[] filterArray, int fw, int fh, double factor = 1.00, double bias = 1.00) {
			var srcbuf = bmpbuf.getBuffer();
			var w = bmpbuf.getWidth();
			var h = bmpbuf.getHeight();
			if((w < 1) || (h < 1)) {
				return(null);
			}
			var desbuf = new byte[(w * h) * 4];
			var x = 0;
			var y = 0;
			var srcptr = srcbuf;
			var desptr = desbuf;
			var sz = (int)cape.Buffer.getSize(srcbuf);
			for(x = 0 ; x < w ; x++) {
				for(y = 0 ; y < h ; y++) {
					var sr = 0.00;
					var sg = 0.00;
					var sb = 0.00;
					var sa = 0.00;
					var fx = 0;
					var fy = 0;
					for(fy = 0 ; fy < fh ; fy++) {
						for(fx = 0 ; fx < fw ; fx++) {
							var ix = (x - (fw / 2)) + fx;
							var iy = (y - (fh / 2)) + fy;
							sr += (double)(getSafeByte(srcptr, sz, (((iy * w) + ix) * 4) + 0) * filterArray[(fy * fw) + fx]);
							sg += (double)(getSafeByte(srcptr, sz, (((iy * w) + ix) * 4) + 1) * filterArray[(fy * fw) + fx]);
							sb += (double)(getSafeByte(srcptr, sz, (((iy * w) + ix) * 4) + 2) * filterArray[(fy * fw) + fx]);
							sa += (double)(getSafeByte(srcptr, sz, (((iy * w) + ix) * 4) + 3) * filterArray[(fy * fw) + fx]);
						}
					}
					cape.Buffer.setByte(desptr, (long)((((y * w) + x) * 4) + 0), (byte)clamp((factor * sr) + bias));
					cape.Buffer.setByte(desptr, (long)((((y * w) + x) * 4) + 1), (byte)clamp((factor * sg) + bias));
					cape.Buffer.setByte(desptr, (long)((((y * w) + x) * 4) + 2), (byte)clamp((factor * sb) + bias));
					cape.Buffer.setByte(desptr, (long)((((y * w) + x) * 4) + 3), (byte)clamp((factor * sa) + bias));
				}
			}
			return(capex.BitmapBuffer.create(desbuf, w, h));
		}
	}
}
