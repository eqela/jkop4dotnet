
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
	public class OS
	{
		public static bool isWindows() {
			return(isSystemType("windows"));
		}

		public static bool isLinux() {
			return(isSystemType("linux"));
		}

		public static bool isOSX() {
			return(isSystemType("osx"));
		}

		public static bool isAndroid() {
			return(isSystemType("android"));
		}

		public static bool isIOS() {
			return(isSystemType("ios"));
		}

		public static bool isSystemType(string id) {
			var v = false;
			if(object.Equals(id, "windows")) {
				if(System.Environment.OSVersion.Platform == System.PlatformID.Win32NT ||
				System.Environment.OSVersion.Platform == System.PlatformID.Win32S ||
				System.Environment.OSVersion.Platform == System.PlatformID.Win32Windows ||
				System.Environment.OSVersion.Platform == System.PlatformID.WinCE ||
				System.Environment.OSVersion.Platform == System.PlatformID.Xbox) {
					v = true;
				}
				return(v);
			}
			if(object.Equals(id, "osx")) {
				if(System.Environment.OSVersion.Platform == System.PlatformID.MacOSX) {
					v = true;
				}
				if(v) {
					return(v);
				}
				if(isSystemType("posix") == false) {
					return(false);
				}
				if(cape.FileInstance.forPath("/Applications").isDirectory()) {
					return(true);
				}
				return(false);
			}
			if(((object.Equals(id, "posix")) || (object.Equals(id, "linux"))) || (object.Equals(id, "unix"))) {
				if(System.Environment.OSVersion.Platform == System.PlatformID.Unix) {
					v = true;
				}
				return(v);
			}
			return(false);
		}
	}
}
