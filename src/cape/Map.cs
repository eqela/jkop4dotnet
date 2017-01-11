
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
	public class Map
	{
		private class MyMapObject<K, V> : MapObject<K, V>
		{
			private System.Collections.Generic.Dictionary<K,V> map = null;

			public virtual System.Collections.Generic.Dictionary<K,V> toMap() {
				return(map);
			}

			public System.Collections.Generic.Dictionary<K,V> getMap() {
				return(map);
			}

			public MyMapObject<K, V> setMap(System.Collections.Generic.Dictionary<K,V> v) {
				map = v;
				return(this);
			}
		}

		public static MapObject<K, V> asObject<K, V>(System.Collections.Generic.Dictionary<K,V> map) {
			if(map == null) {
				return(null);
			}
			var v = new MyMapObject<K, V>();
			v.setMap(map);
			return((MapObject<K, V>)v);
		}

		public static V get<K, V>(System.Collections.Generic.Dictionary<K,V> map, K key, V ddf) {
			if((map == null) || (key == null)) {
				return(ddf);
			}
			if(containsKey(map, key) == false) {
				return(ddf);
			}
			return(getValue(map, key));
		}

		public static V get<K, V>(System.Collections.Generic.Dictionary<K,V> map, K key) {
			return(getValue(map, key));
		}

		public static V getValue<K, V>(System.Collections.Generic.Dictionary<K,V> map, K key) {
			if((map == null) || (key == null)) {
				return((V)(default(V)));
			}
			var v = (V)(default(V));
			try {
				v = map[key];
			}
			catch {
				v = default(V);
			}
			return(v);
		}

		public static bool set<K, V>(System.Collections.Generic.Dictionary<K,V> data, K key, V val) {
			if((data == null) || (key == null)) {
				return(false);
			}
			data[key] = val;
			return(true);
		}

		public static bool setValue<K, V>(System.Collections.Generic.Dictionary<K,V> data, K key, V val) {
			return(set(data, key, val));
		}

		public static void remove<K, V>(System.Collections.Generic.Dictionary<K,V> data, K key) {
			if((data == null) || (key == null)) {
				return;
			}
			data.Remove(key);
		}

		public static int count<K, V>(System.Collections.Generic.Dictionary<K,V> data) {
			if(data == null) {
				return(0);
			}
			return(data.Count);
		}

		public static bool containsKey<K, V>(System.Collections.Generic.Dictionary<K,V> data, K key) {
			if((data == null) || (key == null)) {
				return(false);
			}
			return(data.ContainsKey(key));
		}

		public static bool containsValue<K, V>(System.Collections.Generic.Dictionary<K,V> data, V val) {
			if((data == null) || (val == null)) {
				return(false);
			}
			return(data.ContainsValue(val));
		}

		public static void clear<K, V>(System.Collections.Generic.Dictionary<K,V> data) {
			if(data == null) {
				return;
			}
			data.Clear();
		}

		public static System.Collections.Generic.Dictionary<K,V> dup<K, V>(System.Collections.Generic.Dictionary<K,V> data) {
			if(data == null) {
				return(null);
			}
			return(new System.Collections.Generic.Dictionary<K, V>(data));
		}

		public static System.Collections.Generic.List<K> getKeys<K, V>(System.Collections.Generic.Dictionary<K,V> data) {
			if(data == null) {
				return(null);
			}
			var v = new System.Collections.Generic.List<K>();
			foreach(K key in data.Keys) {
				v.Add(key);
			}
			return(v);
		}

		public static System.Collections.Generic.List<V> getValues<K, V>(System.Collections.Generic.Dictionary<K,V> data) {
			if(data == null) {
				return(null);
			}
			var v = new System.Collections.Generic.List<V>();
			foreach(V value in data.Values) {
				v.Add(value);
			}
			return(v);
		}

		public static Iterator<K> iterateKeys<K, V>(System.Collections.Generic.Dictionary<K,V> data) {
			return(cape.Vector.iterate(getKeys(data)));
		}

		public static Iterator<V> iterateValues<K, V>(System.Collections.Generic.Dictionary<K,V> data) {
			return(cape.Vector.iterate(getValues(data)));
		}
	}
}
