
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

namespace sympathy
{
	public class IOManagerDotNetSelect : IOManager
	{
		private class MyTimer : IOManagerTimer, cape.Runnable
		{
			private long delay = (long)0;
			private IOManagerDotNetSelect parent = null;
			private System.Func<bool> handler = null;
			private bool stopFlag = false;
			private cape.TimeValue targetTime = null;

			public bool hasElapsed(cape.TimeValue now) {
				if(cape.TimeValue.diff(targetTime, now) <= 0) {
					return(true);
				}
				return(false);
			}

			public bool process(cape.TimeValue now) {
				if(handler == null) {
					return(false);
				}
				if(hasElapsed(now) == false) {
					return(true);
				}
				if(handler() == false) {
					return(false);
				}
				targetTime = null;
				if(initialize() == false) {
					return(false);
				}
				return(true);
			}

			public virtual void stop() {
				stopFlag = true;
			}

			public virtual void run() {
				stopFlag = false;
				var now = new cape.TimeValue();
				while(stopFlag == false) {
					cape.SystemClock.update(now);
					var diff = cape.TimeValue.diff(targetTime, now);
					if(diff <= 0) {
						break;
					}
					if(diff > 5000000) {
						diff = (long)5000000;
					}
					cape.CurrentThread.sleepMicroSeconds((int)diff);
				}
				parent.sendSignal();
			}

			public bool initialize() {
				if(targetTime != null) {
					return(false);
				}
				var orig = cape.SystemClock.asTimeValue();
				targetTime = orig.add((long)0, delay);
				if(cape.Thread.start((cape.Runnable)this) == false) {
					return(false);
				}
				return(true);
			}

			public long getDelay() {
				return(delay);
			}

			public MyTimer setDelay(long v) {
				delay = v;
				return(this);
			}

			public IOManagerDotNetSelect getParent() {
				return(parent);
			}

			public MyTimer setParent(IOManagerDotNetSelect v) {
				parent = v;
				return(this);
			}

			public System.Func<bool> getHandler() {
				return(handler);
			}

			public MyTimer setHandler(System.Func<bool> v) {
				handler = v;
				return(this);
			}
		}

		private class MyEntry : IOManagerEntry
		{
			private TCPSocket socket = null;
			private IOManagerDotNetSelect master = null;
			private System.Action rrl = null;
			private System.Action wrl = null;
			private bool added = false;

			public void onReadReady() {
				var rrl = this.rrl;
				if(rrl != null) {
					rrl();
				}
			}

			public void onWriteReady() {
				var wrl = this.wrl;
				if(wrl != null) {
					wrl();
				}
			}

			public virtual void setListeners(System.Action rrl, System.Action wrl) {
				this.rrl = rrl;
				this.wrl = wrl;
				update();
			}

			public virtual void setReadListener(System.Action rrl) {
				this.rrl = rrl;
				update();
			}

			public virtual void setWriteListener(System.Action wrl) {
				this.wrl = wrl;
				update();
			}

			public void update() {
				remove();
				if((socket == null) || (master == null)) {
					return;
				}
				if((rrl == null) && (wrl == null)) {
					return;
				}
				if(rrl != null) {
					cape.Vector.append(master.readlist, this);
				}
				if(wrl != null) {
					cape.Vector.append(master.writelist, this);
				}
				added = true;
			}

			public virtual void remove() {
				if((added == false) || (master == null)) {
					return;
				}
				cape.Vector.removeValue(master.readlist, this);
				cape.Vector.removeValue(master.writelist, this);
				added = false;
			}

			public System.Net.Sockets.Socket getNetSocket() {
				var myss = socket as TCPSocketImpl;
				if(myss == null) {
					return(null);
				}
				return(myss.getNetSocket());
			}

			public TCPSocket getSocket() {
				return(socket);
			}

			public MyEntry setSocket(TCPSocket v) {
				socket = v;
				return(this);
			}

			public IOManagerDotNetSelect getMaster() {
				return(master);
			}

			public MyEntry setMaster(IOManagerDotNetSelect v) {
				master = v;
				return(this);
			}
		}

		private bool exitflag = false;
		private bool running = false;
		private int signalPort = -1;
		public System.Collections.Generic.List<IOManagerEntry> readlist = null;
		public System.Collections.Generic.List<IOManagerEntry> writelist = null;
		private System.Collections.Generic.List<MyTimer> timers = null;

		public IOManagerDotNetSelect() {
			readlist = new System.Collections.Generic.List<IOManagerEntry>();
			writelist = new System.Collections.Generic.List<IOManagerEntry>();
			timers = new System.Collections.Generic.List<MyTimer>();
		}

		public IOManagerEntry getEntryForSocket(System.Net.Sockets.Socket socket, System.Collections.Generic.List<IOManagerEntry> list) {
			if(list != null) {
				var n = 0;
				var m = list.Count;
				for(n = 0 ; n < m ; n++) {
					var ee = list[n] as MyEntry;
					if(ee != null) {
						if(ee.getNetSocket() == socket) {
							return((IOManagerEntry)ee);
						}
					}
				}
			}
			return(null);
		}

		public void onReadReady(TCPSocket socket) {
			if(socket == null) {
				return;
			}
			var ss = socket.accept();
			if(ss != null) {
				ss.close();
			}
		}

