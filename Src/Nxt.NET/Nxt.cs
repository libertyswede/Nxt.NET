using System;
using System.IO;

namespace Nxt.NET
{
    class Nxt
    {
        static Nxt()
        {
            RedirectSystemStreams("out");
            RedirectSystemStreams("error");
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
