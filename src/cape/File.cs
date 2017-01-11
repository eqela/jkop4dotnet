
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
	public interface File
	{
		File entry(string name);
		File asExecutable();
		File getParent();
		File getSibling(string name);
		bool hasExtension(string ext);
		string extension();
		Iterator<File> entries();
		bool move(File dest, bool replace);
		bool rename(string newname, bool replace);
		bool touch();
		FileReader read();
		FileWriter write();
		FileWriter append();
		bool setMode(int mode);
		bool setOwnerUser(int uid);
		bool setOwnerGroup(int gid);
		bool makeExecutable();
		FileInfo stat();
		int getSize();
		bool exists();
		bool isExecutable();
		bool isFile();
		bool isDirectory();
		bool isLink();
		bool createFifo();
		bool createDirectory();
		bool createDirectoryRecursive();
		bool removeDirectory();
		string getPath();
		bool isSame(File file);
		bool remove();
		bool removeRecursive();
		bool writeFromReader(Reader reader, bool append);
		bool copyFileTo(File dest);
		int compareModificationTime(File file);
		bool isNewerThan(File file);
		bool isOlderThan(File file);
		string directoryName();
		string baseName();
		string baseNameWithoutExtension();
		bool isIdentical(File file);
		byte[] getContentsBuffer();
		string getContentsString(string encoding);
		bool setContentsBuffer(byte[] buf);
		bool setContentsString(string str, string encoding);
		bool hasChangedSince(long originalTimeStamp);
		long getLastModifiedTimeStamp();
		Iterator<string> readLines();
	}
}
