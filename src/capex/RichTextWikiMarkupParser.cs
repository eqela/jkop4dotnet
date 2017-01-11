
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
	public class RichTextWikiMarkupParser
	{
		public static RichTextDocument parseFile(cape.File file) {
			return(new RichTextWikiMarkupParser().setFile(file).parse());
		}

		public static RichTextDocument parseString(string data) {
			return(new RichTextWikiMarkupParser().setData(data).parse());
		}

		private cape.File file = null;
		private string data = null;

		public string skipEmptyLines(cape.LineReader pr) {
			string line = null;
			while(!(object.Equals(line = pr.readLine(), null))) {
				line = cape.String.strip(line);
				if(!(object.Equals(line, null)) && cape.String.startsWith(line, "#")) {
					continue;
				}
				if(cape.String.isEmpty(line) == false) {
					break;
				}
			}
			return(line);
		}

		public RichTextPreformattedParagraph readPreformattedParagraph(string id, cape.LineReader pr) {
			var sb = new cape.StringBuilder();
			string line = null;
			while(!(object.Equals(line = pr.readLine(), null))) {
				if(cape.String.startsWith(line, "---") && cape.String.endsWith(line, "---")) {
					var lid = cape.String.subString(line, 3, cape.String.getLength(line) - 6);
					if(!(object.Equals(lid, null))) {
						lid = cape.String.strip(lid);
					}
					if(cape.String.isEmpty(id)) {
						if(cape.String.isEmpty(lid)) {
							break;
						}
					}
					else if(cape.String.equals(id, lid)) {
						break;
					}
				}
				sb.append(line);
				sb.append('\n');
			}
			return(new RichTextPreformattedParagraph().setId(id).setText(sb.toString()));
		}

		public RichTextBlockParagraph readBlockParagraph(string id, cape.LineReader pr) {
			var sb = new cape.StringBuilder();
			string line = null;
			while(!(object.Equals(line = pr.readLine(), null))) {
				if(cape.String.startsWith(line, "--") && cape.String.endsWith(line, "--")) {
					var lid = cape.String.subString(line, 2, cape.String.getLength(line) - 4);
					if(!(object.Equals(lid, null))) {
						lid = cape.String.strip(lid);
					}
					if(cape.String.isEmpty(id)) {
						if(cape.String.isEmpty(lid)) {
							break;
						}
					}
					else if(cape.String.equals(id, lid)) {
						break;
					}
				}
				sb.append(line);
				sb.append('\n');
			}
			return(new RichTextBlockParagraph().setId(id).setText(sb.toString()));
		}

		public bool processInput(cape.LineReader pr, RichTextDocument doc) {
			var line = skipEmptyLines(pr);
			if(object.Equals(line, null)) {
				return(false);
			}
			if(object.Equals(line, "-")) {
				doc.addParagraph((RichTextParagraph)new RichTextSeparatorParagraph());
				return(true);
			}
			if(cape.String.startsWith(line, "@content ")) {
				var id = cape.String.subString(line, 9);
				if(!(object.Equals(id, null))) {
					id = cape.String.strip(id);
				}
				doc.addParagraph((RichTextParagraph)new RichTextContentParagraph().setContentId(id));
				return(true);
			}
			if(cape.String.startsWith(line, "@image ")) {
				var @ref = cape.String.subString(line, 7);
				if(!(object.Equals(@ref, null))) {
					@ref = cape.String.strip(@ref);
				}
				doc.addParagraph((RichTextParagraph)new RichTextImageParagraph().setFilename(@ref));
				return(true);
			}
			if(cape.String.startsWith(line, "@image100 ")) {
				var ref1 = cape.String.subString(line, 10);
				if(!(object.Equals(ref1, null))) {
					ref1 = cape.String.strip(ref1);
				}
				doc.addParagraph((RichTextParagraph)new RichTextImageParagraph().setFilename(ref1));
				return(true);
			}
			if(cape.String.startsWith(line, "@image75 ")) {
				var ref2 = cape.String.subString(line, 9);
				if(!(object.Equals(ref2, null))) {
					ref2 = cape.String.strip(ref2);
				}
				doc.addParagraph((RichTextParagraph)new RichTextImageParagraph().setFilename(ref2).setWidth(75));
				return(true);
			}
			if(cape.String.startsWith(line, "@image50 ")) {
				var ref3 = cape.String.subString(line, 9);
				if(!(object.Equals(ref3, null))) {
					ref3 = cape.String.strip(ref3);
				}
				doc.addParagraph((RichTextParagraph)new RichTextImageParagraph().setFilename(ref3).setWidth(50));
				return(true);
			}
			if(cape.String.startsWith(line, "@image25 ")) {
				var ref4 = cape.String.subString(line, 9);
				if(!(object.Equals(ref4, null))) {
					ref4 = cape.String.strip(ref4);
				}
				doc.addParagraph((RichTextParagraph)new RichTextImageParagraph().setFilename(ref4).setWidth(25));
				return(true);
			}
			if(cape.String.startsWith(line, "@reference ")) {
				var ref5 = cape.String.subString(line, 11);
				if(!(object.Equals(ref5, null))) {
					ref5 = cape.String.strip(ref5);
				}
				var sq = cape.String.quotedStringToVector(ref5, ' ');
				var rrf = cape.Vector.getAt(sq, 0);
				var txt = cape.Vector.getAt(sq, 1);
				doc.addParagraph((RichTextParagraph)new RichTextReferenceParagraph().setReference(rrf).setText(txt));
				return(true);
			}
			if(cape.String.startsWith(line, "@set ")) {
				var link = cape.String.subString(line, 5);
				if(!(object.Equals(link, null))) {
					link = cape.String.strip(link);
				}
				var sq1 = cape.String.quotedStringToVector(link, ' ');
				var key = cape.Vector.getAt(sq1, 0);
				var val = cape.Vector.getAt(sq1, 1);
				if(cape.String.isEmpty(key)) {
					return(true);
				}
				doc.setMetadata(key, val);
				return(true);
			}
			if(cape.String.startsWith(line, "@link ")) {
				var link1 = cape.String.subString(line, 6);
				if(!(object.Equals(link1, null))) {
					link1 = cape.String.strip(link1);
				}
				var sq2 = cape.String.quotedStringToVector(link1, ' ');
				var url = cape.Vector.getAt(sq2, 0);
				var txt1 = cape.Vector.getAt(sq2, 1);
				var flags = cape.Vector.getAt(sq2, 2);
				if(cape.String.isEmpty(txt1)) {
					txt1 = url;
				}
				var v = new RichTextLinkParagraph();
				v.setLink(url);
				v.setText(txt1);
				if(cape.String.equals("internal", flags)) {
					v.setPopup(false);
				}
				else {
					v.setPopup(true);
				}
				doc.addParagraph((RichTextParagraph)v);
				return(true);
			}
			if(cape.String.startsWith(line, "---") && cape.String.endsWith(line, "---")) {
				var id1 = cape.String.subString(line, 3, cape.String.getLength(line) - 6);
				if(!(object.Equals(id1, null))) {
					id1 = cape.String.strip(id1);
				}
				if(cape.String.isEmpty(id1)) {
					id1 = null;
				}
				doc.addParagraph((RichTextParagraph)readPreformattedParagraph(id1, pr));
				return(true);
			}
			if(cape.String.startsWith(line, "--") && cape.String.endsWith(line, "--")) {
				var id2 = cape.String.subString(line, 2, cape.String.getLength(line) - 4);
				if(!(object.Equals(id2, null))) {
					id2 = cape.String.strip(id2);
				}
				if(cape.String.isEmpty(id2)) {
					id2 = null;
				}
				doc.addParagraph((RichTextParagraph)readBlockParagraph(id2, pr));
				return(true);
			}
			var sb = new cape.StringBuilder();
			var pc = (char)0;
			do {
				line = cape.String.strip(line);
				if(cape.String.isEmpty(line)) {
					break;
				}
				if(cape.String.startsWith(line, "#") == false) {
					var it = cape.String.iterate(line);
					var c = ' ';
					if((sb.count() > 0) && (pc != ' ')) {
						sb.append(' ');
						pc = ' ';
					}
					while((c = it.getNextChar()) > 0) {
						if((((c == ' ') || (c == '\t')) || (c == '\r')) || (c == '\n')) {
							if(pc == ' ') {
								continue;
							}
							c = ' ';
						}
						sb.append(c);
						pc = c;
					}
				}
			}
			while(!(object.Equals(line = pr.readLine(), null)));
			var s = sb.toString();
			if(cape.String.isEmpty(s)) {
				return(false);
			}
			doc.addParagraph((RichTextParagraph)capex.RichTextStyledParagraph.forString(s));
			return(true);
		}

		public RichTextDocument parse() {
			cape.LineReader pr = null;
			if(file != null) {
				pr = (cape.LineReader)new cape.PrintReader((cape.Reader)file.read());
			}
			else if(!(object.Equals(data, null))) {
				pr = (cape.LineReader)new cape.StringLineReader(data);
			}
			if(pr == null) {
				return(null);
			}
			var v = new RichTextDocument();
			while(processInput(pr, v)) {
				;
			}
			return(v);
		}

		public cape.File getFile() {
			return(file);
		}

		public RichTextWikiMarkupParser setFile(cape.File v) {
			file = v;
			return(this);
		}

		public string getData() {
			return(data);
		}

		public RichTextWikiMarkupParser setData(string v) {
			data = v;
			return(this);
		}
	}
}
