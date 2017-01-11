
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
	public class WikiHandler : HTTPServerRequestHandlerAdapter
	{
		private WikiBackend backend = null;
		private cape.DynamicMap siteData = null;
		private WikiTheme theme = null;

		public override bool onGET(HTTPServerRequest req) {
			if(req.isForResource("/style.css")) {
				req.sendResponse(sympathy.HTTPServerResponse.forString(theme.getCss(), "text/css"));
				return(true);
			}
			var rrp = req.getRelativeResourcePath();
			if(cape.String.isEmpty(rrp)) {
				return(false);
			}
			var res = backend.getResourceForPath(rrp);
			if(res != null) {
				req.sendResponse(sympathy.HTTPServerResponse.forFile(res));
				return(true);
			}
			var doc = backend.getDocumentForPath(rrp);
			if(doc != null) {
				if(req.isForDirectory() == false) {
					req.sendRedirectAsDirectory();
					return(true);
				}
				var html = doc.getAsHtml();
				if(object.Equals(html, null)) {
					cape.Log.error(logContext, ("Failed to get document as HTML: `" + rrp) + "'");
					req.sendResponse(sympathy.HTTPServerResponse.forHTTPInternalError());
					return(true);
				}
				var data = new cape.DynamicMap();
				data.set("site", (object)siteData);
				var docConfig = new cape.DynamicMap();
				var tit = doc.getTitle();
				string appTitle = null;
				if(siteData != null) {
					appTitle = siteData.getString("title");
				}
				if((cape.String.isEmpty(tit) == false) && !(object.Equals(tit, appTitle))) {
					data.set("page_title", (object)((tit + " | ") + appTitle));
					docConfig.set("title", (object)tit);
				}
				else {
					docConfig.set("title", (object)appTitle);
				}
				docConfig.set("slogan", (object)doc.getSlogan());
				docConfig.set("intro", (object)doc.getIntro());
				docConfig.set("author", (object)doc.getAuthor());
				docConfig.set("date", (object)doc.getDate());
				docConfig.set("banner", (object)doc.getBannerName());
				data.set("document", (object)docConfig);
				var pageHtml = theme.getArticlePageHtml(html, data);
				req.sendHTMLString(pageHtml);
				return(true);
			}
			return(false);
		}

		public WikiBackend getBackend() {
			return(backend);
		}

		public WikiHandler setBackend(WikiBackend v) {
			backend = v;
			return(this);
		}

		public cape.DynamicMap getSiteData() {
			return(siteData);
		}

		public WikiHandler setSiteData(cape.DynamicMap v) {
			siteData = v;
			return(this);
		}

		public WikiTheme getTheme() {
			return(theme);
		}

		public WikiHandler setTheme(WikiTheme v) {
			theme = v;
			return(this);
		}
	}
}
