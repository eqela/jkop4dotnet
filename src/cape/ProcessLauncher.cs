
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
	/// <summary>
	/// The ProcessLauncher class provides a mechanism for starting and controlling
	/// additional processes.
	/// </summary>

	public class ProcessLauncher
	{
		private class MyStringPipeHandler : BufferDataReceiver
		{
			private StringBuilder builder = null;
			private string encoding = null;

			public MyStringPipeHandler() {
				encoding = "UTF-8";
			}

			public virtual void onBufferData(byte[] data, long size) {
				if(((builder == null) || (data == null)) || (size < 1)) {
					return;
				}
				var str = cape.String.forBuffer(cape.Buffer.getSubBuffer(data, (long)0, size), encoding);
				if(object.Equals(str, null)) {
					return;
				}
				builder.append(str);
			}

			public StringBuilder getBuilder() {
				return(builder);
			}

			public MyStringPipeHandler setBuilder(StringBuilder v) {
				builder = v;
				return(this);
			}

			public string getEncoding() {
				return(encoding);
			}

			public MyStringPipeHandler setEncoding(string v) {
				encoding = v;
				return(this);
			}
		}

		private class MyBufferPipeHandler : BufferDataReceiver
		{
			private byte[] data = null;

			public virtual void onBufferData(byte[] newData, long size) {
				data = cape.Buffer.append(data, newData, size);
			}

			public byte[] getData() {
				return(data);
			}

			public MyBufferPipeHandler setData(byte[] v) {
				data = v;
				return(this);
			}
		}

		private class QuietPipeHandler : BufferDataReceiver
		{
			public virtual void onBufferData(byte[] data, long size) {
			}
		}

		public static ProcessLauncher forSelf() {
			var exe = cape.CurrentProcess.getExecutableFile();
			if(exe == null) {
				return(null);
			}
			var v = new ProcessLauncher();
			var filename = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
			if(!(object.Equals(filename, null))) {
				var ff = cape.FileInstance.forPath(filename);
				var bn = ff.baseName();
				if(!(object.Equals(bn, null)) && cape.String.startsWith(bn, "mono")) {
					v.setFile(ff);
					v.addToParams(exe.getPath());
					return(v);
				}
			}
			v.setFile(exe);
			return(v);
		}

		/// <summary>
		/// Creates a launcher for the given executable file. If the file does not exist,
		/// this method returns a null object instead.
		/// </summary>

		public static ProcessLauncher forFile(File file, string[] @params = null) {
			if((file == null) || (file.isFile() == false)) {
				return(null);
			}
			var v = new ProcessLauncher();
			v.setFile(file);
			if(@params != null) {
				var n = 0;
				var m = @params.Length;
				for(n = 0 ; n < m ; n++) {
					var param = @params[n];
					if(param != null) {
						v.addToParams(param);
					}
				}
			}
			return(v);
		}

		/// <summary>
		/// Creates a process launcher for the given command. The command can either be a
		/// full or relative path to an executable file or, if not, a matching executable
		/// file will be searched for in the PATH environment variable (or through other
		/// applicable standard means on the given platform), and an appropriately
		/// configured ProcessLauncher object will be returned. However, if the given
		/// command is not found, this method returns null.
		/// </summary>

		public static ProcessLauncher forCommand(string command, string[] @params = null) {
			if(cape.String.isEmpty(command)) {
				return(null);
			}
			File file = null;
			if(cape.String.indexOf(command, cape.Environment.getPathSeparator()) >= 0) {
				file = cape.FileInstance.forPath(command);
			}
			else {
				file = cape.Environment.findCommand(command);
			}
			return(forFile(file, @params));
		}

		/// <summary>
		/// Creates a new process launcher object for the given string, which includes a
		/// complete command line for executing the process, including the command itself
		/// and all the parameters, delimited with spaces. If parameters will need to
		/// contain space as part of their value, those parameters can be enclosed in double
		/// quotes. This method will return null if the command does not exist and/or is not
		/// found.
		/// </summary>

		public static ProcessLauncher forString(string str) {
			if(cape.String.isEmpty(str)) {
				return(null);
			}
			var arr = cape.String.quotedStringToVector(str, ' ');
			if((arr == null) || (cape.Vector.getSize(arr) < 1)) {
				return(null);
			}
			var vsz = cape.Vector.getSize(arr);
			var cmd = arr[0];
			string[] @params = null;
			var paramCount = vsz - 1;
			if(paramCount > 0) {
				@params = new string[paramCount];
				for(var n = 1 ; n < vsz ; n++) {
					@params[n - 1] = arr[n];
				}
			}
			return(forCommand(cmd, @params));
		}

		private File file = null;
		private System.Collections.Generic.List<string> @params = null;
		private System.Collections.Generic.Dictionary<string,string> env = null;
		private File cwd = null;
		private int uid = -1;
		private int gid = -1;
		private bool trapSigint = true;
		private bool replaceSelf = false;
		private bool pipePty = false;
		private bool startGroup = false;
		private bool noCmdWindow = false;
		private StringBuilder errorBuffer = null;

		public ProcessLauncher() {
			@params = new System.Collections.Generic.List<string>();
			env = new System.Collections.Generic.Dictionary<string,string>();
		}

		private void appendProperParam(StringBuilder sb, string p) {
			var noQuotes = false;
			if(cape.OS.isWindows()) {
				var rc = cape.String.lastIndexOf(p, ' ');
				if(rc < 0) {
					noQuotes = true;
				}
			}
			sb.append(' ');
			if(noQuotes) {
				sb.append(p);
			}
			else {
				sb.append('\"');
				sb.append(p);
				sb.append('\"');
			}
		}

		/// <summary>
		/// Produces a string representation of this command with the command itself,
		/// parameters and environment variables included.
		/// </summary>

		public string toString(bool includeEnv = true) {
			var sb = new StringBuilder();
			if(includeEnv) {
				System.Collections.Generic.List<string> keys = cape.Map.getKeys(env);
				if(keys != null) {
					var n = 0;
					var m = keys.Count;
					for(n = 0 ; n < m ; n++) {
						var key = keys[n];
						if(key != null) {
							sb.append(key);
							sb.append("=");
							sb.append(env[key]);
							sb.append(" ");
						}
					}
				}
			}
			sb.append("\"");
			if(file != null) {
				sb.append(file.getPath());
			}
			sb.append("\"");
			if(@params != null) {
				var n2 = 0;
				var m2 = @params.Count;
				for(n2 = 0 ; n2 < m2 ; n2++) {
					var p = @params[n2];
					if(p != null) {
						appendProperParam(sb, p);
					}
				}
			}
			return(sb.toString());
		}

		public ProcessLauncher addToParams(string arg) {
			if(!(object.Equals(arg, null))) {
				if(@params == null) {
					@params = new System.Collections.Generic.List<string>();
				}
				@params.Add(arg);
			}
			return(this);
		}

		public ProcessLauncher addToParams(File file) {
			if(file != null) {
				addToParams(file.getPath());
			}
			return(this);
		}

		public ProcessLauncher addToParams(System.Collections.Generic.List<string> @params) {
			if(@params != null) {
				var n = 0;
				var m = @params.Count;
				for(n = 0 ; n < m ; n++) {
					var s = @params[n];
					if(s != null) {
						addToParams(s);
					}
				}
			}
			return(this);
		}

		public void setEnvVariable(string key, string val) {
			if(!(object.Equals(key, null))) {
				if(env == null) {
					env = new System.Collections.Generic.Dictionary<string,string>();
				}
				env[key] = val;
			}
		}

		private Process startProcess(bool wait, BufferDataReceiver pipeHandler, bool withIO = false) {
			return(cape.ProcessLauncherNetX.startProcess(this, wait, pipeHandler, withIO, errorBuffer));
			System.Console.WriteLine("[cape.ProcessLauncher.startProcess] (ProcessLauncher.sling:278:2): Not implemented");
			return(null);
		}

		public Process start() {
			return(startProcess(false, null));
		}

		public ProcessWithIO startWithIO() {
			return(startProcess(false, null, true) as ProcessWithIO);
		}

		public int execute() {
			var cp = startProcess(true, null);
			if(cp == null) {
				return(-1);
			}
			return(cp.getExitStatus());
		}

		public int executeSilent() {
			var cp = startProcess(true, (BufferDataReceiver)new QuietPipeHandler());
			if(cp == null) {
				return(-1);
			}
			return(cp.getExitStatus());
		}

		public int executeToStringBuilder(StringBuilder output) {
			var msp = new MyStringPipeHandler();
			msp.setBuilder(output);
			var cp = startProcess(true, (BufferDataReceiver)msp);
			if(cp == null) {
				return(-1);
			}
			return(cp.getExitStatus());
		}

		public string executeToString() {
			var sb = new StringBuilder();
			if(executeToStringBuilder(sb) < 0) {
				return(null);
			}
			return(sb.toString());
		}

		public byte[] executeToBuffer() {
			var ph = new MyBufferPipeHandler();
			var cp = startProcess(true, (BufferDataReceiver)ph);
			if(cp == null) {
				return(null);
			}
			return(ph.getData());
		}

		public int executeToPipe(BufferDataReceiver pipeHandler) {
			var cp = startProcess(true, pipeHandler);
			if(cp == null) {
				return(-1);
			}
			return(cp.getExitStatus());
		}

		public File getFile() {
			return(file);
		}

		public ProcessLauncher setFile(File v) {
			file = v;
			return(this);
		}

		public System.Collections.Generic.List<string> getParams() {
			return(@params);
		}

		public ProcessLauncher setParams(System.Collections.Generic.List<string> v) {
			@params = v;
			return(this);
		}

		public System.Collections.Generic.Dictionary<string,string> getEnv() {
			return(env);
		}

		public ProcessLauncher setEnv(System.Collections.Generic.Dictionary<string,string> v) {
			env = v;
			return(this);
		}

		public File getCwd() {
			return(cwd);
		}

		public ProcessLauncher setCwd(File v) {
			cwd = v;
			return(this);
		}

		public int getUid() {
			return(uid);
		}

		public ProcessLauncher setUid(int v) {
			uid = v;
			return(this);
		}

		public int getGid() {
			return(gid);
		}

		public ProcessLauncher setGid(int v) {
			gid = v;
			return(this);
		}

		public bool getTrapSigint() {
			return(trapSigint);
		}

		public ProcessLauncher setTrapSigint(bool v) {
			trapSigint = v;
			return(this);
		}

		public bool getReplaceSelf() {
			return(replaceSelf);
		}

		public ProcessLauncher setReplaceSelf(bool v) {
			replaceSelf = v;
			return(this);
		}

		public bool getPipePty() {
			return(pipePty);
		}

		public ProcessLauncher setPipePty(bool v) {
			pipePty = v;
			return(this);
		}

		public bool getStartGroup() {
			return(startGroup);
		}

		public ProcessLauncher setStartGroup(bool v) {
			startGroup = v;
			return(this);
		}

		public bool getNoCmdWindow() {
			return(noCmdWindow);
		}

		public ProcessLauncher setNoCmdWindow(bool v) {
			noCmdWindow = v;
			return(this);
		}

		public StringBuilder getErrorBuffer() {
			return(errorBuffer);
		}

		public ProcessLauncher setErrorBuffer(StringBuilder v) {
			errorBuffer = v;
			return(this);
		}
	}
}
