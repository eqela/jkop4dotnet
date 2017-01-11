
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

namespace sympathy.webcontent
{
	public class WikiDirectoryBackend : WikiBackend
	{
		public static WikiDirectoryBackend forDirectory(cape.File dir) {
			var v = new WikiDirectoryBackend();
			v.setDirectory(dir);
			return(v);
		}

		private cape.LoggingContext logContext = null;
		private cape.File directory = null;
		private cape.File themeDirectory = null;
		private System.Collections.Generic.Dictionary<string,WikiDirectoryDocument> cache = null;

		public WikiDirectoryBackend() {
			cache = new System.Collections.Generic.Dictionary<string,WikiDirectoryDocument>();
		}

		public void clearCache() {
			if(directory != null) {
				cape.Log.debug(logContext, ("Clearing the memory cache for directory `" + directory.getPath()) + "'");
			}
			cape.Map.clear(cache);
		}

		private System.Collections.Generic.List<string> processPath(string path) {
			if(object.Equals(path, null)) {
				return(null);
			}
			var v = new System.Collections.Generic.List<string>();
			var comps = cape.String.split(path, '/');
			if(comps != null) {
				var n = 0;
				var m = comps.Count;
				for(n = 0 ; n < m ; n++) {
					var comp = comps[n];
					if(comp != null) {
						if(cape.String.isEmpty(comp)) {
							;
						}
						else if(object.Equals(comp, ".")) {
							;
						}
						else if(object.Equals(comp, "..")) {
							cape.Vector.removeLast(v);
						}
						else {
							v.Add(comp);
						}
					}
				}
			}
			return(v);
		}

		public virtual cape.File getResourceForPath(string path) {
			if((directory == null) || (object.Equals(path, null))) {
				return(null);
			}
			var pp = processPath(path);
			if(cape.Vector.isEmpty(pp)) {
				return(null);
			}
			var ff = directory;
			if(pp != null) {
				var n = 0;
				var m = pp.Count;
				for(n = 0 ; n < m ; n++) {
					var p = pp[n];
					if(p != null) {
						ff = ff.entry(p);
					}
				}
			}
			if(ff.isFile() == false) {
				return(null);
			}
			return(ff);
		}

		public virtual WikiDocument getDocumentForPath(string path) {
			if(object.Equals(path, null)) {
				return(null);
			}
			var pp = processPath(path);
			string pps = null;
			if(cape.Vector.isEmpty(pp)) {
				pps = "/";
			}
			else {
				pps = "/" + cape.String.combine(pp, '/');
			}
			var doc = cape.Map.get(cache, pps);
			if(doc != null) {
				if(doc.isUpToDate()) {
					return((WikiDocument)doc);
				}
				cape.Map.remove(cache, pps);
			}
			if(directory == null) {
				return(null);
			}
			var ff = directory;
			if(pp != null) {
				var n = 0;
				var m = pp.Count;
				for(n = 0 ; n < m ; n++) {
					var p = pp[n];
					if(p != null) {
						ff = ff.entry(p);
					}
				}
			}
			if(ff.isDirectory() == false) {
				return(null);
			}
			var v = sympathy.webcontent.WikiDirectoryDocument.forDirectory(ff, themeDirectory, (WikiBackend)this);
			if(v == null) {
				return(null);
			}
			v.setPath(pps);
			cape.Map.set(cache, pps, v);
			return((WikiDocument)v);
		}

		public cape.LoggingContext getLogContext() {
			return(logContext);
		}

		public WikiDirectoryBackend setLogContext(cape.LoggingContext v) {
			logContext = v;
			return(this);
		}

		public cape.File getDirectory() {
			return(directory);
		}

		public WikiDirectoryBackend setDirectory(cape.File v) {
			directory = v;
			return(this);
		}

		public cape.File getThemeDirectory() {
			return(themeDirectory);
		}

		public WikiDirectoryBackend setThemeDirectory(cape.File v) {
			themeDirectory = v;
			return(this);
		}
	}
}
