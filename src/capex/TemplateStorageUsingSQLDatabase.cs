
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
	public class TemplateStorageUsingSQLDatabase : TemplateStorage
	{
		public static TemplateStorageUsingSQLDatabase forDatabase(SQLDatabase db, string table = null, string idColumn = null, string contentColumn = null) {
			var v = new TemplateStorageUsingSQLDatabase();
			v.setDatabase(db);
			if(!(object.Equals(table, null))) {
				v.setTable(table);
			}
			if(!(object.Equals(idColumn, null))) {
				v.setIdColumn(idColumn);
			}
			if(!(object.Equals(contentColumn, null))) {
				v.setContentColumn(contentColumn);
			}
			return(v);
		}

		private SQLDatabase database = null;
		private string table = null;
		private string idColumn = null;
		private string contentColumn = null;

		public TemplateStorageUsingSQLDatabase() {
			table = "templates";
			idColumn = "id";
			contentColumn = "content";
		}

		public virtual void getTemplate(string id, System.Action<string> callback) {
			if(callback == null) {
				return;
			}
			if(((database == null) || cape.String.isEmpty(table)) || cape.String.isEmpty(id)) {
				callback(null);
				return;
			}
			var stmt = database.prepare(((("SELECT content FROM " + table) + " WHERE ") + idColumn) + " = ?;");
			if(stmt == null) {
				callback(null);
				return;
			}
			stmt.addParamString(id);
			var cb = callback;
			database.querySingleRow(stmt, (cape.DynamicMap data) => {
				if(data == null) {
					cb(null);
					return;
				}
				cb(data.getString("content"));
			});
		}

		public SQLDatabase getDatabase() {
			return(database);
		}

		public TemplateStorageUsingSQLDatabase setDatabase(SQLDatabase v) {
			database = v;
			return(this);
		}

		public string getTable() {
			return(table);
		}

		public TemplateStorageUsingSQLDatabase setTable(string v) {
			table = v;
			return(this);
		}

		public string getIdColumn() {
			return(idColumn);
		}

		public TemplateStorageUsingSQLDatabase setIdColumn(string v) {
			idColumn = v;
			return(this);
		}

		public string getContentColumn() {
			return(contentColumn);
		}

		public TemplateStorageUsingSQLDatabase setContentColumn(string v) {
			contentColumn = v;
			return(this);
		}
	}
}
