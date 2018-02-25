using System;
using System.Collections.Generic;
using System.Net;
using System.Windows.Forms;
using static RconTool.Form1;
using static RconTool.ServerInterface;

namespace RconTool
{
    public partial class ServerEditor : Form
    {
        bool isEditing = false;
        private Connection connection;
        private ListBox listbox;

        public ServerEditor(ListBox lsbx)
        {
            this.listbox = lsbx;
            InitializeComponent();
            string commandlist = "";
            commandlist = "Server.RconMessageSpoolSize 30" + System.Environment.NewLine + "Server.SendChatToRconClients 1" + System.Environment.NewLine + "Server.SendChatSpool" + System.Environment.NewLine;
            textBox5.Text = commandlist;
        }

        public ServerEditor(Connection connections, ListBox lsbox)
        {
            listbox = lsbox;
            connection = connections;
            ServerInfo info = connections.serverinfo;
            isEditing = true;
            InitializeComponent();
            textBox1.Text = info.Ip;
            textBox2.Text = info.InfoPort;
            textBox3.Text = info.RconPassword;
            textBox4.Text = info.RconPort;
            string commandlist = "";
            if (!(info.sendOnConnect == null || info.sendOnConnect.Count == 0))
            {
                foreach (string sc in info.sendOnConnect)
                {
                    commandlist = commandlist + sc + System.Environment.NewLine;
                }
            }
            textBox5.Text = commandlist;
        }

        private bool isValidIPAddress(string input)
        {
            IPAddress address;
            if (IPAddress.TryParse(input, out address))
            {
                switch (address.AddressFamily)
                {
                    case System.Net.Sockets.AddressFamily.InterNetwork:
                        return true;

                    case System.Net.Sockets.AddressFamily.InterNetworkV6:
                        return false;

                    default:
                        return false;

                }
            }
            return false;
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            var isPortValid = int.TryParse(textBox2.Text, out int n);

            if (textBox1.Text.Equals(""))
            {
                MessageBox.Show("Server IP cannot be blank!", "WARNING", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            if (textBox2.Text.Equals(""))
            {
                MessageBox.Show("InfoServer Port cannot be blank!", "WARNING", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            if (isValidIPAddress(textBox1.Text) == false)
            {
                MessageBox.Show("Server IP must be valid!", "WARNING", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            if (isPortValid == false)
            {
                MessageBox.Show("InfoServer Port must be valid!", "WARNING", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                //save
                if (isEditing)
                {
                    Console.WriteLine("removed connection");
                    connection.CloseConnection();
                    Form1.connectionList.Remove(connection);
                    listbox.Items.Remove(listbox.SelectedItem);
                }

                List<string> list = new List<string>(textBox5.Text.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries));

                ServerInfo newserver = new ServerInfo(textBox1.Text, textBox2.Text, textBox3.Text, textBox4.Text, list);

                Connection cm = new Connection(Form1.form, newserver);

                listbox.Items.Add(new NewItem
                {
                    Ip = newserver.Ip + ":" + newserver.InfoPort,
                    Connection = cm
                });

                SaveSettings();

                //new ServerInterface().ShowDialog();
                this.Close();
            }
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
