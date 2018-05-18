using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using static RconTool.Form1;
using static RconTool.TimedCommand;

namespace RconTool
{
    public partial class TimedCommandForm : Form
    {

        bool isEditing = false;
        TimedCommandItem editing;
        ListBox lstbox;

        public TimedCommandForm(ListBox lsbx)
        {
            this.lstbox = lsbx;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;

            InitializeComponent();
        }

        public TimedCommandForm(TimedCommandItem cmd, ListBox lsbx)
        {
            this.lstbox = lsbx;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;

            isEditing = true;
            InitializeComponent();

            button1.Text = "Apply Edit";

            textBox1.Text = cmd.GetName();
            numericUpDown2.Value = cmd.GetTime();
            checkBox1.Checked = cmd.RunEveryXMin();
            numericUpDown1.Value = cmd.GetEveryXMin();

            for (int x = 0; x < cmd.GetCommands().Count(); x++)
            {
                textBox2.AppendText(cmd.GetCommands()[x] + System.Environment.NewLine);
            }
            editing = cmd;
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (isEditing)
            {
                timedCommandArray.RemoveCommandItem(editing);
                lstbox.Items.Remove(lstbox.SelectedItem);

            }

            List<string> list = new List<string>(
                           textBox2.Text.Split(new string[] { "\r\n" },
                           StringSplitOptions.RemoveEmptyEntries));
            TimedCommandItem cm = new TimedCommandItem(textBox1.Text,checkBox1.Checked,Decimal.ToInt32(numericUpDown2.Value),Decimal.ToInt32(numericUpDown1.Value),checkBox2.Checked,list);

            timedCommandArray.AddTimedCommand(cm);

            lstbox.Items.Add(new NewItem
            {
                Command = cm.GetName(),
                Cmd = cm
            });

            SaveSettings();

            this.Close();
        }

        private void label2_Click_1(object sender, EventArgs e)
        {

        }

        private void checkBox1_CheckedChanged_1(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                numericUpDown1.Enabled = true;
                numericUpDown2.Enabled = false;
            }
            else
            {
                numericUpDown1.Enabled = false;
                numericUpDown2.Enabled = true;
            }
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {

        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {

        }

        private void TimedCommandForm_Load(object sender, EventArgs e)
        {

        }

        private void label9_Click(object sender, EventArgs e)
        {

        }
    }
}
