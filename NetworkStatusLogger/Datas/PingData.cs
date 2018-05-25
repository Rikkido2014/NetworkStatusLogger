using System.Net;
using System.Net.NetworkInformation;
using System;
using System.Collections.Generic;

namespace NetworkStatusLogger.Datas
{
    class PingData
    {
        public PingReply Reply { get; set; }
        public IPHostEntry iPHostEntry { get; set; }
        public decimal AveragePingTime { get; set; }
        
    }
}
