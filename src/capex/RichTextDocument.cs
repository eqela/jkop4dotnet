
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
	public class RichTextDocument
	{
		public static RichTextDocument forWikiMarkupFile(cape.File file) {
			return(capex.RichTextWikiMarkupParser.parseFile(file));
		}

		public static RichTextDocument forWikiMarkupString(string str) {
			return(capex.RichTextWikiMarkupParser.parseString(str));
		}

		private cape.DynamicMap metadata = null;
		private System.Collections.Generic.List<RichTextParagraph> paragraphs = null;

		public RichTextDocument() {
			metadata = new cape.DynamicMap();
		}

		public string getTitle() {
			return(metadata.getString("title"));
		}

		public RichTextDocument setTitle(string v) {
			metadata.set("title", (object)v);
			return(this);
		}

		public string getMetadata(string k) {
			return(metadata.getString(k));
		}

		public RichTextDocument setMetadata(string k, string v) {
			metadata.set(k, (object)v);
			return(this);
		}

		public RichTextDocument addParagraph(RichTextParagraph rtp) {
			if(rtp == null) {
				return(this);
			}
			if(paragraphs == null) {
				paragraphs = new System.Collections.Generic.List<RichTextParagraph>();
			}
			cape.Vector.append(paragraphs, rtp);
			if(((object.Equals(getTitle(), null)) && (rtp is RichTextStyledParagraph)) && ((rtp as RichTextStyledParagraph).getHeading() == 1)) {
				setTitle((rtp as RichTextStyledParagraph).getTextContent());
			}
			return(this);
		}

		public System.Collections.Generic.List<string> getAllReferences() {
			var v = new System.Collections.Generic.List<string>();
			if(paragraphs != null) {
				var n = 0;
				var m = paragraphs.Count;
				for(n = 0 ; n < m ; n++) {
					var paragraph = paragraphs[n];
					if(paragraph != null) {
						if(paragraph is RichTextReferenceParagraph) {
							var @ref = ((RichTextReferenceParagraph)paragraph).getReference();
							if(cape.String.isEmpty(@ref) == false) {
								v.Add(@ref);
							}
						}
						else if(paragraph is RichTextStyledParagraph) {
							var array = ((RichTextStyledParagraph)paragraph).getSegments();
							if(array != null) {
								var n2 = 0;
								var m2 = array.Count;
								for(n2 = 0 ; n2 < m2 ; n2++) {
									var segment = array[n2];
									if(segment != null) {
										var ref1 = segment.getReference();
										if(cape.String.isEmpty(ref1) == false) {
											v.Add(ref1);
										}
									}
								}
							}
						}
					}
				}
			}
			return(v);
		}

		public System.Collections.Generic.List<string> getAllLinks() {
			var v = new System.Collections.Generic.List<string>();
			if(paragraphs != null) {
				var n = 0;
				var m = paragraphs.Count;
				for(n = 0 ; n < m ; n++) {
					var paragraph = paragraphs[n];
					if(paragraph != null) {
						if(paragraph is RichTextLinkParagraph) {
							var link = ((RichTextLinkParagraph)paragraph).getLink();
							if(cape.String.isEmpty(link) == false) {
								v.Add(link);
							}
						}
						else if(paragraph is RichTextStyledParagraph) {
							var array = ((RichTextStyledParagraph)paragraph).getSegments();
							if(array != null) {
								var n2 = 0;
								var m2 = array.Count;
								for(n2 = 0 ; n2 < m2 ; n2++) {
									var segment = array[n2];
									if(segment != null) {
										var link1 = segment.getLink();
										if(cape.String.isEmpty(link1) == false) {
											v.Add(link1);
										}
									}
								}
							}
						}
					}
				}
			}
			return(v);
		}

		public cape.DynamicMap toJson() {
			var v = new cape.DynamicMap();
			v.set("metadata", (object)metadata);
			v.set("title", (object)getTitle());
			var pp = new System.Collections.Generic.List<object>();
			if(paragraphs != null) {
				var n = 0;
				var m = paragraphs.Count;
				for(n = 0 ; n < m ; n++) {
					var par = paragraphs[n];
					if(par != null) {
						var pj = par.toJson();
						if(pj != null) {
							cape.Vector.append(pp, pj);
						}
					}
				}
			}
			v.set("paragraphs", (object)pp);
			return(v);
		}

		public string toHtml(RichTextDocumentReferenceResolver refs) {
			var sb = new cape.StringBuilder();
			var array = getParagraphs();
			if(array != null) {
				var n = 0;
				var m = array.Count;
				for(n = 0 ; n < m ; n++) {
					var paragraph = array[n];
					if(paragraph != null) {
						var html = paragraph.toHtml(refs);
						if(cape.String.isEmpty(html) == false) {
							sb.append(html);
							sb.append('\n');
						}
					}
				}
			}
			return(sb.toString());
		}

		public System.Collections.Generic.List<RichTextParagraph> getParagraphs() {
			return(paragraphs);
		}

		public RichTextDocument setParagraphs(System.Collections.Generic.List<RichTextParagraph> v) {
			paragraphs = v;
			return(this);
		}
	}
}
