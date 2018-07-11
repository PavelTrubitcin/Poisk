using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TestComputera
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            СompeteraAPI сompetera = new СompeteraAPI("https://dashboard.competera.net/api/v2/", "trubicinp@tdpoisk.ru", "88fe8e560d610ad0cf3d412f695b51c502bb2a5e");
            сompetera.test();
        }
    }
}
