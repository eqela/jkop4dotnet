
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

namespace sympathy
{
	public class HTTPServerDirectoryHandler : HTTPServerRequestHandlerAdapter
	{
		public static HTTPServerDirectoryHandler forDirectory(cape.File dir) {
			var v = new HTTPServerDirectoryHandler();
			v.setDirectory(dir);
			return(v);
		}

		private bool listDirectories = false;
		private bool processTemplateFiles = false;
		private cape.File directory = null;
		private int maxAge = 300;
		private string serverURL = null;
		private System.Collections.Generic.List<string> indexFiles = null;
		private System.Collections.Generic.List<cape.File> templateIncludeDirs = null;
		private string serverName = null;
		private cape.DynamicMap templateData = null;

		public void addTemplateIncludeDir(cape.File dir) {
			if(dir == null) {
				return;
			}
			if(templateIncludeDirs == null) {
				templateIncludeDirs = new System.Collections.Generic.List<cape.File>();
			}
			templateIncludeDirs.Add(dir);
		}

		public HTTPServerDirectoryHandler setIndexFiles(string[] files) {
			indexFiles = new System.Collections.Generic.List<string>();
			if(files != null) {
				var n = 0;
				var m = files.Length;
				for(n = 0 ; n < m ; n++) {
					var file = files[n];
					if(file != null) {
						indexFiles.Add(file);
					}
				}
			}
			return(this);
		}

		public HTTPServerDirectoryHandler setServerName(string name) {
			this.serverName = name;
			return(this);
		}

		public string getServerName() {
			if(!(object.Equals(serverName, null))) {
				return(serverName);
			}
			var server = getServer();
			if(server == null) {
				return(null);
			}
			return(server.getServerName());
		}

		public void getDirectoryEntries(cape.File dir, System.Collections.Generic.List<string> allEntries, System.Collections.Generic.List<string> dirs, System.Collections.Generic.List<string> files) {
			if(dir == null) {
				return;
			}
			var entries = dir.entries();
			while(entries != null) {
				var entry = entries.next();
				if(entry == null) {
					break;
				}
				var name = entry.baseName();
				if((dirs != null) && entry.isDirectory()) {
					dirs.Add(name);
				}
				if((files != null) && entry.isFile()) {
					files.Add(name);
				}
				if(allEntries != null) {
					allEntries.Add(name);
				}
			}
		}

		public cape.DynamicMap dirToJSONObject(cape.File dir) {
			var allEntries = new System.Collections.Generic.List<string>();
			var dirs = new System.Collections.Generic.List<string>();
			var files = new System.Collections.Generic.List<string>();
			getDirectoryEntries(dir, allEntries, dirs, files);
			var v = new cape.DynamicMap();
			v.set("files", (object)cape.DynamicVector.forStringVector(files));
			v.set("dirs", (object)cape.DynamicVector.forStringVector(dirs));
			v.set("all", (object)cape.DynamicVector.forStringVector(allEntries));
			return(v);
		}

		public string dirToJSON(cape.File dir) {
			return(cape.JSONEncoder.encode((object)dirToJSONObject(dir)));
		}

		public virtual cape.DynamicMap getTemplateVariablesForFile(cape.File file) {
			return(null);
		}

