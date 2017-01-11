
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
	public class WikiTheme
	{
		public static WikiTheme forDirectory(cape.File dir) {
			if(dir == null) {
				return(null);
			}
			if(dir.isDirectory() == false) {
				return(null);
			}
			var includes = new System.Collections.Generic.List<cape.File>();
			includes.Add(dir);
			var v = new WikiTheme();
			v.setDirectory(dir);
			v.setCss(readFile(dir, "style.css"));
			v.setDocumentHeader(capex.TextTemplate.forHTMLString(readFile(dir, "document_header.html"), includes));
			v.setFrameHeader(capex.TextTemplate.forHTMLString(readFile(dir, "frame_header.html"), includes));
			v.setFrameFooter(capex.TextTemplate.forHTMLString(readFile(dir, "frame_footer.html"), includes));
			v.setArticleHeader(capex.TextTemplate.forHTMLString(readFile(dir, "article_header.html"), includes));
			v.setArticleFooter(capex.TextTemplate.forHTMLString(readFile(dir, "article_footer.html"), includes));
			v.setIncludes(includes);
			return(v);
		}

		private static string readFile(cape.File dir, string fileName) {
			var ff = dir.entry(fileName);
			if(ff.isFile() == false) {
				return(null);
			}
			var v = ff.getContentsString("UTF-8");
			if(object.Equals(v, null)) {
				return(null);
			}
			return(v);
		}

		private string css = null;
		private cape.File directory = null;
		private capex.TextTemplate documentHeader = null;
		private capex.TextTemplate frameHeader = null;
		private capex.TextTemplate frameFooter = null;
		private capex.TextTemplate articleHeader = null;
		private capex.TextTemplate articleFooter = null;
		private System.Collections.Generic.List<cape.File> includes = null;

		public string getPageHtml(string contentHtml, cape.DynamicMap data) {
			var sb = new cape.StringBuilder();
			sb.append("<!DOCTYPE html>\n");
			sb.append("<html>\n");
			sb.append("<head>\n");
			sb.append("<link rel=\"stylesheet\" type=\"text/css\" href=\"/style.css\" />\n");
			sb.append("<link rel=\"icon\" type=\"image/png\" href=\"/res/favicon.png\" />\n");
			sb.append("<meta name=\"viewport\" content=\"initial-scale=1,maximum-scale=1\" />\n");
			string intro = null;
			if(data != null) {
				intro = capex.HTMLString.toQuotedString(data.getString("intro"));
			}
			sb.append(("<meta property=\"og:description\" content=" + intro) + " />\n");
			if(documentHeader != null) {
				sb.append(documentHeader.execute(data, includes));
			}
			sb.append("</head>\n");
			sb.append("<body>\n");
			sb.append("<div id=\"padget_container\">\n");
			sb.append("<div id=\"padget_header\">\n");
			if(frameHeader != null) {
				sb.append(frameHeader.execute(data, includes));
			}
			sb.append("</div>");
			sb.append("<div id=\"padget_content\">\n");
			sb.append(contentHtml);
			sb.append("</div>");
			if(frameFooter != null) {
				sb.append(frameFooter.execute(data, includes));
			}
			sb.append("</body>\n");
			sb.append("</div>");
			sb.append("</html>\n");
			return(sb.toString());
		}

		public string getArticlePageHtml(string contentHtml, cape.DynamicMap data) {
			var sb = new cape.StringBuilder();
			if(articleHeader != null) {
				sb.append(articleHeader.execute(data, includes));
			}
			sb.append("<div class=\"wikidocument\">\n");
			sb.append(contentHtml);
			sb.append("</div>");
			if(articleFooter != null) {
				sb.append(articleFooter.execute(data, includes));
			}
			return(getPageHtml(sb.toString(), data));
		}

		public string getNotFoundHtml() {
			var errorHtml = "<div class=\"wikidocument\">\n<h1>Document not found</h1><p>No such document.</p><p><a href=\"/\">Return to main page</a></p>\n</div>\n";
			return(getPageHtml(errorHtml, null));
		}

		public string getCss() {
			return(css);
		}

		public WikiTheme setCss(string v) {
			css = v;
			return(this);
		}

		public cape.File getDirectory() {
			return(directory);
		}

		public WikiTheme setDirectory(cape.File v) {
			directory = v;
			return(this);
		}

		public capex.TextTemplate getDocumentHeader() {
			return(documentHeader);
		}

		public WikiTheme setDocumentHeader(capex.TextTemplate v) {
			documentHeader = v;
			return(this);
		}

		public capex.TextTemplate getFrameHeader() {
			return(frameHeader);
		}

		public WikiTheme setFrameHeader(capex.TextTemplate v) {
			frameHeader = v;
			return(this);
		}

		public capex.TextTemplate getFrameFooter() {
			return(frameFooter);
		}

		public WikiTheme setFrameFooter(capex.TextTemplate v) {
			frameFooter = v;
			return(this);
		}

		public capex.TextTemplate getArticleHeader() {
			return(articleHeader);
		}

		public WikiTheme setArticleHeader(capex.TextTemplate v) {
			articleHeader = v;
			return(this);
		}

		public capex.TextTemplate getArticleFooter() {
			return(articleFooter);
		}

		public WikiTheme setArticleFooter(capex.TextTemplate v) {
			articleFooter = v;
			return(this);
		}

		public System.Collections.Generic.List<cape.File> getIncludes() {
			return(includes);
		}

		public WikiTheme setIncludes(System.Collections.Generic.List<cape.File> v) {
			includes = v;
			return(this);
		}
	}
}
