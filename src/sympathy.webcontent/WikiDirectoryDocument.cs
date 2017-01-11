
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
	public class WikiDirectoryDocument : WikiDocument
	{
		private class MyReferenceResolver : capex.RichTextDocumentReferenceResolver
		{
			private WikiBackend backend = null;
			private string path = null;
			private cape.File themeDirectory = null;

			private string toAbsoluteRef(string refid) {
				if(object.Equals(refid, null)) {
					return(null);
				}
				if(cape.String.startsWith(refid, "/")) {
					return(refid);
				}
				var sb = new cape.StringBuilder();
				sb.append(path);
				if(!(object.Equals(path, null)) && (cape.String.endsWith(path, "/") == false)) {
					sb.append('/');
				}
				sb.append(refid);
				sb.append('/');
				return(sb.toString());
			}

			public virtual string getReferenceHref(string refid) {
				return(toAbsoluteRef(refid));
			}

			public virtual string getReferenceTitle(string arefid) {
				var refid = toAbsoluteRef(arefid);
				if(backend == null) {
					return(refid);
				}
				var doc = backend.getDocumentForPath(refid);
				if(doc == null) {
					return(refid);
				}
				var tit = doc.getTitle();
				if(cape.String.isEmpty(tit)) {
					return(refid);
				}
				return(tit);
			}

			public virtual string getContentString(string acid) {
				var cid = acid;
				if((themeDirectory == null) || (object.Equals(cid, null))) {
					return(null);
				}
				if(cape.String.indexOf(cid, '/') >= 0) {
					var sb = new cape.StringBuilder();
					var it = cape.String.iterate(cid);
					var c = ' ';
					while((c = it.getNextChar()) > 0) {
						if(c != '/') {
							sb.append(c);
						}
					}
					cid = sb.toString();
				}
				return(themeDirectory.entry("content").entry(cid + ".html").getContentsString("UTF-8"));
			}

			public WikiBackend getBackend() {
				return(backend);
			}

			public MyReferenceResolver setBackend(WikiBackend v) {
				backend = v;
				return(this);
			}

			public string getPath() {
				return(path);
			}

			public MyReferenceResolver setPath(string v) {
				path = v;
				return(this);
			}

			public cape.File getThemeDirectory() {
				return(themeDirectory);
			}

			public MyReferenceResolver setThemeDirectory(cape.File v) {
				themeDirectory = v;
				return(this);
			}
		}

		public static WikiDirectoryDocument forDirectory(cape.File dir, cape.File themedir, WikiBackend backend) {
			var v = new WikiDirectoryDocument();
			v.setBackend(backend);
			v.setThemeDirectory(themedir);
			if(v.initialize(dir) == false) {
				v = null;
			}
			return(v);
		}

		private WikiBackend backend = null;
		private cape.File themeDirectory = null;
		private string path = null;
		private bool cacheHtml = true;
		private cape.File dir = null;
		private cape.File markupFile = null;
		private string markup = null;
		private capex.RichTextDocument doc = null;
		private long timeStamp = (long)0;
		private string cachedHtmlContent = null;

		public void processDocument(capex.RichTextDocument doc) {
			var pars = doc.getParagraphs();
			if(pars == null) {
				return;
			}
			var p0 = cape.Vector.get(pars, 0) as capex.RichTextStyledParagraph;
			if((p0 == null) || (p0.getHeading() != 1)) {
				return;
			}
			var tc = p0.getTextContent();
			if(cape.String.isEmpty(tc)) {
				return;
			}
			if(!(object.Equals(tc, doc.getTitle()))) {
				return;
			}
			cape.Vector.removeFirst(pars);
		}

		public bool initialize(cape.File dir) {
			if(dir == null) {
				return(false);
			}
			var ff = dir.entry("content.markup");
			var st = ff.stat();
			if(st == null) {
				return(false);
			}
			var str = ff.getContentsString("UTF-8");
			if(object.Equals(str, null)) {
				return(false);
			}
			var doc = capex.RichTextDocument.forWikiMarkupString(str);
			if(doc == null) {
				return(false);
			}
			processDocument(doc);
			this.dir = dir;
			this.markupFile = ff;
			this.markup = str;
			this.doc = doc;
			this.timeStamp = (long)st.getModifyTime();
			return(true);
		}

		public bool isUpToDate() {
			if(markupFile == null) {
				return(false);
			}
			var st = markupFile.stat();
			if(st == null) {
				return(false);
			}
			if(st.getModifyTime() <= timeStamp) {
				return(true);
			}
			return(false);
		}

		public virtual string getTitle() {
			if(doc == null) {
				return(null);
			}
			return(doc.getTitle());
		}

		public virtual string getAuthor() {
			if(doc == null) {
				return(null);
			}
			return(doc.getMetadata("author"));
		}

		public virtual string getDate() {
			if(doc == null) {
				return(null);
			}
			return(doc.getMetadata("date"));
		}

		public virtual string getSlogan() {
			if(doc == null) {
				return(null);
			}
			return(doc.getMetadata("slogan"));
		}

		public virtual string getIntro() {
			if(doc == null) {
				return(null);
			}
			return(doc.getMetadata("intro"));
		}

		public virtual string getBannerName() {
			if(doc == null) {
				return(null);
			}
			return(doc.getMetadata("banner"));
		}

		public virtual string getAsMarkup() {
			return(markup);
		}

		public virtual string getAsHtml() {
			if(!(object.Equals(cachedHtmlContent, null))) {
				return(cachedHtmlContent);
			}
			if(doc == null) {
				return(null);
			}
			var html = doc.toHtml((capex.RichTextDocumentReferenceResolver)new MyReferenceResolver().setPath(path).setThemeDirectory(themeDirectory).setBackend(backend));
			if(cacheHtml) {
				cachedHtmlContent = html;
			}
			return(html);
		}

		public virtual System.Collections.Generic.List<string> getAttachmentHeaders() {
			return(null);
		}

		public virtual cape.Reader getAttachment(string name) {
			return(null);
		}

		public WikiBackend getBackend() {
			return(backend);
		}

		public WikiDirectoryDocument setBackend(WikiBackend v) {
			backend = v;
			return(this);
		}

		public cape.File getThemeDirectory() {
			return(themeDirectory);
		}

		public WikiDirectoryDocument setThemeDirectory(cape.File v) {
			themeDirectory = v;
			return(this);
		}

		public string getPath() {
			return(path);
		}

		public WikiDirectoryDocument setPath(string v) {
			path = v;
			return(this);
		}

		public bool getCacheHtml() {
			return(cacheHtml);
		}

		public WikiDirectoryDocument setCacheHtml(bool v) {
			cacheHtml = v;
			return(this);
		}
	}
}
