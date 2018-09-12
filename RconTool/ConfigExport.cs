using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RconTool
{
    public partial class ConfigExport : Form
    {
        public ConfigExport()
        {
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;

            InitializeComponent();

            ServerArray sA = new ServerArray();
            for (int x = 0; x < Form1.connectionList.Count; x++)
            {
                Connection c = Form1.connectionList[x];
                sA.servers.Add(c.serverinfo);
            }
            textBox1.Text = sA.ToBase64();
        }


        class ServerArray
        {
            public List<ServerInfo> servers = new List<ServerInfo>();

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
}
