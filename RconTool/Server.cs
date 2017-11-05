using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Newtonsoft.Json;
using System.Drawing;

namespace RconTool
{
    public class Server
    {
        public ServerData serverData;

        public List<Team> teamlist = new List<Team>();

        public List<Team> prevteamlist = new List<Team>();

        public Server()
        {
            teamlist = new List<Team>();
            teamlist.Add(new Team(0, 0, Hex2Rgb("#9b3332")));
            teamlist.Add(new Team(1, 0, Hex2Rgb("#325992")));
            teamlist.Add(new Team(2, 0, Hex2Rgb("#1F3602")));
            teamlist.Add(new Team(3, 0, Hex2Rgb("#BC4D00")));
            teamlist.Add(new Team(4, 0, Hex2Rgb("#1D1052")));
            teamlist.Add(new Team(5, 0, Hex2Rgb("#A77708")));
            teamlist.Add(new Team(6, 0, Hex2Rgb("#1C0D02")));
            teamlist.Add(new Team(7, 0, Hex2Rgb("#FF4D8A")));
            teamlist.Add(new Team(8, 0, Hex2Rgb("#D8D8D8")));
            teamlist.Add(new Team(8, 0, Hex2Rgb("#0B0B0B")));
            teamlist.Add(new Team(9, 0, Hex2Rgb("#D8D8D8")));
            teamlist.Add(new Team(10, 0, Hex2Rgb("#D8D8D8")));
            teamlist.Add(new Team(11, 0, Hex2Rgb("#D8D8D8")));
            teamlist.Add(new Team(12, 0, Hex2Rgb("#D8D8D8")));
            teamlist.Add(new Team(13, 0, Hex2Rgb("#D8D8D8")));
            teamlist.Add(new Team(14, 0, Hex2Rgb("#D8D8D8")));
            teamlist.Add(new Team(15, 0, Hex2Rgb("#D8D8D8")));
            teamlist.Add(new Team(16, 0, Hex2Rgb("#D8D8D8")));
            teamlist.Add(new Team(17, 0, Hex2Rgb("#D8D8D8")));
            teamlist.Add(new Team(18, 0, Hex2Rgb("#D8D8D8")));
            teamlist.Add(new Team(19, 0, Hex2Rgb("#D8D8D8")));
            teamlist.Add(new Team(20, 0, Hex2Rgb("#D8D8D8")));
        }

        public void GetStats(string address, string infoPort)
        {
            for (int x = 0; x < teamlist.Count; x++)
            {
                Team tm = teamlist[x];
                tm.ResetPlayerArray();
                tm.SetScore(0);
            }

            var json = GetJsonfromURL(address, infoPort);
            serverData = JsonConvert.DeserializeObject<ServerData>(json);

            if (serverData.Players != null)
            {
                for (int x = 0; x < serverData.Players.Count; x++)
                {
                    for (int y = 0; y < teamlist.Count; y++)
                    {
                        if (teamlist[y].GetIndex() == serverData.Players[x].Team)
                        {
                            teamlist[y].AddPlayer(serverData.Players[x]);
                            goto SKIPTONEXT;
                        }
                    }
                    SKIPTONEXT:;
                }


                for (int x = 0; x < teamlist.Count; x++)
                {
                    teamlist[x].SetScore(teamlist[x].GetScore() + teamlist[x].GetPlayerScore());
                }

                for (int x = 0; x < teamlist.Count; x++)
                {
                    teamlist[x].SortPlayers();
                }

                if (serverData.teamGame == false || serverData.variant.ToLower().Contains("ffa"))
                {
                    SortTeams();
                }
                else
                {
                    teamlist.Sort((y, x) => x.GetScore().CompareTo(y.GetScore()));
                }

            }

        }

        public void SortTeams()
        {
            var sorted = teamlist.OrderBy(x => -x.GetScore()).ThenBy(x => -x.GetPlayerKills()).ThenBy(x => x.GetPlayerDeaths());
            teamlist = sorted.ToList();
        }

        public static Color Hex2Rgb(String colorStr)
        {
            System.Drawing.Color col = System.Drawing.ColorTranslator.FromHtml(colorStr);
            return col;
        }

        private string GetJsonfromURL(string ip, string port)
        {
            using (WebClient wc = new WebClient())
            {
                string newurl = "http://" + ip + ":" + port + "/";
                var json = wc.DownloadString(newurl);
                return json;
            }
        }

        public string GetJson(string url)
        {
            using (WebClient wc = new WebClient())
            {
                var json = wc.DownloadString(url);
                return json;
            }
        }
    }
}
