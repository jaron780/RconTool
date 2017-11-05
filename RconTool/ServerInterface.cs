using System;
using System.Windows.Forms;
using static RconTool.Form1;

namespace RconTool
{
    public partial class ServerInterface : Form
    {
        class NewItem
        {
            public string Ip { get; set; }
            public Connection Connection { get; set; }
        }

        public ServerInterface()
        {
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;

            InitializeComponent();

            comboBox1.SelectedIndex = comboBox1.Items.IndexOf(Form1.titleOption == "" ? "None" : Form1.titleOption);
            listBox1.DisplayMember = "Ip";
            listBox1.ValueMember = "connection";

            AddItems();
        }

        public void AddItems()
        {
            listBox1.Items.Clear();

            for (int x = 0; x < Form1.connectionList.Count; x++)
            {
                listBox1.Items.Add(new NewItem
                {
                    Ip = Form1.connectionList[x].serverinfo.Ip,
                    Connection = Form1.connectionList[x]
                });
            }
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem != null && ((NewItem)listBox1.SelectedItem).Connection != null)
            {                
                new ServerEditor(((NewItem)listBox1.SelectedItem).Connection).ShowDialog();
                this.Close();
            }
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem != null && ((NewItem)listBox1.SelectedItem).Connection != null)
            {
                var confirmResult = MessageBox.Show("Are you sure you want to delete the server?", "Warning", MessageBoxButtons.YesNo);
                if (confirmResult == DialogResult.Yes)
                {
                    RemoveServers();
                    ((NewItem)listBox1.SelectedItem).Connection.CloseConnection();
                    Form1.connectionList.Remove(((NewItem)listBox1.SelectedItem).Connection);
                    listBox1.Items.Remove(listBox1.SelectedItem);
                }
            }

            SaveSettings();
        }

        private void RemoveServers()
        {
            for (int x = 0; x < 100; x++)
            {
                RemoveSetting("Server." + x );
            }

            SaveConfigToFile();
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            new ServerEditor().ShowDialog();
            this.Close();
        }

        private void Button4_Click(object sender, EventArgs e)
        {
            Form1.titleOption = comboBox1.SelectedItem.ToString();
            Form1.SaveSettings();
            this.Close();
        }
    }
}
