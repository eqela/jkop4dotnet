
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
	public class Article
	{
		private int id = -1;
		private string name = null;
		private string title = null;
		private string content = null;
		private string intro = null;
		private long timeStamp = (long)0;
		private string author = null;
		private string category = null;

		public Article() {
			setTimeStamp(cape.SystemClock.asSeconds());
		}

		public void generateName() {
			this.name = doGenerateName();
		}

		private string doGenerateName() {
			if(object.Equals(title, null)) {
				return(null);
			}
			var sb = new cape.StringBuilder();
			var dt = cape.DateTime.forTimeSeconds(timeStamp);
			if(dt != null) {
				var month = dt.getMonth();
				var year = dt.getYear();
				if((month > 0) && (year > 1900)) {
					sb.append(cape.String.forInteger(year));
					sb.append('-');
					sb.append(cape.String.forIntegerWithPadding(month, 2, "0"));
					sb.append('-');
				}
			}
			var it = cape.String.iterate(title);
			var c = ' ';
			var pc = (char)0;
			while((c = it.getNextChar()) > 0) {
				if((c >= 'A') && (c <= 'Z')) {
					c = (char)((c - 'A') + 'a');
				}
				if((((c >= 'a') && (c <= 'z')) || ((c >= '0') && (c <= '9'))) || (c == '-')) {
					;
				}
				else {
					c = '-';
				}
				if(c == '-') {
					if(pc != '-') {
						sb.append(c);
					}
				}
				else {
					sb.append(c);
				}
				pc = c;
			}
			var r = sb.toString();
			if(cape.String.endsWith(r, "-")) {
				r = cape.String.getSubString(r, 0, cape.String.getLength(r) - 1);
			}
			return(r);
		}

		public void validate(System.Action<cape.Error> callback) {
			if(cape.String.isEmpty(title)) {
				callback(cape.Error.forCode("emptyTitle"));
				return;
			}
			if(cape.String.getLength(title) < 3) {
				callback(cape.Error.forCode("titleTooShort"));
				return;
			}
			if(cape.String.isEmpty(name)) {
				callback(cape.Error.forCode("emptyName"));
				return;
			}
			callback(null);
		}

		public static Article forDynamicMap(cape.DynamicMap data) {
			var v = new Article();
			v.importData(data);
			return(v);
		}

		public static void forValidDynamicMap(cape.DynamicMap data, System.Action<Article, cape.Error> callback) {
			var v = new Article();
			v.importData(data);
			var cb = callback;
			v.validate((cape.Error error) => {
				if(error == null) {
					cb(v, null);
				}
				else {
					cb(null, error);
				}
			});
		}

		public cape.DynamicMap toDynamicMap() {
			var v = new cape.DynamicMap();
			exportData(v);
			return(v);
		}

		public void exportData(cape.DynamicMap data, bool includePrimaryKey = true) {
			if(data == null) {
				return;
			}
			if(includePrimaryKey && (id >= 0)) {
				data.set("id", id);
			}
			if(!(object.Equals(name, null))) {
				data.set("name", (object)name);
			}
			if(!(object.Equals(title, null))) {
				data.set("title", (object)title);
			}
			if(!(object.Equals(content, null))) {
				data.set("content", (object)content);
			}
			if(!(object.Equals(intro, null))) {
				data.set("intro", (object)intro);
			}
			data.set("timeStamp", (int)timeStamp);
			if(!(object.Equals(author, null))) {
				data.set("author", (object)author);
			}
			if(!(object.Equals(category, null))) {
				data.set("category", (object)category);
			}
		}

		public void importData(cape.DynamicMap data, bool partial = false) {
			if(data == null) {
				return;
			}
			if((partial == false) || data.containsKey("id")) {
				setId(data.getInteger("id", id));
			}
			if((partial == false) || data.containsKey("name")) {
				setName(data.getString("name", name));
			}
			if((partial == false) || data.containsKey("title")) {
				setTitle(data.getString("title", title));
			}
			if((partial == false) || data.containsKey("content")) {
				setContent(data.getString("content", content));
			}
			if((partial == false) || data.containsKey("intro")) {
				setIntro(data.getString("intro", intro));
			}
			if((partial == false) || data.containsKey("timeStamp")) {
				setTimeStamp((long)data.getInteger("timeStamp", (int)timeStamp));
			}
			if((partial == false) || data.containsKey("author")) {
				setAuthor(data.getString("author", author));
			}
			if((partial == false) || data.containsKey("category")) {
				setCategory(data.getString("category", category));
			}
			onImportDataCompleted();
		}

		public void onImportDataCompleted() {
			generateName();
		}

		public class SQL
		{
			public static capex.SQLTableInfo getTableInfo(string tableName) {
				var v = capex.SQLTableInfo.forName(tableName);
				v.addIntegerKeyColumn("id");
				v.addStringColumn("name");
				v.addUniqueIndex("name");
				v.addStringColumn("title");
				v.addStringColumn("content");
				v.addStringColumn("intro");
				v.addIntegerColumn("timeStamp");
				v.addStringColumn("author");
				v.addStringColumn("category");
				v.addIndex("category");
				return(v);
			}

			class MyIterator : cape.Iterator<Article>
			{
				private capex.SQLResultSetIterator iterator = null;

				public virtual Article next() {
					if(iterator == null) {
						return(null);
					}
					var r = iterator.next();
					if(r == null) {
						return(null);
					}
					var v = new Article();
					v.importData(r);
					return(v);
				}

				public capex.SQLResultSetIterator getIterator() {
					return(iterator);
				}

				public MyIterator setIterator(capex.SQLResultSetIterator v) {
					iterator = v;
					return(this);
				}
			}

			public static void queryRecordCount(capex.SQLDatabase database, string tableName, System.Action<int> callback) {
				if(callback == null) {
					return;
				}
				if(database == null) {
					callback(0);
					return;
				}
				var stmt = database.prepareCountRecordsStatement(tableName, null);
				if(stmt == null) {
					callback(0);
					return;
				}
				database.querySingleRow(stmt, (cape.DynamicMap result) => {
					if(result == null) {
						callback(0);
					}
					else {
						callback(result.getInteger("count"));
					}
				});
			}

			public static void queryAll(capex.SQLDatabase database, string tableName, capex.SQLOrderingRule[] orderBy, System.Action<cape.Iterator<Article>> callback) {
				if(callback == null) {
					return;
				}
				if(database == null) {
					callback(null);
					return;
				}
				var stmt = database.prepareQueryAllStatement(tableName, new string[] {
					"id",
					"name",
					"title",
					"intro",
					"timeStamp",
					"author",
					"category"
				}, orderBy);
				if(stmt == null) {
					callback(null);
					return;
				}
				database.query(stmt, (capex.SQLResultSetIterator results) => {
					if(results == null) {
						callback(null);
					}
					else {
						callback((cape.Iterator<Article>)new MyIterator().setIterator(results));
					}
				});
			}

			public static void queryPartial(capex.SQLDatabase database, string tableName, int offset, int limit, capex.SQLOrderingRule[] orderBy, System.Action<cape.Iterator<Article>> callback) {
				if(callback == null) {
					return;
				}
				if(database == null) {
					callback(null);
					return;
				}
				var stmt = database.prepareQueryWithCriteriaStatement(tableName, null, limit, offset, new string[] {
					"id",
					"name",
					"title",
					"intro",
					"timeStamp",
					"author",
					"category"
				}, orderBy);
				if(stmt == null) {
					callback(null);
					return;
				}
				database.query(stmt, (capex.SQLResultSetIterator results) => {
					if(results == null) {
						callback(null);
					}
					else {
						callback((cape.Iterator<Article>)new MyIterator().setIterator(results));
					}
				});
			}

			public static void insert(capex.SQLDatabase database, string tableName, Article @object, System.Action<bool> callback) {
				if((database == null) || (@object == null)) {
					if(callback != null) {
						callback(false);
					}
					return;
				}
				var data = new cape.DynamicMap();
				@object.exportData(data, false);
				var stmt = database.prepareInsertStatement(tableName, data);
				if(stmt == null) {
					if(callback != null) {
						callback(false);
					}
					return;
				}
				database.execute(stmt, callback);
			}

			public static void update(capex.SQLDatabase database, string tableName, Article @object, System.Action<bool> callback) {
				if((database == null) || (@object == null)) {
					if(callback != null) {
						callback(false);
					}
					return;
				}
				var data = new cape.DynamicMap();
				@object.exportData(data, false);
				var stmt = database.prepareUpdateStatement(tableName, cape.DynamicMap.forStringMap(new System.Collections.Generic.Dictionary<string,string> {
					{"id", cape.String.asString(@object.getId())}
				}), data);
				if(stmt == null) {
					if(callback != null) {
						callback(false);
					}
					return;
				}
				database.execute(stmt, callback);
			}

			public static void delete(capex.SQLDatabase database, string tableName, Article @object, System.Action<bool> callback) {
				if(@object == null) {
					if(callback != null) {
						callback(false);
					}
					return;
				}
				deleteById(database, tableName, @object.getId(), callback);
			}

			public static void queryById(capex.SQLDatabase database, string tableName, int id, System.Action<Article> callback) {
				if(callback == null) {
					return;
				}
				if(database == null) {
					callback(null);
					return;
				}
				var stmt = database.prepareQueryWithCriteriaStatement(tableName, new System.Collections.Generic.Dictionary<string,string> {
					{"id", cape.String.asString(id)}
				}, 1);
				if(stmt == null) {
					callback(null);
					return;
				}
				database.querySingleRow(stmt, (cape.DynamicMap result) => {
					if(result == null) {
						callback(null);
					}
					else {
						var v = new Article();
						v.importData(result);
						callback(v);
					}
				});
			}

			public static void deleteById(capex.SQLDatabase database, string tableName, int id, System.Action<bool> callback) {
				if(database == null) {
					if(callback != null) {
						callback(false);
					}
					return;
				}
				var stmt = database.prepareDeleteStatement(tableName, cape.DynamicMap.forStringMap(new System.Collections.Generic.Dictionary<string,string> {
					{"id", cape.String.asString(id)}
				}));
				if(stmt == null) {
					if(callback != null) {
						callback(false);
					}
					return;
				}
				database.execute(stmt, callback);
			}

			public static void queryByName(capex.SQLDatabase database, string tableName, string name, System.Action<Article> callback) {
				if(callback == null) {
					return;
				}
				if(database == null) {
					callback(null);
					return;
				}
				var stmt = database.prepareQueryWithCriteriaStatement(tableName, new System.Collections.Generic.Dictionary<string,string> {
					{"name", cape.String.asString((object)name)}
				}, 1);
				if(stmt == null) {
					callback(null);
					return;
				}
				database.querySingleRow(stmt, (cape.DynamicMap result) => {
					if(result == null) {
						callback(null);
					}
					else {
						var v = new Article();
						v.importData(result);
						callback(v);
					}
				});
			}

			public static void deleteByName(capex.SQLDatabase database, string tableName, string name, System.Action<bool> callback) {
				if(database == null) {
					if(callback != null) {
						callback(false);
					}
					return;
				}
				var stmt = database.prepareDeleteStatement(tableName, cape.DynamicMap.forStringMap(new System.Collections.Generic.Dictionary<string,string> {
					{"name", cape.String.asString((object)name)}
				}));
				if(stmt == null) {
					if(callback != null) {
						callback(false);
					}
					return;
				}
				database.execute(stmt, callback);
			}

			public static void queryByCategory(capex.SQLDatabase database, string tableName, string category, int offset, int limit, capex.SQLOrderingRule[] orderBy, System.Action<cape.Iterator<Article>> callback) {
				if(callback == null) {
					return;
				}
				if(database == null) {
					callback(null);
					return;
				}
				var stmt = database.prepareQueryWithCriteriaStatement(tableName, new System.Collections.Generic.Dictionary<string,string> {
					{"category", cape.String.asString((object)category)}
				}, limit, offset, new string[] {
					"id",
					"name",
					"title",
					"intro",
					"timeStamp",
					"author",
					"category"
				}, orderBy);
				if(stmt == null) {
					callback(null);
					return;
				}
				database.query(stmt, (capex.SQLResultSetIterator results) => {
					if(results == null) {
						callback(null);
					}
					else {
						callback((cape.Iterator<Article>)new MyIterator().setIterator(results));
					}
				});
			}

			public static void queryAllUniqueCategoryValues(capex.SQLDatabase database, string tableName, System.Action<cape.DynamicIterator> callback) {
				if(callback == null) {
					return;
				}
				if(database == null) {
					callback(null);
					return;
				}
				var stmt = database.prepareQueryDistinctValuesStatement(tableName, "category");
				if(stmt == null) {
					callback(null);
					return;
				}
				database.query(stmt, (capex.SQLResultSetIterator results) => {
					if(results == null) {
						callback(null);
						return;
					}
					callback((cape.DynamicIterator)new capex.SQLResultSetSingleColumnIterator().setColumnName("category").setIterator(results));
				});
			}

			public static void queryCountForCategory(capex.SQLDatabase database, string tableName, string category, System.Action<int> callback) {
				if(callback == null) {
					return;
				}
				if(database == null) {
					callback(0);
					return;
				}
				var stmt = database.prepareCountRecordsStatement(tableName, new System.Collections.Generic.Dictionary<string,string> {
					{"category", cape.String.asString((object)category)}
				});
				if(stmt == null) {
					callback(0);
					return;
				}
				database.querySingleRow(stmt, (cape.DynamicMap result) => {
					if(result == null) {
						callback(0);
					}
					else {
						callback(result.getInteger("count"));
					}
				});
			}

			public static void deleteByCategory(capex.SQLDatabase database, string tableName, string category, System.Action<bool> callback) {
				if(database == null) {
					if(callback != null) {
						callback(false);
					}
					return;
				}
				var stmt = database.prepareDeleteStatement(tableName, cape.DynamicMap.forStringMap(new System.Collections.Generic.Dictionary<string,string> {
					{"category", cape.String.asString((object)category)}
				}));
				if(stmt == null) {
					if(callback != null) {
						callback(false);
					}
					return;
				}
				database.execute(stmt, callback);
			}
		}

		public class HTTPServerRequestHandler : HTTPServerRequestHandlerMap
		{
			public static HTTPServerRequestHandler forDatabase(capex.SQLDatabase database, string tableName) {
				var v = new HTTPServerRequestHandler();
				v.setDatabase(database);
				v.setTableName(tableName);
				return(v);
			}

			private capex.SQLDatabase database = null;
			private string tableName = null;
			private int recordsPerPage = 50;

			public override void initialize(HTTPServerBase server) {
				base.initialize(server);
				if((database != null) || !(object.Equals(tableName, null))) {
					database.ensureTableExists(sympathy.webcontent.Article.SQL.getTableInfo(tableName));
				}
				get("", onGetAllRecords);
				get("id/*", (HTTPServerRequest req) => {
					onGetById(req, req.popResource());
				});
				delete("id/*", (HTTPServerRequest req2) => {
					onDeleteById(req2, req2.popResource());
				});
				put("id/*", (HTTPServerRequest req4) => {
					onReplaceById(req4, req4.popResource());
				});
				get("name/*", (HTTPServerRequest req6) => {
					onGetByName(req6, req6.popResource());
				});
				delete("name/*", (HTTPServerRequest req8) => {
					onDeleteByName(req8, req8.popResource());
				});
				put("name/*", (HTTPServerRequest req10) => {
					onReplaceByName(req10, req10.popResource());
				});
				get("category", onGetCategoryList);
				get("category/*", (HTTPServerRequest req12) => {
					onGetByCategory(req12, req12.popResource());
				});
				delete("category/*", (HTTPServerRequest req14) => {
					onDeleteByCategory(req14, req14.popResource());
				});
				post("", onPostRecord);
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

			public void onGetAllRecords(HTTPServerRequest req) {
				var pp = 0;
				var spage = req.getQueryParameter("page");
				if(!(object.Equals(spage, null))) {
					pp = cape.String.toInteger(spage) - 1;
				}
				if(pp < 0) {
					pp = 0;
				}
				sympathy.webcontent.Article.SQL.queryRecordCount(database, tableName, (int count) => {
					var pages = count / recordsPerPage;
					if((count % recordsPerPage) > 0) {
						pages++;
					}
					if(pages < 1) {
						pages = 1;
					}
					var orderBy = new capex.SQLOrderingRule[] {
						capex.SQLOrderingRule.forDescending("timeStamp")
					};
					sympathy.webcontent.Article.SQL.queryPartial(database, tableName, pp * recordsPerPage, recordsPerPage, orderBy, (cape.Iterator<Article> results) => {
						if(results == null) {
							req.sendJSONObject((object)sympathy.JSONResponse.forInternalError());
							return;
						}
						var data = new cape.DynamicMap();
						data.set("records", (object)vectorForIterator(results));
						data.set("currentPage", pp + 1);
						data.set("pageCount", pages);
						req.sendJSONObject((object)sympathy.JSONResponse.forOk((object)data));
					});
				});
			}

			public void onPostRecord(HTTPServerRequest req) {
				var data = req.getBodyJSONMap();
				if(data == null) {
					req.sendJSONObject((object)sympathy.JSONResponse.forInvalidRequest());
					return;
				}
				data.remove("id");
				var @object = new Article();
				@object.importData(data);
				@object.validate((cape.Error error) => {
					if(error != null) {
						req.sendJSONError(error);
						return;
					}
					sympathy.webcontent.Article.SQL.insert(database, tableName, @object, (bool status) => {
						if(status == false) {
							req.sendJSONObject((object)sympathy.JSONResponse.forInternalError());
							return;
						}
						req.sendJSONObject((object)sympathy.JSONResponse.forOk());
					});
				});
			}

			public void onGetById(HTTPServerRequest req, string id) {
				var dd = cape.String.toInteger(id);
				sympathy.webcontent.Article.SQL.queryById(database, tableName, dd, (Article record) => {
					if(record == null) {
						req.sendJSONObject((object)sympathy.JSONResponse.forNotFound());
						return;
					}
					var adata = new cape.DynamicMap();
					record.exportData(adata);
					req.sendJSONObject((object)sympathy.JSONResponse.forOk((object)adata));
				});
			}

			public void onReplaceById(HTTPServerRequest req, string id) {
				var data = req.getBodyJSONMap();
				if(data == null) {
					req.sendJSONObject((object)sympathy.JSONResponse.forInvalidRequest());
					return;
				}
				var dd = cape.String.toInteger(id);
				var record = new Article();
				record.importData(data);
				record.setId(dd);
				record.validate((cape.Error error) => {
					if(error != null) {
						req.sendJSONError(error);
						return;
					}
					sympathy.webcontent.Article.SQL.update(database, tableName, record, (bool status) => {
						if(status == false) {
							req.sendJSONObject((object)sympathy.JSONResponse.forInternalError());
							return;
						}
						req.sendJSONObject((object)sympathy.JSONResponse.forOk());
					});
				});
			}

			public void onDeleteById(HTTPServerRequest req, string id) {
				var dd = cape.String.toInteger(id);
				sympathy.webcontent.Article.SQL.deleteById(database, tableName, dd, (bool status) => {
					if(status == false) {
						req.sendJSONObject((object)sympathy.JSONResponse.forInternalError());
						return;
					}
					req.sendJSONObject((object)sympathy.JSONResponse.forOk());
				});
			}

			public void onGetByName(HTTPServerRequest req, string name) {
				var dd = name;
				sympathy.webcontent.Article.SQL.queryByName(database, tableName, dd, (Article record) => {
					if(record == null) {
						req.sendJSONObject((object)sympathy.JSONResponse.forNotFound());
						return;
					}
					var adata = new cape.DynamicMap();
					record.exportData(adata);
					req.sendJSONObject((object)sympathy.JSONResponse.forOk((object)adata));
				});
			}

			public void onReplaceByName(HTTPServerRequest req, string name) {
				var data = req.getBodyJSONMap();
				if(data == null) {
					req.sendJSONObject((object)sympathy.JSONResponse.forInvalidRequest());
					return;
				}
				var dd = name;
				var record = new Article();
				record.importData(data);
				record.setName(dd);
				record.validate((cape.Error error) => {
					if(error != null) {
						req.sendJSONError(error);
						return;
					}
					sympathy.webcontent.Article.SQL.update(database, tableName, record, (bool status) => {
						if(status == false) {
							req.sendJSONObject((object)sympathy.JSONResponse.forInternalError());
							return;
						}
						req.sendJSONObject((object)sympathy.JSONResponse.forOk());
					});
				});
			}

			public void onDeleteByName(HTTPServerRequest req, string name) {
				var dd = name;
				sympathy.webcontent.Article.SQL.deleteByName(database, tableName, dd, (bool status) => {
					if(status == false) {
						req.sendJSONObject((object)sympathy.JSONResponse.forInternalError());
						return;
					}
					req.sendJSONObject((object)sympathy.JSONResponse.forOk());
				});
			}

			public void onGetCategoryList(HTTPServerRequest req) {
				sympathy.webcontent.Article.SQL.queryAllUniqueCategoryValues(database, tableName, (cape.DynamicIterator results) => {
					if(results == null) {
						req.sendJSONObject((object)sympathy.JSONResponse.forInternalError());
						return;
					}
					req.sendJSONObject((object)sympathy.JSONResponse.forOk((object)vectorForIterator(results)));
				});
			}

			public void onGetByCategory(HTTPServerRequest req, string category) {
				var dd = category;
				var pp = 0;
				var spage = req.getQueryParameter("page");
				if(!(object.Equals(spage, null))) {
					pp = cape.String.toInteger(spage) - 1;
				}
				if(pp < 0) {
					pp = 0;
				}
				sympathy.webcontent.Article.SQL.queryCountForCategory(database, tableName, dd, (int count) => {
					var pages = count / recordsPerPage;
					if((count % recordsPerPage) > 0) {
						pages++;
					}
					if(pages < 1) {
						pages = 1;
					}
					var orderBy = new capex.SQLOrderingRule[] {
						capex.SQLOrderingRule.forDescending("timeStamp")
					};
					sympathy.webcontent.Article.SQL.queryByCategory(database, tableName, dd, pp * recordsPerPage, recordsPerPage, orderBy, (cape.Iterator<Article> results) => {
						if(results == null) {
							req.sendJSONObject((object)sympathy.JSONResponse.forInternalError());
							return;
						}
						var data = new cape.DynamicMap();
						data.set("records", (object)vectorForIterator(results));
						data.set("category", (object)category);
						data.set("currentPage", pp + 1);
						data.set("pageCount", pages);
						req.sendJSONObject((object)sympathy.JSONResponse.forOk((object)data));
					});
				});
			}

			public void onDeleteByCategory(HTTPServerRequest req, string category) {
				var dd = category;
				sympathy.webcontent.Article.SQL.deleteByCategory(database, tableName, dd, (bool status) => {
					if(status == false) {
						req.sendJSONObject((object)sympathy.JSONResponse.forInternalError());
						return;
					}
					req.sendJSONObject((object)sympathy.JSONResponse.forOk());
				});
			}

			public capex.SQLDatabase getDatabase() {
				return(database);
			}

			public HTTPServerRequestHandler setDatabase(capex.SQLDatabase v) {
				database = v;
				return(this);
			}

			public string getTableName() {
				return(tableName);
			}

			public HTTPServerRequestHandler setTableName(string v) {
				tableName = v;
				return(this);
			}

			public int getRecordsPerPage() {
				return(recordsPerPage);
			}

			public HTTPServerRequestHandler setRecordsPerPage(int v) {
				recordsPerPage = v;
				return(this);
			}
		}

		public int getId() {
			return(id);
		}

		public Article setId(int v) {
			id = v;
			return(this);
		}

		public string getName() {
			return(name);
		}

		public Article setName(string v) {
			name = v;
			return(this);
		}

		public string getTitle() {
			return(title);
		}

		public Article setTitle(string v) {
			title = v;
			return(this);
		}

		public string getContent() {
			return(content);
		}

		public Article setContent(string v) {
			content = v;
			return(this);
		}

		public string getIntro() {
			return(intro);
		}

		public Article setIntro(string v) {
			intro = v;
			return(this);
		}

		public long getTimeStamp() {
			return(timeStamp);
		}

		public Article setTimeStamp(long v) {
			timeStamp = v;
			return(this);
		}

		public string getAuthor() {
			return(author);
		}

		public Article setAuthor(string v) {
			author = v;
			return(this);
		}

		public string getCategory() {
			return(category);
		}

		public Article setCategory(string v) {
			category = v;
			return(this);
		}
	}
}
