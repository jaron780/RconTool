using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace RconTool
{
    [DataContract]
    public class ServerInfo
    {
        [DataMember]
        public string Ip { get; set; }
        [DataMember]
        public string InfoPort { get; set; }
        [DataMember]
        public string RconPassword { get; set; }
        [DataMember]
        public string RconPort { get; set; }

        public ServerInfo(string ip, string infoport, string rconpassword, string rconport)
        {
            this.Ip = ip;
            this.InfoPort = infoport;
            this.RconPassword = rconpassword;
            this.RconPort = rconport;
        }

        public string ToBase64()
        {
            string s = JsonConvert.SerializeObject(this);
            return Base64Encode(s);
        }

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }
    }
}
