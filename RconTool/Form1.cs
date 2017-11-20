using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Configuration;
using System.Diagnostics;
using Newtonsoft.Json;
using System.IO;
using System.Runtime.Serialization.Json;

namespace RconTool
{
    public partial class Form1 : Form
    {
        public static Form1 form;
        public static string toolversion = "3.0";
        public static string titleOption = "";
        private static bool autoScroll = true;
        bool autoUpdateEnabled = true;

        public static List<Connection> connectionList = new List<Connection>();
        public static Connection currentConnection = null;

        public static Thread RconThread;
        public static Thread tick;
        public static Thread tick2;

        private Graphics g = null;
        private static Configuration config = ConfigurationManager.OpenExeConfiguration(Application.ExecutablePath);
        bool sendChat = false;

        PlayerInfo contextplayer;
        PlayerInfo newContextPlayer;
        private int contextPlayerPos = 0;

        ContextMenuStrip scoreBoardContextMenu = null;

        public Form1()
        {
            form = this;
            InitializeComponent();

            if (!HasSetting())
            {
                new ServerInterface().ShowDialog();
            }
            else
            {
                LoadSettings();
            }
            trackBar1.Maximum = connectionList.Count - 1;
            trackBar1.Minimum = 0;
            if (connectionList.Count >= 1)
            {
                currentConnection = connectionList[0];

                SetTextBoxText(textBox2, currentConnection.GetConsole());
                SetTextBoxText(textBox3, currentConnection.GetChat());

                currentConnection.SetVisable(true);
            }
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;

            SetStyle(ControlStyles.DoubleBuffer, true);
            DoubleBuffered = true;

            AddContextMenuAndItems();

            g = CreateGraphics();

            tick = new Thread(new ThreadStart(Tick));
            tick.Start();

            tick2 = new Thread(new ThreadStart(Tick2));
            tick2.Start();

            RconThread = new Thread(new ThreadStart(AllRcon));
            RconThread.Start();
        }

        public void AllRcon()
        {
            while (true)
            {
                if (currentConnection != null)
                {
                    SetTrackBar();
                }
                Thread.Sleep(100);
            }
        }

        public void AppendConsole(string txt)
        {
            if (autoUpdateEnabled)
                AppendText(textBox2, txt);
        }

        public void AppendChat(string txt)
        {
            if (autoUpdateEnabled)
                AppendText(textBox3, txt);
        }

        public void AddContextMenuAndItems()
        {
            ContextMenuStrip mnuContextMenu = new ContextMenuStrip();
            this.scoreBoardContextMenu = mnuContextMenu;

            mnuContextMenu.Items.Add("Kick", null, this.KickPlayer);
            mnuContextMenu.Items.Add("Ban", null, this.BanPlayer);
            mnuContextMenu.Items.Add("Copy UID", null, this.CopyUID);
            mnuContextMenu.Items.Add("Copy Name", null, this.CopyName);
            mnuContextMenu.Items.Add("Copy Name and UID", null, this.CopyNameAndUID);
            mnuContextMenu.Items.Add("Copy Stats URL", null, this.CopyStatsUrl);
        }

        private void KickPlayer(object sender, System.EventArgs e)
        {
            if (contextplayer != null)
            {
                if (currentConnection.server != null)
                {
                    var confirmResult = MessageBox.Show("Are you sure you want to kick Player: " + contextplayer.Name + ":" + contextplayer.GetUid(), "Kick Player", MessageBoxButtons.YesNo);
                    if (confirmResult == DialogResult.Yes)
                    {
                        currentConnection.PrintToConsole("Kicking: " + contextplayer.Name + "/" + contextplayer.GetUid() + "");
                        currentConnection.SendToRcon("Server.KickUid " + contextplayer.GetUid());
                    }
                }
            }
        }

        private void CopyUID(object sender, EventArgs e)
        {
            if (contextplayer != null)
            {
                Clipboard.SetText(contextplayer.GetUid());
                currentConnection.PrintToConsole("Copied " + contextplayer.GetUid() + " to Clipboard");
            }
        }

