
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
	public class VerboseDateTimeString
	{
		public static string forNow() {
			return(forDateTime(cape.DateTime.forNow()));
		}

		public static string forDateTime(cape.DateTime dd) {
			if(dd == null) {
				return("NODATE");
			}
			var sb = new cape.StringBuilder();
			sb.append(getShortDayName(dd.getWeekDay()));
			sb.append(", ");
			sb.append(cape.String.forIntegerWithPadding(dd.getDayOfMonth(), 2, "0"));
			sb.append(' ');
			sb.append(getShortMonthName(dd.getMonth()));
			sb.append(' ');
			sb.append(cape.String.forInteger(dd.getYear()));
			sb.append(' ');
			sb.append(cape.String.forIntegerWithPadding(dd.getHours(), 2, "0"));
			sb.append(':');
			sb.append(cape.String.forIntegerWithPadding(dd.getMinutes(), 2, "0"));
			sb.append(':');
			sb.append(cape.String.forIntegerWithPadding(dd.getSeconds(), 2, "0"));
			sb.append(" GMT");
			return(sb.toString());
		}

		public static string getTimeStringForDateTime(cape.DateTime dd, bool includeTimeZone = false) {
			if(dd == null) {
				return("NOTIME");
			}
			var sb = new cape.StringBuilder();
			sb.append(cape.String.forIntegerWithPadding(dd.getHours(), 2, "0"));
			sb.append(':');
			sb.append(cape.String.forIntegerWithPadding(dd.getMinutes(), 2, "0"));
			sb.append(':');
			sb.append(cape.String.forIntegerWithPadding(dd.getSeconds(), 2, "0"));
			if(includeTimeZone) {
				sb.append(" GMT");
			}
			return(sb.toString());
		}

		public static string getDateStringForDateTime(cape.DateTime dd) {
			if(dd == null) {
				return("NODATE");
			}
			var sb = new cape.StringBuilder();
			sb.append(getLongMonthName(dd.getMonth()));
			sb.append(' ');
			sb.append(cape.String.forInteger(dd.getDayOfMonth()));
			sb.append(", ");
			sb.append(cape.String.forInteger(dd.getYear()));
			return(sb.toString());
		}

		public static string getShortDayName(int n) {
			switch(n) {
				case 1: {
					return("Sun");
					break;
				}
				case 2: {
					return("Mon");
					break;
				}
				case 3: {
					return("Tue");
					break;
				}
				case 4: {
					return("Wed");
					break;
				}
				case 5: {
					return("Thu");
					break;
				}
				case 6: {
					return("Fri");
					break;
				}
				case 7: {
					return("Sat");
					break;
				}
			}
			return(null);
		}

		public static string getShortMonthName(int n) {
			switch(n) {
				case 1: {
					return("Jan");
					break;
				}
				case 2: {
					return("Feb");
					break;
				}
				case 3: {
					return("Mar");
					break;
				}
				case 4: {
					return("Apr");
					break;
				}
				case 5: {
					return("May");
					break;
				}
				case 6: {
					return("Jun");
					break;
				}
				case 7: {
					return("Jul");
					break;
				}
				case 8: {
					return("Aug");
					break;
				}
				case 9: {
					return("Sep");
					break;
				}
				case 10: {
					return("Oct");
					break;
				}
				case 11: {
					return("Nov");
					break;
				}
				case 12: {
					return("Dec");
					break;
				}
			}
			return(null);
		}

		public static string getLongMonthName(int n) {
			switch(n) {
				case 1: {
					return("January");
					break;
				}
				case 2: {
					return("February");
					break;
				}
				case 3: {
					return("March");
					break;
				}
				case 4: {
					return("April");
					break;
				}
				case 5: {
					return("May");
					break;
				}
				case 6: {
					return("June");
					break;
				}
				case 7: {
					return("July");
					break;
				}
				case 8: {
					return("August");
					break;
				}
				case 9: {
					return("September");
					break;
				}
				case 10: {
					return("October");
					break;
				}
				case 11: {
					return("November");
					break;
				}
				case 12: {
					return("December");
					break;
				}
			}
			return(null);
		}
	}
}
