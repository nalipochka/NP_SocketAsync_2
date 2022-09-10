using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NP_SocketAsync_2
{
    public class LogClient
    {
        public string ClientIp { get; set; }

        public DateTime ConnectingTime { get; set; }
        
        public List<string> Quotes { get; set; } = null!;
        public DateTime DisconnectingTime { get; set; }

        public string ClientIpToString()
        {
            return $"{ClientIp} was connected!";
        }
        public string ConnectingTimeToString()
        {
            return $"Connection happened at {ConnectingTime}";
        }
        public string QuotesToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Quotes ({ClientIp}):");
            foreach (var item in Quotes)
            {
                sb.AppendLine(item);
            }
            return sb.ToString();
        }
        public string DisconnectingTimeToString()
        {
            return $"Disconnection {ClientIp} happened at {DisconnectingTime}";
        }

    }
}
