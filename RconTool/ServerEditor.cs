using System;
using System.Windows.Forms;
using static RconTool.Form1;

namespace RconTool
{
    public partial class ServerEditor : Form
    {
        bool isEditing = false;
        private Connection connection;
        public ServerEditor()
        {
            InitializeComponent();
        }

        public ServerEditor(Connection connections)
        {
            connection = connections;
            ServerInfo info = connections.serverinfo;
            isEditing = true;
            InitializeComponent();
            textBox1.Text = info.Ip;
            textBox2.Text = info.InfoPort;
            textBox3.Text = info.RconPassword;
            textBox4.Text = info.RconPort;
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            if (isEditing)
            {
                connection.CloseConnection();
                Form1.connectionList.Remove(connection);
            }

            ServerInfo newserver = new ServerInfo(textBox1.Text, textBox2.Text, textBox3.Text, textBox4.Text);
            new Connection(Form1.form, newserver);
            SaveSettings();
            this.Close();
        }
    }
}