		public override bool execute(cape.LoggingContext ctx) {
			exitflag = false;
			running = true;
			var signalSocket = openSignalSocket();
			if(signalSocket != null) {
				var ee = add((object)signalSocket);
				if(ee != null) {
					ee.setReadListener(() => {
						onReadReady(signalSocket);
					});
				}
			}
			var reads = new System.Collections.Generic.List<object>();
			var writes = new System.Collections.Generic.List<object>();
			var now = new cape.TimeValue();
			cape.Log.debug(ctx, "IOManagerDotNetSelect" + " started");
			while(exitflag == false) {
				if(executeSelect(ctx, reads, writes) == false) {
					continue;
				}
				if(reads != null) {
					var n = 0;
					var m = reads.Count;
					for(n = 0 ; n < m ; n++) {
						var ele = reads[n] as MyEntry;
						if(ele != null) {
							ele.onReadReady();
						}
					}
				}
				if(writes != null) {
					var n2 = 0;
					var m2 = writes.Count;
					for(n2 = 0 ; n2 < m2 ; n2++) {
						var ele1 = writes[n2] as MyEntry;
						if(ele1 != null) {
							ele1.onWriteReady();
						}
					}
				}
				cape.Vector.clear(reads);
				cape.Vector.clear(writes);
				if(cape.Vector.isEmpty(timers) == false) {
					cape.SystemClock.update(now);
					System.Collections.Generic.List<MyTimer> timersToRemove = null;
					if(timers != null) {
						var n3 = 0;
						var m3 = timers.Count;
						for(n3 = 0 ; n3 < m3 ; n3++) {
							var timer = timers[n3];
							if(timer != null) {
								if(timer.process(now) == false) {
									if(timersToRemove == null) {
										timersToRemove = new System.Collections.Generic.List<MyTimer>();
									}
									timersToRemove.Add(timer);
								}
							}
						}
					}
					if(timersToRemove != null) {
						var n4 = 0;
						var m4 = timersToRemove.Count;
						for(n4 = 0 ; n4 < m4 ; n4++) {
							var timer1 = timersToRemove[n4];
							if(timer1 != null) {
								cape.Vector.removeValue(timers, timer1);
							}
						}
					}
				}
			}
			if(signalSocket != null) {
				signalSocket.close();
				signalSocket = null;
			}
			cape.Vector.clear(readlist);
			cape.Vector.clear(writelist);
			signalPort = -1;
			running = false;
			cape.Log.debug(ctx, "IOManagerDotNetSelect" + " ended");
			return(true);
		}

		private bool executeSelect(cape.LoggingContext ctx, System.Collections.Generic.List<object> reads, System.Collections.Generic.List<object> writes) {
			var fdsetr = new System.Collections.ArrayList();
			var fdsetw = new System.Collections.ArrayList();
			if(readlist != null) {
				var n = 0;
				var m = readlist.Count;
				for(n = 0 ; n < m ; n++) {
					var myo1 = readlist[n] as MyEntry;
					if(myo1 != null) {
						fdsetr.Add(myo1.getNetSocket());
					}
				}
			}
			if(writelist != null) {
				var n2 = 0;
				var m2 = writelist.Count;
				for(n2 = 0 ; n2 < m2 ; n2++) {
					var myo2 = writelist[n2] as MyEntry;
					if(myo2 != null) {
						fdsetw.Add(myo2.getNetSocket());
					}
				}
			}
			try {
				System.Net.Sockets.Socket.Select(fdsetr, fdsetw, null, -1);
			}
			catch(System.Exception e) {
				cape.Log.error(ctx, "Call to Select failed: " + e.ToString());
				return(false);
			}
			foreach(System.Net.Sockets.Socket socket in fdsetr) {
				var e = getEntryForSocket(socket, readlist);
				if(e != null) {
					cape.Vector.append(reads, e);
				}
			}
			foreach(System.Net.Sockets.Socket socket in fdsetw) {
				var e = getEntryForSocket(socket, writelist);
				if(e != null) {
					cape.Vector.append(writes, e);
				}
			}
			return(true);
		}

		public TCPSocket openSignalSocket() {
			var v = sympathy.TCPSocket.create();
			var n = 0;
			for(n = 20000 ; n < 60000 ; n++) {
				if(v.listen(n)) {
					signalPort = n;
					return(v);
				}
			}
			return(null);
		}

		public override void stop() {
			exitflag = true;
			sendSignal();
		}

		public void sendSignal() {
			if(signalPort > 0) {
				var signalClient = sympathy.TCPSocket.createAndConnect("127.0.0.1", signalPort);
				if(signalClient != null) {
					signalClient.close();
				}
			}
		}

		public bool isRunning() {
			return(running);
		}

		public override IOManagerEntry add(object o) {
			var ss = o as TCPSocket;
			if(ss == null) {
				return(null);
			}
			return((IOManagerEntry)new MyEntry().setMaster(this).setSocket(ss));
		}

		public override IOManagerTimer startTimer(long delay, System.Func<bool> handler) {
			var timer = new MyTimer();
			timer.setDelay(delay);
			timer.setParent(this);
			timer.setHandler(handler);
			if(timer.initialize() == false) {
				return(null);
			}
			timers.Add(timer);
			return((IOManagerTimer)timer);
		}
	}
}
