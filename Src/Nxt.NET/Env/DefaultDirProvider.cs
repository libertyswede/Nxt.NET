using System;
using System.Collections.Generic;
using System.IO;

namespace Nxt.NET.Env
{
    public class DefaultDirProvider : DirProvider
    {
        public DirectoryInfo GetConfDir()
        {
            return new DirectoryInfo(Path.Combine(GetUserHomeDir(), "conf"));
        }

        public string GetDbDir(string dbDir)
        {
            return Path.Combine(GetUserHomeDir(), dbDir);
        }

        public DirectoryInfo GetDefaultConfDir()
        {
            return GetConfDir();
        }

        public DirectoryInfo GetLogFileDir()
        {
            return new DirectoryInfo(Path.Combine(GetUserHomeDir(), "logs"));
        }

        public string GetUserHomeDir()
        {
            return new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory).FullName;
        }

        public bool IsLoadPropertyFileFromUserDir()
        {
            return false;
        }

        public void UpdateLogFileHandler(Dictionary<string, string> loggingProperties)
        {
        }
    }
}