
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
	public class PageContainerUsingFiles : PageContainer
	{
		public static PageContainerUsingFiles forDirectory(cape.File dir) {
			var v = new PageContainerUsingFiles();
			v.setDirectory(dir);
			return(v);
		}

		private cape.File directory = null;

		public virtual void getAllPageIds(System.Action<cape.DynamicVector> callback) {
			if(callback == null) {
				return;
			}
			var v = new cape.DynamicVector();
			if(directory != null) {
				var it = directory.entries();
				while(it != null) {
					var file = it.next();
					if(file == null) {
						break;
					}
					if(file.isDirectory()) {
						v.append((object)file);
					}
				}
			}
			callback(v);
		}

		public virtual void getPageData(string pageId, System.Action<string, cape.DynamicMap, cape.DynamicVector> callback) {
			if(callback == null) {
				return;
			}
			if(((((directory == null) || cape.String.isEmpty(pageId)) || (object.Equals(pageId, ".."))) || (object.Equals(pageId, "."))) || (cape.String.indexOf(pageId, '/') >= 0)) {
				callback(null, null, null);
				return;
			}
			var cdir = directory.entry(pageId);
			if(cdir.isDirectory() == false) {
				callback(null, null, null);
				return;
			}
			var type = cape.String.strip(cdir.entry("type.txt").getContentsString("UTF-8"));
			var dataString = cdir.entry("content.json").getContentsString("UTF-8");
			if(object.Equals(dataString, null)) {
				callback(null, null, null);
				return;
			}
			var data = cape.JSONParser.parse(dataString) as cape.DynamicMap;
			if(data == null) {
				callback(null, null, null);
				return;
			}
			callback(type, data, null);
		}

		public virtual void createPage(string pageId, string type, System.Action<bool> callback) {
			System.Console.WriteLine("--- stub --- sympathy.webcontent.PageContainerUsingFiles :: createPage");
		}

		public virtual void changePageType(string pageId, string type, System.Action<bool> callback) {
			System.Console.WriteLine("--- stub --- sympathy.webcontent.PageContainerUsingFiles :: changePageType");
		}

		public virtual void deletePage(string pageId, System.Action<bool> callback) {
			System.Console.WriteLine("--- stub --- sympathy.webcontent.PageContainerUsingFiles :: deletePage");
		}

		public virtual void updatePageContent(string pageId, cape.DynamicMap content, System.Action<bool> callback) {
			System.Console.WriteLine("--- stub --- sympathy.webcontent.PageContainerUsingFiles :: updatePageContent");
		}

		public virtual void addAttachment(string pageId, string fileName, byte[] content, System.Action<bool> callback) {
			System.Console.WriteLine("--- stub --- sympathy.webcontent.PageContainerUsingFiles :: addAttachment");
		}

		public virtual void replaceAttachment(string pageId, string fileName, byte[] content, System.Action<bool> callback) {
			System.Console.WriteLine("--- stub --- sympathy.webcontent.PageContainerUsingFiles :: replaceAttachment");
		}

		public virtual void getAttachment(string pageId, string fileName, System.Action<byte[]> callback) {
			System.Console.WriteLine("--- stub --- sympathy.webcontent.PageContainerUsingFiles :: getAttachment");
		}

		public virtual void deleteAttachment(string pageId, string fileName, System.Action<bool> callback) {
			System.Console.WriteLine("--- stub --- sympathy.webcontent.PageContainerUsingFiles :: deleteAttachment");
		}

		public virtual void deleteAllAttachments(string pageId, System.Action<bool> callback) {
			System.Console.WriteLine("--- stub --- sympathy.webcontent.PageContainerUsingFiles :: deleteAllAttachments");
		}

		public cape.File getDirectory() {
			return(directory);
		}

		public PageContainerUsingFiles setDirectory(cape.File v) {
			directory = v;
			return(this);
		}
	}
}
