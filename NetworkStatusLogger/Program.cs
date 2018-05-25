using System;
using System.Threading.Tasks;
using System.IO;
using System.Net.NetworkInformation;
using System.Net;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace NetworkStatusLogger
{
    class Program
    {
        static Logger logger;

        //const string configstr = @"C:\Users\USR\source\repos\NetworkStatusLogger\NetworkStatusLogger\bin\Debug\netcoreapp2.0\netstate.xml";
        const string configstr = "/etc/netlog/netconfig.xml";
        private static int pingCnt = 10;
        private const int timeout = 2048;
        public static Datas.Config config = null;

        static int Main(string[] args)
        {



            Console.WriteLine($"Read Config File at {configstr}");
            //コンフィグファイルのチェック
            if (!FileCheck(configstr))
            {
                config = new Datas.Config();
                config.Adresses = new System.Collections.Generic.List<string>();
                config.Adresses.Add("");
                config.Span = new TimeSpan(0, 0, 45);
                config.LogFilePath = "/var/log/netStatus/net.log";
                string cfg = xml<Datas.Config>.SerializeXml(config);
                using (TextWriter writer = new StreamWriter(configstr))
                {

                    writer.Write(cfg);
                    writer.Flush();
                    writer.Close();
                }

            }
            else
            {
                using (StreamReader sr = new StreamReader(configstr))
                {
                    config = xml<Datas.Config>.ReadXml(sr.BaseStream);
                }
            }
            logger = new Logger(config.LogFilePath);
            //メイン関数
            while (true)
            {
                try
                {
                    foreach (var addressStr in config.Adresses)
                    {
                        IPAddress address = IPAddress.Any;
                        Datas.PingData data = new Datas.PingData();
                        //セーブ記載のデータがIPアドレスでなかった（ドメイン）だった時の処理
                        if (IPAddress.TryParse(addressStr, out address) == false)
                        {
                            IPHostEntry iPHost = Dns.GetHostEntry(address);
                            address = iPHost.AddressList[0];
                            data.iPHostEntry = iPHost;

                        }
                        data = network.DoPing(address, timeout, pingCnt);
                        if (!Logging(data)) break;
                        //以下到達できなかった時のトレースコード
                        var traceData = network.TraceRoute(address, 2, 2048);
                        int Routes = 0;
                        logger.writeText("TracePahse...");
                        foreach(var traceDetum in traceData)
                        {
                            logger.writeText($"[{Routes}]: {traceDetum.reply.Address} / {traceDetum.reply.RoundtripTime} ms | {traceDetum.reply.Status.ToString()}");
                        }
                    }
                    System.Threading.Thread.Sleep(config.Span);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }

            }

        }

        ~Program()
        {
            string cfg = xml<Datas.Config>.SerializeXml(config);
            using (TextWriter writer = new StreamWriter(configstr))
            {
                writer.Write(cfg);
                writer.Flush();
                writer.Close();
            }
            Console.WriteLine("NetWorkerProcess Was Stopping");
        }

        static public bool FileCheck(string path)
        {
            FileInfo fileInfo = new FileInfo(path);
            if (!Directory.Exists(fileInfo.DirectoryName)) Directory.CreateDirectory(fileInfo.DirectoryName);
            if (fileInfo.Exists) return true;
            return false;
        }

        static public bool Logging(Datas.PingData data)
        {
            IPHostEntry iPHost = Dns.GetHostEntry(data.Reply.Address);
            switch (data.Reply.Status)
            {
                case IPStatus.Success: logger.logState($"SUCCEED TO:{data.iPHostEntry.HostName} TIME_AVE:{data.AveragePingTime} ms", Logger.status.Connect); break;
                case IPStatus.TimedOut: logger.logState($"TIMEOUT TO:{data.iPHostEntry.HostName} TIME_AVE:{data.AveragePingTime} ms", Logger.status.DisConnect); break;
                case IPStatus.BadDestination: logger.logState($"BADROUTE TO:{data.iPHostEntry.HostName} TIME_AVE:{data.AveragePingTime } ms", Logger.status.DisConnect); break;
                case IPStatus.Unknown: logger.logState($"UNKNOWN TO:{data.iPHostEntry.HostName} TIME_AVE:{data.AveragePingTime} ms", Logger.status.DisConnect); break;
                default: logger.logState($"SOMEREASON TO{data.iPHostEntry.HostName} TIME_AVE:{data.AveragePingTime} ms", Logger.status.DisConnect); break;
            }
            return data.Reply.Status == IPStatus.Success;
        }

    }
}
