
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
	public class ProcessLauncherNetX
	{
		private class MyProcess : Process, ProcessWithIO
		{
			public System.Diagnostics.Process process = null;
			private int exitCode = 0;
			private Writer stdin = null;
			private Reader stdout = null;
			private Reader stderr = null;

			public void close() {
				if(process != null) {
					exitCode = process.ExitCode;
					process.Close();
					process = null;
				}
			}

			public virtual Writer getStandardInput() {
				if(stdin == null) {
					System.IO.Stream stream = null;
					stream = process.StandardInput.BaseStream;
					stdin = (Writer)cape.DotNetStreamWriter.forStream(stream);
				}
				return(stdin);
			}

			public virtual Reader getStandardOutput() {
				if(stdout == null) {
					System.IO.Stream stream = null;
					stream = process.StandardOutput.BaseStream;
					stdout = (Reader)cape.DotNetStreamReader.forStream(stream);
				}
				return(stdout);
			}

			public virtual Reader getStandardError() {
				if(stderr == null) {
					System.IO.Stream stream = null;
					stream = process.StandardError.BaseStream;
					stderr = (Reader)cape.DotNetStreamReader.forStream(stream);
				}
				return(stderr);
			}

			public virtual string getId() {
				var id = 0;
				if(process != null) {
					id = process.Id;
				}
				return(cape.String.forInteger(id));
			}

			public virtual bool isRunning() {
				var v = false;
				if(process != null) {
					try {
						int x = process.ExitCode;
					}
					catch {
						v = true;
					}
				}
				return(v);
			}

			public virtual int getExitStatus() {
				if(isRunning()) {
					return(-1);
				}
				if(process != null) {
					int v;
					try {
						v = process.ExitCode;
					}
					catch {
						v = -1;
					}
					exitCode = v;
				}
				close();
				return(exitCode);
			}

			public virtual void sendInterrupt() {
			}

			public virtual void killRequest() {
				killForce();
			}

			public virtual void killForce() {
				if(process != null) {
					try {
						process.Kill();
					}
					catch {
					}
				}
			}

			public virtual void kill(int timeout) {
				killForce();
				if(process != null) {
					process.WaitForExit(timeout);
				}
			}

			public virtual int waitForExit() {
				if(process != null) {
					process.WaitForExit();
				}
				return(getExitStatus());
			}
		}

		public static Process startProcess(ProcessLauncher launcher, bool wait, BufferDataReceiver pipeHandler, bool withIO, StringBuilder errorBuffer) {
			var ff = launcher.getFile();
			if(ff == null) {
				return(null);
			}
			var sb = new StringBuilder();
			var array = launcher.getParams();
			if(array != null) {
				var n = 0;
				var m = array.Count;
				for(n = 0 ; n < m ; n++) {
					var param = array[n];
					if(param != null) {
						if(sb.count() > 0) {
							sb.append(' ');
						}
						sb.append('\"');
						sb.append(param);
						sb.append('\"');
					}
				}
			}
			var sbs = sb.toString();
			if(object.Equals(sbs, null)) {
				sbs = "";
			}
			var np = new MyProcess();
			string cwdp = null;
			var cwd = launcher.getCwd();
			if(cwd != null) {
				cwdp = cwd.getPath();
			}
			System.Diagnostics.ProcessStartInfo pi = new System.Diagnostics.ProcessStartInfo();
			pi.UseShellExecute = false;
			pi.FileName = ff.getPath();
			if(withIO) {
				pi.RedirectStandardOutput = true;
				pi.RedirectStandardError = true;
				pi.RedirectStandardInput = true;
			}
			else if(pipeHandler != null) {
				pi.RedirectStandardOutput = true;
				pi.RedirectStandardError = true;
				pi.RedirectStandardInput = false;
			}
			else {
				pi.RedirectStandardOutput = false;
				pi.RedirectStandardError = false;
				pi.RedirectStandardInput = false;
			}
			pi.Arguments = sbs;
			pi.WorkingDirectory = cwdp;
			var env = launcher.getEnv();
			if((env != null) && (cape.Map.count(env) > 0)) {
				System.Collections.Generic.List<string> keys = cape.Map.getKeys(env);
				if(keys != null) {
					var n2 = 0;
					var m2 = keys.Count;
					for(n2 = 0 ; n2 < m2 ; n2++) {
						var key = keys[n2];
						if(key != null) {
							var val = env[key];
							if(object.Equals(val, null)) {
								val = "";
							}
							pi.EnvironmentVariables[key] = val;
						}
					}
				}
			}
			try {
				np.process = System.Diagnostics.Process.Start(pi);
			}
			catch(System.Exception e) {
				if(errorBuffer != null) {
					errorBuffer.append(e.ToString());
				}
				np = null;
			}
			if(np == null) {
				return(null);
			}
			if(wait) {
				if(pipeHandler != null) {
					try {
						var bb0 = new byte[1024];
						var bb1 = new byte[1024];
						System.IO.Stream sro = np.process.StandardOutput.BaseStream;
						System.IO.Stream sre = np.process.StandardError.BaseStream;
						var tasks = new System.Threading.Tasks.Task<int>[2];
						tasks[0] = sro.ReadAsync(bb0, 0, 1024);
						tasks[1] = sro.ReadAsync(bb1, 0, 1024);
						while(true) {
							var tt = System.Threading.Tasks.Task.WaitAny(tasks);
							if(tt == 0) {
								if(tasks[0].Result < 1) {
									break;
								}
								pipeHandler.onBufferData(bb0, tasks[0].Result);
								tasks[0] = sro.ReadAsync(bb0, 0, 1024);
							}
							else if(tt == 1) {
								if(tasks[1].Result < 1) {
									break;
								}
								pipeHandler.onBufferData(bb1, tasks[1].Result);
								tasks[1] = sro.ReadAsync(bb1, 0, 1024);
							}
							else {
								break;
							}
						}
					}
					catch(System.Exception e) {
						if(errorBuffer != null) {
							errorBuffer.append(e.ToString());
						}
					}
				}
				np.waitForExit();
				np.close();
			}
			return((Process)np);
		}
	}
}