        private void CopyName(object sender, EventArgs e)
        {
            if (contextplayer != null)
            {
                Clipboard.SetText(contextplayer.Name);
                currentConnection.PrintToConsole("Copied " + contextplayer.Name + " to Clipboard");
            }
        }

        private void CopyNameAndUID(object sender, EventArgs e)
        {
            if (contextplayer != null)
            {
                Clipboard.SetText(contextplayer.Name + ":" + contextplayer.GetUid());
                currentConnection.PrintToConsole("Copied " + contextplayer.Name + ":" + contextplayer.GetUid() + " to Clipboard");
            }
        }

        private void CopyStatsUrl(object sender, EventArgs e)
        {
            if (contextplayer != null)
            {
                string surl = "http://halostats.click/privateapi/searchPlayers?name=" + contextplayer.Name + "&uid=" + ReverseHex(contextplayer.Uid);
                string json = currentConnection.server.GetJson(surl);
                Console.WriteLine("URL: " + surl);
                Statdata stat = JsonConvert.DeserializeObject<Statdata>(json);
                try
                {
                    string url = "http://halostats.click/Player/" + stat.Data[0].PlayerID;
                    Clipboard.SetText(url);
                    currentConnection.PrintToConsole("Copied " + url + " to Clipboard");
                }
                catch (Exception)
                {
                    string surl2 = "http://halostats.click/privateapi/searchPlayers?name=" + contextplayer.Name + "&uid=0" + ReverseHex(contextplayer.Uid);
                    string json2 = currentConnection.server.GetJson(surl2);
                    Console.WriteLine("URL: " + surl2);
                    Statdata stat2 = JsonConvert.DeserializeObject<Statdata>(json2);

                    try
                    {
                        string url2 = "http://halostats.click/Player/" + stat2.Data[0].PlayerID;
                        Clipboard.SetText(url2);
                        currentConnection.PrintToConsole("Copied USING '0' " + url2 + " to Clipboard");
                    }
                    catch (Exception)
                    {

                    }
                }
            }
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
            string reversed = new string(chars);

            return reversed.TrimStart('0');
        }

        public class Statdata
        {
            public List<Resultdata> Data { get; set; }
        }

        public class Resultdata
        {
            public string PlayerID;
            public int Rank;
        }

        private void BanPlayer(object sender, System.EventArgs e)
        {
            if (contextplayer != null)
            {
                if (currentConnection.server != null)
                {
                    var confirmResult = MessageBox.Show("Are you sure you want to ban Player: " + contextplayer.Name + ":" + contextplayer.GetUid(), "Ban Player", MessageBoxButtons.YesNo);
                    if (confirmResult == DialogResult.Yes)
                    {
                        currentConnection.PrintToConsole("Banning: " + contextplayer.Name + "/" + contextplayer.GetUid() + "");
                        currentConnection.SendToRcon("Server.KickBanUid " + contextplayer.GetUid());
                    }
                }
            }
        }

        public static bool HasSetting()
        {
            if (LoadSetting("Server.0") != null && LoadSetting("Server.0") != "" && LoadSetting("Server.0").Length > 1)
            {
                Console.WriteLine("Config Set");
                return true;
            }
            return false;
        }

        public static void LoadSettings()
        {
            titleOption = LoadSetting("titleOption");
            LoadServers();
        }

        public static void LoadServers()
        {
            for (int x = 0; x < 100; x++)
            {
                if (LoadSetting("Server." + x) != null)
                {
                    string json = Base64Decode(LoadSetting("Server." + x));
                    using (var ms = new MemoryStream(Encoding.Unicode.GetBytes(json)))
                    {
                        DataContractJsonSerializer deserializer = new DataContractJsonSerializer(typeof(ServerInfo));
                        ServerInfo bsObj2 = (ServerInfo)deserializer.ReadObject(ms);
                        new Connection(form, bsObj2);
                    }
                }
            }
        }

        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }

        public static string LoadSetting(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }

        public static void RemoveSetting(string key)
        {
            config.AppSettings.Settings.Remove(key);
        }

        public static void SaveServers()
        {
            for (int x = 0; x < connectionList.Count; x++)
            {
                Connection c = connectionList[x];
                SaveSetting("Server." + x, c.serverinfo.ToBase64());
            }
        }

