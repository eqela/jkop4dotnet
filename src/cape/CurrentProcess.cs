
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
	public class CurrentProcess
	{
		public static void terminate(int value) {
			System.Console.WriteLine("[cape.CurrentProcess.terminate] (CurrentProcess.sling:40:3): Not implemented");
		}

		public static File getExecutableFile() {
			string fn = null;
			var assembly = System.Reflection.Assembly.GetEntryAssembly();
			if(assembly != null) {
				fn = assembly.Location;
			}
			if(fn == null) {
				fn = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
			}
			if(cape.String.isEmpty(fn)) {
				return(null);
			}
			return(cape.FileInstance.forPath(fn));
		}
	}
}
