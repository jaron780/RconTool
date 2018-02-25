using System.Linq;

namespace RconTool
{
    public class PlayerInfo
    {
        public string Name { get; set; }
        public string serviceTag { get; set; }
        public string primaryColor { get; set; }
        public int Score { get; set; }
        public int Kills { get; set; }
        public int Assists { get; set; }
        public int Deaths { get; set; }
        public int Betrayals { get; set; }
        public int TimeSpentAlive { get; set; }
        public int Suicides { get; set; }
        public int BestStreak { get; set; }
        public int Team { get; set; }
        public bool IsAlive { get; set; }
        public string Uid { get; set; }
        public string GetUid()
        {
            return ReverseHex(this.Uid);
        }
        private string ReverseHex(string originalHex)
        {
            int lengthInBytes = originalHex.Count() / 2;
            char[] chars = new char[lengthInBytes * 2];
            for (int index = 0; index < lengthInBytes; index++)
            {
                int reversedIndex = lengthInBytes - 1 - index;
                chars[reversedIndex * 2] = originalHex[index * 2];
                chars[reversedIndex * 2 + 1] = originalHex[index * 2 + 1];
            }
            return new string(chars);
        }
    }
}
