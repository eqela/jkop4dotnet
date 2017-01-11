
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

namespace cape
{
	public class DateTime : StringObject
	{
		public static DateTime forNow() {
			return(forTimeSeconds(cape.SystemClock.asSeconds()));
		}

		public static DateTime forTimeSeconds(long seconds) {
			var v = new DateTime();
			if(v == null) {
				return(null);
			}
			v.setTimeSeconds(seconds);
			return(v);
		}

		public static DateTime forTimeValue(TimeValue tv) {
			if(tv == null) {
				return(null);
			}
			return(forTimeSeconds(tv.getSeconds()));
		}

		private long timeSeconds = (long)0;
		private int weekDay = 0;
		private int dayOfMonth = 0;
		private int month = 0;
		private int year = 0;
		private int hours = 0;
		private int minutes = 0;
		private int seconds = 0;
		private bool utc = false;

		public void onTimeSecondsChanged(long seconds) {
			var dt = new System.DateTime(1970, 1, 1).AddSeconds(seconds);
			setWeekDay((int)dt.DayOfWeek);
			setDayOfMonth(dt.Day);
			setMonth(dt.Month);
			setYear(dt.Year);
			setHours(dt.Hour);
			setMinutes(dt.Minute);
			setSeconds(dt.Second);
		}

		public long getTimeSeconds() {
			return(timeSeconds);
		}

		public void setTimeSeconds(long seconds) {
			timeSeconds = seconds;
			onTimeSecondsChanged(seconds);
		}

		public string toStringDate(char delim = '-') {
			var sb = new StringBuilder();
			sb.append(cape.String.forIntegerWithPadding(getYear(), 4));
			if(delim > 0) {
				sb.append(delim);
			}
			sb.append(cape.String.forIntegerWithPadding(getMonth(), 2));
			if(delim > 0) {
				sb.append(delim);
			}
			sb.append(cape.String.forIntegerWithPadding(getDayOfMonth(), 2));
			return(sb.toString());
		}

		public string toStringTime(char delim = '-') {
			var sb = new StringBuilder();
			sb.append(cape.String.forIntegerWithPadding(getHours(), 2));
			if(delim > 0) {
				sb.append(delim);
			}
			sb.append(cape.String.forIntegerWithPadding(getMinutes(), 2));
			if(delim > 0) {
				sb.append(delim);
			}
			sb.append(cape.String.forIntegerWithPadding(getSeconds(), 2));
			return(sb.toString());
		}

		public string toStringDateTime() {
			var sb = new StringBuilder();
			sb.append(toStringDate());
			sb.append(" ");
			sb.append(toStringTime());
			return(sb.toString());
		}

		public virtual string toString() {
			return(toStringDateTime());
		}

		public int getWeekDay() {
			return(weekDay);
		}

		public DateTime setWeekDay(int v) {
			weekDay = v;
			return(this);
		}

		public int getDayOfMonth() {
			return(dayOfMonth);
		}

		public DateTime setDayOfMonth(int v) {
			dayOfMonth = v;
			return(this);
		}

		public int getMonth() {
			return(month);
		}

		public DateTime setMonth(int v) {
			month = v;
			return(this);
		}

		public int getYear() {
			return(year);
		}

		public DateTime setYear(int v) {
			year = v;
			return(this);
		}

		public int getHours() {
			return(hours);
		}

		public DateTime setHours(int v) {
			hours = v;
			return(this);
		}

		public int getMinutes() {
			return(minutes);
		}

		public DateTime setMinutes(int v) {
			minutes = v;
			return(this);
		}

		public int getSeconds() {
			return(seconds);
		}

		public DateTime setSeconds(int v) {
			seconds = v;
			return(this);
		}

		public bool getUtc() {
			return(utc);
		}

		public DateTime setUtc(bool v) {
			utc = v;
			return(this);
		}
	}
}
