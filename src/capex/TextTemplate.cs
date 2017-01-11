
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
	public class TextTemplate
	{
		private class TagData
		{
			public System.Collections.Generic.List<string> words = null;

			public TagData(System.Collections.Generic.List<string> words) {
				this.words = words;
			}
		}

		public static TextTemplate forFile(cape.File file, string markerBegin, string markerEnd, int type = 0, System.Collections.Generic.List<cape.File> includeDirs = null, cape.LoggingContext logContext = null) {
			if(file == null) {
				return(null);
			}
			var text = file.getContentsString("UTF-8");
			if(object.Equals(text, null)) {
				return(null);
			}
			var ids = includeDirs;
			if(ids == null) {
				ids = new System.Collections.Generic.List<cape.File>();
				ids.Add(file.getParent());
			}
			return(forString(text, markerBegin, markerEnd, type, ids, logContext));
		}

		public static TextTemplate forString(string text, string markerBegin, string markerEnd, int type = 0, System.Collections.Generic.List<cape.File> includeDirs = null, cape.LoggingContext logContext = null) {
			var v = new TextTemplate();
			v.setLogContext(logContext);
			v.setText(text);
			v.setType(type);
			v.setMarkerBegin(markerBegin);
			v.setMarkerEnd(markerEnd);
			if(v.prepare(includeDirs) == false) {
				v = null;
			}
			return(v);
		}

		public static TextTemplate forHTMLString(string text, System.Collections.Generic.List<cape.File> includeDirs = null, cape.LoggingContext logContext = null) {
			var v = new TextTemplate();
			v.setLogContext(logContext);
			v.setText(text);
			v.setType(TYPE_HTML);
			v.setMarkerBegin("<%");
			v.setMarkerEnd("%>");
			if(v.prepare(includeDirs) == false) {
				v = null;
			}
			return(v);
		}

		public static TextTemplate forJSONString(string text, System.Collections.Generic.List<cape.File> includeDirs = null, cape.LoggingContext logContext = null) {
			var v = new TextTemplate();
			v.setLogContext(logContext);
			v.setText(text);
			v.setType(TYPE_JSON);
			v.setMarkerBegin("{{");
			v.setMarkerEnd("}}");
			if(v.prepare(includeDirs) == false) {
				v = null;
			}
			return(v);
		}

		public const int TYPE_GENERIC = 0;
		public const int TYPE_HTML = 1;
		public const int TYPE_JSON = 2;
		private System.Collections.Generic.List<object> tokens = null;
		private string text = null;
		private string markerBegin = null;
		private string markerEnd = null;
		private cape.LoggingContext logContext = null;
		private int type = TYPE_GENERIC;
		private System.Collections.Generic.List<string> languagePreferences = null;

		public void setLanguagePreference(string language) {
			languagePreferences = new System.Collections.Generic.List<string>();
			if(!(object.Equals(language, null))) {
				languagePreferences.Add(language);
			}
		}

		private System.Collections.Generic.List<object> tokenizeString(string inputdata, System.Collections.Generic.List<cape.File> includeDirs) {
			if((object.Equals(markerBegin, null)) || (object.Equals(markerEnd, null))) {
				cape.Log.error(logContext, "No template markers were given");
				return(null);
			}
			if((cape.String.getLength(markerBegin) != 2) || (cape.String.getLength(markerEnd) != 2)) {
				cape.Log.error(logContext, ((("Invalid template markers: `" + markerBegin) + "' and `") + markerEnd) + "'");
				return(null);
			}
			var mb1 = cape.String.charAt(markerBegin, 0);
			var mb2 = cape.String.charAt(markerBegin, 1);
			var me1 = cape.String.charAt(markerEnd, 0);
			var me2 = cape.String.charAt(markerEnd, 1);
			var pc = ' ';
			cape.StringBuilder tag = null;
			cape.StringBuilder data = null;
			var it = cape.String.iterate(inputdata);
			var v = new System.Collections.Generic.List<object>();
			while(it != null) {
				var c = it.getNextChar();
				if(c <= 0) {
					break;
				}
				if(tag != null) {
					if((pc == me1) && (tag.count() > 2)) {
						tag.append(pc);
						tag.append(c);
						if(c == me2) {
							var tt = tag.toString();
							var tts = cape.String.strip(cape.String.subString(tt, 2, cape.String.getLength(tt) - 4));
							var words = cape.String.quotedStringToVector(tts, ' ');
							if(object.Equals(cape.Vector.get(words, 0), "include")) {
								var fileName = cape.Vector.get(words, 1);
								if(cape.String.isEmpty(fileName)) {
									cape.Log.warning(logContext, "Include tag with empty filename. Ignoring it.");
								}
								else {
									cape.File ff = null;
									if(cape.Environment.isAbsolutePath(fileName)) {
										ff = cape.FileInstance.forPath(fileName);
									}
									else if(includeDirs != null) {
										var n = 0;
										var m = includeDirs.Count;
										for(n = 0 ; n < m ; n++) {
											var includeDir = includeDirs[n];
											if(includeDir != null) {
												var x = cape.FileInstance.forRelativePath(fileName, includeDir);
												if(x.isFile()) {
													ff = x;
													break;
												}
											}
										}
									}
									if((ff == null) || (ff.isFile() == false)) {
										cape.Log.warning(logContext, ("Included file was not found: `" + fileName) + "'");
									}
									else {
										var cc = ff.getContentsString("UTF-8");
										if(object.Equals(cc, null)) {
											cape.Log.warning(logContext, ("Failed to read included file: `" + ff.getPath()) + "'");
										}
										else {
											var nt = forString(cc, markerBegin, markerEnd, type, includeDirs);
											if(nt == null) {
												cape.Log.warning(logContext, ("Failed to read included template file: `" + ff.getPath()) + "'");
											}
											else {
												var array = nt.getTokens();
												if(array != null) {
													var n2 = 0;
													var m2 = array.Count;
													for(n2 = 0 ; n2 < m2 ; n2++) {
														var token = array[n2];
														if(token != null) {
															v.Add(token);
														}
													}
												}
											}
										}
									}
								}
							}
							else {
								v.Add(new TagData(words));
							}
							tag = null;
						}
					}
					else if(c != me1) {
						tag.append(c);
					}
				}
				else if(pc == mb1) {
					if(c == mb2) {
						if(data != null) {
							v.Add(data);
							data = null;
						}
						tag = new cape.StringBuilder();
						tag.append(pc);
						tag.append(c);
					}
					else {
						if(data == null) {
							data = new cape.StringBuilder();
						}
						data.append(pc);
						data.append(c);
					}
				}
				else if(c != mb1) {
					if(data == null) {
						data = new cape.StringBuilder();
					}
					data.append(c);
				}
				pc = c;
			}
			if(pc == mb1) {
				if(data == null) {
					data = new cape.StringBuilder();
				}
				data.append(pc);
			}
			if(data != null) {
				v.Add(data);
				data = null;
			}
			if(tag != null) {
				cape.Log.error(logContext, ("Unfinished tag: `" + tag.toString()) + "'");
				return(null);
			}
			return(v);
		}

		public bool prepare(System.Collections.Generic.List<cape.File> includeDirs) {
			var str = text;
			if(object.Equals(str, null)) {
				cape.Log.error(logContext, "No input string was specified.");
				return(false);
			}
			tokens = tokenizeString(str, includeDirs);
			if(tokens == null) {
				return(false);
			}
			return(true);
		}

		public string execute(cape.DynamicMap vars, System.Collections.Generic.List<cape.File> includeDirs = null) {
			if(tokens == null) {
				cape.Log.error(logContext, "TextTemplate has not been prepared");
				return(null);
			}
			var result = new cape.StringBuilder();
			if(doExecute(tokens, vars, result, includeDirs) == false) {
				return(null);
			}
			return(result.toString());
		}

		private string substituteVariables(string orig, cape.DynamicMap vars) {
			if(object.Equals(orig, null)) {
				return(orig);
			}
			if(cape.String.indexOf(orig, "${") < 0) {
				return(orig);
			}
			var sb = new cape.StringBuilder();
			cape.StringBuilder varbuf = null;
			var flag = false;
			var it = cape.String.iterate(orig);
			while(it != null) {
				var c = it.getNextChar();
				if(c <= 0) {
					break;
				}
				if(varbuf != null) {
					if(c == '}') {
						var varname = varbuf.toString();
						if(vars != null) {
							string varcut = null;
							if(cape.String.indexOf(varname, '!') > 0) {
								cape.Iterator<string> sp = cape.Vector.iterate(cape.String.split(varname, '!', 2));
								varname = sp.next();
								varcut = sp.next();
							}
							var r = getVariableValueString(vars, varname);
							if(cape.String.isEmpty(varcut) == false) {
								var itc = cape.String.iterate(varcut);
								var cx = ' ';
								while((cx = itc.getNextChar()) > 0) {
									var n = cape.String.lastIndexOf(r, cx);
									if(n < 0) {
										break;
									}
									r = cape.String.subString(r, 0, n);
								}
							}
							sb.append(r);
						}
						varbuf = null;
					}
					else {
						varbuf.append(c);
					}
					continue;
				}
				if(flag == true) {
					flag = false;
					if(c == '{') {
						varbuf = new cape.StringBuilder();
					}
					else {
						sb.append('$');
						sb.append(c);
					}
					continue;
				}
				if(c == '$') {
					flag = true;
					continue;
				}
				sb.append(c);
			}
			return(sb.toString());
		}

		public object getVariableValue(cape.DynamicMap vars, string varname) {
			if((vars == null) || (object.Equals(varname, null))) {
				return(null);
			}
			var vv = vars.get(varname);
			if(vv != null) {
				return(vv);
			}
			var ll = cape.String.split(varname, '.');
			if(cape.Vector.getSize(ll) < 2) {
				return(null);
			}
			var nvar = cape.Vector.get(ll, cape.Vector.getSize(ll) - 1) as string;
			cape.Vector.removeLast(ll);
			var cc = vars;
			if(ll != null) {
				var n = 0;
				var m = ll.Count;
				for(n = 0 ; n < m ; n++) {
					var vv2 = ll[n];
					if(vv2 != null) {
						if(cc == null) {
							return(null);
						}
						var vv2o = cc.get(vv2);
						cc = vv2o as cape.DynamicMap;
						if(((cc == null) && (vv2o != null)) && (vv2o is cape.JSONObject)) {
							cc = ((cape.JSONObject)vv2o).toJsonObject() as cape.DynamicMap;
						}
					}
				}
			}
			if(cc != null) {
				return(cc.get(nvar));
			}
			return(null);
		}

		public string getVariableValueString(cape.DynamicMap vars, string varname) {
			var v = getVariableValue(vars, varname);
			if(v == null) {
				return(null);
			}
			if(v is cape.DynamicMap) {
				if(languagePreferences != null) {
					if(languagePreferences != null) {
						var n = 0;
						var m = languagePreferences.Count;
						for(n = 0 ; n < m ; n++) {
							var language = languagePreferences[n];
							if(language != null) {
								var s = ((cape.DynamicMap)v).getString(language);
								if(!(object.Equals(s, null))) {
									return(s);
								}
							}
						}
					}
				}
				else {
					var s1 = ((cape.DynamicMap)v).getString("en");
					if(!(object.Equals(s1, null))) {
						return(s1);
					}
				}
				return(null);
			}
			return(cape.String.asString(v));
		}

		public System.Collections.Generic.List<object> getVariableValueVector(cape.DynamicMap vars, string varname) {
			var v = getVariableValue(vars, varname);
			if(v == null) {
				return(null);
			}
			if(v is System.Collections.Generic.List<object>) {
				return((System.Collections.Generic.List<object>)v);
			}
			if(v is cape.VectorObject<object>) {
				var vo = (cape.VectorObject<object>)v;
				System.Collections.Generic.List<object> vv = vo.toVector();
				return(vv);
			}
			if(v is cape.ArrayObject<object>) {
				var vo1 = v as cape.ArrayObject<object>;
				object[] vv1 = vo1.toArray();
				System.Collections.Generic.List<object> vvx = cape.Vector.forArray(vv1);
				return(vvx);
			}
			if(v is cape.DynamicVector) {
				return(((cape.DynamicVector)v).toVector());
			}
			return(null);
		}

		public bool handleBlock(cape.DynamicMap vars, System.Collections.Generic.List<string> tag, System.Collections.Generic.List<object> data, cape.StringBuilder result, System.Collections.Generic.List<cape.File> includeDirs) {
			if(tag == null) {
				return(false);
			}
			var tagname = cape.Vector.get(tag, 0);
			if(cape.String.isEmpty(tagname)) {
				return(false);
			}
			if(object.Equals(tagname, "for")) {
				var varname = cape.Vector.get(tag, 1);
				var inword = cape.Vector.get(tag, 2);
				var origvar = substituteVariables(cape.Vector.get(tag, 3), vars);
				if((cape.String.isEmpty(varname) || cape.String.isEmpty(origvar)) || !(object.Equals(inword, "in"))) {
					cape.Log.error(logContext, ("Invalid for tag: `" + cape.String.combine(tag, ' ')) + "'");
					return(false);
				}
				var index = 0;
				vars.set("__for_first", (object)"true");
				var vv = getVariableValueVector(vars, origvar);
				if(vv != null) {
					var n = 0;
					var m = vv.Count;
					for(n = 0 ; n < m ; n++) {
						var o = vv[n];
						if(o != null) {
							if((index % 2) == 0) {
								vars.set("__for_parity", (object)"even");
							}
							else {
								vars.set("__for_parity", (object)"odd");
							}
							vars.set(varname, o);
							if(doExecute(data, vars, result, includeDirs) == false) {
								return(false);
							}
							if(index == 0) {
								vars.set("__for_first", (object)"false");
							}
							index++;
						}
					}
				}
				vars.remove("__for_first");
				vars.remove("__for_parity");
				return(true);
			}
			if(object.Equals(tagname, "if")) {
				var lvalue = cape.Vector.get(tag, 1);
				if(object.Equals(lvalue, null)) {
					return(true);
				}
				var @operator = cape.Vector.get(tag, 2);
				if(object.Equals(@operator, null)) {
					var varval = getVariableValue(vars, lvalue);
					if(varval == null) {
						return(true);
					}
					if(varval is cape.VectorObject<object>) {
						if(cape.Vector.isEmpty(((cape.VectorObject<object>)varval).toVector())) {
							return(true);
						}
					}
					if(varval is cape.DynamicMap) {
						var value = (cape.DynamicMap)varval;
						if(value.getCount() < 1) {
							return(true);
						}
					}
					if(varval is string) {
						if(cape.String.isEmpty((string)varval)) {
							return(true);
						}
					}
					if(varval is cape.StringObject) {
						if(cape.String.isEmpty(((cape.StringObject)varval).toString())) {
							return(true);
						}
					}
					if(doExecute(data, vars, result, includeDirs) == false) {
						return(false);
					}
					return(true);
				}
				lvalue = substituteVariables(lvalue, vars);
				var rvalue = cape.Vector.get(tag, 3);
				if(!(object.Equals(rvalue, null))) {
					rvalue = substituteVariables(rvalue, vars);
				}
				if(((object.Equals(lvalue, null)) || cape.String.isEmpty(@operator)) || (object.Equals(rvalue, null))) {
					cape.Log.error(logContext, ("Invalid if tag: `" + cape.String.combine(tag, ' ')) + "'");
					return(false);
				}
				if(((object.Equals(@operator, "==")) || (object.Equals(@operator, "="))) || (object.Equals(@operator, "is"))) {
					if(!(object.Equals(rvalue, lvalue))) {
						return(true);
					}
					if(doExecute(data, vars, result, includeDirs) == false) {
						return(false);
					}
					return(true);
				}
				if((object.Equals(@operator, "!=")) || (object.Equals(@operator, "not"))) {
					if(object.Equals(rvalue, lvalue)) {
						return(true);
					}
					if(doExecute(data, vars, result, includeDirs) == false) {
						return(false);
					}
					return(true);
				}
				cape.Log.error(logContext, ((("Unknown operator `" + @operator) + "' in if tag: `") + cape.String.combine(tag, ' ')) + "'");
				return(false);
			}
			return(false);
		}

		private bool doExecute(System.Collections.Generic.List<object> data, cape.DynamicMap avars, cape.StringBuilder result, System.Collections.Generic.List<cape.File> includeDirs) {
			if(data == null) {
				return(true);
			}
			var blockctr = 0;
			System.Collections.Generic.List<object> blockdata = null;
			System.Collections.Generic.List<string> blocktag = null;
			var vars = avars;
			if(vars == null) {
				vars = new cape.DynamicMap();
			}
			if(data != null) {
				var n2 = 0;
				var m = data.Count;
				for(n2 = 0 ; n2 < m ; n2++) {
					var o = data[n2];
					if(o != null) {
						string tagname = null;
						System.Collections.Generic.List<string> words = null;
						var tagData = o as TagData;
						if(tagData != null) {
							words = tagData.words;
							if(words != null) {
								tagname = cape.Vector.get(words, 0);
								if(cape.String.isEmpty(tagname)) {
									cape.Log.warning(logContext, "Empty tag encountered. Ignoring it.");
									continue;
								}
							}
						}
						if(object.Equals(tagname, "end")) {
							blockctr--;
							if((blockctr == 0) && (blockdata != null)) {
								if(handleBlock(vars, blocktag, blockdata, result, includeDirs) == false) {
									cape.Log.error(logContext, "Handling of a block failed");
									continue;
								}
								blockdata = null;
								blocktag = null;
							}
						}
						if(blockctr > 0) {
							if((object.Equals(tagname, "for")) || (object.Equals(tagname, "if"))) {
								blockctr++;
							}
							if(blockdata == null) {
								blockdata = new System.Collections.Generic.List<object>();
							}
							blockdata.Add(o);
							continue;
						}
						if((o is string) || (o is cape.StringObject)) {
							result.append(cape.String.asString(o));
							continue;
						}
						if((object.Equals(tagname, "=")) || (object.Equals(tagname, "printstring"))) {
							var varname = substituteVariables(cape.Vector.get(words, 1), vars);
							if(cape.String.isEmpty(varname) == false) {
								var vv = getVariableValueString(vars, varname);
								if(cape.String.isEmpty(vv) == false) {
									if(this.type == TYPE_HTML) {
										vv = capex.HTMLString.sanitize(vv);
									}
									result.append(vv);
								}
								else {
									var defaultvalue = substituteVariables(cape.Vector.get(words, 2), vars);
									if(cape.String.isEmpty(defaultvalue) == false) {
										if(this.type == TYPE_HTML) {
											defaultvalue = capex.HTMLString.sanitize(defaultvalue);
										}
										result.append(defaultvalue);
									}
								}
							}
						}
						else if(object.Equals(tagname, "printRaw")) {
							var varname1 = substituteVariables(cape.Vector.get(words, 1), vars);
							if(cape.String.isEmpty(varname1) == false) {
								var vv1 = getVariableValueString(vars, varname1);
								if(cape.String.isEmpty(vv1) == false) {
									result.append(vv1);
								}
								else {
									var defaultvalue1 = substituteVariables(cape.Vector.get(words, 2), vars);
									if(cape.String.isEmpty(defaultvalue1) == false) {
										result.append(defaultvalue1);
									}
								}
							}
						}
						else if(object.Equals(tagname, "catPath")) {
							var hasSlash = false;
							var n = 0;
							if(words != null) {
								var n3 = 0;
								var m2 = words.Count;
								for(n3 = 0 ; n3 < m2 ; n3++) {
									var word = words[n3];
									if(word != null) {
										n++;
										if(n == 1) {
											continue;
										}
										word = substituteVariables(word, vars);
										var it = cape.String.iterate(word);
										if(it == null) {
											continue;
										}
										if((n > 2) && (hasSlash == false)) {
											result.append('/');
											hasSlash = true;
										}
										while(true) {
											var c = it.getNextChar();
											if(c < 1) {
												break;
											}
											if(c == '/') {
												if(hasSlash == false) {
													result.append(c);
													hasSlash = true;
												}
											}
											else {
												result.append(c);
												hasSlash = false;
											}
										}
									}
								}
							}
						}
						else if(object.Equals(tagname, "printJson")) {
							var varname2 = substituteVariables(cape.Vector.get(words, 1), vars);
							if(cape.String.isEmpty(varname2) == false) {
								var vv2 = getVariableValue(vars, varname2);
								if(vv2 != null) {
									result.append(cape.JSONEncoder.encode(vv2));
								}
							}
						}
						else if(object.Equals(tagname, "printRichText")) {
							var markup = substituteVariables(cape.Vector.get(words, 1), vars);
							if(cape.String.isEmpty(markup) == false) {
								var doc = capex.RichTextDocument.forWikiMarkupString(markup);
								if(doc != null) {
									result.append(doc.toHtml(null));
								}
							}
						}
						else if(object.Equals(tagname, "printDate")) {
							var timestamp = substituteVariables(cape.Vector.get(words, 1), vars);
							var aslong = cape.String.toLong(timestamp);
							var asstring = cape.DateTime.forTimeSeconds(aslong).toStringDate('/');
							result.append(asstring);
						}
						else if(object.Equals(tagname, "printTime")) {
							var timestamp1 = substituteVariables(cape.Vector.get(words, 1), vars);
							var aslong1 = cape.String.toLong(timestamp1);
							var asstring1 = cape.DateTime.forTimeSeconds(aslong1).toStringTime(':');
							result.append(asstring1);
						}
						else if(object.Equals(tagname, "printDateTime")) {
							var timestamp2 = substituteVariables(cape.Vector.get(words, 1), vars);
							var aslong2 = cape.String.toLong(timestamp2);
							var dt = cape.DateTime.forTimeSeconds(aslong2);
							result.append(dt.toStringDate('/'));
							result.append(' ');
							result.append(dt.toStringTime(':'));
						}
						else if(object.Equals(tagname, "import")) {
							var type = cape.Vector.get(words, 1);
							var filename = substituteVariables(cape.Vector.get(words, 2), vars);
							if(cape.String.isEmpty(filename)) {
								cape.Log.warning(logContext, "Invalid import tag with empty filename");
								continue;
							}
							cape.File ff = null;
							if(includeDirs != null) {
								var n4 = 0;
								var m3 = includeDirs.Count;
								for(n4 = 0 ; n4 < m3 ; n4++) {
									var dir = includeDirs[n4];
									if(dir != null) {
										ff = cape.FileInstance.forRelativePath(filename, dir);
										if((ff != null) && ff.isFile()) {
											break;
										}
									}
								}
							}
							if((ff == null) || (ff.isFile() == false)) {
								cape.Log.error(logContext, ("Unable to find file to import: `" + filename) + "'");
								continue;
							}
							cape.Log.debug(logContext, ("Attempting to import file: `" + ff.getPath()) + "'");
							var content = ff.getContentsString("UTF-8");
							if(cape.String.isEmpty(content)) {
								cape.Log.error(logContext, ("Unable to read import file: `" + ff.getPath()) + "'");
								continue;
							}
							if(object.Equals(type, "html")) {
								content = capex.HTMLString.sanitize(content);
							}
							else if(object.Equals(type, "template")) {
								var t = forString(content, markerBegin, markerEnd, this.type, includeDirs);
								if(t == null) {
									cape.Log.error(logContext, ("Failed to parse imported template file: `" + ff.getPath()) + "'");
									continue;
								}
								if(doExecute(t.getTokens(), vars, result, includeDirs) == false) {
									cape.Log.error(logContext, ("Failed to process imported template file: `" + ff.getPath()) + "'");
									continue;
								}
								content = null;
							}
							else if(object.Equals(type, "raw")) {
								;
							}
							else {
								cape.Log.error(logContext, ("Unknown type for import: `" + type) + "'. Ignoring the import.");
								continue;
							}
							if(cape.String.isEmpty(content) == false) {
								result.append(content);
							}
						}
						else if(object.Equals(tagname, "escapeHtml")) {
							var content1 = substituteVariables(cape.Vector.get(words, 1), vars);
							if(cape.String.isEmpty(content1) == false) {
								content1 = capex.HTMLString.sanitize(content1);
								if(cape.String.isEmpty(content1) == false) {
									result.append(content1);
								}
							}
						}
						else if(object.Equals(tagname, "set")) {
							if(cape.Vector.getSize(words) != 3) {
								cape.Log.warning(logContext, "Invalid number of parameters for set tag encountered: " + cape.String.forInteger(cape.Vector.getSize(words)));
								continue;
							}
							var varname3 = substituteVariables(cape.Vector.get(words, 1), vars);
							if(cape.String.isEmpty(varname3)) {
								cape.Log.warning(logContext, "Empty variable name in set tag encountered.");
								continue;
							}
							var newValue = substituteVariables(cape.Vector.get(words, 2), vars);
							vars.set(varname3, (object)newValue);
						}
						else if(object.Equals(tagname, "assign")) {
							if(cape.Vector.getSize(words) != 3) {
								cape.Log.warning(logContext, "Invalid number of parameters for assign tag encountered: " + cape.String.forInteger(cape.Vector.getSize(words)));
								continue;
							}
							var varname4 = substituteVariables(cape.Vector.get(words, 1), vars);
							if(cape.String.isEmpty(varname4)) {
								cape.Log.warning(logContext, "Empty variable name in assign tag encountered.");
								continue;
							}
							var vv3 = cape.Vector.get(words, 2);
							if(object.Equals(vv3, "none")) {
								vars.remove(varname4);
							}
							else {
								vars.set(varname4, getVariableValue(vars, vv3));
							}
						}
						else if((object.Equals(tagname, "for")) || (object.Equals(tagname, "if"))) {
							if(blockctr == 0) {
								blocktag = words;
							}
							blockctr++;
						}
						else if(object.Equals(tagname, "end")) {
							;
						}
						else if(type == TYPE_HTML) {
							onHTMLTag(vars, result, includeDirs, tagname, words);
						}
						else if(type == TYPE_JSON) {
							onJSONTag(vars, result, includeDirs, tagname, words);
						}
						else {
							;
						}
					}
				}
			}
			return(true);
		}

		private string basename(string path) {
			if(object.Equals(path, null)) {
				return(null);
			}
			var v = "";
			cape.Iterator<string> comps = cape.Vector.iterate(cape.String.split(path, '/'));
			if(comps != null) {
				var comp = comps.next();
				while(!(object.Equals(comp, null))) {
					if(cape.String.getLength(comp) > 0) {
						v = comp;
					}
					comp = comps.next();
				}
			}
			return(v);
		}

		private void onHTMLTag(cape.DynamicMap vars, cape.StringBuilder result, System.Collections.Generic.List<cape.File> includeDirs, string tagname, System.Collections.Generic.List<string> words) {
			if(object.Equals(tagname, "link")) {
				var path = substituteVariables(cape.Vector.get(words, 1), vars);
				var text = substituteVariables(cape.Vector.get(words, 2), vars);
				if(cape.String.isEmpty(text)) {
					text = basename(path);
				}
				if(cape.String.isEmpty(text)) {
					return;
				}
				result.append(((("<a href=\"" + path) + "\"><span>") + text) + "</span></a>");
				return;
			}
		}

		public void encodeJSONString(string s, cape.StringBuilder sb) {
			if(object.Equals(s, null)) {
				return;
			}
			var it = cape.String.iterate(s);
			if(it == null) {
				return;
			}
			var c = ' ';
			while((c = it.getNextChar()) > 0) {
				if(c == '\"') {
					sb.append('\\');
				}
				else if(c == '\\') {
					sb.append('\\');
				}
				sb.append(c);
			}
		}

		private void onJSONTag(cape.DynamicMap vars, cape.StringBuilder result, System.Collections.Generic.List<cape.File> includeDirs, string tagname, System.Collections.Generic.List<string> words) {
			if(object.Equals(tagname, "quotedstring")) {
				var @string = substituteVariables(cape.Vector.get(words, 1), vars);
				encodeJSONString(@string, result);
			}
		}

		public System.Collections.Generic.List<object> getTokens() {
			return(tokens);
		}

		public TextTemplate setTokens(System.Collections.Generic.List<object> v) {
			tokens = v;
			return(this);
		}

		public string getText() {
			return(text);
		}

		public TextTemplate setText(string v) {
			text = v;
			return(this);
		}

		public string getMarkerBegin() {
			return(markerBegin);
		}

		public TextTemplate setMarkerBegin(string v) {
			markerBegin = v;
			return(this);
		}

		public string getMarkerEnd() {
			return(markerEnd);
		}

		public TextTemplate setMarkerEnd(string v) {
			markerEnd = v;
			return(this);
		}

		public cape.LoggingContext getLogContext() {
			return(logContext);
		}

		public TextTemplate setLogContext(cape.LoggingContext v) {
			logContext = v;
			return(this);
		}

		public int getType() {
			return(type);
		}

		public TextTemplate setType(int v) {
			type = v;
			return(this);
		}

		public System.Collections.Generic.List<string> getLanguagePreferences() {
			return(languagePreferences);
		}

		public TextTemplate setLanguagePreferences(System.Collections.Generic.List<string> v) {
			languagePreferences = v;
			return(this);
		}
	}
}