		public string dirToHTML(cape.File dir) {
			if(dir == null) {
				return(null);
			}
			var sb = new cape.StringBuilder();
			sb.append("<html>\n");
			sb.append("<head>\n");
			sb.append("<title>");
			sb.append(dir.baseName());
			sb.append("</title>\n");
			sb.append("<style>\n");
			sb.append("* { font-face: arial; font-size: 12px; }\n");
			sb.append("h1 { font-face: arial; font-size: 14px; font-style: bold; border-bottom: solid 1px black; padding: 4px; margin: 4px}\n");
			sb.append("#content a { text-decoration: none; color: #000080; }\n");
			sb.append("#content a:hover { text-decoration: none; color: #FFFFFF; font-weight: bold; }\n");
			sb.append(".entry { padding: 4px; }\n");
			sb.append(".entry:hover { background-color: #AAAACC; }\n");
			sb.append(".dir { font-weight: bold; }\n");
			sb.append(".even { background-color: #DDDDDD; }\n");
			sb.append(".odd { background-color: #EEEEEE; }\n");
			sb.append("#footer { border-top: 1px solid black; padding: 4px; margin: 4px; font-size: 10px; text-align: right; }\n");
			sb.append("#footer a { font-size: 10px; text-decoration: none; color: #000000; }\n");
			sb.append("#footer a:hover { font-size: 10px; text-decoration: underline; color: #000000; }\n");
			sb.append("</style>\n");
			sb.append("<meta name=\"viewport\" content=\"initial-scale=1,maximum-scale=1\" />\n");
			sb.append("</head>\n");
			sb.append("<body>\n");
			sb.append("<h1>");
			sb.append(dir.baseName());
			sb.append("</h1>\n");
			sb.append("<div id=\"content\">\n");
			var dirs = new System.Collections.Generic.List<string>();
			var files = new System.Collections.Generic.List<string>();
			getDirectoryEntries(dir, null, dirs, files);
			var n = 0;
			if(dirs != null) {
				var n2 = 0;
				var m = dirs.Count;
				for(n2 = 0 ; n2 < m ; n2++) {
					var dn = dirs[n2];
					if(dn != null) {
						string cc = null;
						if((n % 2) == 0) {
							cc = "even";
						}
						else {
							cc = "odd";
						}
						sb.append("<a href=\"");
						sb.append(dn);
						sb.append("/\"><div class=\"entry dir ");
						sb.append(cc);
						sb.append("\">");
						sb.append(dn);
						sb.append("/</div></a>\n");
						n++;
					}
				}
			}
			if(files != null) {
				var n3 = 0;
				var m2 = files.Count;
				for(n3 = 0 ; n3 < m2 ; n3++) {
					var fn = files[n3];
					if(fn != null) {
						string cc1 = null;
						if((n % 2) == 0) {
							cc1 = "even";
						}
						else {
							cc1 = "odd";
						}
						sb.append("<a href=\"");
						sb.append(fn);
						sb.append("\"><div class=\"entry ");
						sb.append(cc1);
						sb.append("\">");
						sb.append(fn);
						sb.append("</div></a>\n");
						n++;
					}
				}
			}
			sb.append("</div>\n");
			sb.append("<div id=\"footer\">");
			var serverName = getServerName();
			if(cape.String.isEmpty(serverName) == false) {
				if(cape.String.isEmpty(serverURL) == false) {
					sb.append("Generated by <a href=\"");
					if(cape.String.contains(serverURL, "://") == false) {
						sb.append("http://");
					}
					sb.append(serverURL);
					sb.append("\">");
					sb.append(serverName);
					sb.append("</a>\n");
				}
				else {
					sb.append("Generated by ");
					sb.append(serverName);
					sb.append("\n");
				}
			}
			sb.append("</div>\n");
			sb.append("</body>\n");
			sb.append("</html>\n");
			return(sb.toString());
		}

