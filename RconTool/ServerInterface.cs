using System;
using System.Windows.Forms;
using static RconTool.Form1;

namespace RconTool
{
    public partial class ServerInterface : Form
    {
        public class NewItem
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

            textBox1.Text = Form1.webhook;
            textBox2.Text = Form1.webhookTrigger;
            textBox3.Text = Form1.webhookRole;

            AddItems();

            listBox1.SelectedIndex = listBox1.Items.Count - 1;
        }

        public void AddItems()
        {
            listBox1.Items.Clear();

            for (int x = 0; x < Form1.connectionList.Count; x++)
            {
                listBox1.Items.Add(new NewItem
                {
                    Ip = Form1.connectionList[x].serverinfo.Ip + ":" + Form1.connectionList[x].serverinfo.InfoPort,
                    Connection = Form1.connectionList[x]
                });
            }
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem != null && ((NewItem)listBox1.SelectedItem).Connection != null)
            {                
                new ServerEditor(((NewItem)listBox1.SelectedItem).Connection,listBox1).ShowDialog();
                //this.Close();
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
            new ServerEditor(listBox1).ShowDialog();
        }

        private void Button4_Click(object sender, EventArgs e)
        {
            if (Form1.connectionList.Count == 0)
            {
                MessageBox.Show("Must have atleast 1 server!", "WARNING", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                Form1.webhook = textBox1.Text;
                Form1.webhookTrigger = textBox2.Text;
                Form1.webhookRole = textBox3.Text;
                Form1.titleOption = comboBox1.SelectedItem.ToString();
                Form1.SaveSettings();
                this.Close();
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            //webhook url
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            //webhook trigger
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            //role id text box
        }
    }
}
