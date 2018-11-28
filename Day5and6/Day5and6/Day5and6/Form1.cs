using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                Regex re = new Regex(@"([a-zA-Z]|\d|\.)+@[a-zA-Z]+\.[a-z]+");
                if (re.IsMatch(textBox1.Text))
                {
                    label1.Text = "It's a valid email!";
                }
                else
                {
                    label1.Text = "This not a valid email";
                }
            }
            catch (Exception exception) {
                Console.WriteLine(exception.Message);

            }
        }
    }
}
