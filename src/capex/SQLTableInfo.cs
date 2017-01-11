
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
	public class SQLTableInfo
	{
		public static SQLTableInfo forName(string name) {
			return(new SQLTableInfo().setName(name));
		}

		public static SQLTableInfo forDetails(string name, SQLTableColumnInfo[] columns = null, string[] indexes = null, string[] uniqueIndexes = null) {
			var v = new SQLTableInfo();
			v.setName(name);
			if(columns != null) {
				var n = 0;
				var m = columns.Length;
				for(n = 0 ; n < m ; n++) {
					var column = columns[n];
					if(column != null) {
						v.addColumn(column);
					}
				}
			}
			if(indexes != null) {
				var n2 = 0;
				var m2 = indexes.Length;
				for(n2 = 0 ; n2 < m2 ; n2++) {
					var index = indexes[n2];
					if(index != null) {
						v.addIndex(index);
					}
				}
			}
			if(uniqueIndexes != null) {
				var n3 = 0;
				var m3 = uniqueIndexes.Length;
				for(n3 = 0 ; n3 < m3 ; n3++) {
					var uniqueIndex = uniqueIndexes[n3];
					if(uniqueIndex != null) {
						v.addUniqueIndex(uniqueIndex);
					}
				}
			}
			return(v);
		}

		private string name = null;
		private System.Collections.Generic.List<SQLTableColumnInfo> columns = null;
		private System.Collections.Generic.List<SQLTableColumnIndexInfo> indexes = null;

		public SQLTableInfo addColumn(SQLTableColumnInfo info) {
			if(info == null) {
				return(this);
			}
			if(columns == null) {
				columns = new System.Collections.Generic.List<SQLTableColumnInfo>();
			}
			columns.Add(info);
			return(this);
		}

		public SQLTableInfo addIntegerColumn(string name) {
			return(addColumn(capex.SQLTableColumnInfo.forInteger(name)));
		}

		public SQLTableInfo addStringColumn(string name) {
			return(addColumn(capex.SQLTableColumnInfo.forString(name)));
		}

		public SQLTableInfo addStringKeyColumn(string name) {
			return(addColumn(capex.SQLTableColumnInfo.forStringKey(name)));
		}

		public SQLTableInfo addTextColumn(string name) {
			return(addColumn(capex.SQLTableColumnInfo.forText(name)));
		}

		public SQLTableInfo addIntegerKeyColumn(string name) {
			return(addColumn(capex.SQLTableColumnInfo.forIntegerKey(name)));
		}

		public SQLTableInfo addDoubleColumn(string name) {
			return(addColumn(capex.SQLTableColumnInfo.forDouble(name)));
		}

		public SQLTableInfo addBlobColumn(string name) {
			return(addColumn(capex.SQLTableColumnInfo.forBlob(name)));
		}

		public SQLTableInfo addIndex(string column) {
			if(cape.String.isEmpty(column) == false) {
				if(indexes == null) {
					indexes = new System.Collections.Generic.List<SQLTableColumnIndexInfo>();
				}
				indexes.Add(new SQLTableColumnIndexInfo().setColumn(column).setUnique(false));
			}
			return(this);
		}

		public SQLTableInfo addUniqueIndex(string column) {
			if(cape.String.isEmpty(column) == false) {
				if(indexes == null) {
					indexes = new System.Collections.Generic.List<SQLTableColumnIndexInfo>();
				}
				indexes.Add(new SQLTableColumnIndexInfo().setColumn(column).setUnique(true));
			}
			return(this);
		}

		public string getName() {
			return(name);
		}

		public SQLTableInfo setName(string v) {
			name = v;
			return(this);
		}

		public System.Collections.Generic.List<SQLTableColumnInfo> getColumns() {
			return(columns);
		}

		public SQLTableInfo setColumns(System.Collections.Generic.List<SQLTableColumnInfo> v) {
			columns = v;
			return(this);
		}

		public System.Collections.Generic.List<SQLTableColumnIndexInfo> getIndexes() {
			return(indexes);
		}

		public SQLTableInfo setIndexes(System.Collections.Generic.List<SQLTableColumnIndexInfo> v) {
			indexes = v;
			return(this);
		}
	}
}
