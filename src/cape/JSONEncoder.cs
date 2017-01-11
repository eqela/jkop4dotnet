
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
	public class JSONEncoder
	{
		private bool isNewLine = true;

		private void print(string line, int indent, bool startline, bool endline, StringBuilder sb, bool niceFormatting) {
			if(startline && (isNewLine == false)) {
				if(niceFormatting) {
					sb.append('\n');
				}
				else {
					sb.append(' ');
				}
				isNewLine = true;
			}
			if(isNewLine && niceFormatting) {
				for(var n = 0 ; n < indent ; n++) {
					sb.append('\t');
				}
			}
			sb.append(line);
			if(endline) {
				if(niceFormatting) {
					sb.append('\n');
				}
				else {
					sb.append(' ');
				}
				isNewLine = true;
			}
			else {
				isNewLine = false;
			}
		}

		private void encodeArray(object[] cc, int indent, StringBuilder sb, bool niceFormatting) {
			print("[", indent, false, true, sb, niceFormatting);
			var first = true;
			if(cc != null) {
				var n = 0;
				var m = cc.Length;
				for(n = 0 ; n < m ; n++) {
					var o = cc[n];
					if(o != null) {
						if(first == false) {
							print(",", indent, false, true, sb, niceFormatting);
						}
						encodeObject(o, indent + 1, sb, niceFormatting);
						first = false;
					}
				}
			}
			print("]", indent, true, false, sb, niceFormatting);
		}

		private void encodeDynamicVector(DynamicVector cc, int indent, StringBuilder sb, bool niceFormatting) {
			print("[", indent, false, true, sb, niceFormatting);
			var first = true;
			var it = cc.iterate();
			while(it != null) {
				var o = it.next();
				if(o == null) {
					break;
				}
				if(first == false) {
					print(",", indent, false, true, sb, niceFormatting);
				}
				encodeObject(o, indent + 1, sb, niceFormatting);
				first = false;
			}
			print("]", indent, true, false, sb, niceFormatting);
		}

		private void encodeVector(System.Collections.Generic.List<object> cc, int indent, StringBuilder sb, bool niceFormatting) {
			print("[", indent, false, true, sb, niceFormatting);
			var first = true;
			if(cc != null) {
				var n = 0;
				var m = cc.Count;
				for(n = 0 ; n < m ; n++) {
					var o = cc[n];
					if(o != null) {
						if(first == false) {
							print(",", indent, false, true, sb, niceFormatting);
						}
						encodeObject(o, indent + 1, sb, niceFormatting);
						first = false;
					}
				}
			}
			print("]", indent, true, false, sb, niceFormatting);
		}

		private void encodeMap(System.Collections.Generic.Dictionary<string,object> map, int indent, StringBuilder sb, bool niceFormatting) {
			print("{", indent, false, true, sb, niceFormatting);
			var first = true;
			System.Collections.Generic.List<string> keys = cape.Map.getKeys(map);
			if(keys != null) {
				var n = 0;
				var m = keys.Count;
				for(n = 0 ; n < m ; n++) {
					var key = keys[n];
					if(key != null) {
						if(first == false) {
							print(",", indent, false, true, sb, niceFormatting);
						}
						encodeString(key, indent + 1, sb, niceFormatting);
						sb.append(" : ");
						encodeObject(map[key], indent + 1, sb, niceFormatting);
						first = false;
					}
				}
			}
			print("}", indent, true, false, sb, niceFormatting);
		}

		public void encodeDynamicMap(DynamicMap map, int indent, StringBuilder sb, bool niceFormatting) {
			print("{", indent, false, true, sb, niceFormatting);
			var first = true;
			var keys = map.getKeys();
			if(keys != null) {
				var n = 0;
				var m = keys.Count;
				for(n = 0 ; n < m ; n++) {
					var key = keys[n];
					if(key != null) {
						if(first == false) {
							print(",", indent, false, true, sb, niceFormatting);
						}
						encodeString(key, indent + 1, sb, niceFormatting);
						sb.append(" : ");
						encodeObject(map.get(key), indent + 1, sb, niceFormatting);
						first = false;
					}
				}
			}
			print("}", indent, true, false, sb, niceFormatting);
		}

		private void encodeKeyValueList(KeyValueListForStrings list, int indent, StringBuilder sb, bool niceFormatting) {
			print("{", indent, false, true, sb, niceFormatting);
			var first = true;
			Iterator<KeyValuePair<string, string>> it = list.iterate();
			while(it != null) {
				var pair = it.next();
				if(pair == null) {
					break;
				}
				if(first == false) {
					print(",", indent, false, true, sb, niceFormatting);
				}
				encodeString(pair.key, indent + 1, sb, niceFormatting);
				sb.append(" : ");
				encodeString(pair.value, indent + 1, sb, niceFormatting);
				first = false;
			}
			print("}", indent, true, false, sb, niceFormatting);
		}

		private void encodeString(string s, int indent, StringBuilder sb, bool niceFormatting) {
			var mysb = new StringBuilder();
			mysb.append('\"');
			var it = new CharacterIteratorForString(s);
			while(true) {
				var c = it.getNextChar();
				if(c < 1) {
					break;
				}
				if(c == '\"') {
					mysb.append('\\');
				}
				else if(c == '\\') {
					mysb.append('\\');
				}
				mysb.append(c);
			}
			mysb.append('\"');
			print(mysb.toString(), indent, false, false, sb, niceFormatting);
		}

		private void encodeObject(object o, int indent, StringBuilder sb, bool niceFormatting) {
			if(o == null) {
				encodeString("", indent, sb, niceFormatting);
			}
			else if(o is object[]) {
				encodeArray((object[])o, indent, sb, niceFormatting);
			}
			else if(o is System.Collections.Generic.List<object>) {
				encodeVector((System.Collections.Generic.List<object>)o, indent, sb, niceFormatting);
			}
			else if(o is DynamicMap) {
				encodeDynamicMap((DynamicMap)o, indent, sb, niceFormatting);
			}
			else if(o is string) {
				encodeString((string)o, indent, sb, niceFormatting);
			}
			else if(o is StringObject) {
				encodeString(((StringObject)o).toString(), indent, sb, niceFormatting);
			}
			else if(o is ArrayObject<object>) {
				object[] aa = ((ArrayObject<object>)o).toArray();
				encodeArray(aa, indent, sb, niceFormatting);
			}
			else if(o is VectorObject<object>) {
				System.Collections.Generic.List<object> vv = ((VectorObject<object>)o).toVector();
				encodeVector(vv, indent, sb, niceFormatting);
			}
			else if(o is DynamicVector) {
				System.Collections.Generic.List<object> vv1 = ((VectorObject<object>)o).toVector();
				encodeDynamicVector(cape.DynamicVector.forObjectVector(vv1), indent, sb, niceFormatting);
			}
			else if(o is KeyValueListForStrings) {
				encodeKeyValueList((KeyValueListForStrings)o, indent, sb, niceFormatting);
			}
			else if((((o is IntegerObject) || (o is BooleanObject)) || (o is DoubleObject)) || (o is CharacterObject)) {
				encodeString(cape.String.asString(o), indent, sb, niceFormatting);
			}
			else {
				encodeString("", indent, sb, niceFormatting);
			}
		}

		public static string encode(object o, bool niceFormatting = true) {
			var sb = new StringBuilder();
			new JSONEncoder().encodeObject(o, 0, sb, niceFormatting);
			return(sb.toString());
		}
	}
}