        public static void SaveSettings()
        {
            SaveSetting("TitleOption", titleOption);
            SaveServers();
            SaveConfigToFile();
        }

        public static void SaveConfigToFile()
        {
            config.Save(ConfigurationSaveMode.Full);
        }

        public static void SaveSetting(string Key, string value)
        {
            config.AppSettings.Settings.Remove(Key);
            config.AppSettings.Settings.Add(Key, value);
            SaveConfigToFile();
        }

        public bool shouldUpdate = false;

        public void Tick()
        {
            while (true)
            {
                try
                {
                    Invalidate();
                    Thread.Sleep(20);
                }
                catch (Exception)
                {
                }
            }
        }

        public void Tick2()
        {
            while (true)
            {
                try
                {
                    if (currentConnection.server.serverData != null && currentConnection.server.serverData.name != null)
                    {
                        SetLabelText(ServerName, "Name: " + currentConnection.server.serverData.name);
                        SetLabelText(Host, "Host: " + currentConnection.server.serverData.hostPlayer);
                        SetLabelText(SprintEnabled, "Sprint Enabled: " + currentConnection.server.serverData.sprintEnabled);
                        SetLabelText(Assassinations, "Assassinations: " + currentConnection.server.serverData.assassinationEnabled);
                        SetLabelText(voip, "VoIP: " + currentConnection.server.serverData.voip);
                        SetLabelText(map, "Map: " + currentConnection.server.serverData.map);
                        SetLabelText(variant, "Variant: " + currentConnection.server.serverData.variant);
                        SetLabelText(varianttype, "Variant Type: " + currentConnection.server.serverData.variantType);
                        SetLabelText(status, "Status: " + currentConnection.server.serverData.status);
                        SetLabelText(players, "Players: " + currentConnection.server.serverData.numPlayers + "/" + currentConnection.server.serverData.maxPlayers);
                        SetLabelText(version, "Version: " + currentConnection.server.serverData.eldewritoVersion);
                        toolStripStatusLabel3.Text = "Stats: True";
                    }
                    else
                    {
                        SetLabelText(ServerName, "Name: ");
                        SetLabelText(Host, "Host: ");
                        SetLabelText(SprintEnabled, "Sprint Enabled: ");
                        SetLabelText(Assassinations, "Assassinations: ");
                        SetLabelText(voip, "VoIP: ");
                        SetLabelText(map, "Map: ");
                        SetLabelText(variant, "Variant: ");
                        SetLabelText(varianttype, "Variant Type: ");
                        SetLabelText(status, "Status: ");
                        SetLabelText(players, "Players: 0/0");
                        SetLabelText(version, "Version: ");
                        toolStripStatusLabel3.Text = "Stats: false";
                    }

                    if (currentConnection.server.serverData != null && titleOption != null && titleOption != "")
                    {
                        if (titleOption.Equals("Server IP"))
                            SetTitle("Dedicated Rcon Tool - " + currentConnection.serverinfo.Ip);
                        else if (titleOption.Equals("Server Name") && currentConnection.server.serverData != null && currentConnection.server.serverData.name != null)
                            SetTitle("Dedicated Rcon Tool - " + currentConnection.server.serverData.name);
                        else if (titleOption.Equals("Server Game") && currentConnection.server.serverData != null && currentConnection.server.serverData.variant != null)
                        {
                            if (currentConnection.server.serverData.status.Equals("InLobby"))
                            {
                                SetTitle("Dedicated Rcon Tool - In Lobby");
                            }
                            else
                                SetTitle("Dedicated Rcon Tool - " + currentConnection.server.serverData.variant + " on " + currentConnection.server.serverData.map);
                        }
                        else
                        if (titleOption.Equals("None"))
                        {
                            SetTitle("Dedicated Rcon Tool");
                        }

                    }

                    toolStripStatusLabel2.Text = "Rcon: " + currentConnection.IsConnected();
                    toolStripStatusLabel4.Text = "Version: " + toolversion;

                    if (currentConnection.server.serverData != null)
                    {


                        SetButtonLabel(button4, (currentConnection.server.serverData.sprintEnabled == "0" ? "Enable Sprint" : "Disable Sprint"));
                        SetButtonLabel(button5, (currentConnection.server.serverData.sprintUnlimitedEnabled == "0" ? "Enable Unlimited Sprint" : "Disable Unlimited Sprint"));
                        SetButtonLabel(button6, (currentConnection.server.serverData.assassinationEnabled == "0" ? "Enable Assassinations" : "Disable Assassinations"));
                    }

                    Thread.Sleep(100);
                }
                catch (Exception)
                {
                }
            }
        }

