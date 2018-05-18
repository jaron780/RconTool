using System;
using System.Windows.Forms;
using static RconTool.Form1;

namespace RconTool
{
    public partial class TimedCommand : Form
    {
        public class NewItem
        {
            public string Command { get; set; }
            public TimedCommandItem Cmd { get; set; }
        }

        public TimedCommand()
        {
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            InitializeComponent();

            listBox1.DisplayMember = "Command";
            listBox1.ValueMember = "cmd";

            for (int x = 0; x < timedCommandArray.GetCommandList().Count; x++)
            {
                listBox1.Items.Add(new NewItem
                {
                    Command = timedCommandArray.GetCommandList()[x].GetName(),
                    Cmd = timedCommandArray.GetCommandList()[x]
                });
            }

            listBox1.SelectedIndex = listBox1.Items.Count - 1;
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem != null && ((NewItem)listBox1.SelectedItem).Cmd != null)
            {
                var confirmResult = MessageBox.Show("Are you sure you want to delete the command?", "Warning", MessageBoxButtons.YesNo);
                if (confirmResult == DialogResult.Yes)
                {
                    timedCommandArray.RemoveCommandItem(((NewItem)listBox1.SelectedItem).Cmd);
                    listBox1.Items.Remove(listBox1.SelectedItem);
                }
            }
            SaveSettings();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem != null && ((NewItem)listBox1.SelectedItem).Cmd != null)
            {
                new TimedCommandForm(((NewItem)listBox1.SelectedItem).Cmd,listBox1).ShowDialog();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            new TimedCommandForm(listBox1).ShowDialog();
        }
    }
}
