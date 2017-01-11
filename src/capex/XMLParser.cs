
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
	public class XMLParser
	{
		public static cape.DynamicMap parseAsTreeObject(string xml, bool ignoreWhiteSpace = true) {
			cape.DynamicMap root = null;
			var stack = new cape.Stack<cape.DynamicMap>();
			var pp = forString(xml);
			pp.setIgnoreWhiteSpace(ignoreWhiteSpace);
			while(true) {
				var o = pp.next();
				if(o == null) {
					break;
				}
				if(o is StartElement) {
					var nn = new cape.DynamicMap();
					nn.set("name", (object)((StartElement)o).getName());
					nn.set("attributes", (object)((StartElement)o).getParams());
					if(root == null) {
						root = nn;
						stack.push((cape.DynamicMap)nn);
					}
					else {
						var current = stack.peek();
						if(current == null) {
							current = root;
						}
						var children = current.getDynamicVector("children");
						if(children == null) {
							children = new cape.DynamicVector();
							current.set("children", (object)children);
						}
						children.append((object)nn);
						stack.push((cape.DynamicMap)nn);
					}
				}
				else if(o is EndElement) {
					stack.pop();
				}
				else if(o is CharacterData) {
					var current1 = stack.peek();
					if(current1 != null) {
						var children1 = current1.getDynamicVector("children");
						if(children1 == null) {
							children1 = new cape.DynamicVector();
							current1.set("children", (object)children1);
						}
						children1.append((object)((CharacterData)o).getData());
					}
				}
			}
			return(root);
		}

		public class StartElement
		{
			private string name = null;
			private cape.DynamicMap @params = null;

			public string getParam(string pname) {
				if(@params == null) {
					return(null);
				}
				return(@params.getString(pname));
			}

			public string getName() {
				return(name);
			}

			public StartElement setName(string v) {
				name = v;
				return(this);
			}

			public cape.DynamicMap getParams() {
				return(@params);
			}

			public StartElement setParams(cape.DynamicMap v) {
				@params = v;
				return(this);
			}
		}

		public class EndElement
		{
			private string name = null;

			public string getName() {
				return(name);
			}

			public EndElement setName(string v) {
				name = v;
				return(this);
			}
		}

		public class CharacterData
		{
			private string data = null;

			public string getData() {
				return(data);
			}

			public CharacterData setData(string v) {
				data = v;
				return(this);
			}
		}

		public class Comment
		{
			private string text = null;

			public string getText() {
				return(text);
			}

			public Comment setText(string v) {
				text = v;
				return(this);
			}
		}

		private cape.CharacterIterator it = null;
		private object nextQueue = null;
		private string cdataStart = "![CDATA[";
		private string commentStart = "!--";
		private cape.StringBuilder tag = null;
		private cape.StringBuilder def = null;
		private cape.StringBuilder cdata = null;
		private cape.StringBuilder comment = null;
		private bool ignoreWhiteSpace = false;

		public static XMLParser forFile(cape.File file) {
			if(file == null) {
				return(null);
			}
			var reader = file.read();
			if(reader == null) {
				return(null);
			}
			var v = new XMLParser();
			v.it = (cape.CharacterIterator)new cape.CharacterIteratorForReader((cape.Reader)reader);
			return(v);
		}

		public static XMLParser forString(string @string) {
			if(object.Equals(@string, null)) {
				return(null);
			}
			var v = new XMLParser();
			v.it = (cape.CharacterIterator)new cape.CharacterIteratorForString(@string);
			return(v);
		}

		public static XMLParser forIterator(cape.CharacterIterator it) {
			if(it == null) {
				return(null);
			}
			var v = new XMLParser();
			v.it = it;
			return(v);
		}

		private object onTagString(string tagstring) {
			if(cape.String.charAt(tagstring, 0) == '/') {
				return((object)new EndElement().setName(cape.String.subString(tagstring, 1)));
			}
			var element = new cape.StringBuilder();
			var @params = new cape.DynamicMap();
			var it = new cape.CharacterIteratorForString(tagstring);
			var c = ' ';
			while((c = it.getNextChar()) > 0) {
				if(((((c == ' ') || (c == '\t')) || (c == '\n')) || (c == '\r')) || (c == '/')) {
					if(element.count() > 0) {
						break;
					}
				}
				else {
					element.append(c);
				}
			}
			while((c > 0) && (c != '/')) {
				var pname = new cape.StringBuilder();
				var pval = new cape.StringBuilder();
				while((((c == ' ') || (c == '\t')) || (c == '\n')) || (c == '\r')) {
					c = it.getNextChar();
				}
				while((((((c > 0) && (c != ' ')) && (c != '\t')) && (c != '\n')) && (c != '\r')) && (c != '=')) {
					pname.append(c);
					c = it.getNextChar();
				}
				while((((c == ' ') || (c == '\t')) || (c == '\n')) || (c == '\r')) {
					c = it.getNextChar();
				}
				if(c != '=') {
					;
				}
				else {
					c = it.getNextChar();
					while((((c == ' ') || (c == '\t')) || (c == '\n')) || (c == '\r')) {
						c = it.getNextChar();
					}
					if(c != '\"') {
						;
						while(((((c > 0) && (c != ' ')) && (c != '\t')) && (c != '\n')) && (c != '\r')) {
							pval.append(c);
							c = it.getNextChar();
						}
						while((((c == ' ') || (c == '\t')) || (c == '\n')) || (c == '\r')) {
							c = it.getNextChar();
						}
					}
					else {
						c = it.getNextChar();
						while((c > 0) && (c != '\"')) {
							pval.append(c);
							c = it.getNextChar();
						}
						if(c != '\"') {
							;
						}
						else {
							c = it.getNextChar();
						}
						while((((c == ' ') || (c == '\t')) || (c == '\n')) || (c == '\r')) {
							c = it.getNextChar();
						}
					}
				}
				var pnamestr = pname.toString();
				var pvalstr = pval.toString();
				@params.set(pnamestr, (object)pvalstr);
			}
			var els = element.toString();
			if(c == '/') {
				nextQueue = (object)new EndElement().setName(els);
			}
			return((object)new StartElement().setName(els).setParams(@params));
		}

		private bool isOnlyWhiteSpace(string str) {
			if(object.Equals(str, null)) {
				return(true);
			}
			var array = cape.String.toCharArray(str);
			if(array != null) {
				var n = 0;
				var m = array.Length;
				for(n = 0 ; n < m ; n++) {
					var c = array[n];
					if((((c == ' ') || (c == '\t')) || (c == '\n')) || (c == '\r')) {
						;
					}
					else {
						return(false);
					}
				}
			}
			return(true);
		}

		public object next() {
			if(nextQueue != null) {
				var v = nextQueue;
				nextQueue = null;
				return(v);
			}
			while(it.hasEnded() == false) {
				var nxb = it.getNextChar();
				if(nxb < 1) {
					continue;
				}
				if(tag != null) {
					if(nxb == '>') {
						var ts = tag.toString();
						tag = null;
						return(onTagString(ts));
					}
					tag.append(nxb);
					if(((nxb == '[') && (tag.count() == cape.String.getLength(cdataStart))) && cape.String.equals(cdataStart, tag.toString())) {
						tag = null;
						cdata = new cape.StringBuilder();
					}
					else if(((nxb == '-') && (tag.count() == cape.String.getLength(commentStart))) && cape.String.equals(commentStart, tag.toString())) {
						tag = null;
						comment = new cape.StringBuilder();
					}
				}
				else if(cdata != null) {
					var c0 = nxb;
					var c1 = ' ';
					var c2 = ' ';
					if(c0 == ']') {
						c1 = it.getNextChar();
						if(c1 == ']') {
							c2 = it.getNextChar();
							if(c2 == '>') {
								var dd = cdata.toString();
								cdata = null;
								return((object)new CharacterData().setData(dd));
							}
							else {
								it.moveToPreviousChar();
								it.moveToPreviousChar();
								cdata.append(c0);
							}
						}
						else {
							it.moveToPreviousChar();
							cdata.append(c0);
						}
					}
					else {
						cdata.append(c0);
					}
				}
				else if(comment != null) {
					var c01 = nxb;
					var c11 = ' ';
					var c21 = ' ';
					if(c01 == '-') {
						c11 = it.getNextChar();
						if(c11 == '-') {
							c21 = it.getNextChar();
							if(c21 == '>') {
								var ct = comment.toString();
								comment = null;
								return((object)new Comment().setText(ct));
							}
							else {
								it.moveToPreviousChar();
								it.moveToPreviousChar();
								comment.append(c01);
							}
						}
						else {
							it.moveToPreviousChar();
							comment.append(c01);
						}
					}
					else {
						comment.append(c01);
					}
				}
				else if(nxb == '<') {
					if(def != null) {
						var cd = def.toString();
						def = null;
						if(ignoreWhiteSpace && !(object.Equals(cd, null))) {
							if(isOnlyWhiteSpace(cd)) {
								cd = null;
							}
						}
						if(!(object.Equals(cd, null))) {
							it.moveToPreviousChar();
							return((object)new CharacterData().setData(cd));
						}
					}
					tag = new cape.StringBuilder();
				}
				else {
					if(def == null) {
						def = new cape.StringBuilder();
					}
					def.append(nxb);
				}
			}
			return(null);
		}

		public bool getIgnoreWhiteSpace() {
			return(ignoreWhiteSpace);
		}

		public XMLParser setIgnoreWhiteSpace(bool v) {
			ignoreWhiteSpace = v;
			return(this);
		}
	}
}
