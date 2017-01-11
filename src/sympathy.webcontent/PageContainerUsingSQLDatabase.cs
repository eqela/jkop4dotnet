
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
	public class PageContainerUsingSQLDatabase : PageContainer
	{
		public static PageContainerUsingSQLDatabase forDatabase(capex.SQLDatabase database, string tablePrefix = null) {
			var v = new PageContainerUsingSQLDatabase();
			v.setDatabase(database);
			if(!(object.Equals(tablePrefix, null))) {
				v.setTablePrefix(tablePrefix);
			}
			return(v);
		}

		private capex.SQLDatabase database = null;
		private string tablePrefix = null;

		public PageContainerUsingSQLDatabase() {
			tablePrefix = "pages";
		}

		private string getTableName(string name) {
			if(object.Equals(name, null)) {
				return(null);
			}
			if(cape.String.isEmpty(tablePrefix)) {
				return(name);
			}
			return((tablePrefix + "_") + name);
		}

		public bool initializeTables() {
			if(database == null) {
				return(false);
			}
			var content = capex.SQLTableInfo.forName(getTableName("content"));
			content.addStringKeyColumn("id");
			content.addStringColumn("type");
			content.addTextColumn("data");
			content.addUniqueIndex("id");
			if(database.ensureTableExists(content) == false) {
				return(false);
			}
			var attachments = capex.SQLTableInfo.forName(getTableName("attachments"));
			attachments.addStringColumn("pageid");
			attachments.addStringColumn("filename");
			attachments.addBlobColumn("data");
			attachments.addIndex("pageid");
			attachments.addIndex("filename");
			if(database.ensureTableExists(attachments) == false) {
				return(false);
			}
			return(true);
		}

		public virtual void getAllPageIds(System.Action<cape.DynamicVector> callback) {
			if(callback == null) {
				return;
			}
			if(database == null) {
				callback(null);
				return;
			}
			database.query(database.prepare(("SELECT id FROM " + getTableName("content")) + ";"), (capex.SQLResultSetIterator results) => {
				if(results == null) {
					callback(null);
				}
				else {
					callback(results.toVector());
				}
			});
		}

		public virtual void getPageData(string pageId, System.Action<string, cape.DynamicMap, cape.DynamicVector> callback) {
			if(callback == null) {
				return;
			}
			if(database == null) {
				callback(null, null, null);
				return;
			}
			var stmt1 = database.prepare(("SELECT type,data FROM " + getTableName("content")) + " WHERE id = ?;");
			if(stmt1 == null) {
				callback(null, null, null);
				return;
			}
			stmt1.addParamString(pageId);
			database.querySingleRow(stmt1, (cape.DynamicMap result) => {
				if(result == null) {
					callback(null, null, null);
					return;
				}
				var stmt2 = database.prepare(("SELECT filename FROM " + getTableName("attachments")) + " WHERE pageid = ?;");
				if(stmt2 == null) {
					callback(null, null, null);
					return;
				}
				stmt2.addParamString(pageId);
				database.query(stmt2, (capex.SQLResultSetIterator attrs) => {
					callback(result.getString("type"), cape.JSONParser.parse(result.getString("data")) as cape.DynamicMap, attrs.toVector());
				});
			});
		}

		public virtual void createPage(string pageId, string type, System.Action<bool> callback) {
			if(database == null) {
				if(callback != null) {
					callback(false);
				}
				return;
			}
			var stmt = database.prepareInsertStatement(getTableName("content"), cape.DynamicMap.forStringMap(new System.Collections.Generic.Dictionary<string,string> {
				{"id", pageId},
				{"type", type},
				{"data", ""}
			}));
			if(stmt == null) {
				if(callback != null) {
					callback(false);
				}
				return;
			}
			database.execute(stmt, callback);
		}

		public virtual void changePageType(string pageId, string type, System.Action<bool> callback) {
			if(database == null) {
				if(callback != null) {
					callback(false);
				}
				return;
			}
			var criteria = cape.DynamicMap.forStringMap(new System.Collections.Generic.Dictionary<string,string> {
				{"id", pageId}
			});
			var data = cape.DynamicMap.forStringMap(new System.Collections.Generic.Dictionary<string,string> {
				{"type", type}
			});
			var stmt = database.prepareUpdateStatement(getTableName("content"), criteria, data);
			if(stmt == null) {
				if(callback != null) {
					callback(false);
				}
				return;
			}
			database.execute(stmt, callback);
		}

		public virtual void deletePage(string pageId, System.Action<bool> callback) {
			if(database == null) {
				if(callback != null) {
					callback(false);
				}
				return;
			}
			deleteAllAttachments(pageId, (bool status) => {
				if(status == false) {
					if(callback != null) {
						callback(false);
					}
					return;
				}
				var stmt = database.prepare(("DELETE FROM " + getTableName("content")) + " WHERE id = ?;");
				if(stmt == null) {
					if(callback != null) {
						callback(false);
					}
					return;
				}
				stmt.addParamString(pageId);
				database.execute(stmt, callback);
			});
		}

		public virtual void updatePageContent(string pageId, cape.DynamicMap content, System.Action<bool> callback) {
			if(database == null) {
				if(callback != null) {
					callback(false);
				}
				return;
			}
			var criteria = cape.DynamicMap.forStringMap(new System.Collections.Generic.Dictionary<string,string> {
				{"id", pageId}
			});
			var data = cape.DynamicMap.forStringMap(new System.Collections.Generic.Dictionary<string,string> {
				{"data", cape.JSONEncoder.encode((object)content)}
			});
			var stmt = database.prepareUpdateStatement(getTableName("content"), criteria, data);
			if(stmt == null) {
				if(callback != null) {
					callback(false);
				}
				return;
			}
			database.execute(stmt, callback);
		}

		public virtual void addAttachment(string pageId, string fileName, byte[] content, System.Action<bool> callback) {
			if(database == null) {
				if(callback != null) {
					callback(false);
				}
				return;
			}
			var data = new cape.DynamicMap();
			data.set("pageid", (object)pageId);
			data.set("filename", (object)fileName);
			data.set("data", content);
			var stmt = database.prepareInsertStatement(getTableName("attachments"), data);
			if(stmt == null) {
				if(callback != null) {
					callback(false);
				}
				return;
			}
			database.execute(stmt, callback);
		}

		public virtual void replaceAttachment(string pageId, string fileName, byte[] content, System.Action<bool> callback) {
			if(database == null) {
				if(callback != null) {
					callback(false);
				}
				return;
			}
			var criteria = new cape.DynamicMap();
			criteria.set("pageid", (object)pageId);
			criteria.set("filename", (object)fileName);
			var data = new cape.DynamicMap();
			data.set("data", content);
			var stmt = database.prepareUpdateStatement(getTableName("attachments"), criteria, data);
			if(stmt == null) {
				if(callback != null) {
					callback(false);
				}
				return;
			}
			database.execute(stmt, callback);
		}

		public virtual void getAttachment(string pageId, string fileName, System.Action<byte[]> callback) {
			if(callback == null) {
				return;
			}
			if(database == null) {
				callback(null);
				return;
			}
			var stmt = database.prepare(("SELECT data FROM " + getTableName("attachments")) + " WHERE pageid = ? AND filename = ?;");
			if(stmt == null) {
				callback(null);
				return;
			}
			stmt.addParamString(pageId);
			stmt.addParamString(fileName);
			database.querySingleRow(stmt, (cape.DynamicMap data) => {
				if(data == null) {
					callback(null);
				}
				else {
					callback(data.getBuffer("data"));
				}
			});
		}

		public virtual void deleteAttachment(string pageId, string fileName, System.Action<bool> callback) {
			if(database == null) {
				if(callback != null) {
					callback(false);
				}
				return;
			}
			var stmt = database.prepare(("DELETE FROM " + getTableName("attachments")) + " WHERE pageid = ? AND filename = ?;");
			if(stmt == null) {
				if(callback != null) {
					callback(false);
				}
				return;
			}
			stmt.addParamString(pageId);
			stmt.addParamString(fileName);
			database.execute(stmt, callback);
		}

		public virtual void deleteAllAttachments(string pageId, System.Action<bool> callback) {
			if(database == null) {
				if(callback != null) {
					callback(false);
				}
				return;
			}
			var stmt = database.prepare(("DELETE FROM " + getTableName("attachments")) + " WHERE pageid = ?;");
			if(stmt == null) {
				if(callback != null) {
					callback(false);
				}
				return;
			}
			stmt.addParamString(pageId);
			database.execute(stmt, callback);
		}

		public capex.SQLDatabase getDatabase() {
			return(database);
		}

		public PageContainerUsingSQLDatabase setDatabase(capex.SQLDatabase v) {
			database = v;
			return(this);
		}

		public string getTablePrefix() {
			return(tablePrefix);
		}

		public PageContainerUsingSQLDatabase setTablePrefix(string v) {
			tablePrefix = v;
			return(this);
		}
	}
}
