using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace RconTool
{
    [DataContract]
    public class TimedCommandItem
    {

        [DataMember]
        private string name { get; set; }
        [DataMember]
        private bool everyx { get; set; }
        [DataMember]
        private int everyxmin { get; set; }
        [DataMember]
        private List<string> rconCommands { get; set; }
        [DataMember]
        private bool enabled { get; set; }
        [DataMember]
        private int time { get; set; }
        public bool hasRan = false;
        public DateTime lastRan;
        public DateTime NextRun;

        public TimedCommandItem(string name, bool everyx, int time, int everyxmin, bool enabled, List<string> runs)
        {
            this.name = name;
            this.everyx = everyx;
            this.everyxmin = everyxmin;
            this.rconCommands = runs;
            this.enabled = enabled;
            this.time = time;
        }

        public string GetName()
        {
            return this.name;
        }

        public bool RunEveryXMin()
        {
            return this.everyx;
        }

        public bool IsEnabled()
        {
            return this.enabled;
        }

        public int GetTime()
        {
            return this.time;
        }

        public List<string> GetCommands()
        {
            return this.rconCommands;
        }

        public int GetEveryXMin()
        {
            return this.everyxmin;
        }
    }
}
