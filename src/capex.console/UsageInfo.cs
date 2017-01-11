
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

namespace capex.console
{
	public class UsageInfo
	{
		public static UsageInfo forCommand(string command) {
			var v = new UsageInfo();
			v.setCommand(command);
			return(v);
		}

		private class Parameter
		{
			public string name = null;
			public string description = null;
		}

		private class Flag
		{
			public string flag = null;
			public string description = null;
		}

		private class Option
		{
			public string name = null;
			public string value = null;
			public string description = null;
		}

		private string command = null;
		private string description = null;
		private string paramDesc = null;
		private System.Collections.Generic.List<object> data = null;

		public UsageInfo() {
			data = new System.Collections.Generic.List<object>();
			paramDesc = "[parameters]";
			command = "(command)";
		}

		private void ensureSection() {
			if(cape.Vector.getSize(data) < 1) {
				addSection("Available parameters");
			}
		}

		public void addSection(string name) {
			data.Add(name);
		}

		public void addParameter(string name, string description) {
			ensureSection();
			var p = new Parameter();
			p.name = name;
			p.description = description;
			data.Add(p);
		}

		public void addFlag(string flag, string description) {
			ensureSection();
			var f = new Flag();
			f.flag = flag;
			f.description = description;
			data.Add(f);
		}

		public void addOption(string name, string value, string description) {
			ensureSection();
			var o = new Option();
			o.name = name;
			o.value = value;
			o.description = description;
			data.Add(o);
		}

		public string toString() {
			var sb = new cape.StringBuilder();
			sb.append("Usage: ");
			sb.append(command);
			if(cape.String.isEmpty(paramDesc) == false) {
				sb.append(' ');
				sb.append(paramDesc);
			}
			sb.append('\n');
			sb.append('\n');
			if(cape.String.isEmpty(description) == false) {
				sb.append(description);
				sb.append('\n');
				sb.append('\n');
			}
			var longest = 0;
			var db = true;
			if(data != null) {
				var n = 0;
				var m = data.Count;
				for(n = 0 ; n < m ; n++) {
					var o = data[n];
					if(o != null) {
						if(o is Parameter) {
							var nn = ((Parameter)o).name;
							if(!(object.Equals(nn, null))) {
								var ll = cape.String.getLength(nn);
								if(ll > longest) {
									longest = ll;
								}
							}
						}
						else if(o is Flag) {
							var ff = ((Flag)o).flag;
							if(!(object.Equals(ff, null))) {
								var ll1 = cape.String.getLength(ff) + 1;
								if(ll1 > longest) {
									longest = ll1;
								}
							}
						}
						else if(o is Option) {
							var name = ((Option)o).name;
							var value = ((Option)o).value;
							var ll2 = (((1 + cape.String.getLength(name)) + 2) + cape.String.getLength(value)) + 1;
							if(ll2 > longest) {
								longest = ll2;
							}
						}
					}
				}
			}
			if(longest < 30) {
				longest = 30;
			}
			if(data != null) {
				var n2 = 0;
				var m2 = data.Count;
				for(n2 = 0 ; n2 < m2 ; n2++) {
					var o1 = data[n2];
					if(o1 != null) {
						if((o1 is string) || (o1 is cape.StringObject)) {
							if(db == false) {
								sb.append('\n');
							}
							sb.append(cape.String.asString(o1));
							sb.append(':');
							sb.append('\n');
							sb.append('\n');
							db = true;
						}
						else if(o1 is Parameter) {
							var p = (Parameter)o1;
							sb.append("  ");
							sb.append(cape.String.padToLength(p.name, longest, -1, ' '));
							if(cape.String.isEmpty(p.description) == false) {
								sb.append(" - ");
								sb.append(p.description);
							}
							sb.append('\n');
							db = false;
						}
						else if(o1 is Flag) {
							var p1 = (Flag)o1;
							sb.append("  -");
							sb.append(cape.String.padToLength(p1.flag, longest - 1, -1, ' '));
							if(cape.String.isEmpty(p1.description) == false) {
								sb.append(" - ");
								sb.append(p1.description);
							}
							sb.append('\n');
							db = false;
						}
						else if(o1 is Option) {
							var p2 = (Option)o1;
							sb.append("  ");
							sb.append(cape.String.padToLength(((("-" + p2.name) + "=[") + p2.value) + "]", longest, -1, ' '));
							if(cape.String.isEmpty(p2.description) == false) {
								sb.append(" - ");
								sb.append(p2.description);
							}
							sb.append('\n');
							db = false;
						}
					}
				}
			}
			return(sb.toString());
		}

		public string getCommand() {
			return(command);
		}

		public UsageInfo setCommand(string v) {
			command = v;
			return(this);
		}

		public string getDescription() {
			return(description);
		}

		public UsageInfo setDescription(string v) {
			description = v;
			return(this);
		}

		public string getParamDesc() {
			return(paramDesc);
		}

		public UsageInfo setParamDesc(string v) {
			paramDesc = v;
			return(this);
		}
	}
}
