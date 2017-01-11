
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
	public class ArticleWebHandler : HTTPServerRequestHandlerAdapter
	{
		public static ArticleWebHandler forDatabase(capex.SQLDatabase database, string articleTable) {
			var v = new ArticleWebHandler();
			v.setDatabase(database);
			v.setArticleTable(articleTable);
			return(v);
		}

		private capex.SQLDatabase database = null;
		private string articleTable = null;
		private int articlesPerPage = 10;
		private capex.TemplateStorage templateStorage = null;
		private cape.DynamicMap siteData = null;
		private string templatePrefix = null;
		private string country = null;
		private System.Collections.Generic.List<string> languages = null;

		public ArticleWebHandler() {
			articleTable = "articles";
			templatePrefix = "";
		}

		public override void initialize(HTTPServerBase server) {
			base.initialize(server);
			if((database != null) || !(object.Equals(articleTable, null))) {
				database.ensureTableExists(sympathy.webcontent.Article.SQL.getTableInfo(articleTable));
			}
		}

		public bool isResponseHTML(HTTPServerRequest req) {
			if(templateStorage == null) {
				return(false);
			}
			if(object.Equals(req.getQueryParameter("format"), "json")) {
				return(false);
			}
			return(true);
		}

		private System.Collections.Generic.List<object> vectorForIterator(cape.DynamicIterator iterator) {
			if(iterator == null) {
				return(null);
			}
			var v = new System.Collections.Generic.List<object>();
			while(true) {
				var o = iterator.nextString();
				if(object.Equals(o, null)) {
					break;
				}
				v.Add(o);
			}
			return(v);
		}

		private System.Collections.Generic.List<object> vectorForIterator(cape.Iterator<Article> iterator) {
			if(iterator == null) {
				return(null);
			}
			var v = new System.Collections.Generic.List<object>();
			while(true) {
				var o = iterator.next();
				if(o == null) {
					break;
				}
				var m = new cape.DynamicMap();
				o.exportData(m);
				v.Add(m);
			}
			return(v);
		}

		public void onGETCategoryList(HTTPServerRequest req, System.Action next) {
			sympathy.webcontent.Article.SQL.queryAllUniqueCategoryValues(database, articleTable, (cape.DynamicIterator results) => {
				if(results == null) {
					sendResponse(req, "error", (object)sympathy.JSONResponse.forErrorCode("query_failed"));
					return;
				}
				var data = new cape.DynamicMap();
				data.set("categories", (object)vectorForIterator(results));
				sendResponse(req, "categorylist", (object)data);
			});
		}

		public void onGETCategoryArticles(HTTPServerRequest req, string catId, System.Action next) {
			var pp = 0;
			var spage = req.getQueryParameter("page");
			if(!(object.Equals(spage, null))) {
				pp = cape.String.toInteger(spage) - 1;
			}
			if(pp < 0) {
				pp = 0;
			}
			sympathy.webcontent.Article.SQL.queryCountForCategory(database, articleTable, catId, (int count) => {
				var pages = count / articlesPerPage;
				if((count % articlesPerPage) > 0) {
					pages++;
				}
				if(pages < 1) {
					pages = 1;
				}
				var orderBy = new capex.SQLOrderingRule[] {
					capex.SQLOrderingRule.forDescending("timeStamp")
				};
				sympathy.webcontent.Article.SQL.queryByCategory(database, articleTable, catId, pp * articlesPerPage, articlesPerPage, orderBy, (cape.Iterator<Article> results) => {
					if(results == null) {
						sendResponse(req, "error", (object)sympathy.JSONResponse.forErrorCode("query_failed"));
					}
					else {
						var data = new cape.DynamicMap();
						data.set("articles", (object)vectorForIterator(results));
						data.set("category", (object)catId);
						if(pp > 0) {
							data.set("previousPage", pp);
						}
						if((pp + 2) <= pages) {
							data.set("nextPage", pp + 2);
						}
						data.set("currentPage", pp + 1);
						data.set("pageCount", pages);
						sendResponse(req, "articlelist", (object)data);
					}
				});
			});
		}

		public void onGETAllArticles(HTTPServerRequest req, System.Action next) {
			var pp = 0;
			var spage = req.getQueryParameter("page");
			if(!(object.Equals(spage, null))) {
				pp = cape.String.toInteger(spage) - 1;
			}
			if(pp < 0) {
				pp = 0;
			}
			sympathy.webcontent.Article.SQL.queryRecordCount(database, articleTable, (int count) => {
				var pages = count / articlesPerPage;
				if((count % articlesPerPage) > 0) {
					pages++;
				}
				if(pages < 1) {
					pages = 1;
				}
				var orderBy = new capex.SQLOrderingRule[] {
					capex.SQLOrderingRule.forDescending("timeStamp")
				};
				sympathy.webcontent.Article.SQL.queryPartial(database, articleTable, pp * articlesPerPage, articlesPerPage, orderBy, (cape.Iterator<Article> results) => {
					if(results == null) {
						sendResponse(req, "error", (object)sympathy.JSONResponse.forErrorCode("query_failed"));
					}
					else {
						var data = new cape.DynamicMap();
						data.set("articles", (object)vectorForIterator(results));
						if(pp > 0) {
							data.set("previousPage", pp);
						}
						if((pp + 2) <= pages) {
							data.set("nextPage", pp + 2);
						}
						data.set("currentPage", pp + 1);
						data.set("pageCount", pages);
						sendResponse(req, "articlelist", (object)data);
					}
				});
			});
		}

		public void onGETArticle(HTTPServerRequest req, string pageId, System.Action next) {
			sympathy.webcontent.Article.SQL.queryByName(database, articleTable, pageId, (Article article) => {
				if(article == null) {
					req.unpopResource();
					next();
					return;
				}
				else {
					var adata = new cape.DynamicMap();
					article.exportData(adata);
					sendResponse(req, "article", (object)adata);
				}
			});
		}

		public void sendResponse(HTTPServerRequest req, string type, object content) {
			var rdata = new cape.DynamicMap();
			if(cape.String.isEmpty(country) == false) {
				rdata.set("country", (object)country);
			}
			rdata.set("type", (object)type);
			rdata.set("content", content);
			if(isResponseHTML(req)) {
				var templateName = templatePrefix + type;
				templateStorage.getTemplate(templateName, (string template) => {
					if(object.Equals(template, null)) {
						req.sendJSONObject((object)rdata);
					}
					else {
						rdata.set("site", (object)siteData);
						sendHTMLResponse(req, rdata, template);
					}
				});
			}
			else {
				req.sendJSONObject((object)rdata);
			}
		}

		public void sendHTMLResponse(HTTPServerRequest req, cape.DynamicMap data, string tt) {
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

		public override void onGET(HTTPServerRequest req, System.Action next) {
			if((database == null) || (object.Equals(articleTable, null))) {
				next();
				return;
			}
			if(req.hasMoreResources() == false) {
				if(req.isForDirectory() == false) {
					req.sendRedirectAsDirectory();
					return;
				}
				onGETAllArticles(req, next);
				return;
			}
			if(req.acceptResource("articles")) {
				var pageId = req.popResource();
				if(cape.String.isEmpty(pageId)) {
					req.sendRedirect("../");
					return;
				}
				if(req.hasMoreResources()) {
					next();
					return;
				}
				if(req.isForDirectory() == false) {
					req.sendRedirectAsDirectory();
					return;
				}
				onGETArticle(req, pageId, next);
				return;
			}
			if(req.acceptResource("categories")) {
				if(req.isForDirectory() == false) {
					req.sendRedirectAsDirectory();
					return;
				}
				var catId = req.popResource();
				if(cape.String.isEmpty(catId)) {
					onGETCategoryList(req, next);
					return;
				}
				if(req.hasMoreResources()) {
					next();
					return;
				}
				onGETCategoryArticles(req, catId, next);
				return;
			}
			next();
		}

		public capex.SQLDatabase getDatabase() {
			return(database);
		}

		public ArticleWebHandler setDatabase(capex.SQLDatabase v) {
			database = v;
			return(this);
		}

		public string getArticleTable() {
			return(articleTable);
		}

		public ArticleWebHandler setArticleTable(string v) {
			articleTable = v;
			return(this);
		}

		public int getArticlesPerPage() {
			return(articlesPerPage);
		}

		public ArticleWebHandler setArticlesPerPage(int v) {
			articlesPerPage = v;
			return(this);
		}

		public capex.TemplateStorage getTemplateStorage() {
			return(templateStorage);
		}

		public ArticleWebHandler setTemplateStorage(capex.TemplateStorage v) {
			templateStorage = v;
			return(this);
		}

		public cape.DynamicMap getSiteData() {
			return(siteData);
		}

		public ArticleWebHandler setSiteData(cape.DynamicMap v) {
			siteData = v;
			return(this);
		}

		public string getTemplatePrefix() {
			return(templatePrefix);
		}

		public ArticleWebHandler setTemplatePrefix(string v) {
			templatePrefix = v;
			return(this);
		}

		public string getCountry() {
			return(country);
		}

		public ArticleWebHandler setCountry(string v) {
			country = v;
			return(this);
		}

		public System.Collections.Generic.List<string> getLanguages() {
			return(languages);
		}

		public ArticleWebHandler setLanguages(System.Collections.Generic.List<string> v) {
			languages = v;
			return(this);
		}
	}
}
