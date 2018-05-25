using System;
using System.Collections.Generic;
using System.Text;
using System.Net.NetworkInformation;
using System.Net;

namespace NetworkStatusLogger.Datas
{
    class TraceData
    {
        public TraceData()
        {
            address = IPAddress.Any;
        }
        public IPAddress address { get; set; }
        public PingReply reply { get; set; }
    }
}
