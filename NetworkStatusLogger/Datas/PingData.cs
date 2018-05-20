using System.Net;
using System.Net.NetworkInformation;

namespace NetworkStatusLogger.Datas
{
    class PingData
    {
        public PingReply Reply { get; set; }
        public IPHostEntry iPHostEntry { get; set; }
        public decimal AveragePingTime { get; set; }
    }
}
