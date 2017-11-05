using System.Collections.Generic;

namespace RconTool
{
    public class ServerData
    {
        public string name = "";
        public int port = 0;
        public string hostPlayer = "";
        public bool isDedicated = false;
        public string sprintEnabled = "";
        public string sprintUnlimitedEnabled = "";
        public string assassinationEnabled = "";
        public bool voip = false;
        public bool teams = false;
        public int redScore = 0;
        public int blueScore = 0;
        public string map = "";
        public string mapFile = "";
        public string variant = "";
        public string variantType = "";
        public string status = "";
        public int numPlayers = 0;
        public bool teamGame = false;
        public int maxPlayers = 0;
        public string eldewritoVersion = "";
        public List<PlayerInfo> Players { get; set; }
    }
}