        private void DrawTab(System.Drawing.Graphics g, System.Drawing.Rectangle rectangle, string name)
        {
            try
            {
                g.DrawRectangle(System.Drawing.Pens.Black, rectangle);
                System.Drawing.Font drawFont = new Font("SansSerif", 12);
                StringFormat sf = new StringFormat();
                sf.LineAlignment = StringAlignment.Center;
                sf.Alignment = StringAlignment.Center;
                try
                {
                    g.DrawString(name, this.Font, Brushes.Black, rectangle, sf);
                }
                catch (ArgumentException)
                {
                }
            }
            catch (InvalidOperationException)
            {
            }
        }

        private void DrawString(System.Drawing.Graphics g, System.Drawing.Rectangle rectangle, string name, Color color, int size)
        {
            System.Drawing.Font drawFont = new Font("SansSerif", size, FontStyle.Bold);
            StringFormat sf = new StringFormat();
            sf.LineAlignment = StringAlignment.Center;
            sf.Alignment = StringAlignment.Center;
            g.DrawString(name, drawFont, new SolidBrush(color), rectangle, sf);

        }

        private void DrawInfo(System.Drawing.Graphics g, System.Drawing.Rectangle rectangle, string name, Color color, Color text)
        {
            System.Drawing.SolidBrush brush1 = new System.Drawing.SolidBrush(color);
            g.FillRectangle(brush1, rectangle);
            System.Drawing.Font drawFont = new Font("SansSerif", 10);
            StringFormat sf = new StringFormat();
            sf.LineAlignment = StringAlignment.Center;
            sf.Alignment = StringAlignment.Center;
            g.DrawString(name, drawFont, new SolidBrush(text), rectangle, sf);

        }

        public static double Round(double v, int place)
        {
            double factor = Math.Pow(10, place);
            v = v * factor;
            double tmp = Math.Round(v);
            return (double)tmp / (double)factor;
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            if (sendChat)
            {
                currentConnection.sendToChat(textBox1.Text);
                textBox1.Clear();
            }
            else
            {
                currentConnection.PrintToConsole(textBox1.Text);
                if (currentConnection.serverConnection != null)
                {
                    currentConnection.SendToRcon(textBox1.Text);

                }
                textBox1.Clear();

            }
        }

        delegate void SetTrackbarCallBack();
        private void SetTrackBar()
        {
            if (this.trackBar1.InvokeRequired)
            {
                SetTrackbarCallBack d = new SetTrackbarCallBack(SetTrackBar);
                this.Invoke(d, new object[] { });
            }
            else
            {
                this.trackBar1.Maximum = connectionList.Count - 1;
                SetLabelText(label1, "#" + this.trackBar1.Value);
            }
        }


        delegate void AppendTextCallback(TextBox tx, string text);
        private void AppendText(TextBox tx, string text)
        {
            if (tx.InvokeRequired)
            {
                AppendTextCallback d = new AppendTextCallback(AppendText);
                this.Invoke(d, new object[] { tx, text });
            }
            else
            {
                if (autoScroll)
                {
                    tx.AppendText(text);
                }
                else
                {

                    int caretPos = tx.Text.Length;
                    tx.Text += text;
                    tx.Select(caretPos, 0);
                    tx.ScrollToCaret();
                }
            }
        }

        delegate void SetTextBoxTextCallback(TextBox textbox, string text);
        private void SetTextBoxText(TextBox textbox, string text)
        {
            if (textbox.InvokeRequired)
            {
                SetTextBoxTextCallback de = new SetTextBoxTextCallback(SetTextBoxText);
                this.Invoke(de, new object[] { textbox, text });
            }
            else
            {
                textbox.Text = text;
            }
        }