		public override bool onGET(HTTPServerRequest req) {
			if(directory == null) {
				return(false);
			}
			var dd = directory;
			while(true) {
				var comp = req.popResource();
				if(object.Equals(comp, null)) {
					break;
				}
				dd = dd.entry(comp);
			}
			if(dd.isDirectory()) {
				if(indexFiles != null) {
					var n = 0;
					var m = indexFiles.Count;
					for(n = 0 ; n < m ; n++) {
						var indexFile = indexFiles[n];
						if(indexFile != null) {
							var ff = dd.entry(indexFile);
							if(ff.isFile()) {
								dd = ff;
								break;
							}
						}
					}
				}
			}
			if(dd.isDirectory()) {
				if(req.isForDirectory() == false) {
					req.sendRedirectAsDirectory();
					return(true);
				}
				if(listDirectories == false) {
					return(false);
				}
				req.sendHTMLString(dirToHTML(dd));
				return(true);
			}
			if((dd.exists() == false) && processTemplateFiles) {
				var bn = dd.baseName();
				var nf = dd.getParent().entry(bn + ".t");
				if(nf.isFile()) {
					dd = nf;
				}
				else {
					nf = dd.getParent().entry(bn + ".html.t");
					if(nf.isFile()) {
						dd = nf;
					}
				}
			}
			if(dd.isFile()) {
				HTTPServerResponse resp = null;
				if(processTemplateFiles) {
					var bn1 = dd.baseName();
					var isJSONTemplate = false;
					var isHTMLTemplate = false;
					var isCSSTemplate = false;
					if(cape.String.endsWith(bn1, ".html.t")) {
						isHTMLTemplate = true;
					}
					else if(cape.String.endsWith(bn1, ".css.t")) {
						isCSSTemplate = true;
					}
					else if(cape.String.endsWith(bn1, ".json.t")) {
						isJSONTemplate = true;
					}
					if((isHTMLTemplate || isCSSTemplate) || isJSONTemplate) {
						var data = dd.getContentsString("UTF-8");
						if(object.Equals(data, null)) {
							cape.Log.error(logContext, ("Failed to read template file content: `" + dd.getPath()) + "'");
							req.sendResponse(sympathy.HTTPServerResponse.forHTTPInternalError());
							return(true);
						}
						var includeDirs = new System.Collections.Generic.List<cape.File>();
						includeDirs.Add(dd.getParent());
						if(templateIncludeDirs != null) {
							var n2 = 0;
							var m2 = templateIncludeDirs.Count;
							for(n2 = 0 ; n2 < m2 ; n2++) {
								var dir = templateIncludeDirs[n2];
								if(dir != null) {
									includeDirs.Add(dir);
								}
							}
						}
						capex.TextTemplate tt = null;
						if(isHTMLTemplate || isCSSTemplate) {
							tt = capex.TextTemplate.forHTMLString(data, includeDirs, logContext);
						}
						else {
							tt = capex.TextTemplate.forJSONString(data, includeDirs, logContext);
						}
						if(tt == null) {
							cape.Log.error(logContext, ("Failed to process template file content: `" + dd.getPath()) + "'");
							req.sendResponse(sympathy.HTTPServerResponse.forHTTPInternalError());
							return(true);
						}
						var tdata = templateData;
						var dynamicData = getTemplateVariablesForFile(dd);
						if(dynamicData != null) {
							if(tdata == null) {
								tdata = dynamicData;
							}
							else {
								tdata.mergeFrom(dynamicData);
							}
						}
						var text = tt.execute(tdata, includeDirs);
						if(object.Equals(text, null)) {
							cape.Log.error(logContext, ("Failed to execute template: `" + dd.getPath()) + "'");
							req.sendResponse(sympathy.HTTPServerResponse.forHTTPInternalError());
							return(true);
						}
						if(isHTMLTemplate) {
							resp = sympathy.HTTPServerResponse.forHTMLString(text);
						}
						else if(isCSSTemplate) {
							resp = sympathy.HTTPServerResponse.forString(text, "text/css");
						}
						else {
							resp = sympathy.HTTPServerResponse.forJSONString(text);
						}
					}
				}
				if(resp == null) {
					resp = sympathy.HTTPServerResponse.forFile(dd);
				}
				if(maxAge > 0) {
					resp.addHeader("Cache-Control", "max-age=" + cape.String.forInteger(maxAge));
				}
				req.sendResponse(resp);
				return(true);
			}
			return(false);
		}

		public bool getListDirectories() {
			return(listDirectories);
		}

		public HTTPServerDirectoryHandler setListDirectories(bool v) {
			listDirectories = v;
			return(this);
		}

		public bool getProcessTemplateFiles() {
			return(processTemplateFiles);
		}

		public HTTPServerDirectoryHandler setProcessTemplateFiles(bool v) {
			processTemplateFiles = v;
			return(this);
		}

		public cape.File getDirectory() {
			return(directory);
		}

		public HTTPServerDirectoryHandler setDirectory(cape.File v) {
			directory = v;
			return(this);
		}

		public int getMaxAge() {
			return(maxAge);
		}

		public HTTPServerDirectoryHandler setMaxAge(int v) {
			maxAge = v;
			return(this);
		}

		public string getServerURL() {
			return(serverURL);
		}

		public HTTPServerDirectoryHandler setServerURL(string v) {
			serverURL = v;
			return(this);
		}

		public System.Collections.Generic.List<string> getIndexFiles() {
			return(indexFiles);
		}

		public System.Collections.Generic.List<cape.File> getTemplateIncludeDirs() {
			return(templateIncludeDirs);
		}

		public HTTPServerDirectoryHandler setTemplateIncludeDirs(System.Collections.Generic.List<cape.File> v) {
			templateIncludeDirs = v;
			return(this);
		}

		public cape.DynamicMap getTemplateData() {
			return(templateData);
		}

		public HTTPServerDirectoryHandler setTemplateData(cape.DynamicMap v) {
			templateData = v;
			return(this);
		}
	}
}
