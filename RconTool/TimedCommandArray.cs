using Newtonsoft.Json;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace RconTool
{
    [DataContract]
    public class TimedCommandArray
    {
        [DataMember]
        private List<TimedCommandItem> timedCommands { get; set; }

        public TimedCommandArray()
        {
            timedCommands = new List<TimedCommandItem>();
        }

        public List<TimedCommandItem> GetCommandList()
        {
            return this.timedCommands;
        }

        public void RemoveCommandItem(TimedCommandItem tci)
        {
            timedCommands.Remove(tci);
        }

        public void AddTimedCommand(TimedCommandItem tci)
        {
            timedCommands.Add(tci);
        }

        public string toBase64()
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
