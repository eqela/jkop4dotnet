
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

namespace capex.console
{
	public class ConsoleApplication
	{
		public static System.Collections.Generic.List<CommandLineArgument> parseCommandLineArguments(string[] args) {
			var v = new System.Collections.Generic.List<CommandLineArgument>();
			if(args != null) {
				var n = 0;
				var m = args.Length;
				for(n = 0 ; n < m ; n++) {
					var arg = args[n];
					if(arg != null) {
						if(cape.String.startsWith(arg, "-")) {
							if(cape.String.indexOf(arg, '=') > 0) {
								var comps = cape.String.split(arg, '=', 2);
								var key = cape.String.getSubString(comps[0], 1);
								var val = comps[1];
								v.Add(new CommandLineArgument(arg, null, null, key, val));
							}
							else {
								v.Add(new CommandLineArgument(arg, null, cape.String.getSubString(arg, 1), null, "true"));
							}
						}
						else {
							v.Add(new CommandLineArgument(arg, arg, null, null, arg));
						}
					}
				}
			}
			return(v);
		}

		private static int getLongestString(string[][] strings, int add, int ov) {
			var v = ov;
			if(strings != null) {
				var n = 0;
				var m = strings.Length;
				for(n = 0 ; n < m ; n++) {
					var @string = strings[n];
					if(@string != null) {
						var ss = cape.String.getLength(@string[0]) + add;
						if(ss > v) {
							v = ss;
						}
					}
				}
			}
			return(v);
		}

		public static void printUsage(cape.PrintWriter stdout, string name, string[][] parameters, string[][] flags, string[][] options) {
			var nn = name;
			if(cape.String.isEmpty(nn)) {
				var exe = cape.CurrentProcess.getExecutableFile();
				if(exe != null) {
					nn = exe.baseNameWithoutExtension();
				}
				if(cape.String.isEmpty(nn)) {
					nn = "(command)";
				}
			}
			var ll = 0;
			ll = getLongestString(parameters, 0, ll);
			ll = getLongestString(flags, 1, ll);
			ll = getLongestString(options, 9, ll);
			stdout.print(("Usage: " + nn) + " [parameters, flags and options]\n");
			if(cape.Array.isEmpty(parameters) == false) {
				stdout.print("\n");
				stdout.print("Supported Parameters:\n");
				stdout.print("\n");
				if(parameters != null) {
					var n = 0;
					var m = parameters.Length;
					for(n = 0 ; n < m ; n++) {
						var parameter = parameters[n];
						if(parameter != null) {
							stdout.print(((("  " + cape.String.padToLength(parameter[0], ll)) + " - ") + parameter[1]) + "\n");
						}
					}
				}
			}
			if(cape.Array.isEmpty(flags) == false) {
				stdout.print("\n");
				stdout.print("Supported Flags:\n");
				stdout.print("\n");
				if(flags != null) {
					var n2 = 0;
					var m2 = flags.Length;
					for(n2 = 0 ; n2 < m2 ; n2++) {
						var flag = flags[n2];
						if(flag != null) {
							stdout.print(((("  -" + cape.String.padToLength(flag[0], ll)) + " - ") + flag[1]) + "\n");
						}
					}
				}
			}
			if(cape.Array.isEmpty(options) == false) {
				stdout.print("\n");
				stdout.print("Supported Options:\n");
				stdout.print("\n");
				if(options != null) {
					var n3 = 0;
					var m3 = options.Length;
					for(n3 = 0 ; n3 < m3 ; n3++) {
						var option = options[n3];
						if(option != null) {
							stdout.print(((("  -" + cape.String.padToLength(option[0] + "=<value>", ll)) + " - ") + option[1]) + "\n");
						}
					}
				}
			}
			stdout.print("\n");
		}
	}
}
