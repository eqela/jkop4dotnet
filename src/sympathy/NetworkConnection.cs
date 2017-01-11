
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
	public abstract class NetworkConnection
	{
		private int storageIndex = 0;
		private IOManagerEntry ioEntry = null;
		private long id = (long)0;
		protected cape.LoggingContext logContext = null;
		protected ConnectedSocket socket = null;
		private static int idcounter = 0;
		protected long lastActivity = (long)0;
		private string remoteAddress = null;
		private int currentListenMode = -1;
		private NetworkConnectionManager manager = null;
		private int defaultListenMode = 1;

		public NetworkConnection() {
			id = (long)idcounter++;
		}

		public long getId() {
			return(id);
		}

		public ConnectedSocket getSocket() {
			return(socket);
		}

		public NetworkConnectionManager getManager() {
			return(manager);
		}

		public long getLastActivity() {
			return(lastActivity);
		}

		public string getRemoteAddress() {
			if(object.Equals(remoteAddress, null)) {
				var ts = socket as TCPSocket;
				if(ts != null) {
					remoteAddress = (ts.getRemoteAddress() + ":") + cape.String.forInteger(ts.getRemotePort());
				}
			}
			return(remoteAddress);
		}

		public void logDebug(string text) {
			cape.Log.debug(logContext, (("[Connection:" + cape.String.forInteger((int)getId())) + "] ") + text);
		}

		public void logError(string text) {
			cape.Log.error(logContext, (("[Connection:" + cape.String.forInteger((int)getId())) + "] ") + text);
		}

		public void onActivity() {
			lastActivity = cape.SystemClock.asSeconds();
		}

		public virtual bool initialize() {
			return(true);
		}

		public virtual void cleanup() {
		}

		public bool doInitialize(cape.LoggingContext logContext, ConnectedSocket socket, NetworkConnectionManager manager) {
			this.logContext = logContext;
			this.socket = socket;
			this.manager = manager;
			if(initialize() == false) {
				return(false);
			}
			onActivity();
			return(true);
		}

		public IOManagerEntry getIoEntry() {
			return(ioEntry);
		}

		public void setIoEntry(IOManagerEntry entry) {
			this.ioEntry = entry;
			this.currentListenMode = -1;
			if(entry != null) {
				setListenMode(getDefaultListenMode());
			}
		}

		public int sendData(byte[] data, int size) {
			if(socket == null) {
				return(0);
			}
			var v = socket.write(data, size);
			onActivity();
			return(v);
		}

		public void close() {
			if(socket == null) {
				return;
			}
			var ss = socket;
			socket = null;
			if(ioEntry != null) {
				ioEntry.remove();
				ioEntry = null;
			}
			ss.close();
			if(manager != null) {
				manager.onConnectionClosed(this);
			}
			cleanup();
			onClosed();
			this.socket = null;
			this.manager = null;
		}

		public virtual void onReadReady() {
			if(socket == null) {
				return;
			}
			byte[] recvBuffer = null;
			if(manager != null) {
				recvBuffer = manager.getReceiveBuffer();
			}
			if(recvBuffer == null) {
				recvBuffer = new byte[1024];
			}
			var n = socket.read(recvBuffer);
			if(n < 0) {
				close();
			}
			else {
				onDataReceived(recvBuffer, n);
			}
			onActivity();
		}

		public virtual void onWriteReady() {
		}

		public void enableIdleMode() {
			setListenMode(0);
		}

		public void enableReadMode() {
			setListenMode(1);
		}

		public void enableWriteMode() {
			setListenMode(2);
		}

		public void enableReadWriteMode() {
			setListenMode(3);
		}

		private void setListenMode(int n) {
			if(ioEntry == null) {
				defaultListenMode = n;
				return;
			}
			if(n == currentListenMode) {
				return;
			}
			currentListenMode = n;
			if(n == 0) {
				ioEntry.setListeners(null, null);
			}
			else if(n == 1) {
				ioEntry.setListeners(onReadReady, null);
			}
			else if(n == 2) {
				ioEntry.setListeners(null, onWriteReady);
			}
			else if(n == 3) {
				ioEntry.setListeners(onReadReady, onWriteReady);
			}
		}

		public abstract void onOpened();
		public abstract void onDataReceived(byte[] data, int size);
		public abstract void onClosed();
		public abstract void onError(string message);

		public int getStorageIndex() {
			return(storageIndex);
		}

		public NetworkConnection setStorageIndex(int v) {
			storageIndex = v;
			return(this);
		}

		public int getDefaultListenMode() {
			return(defaultListenMode);
		}

		public NetworkConnection setDefaultListenMode(int v) {
			defaultListenMode = v;
			return(this);
		}
	}
}
