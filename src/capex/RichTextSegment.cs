
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
	public class RichTextSegment
	{
		private string text = null;
		private bool bold = false;
		private bool italic = false;
		private bool underline = false;
		private string color = null;
		private string link = null;
		private string reference = null;
		private bool isInline = false;
		private bool linkPopup = false;

		public void addMarkupModifiers(cape.StringBuilder sb) {
			if(bold) {
				sb.append("**");
			}
			if(italic) {
				sb.append("''");
			}
			if(underline) {
				sb.append("__");
			}
		}

		public string toMarkup() {
			var sb = new cape.StringBuilder();
			addMarkupModifiers(sb);
			if(cape.String.isEmpty(link) == false) {
				sb.append('[');
				if(isInline) {
					sb.append('>');
				}
				sb.append(link);
				if(cape.String.isEmpty(text) == false) {
					sb.append('|');
					sb.append(text);
				}
				sb.append(']');
			}
			else if(cape.String.isEmpty(reference) == false) {
				sb.append('{');
				if(isInline) {
					sb.append('>');
				}
				sb.append(reference);
				if(cape.String.isEmpty(text) == false) {
					sb.append('|');
					sb.append(text);
				}
				sb.append('}');
			}
			else {
				sb.append(text);
			}
			addMarkupModifiers(sb);
			return(sb.toString());
		}

		public cape.DynamicMap toJson() {
			var v = new cape.DynamicMap();
			v.set("text", (object)text);
			if(isInline) {
				v.set("inline", isInline);
			}
			if(bold) {
				v.set("bold", bold);
			}
			if(italic) {
				v.set("italic", italic);
			}
			if(underline) {
				v.set("underline", underline);
			}
			if(cape.String.isEmpty(color) == false) {
				v.set("color", (object)color);
			}
			if(cape.String.isEmpty(link) == false) {
				v.set("link", (object)link);
			}
			if(cape.String.isEmpty(reference) == false) {
				v.set("reference", (object)reference);
			}
			return(v);
		}

		public string getText() {
			return(text);
		}

		public RichTextSegment setText(string v) {
			text = v;
			return(this);
		}

		public bool getBold() {
			return(bold);
		}

		public RichTextSegment setBold(bool v) {
			bold = v;
			return(this);
		}

		public bool getItalic() {
			return(italic);
		}

		public RichTextSegment setItalic(bool v) {
			italic = v;
			return(this);
		}

		public bool getUnderline() {
			return(underline);
		}

		public RichTextSegment setUnderline(bool v) {
			underline = v;
			return(this);
		}

		public string getColor() {
			return(color);
		}

		public RichTextSegment setColor(string v) {
			color = v;
			return(this);
		}

		public string getLink() {
			return(link);
		}

		public RichTextSegment setLink(string v) {
			link = v;
			return(this);
		}

		public string getReference() {
			return(reference);
		}

		public RichTextSegment setReference(string v) {
			reference = v;
			return(this);
		}

		public bool getIsInline() {
			return(isInline);
		}

		public RichTextSegment setIsInline(bool v) {
			isInline = v;
			return(this);
		}

		public bool getLinkPopup() {
			return(linkPopup);
		}

		public RichTextSegment setLinkPopup(bool v) {
			linkPopup = v;
			return(this);
		}
	}
}
