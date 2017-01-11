
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
	public class RichTextReferenceParagraph : RichTextParagraph
	{
		private string reference = null;
		private string text = null;

		public override string toMarkup() {
			var sb = new cape.StringBuilder();
			sb.append("@reference ");
			sb.append(reference);
			if(cape.String.isEmpty(text) == false) {
				sb.append(' ');
				sb.append('\"');
				sb.append(text);
				sb.append('\"');
			}
			return(sb.toString());
		}

		public override string toText() {
			var v = text;
			if(cape.String.isEmpty(text)) {
				v = reference;
			}
			return(v);
		}

		public override cape.DynamicMap toJson() {
			var v = new cape.DynamicMap();
			v.set("type", (object)"reference");
			v.set("reference", (object)reference);
			v.set("text", (object)text);
			return(v);
		}

		public override string toHtml(RichTextDocumentReferenceResolver refs) {
			string reftitle = null;
			string href = null;
			if(cape.String.isEmpty(text) == false) {
				reftitle = text;
			}
			if(cape.String.isEmpty(reftitle)) {
				if(refs != null) {
					reftitle = refs.getReferenceTitle(reference);
				}
				else {
					reftitle = reference;
				}
			}
			if(refs != null) {
				href = refs.getReferenceHref(reference);
			}
			else {
				href = reference;
			}
			if(cape.String.isEmpty(href)) {
				return("");
			}
			if(cape.String.isEmpty(reftitle)) {
				reftitle = href;
			}
			return(((("<p class=\"reference\"><a href=\"" + capex.HTMLString.sanitize(href)) + "\">") + capex.HTMLString.sanitize(reftitle)) + "</a></p>\n");
		}

		public string getReference() {
			return(reference);
		}

		public RichTextReferenceParagraph setReference(string v) {
			reference = v;
			return(this);
		}

		public string getText() {
			return(text);
		}

		public RichTextReferenceParagraph setText(string v) {
			text = v;
			return(this);
		}
	}
}
