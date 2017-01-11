
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
	/// <summary>
	/// The Boolean class provides convenience methods for dealing with boolean values
	/// (either true or false).
	/// </summary>

	public class Boolean
	{
		private class MyBooleanObject : BooleanObject
		{
			private bool value = false;

			public virtual bool toBoolean() {
				return(value);
			}

			public bool getValue() {
				return(value);
			}

			public MyBooleanObject setValue(bool v) {
				value = v;
				return(this);
			}
		}

		/// <summary>
		/// Returns the given boolean value as a BooleanObject (which is an object type)
		/// that can be used wherever an object is required.
		/// </summary>

		public static BooleanObject asObject(bool value) {
			var v = new MyBooleanObject();
			v.setValue(value);
			return((BooleanObject)v);
		}

		/// <summary>
		/// Converts the given object to a boolean value, as much as is possible. Various
		/// different objects can be supplied (including instances of BooleanObject,
		/// IntegerObject, StringObject, DoubleObject, CharacterObject, ObjectWithSize as
		/// well as strings.
		/// </summary>

		public static bool asBoolean(object obj) {
			if(obj == null) {
				return(false);
			}
			if(obj is BooleanObject) {
				return(((BooleanObject)obj).toBoolean());
			}
			if(obj is IntegerObject) {
				if(((IntegerObject)obj).toInteger() == 0) {
					return(false);
				}
				return(true);
			}
			if(obj is string) {
				var str = cape.String.toLowerCase((string)obj);
				if((object.Equals(str, "yes")) || (object.Equals(str, "true"))) {
					return(true);
				}
				return(false);
			}
			if(obj is StringObject) {
				var str1 = ((StringObject)obj).toString();
				if(!(object.Equals(str1, null))) {
					str1 = cape.String.toLowerCase(str1);
					if((object.Equals(str1, "yes")) || (object.Equals(str1, "true"))) {
						return(true);
					}
				}
				return(false);
			}
			if(obj is DoubleObject) {
				if(((DoubleObject)obj).toDouble() == 0.00) {
					return(false);
				}
				return(true);
			}
			if(obj is CharacterObject) {
				if(((int)((CharacterObject)obj).toCharacter()) == 0) {
					return(false);
				}
				return(true);
			}
			if(obj is ObjectWithSize) {
				var sz = ((ObjectWithSize)obj).getSize();
				if(sz == 0) {
					return(false);
				}
				return(true);
			}
			return(false);
		}
	}
}
