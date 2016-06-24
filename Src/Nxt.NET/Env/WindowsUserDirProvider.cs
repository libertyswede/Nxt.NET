using System;
using System.IO;

namespace Nxt.NET.Env
{
    public class WindowsUserDirProvider : DesktopUserDirProvider
    {
        private readonly string NXT_USER_HOME = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Nxt.NET");

        public override string GetUserHomeDir()
        {
            return NXT_USER_HOME;
        }
    }
}
