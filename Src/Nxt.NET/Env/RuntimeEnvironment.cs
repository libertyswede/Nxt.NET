namespace Nxt.NET.Env
{
    public class RuntimeEnvironment
    {
        public const string DIRPROVIDER_ARG = "nxt.net.runtime.dirProvider";

        public static RuntimeMode GetRuntimeMode()
        {
            return new CommandLineMode();
        }

        public static DirProvider GetDirProvider()
        {
            return new WindowsUserDirProvider();
        }
    }
}
