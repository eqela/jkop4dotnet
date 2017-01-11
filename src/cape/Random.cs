
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
	public class Random
	{
		System.Random random;

		public Random() {
			random = new System.Random();
		}

		public Random(long seed) {
			setSeed(seed);
		}

		public void setSeed(long seed) {
			random = new System.Random((System.Int32)seed);
		}

		public bool nextBoolean() {
			if((nextInt() % 2) == 0) {
				return(false);
			}
			return(true);
		}

		public int nextInt() {
			return(random.Next());
		}

		public int nextInt(int n) {
			return(random.Next(n));
		}

		/// <summary>
		/// Used to retrieve integer values from "s" (inclusive) and "e" (exclusive)
		/// </summary>

		public int nextInt(int s, int e) {
			return(random.Next(s, e));
		}

		public void nextBytes(sbyte[] buf) {
			random.NextBytes((byte[])(System.Array)buf);
		}

		public double nextDouble() {
			return(random.NextDouble());
		}

		public float nextFloat() {
			return((float)random.NextDouble());
		}

		public double nextGaussian() {
			System.Console.WriteLine("--- stub --- cape.Random :: nextGaussian");
			return(0.00);
		}
	}
}
