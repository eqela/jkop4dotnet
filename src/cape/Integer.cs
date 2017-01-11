
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
	public class Integer
	{
		private class MyIntegerObject : IntegerObject
		{
			private int integer = 0;

			public virtual int toInteger() {
				return(integer);
			}

			public int getInteger() {
				return(integer);
			}

			public MyIntegerObject setInteger(int v) {
				integer = v;
				return(this);
			}
		}

		public static IntegerObject asObject(int integer) {
			var v = new MyIntegerObject();
			v.setInteger(integer);
			return((IntegerObject)v);
		}

		public static int asInteger(string str) {
			if(object.Equals(str, null)) {
				return(0);
			}
			return(cape.String.toInteger(str));
		}

		public static int asInteger(object obj) {
			if(obj == null) {
				return(0);
			}
			if(obj is IntegerObject) {
				return(((IntegerObject)obj).toInteger());
			}
			if(obj is string) {
				return(cape.String.toInteger((string)obj));
			}
			if(obj is StringObject) {
				return(cape.String.toInteger(((StringObject)obj).toString()));
			}
			if(obj is DoubleObject) {
				return((int)((DoubleObject)obj).toDouble());
			}
			if(obj is BooleanObject) {
				if(((BooleanObject)obj).toBoolean()) {
					return(1);
				}
				return(0);
			}
			if(obj is CharacterObject) {
				return((int)((CharacterObject)obj).toCharacter());
			}
			return(0);
		}
	}
}
