
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
	public class RichTextLinkParagraph : RichTextParagraph
	{
		private string link = null;
		private string text = null;
		private bool popup = false;

		public override string toMarkup() {
			var sb = new cape.StringBuilder();
			sb.append("@link ");
			sb.append(link);
			sb.append(' ');
			sb.append('\"');
			if(cape.String.isEmpty(text) == false) {
				sb.append(text);
			}
			sb.append('\"');
			if(popup) {
				sb.append(" popup");
			}
			return(sb.toString());
		}

		public override string toText() {
			var v = text;
			if(cape.String.isEmpty(v)) {
				v = link;
			}
			return(v);
		}

		public override cape.DynamicMap toJson() {
			var v = new cape.DynamicMap();
			v.set("type", (object)"link");
			v.set("link", (object)link);
			v.set("text", (object)text);
			return(v);
		}

		public override string toHtml(RichTextDocumentReferenceResolver refs) {
			var href = capex.HTMLString.sanitize(link);
			var tt = text;
			if(cape.String.isEmpty(tt)) {
				tt = href;
			}
			if(cape.String.isEmpty(tt)) {
				tt = "(empty link)";
			}
			var targetblank = "";
			if(popup) {
				targetblank = " target=\"_blank\"";
			}
			return(((((("<p class=\"link\"><a href=\"" + href) + "\"") + targetblank) + ">") + tt) + "</a></p>\n");
		}

		public string getLink() {
			return(link);
		}

		public RichTextLinkParagraph setLink(string v) {
			link = v;
			return(this);
		}

		public string getText() {
			return(text);
		}

		public RichTextLinkParagraph setText(string v) {
			text = v;
			return(this);
		}

		public bool getPopup() {
			return(popup);
		}

		public RichTextLinkParagraph setPopup(bool v) {
			popup = v;
			return(this);
		}
	}
}
