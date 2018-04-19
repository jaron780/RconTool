using System;
using System.Windows.Forms;

namespace RconTool
{
    public partial class About : Form
    {
        public About()
        {
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            InitializeComponent();
            label2.Text = "Version: " + Form1.toolversion;
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void About_Load(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }
    }
}
