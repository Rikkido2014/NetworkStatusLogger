using System;
using System.Threading.Tasks;
using System.IO;
using System.Net.NetworkInformation;
using System.Net;
using System.Text.RegularExpressions;

namespace NetworkStatusLogger
{
    class Program
    {
        static Logger logger;

        const string configstr = @"C:\Users\masay\source\repos\NetworkStatusLogger\NetworkStatusLogger\bin\Debug\netcoreapp2.0\netstate.xml";
        private static int pingCnt = 10;
        private const int timeout = 1200;
        public static Datas.Config config = null;

        static void Main(string[] args)
        {


            
            Console.WriteLine($"Read Config File at {configstr}");
            //コンフィグファイルのチェック
            if (!FileCheck(configstr))
            {
                config = new Datas.Config();
                config.Adresses = new System.Collections.Generic.List<string>();
                config.Adresses.Add("");
                config.Span = new TimeSpan(0, 0, 10);
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
                foreach (var address in config.Adresses)
                {
                    IPHostEntry iPHost = Dns.GetHostEntry(address);
                    Datas.PingData data = ping(iPHost.AddressList[0], timeout, pingCnt);
                    data.iPHostEntry = iPHost;
                    if (Logging(data)) break;
                }
                System.Threading.Thread.Sleep(config.Span);
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
                case IPStatus.Success: logger.log($"SUCCEED TO:{data.iPHostEntry.HostName} TIME_AVE:{data.AveragePingTime} ms", Logger.status.Connect); break;
                case IPStatus.TimedOut: logger.log($"TIMEOUT TO:{data.iPHostEntry.HostName} TIME_AVE:{data.AveragePingTime} ms", Logger.status.DisConnect); break;
                case IPStatus.BadDestination: logger.log($"BADROUTE TO:{data.iPHostEntry.HostName} TIME_AVE:{data.AveragePingTime } ms", Logger.status.DisConnect); break;
                case IPStatus.Unknown: logger.log($"UNKNOWN TO:{data.iPHostEntry.HostName} TIME_AVE:{data.AveragePingTime} ms", Logger.status.DisConnect); break;
                default: logger.log($"SOMEREASON TO{data.iPHostEntry.HostName} TIME_AVE:{data.AveragePingTime} ms", Logger.status.DisConnect); break;
            }
            return data.Reply.Status == IPStatus.Success ? true : false;
        }
        /// <summary>
        /// ping送信関数
        /// </summary>
        /// <param name="address">アドレス</param>
        /// <param name="timeout">タイムアウト時間</param>
        /// <param name="count">ping送信回数</param>
        /// <returns>統計データ</returns>
        static public Datas.PingData ping(IPAddress address, int timeout, int count)
        {
            var dReply = new Datas.PingData();

            using (Ping ping = new Ping())
            {
                long TotalTime = new long();
                for (int cnt = 0; cnt < count; cnt++)
                {

                    var reply = ping.Send(address, timeout);
                    if (reply.Status == IPStatus.Success)
                    {
                        TotalTime += reply.RoundtripTime;
                        dReply.Reply = reply;
                    }
                }
                dReply.AveragePingTime = TotalTime / count;
            }
            return dReply;
        }
    }
}
