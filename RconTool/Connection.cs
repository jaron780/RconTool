using EldoritoRcon;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;

namespace RconTool
{
    public class Connection
    {
        public ServerInfo serverinfo;
        private Form1 form;
        public Server server = null;
        private string consoleText = "";
        private string chatText = "";
        public ServerConnection serverConnection;

        private string laststatus;
        public Thread StatsThread;
        public Thread RconThread;
        public Thread KeepRconAlive;
        public Boolean visable = false;
        
        public Connection(Form1 form,string ip, string rconport, string infoport, string rconpassword, List<string> sendOnConnect)
        {
            this.form = form;
            this.serverinfo = new ServerInfo(ip, infoport, rconpassword, rconport, sendOnConnect);
            server = new Server();

            RconThread = new Thread(new ThreadStart(StartConnection));
            RconThread.Start();
            
            if (!Form1.connectionList.Contains(this))
            {
                Form1.connectionList.Add(this);
            }

            StatsThread = new Thread(new ThreadStart(RunStats));
            StatsThread.Start();

            if (!serverinfo.RconPort.Equals(""))
            {
                KeepRconAlive = new Thread(new ThreadStart(KeepAlive));
                KeepRconAlive.Start();
            }
        }

        private void RunStats()
        {
            while (true)
            {
                GetStats();
                Thread.Sleep(1000);
            }

        }
        public Connection(Form1 form, ServerInfo serverinfo)
        {
            this.form = form;
            this.serverinfo = serverinfo;
            server = new Server();

            RconThread = new Thread(new ThreadStart(StartConnection));
            RconThread.Start();

            if (!Form1.connectionList.Contains(this))
            {
                Form1.connectionList.Add(this);
            }

            StatsThread = new Thread(new ThreadStart(RunStats));
            StatsThread.Start();

            if (!serverinfo.RconPort.Equals(""))
            {
                KeepRconAlive = new Thread(new ThreadStart(KeepAlive));
                KeepRconAlive.Start();
            }
        }
        public bool IsConnected()
        {
            
            if(serverConnection == null || serverConnection.ws == null)
            {
                return false;
            }
            return serverConnection.ws.IsAlive;
        }

        public void KeepAlive()
        {
            Thread.Sleep(2000);
            while (true)
            {
                new Thread(delegate () {
                    Thread.Sleep(2000);
                    if (serverConnection != null)
                        serverConnection.Command("version");
                }).Start();

                Thread.Sleep(120000);
            }
        }

        public bool IsVisable()
        {
            return visable;
        }

        public void SetVisable(bool set)
        {
            visable = set;
        }

        public void CloseConnection()
        {
            if (StatsThread != null && StatsThread.IsAlive)
                StatsThread.Abort();
            if (serverConnection != null && serverConnection.ws != null)
            serverConnection.ws.Close();
        }

        public void StartConnection()
        {
            if (IsConnected())
            {
                CloseConnection();
            }
            if (serverConnection != null && serverConnection.ws != null)
            {
                Console.WriteLine("Nulling websocket");
                if (serverConnection.ws != null && serverConnection.ws.IsAlive)
                    serverConnection.ws.Close();

                serverConnection.ws = null;
                server = null;
               
            }

            if (consoleText != null) consoleText = "" ;
            if (chatText != null) chatText = "";
            Connect();
        }

        public void Connect()
        {
            if (serverinfo.RconPort.Equals(""))
            {
                return;
            }
            Console.WriteLine("Connecting websocket: " + serverinfo.Ip + ":" + serverinfo.RconPort);
            serverConnection = new ServerConnection(this, "ws://" + serverinfo.Ip + ":" + serverinfo.RconPort, serverinfo.RconPassword);
            serverConnection.ServerMessage += new ServerEventHandler(this.OnMessage);
            serverConnection.ServerClose += new ServerCloseHandler(this.OnClose);
        }

        private void OnClose(object sender, int code)
        {
            PrintToConsole("Lost Connection to Websocket code: " + code);
        }

        private void OnMessage(object sender, string message)
        {
            if (message.StartsWith("0.5.1.1") || message.StartsWith("0.6"))
            {

            }
            else
            if (message.StartsWith("accept"))
            {
                PrintToConsole("Successfully Connected to Rcon");
            }
            else
            if (!message.Equals(""))
            {
                
                if (RegexParser.IsChat(message))
                {
                    ChatMessage cm = RegexParser.ParseChat(message);
                    ChatManager.ProccessChat(this, cm);
                }
                else
                {
                    PrintToConsole(message);
                }
            }
        }

        public void ClearConsole()
        {
            this.consoleText = "";
        }

        public void ClearChat()
        {
            this.chatText = "";
        }
        
        public void PrintToConsole(string line)
        {
            
            string result = Regex.Replace(line, @"\r\n?|\n", System.Environment.NewLine);
            if (IsVisable())
            {
                form.AppendConsole((result + System.Environment.NewLine));
            }
            consoleText = consoleText + (result + System.Environment.NewLine);
            Console.WriteLine(serverinfo.Ip + ": " +result);
        }

        public void PrintToChat(string line)
        {            
            string result = Regex.Replace(line, @"\r\n?|\n", System.Environment.NewLine);
            if (IsVisable())
            {
                form.AppendChat((result + System.Environment.NewLine));
            }
            chatText = chatText + (result + System.Environment.NewLine);
            Console.WriteLine(serverinfo.Ip + ": " + result);
        }
        
        public void SendPM(string name, string message)
        {
            SendToRcon("Server.PM \"" + name + "\" \"" + message + "\"");
            Console.WriteLine("Server.PM \"" + name + "\" \"" + message + "\"");
        }

        public string GetConsole()
        {
            return this.consoleText;
        }

        public string GetChat()
        {
            return this.chatText;
        }

        public void SendToRcon(string cmd)
        {
            new Thread(delegate () {
                serverConnection.Command(cmd);
            }).Start();
        }

        public void sendToChat(string cmd)
        {
            new Thread(delegate () {
                serverConnection.Command("Server.say \"" + cmd + "\"");
            }).Start();
        }

        public void GetStats()
        {
            try
            {
                try
                {
                    server.GetStats(serverinfo.Ip, serverinfo.InfoPort);

                    if (laststatus == null)
                    {
                        laststatus = server.serverData.status;
                        if (server.serverData.variant != null && !server.serverData.variant.Equals("null") && !server.serverData.variant.Equals(""))
                        {
                            PrintToConsole("Current Game - " + server.serverData.variant + ":" + server.serverData.variantType + " - " + server.serverData.map);
                        }
                    }
                    else
                    {
                        string date = DateTime.Now.ToString("[MM-dd-yyyy HH:mm:ss] ");
                        if (!laststatus.ToLower().Equals(server.serverData.status.ToLower()))
                        {
                            if (server.serverData.status.ToLower().Equals("ingame"))
                            {
                                PrintToConsole(date + "Game Started - " + server.serverData.variant + ":" + server.serverData.variantType + " - " + server.serverData.map);
                            }
                            else
                            if (server.serverData.status.ToLower().Equals("inlobby"))
                            {
                                PrintToConsole(date + "Match Ended");
                            }
                            laststatus = server.serverData.status;
                        }
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine("Error getting stats: " + serverinfo.Ip);
                }
                server.prevteamlist = new List<Team>(server.teamlist.Count);

                server.teamlist.ForEach((item) =>
                {
                    server.prevteamlist.Add(new Team(item));
                });


            }
            catch (Exception)
            {
                Console.WriteLine("Connection Stats Error: " + serverinfo.Ip);
            }
        }
    }
}
