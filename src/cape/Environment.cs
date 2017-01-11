
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
	public class Environment
	{
		public static char getPathSeparator() {
			return(System.IO.Path.DirectorySeparatorChar);
		}

		public static bool isAbsolutePath(string path) {
			if(object.Equals(path, null)) {
				return(false);
			}
			var sep = getPathSeparator();
			var c0 = cape.String.getChar(path, 0);
			if(c0 == sep) {
				return(true);
			}
			if(((cape.Character.isAlpha(c0) && cape.OS.isWindows()) && (cape.String.getChar(path, 1) == ':')) && (cape.String.getChar(path, 2) == '\\')) {
				return(true);
			}
			return(false);
		}

		public static System.Collections.Generic.Dictionary<string,string> getVariables() {
			System.Console.WriteLine("[cape.Environment.getVariables] (Environment.sling:60:1): Not implemented");
			return(null);
		}

		public static string getVariable(string key) {
			if(object.Equals(key, null)) {
				return(null);
			}
			string v = null;
			v = System.Environment.GetEnvironmentVariable(key);
			return(v);
			var vars = getVariables();
			if(vars == null) {
				return(null);
			}
			return(cape.Map.get(vars, key));
		}

		public static void setVariable(string key, string val) {
			System.Console.WriteLine("[cape.Environment.setVariable] (Environment.sling:94:1): Not implemented");
		}

		public static void unsetVariable(string key) {
			System.Console.WriteLine("[cape.Environment.unsetVariable] (Environment.sling:99:1): Not implemented");
		}

		public static void setCurrentDirectory(File dir) {
			System.Console.WriteLine("[cape.Environment.setCurrentDirectory] (Environment.sling:104:1): Not implemented");
		}

		public static File getCurrentDirectory() {
			return(cape.FileInstance.forPath(System.IO.Directory.GetCurrentDirectory()));
		}

		private static File findInPath(string command) {
			var path = getVariable("PATH");
			if(cape.String.isEmpty(path)) {
				return(null);
			}
			var separator = ':';
			if(cape.OS.isWindows()) {
				separator = ';';
			}
			var array = cape.String.split(path, separator);
			if(array != null) {
				var n = 0;
				var m = array.Count;
				for(n = 0 ; n < m ; n++) {
					var dir = array[n];
					if(dir != null) {
						var pp = cape.FileInstance.forPath(dir).entry(command).asExecutable();
						if(pp.isFile()) {
							return(pp);
						}
					}
				}
			}
			return(null);
		}

		public static File findCommand(string command) {
			if(object.Equals(command, null)) {
				return(null);
			}
			return(findInPath(command));
		}

		public static File getTemporaryDirectory() {
			return(cape.FileInstance.forPath(System.IO.Path.GetTempPath()));
		}

		public static File getHomeDirectory() {
			string v = null;
			var hd = System.Environment.GetEnvironmentVariable("HOMEDRIVE");
			if(hd != null) {
				var hp = System.Environment.GetEnvironmentVariable("HOMEPATH");
				if(hp != null) {
					v = hd + System.IO.Path.DirectorySeparatorChar + hp;
				}
			}
			if(object.Equals(v, null)) {
				var h = System.Environment.GetEnvironmentVariable("HOME");
				if(h != null) {
					v = h;
				}
			}
			if(object.Equals(v, null)) {
				v = System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile);
			}
			return(cape.FileInstance.forPath(v));
		}

		public static File getAppDirectory() {
			return(cape.FileInstance.forPath(System.AppDomain.CurrentDomain.BaseDirectory));
		}
	}
}
