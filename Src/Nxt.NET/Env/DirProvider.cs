using System.Collections.Generic;
using System.IO;

namespace Nxt.NET.Env
{
    public interface DirProvider
    {
        bool IsLoadPropertyFileFromUserDir();

        void UpdateLogFileHandler(Dictionary<string, string> loggingProperties);

        string GetDbDir(string dbDir);

        DirectoryInfo GetLogFileDir();

        DirectoryInfo GetConfDir();

        DirectoryInfo GetDefaultConfDir();

        string GetUserHomeDir();
    }
}