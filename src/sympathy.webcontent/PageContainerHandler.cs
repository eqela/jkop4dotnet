
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
	public class PageContainerHandler : HTTPServerRequestHandlerAdapter
	{
		private string defaultPage = null;
		private PageContainer pageContainer = null;
		private capex.TemplateStorage templateStorage = null;
		private string country = null;
		private System.Collections.Generic.List<string> languages = null;
		private cape.DynamicMap siteData = null;

		public PageContainerHandler() {
			defaultPage = "index";
		}

		public void setLocale(string country, string[] languages = null) {
			this.country = country;
			this.languages = new System.Collections.Generic.List<string>();
			if(languages == null) {
				if(object.Equals(country, null)) {
					;
				}
				else if(object.Equals(country, "ph")) {
					this.languages.Add("en_PH");
					this.languages.Add("en");
				}
				else if(object.Equals(country, "sg")) {
					this.languages.Add("en_SG");
					this.languages.Add("en");
				}
				else if(object.Equals(country, "us")) {
					this.languages.Add("en_US");
					this.languages.Add("en");
				}
				else if(object.Equals(country, "uk")) {
					this.languages.Add("en_UK");
					this.languages.Add("en");
				}
				else if(object.Equals(country, "au")) {
					this.languages.Add("en_AU");
					this.languages.Add("en");
				}
				else {
					this.languages.Add(country);
				}
			}
			else if(languages != null) {
				var n = 0;
				var m = languages.Length;
				for(n = 0 ; n < m ; n++) {
					var language = languages[n];
					if(language != null) {
						this.languages.Add(language);
					}
				}
			}
		}

		public override void onGET(HTTPServerRequest req, System.Action next) {
			if(pageContainer == null) {
				next();
				return;
			}
			var pageId = req.popResource();
			if(cape.String.isEmpty(pageId)) {
				pageId = defaultPage;
			}
			if(cape.String.isEmpty(pageId)) {
				next();
				return;
			}
			if(req.hasMoreResources()) {
				req.unpopResource();
				next();
				return;
			}
			pageContainer.getPageData(pageId, (string type, cape.DynamicMap data, cape.DynamicVector attachments) => {
				if(data == null) {
					next();
					return;
				}
				var rdata = new cape.DynamicMap();
				rdata.set("country", (object)country);
				rdata.set("type", (object)type);
				rdata.set("content", (object)data);
				rdata.set("attachments", (object)attachments);
				rdata.set("site", (object)siteData);
				if(cape.String.isEmpty(type)) {
					req.sendJSONObject((object)rdata);
					return;
				}
				templateStorage.getTemplate(type, (string template) => {
					if(object.Equals(template, null)) {
						req.sendJSONObject((object)rdata);
						return;
					}
					sendCompleteResponse(req, rdata, template);
				});
			});
		}

		public void sendCompleteResponse(HTTPServerRequest req, cape.DynamicMap data, string tt) {
			var includeDirs = new System.Collections.Generic.List<cape.File>();
			var tsf = templateStorage as capex.TemplateStorageUsingFiles;
			if(tsf != null) {
				includeDirs.Add(tsf.getDirectory());
			}
			var template = capex.TextTemplate.forHTMLString(tt, includeDirs);
			if(template == null) {
				req.sendInternalError("Text template parsing");
				return;
			}
			template.setLanguagePreferences(languages);
			var str = template.execute(data, includeDirs);
			if(object.Equals(str, null)) {
				req.sendInternalError("Text template execution");
				return;
			}
			req.sendHTMLString(str);
		}

		public string getDefaultPage() {
			return(defaultPage);
		}

		public PageContainerHandler setDefaultPage(string v) {
			defaultPage = v;
			return(this);
		}

		public PageContainer getPageContainer() {
			return(pageContainer);
		}

		public PageContainerHandler setPageContainer(PageContainer v) {
			pageContainer = v;
			return(this);
		}

		public capex.TemplateStorage getTemplateStorage() {
			return(templateStorage);
		}

		public PageContainerHandler setTemplateStorage(capex.TemplateStorage v) {
			templateStorage = v;
			return(this);
		}

		public string getCountry() {
			return(country);
		}

		public PageContainerHandler setCountry(string v) {
			country = v;
			return(this);
		}

		public System.Collections.Generic.List<string> getLanguages() {
			return(languages);
		}

		public PageContainerHandler setLanguages(System.Collections.Generic.List<string> v) {
			languages = v;
			return(this);
		}

		public cape.DynamicMap getSiteData() {
			return(siteData);
		}

		public PageContainerHandler setSiteData(cape.DynamicMap v) {
			siteData = v;
			return(this);
		}
	}
}
