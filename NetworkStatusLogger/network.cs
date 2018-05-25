using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.NetworkInformation;

namespace NetworkStatusLogger
{
    static class network
    {
        /// <summary>
        /// ping送信関数
        /// </summary>
        /// <param name="address">アドレス</param>
        /// <param name="timeout">タイムアウト時間</param>
        /// <param name="count">ping送信回数</param>
        /// <returns>統計データ</returns>
        static public Datas.PingData DoPing(IPAddress address, int timeout, int count, int maxHop = 32)
        {
            var dReply = new Datas.PingData();
            var pOption = new PingOptions();
            pOption.Ttl = maxHop;
            using (Ping ping = new Ping())
            {
                byte[] buffer = new byte[32];
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
        /// <summary>
        /// 経路情報を調べ上げる
        /// </summary>
        /// <param name="destAddress">目的アドレス</param>
        /// <param name="pingCount">ping回数</param>
        /// <param name="timeOut">タイムアウト時間</param>
        /// <param name="maxHop">TTL,調べるノードの最大数</param>
        /// <returns></returns>
        static public List<Datas.TraceData> TraceRoute(IPAddress destAddress, int pingCount, int timeOut, int maxHop = 32)
        {
            var traceData = new List<Datas.TraceData>();
            for (int cnt = 0; cnt < maxHop; cnt++)
            {
                traceData.Add(new Datas.TraceData());
                Datas.PingData data = DoPing(destAddress, timeOut, pingCount, maxHop);
                traceData[cnt].address = destAddress;
                traceData[cnt].reply = data.Reply;
                if (data.Reply.Status != IPStatus.Success) break;//pingが通らなかったら切断として認識する。
                if (data.Reply.Address == destAddress) break;//アドレスが目的地アドレスだったら終了
            }
            return traceData;
        }
    }
}
