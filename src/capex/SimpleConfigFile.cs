
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
	public class SimpleConfigFile
	{
		public static SimpleConfigFile forFile(cape.File file) {
			var v = new SimpleConfigFile();
			if(v.read(file) == false) {
				v = null;
			}
			return(v);
		}

		public static SimpleConfigFile forMap(cape.DynamicMap map) {
			var v = new SimpleConfigFile();
			v.setDataAsMap(map);
			return(v);
		}

		public static cape.DynamicMap readFileAsMap(cape.File file) {
			var cf = forFile(file);
			if(cf == null) {
				return(null);
			}
			return(cf.getDataAsMap());
		}

		private cape.KeyValueList<string, string> data = null;
		private cape.DynamicMap mapData = null;
		private cape.File file = null;

		public SimpleConfigFile() {
			data = new cape.KeyValueList<string, string>();
		}

		public cape.File getFile() {
			return(file);
		}

		public cape.File getRelativeFile(string fileName) {
			if((file == null) || (object.Equals(fileName, null))) {
				return(null);
			}
			var p = file.getParent();
			if(p == null) {
				return(null);
			}
			return(p.entry(fileName));
		}

		public void clear() {
			data.clear();
			mapData = null;
		}

		public SimpleConfigFile setDataAsMap(cape.DynamicMap map) {
			clear();
			var keys = map.iterateKeys();
			while(keys != null) {
				var key = keys.next();
				if(object.Equals(key, null)) {
					break;
				}
				data.add((string)key, (string)map.getString(key));
			}
			return(this);
		}

		public cape.DynamicMap getDataAsMap() {
			if(mapData == null) {
				mapData = new cape.DynamicMap();
				var it = data.iterate();
				while(it != null) {
					var kvp = it.next();
					if(kvp == null) {
						break;
					}
					mapData.set(kvp.key, (object)kvp.value);
				}
			}
			return(mapData);
		}

		public cape.DynamicMap getDynamicMapValue(string key, cape.DynamicMap defval) {
			var str = getStringValue(key, null);
			if(object.Equals(str, null)) {
				return(defval);
			}
			var v = cape.JSONParser.parse(str) as cape.DynamicMap;
			if(v == null) {
				return(defval);
			}
			return(v);
		}

		public cape.DynamicVector getDynamicVectorValue(string key, cape.DynamicVector defval) {
			var str = getStringValue(key, null);
			if(object.Equals(str, null)) {
				return(defval);
			}
			var v = cape.JSONParser.parse(str) as cape.DynamicVector;
			if(v == null) {
				return(defval);
			}
			return(v);
		}

		public string getStringValue(string key, string defval = null) {
			var map = getDataAsMap();
			if(map == null) {
				return(defval);
			}
			var v = map.getString(key);
			if(object.Equals(v, null)) {
				return(defval);
			}
			if(cape.String.startsWith(v, "\"\"\"\n") && cape.String.endsWith(v, "\n\"\"\"")) {
				v = cape.String.getSubString(v, 4, cape.String.getLength(v) - 8);
			}
			return(v);
		}

		public int getIntegerValue(string key, int defval = 0) {
			var map = getDataAsMap();
			if(map == null) {
				return(defval);
			}
			return(map.getInteger(key, defval));
		}

		public double getDoubleValue(string key, double defval = 0.00) {
			var map = getDataAsMap();
			if(map == null) {
				return(defval);
			}
			return(map.getDouble(key, defval));
		}

		public bool getBooleanValue(string key, bool defval = false) {
			var map = getDataAsMap();
			if(map == null) {
				return(defval);
			}
			return(map.getBoolean(key, defval));
		}

		public cape.File getFileValue(string key, cape.File defval = null) {
			var v = getRelativeFile(getStringValue(key, null));
			if(v == null) {
				return(defval);
			}
			return(v);
		}

		public cape.Iterator<cape.KeyValuePair<string, string>> iterate() {
			if(data == null) {
				return(null);
			}
			return(data.iterate());
		}

		public virtual bool read(cape.File file) {
			if(file == null) {
				return(false);
			}
			var reader = file.read();
			if(reader == null) {
				return(false);
			}
			var ins = new cape.PrintReader((cape.Reader)reader);
			string line = null;
			string tag = null;
			cape.StringBuilder lineBuffer = null;
			string terminator = null;
			while(!(object.Equals(line = ins.readLine(), null))) {
				if(lineBuffer != null) {
					lineBuffer.append(line);
					if(object.Equals(line, terminator)) {
						line = lineBuffer.toString();
						lineBuffer = null;
						terminator = null;
					}
					else {
						lineBuffer.append('\n');
						continue;
					}
				}
				line = cape.String.strip(line);
				if(cape.String.isEmpty(line) || cape.String.startsWith(line, "#")) {
					continue;
				}
				if(cape.String.endsWith(line, "{")) {
					if(cape.String.indexOf(line, ':') < 0) {
						if(object.Equals(tag, null)) {
							tag = cape.String.strip(cape.String.getSubString(line, 0, cape.String.getLength(line) - 1));
						}
						continue;
					}
					else {
						lineBuffer = new cape.StringBuilder();
						lineBuffer.append(line);
						lineBuffer.append('\n');
						terminator = "}";
						continue;
					}
				}
				if(cape.String.endsWith(line, "[")) {
					lineBuffer = new cape.StringBuilder();
					lineBuffer.append(line);
					lineBuffer.append('\n');
					terminator = "]";
					continue;
				}
				if(cape.String.endsWith(line, "\"\"\"")) {
					lineBuffer = new cape.StringBuilder();
					lineBuffer.append(line);
					lineBuffer.append('\n');
					terminator = "\"\"\"";
					continue;
				}
				if(!(object.Equals(tag, null)) && (object.Equals(line, "}"))) {
					tag = null;
					continue;
				}
				var sp = cape.String.split(line, ':', 2);
				if(sp == null) {
					continue;
				}
				var key = cape.String.strip(cape.Vector.get(sp, 0));
				var val = cape.String.strip(cape.Vector.get(sp, 1));
				if(cape.String.startsWith(val, "\"") && cape.String.endsWith(val, "\"")) {
					val = cape.String.getSubString(val, 1, cape.String.getLength(val) - 2);
				}
				if(cape.String.isEmpty(key)) {
					continue;
				}
				if(!(object.Equals(tag, null))) {
					key = ((key + "[") + tag) + "]";
				}
				data.add((string)key, (string)val);
			}
			this.file = file;
			return(true);
		}

		public virtual bool write(cape.File outfile) {
			if((outfile == null) || (data == null)) {
				return(false);
			}
			var os = cape.PrintWriterWrapper.forWriter((cape.Writer)outfile.write());
			if(os == null) {
				return(false);
			}
			var it = data.iterate();
			while(it != null) {
				var kvp = it.next();
				if(kvp == null) {
					break;
				}
				os.println((kvp.key + ": ") + kvp.value);
			}
			return(true);
		}
	}
}
