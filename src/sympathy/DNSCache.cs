
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
	public class DNSCache
	{
		private class DNSCacheEntry
		{
			private string ip = null;
			private int timestamp = 0;

			public static DNSCacheEntry create(string ip) {
				var v = new DNSCacheEntry();
				v.setIp(ip);
				v.setTimestamp((int)cape.SystemClock.asSeconds());
				return(v);
			}

			public string getIp() {
				return(ip);
			}

			public DNSCacheEntry setIp(string v) {
				ip = v;
				return(this);
			}

			public int getTimestamp() {
				return(timestamp);
			}

			public DNSCacheEntry setTimestamp(int v) {
				timestamp = v;
				return(this);
			}
		}

		private class DNSCacheImpl
		{
			private cape.DynamicMap entries = null;
			private cape.Mutex mutex = null;

			public DNSCacheImpl() {
				entries = new cape.DynamicMap();
				mutex = cape.Mutex.create();
			}

			private void add(string hostname, string ip) {
				if(mutex != null) {
					mutex.lockMutex();
				}
				entries.set(hostname, (object)sympathy.DNSCache.DNSCacheEntry.create(ip));
				if(mutex != null) {
					mutex.unlockMutex();
				}
			}

			private string getCachedEntry(string hostname) {
				DNSCacheEntry v = null;
				if(mutex != null) {
					mutex.lockMutex();
				}
				v = entries.get(hostname) as DNSCacheEntry;
				if(mutex != null) {
					mutex.unlockMutex();
				}
				if(v != null) {
					if((cape.SystemClock.asSeconds() - v.getTimestamp()) > (60 * 60)) {
						if(mutex != null) {
							mutex.lockMutex();
						}
						entries.remove(hostname);
						if(mutex != null) {
							mutex.unlockMutex();
						}
						v = null;
					}
				}
				if(v != null) {
					return(v.getIp());
				}
				return(null);
			}

			public string resolve(string hostname) {
				var v = getCachedEntry(hostname);
				if(!(object.Equals(v, null))) {
					return(v);
				}
				var dr = sympathy.DNSResolver.create();
				if(dr == null) {
					return(null);
				}
				v = dr.getIPAddress(hostname, null);
				if(!(object.Equals(v, null))) {
					add(hostname, v);
				}
				return(v);
			}
		}

		private static DNSCacheImpl cc = null;

		public static string resolve(string hostname) {
			if(cc == null) {
				cc = new DNSCacheImpl();
			}
			return(cc.resolve(hostname));
		}
	}
}
