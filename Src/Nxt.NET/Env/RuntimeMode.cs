using System;
using System.IO;

namespace Nxt.NET.Env
{
    public interface RuntimeMode
    {
        void Init();

        void SetServerStatus(ServerStatus status, Uri wallet, FileInfo logFileDir);

        void LaunchDesktopApplication();

        void Shutdown();
    }
}
