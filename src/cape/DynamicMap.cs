
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
	public class DynamicMap : Duplicateable, Iterateable<string>
	{
		public static DynamicMap asDynamicMap(object @object) {
			if(@object == null) {
				return(null);
			}
			if(@object is DynamicMap) {
				return((DynamicMap)@object);
			}
			if(@object is System.Collections.Generic.Dictionary<object,object>) {
				return(forObjectMap((System.Collections.Generic.Dictionary<object,object>)@object));
			}
			return(null);
		}

		public static DynamicMap forObjectMap(System.Collections.Generic.Dictionary<object,object> map) {
			var v = new DynamicMap();
			if(map != null) {
				Iterator<object> it = cape.Map.iterateKeys(map);
				while(it != null) {
					var key = it.next();
					if(key == null) {
						break;
					}
					if((key is string) == false) {
						continue;
					}
					v.set((string)key, cape.Map.getValue(map, (string)key));
				}
			}
			return(v);
		}

		public static DynamicMap forStringMap(System.Collections.Generic.Dictionary<string,string> map) {
			var v = new DynamicMap();
			if(map != null) {
				Iterator<string> it = cape.Map.iterateKeys(map);
				while(it != null) {
					var key = it.next();
					if(object.Equals(key, null)) {
						break;
					}
					v.set(key, (object)cape.Map.getValue(map, key));
				}
			}
			return(v);
		}

		private System.Collections.Generic.Dictionary<string,object> map = null;

		public DynamicMap() {
			map = new System.Collections.Generic.Dictionary<string,object>();
		}

		public virtual object duplicate() {
			return((object)duplicateMap());
		}

		public DynamicMap duplicateMap() {
			var v = new DynamicMap();
			var it = iterateKeys();
			while(it != null) {
				var key = it.next();
				if(object.Equals(key, null)) {
					break;
				}
				v.set(key, get(key));
			}
			return(v);
		}

		public DynamicMap mergeFrom(DynamicMap other) {
			if(other == null) {
				return(this);
			}
			var it = other.iterateKeys();
			while(it != null) {
				var key = it.next();
				if(object.Equals(key, null)) {
					break;
				}
				set(key, other.get(key));
			}
			return(this);
		}

		public DynamicMap set(string key, object value) {
			if(!(object.Equals(key, null))) {
				map[key] = value;
			}
			return(this);
		}

		public DynamicMap set(string key, byte[] value) {
			return(set(key, (object)cape.Buffer.asObject(value)));
		}

		public DynamicMap set(string key, int value) {
			return(set(key, (object)cape.Integer.asObject(value)));
		}

		public DynamicMap set(string key, bool value) {
			return(set(key, (object)cape.Boolean.asObject(value)));
		}

		public DynamicMap set(string key, double value) {
			return(set(key, (object)cape.Double.asObject(value)));
		}

		public object get(string key) {
			return(cape.Map.get(map, key, null));
		}

		public string getString(string key, string defval = null) {
			var v = cape.String.asString(get(key));
			if(object.Equals(v, null)) {
				return(defval);
			}
			return(v);
		}

		public int getInteger(string key, int defval = 0) {
			var vv = get(key);
			if(vv == null) {
				return(defval);
			}
			return(cape.Integer.asInteger(vv));
		}

		public bool getBoolean(string key, bool defval = false) {
			var vv = get(key);
			if(vv == null) {
				return(defval);
			}
			return(cape.Boolean.asBoolean(vv));
		}

		public double getDouble(string key, double defval = 0.00) {
			var vv = get(key);
			if(vv == null) {
				return(defval);
			}
			return(cape.Double.asDouble(vv));
		}

		public byte[] getBuffer(string key) {
			var vv = get(key);
			if(vv == null) {
				return(null);
			}
			return(cape.Buffer.asBuffer(vv));
		}

		public DynamicVector getDynamicVector(string key) {
			var vv = get(key) as DynamicVector;
			if(vv != null) {
				return(vv);
			}
			var v = getVector(key);
			if(v != null) {
				return(cape.DynamicVector.forObjectVector(v));
			}
			return(null);
		}

		public System.Collections.Generic.List<object> getVector(string key) {
			var val = get(key);
			if(val == null) {
				return(null);
			}
			if(val is System.Collections.Generic.List<object>) {
				return((System.Collections.Generic.List<object>)val);
			}
			if(val is VectorObject<object>) {
				var vo = (VectorObject<object>)val;
				System.Collections.Generic.List<object> vv = vo.toVector();
				return(vv);
			}
			return(null);
		}

		public DynamicMap getDynamicMap(string key) {
			return(get(key) as DynamicMap);
		}

		public System.Collections.Generic.List<string> getKeys() {
			var v = new System.Collections.Generic.List<string>();
			var it = iterateKeys();
			while(true) {
				var kk = it.next();
				if(object.Equals(kk, null)) {
					break;
				}
				v.Add(kk);
			}
			return(v);
		}

		public virtual Iterator<string> iterate() {
			Iterator<string> v = cape.Map.iterateKeys(map);
			return(v);
		}

		public Iterator<string> iterateKeys() {
			Iterator<string> v = cape.Map.iterateKeys(map);
			return(v);
		}

		public Iterator<object> iterateValues() {
			Iterator<object> v = cape.Map.iterateValues(map);
			return(v);
		}

		public void remove(string key) {
			cape.Map.remove(map, key);
		}

		public void clear() {
			cape.Map.clear(map);
		}

		public int getCount() {
			return(cape.Map.count(map));
		}

		public bool containsKey(string key) {
			return(cape.Map.containsKey(map, key));
		}
	}
}
