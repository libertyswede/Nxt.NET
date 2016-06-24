using Nxt.NET.Env;
using Nxt.NET.Util;
using System;
using System.Collections.Generic;
using System.IO;

namespace Nxt.NET
{
    class Nxt
    {
        public const string VERSION = "1.9.1e";
        public const string APPLICATION = "NRS.NET";

        //private static volatile Time time = new Time.EpochTime();

        public const string NXT_DEFAULT_PROPERTIES = "nxt.net-default.properties";
        public const string NXT_PROPERTIES = "nxt.net.properties";
        public const string CONFIG_DIR = "conf";

        private static readonly RuntimeMode runtimeMode;
        private static readonly DirProvider dirProvider;
        
        private static readonly Dictionary<string, string> properties = new Dictionary<string, string>();

        static Nxt()
        {
            RedirectSystemStreams("out");
            RedirectSystemStreams("error");
            Console.Out.WriteLine("Initializing Nxt server version " + VERSION);
            //PrintCommandLineArguments(); // Java specific memory stuff
            runtimeMode = RuntimeEnvironment.GetRuntimeMode();
            Console.Out.WriteLine($"Runtime mode {runtimeMode.GetType().Name}");
            dirProvider = RuntimeEnvironment.GetDirProvider();
            Console.Out.WriteLine($"User home folder {dirProvider.GetUserHomeDir()}");

            LoadProperties(properties, NXT_DEFAULT_PROPERTIES, true);
            LoadProperties(properties, NXT_PROPERTIES, false);
            if (!VERSION.Equals(properties["nxt.version"]))
            {
                throw new ApplicationException("Using an nxt.net-default.properties file from a version other than " + VERSION + " is not supported!!!");
            }
        }

        private static void LoadProperties(Dictionary<string, string> properties, string propertiesFile, bool isDefault)
        {
            try
            {
                try
                {
                    var homeDir = dirProvider.GetUserHomeDir();
                    if (!Directory.Exists(homeDir))
                    {
                        Console.Out.WriteLine($"Creating dir {homeDir}");
                        Directory.CreateDirectory(homeDir);
                    }
                    var confDir = isDefault ? dirProvider.GetDefaultConfDir() : dirProvider.GetConfDir();
                    if (!Directory.Exists(confDir.FullName))
                    {
                        Console.Out.WriteLine($"Creating dir {confDir.FullName}");
                        Directory.CreateDirectory(confDir.FullName);
                    }
                    var propPath = Path.Combine(confDir.FullName, propertiesFile);
                    if (File.Exists(propPath))
                    {
                        Console.Out.WriteLine($"Loading {propertiesFile} from dir {confDir}");
                        using (var stream = File.OpenRead(propPath))
                        {
                            new PropertiesReader(properties).Parse(stream);
                        }
                    }
                    else
                    {
                        Console.Out.WriteLine($"Creating property file {propPath}");
                        using (var stream = File.CreateText(propPath))
                        {
                            stream.Write("# use this file for workstation specific " + propertiesFile);
                        }
                    }
                }
                catch (IOException e)
                {
                    throw new ArgumentException("Error loading " + propertiesFile, e);
                }
            }
            catch (ArgumentException e)
            {
                Console.Error.WriteLine(e.StackTrace);
                throw e;
            }
        }

        private static void RedirectSystemStreams(string streamName)
        {
            var isStandardRedirect = Environment.GetEnvironmentVariable("nxt.net.redirect.console." + streamName);
            FileInfo path = null;
            if (isStandardRedirect != null)
            {
                try
                {
                    path = new FileInfo(Path.GetTempPath() + "nxt.net.console." + streamName + ".log");
                }
                catch (IOException e)
                {
                    Console.Out.WriteLine(e.StackTrace);
                    return;
                }
            }
            else
            {
                var explicitFileName = Environment.GetEnvironmentVariable("nxt.net.console." + streamName);
                if (explicitFileName != null)
                {
                    path = new FileInfo(explicitFileName);
                }
            }
            if (path != null)
            {
                try
                {
                    var writer = new StreamWriter(path.Open(FileMode.OpenOrCreate, FileAccess.Write));
                    writer.AutoFlush = true;
                    if (streamName.Equals("out"))
                    {
                        Console.SetOut(writer);
                    }
                    else
                    {
                        Console.SetError(writer);
                    }
                }
                catch (Exception)
                {

                    throw;
                }
            }
        }

        private static void PrintCommandLineArguments()
        {
            throw new NotImplementedException();
        }

        static void Main(string[] args)
        {
            try
            {
                AppDomain.CurrentDomain.ProcessExit += Shutdown;
                Init();
            }
            catch (Exception e)
            {
                Console.Out.WriteLine("Fatal error: " + e.Message);
                Console.Out.WriteLine(e.StackTrace);
            }
        }

        private static void Init()
        {
            throw new NotImplementedException();
        }

        private static void Shutdown(object sender, EventArgs e)
        {
        }
    }
}