        delegate void SetLabelTextCallback(Label label, string text);
        private void SetLabelText(Label label, string text)
        {
            if (label.InvokeRequired)
            {
                SetLabelTextCallback de = new SetLabelTextCallback(SetLabelText);
                this.Invoke(de, new object[] { label, text });
            }
            else
            {
                label.Text = text;
            }
        }

        delegate void SetButtonCallback(Button label, string text);
        public void SetButtonLabel(Button button, string text)
        {
            if (button.InvokeRequired)
            {
                SetButtonCallback d = new SetButtonCallback(SetButtonLabel);
                this.Invoke(d, new object[] { button, text });
            }
            else
            {
                button.Text = text;
            }
        }

        delegate void SetTitleCallback(string text);
        public void SetTitle(string text)
        {
            if (InvokeRequired)
            {
                SetTitleCallback d = new SetTitleCallback(SetTitle);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                Text = text;
            }
        }

        private void TextBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                Button1_Click(this, new EventArgs());
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        Color red = Color.FromArgb(155, 51, 50);
        Color blue = Color.FromArgb(50, 89, 146);
        Color gray = Color.FromArgb(98, 98, 98);
        Color darkgray = Color.FromArgb(50, 50, 50);
        Color gold = Color.FromArgb(204, 174, 44);

        private Point mspt = new Point();
        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Graphics graphics = e.Graphics;
            if(currentConnection == null)
            {
                return;
            }
            List<Team> teams = currentConnection.server.prevteamlist;

            System.Drawing.Rectangle name = new System.Drawing.Rectangle(0, 25, 180, 20);
            DrawTab(graphics, name, "NAME");

            System.Drawing.Rectangle score = new System.Drawing.Rectangle(180, 25, 50, 20);
            DrawTab(graphics, score, "SCORE");

            System.Drawing.Rectangle kills = new System.Drawing.Rectangle(230, 25, 50, 20);
            DrawTab(graphics, kills, "KILLS");

            System.Drawing.Rectangle deaths = new System.Drawing.Rectangle(280, 25, 50, 20);
            DrawTab(graphics, deaths, "DEATHS");

            System.Drawing.Rectangle Kd = new System.Drawing.Rectangle(330, 25, 50, 20);
            DrawTab(graphics, Kd, "K/D");

            System.Drawing.Rectangle assists = new System.Drawing.Rectangle(380, 25, 60, 20);
            DrawTab(graphics, assists, "ASSISTS");

            System.Drawing.Rectangle betrayal = new System.Drawing.Rectangle(440, 25, 80, 20);
            DrawTab(graphics, betrayal, "BETRAYALS");

            System.Drawing.Rectangle suicide = new System.Drawing.Rectangle(520, 25, 60, 20);
            DrawTab(graphics, suicide, "SUICIDES");

            System.Drawing.Rectangle timealive = new System.Drawing.Rectangle(580, 25, 90, 20);
            DrawTab(graphics, timealive, "TIME ALIVE");

            System.Drawing.Rectangle beststreak = new System.Drawing.Rectangle(670, 25, 90, 20);
            DrawTab(graphics, beststreak, "BEST STREAK");

            if (currentConnection.server.serverData != null)
            {
                System.Drawing.Rectangle team1 = new System.Drawing.Rectangle(-5, 27, 50, 20);
                DrawString(graphics, team1, "" + currentConnection.server.serverData.redScore, red, 10);

                System.Drawing.Rectangle teamdivider = new System.Drawing.Rectangle(27, 27, 20, 20);
                DrawString(graphics, teamdivider, "-", Color.Black, 10);

                System.Drawing.Rectangle team2 = new System.Drawing.Rectangle(30, 27, 50, 20);
                DrawString(graphics, team2, "" + currentConnection.server.serverData.blueScore, blue, 10);
            }

