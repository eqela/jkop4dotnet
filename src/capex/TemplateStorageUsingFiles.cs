
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
	public class TemplateStorageUsingFiles : TemplateStorage
	{
		public static TemplateStorageUsingFiles forDirectory(cape.File dir) {
			var v = new TemplateStorageUsingFiles();
			v.setDirectory(dir);
			return(v);
		}

		public static TemplateStorageUsingFiles forHTMLTemplateDirectory(cape.File dir) {
			var v = new TemplateStorageUsingFiles();
			v.setDirectory(dir);
			v.setSuffix(".html.t");
			return(v);
		}

		private cape.File directory = null;
		private string suffix = null;

		public TemplateStorageUsingFiles() {
			suffix = ".txt";
		}

		public virtual void getTemplate(string id, System.Action<string> callback) {
			if(callback == null) {
				return;
			}
			if((((directory == null) || cape.String.isEmpty(id)) || (cape.String.indexOf(id, '/') >= 0)) || (cape.String.indexOf(id, '\\') >= 0)) {
				callback(null);
				return;
			}
			var ff = directory.entry(id + suffix);
			if(ff.isFile() == false) {
				callback(null);
				return;
			}
			callback(ff.getContentsString("UTF-8"));
		}

		public cape.File getDirectory() {
			return(directory);
		}

		public TemplateStorageUsingFiles setDirectory(cape.File v) {
			directory = v;
			return(this);
		}

		public string getSuffix() {
			return(suffix);
		}

		public TemplateStorageUsingFiles setSuffix(string v) {
			suffix = v;
			return(this);
		}
	}
}
