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
                Init.Initialize();
            }
            catch (Exception e)
            {
                Console.Out.WriteLine("Fatal error: " + e.Message);
                Console.Out.WriteLine(e.StackTrace);
            }
        }

        private static void Shutdown(object sender, EventArgs e)
        {
        }

        private static class Init
        {
            private static volatile bool initialized = false;

            static Init()
            {
                try
                {
                    var startTime = DateTime.Now;

                    Logger.Init();
                    //Logger.init();
                    //setSystemProperties();
                    //logSystemProperties();
                    //runtimeMode.init();
                    //Thread secureRandomInitThread = initSecureRandom();
                    //setServerStatus(ServerStatus.BEFORE_DATABASE, null);
                    //Db.init();
                    //setServerStatus(ServerStatus.AFTER_DATABASE, null);
                    //TransactionProcessorImpl.getInstance();
                    //BlockchainProcessorImpl.getInstance();
                    //Account.init();
                    //AccountRestrictions.init();
                    //AccountLedger.init();
                    //Alias.init();
                    //Asset.init();
                    //DigitalGoodsStore.init();
                    //Hub.init();
                    //Order.init();
                    //Poll.init();
                    //PhasingPoll.init();
                    //Trade.init();
                    //AssetTransfer.init();
                    //AssetDelete.init();
                    //AssetDividend.init();
                    //Vote.init();
                    //PhasingVote.init();
                    //Currency.init();
                    //CurrencyBuyOffer.init();
                    //CurrencySellOffer.init();
                    //CurrencyFounder.init();
                    //CurrencyMint.init();
                    //CurrencyTransfer.init();
                    //Exchange.init();
                    //ExchangeRequest.init();
                    //Shuffling.init();
                    //ShufflingParticipant.init();
                    //PrunableMessage.init();
                    //TaggedData.init();
                    //FxtDistribution.init();
                    //Peers.init();
                    //Generator.init();
                    //AddOns.init();
                    //API.init();
                    //Users.init();
                    //DebugTrace.init();
                    //int timeMultiplier = (Constants.isTestnet && Constants.isOffline) ? Math.max(Nxt.getIntProperty("nxt.timeMultiplier"), 1) : 1;
                    //ThreadPool.start(timeMultiplier);
                    //if (timeMultiplier > 1)
                    //{
                    //    setTime(new Time.FasterTime(Math.max(getEpochTime(), Nxt.getBlockchain().getLastBlock().getTimestamp()), timeMultiplier));
                    //    Logger.logMessage("TIME WILL FLOW " + timeMultiplier + " TIMES FASTER!");
                    //}
                    //try
                    //{
                    //    secureRandomInitThread.join(10000);
                    //}
                    //catch (InterruptedException ignore) { }
                    //testSecureRandom();
                    //long currentTime = System.currentTimeMillis();
                    //Logger.logMessage("Initialization took " + (currentTime - startTime) / 1000 + " seconds");
                    //Logger.logMessage("Nxt server " + VERSION + " started successfully.");
                    //Logger.logMessage("Copyright © 2013-2016 The Nxt Core Developers.");
                    //Logger.logMessage("Distributed under GPLv2, with ABSOLUTELY NO WARRANTY.");
                    //if (API.getWelcomePageUri() != null)
                    //{
                    //    Logger.logMessage("Client UI is at " + API.getWelcomePageUri());
                    //}
                    //setServerStatus(ServerStatus.STARTED, API.getWelcomePageUri());
                    //if (isDesktopApplicationEnabled())
                    //{
                    //    launchDesktopApplication();
                    //}
                    //if (Constants.isTestnet)
                    //{
                    //    Logger.logMessage("RUNNING ON TESTNET - DO NOT USE REAL ACCOUNTS!");
                    //}
                }
                catch (Exception)
                {
                    //Logger.logErrorMessage(e.getMessage(), e);
                    //System.exit(1);
                }
            }
            
            public static void Initialize()
            {
                if (initialized)
                {
                    throw new ApplicationException("Nxt.init has already been called");
                }
                initialized = true;
            }
        }
    }
}
