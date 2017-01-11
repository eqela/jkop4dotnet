
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
	public abstract class SQLiteDatabase : SQLDatabase
	{
		private static SQLiteDatabase instance() {
			System.Console.WriteLine("[capex.SQLiteDatabase.instance] (SQLiteDatabase.sling:40:2): Not implemented");
			return(null);
		}

		public static SQLiteDatabase forFile(cape.File file, bool allowCreate = true, cape.LoggingContext logger = null) {
			if(file == null) {
				return(null);
			}
			var v = instance();
			if(v == null) {
				return(null);
			}
			if(logger != null) {
				v.setLogger(logger);
			}
			if(file.isFile() == false) {
				if(allowCreate == false) {
					return(null);
				}
				var pp = file.getParent();
				if(pp.isDirectory() == false) {
					if(pp.createDirectoryRecursive() == false) {
						cape.Log.error(v.getLogger() as cape.LoggingContext, "Failed to create directory: " + pp.getPath());
					}
				}
				if(v.initialize(file, true) == false) {
					v = null;
				}
			}
			else if(v.initialize(file, false) == false) {
				v = null;
			}
			return(v);
		}

		public override string getDatabaseTypeId() {
			return("sqlite");
		}

		public virtual bool initialize(cape.File file, bool create) {
			return(true);
		}

		public override cape.DynamicMap querySingleRow(SQLStatement stmt) {
			var it = query(stmt);
			if(it == null) {
				return(null);
			}
			var v = it.next();
			return(v);
		}

		public override bool tableExists(string table) {
			if(object.Equals(table, null)) {
				return(false);
			}
			var stmt = prepare("SELECT name FROM sqlite_master WHERE type='table' AND name=?;");
			if(stmt == null) {
				return(false);
			}
			stmt.addParamString(table);
			var sr = querySingleRow(stmt);
			if(sr == null) {
				return(false);
			}
			if(cape.String.equals(table, sr.getString("name")) == false) {
				return(false);
			}
			return(true);
		}

		public override void queryAllTableNames(System.Action<System.Collections.Generic.List<object>> callback) {
			var v = queryAllTableNames();
			if(callback != null) {
				callback(v);
			}
		}

		public override System.Collections.Generic.List<object> queryAllTableNames() {
			var stmt = prepare("SELECT name FROM sqlite_master WHERE type='table';");
			if(stmt == null) {
				return(null);
			}
			var it = query(stmt);
			if(it == null) {
				return(null);
			}
			var v = new System.Collections.Generic.List<object>();
			while(true) {
				var o = it.next();
				if(o == null) {
					break;
				}
				var name = o.getString("name");
				if(cape.String.isEmpty(name) == false) {
					v.Add(name);
				}
			}
			return(v);
		}

		public virtual string columnToCreateString(SQLTableColumnInfo cc) {
			var sb = new cape.StringBuilder();
			sb.append(cc.getName());
			sb.append(' ');
			var tt = cc.getType();
			if(tt == capex.SQLTableColumnInfo.TYPE_INTEGER_KEY) {
				sb.append("INTEGER PRIMARY KEY AUTOINCREMENT");
			}
			else if(tt == capex.SQLTableColumnInfo.TYPE_STRING_KEY) {
				sb.append("TEXT PRIMARY KEY");
			}
			else if(tt == capex.SQLTableColumnInfo.TYPE_INTEGER) {
				sb.append("INTEGER");
			}
			else if(tt == capex.SQLTableColumnInfo.TYPE_STRING) {
				sb.append("VARCHAR(255)");
			}
			else if(tt == capex.SQLTableColumnInfo.TYPE_TEXT) {
				sb.append("TEXT");
			}
			else if(tt == capex.SQLTableColumnInfo.TYPE_BLOB) {
				sb.append("BLOB");
			}
			else if(tt == capex.SQLTableColumnInfo.TYPE_DOUBLE) {
				sb.append("REAL");
			}
			else {
				cape.Log.error(getLogger(), "Unknown column type: " + cape.String.forInteger(tt));
				sb.append("UNKNOWN");
			}
			return(sb.toString());
		}

		public override SQLStatement prepareCreateTableStatement(string table, System.Collections.Generic.List<SQLTableColumnInfo> columns) {
			if((object.Equals(table, null)) || (columns == null)) {
				return(null);
			}
			var sb = new cape.StringBuilder();
			sb.append("CREATE TABLE ");
			sb.append(table);
			sb.append(" (");
			var first = true;
			if(columns != null) {
				var n = 0;
				var m = columns.Count;
				for(n = 0 ; n < m ; n++) {
					var column = columns[n];
					if(column != null) {
						if(first == false) {
							sb.append(',');
						}
						sb.append(' ');
						sb.append(columnToCreateString(column));
						first = false;
					}
				}
			}
			sb.append(" );");
			return(prepare(sb.toString()));
		}

		public override SQLStatement prepareDeleteTableStatement(string table) {
			if(object.Equals(table, null)) {
				return(null);
			}
			var sb = new cape.StringBuilder();
			sb.append("DROP TABLE ");
			sb.append(table);
			sb.append(";");
			return(prepare(sb.toString()));
		}

		public override SQLStatement prepareCreateIndexStatement(string table, string column, bool unique) {
			if((object.Equals(table, null)) || (object.Equals(column, null))) {
				return(null);
			}
			var unq = "";
			if(unique) {
				unq = "UNIQUE ";
			}
			var sql = ((((((((("CREATE " + unq) + "INDEX ") + table) + "_") + column) + " ON ") + table) + " (") + column) + ")";
			return(prepare(sql));
		}
	}
}
