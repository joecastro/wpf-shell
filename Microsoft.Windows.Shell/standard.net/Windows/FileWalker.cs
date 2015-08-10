// Cribbed heavily from MSDN magazine's .Net Matters (12/2005) by Stephen Toub.
// http://msdn.microsoft.com/msdnmag/issues/05/12/NETMatters/
// This started off an excellent implementation, I'd rather not unnecessarily rewrite it.
// Some small stylistic changes were made to make it more consistent with the rest of the code
// and also changed the recurse criteria.

namespace Standard
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Security.Permissions;

    internal class FileWalker
    {
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public static IEnumerable<FileInfo> GetFiles(DirectoryInfo startDirectory, string pattern, bool recurse)
        {
            // We suppressed this demand for each p/invoke call, so demand it upfront once
            new SecurityPermission(SecurityPermissionFlag.UnmanagedCode).Demand();

            // Validate parameters
            Verify.IsNotNull(startDirectory, "startDirectory");
            Verify.IsNeitherNullNorEmpty(pattern, "pattern");

            // Setup
            var findData = new WIN32_FIND_DATAW();
            var directories = new Stack<DirectoryInfo>();
            directories.Push(startDirectory);

            // Process each directory.  Only push new directories if we're recursing.
            ErrorModes origErrorMode = NativeMethods.SetErrorMode(ErrorModes.FailCriticalErrors);
            try
            {
                while (directories.Count > 0)
                {
                    // Get the name of the next directory and the corresponding search pattern
                    DirectoryInfo dir = directories.Pop();
                    string dirPath = dir.FullName.Trim();
                    if (dirPath.Length == 0)
                    {
                        continue;
                    }
                    char lastChar = dirPath[dirPath.Length - 1];
                    if (lastChar != Path.DirectorySeparatorChar && lastChar != Path.AltDirectorySeparatorChar)
                    {
                        dirPath += Path.DirectorySeparatorChar;
                    }

                    // Process all files in that directory
                    using (SafeFindHandle handle = NativeMethods.FindFirstFileW(dirPath + pattern, findData))
                    {
                        Win32Error error;
                        if (handle.IsInvalid)
                        {
                            error = Win32Error.GetLastError();
                            if (error == Win32Error.ERROR_ACCESS_DENIED || error == Win32Error.ERROR_FILE_NOT_FOUND)
                            {
                                continue;
                            }
                            Assert.AreNotEqual(Win32Error.ERROR_SUCCESS, error);
                            ((HRESULT)error).ThrowIfFailed();
                        }

                        do
                        {
                            if (!Utility.IsFlagSet((int)findData.dwFileAttributes, (int)FileAttributes.Directory))
                            {
                                yield return new FileInfo(dirPath + findData.cFileName);
                            }
                        }
                        while (NativeMethods.FindNextFileW(handle, findData));
                        error = Win32Error.GetLastError();
                        if (error != Win32Error.ERROR_NO_MORE_FILES)
                        {
                            ((HRESULT)error).ThrowIfFailed();
                        }
                    }

                    // Push subdirectories onto the stack if we are recursing.
                    if (recurse)
                    {
                        // In a volatile system we can't count on all the file information staying valid.
                        // Catch reasonable exceptions and move on.
                        try
                        {
                            foreach (DirectoryInfo childDir in dir.GetDirectories())
                            {
                                try
                                {
                                    FileAttributes attrib = File.GetAttributes(childDir.FullName);
                                    // If it's not a hidden, system folder, nor a reparse point
                                    if (!Utility.IsFlagSet((int)attrib, (int)(FileAttributes.Hidden | FileAttributes.System | FileAttributes.ReparsePoint)))
                                    {
                                        directories.Push(childDir);
                                    }
                                }
                                catch (FileNotFoundException)
                                {
                                    // Shouldn't see this.
                                    Assert.Fail();
                                }
                                catch (DirectoryNotFoundException) { }
                            }
                        }
                        catch (DirectoryNotFoundException) { }
                    }
                }
            }
            finally
            { 
                NativeMethods.SetErrorMode(origErrorMode);
            }
        }
    }
}