            int x = 0;
            for (int t = 0; t < teams.Count; t++)
            {
                Team team = teams[t];
                if (team != null && team.GetPlayers() != null && team.GetPlayers().Count > 0)
                {
                    for (int p = 0; p < team.GetPlayers().Count; p++)
                    {
                        PlayerInfo ps = team.GetPlayers()[p];

                        Color color = gray;
                        Color textcolor = Color.White;
                        System.Drawing.Rectangle selection = new System.Drawing.Rectangle(0, 45 + (21 * x), 900, 20);

                        
                        if (currentConnection.server.serverData.teamGame == true || currentConnection.server.serverData.teams == true)
                        {
                            color = team.GetColor();
                        }

                        if (currentConnection.server.serverData.status.ToLower().Contains("inlobby"))
                        {
                            color = darkgray;
                        }

                        if (selection.Contains(mspt))
                        {
                            textcolor = gold;
                            newContextPlayer = ps;
                        }

                        String playername = (ps.Name == "") ? "Loading..." : ps.Name;
                        System.Drawing.Rectangle pname = new System.Drawing.Rectangle(0, 45 + (21 * x), 179, 20);

                        DrawInfo(graphics, pname, playername, color, textcolor);

                        if (ps.IsAlive == false && currentConnection.server.serverData.status.ToLower() != "inlobby" && currentConnection.server.serverData.status != "Loading")
                        {
                            System.Drawing.Rectangle deadX = new System.Drawing.Rectangle(10, 47 + (21 * x), 13, 20);
                            DrawString(graphics, deadX, "X", Color.Black, 16);
                        }

                        System.Drawing.Rectangle pscore = new System.Drawing.Rectangle(180, 45 + (21 * x), 49, 20);
                        DrawInfo(graphics, pscore, "" + ps.Score, color, textcolor);

                        System.Drawing.Rectangle pkills = new System.Drawing.Rectangle(230, 45 + (21 * x), 49, 20);
                        DrawInfo(graphics, pkills, "" + ps.Kills, color, textcolor);

                        System.Drawing.Rectangle pdeath = new System.Drawing.Rectangle(280, 45 + (21 * x), 49, 20);
                        DrawInfo(graphics, pdeath, "" + ps.Deaths, color, textcolor);

                        double kd = 0;
                        if (ps.Deaths > 0)
                        {
                            kd = Round((double)ps.Kills / (double)ps.Deaths, 2);
                        }
                        else
                        if (ps.Deaths == 0)
                        {
                            kd = ps.Kills;
                        }

                        System.Drawing.Rectangle pkd = new System.Drawing.Rectangle(330, 45 + (21 * x), 49, 20);
                        if (kd >= 6)
                        {
                            DrawInfo(graphics, pkd, "" + kd, gold, textcolor);
                        }
                        else
                        {
                            DrawInfo(graphics, pkd, "" + kd, color, textcolor);
                        }
                        System.Drawing.Rectangle passist = new System.Drawing.Rectangle(380, 45 + (21 * x), 59, 20);
                        DrawInfo(graphics, passist, "" + ps.Assists, color, textcolor);

                        System.Drawing.Rectangle pbetray = new System.Drawing.Rectangle(440, 45 + (21 * x), 79, 20);
                        if (ps.Betrayals >= 1)
                        {
                            DrawInfo(graphics, pbetray, "" + ps.Betrayals, gold, textcolor);
                        }
                        else
                        if (ps.Betrayals >= 2)
                        {
                            DrawInfo(graphics, pbetray, "" + ps.Betrayals, color, textcolor);
                        }
                        else
                        {
                            DrawInfo(graphics, pbetray, "" + ps.Betrayals, color, textcolor);
                        }

                        System.Drawing.Rectangle psuicide = new System.Drawing.Rectangle(520, 45 + (21 * x), 59, 20);
                        DrawInfo(graphics, psuicide, "" + ps.Suicides, color, textcolor);

                        System.Drawing.Rectangle ptimealive = new System.Drawing.Rectangle(580, 45 + (21 * x), 89, 20);
                        DrawInfo(graphics, ptimealive, "" + ps.TimeSpentAlive, color, textcolor);

                        System.Drawing.Rectangle pbeststreak = new System.Drawing.Rectangle(670, 45 + (21 * x), 89, 20);
                        DrawInfo(graphics, pbeststreak, "" + ps.BestStreak, color, textcolor);

                        x += 1;
                    }
                }
            }
        }

        private void SettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new ServerInterface().ShowDialog();
        }

        public void CloseProgram()
        {
            if (tick2 != null && tick2.IsAlive)
                tick2.Abort();
            if (RconThread != null && RconThread.IsAlive)
                RconThread.Abort();

            Process.GetCurrentProcess().Kill();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            CloseProgram();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            CloseProgram();
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            mspt = e.Location;
        }

        private void TabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (tabControl1.SelectedTab == tabControl1.TabPages["tabPage2"])
            {
                button1.Text = "Say";
                sendChat = true;
                button1.Visible = true;
                textBox1.Visible = true;
                button10.Visible = true;
                this.tabControl1.Size = new System.Drawing.Size(751, 312);
            }
            else
            if (tabControl1.SelectedTab == tabControl1.TabPages["tabPage3"])
            {
                button1.Visible = false;
                textBox1.Visible = false;
                button10.Visible = false;
                this.tabControl1.Size = new System.Drawing.Size(751, 340);
            }
            else
            if (tabControl1.SelectedTab == tabControl1.TabPages["tabPage4"])
            {
                button1.Visible = false;
                textBox1.Visible = false;
                button10.Visible = false;
                this.tabControl1.Size = new System.Drawing.Size(751, 340);
            }
            else
            {
                textBox1.Visible = true;
                button1.Visible = true;
                button10.Visible = true;
                button1.Text = "Send";
                sendChat = false;
                this.tabControl1.Size = new System.Drawing.Size(751, 312);

            }
        }

        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CloseProgram();
        }

        private void AboutToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            new About().ShowDialog();
        }

        private void Button4_Click(object sender, EventArgs e)
        {
            string ln = (currentConnection.server.serverData.sprintEnabled == "1" ? "0" : "1");
            currentConnection.SendToRcon("Server.SprintEnabled " + ln);
            currentConnection.PrintToConsole("Server.SprintEnabled " + ln);
        }

        private void Button5_Click(object sender, EventArgs e)
        {
            string ln = (currentConnection.server.serverData.sprintUnlimitedEnabled == "1" ? "0" : "1");
            currentConnection.SendToRcon("Server.UnlimitedSprint " + ln);
            currentConnection.PrintToConsole("Server.UnlimitedSprint " + ln);
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            var confirmResult = MessageBox.Show("Are you sure you want to Start the game?", "Warning", MessageBoxButtons.YesNo);
            if (confirmResult == DialogResult.Yes)
            {
                currentConnection.SendToRcon("Game.Start");
                currentConnection.PrintToConsole("Game.Start");
            }
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            var confirmResult = MessageBox.Show("Are you sure you want to Stop the game?", "Warning", MessageBoxButtons.YesNo);
            if (confirmResult == DialogResult.Yes)
            {
                currentConnection.SendToRcon("Game.stop");
                currentConnection.PrintToConsole("Game.stop");
            }
        }

        private void Button6_Click(object sender, EventArgs e)
        {
            string ln = (currentConnection.server.serverData.assassinationEnabled == "1" ? "0" : "1");
            currentConnection.SendToRcon("Server.AssassinationEnabled " + ln);
            currentConnection.PrintToConsole("Server.AssassinationEnabled " + ln);
        }

        private void Button7_Click(object sender, EventArgs e)
        {
            var confirmResult = MessageBox.Show("Are you sure you want to set the max amount of players?", "Warning", MessageBoxButtons.YesNo);
            if (confirmResult == DialogResult.Yes)
            {
                currentConnection.SendToRcon("Server.MaxPlayers " + textBox4.Text);
                currentConnection.PrintToConsole("Server.MaxPlayers " + textBox4.Text);
            }
        }

        private void Button8_Click(object sender, EventArgs e)
        {
            string ln = "1";
            currentConnection.SendToRcon("Server.TeamShuffleEnabled " + ln);
            currentConnection.PrintToConsole("Server.TeamShuffleEnabled " + ln);
        }

        private void Button9_Click(object sender, EventArgs e)
        {
            string ln = "0";
            currentConnection.SendToRcon("Server.TeamShuffleEnabled " + ln);
            currentConnection.PrintToConsole("Server.TeamShuffleEnabled " + ln);
        }

        delegate void ClearTextBox(TextBox txtbx, string text);
        public void ClearTextBoxs(TextBox txtbx, string text)
        {
            if (txtbx.InvokeRequired)
            {
                ClearTextBox d = new ClearTextBox(ClearTextBoxs);
                this.Invoke(d, new object[] { txtbx, text });
            }
            else
            {
                txtbx.Text = text;
            }
        }

        private void Button10_Click(object sender, EventArgs e)
        {
            if (tabControl1.SelectedTab == tabControl1.TabPages["tabPage1"])
            {
                currentConnection.ClearConsole();
                ClearTextBoxs(textBox2, "");

            }
            else
            if (tabControl1.SelectedTab == tabControl1.TabPages["tabPage2"])
            {
                currentConnection.ClearChat();
                ClearTextBoxs(textBox3, "");
            }

        }

        private void TrackBar1_Scroll(object sender, EventArgs e)
        {
            int sel = trackBar1.Value;
            if (sel <= connectionList.Count)
            {
                if (currentConnection != null)
                {
                    currentConnection.SetVisable(false);
                }
                currentConnection = connectionList[sel];
                currentConnection.SetVisable(true);
            }
            SetTextBoxText(textBox2, currentConnection.GetConsole());
            SetTextBoxText(textBox3, currentConnection.GetChat());
        }

        private void TextBox2_TextChanged(object sender, EventArgs e)
        {
            if (autoScroll)
            {
                textBox2.SelectionStart = textBox2.TextLength;
                textBox2.ScrollToCaret();
            }
        }

        private void TextBox3_TextChanged(object sender, EventArgs e)
        {
            if (autoScroll)
            {
                textBox3.SelectionStart = textBox3.TextLength;
                textBox3.ScrollToCaret();
            }
        }

        private void CheckBox1_CheckedChanged(object sender, EventArgs e)
        {
            autoScroll = checkBox1.Checked;
            autoUpdateEnabled = checkBox1.Checked;

            if (autoUpdateEnabled)
            {
                SetTextBoxText(textBox2, currentConnection.GetConsole());
                SetTextBoxText(textBox3, currentConnection.GetChat());
            }
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {

            scoreBoardContextMenu.Close();
            if (e.Button == MouseButtons.Right)
            {
                mspt = e.Location;
                int pos = 0;
                for (int p = 0; p < 25; p++)
                {
                    System.Drawing.Rectangle selection = new System.Drawing.Rectangle(0, 45 + (21 * pos), 900, 21);

                    if (selection.Contains(mspt))
                    {
                        contextPlayerPos = p;
                    }
                    pos += 1;
                }

                if (currentConnection.server.serverData.Players.Count >= contextPlayerPos)
                {
                    int x = 0;
                    for (int t = 0; t < currentConnection.server.prevteamlist.Count; t++)
                    {
                        Team team = currentConnection.server.prevteamlist[t];
                        if (team != null && team.GetPlayers() != null && team.GetPlayers().Count > 0)
                        {
                            for (int p = 0; p < team.GetPlayers().Count; p++)
                            {
                                PlayerInfo ps = team.GetPlayers()[p];
                                System.Drawing.Rectangle selection = new System.Drawing.Rectangle(0, 45 + (21 * x), 900, 20);

                                if (selection.Contains(mspt))
                                {
                                    newContextPlayer = ps;
                                }
                                x += 1;
                            }
                        }
                    }

                    contextplayer = newContextPlayer;

                    if (contextplayer != null && !contextplayer.Name.Equals(""))
                    {
                        scoreBoardContextMenu.Items[0].Text = "&Kick " + contextplayer.Name;
                        scoreBoardContextMenu.Items[1].Text = "&Ban " + contextplayer.Name;

                        this.scoreBoardContextMenu.Show(this, new Point(e.X, e.Y));
                    }
                }
            }
        }
    }
}
