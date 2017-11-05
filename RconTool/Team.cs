using System.Collections.Generic;
using System.Drawing;


namespace RconTool
{
    public class Team
    {
        private int score;
        private int index;
        private Color teamColor;
        private List<PlayerInfo> players = new List<PlayerInfo>();

        public Team(int index, int score, Color c)
        {
            this.score = score;
            this.index = index;
            this.teamColor = c;
        }
        
        public Team(Team team)
        {
            this.score = team.score;
            this.index = team.index;
            this.teamColor = team.teamColor;
            this.players = team.players;
        }

        public List<PlayerInfo> GetPlayers()
        {
            return players;
        }

        public void AddPlayer(PlayerInfo p)
        {
            players.Add(p);
        }

        public int GetPlayerScore()
        {
            int sc = 0;
            for (int x = 0; x < players.Count; x++)
            {
                sc += players[x].Score;
            }
            return sc;
        }

        public void ResetPlayerArray()
        {
            players = null;
            players = new List<PlayerInfo>();
        }

        public int GetPlayerKills()
        {
            int kills = 0;
            for (int x = 0; x < players.Count; x++)
            {
                kills += players[x].Kills;
            }
            return kills;
        }

        public int GetPlayerDeaths()
        {
            int deaths = 0;
            for(int x = 0; x < players.Count; x++)
            {
                deaths += players[x].Deaths;
            }
            return deaths;
        }

        public void SortPlayers()
        {
            players.Sort((y, x) =>
            {
                int result = x.Score.CompareTo(y.Score);
                int result2 = result != 0 ? result : x.Kills.CompareTo(y.Kills);
                int result3 = result2 != 0 ? result2 : y.Deaths.CompareTo(x.Deaths);
                return result3 != 0 ? result3 : x.Assists.CompareTo(y.Assists);
            });
        }

        public int GetScore()
        {
            return this.score;
        }

        public int GetIndex()
        {
            return index;
        }

        public void SetScore(int score)
        {
            this.score = score;
        }

        public Color GetColor()
        {
            return this.teamColor;
        }
    }
}
