
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
	public class Math
	{
		public const double M_PI = 3.14;
		public const double M_PI_2 = 1.57;
		public const double M_PI_4 = 0.79;
		public const double M_1_PI = 0.32;
		public const double M_2_PI = 0.64;
		public const double M_2_SQRTPI = 1.13;
		public const double M_SQRT2 = 1.41;
		public const double M_SQRT1_2 = 0.71;

		public static double abs(double d) {
			return(System.Math.Abs(d));
		}

		public static float abs(float f) {
			return(System.Math.Abs(f));
		}

		public static int abs(int i) {
			return(System.Math.Abs(i));
		}

		public static long abs(long l) {
			return(System.Math.Abs(l));
		}

		public static double acos(double d) {
			return(System.Math.Acos(d));
		}

		public static double asin(double d) {
			return(System.Math.Asin(d));
		}

		public static double atan(double d) {
			return(System.Math.Atan(d));
		}

		public static double atan2(double y, double x) {
			return(System.Math.Atan2(y,x));
		}

		public static double ceil(double d) {
			return(System.Math.Ceiling(d));
		}

		public static double cos(double d) {
			return(System.Math.Cos(d));
		}

		public static double cosh(double d) {
			return(System.Math.Cosh(d));
		}

		public static double exp(double d) {
			return(System.Math.Exp(d));
		}

		public static double floor(double d) {
			return(System.Math.Floor(d));
		}

		public static double remainder(double x, double y) {
			return(System.Math.IEEERemainder (x,y));
		}

		public static double log(double d) {
			return(System.Math.Log(d));
		}

		public static double log10(double d) {
			return(System.Math.Log10(d));
		}

		public static double max(double d1, double d2) {
			return(System.Math.Max(d1,d2));
		}

		public static float max(float f1, float f2) {
			return(System.Math.Max(f1,f2));
		}

		public static int max(int i1, int i2) {
			return(System.Math.Max(i1,i2));
		}

		public static long max(long l1, long l2) {
			return(System.Math.Max(l1,l2));
		}

		public static double min(double d1, double d2) {
			return(System.Math.Min(d1,d2));
		}

		public static float min(float f1, float f2) {
			return(System.Math.Min(f1,f2));
		}

		public static int min(int i1, int i2) {
			return(System.Math.Min(i1,i2));
		}

		public static long min(long l1, long l2) {
			return(System.Math.Min(l1,l2));
		}

		public static double pow(double x, double y) {
			return(System.Math.Pow(x,y));
		}

		public static double round(double d) {
			return(System.Math.Round(d));
		}

		public static double sin(double d) {
			return(System.Math.Sin(d));
		}

		public static double sinh(double d) {
			return(System.Math.Sinh(d));
		}

		public static double sqrt(double d) {
			return(System.Math.Sqrt(d));
		}

		public static double tan(double d) {
			return(System.Math.Tan(d));
		}

		public static double tanh(double d) {
			return(System.Math.Tanh(d));
		}

		public static double rint(double n) {
			return(System.Math.Round(n));
		}
	}
}
