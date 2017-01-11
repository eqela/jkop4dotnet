
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
	/// The String class provides all the common string manipulation functions,
	/// including comparisons, concatenation, transformations to upper and lowercase,
	/// getting substrings, finding characters or substrings, splitting the strings,
	/// converting to byte buffers, etc.
	/// </summary>

	public class String
	{
		/// <summary>
		/// Converts an arbitrary object to a string, if possible.
		/// </summary>

		public static string asString(object obj) {
			if(obj == null) {
				return(null);
			}
			if(obj is string) {
				return((string)obj);
			}
			if(obj is StringObject) {
				return(((StringObject)obj).toString());
			}
			if(obj is IntegerObject) {
				return(forInteger(((IntegerObject)obj).toInteger()));
			}
			if(obj is DoubleObject) {
				return(forDouble(((DoubleObject)obj).toDouble()));
			}
			if(obj is BooleanObject) {
				return(forBoolean(((BooleanObject)obj).toBoolean()));
			}
			if(obj is CharacterObject) {
				return(forCharacter(((CharacterObject)obj).toCharacter()));
			}
			var js = cape.JSONEncoder.encode(obj);
			if(!(object.Equals(js, null))) {
				return(js);
			}
			return(null);
		}

		/// <summary>
		/// Converts an integer to a string
		/// </summary>

		public static string asString(int value) {
			return(forInteger(value));
		}

		/// <summary>
		/// Converts a double to a string
		/// </summary>

		public static string asString(double value) {
			return(forDouble(value));
		}

		/// <summary>
		/// Converts a buffer to a string
		/// </summary>

		public static string asString(byte[] value) {
			return(forUTF8Buffer(value));
		}

		/// <summary>
		/// Converts a boolean to a string
		/// </summary>

		public static string asString(bool value) {
			return(forBoolean(value));
		}

		/// <summary>
		/// Converts a character to a string
		/// </summary>

		public static string asString(char value) {
			return(forCharacter(value));
		}

		/// <summary>
		/// Converts a float to a string
		/// </summary>

		public static string asString(float value) {
			return(forFloat(value));
		}

		/// <summary>
		/// Checks if a string is empty (ie., either the object is a null pointer or the
		/// length of the string is less than one character).
		/// </summary>

		public static bool isEmpty(string str) {
			if(object.Equals(str, null)) {
				return(true);
			}
			if(getLength(str) < 1) {
				return(true);
			}
			return(false);
		}

		/// <summary>
		/// Constructs a new string, given a buffer of bytes and the name of the encoding.
		/// Supported encodings: UTF8, UCS2 and ASCII
		/// </summary>

		public static string forBuffer(byte[] data, string encoding) {
			if(data == null) {
				return(null);
			}
			if(equalsIgnoreCase("UTF8", encoding) || equalsIgnoreCase("UTF-8", encoding)) {
				return(forUTF8Buffer(data));
			}
			if(equalsIgnoreCase("UCS2", encoding) || equalsIgnoreCase("UCS-2", encoding)) {
				return(forUCS2Buffer(data));
			}
			if(equalsIgnoreCase("ASCII", encoding)) {
				return(forASCIIBuffer(data));
			}
			return(null);
		}

		private static int getDataLength(byte[] data) {
			if(data == null) {
				return(0);
			}
			var v = 0;
			v = data.Length;
			while(v > 0) {
				if(data[v-1] == 0) {
					v--;
				}
				else {
					break;
				}
			}
			return(v);
		}

		/// <summary>
		/// Constructs a new string for a buffer of bytes, encoded using the ASCII character
		/// set.
		/// </summary>

		public static string forASCIIBuffer(byte[] data) {
			if(data == null) {
				return(null);
			}
			return(System.Text.Encoding.ASCII.GetString(data, 0, getDataLength(data)));
		}

		/// <summary>
		/// Constructs a new string for a buffer of bytes, encoded using the UTF8 character
		/// encoding.
		/// </summary>

		public static string forUTF8Buffer(byte[] data) {
			if(data == null) {
				return(null);
			}
			return(System.Text.Encoding.UTF8.GetString(data, 0, getDataLength(data)));
		}

		/// <summary>
		/// Constructs a new string for a buffer of bytes, encoded using the UCS2 character
		/// encoding.
		/// </summary>

		public static string forUCS2Buffer(byte[] data) {
			if(data == null) {
				return(null);
			}
			System.Console.WriteLine("[cape.String.forUCS2Buffer] (String.sling:311:2): Not implemented.");
			return(null);
		}

		/// <summary>
		/// Constructs a new string for an array or characters.
		/// </summary>

		public static string forCharArray(char[] chars, int offset, int count) {
			if(chars == null) {
				return(null);
			}
			return(new string(chars, offset, count));
		}

		/// <summary>
		/// Converts a boolean value to a string. The resulting string will be either "true"
		/// or "false", depending of the boolean value.
		/// </summary>

		public static string forBoolean(bool vv) {
			if(vv == true) {
				return("true");
			}
			return("false");
		}

		/// <summary>
		/// Converts an integer value to a string. The resulting string will be a string
		/// representation of the value of the integer using the "base-10" decimal notation.
		/// </summary>

		public static string forInteger(int vv) {
			return(vv.ToString());
		}

		/// <summary>
		/// Converts a long integer value to a string. The resulting string will be a string
		/// representation of the value of the integer using the "base-10" decimal notation.
		/// </summary>

		public static string forLong(long vv) {
			return(vv.ToString());
		}

		/// <summary>
		/// Converts an integer value to a string while ensuring that the length of the
		/// resulting string will reach or exceed the given "length". If the length of the
		/// string naturally is less than the given length, then "padding" characters will
		/// be prepended to the string in order to make the string long enough. The default
		/// padding character is "0", but can be customized with the "paddingString"
		/// parameter. eg. String.forIntegerWithPadding(9, 3, "0") would yield "009". eg.
		/// String.forIntegerWithPadding(10, 4, " ") would yield " 10".
		/// </summary>

		public static string forIntegerWithPadding(int vv, int length, string paddingString = null) {
			var r = forInteger(vv);
			if(object.Equals(r, null)) {
				return(null);
			}
			var ll = getLength(r);
			if(ll >= length) {
				return(r);
			}
			var ps = paddingString;
			if(object.Equals(ps, null)) {
				ps = "0";
			}
			var sb = new StringBuilder();
			var n = 0;
			for(n = 0 ; n < (length - ll) ; n++) {
				sb.append(ps);
			}
			sb.append(r);
			return(sb.toString());
		}

		/// <summary>
		/// Converts an integer value to a string using the "base-16" or hexadecimal
		/// notation.
		/// </summary>

		public static string forIntegerHex(int vv) {
			return(vv.ToString("X"));
		}

		/// <summary>
		/// Converts a character to a string. The result will be a single-character string.
		/// </summary>

		public static string forCharacter(char vv) {
			return(new string(vv, 1));
		}

		/// <summary>
		/// Converts a floating point value to a string.
		/// </summary>

		public static string forFloat(float vv) {
			return(vv.ToString());
		}

		/// <summary>
		/// Converts a double-precision floating point value to a string.
		/// </summary>

		public static string forDouble(double vv) {
			string v = null;
			v = vv.ToString();
			if(v != null && v.IndexOf('.') < 0) {
				v = v + ".0";
			}
			return(v);
		}

		/// <summary>
		/// Converts a string to a buffer of bytes, encoded using the UTF8 encoding.
		/// </summary>

		public static byte[] toUTF8Buffer(string str) {
			return(toBuffer(str, "UTF8"));
		}

		/// <summary>
		/// Converts a string to a buffer of bytes, encoded using the specified "charset" as
		/// the character set or encoding. Vaid options for "charset" are platform
		/// dependent, and would includes values such as UTF8, UTF16, UCS2 and ASCII. UTF8
		/// encoding, however, is implemented for all platforms, and is therefore the
		/// recommended character set to use.
		/// </summary>

		public static byte[] toBuffer(string str, string charset) {
			if((object.Equals(str, null)) || (object.Equals(charset, null))) {
				return(null);
			}
			var bytes = getBytesUnsigned(str, charset);
			if(bytes == null) {
				return(null);
			}
			var c = bytes.Length;
			var bb = new byte[c];
			var n = 0;
			for(n = 0 ; n < c ; n++) {
				cape.Buffer.setByte(bb, (long)n, bytes[n]);
			}
			return(bb);
		}

		private static byte[] getBytesUnsigned(string str) {
			return(getBytesUnsigned(str, "UTF-8"));
		}

		private static byte[] getBytesUnsigned(string str, string charset) {
			if((object.Equals(str, null)) || (object.Equals(charset, null))) {
				return(null);
			}
			byte[] bytes = null;
			System.Text.Encoding encoding;
			if("UTF-8".Equals(charset) || "UTF8".Equals(charset)) {
				encoding = System.Text.Encoding.UTF8;
			}
			else if("UTF-16".Equals(charset) || "UTF16".Equals(charset)) {
				encoding = System.Text.Encoding.Unicode;
			}
			else {
				return(null);
			}
			bytes = encoding.GetBytes(str);
			return(bytes);
		}

		private static sbyte[] getBytesSigned(string str) {
			return(getBytesSigned(str, "UTF-8"));
		}

		private static sbyte[] getBytesSigned(string str, string charset) {
			if((object.Equals(str, null)) || (object.Equals(charset, null))) {
				return(null);
			}
			sbyte[] bytes = null;
			var enc = getBytesUnsigned(str, charset);
			if(enc == null) {
				return(null);
			};
			bytes = new sbyte[enc.Length];
			System.Buffer.BlockCopy(enc, 0, bytes, 0, bytes.Length);
			return(bytes);
		}

		/// <summary>
		/// Gets the length of a string, representing the number of characters composing the
		/// string (note: This is not the same as the number of bytes allocated to hold the
		/// memory for the string).
		/// </summary>

		public static int getLength(string str) {
			if(object.Equals(str, null)) {
				return(0);
			}
			return(str.Length);
		}

		/// <summary>
		/// Appends a string "str2" to the end of another string "str1". Either one of the
		/// values may be null: If "str1" is null, the value of "str2" is returned, and if
		/// "str2" is null, the value of "str1" is returned.
		/// </summary>

		public static string append(string str1, string str2) {
			if(object.Equals(str1, null)) {
				return(str2);
			}
			if(object.Equals(str2, null)) {
				return(str1);
			}
			return(System.String.Concat(str1, str2));
		}

		/// <summary>
		/// Appends an integer value "intvalue" to the end of string "str". If the original
		/// string is null, then a new string is returned, representing only the integer
		/// value.
		/// </summary>

		public static string append(string str, int intvalue) {
			return(cape.String.append(str, forInteger(intvalue)));
		}

		/// <summary>
		/// Appends a character value "charvalue" to the end of string "str". If the
		/// original string is null, then a new string is returned, representing only the
		/// character value.
		/// </summary>

		public static string append(string str, char charvalue) {
			return(cape.String.append(str, forCharacter(charvalue)));
		}

		/// <summary>
		/// Appends a floating point value "floatvalue" to the end of string "str". If the
		/// original string is null, then a new string is returned, representing only the
		/// floating point value.
		/// </summary>

		public static string append(string str, float floatvalue) {
			return(cape.String.append(str, forFloat(floatvalue)));
		}

		/// <summary>
		/// Appends a double-precision floating point value "doublevalue" to the end of
		/// string "str". If the original string is null, then a new string is returned,
		/// representing only the floating point value.
		/// </summary>

		public static string append(string str, double doublevalue) {
			return(cape.String.append(str, forDouble(doublevalue)));
		}

		/// <summary>
		/// Appends a boolean value "boolvalue" to the end of string "str". If the original
		/// string is null, then a new string is returned, representing only the boolean
		/// value.
		/// </summary>

		public static string append(string str, bool boolvalue) {
			return(cape.String.append(str, forBoolean(boolvalue)));
		}

		/// <summary>
		/// Converts all characters of a string to lowercase.
		/// </summary>

		public static string toLowerCase(string str) {
			if(object.Equals(str, null)) {
				return(null);
			}
			return(str.ToLower());
		}

		/// <summary>
		/// Converts all characters of a string to uppercase.
		/// </summary>

		public static string toUpperCase(string str) {
			if(object.Equals(str, null)) {
				return(null);
			}
			return(str.ToUpper());
		}

		/// <summary>
		/// Gets a character with the specified index "index" from the given string. This
		/// method is deprecated: Use getChar() instead.
		/// </summary>

		public static char charAt(string str, int index) {
			return(getChar(str, index));
		}

		/// <summary>
		/// Gets a character with the specified index "index" from the given string.
		/// </summary>

		public static char getChar(string str, int index) {
			if(object.Equals(str, null)) {
				return((char)0);
			}
			return(str == null || index < 0 || index >= str.Length ? (char)0 : str[index]);
		}

		/// <summary>
		/// Compares two strings, and returns true if both strings contain exactly the same
		/// contents (even though they may be represented by different objects). Either of
		/// the strings may be null, in which case the comparison always results to false.
		/// </summary>

		public static bool equals(string str1, string str2) {
			if((object.Equals(str1, null)) || (object.Equals(str2, null))) {
				return(false);
			}
			return(str1.Equals(str2));
		}

		/// <summary>
		/// Compares two strings for equality (like equals()) while considering uppercase
		/// and lowercase versions of the same character as equivalent. Eg. strings
		/// "ThisIsAString" and "thisisastring" would be considered equivalent, and the
		/// result of comparison would be "true".
		/// </summary>

		public static bool equalsIgnoreCase(string str1, string str2) {
			if((object.Equals(str1, null)) || (object.Equals(str2, null))) {
				return(false);
			}
			return(System.String.Compare(str1, str2, true) == 0);
		}

		/// <summary>
		/// Compares two strings, and returns an integer value representing their sorting
		/// order. The return value 0 indicates that the two strings are equivalent. A
		/// negative return value (< 0) indicates that "str1" is less than "str2", and a
		/// positive return value (> 0) indicates that "str1" is greater than "str2". If
		/// either "str1" or "str2" is null, the comparison yields the value 0.
		/// </summary>

		public static int compare(string str1, string str2) {
			if((object.Equals(str1, null)) || (object.Equals(str2, null))) {
				return(0);
			}
			return(System.String.Compare(str1, str2, false));
		}

		/// <summary>
		/// Compares strings exactly like compareTo(), but this method considers uppercase
		/// and lowercase versions of each character as equivalent.
		/// </summary>

		public static int compareToIgnoreCase(string str1, string str2) {
			if((object.Equals(str1, null)) || (object.Equals(str2, null))) {
				return(0);
			}
			return(System.String.Compare(str1, str2, true));
		}

		/// <summary>
		/// Gets a hash code version of the string as an integer.
		/// </summary>

		public static int getHashCode(string str) {
			if(object.Equals(str, null)) {
				return(0);
			}
			return(str.GetHashCode());
		}

		/// <summary>
		/// Gets the first index of a given character "c" in string "str", starting from
		/// index "start". This method is deprecated, use getIndexOf() instead.
		/// </summary>

		public static int indexOf(string str, char c, int start = 0) {
			return(getIndexOf(str, c, start));
		}

		/// <summary>
		/// Gets the first index of a given character "c" in string "str", starting from
		/// index "start".
		/// </summary>

		public static int getIndexOf(string str, char c, int start = 0) {
			if(object.Equals(str, null)) {
				return(-1);
			}
			return(str.IndexOf(c, start));
		}

		/// <summary>
		/// Gets the first index of a given substring "s" in string "str", starting from
		/// index "start". This method is deprecated, use getIndexOf() instead.
		/// </summary>

		public static int indexOf(string str, string s, int start = 0) {
			return(getIndexOf(str, s, start));
		}

		/// <summary>
		/// Gets the first index of a given substring "s" in string "str", starting from
		/// index "start".
		/// </summary>

		public static int getIndexOf(string str, string s, int start = 0) {
			if(object.Equals(str, null)) {
				return(-1);
			}
			return(str.IndexOf(s, start));
		}

		/// <summary>
		/// Gets the last index of a given character "c" in string "str", starting from
		/// index "start". This method is deprecated, use getLastIndexOf() instead.
		/// </summary>

		public static int lastIndexOf(string str, char c, int start = -1) {
			return(getLastIndexOf(str, c, start));
		}

		/// <summary>
		/// Gets the last index of a given character "c" in string "str", starting from
		/// index "start".
		/// </summary>

		public static int getLastIndexOf(string str, char c, int start = -1) {
			if(object.Equals(str, null)) {
				return(-1);
			}
			if(start < 0) {
				return(str.LastIndexOf(c));
			}
			return(str.LastIndexOf(c, start));
		}

		/// <summary>
		/// Gets the last index of a given substring "s" in string "str", starting from
		/// index "start". This method is deprecated, use getLastIndexOf() instead.
		/// </summary>

		public static int lastIndexOf(string str, string s, int start = -1) {
			return(getLastIndexOf(str, s, start));
		}

		/// <summary>
		/// Gets the last index of a given substring "s" in string "str", starting from
		/// index "start".
		/// </summary>

		public static int getLastIndexOf(string str, string s, int start = -1) {
			if(object.Equals(str, null)) {
				return(-1);
			}
			if(start < 0) {
				return(str.LastIndexOf(s));
			}
			return(str.LastIndexOf(s, start));
		}

		/// <summary>
		/// Gets a substring of "str", starting from index "start". This method is
		/// deprecated, use getSubString() instead.
		/// </summary>

		public static string subString(string str, int start) {
			return(getSubString(str, start));
		}

		/// <summary>
		/// Gets a substring of "str", starting from index "start".
		/// </summary>

		public static string getSubString(string str, int start) {
			if(object.Equals(str, null)) {
				return(null);
			}
			if(start >= getLength(str)) {
				return("");
			}
			var ss = start;
			if(ss < 0) {
				ss = 0;
			}
			return(str.Substring(ss));
		}

		/// <summary>
		/// Gets a substring of "str", starting from index "start" and with a maximum length
		/// of "length". This method is deprecated, use getSubString() instead.
		/// </summary>

		public static string subString(string str, int start, int length) {
			return(getSubString(str, start, length));
		}

		/// <summary>
		/// Gets a substring of "str", starting from index "start" and with a maximum length
		/// of "length".
		/// </summary>

		public static string getSubString(string str, int start, int length) {
			if(object.Equals(str, null)) {
				return(null);
			}
			var strl = getLength(str);
			if(start >= strl) {
				return("");
			}
			var ss = start;
			if(ss < 0) {
				ss = 0;
			}
			var ll = length;
			if(ll > (strl - start)) {
				ll = strl - start;
			}
			return(str.Substring(ss, ll));
		}

		/// <summary>
		/// Checks is the string "str1" contains a substring "str2". Returns true if the
		/// substring is found.
		/// </summary>

		public static bool contains(string str1, string str2) {
			if((object.Equals(str1, null)) || (object.Equals(str2, null))) {
				return(false);
			}
			return(str1.Contains(str2));
		}

		/// <summary>
		/// Checks if the string "str1" starts with the complete contents of "str2". If the
		/// "offset" parameter is supplied an is greater than zero, the checking does not
		/// start from the beginning of "str1" but from the given character index.
		/// </summary>

		public static bool startsWith(string str1, string str2, int offset = 0) {
			if((object.Equals(str1, null)) || (object.Equals(str2, null))) {
				return(false);
			}
			string nstr = null;
			if(offset > 0) {
				nstr = getSubString(str1, offset);
			}
			else {
				nstr = str1;
			}
			return(nstr.StartsWith(str2));
		}

		/// <summary>
		/// Checks if the string "str1" ends with the complete contents of "str2".
		/// </summary>

		public static bool endsWith(string str1, string str2) {
			if((object.Equals(str1, null)) || (object.Equals(str2, null))) {
				return(false);
			}
			return(str1.EndsWith(str2));
		}

		/// <summary>
		/// Strips (or trims) the given string "str" by removing all blank characters from
		/// both ends (the beginning and the end) of the string.
		/// </summary>

		public static string strip(string str) {
			if(object.Equals(str, null)) {
				return(null);
			}
			return(str.Trim());
		}

		/// <summary>
		/// Replaces all instances of "oldChar" with the value of "newChar" in the given
		/// string "str". The return value is a new string where the changes have been
		/// effected. The original string remains unchanged.
		/// </summary>

		public static string replace(string str, char oldChar, char newChar) {
			if(object.Equals(str, null)) {
				return(null);
			}
			return(str.Replace(oldChar, newChar));
		}

		/// <summary>
		/// Replaces all instances of the substring "target" with the value of "replacement"
		/// in the given string "str". The return value is a new string where the changes
		/// have been effected. The original string remains unchanged.
		/// </summary>

		public static string replace(string str, string target, string replacement) {
			if(object.Equals(str, null)) {
				return(null);
			}
			return(str.Replace(target, replacement));
		}

		/// <summary>
		/// Converts the string "str" to an array of characters.
		/// </summary>

		public static char[] toCharArray(string str) {
			if(object.Equals(str, null)) {
				return(null);
			}
			return(str.ToCharArray());
		}

		/// <summary>
		/// Splits the string "str" to a vector of strings, cutting the original string at
		/// each instance of the character "delim". If the value of "max" is supplied and is
		/// given a value greater than 0, then the resulting vector will only have a maximum
		/// of "max" entries. If the delimiter character appears beyond the maximum entries,
		/// the last entry in the vector will be the complete remainder of the original
		/// string, including the remaining delimiter characters.
		/// </summary>

		public static System.Collections.Generic.List<string> split(string str, char delim, int max = 0) {
			var v = new System.Collections.Generic.List<string>();
			if(object.Equals(str, null)) {
				return(v);
			}
			var n = 0;
			while(true) {
				if((max > 0) && (cape.Vector.getSize(v) >= (max - 1))) {
					cape.Vector.append(v, subString(str, n));
					break;
				}
				var x = indexOf(str, delim, n);
				if(x < 0) {
					cape.Vector.append(v, subString(str, n));
					break;
				}
				cape.Vector.append(v, subString(str, n, x - n));
				n = x + 1;
			}
			return(v);
		}

		/// <summary>
		/// Checks if the given string is fully an integer (no other characters appear in
		/// the string).
		/// </summary>

		public static bool isInteger(string str) {
			if(object.Equals(str, null)) {
				return(false);
			}
			var it = iterate(str);
			if(it == null) {
				return(false);
			}
			while(true) {
				var c = it.getNextChar();
				if(c < 1) {
					break;
				}
				if((c < '0') || (c > '9')) {
					return(false);
				}
			}
			return(true);
		}

		/// <summary>
		/// Converts the string to an integer. If the string does not represent a valid
		/// integer, than the value "0" is returned. Eg. toInteger("99") = 99 Eg.
		/// toInteger("asdf") = 0
		/// </summary>

		public static int toInteger(string str) {
			if(isEmpty(str)) {
				return(0);
			}
			var v = 0;
			var m = getLength(str);
			var n = 0;
			for(n = 0 ; n < m ; n++) {
				var c = charAt(str, n);
				if((c >= '0') && (c <= '9')) {
					v = v * 10;
					v += (int)(c - '0');
				}
				else {
					break;
				}
			}
			return(v);
		}

		/// <summary>
		/// Converts the string to a long integer. If the string does not represent a valid
		/// integer, than the value "0" is returned. Eg. toLong("99") = 99 Eg.
		/// toLong("asdf") = 0
		/// </summary>

		public static long toLong(string str) {
			if(isEmpty(str)) {
				return((long)0);
			}
			var v = (long)0;
			var m = getLength(str);
			var n = 0;
			for(n = 0 ; n < m ; n++) {
				var c = charAt(str, n);
				if((c >= '0') && (c <= '9')) {
					v = v * 10;
					v += (long)(c - '0');
				}
				else {
					break;
				}
			}
			return(v);
		}

		/// <summary>
		/// Converts the string to an integer, while treating the string as a hexadecimal
		/// representation of a number, eg. toIntegerFromHex("a") = 10
		/// </summary>

		public static int toIntegerFromHex(string str) {
			if(isEmpty(str)) {
				return(0);
			}
			var v = 0;
			var m = getLength(str);
			var n = 0;
			for(n = 0 ; n < m ; n++) {
				var c = charAt(str, n);
				if((c >= '0') && (c <= '9')) {
					v = v * 16;
					v += (int)(c - '0');
				}
				else if((c >= 'a') && (c <= 'f')) {
					v = v * 16;
					v += (int)((10 + c) - 'a');
				}
				else if((c >= 'A') && (c <= 'F')) {
					v = v * 16;
					v += (int)((10 + c) - 'A');
				}
				else {
					break;
				}
			}
			return(v);
		}

		/// <summary>
		/// Converts the string to a double-precision floating point number. If the string
		/// does not contain a valid representation of a floating point number, then the
		/// value "0.0" is returned.
		/// </summary>

		public static double toDouble(string str) {
			if(object.Equals(str, null)) {
				return(0.00);
			}
			var v = 0.00;
			try {
				v = System.Double.Parse(str);
			}
			catch {
				v = 0.0;
			}
			return(v);
		}

		/// <summary>
		/// Iterates the string "string" character by character by using an instance of
		/// CharacterIterator.
		/// </summary>

		public static CharacterIterator iterate(string @string) {
			return((CharacterIterator)new CharacterIteratorForString(@string));
		}

		/// <summary>
		/// Creates a new string that contains the same contents as "string", but with all
		/// characters appearing in reverse order.
		/// </summary>

		public static string reverse(string @string) {
			if(object.Equals(@string, null)) {
				return(null);
			}
			var sb = new StringBuilder();
			var it = iterate(@string);
			var c = ' ';
			while((c = it.getNextChar()) > 0) {
				sb.insert(0, c);
			}
			return(sb.toString());
		}

		/// <summary>
		/// Iterates the string "string" in reverse order (starting from the end, moving
		/// towards the beginning).
		/// </summary>

		public static CharacterIterator iterateReverse(string @string) {
			return(iterate(reverse(@string)));
		}

		/// <summary>
		/// Creates a new string based on "string" that is at least "length" characters
		/// long. If the original string is shorter, it will be padded to the desired length
		/// by adding instances of paddingCharacter (default of which is the space
		/// character). The "aling" attribute may be either -1, 0 or 1, and determines if
		/// the padding characters willb e added to the end, both sides or to the beginning,
		/// respectively.
		/// </summary>

		public static string padToLength(string @string, int length, int align = -1, char paddingCharacter = ' ') {
			var ll = 0;
			if(object.Equals(@string, null)) {
				ll = 0;
			}
			else {
				ll = getLength(@string);
			}
			if(ll >= length) {
				return(@string);
			}
			var add = length - ll;
			var n = 0;
			var sb = new StringBuilder();
			if(align < 0) {
				sb.append(@string);
				for(n = 0 ; n < add ; n++) {
					sb.append(paddingCharacter);
				}
			}
			else if(align > 0) {
				var ff = add / 2;
				var ss = add - ff;
				for(n = 0 ; n < ff ; n++) {
					sb.append(paddingCharacter);
				}
				sb.append(@string);
				for(n = 0 ; n < ss ; n++) {
					sb.append(paddingCharacter);
				}
			}
			else {
				for(n = 0 ; n < add ; n++) {
					sb.append(paddingCharacter);
				}
				sb.append(@string);
			}
			return(sb.toString());
		}

		/// <summary>
		/// Splits the given string "str" to a vector of multiple strings, cutting the
		/// string at each instance of the "delim" character where the delim does not appear
		/// enclosed in either double quotes or single quotes. Eg.
		/// quotedStringToVector("first 'second third'", ' ') => [ "first", "second third" ]
		/// </summary>

		public static System.Collections.Generic.List<string> quotedStringToVector(string str, char delim) {
			var v = new System.Collections.Generic.List<string>();
			if(object.Equals(str, null)) {
				return(v);
			}
			var dquote = false;
			var quote = false;
			StringBuilder sb = null;
			var c = ' ';
			var it = iterate(str);
			while((c = it.getNextChar()) > 0) {
				if((c == '\"') && (quote == false)) {
					dquote = !dquote;
				}
				else if((c == '\'') && (dquote == false)) {
					quote = !quote;
				}
				else if(((quote == false) && (dquote == false)) && (c == delim)) {
					if(sb != null) {
						var r = sb.toString();
						if(object.Equals(r, null)) {
							r = "";
						}
						v.Add(r);
					}
					sb = null;
				}
				else {
					if(sb == null) {
						sb = new StringBuilder();
					}
					sb.append(c);
				}
				if(((quote == true) || (dquote == true)) && (sb == null)) {
					sb = new StringBuilder();
				}
			}
			if(sb != null) {
				var r1 = sb.toString();
				if(object.Equals(r1, null)) {
					r1 = "";
				}
				v.Add(r1);
			}
			return(v);
		}

		/// <summary>
		/// Parses the string "str", splitting it to multiple strings using
		/// quotedStringToVector(), and then further processing it to key/value pairs,
		/// splitting each string at the equal sign '=' and adding the entries to a map.
		/// </summary>

		public static System.Collections.Generic.Dictionary<string,string> quotedStringToMap(string str, char delim) {
			var v = new System.Collections.Generic.Dictionary<string,string>();
			var vector = quotedStringToVector(str, delim);
			if(vector != null) {
				var n = 0;
				var m = vector.Count;
				for(n = 0 ; n < m ; n++) {
					var c = vector[n];
					if(c != null) {
						var sp = split(c, '=', 2);
						var key = sp[0];
						var val = sp[1];
						if(isEmpty(key) == false) {
							v[key] = val;
						}
					}
				}
			}
			return(v);
		}

		/// <summary>
		/// Combines a vector of strings to a single string, incorporating the "delim"
		/// character between each string in the vector. If the "unique" variable is set to
		/// "true", only one instance of each unique string will be appended to the
		/// resulting string.
		/// </summary>

		public static string combine(System.Collections.Generic.List<string> strings, char delim, bool unique = false) {
			var sb = new StringBuilder();
			System.Collections.Generic.Dictionary<string,string> flags = null;
			if(unique) {
				flags = new System.Collections.Generic.Dictionary<string,string>();
			}
			if(strings != null) {
				var n = 0;
				var m = strings.Count;
				for(n = 0 ; n < m ; n++) {
					var o = strings[n];
					if(o != null) {
						if(object.Equals(o, null)) {
							continue;
						}
						if(flags != null) {
							if(!(object.Equals(cape.Map.get(flags, o), null))) {
								continue;
							}
							cape.Map.set(flags, o, "true");
						}
						if((delim > 0) && (sb.count() > 0)) {
							sb.append(delim);
						}
						sb.append(o);
					}
				}
			}
			return(sb.toString());
		}

		/// <summary>
		/// Vaidates a string character-by-character, calling the supplied function to
		/// determine the validity of each character. Returns true if the entire string was
		/// validated, false if not.
		/// </summary>

		public static bool validateCharacters(string str, System.Func<char, bool> validator) {
			if(object.Equals(str, null)) {
				return(false);
			}
			var it = iterate(str);
			if(it == null) {
				return(false);
			}
			while(true) {
				var c = it.getNextChar();
				if(c < 1) {
					break;
				}
				if(validator(c) == false) {
					return(false);
				}
			}
			return(true);
		}
	}
}
