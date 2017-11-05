using System;
using System.Windows.Forms;

namespace RconTool
{
    public partial class SettingsForm : Form
    {
        Form1 main;
        public SettingsForm(Form1 form)
        {
            main = form;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;

            
            InitializeComponent();
            comboBox1.SelectedIndex = comboBox1.Items.IndexOf(Form1.titleOption == "" ? "None" : Form1.titleOption);

        }

        private void Button1_Click(object sender, EventArgs e)
        {
            Form1.titleOption = comboBox1.SelectedItem.ToString();
            Form1.SaveSettings();
            this.Close();
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            this.Close();
            new ServerInterface().ShowDialog();
        }
    }
}
