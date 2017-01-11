
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

namespace sympathy
{
	public class ContentCache
	{
		private class CacheEntry
		{
			private object data = null;
			private int ttl = 0;
			private int timestamp = 0;

			public object getData() {
				return(data);
			}

			public CacheEntry setData(object v) {
				data = v;
				return(this);
			}

			public int getTtl() {
				return(ttl);
			}

			public CacheEntry setTtl(int v) {
				ttl = v;
				return(this);
			}

			public int getTimestamp() {
				return(timestamp);
			}

			public CacheEntry setTimestamp(int v) {
				timestamp = v;
				return(this);
			}
		}

		private System.Collections.Generic.Dictionary<string,CacheEntry> cache = null;
		private int cacheTtl = 3600;

		public void onMaintenance() {
			if(cache == null) {
				return;
			}
			var now = cape.SystemClock.asSeconds();
			System.Collections.Generic.List<string> keys = cape.Map.getKeys(cache);
			if(keys != null) {
				var n = 0;
				var m = keys.Count;
				for(n = 0 ; n < m ; n++) {
					var key = keys[n];
					if(key != null) {
						var ce = cape.Map.get(cache, key) as CacheEntry;
						if(ce == null) {
							cape.Map.remove(cache, key);
						}
						else {
							var diff = now - ce.getTimestamp();
							if(diff >= ce.getTtl()) {
								cape.Map.remove(cache, key);
							}
						}
					}
				}
			}
		}

		public void clear() {
			cache = null;
		}

		public void remove(string cacheid) {
			cape.Map.remove(cache, cacheid);
		}

		public void set(string cacheid, object content, int ttl = -1) {
			if(object.Equals(cacheid, null)) {
				return;
			}
			var ee = new CacheEntry();
			ee.setData(content);
			if(ttl >= 0) {
				ee.setTtl(ttl);
			}
			else {
				ee.setTtl(cacheTtl);
			}
			if(ee.getTtl() < 1) {
				return;
			}
			ee.setTimestamp((int)cape.SystemClock.asSeconds());
			if(cache == null) {
				cache = new System.Collections.Generic.Dictionary<string,CacheEntry>();
			}
			cache[cacheid] = ee;
		}

		public object get(string cacheid) {
			if(cache == null) {
				return(null);
			}
			var ee = cape.Map.getValue(cache, cacheid) as CacheEntry;
			if(ee != null) {
				var diff = cape.SystemClock.asSeconds() - ee.getTimestamp();
				if(diff >= ee.getTtl()) {
					cape.Map.remove(cache, cacheid);
					ee = null;
				}
			}
			if(ee != null) {
				return(ee.getData());
			}
			return(null);
		}

		public int getCacheTtl() {
			return(cacheTtl);
		}

		public ContentCache setCacheTtl(int v) {
			cacheTtl = v;
			return(this);
		}
	}
}
