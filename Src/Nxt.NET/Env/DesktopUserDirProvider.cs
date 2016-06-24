using System;
using System.Collections.Generic;
using System.IO;

namespace Nxt.NET.Env
{
    public abstract class DesktopUserDirProvider : DirProvider
    {
        public DirectoryInfo GetConfDir()
        {
            return new DirectoryInfo(Path.Combine(GetUserHomeDir(), "conf"));
        }

        public DirectoryInfo GetDefaultConfDir()
        {
            return new DirectoryInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "conf"));
        }

        public string GetDbDir(string dbDir)
        {
            return Path.Combine(GetUserHomeDir(), dbDir);
        }

        public DirectoryInfo GetLogFileDir()
        {
            return new DirectoryInfo(Path.Combine(GetUserHomeDir(), "logs"));
        }

        public bool IsLoadPropertyFileFromUserDir()
        {
            return true;
        }

        public void UpdateLogFileHandler(Dictionary<string, string> loggingProperties)
        {
        }

        public abstract string GetUserHomeDir();
    }
}
