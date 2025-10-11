using Giis.Portable.Util;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace Giis.Selema.Portable.Selenium
{
    public static class CommandLine
    {
        /// <summary>
        /// Runs a shell/cmd command and returns a string with the standard output
        /// </summary>
        public static string RunCommand(string command)
        {
            try
            {
                return Run(command);
            }
            catch (Exception)
            {
                throw new PortableException("Can't execute command: " + command);
            }
        }

        private static string Run(string command)
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = GetShell(),
                    Arguments = GetShellArgs(command),
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };
            process.Start();
            string stdout = process.StandardOutput.ReadToEnd();
            string stderr = process.StandardError.ReadToEnd();
            process.WaitForExit();

            return string.Join(stdout, stderr, "\n").Trim();
        }

        private static bool IsWindows()
        {
            return RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
        }

        private static string GetShell()
        {
            return IsWindows() ? "cmd.exe" : "/bin/bash";
        }

        private static string GetShellArgs(string command)
        {
            return IsWindows() ? $"/c {command}" : $"-c \"{command}\"";
        }

        // Temporal methods, to be included later in the portable component

        public static void FileDelete(string fileName, bool throwIfNotExists)
        {
            try
            {
                if (File.Exists(fileName))
                    File.Delete(fileName);
                else if (throwIfNotExists)
                    throw new PortableException("File to delete does not exist: " + fileName);
            }
            catch (Exception e)
            {
                throw new PortableException(e);
            }
        }

        public static void FileCopy(string fileFrom, string fileTo)
        {
            try
            {
                File.Copy(fileFrom, fileTo);
            }
            catch (Exception)
            {
                throw new PortableException("Can't copy " + fileFrom + " to " + fileTo);
            }
        }

        public static bool FileExists(string fileName)
        {
            return File.Exists(fileName);
        }

        public static bool IsAbsolute(string fileName)
        {
            return fileName.StartsWith("/") ||
                    fileName.Length >= 3 && fileName[1] == ':' && fileName[2] == '/';
        }



    }
}
